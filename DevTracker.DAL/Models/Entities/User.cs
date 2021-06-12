using System;
using DevTracker.DAL.Models.Base;
using Microsoft.AspNetCore.Identity;

namespace DevTracker.DAL.Models.Entities
{
    public class User : IdentityUser, IAuditableEntity
    {
        public DateTimeOffset CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
    }
}
