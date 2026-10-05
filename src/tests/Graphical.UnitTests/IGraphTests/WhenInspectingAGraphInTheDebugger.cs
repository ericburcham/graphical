using System.Diagnostics;
using System.Reflection;
using AwesomeAssertions;

namespace Graphical.UnitTests.IGraphTests;

[TestFixture(typeof(UndirectedGraph<>))]
internal sealed class WhenInspectingAGraphInTheDebugger
{
    private readonly Type _graphType;

    private DebuggerDisplayAttribute? _attribute;

    public WhenInspectingAGraphInTheDebugger(Type graphType)
    {
        _graphType = graphType;
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _attribute = _graphType.GetCustomAttribute<DebuggerDisplayAttribute>(inherit: false);
    }

    [Test]
    public void TheDisplayShouldShowTheNodeAndEdgeCounts()
    {
        _attribute!.Value.Should().Be("NodeCount = {NodeCount}, EdgeCount = {EdgeCount}");
    }
}
