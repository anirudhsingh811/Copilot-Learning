// ============================================================
// REACT ADVANCED LEARNING APP FOR SENIOR DEVELOPERS
// Topics: Hooks, Context, Custom Hooks, Performance, Patterns
// ============================================================

// ============================================================
// 1. CONTEXT API - Global State Management
// ============================================================
const ThemeContext = React.createContext();
const UserContext = React.createContext();

// Theme Provider with useReducer for complex state
const themeReducer = (state, action) => {
    switch (action.type) {
        case 'TOGGLE_THEME':
            return { ...state, mode: state.mode === 'light' ? 'dark' : 'light' };
        case 'SET_PRIMARY_COLOR':
            return { ...state, primaryColor: action.payload };
        default:
            return state;
    }
};

const ThemeProvider = ({ children }) => {
    const [themeState, dispatch] = React.useReducer(themeReducer, {
        mode: 'light',
        primaryColor: '#007bff'
    });

    const value = React.useMemo(() => ({
        theme: themeState,
        toggleTheme: () => dispatch({ type: 'TOGGLE_THEME' }),
        setPrimaryColor: (color) => dispatch({ type: 'SET_PRIMARY_COLOR', payload: color })
    }), [themeState]);

    return (
        <ThemeContext.Provider value={value}>
            {children}
        </ThemeContext.Provider>
    );
};

// Custom Hook for Theme
const useTheme = () => {
    const context = React.useContext(ThemeContext);
    if (!context) {
        throw new Error('useTheme must be used within ThemeProvider');
    }
    return context;
};

// ============================================================
// 2. CUSTOM HOOKS - Reusable Logic
// ============================================================

// Custom Hook: Fetch Data with Loading and Error states
const useFetch = (url, options = {}) => {
    const [data, setData] = React.useState(null);
    const [loading, setLoading] = React.useState(true);
    const [error, setError] = React.useState(null);

    React.useEffect(() => {
        let isMounted = true;
        const abortController = new AbortController();

        const fetchData = async () => {
            try {
                setLoading(true);
                const response = await fetch(url, {
                    ...options,
                    signal: abortController.signal
                });
                const result = await response.json();
                
                if (isMounted) {
                    setData(result);
                    setError(null);
                }
            } catch (err) {
                if (isMounted && err.name !== 'AbortError') {
                    setError(err.message);
                }
            } finally {
                if (isMounted) {
                    setLoading(false);
                }
            }
        };

        fetchData();

        return () => {
            isMounted = false;
            abortController.abort();
        };
    }, [url]);

    return { data, loading, error };
};

// Custom Hook: Local Storage with sync
const useLocalStorage = (key, initialValue) => {
    const [storedValue, setStoredValue] = React.useState(() => {
        try {
            const item = window.localStorage.getItem(key);
            return item ? JSON.parse(item) : initialValue;
        } catch (error) {
            console.error(error);
            return initialValue;
        }
    });

    const setValue = React.useCallback((value) => {
        try {
            const valueToStore = value instanceof Function ? value(storedValue) : value;
            setStoredValue(valueToStore);
            window.localStorage.setItem(key, JSON.stringify(valueToStore));
        } catch (error) {
            console.error(error);
        }
    }, [key, storedValue]);

    return [storedValue, setValue];
};

// Custom Hook: Debounce
const useDebounce = (value, delay) => {
    const [debouncedValue, setDebouncedValue] = React.useState(value);

    React.useEffect(() => {
        const handler = setTimeout(() => {
            setDebouncedValue(value);
        }, delay);

        return () => {
            clearTimeout(handler);
        };
    }, [value, delay]);

    return debouncedValue;
};

// Custom Hook: Previous Value
const usePrevious = (value) => {
    const ref = React.useRef();
    React.useEffect(() => {
        ref.current = value;
    }, [value]);
    return ref.current;
};

// ============================================================
// 3. PERFORMANCE OPTIMIZATION - useMemo, useCallback, React.memo
// ============================================================

