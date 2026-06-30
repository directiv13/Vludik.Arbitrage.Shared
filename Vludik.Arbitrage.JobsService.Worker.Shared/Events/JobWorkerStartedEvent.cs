namespace Vludik.Arbitrage.JobsService.Worker.Shared.Events;

public record JobWorkerStartedEvent(
    Guid JobId,
    long Timestamp
);
