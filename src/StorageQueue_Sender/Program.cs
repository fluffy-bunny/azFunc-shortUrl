using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace StorageQueue_Sender
{
    public class Message
    {
        public string Name { get; set; }
        public DateTime DirectiveDate { get; set; }
    }
    class Program
    {
        public static string GuidS => Guid.NewGuid().ToString();
        public static string ConnectionString { get; private set; }
        public static int Count { get; private set; }
        public static CloudStorageAccount CloudStorageAccount { get; private set; }
        public static int CountMax = 100;

        public static int SafeGetInt(string value, int defaultValue)
        {
            if (!int.TryParse(value, out var parsed))
                parsed = defaultValue;
            return parsed;
        }
        static async Task Main(string[] args)
        {
            Console.WriteLine("https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-dotnet-get-started-with-queues");
            Count = 1; 
            if (args.Any())
            {
                ConnectionString = args[0];
               
                if(args.Count() > 1)
                {
                    Count = SafeGetInt(args[1], Count);
                }
            }
            else
            {
                Console.Write("Enter Connection String: ");
                ConnectionString = Console.ReadLine();
            }
            Console.WriteLine($">> {ConnectionString} <<");
            CloudStorageAccount = CloudStorageAccount.Parse(ConnectionString);

            var helloWorld = await GetHelloWorldAsync();
            Console.WriteLine(helloWorld);
        }
        static async Task<string> GetHelloWorldAsync()
        {
            
            var queueClient = CloudStorageAccount.CreateCloudQueueClient();
            // Retrieve a reference to a container.
            CloudQueue queue = queueClient.GetQueueReference("backend-processing");

            // Create the queue if it doesn't already exist
            queue.CreateIfNotExists();

            if(Count >= CountMax)
            {
                Count = CountMax;
            }
            var i = 0;
            for(; i < Count; i++)
            {
                var message = new Message
                {
                    Name = GuidS,
                    DirectiveDate = DateTime.UtcNow
                };
                var json = JsonConvert.SerializeObject(message);

                CloudQueueMessage cqMessage = new CloudQueueMessage(json);

                await queue.AddMessageAsync(cqMessage);
            }
            return $"{i} messages added.";
        }
    }
}
