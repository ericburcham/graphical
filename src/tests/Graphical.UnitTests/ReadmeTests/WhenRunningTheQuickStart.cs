using AwesomeAssertions;

namespace Graphical.UnitTests.ReadmeTests;

// Mirrors the README "Quick start" sample, with Console replaced by a StringWriter.
[TestFixture]
internal sealed class WhenRunningTheQuickStart
{
    private readonly List<string> _lines = [];

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        using var console = new StringWriter();

        var build = new DirectedAcyclicGraph<string>();
        build.AddEdge("restore", "compile");   // restore must run before compile
        build.AddEdge("compile", "test");
        build.AddEdge("compile", "pack");

        console.WriteLine(string.Join(" -> ", build.GetTopologicalOrder()));
        // restore -> compile -> test -> pack

        try { build.AddEdge("pack", "restore"); }
        catch (GraphCycleException e) { console.WriteLine(e.Message); }
        // Adding the edge (pack -> restore) would create a cycle.

        _lines.AddRange(console.ToString().Split([Environment.NewLine], StringSplitOptions.RemoveEmptyEntries));
    }

    [Test]
    public void ItShouldPrintTheTopologicalOrderThenTheCycleMessage()
    {
        _lines.Should().Equal(
            "restore -> compile -> test -> pack",
            "Adding the edge (pack -> restore) would create a cycle.");
    }
}
