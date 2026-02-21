import { useState, useEffect } from 'react';
import { Button, Card, Input, Badge, Alert, Spinner, Select } from '../ui/UIComponents';
import apiClient from '../../utils/apiClient';
import { API_ENDPOINTS } from '../../config/api';

const UsersPage = () => {
    const [users, setUsers] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [searchTerm, setSearchTerm] = useState('');

    useEffect(() => {
        fetchUsers();
    }, []);

    const fetchUsers = async () => {
        setLoading(true);
        try {
            const data = await apiClient.get(API_ENDPOINTS.USERS.BASE);
            setUsers(data);
        } catch (err) {
            setError(err.message || 'Failed to fetch users');
        } finally {
            setLoading(false);
        }
    };

    const handleRoleChange = async (userId, newRole) => {
        try {
            await apiClient.put(API_ENDPOINTS.USERS.UPDATE_ROLE(userId), { role: newRole });
            setUsers(users.map(u => u.id === userId ? { ...u, role: newRole } : u));
        } catch (err) {
            setError(err.message || 'Failed to update role');
        }
    };

    const handleToggleStatus = async (userId) => {
        try {
            const result = await apiClient.put(API_ENDPOINTS.USERS.TOGGLE_STATUS(userId));
            setUsers(users.map(u => u.id === userId ? { ...u, isActive: result.isActive } : u));
        } catch (err) {
            setError(err.message || 'Failed to update status');
        }
    };

    const handleDelete = async (id) => {
        if (!window.confirm('Are you sure you want to delete this user?')) return;

        try {
            await apiClient.delete(API_ENDPOINTS.USERS.DELETE(id));
            setUsers(users.filter(u => u.id !== id));
        } catch (err) {
            setError(err.message || 'Failed to delete user');
        }
    };

    const filteredUsers = users.filter(user =>
        user.firstName?.toLowerCase().includes(searchTerm.toLowerCase()) ||
        user.lastName?.toLowerCase().includes(searchTerm.toLowerCase()) ||
        user.email?.toLowerCase().includes(searchTerm.toLowerCase())
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
            <div>
                <h2 className="text-3xl font-bold text-white">Users Management</h2>
                <p className="text-slate-400 mt-1">Manage all system users, roles, and access</p>
            </div>

            {error && <Alert type="error" message={error} onClose={() => setError('')} />}

            <div className="flex gap-4">
                <Input
                    placeholder="Search users..."
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                    className="max-w-md"
                />
            </div>

            <Card>
                <div className="overflow-x-auto">
                    <table className="w-full">
                        <thead>
                            <tr className="border-b border-slate-800">
                                <th className="text-left py-4 px-4 text-sm font-semibold text-slate-300">User</th>
                                <th className="text-left py-4 px-4 text-sm font-semibold text-slate-300">Email</th>
                                <th className="text-left py-4 px-4 text-sm font-semibold text-slate-300">Role</th>
                                <th className="text-left py-4 px-4 text-sm font-semibold text-slate-300">Status</th>
                                <th className="text-left py-4 px-4 text-sm font-semibold text-slate-300">Joined</th>
                                <th className="text-right py-4 px-4 text-sm font-semibold text-slate-300">Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            {filteredUsers.length === 0 ? (
                                <tr>
                                    <td colSpan="6" className="text-center py-12 text-slate-400">
                                        No users found
                                    </td>
                                </tr>
                            ) : (
                                filteredUsers.map((user) => (
                                    <tr key={user.id} className="border-b border-slate-800/50 hover:bg-slate-800/30 transition-colors">
                                        <td className="py-4 px-4">
                                            <div className="flex items-center gap-3">
                                                <div className="w-10 h-10 rounded-full bg-slate-700 flex items-center justify-center text-white font-bold">
                                                    {user.firstName?.[0]}{user.lastName?.[0]}
                                                </div>
                                                <div>
                                                    <p className="font-medium text-white">{user.firstName} {user.lastName}</p>
                                                </div>
                                            </div>
                                        </td>
                                        <td className="py-4 px-4 text-slate-300">{user.email}</td>
                                        <td className="py-4 px-4">
                                            <Select
                                                value={user.role}
                                                onChange={(e) => handleRoleChange(user.id, e.target.value)}
                                                options={[
                                                    { value: 'Admin', label: 'Admin' },
                                                    { value: 'Teacher', label: 'Teacher' },
                                                    { value: 'Student', label: 'Student' }
                                                ]}
                                                className="w-32"
                                            />
                                        </td>
                                        <td className="py-4 px-4">
                                            <button onClick={() => handleToggleStatus(user.id)}>
                                                <Badge variant={user.isActive ? 'success' : 'danger'}>
                                                    {user.isActive ? 'Active' : 'Deactivated'}
                                                </Badge>
                                            </button>
                                        </td>
                                        <td className="py-4 px-4 text-slate-300">
                                            {new Date(user.createdAt).toLocaleDateString()}
                                        </td>
                                        <td className="py-4 px-4">
                                            <div className="flex items-center justify-end">
                                                <Button
                                                    variant="ghost"
                                                    size="sm"
                                                    onClick={() => handleDelete(user.id)}
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
        </div>
    );
};

export default UsersPage;
