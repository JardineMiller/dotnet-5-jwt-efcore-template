using System;

namespace DevTracker.DAL.Models.Base
{
    public interface IAuditableEntity
    {
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset? ModifiedOn { get; set; }
    }

    public class AuditableEntity : BaseEntity, IAuditableEntity
    {
        public string ModifiedBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset? ModifiedOn { get; set; }
    }
}
