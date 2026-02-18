import apiClient from './apiClient';
import API_BASE_URL from '../config/api';

const notificationService = {
    getNotifications: async () => {
        return await apiClient.get(`${API_BASE_URL}/Notifications`);
    },

    markAsRead: async (id) => {
        return await apiClient.post(`${API_BASE_URL}/Notifications/${id}/read`);
    },

    deleteNotification: async (id) => {
        return await apiClient.delete(`${API_BASE_URL}/Notifications/${id}`);
    },

    sendNotification: async (notificationData) => {
        return await apiClient.post(`${API_BASE_URL}/Notifications`, notificationData);
    }
};

export default notificationService;
