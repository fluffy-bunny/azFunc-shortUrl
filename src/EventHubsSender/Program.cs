using Azure.Identity;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EventHubsSender
{
    public class Message
    {
        public string Value { get; set; }
        public int Count { get; set; }
    }
    class Program
    {
        static string GuidS => Guid.NewGuid().ToString();
        static string ConnectionString { get; set; }
        static string EventHubNamespace = "evhns-shorturl2";
        static string EventHubName = "evh-shorturl2";
        static string KeyVaultName = "kv-shorturl2";
        static async Task Main(string[] args)
        {
            var keyVaultUrlBaseUrl = $"https://{KeyVaultName}.vault.azure.net/";
            var secretName = "sas-sender-evh-shorturl2-primary-connection-string";
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var keyVaultClient = new KeyVaultClient(
                    new KeyVaultClient.AuthenticationCallback(
                        azureServiceTokenProvider.KeyVaultTokenCallback));
            var secret = await keyVaultClient.GetSecretAsync($"{keyVaultUrlBaseUrl}secrets/{secretName}").ConfigureAwait(false);

            Console.WriteLine("https://docs.microsoft.com/en-us/azure/event-hubs/get-started-dotnet-standard-send-v2");
            Console.Write("Enter Connection String: ");

            string myFirstName;
            //            ConnectionString = Console.ReadLine();
            ConnectionString = secret.Value;
            var helloWorld = await GetHelloWorldAsync();
            Console.WriteLine(helloWorld);
        }
        //Endpoint=sb://<namespace>.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=<secret>
        static async Task<string> GetHelloWorldAsync()
        {
            //  await using (EventHubProducerClient producerClient = new EventHubProducerClient("evhns-shorturl-001", "evh-shorturl", new DefaultAzureCredential()))
            var dd = new DefaultAzureCredential();

            await using (EventHubProducerClient producerClient = new EventHubProducerClient(ConnectionString, EventHubName))

//            await using (EventHubProducerClient producerClient
//               = new EventHubProducerClient($"{EventHubNamespace}.servicebus.windows.net", EventHubName, new DefaultAzureCredential()))
            {
                for (int k = 0; k < 10; k++)
                {

                    // create a batch
                    using (EventDataBatch eventBatch = await producerClient.CreateBatchAsync())
                    {

                        for (int i = 0; i < 10; i++)
                        {
                            var msg = new Message
                            {
                                Value = $"test -{GuidS}",
                                Count = i
                            };
                            var json = JsonConvert.SerializeObject(msg);
                            // add events to the batch. only one in this case. 
                            eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes(json)));
                        }


                        // send the batch to the event hub
                        await producerClient.SendAsync(eventBatch);
                    }
                    Console.WriteLine($"Sent:{k}....");
                    Thread.Sleep(3000);
                }
            }
            return $"{DateTime.Now} - SENT{Environment.NewLine}";
        }
    }
}
