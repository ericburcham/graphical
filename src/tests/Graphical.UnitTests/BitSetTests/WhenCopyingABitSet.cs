using AwesomeAssertions;

namespace Graphical.UnitTests.BitSetTests;

[TestFixture]
internal sealed class WhenCopyingABitSet
{
    private const int CAPACITY = 130;

    private const int TARGET_ONLY_BIT = 1;

    private const int SOURCE_BIT = 128;

    private bool _targetOnlyBit;

    private bool _sourceBit;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var target = new BitSet(CAPACITY);
        var source = new BitSet(CAPACITY);
        target.Set(TARGET_ONLY_BIT);
        source.Set(SOURCE_BIT);

        target.CopyFrom(source);

        _targetOnlyBit = target.Get(TARGET_ONLY_BIT);
        _sourceBit = target.Get(SOURCE_BIT);
    }

    [Test]
    public void BitsOnlyInTheTargetShouldBeCleared()
    {
        _targetOnlyBit.Should().BeFalse();
    }

    [Test]
    public void BitsFromTheSourceShouldBeSet()
    {
        _sourceBit.Should().BeTrue();
    }
}
