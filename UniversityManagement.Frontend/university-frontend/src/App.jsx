import { useState, useEffect } from 'react';
import Login from './components/auth/Login';
import DashboardLayout from './components/layout/DashboardLayout';
import DashboardHome from './components/pages/DashboardHome';
import StudentsPage from './components/pages/StudentsPage';
import TeachersPage from './components/pages/TeachersPage';
import ProgramsPage from './components/pages/ProgramsPage';
import SubjectsPage from './components/pages/SubjectsPage';
import ExamsPage from './components/pages/ExamsPage';
import AttendancePage from './components/pages/AttendancePage';
import TimetablePage from './components/pages/TimetablePage';
import AnnouncementsPage from './components/pages/AnnouncementsPage';
import authService from './utils/authService';
import './index.css';

function App() {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    // Check if user is already authenticated
    const checkAuth = () => {
      const authenticated = authService.isAuthenticated();
      setIsAuthenticated(authenticated);
      setIsLoading(false);
    };

    checkAuth();
  }, []);

  const handleLoginSuccess = () => {
    setIsAuthenticated(true);
  };

  const handleLogout = async () => {
    await authService.logout();
    setIsAuthenticated(false);
  };

  if (isLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-slate-950">
        <div className="w-12 h-12 border-4 border-primary-600 border-t-transparent rounded-full animate-spin" />
      </div>
    );
  }

  if (!isAuthenticated) {
    return <Login onLoginSuccess={handleLoginSuccess} />;
  }

  return (
    <DashboardLayout
      title="University Management System"
      onLogout={handleLogout}
    >
      {({ activeTab, userRole, setActiveTab }) => {
        switch (activeTab) {
          case 'dashboard':
            return <DashboardHome setActiveTab={setActiveTab} />;
          case 'students':
            return <StudentsPage />;
          case 'teachers':
            return <TeachersPage />;
          case 'programs':
            return <ProgramsPage />;
          case 'subjects':
            return <SubjectsPage />;
          case 'exams':
            return <ExamsPage />;
          case 'attendance':
            return <AttendancePage />;
          case 'timetable':
            return <TimetablePage />;
          case 'announcements':
            return <AnnouncementsPage />;
          default:
            return <DashboardHome setActiveTab={setActiveTab} />;
        }
      }}
    </DashboardLayout>
  );
}

export default App;
