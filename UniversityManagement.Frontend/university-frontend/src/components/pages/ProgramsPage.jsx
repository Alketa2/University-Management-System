import { useState, useEffect } from 'react';
import { Button, Card, Input, Modal, Badge, Alert, Spinner } from '../ui/UIComponents';
import apiClient from '../../utils/apiClient';
import { API_ENDPOINTS } from '../../config/api';

const ProgramsPage = () => {
    const [programs, setPrograms] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [selectedProgram, setSelectedProgram] = useState(null);
    const [searchTerm, setSearchTerm] = useState('');

    useEffect(() => {
        fetchPrograms();
    }, []);

    const fetchPrograms = async () => {
        setLoading(true);
        try {
            const data = await apiClient.get(API_ENDPOINTS.PROGRAMS.BASE);
            setPrograms(data);
        } catch (err) {
            setError(err.message || 'Failed to fetch programs');
        } finally {
            setLoading(false);
        }
    };

    const handleDelete = async (id) => {
        if (!window.confirm('Are you sure you want to delete this program? Note: Programs with linked subjects cannot be deleted.')) return;

        try {
            await apiClient.delete(API_ENDPOINTS.PROGRAMS.BY_ID(id));
            setPrograms(programs.filter(p => p.id !== id));
        } catch (err) {
            // Show user-friendly error message for foreign key constraint
            if (err.message?.includes('foreign key constraint') || err.message?.includes('Cannot delete')) {
                setError('Cannot delete this program because it has subjects or other records linked to it. Please remove or reassign those records first.');
            } else {
                setError(err.message || 'Failed to delete program');
            }
        }
    };

    const filteredPrograms = programs.filter(program =>
        program.name?.toLowerCase().includes(searchTerm.toLowerCase()) ||
        program.code?.toLowerCase().includes(searchTerm.toLowerCase()) ||
        program.description?.toLowerCase().includes(searchTerm.toLowerCase())
    );

    const activePrograms = programs.filter(p => p.isActive !== false);
    const avgDuration = programs.length > 0
        ? (programs.reduce((sum, p) => sum + (p.duration || 0), 0) / programs.length).toFixed(1)
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
                    <h2 className="text-3xl font-bold text-white">Programs Management</h2>
                    <p className="text-slate-400 mt-1">Manage academic programs and degrees</p>
                </div>
                <Button onClick={() => { setSelectedProgram(null); setIsModalOpen(true); }}>
                    + Add Program
                </Button>
            </div>

            {error && <Alert type="error" message={error} onClose={() => setError('')} />}

            {/* Search bar */}
            <div className="flex gap-4">
                <Input
                    placeholder="Search programs..."
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
                            <p className="text-sm text-slate-400">Total Programs</p>
                            <p className="text-3xl font-bold text-white mt-1">{programs.length}</p>
                        </div>
                        <div className="w-12 h-12 bg-primary-600/20 rounded-xl flex items-center justify-center text-2xl">
                            🎓
                        </div>
                    </div>
                </Card>
                <Card>
                    <div className="flex items-center justify-between">
                        <div>
                            <p className="text-sm text-slate-400">Active Programs</p>
                            <p className="text-3xl font-bold text-success-400 mt-1">
                                {activePrograms.length}
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
                            <p className="text-sm text-slate-400">Avg Duration</p>
                            <p className="text-3xl font-bold text-accent-400 mt-1">
                                {avgDuration}<span className="text-sm text-slate-400 ml-1">sem</span>
                            </p>
                        </div>
                        <div className="w-12 h-12 bg-accent-600/20 rounded-xl flex items-center justify-center text-2xl">
                            📚
                        </div>
                    </div>
                </Card>
                <Card>
                    <div className="flex items-center justify-between">
                        <div>
                            <p className="text-sm text-slate-400">Total Credits</p>
                            <p className="text-3xl font-bold text-warning-400 mt-1">
                                {programs.reduce((sum, p) => sum + (p.creditsRequired || 0), 0)}
                            </p>
                        </div>
                        <div className="w-12 h-12 bg-warning-600/20 rounded-xl flex items-center justify-center text-2xl">
                            🎯
                        </div>
                    </div>
                </Card>
            </div>

            {/* Programs Grid */}
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {filteredPrograms.length === 0 ? (
                    <Card className="col-span-full">
                        <div className="text-center py-12 text-slate-400">
                            No programs found
                        </div>
                    </Card>
                ) : (
                    filteredPrograms.map((program) => (
                        <Card key={program.id} hover className="relative">
                            <div className="absolute top-4 right-4">
                                <Badge variant={program.isActive !== false ? 'success' : 'default'}>
                                    {program.isActive !== false ? 'Active' : 'Inactive'}
                                </Badge>
                            </div>

                            <div className="mb-4">
                                <div className="w-14 h-14 rounded-xl bg-gradient-to-br from-primary-500 to-accent-500 flex items-center justify-center text-2xl mb-4">
                                    🎓
                                </div>
                                <h3 className="text-xl font-bold text-white mb-2">{program.name}</h3>
                                {program.code && (
                                    <p className="text-sm font-mono text-primary-400 font-semibold mb-2">{program.code}</p>
                                )}
                                {program.description && (
                                    <p className="text-sm text-slate-400 mb-4 line-clamp-2">
                                        {program.description}
                                    </p>
                                )}
                            </div>

                            <div className="space-y-2 mb-4 text-sm">
                                {program.duration && (
                                    <div className="flex items-center gap-2 text-slate-300">
                                        <span className="text-success-400">⏱️</span>
                                        <span>{program.duration} {program.duration === 1 ? 'semester' : 'semesters'}</span>
                                    </div>
                                )}
                                {program.creditsRequired && (
                                    <div className="flex items-center gap-2 text-slate-300">
                                        <span className="text-accent-400">🎯</span>
                                        <span>{program.creditsRequired} credits required</span>
                                    </div>
                                )}
                                {program.startDate && (
                                    <div className="flex items-center gap-2 text-slate-300">
                                        <span className="text-warning-400">📅</span>
                                        <span>Started {new Date(program.startDate).toLocaleDateString()}</span>
                                    </div>
                                )}
                            </div>

                            <div className="flex gap-2 pt-4 border-t border-slate-800">
                                <Button
                                    variant="ghost"
                                    size="sm"
                                    onClick={() => { setSelectedProgram(program); setIsModalOpen(true); }}
                                    className="flex-1"
                                >
                                    Edit
                                </Button>
                                <Button
                                    variant="ghost"
                                    size="sm"
                                    onClick={() => handleDelete(program.id)}
                                    className="flex-1 text-danger-400 hover:text-danger-300"
                                >
                                    Delete
                                </Button>
                            </div>
                        </Card>
                    ))
                )}
            </div>

            {/* Program Modal */}
            <ProgramModal
                isOpen={isModalOpen}
                onClose={() => setIsModalOpen(false)}
                program={selectedProgram}
                onSuccess={fetchPrograms}
            />
        </div>
    );
};

