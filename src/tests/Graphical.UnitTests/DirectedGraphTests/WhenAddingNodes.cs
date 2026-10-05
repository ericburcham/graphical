using AwesomeAssertions;

namespace Graphical.UnitTests.DirectedGraphTests;

[TestFixture]
internal sealed class WhenAddingNodes
{
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var graph = new DirectedGraph<int>();
        graph.AddNode(ONE);
        graph.AddNode(TWO);
        graph.AddNode(THREE);
        Nodes.AddRange(graph.Nodes);
    }

    public WhenAddingNodes()
    {
        Nodes = new List<int>();
    }

    private const int ONE = 1;

    private const int TWO = 2;

    private const int THREE = 3;

    public IList<int> Nodes { get; }

    [Test]
    public void TheNodeShouldBePresent()
    {
        Nodes.Should().Contain(ONE).And.Contain(TWO).And.Contain(THREE);
    }
}
