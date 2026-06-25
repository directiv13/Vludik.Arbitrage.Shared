using Vludik.Arbitrage.Events.Entities;

namespace Vludik.Arbitrage.Events;

public record SubscriptionCreatedEvent(
    Guid SubscriptionId,
    string ConnectionId,
    string Symbol,
    ExchangeRef BuyExchange,
    ExchangeRef SellExchange,
    long Timestamp
);
