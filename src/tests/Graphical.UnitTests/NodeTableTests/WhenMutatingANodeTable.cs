using AwesomeAssertions;

namespace Graphical.UnitTests.NodeTableTests;

[TestFixture]
internal sealed class WhenMutatingANodeTable
{
    private readonly List<int> _versions = [];

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var table = new NodeTable<string>(0, EqualityComparer<string>.Default);
        _versions.Add(table.Version);
        var slot = table.Add("a");
        _versions.Add(table.Version);
        table.Remove(slot);
        _versions.Add(table.Version);
        table.IncrementVersion();
        _versions.Add(table.Version);
        table.Clear();
        _versions.Add(table.Version);
    }

    [Test]
    public void EveryMutationShouldChangeTheVersion()
    {
        _versions.Should().OnlyHaveUniqueItems();
    }
}
