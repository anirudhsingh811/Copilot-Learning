// ============================================================
// ADVANCED REACT PATTERNS & TECHNIQUES
// For interviews and real-world applications
// ============================================================

// ============================================================
// 1. ERROR BOUNDARY (Class Component - Required)
// ============================================================

class ErrorBoundary extends React.Component {
    constructor(props) {
        super(props);
        this.state = { hasError: false, error: null };
    }

    static getDerivedStateFromError(error) {
        return { hasError: true, error };
    }

    componentDidCatch(error, errorInfo) {
        console.error('Error caught by boundary:', error, errorInfo);
    }

    render() {
        if (this.state.hasError) {
            return (
                <div className="alert alert-danger">
                    <h4>Something went wrong!</h4>
                    <p>{this.state.error?.message}</p>
                    <button
                        className="btn btn-primary"
                        onClick={() => this.setState({ hasError: false })}
                    >
                        Try Again
                    </button>
                </div>
            );
        }

        return this.props.children;
    }
}

// ============================================================
// 2. CUSTOM HOOKS - ADVANCED
// ============================================================

// Hook: Intersection Observer (Infinite Scroll, Lazy Load)
const useIntersectionObserver = (options = {}) => {
    const [entry, setEntry] = React.useState(null);
    const elementRef = React.useRef(null);

    React.useEffect(() => {
        const element = elementRef.current;
        if (!element) return;

        const observer = new IntersectionObserver(([entry]) => {
            setEntry(entry);
        }, options);

        observer.observe(element);

        return () => {
            if (element) {
                observer.unobserve(element);
            }
        };
    }, [options.threshold, options.root, options.rootMargin]);

    return [elementRef, entry];
};

// Hook: Window Size (Responsive)
const useWindowSize = () => {
    const [size, setSize] = React.useState({
        width: window.innerWidth,
        height: window.innerHeight
    });

    React.useEffect(() => {
        const handleResize = () => {
            setSize({
                width: window.innerWidth,
                height: window.innerHeight
            });
        };

        window.addEventListener('resize', handleResize);
        return () => window.removeEventListener('resize', handleResize);
    }, []);

    return size;
};

// Hook: Media Query
const useMediaQuery = (query) => {
    const [matches, setMatches] = React.useState(
        () => window.matchMedia(query).matches
    );

    React.useEffect(() => {
        const mediaQuery = window.matchMedia(query);
        const handler = (e) => setMatches(e.matches);

        mediaQuery.addEventListener('change', handler);
        return () => mediaQuery.removeEventListener('change', handler);
    }, [query]);

    return matches;
};

// Hook: Online Status
const useOnlineStatus = () => {
    const [isOnline, setIsOnline] = React.useState(navigator.onLine);

    React.useEffect(() => {
        const handleOnline = () => setIsOnline(true);
        const handleOffline = () => setIsOnline(false);

        window.addEventListener('online', handleOnline);
        window.addEventListener('offline', handleOffline);

        return () => {
            window.removeEventListener('online', handleOnline);
            window.removeEventListener('offline', handleOffline);
        };
    }, []);

    return isOnline;
};

// Hook: Async with Abort
const useAsync = (asyncFunction, immediate = true) => {
    const [status, setStatus] = React.useState('idle');
    const [value, setValue] = React.useState(null);
    const [error, setError] = React.useState(null);

    const execute = React.useCallback(async (...args) => {
        setStatus('pending');
        setValue(null);
        setError(null);

        try {
            const response = await asyncFunction(...args);
            setValue(response);
            setStatus('success');
        } catch (error) {
            setError(error);
            setStatus('error');
        }
    }, [asyncFunction]);

    React.useEffect(() => {
        if (immediate) {
            execute();
        }
    }, [execute, immediate]);

    return { execute, status, value, error };
};

// ============================================================
// 3. PORTAL PATTERN - Modal
// ============================================================

const Modal = ({ isOpen, onClose, children }) => {
    const modalRoot = document.getElementById('modal-root') || document.body;

    React.useEffect(() => {
        if (isOpen) {
            document.body.style.overflow = 'hidden';
        } else {
            document.body.style.overflow = 'unset';
        }

        return () => {
            document.body.style.overflow = 'unset';
        };
    }, [isOpen]);

    if (!isOpen) return null;

    return ReactDOM.createPortal(
        <div
            className="modal fade show d-block"
            tabIndex="-1"
            style={{ backgroundColor: 'rgba(0,0,0,0.5)' }}
            onClick={onClose}
        >
            <div
                className="modal-dialog modal-dialog-centered"
                onClick={(e) => e.stopPropagation()}
            >
                <div className="modal-content">
                    {children}
                </div>
            </div>
        </div>,
        modalRoot
    );
};

// ============================================================
// 4. LAZY LOADING EXAMPLE
// ============================================================

