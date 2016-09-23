using Microsoft.WindowsAzure.Storage.Table;

namespace AzureQueueWebjobWithoutSDKDemo
{
    public class Order : TableEntity
    {
        public string Name { get; set; }
        public string OrderId { get; set; }
    }
}