// ============================================================
// REACT INTERVIEW QUESTIONS & ANSWERS WITH CODE EXAMPLES
// For Senior Developer Positions (13+ years experience)
// ============================================================

// ============================================================
// QUESTION 1: Explain the difference between useMemo and useCallback
// ============================================================

const UseMemoVsUseCallbackExample = () => {
    const [count, setCount] = React.useState(0);
    const [text, setText] = React.useState('');

    // useMemo: memoizes a VALUE (computed result)
    const expensiveValue = React.useMemo(() => {
        console.log('Computing expensive value...');
        return count * 2;
    }, [count]); // Only recomputes when count changes

    // useCallback: memoizes a FUNCTION
    const handleClick = React.useCallback(() => {
        console.log('Button clicked with count:', count);
    }, [count]); // Function reference only changes when count changes

    // Without useCallback, this would create a new function on every render
    const handleTextChange = React.useCallback((e) => {
        setText(e.target.value);
    }, []); // Empty deps = function never changes

    return (
        <div className="card mb-3">
            <div className="card-header">
                <h5>Q1: useMemo vs useCallback</h5>
            </div>
            <div className="card-body">
                <p><strong>useMemo Result:</strong> {expensiveValue}</p>
                <button className="btn btn-primary me-2" onClick={() => setCount(count + 1)}>
                    Increment Count ({count})
                </button>
                <button className="btn btn-secondary" onClick={handleClick}>
                    Log Count
                </button>
                <input
                    type="text"
                    className="form-control mt-2"
                    value={text}
                    onChange={handleTextChange}
                    placeholder="Type something"
                />
                <small className="text-muted d-block mt-2">
                    Check console - expensive value only recomputes when count changes
                </small>
            </div>
        </div>
    );
};

// ============================================================
// QUESTION 2: How do you prevent unnecessary re-renders?
// ============================================================

// Child component WITHOUT React.memo - will re-render on every parent update
const SlowChild = ({ count, onClick }) => {
    console.log('SlowChild rendered');
    return (
        <div className="alert alert-warning">
            <p>Count: {count}</p>
            <button className="btn btn-sm btn-primary" onClick={onClick}>Increment</button>
        </div>
    );
};

// Child component WITH React.memo - only re-renders when props change
const FastChild = React.memo(({ count, onClick }) => {
    console.log('FastChild rendered');
    return (
        <div className="alert alert-success">
            <p>Count: {count}</p>
            <button className="btn btn-sm btn-primary" onClick={onClick}>Increment</button>
        </div>
    );
});

const PreventRerendersExample = () => {
    const [count, setCount] = React.useState(0);
    const [text, setText] = React.useState('');

    // useCallback prevents onClick from changing on every render
    const handleIncrement = React.useCallback(() => {
        setCount(c => c + 1);
    }, []);

    return (
        <div className="card mb-3">
            <div className="card-header">
                <h5>Q2: Preventing Unnecessary Re-renders</h5>
            </div>
            <div className="card-body">
                <input
                    type="text"
                    className="form-control mb-3"
                    value={text}
                    onChange={(e) => setText(e.target.value)}
                    placeholder="Type to trigger parent re-render"
                />
                
                <h6>Without React.memo:</h6>
                <SlowChild count={count} onClick={handleIncrement} />
                
                <h6>With React.memo:</h6>
                <FastChild count={count} onClick={handleIncrement} />
                
                <small className="text-muted">
                    Type in the input - FastChild won't re-render, SlowChild will
                </small>
            </div>
        </div>
    );
};

// ============================================================
// QUESTION 3: Explain useEffect cleanup and dependency array
// ============================================================

