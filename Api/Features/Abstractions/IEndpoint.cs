namespace Api.Features.Abstractions;

internal interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}
