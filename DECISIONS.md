# Decisions

Judgment calls made while implementing `docs/graph-types-spec.md`. One entry per decision: the decision, then the reason.

## Answers to pre-run questions (from the repository owner)

1. **Library retargeted to `netstandard2.0` in milestone 1, together with the tests.** .NET Framework 4.8 cannot reference a `netstandard2.1` library, so the `net48` test target cannot build otherwise. Milestone 2 covers the rest of the build setup.
2. **Context7 is used for documentation lookups as the spec describes.** If it becomes unavailable, fall back to the official documentation sites (learn.microsoft.com, nunit.org, awesomeassertions.org, project GitHub READMEs), confirm versions with `dotnet package search`, and log the switch here.
3. **`UndirectedGraph<TNode>.Edges` reports `Source` as the endpoint whose node was added to the graph earlier**, judged by insertion sequence rather than slot index, because slots get reused.
4. **Benchmarks live in `src/benchmarks/Graphical.Benchmarks/`.** Benchmarks aren't tests, and this follows the existing `src/code` / `src/tests` convention.
5. **`AddEdges` and `AddNodes` copy the batch into a list and validate every item before changing anything**, so no graph type is left half-updated by an argument error.

## Implementation decisions
- **Test packages:** NUnit 4.6.1 (latest 4.x; 5.0.0 exists but the spec requires 4.x), NUnit3TestAdapter 6.3.0 (supports net462+ and net8+ per its README), Microsoft.NET.Test.Sdk 18.10.1, NUnit.Analyzers 4.15.0, FluentAssertions 7.2.2 (latest 7.x). `coverlet.collector` is kept (already referenced) and upgraded to 10.1.0.
- **`LangVersion` latest set in the test project during milestone 1**, because the `net48` target otherwise defaults to C# 7.3 and rejects nullable and file-scoped namespaces. It moves to `Directory.Build.props` in milestone 2.
- **`global.json` pins `10.0.401`**, the newest installed 10.0 SDK, with `rollForward: latestFeature` as the spec requires.
- **`ContinuousIntegrationBuild` is enabled when `CI` is `true`** (MSBuild comparisons are case-insensitive), the value GitHub Actions, Azure Pipelines and most CI hosts set.
- **Test fixtures are `sealed`** to satisfy CA1852 (raised by `AnalysisLevel` latest-recommended) rather than suppressing it.
- **Package metadata lives in `Graphical.csproj`**, not `Directory.Build.props`, because it applies only to the packable library.
- **`Authors` is `Eric Burcham` and `Copyright` is `Copyright (c) 2023 Eric Burcham`**, from the copyright line in `LICENSE`.
- **PolySharp 1.16.0 and Microsoft.CodeAnalysis.PublicApiAnalyzers 5.6.0** (latest stable). PolySharp's generated types stay internal (its default), so they never appear in the public API.
- **The pre-existing vertex API got placeholder XML docs and `PublicAPI.Unshipped.txt` entries in milestone 2** so the build passes with `GenerateDocumentationFile` and the public API analyzer; both go away when milestone 6 removes that API.
- **Serena fallback:** Serena activated the project but its C# language server failed to initialise ("The language server manager is not initialized"), so symbol tools are unavailable. Per the spec, ordinary file tools (Grep/Read/Edit, compiler-checked renames) are used instead. Serena's generated `.serena/` folder is excluded locally via `.git/info/exclude` rather than committed.
- **Generic type files are named `Name{TNode}.cs`** (e.g. `Edge{TNode}.cs`) so `Edge<TNode>` and the static `Edge` class can live in separate files; this is the common .NET convention.
- **`Edge<TNode>.Edge() -> void` appears in `PublicAPI.Unshipped.txt`.** Every struct has an implicit parameterless constructor and the analyzer requires it to be listed; it is not an additional member.
- **AwesomeAssertions 9.6.0 replaces FluentAssertions (owner instruction, supersedes the FluentAssertions 7.x pin above and every FluentAssertions instruction in the original spec).** FluentAssertions is no longer free and open source; AwesomeAssertions is its Apache-2.0 fork with the same API. 9.6.0 is the latest stable and ships `net47`/`netstandard2.0`/`net8.0` assets, so it covers both `net48` and `net10.0`. Version 9 renamed the namespaces, so tests use `using AwesomeAssertions;`. The spec's references were updated to match.
- **Live neighbor views follow the node, not the slot.** A view returned by `GetNeighbors` (and later `GetSuccessors`/`GetPredecessors`) records the node's insertion sequence; once the node is removed the view is empty, even if its slot is reused. This avoids a stale view silently reporting another node's neighbors.
- **Contract fixtures take the open generic graph type** (`[TestFixture(typeof(UndirectedGraph<>))]`) and build graphs through `Extensions/GraphFactory`, which picks the matching public constructor by reflection and unwraps `TargetInvocationException`, so every implementation is exercised through each of its constructors.
- **A null item inside `AddNodes`' collection throws `ArgumentException` (param `nodes`)**, matching how `AddEdges` reports an edge with a null endpoint (param `edges`); `ArgumentNullException` is reserved for null arguments themselves.
- **RS0022 is suppressed (with a `#pragma` and justification) on `DirectedGraph<TNode>`'s constructors.** The analyzer reports that public constructors make the non-inheritable `Graph<TNode>` inheritable; the spec requires exactly that shape, and `Graph<TNode>` exposes no protected members (its hooks are `private protected`), so nothing new is reachable.
- **The migrated `WhenAddingAnEdgeBetweenNodesOneAndTwo` fixture asserts on `GetSuccessors`**, because the old vertex `GetNeighbors` returned successors only, while the new `GetNeighbors` returns successors and predecessors.
