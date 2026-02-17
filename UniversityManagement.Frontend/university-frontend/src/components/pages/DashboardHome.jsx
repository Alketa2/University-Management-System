import { useState, useEffect } from 'react';
import { Card, Badge, Spinner } from '../ui/UIComponents';
import apiClient from '../../utils/apiClient';
import { API_ENDPOINTS } from '../../config/api';
import authService from '../../utils/authService';

const DashboardHome = ({ setActiveTab }) => {
    const [stats, setStats] = useState(null);
    const [announcements, setAnnouncements] = useState([]);
    const [recentActivity, setRecentActivity] = useState([]);
    const [upcomingEvents, setUpcomingEvents] = useState([]);
    const [todaySchedule, setTodaySchedule] = useState([]);
    const [atRiskStudents, setAtRiskStudents] = useState([]);
    const [loading, setLoading] = useState(true);

    const currentUser = authService.getUser();
    const userRole = authService.getUserRole();
    const userName = currentUser?.firstName || currentUser?.email?.split('@')[0] || 'User';
    const isStudent = userRole === 'Student';

    useEffect(() => {
        fetchDashboardData();
    }, []);

    const fetchDashboardData = async () => {
        setLoading(true);
        try {
            const daysOfWeek = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
            const todayName = daysOfWeek[new Date().getDay()];

            if (isStudent) {
                const [activeAnnouncements, allExams, allTimetables] = await Promise.all([
                    apiClient.get(API_ENDPOINTS.ANNOUNCEMENTS.ACTIVE).catch(() => []),
                    apiClient.get(API_ENDPOINTS.EXAMS.BASE).catch(() => []),
                    apiClient.get(API_ENDPOINTS.TIMETABLES.BASE).catch(() => []),
                ]);

                const studentProgramId = currentUser?.primaryProgramId;

                // Filter exams for student's program and future dates
                const studentExams = allExams
                    .filter(e => (!studentProgramId || e.programId === studentProgramId) && new Date(e.date) >= new Date())
                    .sort((a, b) => new Date(a.date) - new Date(b.date))
                    .slice(0, 4);

                // Filter schedule for today
                const studentSchedule = allTimetables
                    .filter(t => t.dayOfWeek === todayName && (!studentProgramId || t.programId === studentProgramId))
                    .sort((a, b) => a.startTime.localeCompare(b.startTime));

                setAnnouncements(activeAnnouncements.slice(0, 5));
                setUpcomingEvents(studentExams);
                setTodaySchedule(studentSchedule);
            } else {
                const [students, teachers, programs, subjects, exams, activeAnnouncements] = await Promise.all([
                    apiClient.get(API_ENDPOINTS.STUDENTS.BASE).catch(() => []),
                    apiClient.get(API_ENDPOINTS.TEACHERS.BASE).catch(() => []),
                    apiClient.get(API_ENDPOINTS.PROGRAMS.BASE).catch(() => []),
                    apiClient.get(API_ENDPOINTS.SUBJECTS.BASE).catch(() => []),
                    apiClient.get(API_ENDPOINTS.EXAMS.BASE).catch(() => []),
                    apiClient.get(API_ENDPOINTS.ANNOUNCEMENTS.ACTIVE).catch(() => []),
                ]);

                setStats({
                    students: students.length || 0,
                    teachers: teachers.length || 0,
                    programs: programs.length || 0,
                    subjects: subjects.length || 0,
                    exams: exams.length || 0,
                });

                // Calculate recent activity
                const activity = [
                    ...students.slice(-2).map(s => ({ type: 'student', title: 'New student enrolled', desc: `${s.firstName} ${s.lastName} joined`, time: s.createdAt, icon: '👨‍🎓' })),
                    ...exams.slice(-2).map(e => ({ type: 'exam', title: 'Exam scheduled', desc: `${e.name} for ${e.subjectName || 'Subject'}`, time: e.createdAt, icon: '📝' })),
                    ...subjects.slice(-2).map(s => ({ type: 'subject', title: 'New course added', desc: s.name, time: s.createdAt, icon: '📚' }))
                ].sort((a, b) => new Date(b.time) - new Date(a.time)).slice(0, 4);

                const futureExams = exams
                    .filter(e => new Date(e.date) >= new Date())
                    .sort((a, b) => new Date(a.date) - new Date(b.date))
                    .slice(0, 4);

                setRecentActivity(activity);
                setUpcomingEvents(futureExams);
                setAnnouncements(activeAnnouncements.slice(0, 5));

                // Identify at-risk students (GPA < 2.0)
                const atRisk = students
                    .filter(s => s.gpa > 0 && s.gpa < 2.0)
                    .sort((a, b) => a.gpa - b.gpa)
                    .slice(0, 4);
                setAtRiskStudents(atRisk);
            }
        } catch (error) {
            console.error('Error fetching dashboard data:', error);
        } finally {
            setLoading(false);
        }
    };

    if (loading) {
        return (
            <div className="flex items-center justify-center h-96">
                <Spinner size="lg" />
            </div>
        );
    }

    return (
        <div className="space-y-8">
            {/* Welcome Section */}
            <div className="relative overflow-hidden rounded-3xl bg-gradient-to-r from-primary-600/20 via-accent-600/20 to-primary-600/20 border border-primary-500/30 p-8">
                <div className="relative z-10">
                    <h1 className="text-4xl font-bold text-white mb-2">
                        Welcome back, {userName}! 👋
                    </h1>
                    <p className="text-slate-300 text-lg">
                        Here's what's happening in your university today
                    </p>
                </div>
                <div className="absolute top-0 right-0 w-64 h-64 bg-primary-500/10 rounded-full blur-3xl" />
                <div className="absolute bottom-0 left-0 w-64 h-64 bg-accent-500/10 rounded-full blur-3xl" />
            </div>

            {/* Stats Grid - Only for Admin and Teacher */}
            {!isStudent && (
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-5 gap-6">
                    <StatCard
                        title="Students"
                        value={stats?.students || 0}
                        icon="👨‍🎓"
                        color="primary"
                        trend="+12% from last month"
                    />
                    <StatCard
                        title="Teachers"
                        value={stats?.teachers || 0}
                        icon="👨‍🏫"
                        color="accent"
                        trend="+5% from last month"
                    />
                    <StatCard
                        title="Programs"
                        value={stats?.programs || 0}
                        icon="📚"
                        color="success"
                        trend="2 new this term"
                    />
                    <StatCard
                        title="Subjects"
                        value={stats?.subjects || 0}
                        icon="📖"
                        color="warning"
                        trend="Active courses"
                    />
                    <StatCard
                        title="Exams"
                        value={stats?.exams || 0}
                        icon="📝"
                        color="danger"
                        trend="Upcoming"
                    />
                </div>
            )}

            {/* Main Content Grid */}
            <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
                {/* Recent Announcements */}
                <Card className="lg:col-span-2">
                    <div className="flex items-center justify-between mb-6">
                        <h3 className="text-xl font-bold text-white">Recent Announcements</h3>
                        <Badge variant="primary">{announcements.length} Active</Badge>
                    </div>

                    {announcements.length === 0 ? (
                        <div className="text-center py-12 text-slate-400">
                            <p className="text-4xl mb-2">📢</p>
                            <p>No announcements at the moment</p>
                        </div>
                    ) : (
                        <div className="space-y-4">
                            {announcements.map((announcement) => (
                                <div
                                    key={announcement.id}
                                    className="p-4 bg-slate-800/50 rounded-xl border border-slate-700/50 hover:border-slate-600 transition-all"
                                >
                                    <div className="flex items-start justify-between mb-2">
                                        <h4 className="font-semibold text-white">{announcement.title}</h4>
                                        <Badge variant="success" className="text-xs">New</Badge>
                                    </div>
                                    <p className="text-sm text-slate-300 mb-3 line-clamp-2">
                                        {announcement.content}
                                    </p>
                                    <div className="flex items-center gap-4 text-xs text-slate-400">
                                        <span>📅 {new Date(announcement.createdAt).toLocaleDateString()}</span>
                                        {announcement.teacherName && (
                                            <span>👨‍🏫 {announcement.teacherName}</span>
                                        )}
                                    </div>
                                </div>
                            ))}
                        </div>
                    )}
                </Card>

                {/* Quick Actions - Only for Admin and Teacher */}
                {!isStudent && (
                    <Card>
                        <h3 className="text-xl font-bold text-white mb-6">Quick Actions</h3>
                        <div className="space-y-3">
                            <QuickActionButton
                                icon="👨‍🎓"
                                label="Add Student"
                                color="primary"
                                onClick={() => setActiveTab('students')}
                            />
                            {userRole === 'Admin' && (
                                <QuickActionButton
                                    icon="👨‍🏫"
                                    label="Add Teacher"
                                    color="accent"
                                    onClick={() => setActiveTab('teachers')}
                                />
                            )}
                            <QuickActionButton
                                icon="📚"
                                label="Create Program"
                                color="success"
                                onClick={() => setActiveTab('programs')}
                            />
                            <QuickActionButton
                                icon="📝"
                                label="Schedule Exam"
                                color="warning"
                                onClick={() => setActiveTab('exams')}
                            />
                            <QuickActionButton
                                icon="📢"
                                label="Post Announcement"
                                color="danger"
                                onClick={() => setActiveTab('announcements')}
                            />
                        </div>
                    </Card>
                )}
                {/* At-Risk Students - Only for Admin/Teacher */}
                {!isStudent && atRiskStudents.length > 0 && (
                    <Card>
                        <h3 className="text-xl font-bold text-white mb-6 flex items-center gap-2">
                            ⚠️ Students at Risk
                        </h3>
                        <div className="space-y-4">
                            {atRiskStudents.map((student) => (
                                <div
                                    key={student.id}
                                    className="p-4 bg-danger-500/10 border border-danger-500/20 rounded-xl flex items-center justify-between"
                                >
                                    <div>
                                        <p className="font-semibold text-white">{student.firstName} {student.lastName}</p>
                                        <p className="text-xs text-danger-400">{student.primaryProgramName}</p>
                                    </div>
                                    <Badge variant="danger" className="text-sm font-bold">
                                        GPA: {student.gpa}
                                    </Badge>
                                </div>
                            ))}
                            <button
                                onClick={() => setActiveTab('students')}
                                className="w-full text-center text-sm text-slate-400 hover:text-white transition-colors pt-2"
                            >
                                View all students
                            </button>
                        </div>
                    </Card>
                )}
            </div>

            {/* Dashboard Bottom Section */}
            <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                {/* Recent Activity (Staff) or Today's Schedule (Student) */}
                <Card>
                    <h3 className="text-xl font-bold text-white mb-6">
                        {isStudent ? "Today's Schedule" : "Recent Activity"}
                    </h3>
                    <div className="space-y-4">
                        {isStudent ? (
                            todaySchedule.length === 0 ? (
                                <div className="text-center py-8 text-slate-400">
                                    <p className="text-2xl mb-2">🏖️</p>
                                    <p>No classes scheduled for today</p>
                                </div>
                            ) : (
                                todaySchedule.map((item, idx) => (
                                    <ActivityItem
                                        key={idx}
                                        icon="⏰"
                                        title={item.subjectName || 'Class'}
                                        description={`${item.startTime} - ${item.endTime} | Room: ${item.room || 'N/A'}`}
                                        time={item.dayOfWeek}
                                    />
                                ))
                            )
                        ) : (
                            recentActivity.length === 0 ? (
                                <div className="text-center py-8 text-slate-400">No recent activity</div>
                            ) : (
                                recentActivity.map((item, idx) => (
                                    <ActivityItem
                                        key={idx}
                                        icon={item.icon}
                                        title={item.title}
                                        description={item.desc}
                                        time={formatTimeAgo(item.time)}
                                    />
                                ))
                            )
                        )}
                    </div>
                </Card>

                {/* Upcoming Events (Exams) */}
                <Card>
                    <h3 className="text-xl font-bold text-white mb-6">Upcoming Exams</h3>
                    <div className="space-y-4">
                        {upcomingEvents.length === 0 ? (
                            <div className="text-center py-8 text-slate-400">No upcoming exams</div>
                        ) : (
                            upcomingEvents.map((event, idx) => {
                                const eventDate = new Date(event.date);
                                return (
                                    <EventItem
                                        key={idx}
                                        date={eventDate.getDate()}
                                        month={eventDate.toLocaleString('default', { month: 'short' }).toUpperCase()}
                                        title={event.name}
                                        description={`${event.subjectName || 'Subject'} | ${event.startTime || ''}`}
                                        color={idx % 2 === 0 ? "primary" : "warning"}
                                    />
                                );
                            })
                        )}
                    </div>
                </Card>
            </div>
        </div>
    );
};

