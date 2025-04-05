using Api.Features.RolePermissions.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Persistence.Configurations;

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        _ = builder.HasKey(rolePermission => rolePermission.Id);

        _ = builder
            .HasOne(rp => rp.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(rp => rp.RoleId);

        _ = builder
            .HasOne(rp => rp.Permission)
            .WithMany(p => p.RolePermissions)
            .HasForeignKey(rp => rp.PermissionId);

        _ = builder.HasData(
            RolePermission.UserRead,
            RolePermission.UserWrite,
            RolePermission.UserDelete,
            RolePermission.AdminRead,
            RolePermission.AdminWrite,
            RolePermission.AdminDelete,
            RolePermission.AdminAdmin
        );
    }
}
