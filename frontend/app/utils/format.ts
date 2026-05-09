export function formatPrice(price: number): string {
	return `$${price.toFixed(2)}`;
}

export function formatDate(input: string | Date | null | undefined): string {
	if (!input) return "";
	const d = typeof input === "string" ? new Date(input) : input;
	if (Number.isNaN(d.getTime())) return "";
	return d.toLocaleDateString("en-US", {
		year: "numeric",
		month: "short",
		day: "numeric",
	});
}

export function deriveDisplayName(email: string | undefined | null): string {
	if (!email) return "Trainer";
	const local = email.split("@")[0] ?? "Trainer";
	return local.charAt(0).toUpperCase() + local.slice(1);
}
