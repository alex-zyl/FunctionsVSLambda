using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;

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
}