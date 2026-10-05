using AwesomeAssertions;

namespace Graphical.UnitTests.EdgeTests;

[TestFixture]
internal sealed class WhenComparingEdgesWithSwappedEndpoints
{
    private const string FIRST = "a";

    private const string SECOND = "b";

    private Edge<string> _left;

    private Edge<string> _right;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _left = new Edge<string>(FIRST, SECOND);
        _right = new Edge<string>(SECOND, FIRST);
    }

    [Test]
    public void EqualsShouldBeFalse()
    {
        _left.Equals(_right).Should().BeFalse();
    }

    [Test]
    public void ObjectEqualsShouldBeFalse()
    {
        _left.Equals((object)_right).Should().BeFalse();
    }

    [Test]
    public void EqualityOperatorShouldBeFalse()
    {
        (_left == _right).Should().BeFalse();
    }

    [Test]
    public void InequalityOperatorShouldBeTrue()
    {
        (_left != _right).Should().BeTrue();
    }
}
