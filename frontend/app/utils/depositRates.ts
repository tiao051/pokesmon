// Mirrors backend/Core/Common/RarityDepositRates.cs. Keep both in sync.
const DEFAULT_RATE = 0.20;

const RATES: Record<string, number> = {
	common: 0.10,
	uncommon: 0.15,
	rare: 0.25,
	"holo rare": 0.30,
	"ultra rare": 0.40,
	"secret rare": 0.50,
	"hyper rare": 0.50,
};

export function depositRateFor(rarityName?: string | null): number {
	if (!rarityName) return DEFAULT_RATE;
	return RATES[rarityName.trim().toLowerCase()] ?? DEFAULT_RATE;
}
