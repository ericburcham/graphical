# Graphical

A graph library for .NET

[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/ericburcham/graphical/blob/main/LICENSE)

Graphical gives you undirected, directed and acyclic graphs behind a small set of interfaces. Directed acyclic graphs sort topologically and refuse any edge that would create a cycle, and an optional graph type maintains its own transitive closure so reachability questions are answered in O(1). The library has zero dependencies and targets .NET Standard 2.0.

## Installation

```shell
dotnet add package Graphical --prerelease
```

Graphical runs anywhere .NET Standard 2.0 does, including .NET Framework 4.7.2 and later, and .NET Core 2.0 / .NET 5 and later.

Graphical 2.x is a ground-up rewrite. It shares no API with Graphical 1.x.

## Quick start

```csharp
using Graphical;

var build = new DirectedAcyclicGraph<string>();
build.AddEdge("restore", "compile");   // restore must run before compile
build.AddEdge("compile", "test");
build.AddEdge("compile", "pack");

Console.WriteLine(string.Join(" -> ", build.GetTopologicalOrder()));
// restore -> compile -> test -> pack

try { build.AddEdge("pack", "restore"); }
catch (GraphCycleException e) { Console.WriteLine(e.Message); }
// Adding the edge (pack -> restore) would create a cycle.
```

## Choosing a graph type

| Type | Use it when | Trade-off |
| :- | :- | :- |
| `UndirectedGraph<TNode>` | relationships are symmetric (networks, adjacency, connectivity) | no direction |
| `DirectedGraph<TNode>` | edges have direction and cycles are allowed (state machines, link graphs) | `HasPath` traverses: O(V + E) |
| `DirectedAcyclicGraph<TNode>` | you need dependency ordering and cycle rejection (builds, task scheduling) | reachability queries traverse: O(V + E) |
| `ReachabilityDirectedAcyclicGraph<TNode>` | same as above, but you query reachability far more often than you change the graph | O(1) `HasPath` and `WouldCreateCycle`, but writes cost more and memory is quadratic |

Start with `DirectedAcyclicGraph<TNode>`. Switch to `ReachabilityDirectedAcyclicGraph<TNode>` (same API, drop-in) when reachability reads dominate.

The closure takes about V²/4 bytes:

| Nodes | Closure memory |
| -: | -: |
| 1,000 | ≈ 250 KB |
| 10,000 | ≈ 25 MB |
| 100,000 | ≈ 2.5 GB |

## Type hierarchy

```text
IReadOnlyGraph<TNode>
├── IReadOnlyUndirectedGraph<TNode>
├── IReadOnlyDirectedGraph<TNode>
│   └── IReadOnlyDirectedAcyclicGraph<TNode>
└── IGraph<TNode>
    ├── IUndirectedGraph<TNode>          (also IReadOnlyUndirectedGraph<TNode>)
    └── IDirectedGraph<TNode>            (also IReadOnlyDirectedGraph<TNode>)
        └── IDirectedAcyclicGraph<TNode> (also IReadOnlyDirectedAcyclicGraph<TNode>)

Graph<TNode>                                  : IGraph<TNode>  (abstract)
├── UndirectedGraph<TNode>                    : IUndirectedGraph<TNode>  (sealed)
└── DirectedGraph<TNode>                      : IDirectedGraph<TNode>
    └── DirectedAcyclicGraph<TNode>           : IDirectedAcyclicGraph<TNode>
        └── ReachabilityDirectedAcyclicGraph<TNode>  (sealed)
```

Accept `IGraph<TNode>` when direction doesn't matter to your code, and `IUndirectedGraph<TNode>` or `IDirectedGraph<TNode>` when it does. Accept the `IReadOnly…` interfaces when your code only reads the graph.

## Usage

### UndirectedGraph

```csharp
var network = new UndirectedGraph<string>();
network.AddEdge("alice", "bob");
network.AddEdge("bob", "carol");
network.AddNode("dave");

var symmetric = network.ContainsEdge("bob", "alice");     // true: edges have no direction
var together = network.AreConnected("alice", "carol");    // true: same component
var apart = network.AreConnected("alice", "dave");        // false
var neighbors = network.GetNeighbors("bob");              // alice, carol
```

### DirectedGraph

