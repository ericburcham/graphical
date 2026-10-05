using AwesomeAssertions;

namespace Graphical.UnitTests.GraphCycleExceptionTests;

[TestFixture]
internal sealed class WhenCreatingAGraphCycleExceptionWithoutArguments
{
    private GraphCycleException? _exception;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _exception = new GraphCycleException();
    }

    [Test]
    public void ItShouldBeAnInvalidOperationException()
    {
        _exception.Should().BeAssignableTo<InvalidOperationException>();
    }

    [Test]
    public void TheMessageShouldNotBeEmpty()
    {
        _exception!.Message.Should().NotBeNullOrWhiteSpace();
    }
}
