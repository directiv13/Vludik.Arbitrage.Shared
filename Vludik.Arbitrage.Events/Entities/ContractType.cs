namespace Vludik.Arbitrage.Events.Entities;

/// <summary>
/// The kind of market a subscription targets.
/// </summary>
public enum ContractType
{
    /// <summary>Futures perpetual swap.</summary>
    Perpetual,

    /// <summary>Spot market.</summary>
    Spot
}
