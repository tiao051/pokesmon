import {computed} from "vue";
import type {Product} from "../types/product";
import {usePersistedState} from "./usePersistedState";

export interface CartItem {
	product: Product;
	quantity: number;
}

const STORAGE_KEY = "pokegogh-cart";

export const useCart = () => {
	const items = usePersistedState<CartItem[]>(STORAGE_KEY, [], "cart");

	const add = (product: Product, qty = 1) => {
		if (product.stock <= 0) {
			return {added: 0, reason: "sold-out" as const};
		}
		const existing = items.value.find((i) => i.product.id === product.id);
		const currentQty = existing?.quantity ?? 0;
		const targetQty = Math.min(currentQty + qty, product.stock);
		const added = targetQty - currentQty;
		if (added <= 0) {
			return {added: 0, reason: "cap-reached" as const};
		}
		if (existing) existing.quantity = targetQty;
		else items.value = [...items.value, {product, quantity: targetQty}];
		return {added, reason: null};
	};

	const remove = (productId: number) => {
		items.value = items.value.filter((i) => i.product.id !== productId);
	};

	const updateQty = (productId: number, qty: number) => {
		const item = items.value.find((i) => i.product.id === productId);
		if (!item) return;
		if (qty <= 0) {
			remove(productId);
			return;
		}
		item.quantity = Math.min(qty, item.product.stock);
	};

	const clear = () => {
		items.value = [];
	};

	const count = computed(() => items.value.reduce((sum, i) => sum + i.quantity, 0));
	const subtotal = computed(() =>
		items.value.reduce((sum, i) => sum + i.product.price * i.quantity, 0),
	);

	return {items, count, subtotal, add, remove, updateQty, clear};
};
