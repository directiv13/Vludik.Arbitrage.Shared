using Vludik.Arbitrage.JobsService.Shared.Enums;
using Vludik.Arbitrage.Shared.Models;

namespace Vludik.Arbitrage.JobsService.Shared.Events;

public record JobDeletedEvent(
    Guid JobId,
    string Symbol,
    ExchangeRef BuyExchange,
    ExchangeRef SellExchange,
    JobMode JobMode,
    MarginType MarginType,
    short Leverage,
    decimal SpreadPercent,
    decimal Size,
    decimal ChunkSize,
    long Timestamp
);
