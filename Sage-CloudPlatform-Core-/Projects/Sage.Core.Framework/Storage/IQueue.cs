namespace Sage.Core.Framework.Storage
{
    using System;

    /// <summary>
    /// Represents a Queue storage
    /// </summary>
    public interface IQueue
    {
        /// <summary>
        /// Enqueue a message to the queue
        /// </summary>
        /// <param name="message">Message to be enqueued</param>
        void Enqueue(IQueueMessage message);

        /// <summary>
        /// Enqueue a message to the queue
        /// </summary>
        /// <param name="message">Message to be enqueued</param>
        /// <param name="availabilityDate">Date and time when the message should be available for processing.</param>
        void Enqueue(IQueueMessage message, DateTime availabilityDate);

        /// <summary>
        /// Gets a message from top of the Queue.
        /// </summary>
        /// <remarks>
        /// This action will not remove the message from queue, caller should explicitly call delete once the processing of message is complete.
        /// Once a message is dequeue it will be hidden for an hour before it is visible back in the queue.
        /// if it take longer than an hour to process the message, caller should call extend lease method to keep it hidden.
        /// </remarks>
        IQueueMessage Dequeue();

        /// <summary>
        /// Deletes a specific message from the queue
        /// </summary>
        /// <param name="message">Message to be deleted</param>
        void Delete(IQueueMessage message);

        /// <summary>
        /// Extends the lease of the message.
        /// </summary>
        /// <param name="message">Message whose lease needs to be extended</param>
        void ExtendLease(IQueueMessage message);

        /// <summary>
        /// Get the number of messages sitting in the queue.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Clears out the entire queue.
        /// </summary>
        void Clear();

    }
}
