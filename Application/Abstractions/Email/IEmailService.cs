using FluentResults;
using System.Net.Mail;

namespace Identity_JWT.Application.Abstractions.Email
{
    public interface IEmailService
    {
        Task<Result> SendEmailAsync(MailMessage mail);


    }
}
