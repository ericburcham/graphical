using AwesomeAssertions;

namespace Graphical.UnitTests.EdgeTests;

[TestFixture]
internal sealed class WhenComparingAnEdgeWithAnotherType
{
    private bool _equalsString;

    private bool _equalsNull;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var edge = new Edge<string>("a", "b");
        _equalsString = edge.Equals("(a -> b)");
        _equalsNull = edge.Equals(null);
    }

    [Test]
    public void EqualsAStringShouldBeFalse()
    {
        _equalsString.Should().BeFalse();
    }

    [Test]
    public void EqualsNullShouldBeFalse()
    {
        _equalsNull.Should().BeFalse();
    }
}
