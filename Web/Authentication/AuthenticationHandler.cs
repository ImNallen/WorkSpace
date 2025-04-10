using System.Net.Http.Headers;
using NetcodeHub.Packages.Extensions.SessionStorage;

namespace Web.Authentication;

public class AuthenticationHandler(ISessionStorageService sessionStorageService)
    : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        string? token = await sessionStorageService.GetEncryptedItemAsStringAsync("access-token");

        request.Headers.Authorization = new AuthenticationHeaderValue(
            "Bearer", token);

        return await base.SendAsync(request, cancellationToken);
    }
}
