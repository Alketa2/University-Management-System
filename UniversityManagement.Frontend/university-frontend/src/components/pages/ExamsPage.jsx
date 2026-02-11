import { useState, useEffect } from 'react';
import { Button, Card, Input, Modal, Badge, Alert, Spinner, Select } from '../ui/UIComponents';
import apiClient from '../../utils/apiClient';
import { API_ENDPOINTS } from '../../config/api';

const ExamsPage = () => {
    const [exams, setExams] = useState([]);
    const [subjects, setSubjects] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [selectedExam, setSelectedExam] = useState(null);
    const [searchTerm, setSearchTerm] = useState('');

    useEffect(() => {
        fetchData();
    }, []);

    const fetchData = async () => {
        setLoading(true);
        try {
            const [examsData, subjectsData] = await Promise.all([
                apiClient.get(API_ENDPOINTS.EXAMS.BASE),
                apiClient.get(API_ENDPOINTS.SUBJECTS.BASE)
            ]);
            setExams(examsData);
            setSubjects(subjectsData);
        } catch (err) {
            setError(err.message || 'Failed to fetch data');
        } finally {
            setLoading(false);
        }
    };

    const handleDelete = async (id) => {
        if (!window.confirm('Are you sure you want to delete this exam?')) return;

        try {
            await apiClient.delete(API_ENDPOINTS.EXAMS.BY_ID(id));
            setExams(exams.filter(e => e.id !== id));
        } catch (err) {
            setError(err.message || 'Failed to delete exam');
        }
    };

    const getSubjectName = (subjectId) => {
        const subject = subjects.find(s => s.id === subjectId);
        return subject ? subject.name : 'Unknown Subject';
    };

    const filteredExams = exams.filter(exam =>
        exam.title?.toLowerCase().includes(searchTerm.toLowerCase()) ||
        getSubjectName(exam.subjectId)?.toLowerCase().includes(searchTerm.toLowerCase())
    );

    const upcomingExams = exams.filter(exam => {
        const examDate = new Date(exam.examDate);
        return examDate > new Date();
    });

    const completedExams = exams.filter(exam => {
        const examDate = new Date(exam.examDate);
        return examDate <= new Date();
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
                    <h2 className="text-3xl font-bold text-white">Exams Management</h2>
                    <p className="text-slate-400 mt-1">Schedule and manage examinations</p>
                </div>
                <Button onClick={() => { setSelectedExam(null); setIsModalOpen(true); }}>
                    + Schedule Exam
                </Button>
            </div>

            {error && <Alert type="error" message={error} onClose={() => setError('')} />}

            {/* Search bar */}
            <div className="flex gap-4">
                <Input
                    placeholder="Search exams..."
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
                            <p className="text-sm text-slate-400">Total Exams</p>
                            <p className="text-3xl font-bold text-white mt-1">{exams.length}</p>
                        </div>
                        <div className="w-12 h-12 bg-primary-600/20 rounded-xl flex items-center justify-center text-2xl">
                            📝
                        </div>
                    </div>
                </Card>
                <Card>
                    <div className="flex items-center justify-between">
                        <div>
                            <p className="text-sm text-slate-400">Upcoming</p>
                            <p className="text-3xl font-bold text-warning-400 mt-1">
                                {upcomingExams.length}
                            </p>
                        </div>
                        <div className="w-12 h-12 bg-warning-600/20 rounded-xl flex items-center justify-center text-2xl">
                            ⏰
                        </div>
                    </div>
                </Card>
                <Card>
                    <div className="flex items-center justify-between">
                        <div>
                            <p className="text-sm text-slate-400">Completed</p>
                            <p className="text-3xl font-bold text-success-400 mt-1">
                                {completedExams.length}
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
                            <p className="text-sm text-slate-400">This Month</p>
                            <p className="text-3xl font-bold text-accent-400 mt-1">
                                {exams.filter(e => {
                                    const examDate = new Date(e.examDate);
                                    const now = new Date();
                                    return examDate.getMonth() === now.getMonth() && examDate.getFullYear() === now.getFullYear();
                                }).length}
                            </p>
                        </div>
                        <div className="w-12 h-12 bg-accent-600/20 rounded-xl flex items-center justify-center text-2xl">
                            📅
                        </div>
                    </div>
                </Card>
            </div>

            {/* Exams table */}
            <Card>
                <div className="overflow-x-auto">
                    <table className="w-full">
                        <thead>
                            <tr className="border-b border-slate-800">
                                <th className="text-left py-4 px-4 text-sm font-semibold text-slate-300">Title</th>
                                <th className="text-left py-4 px-4 text-sm font-semibold text-slate-300">Subject</th>
                                <th className="text-left py-4 px-4 text-sm font-semibold text-slate-300">Date</th>
                                <th className="text-left py-4 px-4 text-sm font-semibold text-slate-300">Duration</th>
                                <th className="text-left py-4 px-4 text-sm font-semibold text-slate-300">Status</th>
                                <th className="text-right py-4 px-4 text-sm font-semibold text-slate-300">Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            {filteredExams.length === 0 ? (
                                <tr>
                                    <td colSpan="6" className="text-center py-12 text-slate-400">
                                        No exams found
                                    </td>
                                </tr>
                            ) : (
                                filteredExams.map((exam) => {
                                    const examDate = new Date(exam.examDate);
                                    const isUpcoming = examDate > new Date();

                                    return (
                                        <tr key={exam.id} className="border-b border-slate-800/50 hover:bg-slate-800/30 transition-colors">
                                            <td className="py-4 px-4">
                                                <p className="font-medium text-white">{exam.title}</p>
                                                {exam.location && (
                                                    <p className="text-sm text-slate-400 mt-1">📍 {exam.location}</p>
                                                )}
                                            </td>
                                            <td className="py-4 px-4 text-slate-300">{getSubjectName(exam.subjectId)}</td>
                                            <td className="py-4 px-4">
                                                <p className="text-white">{examDate.toLocaleDateString()}</p>
                                                <p className="text-sm text-slate-400">{examDate.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}</p>
                                            </td>
                                            <td className="py-4 px-4 text-slate-300">{exam.durationMinutes || 'N/A'} min</td>
                                            <td className="py-4 px-4">
                                                <Badge variant={isUpcoming ? 'warning' : 'success'}>
                                                    {isUpcoming ? 'Upcoming' : 'Completed'}
                                                </Badge>
                                            </td>
                                            <td className="py-4 px-4">
                                                <div className="flex items-center justify-end gap-2">
                                                    <Button
                                                        variant="ghost"
                                                        size="sm"
                                                        onClick={() => { setSelectedExam(exam); setIsModalOpen(true); }}
                                                    >
                                                        Edit
                                                    </Button>
                                                    <Button
                                                        variant="ghost"
                                                        size="sm"
                                                        onClick={() => handleDelete(exam.id)}
                                                        className="text-danger-400 hover:text-danger-300"
                                                    >
                                                        Delete
                                                    </Button>
                                                </div>
                                            </td>
                                        </tr>
                                    );
                                })
                            )}
                        </tbody>
                    </table>
                </div>
            </Card>

            {/* Exam Modal */}
            <ExamModal
                isOpen={isModalOpen}
                onClose={() => setIsModalOpen(false)}
                exam={selectedExam}
                subjects={subjects}
                onSuccess={fetchData}
            />
        </div>
    );
};

const ExamModal = ({ isOpen, onClose, exam, subjects, onSuccess }) => {
    const [formData, setFormData] = useState({
        title: '',
        subjectId: '',
        examDate: '',
        durationMinutes: '',
        location: '',
        maxMarks: '',
    });
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');

    useEffect(() => {
        if (exam) {
            setFormData({
                title: exam.title || '',
                subjectId: exam.subjectId || '',
                examDate: exam.examDate ? new Date(exam.examDate).toISOString().slice(0, 16) : '',
                durationMinutes: exam.durationMinutes || '',
                location: exam.location || '',
                maxMarks: exam.maxMarks || '',
            });
        } else {
            setFormData({
                title: '',
                subjectId: '',
                examDate: '',
                durationMinutes: '',
                location: '',
                maxMarks: '',
            });
        }
    }, [exam]);

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);
        setError('');

        try {
            const payload = {
                ...formData,
                durationMinutes: parseInt(formData.durationMinutes) || 0,
                maxMarks: parseFloat(formData.maxMarks) || 100,
            };

            if (exam) {
                await apiClient.put(API_ENDPOINTS.EXAMS.BY_ID(exam.id), {
                    id: exam.id,
                    ...payload,
                });
            } else {
                await apiClient.post(API_ENDPOINTS.EXAMS.BASE, payload);
            }
            onSuccess();
            onClose();
        } catch (err) {
            setError(err.message || 'Failed to save exam');
        } finally {
            setLoading(false);
        }
    };

    return (
        <Modal
            isOpen={isOpen}
            onClose={onClose}
            title={exam ? 'Edit Exam' : 'Schedule New Exam'}
        >
            {error && <Alert type="error" message={error} onClose={() => setError('')} className="mb-4" />}

            <form onSubmit={handleSubmit} className="space-y-4">
                <Input
                    label="Exam Title"
                    value={formData.title}
                    onChange={(e) => setFormData({ ...formData, title: e.target.value })}
                    placeholder="e.g., Midterm Exam"
                    required
                />

                <Select
                    label="Subject"
                    value={formData.subjectId}
                    onChange={(e) => setFormData({ ...formData, subjectId: e.target.value })}
                    options={[
                        { value: '', label: 'Select Subject' },
                        ...subjects.map(s => ({ value: s.id, label: `${s.code} - ${s.name}` }))
                    ]}
                    required
                />

                <div className="grid grid-cols-2 gap-4">
                    <Input
                        label="Exam Date & Time"
                        type="datetime-local"
                        value={formData.examDate}
                        onChange={(e) => setFormData({ ...formData, examDate: e.target.value })}
                        required
                    />
                    <Input
                        label="Duration (minutes)"
                        type="number"
                        value={formData.durationMinutes}
                        onChange={(e) => setFormData({ ...formData, durationMinutes: e.target.value })}
                        min="15"
                        max="300"
                        required
                    />
                </div>

                <div className="grid grid-cols-2 gap-4">
                    <Input
                        label="Location"
                        value={formData.location}
                        onChange={(e) => setFormData({ ...formData, location: e.target.value })}
                        placeholder="e.g., Room 101"
                    />
                    <Input
                        label="Max Marks"
                        type="number"
                        value={formData.maxMarks}
                        onChange={(e) => setFormData({ ...formData, maxMarks: e.target.value })}
                        min="1"
                        max="1000"
                    />
                </div>

                <div className="flex gap-3 pt-4">
                    <Button type="submit" disabled={loading} className="flex-1">
                        {loading ? 'Saving...' : exam ? 'Update Exam' : 'Schedule Exam'}
                    </Button>
                    <Button type="button" variant="secondary" onClick={onClose} className="flex-1">
                        Cancel
                    </Button>
                </div>
            </form>
        </Modal>
    );
};

export default ExamsPage;
