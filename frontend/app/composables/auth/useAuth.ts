import {computed} from "vue";
import {authService} from "../../services/auth.service";
import {usePersistedState} from "../shared/usePersistedState";

export interface Location {
	id: string;
	label: string;
	address: string;
	city: string;
	country: string;
	isDefault: boolean;
}

export type LocationInput = Omit<Location, "id" | "isDefault">;
export type LocationPatch = Partial<LocationInput>;

export interface AuthState {
	isLoggedIn: boolean;
	email?: string;
	avatar?: string;
	displayName?: string;
	phone?: string;
	locations?: Location[];
}

export type ProfilePatch = Partial<Pick<AuthState, "displayName" | "phone">>;

const STORAGE_KEY = "pokegogh-auth";

const generateId = (): string => {
	if (typeof crypto !== "undefined" && typeof crypto.randomUUID === "function") {
		return crypto.randomUUID();
	}
	return `loc-${Date.now()}-${Math.random().toString(36).slice(2, 8)}`;
};

const validateLocationInput = (input: LocationInput | LocationPatch) => {
	for (const key of ["label", "address", "city", "country"] as const) {
		const v = input[key];
		if (v !== undefined && !v.trim()) {
			throw new Error(`${key} cannot be empty`);
		}
	}
};

export const useAuth = () => {
	const state = usePersistedState<AuthState>(STORAGE_KEY, {isLoggedIn: false}, "auth");

	const signIn = async (email: string, password: string) => {
		const data = await authService.login({email, password});
		state.value = {
			...state.value,
			isLoggedIn: true,
			email: data.user.email,
		};
		return data;
	};

	const signUp = async (email: string, password: string) =>
		authService.register({email, password});

	const verifyEmail = async (email: string, pin: string) => {
		const data = await authService.verifyEmail({email, pin});
		state.value = {
			...state.value,
			isLoggedIn: true,
			email: data.user.email,
		};
		return data;
	};

	const signOut = () => {
		authService.logout();
		state.value = {isLoggedIn: false};
	};

	const setAvatar = (avatar: string) => {
		state.value = {...state.value, avatar};
	};

	const updateProfile = (patch: ProfilePatch) => {
		const next: AuthState = {...state.value};
		for (const key of ["displayName", "phone"] as const) {
			if (key in patch) {
				const value = patch[key]?.trim();
				if (value) next[key] = value;
				else delete next[key];
			}
		}
		state.value = next;
	};

	const changePassword = async (currentPassword: string, newPassword: string) => {
		if (!currentPassword) throw new Error("Current password is required");
		if (!newPassword || newPassword.length < 8)
			throw new Error("New password must be at least 8 characters");
		if (currentPassword === newPassword)
			throw new Error("New password must differ from current password");
		await authService.changePassword({currentPassword, newPassword});
		return true;
	};

	const requestPasswordReset = async (email: string) => {
		const trimmed = email.trim();
		if (!trimmed) throw new Error("Email is required");
		if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(trimmed)) {
			throw new Error("Please enter a valid email address");
		}
		return authService.requestPasswordReset({email: trimmed});
	};

	// Locations API.
	// TODO: backend endpoints not yet wired. These mutate the persisted client-side
	// state only — replace each body with an API call when the locations module ships.
	const addLocation = async (input: LocationInput): Promise<Location> => {
		validateLocationInput(input);
		const existing = state.value.locations ?? [];
		const created: Location = {
			id: generateId(),
			label: input.label.trim(),
			address: input.address.trim(),
			city: input.city.trim(),
			country: input.country.trim(),
			isDefault: existing.length === 0,
		};
		state.value = {...state.value, locations: [...existing, created]};
		return created;
	};

	const updateLocation = async (id: string, patch: LocationPatch): Promise<Location> => {
		validateLocationInput(patch);
		const existing = state.value.locations ?? [];
		const idx = existing.findIndex((l) => l.id === id);
		if (idx === -1) throw new Error("Location not found");
		const current = existing[idx]!;
		const updated: Location = {
			...current,
			...(patch.label !== undefined && {label: patch.label.trim()}),
			...(patch.address !== undefined && {address: patch.address.trim()}),
			...(patch.city !== undefined && {city: patch.city.trim()}),
			...(patch.country !== undefined && {country: patch.country.trim()}),
		};
		const nextList = [...existing];
		nextList[idx] = updated;
		state.value = {...state.value, locations: nextList};
		return updated;
	};

	const removeLocation = async (id: string): Promise<void> => {
		const existing = state.value.locations ?? [];
		const target = existing.find((l) => l.id === id);
		if (!target) return;
		let nextList = existing.filter((l) => l.id !== id);
		if (target.isDefault && nextList.length > 0) {
			nextList = nextList.map((l, i) => ({...l, isDefault: i === 0}));
		}
		state.value = {...state.value, locations: nextList};
	};

	const setDefaultLocation = async (id: string): Promise<void> => {
		const existing = state.value.locations ?? [];
		if (!existing.some((l) => l.id === id)) throw new Error("Location not found");
		state.value = {
			...state.value,
			locations: existing.map((l) => ({...l, isDefault: l.id === id})),
		};
	};

	return {
		isLoggedIn: computed(() => state.value.isLoggedIn),
		email: computed(() => state.value.email),
		avatar: computed(() => state.value.avatar),
		storedDisplayName: computed(() => state.value.displayName),
		phone: computed(() => state.value.phone),
		locations: computed<Location[]>(() => state.value.locations ?? []),
		defaultLocation: computed<Location | undefined>(() =>
			(state.value.locations ?? []).find((l) => l.isDefault),
		),
		signIn,
		signUp,
		verifyEmail,
		signOut,
		setAvatar,
		updateProfile,
		changePassword,
		requestPasswordReset,
		addLocation,
		updateLocation,
		removeLocation,
		setDefaultLocation,
	};
};
