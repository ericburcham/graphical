using AwesomeAssertions;

namespace Graphical.UnitTests.BitSetTests;

[TestFixture]
internal sealed class WhenCountingAndEnumeratingSetBits
{
    private const int CAPACITY = 200;

    private static readonly int[] SET_BITS = [0, 1, 63, 64, 100, 127, 128, 199];

    private readonly List<int> _enumerated = [];

    private int _popCount;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var bits = new BitSet(CAPACITY);
        foreach (var bit in SET_BITS.OrderByDescending(bit => bit)) bits.Set(bit);

        _popCount = bits.PopCount();
        _enumerated.AddRange(bits.EnumerateSetBits());
    }

    [Test]
    public void PopCountShouldBeTheNumberOfSetBits()
    {
        _popCount.Should().Be(SET_BITS.Length);
    }

    [Test]
    public void EnumerationShouldYieldTheSetBitsInAscendingOrder()
    {
        _enumerated.Should().Equal(SET_BITS);
    }
}
