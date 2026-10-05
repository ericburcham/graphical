using AwesomeAssertions;

namespace Graphical.UnitTests.ReachabilityDirectedAcyclicGraphTests;

[TestFixture]
internal sealed class WhenTheGraphIsALongChain
{
    private const int LENGTH = 5_000;

    private const int MIDDLE = LENGTH / 2;

    private bool _hasPathEndToEnd;

    private bool _closingTheChainWouldCreateACycle;

    private bool _orderIsTheChain;

    private int _descendantsOfTheHead;

    private bool _hasPathAcrossTheCut;

    private int _ancestorsOfTheTailAfterTheCut;

    private bool _batchBuiltChainHasPathEndToEnd;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var graph = new ReachabilityDirectedAcyclicGraph<int>();
        for (var node = 1; node < LENGTH; node++) graph.AddEdge(node - 1, node);

        _hasPathEndToEnd = graph.HasPath(0, LENGTH - 1);
        _closingTheChainWouldCreateACycle = graph.WouldCreateCycle(LENGTH - 1, 0);
        _orderIsTheChain = graph.GetTopologicalOrder().SequenceEqual(Enumerable.Range(0, LENGTH));
        _descendantsOfTheHead = graph.GetDescendants(0).Count;

        graph.RemoveEdge(MIDDLE - 1, MIDDLE);
        _hasPathAcrossTheCut = graph.HasPath(0, LENGTH - 1);
        _ancestorsOfTheTailAfterTheCut = graph.GetAncestors(LENGTH - 1).Count;

        var batchBuilt = new ReachabilityDirectedAcyclicGraph<int>();
        batchBuilt.AddEdges(Enumerable.Range(1, LENGTH - 1).Select(node => Edge.Create(node - 1, node)));
        _batchBuiltChainHasPathEndToEnd = batchBuilt.HasPath(0, LENGTH - 1);
    }

    [Test]
    public void HasPathShouldFindTheFarEnd()
    {
        _hasPathEndToEnd.Should().BeTrue();
    }

    [Test]
    public void ClosingTheChainShouldBeDetectedAsACycle()
    {
        _closingTheChainWouldCreateACycle.Should().BeTrue();
    }

    [Test]
    public void TheTopologicalOrderShouldFollowTheChain()
    {
        _orderIsTheChain.Should().BeTrue();
    }

    [Test]
    public void TheHeadShouldReachEveryOtherNode()
    {
        _descendantsOfTheHead.Should().Be(LENGTH - 1);
    }

    [Test]
    public void CuttingTheChainShouldRemoveTheEndToEndPath()
    {
        _hasPathAcrossTheCut.Should().BeFalse();
    }

    [Test]
    public void CuttingTheChainShouldLeaveTheTailOnlyItsOwnHalfAsAncestors()
    {
        _ancestorsOfTheTailAfterTheCut.Should().Be(LENGTH - MIDDLE - 1);
    }

    [Test]
    public void AChainAddedInOneBatchShouldBeFullyReachable()
    {
        _batchBuiltChainHasPathEndToEnd.Should().BeTrue();
    }
}
