using Vludik.Arbitrage.Shared.Models;

namespace Vludik.Arbitrage.SubscriptionsService.Shared.Events;

public record SubscriptionCreatedEvent(
    Guid SubscriptionId,
    string ConnectionId,
    string Symbol,
    ExchangeRef BuyExchange,
    ExchangeRef SellExchange,
    long Timestamp
);
