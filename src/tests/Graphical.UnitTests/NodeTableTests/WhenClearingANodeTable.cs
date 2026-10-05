using AwesomeAssertions;

namespace Graphical.UnitTests.NodeTableTests;

[TestFixture]
internal sealed class WhenClearingANodeTable
{
    private int _count;

    private int _slotLimit;

    private bool _oldNodeFound;

    private bool _oldSlotOccupied;

    private int _slotAfterClear;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var table = new NodeTable<string>(0, EqualityComparer<string>.Default);
        table.Add("a");
        var removed = table.Add("b");
        table.Add("c");
        table.Remove(removed);

        table.Clear();

        _count = table.Count;
        _slotLimit = table.SlotLimit;
        _oldNodeFound = table.TryGetSlot("a", out _);
        _oldSlotOccupied = table.IsOccupied(0);
        _slotAfterClear = table.Add("d");
    }

    [Test]
    public void CountShouldBeZero()
    {
        _count.Should().Be(0);
    }

    [Test]
    public void TheSlotLimitShouldBeZero()
    {
        _slotLimit.Should().Be(0);
    }

    [Test]
    public void OldNodesShouldNotBeFound()
    {
        _oldNodeFound.Should().BeFalse();
    }

    [Test]
    public void OldSlotsShouldBeFree()
    {
        _oldSlotOccupied.Should().BeFalse();
    }

    [Test]
    public void SlotsShouldStartFromZeroAgain()
    {
        _slotAfterClear.Should().Be(0);
    }
}
