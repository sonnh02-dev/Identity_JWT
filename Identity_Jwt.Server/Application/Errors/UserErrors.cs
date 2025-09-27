using FluentResults;

namespace Identity_Jwt.Server.Application.Errors;

public static class UserErrors
{
    public static readonly Error NotFound =
        new Error("User not found")
            .WithMetadata("errorCode", "User.NotFound")
            .WithMetadata("errorType", "NotFound");

    public static readonly Error EmailNotConfirmed =
        new Error("You must confirm your email to login")
            .WithMetadata("errorCode", "User.EmailNotConfirmed")
            .WithMetadata("errorType", "Validation");

    public static Error InvalidPassword(int remaining) =>
     new Error($"Wrong password. You have {remaining} attempt(s) remaining !")
         .WithMetadata("errorCode", "User.InvalidPassword")
         .WithMetadata("errorType", "Validation");


    public static readonly Error AccountLocked =
    new Error("User account is locked")
        .WithMetadata("errorCode", "User.AccountLocked")
        .WithMetadata("errorType", "Locked");

}
