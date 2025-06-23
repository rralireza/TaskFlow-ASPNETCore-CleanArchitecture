using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(d => d.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.HasOne(u => u.CreatedByUser)
            .WithMany(p => p.ProjectsCreated)
            .HasForeignKey(u => u.CreatedByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(t => t.Tasks)
            .WithOne(p => p.Project)
            .HasForeignKey(p => p.ProjectId);
    }
}
