using Template.DAL.Models.Base;

namespace Template.DAL.Models.Entities
{
    public class Task : SoftDeletableEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public string AssigneeId { get; set; }
        public User Assignee { get; set; }
    }
}