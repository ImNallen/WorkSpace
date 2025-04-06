using Api.Features.Abstractions;
using Api.Features.Roles.Entities;
using Api.Features.Shared;
using Api.Features.Users.Entities;
using Api.Persistence;
using Api.Services.Encryption;
using Api.Services.Exceptions;
using Api.Services.Time;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.Users.Register;

public record Request(
    string Email,
    string Password,
    Guid RoleId,
    string FirstName,
    string LastName
);

public record Command(string Email, string Password, Guid RoleId, string FirstName, string LastName)
    : ICommand<UserDto>;

public class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/users",
                async (Request request, ISender sender) =>
                {
                    var command = new Command(
                        request.Email,
                        request.Password,
                        request.RoleId,
                        request.FirstName,
                        request.LastName
                    );

                    Result<UserDto> result = await sender.Send(command);

                    return result.IsFailure
                        ? CustomResults.Problem(result)
                        : Results.CreatedAtRoute(
                            "GetUserById",
                            new { id = result.Value.Id },
                            result.Value
                        );
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
        RuleFor(c => c.FirstName)
            .NotEmpty()
            .MinimumLength(Name.MinLength)
            .MaximumLength(Name.MaxLength);
        RuleFor(c => c.LastName)
            .NotEmpty()
            .MinimumLength(Name.MinLength)
            .MaximumLength(Name.MaxLength);
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

        Role? role = await unitOfWork
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

        Result<Name> firstNameResult = Name.Create(request.FirstName);

        if (firstNameResult.IsFailure)
        {
            return Result.Failure<UserDto>(firstNameResult.Error);
        }

        Result<Name> lastNameResult = Name.Create(request.LastName);

        if (lastNameResult.IsFailure)
        {
            return Result.Failure<UserDto>(lastNameResult.Error);
        }

        var user = User.Create(
            emailResult.Value,
            passwordResult.Value,
            role.Id,
            firstNameResult.Value,
            lastNameResult.Value,
            dateTimeProvider.UtcNow
        );

        unitOfWork.Users.Add(user);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new UserDto(
            user.Id,
            user.Email.Value,
            user.FirstName.Value,
            user.LastName.Value,
            user.Role.Name.Value,
            user.CreatedOnUtc
        );
    }
}
