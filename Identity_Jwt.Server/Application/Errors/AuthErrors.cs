using FluentResults;

namespace Identity_Jwt.Server.Application.Errors;

public static class AuthErrors
{
    public static readonly Error RefreshTokenNotFoundOrExpired =
    new Error("Refresh token not found or expired")
        .WithMetadata("errorCode", "Auth.RefreshToken.NotFoundOrExpired")
        .WithMetadata("errorType", "Unauthorized");

    public static readonly Error TwoFactorRequired =
        new Error("Two-factor authentication code required")
            .WithMetadata("errorCode", "Auth.2FARequired")
            .WithMetadata("errorType", "Unauthorized");

    public static readonly Error InvalidTwoFactorCode =
        new Error("Invalid two-factor code")
            .WithMetadata("errorCode", "Auth.Invalid2FACode")
            .WithMetadata("errorType", "Unauthorized");

    public static readonly Error OtpSendFailed =
        new Error("Failed to send OTP email")
            .WithMetadata("errorCode", "Auth.OTPSendFailed")
            .WithMetadata("errorType", "InternalError");

    public static readonly Error Unauthorized =
       new Error("User is not authorized")
           .WithMetadata("errorCode", " Auth.Unauthorized")
           .WithMetadata("errorType", "Unauthorized");
}
