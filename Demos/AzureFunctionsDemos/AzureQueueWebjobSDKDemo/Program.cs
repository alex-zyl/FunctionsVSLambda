using Microsoft.Azure.WebJobs;

namespace AzureQueueWebjobSDKDemo
{
    class Program
    {
        static void Main()
        {
            JobHost host = new JobHost();
            host.RunAndBlock();
        }
    }
}