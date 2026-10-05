using AwesomeAssertions;

namespace Graphical.UnitTests.BitSetTests;

[TestFixture]
internal sealed class WhenClearingAllBits
{
    private const int CAPACITY = 130;

    private bool _anySet;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var bits = new BitSet(CAPACITY);
        bits.Set(0);
        bits.Set(64);
        bits.Set(129);

        bits.ClearAll();

        _anySet = bits.Get(0) || bits.Get(64) || bits.Get(129);
    }

    [Test]
    public void NoBitShouldBeSet()
    {
        _anySet.Should().BeFalse();
    }
}
