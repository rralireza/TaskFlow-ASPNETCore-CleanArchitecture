using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Configurations;

public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(t => t.Status)
            .HasConversion<byte>()
            .IsRequired();

        builder.Property(t => t.Priority)
            .HasConversion<byte>()
            .IsRequired();

        builder.Property(t => t.Deadline)
            .IsRequired();

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.HasOne(p => p.Project)
            .WithMany(t => t.Tasks)
            .HasForeignKey(p => p.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(u => u.AssignedToUser)
            .WithMany(t => t.AssignedTasks)
            .HasForeignKey(u => u.AssignedToUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(c => c.Comments)
            .WithOne(t => t.TaskItem)
            .HasForeignKey(t => t.TaskItemId);

        builder.Property(t => t.InsertUser)
            .IsRequired();

        builder.HasOne(x => x.InsertUserDetails)
            .WithMany()
            .HasForeignKey(x => x.InsertUser)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
