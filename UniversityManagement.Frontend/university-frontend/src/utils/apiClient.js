import authService from './authService';

class ApiClient {
    async request(url, options = {}) {
        console.log('Request method:', options.method, 'Full URL:', url);
        // If url is relative, concatenate but here it should be absolute from services
        const token = authService.getAccessToken();
        console.log('Access token present:', !!token);

        const headers = {
            'Content-Type': 'application/json',
            ...options.headers,
        };

        if (token) {
            headers['Authorization'] = `Bearer ${token}`;
        }

        let response;
        try {
            response = await fetch(url, {
                ...options,
                headers,
            });
        } catch (err) {
            console.error('Network error for URL:', url, err);
            throw new Error(`Network error (is the backend running on ${url.split('/api')[0]}?): ${err.message}`);
        }

        console.log('Fetch response status:', response.status);

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
            console.error('Response error:', response.status, errorText, 'for URL:', url);
            throw new Error(errorText || `Request failed with status ${response.status} for ${url}`);
        }

        // Handle 204 No Content
        if (response.status === 204) {
            return null;
        }

        return await response.json();
    }

    async get(url) {
        console.log('API GET request to:', url);
        try {
            const result = await this.request(url, { method: 'GET' });
            console.log('API GET response:', result);
            return result;
        } catch (err) {
            console.error('API GET error:', err);
            throw err;
        }
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

const apiClient = new ApiClient();
export default apiClient;
