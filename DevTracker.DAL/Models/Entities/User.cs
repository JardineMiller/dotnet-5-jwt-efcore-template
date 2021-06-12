using System;
using System.Collections.Generic;
using DevTracker.DAL.Models.Base;
using Microsoft.AspNetCore.Identity;

namespace DevTracker.DAL.Models.Entities
{
    public class User : IdentityUser, IAuditableEntity
    {
        public List<RefreshToken> RefreshTokens { get; } = new List<RefreshToken>();
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset? ModifiedOn { get; set; }
    }
}
