using Api.Features.Abstractions;
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
