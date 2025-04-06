using Api.Features.Abstractions;
using Api.Features.Roles.Entities;
using Api.Persistence;
using Api.Services.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.Roles.Search;

public record Query(string? SearchTerm, int Page = 1, int PageSize = 25)
    : IQuery<PagedList<RoleDto>>;

public class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/roles",
                async (
                    string? searchTerm,
                    int page,
                    int pageSize,
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                {
                    var query = new Query(searchTerm, page, pageSize);

                    Result<PagedList<RoleDto>> result = await sender.Send(query, cancellationToken);

                    return result.IsFailure
                        ? CustomResults.Problem(result)
                        : Results.Ok(result.Value);
                }
            )
            .WithTags(Tags.Roles)
            .RequireAuthorization(Requires.RolesRead);
    }
}

public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<PagedList<RoleDto>>>
{
    public async Task<Result<PagedList<RoleDto>>> Handle(
        Query request,
        CancellationToken cancellationToken
    )
    {
        IQueryable<Role> query = unitOfWork.Roles.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(x => ((string)x.Name).Contains(request.SearchTerm));
        }

        int totalCount = await query.CountAsync(cancellationToken);

        List<RoleDto> roles = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new RoleDto(x.Id, x.Name.Value))
            .ToListAsync(cancellationToken);

        return new PagedList<RoleDto>(roles, request.Page, request.PageSize, totalCount);
    }
}