```csharp
var links = new DirectedGraph<string>();
links.AddEdges(new[]
{
    Edge.Create("home", "about"), Edge.Create("home", "blog"),
    Edge.Create("blog", "post"), Edge.Create("post", "home"),
});
links.AddNode("orphan");

var successors = links.GetSuccessors("home");       // about, blog
var predecessors = links.GetPredecessors("home");   // post
var sources = links.GetSources();                   // orphan (nothing links to it)
var sinks = links.GetSinks();                       // about, orphan (they link nowhere)
var reachable = links.HasPath("post", "about");     // true: post -> home -> about
var onACycle = links.HasPath("home", "home");       // true: home -> blog -> post -> home
```

### DirectedAcyclicGraph

```csharp
var tasks = new DirectedAcyclicGraph<string>();
tasks.AddEdges(new[] { Edge.Create("design", "build"), Edge.Create("build", "test"), Edge.Create("build", "docs") });

var wouldCycle = tasks.WouldCreateCycle("test", "design");   // true
var ancestors = tasks.GetAncestors("test");                  // design, build
var descendants = tasks.GetDescendants("design");            // build, test, docs

try
{
    // Together these two edges close a cycle, so neither is added.
    tasks.AddEdges(new[] { Edge.Create("test", "release"), Edge.Create("release", "build") });
}
catch (GraphCycleException)
{
}

var added = tasks.ContainsNode("release");                   // false: the batch is atomic
var order = tasks.GetTopologicalOrder();                     // design, build, test, docs
```

### ReachabilityDirectedAcyclicGraph

Change only the constructor:

```csharp
var tasks = new ReachabilityDirectedAcyclicGraph<string>();   // was: new DirectedAcyclicGraph<string>()
tasks.AddEdges(new[] { Edge.Create("design", "build"), Edge.Create("build", "test") });

var reachable = tasks.HasPath("design", "test");              // true, in O(1)
var wouldCycle = tasks.WouldCreateCycle("test", "design");    // true, in O(1)
```

## Core concepts

### Nodes

A node can be any non-null type whose equality and hash code stay the same while it is in a graph. Graphs compare nodes with `EqualityComparer<TNode>.Default` unless you pass a comparer:

```csharp
var tags = new UndirectedGraph<string>(StringComparer.OrdinalIgnoreCase);
tags.AddEdge("CSharp", "dotnet");

var found = tags.ContainsNode("csharp");          // true
var added = tags.AddEdge("DOTNET", "csharp");     // false: it is the same edge
var nodes = tags.Nodes;                           // CSharp, dotnet (first spelling wins)
```

### Edges

Every graph is simple: there is at most one edge from a given source to a given target, and `AddEdge` returns `false` for a duplicate. `AddEdge` adds missing endpoints for you. Self-loops are allowed in `UndirectedGraph` and `DirectedGraph`; a DAG rejects them as cycles. In an undirected graph a self-loop counts once in `EdgeCount` and adds 2 to the node's degree.

`Edge<TNode>` is a small value type for passing edges around:

```csharp
var edge = Edge.Create("a", "b");
var (source, target) = edge;                    // "a", "b"
var reversed = edge.Reverse().ToString();       // "(b -> a)"
var equal = edge == new Edge<string>("a", "b"); // true

var graph = new DirectedGraph<string>();
graph.AddEdge("a", "b");                        // adds "a" and "b" too
var addedAgain = graph.AddEdge("a", "b");       // false: graphs are simple
var nodeCount = graph.NodeCount;                // 2
```

Edge equality is ordered and always uses `EqualityComparer<TNode>.Default`, even for edges from a graph with a custom comparer.

### Live views vs. snapshots

`Nodes`, `Edges`, `GetNeighbors`, `GetSuccessors` and `GetPredecessors` return live read-only views: they always reflect the graph's current state, and an enumeration in progress throws `InvalidOperationException` if the graph changes. `GetSources`, `GetSinks`, `GetAncestors`, `GetDescendants` and `GetTopologicalOrder` return snapshots.

```csharp
var graph = new DirectedGraph<int>();
var nodes = graph.Nodes;            // live view
var sources = graph.GetSources();   // snapshot

graph.AddNode(1);

var viewCount = nodes.Count;        // 1: the view sees the new node
var snapshotCount = sources.Count;  // 0: the snapshot does not
```

### Missing nodes and exceptions

