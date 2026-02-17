import { useState, useEffect } from 'react';
import { Card, Badge, Spinner, Alert, Button, Input } from '../ui/UIComponents';
import apiClient from '../../utils/apiClient';
import { API_ENDPOINTS } from '../../config/api';
import authService from '../../utils/authService';

const ProfilePage = () => {
    const [profile, setProfile] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [isEditing, setIsEditing] = useState(false);
    const [formData, setFormData] = useState({});
    const [saving, setSaving] = useState(false);

    const userRole = authService.getUserRole();
    const isStudent = userRole === 'Student';
    const isTeacher = userRole === 'Teacher';
    const isAdmin = userRole === 'Admin';

    useEffect(() => {
        fetchProfile();
    }, []);

    const fetchProfile = async () => {
        setLoading(true);
        try {
            const endpoint = isStudent
                ? API_ENDPOINTS.STUDENTS.PROFILE
                : isTeacher
                    ? API_ENDPOINTS.TEACHERS.PROFILE
                    : null;

            if (endpoint) {
                const data = await apiClient.get(endpoint);
                setProfile(data);
                setFormData(data);
            } else {
                // Admin might not have a student/teacher profile
                const user = authService.getUser();
                setProfile({
                    firstName: user.firstName,
                    lastName: user.lastName,
                    email: user.email,
                    role: user.role
                });
            }
        } catch (err) {
            setError(err.message || 'Failed to fetch profile');
        } finally {
            setLoading(false);
        }
    };

    const handleUpdateProfile = async (e) => {
        e.preventDefault();
        setSaving(true);
        try {
            // Only update email/phone/address if applicable
            // Note: In real system, admins handle core changes, but we'll allow small updates
            const endpoint = isStudent
                ? API_ENDPOINTS.STUDENTS.BY_ID(profile.id)
                : isTeacher
                    ? API_ENDPOINTS.TEACHERS.BY_ID(profile.id)
                    : API_ENDPOINTS.AUTH.UPDATE_PROFILE;

            if (endpoint) {
                // We shouldn't allow role change or ID change
                const { id, role, ...updateData } = formData;

                if (isAdmin) {
                    await apiClient.put(endpoint, updateData);
                } else {
                    await apiClient.put(endpoint, { id, ...updateData });
                }

                setProfile(formData);
                setIsEditing(false);
            }
        } catch (err) {
            setError(err.message || 'Failed to update profile');
        } finally {
            setSaving(false);
        }
    };

    if (loading) {
        return (
            <div className="flex items-center justify-center h-96">
                <Spinner size="lg" />
            </div>
        );
    }

    return (
        <div className="max-w-4xl mx-auto space-y-8">
            <div className="flex items-center justify-between">
                <div>
                    <h2 className="text-3xl font-bold text-white">Your Profile</h2>
                    <p className="text-slate-400 mt-1">Manage your personal information and preferences</p>
                </div>
                {!isEditing ? (
                    <Button variant="secondary" onClick={() => setIsEditing(true)}>
                        Edit Profile
                    </Button>
                ) : (
                    <div className="flex gap-3">
                        <Button variant="ghost" onClick={() => { setIsEditing(false); setFormData(profile); }}>
                            Cancel
                        </Button>
                        <Button onClick={handleUpdateProfile} disabled={saving}>
                            {saving ? 'Saving...' : 'Save Changes'}
                        </Button>
                    </div>
                )}
            </div>

            {error && <Alert type="error" message={error} onClose={() => setError('')} />}

            <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
                {/* Profile Overview */}
                <Card className="md:col-span-1 text-center py-10">
                    <div className="relative inline-block mb-6">
                        <div className="w-32 h-32 bg-gradient-to-br from-primary-600 to-accent-600 rounded-full flex items-center justify-center text-4xl font-bold text-white border-4 border-slate-800 shadow-xl">
                            {profile?.firstName?.[0]}{profile?.lastName?.[0]}
                        </div>
                        <Badge variant="primary" className="absolute -bottom-2 left-1/2 -translate-x-1/2 px-4 py-1">
                            {userRole}
                        </Badge>
                    </div>
                    <h3 className="text-2xl font-bold text-white">
                        {profile?.firstName} {profile?.lastName}
                    </h3>
                    <p className="text-slate-400 mt-1">{profile?.email}</p>

                    {isStudent && profile?.gpa !== undefined && (
                        <div className="mt-8 pt-8 border-t border-slate-800">
                            <p className="text-xs uppercase tracking-wider text-slate-500 font-bold mb-2">Academic Standing</p>
                            <p className="text-4xl font-bold text-success-400">{profile.gpa}</p>
                            <p className="text-sm text-slate-400 mt-1">Cumulative GPA</p>
                        </div>
                    )}
                </Card>

                {/* Profile Details */}
                <Card className="md:col-span-2">
                    <h3 className="text-xl font-bold text-white mb-6">Personal Details</h3>
                    <form className="grid grid-cols-1 md:grid-cols-2 gap-6">
                        <Input
                            label="First Name"
                            value={formData?.firstName || ''}
                            onChange={(e) => setFormData({ ...formData, firstName: e.target.value })}
                            disabled={!isEditing || true} // Don't allow name change in self-service usually
                        />
                        <Input
                            label="Last Name"
                            value={formData?.lastName || ''}
                            onChange={(e) => setFormData({ ...formData, lastName: e.target.value })}
                            disabled={!isEditing || true}
                        />
                        <Input
                            label="Email Address"
                            value={formData?.email || ''}
                            onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                            disabled={!isEditing}
                            type="email"
                        />
                        <Input
                            label="Phone Number"
                            value={formData?.phone || ''}
                            onChange={(e) => setFormData({ ...formData, phone: e.target.value })}
                            disabled={!isEditing}
                            type="tel"
                        />
                        {isStudent && (
                            <>
                                <Input
                                    label="Enrollment Date"
                                    value={profile?.enrollmentDate ? new Date(profile.enrollmentDate).toLocaleDateString() : 'N/A'}
                                    disabled
                                />
                                <Input
                                    label="Primary Program"
                                    value={profile?.primaryProgramName || 'Not Assigned'}
                                    disabled
                                />
                            </>
                        )}
                        {isTeacher && (
                            <>
                                <Input
                                    label="Department"
                                    value={profile?.department || 'N/A'}
                                    disabled
                                />
                                <Input
                                    label="Hire Date"
                                    value={profile?.hireDate ? new Date(profile.hireDate).toLocaleDateString() : 'N/A'}
                                    disabled
                                />
                            </>
                        )}
                        <div className="md:col-span-2">
                            <Input
                                label="Home Address"
                                value={formData?.address || ''}
                                onChange={(e) => setFormData({ ...formData, address: e.target.value })}
                                disabled={!isEditing}
                                placeholder="Enter your full address"
                            />
                        </div>
                    </form>
                </Card >
            </div >
        </div >
    );
};

export default ProfilePage;
