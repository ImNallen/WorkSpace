namespace Api.Services.Time;

public interface IDateTimeProvider
{
    public DateTime UtcNow { get; }
}
