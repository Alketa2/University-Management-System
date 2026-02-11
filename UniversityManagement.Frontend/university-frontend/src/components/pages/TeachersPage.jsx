import { useState, useEffect } from 'react';
import { Button, Card, Input, Modal, Badge, Alert, Spinner } from '../ui/UIComponents';
import apiClient from '../../utils/apiClient';
import { API_ENDPOINTS } from '../../config/api';

const TeachersPage = () => {
    const [teachers, setTeachers] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [selectedTeacher, setSelectedTeacher] = useState(null);
    const [searchTerm, setSearchTerm] = useState('');

    useEffect(() => {
        fetchTeachers();
    }, []);

    const fetchTeachers = async () => {
        setLoading(true);
        try {
            const data = await apiClient.get(API_ENDPOINTS.TEACHERS.BASE);
            setTeachers(data);
        } catch (err) {
            setError(err.message || 'Failed to fetch teachers');
        } finally {
            setLoading(false);
        }
    };

    const handleDelete = async (id) => {
        if (!window.confirm('Are you sure you want to delete this teacher?')) return;

        try {
            await apiClient.delete(API_ENDPOINTS.TEACHERS.BY_ID(id));
            setTeachers(teachers.filter(t => t.id !== id));
        } catch (err) {
            setError(err.message || 'Failed to delete teacher');
        }
    };

    const filteredTeachers = teachers.filter(teacher =>
        teacher.firstName?.toLowerCase().includes(searchTerm.toLowerCase()) ||
        teacher.lastName?.toLowerCase().includes(searchTerm.toLowerCase()) ||
        teacher.email?.toLowerCase().includes(searchTerm.toLowerCase()) ||
        teacher.department?.toLowerCase().includes(searchTerm.toLowerCase())
    );

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
                    <h2 className="text-3xl font-bold text-white">Teachers Management</h2>
                    <p className="text-slate-400 mt-1">Manage faculty members and their assignments</p>
                </div>
                <Button onClick={() => { setSelectedTeacher(null); setIsModalOpen(true); }}>
                    + Add Teacher
                </Button>
            </div>

            {error && <Alert type="error" message={error} onClose={() => setError('')} />}

            {/* Search bar */}
            <div className="flex gap-4">
                <Input
                    placeholder="Search teachers..."
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
                            <p className="text-sm text-slate-400">Total Teachers</p>
                            <p className="text-3xl font-bold text-white mt-1">{teachers.length}</p>
                        </div>
                        <div className="w-12 h-12 bg-accent-600/20 rounded-xl flex items-center justify-center text-2xl">
                            👨‍🏫
                        </div>
                    </div>
                </Card>
                <Card>
                    <div className="flex items-center justify-between">
                        <div>
                            <p className="text-sm text-slate-400">Active</p>
                            <p className="text-3xl font-bold text-success-400 mt-1">
                                {teachers.filter(t => t.isActive !== false).length}
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
                            <p className="text-sm text-slate-400">Departments</p>
                            <p className="text-3xl font-bold text-primary-400 mt-1">
                                {new Set(teachers.map(t => t.department).filter(Boolean)).size}
                            </p>
                        </div>
                        <div className="w-12 h-12 bg-primary-600/20 rounded-xl flex items-center justify-center text-2xl">
                            🏛️
                        </div>
                    </div>
                </Card>
                <Card>
                    <div className="flex items-center justify-between">
                        <div>
                            <p className="text-sm text-slate-400">New This Year</p>
                            <p className="text-3xl font-bold text-accent-400 mt-1">
                                {teachers.filter(t => {
                                    const hired = new Date(t.hireDate || t.createdAt);
                                    const now = new Date();
                                    return hired.getFullYear() === now.getFullYear();
                                }).length}
                            </p>
                        </div>
                        <div className="w-12 h-12 bg-accent-600/20 rounded-xl flex items-center justify-center text-2xl">
                            📈
                        </div>
                    </div>
                </Card>
            </div>

            {/* Teachers table */}
            <Card>
                <div className="overflow-x-auto">
                    <table className="w-full">
                        <thead>
                            <tr className="border-b border-slate-800">
                                <th className="text-left py-4 px-4 text-sm font-semibold text-slate-300">Name</th>
                                <th className="text-left py-4 px-4 text-sm font-semibold text-slate-300">Email</th>
                                <th className="text-left py-4 px-4 text-sm font-semibold text-slate-300">Department</th>
                                <th className="text-left py-4 px-4 text-sm font-semibold text-slate-300">Phone</th>
                                <th className="text-left py-4 px-4 text-sm font-semibold text-slate-300">Status</th>
                                <th className="text-right py-4 px-4 text-sm font-semibold text-slate-300">Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            {filteredTeachers.length === 0 ? (
                                <tr>
                                    <td colSpan="6" className="text-center py-12 text-slate-400">
                                        No teachers found
                                    </td>
                                </tr>
                            ) : (
                                filteredTeachers.map((teacher) => (
                                    <tr key={teacher.id} className="border-b border-slate-800/50 hover:bg-slate-800/30 transition-colors">
                                        <td className="py-4 px-4">
                                            <div className="flex items-center gap-3">
                                                <div className="w-10 h-10 rounded-full bg-gradient-to-r from-accent-500 to-primary-500 flex items-center justify-center text-white font-bold">
                                                    {teacher.firstName?.[0]}{teacher.lastName?.[0]}
                                                </div>
                                                <div>
                                                    <p className="font-medium text-white">{teacher.firstName} {teacher.lastName}</p>
                                                    <p className="text-xs text-slate-400">{teacher.department || 'Faculty'}</p>
                                                </div>
                                            </div>
                                        </td>
                                        <td className="py-4 px-4 text-slate-300">{teacher.email}</td>
                                        <td className="py-4 px-4">
                                            <Badge variant="primary">{teacher.department || 'N/A'}</Badge>
                                        </td>
                                        <td className="py-4 px-4 text-slate-300">{teacher.phone || 'N/A'}</td>
                                        <td className="py-4 px-4">
                                            <Badge variant={teacher.isActive !== false ? 'success' : 'default'}>
                                                {teacher.isActive !== false ? 'Active' : 'Inactive'}
                                            </Badge>
                                        </td>
                                        <td className="py-4 px-4">
                                            <div className="flex items-center justify-end gap-2">
                                                <Button
                                                    variant="ghost"
                                                    size="sm"
                                                    onClick={() => { setSelectedTeacher(teacher); setIsModalOpen(true); }}
                                                >
                                                    Edit
                                                </Button>
                                                <Button
                                                    variant="ghost"
                                                    size="sm"
                                                    onClick={() => handleDelete(teacher.id)}
                                                    className="text-danger-400 hover:text-danger-300"
                                                >
                                                    Delete
                                                </Button>
                                            </div>
                                        </td>
                                    </tr>
                                ))
                            )}
                        </tbody>
                    </table>
                </div>
            </Card>

            {/* Teacher Modal */}
            <TeacherModal
                isOpen={isModalOpen}
                onClose={() => setIsModalOpen(false)}
                teacher={selectedTeacher}
                onSuccess={fetchTeachers}
            />
        </div>
    );
};

