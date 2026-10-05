using BenchmarkDotNet.Attributes;

namespace Graphical.Benchmarks;

/// <summary>
/// Write cost: adding and removing a fixed set of edges on each DAG type. Each iteration starts from the same graph,
/// so iterations run one invocation at a time and the setup puts the graph back.
/// </summary>
[MemoryDiagnoser]
[InvocationCount(1)]
public class EdgeMutationBenchmarks
{
    private const int EDGES_PER_ITERATION = 64;

    private DirectedAcyclicGraph<int> _graph = null!;

    private Edge<int>[] _candidates = [];

    [Params(GraphKind.DirectedAcyclicGraph, GraphKind.ReachabilityDirectedAcyclicGraph)]
    public GraphKind Kind { get; set; }

    [Params(1_000, 4_000)]
    public int Nodes { get; set; }

    [Params(2, 8)]
    public int AverageOutDegree { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var edges = RandomDag.Edges(Nodes, AverageOutDegree);
        _candidates = [.. edges.Take(EDGES_PER_ITERATION)];
        _graph = RandomDag.Create(Kind, Nodes);
        _graph.AddNodes(Enumerable.Range(0, Nodes));
        _graph.AddEdges(edges.Skip(EDGES_PER_ITERATION));
    }

    [IterationSetup(Target = nameof(AddEdge))]
    public void RemoveCandidates()
    {
        foreach (var edge in _candidates) _graph.RemoveEdge(edge.Source, edge.Target);
    }

    [IterationSetup(Target = nameof(RemoveEdge))]
    public void AddCandidates()
    {
        foreach (var edge in _candidates) _graph.AddEdge(edge.Source, edge.Target);
    }

    [Benchmark(OperationsPerInvoke = EDGES_PER_ITERATION)]
    public int AddEdge()
    {
        var added = 0;
        foreach (var edge in _candidates)
        {
            if (_graph.AddEdge(edge.Source, edge.Target)) added++;
        }

        return added;
    }

    [Benchmark(OperationsPerInvoke = EDGES_PER_ITERATION)]
    public int RemoveEdge()
    {
        var removed = 0;
        foreach (var edge in _candidates)
        {
            if (_graph.RemoveEdge(edge.Source, edge.Target)) removed++;
        }

        return removed;
    }
}
