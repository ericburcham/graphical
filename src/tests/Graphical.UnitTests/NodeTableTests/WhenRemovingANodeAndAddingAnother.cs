using AwesomeAssertions;

namespace Graphical.UnitTests.NodeTableTests;

[TestFixture]
internal sealed class WhenRemovingANodeAndAddingAnother
{
    private const string KEPT = "a";

    private const string REMOVED = "b";

    private const string REPLACEMENT = "c";

    private int _removedSlot;

    private int _replacementSlot;

    private bool _removedFound;

    private bool _removedSlotOccupiedBeforeReuse;

    private long _removedSequence;

    private long _replacementSequence;

    private int _count;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var table = new NodeTable<string>(0, EqualityComparer<string>.Default);
        table.Add(KEPT);
        _removedSlot = table.Add(REMOVED);
        _removedSequence = table.GetSequence(_removedSlot);

        table.Remove(_removedSlot);
        _removedSlotOccupiedBeforeReuse = table.IsOccupied(_removedSlot);
        _removedFound = table.TryGetSlot(REMOVED, out _);

        _replacementSlot = table.Add(REPLACEMENT);
        _replacementSequence = table.GetSequence(_replacementSlot);
        _count = table.Count;
    }

    [Test]
    public void TheRemovedNodeShouldNotBeFound()
    {
        _removedFound.Should().BeFalse();
    }

    [Test]
    public void TheRemovedSlotShouldBeFree()
    {
        _removedSlotOccupiedBeforeReuse.Should().BeFalse();
    }

    [Test]
    public void TheNextNodeShouldReuseTheFreedSlot()
    {
        _replacementSlot.Should().Be(_removedSlot);
    }

    [Test]
    public void TheReusedSlotShouldGetANewerSequence()
    {
        _replacementSequence.Should().BeGreaterThan(_removedSequence);
    }

    [Test]
    public void CountShouldIncludeOnlyPresentNodes()
    {
        _count.Should().Be(2);
    }
}
