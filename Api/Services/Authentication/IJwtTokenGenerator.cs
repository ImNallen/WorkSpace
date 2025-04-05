using Api.Features.Users.Entities;

namespace Api.Services.Authentication;

public interface IJwtTokenGenerator
{
    Token GenerateToken(User user);
}
