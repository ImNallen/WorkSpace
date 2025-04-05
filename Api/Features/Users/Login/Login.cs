using Api.Features.Abstractions;
using Api.Features.Users.Entities;
using Api.Persistence;
using Api.Services.Authentication;
using Api.Services.Encryption;
using Api.Services.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.Users.Login;

public record Request(string Email, string Password);

public record Command(string Email, string Password) : ICommand<Token>;

public class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/users/login",
                async (Request request, ISender sender) =>
                {
                    var command = new Command(request.Email, request.Password);

                    Result<Token> result = await sender.Send(command);

                    return result.IsFailure
                        ? CustomResults.Problem(result)
                        : Results.Ok(result.Value);
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
        RuleFor(c => c.Password).NotEmpty();
    }
}

public class Handler(
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator
) : ICommandHandler<Command, Token>
{
    public async Task<Result<Token>> Handle(Command request, CancellationToken cancellationToken)
    {
        // Check if the user exists
        User? user = await unitOfWork.Users.FirstOrDefaultAsync(
            u => (string)u.Email == request.Email,
            cancellationToken
        );

        if (user is null)
        {
            return Result.Failure<Token>(Errors.InvalidCredentials);
        }

        // Check if the password is valid
        bool isPasswordValid = passwordHasher.Verify(request.Password, user.Password.Value);

        if (!isPasswordValid)
        {
            return Result.Failure<Token>(Errors.InvalidCredentials);
        }

        // Generate a JWT token
        Token token = jwtTokenGenerator.GenerateToken(user);

        return token;
    }
}
