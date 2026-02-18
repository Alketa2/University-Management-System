import { useState, useEffect, useRef } from 'react';
import notificationService from '../../utils/notificationService';

const NotificationBell = () => {
    const [notifications, setNotifications] = useState([]);
    const [isOpen, setIsOpen] = useState(false);
    const [loading, setLoading] = useState(false);
    const dropdownRef = useRef(null);

    const unreadCount = notifications.filter(n => !n.isRead).length;

    useEffect(() => {
        fetchNotifications();

        // Listen for clicks outside to close dropdown
        const handleClickOutside = (event) => {
            if (dropdownRef.current && !dropdownRef.current.contains(event.target)) {
                setIsOpen(false);
            }
        };
        document.addEventListener('mousedown', handleClickOutside);
        return () => document.removeEventListener('mousedown', handleClickOutside);
    }, []);

    const fetchNotifications = async () => {
        try {
            setLoading(true);
            const data = await notificationService.getNotifications();
            setNotifications(data || []);
        } catch (error) {
            console.error('Failed to fetch notifications:', error);
        } finally {
            setLoading(false);
        }
    };

    const handleMarkAsRead = async (id) => {
        try {
            await notificationService.markAsRead(id);
            setNotifications(notifications.map(n =>
                n.id === id ? { ...n, isRead: true } : n
            ));
        } catch (error) {
            console.error('Failed to mark as read:', error);
        }
    };

    return (
        <div className="relative" ref={dropdownRef}>
            <button
                onClick={() => setIsOpen(!isOpen)}
                className="relative p-2 text-slate-400 hover:text-white transition-colors"
            >
                <svg className="w-6 h-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9" />
                </svg>
                {unreadCount > 0 && (
                    <span className="absolute top-1 right-1 flex h-4 w-4 items-center justify-center bg-danger-500 rounded-full text-[10px] text-white font-bold border-2 border-slate-900">
                        {unreadCount}
                    </span>
                )}
            </button>

            {isOpen && (
                <div className="absolute right-0 mt-3 w-80 bg-slate-900 border border-slate-800 rounded-2xl shadow-2xl z-50 overflow-hidden ring-1 ring-white/10">
                    <div className="p-4 border-b border-slate-800 flex justify-between items-center bg-slate-900/50">
                        <h3 className="font-bold text-white uppercase text-xs tracking-wider">Notifications</h3>
                        <button
                            onClick={fetchNotifications}
                            className="text-[10px] text-primary-400 hover:text-primary-300 font-bold uppercase"
                        >
                            Refresh
                        </button>
                    </div>

                    <div className="max-h-96 overflow-y-auto">
                        {loading ? (
                            <div className="p-8 text-center text-slate-500 text-sm">Loading...</div>
                        ) : notifications.length === 0 ? (
                            <div className="p-8 text-center text-slate-500 text-sm italic">
                                No notifications yet
                            </div>
                        ) : (
                            <div className="divide-y divide-slate-800/50">
                                {notifications.map((notification) => (
                                    <div
                                        key={notification.id}
                                        className={`p-4 hover:bg-slate-800/50 transition-colors cursor-pointer group ${!notification.isRead ? 'bg-primary-600/5' : ''}`}
                                        onClick={() => !notification.isRead && handleMarkAsRead(notification.id)}
                                    >
                                        <div className="flex justify-between items-start mb-1">
                                            <span className={`text-[10px] px-1.5 py-0.5 rounded font-bold uppercase tracking-wider ${notification.type === 'Info' ? 'bg-blue-500/10 text-blue-400' :
                                                    notification.type === 'Warning' ? 'bg-yellow-500/10 text-yellow-400' :
                                                        notification.type === 'Error' ? 'bg-red-500/10 text-red-400' :
                                                            'bg-primary-500/10 text-primary-400'
                                                }`}>
                                                {notification.type}
                                            </span>
                                            <span className="text-[10px] text-slate-500 font-medium">
                                                {new Date(notification.createdAt).toLocaleDateString()}
                                            </span>
                                        </div>
                                        <h4 className={`text-sm font-semibold mb-1 ${!notification.isRead ? 'text-white' : 'text-slate-300'}`}>
                                            {notification.title}
                                        </h4>
                                        <p className="text-xs text-slate-400 leading-relaxed mb-2">
                                            {notification.message}
                                        </p>
                                        {!notification.isRead && (
                                            <div className="h-1.5 w-1.5 rounded-full bg-primary-500 group-hover:scale-125 transition-transform"></div>
                                        )}
                                    </div>
                                ))}
                            </div>
                        )}
                    </div>

                    <div className="p-3 bg-slate-900/80 border-t border-slate-800 text-center">
                        <button className="text-[11px] text-slate-400 hover:text-white font-medium transition-colors">
                            View All Notifications
                        </button>
                    </div>
                </div>
            )}
        </div>
    );
};

export default NotificationBell;
