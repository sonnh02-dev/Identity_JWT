using Amazon.SimpleEmailV2.Model;
using Identity_Jwt.Application.DTOs.Requests;
using Microsoft.Extensions.Options;
using System.Net.Mail;

namespace Identity_Jwt.Infrastructure.Email
{
    public static class EmailMessageMapper
    {

        public static MailMessage ToMailMessage(this EmailMessage request)
        {
            var mail = new MailMessage
            {
                From = request.From,
                Subject = request.Subject,
                Body = request.Body,
                IsBodyHtml = request.IsBodyHtml
            };

            mail.To.Add(request.To);
            return mail;
        }
        public static SendEmailRequest ToSendEmailRequest(this EmailMessage request)
        {
            return new SendEmailRequest
            {
                FromEmailAddress = request.From.ToString(),
                Destination = new Destination
                {
                    ToAddresses = new List<string> { request.To }
                },
                Content = new EmailContent
                {
                    Simple = new Message
                    {
                        Subject = new Content
                        {
                            Data = request.Subject
                        },
                        Body = new Body
                        {
                            Html = request.IsBodyHtml ? new Content { Data = request.Body } : null,
                            Text = !request.IsBodyHtml ? new Content { Data = request.Body } : null
                        }
                    }
                }
            };
        }
    }
}