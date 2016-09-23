#r "SendGridMail"
#r "Microsoft.WindowsAzure.Storage"

using System;
using SendGrid;
using Microsoft.WindowsAzure.Storage.Blob;

public static void Run(TaskInfo task, out SendGridMessage message, TraceWriter log)
{
    if(task.Status == "Failed")
    {
        message = new SendGridMessage()
        {
            Subject = "Online recognition has failed!",
            Text = string.Format("We're sorry, please try again later")
        };
    }
    else
    {
        message = new SendGridMessage()
        {
            Subject = "Online recognition has been finished!",
            Text = string.Format("Result: {0}", task.ResultUri)
        };
    }
    
    message.AddTo(task.Email);
}

public class TaskInfo
{
    public string Id {get;set;}
    public string Email {get;set;}
    public string ResultUri { get; set; }
}