const UseEffectCleanupExample = () => {
    const [count, setCount] = React.useState(0);
    const [isRunning, setIsRunning] = React.useState(false);

    // Effect with cleanup
    React.useEffect(() => {
        if (!isRunning) return;

        console.log('Timer started');
        const intervalId = setInterval(() => {
            setCount(c => c + 1);
        }, 1000);

        // Cleanup function - runs before next effect and on unmount
        return () => {
            console.log('Timer stopped');
            clearInterval(intervalId);
        };
    }, [isRunning]); // Only re-run when isRunning changes

    return (
        <div className="card mb-3">
            <div className="card-header">
                <h5>Q3: useEffect Cleanup & Dependencies</h5>
            </div>
            <div className="card-body">
                <p>Timer: {count} seconds</p>
                <button
                    className="btn btn-primary"
                    onClick={() => setIsRunning(!isRunning)}
                >
                    {isRunning ? 'Stop' : 'Start'}
                </button>
                <button
                    className="btn btn-secondary ms-2"
                    onClick={() => setCount(0)}
                >
                    Reset
                </button>
                <div className="alert alert-info mt-3">
                    <strong>Key Points:</strong>
                    <ul className="mb-0">
                        <li>Cleanup runs before effect re-executes</li>
                        <li>Cleanup runs on component unmount</li>
                        <li>Empty deps [] = runs once on mount</li>
                        <li>No deps = runs after every render</li>
                        <li>Deps array = runs when dependencies change</li>
                    </ul>
                </div>
            </div>
        </div>
    );
};

// ============================================================
// QUESTION 4: Implement a custom hook for API calls
// ============================================================

const useAPI = (url, options = {}) => {
    const [data, setData] = React.useState(null);
    const [loading, setLoading] = React.useState(false);
    const [error, setError] = React.useState(null);

    const fetchData = React.useCallback(async () => {
        setLoading(true);
        setError(null);
        
        try {
            const response = await fetch(url, options);
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            const json = await response.json();
            setData(json);
        } catch (err) {
            setError(err.message);
        } finally {
            setLoading(false);
        }
    }, [url]);

    React.useEffect(() => {
        fetchData();
    }, [fetchData]);

    return { data, loading, error, refetch: fetchData };
};

const CustomHookAPIExample = () => {
    const { data, loading, error, refetch } = useAPI(
        'https://jsonplaceholder.typicode.com/posts/1'
    );

    return (
        <div className="card mb-3">
            <div className="card-header">
                <h5>Q4: Custom Hook for API Calls</h5>
            </div>
            <div className="card-body">
                {loading && <div className="spinner-border" />}
                {error && <div className="alert alert-danger">{error}</div>}
                {data && (
                    <div>
                        <h6>{data.title}</h6>
                        <p>{data.body}</p>
                    </div>
                )}
                <button className="btn btn-primary" onClick={refetch}>
                    Refetch
                </button>
                <pre className="mt-3 p-3 bg-light">
{`const useAPI = (url, options = {}) => {
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const fetchData = useCallback(async () => {
    setLoading(true);
    try {
      const response = await fetch(url, options);
      const json = await response.json();
      setData(json);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }, [url]);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  return { data, loading, error, refetch: fetchData };
};`}
                </pre>
            </div>
        </div>
    );
};

// ============================================================
// QUESTION 5: Explain Context API and when to use it
// ============================================================

const AuthContext = React.createContext();

const AuthProvider = ({ children }) => {
    const [user, setUser] = React.useState(null);
    const [isAuthenticated, setIsAuthenticated] = React.useState(false);

    const login = React.useCallback((username) => {
        setUser({ username, id: Date.now() });
        setIsAuthenticated(true);
    }, []);

    const logout = React.useCallback(() => {
        setUser(null);
        setIsAuthenticated(false);
    }, []);

    const value = React.useMemo(() => ({
        user,
        isAuthenticated,
        login,
        logout
    }), [user, isAuthenticated, login, logout]);

    return (
        <AuthContext.Provider value={value}>
            {children}
        </AuthContext.Provider>
    );
};

const useAuth = () => {
    const context = React.useContext(AuthContext);
    if (!context) {
        throw new Error('useAuth must be used within AuthProvider');
    }
    return context;
};

const UserProfile = () => {
    const { user, logout } = useAuth();
    return (
        <div className="alert alert-success">
            <p>Welcome, {user.username}!</p>
            <button className="btn btn-sm btn-danger" onClick={logout}>
                Logout
            </button>
        </div>
    );
};

const LoginForm = () => {
    const [username, setUsername] = React.useState('');
    const { login } = useAuth();

    const handleSubmit = (e) => {
        e.preventDefault();
        if (username.trim()) {
            login(username);
        }
    };

    return (
        <form onSubmit={handleSubmit} className="input-group">
            <input
                type="text"
                className="form-control"
                value={username}
                onChange={(e) => setUsername(e.target.value)}
                placeholder="Enter username"
            />
            <button className="btn btn-primary" type="submit">Login</button>
        </form>
    );
};

