export interface Product {
	id: number;
	slug: string;
	title: string;
	price: number;
	image: string;
	imageBase64?: string | null;
	description: string;
	stock: number;
	category: string;
	productTypeName: string;
	setName: string;
	rarityName: string;
	sealed: boolean;
	tags: string[];
}

// Re-export for backwards compatibility with existing imports.
export { formatPrice } from "../utils/format";
