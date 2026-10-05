using AwesomeAssertions;

namespace Graphical.UnitTests.BitSetTests;

[TestFixture]
internal sealed class WhenGrowingABitSet
{
    private const int INITIAL_CAPACITY = 10;

    private const int GROWN_CAPACITY = 200;

    private const int LOW_BIT = 3;

    private const int HIGH_BIT = 150;

    private bool _lowBit;

    private bool _highBit;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var bits = new BitSet(INITIAL_CAPACITY);
        bits.Set(LOW_BIT);
        bits.EnsureCapacity(GROWN_CAPACITY);
        bits.Set(HIGH_BIT);

        _lowBit = bits.Get(LOW_BIT);
        _highBit = bits.Get(HIGH_BIT);
    }

    [Test]
    public void BitsSetBeforeGrowingShouldBeKept()
    {
        _lowBit.Should().BeTrue();
    }

    [Test]
    public void BitsBeyondTheFirstWordShouldBeSettable()
    {
        _highBit.Should().BeTrue();
    }
}
