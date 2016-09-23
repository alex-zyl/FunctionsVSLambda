#r "Newtonsoft.Json"
#r "Microsoft.WindowsAzure.Storage"

using System;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req,
    CloudBlobContainer tasksBlobContainer,
    IAsyncCollector<TaskInfo> taskStatuses,
    IAsyncCollector<TaskInfo> taskInfoMessages, 
    TraceWriter log)
{
    var email = req.Headers.GetValues("email").First();
    
    var image = await req.Content.ReadAsByteArrayAsync();
    if (image == null || image.Length == 0)
    {
        return req.CreateResponse(HttpStatusCode.BadRequest);
    }

    var id = Guid.NewGuid().ToString();
    var infoMessage = new TaskInfo
    {
        Id = id,
        Email = email,
        Status = "Completed",
        PartitionKey = id,
        RowKey = id
    };

    string result = null;
    try
    {
        result = await RecognizeAsync(image);
    }
    catch (Exception)
    {
        infoMessage.Status = "Failed";
    }
    
    if(infoMessage.Status == "Completed")
    {
        await tasksBlobContainer.CreateIfNotExistsAsync();
        var blob = tasksBlobContainer.GetBlockBlobReference(id);

        var content = Encoding.Unicode.GetBytes(result);
        await blob.UploadFromByteArrayAsync(content, 0, content.Length);

        var token = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy
        {
            Permissions = SharedAccessBlobPermissions.Read,
            SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddDays(7)
        });

        infoMessage.ResultUri = blob.Uri.AbsoluteUri + token;
    }
    
    await taskInfos.AddAsync(infoMessage);
    await taskInfoMessages.AddAsync(infoMessage);

    return req.CreateResponse(HttpStatusCode.OK, new {
        Id = id
    });
}

public class TaskInfo
{
    public string Id {get;set;}
    public string Status { get; set; }
    public string Email {get;set;}
    public string ResultUri { get; set; }
}

private static async Task<string> RecognizeAsync(byte[] imageData)
{
    var apiKey = System.Configuration.ConfigurationManager.AppSettings["OCR.Space.Api.Key"];

    using (var client = new HttpClient())
    {
        var form = new MultipartFormDataContent();
        form.Add(new StringContent(apiKey), "apikey");
        form.Add(new ByteArrayContent(imageData, 0, imageData.Length), "image", "image.jpg");

        var response = await client.PostAsync("https://api.ocr.space/Parse/Image", form);

        var strContent = await response.Content.ReadAsStringAsync();
        var root = JsonConvert.DeserializeObject<Rootobject>(strContent);

        if (apiResult.OCRExitCode == 3 || apiResult.OCRExitCode == 4)
        {
            throw new Exception("Recognition failed");
        }

        string result = "";
        foreach (var parsedResult in root.ParsedResults)
        {
            result = result + parsedResult.ParsedText;
        }
        
        return result;
    }
}

public class Rootobject
{
    public Parsedresult[] ParsedResults { get; set; }
    public int OCRExitCode { get; set; }
    public bool IsErroredOnProcessing { get; set; }
    public string ErrorMessage { get; set; }
    public string ErrorDetails { get; set; }
}

public class Parsedresult
{
    public object FileParseExitCode { get; set; }
    public string ParsedText { get; set; }
    public string ErrorMessage { get; set; }
    public string ErrorDetails { get; set; }
}