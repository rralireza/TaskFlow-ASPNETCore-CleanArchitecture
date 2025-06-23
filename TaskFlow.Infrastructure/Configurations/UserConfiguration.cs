using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(n => n.Fullname)
            .IsRequired()
            .HasMaxLength(90);

        builder.Property(e => e.Email)
            .IsRequired();

        builder.HasIndex(e => e.Email)
            .IsUnique();

        builder.Property(p => p.PasswordHash)
            .IsRequired();

        builder.Property(r => r.Role)
            .IsRequired();

        builder.HasMany(p => p.ProjectsCreated)
            .WithOne(u => u.CreatedByUser)
            .HasForeignKey(u => u.CreatedByUserId);

        builder.HasMany(t => t.AssignedTasks)
            .WithOne(u => u.AssignedToUser)
            .HasForeignKey(u => u.AssignedToUserId);

        builder.HasMany(c => c.Comments)
            .WithOne(u => u.User)
            .HasForeignKey(u => u.UserId);
    }
}
