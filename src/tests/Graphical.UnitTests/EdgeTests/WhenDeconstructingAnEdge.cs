using FluentAssertions;

namespace Graphical.UnitTests.EdgeTests;

[TestFixture]
internal sealed class WhenDeconstructingAnEdge
{
    private const string SOURCE = "a";

    private const string TARGET = "b";

    private string? _source;

    private string? _target;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        (_source, _target) = new Edge<string>(SOURCE, TARGET);
    }

    [Test]
    public void TheFirstValueShouldBeTheSource()
    {
        _source.Should().Be(SOURCE);
    }

    [Test]
    public void TheSecondValueShouldBeTheTarget()
    {
        _target.Should().Be(TARGET);
    }
}
