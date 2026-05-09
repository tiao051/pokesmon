import {watch} from "vue";

export function usePersistedState<T>(
	key: string,
	defaultValue: T,
	stateKey?: string,
) {
	const state = useState<T>(stateKey ?? `persisted-${key}`, () => defaultValue);
	const hydratedKey = `__persisted_hydrated_${key}`;

	if (import.meta.client) {
		const flag = (globalThis as Record<string, unknown>)[hydratedKey];
		if (!flag) {
			(globalThis as Record<string, unknown>)[hydratedKey] = true;
			try {
				const raw = localStorage.getItem(key);
				if (raw !== null) state.value = JSON.parse(raw) as T;
			} catch {
				// ignore corrupt data
			}
			watch(
				state,
				(v) => {
					try {
						localStorage.setItem(key, JSON.stringify(v));
					} catch {
						// ignore quota errors
					}
				},
				{deep: true},
			);
		}
	}

	return state;
}
