import {useApi, setAuthToken} from "../composables/shared/useApi";

export interface User {
	id: string;
	email: string;
	isVerified: boolean;
}

export interface AuthResponse {
	user: User;
	token: string;
}

export interface LoginRequest {
	email: string;
	password: string;
}

export interface RegisterRequest {
	email: string;
	password: string;
}

export interface VerifyEmailRequest {
	email: string;
	pin: string;
}

export interface RequestPasswordResetRequest {
	email: string;
}

export interface ResetPasswordRequest {
	email: string;
	pin: string;
	newPassword: string;
}

export interface ChangePasswordRequest {
	currentPassword: string;
	newPassword: string;
}

export const authService = {
	async register(payload: RegisterRequest): Promise<{message: string}> {
		const {data} = await useApi().post("/auth/register", payload);
		return data;
	},

	async verifyEmail(payload: VerifyEmailRequest): Promise<AuthResponse> {
		const {data} = await useApi().post<AuthResponse>("/auth/verify-email", payload);
		setAuthToken(data.token);
		return data;
	},

	async login(payload: LoginRequest): Promise<AuthResponse> {
		const {data} = await useApi().post<AuthResponse>("/auth/login", payload);
		setAuthToken(data.token);
		return data;
	},

	async requestPasswordReset(payload: RequestPasswordResetRequest): Promise<{message: string}> {
		const {data} = await useApi().post("/auth/request-password-reset", payload);
		return data;
	},

	async resetPassword(payload: ResetPasswordRequest): Promise<{message: string}> {
		const {data} = await useApi().post("/auth/reset-password", payload);
		return data;
	},

	async changePassword(payload: ChangePasswordRequest): Promise<{message: string}> {
		const {data} = await useApi().post("/auth/change-password", payload);
		return data;
	},

	logout(): void {
		setAuthToken(null);
	},
};
