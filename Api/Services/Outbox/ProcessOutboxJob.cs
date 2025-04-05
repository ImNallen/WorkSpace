using Api.Features.Abstractions;
using Api.Persistence;
using Api.Services.Serialization;
using Api.Services.Time;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Quartz;

namespace Api.Services.Outbox;

[DisallowConcurrentExecution]
internal sealed class ProcessOutboxJob(
    IUnitOfWork unitOfWork,
    IPublisher publisher,
    IDateTimeProvider dateTimeProvider,
    IOptions<OutboxOptions> options,
    ILogger<ProcessOutboxJob> logger
) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("Beginning processing of outbox messages");

        List<OutboxMessage> outboxMessages = await unitOfWork
            .OutboxMessages.Where(outboxMessage => outboxMessage.ProcessedOnUtc == null)
            .OrderBy(outboxMessage => outboxMessage.OccurredOnUtc)
            .Take(options.Value.BatchSize)
            .ToListAsync();

        foreach (OutboxMessage outboxMessage in outboxMessages)
        {
            logger.LogInformation("Processing outbox message {OutboxMessageId}", outboxMessage.Id);
            Exception? exception = null;
            try
            {
                IDomainEvent domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(
                    outboxMessage.Content,
                    SerializerSettings.Instance
                )!;

                await publisher.Publish(domainEvent);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "An error occurred while processing outbox message {OutboxMessageId}",
                    outboxMessage.Id
                );

                exception = ex;
            }

            _ = outboxMessage.Proccess(dateTimeProvider.UtcNow, exception);
        }

        _ = await unitOfWork.SaveChangesAsync();
    }
}
