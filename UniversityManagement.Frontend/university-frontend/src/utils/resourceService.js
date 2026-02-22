import apiClient from './apiClient';
import API_BASE_URL from '../config/api';

const resourceService = {
    getSubjectResources: async (subjectId) => {
        return await apiClient.get(`${API_BASE_URL}/CourseResources/subject/${subjectId}`);
    },

    addResource: async (resourceData) => {
        return await apiClient.post(`${API_BASE_URL}/CourseResources`, resourceData);
    },

    deleteResource: async (id) => {
        return await apiClient.delete(`${API_BASE_URL}/CourseResources/${id}`);
    },

    updateResource: async (id, resourceData) => {
        return await apiClient.put(`${API_BASE_URL}/CourseResources/${id}`, resourceData);
    }
};

export default resourceService;
