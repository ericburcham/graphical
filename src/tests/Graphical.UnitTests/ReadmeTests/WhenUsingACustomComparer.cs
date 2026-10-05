using AwesomeAssertions;

namespace Graphical.UnitTests.ReadmeTests;

// Mirrors the README "Nodes" sample.
[TestFixture]
internal sealed class WhenUsingACustomComparer
{
    private bool _containsDifferentCase;

    private bool _addedSameEdge;

    private IReadOnlyCollection<string> _nodes = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var tags = new UndirectedGraph<string>(StringComparer.OrdinalIgnoreCase);
        tags.AddEdge("CSharp", "dotnet");

        _containsDifferentCase = tags.ContainsNode("csharp");   // true
        _addedSameEdge = tags.AddEdge("DOTNET", "csharp");      // false: it is the same edge
        _nodes = tags.Nodes;                                    // CSharp, dotnet (first spelling wins)
    }

    [Test]
    public void LookupShouldIgnoreCase()
    {
        _containsDifferentCase.Should().BeTrue();
    }

    [Test]
    public void TheSameEdgeInADifferentCaseShouldBeADuplicate()
    {
        _addedSameEdge.Should().BeFalse();
    }

    [Test]
    public void TheFirstSpellingShouldBeKept()
    {
        _nodes.Should().Equal("CSharp", "dotnet");
    }
}
