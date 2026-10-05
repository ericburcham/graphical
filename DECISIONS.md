# Decisions

Judgment calls made while implementing `docs/graph-types-spec.md`. One entry per decision: the decision, then the reason.

## Answers to pre-run questions (from the repository owner)

1. **Library retargeted to `netstandard2.0` in milestone 1, together with the tests.** .NET Framework 4.8 cannot reference a `netstandard2.1` library, so the `net48` test target cannot build otherwise. Milestone 2 covers the rest of the build setup.
2. **Context7 is used for documentation lookups as the spec describes.** If it becomes unavailable, fall back to the official documentation sites (learn.microsoft.com, nunit.org, fluentassertions.com, project GitHub READMEs), confirm versions with `dotnet package search`, and log the switch here.
3. **`UndirectedGraph<TNode>.Edges` reports `Source` as the endpoint whose node was added to the graph earlier**, judged by insertion sequence rather than slot index, because slots get reused.
4. **Benchmarks live in `src/benchmarks/Graphical.Benchmarks/`.** Benchmarks aren't tests, and this follows the existing `src/code` / `src/tests` convention.
5. **`AddEdges` and `AddNodes` copy the batch into a list and validate every item before changing anything**, so no graph type is left half-updated by an argument error.

## Implementation decisions
