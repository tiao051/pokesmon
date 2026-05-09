import {computed} from "vue";
import type {Product} from "../../types/product";
import {depositRateFor} from "../../utils/depositRates";
import {usePersistedState} from "../shared/usePersistedState";

export interface CartItem {
	product: Product;
	quantity: number;
}

const STORAGE_KEY = "pokegogh-cart";

const sumQty = (list: CartItem[]) => list.reduce((s, i) => s + i.quantity, 0);
const sumPrice = (list: CartItem[]) =>
	list.reduce((s, i) => s + i.product.price * i.quantity, 0);
const sumDeposit = (list: CartItem[]) =>
	list.reduce(
		(s, i) =>
			s + i.product.price * i.quantity * depositRateFor(i.product.rarityName),
		0,
	);

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

	const standardItems = computed(() =>
		items.value.filter((i) => !i.product.isPreorder),
	);
	const preorderItems = computed(() =>
		items.value.filter((i) => i.product.isPreorder),
	);

	const count = computed(() => sumQty(items.value));
	const subtotal = computed(() => sumPrice(items.value));

	const standardCount = computed(() => sumQty(standardItems.value));
	const standardSubtotal = computed(() => sumPrice(standardItems.value));

	const preorderCount = computed(() => sumQty(preorderItems.value));
	const preorderSubtotal = computed(() => sumPrice(preorderItems.value));
	const preorderDeposit = computed(() => sumDeposit(preorderItems.value));
	const preorderRemaining = computed(
		() => preorderSubtotal.value - preorderDeposit.value,
	);

	return {
		items,
		count,
		subtotal,
		add,
		remove,
		updateQty,
		clear,
		standardItems,
		standardCount,
		standardSubtotal,
		preorderItems,
		preorderCount,
		preorderSubtotal,
		preorderDeposit,
		preorderRemaining,
	};
};
