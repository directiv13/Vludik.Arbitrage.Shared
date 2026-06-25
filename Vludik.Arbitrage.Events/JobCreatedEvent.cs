using Vludik.Arbitrage.Events.Entities;

namespace Vludik.Arbitrage.Events;

public record JobCreatedEvent(
    Guid JobId,
    string Symbol,
    ExchangeRef BuyExchange,
    ExchangeRef SellExchange,
    long Timestamp
);
