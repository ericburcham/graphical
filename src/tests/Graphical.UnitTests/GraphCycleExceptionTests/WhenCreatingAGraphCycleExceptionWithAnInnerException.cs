using FluentAssertions;

namespace Graphical.UnitTests.GraphCycleExceptionTests;

[TestFixture]
internal sealed class WhenCreatingAGraphCycleExceptionWithAnInnerException
{
    private const string MESSAGE = "cycle";

    private readonly Exception _inner = new InvalidOperationException("inner");

    private GraphCycleException? _exception;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _exception = new GraphCycleException(MESSAGE, _inner);
    }

    [Test]
    public void TheMessageShouldBeKept()
    {
        _exception!.Message.Should().Be(MESSAGE);
    }

    [Test]
    public void TheInnerExceptionShouldBeKept()
    {
        _exception!.InnerException.Should().BeSameAs(_inner);
    }
}
