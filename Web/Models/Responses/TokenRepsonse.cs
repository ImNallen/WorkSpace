namespace Web.Models.Responses;

public record TokenResponse(string AccessToken, DateTime ExpiresAt);
