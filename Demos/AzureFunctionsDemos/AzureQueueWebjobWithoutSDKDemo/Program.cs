using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace AzureQueueWebjobWithoutSDKDemo
{
    class Program
    {
        static void Main()
        {
            var tokenSource = new CancellationTokenSource();

            var p = new Program();
            var task = Task.Factory.StartNew(async () => await p.ProcessAsync(tokenSource.Token), tokenSource.Token);

            Console.ReadLine();
            tokenSource.Cancel();
        }

        public async Task ProcessAsync(CancellationToken token)
        {
            var queue = await InitializeQueue();
            var table = await InitializeTable();

            while (true)
            {
                token.ThrowIfCancellationRequested();

                var messages = await queue.GetMessagesAsync(10, token);
                if (!messages.Any())
                {
                    await Task.Delay(TimeSpan.FromMinutes(1), token);
                    continue;
                }

                var batch = new TableBatchOperation();

                foreach (var message in messages)
                {
                    var order = JsonConvert.DeserializeObject<Order>(message.AsString);
                    order.PartitionKey = Guid.NewGuid().ToString();

                    batch.Add(TableOperation.Insert(order));
                }

                await table.ExecuteBatchAsync(batch, token);
            }
        }

        private async Task<CloudQueue> InitializeQueue()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"].ConnectionString);

            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference("orders");

            await queue.CreateIfNotExistsAsync();
            return queue;
        }

        private async Task<CloudTable> InitializeTable()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"].ConnectionString);

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("orders");

            await table.CreateIfNotExistsAsync();
            return table;
        }
    }

    public class Order : TableEntity
    {
        public string Name { get; set; }
        public string OrderId { get; set; }
    }
}