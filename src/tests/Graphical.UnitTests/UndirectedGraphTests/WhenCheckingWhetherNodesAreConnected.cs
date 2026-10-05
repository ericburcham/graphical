using AwesomeAssertions;

namespace Graphical.UnitTests.UndirectedGraphTests;

[TestFixture]
internal sealed class WhenCheckingWhetherNodesAreConnected
{
    private const string MISSING = "missing";

    private UndirectedGraph<string> _graph = null!;

    private Exception? _nullFirstException;

    private Exception? _nullSecondException;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _graph = new UndirectedGraph<string>();
        _graph.AddEdges([Edge.Create("a", "b"), Edge.Create("c", "b"), Edge.Create("d", "e")]);
        _graph.AddNode("isolated");

        _nullFirstException = Catch.Exception(() => _graph.AreConnected(null!, "a"));
        _nullSecondException = Catch.Exception(() => _graph.AreConnected("a", null!));
    }

    [TestCase("a", "c")]
    [TestCase("c", "a")]
    [TestCase("a", "b")]
    [TestCase("d", "e")]
    public void NodesInTheSameComponentShouldBeConnected(string first, string second)
    {
        _graph.AreConnected(first, second).Should().BeTrue();
    }

    [TestCase("a", "d")]
    [TestCase("e", "c")]
    [TestCase("a", "isolated")]
    public void NodesInDifferentComponentsShouldNotBeConnected(string first, string second)
    {
        _graph.AreConnected(first, second).Should().BeFalse();
    }

    [TestCase("isolated")]
    [TestCase("a")]
    public void AnExistingNodeShouldBeConnectedToItself(string node)
    {
        _graph.AreConnected(node, node).Should().BeTrue();
    }

    [TestCase("a", MISSING)]
    [TestCase(MISSING, "a")]
    [TestCase(MISSING, MISSING)]
    public void AMissingNodeShouldNotBeConnected(string first, string second)
    {
        _graph.AreConnected(first, second).Should().BeFalse();
    }

    [Test]
    public void ANullFirstNodeShouldThrowArgumentNullException()
    {
        _nullFirstException.Should().BeOfType<ArgumentNullException>().Which.ParamName.Should().Be("first");
    }

    [Test]
    public void ANullSecondNodeShouldThrowArgumentNullException()
    {
        _nullSecondException.Should().BeOfType<ArgumentNullException>().Which.ParamName.Should().Be("second");
    }
}
