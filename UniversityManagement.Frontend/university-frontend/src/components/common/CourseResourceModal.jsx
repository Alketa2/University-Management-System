import { useState, useEffect } from 'react';
import { Modal, Button, Input, Select, Badge, Card, Alert, Spinner } from '../ui/UIComponents';
import resourceService from '../../utils/resourceService';
import authService from '../../utils/authService';

const CourseResourceModal = ({ isOpen, onClose, subject }) => {
    const [resources, setResources] = useState([]);
    const [loading, setLoading] = useState(false);
    const [adding, setAdding] = useState(false);
    const [error, setError] = useState('');
    const [formData, setFormData] = useState({
        title: '',
        description: '',
        resourceType: 'Link',
        url: ''
    });

    const userRole = authService.getUserRole();
    const canManage = userRole === 'Admin' || userRole === 'Teacher';

    useEffect(() => {
        if (isOpen && subject) {
            fetchResources();
        }
    }, [isOpen, subject]);

    const fetchResources = async () => {
        setLoading(true);
        try {
            const data = await resourceService.getSubjectResources(subject.id);
            setResources(data || []);
        } catch (err) {
            setError(err.message || 'Failed to load resources');
        } finally {
            setLoading(false);
        }
    };

    const handleAddResource = async (e) => {
        e.preventDefault();
        setAdding(true);
        setError('');
        try {
            const payload = {
                ...formData,
                subjectId: subject.id
            };
            await resourceService.addResource(payload);
            setFormData({ title: '', description: '', resourceType: 'Link', url: '' });
            fetchResources();
        } catch (err) {
            setError(err.message || 'Failed to add resource');
        } finally {
            setAdding(false);
        }
    };

    const handleDelete = async (id) => {
        if (!window.confirm('Delete this resource?')) return;
        try {
            await resourceService.deleteResource(id);
            setResources(resources.filter(r => r.id !== id));
        } catch (err) {
            setError(err.message || 'Failed to delete resource');
        }
    };

    const getIcon = (type) => {
        switch (type) {
            case 'Video': return '🎥';
            case 'PDF': return '📄';
            case 'Quiz': return '❓';
            case 'Document': return '📝';
            default: return '🔗';
        }
    };

    return (
        <Modal
            isOpen={isOpen}
            onClose={onClose}
            title={`Course Materials: ${subject?.name}`}
            size="lg"
        >
            <div className="space-y-6 max-h-[70vh] overflow-y-auto pr-2">
                {error && <Alert type="error" message={error} onClose={() => setError('')} />}

                {canManage && (
                    <Card className="bg-slate-800/30 border-slate-700/50">
                        <h4 className="text-sm font-bold text-white mb-4 flex items-center gap-2">
                            <span className="text-primary-400">⊕</span> Add New Material
                        </h4>
                        <form onSubmit={handleAddResource} className="grid grid-cols-1 md:grid-cols-2 gap-4">
                            <Input
                                label="Title"
                                value={formData.title}
                                onChange={(e) => setFormData({ ...formData, title: e.target.value })}
                                placeholder="Lecture Notes, Video Tutorial, etc."
                                required
                            />
                            <Select
                                label="Type"
                                value={formData.resourceType}
                                onChange={(e) => setFormData({ ...formData, resourceType: e.target.value })}
                                options={[
                                    { value: 'Link', label: 'Web Link' },
                                    { value: 'Video', label: 'Video' },
                                    { value: 'PDF', label: 'PDF Document' },
                                    { value: 'Quiz', label: 'Quiz/Test' },
                                    { value: 'Document', label: 'Other Document' },
                                ]}
                            />
                            <Input
                                label="URL / Link"
                                value={formData.url}
                                onChange={(e) => setFormData({ ...formData, url: e.target.value })}
                                placeholder="https://..."
                                required
                            />
                            <Input
                                label="Description"
                                value={formData.description}
                                onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                                placeholder="Optional brief details"
                            />
                            <div className="md:col-span-2 flex justify-end">
                                <Button type="submit" disabled={adding} size="sm">
                                    {adding ? 'Adding...' : 'Add Material'}
                                </Button>
                            </div>
                        </form>
                    </Card>
                )}

                <div className="space-y-3">
                    <h4 className="text-sm font-bold text-slate-400 uppercase tracking-wider px-1">Available Resources</h4>
                    {loading ? (
                        <div className="flex justify-center p-8"><Spinner /></div>
                    ) : resources.length === 0 ? (
                        <div className="text-center p-12 bg-slate-900/50 rounded-2xl border border-slate-800 border-dashed text-slate-500 text-sm">
                            No materials uploaded for this subject yet.
                        </div>
                    ) : (
                        <div className="grid grid-cols-1 gap-3">
                            {resources.map((res) => (
                                <div key={res.id} className="group p-4 bg-slate-900 border border-slate-800 rounded-xl hover:border-slate-700 transition-all flex items-center justify-between">
                                    <div className="flex items-center gap-4">
                                        <div className="w-10 h-10 rounded-lg bg-slate-800 flex items-center justify-center text-xl group-hover:bg-primary-600/20 transition-colors">
                                            {getIcon(res.resourceType)}
                                        </div>
                                        <div>
                                            <div className="flex items-center gap-2">
                                                <h5 className="font-semibold text-white">{res.title}</h5>
                                                <Badge variant="secondary" className="text-[10px] py-0">{res.resourceType}</Badge>
                                            </div>
                                            <p className="text-xs text-slate-400 mt-0.5">{res.description || 'No description provided'}</p>
                                        </div>
                                    </div>
                                    <div className="flex items-center gap-2">
                                        <a
                                            href={res.url}
                                            target="_blank"
                                            rel="noopener noreferrer"
                                            className="px-3 py-1.5 text-xs font-semibold text-primary-400 hover:text-white hover:bg-primary-600 rounded-lg transition-all"
                                        >
                                            Open ↗
                                        </a>
                                        {canManage && (
                                            <button
                                                onClick={() => handleDelete(res.id)}
                                                className="p-1.5 text-slate-500 hover:text-danger-400 transition-colors"
                                            >
                                                <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                                                </svg>
                                            </button>
                                        )}
                                    </div>
                                </div>
                            ))}
                        </div>
                    )}
                </div>
            </div>
        </Modal>
    );
};

export default CourseResourceModal;
