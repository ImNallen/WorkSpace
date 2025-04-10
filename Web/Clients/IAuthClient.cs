using Refit;
using Web.Models;
using Web.Models.Responses;

namespace Web.Clients;

public interface IAuthClient
{
    [Post("/api/v1/users/login")]
    Task<ApiResponse<TokenResponse>> Login(LoginModel loginModel);
}
