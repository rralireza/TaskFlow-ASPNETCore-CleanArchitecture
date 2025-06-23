using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Content)
            .IsRequired()
            .HasMaxLength(400);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.HasOne(t => t.TaskItem)
            .WithMany(c => c.Comments)
            .HasForeignKey(t => t.TaskItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(u => u.User)
            .WithMany(c => c.Comments)
            .HasForeignKey(u => u.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
