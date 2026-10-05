# Graphical graph types: build spec

This spec is your complete brief for this work. Where it conflicts with the existing code in `src`, the spec wins. Where it says nothing, follow the existing conventions described below.

## Context

- Repository root: `D:\src\graphical`. The branch `create-graph-types` already exists (created from `develop`) and is checked out. All work happens on this branch.
- **Existing library.** `src/code/Graphical/DirectedGraph.cs` defines `DirectedGraph<T>`, which stores neighbours in a `Dictionary<T, HashSet<T>>` and exposes `AddVertex`, `AddEdge`, `GetVertices`, and `GetNeighbors`, with no input validation. The project targets `netstandard2.1`, with nullable reference types on and warnings treated as errors.
- **Existing tests.** NUnit 3.13 with FluentAssertions 6.8 on `net6.0`. They don't run on this machine because the .NET 6 runtime isn't installed; the .NET 8, 9, and 10 SDKs are.
- **What changes.** This spec replaces the vertex-based `DirectedGraph<T>` with a new family of graph types. The repo's current code has never shipped, so replace the old API outright, with no compatibility shims, and migrate its tests to the new API.
- **Package history.** The NuGet ID `Graphical` already has versions 1.0.0–1.0.2 (2018, `net47`) from an older, unrelated codebase. This work is version 2.0, a ground-up rewrite with no compatibility obligations to 1.x.
- **Remote:** `https://github.com/ericburcham/graphical` (public, MIT-licensed, default branch `main`). Keep the existing `LICENSE` and `.github/` content as they are.

## How to work

### Autonomy

- Work from start to finish without stopping to ask questions. When something is ambiguous, choose what best fits this spec, then the existing conventions, then mainstream .NET library practice.
- Record each judgment call in `DECISIONS.md` at the repo root, one short entry each: the decision and the reason.
- Track the milestones below as a task list and keep it current. After any context compaction, re-read this spec and `DECISIONS.md` before continuing.
- Never delete, skip, `[Ignore]`, or weaken a test to get to green. Never suppress a warning without a justification comment.
- Git: commit only to `create-graph-types` and don't touch `develop`. No push, merge, rebase, amend, or force operations.

### TDD cycle

Build every library behavior with this cycle: Think (often omitted) → Red → Green → Refactor.

- **Red:** write one failing test for the next small behavior. Run it and confirm it fails for the expected reason. A compile error for a member that doesn't exist yet counts.
- **Green:** write the minimum reasonable code to make it pass, then run the whole suite.
- **Refactor:** refactor both the tests and the implementation toward the patterns in this spec until you're satisfied it's the best solution. The suite stays green throughout.
- **Commits:** commit after each Green, and after each Refactor that changes code. Every commit builds with zero warnings and passes every test. Keep commits small and isolated, and match the message style in `git log`.
- **Non-behavioral work** (build infrastructure, packaging, benchmarks, docs) doesn't need tests first, but it still goes in small, separate commits.
- **Public API tracking:** the public API analyzer fails the build on any public member missing from `PublicAPI.Unshipped.txt`, so add each new member to that file in the same commit as the member.

### Tools

- **Serena:** use its symbol-level tools to explore the existing code, find references, and do cross-file renames and refactors, such as the vertex → node migration. Activate the project in Serena first if it isn't already. Fall back to ordinary file tools for anything Serena can't do.
- **Context7:** check current documentation before adding or upgrading a package, and whenever you rely on an API you aren't sure of. That includes NUnit 4, FluentAssertions 7, PolySharp, Microsoft.CodeAnalysis.PublicApiAnalyzers, BenchmarkDotNet, and MSBuild/SDK properties. Confirm the latest stable versions with `dotnet package search <name> --exact-match`.
- **Installing prerequisites:** if something required is missing, install it with winget.
  - Find the exact ID with `winget search`, then run `winget install -e --id <Id> --accept-source-agreements --accept-package-agreements`.
  - Don't install the .NET 6 runtime; the tests are being retargeted instead.
  - If an install needs elevation or fails, add the exact commands to a **Setup** section in `DECISIONS.md`, continue with everything that doesn't depend on it, and repeat those commands in your final report.

