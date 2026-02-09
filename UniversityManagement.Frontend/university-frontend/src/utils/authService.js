import authService from './authService';

class ApiClient {
    async request(url, options = {}) {
        const token = authService.getAccessToken();

        const headers = {
            'Content-Type': 'application/json',
            ...options.headers,
        };

        if (token) {
            headers['Authorization'] = `Bearer ${token}`;
        }

        let response = await fetch(url, {
            ...options,
            headers,
        });

        // Handle 401 Unauthorized - try to refresh token
        if (response.status === 401 && token) {
            try {
                await authService.refreshToken();
                const newToken = authService.getAccessToken();
                headers['Authorization'] = `Bearer ${newToken}`;

                // Retry the original request with new token
                response = await fetch(url, {
                    ...options,
                    headers,
                });
            } catch (error) {
                // Refresh failed, redirect to login
                authService.clearAuthData();
                window.location.href = '/login';
                throw new Error('Session expired. Please login again.');
            }
        }

        if (!response.ok) {
            const errorText = await response.text();
            throw new Error(errorText || `Request failed with status ${response.status}`);
        }

        // Handle 204 No Content
        if (response.status === 204) {
            return null;
        }

        return await response.json();
    }

    async get(url) {
        return this.request(url, { method: 'GET' });
    }

    async post(url, data) {
        return this.request(url, {
            method: 'POST',
            body: JSON.stringify(data),
        });
    }

    async put(url, data) {
        return this.request(url, {
            method: 'PUT',
            body: JSON.stringify(data),
        });
    }

    async delete(url) {
        return this.request(url, { method: 'DELETE' });
    }
}

export default new ApiClient();
