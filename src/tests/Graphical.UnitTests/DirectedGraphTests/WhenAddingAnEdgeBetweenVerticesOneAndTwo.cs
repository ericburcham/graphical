using FluentAssertions;

namespace Graphical.UnitTests.DirectedGraphTests;

[TestFixture]
internal class WhenAddingAnEdgeBetweenVerticesOneAndTwo
{
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var graph = new DirectedGraph<int>();
        graph.AddVertex(ONE);
        graph.AddVertex(TWO);
        graph.AddEdge(ONE, TWO);

        _vertexOneNeighbors.AddRange(graph.GetNeighbors(ONE));
        _vertexTwoNeighbors.AddRange(graph.GetNeighbors(TWO));
    }

    private readonly IList<int> _vertexOneNeighbors;
    private readonly IList<int> _vertexTwoNeighbors;

    public WhenAddingAnEdgeBetweenVerticesOneAndTwo()
    {
        _vertexOneNeighbors = new List<int>();
        _vertexTwoNeighbors = new List<int>();
    }

    private const int ONE = 1;

    private const int TWO = 2;

    [Test]
    public void VertexOneShouldHaveNeighborTwo()
    {
        _vertexOneNeighbors.Should().Contain(TWO);
    }
}