| Situation | Result |
| :- | :- |
| `ContainsNode`, `ContainsEdge`, `HasPath`, `AreConnected` or `WouldCreateCycle` with a missing node | `false` (except `WouldCreateCycle(x, x)`, which is always `true`) |
| `RemoveNode` or `RemoveEdge` with a missing node | `false` |
| Any other member given a missing node | `KeyNotFoundException` |
| A `null` node, or a `null` `nodes` / `edges` argument | `ArgumentNullException` |
| A `null` inside `nodes`, or an edge with a `null` endpoint such as `default(Edge<TNode>)` | `ArgumentException`, and nothing is added |
| A negative `nodeCapacity` | `ArgumentOutOfRangeException` |
| An edge or batch that would create a cycle in a DAG | `GraphCycleException`, and the graph is unchanged |

### Determinism

`Nodes` enumerates in slot order: insertion order, except that a node added after a removal reuses the most recently freed slot. `GetTopologicalOrder` breaks ties by insertion order, so the same sequence of operations always produces the same order.

### Thread safety

The graphs are not thread-safe for mutation. Any number of threads may read a graph at once as long as nothing writes to it; a write needs exclusive access.

## Performance

V is the number of nodes, E the number of edges and k the size of a batch. "Closure" is `ReachabilityDirectedAcyclicGraph`.

| Member | Undirected / Directed | DirectedAcyclicGraph | Closure |
| :- | :- | :- | :- |
| `IsDirected`, `Comparer`, `NodeCount`, `EdgeCount` | O(1) | O(1) | O(1) |
| enumerate `Nodes` / `Edges` | O(V) / O(V + E) | O(V) / O(V + E) | O(V) / O(V + E) |
| `ContainsNode`, `ContainsEdge`, `GetDegree` | O(1) | O(1) | O(1) |
| `GetNeighbors` (view; enumeration is O(degree)) | O(1) | O(1) | O(1) |
| `AddNode` | amortized O(1) | amortized O(1) | amortized O(1); growth is O(V²/64) |
| `AddNodes` | O(k) | O(k) | O(k) plus growth |
| `RemoveNode` | O(degree) | O(degree) | O((V + E) · V/64) worst case |
| `AddEdge` | amortized O(1) | O(V + E) | O(V²/64) worst case |
| `AddEdges` | O(k) | O(V + E + k) | O(V + E + k) plus closure update or rebuild |
| `RemoveEdge` | O(1) | O(1) | O((V + E) · V/64) worst case |
| `Clear` | O(V) | O(V) | O(V²/64) |
| `AreConnected` (undirected) | O(V + E) | | |
| `GetSuccessors`, `GetPredecessors` (views) | O(1) | O(1) | O(1) |
| `GetInDegree`, `GetOutDegree` | O(1) | O(1) | O(1) |
| `GetSources`, `GetSinks` | O(V) | O(V) | O(V) |
| `HasPath` | O(V + E) | O(V + E) | O(1) |
| `WouldCreateCycle` | | O(V + E) | O(1) |
| `GetAncestors`, `GetDescendants` | | O(V + E) | O(V/64 + result) |
| `GetTopologicalOrder` | | O(V log V + E), O(1) when cached | O(V log V + E), O(1) when cached |

The closure needs about V²/4 bytes (two bits per pair of nodes): roughly 250 KB at 1,000 nodes, 25 MB at 10,000 and 2.5 GB at 100,000.

## Building from source

You need the .NET 10 SDK. The `net48` tests also need Windows.

```shell
cd src
dotnet build
dotnet test
dotnet run -c Release --project benchmarks/Graphical.Benchmarks -- --filter "*"
```

No other setup is needed.

## Contributing

Report bugs and request features in [Issues](https://github.com/ericburcham/graphical/issues); ask questions in [Discussions](https://github.com/ericburcham/graphical/discussions).

Changes are built test-first: write one failing test, make it pass with the simplest reasonable code, then refactor while the suite stays green. Each test fixture describes one scenario and is named `When…`; its `[OneTimeSetUp]` runs the scenario once, and each test makes exactly one assertion and is named `…Should…`. Behavior shared by several graph types lives in contract fixtures that run against every implementation.

Public API changes must be recorded in `src/code/Graphical/PublicAPI.Unshipped.txt`; the build fails otherwise.

## License

Graphical is released under the [MIT License](https://github.com/ericburcham/graphical/blob/main/LICENSE).
