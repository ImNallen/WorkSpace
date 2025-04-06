using Api.Features.Abstractions;
using Api.Features.Users.Entities;
using Api.Persistence;
using Api.Services.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.Users.Delete;

public record Command(Guid Id) : ICommand;

public class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete(
                "/users/{id:guid}",
                async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                {
                    var command = new Command(id);

                    Result result = await sender.Send(command, cancellationToken);

                    return result.IsFailure ? CustomResults.Problem(result) : Results.NoContent();
                }
            )
            .WithTags(Tags.Users)
            .RequireAuthorization(Requires.Delete);
    }
}

public class Handler(IUnitOfWork unitOfWork) : ICommandHandler<Command>
{
    public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
    {
        User? user = await unitOfWork
            .Users.Where(x => x.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            return Result.Failure(Errors.NotFound(nameof(User)));
        }

        unitOfWork.Users.Remove(user);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
