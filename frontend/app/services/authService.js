// frontend/app/services/authService.js
// Uses Nuxt's built-in $fetch or native fetch

const API_BASE_URL = 'http://localhost:5246/api'; // Change to match your .NET backend port

export const authService = {
  /**
   * Register a new user
   * @param {Object} userData - { email, username, password }
   * @returns {Promise<Object>} - { user, token }
   */
  async register(userData) {
    try {
      const response = await fetch(`${API_BASE_URL}/auth/register`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(userData),
      });

      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(errorText || 'Registration failed');
      }

      return await response.json();
    } catch (error) {
      console.error('AuthService register error:', error);
      throw error;
    }
  },

  /**
   * Login a user
   * @param {Object} credentials - { email, password }
   * @returns {Promise<Object>} - { user, token }
   */
  async login(credentials) {
    try {
      const response = await fetch(`${API_BASE_URL}/auth/login`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(credentials),
      });

      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(errorText || 'Login failed');
      }

      const data = await response.json();
      
      // Store token in localStorage (or preferably an HttpOnly cookie via a server route)
      if (typeof window !== 'undefined' && data.token) {
        localStorage.setItem('auth_token', data.token);
      }
      
      return data;
    } catch (error) {
      console.error('AuthService login error:', error);
      throw error;
    }
  },

  /**
   * Verify email with PIN
   * @param {Object} data - { email, pin }
   * @returns {Promise<Object>} - { user, token }
   */
  async verifyEmail(data) {
    try {
      const response = await fetch(`${API_BASE_URL}/auth/verify-email`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data),
      });

      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(errorText || 'Verification failed');
      }

      const responseData = await response.json();
      if (typeof window !== 'undefined' && responseData.token) {
        localStorage.setItem('auth_token', responseData.token);
      }
      return responseData;
    } catch (error) {
      console.error('AuthService verifyEmail error:', error);
      throw error;
    }
  },

  /**
   * Request password reset PIN
   * @param {Object} data - { email }
   */
  async requestPasswordReset(data) {
    try {
      const response = await fetch(`${API_BASE_URL}/auth/request-password-reset`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data),
      });

      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(errorText || 'Request failed');
      }

      return await response.json();
    } catch (error) {
      console.error('AuthService requestPasswordReset error:', error);
      throw error;
    }
  },

  /**
   * Reset password with PIN
   * @param {Object} data - { email, pin, newPassword }
   */
  async resetPassword(data) {
    try {
      const response = await fetch(`${API_BASE_URL}/auth/reset-password`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data),
      });

      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(errorText || 'Password reset failed');
      }

      return await response.json();
    } catch (error) {
      console.error('AuthService resetPassword error:', error);
      throw error;
    }
  },

  /**
   * Logout user
   */
  logout() {
    if (typeof window !== 'undefined') {
      localStorage.removeItem('auth_token');
    }
  },

  /**
   * Get auth token
   */
  getToken() {
    if (typeof window !== 'undefined') {
      return localStorage.getItem('auth_token');
    }
    return null;
  }
};
