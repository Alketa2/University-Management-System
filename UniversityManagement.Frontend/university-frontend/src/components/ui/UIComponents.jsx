export const Button = ({ children, variant = 'primary', size = 'md', className = '', onClick, type = 'button', disabled = false }) => {
    const baseStyles = 'inline-flex items-center justify-center font-semibold rounded-xl transition-all duration-200 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-offset-slate-950 disabled:opacity-50 disabled:cursor-not-allowed';

    const variants = {
        primary: 'bg-primary-600 hover:bg-primary-700 text-white shadow-lg shadow-primary-600/30 focus:ring-primary-500',
        secondary: 'bg-slate-800 hover:bg-slate-700 text-white border border-slate-700 focus:ring-slate-500',
        success: 'bg-success-600 hover:bg-success-700 text-white shadow-lg shadow-success-600/30 focus:ring-success-500',
        danger: 'bg-danger-600 hover:bg-danger-700 text-white shadow-lg shadow-danger-600/30 focus:ring-danger-500',
        outline: 'border-2 border-primary-600 text-primary-400 hover:bg-primary-600/10 focus:ring-primary-500',
        ghost: 'text-slate-300 hover:bg-slate-800 hover:text-white focus:ring-slate-500',
    };

    const sizes = {
        sm: 'px-3 py-1.5 text-sm',
        md: 'px-5 py-2.5 text-base',
        lg: 'px-6 py-3 text-lg',
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
                <label className="block text-sm font-medium text-slate-300 mb-2">
                    {label}
                </label>
            )}
            <input
                className={`w-full px-4 py-2.5 bg-slate-900 border ${error ? 'border-danger-500' : 'border-slate-700'
                    } rounded-xl text-white placeholder-slate-500 focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-transparent transition-all ${className}`}
                {...props}
            />
            {error && (
                <p className="mt-1.5 text-sm text-danger-400">{error}</p>
            )}
        </div>
    );
};

export const Select = ({ label, error, options, className = '', ...props }) => {
    return (
        <div className="w-full">
            {label && (
                <label className="block text-sm font-medium text-slate-300 mb-2">
                    {label}
                </label>
            )}
            <select
                className={`w-full px-4 py-2.5 bg-slate-900 border ${error ? 'border-danger-500' : 'border-slate-700'
                    } rounded-xl text-white focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-transparent transition-all ${className}`}
                {...props}
            >
                {options.map((option) => (
                    <option key={option.value} value={option.value}>
                        {option.label}
                    </option>
                ))}
            </select>
            {error && (
                <p className="mt-1.5 text-sm text-danger-400">{error}</p>
            )}
        </div>
    );
};

export const Card = ({ children, className = '', hover = false }) => {
    return (
        <div
            className={`bg-slate-900/70 border border-slate-800 rounded-2xl p-6 ${hover ? 'hover:border-slate-700 hover:shadow-lg transition-all duration-300' : ''
                } ${className}`}
        >
            {children}
        </div>
    );
};

export const Badge = ({ children, variant = 'default', className = '' }) => {
    const variants = {
        default: 'bg-slate-800 text-slate-300',
        primary: 'bg-primary-600/20 text-primary-400 border border-primary-600/30',
        success: 'bg-success-600/20 text-success-400 border border-success-600/30',
        warning: 'bg-warning-600/20 text-warning-400 border border-warning-600/30',
        danger: 'bg-danger-600/20 text-danger-400 border border-danger-600/30',
    };

    return (
        <span className={`inline-flex items-center px-3 py-1 rounded-full text-xs font-semibold ${variants[variant]} ${className}`}>
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
