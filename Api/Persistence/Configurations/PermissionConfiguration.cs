using Api.Features.Permissions.Entities;
using Api.Features.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Persistence.Configurations;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        _ = builder.HasKey(permission => permission.Id);

        _ = builder
            .Property(p => p.Name)
            .HasConversion(p => p.Value, v => Name.Create(v).Value)
            .HasColumnName(nameof(Name))
            .HasMaxLength(Name.MaxLength)
            .IsRequired();

        _ = builder.HasMany(p => p.RolePermissions).WithOne().HasForeignKey(rp => rp.PermissionId);

        _ = builder.HasData(Permission.Read, Permission.Write, Permission.Delete, Permission.Admin);
    }
}
