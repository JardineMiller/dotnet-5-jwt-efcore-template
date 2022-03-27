using System;

namespace Template.DAL.Models.Base
{
    public interface IAuditableEntity
    {
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset? ModifiedOn { get; set; }
    }

    public class AuditableEntity : BaseEntity, IAuditableEntity
    {
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset? ModifiedOn { get; set; }
    }
}