using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Api.Services.Authorization;

public class PermissionAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
    : IAuthorizationPolicyProvider
{
    private readonly AuthorizationOptions _options = options.Value;

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        AuthorizationPolicy? policy = _options.GetPolicy(policyName);

        if (policy is not null)
        {
            return Task.FromResult<AuthorizationPolicy?>(policy);
        }

        policy = new AuthorizationPolicyBuilder()
            .AddRequirements(new PermissionRequirement(policyName))
            .Build();

        _options.AddPolicy(policyName, policy);

        return Task.FromResult<AuthorizationPolicy?>(policy);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
    {
        return Task.FromResult(_options.DefaultPolicy);
    }

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
    {
        return Task.FromResult(_options.FallbackPolicy);
    }
}
