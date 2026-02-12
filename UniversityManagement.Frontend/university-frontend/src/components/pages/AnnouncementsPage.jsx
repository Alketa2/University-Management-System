import { useState, useEffect } from 'react';
import { Button, Card, Input, Modal, Badge, Alert, Spinner, Select } from '../ui/UIComponents';
import apiClient from '../../utils/apiClient';
import { API_ENDPOINTS } from '../../config/api';
import authService from '../../utils/authService';

const AnnouncementsPage = () => {
    const [announcements, setAnnouncements] = useState([]);
    const [teachers, setTeachers] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [selectedAnnouncement, setSelectedAnnouncement] = useState(null);
    const [searchTerm, setSearchTerm] = useState('');

    const currentUser = authService.getUser();
    const isAdmin = currentUser?.role === 'Admin';

    useEffect(() => {
        fetchData();
    }, []);

    const fetchData = async () => {
        setLoading(true);
        try {
            const [announcementsData, teachersData] = await Promise.all([
                apiClient.get(API_ENDPOINTS.ANNOUNCEMENTS.BASE),
                apiClient.get(API_ENDPOINTS.TEACHERS.BASE)
            ]);
            setAnnouncements(announcementsData);
            setTeachers(teachersData);
        } catch (err) {
            setError(err.message || 'Failed to fetch data');
        } finally {
            setLoading(false);
        }
    };

    const handleDelete = async (id) => {
        if (!window.confirm('Are you sure you want to delete this announcement?')) return;

        try {
            await apiClient.delete(API_ENDPOINTS.ANNOUNCEMENTS.BY_ID(id));
            setAnnouncements(announcements.filter(a => a.id !== id));
        } catch (err) {
            setError(err.message || 'Failed to delete announcement');
        }
    };

    const getTeacherName = (teacherId) => {
        const teacher = teachers.find(t => t.id === teacherId);
        return teacher ? `${teacher.firstName} ${teacher.lastName}` : 'Unknown';
    };

    const filteredAnnouncements = announcements.filter(announcement =>
        announcement.title?.toLowerCase().includes(searchTerm.toLowerCase()) ||
        announcement.content?.toLowerCase().includes(searchTerm.toLowerCase())
    );

    const activeAnnouncements = announcements.filter(a => a.isActive === true);
    const recentAnnouncements = announcements.filter(a => {
        const created = new Date(a.createdAt);
        const daysDiff = (new Date() - created) / (1000 * 60 * 60 * 24);
        return daysDiff <= 7;
    });

    if (loading) {
        return (
            <div className="flex items-center justify-center h-96">
                <Spinner size="lg" />
            </div>
        );
    }

    return (
        <div className="space-y-6">
            {/* Header */}
            <div className="flex items-center justify-between">
                <div>
                    <h2 className="text-3xl font-bold text-white">Announcements</h2>
                    <p className="text-slate-400 mt-1">Important notices and updates</p>
                </div>
                {isAdmin && (
                    <Button onClick={() => { setSelectedAnnouncement(null); setIsModalOpen(true); }}>
                        + Create Announcement
                    </Button>
                )}
            </div>

            {error && <Alert type="error" message={error} onClose={() => setError('')} />}

            {/* Search bar */}
            <div className="flex gap-4">
                <Input
                    placeholder="Search announcements..."
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                    className="max-w-md"
                />
            </div>

            {/* Stats */}
            <div className="grid grid-cols-1 md:grid-cols-4 gap-6">
                <Card>
                    <div className="flex items-center justify-between">
                        <div>
                            <p className="text-sm text-slate-400">Total</p>
                            <p className="text-3xl font-bold text-white mt-1">{announcements.length}</p>
                        </div>
                        <div className="w-12 h-12 bg-primary-600/20 rounded-xl flex items-center justify-center text-2xl">
                            📢
                        </div>
                    </div>
                </Card>
                <Card>
                    <div className="flex items-center justify-between">
                        <div>
                            <p className="text-sm text-slate-400">Active</p>
                            <p className="text-3xl font-bold text-success-400 mt-1">
                                {activeAnnouncements.length}
                            </p>
                        </div>
                        <div className="w-12 h-12 bg-success-600/20 rounded-xl flex items-center justify-center text-2xl">
                            ✓
                        </div>
                    </div>
                </Card>
                <Card>
                    <div className="flex items-center justify-between">
                        <div>
                            <p className="text-sm text-slate-400">This Week</p>
                            <p className="text-3xl font-bold text-accent-400 mt-1">
                                {recentAnnouncements.length}
                            </p>
                        </div>
                        <div className="w-12 h-12 bg-accent-600/20 rounded-xl flex items-center justify-center text-2xl">
                            🆕
                        </div>
                    </div>
                </Card>
                <Card>
                    <div className="flex items-center justify-between">
                        <div>
                            <p className="text-sm text-slate-400">Important</p>
                            <p className="text-3xl font-bold text-warning-400 mt-1">
                                {announcements.filter(a => a.priority === 'High' || a.isImportant).length}
                            </p>
                        </div>
                        <div className="w-12 h-12 bg-warning-600/20 rounded-xl flex items-center justify-center text-2xl">
                            ⚠️
                        </div>
                    </div>
                </Card>
            </div>

            {/* Announcements Grid */}
            <div className="grid grid-cols-1 gap-4">
                {filteredAnnouncements.length === 0 ? (
                    <Card>
                        <div className="text-center py-12 text-slate-400">
                            No announcements found
                        </div>
                    </Card>
                ) : (
                    filteredAnnouncements
                        .sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt))
                        .map((announcement) => {
                            const isImportant = announcement.priority === 'High' || announcement.isImportant;
                            const isRecent = recentAnnouncements.includes(announcement);

                            return (
                                <Card
                                    key={announcement.id}
                                    className={`${isImportant ? 'border-warning-500/50' : ''}`}
                                    hover
                                >
                                    <div className="flex items-start justify-between gap-4">
                                        <div className="flex-1">
                                            <div className="flex items-center gap-3 mb-2">
                                                <h3 className="text-xl font-bold text-white">
                                                    {announcement.title}
                                                </h3>
                                                {isImportant && (
                                                    <Badge variant="warning">Important</Badge>
                                                )}
                                                {isRecent && (
                                                    <Badge variant="primary">New</Badge>
                                                )}
                                                {announcement.isActive === true && (
                                                    <Badge variant="success">Active</Badge>
                                                )}
                                                {announcement.isActive === false && (
                                                    <Badge variant="default">Inactive</Badge>
                                                )}
                                            </div>

                                            <p className="text-slate-300 mb-4 leading-relaxed">
                                                {announcement.content}
                                            </p>

                                            <div className="flex items-center gap-4 text-sm text-slate-400">
                                                <span className="flex items-center gap-2">
                                                    👤 {getTeacherName(announcement.teacherId)}
                                                </span>
                                                <span className="flex items-center gap-2">
                                                    📅 {new Date(announcement.createdAt).toLocaleDateString()}
                                                </span>
                                                {announcement.expiryDate && (
                                                    <span className="flex items-center gap-2">
                                                        ⏰ Expires: {new Date(announcement.expiryDate).toLocaleDateString()}
                                                    </span>
                                                )}
                                            </div>
                                        </div>

                                        {isAdmin && (
                                            <div className="flex flex-col gap-2">
                                                <Button
                                                    variant="ghost"
                                                    size="sm"
                                                    onClick={() => {
                                                        setSelectedAnnouncement(announcement);
                                                        setIsModalOpen(true);
                                                    }}
                                                >
                                                    Edit
                                                </Button>
                                                <Button
                                                    variant="ghost"
                                                    size="sm"
                                                    onClick={() => handleDelete(announcement.id)}
                                                    className="text-danger-400 hover:text-danger-300"
                                                >
                                                    Delete
                                                </Button>
                                            </div>
                                        )}
                                    </div>
                                </Card>
                            );
                        })
                )}
            </div>

            {/* Announcement Modal */}
            {isAdmin && (
                <AnnouncementModal
                    isOpen={isModalOpen}
                    onClose={() => setIsModalOpen(false)}
                    announcement={selectedAnnouncement}
                    teachers={teachers}
                    currentUser={currentUser}
                    onSuccess={fetchData}
                />
            )}
        </div>
    );
};

