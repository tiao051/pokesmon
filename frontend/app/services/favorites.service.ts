import {useApi} from "../composables/shared/useApi";
import type {Product} from "../types/product";

export interface Favorite {
	productId: number;
	createdAt: string;
	product: Product;
}

export const favoritesService = {
	async list(): Promise<Favorite[]> {
		const {data} = await useApi().get<{items: Favorite[]}>("/favorites");
		return data.items ?? [];
	},

	async listIds(): Promise<number[]> {
		const {data} = await useApi().get<{ids: number[]}>("/favorites", {
			params: {view: "ids"},
		});
		return data.ids ?? [];
	},

	async add(productId: number): Promise<void> {
		await useApi().post("/favorites", {productId});
	},

	async remove(productId: number): Promise<void> {
		await useApi().delete(`/favorites/${productId}`);
	},
};
