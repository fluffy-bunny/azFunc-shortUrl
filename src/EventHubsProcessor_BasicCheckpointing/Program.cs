using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using Azure.Messaging.EventHubs.Producer;
using Azure.Storage.Blobs;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EventHubsProcessor_BasicCheckpointing
{
    class Program
    {
        static string GuidS => Guid.NewGuid().ToString();
        static string ConnectionStringEventHub { get; set; }
        static string ConnectionStringBlob { get;  set; }

        static async Task Main(string[] args)
        {
            Console.WriteLine("https://docs.microsoft.com/en-us/azure/event-hubs/get-started-dotnet-standard-send-v2");
            Console.Write("EventHub Connection String: ");
            ConnectionStringEventHub = Console.ReadLine();

            Console.Write("Blob Connection String: ");
            ConnectionStringBlob = Console.ReadLine();

            var helloWorld = await GetHelloWorldAsync();
            Console.WriteLine(helloWorld);
        }
        //Endpoint=sb://<namespace>.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=<secret>
        static async Task<string> GetHelloWorldAsync()
        {
            string consumerGroup = EventHubConsumerClient.DefaultConsumerGroupName;
            BlobContainerClient storageClient = new BlobContainerClient(ConnectionStringBlob, "evh-blob");
            EventProcessorClient processor = new EventProcessorClient(storageClient, consumerGroup, ConnectionStringEventHub, "evh-shorturl");
            
            int eventIndex = 0;
            int eventsSinceLastCheckpoint = 0;

            async Task processEventHandler(ProcessEventArgs eventArgs)
            {
                if (eventArgs.CancellationToken.IsCancellationRequested)
                {
                    return;
                }

                try
                {
                    ++eventIndex;
                    ++eventsSinceLastCheckpoint;

                    if (eventsSinceLastCheckpoint >= 1)
                    {
                        // Updating the checkpoint will interact with the Azure Storage.  As a service call,
                        // this is done asynchronously and may be long-running.  You may want to influence its behavior,
                        // such as limiting the time that it may execute in order to ensure throughput for
                        // processing events.
                        //
                        // In our case, we'll limit the checkpoint operation to a second and request cancellation
                        // if it runs longer.

                        using CancellationTokenSource cancellationSource = new CancellationTokenSource(TimeSpan.FromSeconds(1));

                        try
                        {
                            await eventArgs.UpdateCheckpointAsync(cancellationSource.Token);
                            eventsSinceLastCheckpoint = 0;

                            Console.WriteLine("Created checkpoint");
                        }
                        catch (TaskCanceledException)
                        {
                            Console.WriteLine("Checkpoint creation took too long and was canceled.");
                        }

                        Console.WriteLine();
                    }

                    Console.WriteLine($"Event Received: { Encoding.UTF8.GetString(eventArgs.Data.Body.ToArray()) }");
                }
                catch (Exception ex)
                {
                    // For real-world scenarios, you should take action appropriate to your application.  For our example, we'll just log
                    // the exception to the console.

                    Console.WriteLine();
                    Console.WriteLine($"An error was observed while processing events.  Message: { ex.Message }");
                    Console.WriteLine();
                }
            };
            // For this example, exceptions will just be logged to the console.

            Task processErrorHandler(ProcessErrorEventArgs eventArgs)
            {
                if (eventArgs.CancellationToken.IsCancellationRequested)
                {
                    return Task.CompletedTask;
                }

                Console.WriteLine("===============================");
                Console.WriteLine($"The error handler was invoked during the operation: { eventArgs.Operation ?? "Unknown" }, for Exception: { eventArgs.Exception.Message }");
                Console.WriteLine("===============================");
                Console.WriteLine();

                return Task.CompletedTask;
            }
            processor.ProcessEventAsync += processEventHandler;
            processor.ProcessErrorAsync += processErrorHandler;

            try
            {
                // In order to begin processing, an explicit call must be made to the processor.  This will instruct the processor to begin
                // processing in the background, invoking your handlers when they are needed.

                eventIndex = 0;
                await processor.StartProcessingAsync();

                // Because processing takes place in the background, we'll continue to wait until all of our events were
                // read and handled before stopping.  To ensure that we don't wait indefinitely should an unrecoverable
                // error be encountered, we'll also add a timed cancellation.

                using var cancellationSource = new CancellationTokenSource();
                cancellationSource.CancelAfter(TimeSpan.FromSeconds(30));

                while ((!cancellationSource.IsCancellationRequested) && (eventIndex <= 10))
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(250));
                }

                // Once we arrive at this point, either cancellation was requested or we have processed all of our events.  In
                // both cases, we'll want to shut down the processor.

                await processor.StopProcessingAsync();
            }
            finally
            {
                // It is encouraged that you unregister your handlers when you have finished
                // using the Event Processor to ensure proper cleanup.  This is especially
                // important when using lambda expressions or handlers in any form that may
                // contain closure scopes or hold other references.

                processor.ProcessEventAsync -= processEventHandler;
                processor.ProcessErrorAsync -= processErrorHandler;
            }

            return "ok";
        }
    }
}
