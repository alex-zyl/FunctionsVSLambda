using System;
using System.Net;
using System.Net.Http;

public static HttpResponseMessage Run(string taskId, HttpRequestMessage req, TaskInfo task, TraceWriter log)
{
    return task == null
        ? req.CreateResponse(HttpStatusCode.NotFound)
        : req.CreateResponse(HttpStatusCode.OK, new TaskInfo
        {
            Id = task.Id,
            Status = task.Status,
            ResultUri = task.ResultUri
        });
}

public class TaskInfo
{
    public string Id { get; set; }
    public string Status { get; set; }
    public string ResultUri { get; set; }
}