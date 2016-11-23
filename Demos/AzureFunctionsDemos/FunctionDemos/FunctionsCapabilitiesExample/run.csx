#load "Models.csx"
#r "BinaryModels.dll"
#r "Microsoft.WindowsAzure.Storage"
#r "Newtonsoft.Json"

using BinaryModels;
using System.Net;
using System.Spatial;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Table;

public static async Task<HttpResponseMessage> Run(
    GreetingModel greeting, 
    HttpRequestMessage req, 
    TextWriter logsBlob, 
    IAsyncCollector<GreetingLogs> logs, 
    TraceWriter log)
{
    var binaryUser = new BinaryUser { Name = "Alex" };
    var scriptUser = new ScriptUser { Name = "David" };

    var serialized = JsonConvert.SerializeObject(new
    {
        Data = "Some data"
    });

    var text = string.Format("Hey, {0}! Hello from {1} and {2}. Here is your data: {3}",
        greeting.Name, binaryUser.Name, scriptUser.Name, serialized);

    logsBlob.Write(text);
    
    await logs.AddAsync(new GreetingLogs
    {
        Text = text,
        PartitionKey = Guid.NewGuid().ToString(),
        RowKey = Guid.NewGuid().ToString()        
    });

    return req.CreateResponse(HttpStatusCode.OK, text);
}

public class GreetingModel
{
    public string Name { get; set; }
}

public class GreetingLogs
{
    public string PartitionKey {get;set;}
    public string RowKey { get;set;}
    public string Text {get;set;}
}