const AnnouncementModal = ({ isOpen, onClose, announcement, teachers, currentUser, onSuccess }) => {
    const [formData, setFormData] = useState({
        title: '',
        content: '',
        teacherId: '',
        isActive: true,
        expiryDate: '',
        priority: 'Medium',
    });
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');

    useEffect(() => {
        if (announcement) {
            setFormData({
                title: announcement.title || '',
                content: announcement.content || '',
                teacherId: announcement.teacherId || '',
                isActive: announcement.isActive !== false,
                expiryDate: announcement.expiryDate
                    ? new Date(announcement.expiryDate).toISOString().split('T')[0]
                    : '',
                priority: announcement.priority || 'Medium',
            });
        } else {
            // Default to current user's teacher ID if available
            const defaultTeacherId = teachers.find(t =>
                t.email === currentUser?.email
            )?.id || '';

            setFormData({
                title: '',
                content: '',
                teacherId: defaultTeacherId,
                isActive: true,
                expiryDate: '',
                priority: 'Medium',
            });
        }
    }, [announcement, teachers, currentUser]);

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);
        setError('');

        try {
            const payload = {
                ...formData,
                isActive: formData.isActive === true || formData.isActive === 'true',
                expiryDate: formData.expiryDate || null,
            };

            if (announcement) {
                await apiClient.put(API_ENDPOINTS.ANNOUNCEMENTS.BY_ID(announcement.id), {
                    id: announcement.id,
                    ...payload,
                });
            } else {
                await apiClient.post(API_ENDPOINTS.ANNOUNCEMENTS.BASE, payload);
            }
            onSuccess();
            onClose();
        } catch (err) {
            setError(err.message || 'Failed to save announcement');
        } finally {
            setLoading(false);
        }
    };

    return (
        <Modal
            isOpen={isOpen}
            onClose={onClose}
            title={announcement ? 'Edit Announcement' : 'Create New Announcement'}
        >
            {error && <Alert type="error" message={error} onClose={() => setError('')} className="mb-4" />}

            <form onSubmit={handleSubmit} className="space-y-4">
                <Input
                    label="Title"
                    value={formData.title}
                    onChange={(e) => setFormData({ ...formData, title: e.target.value })}
                    placeholder="Enter announcement title"
                    required
                />

                <div className="w-full">
                    <label className="block text-sm font-medium text-slate-300 mb-2">
                        Content
                    </label>
                    <textarea
                        className="w-full px-4 py-2.5 bg-slate-900 border border-slate-700 rounded-xl text-white placeholder-slate-500 focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-transparent transition-all min-h-[120px]"
                        value={formData.content}
                        onChange={(e) => setFormData({ ...formData, content: e.target.value })}
                        placeholder="Enter announcement content"
                        required
                    />
                </div>

                <div className="grid grid-cols-2 gap-4">
                    <Select
                        label="Teacher/Author"
                        value={formData.teacherId}
                        onChange={(e) => setFormData({ ...formData, teacherId: e.target.value })}
                        options={[
                            { value: '', label: 'Select Teacher' },
                            ...teachers.map(t => ({
                                value: t.id,
                                label: `${t.firstName} ${t.lastName}`
                            }))
                        ]}
                        required
                    />
                    <Select
                        label="Priority"
                        value={formData.priority}
                        onChange={(e) => setFormData({ ...formData, priority: e.target.value })}
                        options={[
                            { value: 'Low', label: 'Low' },
                            { value: 'Medium', label: 'Medium' },
                            { value: 'High', label: 'High' }
                        ]}
                    />
                </div>

                <div className="grid grid-cols-2 gap-4">
                    <Input
                        label="Expiry Date (Optional)"
                        type="date"
                        value={formData.expiryDate}
                        onChange={(e) => setFormData({ ...formData, expiryDate: e.target.value })}
                    />
                    <Select
                        label="Status"
                        value={formData.isActive.toString()}
                        onChange={(e) => setFormData({ ...formData, isActive: e.target.value === 'true' })}
                        options={[
                            { value: 'true', label: 'Active' },
                            { value: 'false', label: 'Inactive' }
                        ]}
                    />
                </div>

                <div className="flex gap-3 pt-4">
                    <Button type="submit" disabled={loading} className="flex-1">
                        {loading ? 'Saving...' : announcement ? 'Update Announcement' : 'Create Announcement'}
                    </Button>
                    <Button type="button" variant="secondary" onClick={onClose} className="flex-1">
                        Cancel
                    </Button>
                </div>
            </form>
        </Modal>
    );
};

export default AnnouncementsPage;
