import { useState, useEffect } from 'react';
import Login from './components/auth/Login';
import DashboardLayout from './components/layout/DashboardLayout';
import DashboardHome from './components/pages/DashboardHome';
import StudentsPage from './components/pages/StudentsPage';
import TeachersPage from './components/pages/TeachersPage';
import authService from './utils/authService';
import './index.css';

// Placeholder components for other pages
const ProgramsPage = () => <div className="text-white">Programs Management - Coming Soon</div>;
const SubjectsPage = () => <div className="text-white">Subjects Management - Coming Soon</div>;
const ExamsPage = () => <div className="text-white">Exams Management - Coming Soon</div>;
const AttendancePage = () => <div className="text-white">Attendance Management - Coming Soon</div>;
const TimetablePage = () => <div className="text-white">Timetable Management - Coming Soon</div>;
const AnnouncementsPage = () => <div className="text-white">Announcements - Coming Soon</div>;

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
      {({ activeTab, userRole }) => {
        switch (activeTab) {
          case 'dashboard':
            return <DashboardHome />;
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
            return <DashboardHome />;
        }
      }}
    </DashboardLayout>
  );
}

export default App;
