using AwesomeAssertions;

namespace Graphical.UnitTests.DirectedAcyclicGraphTests;

[TestFixture]
internal sealed class WhenTheGraphIsAVeryLongChain
{
    private const int LENGTH = 100_000;

    private bool _hasPathEndToEnd;

    private bool _hasPathBackwards;

    private bool _orderIsTheChain;

    private int _descendantsOfTheHead;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var graph = new DirectedAcyclicGraph<int>();
        for (var node = 1; node < LENGTH; node++) graph.AddEdge(node - 1, node);

        _hasPathEndToEnd = graph.HasPath(0, LENGTH - 1);
        _hasPathBackwards = graph.WouldCreateCycle(LENGTH - 1, 0);
        _orderIsTheChain = graph.GetTopologicalOrder().SequenceEqual(Enumerable.Range(0, LENGTH));
        _descendantsOfTheHead = graph.GetDescendants(0).Count;
    }

    [Test]
    public void HasPathShouldFindTheFarEnd()
    {
        _hasPathEndToEnd.Should().BeTrue();
    }

    [Test]
    public void ClosingTheChainShouldBeDetectedAsACycle()
    {
        _hasPathBackwards.Should().BeTrue();
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
}
