import { useState } from 'react';
import authService from '../../utils/authService';

const Sidebar = ({ activeTab, setActiveTab, userRole }) => {
    const [isCollapsed, setIsCollapsed] = useState(false);

    const menuItems = [
        { id: 'dashboard', label: 'Dashboard', icon: '📊', roles: ['Admin', 'Teacher', 'Student'] },
        { id: 'students', label: 'Students', icon: '👨‍🎓', roles: ['Admin', 'Teacher'] },
        { id: 'teachers', label: 'Teachers', icon: '👨‍🏫', roles: ['Admin'] },
        { id: 'programs', label: 'Programs', icon: '📚', roles: ['Admin', 'Teacher', 'Student'] },
        { id: 'subjects', label: 'Subjects', icon: '📖', roles: ['Admin', 'Teacher', 'Student'] },
        { id: 'exams', label: 'Exams', icon: '📝', roles: ['Admin', 'Teacher', 'Student'] },
        { id: 'attendance', label: 'Attendance', icon: '✓', roles: ['Admin', 'Teacher', 'Student'] },
        { id: 'timetable', label: 'Timetable', icon: '📅', roles: ['Admin', 'Teacher', 'Student'] },
        { id: 'announcements', label: 'Announcements', icon: '📢', roles: ['Admin', 'Teacher', 'Student'] },
    ];

    const filteredMenuItems = menuItems.filter(item => item.roles.includes(userRole));

    return (
        <div className={`${isCollapsed ? 'w-20' : 'w-64'} bg-slate-900/50 border-r border-slate-800 h-screen transition-all duration-300 flex flex-col`}>
            {/* Logo */}
            <div className="p-6 border-b border-slate-800">
                <div className="flex items-center justify-between">
                    {!isCollapsed && (
                        <div className="flex items-center gap-3">
                            <div className="flex h-10 w-10 items-center justify-center rounded-xl bg-primary-600/20 text-primary-400">
                                <span className="text-xl font-semibold">UM</span>
                            </div>
                            <div>
                                <h2 className="text-sm font-bold text-white">University</h2>
                                <p className="text-xs text-slate-400">Management</p>
                            </div>
                        </div>
                    )}
                    <button
                        onClick={() => setIsCollapsed(!isCollapsed)}
                        className="text-slate-400 hover:text-white transition-colors"
                    >
                        <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d={isCollapsed ? "M9 5l7 7-7 7" : "M15 19l-7-7 7-7"} />
                        </svg>
                    </button>
                </div>
            </div>

            {/* Navigation */}
            <nav className="flex-1 p-4 space-y-2 overflow-y-auto">
                {filteredMenuItems.map((item) => (
                    <button
                        key={item.id}
                        onClick={() => setActiveTab(item.id)}
                        className={`w-full flex items-center gap-3 px-4 py-3 rounded-xl transition-all ${activeTab === item.id
                            ? 'bg-primary-600 text-white shadow-lg shadow-primary-600/30'
                            : 'text-slate-300 hover:bg-slate-800 hover:text-white'
                            }`}
                    >
                        <span className="text-xl">{item.icon}</span>
                        {!isCollapsed && <span className="font-medium">{item.label}</span>}
                    </button>
                ))}
            </nav>

            {/* User info */}
            <div className="p-4 border-t border-slate-800">
                <div className={`flex items-center gap-3 ${isCollapsed ? 'justify-center' : ''}`}>
                    <div className="w-10 h-10 rounded-full bg-gradient-to-r from-primary-500 to-accent-500 flex items-center justify-center text-white font-bold">
                        {authService.getUser()?.email?.[0]?.toUpperCase() || 'U'}
                    </div>
                    {!isCollapsed && (
                        <div className="flex-1 min-w-0">
                            <p className="text-sm font-semibold text-white truncate">
                                {authService.getUser()?.email}
                            </p>
                            <p className="text-xs text-slate-400">{userRole}</p>
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
};

const Header = ({ title, onLogout }) => {
    return (
        <header className="bg-slate-900/50 border-b border-slate-800 px-8 py-4">
            <div className="flex items-center justify-between">
                <div>
                    <h1 className="text-2xl font-bold text-white">{title}</h1>
                    <p className="text-sm text-slate-400 mt-1">
                        {new Date().toLocaleDateString('en-US', {
                            weekday: 'long',
                            year: 'numeric',
                            month: 'long',
                            day: 'numeric'
                        })}
                    </p>
                </div>
                <div className="flex items-center gap-4">
                    <button className="relative p-2 text-slate-400 hover:text-white transition-colors">
                        <svg className="w-6 h-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9" />
                        </svg>
                        <span className="absolute top-1 right-1 w-2 h-2 bg-danger-500 rounded-full"></span>
                    </button>
                    <button
                        onClick={onLogout}
                        className="px-4 py-2 bg-slate-800 hover:bg-slate-700 text-white rounded-xl transition-colors font-medium"
                    >
                        Logout
                    </button>
                </div>
            </div>
        </header>
    );
};

const DashboardLayout = ({ children, title, onLogout }) => {
    const [activeTab, setActiveTab] = useState('dashboard');
    const userRole = authService.getUserRole();

    return (
        <div className="flex h-screen bg-slate-950">
            <Sidebar activeTab={activeTab} setActiveTab={setActiveTab} userRole={userRole} />
            <div className="flex-1 flex flex-col overflow-hidden">
                <Header title={title} onLogout={onLogout} />
                <main className="flex-1 overflow-y-auto p-8">
                    {children({ activeTab, userRole, setActiveTab })}
                </main>
            </div>
        </div>
    );
};

export default DashboardLayout;
