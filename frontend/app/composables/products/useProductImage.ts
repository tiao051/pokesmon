import {computed, type Ref} from "vue";
import type {Product} from "../../types/product";

export function useProductImage(product: Ref<Product | null | undefined> | Product) {
	const get = () => (typeof product === "object" && "value" in product ? product.value : product);

	const src = computed(() => {
		const p = get();
		if (!p) return "";
		if (p.imageBase64) {
			return p.imageBase64.startsWith("data:")
				? p.imageBase64
				: `data:image/png;base64,${p.imageBase64}`;
		}
		return p.image || "";
	});

	const isDataUrl = computed(() => src.value.startsWith("data:"));

	return {src, isDataUrl};
}
