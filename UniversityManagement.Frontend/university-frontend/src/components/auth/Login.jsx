import { useState } from 'react';
import { Button, Input, Alert, Card, Badge } from '../ui/UIComponents';
import authService from '../../utils/authService';

const Login = ({ onLoginSuccess }) => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');
    const [showRegister, setShowRegister] = useState(false);

    const handleLogin = async (e) => {
        e.preventDefault();
        setError('');
        setLoading(true);

        try {
            await authService.login(email, password);
            onLoginSuccess();
        } catch (err) {
            setError(err.message || 'Login failed. Please check your credentials.');
        } finally {
            setLoading(false);
        }
    };

    if (showRegister) {
        return <Register onBack={() => setShowRegister(false)} onRegisterSuccess={onLoginSuccess} />;
    }

    return (
        <div className="min-h-screen flex items-center justify-center p-6 bg-slate-950">
            <div className="relative w-full max-w-md z-10">
                {/* Simplified Branding Section */}
                <div className="text-center mb-8">
                    <div className="inline-flex mb-6">
                        <div className="h-20 w-20 items-center justify-center flex rounded-2xl bg-primary-600 shadow-lg">
                            <span className="text-4xl font-black text-white">U</span>
                        </div>
                    </div>

                    <div className="flex justify-center mb-4">
                        <Badge variant="primary" className="px-4 py-1">university</Badge>
                    </div>

                    <h1 className="text-4xl font-black text-white tracking-tight mb-2">
                        Welcome Back
                    </h1>
                    <p className="text-slate-500 text-xs tracking-widest uppercase font-bold">Sign in to your university account</p>
                </div>

                {/* Authentication Card */}
                <Card className="p-8 border border-slate-800 bg-slate-900">
                    {error && <Alert type="error" message={error} onClose={() => setError('')} className="mb-6" />}

                    <form onSubmit={handleLogin} className="space-y-6">
                        <Input
                            label="Email Address"
                            type="email"
                            placeholder="Enter your email"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            required
                        />

                        <Input
                            label="Password"
                            type="password"
                            placeholder="Enter your password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            required
                        />

                        <div className="flex items-center justify-between">
                            <label className="flex items-center text-slate-500 text-[10px] font-bold uppercase tracking-wider cursor-pointer">
                                <input type="checkbox" className="mr-2 w-4 h-4 rounded border-slate-700 bg-slate-800 text-primary-600 focus:ring-primary-600" />
                                <span>Remember me</span>
                            </label>
                            <button type="button" className="text-[10px] font-bold uppercase tracking-wider text-slate-500 hover:text-primary-400 transition-colors">
                                Forgot password?
                            </button>
                        </div>

                        <Button type="submit" className="w-full h-14 text-xs tracking-widest uppercase font-bold" disabled={loading}>
                            {loading ? 'Signing in...' : 'Sign In'}
                        </Button>
                    </form>

                    <div className="mt-8 pt-6 border-t border-slate-800 text-center">
                        <p className="text-slate-500 text-[10px] font-bold uppercase tracking-wider">
                            Don't have an account?{' '}
                            <button
                                type="button"
                                onClick={() => setShowRegister(true)}
                                className="text-primary-400 hover:text-white transition-colors ml-1 font-bold"
                            >
                                Sign up
                            </button>
                        </p>
                    </div>
                </Card>

                {/* Footer info */}
                <div className="mt-8 text-center">
                    <p className="text-[10px] font-bold uppercase tracking-[0.4em] text-slate-700">University Management System</p>
                </div>
            </div>
        </div>
    );
};

const Register = ({ onBack, onRegisterSuccess }) => {
    const [formData, setFormData] = useState({
        firstName: '',
        lastName: '',
        email: '',
        password: '',
        confirmPassword: '',
    });
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');

    const handleChange = (e) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    const handleRegister = async (e) => {
        e.preventDefault();
        setError('');

        if (formData.password !== formData.confirmPassword) {
            setError('Passwords do not match');
            return;
        }

        if (formData.password.length < 6) {
            setError('Password must be at least 6 characters long');
            return;
        }

        setLoading(true);
        try {
            await authService.register(
                formData.firstName,
                formData.lastName,
                formData.email,
                formData.password
            );
            onRegisterSuccess();
        } catch (err) {
            setError(err.message || 'Registration failed. Please try again.');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="min-h-screen flex items-center justify-center p-6 bg-slate-950">
            <div className="relative w-full max-w-lg z-10">
                <div className="text-center mb-8">
                    <div className="inline-flex mb-6">
                        <div className="h-20 w-20 items-center justify-center flex rounded-2xl bg-accent-600 shadow-lg">
                            <span className="text-4xl font-black text-white">U</span>
                        </div>
                    </div>

                    <div className="flex justify-center mb-4">
                        <Badge variant="warning" className="px-4 py-1">university</Badge>
                    </div>

                    <h1 className="text-4xl font-black text-white tracking-tight mb-2">
                        Create Account
                    </h1>
                    <p className="text-slate-500 text-xs tracking-widest uppercase font-bold">Join the university management system</p>
                </div>

                <Card className="p-8 border border-slate-800 bg-slate-900">
                    {error && <Alert type="error" message={error} onClose={() => setError('')} className="mb-6" />}

                    <form onSubmit={handleRegister} className="space-y-6">
                        <div className="grid grid-cols-2 gap-4">
                            <Input
                                label="First Name"
                                name="firstName"
                                placeholder="Name"
                                value={formData.firstName}
                                onChange={handleChange}
                                required
                            />
                            <Input
                                label="Last Name"
                                name="lastName"
                                placeholder="Surname"
                                value={formData.lastName}
                                onChange={handleChange}
                                required
                            />
                        </div>

                        <Input
                            label="Email Address"
                            type="email"
                            name="email"
                            placeholder="user@gmail.com"
                            value={formData.email}
                            onChange={handleChange}
                            required
                        />

                        <div className="grid grid-cols-2 gap-4">
                            <Input
                                label="Password"
                                type="password"
                                name="password"
                                placeholder="••••••"
                                value={formData.password}
                                onChange={handleChange}
                                required
                            />
                            <Input
                                label="Confirm Password"
                                type="password"
                                name="confirmPassword"
                                placeholder="••••••"
                                value={formData.confirmPassword}
                                onChange={handleChange}
                                required
                            />
                        </div>

                        <Button type="submit" className="w-full h-14 text-xs tracking-widest uppercase font-bold" disabled={loading}>
                            {loading ? 'Creating account...' : 'Create Account'}
                        </Button>
                    </form>

                    <div className="mt-8 pt-6 border-t border-slate-800 text-center">
                        <p className="text-slate-500 text-[10px] font-bold uppercase tracking-wider">
                            Already have an account?{' '}
                            <button
                                type="button"
                                onClick={onBack}
                                className="text-primary-400 hover:text-white transition-colors ml-1 font-bold"
                            >
                                Sign in
                            </button>
                        </p>
                    </div>
                </Card>
                <div className="mt-8 text-center">
                    <p className="text-[10px] font-bold uppercase tracking-[0.4em] text-slate-700">University Management System</p>
                </div>
            </div>
        </div>
    );
};

export default Login;