// Expensive calculation simulation
const ExpensiveComponent = React.memo(({ count, onIncrement }) => {
    const expensiveCalculation = React.useMemo(() => {
        console.log('Running expensive calculation...');
        let result = 0;
        for (let i = 0; i < 1000000; i++) {
            result += i;
        }
        return result + count;
    }, [count]);

    return (
        <div className="card mb-3">
            <div className="card-header">
                <h5>Performance Optimization (useMemo & React.memo)</h5>
            </div>
            <div className="card-body">
                <p><strong>Expensive Calculation Result:</strong> {expensiveCalculation}</p>
                <p><strong>Count:</strong> {count}</p>
                <button className="btn btn-primary" onClick={onIncrement}>
                    Increment Count
                </button>
                <small className="d-block mt-2 text-muted">
                    Check console - expensive calculation only runs when count changes
                </small>
            </div>
        </div>
    );
});

// ============================================================
// 4. COMPOUND COMPONENTS PATTERN
// ============================================================

const Accordion = ({ children }) => {
    const [openIndex, setOpenIndex] = React.useState(null);

    const value = React.useMemo(() => ({
        openIndex,
        toggleItem: (index) => setOpenIndex(openIndex === index ? null : index)
    }), [openIndex]);

    return (
        <div className="accordion" id="accordionExample">
            {React.Children.map(children, (child, index) =>
                React.cloneElement(child, { index })
            )}
        </div>
    );
};

const AccordionItem = ({ index, title, children }) => {
    const { openIndex, toggleItem } = React.useContext(React.createContext());
    const isOpen = openIndex === index;

    return (
        <div className="accordion-item">
            <h2 className="accordion-header">
                <button
                    className={`accordion-button ${!isOpen ? 'collapsed' : ''}`}
                    type="button"
                    onClick={() => toggleItem(index)}
                >
                    {title}
                </button>
            </h2>
            <div className={`accordion-collapse collapse ${isOpen ? 'show' : ''}`}>
                <div className="accordion-body">{children}</div>
            </div>
        </div>
    );
};

// ============================================================
// 5. HIGHER-ORDER COMPONENT (HOC) PATTERN
// ============================================================

const withLogger = (WrappedComponent, componentName) => {
    return (props) => {
        React.useEffect(() => {
            console.log(`${componentName} mounted`);
            return () => console.log(`${componentName} unmounted`);
        }, []);

        return <WrappedComponent {...props} />;
    };
};

// ============================================================
// 6. RENDER PROPS PATTERN
// ============================================================

const MouseTracker = ({ render }) => {
    const [position, setPosition] = React.useState({ x: 0, y: 0 });

    const handleMouseMove = React.useCallback((event) => {
        setPosition({ x: event.clientX, y: event.clientY });
    }, []);

    React.useEffect(() => {
        window.addEventListener('mousemove', handleMouseMove);
        return () => window.removeEventListener('mousemove', handleMouseMove);
    }, [handleMouseMove]);

    return render(position);
};

// ============================================================
// 7. CONTROLLED VS UNCONTROLLED COMPONENTS
// ============================================================

const FormExamples = () => {
    // Controlled component
    const [controlledValue, setControlledValue] = React.useState('');
    
    // Uncontrolled component
    const uncontrolledRef = React.useRef(null);

    const handleSubmit = (e) => {
        e.preventDefault();
        alert(`Controlled: ${controlledValue}\nUncontrolled: ${uncontrolledRef.current.value}`);
    };

    return (
        <div className="card mb-3">
            <div className="card-header">
                <h5>Controlled vs Uncontrolled Components</h5>
            </div>
            <div className="card-body">
                <form onSubmit={handleSubmit}>
                    <div className="mb-3">
                        <label className="form-label">Controlled Input:</label>
                        <input
                            type="text"
                            className="form-control"
                            value={controlledValue}
                            onChange={(e) => setControlledValue(e.target.value)}
                            placeholder="Type here (controlled)"
                        />
                        <small className="text-muted">Value: {controlledValue}</small>
                    </div>
                    
                    <div className="mb-3">
                        <label className="form-label">Uncontrolled Input:</label>
                        <input
                            type="text"
                            className="form-control"
                            ref={uncontrolledRef}
                            defaultValue="Default value"
                            placeholder="Type here (uncontrolled)"
                        />
                    </div>
                    
                    <button type="submit" className="btn btn-success">Submit</button>
                </form>
            </div>
        </div>
    );
};

// ============================================================
// 8. TODO APP - COMPREHENSIVE EXAMPLE
// ============================================================

