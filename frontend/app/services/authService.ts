import axios, { type AxiosInstance } from "axios";

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

	_api.interceptors.request.use(
		(config) => {
			if (typeof window !== "undefined") {
				const token = localStorage.getItem("auth_token");
				if (token) {
					config.headers.Authorization = `Bearer ${token}`;
				}
			}
			return config;
		},
		(error) => {
			return Promise.reject(error);
		},
	);

	return _api;
}

export interface User {
	email: string;
}

export interface AuthResponse {
	user: User;
	token: string;
}

export const authService = {
	/**
	 * Đăng ký tài khoản mới
	 * @param userData - { email, password }
	 */
	async register(userData: any): Promise<any> {
		try {
			const response = await getApi().post("/auth/register", userData);
			return response.data;
		} catch (error) {
			return this._handleError(error, "Đăng ký thất bại");
		}
	},

	/**
	 * Xác thực Email bằng mã PIN
	 * @param data - { email, pin }
	 */
	async verifyEmail(data: any): Promise<AuthResponse> {
		try {
			const response = await getApi().post<AuthResponse>("/auth/verify-email", data);
			const responseData = response.data;

			if (typeof window !== "undefined" && responseData.token) {
				localStorage.setItem("auth_token", responseData.token);
			}

			return responseData;
		} catch (error) {
			return this._handleError(error, "Xác thực mã PIN thất bại");
		}
	},

	/**
	 * Đăng nhập
	 * @param credentials - { email, password }
	 */
	async login(credentials: any): Promise<AuthResponse> {
		try {
			const response = await getApi().post<AuthResponse>("/auth/login", credentials);
			const data = response.data;

			if (typeof window !== "undefined" && data.token) {
				localStorage.setItem("auth_token", data.token);
			}

			return data;
		} catch (error) {
			return this._handleError(error, "Đăng nhập thất bại");
		}
	},

	/**
	 * Yêu cầu mã PIN quên mật khẩu
	 * @param data - { email }
	 */
	async requestPasswordReset(data: { email: string }): Promise<any> {
		try {
			const response = await getApi().post(
				"/auth/request-password-reset",
				data,
			);
			return response.data;
		} catch (error) {
			return this._handleError(error, "Yêu cầu đặt lại mật khẩu thất bại");
		}
	},

	/**
	 * Đặt lại mật khẩu bằng mã PIN
	 * @param data - { email, pin, newPassword }
	 */
	async resetPassword(data: any): Promise<any> {
		try {
			const response = await getApi().post("/auth/reset-password", data);
			return response.data;
		} catch (error) {
			return this._handleError(error, "Đặt lại mật khẩu thất bại");
		}
	},

	/**
	 * Đăng xuất
	 */
	logout(): void {
		if (typeof window !== "undefined") {
			localStorage.removeItem("auth_token");
		}
	},

	/**
	 * Lấy Token hiện tại
	 */
	getToken(): string | null {
		if (typeof window !== "undefined") {
			return localStorage.getItem("auth_token");
		}
		return null;
	},

	/**
	 * Hàm xử lý lỗi tập trung
	 */
	_handleError(error: any, defaultMessage: string): never {
		const message = error.response?.data || error.message || defaultMessage;
		console.error(`AuthService Error: ${message}`, error);
		throw new Error(message);
	},
};
