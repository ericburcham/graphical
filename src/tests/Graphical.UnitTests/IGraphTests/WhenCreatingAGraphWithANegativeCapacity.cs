using AwesomeAssertions;

namespace Graphical.UnitTests.IGraphTests;

[TestFixture(typeof(UndirectedGraph<>))]
[TestFixture(typeof(DirectedGraph<>))]
internal sealed class WhenCreatingAGraphWithANegativeCapacity
{
    private const int NEGATIVE_CAPACITY = -1;

    private readonly Type _graphType;

    private Exception? _capacityException;

    private Exception? _capacityAndComparerException;

    public WhenCreatingAGraphWithANegativeCapacity(Type graphType)
    {
        _graphType = graphType;
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _capacityException = Catch.Exception(() => GraphFactory.Create<string>(_graphType, NEGATIVE_CAPACITY));
        _capacityAndComparerException = Catch.Exception(
            () => GraphFactory.Create<string>(_graphType, NEGATIVE_CAPACITY, null));
    }

    [Test]
    public void TheCapacityConstructorShouldThrowArgumentOutOfRange()
    {
        _capacityException.Should().BeOfType<ArgumentOutOfRangeException>();
    }

    [Test]
    public void TheCapacityAndComparerConstructorShouldThrowArgumentOutOfRange()
    {
        _capacityAndComparerException.Should().BeOfType<ArgumentOutOfRangeException>();
    }
}
