using AwesomeAssertions;

namespace Graphical.UnitTests.TypeRelationshipTests;

[TestFixture]
internal sealed class WhenComparingGraphTypes
{
    [Test]
    public void ADirectedGraphShouldNotBeAnUndirectedGraph()
    {
        typeof(DirectedGraph<int>).Should().NotBeAssignableTo<IUndirectedGraph<int>>();
    }

    [Test]
    public void AnUndirectedGraphShouldNotBeADirectedGraph()
    {
        typeof(UndirectedGraph<int>).Should().NotBeAssignableTo<IDirectedGraph<int>>();
    }

    [Test]
    public void ADirectedGraphShouldBeAGraph()
    {
        typeof(DirectedGraph<int>).Should().BeAssignableTo<IGraph<int>>();
    }

    [Test]
    public void AnUndirectedGraphShouldBeAGraph()
    {
        typeof(UndirectedGraph<int>).Should().BeAssignableTo<IGraph<int>>();
    }

    [Test]
    public void ADirectedGraphShouldBeAReadOnlyDirectedGraph()
    {
        typeof(DirectedGraph<int>).Should().BeAssignableTo<IReadOnlyDirectedGraph<int>>();
    }

    [Test]
    public void AnUndirectedGraphShouldBeAReadOnlyUndirectedGraph()
    {
        typeof(UndirectedGraph<int>).Should().BeAssignableTo<IReadOnlyUndirectedGraph<int>>();
    }
}
