##Sage Core Queue usage and Sample
Sage Core AzureQueue service is an extension to Windows Azure Queue, with transient fault handling. Following are the steps involved to configure and use the Queue service.

1. Install Sage.Core.Queue NuGet package from Sage NuGet gallery.
2. Add a new configuration in azure configuration with the name SystemStorageConnectionString and set the storage connection string.
3. You can DI AzureQueue or manually instantiate the object and use it.

####Usage

To instantiate azure queue

    IQueue queue = new AzureQueue(new ConfigurationManager());

To enqueue a message

    queue.Enqueue(new AzureQueueMesage("Simple message in queue"));

To enqueue a message and make it available at later point in time

    var availabilityDate = DateTime.Now.AddHours(5);
    queue.Enqueue(new AzureQueueMesage("Simple timed message in queue"), availabilityDate);

To dequeue a message

    var message = queue.Dequeue();

To delete a message from queue;

    queue.Delete(dequeuedMesage);

To extend a lease of the message.

    queue.ExtendLease(dequeuedMesage);

To get count of messages in the queue. Note property returns an approximate message count not exact.

    queue.Count

To clear all the messages from queue

    queue.Clear();