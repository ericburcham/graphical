using AwesomeAssertions;

namespace Graphical.UnitTests.MinHeapTests;

[TestFixture]
internal sealed class WhenPoppingEveryItem
{
    private static readonly long[] KEYS = [50, 3, 99, 7, 1, 42, 8, 15, 2, 64];

    private readonly List<long> _poppedKeys = [];

    private int _countAfterPushing;

    private bool _poppedFromEmpty;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var heap = new MinHeap();
        foreach (var key in KEYS) heap.Push((int)key * 10, key);
        _countAfterPushing = heap.Count;

        while (heap.TryPop(out var value)) _poppedKeys.Add(value / 10);
        _poppedFromEmpty = heap.TryPop(out _);
    }

    [Test]
    public void CountShouldBeTheNumberOfItemsPushed()
    {
        _countAfterPushing.Should().Be(KEYS.Length);
    }

    [Test]
    public void ItemsShouldComeOutInAscendingKeyOrder()
    {
        _poppedKeys.Should().Equal(KEYS.OrderBy(key => key));
    }

    [Test]
    public void PoppingAnEmptyHeapShouldFail()
    {
        _poppedFromEmpty.Should().BeFalse();
    }
}
