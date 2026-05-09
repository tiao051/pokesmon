import {useApi} from "../composables/useApi";
import type {Product} from "../types/product";

export interface ProductListResponse {
	items: Product[];
	total: number;
	page: number;
	limit: number;
}

export interface ProductListParams {
	search?: string;
	productType?: string;
	setName?: string;
	maxPrice?: number;
	sealed?: boolean;
	isPreorder?: boolean;
	sort?: "newest" | "price-desc";
	page?: number;
	limit?: number;
}

export interface CategoriesResponse {
	productTypes: string[];
	setNames: string[];
}

export interface ProductDetailResponse {
	product: Product;
	related: Product[];
}

export const productService = {
	async getCategories(): Promise<CategoriesResponse> {
		const {data} = await useApi().get<CategoriesResponse>("/products/categories");
		return data;
	},

	async getProducts(params: ProductListParams = {}): Promise<ProductListResponse> {
		const {data} = await useApi().get<ProductListResponse>("/products", {params});
		return data;
	},

	async getProductBySlug(slug: string): Promise<ProductDetailResponse> {
		const {data} = await useApi().get<ProductDetailResponse>(`/products/${slug}`);
		return data;
	},
};
