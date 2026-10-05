using AwesomeAssertions;

namespace Graphical.UnitTests.NodeTableTests;

[TestFixture]
internal sealed class WhenAddingMoreNodesThanTheCapacity
{
    private const int INITIAL_CAPACITY = 2;

    private const int NODE_COUNT = 100;

    private int _initialCapacity;

    private int _finalCapacity;

    private int _slotLimit;

    private readonly List<int> _misplaced = [];

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var table = new NodeTable<int>(INITIAL_CAPACITY, EqualityComparer<int>.Default);
        _initialCapacity = table.Capacity;
        for (var node = 0; node < NODE_COUNT; node++) table.Add(node);

        _finalCapacity = table.Capacity;
        _slotLimit = table.SlotLimit;
        for (var node = 0; node < NODE_COUNT; node++)
        {
            if (!table.TryGetSlot(node, out var slot) || table[slot] != node) _misplaced.Add(node);
        }
    }

    [Test]
    public void TheInitialCapacityShouldBeHonored()
    {
        _initialCapacity.Should().Be(INITIAL_CAPACITY);
    }

    [Test]
    public void TheCapacityShouldGrowToFitEveryNode()
    {
        _finalCapacity.Should().BeGreaterThanOrEqualTo(NODE_COUNT);
    }

    [Test]
    public void TheSlotLimitShouldBeOnePastTheHighestSlotUsed()
    {
        _slotLimit.Should().Be(NODE_COUNT);
    }

    [Test]
    public void EveryNodeShouldStillBeFound()
    {
        _misplaced.Should().BeEmpty();
    }
}
