using AwesomeAssertions;

namespace Graphical.UnitTests.IDirectedGraphTests;

[TestFixture(typeof(DirectedGraph<>))]
internal sealed class WhenCheckingForPaths
{
    private const string MISSING = "missing";

    private readonly Type _graphType;

    private IDirectedGraph<string> _graph = null!;

    private Exception? _nullSourceException;

    private Exception? _nullTargetException;

    public WhenCheckingForPaths(Type graphType)
    {
        _graphType = graphType;
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _graph = GraphFactory.CreateDirected<string>(_graphType);
        _graph.AddEdges([Edge.Create("a", "b"), Edge.Create("b", "c"), Edge.Create("c", "d"), Edge.Create("x", "c")]);
        _graph.AddNode("isolated");

        _nullSourceException = Catch.Exception(() => _graph.HasPath(null!, "a"));
        _nullTargetException = Catch.Exception(() => _graph.HasPath("a", null!));
    }

    [TestCase("a", "b")]
    [TestCase("a", "d")]
    [TestCase("x", "d")]
    public void ReachableTargetsShouldHaveAPath(string source, string target)
    {
        _graph.HasPath(source, target).Should().BeTrue();
    }

    [TestCase("d", "a")]
    [TestCase("b", "x")]
    [TestCase("a", "isolated")]
    [TestCase("a", "x")]
    public void UnreachableTargetsShouldHaveNoPath(string source, string target)
    {
        _graph.HasPath(source, target).Should().BeFalse();
    }

    [TestCase("a")]
    [TestCase("isolated")]
    public void ANodeWithoutACycleShouldHaveNoPathToItself(string node)
    {
        _graph.HasPath(node, node).Should().BeFalse();
    }

    [TestCase("a", MISSING)]
    [TestCase(MISSING, "a")]
    [TestCase(MISSING, MISSING)]
    public void AMissingNodeShouldHaveNoPath(string source, string target)
    {
        _graph.HasPath(source, target).Should().BeFalse();
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
