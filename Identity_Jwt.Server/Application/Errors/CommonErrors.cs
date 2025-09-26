using FluentResults;

namespace Identity_Jwt.Server.Application.Errors;

public static class CommonErrors
{

    public static Error InternalServerError(string description = "An internal server error occurred") =>
         new Error(description)
             .WithMetadata("errorCode", "Common.InternalServerError")
             .WithMetadata("errorType", "Failure");


}
