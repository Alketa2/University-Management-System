export const Button = ({ children, variant = 'primary', size = 'md', className = '', onClick, type = 'button', disabled = false }) => {
    const baseStyles = 'inline-flex items-center justify-center font-bold rounded-2xl transition-all duration-300 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-offset-slate-950 disabled:opacity-50 disabled:cursor-not-allowed active:scale-95';

    const variants = {
        primary: 'bg-primary-600 hover:bg-primary-500 text-white shadow-lg shadow-primary-600/20 hover:shadow-primary-600/40 focus:ring-primary-500',
        secondary: 'bg-white/5 hover:bg-white/10 text-white border border-white/10 focus:ring-slate-500 backdrop-blur-md',
        success: 'bg-success-600 hover:bg-success-500 text-white shadow-lg shadow-success-600/20 focus:ring-success-500',
        danger: 'bg-danger-600 hover:bg-danger-500 text-white shadow-lg shadow-danger-600/20 focus:ring-danger-500',
        outline: 'border-2 border-primary-500/50 text-white hover:bg-primary-500/10 focus:ring-primary-500',
        ghost: 'text-slate-400 hover:text-white hover:bg-white/5 focus:ring-slate-500',
    };

    const sizes = {
        sm: 'px-4 py-2 text-xs tracking-wider uppercase',
        md: 'px-6 py-3 text-sm tracking-widest uppercase',
        lg: 'px-8 py-4 text-base tracking-widest uppercase',
    };

    return (
        <button
            type={type}
            onClick={onClick}
            disabled={disabled}
            className={`${baseStyles} ${variants[variant]} ${sizes[size]} ${className}`}
        >
            {children}
        </button>
    );
};

export const Input = ({ label, error, className = '', ...props }) => {
    return (
        <div className="w-full">
            {label && (
                <label className="block text-[10px] uppercase tracking-[0.2em] font-black text-slate-500 mb-3 ml-1">
                    {label}
                </label>
            )}
            <input
                className={`w-full px-5 py-4 bg-white/5 border ${error ? 'border-danger-500' : 'border-white/10'
                    } rounded-2xl text-white placeholder-slate-600 focus:outline-none focus:ring-2 focus:ring-primary-500/50 focus:border-primary-500/50 transition-all duration-300 backdrop-blur-lg ${className}`}
                {...props}
            />
            {error && (
                <p className="mt-2 text-xs font-bold text-danger-400 ml-1">{error}</p>
            )}
        </div>
    );
};

export const Select = ({ label, error, options, className = '', ...props }) => {
    return (
        <div className="w-full">
            {label && (
                <label className="block text-[10px] uppercase tracking-[0.2em] font-black text-slate-500 mb-3 ml-1">
                    {label}
                </label>
            )}
            <select
                className={`w-full px-5 py-4 bg-white/5 border ${error ? 'border-danger-500' : 'border-white/10'
                    } rounded-2xl text-white focus:outline-none focus:ring-2 focus:ring-primary-500/50 focus:border-primary-500/50 transition-all duration-300 backdrop-blur-lg appearance-none ${className}`}
                {...props}
            >
                {options.map((option) => (
                    <option key={option.value} value={option.value} className="bg-slate-900">
                        {option.label}
                    </option>
                ))}
            </select>
            {error && (
                <p className="mt-2 text-xs font-bold text-danger-400 ml-1">{error}</p>
            )}
        </div>
    );
};

export const Card = ({ children, className = '', hover = false }) => {
    return (
        <div
            className={`glass-card rounded-[2rem] p-8 ${hover ? 'hover:scale-[1.02] hover:shadow-glow-primary' : ''
                } transition-all duration-500 ${className}`}
        >
            {children}
        </div>
    );
};

export const Badge = ({ children, variant = 'default', className = '' }) => {
    const variants = {
        default: 'bg-white/5 text-slate-400 border border-white/10',
        primary: 'bg-primary-500/10 text-primary-400 border border-primary-500/20 shadow-glow',
        success: 'bg-success-500/10 text-success-400 border border-success-500/20',
        warning: 'bg-warning-500/10 text-warning-400 border border-warning-500/20',
        danger: 'bg-danger-500/10 text-danger-400 border border-danger-500/20',
    };

    return (
        <span className={`inline-flex items-center px-4 py-1.5 rounded-xl text-[10px] font-black uppercase tracking-widest ${variants[variant]} ${className}`}>
            {children}
        </span>
    );
};

export const Modal = ({ isOpen, onClose, title, children, className = '' }) => {
    if (!isOpen) return null;

    return (
        <div className="fixed inset-0 z-50 flex items-center justify-center p-4 animate-fade-in">
            <div className="absolute inset-0 bg-black/60 backdrop-blur-sm" onClick={onClose} />
            <div className={`relative bg-slate-900 border border-slate-800 rounded-2xl shadow-2xl max-w-2xl w-full max-h-[90vh] overflow-y-auto animate-scale-in ${className}`}>
                <div className="sticky top-0 bg-slate-900 border-b border-slate-800 px-6 py-4 flex items-center justify-between z-10">
                    <h3 className="text-xl font-bold text-white">{title}</h3>
                    <button
                        onClick={onClose}
                        className="text-slate-400 hover:text-white transition-colors"
                    >
                        <svg className="w-6 h-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                        </svg>
                    </button>
                </div>
                <div className="p-6">
                    {children}
                </div>
            </div>
        </div>
    );
};

export const Spinner = ({ size = 'md', className = '' }) => {
    const sizes = {
        sm: 'w-4 h-4 border-2',
        md: 'w-8 h-8 border-3',
        lg: 'w-12 h-12 border-4',
    };

    return (
        <div className={`${sizes[size]} border-primary-600 border-t-transparent rounded-full animate-spin ${className}`} />
    );
};

export const Alert = ({ type = 'info', message, onClose }) => {
    const types = {
        success: 'bg-success-600/20 border-success-600/30 text-success-400',
        error: 'bg-danger-600/20 border-danger-600/30 text-danger-400',
        warning: 'bg-warning-600/20 border-warning-600/30 text-warning-400',
        info: 'bg-primary-600/20 border-primary-600/30 text-primary-400',
    };

    return (
        <div className={`rounded-xl border p-4 flex items-start justify-between animate-slide-down ${types[type]}`}>
            <p className="flex-1">{message}</p>
            {onClose && (
                <button onClick={onClose} className="ml-4 hover:opacity-70 transition-opacity">
                    <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                    </svg>
                </button>
            )}
        </div>
    );
};
