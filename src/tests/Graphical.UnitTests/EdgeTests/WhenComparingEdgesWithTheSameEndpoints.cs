using AwesomeAssertions;

namespace Graphical.UnitTests.EdgeTests;

[TestFixture]
internal sealed class WhenComparingEdgesWithTheSameEndpoints
{
    private const string SOURCE = "a";

    private const string TARGET = "b";

    private Edge<string> _left;

    private Edge<string> _right;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _left = new Edge<string>(SOURCE, TARGET);
        _right = new Edge<string>(new string(SOURCE.ToCharArray()), new string(TARGET.ToCharArray()));
    }

    [Test]
    public void EqualsShouldBeTrue()
    {
        _left.Equals(_right).Should().BeTrue();
    }

    [Test]
    public void ObjectEqualsShouldBeTrue()
    {
        _left.Equals((object)_right).Should().BeTrue();
    }

    [Test]
    public void EqualityOperatorShouldBeTrue()
    {
        (_left == _right).Should().BeTrue();
    }

    [Test]
    public void InequalityOperatorShouldBeFalse()
    {
        (_left != _right).Should().BeFalse();
    }

    [Test]
    public void HashCodesShouldBeEqual()
    {
        _left.GetHashCode().Should().Be(_right.GetHashCode());
    }
}
