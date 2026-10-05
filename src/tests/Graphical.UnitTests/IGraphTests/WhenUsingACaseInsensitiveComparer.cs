using AwesomeAssertions;

namespace Graphical.UnitTests.IGraphTests;

[TestFixture(typeof(UndirectedGraph<>))]
[TestFixture(typeof(DirectedGraph<>))]
[TestFixture(typeof(DirectedAcyclicGraph<>))]
internal sealed class WhenUsingACaseInsensitiveComparer
{
    private readonly Type _graphType;

    private IGraph<string> _graph = null!;

    private bool _addedDifferentCaseNode;

    private bool _addedDifferentCaseEdge;

    private bool _removedDifferentCaseEdge;

    public WhenUsingACaseInsensitiveComparer(Type graphType)
    {
        _graphType = graphType;
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _graph = GraphFactory.Create<string>(_graphType, StringComparer.OrdinalIgnoreCase);
        _graph.AddEdge("Alpha", "Beta");
        _graph.AddEdge("beta", "gamma");
        _addedDifferentCaseNode = _graph.AddNode("ALPHA");
        _addedDifferentCaseEdge = _graph.AddEdge("alpha", "BETA");
        _removedDifferentCaseEdge = _graph.RemoveEdge("BETA", "GAMMA");
    }

    [Test]
    public void ANodeDifferingOnlyInCaseShouldBeADuplicate()
    {
        _addedDifferentCaseNode.Should().BeFalse();
    }

    [Test]
    public void AnEdgeDifferingOnlyInCaseShouldBeADuplicate()
    {
        _addedDifferentCaseEdge.Should().BeFalse();
    }

    [Test]
    public void RemovingAnEdgeShouldMatchIgnoringCase()
    {
        _removedDifferentCaseEdge.Should().BeTrue();
    }

    [Test]
    public void ContainsNodeShouldMatchIgnoringCase()
    {
        _graph.ContainsNode("GAMMA").Should().BeTrue();
    }

    [Test]
    public void NodesShouldKeepTheFirstSpelling()
    {
        _graph.Nodes.Should().Equal("Alpha", "Beta", "gamma");
    }

    [Test]
    public void GetDegreeShouldMatchIgnoringCase()
    {
        _graph.GetDegree("aLpHa").Should().Be(1);
    }
}
