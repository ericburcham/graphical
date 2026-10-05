using AwesomeAssertions;

namespace Graphical.UnitTests.IDirectedAcyclicGraphTests;

[TestFixture(typeof(DirectedAcyclicGraph<>))]
[TestFixture(typeof(ReachabilityDirectedAcyclicGraph<>))]
internal sealed class WhenCheckingWhetherAnEdgeWouldCreateACycle
{
    private const string MISSING = "missing";

    private readonly Type _graphType;

    private IDirectedAcyclicGraph<string> _graph = null!;

    private Exception? _nullSourceException;

    private Exception? _nullTargetException;

    public WhenCheckingWhetherAnEdgeWouldCreateACycle(Type graphType)
    {
        _graphType = graphType;
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _graph = GraphFactory.CreateAcyclic<string>(_graphType);
        _graph.AddEdges([Edge.Create("a", "b"), Edge.Create("b", "c"), Edge.Create("x", "c")]);

        _nullSourceException = Catch.Exception(() => _graph.WouldCreateCycle(null!, "a"));
        _nullTargetException = Catch.Exception(() => _graph.WouldCreateCycle("a", null!));
    }

    [TestCase("b", "a")]
    [TestCase("c", "a")]
    [TestCase("a", "a")]
    [TestCase(MISSING, MISSING)]
    public void AnEdgeClosingALoopShouldCreateACycle(string source, string target)
    {
        _graph.WouldCreateCycle(source, target).Should().BeTrue();
    }

    [TestCase("a", "c")]
    [TestCase("a", "x")]
    [TestCase("x", "a")]
    [TestCase("c", MISSING)]
    [TestCase(MISSING, "a")]
    public void AnEdgeThatClosesNoLoopShouldNotCreateACycle(string source, string target)
    {
        _graph.WouldCreateCycle(source, target).Should().BeFalse();
    }

    [Test]
    public void ANullSourceShouldThrowArgumentNullException()
    {
        _nullSourceException.Should().BeOfType<ArgumentNullException>().Which.ParamName.Should().Be("source");
    }

    [Test]
    public void ANullTargetShouldThrowArgumentNullException()
    {
        _nullTargetException.Should().BeOfType<ArgumentNullException>().Which.ParamName.Should().Be("target");
    }
}
