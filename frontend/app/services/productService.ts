import axios, { type AxiosInstance } from "axios";
import type { Product } from "../types/product";

const API_BASE_URL = "http://localhost:5044/api";

let _api: AxiosInstance | null = null;

function getApi(): AxiosInstance {
	if (_api) return _api;
	_api = axios.create({
		baseURL: API_BASE_URL,
		headers: {
			"Content-Type": "application/json",
		},
	});
	return _api;
}

export interface ProductListResponse {
	items: Product[];
	total: number;
	page: number;
	limit: number;
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
	/**
	 * Lấy danh sách loại sản phẩm & series để filter
	 */
	async getCategories(): Promise<CategoriesResponse> {
		try {
			const response = await getApi().get<CategoriesResponse>("/products/categories");
			return response.data;
		} catch (error) {
			return this._handleError(error, "Không thể tải danh mục sản phẩm");
		}
	},

	/**
	 * Lấy danh sách sản phẩm (có hỗ trợ filter, search, phân trang)
	 * @param params - { productType, setName, search, page, limit }
	 */
	async getProducts(params: any = {}): Promise<ProductListResponse> {
		try {
			const response = await getApi().get<ProductListResponse>("/products", { params });
			return response.data;
		} catch (error) {
			return this._handleError(error, "Không thể tải danh sách sản phẩm");
		}
	},

	/**
	 * Lấy chi tiết một sản phẩm theo slug + sản phẩm liên quan
	 */
	async getProductBySlug(slug: string): Promise<ProductDetailResponse> {
		try {
			const response = await getApi().get<ProductDetailResponse>(`/products/${slug}`);
			return response.data;
		} catch (error) {
			return this._handleError(error, "Không thể tải thông tin sản phẩm");
		}
	},

	/**
	 * Hàm xử lý lỗi tập trung
	 */
	_handleError(error: any, defaultMessage: string): never {
		const message =
			error.response?.data?.error ||
			error.response?.data?.message ||
			error.message ||
			defaultMessage;
		console.error(`ProductService Error: ${message}`, error);
		throw new Error(message);
	},
};