const StatCard = ({ title, value, icon, color, trend }) => {
    const colors = {
        primary: 'from-primary-600/20 to-primary-600/5 border-primary-600/30',
        accent: 'from-accent-600/20 to-accent-600/5 border-accent-600/30',
        success: 'from-success-600/20 to-success-600/5 border-success-600/30',
        warning: 'from-warning-600/20 to-warning-600/5 border-warning-600/30',
        danger: 'from-danger-600/20 to-danger-600/5 border-danger-600/30',
    };

    return (
        <div className={`bg-gradient-to-br ${colors[color]} border rounded-2xl p-6 hover:scale-105 transition-transform duration-300`}>
            <div className="flex items-center justify-between mb-4">
                <span className="text-4xl">{icon}</span>
                <Badge variant={color} className="text-xs">{trend}</Badge>
            </div>
            <h3 className="text-sm text-slate-400 mb-1">{title}</h3>
            <p className="text-3xl font-bold text-white">{value}</p>
        </div>
    );
};

const QuickActionButton = ({ icon, label, color, onClick }) => {
    const colors = {
        primary: 'hover:bg-primary-600/10 hover:border-primary-600/30',
        accent: 'hover:bg-accent-600/10 hover:border-accent-600/30',
        success: 'hover:bg-success-600/10 hover:border-success-600/30',
        warning: 'hover:bg-warning-600/10 hover:border-warning-600/30',
        danger: 'hover:bg-danger-600/10 hover:border-danger-600/30',
    };

    return (
        <button
            onClick={onClick}
            className={`w-full flex items-center gap-3 p-4 rounded-xl border border-slate-800 ${colors[color]} transition-all text-left`}
        >
            <span className="text-2xl">{icon}</span>
            <span className="font-medium text-white">{label}</span>
        </button>
    );
};

