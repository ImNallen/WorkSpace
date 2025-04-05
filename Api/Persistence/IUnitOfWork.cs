using Api.Features.Permissions.Entities;
using Api.Features.Roles.Entities;
using Api.Features.Users.Entities;
using Api.Services.Outbox;
using Microsoft.EntityFrameworkCore;

namespace Api.Persistence;

public interface IUnitOfWork
{
    DbSet<User> Users { get; }

    DbSet<Role> Roles { get; }

    DbSet<Permission> Permissions { get; }

    DbSet<OutboxMessage> OutboxMessages { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
