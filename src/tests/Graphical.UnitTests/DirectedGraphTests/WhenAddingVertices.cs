using FluentAssertions;

namespace Graphical.UnitTests.DirectedGraphTests;

[TestFixture]
internal class WhenAddingVertices
{
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var graph = new DirectedGraph<int>();
        graph.AddVertex(ONE);
        graph.AddVertex(TWO);
        graph.AddVertex(THREE);
        Vertices.AddRange(graph.GetVertices());
    }

    public WhenAddingVertices()
    {
        Vertices = new List<int>();
    }

    private const int ONE = 1;

    private const int TWO = 2;

    private const int THREE = 3;

    public IList<int> Vertices { get; }

    [Test]
    public void TheVertexShouldBePresent()
    {
        Vertices.Should().Contain(ONE).And.Contain(TWO).And.Contain(THREE);
    }
}
