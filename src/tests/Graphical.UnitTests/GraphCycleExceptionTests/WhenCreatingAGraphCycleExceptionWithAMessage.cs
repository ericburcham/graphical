using AwesomeAssertions;

namespace Graphical.UnitTests.GraphCycleExceptionTests;

[TestFixture]
internal sealed class WhenCreatingAGraphCycleExceptionWithAMessage
{
    private const string MESSAGE = "cycle";

    private GraphCycleException? _exception;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _exception = new GraphCycleException(MESSAGE);
    }

    [Test]
    public void TheMessageShouldBeKept()
    {
        _exception!.Message.Should().Be(MESSAGE);
    }
}
