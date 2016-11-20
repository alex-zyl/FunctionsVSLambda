using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureQueueWebjobSDKDemo
{
    public class Functions
    {
        public static Task MultipleOutput(
            [QueueTrigger("orders")] Order order, 
            [Table("orders")] IAsyncCollector<Order> tableOrder)
        {
            order.PartitionKey = Guid.NewGuid().ToString();
            return tableOrder.AddAsync(order);
        }
    }

    public class Order : TableEntity
    {
        public string Name { get; set; }
        public string OrderId { get; set; }
    }
}