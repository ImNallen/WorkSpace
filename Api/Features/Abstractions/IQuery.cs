using MediatR;

namespace Api.Features.Abstractions;

public interface IQuery<TResponse> : IRequest<Result<TResponse>> { }
