using AwesomeAssertions;

namespace Graphical.UnitTests.IDirectedGraphTests;

[TestFixture(typeof(DirectedGraph<>))]
[TestFixture(typeof(DirectedAcyclicGraph<>))]
[TestFixture(typeof(ReachabilityDirectedAcyclicGraph<>))]
internal sealed class WhenMutatingTheGraphDuringDirectedEnumeration
{
    private readonly Type _graphType;

    private Exception? _successorsException;

    private Exception? _predecessorsException;

    private IReadOnlyCollection<string> _liveSuccessors = null!;

    public WhenMutatingTheGraphDuringDirectedEnumeration(Type graphType)
    {
        _graphType = graphType;
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var graph = GraphFactory.CreateDirected<string>(_graphType);
        graph.AddEdges([Edge.Create("a", "b"), Edge.Create("a", "c"), Edge.Create("b", "c")]);

        using (var successors = graph.GetSuccessors("a").GetEnumerator())
        {
            successors.MoveNext();
            graph.AddNode("x");
            _successorsException = Catch.Exception(() => successors.MoveNext());
        }

        using (var predecessors = graph.GetPredecessors("c").GetEnumerator())
        {
            predecessors.MoveNext();
            graph.RemoveEdge("b", "c");
            _predecessorsException = Catch.Exception(() => predecessors.MoveNext());
        }

        _liveSuccessors = graph.GetSuccessors("b");
        graph.AddEdge("b", "y");
    }

    [Test]
    public void ContinuingSuccessorsShouldThrowInvalidOperationException()
    {
        _successorsException.Should().BeOfType<InvalidOperationException>();
    }

    [Test]
    public void ContinuingPredecessorsShouldThrowInvalidOperationException()
    {
        _predecessorsException.Should().BeOfType<InvalidOperationException>();
    }

    [Test]
    public void ASuccessorsViewShouldShowLaterEdges()
    {
        _liveSuccessors.Should().Equal("y");
    }
}
