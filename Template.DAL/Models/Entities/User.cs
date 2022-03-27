using System;
using System.Collections.Generic;
using Template.DAL.Models.Base;
using Microsoft.AspNetCore.Identity;

namespace Template.DAL.Models.Entities
{
    public class User : IdentityUser, IAuditableEntity
    {
        public List<Task> Tasks { get; } = new();
        public List<RefreshToken> RefreshTokens { get; } = new();

        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset? ModifiedOn { get; set; }
    }
}