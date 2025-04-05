using Api.Features.Roles.Entities;
using Api.Features.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        _ = builder.HasKey(role => role.Id);

        _ = builder
            .Property(r => r.Name)
            .HasConversion(r => r.Value, v => Name.Create(v).Value)
            .HasColumnName(nameof(Name))
            .HasMaxLength(Name.MaxLength)
            .IsRequired();

        _ = builder.HasMany(r => r.Users).WithOne(u => u.Role).HasForeignKey(u => u.RoleId);

        _ = builder
            .HasMany(r => r.RolePermissions)
            .WithOne(rp => rp.Role)
            .HasForeignKey(rp => rp.RoleId);

        _ = builder.HasData(Role.Admin, Role.User);
    }
}
