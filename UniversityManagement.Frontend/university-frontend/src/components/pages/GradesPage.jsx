import { useState, useEffect } from 'react';
import { Button, Card, Input, Modal, Badge, Alert, Spinner, Select } from '../ui/UIComponents';
import apiClient from '../../utils/apiClient';
import { API_ENDPOINTS } from '../../config/api';
import authService from '../../utils/authService';

const GradesPage = () => {
    const [grades, setGrades] = useState([]);
    const [students, setStudents] = useState([]);
    const [subjects, setSubjects] = useState([]);
    const [exams, setExams] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [selectedGrade, setSelectedGrade] = useState(null);
    const [filterSubject, setFilterSubject] = useState('');
    const [filterStudent, setFilterStudent] = useState('');

    const currentUser = authService.getUser();
    const isAdmin = currentUser?.role === 'Admin';
    const isTeacher = currentUser?.role === 'Teacher';

    useEffect(() => {
        fetchData();
    }, []);

    const fetchData = async () => {
        setLoading(true);
        try {
            const [studentsData, subjectsData, examsData] = await Promise.all([
                apiClient.get(API_ENDPOINTS.STUDENTS.BASE),
                apiClient.get(API_ENDPOINTS.SUBJECTS.BASE),
                apiClient.get(API_ENDPOINTS.EXAMS.BASE),
            ]);
            setStudents(studentsData);
            setSubjects(subjectsData);
            setExams(examsData);

            // Load grades if a subject is selected
            if (filterSubject) {
                const gradesData = await apiClient.get(API_ENDPOINTS.GRADES.BY_SUBJECT(filterSubject));
                setGrades(gradesData);
            }
        } catch (err) {
            setError(err.message || 'Failed to fetch data');
        } finally {
            setLoading(false);
        }
    };

    const handleSubjectChange = async (subjectId) => {
        setFilterSubject(subjectId);
        if (subjectId) {
            try {
                const gradesData = await apiClient.get(API_ENDPOINTS.GRADES.BY_SUBJECT(subjectId));
                setGrades(gradesData);
            } catch (err) {
                setError(err.message || 'Failed to fetch grades');
            }
        } else {
            setGrades([]);
        }
    };

    const handleDelete = async (id) => {
        if (!window.confirm('Are you sure you want to delete this grade?')) return;

        try {
            await apiClient.delete(API_ENDPOINTS.GRADES.BY_ID(id));
            setGrades(grades.filter(g => g.id !== id));
        } catch (err) {
            setError(err.message || 'Failed to delete grade');
        }
    };

    const getLetterGradeBadge = (letterGrade) => {
        const variants = {
            'A': 'success',
            'B': 'primary',
            'C': 'warning',
            'D': 'default',
            'F': 'danger'
        };
        return variants[letterGrade] || 'default';
    };

    const filteredGrades = grades.filter(grade =>
        (!filterStudent || grade.studentId === filterStudent) &&
        (!filterSubject || grade.subjectId === filterSubject)
    );

    // Grade distribution stats
    const gradeDistribution = {
        A: grades.filter(g => g.letterGrade === 'A').length,
        B: grades.filter(g => g.letterGrade === 'B').length,
        C: grades.filter(g => g.letterGrade === 'C').length,
        D: grades.filter(g => g.letterGrade === 'D').length,
        F: grades.filter(g => g.letterGrade === 'F').length,
    };

    const averageGPA = grades.length > 0
        ? (grades.reduce((sum, g) => sum + g.gradePoint, 0) / grades.length).toFixed(2)
        : '0.00';

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
                    <h2 className="text-3xl font-bold text-white">Grade Management</h2>
                    <p className="text-slate-400 mt-1">Enter and manage student grades</p>
                </div>
                {(isAdmin || isTeacher) && (
                    <Button onClick={() => { setSelectedGrade(null); setIsModalOpen(true); }}>
                        + Add Grade
                    </Button>
                )}
            </div>

            {error && <Alert type="error" message={error} onClose={() => setError('')} />}

            {/* Filters */}
            <Card>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <Select
                        label="Filter by Subject"
                        value={filterSubject}
                        onChange={(e) => handleSubjectChange(e.target.value)}
                        options={[
                            { value: '', label: 'All Subjects' },
                            ...subjects.map(s => ({ value: s.id, label: `${s.code} - ${s.name}` }))
                        ]}
                    />
                    <Select
                        label="Filter by Student"
                        value={filterStudent}
                        onChange={(e) => setFilterStudent(e.target.value)}
                        options={[
                            { value: '', label: 'All Students' },
                            ...students.map(s => ({ value: s.id, label: `${s.firstName} ${s.lastName}` }))
                        ]}
                    />
                </div>
            </Card>

            {/* Stats */}
            {filterSubject && grades.length > 0 && (
                <div className="grid grid-cols-2 md:grid-cols-6 gap-4">
                    <Card>
                        <div className="text-center">
                            <p className="text-sm text-slate-400">Total</p>
                            <p className="text-2xl font-bold text-white mt-1">{grades.length}</p>
                        </div>
                    </Card>
                    <Card>
                        <div className="text-center">
                            <p className="text-sm text-slate-400">Avg GPA</p>
                            <p className="text-2xl font-bold text-primary-400 mt-1">{averageGPA}</p>
                        </div>
                    </Card>
                    <Card>
                        <div className="text-center">
                            <p className="text-sm text-slate-400">A's</p>
                            <p className="text-2xl font-bold text-success-400 mt-1">{gradeDistribution.A}</p>
                        </div>
                    </Card>
                    <Card>
                        <div className="text-center">
                            <p className="text-sm text-slate-400">B's</p>
                            <p className="text-2xl font-bold text-primary-400 mt-1">{gradeDistribution.B}</p>
                        </div>
                    </Card>
                    <Card>
                        <div className="text-center">
                            <p className="text-sm text-slate-400">C's</p>
                            <p className="text-2xl font-bold text-warning-400 mt-1">{gradeDistribution.C}</p>
                        </div>
                    </Card>
                    <Card>
                        <div className="text-center">
                            <p className="text-sm text-slate-400">F's</p>
                            <p className="text-2xl font-bold text-danger-400 mt-1">{gradeDistribution.F}</p>
                        </div>
                    </Card>
                </div>
            )}

            {/* Grades Table */}
            <Card>
                {filteredGrades.length === 0 ? (
                    <div className="text-center py-12 text-slate-400">
                        {filterSubject ? 'No grades found for this subject' : 'Select a subject to view grades'}
                    </div>
                ) : (
                    <div className="overflow-x-auto">
                        <table className="w-full">
                            <thead>
                                <tr className="border-b border-slate-700">
                                    <th className="text-left py-3 px-4 text-slate-300 font-semibold">Student</th>
                                    <th className="text-left py-3 px-4 text-slate-300 font-semibold">Subject</th>
                                    <th className="text-left py-3 px-4 text-slate-300 font-semibold">Exam</th>
                                    <th className="text-center py-3 px-4 text-slate-300 font-semibold">Score</th>
                                    <th className="text-center py-3 px-4 text-slate-300 font-semibold">Percentage</th>
                                    <th className="text-center py-3 px-4 text-slate-300 font-semibold">Grade</th>
                                    <th className="text-center py-3 px-4 text-slate-300 font-semibold">GPA</th>
                                    {(isAdmin || isTeacher) && (
                                        <th className="text-right py-3 px-4 text-slate-300 font-semibold">Actions</th>
                                    )}
                                </tr>
                            </thead>
                            <tbody>
                                {filteredGrades.map((grade) => (
                                    <tr key={grade.id} className="border-b border-slate-800 hover:bg-slate-800/50 transition-colors">
                                        <td className="py-3 px-4 text-white">{grade.studentName}</td>
                                        <td className="py-3 px-4 text-slate-300">{grade.subjectCode}</td>
                                        <td className="py-3 px-4 text-slate-300">{grade.examName || 'Overall'}</td>
                                        <td className="py-3 px-4 text-center text-white">
                                            {grade.score}/{grade.maxScore}
                                        </td>
                                        <td className="py-3 px-4 text-center text-white">{grade.percentage}%</td>
                                        <td className="py-3 px-4 text-center">
                                            <Badge variant={getLetterGradeBadge(grade.letterGrade)}>
                                                {grade.letterGrade}
                                            </Badge>
                                        </td>
                                        <td className="py-3 px-4 text-center text-white">{grade.gradePoint.toFixed(1)}</td>
                                        {(isAdmin || isTeacher) && (
                                            <td className="py-3 px-4 text-right">
                                                <div className="flex items-center justify-end gap-2">
                                                    <Button
                                                        variant="ghost"
                                                        size="sm"
                                                        onClick={() => {
                                                            setSelectedGrade(grade);
                                                            setIsModalOpen(true);
                                                        }}
                                                    >
                                                        Edit
                                                    </Button>
                                                    <Button
                                                        variant="ghost"
                                                        size="sm"
                                                        onClick={() => handleDelete(grade.id)}
                                                        className="text-danger-400 hover:text-danger-300"
                                                    >
                                                        Delete
                                                    </Button>
                                                </div>
                                            </td>
                                        )}
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                )}
            </Card>

            {/* Grade Modal */}
            {(isAdmin || isTeacher) && (
                <GradeModal
                    isOpen={isModalOpen}
                    onClose={() => setIsModalOpen(false)}
                    grade={selectedGrade}
                    students={students}
                    subjects={subjects}
                    exams={exams}
                    onSuccess={fetchData}
                />
            )}
        </div>
    );
};

const GradeModal = ({ isOpen, onClose, grade, students, subjects, exams, onSuccess }) => {
    const [formData, setFormData] = useState({
        studentId: '',
        subjectId: '',
        examId: '',
        score: '',
        maxScore: '100',
        comments: '',
        academicYear: '',
        semester: '1',
    });
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');

    const currentUser = authService.getUser();

    useEffect(() => {
        if (grade) {
            setFormData({
                studentId: grade.studentId || '',
                subjectId: grade.subjectId || '',
                examId: grade.examId || '',
                score: grade.score?.toString() || '',
                maxScore: grade.maxScore?.toString() || '100',
                comments: grade.comments || '',
                academicYear: grade.academicYear || '',
                semester: grade.semester?.toString() || '1',
            });
        } else {
            const currentYear = new Date().getFullYear();
            setFormData({
                studentId: '',
                subjectId: '',
                examId: '',
                score: '',
                maxScore: '100',
                comments: '',
                academicYear: `${currentYear}-${currentYear + 1}`,
                semester: '1',
            });
        }
    }, [grade]);

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);
        setError('');

        try {
            const payload = {
                studentId: formData.studentId,
                subjectId: formData.subjectId,
                examId: formData.examId || null,
                score: parseFloat(formData.score),
                maxScore: parseFloat(formData.maxScore),
                comments: formData.comments || null,
                gradedByTeacherId: currentUser?.id || '00000000-0000-0000-0000-000000000000',
                academicYear: formData.academicYear,
                semester: parseInt(formData.semester),
            };

            if (grade) {
                await apiClient.put(API_ENDPOINTS.GRADES.BY_ID(grade.id), {
                    id: grade.id,
                    ...payload,
                });
            } else {
                await apiClient.post(API_ENDPOINTS.GRADES.BASE, payload);
            }
            onSuccess();
            onClose();
        } catch (err) {
            setError(err.message || 'Failed to save grade');
        } finally {
            setLoading(false);
        }
    };

    // Filter exams by selected subject
    const filteredExams = exams.filter(e => e.subjectId === formData.subjectId);

    return (
        <Modal
            isOpen={isOpen}
            onClose={onClose}
            title={grade ? 'Edit Grade' : 'Add New Grade'}
        >
            {error && <Alert type="error" message={error} onClose={() => setError('')} className="mb-4" />}

            <form onSubmit={handleSubmit} className="space-y-4">
                <Select
                    label="Student"
                    value={formData.studentId}
                    onChange={(e) => setFormData({ ...formData, studentId: e.target.value })}
                    options={[
                        { value: '', label: 'Select Student' },
                        ...students.map(s => ({ value: s.id, label: `${s.firstName} ${s.lastName}` }))
                    ]}
                    required
                />

                <Select
                    label="Subject"
                    value={formData.subjectId}
                    onChange={(e) => setFormData({ ...formData, subjectId: e.target.value, examId: '' })}
                    options={[
                        { value: '', label: 'Select Subject' },
                        ...subjects.map(s => ({ value: s.id, label: `${s.code} - ${s.name}` }))
                    ]}
                    required
                />

                <Select
                    label="Exam (Optional)"
                    value={formData.examId}
                    onChange={(e) => setFormData({ ...formData, examId: e.target.value })}
                    options={[
                        { value: '', label: 'Overall Grade' },
                        ...filteredExams.map(e => ({ value: e.id, label: e.name }))
                    ]}
                />

                <div className="grid grid-cols-2 gap-4">
                    <Input
                        label="Score"
                        type="number"
                        value={formData.score}
                        onChange={(e) => setFormData({ ...formData, score: e.target.value })}
                        min="0"
                        step="0.01"
                        placeholder="e.g., 85"
                        required
                    />
                    <Input
                        label="Max Score"
                        type="number"
                        value={formData.maxScore}
                        onChange={(e) => setFormData({ ...formData, maxScore: e.target.value })}
                        min="1"
                        step="0.01"
                        placeholder="e.g., 100"
                        required
                    />
                </div>

                <div className="grid grid-cols-2 gap-4">
                    <Input
                        label="Academic Year"
                        type="text"
                        value={formData.academicYear}
                        onChange={(e) => setFormData({ ...formData, academicYear: e.target.value })}
                        placeholder="e.g., 2024-2025"
                        required
                    />
                    <Input
                        label="Semester"
                        type="number"
                        value={formData.semester}
                        onChange={(e) => setFormData({ ...formData, semester: e.target.value })}
                        min="1"
                        max="40"
                        required
                    />
                </div>

                <div className="w-full">
                    <label className="block text-sm font-medium text-slate-300 mb-2">
                        Comments (Optional)
                    </label>
                    <textarea
                        className="w-full px-4 py-2.5 bg-slate-900 border border-slate-700 rounded-xl text-white placeholder-slate-500 focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-transparent transition-all min-h-[80px]"
                        value={formData.comments}
                        onChange={(e) => setFormData({ ...formData, comments: e.target.value })}
                        placeholder="Additional feedback for the student..."
                    />
                </div>

                <div className="flex gap-3 pt-4">
                    <Button type="submit" disabled={loading} className="flex-1">
                        {loading ? 'Saving...' : grade ? 'Update Grade' : 'Add Grade'}
                    </Button>
                    <Button type="button" variant="secondary" onClick={onClose} className="flex-1">
                        Cancel
                    </Button>
                </div>
            </form>
        </Modal>
    );
};

export default GradesPage;