const ActivityItem = ({ icon, title, description, time }) => {
    return (
        <div className="flex items-start gap-4 p-4 rounded-xl bg-slate-800/30 border border-slate-800/50">
            <span className="text-2xl">{icon}</span>
            <div className="flex-1 min-w-0">
                <p className="font-semibold text-white">{title}</p>
                <p className="text-sm text-slate-400 mt-1">{description}</p>
                <p className="text-xs text-slate-500 mt-2">{time}</p>
            </div>
        </div>
    );
};

const EventItem = ({ date, month, title, description, color }) => {
    const colors = {
        primary: 'bg-primary-600/20 text-primary-400',
        success: 'bg-success-600/20 text-success-400',
        warning: 'bg-warning-600/20 text-warning-400',
        danger: 'bg-danger-600/20 text-danger-400',
    };

    return (
        <div className="flex items-start gap-4 p-4 rounded-xl bg-slate-800/30 border border-slate-800/50">
            <div className={`${colors[color]} rounded-xl p-3 text-center min-w-[60px]`}>
                <p className="text-2xl font-bold">{date}</p>
                <p className="text-xs font-semibold">{month}</p>
            </div>
            <div className="flex-1">
                <p className="font-semibold text-white">{title}</p>
                <p className="text-sm text-slate-400 mt-1">{description}</p>
            </div>
        </div>
    );
};

const formatTimeAgo = (dateString) => {
    if (!dateString) return '';
    const date = new Date(dateString);
    const now = new Date();
    const diffInSeconds = Math.floor((now - date) / 1000);

    if (diffInSeconds < 60) return 'Just now';
    if (diffInSeconds < 3600) return `${Math.floor(diffInSeconds / 60)}m ago`;
    if (diffInSeconds < 86400) return `${Math.floor(diffInSeconds / 3600)}h ago`;
    return date.toLocaleDateString();
};

export default DashboardHome;
