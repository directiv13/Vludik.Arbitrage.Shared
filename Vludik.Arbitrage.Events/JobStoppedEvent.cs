using Vludik.Arbitrage.Events.Entities;

namespace Vludik.Arbitrage.Events;
public record JobStoppedEvent(
    Guid JobId,
    string Symbol,
    ExchangeRef BuyExchange,
    ExchangeRef SellExchange,
    long Timestamp
);