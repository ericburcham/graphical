using AwesomeAssertions;

namespace Graphical.UnitTests.IGraphTests;

[TestFixture(typeof(UndirectedGraph<>))]
internal sealed class WhenCreatingAGraphWithACapacityAndAComparer
{
    private const int CAPACITY = 16;

    private readonly Type _graphType;

    private IGraph<string> _withCapacityAndComparer = null!;

    private IGraph<string> _withComparer = null!;

    private IGraph<string> _withCapacity = null!;

    private IGraph<string> _withNullComparer = null!;

    public WhenCreatingAGraphWithACapacityAndAComparer(Type graphType)
    {
        _graphType = graphType;
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _withCapacityAndComparer = GraphFactory.Create<string>(_graphType, CAPACITY, StringComparer.OrdinalIgnoreCase);
        _withComparer = GraphFactory.Create<string>(_graphType, StringComparer.OrdinalIgnoreCase);
        _withCapacity = GraphFactory.Create<string>(_graphType, CAPACITY);
        _withNullComparer = GraphFactory.Create<string>(_graphType, CAPACITY, null);
    }

    [Test]
    public void TheCapacityAndComparerConstructorShouldKeepTheComparer()
    {
        _withCapacityAndComparer.Comparer.Should().BeSameAs(StringComparer.OrdinalIgnoreCase);
    }

    [Test]
    public void TheComparerConstructorShouldKeepTheComparer()
    {
        _withComparer.Comparer.Should().BeSameAs(StringComparer.OrdinalIgnoreCase);
    }

    [Test]
    public void TheCapacityConstructorShouldUseTheDefaultComparer()
    {
        _withCapacity.Comparer.Should().BeSameAs(EqualityComparer<string>.Default);
    }

    [Test]
    public void ANullComparerShouldMeanTheDefaultComparer()
    {
        _withNullComparer.Comparer.Should().BeSameAs(EqualityComparer<string>.Default);
    }

    [Test]
    public void TheGraphShouldStartEmpty()
    {
        _withCapacityAndComparer.NodeCount.Should().Be(0);
    }
}
