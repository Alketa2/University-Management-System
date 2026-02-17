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
    const [successMessage, setSuccessMessage] = useState('');
    const [isBulkModalOpen, setIsBulkModalOpen] = useState(false);
    const [isTranscriptOpen, setIsTranscriptOpen] = useState(false);
    const [transcriptData, setTranscriptData] = useState(null);

    const currentUser = authService.getUser();
    const userRole = authService.getUserRole();
    const isStudent = userRole === 'Student';
    const isAdmin = currentUser?.role === 'Admin';
    const isTeacher = currentUser?.role === 'Teacher';

    useEffect(() => {
        fetchData();
    }, []);

    const fetchData = async () => {
        setLoading(true);
        try {
            const fetchPromises = [
                apiClient.get(API_ENDPOINTS.SUBJECTS.BASE),
                apiClient.get(API_ENDPOINTS.EXAMS.BASE),
            ];

            if (!isStudent) {
                fetchPromises.push(apiClient.get(API_ENDPOINTS.STUDENTS.BASE));
            }

            const results = await Promise.all(fetchPromises);
            const subjectsData = results[0];
            const examsData = results[1];
            const studentsData = isStudent ? [] : results[2];

            setSubjects(subjectsData);
            setExams(examsData);

            // Load grades if a subject is selected
            if (isStudent) {
                const studentId = currentUser?.studentId;
                if (studentId) {
                    const gradesData = await apiClient.get(API_ENDPOINTS.GRADES.BY_STUDENT(studentId));
                    setGrades(gradesData);
                } else {
                    setGrades([]);
                }
                setStudents([]);
            } else {
                setStudents(studentsData);

                if (filterSubject) {
                    const gradesData = await apiClient.get(API_ENDPOINTS.GRADES.BY_SUBJECT(filterSubject));
                    setGrades(gradesData);
                } else {
                    setGrades([]);
                }
            }
        } catch (err) {
            setError(err.message || 'Failed to fetch data');
        } finally {
            setLoading(false);
        }
    };

    const handleSubjectChange = async (subjectId) => {
        setFilterSubject(subjectId);
        if (isStudent) {
            return;
        }

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

    const fetchTranscript = async () => {
        setLoading(true);
        try {
            const data = await apiClient.get(API_ENDPOINTS.GRADES.GPA(currentUser.studentId));
            setTranscriptData(data);
            setIsTranscriptOpen(true);
        } catch (err) {
            console.error('Transcript fetch error:', err);
            setError('Failed to fetch transcript data');
        } finally {
            setLoading(false);
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
                    <h2 className="text-3xl font-bold text-white">{isStudent ? 'My Grades' : 'Grade Management'}</h2>
                    <p className="text-slate-400 mt-1">{isStudent ? 'View your academic grades' : 'Enter and manage student grades'}</p>

                </div>
                {(isAdmin || isTeacher) && (
                    <div className="flex gap-3">
                        <Button variant="secondary" onClick={() => setIsBulkModalOpen(true)}>
                            Bulk Entry
                        </Button>
                        <Button onClick={() => { setSelectedGrade(null); setIsModalOpen(true); }}>
                            + Add Grade
                        </Button>
                    </div>
                )}
                {isStudent && (
                    <Button variant="secondary" onClick={() => fetchTranscript()}>
                        📄 View Transcript
                    </Button>
                )}
            </div>

            {error && <Alert type="error" message={error} onClose={() => setError('')} />}
            {successMessage && <Alert type="success" message={successMessage} onClose={() => setSuccessMessage('')} />}

            {/* Filters */}
            <Card>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <Select
                        label="Filter by Subject"
                        value={filterSubject}
                        onChange={(e) => handleSubjectChange(e.target.value)}
                        options={[
                            { value: '', label: isTeacher ? 'Select Subject' : 'All Subjects' },
                            ...subjects
                                .filter(s => !isTeacher || s.teacherId === currentUser?.teacherId)
                                .map(s => ({ value: s.id, label: `${s.code} - ${s.name}` }))
                        ]}
                    />
                    {!isStudent && (
                        <Select
                            label="Filter by Student"
                            value={filterStudent}
                            onChange={(e) => setFilterStudent(e.target.value)}
                            options={[
                                { value: '', label: 'All Students' },
                                ...students
                                    .filter(s => {
                                        if (!filterSubject) return true;
                                        const subject = subjects.find(sub => sub.id === filterSubject);
                                        return s.primaryProgramId === subject?.programId;
                                    })
                                    .map(s => ({ value: s.id, label: `${s.firstName} ${s.lastName}` }))
                            ]}
                        />
                    )}
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
                        {isStudent ? 'No grades found' : (filterSubject ? 'No grades found for this subject' : 'Select a subject to view grades')}
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
                    onSuccess={(msg) => {
                        setSuccessMessage(msg || 'Grade saved successfully!');
                        fetchData();
                        // Clear message after 5 seconds
                        setTimeout(() => setSuccessMessage(''), 5000);
                    }}
                />
            )}

            {/* Bulk Grade Modal */}
            {(isAdmin || isTeacher) && (
                <BulkGradeModal
                    isOpen={isBulkModalOpen}
                    onClose={() => setIsBulkModalOpen(false)}
                    students={students}
                    subjects={subjects}
                    exams={exams}
                    onSuccess={(msg) => {
                        setSuccessMessage(msg || 'Bulk grades saved successfully!');
                        fetchData();
                        setTimeout(() => setSuccessMessage(''), 5000);
                    }}
                />
            )}

            {/* Transcript Modal */}
            {isTranscriptOpen && (
                <Modal
                    isOpen={isTranscriptOpen}
                    onClose={() => setIsTranscriptOpen(false)}
                    title="Official Academic Transcript"
                    className="max-w-4xl"
                >
                    <div className="space-y-6 p-2 text-slate-200" id="transcript-content">
                        <div className="flex border-b border-slate-700 pb-4 justify-between items-start">
                            <div>
                                <h1 className="text-2xl font-bold text-white mb-1">University Transcript</h1>
                                <p className="text-primary-400 font-semibold">{transcriptData?.studentName || 'Student Record'}</p>
                                <p className="text-sm text-slate-400">Student ID: {currentUser?.studentId?.substring(0, 8)}...</p>
                            </div>
                            <div className="text-right">
                                <p className="text-sm text-slate-400">Date Issued</p>
                                <p className="text-white">{new Date().toLocaleDateString()}</p>
                            </div>
                        </div>

                        <div className="grid grid-cols-3 gap-4">
                            <div className="bg-slate-800/50 p-4 rounded-xl border border-slate-700">
                                <p className="text-xs text-slate-400 uppercase tracking-wider mb-1">Cumulative GPA</p>
                                <p className="text-2xl font-bold text-primary-400">{transcriptData?.cumulativeGPA}</p>
                            </div>
                            <div className="bg-slate-800/50 p-4 rounded-xl border border-slate-700">
                                <p className="text-xs text-slate-400 uppercase tracking-wider mb-1">Total Credits</p>
                                <p className="text-2xl font-bold text-white">{transcriptData?.totalCredits}</p>
                            </div>
                            <div className="bg-slate-800/50 p-4 rounded-xl border border-slate-700">
                                <p className="text-xs text-slate-400 uppercase tracking-wider mb-1">Courses Passed</p>
                                <p className="text-2xl font-bold text-success-400">{transcriptData?.grades?.filter(g => g.letterGrade !== 'F').length}</p>
                            </div>
                        </div>

                        <table className="w-full text-sm">
                            <thead>
                                <tr className="border-b border-slate-700 text-slate-400">
                                    <th className="text-left py-2 font-medium">Subject</th>
                                    <th className="text-center py-2 font-medium">Exam</th>
                                    <th className="text-center py-2 font-medium">Percentage</th>
                                    <th className="text-center py-2 font-medium">Grade</th>
                                    <th className="text-right py-2 font-medium">GP</th>
                                </tr>
                            </thead>
                            <tbody className="divide-y divide-slate-800">
                                {transcriptData?.grades?.map((g, idx) => (
                                    <tr key={idx}>
                                        <td className="py-3">
                                            <p className="font-medium text-white">{g.subjectName}</p>
                                            <p className="text-xs text-slate-500">{g.subjectCode}</p>
                                        </td>
                                        <td className="text-center py-3">{g.examName || 'Final'}</td>
                                        <td className="text-center py-3">{g.percentage}%</td>
                                        <td className="text-center py-3">
                                            <Badge variant={getLetterGradeBadge(g.letterGrade)}>{g.letterGrade}</Badge>
                                        </td>
                                        <td className="text-right py-3 font-medium text-white">{g.gradePoint}</td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>

                        <div className="pt-6 border-t border-slate-700 mt-auto flex justify-between items-end">
                            <div className="text-[10px] text-slate-500 max-w-sm">
                                This is an automated academic record generated by the University Management System. This document is for informational purposes only.
                            </div>
                            <Button size="sm" onClick={() => window.print()} variant="ghost" className="hidden md:flex">
                                🖨️ Print Transcript
                            </Button>
                        </div>
                    </div>
                </Modal>
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
            const score = parseFloat(formData.score);
            const maxScore = parseFloat(formData.maxScore);

            if (isNaN(score) || isNaN(maxScore)) {
                throw new Error('Score and Max Score must be valid numbers');
            }

            if (score > maxScore) {
                throw new Error(`Score (${score}) cannot be greater than Max Score (${maxScore})`);
            }

            if (score < 0 || maxScore <= 0) {
                throw new Error('Score must be positive and Max Score must be greater than 0');
            }

            const payload = {
                studentId: formData.studentId,
                subjectId: formData.subjectId,
                examId: formData.examId || null,
                score: score,
                maxScore: maxScore,
                comments: formData.comments || null,
                gradedByTeacherId: currentUser?.teacherId || null,
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
            onSuccess(grade ? 'Grade updated successfully!' : 'Grade added successfully!');
            onClose();
        } catch (err) {
            setError(err.message || 'Failed to save grade');
        } finally {
            setLoading(false);
        }
    };

    // Intelligent filtering for programs
    const selectedStudent = students.find(s => s.id === formData.studentId);
    const selectedSubject = subjects.find(s => s.id === formData.subjectId);

    // Filter subjects based on student's program
    const filteredSubjects = subjects.filter(sub => {
        // Filter by teacher if applicable
        if (currentUser?.role === 'Teacher' && sub.teacherId !== currentUser?.teacherId) return false;

        // Filter by student's program
        if (!formData.studentId || !selectedStudent?.primaryProgramId) return true;
        return sub.programId === selectedStudent.primaryProgramId;
    });

    // Filter students based on subject's program
    const filteredStudentsList = students.filter(st => {
        if (!formData.subjectId || !selectedSubject?.programId) return true;
        return st.primaryProgramId === selectedSubject.programId;
    });

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
                        ...filteredStudentsList.map(s => ({
                            value: s.id,
                            label: `${s.firstName} ${s.lastName} (${s.primaryProgramName || 'No Program'})`
                        }))
                    ]}
                    required
                />

                <Select
                    label="Subject"
                    value={formData.subjectId}
                    onChange={(e) => setFormData({ ...formData, subjectId: e.target.value, examId: '' })}
                    options={[
                        { value: '', label: 'Select Subject' },
                        ...filteredSubjects.map(s => ({
                            value: s.id,
                            label: `${s.code} - ${s.name} (${s.programName})`
                        }))
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

const BulkGradeModal = ({ isOpen, onClose, students, subjects, exams, onSuccess }) => {
    const [selectedSubject, setSelectedSubject] = useState('');
    const [selectedExam, setSelectedExam] = useState('');
    const [academicYear, setAcademicYear] = useState(`${new Date().getFullYear()}-${new Date().getFullYear() + 1}`);
    const [semester, setSemester] = useState('1');
    const [bulkEntries, setBulkEntries] = useState([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');

    const currentUser = authService.getUser();

    // Prepare entries when subject changes
    useEffect(() => {
        if (selectedSubject) {
            const subject = subjects.find(s => s.id === selectedSubject);
            const filteredStudents = students.filter(s => s.primaryProgramId === subject?.programId);

            setBulkEntries(filteredStudents.map(student => ({
                studentId: student.id,
                studentName: `${student.firstName} ${student.lastName}`,
                score: '',
                maxScore: '100',
                comments: ''
            })));
        } else {
            setBulkEntries([]);
        }
    }, [selectedSubject, subjects, students]);

    const handleScoreChange = (studentId, field, value) => {
        setBulkEntries(prev => prev.map(entry =>
            entry.studentId === studentId ? { ...entry, [field]: value } : entry
        ));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        const entriesToSubmit = bulkEntries.filter(e => e.score !== '');

        if (entriesToSubmit.length === 0) {
            setError('Please enter at least one score.');
            return;
        }

        setLoading(true);
        setError('');

        try {
            // Submit each grade one by one (or implement bulk API if available)
            // For now, we'll do sequential posts to reuse existing validation
            const promises = entriesToSubmit.map(entry => {
                const payload = {
                    studentId: entry.studentId,
                    subjectId: selectedSubject,
                    examId: selectedExam || null,
                    score: parseFloat(entry.score),
                    maxScore: parseFloat(entry.maxScore),
                    comments: entry.comments || null,
                    gradedByTeacherId: currentUser?.teacherId || null,
                    academicYear,
                    semester: parseInt(semester),
                };
                return apiClient.post(API_ENDPOINTS.GRADES.BASE, payload);
            });

            await Promise.all(promises);
            onSuccess(`Successfully added ${entriesToSubmit.length} grades!`);
            onClose();
        } catch (err) {
            setError(err.message || 'Failed to save bulk grades');
        } finally {
            setLoading(false);
        }
    };

    return (
        <Modal
            isOpen={isOpen}
            onClose={onClose}
            title="Bulk Grade Entry"
            className="max-w-4xl"
        >
            {error && <Alert type="error" message={error} onClose={() => setError('')} className="mb-4" />}

            <form onSubmit={handleSubmit} className="space-y-6">
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4 bg-slate-800/50 p-4 rounded-xl border border-slate-700">
                    <Select
                        label="Subject"
                        value={selectedSubject}
                        onChange={(e) => setSelectedSubject(e.target.value)}
                        options={[
                            { value: '', label: 'Select Subject' },
                            ...subjects
                                .filter(s => {
                                    if (currentUser.role === 'Teacher') {
                                        const teacherId = currentUser?.teacherId;
                                        // Option A: Only their subjects
                                        // Option B: Subjects in their program (heuristic)
                                        // Let's go with their subjects for bulk entry to be safe
                                        return s.teacherId === teacherId;
                                    }
                                    return true;
                                })
                                .map(s => ({ value: s.id, label: `${s.code} - ${s.name}` }))
                        ]}
                        required
                    />
                    <Select
                        label="Exam (Optional)"
                        value={selectedExam}
                        onChange={(e) => setSelectedExam(e.target.value)}
                        options={[
                            { value: '', label: 'Overall Grade' },
                            ...exams.filter(e => e.subjectId === selectedSubject).map(e => ({ value: e.id, label: e.name }))
                        ]}
                    />
                    <Input
                        label="Academic Year"
                        value={academicYear}
                        onChange={(e) => setAcademicYear(e.target.value)}
                        required
                    />
                    <Input
                        label="Semester"
                        type="number"
                        value={semester}
                        onChange={(e) => setSemester(e.target.value)}
                        min="1"
                        required
                    />
                </div>

                {selectedSubject && (
                    <div className="mt-4">
                        <div className="overflow-x-auto border border-slate-700 rounded-xl">
                            <table className="w-full text-sm">
                                <thead className="bg-slate-800 text-slate-300">
                                    <tr>
                                        <th className="text-left p-3">Student</th>
                                        <th className="text-center p-3 w-32">Score</th>
                                        <th className="text-center p-3 w-32">Max Score</th>
                                        <th className="text-left p-3">Comments</th>
                                    </tr>
                                </thead>
                                <tbody className="divide-y divide-slate-800">
                                    {bulkEntries.length === 0 ? (
                                        <tr>
                                            <td colSpan="4" className="p-8 text-center text-slate-500 italic">
                                                No students enrolled in this program
                                            </td>
                                        </tr>
                                    ) : (
                                        bulkEntries.map((entry) => (
                                            <tr key={entry.studentId} className="hover:bg-slate-800/30">
                                                <td className="p-3 text-white font-medium">{entry.studentName}</td>
                                                <td className="p-3">
                                                    <input
                                                        type="number"
                                                        className="w-full bg-slate-900 border border-slate-700 rounded-lg px-2 py-1.5 text-white text-center focus:ring-1 focus:ring-primary-500 outline-none"
                                                        value={entry.score}
                                                        onChange={(e) => handleScoreChange(entry.studentId, 'score', e.target.value)}
                                                        placeholder="Score"
                                                    />
                                                </td>
                                                <td className="p-3">
                                                    <input
                                                        type="number"
                                                        className="w-full bg-slate-900 border border-slate-700 rounded-lg px-2 py-1.5 text-white text-center focus:ring-1 focus:ring-primary-500 outline-none"
                                                        value={entry.maxScore}
                                                        onChange={(e) => handleScoreChange(entry.studentId, 'maxScore', e.target.value)}
                                                        placeholder="Max"
                                                    />
                                                </td>
                                                <td className="p-3">
                                                    <input
                                                        type="text"
                                                        className="w-full bg-slate-900 border border-slate-700 rounded-lg px-3 py-1.5 text-white focus:ring-1 focus:ring-primary-500 outline-none"
                                                        value={entry.comments}
                                                        onChange={(e) => handleScoreChange(entry.studentId, 'comments', e.target.value)}
                                                        placeholder="Observation..."
                                                    />
                                                </td>
                                            </tr>
                                        ))
                                    )}
                                </tbody>
                            </table>
                        </div>
                    </div>
                )}

                <div className="flex gap-3 pt-4 border-t border-slate-800">
                    <Button type="submit" disabled={loading || !selectedSubject || bulkEntries.length === 0} className="flex-1">
                        {loading ? 'Submitting...' : `Submit ${bulkEntries.filter(e => e.score !== '').length} Grades`}
                    </Button>
                    <Button type="button" variant="secondary" onClick={onClose} className="w-1/4">
                        Cancel
                    </Button>
                </div>
            </form>
        </Modal>
    );
};

export default GradesPage;
