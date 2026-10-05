using AwesomeAssertions;

namespace Graphical.UnitTests.IGraphTests;

[TestFixture(typeof(UndirectedGraph<>))]
[TestFixture(typeof(DirectedGraph<>))]
[TestFixture(typeof(DirectedAcyclicGraph<>))]
internal sealed class WhenQueryingAMissingNode
{
    private const string PRESENT = "a";

    private const string MISSING = "z";

    private readonly Type _graphType;

    private readonly Dictionary<string, Exception?> _exceptions = [];

    private IGraph<string> _graph = null!;

    public WhenQueryingAMissingNode(Type graphType)
    {
        _graphType = graphType;
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _graph = GraphFactory.Create<string>(_graphType);
        _graph.AddNode(PRESENT);

        _exceptions[nameof(_graph.GetNeighbors)] = Catch.Exception(() => _graph.GetNeighbors(MISSING));
        _exceptions[nameof(_graph.GetDegree)] = Catch.Exception(() => _graph.GetDegree(MISSING));
    }

    [Test]
    public void ContainsNodeShouldBeFalse()
    {
        _graph.ContainsNode(MISSING).Should().BeFalse();
    }

    [Test]
    public void ContainsEdgeFromTheMissingNodeShouldBeFalse()
    {
        _graph.ContainsEdge(MISSING, PRESENT).Should().BeFalse();
    }

    [Test]
    public void ContainsEdgeToTheMissingNodeShouldBeFalse()
    {
        _graph.ContainsEdge(PRESENT, MISSING).Should().BeFalse();
    }

    [TestCase(nameof(IGraph<string>.GetNeighbors))]
    [TestCase(nameof(IGraph<string>.GetDegree))]
    public void TheMemberShouldThrowKeyNotFoundException(string member)
    {
        _exceptions[member].Should().BeOfType<KeyNotFoundException>();
    }
}
