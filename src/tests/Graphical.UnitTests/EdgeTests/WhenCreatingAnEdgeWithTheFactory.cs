using FluentAssertions;

namespace Graphical.UnitTests.EdgeTests;

[TestFixture]
internal sealed class WhenCreatingAnEdgeWithTheFactory
{
    private const string SOURCE = "a";

    private const string TARGET = "b";

    private Edge<string> _edge;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _edge = Edge.Create(SOURCE, TARGET);
    }

    [Test]
    public void TheEdgeShouldEqualOneBuiltWithTheConstructor()
    {
        _edge.Should().Be(new Edge<string>(SOURCE, TARGET));
    }
}
