using AwesomeAssertions;

namespace Graphical.UnitTests.BitSetTests;

[TestFixture]
internal sealed class WhenUnioningBitSets
{
    private const int CAPACITY = 130;

    private const int TARGET_ONLY_BIT = 1;

    private const int OTHER_ONLY_BIT = 129;

    private const int UNSET_BIT = 64;

    private bool _targetOnlyBit;

    private bool _otherOnlyBit;

    private bool _unsetBit;

    private bool _otherUnchanged;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var target = new BitSet(CAPACITY);
        var other = new BitSet(CAPACITY);
        target.Set(TARGET_ONLY_BIT);
        other.Set(OTHER_ONLY_BIT);

        target.UnionWith(other);

        _targetOnlyBit = target.Get(TARGET_ONLY_BIT);
        _otherOnlyBit = target.Get(OTHER_ONLY_BIT);
        _unsetBit = target.Get(UNSET_BIT);
        _otherUnchanged = !other.Get(TARGET_ONLY_BIT);
    }

    [Test]
    public void BitsAlreadyInTheTargetShouldStay()
    {
        _targetOnlyBit.Should().BeTrue();
    }

    [Test]
    public void BitsFromTheOtherSetShouldBeAdded()
    {
        _otherOnlyBit.Should().BeTrue();
    }

    [Test]
    public void BitsInNeitherSetShouldStayClear()
    {
        _unsetBit.Should().BeFalse();
    }

    [Test]
    public void TheOtherSetShouldBeUnchanged()
    {
        _otherUnchanged.Should().BeTrue();
    }
}
