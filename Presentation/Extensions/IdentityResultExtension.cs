using FluentResults;
using Microsoft.AspNetCore.Identity;

namespace Identity_JWT.API.Extensions
{
    internal static class IdentityResultExtension
    {
        public static IResult ToProblemDetails(this IdentityResult result)
        {
            if (result.Succeeded)
                throw new InvalidOperationException("Cannot map a success result to ProblemDetails");

            var error = result.Errors.FirstOrDefault();

            return Results.Problem(
                statusCode: MapStatusCode(error?.Code),
                title: error?.Code ?? "Identity error",
                detail: error?.Description,
                extensions: new Dictionary<string, object?>
                {
            { "errors", result.Errors }
                });
        }

        private static int MapStatusCode(string? errorCode) =>
            errorCode switch
            {
                "DuplicateUserName" or "DuplicateEmail" => StatusCodes.Status409Conflict,
                "InvalidToken" or "InvalidEmail" => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status400BadRequest
            };

    }
    public static class ResultExtensions
    {
        public static IResult ToProblemDetails<T>(this Result<T> result)
        {
            var error = result.Errors.First();

            var errorType = error.Metadata.TryGetValue("errorType", out var type)
                ? type?.ToString()
                : "Unexpected";

            return Results.Problem(
                statusCode: MapStatusCode(errorType),
                title: errorType,
                detail: error.Message,
                extensions: new Dictionary<string, object?>
                {
                { "errors", result.Errors.Select(e => new { e.Message, e.Metadata }) }
                }
            );
        }

        private static int MapStatusCode(string? type) =>
            type switch
            {
                "Validation" => StatusCodes.Status400BadRequest,
                "Conflict" => StatusCodes.Status409Conflict,
                "NotFound" => StatusCodes.Status404NotFound,
                "Unauthorized" => StatusCodes.Status401Unauthorized,
                "Forbidden" => StatusCodes.Status403Forbidden,
                _ => StatusCodes.Status500InternalServerError
            };
    }

}
