export interface Product {
	id: number;
	slug: string;
	title: string;
	price: number;
	image: string;
	description: string;
	stock: number;
	category: string;
	productTypeName: string;
	setName: string;
	rarityName: string;
	sealed: boolean;
	tags: string[];
}

export function formatPrice(price: number): string {
	return `$${price.toFixed(2)}`;
}
