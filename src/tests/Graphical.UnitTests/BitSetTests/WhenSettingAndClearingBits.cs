using FluentAssertions;

namespace Graphical.UnitTests.BitSetTests;

[TestFixture]
internal sealed class WhenSettingAndClearingBits
{
    private const int CAPACITY = 10;

    private const int SET_BIT = 3;

    private const int CLEARED_BIT = 5;

    private const int UNTOUCHED_BIT = 7;

    private bool _setBit;

    private bool _clearedBit;

    private bool _untouchedBit;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var bits = new BitSet(CAPACITY);
        bits.Set(SET_BIT);
        bits.Set(CLEARED_BIT);
        bits.Clear(CLEARED_BIT);

        _setBit = bits.Get(SET_BIT);
        _clearedBit = bits.Get(CLEARED_BIT);
        _untouchedBit = bits.Get(UNTOUCHED_BIT);
    }

    [Test]
    public void ASetBitShouldBeTrue()
    {
        _setBit.Should().BeTrue();
    }

    [Test]
    public void AClearedBitShouldBeFalse()
    {
        _clearedBit.Should().BeFalse();
    }

    [Test]
    public void AnUntouchedBitShouldBeFalse()
    {
        _untouchedBit.Should().BeFalse();
    }
}
