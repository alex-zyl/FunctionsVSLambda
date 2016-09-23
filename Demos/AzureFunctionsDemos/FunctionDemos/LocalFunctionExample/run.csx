using System;

public static void Run(string message, ICollector<Entity> tableMessages, TraceWriter log)
{
    log.Info($"Queue trigger function processed: {message}");

    tableMessages.Add(new Entity
    {
        PartitionKey = Guid.NewGuid().ToString(),
        RowKey = Guid.NewGuid().ToString(),
        Message = message
    });
}

public class Entity
{
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public string Message { get; set; }
}