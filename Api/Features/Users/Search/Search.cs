using System.Globalization;
using System.Linq.Expressions;
using Api.Features.Abstractions;
using Api.Features.Users.Entities;
using Api.Persistence;
using Api.Services.Exceptions;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.Users.Search;

public record Query(
    string? SearchTerm,
    string? SortColumn,
    string? SortOrder,
    int Page = 1,
    int PageSize = 25
) : IQuery<PagedList<UserDto>>;

public class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/users",
                async (
                    string? searchTerm,
                    string? sortColumn,
                    string? sortOrder,
                    int page,
                    int pageSize,
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                {
                    var query = new Query(searchTerm, sortColumn, sortOrder, page, pageSize);

                    Result<PagedList<UserDto>> result = await sender.Send(query, cancellationToken);

                    return result.IsFailure
                        ? CustomResults.Problem(result)
                        : Results.Ok(result.Value);
                }
            )
            .WithTags(Tags.Users)
            .RequireAuthorization(Requires.Read);
    }
}

public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<PagedList<UserDto>>>
{
    public async Task<Result<PagedList<UserDto>>> Handle(
        Query request,
        CancellationToken cancellationToken
    )
    {
        IQueryable<User> query = unitOfWork.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(x =>
                x.Id.ToString().Contains(request.SearchTerm)
                || ((string)x.Email).Contains(request.SearchTerm)
            );
        }

        Expression<Func<User, object>> keySelector = request.SortColumn?.ToLower(
            CultureInfo.CurrentCulture
        ) switch
        {
            "email" => c => c.Email,
            _ => c => c.Id,
        };

        query =
            request.SortOrder?.ToLower(CultureInfo.CurrentCulture) == "desc"
                ? query.OrderByDescending(keySelector)
                : query.OrderBy(keySelector);

        int totalCount = await query.CountAsync(cancellationToken);

        List<UserDto> users = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectToType<UserDto>(TypeAdapterConfig.GlobalSettings)
            .ToListAsync(cancellationToken);

        return new PagedList<UserDto>(users, request.Page, request.PageSize, totalCount);
    }
}
