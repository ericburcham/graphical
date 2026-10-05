using AwesomeAssertions;

namespace Graphical.UnitTests.BitSetTests;

[TestFixture]
internal sealed class WhenEnumeratingEachBitPositionAlone
{
    private const int CAPACITY = 128;

    private readonly List<int> _mismatches = [];

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        for (var bit = 0; bit < CAPACITY; bit++)
        {
            var bits = new BitSet(CAPACITY);
            bits.Set(bit);
            if (!bits.EnumerateSetBits().SequenceEqual([bit])) _mismatches.Add(bit);
        }
    }

    [Test]
    public void EveryPositionShouldBeReportedExactly()
    {
        _mismatches.Should().BeEmpty();
    }
}
