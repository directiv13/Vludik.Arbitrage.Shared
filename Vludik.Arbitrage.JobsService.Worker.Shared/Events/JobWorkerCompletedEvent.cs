namespace Vludik.Arbitrage.JobsService.Worker.Shared.Events;

public record JobWorkerCompletedEvent(Guid JobId, long Timestamp);
