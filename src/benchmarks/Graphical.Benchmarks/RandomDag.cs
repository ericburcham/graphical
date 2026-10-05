namespace Graphical.Benchmarks;

/// <summary>Builds reproducible random DAGs: every edge points from a lower-numbered node to a higher-numbered one.</summary>
internal static class RandomDag
{
    private const int SEED = 20_261_005;

    public static DirectedAcyclicGraph<int> Create(GraphKind kind, int nodes)
    {
        return kind == GraphKind.ReachabilityDirectedAcyclicGraph
            ? new ReachabilityDirectedAcyclicGraph<int>(nodes)
            : new DirectedAcyclicGraph<int>(nodes);
    }

    public static List<Edge<int>> Edges(int nodes, int averageOutDegree)
    {
        var random = new Random(SEED);
        var edges = new HashSet<Edge<int>>();
        while (edges.Count < nodes * averageOutDegree / 2)
        {
            var first = random.Next(nodes);
            var second = random.Next(nodes);
            if (first != second) edges.Add(first < second ? Edge.Create(first, second) : Edge.Create(second, first));
        }

        return [.. edges];
    }

    public static (int First, int Second)[] Pairs(int nodes, int count)
    {
        var random = new Random(SEED + 1);
        var pairs = new (int, int)[count];
        for (var index = 0; index < count; index++) pairs[index] = (random.Next(nodes), random.Next(nodes));
        return pairs;
    }
}
