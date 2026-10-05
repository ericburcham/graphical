using AwesomeAssertions;

namespace Graphical.UnitTests.IGraphTests;

[TestFixture(typeof(UndirectedGraph<>))]
internal sealed class WhenPassingANullEndpoint
{
    private const string NODE = "a";

    private readonly Type _graphType;

    private readonly Dictionary<string, Exception?> _exceptions = [];

    private IGraph<string> _graph = null!;

    public WhenPassingANullEndpoint(Type graphType)
    {
        _graphType = graphType;
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _graph = GraphFactory.Create<string>(_graphType);
        _graph.AddNode(NODE);

        _exceptions["AddEdge source"] = Catch.Exception(() => _graph.AddEdge(null!, NODE));
        _exceptions["AddEdge target"] = Catch.Exception(() => _graph.AddEdge(NODE, null!));
        _exceptions["RemoveEdge source"] = Catch.Exception(() => _graph.RemoveEdge(null!, NODE));
        _exceptions["RemoveEdge target"] = Catch.Exception(() => _graph.RemoveEdge(NODE, null!));
        _exceptions["ContainsEdge source"] = Catch.Exception(() => _graph.ContainsEdge(null!, NODE));
        _exceptions["ContainsEdge target"] = Catch.Exception(() => _graph.ContainsEdge(NODE, null!));
    }

    [TestCase("AddEdge source", "source")]
    [TestCase("AddEdge target", "target")]
    [TestCase("RemoveEdge source", "source")]
    [TestCase("RemoveEdge target", "target")]
    [TestCase("ContainsEdge source", "source")]
    [TestCase("ContainsEdge target", "target")]
    public void TheMemberShouldThrowArgumentNullExceptionNamingTheEndpoint(string call, string parameter)
    {
        _exceptions[call].Should().BeOfType<ArgumentNullException>().Which.ParamName.Should().Be(parameter);
    }

    [Test]
    public void TheGraphShouldBeUnchanged()
    {
        _graph.Nodes.Should().Equal(NODE);
    }
}
