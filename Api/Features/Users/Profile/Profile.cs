using System.Security.Claims;
using Api.Features.Abstractions;
using Api.Features.Users.Entities;
using Api.Persistence;
using Api.Services.Authentication;
using Api.Services.Exceptions;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.Users.Profile;

public record Query(Guid Id) : IQuery<UserDto>;

public class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/users/profile",
                async (
                    ClaimsPrincipal claims,
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                {
                    var query = new Query(claims.GetUserId());

                    Result<UserDto> result = await sender.Send(query, cancellationToken);

                    return result.IsFailure
                        ? CustomResults.Problem(result)
                        : Results.Ok(result.Value);
                }
            )
            .WithTags(Tags.Users)
            .RequireAuthorization(Requires.UsersRead);
    }
}

public class Handler(IUnitOfWork unitOfWork) : IQueryHandler<Query, UserDto>
{
    public async Task<Result<UserDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        // Check if the user exists
        UserDto? user = await unitOfWork
            .Users.Where(x => x.Id == request.Id)
            .Include(x => x.Role)
            .ProjectToType<UserDto>(TypeAdapterConfig.GlobalSettings)
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            return Result.Failure<UserDto>(Errors.NotFound(nameof(User)));
        }

        return user;
    }
}
