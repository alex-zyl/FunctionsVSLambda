#r "SendGrid"

using System;
using SendGrid.Helpers.Mail;

public static Mail Run(TaskInfo task, TraceWriter log)
{
    var message = new Mail();
    Content content = new Content
    {
        Type = "text/plain"
    };

    if (task.Status == "Failed")
    {
        message.Subject = "Online recognition has failed!";
        content.Value = string.Format("We're sorry, please try again later");
    }
    else
    {
        message.Subject = "Online recognition has been finished!";
        content.Value = string.Format("Result: {0}", task.ResultUri);
    }

    var personalization = new Personalization();
    personalization.AddTo(new Email(task.Email));
    message.AddPersonalization(personalization);
    message.AddContent(content);

    return message;
}

public class TaskInfo
{
    public string Id {get;set;}
	public string Status {get;set;}
    public string Email {get;set;}
    public string ResultUri { get; set; }
}