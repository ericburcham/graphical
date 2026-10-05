using AwesomeAssertions;

namespace Graphical.UnitTests.ReadmeTests;

// Mirrors the README "DirectedGraph" sample.
[TestFixture]
internal sealed class WhenUsingADirectedGraph
{
    private IReadOnlyCollection<string> _successors = null!;

    private IReadOnlyCollection<string> _predecessors = null!;

    private IReadOnlyCollection<string> _sources = null!;

    private IReadOnlyCollection<string> _sinks = null!;

    private bool _postReachesAbout;

    private bool _homeReachesHome;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var links = new DirectedGraph<string>();
        links.AddEdges(new[]
        {
            Edge.Create("home", "about"), Edge.Create("home", "blog"),
            Edge.Create("blog", "post"), Edge.Create("post", "home"),
        });
        links.AddNode("orphan");

        _successors = links.GetSuccessors("home");       // about, blog
        _predecessors = links.GetPredecessors("home");   // post
        _sources = links.GetSources();                   // orphan (nothing links to it)
        _sinks = links.GetSinks();                       // about, orphan (they link nowhere)
        _postReachesAbout = links.HasPath("post", "about");  // true: post -> home -> about
        _homeReachesHome = links.HasPath("home", "home");    // true: home -> blog -> post -> home
    }

    [Test]
    public void SuccessorsShouldBeAboutAndBlog()
    {
        _successors.Should().BeEquivalentTo(["about", "blog"]);
    }

    [Test]
    public void PredecessorsShouldBePost()
    {
        _predecessors.Should().Equal("post");
    }

    [Test]
    public void SourcesShouldBeOrphan()
    {
        _sources.Should().Equal("orphan");
    }

    [Test]
    public void SinksShouldBeAboutAndOrphan()
    {
        _sinks.Should().Equal("about", "orphan");
    }

    [Test]
    public void PostShouldReachAbout()
    {
        _postReachesAbout.Should().BeTrue();
    }

    [Test]
    public void HomeShouldReachItselfThroughTheCycle()
    {
        _homeReachesHome.Should().BeTrue();
    }
}
