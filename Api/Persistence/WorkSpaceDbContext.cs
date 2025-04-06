using Api.Features.Abstractions;
using Api.Features.Permissions.Entities;
using Api.Features.Roles.Entities;
using Api.Features.Users.Entities;
using Api.Services.Outbox;
using Api.Services.Serialization;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Api.Persistence;

public class WorkSpaceDbContext(DbContextOptions<WorkSpaceDbContext> options)
    : DbContext(options),
        IUnitOfWork
{
    public DbSet<User> Users => Set<User>();

    public DbSet<Role> Roles => Set<Role>();

    public DbSet<Permission> Permissions => Set<Permission>();

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        InsertDomainEvents(this);

        int result = await base.SaveChangesAsync(cancellationToken);

        return result;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.ApplyConfigurationsFromAssembly(typeof(WorkSpaceDbContext).Assembly);

        _ = modelBuilder.HasDefaultSchema(Schemas.Default);
    }

    private static void InsertDomainEvents(DbContext context)
    {
        var domainEvents = context
            .ChangeTracker.Entries<Entity>()
            .Select(x => x.Entity)
            .SelectMany(x =>
            {
                IReadOnlyList<IDomainEvent> domainEvents = x.GetDomainEvents();

                x.ClearDomainEvents();

                return domainEvents;
            })
            .Select(d => new OutboxMessage
            {
                Id = d.Id,
                Type = d.GetType().Name,
                Content = JsonConvert.SerializeObject(d, SerializerSettings.Instance),
                OccurredOnUtc = d.OccurredOnUtc,
            })
            .ToList();

        context.Set<OutboxMessage>().AddRange(domainEvents);
    }
}