const TodoApp = () => {
    const [todos, setTodos] = useLocalStorage('todos', []);
    const [inputValue, setInputValue] = React.useState('');
    const [filter, setFilter] = React.useState('all'); // all, active, completed
    
    const debouncedInput = useDebounce(inputValue, 300);

    const addTodo = React.useCallback(() => {
        if (inputValue.trim()) {
            setTodos([
                ...todos,
                {
                    id: Date.now(),
                    text: inputValue,
                    completed: false,
                    createdAt: new Date().toISOString()
                }
            ]);
            setInputValue('');
        }
    }, [inputValue, todos, setTodos]);

    const toggleTodo = React.useCallback((id) => {
        setTodos(todos.map(todo =>
            todo.id === id ? { ...todo, completed: !todo.completed } : todo
        ));
    }, [todos, setTodos]);

    const deleteTodo = React.useCallback((id) => {
        setTodos(todos.filter(todo => todo.id !== id));
    }, [todos, setTodos]);

    const filteredTodos = React.useMemo(() => {
        switch (filter) {
            case 'active':
                return todos.filter(todo => !todo.completed);
            case 'completed':
                return todos.filter(todo => todo.completed);
            default:
                return todos;
        }
    }, [todos, filter]);

    const stats = React.useMemo(() => ({
        total: todos.length,
        active: todos.filter(t => !t.completed).length,
        completed: todos.filter(t => t.completed).length
    }), [todos]);

    return (
        <div className="card mb-3">
            <div className="card-header">
                <h5>Advanced Todo App (Hooks + LocalStorage)</h5>
            </div>
            <div className="card-body">
                <div className="input-group mb-3">
                    <input
                        type="text"
                        className="form-control"
                        value={inputValue}
                        onChange={(e) => setInputValue(e.target.value)}
                        onKeyPress={(e) => e.key === 'Enter' && addTodo()}
                        placeholder="Add a new todo..."
                    />
                    <button className="btn btn-primary" onClick={addTodo}>Add</button>
                </div>

                <div className="btn-group mb-3" role="group">
                    <button
                        className={`btn btn-outline-secondary ${filter === 'all' ? 'active' : ''}`}
                        onClick={() => setFilter('all')}
                    >
                        All ({stats.total})
                    </button>
                    <button
                        className={`btn btn-outline-secondary ${filter === 'active' ? 'active' : ''}`}
                        onClick={() => setFilter('active')}
                    >
                        Active ({stats.active})
                    </button>
                    <button
                        className={`btn btn-outline-secondary ${filter === 'completed' ? 'active' : ''}`}
                        onClick={() => setFilter('completed')}
                    >
                        Completed ({stats.completed})
                    </button>
                </div>

                <ul className="list-group">
                    {filteredTodos.map(todo => (
                        <li
                            key={todo.id}
                            className="list-group-item d-flex justify-content-between align-items-center"
                        >
                            <div className="form-check">
                                <input
                                    className="form-check-input"
                                    type="checkbox"
                                    checked={todo.completed}
                                    onChange={() => toggleTodo(todo.id)}
                                />
                                <label
                                    className={`form-check-label ${todo.completed ? 'text-decoration-line-through' : ''}`}
                                >
                                    {todo.text}
                                </label>
                            </div>
                            <button
                                className="btn btn-danger btn-sm"
                                onClick={() => deleteTodo(todo.id)}
                            >
                                Delete
                            </button>
                        </li>
                    ))}
                </ul>

                {debouncedInput && (
                    <small className="text-muted d-block mt-2">
                        Debounced search value: {debouncedInput}
                    </small>
                )}
            </div>
        </div>
    );
};

// ============================================================
// 9. ASYNC DATA FETCHING
// ============================================================

