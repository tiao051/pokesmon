import axios, {type AxiosInstance, AxiosError} from "axios";

export class ApiError extends Error {
	status: number;
	body: unknown;
	constructor(message: string, status: number, body: unknown) {
		super(message);
		this.status = status;
		this.body = body;
	}
}

const TOKEN_STORAGE_KEY = "auth_token";

let _client: AxiosInstance | null = null;

export function useApi(): AxiosInstance {
	if (_client) return _client;

	const config = useRuntimeConfig();
	const baseURL = config.public.apiBaseUrl as string;

	const client = axios.create({
		baseURL,
		headers: {"Content-Type": "application/json"},
	});

	client.interceptors.request.use((cfg) => {
		if (import.meta.client) {
			const token = localStorage.getItem(TOKEN_STORAGE_KEY);
			if (token) cfg.headers.Authorization = `Bearer ${token}`;
		}
		return cfg;
	});

	client.interceptors.response.use(
		(res) => res,
		(error: AxiosError) => {
			const status = error.response?.status ?? 0;
			const body = error.response?.data;
			const message =
				(body as {error?: string})?.error ||
				(body as {message?: string})?.message ||
				error.message ||
				"Request failed";
			return Promise.reject(new ApiError(message, status, body));
		},
	);

	_client = client;
	return client;
}

export function setAuthToken(token: string | null): void {
	if (!import.meta.client) return;
	if (token) localStorage.setItem(TOKEN_STORAGE_KEY, token);
	else localStorage.removeItem(TOKEN_STORAGE_KEY);
}

export function getAuthToken(): string | null {
	if (!import.meta.client) return null;
	return localStorage.getItem(TOKEN_STORAGE_KEY);
}
