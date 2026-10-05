using AwesomeAssertions;

namespace Graphical.UnitTests.NodeTableTests;

[TestFixture]
internal sealed class WhenLookingUpNodesWithACustomComparer
{
    private bool _foundWithDifferentCase;

    private IEqualityComparer<string>? _comparer;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var table = new NodeTable<string>(0, StringComparer.OrdinalIgnoreCase);
        table.Add("Node");

        _foundWithDifferentCase = table.TryGetSlot("NODE", out _);
        _comparer = table.Comparer;
    }

    [Test]
    public void LookupShouldUseTheComparer()
    {
        _foundWithDifferentCase.Should().BeTrue();
    }

    [Test]
    public void TheComparerShouldBeExposed()
    {
        _comparer.Should().BeSameAs(StringComparer.OrdinalIgnoreCase);
    }
}
