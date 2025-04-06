using Api.Features.Abstractions;
using Api.Features.Permissions.Entities;
using Api.Persistence;
using Api.Services.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.Permissions.Search;

public record Query(string? SearchTerm, int Page = 1, int PageSize = 25)
    : IQuery<PagedList<PermissionDto>>;

public class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/permissions",
                async (
                    string? searchTerm,
                    int page,
                    int pageSize,
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                {
                    var query = new Query(searchTerm, page, pageSize);

                    Result<PagedList<PermissionDto>> result = await sender.Send(
                        query,
                        cancellationToken
                    );

                    return result.IsFailure
                        ? CustomResults.Problem(result)
                        : Results.Ok(result.Value);
                }
            )
            .WithTags(Tags.Permissions)
            .RequireAuthorization(Requires.PermissionsRead);
    }
}

public class Handler(IUnitOfWork unitOfWork)
    : IRequestHandler<Query, Result<PagedList<PermissionDto>>>
{
    public async Task<Result<PagedList<PermissionDto>>> Handle(
        Query request,
        CancellationToken cancellationToken
    )
    {
        IQueryable<Permission> query = unitOfWork.Permissions.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(x => ((string)x.Name).Contains(request.SearchTerm));
        }

        int totalCount = await query.CountAsync(cancellationToken);

        List<PermissionDto> permissions = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new PermissionDto(x.Id, x.Name.Value))
            .ToListAsync(cancellationToken);

        return new PagedList<PermissionDto>(
            permissions,
            request.Page,
            request.PageSize,
            totalCount
        );
    }
}
