using AwesomeAssertions;

namespace Graphical.UnitTests.IDirectedAcyclicGraphTests;

[TestFixture(typeof(DirectedAcyclicGraph<>))]
internal sealed class WhenGettingTheTopologicalOrder
{
    private readonly Type _graphType;

    private readonly List<string> _violations = [];

    private IReadOnlyList<string> _order = null!;

    private IReadOnlyList<string> _repeatedOrder = null!;

    private IReadOnlyList<string> _orderFromAnIdenticalGraph = null!;

    private IReadOnlyList<string> _orderAfterChange = null!;

    private Exception? _modificationException;

    public WhenGettingTheTopologicalOrder(Type graphType)
    {
        _graphType = graphType;
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var graph = Build();
        _order = graph.GetTopologicalOrder();
        _repeatedOrder = graph.GetTopologicalOrder();
        _orderFromAnIdenticalGraph = Build().GetTopologicalOrder();

        foreach (var edge in graph.Edges)
        {
            if (IndexOf(_order, edge.Source) > IndexOf(_order, edge.Target)) _violations.Add(edge.ToString());
        }

        _modificationException = Catch.Exception(() => ((IList<string>)_order).Add("x"));

        graph.AddEdge("f", "e");
        _orderAfterChange = graph.GetTopologicalOrder();
    }

    [Test]
    public void TiesShouldBeBrokenByInsertionOrder()
    {
        _order.Should().Equal("e", "b", "a", "c", "d");
    }

    [Test]
    public void EveryEdgeShouldPointForwardInTheOrder()
    {
        _violations.Should().BeEmpty();
    }

    [Test]
    public void IdenticalGraphsShouldProduceIdenticalOrders()
    {
        _orderFromAnIdenticalGraph.Should().Equal(_order);
    }

    [Test]
    public void AnUnchangedGraphShouldReturnTheCachedOrder()
    {
        _repeatedOrder.Should().BeSameAs(_order);
    }

    [Test]
    public void TheOrderShouldBeReadOnly()
    {
        _modificationException.Should().BeOfType<NotSupportedException>();
    }

    [Test]
    public void AChangeShouldProduceAFreshOrder()
    {
        _orderAfterChange.Should().Equal("b", "a", "c", "d", "f", "e");
    }

    private IDirectedAcyclicGraph<string> Build()
    {
        var graph = GraphFactory.CreateAcyclic<string>(_graphType);
        graph.AddNodes(["e", "d", "c", "b", "a"]);
        graph.AddEdges([Edge.Create("a", "c"), Edge.Create("b", "c"), Edge.Create("c", "d")]);
        return graph;
    }

    private static int IndexOf(IReadOnlyList<string> order, string node)
    {
        for (var index = 0; index < order.Count; index++)
        {
            if (order[index] == node) return index;
        }

        return -1;
    }
}
