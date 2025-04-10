using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using NetcodeHub.Packages.Extensions.SessionStorage;

namespace Web.Authentication;

public class CustomAuthStateProvider(ISessionStorageService sessionStorageService)
    : AuthenticationStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var identity = new ClaimsIdentity();

        string? accessToken = await sessionStorageService.GetEncryptedItemAsStringAsync(
            "access-token"
        );

        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            var token = new JwtSecurityToken(
                accessToken.Replace("\"", string.Empty, StringComparison.InvariantCulture)
            );

            if (IsTokenExpired(token))
            {
                await LogoutUser();
            }
            else
            {
                try
                {
                    identity = new ClaimsIdentity(
                        [
                            new Claim(
                                ClaimTypes.Email,
                                token.Claims.First(c => c.Type == "email").Value
                            ),
                            new Claim(
                                ClaimTypes.GivenName,
                                token.Claims.First(c => c.Type == "given_name").Value
                            ),
                            new Claim(
                                ClaimTypes.Surname,
                                token.Claims.First(c => c.Type == "family_name").Value
                            ),
                            new Claim("access_token", accessToken),
                        ],
                        "jwt"
                    );
                }
                catch (Exception)
                {
                    await LogoutUser();
                }
            }
        }

        var user = new ClaimsPrincipal(identity);
        var state = new AuthenticationState(user);

        NotifyAuthenticationStateChanged(Task.FromResult(state));

        return state;
    }

    private static bool IsTokenExpired(JwtSecurityToken token)
    {
        return token.ValidTo < DateTime.UtcNow;
    }

    private async Task LogoutUser()
    {
        await sessionStorageService.DeleteItemAsync("access-token");
        var identity = new ClaimsIdentity();
        var user = new ClaimsPrincipal(identity);
        var state = new AuthenticationState(user);
        NotifyAuthenticationStateChanged(Task.FromResult(state));
    }
}