const LazyComponent = React.lazy(() => {
    return new Promise(resolve => {
        setTimeout(() => {
            resolve({
                default: () => (
                    <div className="alert alert-success">
                        <h5>Lazy Loaded Component!</h5>
                        <p>This component was loaded on demand using React.lazy and Suspense.</p>
                    </div>
                )
            });
        }, 1000);
    });
});

// ============================================================
// 5. INFINITE SCROLL EXAMPLE
// ============================================================

const InfiniteScrollList = () => {
    const [items, setItems] = React.useState(
        Array.from({ length: 20 }, (_, i) => `Item ${i + 1}`)
    );
    const [page, setPage] = React.useState(1);
    const [isLoading, setIsLoading] = React.useState(false);

    const [bottomRef, entry] = useIntersectionObserver({
        threshold: 0.1
    });

    React.useEffect(() => {
        if (entry?.isIntersecting && !isLoading) {
            setIsLoading(true);
            setTimeout(() => {
                setItems(prev => [
                    ...prev,
                    ...Array.from({ length: 20 }, (_, i) => 
                        `Item ${prev.length + i + 1}`
                    )
                ]);
                setPage(p => p + 1);
                setIsLoading(false);
            }, 1000);
        }
    }, [entry?.isIntersecting, isLoading]);

    return (
        <div className="card mb-3">
            <div className="card-header">
                <h5>Infinite Scroll (Intersection Observer)</h5>
            </div>
            <div className="card-body">
                <div style={{ maxHeight: '400px', overflowY: 'auto' }}>
                    <ul className="list-group">
                        {items.map((item, index) => (
                            <li key={index} className="list-group-item">
                                {item}
                            </li>
                        ))}
                    </ul>
                    <div ref={bottomRef} style={{ height: '20px', margin: '10px 0' }}>
                        {isLoading && (
                            <div className="text-center">
                                <div className="spinner-border spinner-border-sm" role="status">
                                    <span className="visually-hidden">Loading...</span>
                                </div>
                            </div>
                        )}
                    </div>
                </div>
                <small className="text-muted">Page: {page}</small>
            </div>
        </div>
    );
};

// ============================================================
// 6. RESPONSIVE COMPONENT
// ============================================================

const ResponsiveComponent = () => {
    const size = useWindowSize();
    const isMobile = useMediaQuery('(max-width: 768px)');
    const isOnline = useOnlineStatus();

    return (
        <div className="card mb-3">
            <div className="card-header">
                <h5>Responsive & Network Detection</h5>
            </div>
            <div className="card-body">
                <p><strong>Window Size:</strong> {size.width} x {size.height}</p>
                <p><strong>Device:</strong> {isMobile ? '?? Mobile' : '?? Desktop'}</p>
                <p>
                    <strong>Network Status:</strong>{' '}
                    <span className={`badge ${isOnline ? 'bg-success' : 'bg-danger'}`}>
                        {isOnline ? '?? Online' : '?? Offline'}
                    </span>
                </p>
            </div>
        </div>
    );
};

// ============================================================
// 7. FORM WITH VALIDATION
// ============================================================

