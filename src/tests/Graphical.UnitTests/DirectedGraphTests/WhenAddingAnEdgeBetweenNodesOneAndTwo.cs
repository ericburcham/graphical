using AwesomeAssertions;

namespace Graphical.UnitTests.DirectedGraphTests;

[TestFixture]
internal sealed class WhenAddingAnEdgeBetweenNodesOneAndTwo
{
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var graph = new DirectedGraph<int>();
        graph.AddNode(ONE);
        graph.AddNode(TWO);
        graph.AddEdge(ONE, TWO);

        _nodeOneSuccessors.AddRange(graph.GetSuccessors(ONE));
        _nodeTwoSuccessors.AddRange(graph.GetSuccessors(TWO));
    }

    private readonly IList<int> _nodeOneSuccessors;
    private readonly IList<int> _nodeTwoSuccessors;

    public WhenAddingAnEdgeBetweenNodesOneAndTwo()
    {
        _nodeOneSuccessors = new List<int>();
        _nodeTwoSuccessors = new List<int>();
    }

    private const int ONE = 1;

    private const int TWO = 2;

    [Test]
    public void NodeOneShouldHaveSuccessorTwo()
    {
        _nodeOneSuccessors.Should().Contain(TWO);
    }

    [Test]
    public void NodeTwoShouldHaveNoSuccessors()
    {
        _nodeTwoSuccessors.Should().BeEmpty();
    }
}
