using Microsoft.Azure.ServiceBus;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus_Sender
{
    class Program
    {
        static string GuidS => Guid.NewGuid().ToString();
        static string ConnectionString { get; set; }
        static IQueueClient queueClient;
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

            var helloWorld = await GetHelloWorldAsync();
            Console.WriteLine(helloWorld);
        }
        static async Task<string> GetHelloWorldAsync()
        {
            const int numberOfMessages = 10;
            queueClient = new QueueClient(ConnectionString, "sbque-shorturl");

            Console.WriteLine("======================================================");
            Console.WriteLine("Press ENTER key to exit after sending all the messages.");
            Console.WriteLine("======================================================");

            // Send messages.
            await SendMessagesAsync(numberOfMessages);

            Console.ReadKey();

            await queueClient.CloseAsync();
            return "";
        }
        static async Task SendMessagesAsync(int numberOfMessagesToSend)
        {
            try
            {
                for (var i = 0; i < numberOfMessagesToSend; i++)
                {
                    // Create a new message to send to the queue.
                    string messageBody = $"Message {i}";
                    var message = new Message(Encoding.UTF8.GetBytes(messageBody));

                    // Write the body of the message to the console.
                    Console.WriteLine($"Sending message: {messageBody}");

                    // Send the message to the queue.
                    await queueClient.SendAsync(message);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
            }
        }
    }
}
