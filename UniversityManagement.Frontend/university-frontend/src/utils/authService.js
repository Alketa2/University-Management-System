import { API_ENDPOINTS } from '../config/api';

class AuthService {
    constructor() {
        this.TOKEN_KEY = 'access_token';
        this.REFRESH_TOKEN_KEY = 'refresh_token';
        this.USER_KEY = 'user_data';
    }

    // Get access token
    getAccessToken() {
        return localStorage.getItem(this.TOKEN_KEY);
    }

    // Get refresh token
    getRefreshToken() {
        return localStorage.getItem(this.REFRESH_TOKEN_KEY);
    }

    // Get user data
    getUser() {
        const user = localStorage.getItem(this.USER_KEY);
        return user ? JSON.parse(user) : null;
    }

    // Save authentication data
    saveAuthData(authResponse) {
        localStorage.setItem(this.TOKEN_KEY, authResponse.accessToken);
        localStorage.setItem(this.REFRESH_TOKEN_KEY, authResponse.refreshToken);
        localStorage.setItem(this.USER_KEY, JSON.stringify({
            email: authResponse.email,
            role: authResponse.role,
        }));
    }

    // Clear authentication data
    clearAuthData() {
        localStorage.removeItem(this.TOKEN_KEY);
        localStorage.removeItem(this.REFRESH_TOKEN_KEY);
        localStorage.removeItem(this.USER_KEY);
    }

    // Check if user is authenticated
    isAuthenticated() {
        return !!this.getAccessToken();
    }

    // Get user role
    getUserRole() {
        const user = this.getUser();
        return user?.role || null;
    }

    // Login
    async login(email, password) {
        const response = await fetch(API_ENDPOINTS.AUTH.LOGIN, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ email, password }),
        });

        if (!response.ok) {
            const error = await response.text();
            throw new Error(error || 'Login failed');
        }

        const data = await response.json();
        this.saveAuthData(data);
        return data;
    }

    // Register
    async register(firstName, lastName, email, password, role = 'Student') {
        const response = await fetch(API_ENDPOINTS.AUTH.REGISTER, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ firstName, lastName, email, password, role }),
        });

        if (!response.ok) {
            const error = await response.text();
            throw new Error(error || 'Registration failed');
        }

        const data = await response.json();
        this.saveAuthData(data);
        return data;
    }

    // Refresh token
    async refreshToken() {
        const refreshToken = this.getRefreshToken();
        if (!refreshToken) {
            throw new Error('No refresh token available');
        }

        const response = await fetch(API_ENDPOINTS.AUTH.REFRESH, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ refreshToken }),
        });

        if (!response.ok) {
            this.clearAuthData();
            throw new Error('Token refresh failed');
        }

        const data = await response.json();
        this.saveAuthData(data);
        return data;
    }

    // Logout
    async logout() {
        const refreshToken = this.getRefreshToken();
        if (refreshToken) {
            try {
                await fetch(API_ENDPOINTS.AUTH.LOGOUT, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': `Bearer ${this.getAccessToken()}`,
                    },
                    body: JSON.stringify({ refreshToken }),
                });
            } catch (error) {
                console.error('Logout error:', error);
            }
        }
        this.clearAuthData();
    }
}

const authService = new AuthService();
export default authService;
