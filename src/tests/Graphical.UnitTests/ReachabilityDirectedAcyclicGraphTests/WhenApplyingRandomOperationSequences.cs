using AwesomeAssertions;

namespace Graphical.UnitTests.ReachabilityDirectedAcyclicGraphTests;

[TestFixtureSource(nameof(SCENARIOS))]
internal sealed class WhenApplyingRandomOperationSequences
{
    private static readonly object[] SCENARIOS =
    [
        new object[] { 1, 40, 600 },
        new object[] { 2, 100, 400 },
        new object[] { 3, 200, 330 },
    ];

    private readonly int _seed;

    private readonly int _nodeLimit;

    private readonly int _operations;

    private readonly List<string> _discrepancies = [];

    private Random _random = null!;

    private int[] _ranks = [];

    private DirectedAcyclicGraph<int> _traversing = null!;

    private ReachabilityDirectedAcyclicGraph<int> _closure = null!;

    private Dictionary<int, HashSet<int>> _oracle = null!;

    private readonly HashSet<int> _removed = [];

    private int _peakNodeCount;

    public WhenApplyingRandomOperationSequences(int seed, int nodeLimit, int operations)
    {
        _seed = seed;
        _nodeLimit = nodeLimit;
        _operations = operations;
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _random = new Random(_seed);
        _ranks = Enumerable.Range(0, _nodeLimit).OrderBy(_ => _random.Next()).ToArray();
        _traversing = new DirectedAcyclicGraph<int>();
        _closure = new ReachabilityDirectedAcyclicGraph<int>();
        _oracle = [];

        for (var step = 0; step < _operations; step++)
        {
            var operation = ApplyRandomOperation();
            _peakNodeCount = Math.Max(_peakNodeCount, _oracle.Count);
            Compare($"step {step} ({operation})");
        }
    }

    [Test]
    public void TheGraphsShouldAgreeWithEachOtherAndTheOracle()
    {
        _discrepancies.Should().BeEmpty();
    }

    [Test]
    public void TheSequenceShouldGrowPastTheScenarioSize()
    {
        _peakNodeCount.Should().BeGreaterThan(_nodeLimit * 3 / 4);
    }

    private string ApplyRandomOperation()
    {
        var roll = _random.NextDouble();
        if (_oracle.Count < 2 || roll < 0.25) return AddNode();
        if (roll < 0.55) return AddAcyclicEdge();
        if (roll < 0.62) return AttemptCycle();
        if (roll < 0.72) return RemoveEdge();
        if (roll < 0.78) return RemoveNode();
        if (roll < 0.84) return ReAddRemovedNode();
        if (roll < 0.995) return AddEdgeBatch();
        return Clear();
    }

    private string AddNode()
    {
        var absent = Enumerable.Range(0, _nodeLimit).Where(node => !_oracle.ContainsKey(node)).ToArray();
        if (absent.Length == 0) return "add node skipped";
        var node = absent[_random.Next(absent.Length)];
        Expect("AddNode", _traversing.AddNode(node), _closure.AddNode(node), true);
        _oracle[node] = [];
        _removed.Remove(node);
        return $"add node {node}";
    }

    private string ReAddRemovedNode()
    {
        if (_removed.Count == 0) return AddNode();
        var node = _removed.ElementAt(_random.Next(_removed.Count));
        Expect("AddNode (re-add)", _traversing.AddNode(node), _closure.AddNode(node), true);
        _oracle[node] = [];
        _removed.Remove(node);
        return $"re-add node {node}";
    }

    private string AddAcyclicEdge()
    {
        var (source, target) = RandomForwardPair(allowNewNodes: true);
        var expected = !(_oracle.TryGetValue(source, out var successors) && successors.Contains(target));
        Expect($"AddEdge({source}, {target})", _traversing.AddEdge(source, target), _closure.AddEdge(source, target), expected);
        AddToOracle(source, target);
        return $"add edge {source} -> {target}";
    }

    private string AttemptCycle()
    {
        var nodes = _oracle.Keys.ToArray();
        var source = nodes[_random.Next(nodes.Length)];
        var reachable = OracleDescendants(source).ToArray();
        var (from, to) = reachable.Length == 0 ? (source, source) : (reachable[_random.Next(reachable.Length)], source);
        var edgeCount = _oracle.Values.Sum(set => set.Count);
        var traversingThrew = Catch.Exception(() => _traversing.AddEdge(from, to)) is GraphCycleException;
        var closureThrew = Catch.Exception(() => _closure.AddEdge(from, to)) is GraphCycleException;
        if (!traversingThrew || !closureThrew)
        {
            _discrepancies.Add($"AddEdge({from}, {to}) should throw GraphCycleException: traversing {traversingThrew}, closure {closureThrew}");
        }

        if (_traversing.EdgeCount != edgeCount || _closure.EdgeCount != edgeCount)
        {
            _discrepancies.Add($"a rejected AddEdge({from}, {to}) changed the edge count");
        }

        return $"attempt cycle {from} -> {to}";
    }

    private string RemoveEdge()
    {
        var edges = _oracle.SelectMany(pair => pair.Value.Select(target => (pair.Key, target))).ToArray();
        if (edges.Length == 0) return AddAcyclicEdge();
        var (source, target) = edges[_random.Next(edges.Length)];
        Expect($"RemoveEdge({source}, {target})", _traversing.RemoveEdge(source, target), _closure.RemoveEdge(source, target), true);
        _oracle[source].Remove(target);
        return $"remove edge {source} -> {target}";
    }