const AdvancedForm = () => {
    const [formData, setFormData] = React.useState({
        username: '',
        email: '',
        password: '',
        confirmPassword: ''
    });

    const [errors, setErrors] = React.useState({});
    const [touched, setTouched] = React.useState({});

    const validate = (name, value) => {
        switch (name) {
            case 'username':
                return value.length < 3 ? 'Username must be at least 3 characters' : '';
            case 'email':
                return !/\S+@\S+\.\S+/.test(value) ? 'Email is invalid' : '';
            case 'password':
                return value.length < 6 ? 'Password must be at least 6 characters' : '';
            case 'confirmPassword':
                return value !== formData.password ? 'Passwords do not match' : '';
            default:
                return '';
        }
    };

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({ ...prev, [name]: value }));
        
        if (touched[name]) {
            setErrors(prev => ({ ...prev, [name]: validate(name, value) }));
        }
    };

    const handleBlur = (e) => {
        const { name, value } = e.target;
        setTouched(prev => ({ ...prev, [name]: true }));
        setErrors(prev => ({ ...prev, [name]: validate(name, value) }));
    };

    const handleSubmit = (e) => {
        e.preventDefault();
        
        const newErrors = {};
        Object.keys(formData).forEach(key => {
            const error = validate(key, formData[key]);
            if (error) newErrors[key] = error;
        });

        setErrors(newErrors);
        setTouched({
            username: true,
            email: true,
            password: true,
            confirmPassword: true
        });

        if (Object.keys(newErrors).length === 0) {
            alert('Form submitted successfully!');
        }
    };

    return (
        <div className="card mb-3">
            <div className="card-header">
                <h5>Advanced Form with Validation</h5>
            </div>
            <div className="card-body">
                <form onSubmit={handleSubmit}>
                    <div className="mb-3">
                        <label className="form-label">Username</label>
                        <input
                            type="text"
                            name="username"
                            className={`form-control ${touched.username && errors.username ? 'is-invalid' : ''}`}
                            value={formData.username}
                            onChange={handleChange}
                            onBlur={handleBlur}
                        />
                        {touched.username && errors.username && (
                            <div className="invalid-feedback">{errors.username}</div>
                        )}
                    </div>

                    <div className="mb-3">
                        <label className="form-label">Email</label>
                        <input
                            type="email"
                            name="email"
                            className={`form-control ${touched.email && errors.email ? 'is-invalid' : ''}`}
                            value={formData.email}
                            onChange={handleChange}
                            onBlur={handleBlur}
                        />
                        {touched.email && errors.email && (
                            <div className="invalid-feedback">{errors.email}</div>
                        )}
                    </div>

                    <div className="mb-3">
                        <label className="form-label">Password</label>
                        <input
                            type="password"
                            name="password"
                            className={`form-control ${touched.password && errors.password ? 'is-invalid' : ''}`}
                            value={formData.password}
                            onChange={handleChange}
                            onBlur={handleBlur}
                        />
                        {touched.password && errors.password && (
                            <div className="invalid-feedback">{errors.password}</div>
                        )}
                    </div>

                    <div className="mb-3">
                        <label className="form-label">Confirm Password</label>
                        <input
                            type="password"
                            name="confirmPassword"
                            className={`form-control ${touched.confirmPassword && errors.confirmPassword ? 'is-invalid' : ''}`}
                            value={formData.confirmPassword}
                            onChange={handleChange}
                            onBlur={handleBlur}
                        />
                        {touched.confirmPassword && errors.confirmPassword && (
                            <div className="invalid-feedback">{errors.confirmPassword}</div>
                        )}
                    </div>

                    <button type="submit" className="btn btn-primary">Submit</button>
                </form>
            </div>
        </div>
    );
};

// ============================================================
// 8. MODAL EXAMPLE
// ============================================================

const ModalExample = () => {
    const [isModalOpen, setIsModalOpen] = React.useState(false);

    return (
        <div className="card mb-3">
            <div className="card-header">
                <h5>Portal Pattern - Modal</h5>
            </div>
            <div className="card-body">
                <button
                    className="btn btn-primary"
                    onClick={() => setIsModalOpen(true)}
                >
                    Open Modal
                </button>

                <Modal isOpen={isModalOpen} onClose={() => setIsModalOpen(false)}>
                    <div className="modal-header">
                        <h5 className="modal-title">Modal Title</h5>
                        <button
                            type="button"
                            className="btn-close"
                            onClick={() => setIsModalOpen(false)}
                        ></button>
                    </div>
                    <div className="modal-body">
                        <p>This modal is rendered using ReactDOM.createPortal!</p>
                        <p>It's mounted outside the normal DOM hierarchy.</p>
                    </div>
                    <div className="modal-footer">
                        <button
                            type="button"
                            className="btn btn-secondary"
                            onClick={() => setIsModalOpen(false)}
                        >
                            Close
                        </button>
                    </div>
                </Modal>
            </div>
        </div>
    );
};

// ============================================================
// MAIN ADVANCED APP
// ============================================================

const AdvancedApp = () => {
    const [showLazy, setShowLazy] = React.useState(false);

    return (
        <ErrorBoundary>
            <div className="container my-4">
                <div className="text-center mb-4">
                    <h1 className="display-4">? Advanced React Patterns</h1>
                    <p className="lead">Production-Ready Techniques</p>
                </div>

                <div className="row">
                    <div className="col-12">
                        {/* Responsive Component */}
                        <ResponsiveComponent />

                        {/* Infinite Scroll */}
                        <InfiniteScrollList />

                        {/* Advanced Form */}
                        <AdvancedForm />

                        {/* Modal Example */}
                        <ModalExample />

                        {/* Lazy Loading */}
                        <div className="card mb-3">
                            <div className="card-header">
                                <h5>Code Splitting & Lazy Loading</h5>
                            </div>
                            <div className="card-body">
                                <button
                                    className="btn btn-info"
                                    onClick={() => setShowLazy(!showLazy)}
                                >
                                    {showLazy ? 'Hide' : 'Load'} Lazy Component
                                </button>
                                {showLazy && (
                                    <React.Suspense fallback={
                                        <div className="text-center my-3">
                                            <div className="spinner-border" role="status">
                                                <span className="visually-hidden">Loading...</span>
                                            </div>
                                        </div>
                                    }>
                                        <LazyComponent />
                                    </React.Suspense>
                                )}
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </ErrorBoundary>
    );
};

// Render the advanced app
const rootAdvanced = ReactDOM.createRoot(document.getElementById('root-advanced'));
rootAdvanced.render(<AdvancedApp />);
