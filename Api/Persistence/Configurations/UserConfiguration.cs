using Api.Features.Shared;
using Api.Features.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        _ = builder.HasKey(u => u.Id);

        _ = builder
            .Property(u => u.Email)
            .HasConversion(e => e.Value, v => Email.Create(v).Value)
            .HasColumnName(nameof(Email))
            .HasMaxLength(Email.MaxLength)
            .IsRequired();

        _ = builder
            .Property(u => u.Password)
            .HasConversion(p => p.Value, v => Password.Create(v).Value)
            .HasColumnName(nameof(Password))
            .HasMaxLength(Password.MaxLength)
            .IsRequired();

        _ = builder
            .Property(u => u.FirstName)
            .HasConversion(n => n.Value, v => Name.Create(v).Value)
            .HasColumnName(nameof(User.FirstName))
            .HasMaxLength(Name.MaxLength)
            .IsRequired();

        _ = builder
            .Property(u => u.LastName)
            .HasConversion(n => n.Value, v => Name.Create(v).Value)
            .HasColumnName(nameof(User.LastName))
            .HasMaxLength(Name.MaxLength)
            .IsRequired();

        _ = builder.HasIndex(u => u.Email).IsUnique();

        _ = builder
            .HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .IsRequired();
    }
}
