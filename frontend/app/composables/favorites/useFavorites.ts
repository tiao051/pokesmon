import {computed} from "vue";
import {favoritesService} from "../../services/favorites.service";

interface FavoritesState {
	ids: number[];
	loaded: boolean;
	loading: boolean;
}

export const useFavorites = () => {
	const state = useState<FavoritesState>("favorites-state", () => ({
		ids: [],
		loaded: false,
		loading: false,
	}));

	const idsSet = computed(() => new Set(state.value.ids));
	const count = computed(() => state.value.ids.length);

	const has = (productId: number) => idsSet.value.has(productId);

	const refresh = async () => {
		if (state.value.loading) return;
		state.value.loading = true;
		try {
			const ids = await favoritesService.listIds();
			state.value.ids = ids;
			state.value.loaded = true;
		} finally {
			state.value.loading = false;
		}
	};

	const ensureLoaded = async () => {
		if (state.value.loaded || state.value.loading) return;
		await refresh();
	};

	const add = async (productId: number) => {
		if (has(productId)) return;
		state.value.ids = [...state.value.ids, productId];
		try {
			await favoritesService.add(productId);
		} catch (err) {
			state.value.ids = state.value.ids.filter((id) => id !== productId);
			throw err;
		}
	};

	const remove = async (productId: number) => {
		if (!has(productId)) return;
		const previous = state.value.ids;
		state.value.ids = previous.filter((id) => id !== productId);
		try {
			await favoritesService.remove(productId);
		} catch (err) {
			state.value.ids = previous;
			throw err;
		}
	};

	const toggle = async (productId: number) => {
		if (has(productId)) await remove(productId);
		else await add(productId);
	};

	const reset = () => {
		state.value = {ids: [], loaded: false, loading: false};
	};

	return {
		ids: computed(() => state.value.ids),
		count,
		loading: computed(() => state.value.loading),
		loaded: computed(() => state.value.loaded),
		has,
		refresh,
		ensureLoaded,
		add,
		remove,
		toggle,
		reset,
	};
};
