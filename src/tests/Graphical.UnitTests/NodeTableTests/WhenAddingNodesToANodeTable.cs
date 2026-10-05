using AwesomeAssertions;

namespace Graphical.UnitTests.NodeTableTests;

[TestFixture]
internal sealed class WhenAddingNodesToANodeTable
{
    private const string FIRST = "a";

    private const string SECOND = "b";

    private const string MISSING = "z";

    private readonly List<int> _slots = [];

    private readonly List<string> _nodesBySlot = [];

    private readonly List<int> _foundSlots = [];

    private bool _foundMissing;

    private int _count;

    private long _firstSequence;

    private long _secondSequence;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var table = new NodeTable<string>(0, EqualityComparer<string>.Default);
        _slots.Add(table.Add(FIRST));
        _slots.Add(table.Add(SECOND));

        foreach (var slot in _slots) _nodesBySlot.Add(table[slot]);
        foreach (var node in new[] { FIRST, SECOND })
        {
            table.TryGetSlot(node, out var slot);
            _foundSlots.Add(slot);
        }

        _foundMissing = table.TryGetSlot(MISSING, out _);
        _count = table.Count;
        _firstSequence = table.GetSequence(_slots[0]);
        _secondSequence = table.GetSequence(_slots[1]);
    }

    [Test]
    public void SlotsShouldBeDenseFromZero()
    {
        _slots.Should().Equal(0, 1);
    }

    [Test]
    public void EachSlotShouldHoldItsNode()
    {
        _nodesBySlot.Should().Equal(FIRST, SECOND);
    }

    [Test]
    public void EachNodeShouldMapBackToItsSlot()
    {
        _foundSlots.Should().Equal(_slots);
    }

    [Test]
    public void AMissingNodeShouldNotBeFound()
    {
        _foundMissing.Should().BeFalse();
    }

    [Test]
    public void CountShouldBeTheNumberOfNodes()
    {
        _count.Should().Be(2);
    }

    [Test]
    public void SequencesShouldIncreaseInInsertionOrder()
    {
        _secondSequence.Should().BeGreaterThan(_firstSequence);
    }
}
