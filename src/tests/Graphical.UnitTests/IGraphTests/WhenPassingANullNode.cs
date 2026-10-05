using AwesomeAssertions;

namespace Graphical.UnitTests.IGraphTests;

[TestFixture(typeof(UndirectedGraph<>))]
[TestFixture(typeof(DirectedGraph<>))]
[TestFixture(typeof(DirectedAcyclicGraph<>))]
internal sealed class WhenPassingANullNode
{
    private readonly Type _graphType;

    private readonly Dictionary<string, Exception?> _exceptions = [];

    public WhenPassingANullNode(Type graphType)
    {
        _graphType = graphType;
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var graph = GraphFactory.Create<string>(_graphType);
        graph.AddNode("a");

        _exceptions[nameof(graph.AddNode)] = Catch.Exception(() => graph.AddNode(null!));
        _exceptions[nameof(graph.ContainsNode)] = Catch.Exception(() => graph.ContainsNode(null!));
        _exceptions[nameof(graph.GetNeighbors)] = Catch.Exception(() => graph.GetNeighbors(null!));
        _exceptions[nameof(graph.RemoveNode)] = Catch.Exception(() => graph.RemoveNode(null!));
        _exceptions[nameof(graph.GetDegree)] = Catch.Exception(() => graph.GetDegree(null!));
    }

    [TestCase(nameof(IGraph<string>.AddNode))]
    [TestCase(nameof(IGraph<string>.ContainsNode))]
    [TestCase(nameof(IGraph<string>.GetNeighbors))]
    [TestCase(nameof(IGraph<string>.GetDegree))]
    [TestCase(nameof(IGraph<string>.RemoveNode))]
    public void TheMemberShouldThrowArgumentNullExceptionNamingTheNode(string member)
    {
        _exceptions[member].Should().BeOfType<ArgumentNullException>().Which.ParamName.Should().Be("node");
    }
}
