using System.Net.Mail;
namespace Identity_Jwt.Application.DTOs.Requests
{
    public class EmailMessage
    {
        public MailAddress From { get; set; } = null!;
        public string To { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public string Body { get; set; } = null!;
        public bool IsBodyHtml { get; set; } = true;
    }
}
