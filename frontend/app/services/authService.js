import axios from "axios";

const getApiBaseUrl = () => {
	try {
		const config = useRuntimeConfig();
		return config.public.apiBaseUrl || "http://localhost:5246/api";
	} catch {
		return "http://localhost:5246/api";
	}
};

const api = axios.create({
	baseURL: getApiBaseUrl(),
	headers: {
		"Content-Type": "application/json",
	},
});

api.interceptors.request.use(
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

export const authService = {
	/**
	 * Đăng ký tài khoản mới
	 * @param {Object} userData - { email, username, password }
	 */
	async register(userData) {
		try {
			const response = await api.post("/auth/register", userData);
			return response.data;
		} catch (error) {
			this._handleError(error, "Đăng ký thất bại");
		}
	},

	/**
	 * Xác thực Email bằng mã PIN
	 * @param {Object} data - { email, pin }
	 */
	async verifyEmail(data) {
		try {
			const response = await api.post("/auth/verify-email", data);
			const responseData = response.data;

			if (typeof window !== "undefined" && responseData.token) {
				localStorage.setItem("auth_token", responseData.token);
			}

			return responseData;
		} catch (error) {
			this._handleError(error, "Xác thực mã PIN thất bại");
		}
	},

	/**
	 * Đăng nhập
	 * @param {Object} credentials - { email, password }
	 */
	async login(credentials) {
		try {
			const response = await api.post("/auth/login", credentials);
			const data = response.data;

			if (typeof window !== "undefined" && data.token) {
				localStorage.setItem("auth_token", data.token);
			}

			return data;
		} catch (error) {
			this._handleError(error, "Đăng nhập thất bại");
		}
	},

	/**
	 * Yêu cầu mã PIN quên mật khẩu
	 * @param {Object} data - { email }
	 */
	async requestPasswordReset(data) {
		try {
			const response = await api.post(
				"/auth/request-password-reset",
				data,
			);
			return response.data;
		} catch (error) {
			this._handleError(error, "Yêu cầu đặt lại mật khẩu thất bại");
		}
	},

	/**
	 * Đặt lại mật khẩu bằng mã PIN
	 * @param {Object} data - { email, pin, newPassword }
	 */
	async resetPassword(data) {
		try {
			const response = await api.post("/auth/reset-password", data);
			return response.data;
		} catch (error) {
			this._handleError(error, "Đặt lại mật khẩu thất bại");
		}
	},

	/**
	 * Đăng xuất
	 */
	logout() {
		if (typeof window !== "undefined") {
			localStorage.removeItem("auth_token");
		}
	},

	/**
	 * Lấy Token hiện tại
	 */
	getToken() {
		if (typeof window !== "undefined") {
			return localStorage.getItem("auth_token");
		}
		return null;
	},

	/**
	 * Hàm xử lý lỗi tập trung
	 */
	_handleError(error, defaultMessage) {
		const message = error.response?.data || error.message || defaultMessage;
		console.error(`AuthService Error: ${message}`, error);
		throw new Error(message);
	},
};
