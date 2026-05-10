export function formatPrice(price: number): string {
	return `$${price.toFixed(2)}`;
}

export function formatTimeAgo(input: string | Date | null | undefined): string {
	if (!input) return "";
	const d = typeof input === "string" ? new Date(input) : input;
	if (Number.isNaN(d.getTime())) return "";
	const seconds = Math.max(0, Math.floor((Date.now() - d.getTime()) / 1000));
	if (seconds < 60) return "just now";
	const minutes = Math.floor(seconds / 60);
	if (minutes < 60) return `${minutes}m ago`;
	const hours = Math.floor(minutes / 60);
	if (hours < 24) return `${hours}h ago`;
	const days = Math.floor(hours / 24);
	if (days < 30) return `${days}d ago`;
	const months = Math.floor(days / 30);
	if (months < 12) return `${months}mo ago`;
	const years = Math.floor(days / 365);
	return `${years}y ago`;
}
