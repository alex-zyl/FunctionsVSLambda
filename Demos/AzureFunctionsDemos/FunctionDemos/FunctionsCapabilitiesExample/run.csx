#load "Models.csx"
#r "BinaryModels.dll"
#r "Microsoft.WindowsAzure.Storage"
#r "Newtonsoft.Json"

using BinaryModels;
using System.Net;
using System.Spatial;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Table;

public static HttpResponseMessage Run(GreetingModel greeting, HttpRequestMessage req, CloudTable inputTable, TraceWriter log)
{
    var binaryUser = new BinaryUser { Name = "Alex" };
    var scriptUser = new ScriptUser { Name = "David" };

    var serialized = JsonConvert.SerializeObject(new
    {
        Data = "Some data"
    });

    return req.CreateResponse(HttpStatusCode.OK,
        string.Format("Hey, {0}! Hello from {1} and {2}. Here is your data: {3}",
        greeting.Name, binaryUser.Name, scriptUser.Name, serialized)
    );
}

public class GreetingModel
{
    public string Name { get; set; }
}