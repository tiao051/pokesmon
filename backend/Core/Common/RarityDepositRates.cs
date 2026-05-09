namespace backend.Core.Common;

public static class RarityDepositRates
{
    public const decimal Default = 0.20m;

    private static readonly Dictionary<string, decimal> Rates = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Common"] = 0.10m,
        ["Uncommon"] = 0.15m,
        ["Rare"] = 0.25m,
        ["Holo Rare"] = 0.30m,
        ["Ultra Rare"] = 0.40m,
        ["Secret Rare"] = 0.50m,
        ["Hyper Rare"] = 0.50m,
    };

    public static decimal RateFor(string? rarityName)
    {
        if (string.IsNullOrWhiteSpace(rarityName)) return Default;
        return Rates.TryGetValue(rarityName.Trim(), out var rate) ? rate : Default;
    }
}
