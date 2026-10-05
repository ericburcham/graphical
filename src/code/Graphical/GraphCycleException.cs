using System;

namespace Graphical;

/// <summary>
/// The exception thrown when an operation on an acyclic graph would create a cycle, such as adding an edge
/// whose target can already reach its source.
/// </summary>
/// <remarks>
/// The graph is left unchanged when this exception is thrown.
/// </remarks>
public class GraphCycleException : InvalidOperationException
{
    private const string DEFAULT_MESSAGE = "The operation would create a cycle in the graph.";

    /// <summary>Creates an exception with a default message.</summary>
    public GraphCycleException()
        : base(DEFAULT_MESSAGE)
    {
    }

    /// <summary>Creates an exception with the given message.</summary>
    /// <param name="message">The message that describes the error.</param>
    public GraphCycleException(string? message)
        : base(message)
    {
    }

    /// <summary>Creates an exception with the given message and inner exception.</summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that caused this one, if any.</param>
    public GraphCycleException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
