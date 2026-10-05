using FluentAssertions;

namespace Graphical.UnitTests.EdgeTests;

[TestFixture]
internal sealed class WhenReversingAnEdge
{
    private const string SOURCE = "a";

    private const string TARGET = "b";

    private Edge<string> _reversed;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _reversed = new Edge<string>(SOURCE, TARGET).Reverse();
    }

    [Test]
    public void SourceShouldBeTheOriginalTarget()
    {
        _reversed.Source.Should().Be(TARGET);
    }

    [Test]
    public void TargetShouldBeTheOriginalSource()
    {
        _reversed.Target.Should().Be(SOURCE);
    }
}
