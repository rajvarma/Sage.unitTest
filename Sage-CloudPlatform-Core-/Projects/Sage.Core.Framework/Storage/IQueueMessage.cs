namespace Sage.Core.Framework.Storage
{
    /// <summary>
    /// Represents a queue message
    /// </summary>
    public interface IQueueMessage
    {
        /// <summary>
        /// Gets the message ID
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Get the message payload or the content
        /// </summary>
        string Payload { get; }

        /// <summary>
        /// Get the number of times this message has been dequeued.
        /// </summary>
        int DequeueCount { get; }

    }
}
