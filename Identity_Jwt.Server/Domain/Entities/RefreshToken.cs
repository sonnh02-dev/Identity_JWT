using FluentResults;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace Identity_Jwt.Server.Domain.Entities
{
    public class RefreshToken
    {
        [Key]
        public string Token { get;set; } = null!;
        public int UserAccountId { get;set; }
        public int ExpiryDays { get;set; }
        public DateTime ExpiryDate { get;set; }
     
       
    }

}
