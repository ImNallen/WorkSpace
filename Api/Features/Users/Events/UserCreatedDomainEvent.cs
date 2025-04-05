using Api.Features.Abstractions;
using Api.Features.Users.Entities;

namespace Api.Features.Users.Events;

public sealed record UserCreatedDomainEvent(Guid Id, DateTime OccurredOnUtc, User User) : IDomainEvent;
