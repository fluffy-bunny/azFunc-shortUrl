using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
 

namespace StorageQueue_Reader
{
    class Program
    {
        public static string ConnectionString { get; private set; }
        public static CloudStorageAccount CloudStorageAccount { get; private set; }

        static async Task Main(string[] args)
        {
            Console.WriteLine("https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-dotnet-get-started-with-queues");
           
            if (args.Any())
            {
                ConnectionString = args[0];
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
            TimeSpan sleepDelay = TimeSpan.FromSeconds(10);

            for (var i = 0; i < 10; i++)
            {
                // Note: We used to add this to a list, but that is not necessary.
                // Note: We assign to a variable just to supress a compiler warning
                // that we aren't using await here. 

 
                var task = Task.Run(() => WorkTask( sleepDelay));
 
            }
            Console.ReadLine();
            return "d";
        }
        static async Task WorkTask(TimeSpan sleepDelay)
        {
            await Task.Yield();
            while (true)
            {
                var queueClient = CloudStorageAccount.CreateCloudQueueClient();
                var queue = queueClient.GetQueueReference("backend-processing");
                queue.CreateIfNotExists();
                var retrievedMessage = await queue.GetMessageAsync();
                if(retrievedMessage != null)
                {
                    Console.WriteLine("Retrieved message with content '{0}'", retrievedMessage.AsString);

                }


                await Task.Delay(sleepDelay);
                if(retrievedMessage != null)
                {
//                    await queue.DeleteMessageAsync(retrievedMessage);
//                    Console.WriteLine("Deleted message");

                }

            }
        }
 
    }
}
