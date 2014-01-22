namespace Sage.Core.Framework.Storage
{
    using System;
    using System.Globalization;

    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Microsoft.WindowsAzure.Storage.RetryPolicies;

    using Sage.Core.Framework.Configuration;
    using Sage.Core.Utilities.Diagnostics;

    public sealed class AzureQueue : IQueue
    {
        private readonly CloudQueue _internalQueue;
        private const string SystemStorageConnectionString = "SystemStorageConnectionString";

        /// <summary>
        /// Minimum length of Queue name in Azure
        /// </summary>
        private const int MinQueueNameLength = 3;

        /// <summary>
        /// Maximum length of Queue name in Azure
        /// </summary>
        private const int MaxQueueNameLength = 63;

        /// <summary>
        /// Maximum time a message can live in Queue.
        /// </summary>
        private readonly TimeSpan _maxMessageLifeSpan = TimeSpan.FromDays(7);

        /// <summary>
        /// Maximum time the message is kept hidden after it has been dequeued for precessing.
        /// </summary>
        private readonly TimeSpan _visibilityTimeout = TimeSpan.FromHours(1);
        /// <summary>
        /// Constructs the Azure Queue
        /// </summary>
        /// <param name="configurationManager"></param>
        public AzureQueue(IConfigurationManager configurationManager)
        {
            ArgumentValidator.ValidateNonNullReference(configurationManager, "configurationManager", "AzureQueue.Ctor()");
            this._internalQueue = this.GetCloudQueue(configurationManager);
        }

        /// <summary>
        /// Enqueue a message to the queue
        /// </summary>
        /// <param name="message">Message to be enqueued</param>
        public void Enqueue(IQueueMessage message)
        {
            this.InternalEnqueue(message, TimeSpan.Zero);
        }

        /// <summary>
        /// Enqueue a message to the queue
        /// </summary>
        /// <param name="message">Message to be enqueued</param>
        /// <param name="availabilityDate">Date and time when the message should be available for processing.</param>
        public void Enqueue(IQueueMessage message, DateTime availabilityDate)
        {
            //TODO:Should availabilityDate be converted to UTC?
            TimeSpan initialVisibilityDelay = availabilityDate - DateTime.UtcNow;
            if (initialVisibilityDelay.Ticks < 0)
                initialVisibilityDelay = TimeSpan.Zero;

            this.InternalEnqueue(message, initialVisibilityDelay);
        }

        /// <summary>
        /// Gets a message from top of the Queue.
        /// </summary>
        /// <remarks>
        /// This action will not remove the message from queue, caller should explicitly call delete once the processing of message is complete.
        /// Once a message is dequeue it will be hidden for an hour before it is visible back in the queue.
        /// if it take longer than an hour to process the message, caller should call extend lease method to keep it hidden.
        /// </remarks>
        public IQueueMessage Dequeue()
        {
            IQueueMessage storageMessage = null;
            try
            {
                CloudQueueMessage msg = this._internalQueue.GetMessage(this._visibilityTimeout);

                if (msg != null)
                    storageMessage = new AzureQueueMessage(msg);
            }
            catch (StorageException ex)
            {
                throw new Exception(QueueResource.QueueDequeueError, ex);
            }
            return storageMessage;
        }

        /// <summary>
        /// Deletes a specific message from the queue
        /// </summary>
        /// <param name="message">Message to be deleted</param>
        public void Delete(IQueueMessage message)
        {
            ArgumentValidator.ValidateNonNullReference(message, "message", "AzureQueue.Delete");
            ArgumentValidator.ValidateIsAssignableFromType(message.GetType(), "message", "AzureQueue.Delete", typeof(AzureQueueMessage));
            try
            {
                this._internalQueue.DeleteMessage(((AzureQueueMessage)message).CloudQueueMessage);
            }
            catch (StorageException ex)
            {
                throw new Exception(QueueResource.QueueDeleteError, ex);
            }
        }

        /// <summary>
        /// Extends the lease of the message.
        /// </summary>
        /// <param name="message">Message whose lease needs to be extended</param>
        public void ExtendLease(IQueueMessage message)
        {
            ArgumentValidator.ValidateNonNullReference(message, "message", "AzureQueue.ExtendLease");
            ArgumentValidator.ValidateIsAssignableFromType(message.GetType(), "message", "AzureQueue.ExtendLease", typeof(AzureQueueMessage));
            try
            {
                this._internalQueue.UpdateMessage(((AzureQueueMessage)message).CloudQueueMessage, this._visibilityTimeout, MessageUpdateFields.Visibility);
            }
            catch (StorageException ex)
            {
                throw new Exception(QueueResource.QueueExtendLeaseError, ex);
            }
        }

        /// <summary>
        /// Get the number of messages sitting in the queue.
        /// </summary>
        public int Count {
            get
            {
                int? count = this._internalQueue.ApproximateMessageCount;
                return count.HasValue ? count.Value : 0;
            }
        }
        
        /// <summary>
        /// Clears out the entire queue.
        /// </summary>
        public void Clear()
        {
            try
            {
                this._internalQueue.Clear();
            }
            catch (StorageException ex)
            {
                throw new Exception(QueueResource.QueueClearError, ex);
            }
        }

        /// <summary>
        /// Enqueue a message to the queue
        /// </summary>
        /// <param name="message">Message to be enqueued</param>
        /// <param name="initialVisibilityDelay">Initial time delay before the message can be made visible for processing</param>
        /// <remarks>
        /// Message a set to live for a maximum of 7 days. 
        /// </remarks>
        private void InternalEnqueue(IQueueMessage message, TimeSpan initialVisibilityDelay)
        {
            try
            {
                ArgumentValidator.ValidateNonNullReference(message, "message", "AzureQueue.InternalEnqueue");
                ArgumentValidator.ValidateIsAssignableFromType(message.GetType(), "message", "AzureQueue.InternalEnqueue", typeof(AzureQueueMessage));

                this._internalQueue.AddMessage(((AzureQueueMessage)message).CloudQueueMessage, this._maxMessageLifeSpan, initialVisibilityDelay);
            }
            catch (StorageException ex)
            {
                throw new Exception(QueueResource.QueueEnqueueError, ex);
            }
        }

        /// <summary>
        /// Get the CloudQueue object.
        /// </summary>
        /// <param name="configurationManager">Configuration manager object</param>
        /// <remarks>
        /// The name of the queue is derived from the QueueType.
        /// This method will create a queue if the queue does not exist already.
        /// </remarks>
        /// <returns>Cloud Queue</returns>
        private CloudQueue GetCloudQueue(IConfigurationManager configurationManager)
        {
            var queueName = this.GetQueueNameFromType(QueueTypes.Default);
            ValidateQueueName(queueName);

            CloudQueue queue = null;
            try
            {
                var storageAccountConnectionString = configurationManager.SystemConfiguration[SystemStorageConnectionString];
                var azureStorageAccount = this.GetAzureStorageAccount(storageAccountConnectionString);
                CloudQueueClient queueStorage = azureStorageAccount.CreateCloudQueueClient();

                if (queueStorage != null)
                {
                    queueStorage.RetryPolicy = new ExponentialRetry(new TimeSpan(0, 0, 2), 5);

                    queue = queueStorage.GetQueueReference(queueName);

                    if (queue == null)
                        throw new Exception(string.Format(CultureInfo.CurrentCulture, QueueResource.QueueCreationError, queueName));

                    queue.CreateIfNotExists();
                }
            }
            catch (StorageException err)
            {
                throw new Exception(string.Format(CultureInfo.CurrentCulture, QueueResource.QueueCreationError, queueName), err);
            }
            return queue;
        }

        private static void ValidateQueueName(string queueName)
        {
            if (string.IsNullOrEmpty(queueName))
            {
                throw new ArgumentNullException("queueName");
            }
            if (queueName.Length <= MinQueueNameLength)
                throw new ArgumentOutOfRangeException("queueName", queueName.Length, QueueResource.QueueNameTooShort);
            if (queueName.Length >= MaxQueueNameLength)
                throw new ArgumentOutOfRangeException("queueName", queueName.Length, QueueResource.QueueNameTooLong);
        }

        private CloudStorageAccount GetAzureStorageAccount(string storageAccountConnectionString)
        {
            if (string.IsNullOrEmpty(storageAccountConnectionString))
                return CloudStorageAccount.DevelopmentStorageAccount;
            return CloudStorageAccount.Parse(storageAccountConnectionString);
        }

        private string GetQueueNameFromType(QueueTypes type)
        {
            // Make up a name based on the enum and namespace
            string name = string.Format(CultureInfo.CurrentCulture, "{0}.{1}", type.GetType().FullName, type);

            // Queue names in azure must be lower case
            name = name.ToLower(CultureInfo.CurrentCulture);

            // No '.' allow in the name
            name = name.Replace('.', '-');

            return name;
        }
    }

    internal enum QueueTypes
    {
        /// <summary>
        /// Unit Testing Queue
        /// </summary>
        Test,
        /// <summary>
        /// Unit of work
        /// </summary>
        Default
    }
}
