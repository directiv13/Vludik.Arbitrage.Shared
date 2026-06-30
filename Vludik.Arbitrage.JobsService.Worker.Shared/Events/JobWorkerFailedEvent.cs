namespace Vludik.Arbitrage.JobsService.Worker.Shared.Events;

public record JobWorkerFailedEvent(
    Guid JobId,
    string Message,
    long Timestamp
);
