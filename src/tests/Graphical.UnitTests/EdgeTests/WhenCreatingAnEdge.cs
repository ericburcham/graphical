using FluentAssertions;

namespace Graphical.UnitTests.EdgeTests;

[TestFixture]
internal sealed class WhenCreatingAnEdge
{
    private const string SOURCE = "a";

    private const string TARGET = "b";

    private Edge<string> _edge;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _edge = new Edge<string>(SOURCE, TARGET);
    }

    [Test]
    public void SourceShouldBeTheFirstArgument()
    {
        _edge.Source.Should().Be(SOURCE);
    }

    [Test]
    public void TargetShouldBeTheSecondArgument()
    {
        _edge.Target.Should().Be(TARGET);
    }
}
