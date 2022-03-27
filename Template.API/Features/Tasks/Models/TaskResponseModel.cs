
using Template.DAL.Models.Entities;

namespace Template.API.Features.Tasks.Models
{
    public record TaskResponseModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string AssigneeId { get; set; }

        public TaskResponseModel(Task task)
        {
            this.Name = task.Name;
            this.Description = task.Description;
            this.AssigneeId = task.AssigneeId;
        }
    }
}