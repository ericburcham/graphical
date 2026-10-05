using AwesomeAssertions;

namespace Graphical.UnitTests.MinHeapTests;

[TestFixture]
internal sealed class WhenClearingAHeap
{
    private const int VALUE_AFTER_CLEAR = 7;

    private int _countAfterClear;

    private int _poppedAfterReuse;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var heap = new MinHeap();
        heap.Push(1, 1);
        heap.Push(2, 2);
        heap.Clear();
        _countAfterClear = heap.Count;

        heap.Push(VALUE_AFTER_CLEAR, 5);
        heap.TryPop(out _poppedAfterReuse);
    }

    [Test]
    public void CountShouldBeZero()
    {
        _countAfterClear.Should().Be(0);
    }

    [Test]
    public void OnlyItemsPushedAfterClearingShouldComeOut()
    {
        _poppedAfterReuse.Should().Be(VALUE_AFTER_CLEAR);
    }
}
