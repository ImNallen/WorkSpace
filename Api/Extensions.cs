using Api.Features.Abstractions;
using Api.Features.Permissions.Entities;
using Api.Features.Roles.Entities;
using Api.Features.Shared;
using Api.Persistence;
using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.EntityFrameworkCore;

namespace Api;

internal static class Extensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        using WorkSpaceDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<WorkSpaceDbContext>();

        dbContext.Database.Migrate();
    }

    public static async Task SeedData(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        using WorkSpaceDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<WorkSpaceDbContext>();

        // Get the logger instance
        ILogger<WorkSpaceDbContext> logger = scope.ServiceProvider.GetRequiredService<
            ILogger<WorkSpaceDbContext>
        >();

        logger.LogInformation("Starting database seeding process");

        // Only seed if DB is empty
        if (await dbContext.Permissions.AnyAsync() || await dbContext.Roles.AnyAsync())
        {
            logger.LogInformation("Database already seeded, skipping...");
            return;
        }

        // Seed permissions
        List<Permission> permissions =
        [
            Permission.Create(Name.Create("users:read").Value).Value,
            Permission.Create(Name.Create("users:write").Value).Value,
            Permission.Create(Name.Create("users:delete").Value).Value,
            Permission.Create(Name.Create("roles:read").Value).Value,
            Permission.Create(Name.Create("roles:write").Value).Value,
            Permission.Create(Name.Create("roles:delete").Value).Value,
        ];

        await dbContext.Permissions.AddRangeAsync(permissions);

        await dbContext.SaveChangesAsync();

        Role role = Role.Create(Name.Create("default").Value, permissions).Value;

        await dbContext.Roles.AddAsync(role);

        await dbContext.SaveChangesAsync();

        logger.LogInformation("Database seeding process completed successfully");
    }

    public static IApplicationBuilder MapEndpoints(this WebApplication app)
    {
        ApiVersionSet apiVersionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .ReportApiVersions()
            .Build();

        RouteGroupBuilder? versionedGroupBuilder = app.MapGroup("api/v{version:apiVersion}")
            .WithApiVersionSet(apiVersionSet);

        IEnumerable<IEndpoint> endpoints = app.Services.GetRequiredService<
            IEnumerable<IEndpoint>
        >();

        IEndpointRouteBuilder builder = versionedGroupBuilder;

        foreach (IEndpoint endpoint in endpoints)
        {
            endpoint.MapEndpoint(builder);
        }

        return app;
    }

    public static RouteHandlerBuilder HasPermission(this RouteHandlerBuilder app, string permission)
    {
        return app.RequireAuthorization(permission);
    }
}
