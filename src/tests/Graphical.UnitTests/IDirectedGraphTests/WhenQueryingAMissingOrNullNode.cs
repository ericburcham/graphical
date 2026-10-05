using AwesomeAssertions;

namespace Graphical.UnitTests.IDirectedGraphTests;

[TestFixture(typeof(DirectedGraph<>))]
[TestFixture(typeof(DirectedAcyclicGraph<>))]
internal sealed class WhenQueryingAMissingOrNullNode
{
    private const string MISSING = "missing";

    private readonly Type _graphType;

    private readonly Dictionary<string, Exception?> _missing = [];

    private readonly Dictionary<string, Exception?> _null = [];

    public WhenQueryingAMissingOrNullNode(Type graphType)
    {
        _graphType = graphType;
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var graph = GraphFactory.CreateDirected<string>(_graphType);
        graph.AddEdge("a", "b");

        _missing[nameof(graph.GetSuccessors)] = Catch.Exception(() => graph.GetSuccessors(MISSING));
        _missing[nameof(graph.GetPredecessors)] = Catch.Exception(() => graph.GetPredecessors(MISSING));
        _missing[nameof(graph.GetInDegree)] = Catch.Exception(() => graph.GetInDegree(MISSING));
        _missing[nameof(graph.GetOutDegree)] = Catch.Exception(() => graph.GetOutDegree(MISSING));

        _null[nameof(graph.GetSuccessors)] = Catch.Exception(() => graph.GetSuccessors(null!));
        _null[nameof(graph.GetPredecessors)] = Catch.Exception(() => graph.GetPredecessors(null!));
        _null[nameof(graph.GetInDegree)] = Catch.Exception(() => graph.GetInDegree(null!));
        _null[nameof(graph.GetOutDegree)] = Catch.Exception(() => graph.GetOutDegree(null!));
    }

    [TestCase(nameof(IDirectedGraph<string>.GetSuccessors))]
    [TestCase(nameof(IDirectedGraph<string>.GetPredecessors))]
    [TestCase(nameof(IDirectedGraph<string>.GetInDegree))]
    [TestCase(nameof(IDirectedGraph<string>.GetOutDegree))]
    public void AMissingNodeShouldThrowKeyNotFoundException(string member)
    {
        _missing[member].Should().BeOfType<KeyNotFoundException>();
    }

    [TestCase(nameof(IDirectedGraph<string>.GetSuccessors))]
    [TestCase(nameof(IDirectedGraph<string>.GetPredecessors))]
    [TestCase(nameof(IDirectedGraph<string>.GetInDegree))]
    [TestCase(nameof(IDirectedGraph<string>.GetOutDegree))]
    public void ANullNodeShouldThrowArgumentNullExceptionNamingTheNode(string member)
    {
        _null[member].Should().BeOfType<ArgumentNullException>().Which.ParamName.Should().Be("node");
    }
}