## Milestones

Work through these in order. Each one ends with a clean build and every test passing.

0. **Spec.** If this file isn't committed yet, commit it as `docs/graph-types-spec.md`.
1. **Baseline.**
   - Retarget the test project to `net10.0;net48` and upgrade the test packages (see "Tests: framework and conventions").
   - Get the two existing fixtures passing.
   - Add the missing assertion for `_vertexTwoNeighbors` in `WhenAddingAnEdgeBetweenVerticesOneAndTwo`.
2. **Build infrastructure.** Retarget the library to `netstandard2.0`. Add `Directory.Build.props`, `global.json`, the analyzers, PolySharp, and the public API analyzer with the current API recorded. Update `.editorconfig` as needed.
3. **Value types:** `Edge<TNode>`, `Edge`, and `GraphCycleException`.
4. **Internal data structures:** `NodeTable<TNode>`, `BitSet`, and `MinHeap`.
5. **`Graph<TNode>` and `UndirectedGraph<TNode>`**, plus the interfaces they need.
6. **`DirectedGraph<TNode>`**, replacing the old vertex-based class.
   - Migrate its fixtures to the new API and names, e.g. `WhenAddingVertices` → `WhenAddingNodes` and `WhenAddingAnEdgeBetweenVerticesOneAndTwo` → `WhenAddingAnEdgeBetweenNodesOneAndTwo`.
   - Remove every trace of the old API.
7. **`DirectedAcyclicGraph<TNode>`.**
8. **`ReachabilityDirectedAcyclicGraph<TNode>`**, including the differential randomized fixtures.
9. **Benchmarks project.**
10. **Finish:** the README, an XML documentation pass, the public API files, and the final verification in "Done when".

## Repository layout

