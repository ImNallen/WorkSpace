using Api.Features.Abstractions;
using Api.Features.Permissions.Entities;
using Api.Features.Roles.Entities;
using Api.Features.Shared;
using Api.Persistence;
using Api.Services.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.Roles.Create;

public record Request(string Name, List<Guid> Permissions);

public record Command(string Name, List<Guid> Permissions) : ICommand<RoleDto>;

public class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/roles",
                async (Request request, ISender sender) =>
                {
                    var command = new Command(request.Name, request.Permissions);

                    Result<RoleDto> result = await sender.Send(command);

                    return result.IsFailure
                        ? CustomResults.Problem(result)
                        : Results.Ok(result.Value);
                }
            )
            .WithTags(Tags.Roles)
            .RequireAuthorization(Requires.RolesWrite);
    }
}

public class Validator : AbstractValidator<Command>
{
    public Validator()
    {
        RuleFor(c => c.Name).NotEmpty().MinimumLength(Name.MinLength).MaximumLength(Name.MaxLength);
    }
}

public class Handler(IUnitOfWork unitOfWork) : ICommandHandler<Command, RoleDto>
{
    public async Task<Result<RoleDto>> Handle(Command request, CancellationToken cancellationToken)
    {
        bool nameExists = await unitOfWork.Roles.AnyAsync(
            r => r.Name.Value == request.Name,
            cancellationToken
        );

        if (nameExists)
        {
            return Result.Failure<RoleDto>(Errors.NotUnique(request.Name));
        }

        Result<Name> nameResult = Name.Create(request.Name);

        if (nameResult.IsFailure)
        {
            return Result.Failure<RoleDto>(nameResult.Error);
        }

        List<Permission> permissions = await unitOfWork
            .Permissions.Where(p => request.Permissions.Contains(p.Id))
            .ToListAsync(cancellationToken);

        Result<Role> roleResult = Role.Create(nameResult.Value, permissions);

        if (roleResult.IsFailure)
        {
            return Result.Failure<RoleDto>(roleResult.Error);
        }

        unitOfWork.Roles.Add(roleResult.Value);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new RoleDto(roleResult.Value.Id, roleResult.Value.Name.Value);
    }
}
