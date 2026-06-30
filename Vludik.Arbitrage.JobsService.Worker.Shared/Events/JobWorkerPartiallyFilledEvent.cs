namespace Vludik.Arbitrage.JobsService.Worker.Shared.Events;

public record JobWorkerPartiallyFilledEvent(
    Guid JobId,
    decimal FilledAmount,
    decimal Price,
    long Timestamp
);