- Keep the existing solution, folder layout, and project names. The library stays in `src/code/Graphical/`.
- Put the benchmarks project, `Graphical.Benchmarks`, beside the existing test project, following the same folder convention, and add it to the solution.
- At the repo root, add or extend (don't replace) `Directory.Build.props`, `global.json`, `.editorconfig`, and `DECISIONS.md`.
- Rewrite the current two-line `README.md` as described in "README".

## Target frameworks

- **Library:** `netstandard2.0`. This is a deliberate change from `netstandard2.1`, so that one codebase covers .NET Framework 4.7.2+ and all modern .NET.
- **Tests:** `net10.0;net48`. The .NET Framework 4.8 runtime ships with Windows. If the build can't find .NET Framework reference assemblies, reference `Microsoft.NETFramework.ReferenceAssemblies` with `PrivateAssets="all"`.
- **Benchmarks:** `net10.0`.
- **SDK:** `global.json` pins the installed 10.0 SDK, with `rollForward` set to `latestFeature`.

## Build settings

- **`Directory.Build.props`:** `LangVersion` latest, `Nullable` enable, `TreatWarningsAsErrors` true, `EnableNETAnalyzers` true, `AnalysisLevel` latest-recommended, `Deterministic` true, and `ContinuousIntegrationBuild` true when `CI` is set.
- **Library only:** `GenerateDocumentationFile` true and `ImplicitUsings` disable.
- **Dependencies.** The library has zero runtime package dependencies. Only these build-time packages are allowed, with `PrivateAssets="all"`:
  - PolySharp, for compiler polyfills (nullable attributes, `IsExternalInit`). Add it to any project targeting `netstandard2.0` or `net48` that needs them.
  - Microsoft.CodeAnalysis.PublicApiAnalyzers, with `PublicAPI.Shipped.txt` (empty) and `PublicAPI.Unshipped.txt`.
- **Package metadata:**
  - **Identity and version:** `PackageId` Graphical, `VersionPrefix` 2.0.0, `VersionSuffix` preview.1.
  - **Author and copyright:** `Authors` and `Copyright` taken from the copyright line in `LICENSE`.
  - **Description and tags:** a one-sentence `Description` matching the README tagline, and `PackageTags` `graph;dag;directed-acyclic-graph;topological-sort;transitive-closure;reachability`.
  - **License and URLs:** `PackageLicenseExpression` MIT, `PackageProjectUrl` and `RepositoryUrl` `https://github.com/ericburcham/graphical`, and `RepositoryType` git.
  - **README:** `PackageReadmeFile` README.md, packed from the repo root.
  - **Source Link and symbols:** Source Link enabled (built into the .NET 8+ SDK) with `PublishRepositoryUrl` and `EmbedUntrackedSources` true, and symbols as snupkg.
- **One codebase, no `#if`.** Don't use APIs missing from netstandard2.0 (e.g. `HashCode`, `BitOperations`, `PriorityQueue`, `IReadOnlySet<T>`, `Dictionary.TryAdd`); write small internal helpers instead.
- **Analyzers:** fix warnings rather than suppressing them. Any suppression needs a justification comment.

## Tests: framework and conventions

- **Packages.**
  - Upgrade NUnit to the latest stable 4.x, with matching NUnit3TestAdapter and Microsoft.NET.Test.Sdk. Write all assertions with FluentAssertions, since NUnit 4 moved the classic asserts.
  - Upgrade FluentAssertions to the latest 7.x. Don't move to 8.x, which requires a paid commercial license.
- **Keep the existing conventions:**
  - One fixture per scenario, named `When…`, in a folder per class under test (e.g. `EdgeTests/`, `UndirectedGraphTests/`, `DirectedGraphTests/`).
  - The scenario runs once in `[OneTimeSetUp]`, and its results go into fields. Capture expected exceptions into a field too, so each assertion stays in its own test.
  - Each `[Test]` makes exactly one assertion and is named in the form `…Should…`.
  - Constants use UPPER_SNAKE_CASE (`ONE`, `TWO`). Shared helpers live in `Extensions/`, alongside the existing `AddRange`.
- **Contract fixtures.** Write behavior that several classes share as generic fixtures parameterized over every implementation (`[TestFixture(typeof(...))]` or `[TestFixtureSource]`). Put them in a folder named for the interface (e.g. `IDirectedGraphTests/`), so every implementation is held to the same contract.
- **Randomized differential tests** follow the same one-assertion rule:
  - Each fixture instance takes a fixed seed via `[TestFixtureSource]`.
  - `[OneTimeSetUp]` runs the operation sequence and records every discrepancy into a list.
  - A single test asserts the list is empty, so a failure prints every discrepancy.
- **Public API only.** Tests use the public API. `InternalsVisibleTo` is allowed only for direct tests of `BitSet`, `NodeTable<TNode>`, and `MinHeap`.
- **Analyzer conflicts.** If an analyzer rule conflicts with these conventions (for example CA1707 on underscores), relax it for the test project only, in a scoped `.editorconfig` section with a comment explaining why.

## Public API

Implement exactly this, with no additional public types or members. All types live in the `Graphical` namespace.

```csharp
namespace Graphical;

// ── Value types ──────────────────────────────────────────────────────────────

public readonly struct Edge<TNode> : IEquatable<Edge<TNode>> where TNode : notnull
{
    public Edge(TNode source, TNode target);
    public TNode Source { get; }
    public TNode Target { get; }
    public Edge<TNode> Reverse();
    public void Deconstruct(out TNode source, out TNode target);
    public bool Equals(Edge<TNode> other);
    public override bool Equals(object? obj);
    public override int GetHashCode();
    public override string ToString();   // "(Source -> Target)"
    public static bool operator ==(Edge<TNode> left, Edge<TNode> right);
    public static bool operator !=(Edge<TNode> left, Edge<TNode> right);
}

public static class Edge
{
    public static Edge<TNode> Create<TNode>(TNode source, TNode target) where TNode : notnull;
}

public class GraphCycleException : InvalidOperationException
{
    public GraphCycleException();
    public GraphCycleException(string? message);
    public GraphCycleException(string? message, Exception? innerException);
}

// ── Read-only contracts ──────────────────────────────────────────────────────

public interface IReadOnlyGraph<TNode> where TNode : notnull
{
    bool IsDirected { get; }
    IEqualityComparer<TNode> Comparer { get; }
    int NodeCount { get; }
    int EdgeCount { get; }
    IReadOnlyCollection<TNode> Nodes { get; }
    IReadOnlyCollection<Edge<TNode>> Edges { get; }
    bool ContainsNode(TNode node);
    bool ContainsEdge(TNode source, TNode target);
    IReadOnlyCollection<TNode> GetNeighbors(TNode node);
    int GetDegree(TNode node);
}

public interface IReadOnlyUndirectedGraph<TNode> : IReadOnlyGraph<TNode> where TNode : notnull
{
    bool AreConnected(TNode first, TNode second);   // same connected component
}

public interface IReadOnlyDirectedGraph<TNode> : IReadOnlyGraph<TNode> where TNode : notnull
{
    IReadOnlyCollection<TNode> GetSuccessors(TNode node);
    IReadOnlyCollection<TNode> GetPredecessors(TNode node);
    int GetInDegree(TNode node);
    int GetOutDegree(TNode node);
    IReadOnlyCollection<TNode> GetSources();    // in-degree 0, snapshot
    IReadOnlyCollection<TNode> GetSinks();      // out-degree 0, snapshot
    bool HasPath(TNode source, TNode target);   // path of length >= 1
}

public interface IReadOnlyDirectedAcyclicGraph<TNode> : IReadOnlyDirectedGraph<TNode> where TNode : notnull
{
    IReadOnlyList<TNode> GetTopologicalOrder();
    IReadOnlyCollection<TNode> GetAncestors(TNode node);     // strict, snapshot
    IReadOnlyCollection<TNode> GetDescendants(TNode node);   // strict, snapshot
    bool WouldCreateCycle(TNode source, TNode target);
}

// ── Mutable contracts ────────────────────────────────────────────────────────

public interface IGraph<TNode> : IReadOnlyGraph<TNode> where TNode : notnull
{
    bool AddNode(TNode node);
    int AddNodes(IEnumerable<TNode> nodes);
    bool RemoveNode(TNode node);
    bool AddEdge(TNode source, TNode target);
    int AddEdges(IEnumerable<Edge<TNode>> edges);
    bool RemoveEdge(TNode source, TNode target);
    void Clear();
}

public interface IUndirectedGraph<TNode> : IGraph<TNode>, IReadOnlyUndirectedGraph<TNode> where TNode : notnull { }

public interface IDirectedGraph<TNode> : IGraph<TNode>, IReadOnlyDirectedGraph<TNode> where TNode : notnull { }

public interface IDirectedAcyclicGraph<TNode> : IDirectedGraph<TNode>, IReadOnlyDirectedAcyclicGraph<TNode> where TNode : notnull { }

// ── Classes ──────────────────────────────────────────────────────────────────

public abstract class Graph<TNode> : IGraph<TNode> where TNode : notnull
{
    private protected Graph(bool isDirected, int nodeCapacity, IEqualityComparer<TNode>? comparer);   // not public API
    // IGraph<TNode> members: public, non-virtual
}

public sealed class UndirectedGraph<TNode> : Graph<TNode>, IUndirectedGraph<TNode> where TNode : notnull
{
    public UndirectedGraph();
    public UndirectedGraph(int nodeCapacity);
    public UndirectedGraph(IEqualityComparer<TNode>? comparer);
    public UndirectedGraph(int nodeCapacity, IEqualityComparer<TNode>? comparer);
    // IReadOnlyUndirectedGraph<TNode> members
}

public class DirectedGraph<TNode> : Graph<TNode>, IDirectedGraph<TNode> where TNode : notnull
{
    public DirectedGraph();
    public DirectedGraph(int nodeCapacity);
    public DirectedGraph(IEqualityComparer<TNode>? comparer);
    public DirectedGraph(int nodeCapacity, IEqualityComparer<TNode>? comparer);
    // IReadOnlyDirectedGraph<TNode> members
}

public class DirectedAcyclicGraph<TNode> : DirectedGraph<TNode>, IDirectedAcyclicGraph<TNode> where TNode : notnull
{
    // the same four constructors; IReadOnlyDirectedAcyclicGraph<TNode> members
}

public sealed class ReachabilityDirectedAcyclicGraph<TNode> : DirectedAcyclicGraph<TNode> where TNode : notnull
{
    // the same four constructors; no new public members
}
```

## Semantics

- **Simple graphs only.** There are no parallel edges: `AddEdge` returns false if the edge already exists, and it auto-adds missing endpoints. `AddNodes` and `AddEdges` return the number actually added.
- **Self-loops** are allowed in `UndirectedGraph` and `DirectedGraph`. A DAG rejects them as cycles.
- **`UndirectedGraph` behavior.**
  - `ContainsEdge` is symmetric.
  - `Edges` yields each edge once, with `Source` set to the endpoint inserted first.
  - A self-loop counts once in `EdgeCount` and contributes 2 to `GetDegree`.
  - `AreConnected(first, second)` is true when both nodes are in the same connected component. `AreConnected(x, x)` is true whenever x exists.
- **Directed behavior.** `GetNeighbors` returns successors ∪ predecessors, each node once. `GetDegree` is in-degree + out-degree.
- **Paths are non-empty.** `HasPath(s, t)` means a path of length ≥ 1, so `HasPath(x, x)` is false in a DAG. Ancestors and descendants are strict: they exclude the node itself.
- **Missing nodes.**
  - `ContainsNode`, `ContainsEdge`, `HasPath`, `AreConnected`, and `WouldCreateCycle` return false. The exception is `WouldCreateCycle(x, x)`, which is always true.
  - `RemoveNode` and `RemoveEdge` return false.
  - Every other member that takes a node throws `KeyNotFoundException`.
- **Argument validation.**
  - A null node throws `ArgumentNullException`.
  - A null `nodes` or `edges` argument throws `ArgumentNullException`.
  - An edge with a null endpoint, such as `default(Edge<TNode>)`, throws `ArgumentException`.
  - A negative `nodeCapacity` throws `ArgumentOutOfRangeException`.
  - A null comparer means `EqualityComparer<TNode>.Default`.
- **Equality.** Node lookup uses `Comparer`. `Edge<TNode>` equality always uses `EqualityComparer<TNode>.Default`; document that difference.
- **Edge rejection.**
  - The docs for `IGraph<TNode>.AddEdge` and `AddEdges` state that implementations may reject edges that violate their invariants by throwing an `InvalidOperationException`-derived exception.
  - DAG `AddEdge` throws `GraphCycleException` and leaves the graph unchanged, including any endpoints it would have auto-added. Validate before auto-adding.
  - DAG `AddEdges` is atomic. If any edge would create a cycle, including cycles formed only by combining edges within the batch, throw and leave the graph exactly as it was.
- **Live views.** `Nodes`, `Edges`, `GetSuccessors`, `GetPredecessors`, and `GetNeighbors` return live read-only views, like `Dictionary<TKey, TValue>.KeyCollection`. An enumeration in progress throws `InvalidOperationException` if the graph is mutated.
- **Snapshots.** `GetAncestors`, `GetDescendants`, `GetSources`, `GetSinks`, and `GetTopologicalOrder` return snapshots. `GetTopologicalOrder` may return a cached immutable instance.
- **Determinism.** `Nodes` enumerates in slot order. `GetTopologicalOrder` breaks ties by node insertion sequence, so identical operation sequences produce identical orders.
- **Threading.** The classes are not thread-safe for mutation; concurrent reads with no writers are safe. Document this on each class.
- **Debugging.** Put `[DebuggerDisplay]` on each graph class, showing the node and edge counts.
- **No recursion anywhere.** All traversals are iterative, so deep graphs can't overflow the stack.

## Internal design

- **`NodeTable<TNode>`.** A `Dictionary<TNode, int>` (using `Comparer`) maps each node to a dense slot. It also holds a `TNode[]` indexed by slot, a free-slot stack for reuse, a monotonically increasing insertion sequence per slot, and a version counter incremented on every mutation. It replaces the old per-vertex dictionary storage.
- **Per-class storage.**
  - `Graph<TNode>` is an abstract, direction-neutral base. It owns the `NodeTable`, a `HashSet<int>[]` adjacency array, and the `*Core` hooks.
  - `UndirectedGraph` stores each edge in both directions and overrides the edge, degree, neighbor, and edge-enumeration cores.
  - `DirectedGraph` treats the adjacency array as successors and adds a `HashSet<int>[]` of predecessors.
  - `ReachabilityDirectedAcyclicGraph` adds `BitSet[]` arrays for descendants and ancestors.
- **Hook pattern.**
  - Public members are non-virtual. They validate arguments, map `TNode` to its slot, and call `private protected virtual` `*Core` methods.
  - All variation between classes goes through those hooks, e.g. `OnCapacityChanged`, `OnNodeAdded`, `RemoveNodeCore`, `AddEdgeCore`, `AddEdgesCore`, `RemoveEdgeCore`, `ClearCore`, `GetNeighborsCore`, `GetDegreeCore`, `EnumerateEdgesCore`, `HasPathCore`, `GetAncestorsCore`, `GetDescendantsCore`, and `WouldCreateCycleCore`.
  - Using `private protected` means no extensibility contract is published to outside code. `Graph<TNode>`'s only constructor is `private protected`, so only this library can derive from it. `UndirectedGraph` and `ReachabilityDirectedAcyclicGraph` are sealed.
- **Traversal and ordering.**
  - `UndirectedGraph.AreConnected` and `DirectedGraph.HasPathCore` use iterative BFS.
  - `DirectedAcyclicGraph.WouldCreateCycleCore(s, t)` is `s == t || HasPathCore(t, s)`.
  - Topological order uses Kahn's algorithm with `MinHeap` keyed on insertion sequence, cached by version.
- **`BitSet`.** Backed by `ulong[]`, with Get, Set, Clear, UnionWith, CopyFrom, ClearAll, PopCount (SWAR), and set-bit enumeration. All bitsets grow in `OnCapacityChanged`.
- **Other internals:** read-only view types over slot sets, an edge collection view, and `ThrowHelper`.

## Closure maintenance (ReachabilityDirectedAcyclicGraph)

- **Queries.**
  - `HasPath(s, t)` is `descendants[s].Get(t)`.
  - `WouldCreateCycle(s, t)` is `s == t || descendants[t].Get(s)`.
  - Ancestor and descendant queries enumerate set bits.
- **Add edge u→v.**
  - If v is already in descendants[u], reachability is unchanged; stop after the base adds the edge.
  - Otherwise, before modifying anything, compute A = {u} ∪ ancestors[u] and D = {v} ∪ descendants[v].
  - Then set descendants[a] |= D for each a in A, and ancestors[d] |= A for each d in D.
- **Remove edge u→v.**
  - Before removing, capture A and D as above, plus the current topological order. Removals never invalidate a topological order, so reuse it.
  - Only the descendant sets of nodes in A and the ancestor sets of nodes in D can change.
  - Recompute descendants[a] for each a in A, in reverse topological order, as the union over successors s of ({s} ∪ descendants[s]).
  - Recompute ancestors[d] for each d in D, in topological order, from predecessors.
- **Remove node x.**
  - Capture A = ancestors[x], D = descendants[x], and the topological order.
  - Remove x via the base and clear x's bitsets.
  - Recompute A and D in one pass, as above, rather than once per incident edge.
- **`AddEdges`.** Let the base validate and add atomically. Then either apply incremental updates or rebuild the whole closure in one reverse-topological pass, chosen by a size heuristic that you document.
- **Invariants**, checked with `Debug.Assert` in DEBUG builds and in tests:
  - descendants[a] contains b ⇔ ancestors[b] contains a.
  - No bits are set for free slots.
  - No node is its own ancestor.

## Test coverage

- **`Edge<TNode>`:** equality, hash code, operators, `Reverse`, `Deconstruct`, `ToString`, and `Edge.Create`.
- **Every public member of every graph class.** Cover the happy path, missing nodes, null arguments, duplicates, self-loops, a custom comparer (`StringComparer.OrdinalIgnoreCase`), slot reuse after `RemoveNode`, view enumeration invalidation, `Clear`, and a negative `nodeCapacity`.
- **`UndirectedGraph`:** symmetric `ContainsEdge` and `GetNeighbors`, each edge enumerated once, self-loop degree, and `AreConnected`.
- **`DirectedGraph`:** successor/predecessor consistency, sources and sinks, and `HasPath`.
- **`DirectedAcyclicGraph`.**
  - Rejects self-loops, 2-cycles, and long cycles, and the graph is unchanged after each rejection, with no leftover auto-added nodes.
  - `AddEdges` is atomic, including for cycles formed only within the batch.
  - The topological order is valid and deterministic, and the cached order is invalidated by mutation.
- **Type relationships.**
  - `DirectedGraph<T>` is not assignable to `IUndirectedGraph<T>`.
  - `UndirectedGraph<T>` is not assignable to `IDirectedGraph<T>`.
  - Both are assignable to `IGraph<T>`.
  - `ReachabilityDirectedAcyclicGraph<T>` is assignable to `IDirectedAcyclicGraph<T>`.
- **Differential randomized tests**, with fixed seeds.
  - Generate long random operation sequences: add node; add an edge consistent with a hidden random order (so it stays acyclic); attempt a cycle-creating edge; remove edge; remove node; re-add a removed node; an `AddEdges` batch; and an occasional `Clear`.
  - Apply each sequence to both `DirectedAcyclicGraph` and `ReachabilityDirectedAcyclicGraph`.
  - After every operation, record any disagreement between the two graphs, or between either graph and a brute-force BFS oracle, on `HasPath` for all pairs, `GetAncestors` and `GetDescendants` for all nodes, and `WouldCreateCycle`.
  - Include sizes above 64 and 128 nodes, so multi-word bitsets and growth are exercised.
- **Deep chains**, to prove there's no recursion: 100,000 nodes for `DirectedAcyclicGraph` (`HasPath`, `GetTopologicalOrder`), and about 5,000 for the closure class, whose memory is quadratic.

## Benchmarks

Using BenchmarkDotNet with `[MemoryDiagnoser]`, measure `HasPath`, `WouldCreateCycle`, `AddEdge`, and `RemoveEdge` for `DirectedAcyclicGraph` versus `ReachabilityDirectedAcyclicGraph`, at a few sizes and densities, to quantify the read/write tradeoff. Build the project, but don't run it as part of the test suite.

## README

Rewrite `README.md` at the repo root. It is also packed into the NuGet package, so it has to read well on both GitHub and nuget.org.

### Rendering rules

- Link repo files with absolute URLs (e.g. `https://github.com/ericburcham/graphical/blob/main/LICENSE`), because nuget.org doesn't resolve relative links.
- Use plain Markdown only: no raw HTML and no Mermaid, since nuget.org renders neither. Draw diagrams as text trees in code blocks.
- Add badges only for things that exist. A shields.io MIT license badge is fine. Add no NuGet badge (it would show 1.0.2 until 2.0 is published) and no build badge (there's no CI yet).

### Sections, in this order

1. **Title and tagline.** "Graphical", then "A graph library for .NET", then one short paragraph covering:
   - undirected, directed, and acyclic graphs;
   - topological sorting and cycle-safe edges;
   - an optional self-maintaining transitive closure for O(1) reachability;
   - zero dependencies and .NET Standard 2.0.
2. **Installation.**
   - `dotnet add package Graphical --prerelease`.
   - Supported platforms: anything that runs .NET Standard 2.0, including .NET Framework 4.7.2+ and .NET 5+ / .NET Core 2.0+.
   - A note that 2.x is a ground-up rewrite that shares no API with Graphical 1.x.
3. **Quick start.** One example of 15 lines or fewer: build a DAG of tasks with dependencies, print a topological order, and show that a cycle-creating edge throws `GraphCycleException`.
4. **Choosing a graph type.** A table with the columns *Type*, *Use it when*, and *Trade-off*:

   | Type | Use it when | Trade-off |
   | :- | :- | :- |
   | `UndirectedGraph<TNode>` | relationships are symmetric (networks, adjacency, connectivity) | no direction |
   | `DirectedGraph<TNode>` | edges have direction and cycles are allowed (state machines, link graphs) | `HasPath` traverses: O(V + E) |
   | `DirectedAcyclicGraph<TNode>` | you need dependency ordering and cycle rejection (builds, task scheduling) | reachability queries traverse: O(V + E) |
   | `ReachabilityDirectedAcyclicGraph<TNode>` | same as above, but you query reachability far more often than you change the graph | O(1) `HasPath` and `WouldCreateCycle`, but writes cost more and memory is quadratic |

   Follow the table with:
   - The guidance: start with `DirectedAcyclicGraph<TNode>`, and switch to `ReachabilityDirectedAcyclicGraph<TNode>` (same API, drop-in) when reachability reads dominate.
   - A memory table for the closure, at about V²/4 bytes: 1,000 nodes ≈ 250 KB, 10,000 nodes ≈ 25 MB, 100,000 nodes ≈ 2.5 GB.
5. **Type hierarchy.**
   - A text tree of the interfaces and classes.
   - A short paragraph on which interface to accept: `IGraph<TNode>` when direction doesn't matter, `IUndirectedGraph<TNode>` or `IDirectedGraph<TNode>` when it does, and the `IReadOnly…` interfaces for consumers that shouldn't mutate.
6. **Usage.** One subsection per class, with short examples of what's distinctive about it:
   - `UndirectedGraph`: `AreConnected` and symmetric edges.
   - `DirectedGraph`: successors, predecessors, sources, sinks, and `HasPath`.
   - `DirectedAcyclicGraph`: `WouldCreateCycle`, atomic `AddEdges`, `GetTopologicalOrder`, and ancestors/descendants.
   - `ReachabilityDirectedAcyclicGraph`: swapping it in.
7. **Core concepts.** Short subsections:
   - **Nodes:** any `notnull` type whose equality stays stable while it's in a graph, plus a custom comparer example using `StringComparer.OrdinalIgnoreCase`.
   - **Edges:** simple graphs, the self-loop rules, auto-added endpoints, and `Edge<TNode>`.
   - **Live views vs. snapshots.**
   - **Missing nodes and exceptions:** a small table showing which members return false and which throw `KeyNotFoundException`, `ArgumentNullException`, or `GraphCycleException`.
   - **Determinism.**
   - **Thread safety.**
8. **Performance.** A time-complexity table for every public member of each class, matching the XML docs, plus the closure memory note.
9. **Building from source.**
   - Prerequisites: the .NET 10 SDK, and Windows for the `net48` tests.
   - `dotnet build`, `dotnet test`, and the command to run the benchmarks.
   - Any setup steps recorded in `DECISIONS.md`.
10. **Contributing.**
    - Links to the repo's Issues and Discussions.
    - A summary of the TDD workflow and test conventions (`When…` fixtures, `…Should…` tests, one assertion per test).
    - A note that public API changes must update `PublicAPI.Unshipped.txt`.
11. **License.** MIT, with an absolute link to `LICENSE`.

### Accuracy

- Every C# sample must compile against the final API and behave exactly as described.
- Mirror each sample in a fixture under `ReadmeTests/`, following the test conventions, so the README can't drift from the code.

## Done when

- `dotnet build -c Release` on the solution exits 0 with 0 warnings and 0 errors, benchmarks included.
- `dotnet test -c Release` exits 0 with no failed or skipped tests, on both `net10.0` and `net48`.
- `PublicAPI.Unshipped.txt` lists exactly the Public API above.
- Every public member has XML docs covering behavior, exceptions, and time complexity.
- No trace of the old vertex API remains: no `AddVertex`, no `GetVertices`, and no vertex-named tests.
- `dotnet pack -c Release` produces `Graphical.2.0.0-preview.1.nupkg` and `.snupkg` with no warnings, and the `.nupkg` contains `README.md`.
- The README follows the "README" section, and every sample in it is mirrored by a passing `ReadmeTests/` fixture.
- `git status` is clean on `create-graph-types`, and the history is a series of small TDD commits.
- `DECISIONS.md` lists every judgment call and any setup commands I need to run.
- Your final report prints the evidence for each item above.
