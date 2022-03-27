using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.DAL.Models.Entities;

namespace Template.DAL.Configurations
{
    public class TaskConfiguration : IEntityTypeConfiguration<Task>
    {
        public void Configure(EntityTypeBuilder<Task> builder)
        {
            builder
                .HasKey(x => x.Id);

            builder
                .HasQueryFilter(x => !x.IsDeleted);

            builder
                .HasOne(x => x.Assignee)
                .WithMany(u => u.Tasks)
                .HasForeignKey(x => x.AssigneeId);

            builder
                .Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder
                .Property(x => x.Description)
                .HasMaxLength(250);
        }
    }
}