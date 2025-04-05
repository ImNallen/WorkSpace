using Api.Features.Abstractions;
using Api.Features.Roles.Entities;
using Api.Features.Users.Entities;
using Api.Persistence;
using Api.Services.Encryption;
using Api.Services.Exceptions;
using Api.Services.Time;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.Users.Register;

public record Request(string Email, string Password, Guid RoleId);

public record Response(UserDto User);

public record Command(string Email, string Password, Guid RoleId) : ICommand<UserDto>;

public class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/users",
                async (Request request, ISender sender) =>
                {
                    var command = new Command(request.Email, request.Password, request.RoleId);

                    Result<UserDto> result = await sender.Send(command);

                    // TODO: When we have a Get by id endpoint, we can return Results.CreatedAtRoute(); instead of just Created();
                    return result.IsFailure ? CustomResults.Problem(result) : Results.Created();
                }
            )
            .WithTags(Tags.Users)
            .AllowAnonymous();
    }
}

public class Validator : AbstractValidator<Command>
{
    public Validator()
    {
        RuleFor(c => c.Email).NotEmpty().EmailAddress();
        RuleFor(c => c.Password).NotEmpty().MinimumLength(8);
        RuleFor(c => c.RoleId).NotEmpty();
    }
}

public class Handler(
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider,
    IPasswordHasher passwordHasher
) : ICommandHandler<Command, UserDto>
{
    public async Task<Result<UserDto>> Handle(Command request, CancellationToken cancellationToken)
    {
        // Check if the email is unique or not
        User? emailNotUnique = await unitOfWork
            .Users.Where(x => (string)x.Email == request.Email)
            .FirstOrDefaultAsync(cancellationToken);

        if (emailNotUnique is not null)
        {
            return Result.Failure<UserDto>(Errors.EmailNotUnique);
        }

        // Check if the role exists
        bool hasUsers = await unitOfWork.Users.AnyAsync(cancellationToken);

        Role? role = !hasUsers
            ? await unitOfWork
                .Roles.Where(x => x.Id == Role.Admin.Id)
                .FirstOrDefaultAsync(cancellationToken)
            : await unitOfWork
                .Roles.Where(x => x.Id == request.RoleId)
                .FirstOrDefaultAsync(cancellationToken);

        if (role is null)
        {
            return Result.Failure<UserDto>(Errors.RoleNotFound);
        }

        // Create the user and it's value objects
        Result<Email> emailResult = Email.Create(request.Email);

        if (emailResult.IsFailure)
        {
            return Result.Failure<UserDto>(emailResult.Error);
        }

        string hashedPassword = passwordHasher.Hash(request.Password);

        Result<Password> passwordResult = Password.Create(hashedPassword);

        if (passwordResult.IsFailure)
        {
            return Result.Failure<UserDto>(passwordResult.Error);
        }

        var user = User.Create(
            emailResult.Value,
            passwordResult.Value,
            role.Id,
            dateTimeProvider.UtcNow
        );

        unitOfWork.Users.Add(user);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new UserDto(user.Id, user.Email.Value, user.RoleId);
    }
}
