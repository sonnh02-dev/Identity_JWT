using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace Identity_JWT.API.Extensions
{
    public static class ResultExtensions
    {
        public static IActionResult ToProblemDetails<T>(this Result<T> result, ControllerBase controller)
        {
            var error = result.Errors.First();

            var errorType = error.Metadata.TryGetValue("errorType", out var type)
                ? type?.ToString()
                : "Unexpected";

            return controller.Problem(
                statusCode: MapStatusCode(errorType),
                title: errorType,
                detail: error.Message,
                extensions: new Dictionary<string, object?>
                {
                    { "errors", result.Errors.Select(e => new { e.Message, e.Metadata }) }
                }
            );
        }

        public static IActionResult ToProblemDetails(this Result result, ControllerBase controller)
        {
            var error = result.Errors.First();

            var errorType = error.Metadata.TryGetValue("errorType", out var type)
                ? type?.ToString()
                : "Unexpected";

            return controller.Problem(
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
