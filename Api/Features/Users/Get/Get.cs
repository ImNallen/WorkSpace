using Api.Features.Abstractions;
using Api.Features.Users.Entities;
using Api.Persistence;
using Api.Services.Exceptions;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.Users.Get;

public record Query(Guid Id) : IQuery<UserDto>;

public class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/users/{id:guid}",
                async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                {
                    var query = new Query(id);

                    Result<UserDto> result = await sender.Send(query, cancellationToken);

                    return result.IsFailure
                        ? CustomResults.Problem(result)
                        : Results.Ok(result.Value);
                }
            )
            .WithTags(Tags.Users)
            .RequireAuthorization(Requires.Read);
    }
}

public class Handler(IUnitOfWork unitOfWork) : IQueryHandler<Query, UserDto>
{
    public async Task<Result<UserDto>> Handle(Query request, CancellationToken cancellationToken)
    {
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
