using MediatR;

namespace Api.Features.Abstractions;

public interface ITransactionalCommand : ICommand;

public interface ITransactionalCommand<TResponse>
    : IRequest<Result<TResponse>>,
        ITransactionalCommand;
