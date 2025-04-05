namespace Api.Services.Authentication;

public class Token
{
    public string AccessToken { get; init; } = null!;

    public DateTime ExpiresAt { get; init; }
}
