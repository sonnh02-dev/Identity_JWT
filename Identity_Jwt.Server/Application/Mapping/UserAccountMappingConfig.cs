using FluentEmail.Core;
using Identity_Jwt.Server.Application.DTOs.Responses;
using Identity_Jwt.Server.Infrastructure.Authentication.Entities;
using Mapster;
using Microsoft.AspNetCore.Identity.Data;

namespace Identity_Jwt.Server.Application.Mapping
{
    public static class UserAccountMappingConfig
    {
        public static void Register()
        {
            TypeAdapterConfig<UserAccount, UserAccountResponse>.NewConfig()
                .Map(dest => dest.Roles, src => new List<string>());

            TypeAdapterConfig<RegisterRequest, UserAccount>.NewConfig()
                .Map(dest => dest.UserName,
                     src => src.Email.Substring(0, src.Email.IndexOf('@'))
                    );



        }
    }
}
