using Identity_Jwt.Server.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Identity_Jwt.Server.Infrastructure.Authentication.Entities
{
    public partial class UserAccount : IdentityUser<int>
    {
        public int CreatorId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? ModifierId { get; set; }
        public DateTime? ModifiedAt { get; set; }

        public int? DeleterId { get; set; }
        public DateTime? DeletedAt { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } = new byte[] { 0 }; 

    }
}
