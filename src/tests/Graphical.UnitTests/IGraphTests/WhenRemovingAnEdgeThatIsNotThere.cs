using AwesomeAssertions;

namespace Graphical.UnitTests.IGraphTests;

[TestFixture(typeof(UndirectedGraph<>))]
internal sealed class WhenRemovingAnEdgeThatIsNotThere
{
    private const string FIRST = "a";

    private const string SECOND = "b";

    private const string MISSING = "z";

    private readonly Type _graphType;

    private IGraph<string> _graph = null!;

    private bool _removedUnconnected;

    private bool _removedFromMissing;

    private bool _removedToMissing;

    public WhenRemovingAnEdgeThatIsNotThere(Type graphType)
    {
        _graphType = graphType;
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _graph = GraphFactory.Create<string>(_graphType);
        _graph.AddNode(FIRST);
        _graph.AddNode(SECOND);

        _removedUnconnected = _graph.RemoveEdge(FIRST, SECOND);
        _removedFromMissing = _graph.RemoveEdge(MISSING, FIRST);
        _removedToMissing = _graph.RemoveEdge(FIRST, MISSING);
    }

    [Test]
    public void RemovingBetweenUnconnectedNodesShouldReturnFalse()
    {
        _removedUnconnected.Should().BeFalse();
    }

    [Test]
    public void RemovingFromAMissingNodeShouldReturnFalse()
    {
        _removedFromMissing.Should().BeFalse();
    }

    [Test]
    public void RemovingToAMissingNodeShouldReturnFalse()
    {
        _removedToMissing.Should().BeFalse();
    }

    [Test]
    public void TheGraphShouldBeUnchanged()
    {
        _graph.Nodes.Should().Equal(FIRST, SECOND);
    }
}
