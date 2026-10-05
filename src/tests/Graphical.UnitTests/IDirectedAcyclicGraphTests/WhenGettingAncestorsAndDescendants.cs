using AwesomeAssertions;

namespace Graphical.UnitTests.IDirectedAcyclicGraphTests;

[TestFixture(typeof(DirectedAcyclicGraph<>))]
[TestFixture(typeof(ReachabilityDirectedAcyclicGraph<>))]
internal sealed class WhenGettingAncestorsAndDescendants
{
    private readonly Type _graphType;

    private readonly Dictionary<string, Exception?> _exceptions = [];

    private IDirectedAcyclicGraph<string> _graph = null!;

    private IReadOnlyCollection<string> _descendantsBeforeChange = null!;

    public WhenGettingAncestorsAndDescendants(Type graphType)
    {
        _graphType = graphType;
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _graph = GraphFactory.CreateAcyclic<string>(_graphType);
        _graph.AddEdges([Edge.Create("a", "b"), Edge.Create("b", "c"), Edge.Create("x", "c"), Edge.Create("c", "d")]);
        _graph.AddNode("isolated");

        _descendantsBeforeChange = _graph.GetDescendants("c");
        _graph.AddEdge("d", "e");

        _exceptions["missing ancestors"] = Catch.Exception(() => _graph.GetAncestors("missing"));
        _exceptions["missing descendants"] = Catch.Exception(() => _graph.GetDescendants("missing"));
        _exceptions["null ancestors"] = Catch.Exception(() => _graph.GetAncestors(null!));
        _exceptions["null descendants"] = Catch.Exception(() => _graph.GetDescendants(null!));
    }

    [Test]
    public void DescendantsShouldBeEveryNodeReachableFromTheNode()
    {
        _graph.GetDescendants("a").Should().BeEquivalentTo(["b", "c", "d", "e"]);
    }

    [Test]
    public void AncestorsShouldBeEveryNodeThatReachesTheNode()
    {
        _graph.GetAncestors("c").Should().BeEquivalentTo(["a", "b", "x"]);
    }

    [Test]
    public void ASourceShouldHaveNoAncestors()
    {
        _graph.GetAncestors("a").Should().BeEmpty();
    }

    [Test]
    public void ASinkShouldHaveNoDescendants()
    {
        _graph.GetDescendants("e").Should().BeEmpty();
    }

    [Test]
    public void AnIsolatedNodeShouldHaveNoDescendants()
    {
        _graph.GetDescendants("isolated").Should().BeEmpty();
    }

    [Test]
    public void DescendantsShouldBeASnapshot()
    {
        _descendantsBeforeChange.Should().BeEquivalentTo(["d"]);
    }

    [TestCase("missing ancestors")]
    [TestCase("missing descendants")]
    public void AMissingNodeShouldThrowKeyNotFoundException(string call)
    {
        _exceptions[call].Should().BeOfType<KeyNotFoundException>();
    }

    [TestCase("null ancestors")]
    [TestCase("null descendants")]
    public void ANullNodeShouldThrowArgumentNullExceptionNamingTheNode(string call)
    {
        _exceptions[call].Should().BeOfType<ArgumentNullException>().Which.ParamName.Should().Be("node");
    }
}
