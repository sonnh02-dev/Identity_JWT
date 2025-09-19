using Amazon.SimpleEmailV2;
using Amazon.SimpleEmailV2.Model;
using FluentResults;
using Identity_Jwt.Application.DTOs.Requests;
using Identity_JWT.Application.Abstractions.Email;

namespace Identity_Jwt.Infrastructure.Email.Ses
{
    public class SesEmailService : IEmailService
    {
        private readonly IAmazonSimpleEmailServiceV2 _sesClient;

        public SesEmailService(IAmazonSimpleEmailServiceV2 sesClient)
        {
            _sesClient = sesClient;
        }

        public async Task<Result> SendEmailAsync(EmailMessage message, CancellationToken cancellationToken = default)
        {
            try
            {

                var response = await _sesClient.SendEmailAsync(message.ToSendEmailRequest(), cancellationToken);

                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    return Result.Ok();
                }

                return Result.Fail($"Failed to send email. SES returned {response.HttpStatusCode}");
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }
    }
}
