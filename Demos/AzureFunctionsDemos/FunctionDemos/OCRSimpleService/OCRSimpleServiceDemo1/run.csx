#r "Newtonsoft.Json"

using System;
using System.Text;
using System.Net;
using Newtonsoft.Json;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    var image = await req.Content.ReadAsByteArrayAsync();
    if (image == null)
    {
        return req.CreateResponse(HttpStatusCode.BadRequest);
    }

    var result = await RecognizeAsync(image);

    return req.CreateResponse(HttpStatusCode.OK, new
    {
        Result = result
    });
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

        string result = "";
        foreach (var parsedResult in root.ParsedResults)
        {
            result = result + parsedResult.ParsedText.Replace(System.Environment.NewLine, "");
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