const ProgramModal = ({ isOpen, onClose, program, onSuccess }) => {
    const [formData, setFormData] = useState({
        name: '',
        code: '',
        description: '',
        duration: '',
        creditsRequired: '',
        startDate: '',
        isActive: true,
    });
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');

    useEffect(() => {
        if (program) {
            setFormData({
                name: program.name || '',
                code: program.code || '',
                description: program.description || '',
                duration: program.duration || '',
                creditsRequired: program.creditsRequired || '',
                startDate: program.startDate ? new Date(program.startDate).toISOString().split('T')[0] : '',
                isActive: program.isActive !== false,
            });
        } else {
            setFormData({
                name: '',
                code: '',
                description: '',
                duration: '',
                creditsRequired: '',
                startDate: '',
                isActive: true,
            });
        }
    }, [program]);

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);
        setError('');

        try {
            const payload = {
                ...formData,
                duration: parseInt(formData.duration) || 0,
                creditsRequired: parseInt(formData.creditsRequired) || 0,
                startDate: formData.startDate || new Date().toISOString(),
                isActive: formData.isActive,
            };

            if (program) {
                await apiClient.put(API_ENDPOINTS.PROGRAMS.BY_ID(program.id), {
                    id: program.id,
                    ...payload,
                });
            } else {
                await apiClient.post(API_ENDPOINTS.PROGRAMS.BASE, payload);
            }
            onSuccess();
            onClose();
        } catch (err) {
            setError(err.message || 'Failed to save program');
        } finally {
            setLoading(false);
        }
    };

    return (
        <Modal
            isOpen={isOpen}
            onClose={onClose}
            title={program ? 'Edit Program' : 'Add New Program'}
        >
            {error && <Alert type="error" message={error} onClose={() => setError('')} className="mb-4" />}

            <form onSubmit={handleSubmit} className="space-y-4">
                <div className="grid grid-cols-2 gap-4">
                    <Input
                        label="Program Code"
                        value={formData.code}
                        onChange={(e) => setFormData({ ...formData, code: e.target.value })}
                        placeholder="e.g., CS-BS"
                        required
                    />
                    <Input
                        label="Duration (Semesters)"
                        type="number"
                        value={formData.duration}
                        onChange={(e) => setFormData({ ...formData, duration: e.target.value })}
                        min="1"
                        max="40"
                        required
                    />
                </div>

                <Input
                    label="Program Name"
                    value={formData.name}
                    onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                    placeholder="e.g., Bachelor of Science in Computer Science"
                    required
                />

                <div className="w-full">
                    <label className="block text-sm font-medium text-slate-300 mb-2">
                        Description
                    </label>
                    <textarea
                        className="w-full px-4 py-2.5 bg-slate-900 border border-slate-700 rounded-xl text-white placeholder-slate-500 focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-transparent transition-all min-h-[100px]"
                        value={formData.description}
                        onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                        placeholder="Brief description of the program"
                    />
                </div>

                <div className="grid grid-cols-2 gap-4">
                    <Input
                        label="Credits Required"
                        type="number"
                        value={formData.creditsRequired}
                        onChange={(e) => setFormData({ ...formData, creditsRequired: e.target.value })}
                        min="0"
                        max="500"
                        placeholder="e.g., 120"
                    />
                    <Input
                        label="Start Date"
                        type="date"
                        value={formData.startDate}
                        onChange={(e) => setFormData({ ...formData, startDate: e.target.value })}
                        required
                    />
                </div>

                <div className="flex items-center gap-3 p-4 bg-slate-900/50 rounded-xl border border-slate-700">
                    <input
                        type="checkbox"
                        id="isActive"
                        checked={formData.isActive}
                        onChange={(e) => setFormData({ ...formData, isActive: e.target.checked })}
                        className="w-5 h-5 rounded border-slate-600 bg-slate-800 text-primary-500 focus:ring-2 focus:ring-primary-500 focus:ring-offset-0 cursor-pointer"
                    />
                    <label htmlFor="isActive" className="text-sm font-medium text-slate-300 cursor-pointer flex-1">
                        Active Program
                        <span className="block text-xs text-slate-500 mt-0.5">Inactive programs won't appear in subject creation</span>
                    </label>
                </div>

                <div className="flex gap-3 pt-4">
                    <Button type="submit" disabled={loading} className="flex-1">
                        {loading ? 'Saving...' : program ? 'Update Program' : 'Add Program'}
                    </Button>
                    <Button type="button" variant="secondary" onClick={onClose} className="flex-1">
                        Cancel
                    </Button>
                </div>
            </form>
        </Modal>
    );
};

export default ProgramsPage;
