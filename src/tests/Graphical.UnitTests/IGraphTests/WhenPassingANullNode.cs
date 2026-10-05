using AwesomeAssertions;

namespace Graphical.UnitTests.IGraphTests;

[TestFixture(typeof(UndirectedGraph<>))]
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
    }

    [TestCase(nameof(IGraph<string>.AddNode))]
    [TestCase(nameof(IGraph<string>.ContainsNode))]
    public void TheMemberShouldThrowArgumentNullExceptionNamingTheNode(string member)
    {
        _exceptions[member].Should().BeOfType<ArgumentNullException>().Which.ParamName.Should().Be("node");
    }
}
