using FluentAssertions;

namespace Graphical.UnitTests.EdgeTests;

[TestFixture]
internal sealed class WhenFormattingAnEdge
{
    private string? _text;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _text = new Edge<int>(1, 2).ToString();
    }

    [Test]
    public void TheTextShouldShowSourceArrowTarget()
    {
        _text.Should().Be("(1 -> 2)");
    }
}
