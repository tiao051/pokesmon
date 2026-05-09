import {usePersistedState} from "../shared/usePersistedState";

const STORAGE_KEY = "pokegogh-search-history";
const MAX_TERMS = 5;

export function useSearchHistory() {
	const history = usePersistedState<string[]>(STORAGE_KEY, [], "search-history");

	const addSearchTerm = (term: string) => {
		const trimmed = term.trim();
		if (!trimmed) return;
		const next = history.value.filter((t) => t !== trimmed);
		next.unshift(trimmed);
		history.value = next.slice(0, MAX_TERMS);
	};

	const clearHistory = () => {
		history.value = [];
	};

	return {history, addSearchTerm, clearHistory};
}
