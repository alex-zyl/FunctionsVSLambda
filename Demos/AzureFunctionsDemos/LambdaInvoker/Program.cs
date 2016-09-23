using System;
using Amazon.Lambda;
using Amazon.Lambda.Model;
using Amazon.Runtime;

namespace LambdaInvoker
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new AmazonLambdaClient(new AppConfigAWSCredentials());
            var response = client.Invoke(new InvokeRequest
            {
                FunctionName = "LambdaExample"
            });

            Console.WriteLine(response.Payload);
            Console.Read();
        }
    }
}