import { useState, useEffect } from 'react';
import { Button, Card, Input, Modal, Badge, Alert, Spinner, Select } from '../ui/UIComponents';
import apiClient from '../../utils/apiClient';
import { API_ENDPOINTS } from '../../config/api';

const SubjectsPage = () => {
    const [subjects, setSubjects] = useState([]);
    const [programs, setPrograms] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [selectedSubject, setSelectedSubject] = useState(null);
    const [searchTerm, setSearchTerm] = useState('');

    useEffect(() => {
        fetchData();
    }, []);

    const fetchData = async () => {
        setLoading(true);
        try {
            const [subjectsData, programsData] = await Promise.all([
                apiClient.get(API_ENDPOINTS.SUBJECTS.BASE),
                apiClient.get(API_ENDPOINTS.PROGRAMS.BASE)
            ]);
            setSubjects(subjectsData);
            setPrograms(programsData);
        } catch (err) {
            setError(err.message || 'Failed to fetch data');
        } finally {
            setLoading(false);
        }
    };

    const handleDelete = async (id) => {
        if (!window.confirm('Are you sure you want to delete this subject?')) return;

        try {
            await apiClient.delete(API_ENDPOINTS.SUBJECTS.BY_ID(id));
            setSubjects(subjects.filter(s => s.id !== id));
        } catch (err) {
            setError(err.message || 'Failed to delete subject');
        }
    };

    const filteredSubjects = subjects.filter(subject =>
        subject.name?.toLowerCase().includes(searchTerm.toLowerCase()) ||
        subject.code?.toLowerCase().includes(searchTerm.toLowerCase())
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
                    <h2 className="text-3xl font-bold text-white">Subjects Management</h2>
                    <p className="text-slate-400 mt-1">Manage academic subjects and courses</p>
                </div>
                <Button onClick={() => { setSelectedSubject(null); setIsModalOpen(true); }}>
                    + Add Subject
                </Button>
            </div>

            {error && <Alert type="error" message={error} onClose={() => setError('')} />}

            {/* Search bar */}
            <div className="flex gap-4">
                <Input
                    placeholder="Search subjects..."
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
                            <p className="text-sm text-slate-400">Total Subjects</p>
                            <p className="text-3xl font-bold text-white mt-1">{subjects.length}</p>
                        </div>
                        <div className="w-12 h-12 bg-primary-600/20 rounded-xl flex items-center justify-center text-2xl">
                            📚
                        </div>
                    </div>
                </Card>
                <Card>
                    <div className="flex items-center justify-between">
                        <div>
                            <p className="text-sm text-slate-400">Active</p>
                            <p className="text-3xl font-bold text-success-400 mt-1">
                                {subjects.filter(s => s.isActive !== false).length}
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
                            <p className="text-sm text-slate-400">Programs</p>
                            <p className="text-3xl font-bold text-accent-400 mt-1">{programs.length}</p>
                        </div>
                        <div className="w-12 h-12 bg-accent-600/20 rounded-xl flex items-center justify-center text-2xl">
                            🎓
                        </div>
                    </div>
                </Card>
                <Card>
                    <div className="flex items-center justify-between">
                        <div>
                            <p className="text-sm text-slate-400">Avg Credits</p>
                            <p className="text-3xl font-bold text-primary-400 mt-1">
                                {subjects.length > 0
                                    ? (subjects.reduce((sum, s) => sum + (s.credits || 0), 0) / subjects.length).toFixed(1)
                                    : 0
                                }
                            </p>
                        </div>
                        <div className="w-12 h-12 bg-primary-600/20 rounded-xl flex items-center justify-center text-2xl">
                            ⭐
                        </div>
                    </div>
                </Card>
            </div>

            {/* Subjects table */}
            <Card>
                <div className="overflow-x-auto">
                    <table className="w-full">
                        <thead>
                            <tr className="border-b border-slate-800">
                                <th className="text-left py-4 px-4 text-sm font-semibold text-slate-300">Code</th>
                                <th className="text-left py-4 px-4 text-sm font-semibold text-slate-300">Name</th>
                                <th className="text-left py-4 px-4 text-sm font-semibold text-slate-300">Credits</th>
                                <th className="text-left py-4 px-4 text-sm font-semibold text-slate-300">Semester</th>
                                <th className="text-left py-4 px-4 text-sm font-semibold text-slate-300">Status</th>
                                <th className="text-right py-4 px-4 text-sm font-semibold text-slate-300">Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            {filteredSubjects.length === 0 ? (
                                <tr>
                                    <td colSpan="6" className="text-center py-12 text-slate-400">
                                        No subjects found
                                    </td>
                                </tr>
                            ) : (
                                filteredSubjects.map((subject) => (
                                    <tr key={subject.id} className="border-b border-slate-800/50 hover:bg-slate-800/30 transition-colors">
                                        <td className="py-4 px-4">
                                            <span className="font-mono text-primary-400 font-semibold">{subject.code}</span>
                                        </td>
                                        <td className="py-4 px-4">
                                            <p className="font-medium text-white">{subject.name}</p>
                                            {subject.description && (
                                                <p className="text-sm text-slate-400 mt-1">{subject.description.substring(0, 50)}...</p>
                                            )}
                                        </td>
                                        <td className="py-4 px-4 text-slate-300">{subject.credits || 'N/A'}</td>
                                        <td className="py-4 px-4 text-slate-300">{subject.semester || 'N/A'}</td>
                                        <td className="py-4 px-4">
                                            <Badge variant={subject.isActive !== false ? 'success' : 'default'}>
                                                {subject.isActive !== false ? 'Active' : 'Inactive'}
                                            </Badge>
                                        </td>
                                        <td className="py-4 px-4">
                                            <div className="flex items-center justify-end gap-2">
                                                <Button
                                                    variant="ghost"
                                                    size="sm"
                                                    onClick={() => { setSelectedSubject(subject); setIsModalOpen(true); }}
                                                >
                                                    Edit
                                                </Button>
                                                <Button
                                                    variant="ghost"
                                                    size="sm"
                                                    onClick={() => handleDelete(subject.id)}
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

            {/* Subject Modal */}
            <SubjectModal
                isOpen={isModalOpen}
                onClose={() => setIsModalOpen(false)}
                subject={selectedSubject}
                programs={programs}
                onSuccess={fetchData}
            />
        </div>
    );
};

const SubjectModal = ({ isOpen, onClose, subject, programs, onSuccess }) => {
    const [formData, setFormData] = useState({
        name: '',
        code: '',
        description: '',
        credits: '',
        semester: '',
        programId: '',
    });
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');

    useEffect(() => {
        if (subject) {
            setFormData({
                name: subject.name || '',
                code: subject.code || '',
                description: subject.description || '',
                credits: subject.credits || '',
                semester: subject.semester || '',
                programId: subject.programId || '',
            });
        } else {
            setFormData({
                name: '',
                code: '',
                description: '',
                credits: '',
                semester: '',
                programId: '',
            });
        }
    }, [subject]);

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);
        setError('');

        try {
            const payload = {
                ...formData,
                credits: parseInt(formData.credits) || 0,
                semester: parseInt(formData.semester) || 1,
                programId: formData.programId || null,
            };

            if (subject) {
                await apiClient.put(API_ENDPOINTS.SUBJECTS.BY_ID(subject.id), {
                    id: subject.id,
                    ...payload,
                });
            } else {
                await apiClient.post(API_ENDPOINTS.SUBJECTS.BASE, payload);
            }
            onSuccess();
            onClose();
        } catch (err) {
            setError(err.message || 'Failed to save subject');
        } finally {
            setLoading(false);
        }
    };

    return (
        <Modal
            isOpen={isOpen}
            onClose={onClose}
            title={subject ? 'Edit Subject' : 'Add New Subject'}
        >
            {error && <Alert type="error" message={error} onClose={() => setError('')} className="mb-4" />}

            <form onSubmit={handleSubmit} className="space-y-4">
                <div className="grid grid-cols-2 gap-4">
                    <Input
                        label="Subject Code"
                        value={formData.code}
                        onChange={(e) => setFormData({ ...formData, code: e.target.value })}
                        placeholder="e.g., CS101"
                        required
                    />
                    <Input
                        label="Credits"
                        type="number"
                        value={formData.credits}
                        onChange={(e) => setFormData({ ...formData, credits: e.target.value })}
                        min="1"
                        max="10"
                        required
                    />
                </div>

                <Input
                    label="Subject Name"
                    value={formData.name}
                    onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                    placeholder="e.g., Introduction to Computer Science"
                    required
                />

                <Input
                    label="Description"
                    value={formData.description}
                    onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                    placeholder="Brief description of the subject"
                />

                <div className="grid grid-cols-2 gap-4">
                    <Input
                        label="Semester"
                        type="number"
                        value={formData.semester}
                        onChange={(e) => setFormData({ ...formData, semester: e.target.value })}
                        min="1"
                        max="8"
                    />
                    <Select
                        label="Program"
                        value={formData.programId}
                        onChange={(e) => setFormData({ ...formData, programId: e.target.value })}
                        options={[
                            { value: '', label: 'Select Program (Optional)' },
                            ...programs.map(p => ({ value: p.id, label: p.name }))
                        ]}
                    />
                </div>

                <div className="flex gap-3 pt-4">
                    <Button type="submit" disabled={loading} className="flex-1">
                        {loading ? 'Saving...' : subject ? 'Update Subject' : 'Add Subject'}
                    </Button>
                    <Button type="button" variant="secondary" onClick={onClose} className="flex-1">
                        Cancel
                    </Button>
                </div>
            </form>
        </Modal>
    );
};

export default SubjectsPage;
