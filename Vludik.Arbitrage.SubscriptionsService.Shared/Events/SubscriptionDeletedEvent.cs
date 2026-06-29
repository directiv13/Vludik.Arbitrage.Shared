using Vludik.Arbitrage.Shared.Models;

namespace Vludik.Arbitrage.SubscriptionsService.Shared.Events;

public record SubscriptionDeletedEvent(
    Guid SubscriptionId,
    string ConnectionId,
    string Symbol,
    ExchangeRef BuyExchange,
    ExchangeRef SellExchange,
    string Reason, // "unsubscribed" | "expired"
    long Timestamp
);