const TeacherModal = ({ isOpen, onClose, teacher, onSuccess }) => {
    const [formData, setFormData] = useState({
        firstName: '',
        lastName: '',
        email: '',
        phone: '',
        department: '',
        hireDate: '',
    });
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');

    useEffect(() => {
        if (teacher) {
            setFormData({
                firstName: teacher.firstName || '',
                lastName: teacher.lastName || '',
                email: teacher.email || '',
                phone: teacher.phone || '',
                department: teacher.department || '',
                hireDate: teacher.hireDate ? teacher.hireDate.split('T')[0] : '',
            });
        } else {
            setFormData({
                firstName: '',
                lastName: '',
                email: '',
                phone: '',
                department: '',
                hireDate: '',
            });
        }
    }, [teacher]);

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);
        setError('');

        try {
            if (teacher) {
                await apiClient.put(API_ENDPOINTS.TEACHERS.BY_ID(teacher.id), {
                    id: teacher.id,
                    ...formData,
                });
            } else {
                await apiClient.post(API_ENDPOINTS.TEACHERS.BASE, formData);
            }
            onSuccess();
            onClose();
        } catch (err) {
            setError(err.message || 'Failed to save teacher');
        } finally {
            setLoading(false);
        }
    };

    return (
        <Modal
            isOpen={isOpen}
            onClose={onClose}
            title={teacher ? 'Edit Teacher' : 'Add New Teacher'}
        >
            {error && <Alert type="error" message={error} onClose={() => setError('')} className="mb-4" />}

            <form onSubmit={handleSubmit} className="space-y-4">
                <div className="grid grid-cols-2 gap-4">
                    <Input
                        label="First Name"
                        value={formData.firstName}
                        onChange={(e) => setFormData({ ...formData, firstName: e.target.value })}
                        required
                    />
                    <Input
                        label="Last Name"
                        value={formData.lastName}
                        onChange={(e) => setFormData({ ...formData, lastName: e.target.value })}
                        required
                    />
                </div>

                <Input
                    label="Email"
                    type="email"
                    value={formData.email}
                    onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                    required
                />

                <div className="grid grid-cols-2 gap-4">
                    <Input
                        label="Phone Number"
                        value={formData.phone}
                        onChange={(e) => setFormData({ ...formData, phone: e.target.value })}
                        required
                    />
                    <Input
                        label="Department"
                        value={formData.department}
                        onChange={(e) => setFormData({ ...formData, department: e.target.value })}
                        placeholder="e.g., Computer Science"
                        required
                    />
                </div>

                <Input
                    label="Hire Date"
                    type="date"
                    value={formData.hireDate}
                    onChange={(e) => setFormData({ ...formData, hireDate: e.target.value })}
                />

                <div className="flex gap-3 pt-4">
                    <Button type="submit" disabled={loading} className="flex-1">
                        {loading ? 'Saving...' : teacher ? 'Update Teacher' : 'Add Teacher'}
                    </Button>
                    <Button type="button" variant="secondary" onClick={onClose} className="flex-1">
                        Cancel
                    </Button>
                </div>
            </form>
        </Modal>
    );
};

export default TeachersPage;
