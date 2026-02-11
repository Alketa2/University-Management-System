import { useState, useEffect } from 'react';
import { Button, Card, Input, Modal, Badge, Alert, Spinner, Select } from '../ui/UIComponents';
import apiClient from '../../utils/apiClient';
import { API_ENDPOINTS } from '../../config/api';

const TimetablePage = () => {
    const [timetables, setTimetables] = useState([]);
    const [programs, setPrograms] = useState([]);
    const [subjects, setSubjects] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [selectedTimetable, setSelectedTimetable] = useState(null);
    const [searchTerm, setSearchTerm] = useState('');

    useEffect(() => {
        fetchData();
    }, []);

    const fetchData = async () => {
        setLoading(true);
        try {
            const [timetablesData, programsData, subjectsData] = await Promise.all([
                apiClient.get(API_ENDPOINTS.TIMETABLES.BASE),
                apiClient.get(API_ENDPOINTS.PROGRAMS.BASE),
                apiClient.get(API_ENDPOINTS.SUBJECTS.BASE)
            ]);
            setTimetables(timetablesData);
            setPrograms(programsData);
            setSubjects(subjectsData);
        } catch (err) {
            setError(err.message || 'Failed to fetch data');
        } finally {
            setLoading(false);
        }
    };

    const handleDelete = async (id) => {
        if (!window.confirm('Are you sure you want to delete this timetable entry?')) return;

        try {
            await apiClient.delete(API_ENDPOINTS.TIMETABLES.BY_ID(id));
            setTimetables(timetables.filter(t => t.id !== id));
        } catch (err) {
            setError(err.message || 'Failed to delete timetable');
        }
    };

    const getProgramName = (programId) => {
        const program = programs.find(p => p.id === programId);
        return program ? program.name : 'Unknown';
    };

    const getSubjectName = (subjectId) => {
        const subject = subjects.find(s => s.id === subjectId);
        return subject ? subject.name : 'Unknown Subject';
    };

    const filteredTimetables = timetables.filter(timetable =>
        getProgramName(timetable.programId)?.toLowerCase().includes(searchTerm.toLowerCase()) ||
        getSubjectName(timetable.subjectId)?.toLowerCase().includes(searchTerm.toLowerCase()) ||
        timetable.dayOfWeek?.toLowerCase().includes(searchTerm.toLowerCase())
    );

    const daysOfWeek = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];
    const todayName = daysOfWeek[new Date().getDay() === 0 ? 6 : new Date().getDay() - 1];
    const todayClasses = timetables.filter(t => t.dayOfWeek === todayName);

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
                    <h2 className="text-3xl font-bold text-white">Timetable Management</h2>
                    <p className="text-slate-400 mt-1">Manage class schedules and timing</p>
                </div>
                <Button onClick={() => { setSelectedTimetable(null); setIsModalOpen(true); }}>
                    + Add Schedule
                </Button>
            </div>

            {error && <Alert type="error" message={error} onClose={() => setError('')} />}

            {/* Search bar */}
            <div className="flex gap-4">
                <Input
                    placeholder="Search timetable..."
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
                            <p className="text-sm text-slate-400">Total Schedules</p>
                            <p className="text-3xl font-bold text-white mt-1">{timetables.length}</p>
                        </div>
                        <div className="w-12 h-12 bg-primary-600/20 rounded-xl flex items-center justify-center text-2xl">
                            📅
                        </div>
                    </div>
                </Card>
                <Card>
                    <div className="flex items-center justify-between">
                        <div>
                            <p className="text-sm text-slate-400">Today's Classes</p>
                            <p className="text-3xl font-bold text-success-400 mt-1">
                                {todayClasses.length}
                            </p>
                        </div>
                        <div className="w-12 h-12 bg-success-600/20 rounded-xl flex items-center justify-center text-2xl">
                            📖
                        </div>
                    </div>
                </Card>
                <Card>
                    <div className="flex items-center justify-between">
                        <div>
                            <p className="text-sm text-slate-400">Programs</p>
                            <p className="text-3xl font-bold text-accent-400 mt-1">
                                {[...new Set(timetables.map(t => t.programId))].length}
                            </p>
                        </div>
                        <div className="w-12 h-12 bg-accent-600/20 rounded-xl flex items-center justify-center text-2xl">
                            🎓
                        </div>
                    </div>
                </Card>
                <Card>
                    <div className="flex items-center justify-between">
                        <div>
                            <p className="text-sm text-slate-400">Current Day</p>
                            <p className="text-lg font-bold text-primary-400 mt-1">
                                {todayName}
                            </p>
                        </div>
                        <div className="w-12 h-12 bg-primary-600/20 rounded-xl flex items-center justify-center text-2xl">
                            🕐
                        </div>
                    </div>
                </Card>
            </div>

            {/* Timetable table */}
            <Card>
                <div className="overflow-x-auto">
                    <table className="w-full">
                        <thead>
                            <tr className="border-b border-slate-800">
                                <th className="text-left py-4 px-4 text-sm font-semibold text-slate-300">Day</th>
                                <th className="text-left py-4 px-4 text-sm font-semibold text-slate-300">Subject</th>
                                <th className="text-left py-4 px-4 text-sm font-semibold text-slate-300">Program</th>
                                <th className="text-left py-4 px-4 text-sm font-semibold text-slate-300">Time</th>
                                <th className="text-left py-4 px-4 text-sm font-semibold text-slate-300">Room</th>
                                <th className="text-right py-4 px-4 text-sm font-semibold text-slate-300">Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            {filteredTimetables.length === 0 ? (
                                <tr>
                                    <td colSpan="6" className="text-center py-12 text-slate-400">
                                        No timetable entries found
                                    </td>
                                </tr>
                            ) : (
                                filteredTimetables
                                    .sort((a, b) => {
                                        const dayOrder = daysOfWeek.indexOf(a.dayOfWeek) - daysOfWeek.indexOf(b.dayOfWeek);
                                        if (dayOrder !== 0) return dayOrder;
                                        return (a.startTime || '').localeCompare(b.startTime || '');
                                    })
                                    .map((timetable) => {
                                        const isToday = timetable.dayOfWeek === todayName;

                                        return (
                                            <tr
                                                key={timetable.id}
                                                className={`border-b border-slate-800/50 hover:bg-slate-800/30 transition-colors ${isToday ? 'bg-primary-600/10' : ''
                                                    }`}
                                            >
                                                <td className="py-4 px-4">
                                                    <Badge variant={isToday ? 'primary' : 'default'}>
                                                        {timetable.dayOfWeek || 'N/A'}
                                                    </Badge>
                                                </td>
                                                <td className="py-4 px-4">
                                                    <p className="font-medium text-white">{getSubjectName(timetable.subjectId)}</p>
                                                </td>
                                                <td className="py-4 px-4 text-slate-300">
                                                    {getProgramName(timetable.programId)}
                                                </td>
                                                <td className="py-4 px-4 text-slate-300">
                                                    {timetable.startTime} - {timetable.endTime}
                                                </td>
                                                <td className="py-4 px-4 text-slate-300">
                                                    {timetable.roomNumber || 'TBA'}
                                                </td>
                                                <td className="py-4 px-4">
                                                    <div className="flex items-center justify-end gap-2">
                                                        <Button
                                                            variant="ghost"
                                                            size="sm"
                                                            onClick={() => { setSelectedTimetable(timetable); setIsModalOpen(true); }}
                                                        >
                                                            Edit
                                                        </Button>
                                                        <Button
                                                            variant="ghost"
                                                            size="sm"
                                                            onClick={() => handleDelete(timetable.id)}
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

            {/* Timetable Modal */}
            <TimetableModal
                isOpen={isModalOpen}
                onClose={() => setIsModalOpen(false)}
                timetable={selectedTimetable}
                programs={programs}
                subjects={subjects}
                onSuccess={fetchData}
            />
        </div>
    );
};

const TimetableModal = ({ isOpen, onClose, timetable, programs, subjects, onSuccess }) => {
    const [formData, setFormData] = useState({
        programId: '',
        subjectId: '',
        dayOfWeek: '',
        startTime: '',
        endTime: '',
        roomNumber: '',
    });
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');

    const daysOfWeek = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];

    useEffect(() => {
        if (timetable) {
            setFormData({
                programId: timetable.programId || '',
                subjectId: timetable.subjectId || '',
                dayOfWeek: timetable.dayOfWeek || '',
                startTime: timetable.startTime || '',
                endTime: timetable.endTime || '',
                roomNumber: timetable.roomNumber || '',
            });
        } else {
            setFormData({
                programId: '',
                subjectId: '',
                dayOfWeek: '',
                startTime: '',
                endTime: '',
                roomNumber: '',
            });
        }
    }, [timetable]);

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);
        setError('');

        try {
            if (timetable) {
                await apiClient.put(API_ENDPOINTS.TIMETABLES.BY_ID(timetable.id), {
                    id: timetable.id,
                    ...formData,
                });
            } else {
                await apiClient.post(API_ENDPOINTS.TIMETABLES.BASE, formData);
            }
            onSuccess();
            onClose();
        } catch (err) {
            setError(err.message || 'Failed to save timetable');
        } finally {
            setLoading(false);
        }
    };

    return (
        <Modal
            isOpen={isOpen}
            onClose={onClose}
            title={timetable ? 'Edit Timetable' : 'Add New Schedule'}
        >
            {error && <Alert type="error" message={error} onClose={() => setError('')} className="mb-4" />}

            <form onSubmit={handleSubmit} className="space-y-4">
                <div className="grid grid-cols-2 gap-4">
                    <Select
                        label="Program"
                        value={formData.programId}
                        onChange={(e) => setFormData({ ...formData, programId: e.target.value })}
                        options={[
                            { value: '', label: 'Select Program' },
                            ...programs.map(p => ({ value: p.id, label: p.name }))
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
                </div>

                <Select
                    label="Day of Week"
                    value={formData.dayOfWeek}
                    onChange={(e) => setFormData({ ...formData, dayOfWeek: e.target.value })}
                    options={[
                        { value: '', label: 'Select Day' },
                        ...daysOfWeek.map(day => ({ value: day, label: day }))
                    ]}
                    required
                />

                <div className="grid grid-cols-2 gap-4">
                    <Input
                        label="Start Time"
                        type="time"
                        value={formData.startTime}
                        onChange={(e) => setFormData({ ...formData, startTime: e.target.value })}
                        required
                    />
                    <Input
                        label="End Time"
                        type="time"
                        value={formData.endTime}
                        onChange={(e) => setFormData({ ...formData, endTime: e.target.value })}
                        required
                    />
                </div>

                <Input
                    label="Room Number"
                    value={formData.roomNumber}
                    onChange={(e) => setFormData({ ...formData, roomNumber: e.target.value })}
                    placeholder="e.g., Room 101"
                />

                <div className="flex gap-3 pt-4">
                    <Button type="submit" disabled={loading} className="flex-1">
                        {loading ? 'Saving...' : timetable ? 'Update Schedule' : 'Add Schedule'}
                    </Button>
                    <Button type="button" variant="secondary" onClick={onClose} className="flex-1">
                        Cancel
                    </Button>
                </div>
            </form>
        </Modal>
    );
};

export default TimetablePage;