const UsersList = () => {
    const { data, loading, error } = useFetch('https://jsonplaceholder.typicode.com/users');

    if (loading) return <div className="alert alert-info">Loading users...</div>;
    if (error) return <div className="alert alert-danger">Error: {error}</div>;

    return (
        <div className="card mb-3">
            <div className="card-header">
                <h5>Async Data Fetching (Custom Hook)</h5>
            </div>
            <div className="card-body">
                <div className="table-responsive">
                    <table className="table table-striped">
                        <thead>
                            <tr>
                                <th>ID</th>
                                <th>Name</th>
                                <th>Email</th>
                                <th>Company</th>
                            </tr>
                        </thead>
                        <tbody>
                            {data?.slice(0, 5).map(user => (
                                <tr key={user.id}>
                                    <td>{user.id}</td>
                                    <td>{user.name}</td>
                                    <td>{user.email}</td>
                                    <td>{user.company.name}</td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    );
};

// ============================================================
// 10. THEME TOGGLE COMPONENT
// ============================================================

const ThemeToggle = () => {
    const { theme, toggleTheme } = useTheme();

    return (
        <div className="card mb-3">
            <div className="card-header">
                <h5>Theme Context Example</h5>
            </div>
            <div className="card-body">
                <p>Current Theme: <strong>{theme.mode}</strong></p>
                <p>Primary Color: <strong>{theme.primaryColor}</strong></p>
                <button className="btn btn-secondary" onClick={toggleTheme}>
                    Toggle Theme
                </button>
            </div>
        </div>
    );
};

// ============================================================
// MAIN APP COMPONENT
// ============================================================

const MainApp = () => {
    const [count, setCount] = React.useState(0);
    const [showMouseTracker, setShowMouseTracker] = React.useState(false);
    const previousCount = usePrevious(count);

    return (
        <ThemeProvider>
            <div className="container my-4">
                <div className="text-center mb-4">
                    <h1 className="display-4">?? React Advanced Learning</h1>
                    <p className="lead">Comprehensive examples for Senior Developers</p>
                    <div className="alert alert-info">
                        <strong>Topics Covered:</strong> Hooks, Context API, Custom Hooks, 
                        Performance Optimization, Design Patterns (HOC, Render Props, Compound Components)
                    </div>
                </div>

                <div className="row">
                    <div className="col-12">
                        {/* Theme Context */}
                        <ThemeToggle />

                        {/* Performance Optimization */}
                        <ExpensiveComponent
                            count={count}
                            onIncrement={() => setCount(count + 1)}
                        />
                        <div className="card mb-3">
                            <div className="card-body">
                                <p>Previous Count: {previousCount ?? 'N/A'} (usePrevious hook)</p>
                            </div>
                        </div>

                        {/* Todo App */}
                        <TodoApp />

                        {/* Form Examples */}
                        <FormExamples />

                        {/* Async Data */}
                        <UsersList />

                        {/* Render Props Pattern */}
                        <div className="card mb-3">
                            <div className="card-header">
                                <h5>Render Props Pattern</h5>
                            </div>
                            <div className="card-body">
                                <button
                                    className="btn btn-info mb-2"
                                    onClick={() => setShowMouseTracker(!showMouseTracker)}
                                >
                                    {showMouseTracker ? 'Hide' : 'Show'} Mouse Tracker
                                </button>
                                {showMouseTracker && (
                                    <MouseTracker
                                        render={(position) => (
                                            <div className="alert alert-success">
                                                Mouse position: X: {position.x}, Y: {position.y}
                                            </div>
                                        )}
                                    />
                                )}
                            </div>
                        </div>

                        {/* Interview Tips */}
                        <div className="card bg-light">
                            <div className="card-header">
                                <h5>?? Key Interview Topics</h5>
                            </div>
                            <div className="card-body">
                                <ol>
                                    <li><strong>Hooks:</strong> useState, useEffect, useContext, useReducer, useMemo, useCallback, useRef</li>
                                    <li><strong>Performance:</strong> React.memo, useMemo, useCallback, code splitting, lazy loading</li>
                                    <li><strong>State Management:</strong> Context API, Redux, Zustand, Recoil</li>
                                    <li><strong>Patterns:</strong> HOC, Render Props, Compound Components, Custom Hooks</li>
                                    <li><strong>Lifecycle:</strong> useEffect cleanup, dependency arrays, avoiding infinite loops</li>
                                    <li><strong>Best Practices:</strong> Component composition, prop drilling solutions, error boundaries</li>
                                    <li><strong>Testing:</strong> Jest, React Testing Library, unit vs integration tests</li>
                                    <li><strong>TypeScript:</strong> Typing props, hooks, events, generics</li>
                                </ol>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </ThemeProvider>
    );
};

// Render the app
const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(<MainApp />);
