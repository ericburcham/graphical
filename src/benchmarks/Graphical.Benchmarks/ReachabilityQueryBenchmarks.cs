using BenchmarkDotNet.Attributes;

namespace Graphical.Benchmarks;

/// <summary>Read cost: how long reachability questions take on each DAG type.</summary>
[MemoryDiagnoser]
public class ReachabilityQueryBenchmarks
{
    private const int QUERIES = 256;

    private DirectedAcyclicGraph<int> _graph = null!;

    private (int First, int Second)[] _pairs = [];

    [Params(GraphKind.DirectedAcyclicGraph, GraphKind.ReachabilityDirectedAcyclicGraph)]
    public GraphKind Kind { get; set; }

    [Params(1_000, 4_000)]
    public int Nodes { get; set; }

    [Params(2, 8)]
    public int AverageOutDegree { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _graph = RandomDag.Create(Kind, Nodes);
        _graph.AddNodes(Enumerable.Range(0, Nodes));
        _graph.AddEdges(RandomDag.Edges(Nodes, AverageOutDegree));
        _pairs = RandomDag.Pairs(Nodes, QUERIES);
    }

    [Benchmark(OperationsPerInvoke = QUERIES)]
    public int HasPath()
    {
        var found = 0;
        foreach (var (first, second) in _pairs)
        {
            if (_graph.HasPath(first, second)) found++;
        }

        return found;
    }

    [Benchmark(OperationsPerInvoke = QUERIES)]
    public int WouldCreateCycle()
    {
        var found = 0;
        foreach (var (first, second) in _pairs)
        {
            if (_graph.WouldCreateCycle(first, second)) found++;
        }

        return found;
    }
}
