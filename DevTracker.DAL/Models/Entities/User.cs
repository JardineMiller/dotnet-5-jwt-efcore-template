using System;
using DevTracker.DAL.Models.Base;
using Microsoft.AspNetCore.Identity;

namespace DevTracker.DAL.Models.Entities
{
    public class User : IdentityUser, IAuditableEntity
    {
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset? ModifiedOn { get; set; }
    }
}
