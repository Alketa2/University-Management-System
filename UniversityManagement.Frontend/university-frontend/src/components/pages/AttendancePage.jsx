import { useState, useEffect } from 'react';
import { Button, Card, Input, Modal, Badge, Alert, Spinner, Select } from '../ui/UIComponents';
import apiClient from '../../utils/apiClient';
import { API_ENDPOINTS } from '../../config/api';

const AttendancePage = () => {
    const [attendances, setAttendances] = useState([]);
    const [students, setStudents] = useState([]);
    const [subjects, setSubjects] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [selectedAttendance, setSelectedAttendance] = useState(null);
    const [searchTerm, setSearchTerm] = useState('');

    useEffect(() => {
        fetchData();
    }, []);

    const fetchData = async () => {
        setLoading(true);
        try {
            const [attendancesData, studentsData, subjectsData] = await Promise.all([
                apiClient.get(API_ENDPOINTS.ATTENDANCE.BASE),
                apiClient.get(API_ENDPOINTS.STUDENTS.BASE),
                apiClient.get(API_ENDPOINTS.SUBJECTS.BASE)
            ]);
            setAttendances(attendancesData);
            setStudents(studentsData);
            setSubjects(subjectsData);
        } catch (err) {
            setError(err.message || 'Failed to fetch data');
        } finally {
            setLoading(false);
        }
    };

    const handleDelete = async (id) => {
        if (!window.confirm('Are you sure you want to delete this attendance record?')) return;

        try {
            await apiClient.delete(API_ENDPOINTS.ATTENDANCE.BY_ID(id));
            setAttendances(attendances.filter(a => a.id !== id));
        } catch (err) {
            setError(err.message || 'Failed to delete attendance');
        }
    };

    const getStudentName = (studentId) => {
        const student = students.find(s => s.id === studentId);
        return student ? `${student.firstName} ${student.lastName}` : 'Unknown';
    };

    const getSubjectName = (subjectId) => {
        const subject = subjects.find(s => s.id === subjectId);
        return subject ? subject.name : 'Unknown Subject';
    };

    const filteredAttendances = attendances.filter(attendance =>
        getStudentName(attendance.studentId)?.toLowerCase().includes(searchTerm.toLowerCase()) ||
        getSubjectName(attendance.subjectId)?.toLowerCase().includes(searchTerm.toLowerCase())
    );

    const presentCount = attendances.filter(a => a.isPresent === true).length;
    const absentCount = attendances.filter(a => a.isPresent === false).length;
    const attendanceRate = attendances.length > 0
        ? ((presentCount / attendances.length) * 100).toFixed(1)
        : 0;

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
                    <h2 className="text-3xl font-bold text-white">Attendance Management</h2>
                    <p className="text-slate-400 mt-1">Track student attendance records</p>
                </div>
                <Button onClick={() => { setSelectedAttendance(null); setIsModalOpen(true); }}>
                    + Mark Attendance
                </Button>
            </div>

            {error && <Alert type="error" message={error} onClose={() => setError('')} />}

            {/* Search bar */}
            <div className="flex gap-4">
                <Input
                    placeholder="Search attendance records..."
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
                            <p className="text-sm text-slate-400">Total Records</p>
                            <p className="text-3xl font-bold text-white mt-1">{attendances.length}</p>
                        </div>
                        <div className="w-12 h-12 bg-primary-600/20 rounded-xl flex items-center justify-center text-2xl">
                            📊
                        </div>
                    </div>
                </Card>
                <Card>
                    <div className="flex items-center justify-between">
                        <div>
                            <p className="text-sm text-slate-400">Present</p>
                            <p className="text-3xl font-bold text-success-400 mt-1">
                                {presentCount}
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
                            <p className="text-sm text-slate-400">Absent</p>
                            <p className="text-3xl font-bold text-danger-400 mt-1">
                                {absentCount}
                            </p>
                        </div>
                        <div className="w-12 h-12 bg-danger-600/20 rounded-xl flex items-center justify-center text-2xl">
                            ✗
                        </div>
                    </div>
                </Card>
                <Card>
                    <div className="flex items-center justify-between">
                        <div>
                            <p className="text-sm text-slate-400">Attendance Rate</p>
                            <p className="text-3xl font-bold text-accent-400 mt-1">
                                {attendanceRate}%
                            </p>
                        </div>
                        <div className="w-12 h-12 bg-accent-600/20 rounded-xl flex items-center justify-center text-2xl">
                            📈
                        </div>
                    </div>
                </Card>
            </div>

            {/* Attendance table */}
            <Card>
                <div className="overflow-x-auto">
                    <table className="w-full">
                        <thead>
                            <tr className="border-b border-slate-800">
                                <th className="text-left py-4 px-4 text-sm font-semibold text-slate-300">Student</th>
                                <th className="text-left py-4 px-4 text-sm font-semibold text-slate-300">Subject</th>
                                <th className="text-left py-4 px-4 text-sm font-semibold text-slate-300">Date</th>
                                <th className="text-left py-4 px-4 text-sm font-semibold text-slate-300">Status</th>
                                <th className="text-left py-4 px-4 text-sm font-semibold text-slate-300">Remarks</th>
                                <th className="text-right py-4 px-4 text-sm font-semibold text-slate-300">Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            {filteredAttendances.length === 0 ? (
                                <tr>
                                    <td colSpan="6" className="text-center py-12 text-slate-400">
                                        No attendance records found
                                    </td>
                                </tr>
                            ) : (
                                filteredAttendances.map((attendance) => (
                                    <tr key={attendance.id} className="border-b border-slate-800/50 hover:bg-slate-800/30 transition-colors">
                                        <td className="py-4 px-4">
                                            <p className="font-medium text-white">{getStudentName(attendance.studentId)}</p>
                                        </td>
                                        <td className="py-4 px-4 text-slate-300">{getSubjectName(attendance.subjectId)}</td>
                                        <td className="py-4 px-4 text-slate-300">
                                            {attendance.attendanceDate
                                                ? new Date(attendance.attendanceDate).toLocaleDateString()
                                                : 'N/A'
                                            }
                                        </td>
                                        <td className="py-4 px-4">
                                            <Badge variant={attendance.isPresent ? 'success' : 'danger'}>
                                                {attendance.isPresent ? 'Present' : 'Absent'}
                                            </Badge>
                                        </td>
                                        <td className="py-4 px-4 text-slate-300">
                                            {attendance.remarks || '-'}
                                        </td>
                                        <td className="py-4 px-4">
                                            <div className="flex items-center justify-end gap-2">
                                                <Button
                                                    variant="ghost"
                                                    size="sm"
                                                    onClick={() => { setSelectedAttendance(attendance); setIsModalOpen(true); }}
                                                >
                                                    Edit
                                                </Button>
                                                <Button
                                                    variant="ghost"
                                                    size="sm"
                                                    onClick={() => handleDelete(attendance.id)}
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

            {/* Attendance Modal */}
            <AttendanceModal
                isOpen={isModalOpen}
                onClose={() => setIsModalOpen(false)}
                attendance={selectedAttendance}
                students={students}
                subjects={subjects}
                onSuccess={fetchData}
            />
        </div>
    );
};

const AttendanceModal = ({ isOpen, onClose, attendance, students, subjects, onSuccess }) => {
    const [formData, setFormData] = useState({
        studentId: '',
        subjectId: '',
        attendanceDate: '',
        isPresent: true,
        remarks: '',
    });
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');

    useEffect(() => {
        if (attendance) {
            setFormData({
                studentId: attendance.studentId || '',
                subjectId: attendance.subjectId || '',
                attendanceDate: attendance.attendanceDate
                    ? new Date(attendance.attendanceDate).toISOString().split('T')[0]
                    : '',
                isPresent: attendance.isPresent !== false,
                remarks: attendance.remarks || '',
            });
        } else {
            setFormData({
                studentId: '',
                subjectId: '',
                attendanceDate: new Date().toISOString().split('T')[0],
                isPresent: true,
                remarks: '',
            });
        }
    }, [attendance]);

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);
        setError('');

        try {
            const payload = {
                ...formData,
                isPresent: formData.isPresent === true || formData.isPresent === 'true',
            };

            if (attendance) {
                await apiClient.put(API_ENDPOINTS.ATTENDANCE.BY_ID(attendance.id), {
                    id: attendance.id,
                    ...payload,
                });
            } else {
                await apiClient.post(API_ENDPOINTS.ATTENDANCE.BASE, payload);
            }
            onSuccess();
            onClose();
        } catch (err) {
            setError(err.message || 'Failed to save attendance');
        } finally {
            setLoading(false);
        }
    };

    return (
        <Modal
            isOpen={isOpen}
            onClose={onClose}
            title={attendance ? 'Edit Attendance' : 'Mark Attendance'}
        >
            {error && <Alert type="error" message={error} onClose={() => setError('')} className="mb-4" />}

            <form onSubmit={handleSubmit} className="space-y-4">
                <Select
                    label="Student"
                    value={formData.studentId}
                    onChange={(e) => setFormData({ ...formData, studentId: e.target.value })}
                    options={[
                        { value: '', label: 'Select Student' },
                        ...students.map(s => ({
                            value: s.id,
                            label: `${s.firstName} ${s.lastName}`
                        }))
                    ]}
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
                        label="Date"
                        type="date"
                        value={formData.attendanceDate}
                        onChange={(e) => setFormData({ ...formData, attendanceDate: e.target.value })}
                        required
                    />
                    <Select
                        label="Status"
                        value={formData.isPresent.toString()}
                        onChange={(e) => setFormData({ ...formData, isPresent: e.target.value === 'true' })}
                        options={[
                            { value: 'true', label: 'Present' },
                            { value: 'false', label: 'Absent' }
                        ]}
                    />
                </div>

                <Input
                    label="Remarks (Optional)"
                    value={formData.remarks}
                    onChange={(e) => setFormData({ ...formData, remarks: e.target.value })}
                    placeholder="Any additional notes..."
                />

                <div className="flex gap-3 pt-4">
                    <Button type="submit" disabled={loading} className="flex-1">
                        {loading ? 'Saving...' : attendance ? 'Update Attendance' : 'Mark Attendance'}
                    </Button>
                    <Button type="button" variant="secondary" onClick={onClose} className="flex-1">
                        Cancel
                    </Button>
                </div>
            </form>
        </Modal>
    );
};

export default AttendancePage;
