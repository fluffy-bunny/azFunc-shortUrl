using Azure.Identity;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using System;
using System.Text;
using System.Threading.Tasks;

namespace EventHubsSender
{

    class Program
    {
        static string GuidS => Guid.NewGuid().ToString();
        static string ConnectionString { get; set; }
        static async Task Main(string[] args)
        {
            Console.WriteLine("https://docs.microsoft.com/en-us/azure/event-hubs/get-started-dotnet-standard-send-v2");
            Console.Write("Enter Connection String: ");

            string myFirstName;
            ConnectionString = Console.ReadLine();

            var helloWorld = await GetHelloWorldAsync();
            Console.WriteLine(helloWorld);
        }
        //Endpoint=sb://<namespace>.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=<secret>
        static async Task<string> GetHelloWorldAsync()
        {
            //  await using (EventHubProducerClient producerClient = new EventHubProducerClient("evhns-shorturl-001", "evh-shorturl", new DefaultAzureCredential()))

            await using (EventHubProducerClient producerClient = new EventHubProducerClient(ConnectionString, "evh-shorturl"))
            {
                // create a batch
                using (EventDataBatch eventBatch = await producerClient.CreateBatchAsync())
                {

                    for(int i = 0; i < 10; i++)
                    {
                        // add events to the batch. only one in this case. 
                        eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes($"test-{GuidS}")));
                    }
               

                    // send the batch to the event hub
                    await producerClient.SendAsync(eventBatch);
                }
            }
            return $"{DateTime.Now} - SENT{Environment.NewLine}";
        }
    }
}