const ContextAPIExample = () => {
    return (
        <AuthProvider>
            <div className="card mb-3">
                <div className="card-header">
                    <h5>Q5: Context API for State Management</h5>
                </div>
                <div className="card-body">
                    <AuthContextContent />
                </div>
            </div>
        </AuthProvider>
    );
};

const AuthContextContent = () => {
    const { isAuthenticated } = useAuth();
    
    return (
        <>
            {isAuthenticated ? <UserProfile /> : <LoginForm />}
            <div className="alert alert-info mt-3">
                <strong>When to use Context API:</strong>
                <ul className="mb-0">
                    <li>Theme management (light/dark mode)</li>
                    <li>User authentication state</li>
                    <li>Language/localization</li>
                    <li>Avoid prop drilling (passing props through many levels)</li>
                </ul>
                <strong>When NOT to use:</strong>
                <ul className="mb-0">
                    <li>Frequently changing data (can cause performance issues)</li>
                    <li>Complex state logic (consider Redux/Zustand)</li>
                </ul>
            </div>
        </>
    );
};

// ============================================================
// QUESTION 6: What is the difference between controlled and uncontrolled?
// ============================================================

const ControlledVsUncontrolledExample = () => {
    // Controlled
    const [controlled, setControlled] = React.useState('');
    
    // Uncontrolled
    const uncontrolledRef = React.useRef(null);

    const handleSubmit = () => {
        alert(
            `Controlled: ${controlled}\n` +
            `Uncontrolled: ${uncontrolledRef.current.value}`
        );
    };

    return (
        <div className="card mb-3">
            <div className="card-header">
                <h5>Q6: Controlled vs Uncontrolled Components</h5>
            </div>
            <div className="card-body">
                <div className="mb-3">
                    <label className="form-label">Controlled Input:</label>
                    <input
                        type="text"
                        className="form-control"
                        value={controlled}
                        onChange={(e) => setControlled(e.target.value)}
                    />
                    <small>Current value: {controlled}</small>
                </div>
                
                <div className="mb-3">
                    <label className="form-label">Uncontrolled Input:</label>
                    <input
                        type="text"
                        className="form-control"
                        ref={uncontrolledRef}
                        defaultValue="Initial value"
                    />
                    <small>Value not tracked in React state</small>
                </div>

                <button className="btn btn-primary" onClick={handleSubmit}>
                    Show Values
                </button>

                <div className="alert alert-info mt-3">
                    <strong>Controlled:</strong> React state is source of truth<br />
                    <strong>Uncontrolled:</strong> DOM is source of truth (use refs)<br />
                    <strong>Recommendation:</strong> Use controlled for most cases
                </div>
            </div>
        </div>
    );
};

// ============================================================
// MAIN INTERVIEW APP
// ============================================================

const InterviewQuestionsApp = () => {
    return (
        <div className="container my-4">
            <div className="text-center mb-4">
                <h1 className="display-4">?? React Interview Questions</h1>
                <p className="lead">Common questions with working examples</p>
            </div>

            <UseMemoVsUseCallbackExample />
            <PreventRerendersExample />
            <UseEffectCleanupExample />
            <CustomHookAPIExample />
            <ContextAPIExample />
            <ControlledVsUncontrolledExample />

            <div className="card bg-light">
                <div className="card-body">
                    <h5>?? Additional Common Questions</h5>
                    <ol>
                        <li><strong>What is Virtual DOM?</strong> A lightweight copy of the real DOM that React uses to optimize updates</li>
                        <li><strong>Explain reconciliation:</strong> Process of comparing Virtual DOM trees to determine minimal changes</li>
                        <li><strong>What are keys in lists?</strong> Unique identifiers that help React identify which items changed</li>
                        <li><strong>Lifting state up:</strong> Moving state to common ancestor to share between components</li>
                        <li><strong>React.StrictMode:</strong> Development mode tool to identify potential problems</li>
                        <li><strong>Error Boundaries:</strong> Components that catch JavaScript errors in child component tree</li>
                        <li><strong>Code Splitting:</strong> Using React.lazy and Suspense to split code into smaller chunks</li>
                        <li><strong>Portals:</strong> Render children into DOM node outside parent hierarchy</li>
                    </ol>
                </div>
            </div>
        </div>
    );
};

// Render
const rootInterview = ReactDOM.createRoot(document.getElementById('root-interview'));
rootInterview.render(<InterviewQuestionsApp />);