    private string RemoveNode()
    {
        var nodes = _oracle.Keys.ToArray();
        var node = nodes[_random.Next(nodes.Length)];
        Expect($"RemoveNode({node})", _traversing.RemoveNode(node), _closure.RemoveNode(node), true);
        _oracle.Remove(node);
        foreach (var successors in _oracle.Values) successors.Remove(node);
        _removed.Add(node);
        return $"remove node {node}";
    }

    private string AddEdgeBatch()
    {
        var batch = new List<Edge<int>>();
        var size = _random.Next(1, 40);
        for (var index = 0; index < size; index++)
        {
            var (source, target) = RandomForwardPair(allowNewNodes: true);
            batch.Add(Edge.Create(source, target));
        }

        var closesCycle = _random.NextDouble() < 0.3 && batch.Count > 1;
        if (closesCycle)
        {
            var first = batch[0];
            batch.Add(first.Reverse());
        }

        var expected = closesCycle ? -1 : batch.Distinct().Count(edge => !(_oracle.TryGetValue(edge.Source, out var set) && set.Contains(edge.Target)));
        var traversingResult = Run(() => _traversing.AddEdges(batch));
        var closureResult = Run(() => _closure.AddEdges(batch));
        if (traversingResult != expected || closureResult != expected)
        {
            _discrepancies.Add($"AddEdges of {batch.Count} edges (cycle: {closesCycle}): expected {expected}, traversing {traversingResult}, closure {closureResult}");
        }

        if (!closesCycle)
        {
            foreach (var edge in batch) AddToOracle(edge.Source, edge.Target);
        }

        return $"add batch of {batch.Count} (cycle: {closesCycle})";

        static int Run(Func<int> addEdges)
        {
            try
            {
                return addEdges();
            }
            catch (GraphCycleException)
            {
                return -1;
            }
        }
    }

    private string Clear()
    {
        _traversing.Clear();
        _closure.Clear();
        foreach (var node in _oracle.Keys) _removed.Add(node);
        _oracle.Clear();
        return "clear";
    }

    private (int Source, int Target) RandomForwardPair(bool allowNewNodes)
    {
        var pool = allowNewNodes && _random.NextDouble() < 0.2
            ? Enumerable.Range(0, _nodeLimit).ToArray()
            : _oracle.Keys.ToArray();
        while (true)
        {
            var first = pool[_random.Next(pool.Length)];
            var second = pool[_random.Next(pool.Length)];
            if (first == second) continue;
            return _ranks[first] < _ranks[second] ? (first, second) : (second, first);
        }
    }

    private void AddToOracle(int source, int target)
    {
        if (!_oracle.ContainsKey(source)) _oracle[source] = [];
        if (!_oracle.ContainsKey(target)) _oracle[target] = [];
        _oracle[source].Add(target);
        _removed.Remove(source);
        _removed.Remove(target);
    }

    private HashSet<int> OracleDescendants(int node)
    {
        var reached = new HashSet<int>();
        var queue = new Queue<int>(_oracle[node]);
        while (queue.Count > 0)
        {
            var next = queue.Dequeue();
            if (reached.Add(next))
            {
                foreach (var successor in _oracle[next]) queue.Enqueue(successor);
            }
        }

        return reached;
    }

    private void Expect<T>(string call, T traversing, T closure, T expected)
    {
        if (!EqualityComparer<T>.Default.Equals(traversing, expected) || !EqualityComparer<T>.Default.Equals(closure, expected))
        {
            _discrepancies.Add($"{call}: expected {expected}, traversing {traversing}, closure {closure}");
        }
    }

    private void Compare(string context)
    {
        var nodes = _oracle.Keys.OrderBy(node => node).ToArray();
        if (_traversing.NodeCount != nodes.Length || _closure.NodeCount != nodes.Length)
        {
            _discrepancies.Add($"{context}: node counts {_traversing.NodeCount}/{_closure.NodeCount}, expected {nodes.Length}");
        }

        var descendants = nodes.ToDictionary(node => node, OracleDescendants);
        foreach (var node in nodes)
        {
            var expectedDescendants = descendants[node];
            var expectedAncestors = nodes.Where(other => descendants[other].Contains(node)).ToHashSet();
            CompareSets(context, $"GetDescendants({node})", expectedDescendants, _traversing.GetDescendants(node), _closure.GetDescendants(node));
            CompareSets(context, $"GetAncestors({node})", expectedAncestors, _traversing.GetAncestors(node), _closure.GetAncestors(node));

            foreach (var other in nodes)
            {
                var hasPath = expectedDescendants.Contains(other);
                if (_traversing.HasPath(node, other) != hasPath || _closure.HasPath(node, other) != hasPath)
                {
                    _discrepancies.Add($"{context}: HasPath({node}, {other}) should be {hasPath}");
                }

                var wouldCreateCycle = node == other || descendants[other].Contains(node);
                if (_traversing.WouldCreateCycle(node, other) != wouldCreateCycle || _closure.WouldCreateCycle(node, other) != wouldCreateCycle)
                {
                    _discrepancies.Add($"{context}: WouldCreateCycle({node}, {other}) should be {wouldCreateCycle}");
                }
            }
        }
    }

    private void CompareSets(string context, string call, HashSet<int> expected, IReadOnlyCollection<int> traversing, IReadOnlyCollection<int> closure)
    {
        if (!expected.SetEquals(traversing) || !expected.SetEquals(closure) || traversing.Count != expected.Count || closure.Count != expected.Count)
        {
            _discrepancies.Add($"{context}: {call} expected [{string.Join(", ", expected.OrderBy(n => n))}], traversing [{string.Join(", ", traversing)}], closure [{string.Join(", ", closure)}]");
        }
    }
}
