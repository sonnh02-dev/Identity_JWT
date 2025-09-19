using Amazon.SimpleEmailV2.Model;
using FluentResults;
using Identity_Jwt.Application.DTOs.Requests;
using System.Net.Mail;

namespace Identity_JWT.Application.Abstractions.Email
{
    public interface IEmailService
    {
        Task<Result> SendEmailAsync(EmailMessage request, CancellationToken cancellationToken = default);


    }
}
