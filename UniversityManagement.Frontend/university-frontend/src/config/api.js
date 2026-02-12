// API Configuration
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'https://localhost:7104/api';

export const API_ENDPOINTS = {
  // Auth endpoints
  AUTH: {
    LOGIN: `${API_BASE_URL}/Auth/login`,
    REGISTER: `${API_BASE_URL}/Auth/register`,
    REFRESH: `${API_BASE_URL}/Auth/refresh`,
    LOGOUT: `${API_BASE_URL}/Auth/logout`,
  },

  // Students endpoints
  STUDENTS: {
    BASE: `${API_BASE_URL}/Students`,
    BY_ID: (id) => `${API_BASE_URL}/Students/${id}`,
    PROGRAMS: (id) => `${API_BASE_URL}/Students/${id}/programs`,
    ADMIT_TO_PROGRAM: `${API_BASE_URL}/Students/admit-to-program`,
  },

  // Teachers endpoints
  TEACHERS: {
    BASE: `${API_BASE_URL}/Teachers`,
    BY_ID: (id) => `${API_BASE_URL}/Teachers/${id}`,
  },

  // Programs endpoints
  PROGRAMS: {
    BASE: `${API_BASE_URL}/Programs`,
    BY_ID: (id) => `${API_BASE_URL}/Programs/${id}`,
  },

  // Subjects endpoints
  SUBJECTS: {
    BASE: `${API_BASE_URL}/Subjects`,
    BY_ID: (id) => `${API_BASE_URL}/Subjects/${id}`,
    BY_PROGRAM: (programId) => `${API_BASE_URL}/Subjects/program/${programId}`,
  },

  // Exams endpoints
  EXAMS: {
    BASE: `${API_BASE_URL}/Exams`,
    BY_ID: (id) => `${API_BASE_URL}/Exams/${id}`,
    BY_SUBJECT: (subjectId) => `${API_BASE_URL}/Exams/subject/${subjectId}`,
  },

  // Announcements endpoints
  ANNOUNCEMENTS: {
    BASE: `${API_BASE_URL}/Announcements`,
    BY_ID: (id) => `${API_BASE_URL}/Announcements/${id}`,
    ACTIVE: `${API_BASE_URL}/Announcements/active`,
    BY_TEACHER: (teacherId) => `${API_BASE_URL}/Announcements/teacher/${teacherId}`,
  },

  // Grades endpoints
  GRADES: {
    BASE: `${API_BASE_URL}/Grades`,
    BY_ID: (id) => `${API_BASE_URL}/Grades/${id}`,
    BY_STUDENT: (studentId) => `${API_BASE_URL}/Grades/student/${studentId}`,
    BY_SUBJECT: (subjectId) => `${API_BASE_URL}/Grades/subject/${subjectId}`,
    BY_EXAM: (examId) => `${API_BASE_URL}/Grades/exam/${examId}`,
    GPA: (studentId) => `${API_BASE_URL}/Grades/gpa/student/${studentId}`,
  },

  // Timetables endpoints
  TIMETABLES: {
    BASE: `${API_BASE_URL}/Timetables`,
    BY_ID: (id) => `${API_BASE_URL}/Timetables/${id}`,
    BY_PROGRAM: (programId) => `${API_BASE_URL}/Timetables/program/${programId}`,
  },
};

export default API_BASE_URL;
