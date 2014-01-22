namespace Sage.Core.Framework.Storage
{
    using Microsoft.WindowsAzure.Storage.Queue;

    /// <summary>
    /// Represents a queue message
    /// </summary>
    public class AzureQueueMessage : IQueueMessage
    {
        /// <summary>
        /// Constructs a new Azure Que message
        /// </summary>
        /// <param name="message">Content of the message</param>
        public AzureQueueMessage(string message)
            : this(new CloudQueueMessage(message))
        {

        }

        /// <summary>
        /// Constructs a new Azure Que message
        /// </summary>
        /// <param name="queueMessage">Cloud message object</param>
        internal AzureQueueMessage(CloudQueueMessage queueMessage)
        {
            this.CloudQueueMessage = queueMessage;
        }

        /// <summary>
        /// Gets the message ID
        /// </summary>
        /// <remarks>ID is available only after the message is added to the queue</remarks>
        public string Id
        {
            get
            {
                return this.CloudQueueMessage.Id;
            }
        }
        /// <summary>
        /// Get the message payload or the content
        /// </summary>
        public string Payload
        {
            get
            {
                return this.CloudQueueMessage.AsString;
            }
        }
        /// <summary>
        /// Get the number of times this message has been dequeued.
        /// </summary>
        public int DequeueCount
        {
            get
            {
                return this.CloudQueueMessage.DequeueCount;
            }
        }

        public CloudQueueMessage CloudQueueMessage { get; set; }
    }
}
