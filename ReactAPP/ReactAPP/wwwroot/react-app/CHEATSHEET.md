# React Interview Cheat Sheet ??

## Hooks Quick Reference

### useState
```javascript
const [state, setState] = useState(initialValue);
setState(newValue);
setState(prev => prev + 1); // functional update
```

### useEffect
```javascript
// Runs on every render
useEffect(() => { /* side effect */ });

// Runs once on mount
useEffect(() => { /* side effect */ }, []);

// Runs when dependencies change
useEffect(() => { /* side effect */ }, [dep1, dep2]);

// Cleanup
useEffect(() => {
  const subscription = subscribe();
  return () => subscription.unsubscribe(); // cleanup
}, []);
```

### useContext
```javascript
const MyContext = createContext(defaultValue);
const value = useContext(MyContext);
```

### useReducer
```javascript
const reducer = (state, action) => {
  switch (action.type) {
    case 'INCREMENT': return { count: state.count + 1 };
    default: return state;
  }
};
const [state, dispatch] = useReducer(reducer, initialState);
dispatch({ type: 'INCREMENT' });
```

### useMemo
```javascript
const memoizedValue = useMemo(() => {
  return expensiveCalculation(a, b);
}, [a, b]); // Only recalculates when a or b changes
```

### useCallback
```javascript
const memoizedCallback = useCallback(() => {
  doSomething(a, b);
}, [a, b]); // Function reference only changes when a or b changes
```

### useRef
```javascript
const inputRef = useRef(null);
<input ref={inputRef} />
inputRef.current.focus();

const countRef = useRef(0); // Persists across renders without causing re-render
```

## Common Patterns

### Custom Hook
```javascript
const useWindowSize = () => {
  const [size, setSize] = useState({ width: window.innerWidth, height: window.innerHeight });
  
  useEffect(() => {
    const handleResize = () => setSize({ width: window.innerWidth, height: window.innerHeight });
    window.addEventListener('resize', handleResize);
    return () => window.removeEventListener('resize', handleResize);
  }, []);
  
  return size;
};
```

### Context API
```javascript
// Create Context
const ThemeContext = createContext();

// Provider
<ThemeContext.Provider value={{ theme, setTheme }}>
  {children}
</ThemeContext.Provider>

// Consumer Hook
const useTheme = () => {
  const context = useContext(ThemeContext);
  if (!context) throw new Error('useTheme must be used within ThemeProvider');
  return context;
};
```

### Error Boundary
```javascript
class ErrorBoundary extends React.Component {
  state = { hasError: false };
  
  static getDerivedStateFromError(error) {
    return { hasError: true };
  }
  
  componentDidCatch(error, errorInfo) {
    console.error(error, errorInfo);
  }
  
  render() {
    if (this.state.hasError) return <h1>Something went wrong.</h1>;
    return this.props.children;
  }
}
```

### HOC (Higher-Order Component)
```javascript
const withLogger = (WrappedComponent) => {
  return (props) => {
    useEffect(() => {
      console.log('Component mounted');
    }, []);
    return <WrappedComponent {...props} />;
  };
};
```

### Render Props
```javascript
<DataFetcher url="/api/data" render={(data) => (
  <div>{data.name}</div>
)} />
```

### Portal
```javascript
ReactDOM.createPortal(
  <Modal />,
  document.getElementById('modal-root')
);
```

## Performance Optimization

### React.memo
```javascript
const MyComponent = React.memo(({ prop1, prop2 }) => {
  return <div>{prop1} {prop2}</div>;
});

// With custom comparison
const MyComponent = React.memo(Component, (prevProps, nextProps) => {
  return prevProps.id === nextProps.id; // true = don't re-render
});
```

### Code Splitting
```javascript
const LazyComponent = React.lazy(() => import('./Component'));

<Suspense fallback={<Loading />}>
  <LazyComponent />
</Suspense>
```

## Common Interview Answers

### Virtual DOM
"A lightweight copy of the real DOM that React uses to calculate the minimal set of changes needed to update the UI efficiently."

### Reconciliation
"The process React uses to compare the previous Virtual DOM tree with the new one to determine what changed and update only those parts in the real DOM."

### Keys in Lists
"Unique identifiers that help React identify which items in a list have changed, been added, or removed, enabling efficient updates."

### Lifting State Up
"Moving state to the closest common ancestor component to share state between multiple child components."

### Controlled vs Uncontrolled
- **Controlled**: Form data handled by React state (recommended)
- **Uncontrolled**: Form data handled by DOM (use refs)

### Context vs Redux
- **Context**: Simple state, infrequent updates (theme, auth, i18n)
- **Redux**: Complex state, frequent updates, middleware, dev tools

### useMemo vs useCallback
- **useMemo**: Memoize a **value** (result of calculation)
- **useCallback**: Memoize a **function** (function reference)

## Best Practices

### Do's ?
- Use functional components with hooks
- Keep components small and focused
- Use meaningful names
- Handle loading and error states
- Clean up subscriptions in useEffect
- Use keys for lists (unique, stable)
- Memoize expensive calculations
- Split code with React.lazy

### Don'ts ?
- Don't mutate state directly
- Don't call hooks conditionally
- Don't use index as key for dynamic lists
- Don't forget useEffect cleanup
- Don't over-optimize (measure first)
- Don't put functions in dependency arrays without useCallback
- Don't create components inside components

## Quick Debugging

### Common Issues
1. **Infinite loop in useEffect** ? Check dependency array
2. **Component not re-rendering** ? Make sure you're updating state correctly
3. **Stale closure** ? Include all dependencies in useEffect
4. **Memory leak warning** ? Add cleanup function in useEffect
5. **Props not updating** ? Check if parent is passing new reference

### Debug Tools
- React DevTools (Components & Profiler)
- console.log in render and useEffect
- debugger statements
- React.StrictMode for development warnings

## TypeScript Tips

```typescript
// Props
interface Props {
  name: string;
  age?: number;
  onClick: (id: number) => void;
}
const Component: React.FC<Props> = ({ name, age, onClick }) => { };

// State
const [user, setUser] = useState<User | null>(null);

// Ref
const inputRef = useRef<HTMLInputElement>(null);

// Custom Hook
function useLocalStorage<T>(key: string, initialValue: T): [T, (value: T) => void] {
  // implementation
}
```

## Testing Quick Reference

```javascript
// React Testing Library
import { render, screen, fireEvent } from '@testing-library/react';

test('button increments counter', () => {
  render(<Counter />);
  const button = screen.getByRole('button');
  fireEvent.click(button);
  expect(screen.getByText('Count: 1')).toBeInTheDocument();
});
```

## React 18 New Features

### useTransition
```javascript
const [isPending, startTransition] = useTransition();
startTransition(() => {
  setSearchQuery(value); // Non-urgent update
});
```

### useDeferredValue
```javascript
const deferredValue = useDeferredValue(searchQuery);
```

### Automatic Batching
Multiple setState calls are automatically batched even in promises and event handlers.

## Remember for Interviews

1. **Explain with examples** - Don't just define, demonstrate
2. **Mention trade-offs** - Every solution has pros and cons
3. **Think out loud** - Share your thought process
4. **Ask clarifying questions** - Understand requirements
5. **Consider edge cases** - Loading, error, empty states
6. **Discuss performance** - When and why to optimize
7. **Show best practices** - Demonstrate experience

## 30-Second Answers

**Q: What is React?**
A: A JavaScript library for building user interfaces using a component-based architecture. It uses a Virtual DOM for efficient updates and follows a unidirectional data flow.

**Q: Why React?**
A: Component reusability, efficient updates with Virtual DOM, large ecosystem, strong community, declarative syntax, and great developer experience.

**Q: Hooks benefits?**
A: Reuse stateful logic without HOCs or render props, simpler code, better code organization, and easier to test.

**Q: When to optimize?**
A: After measuring performance issues. Use React Profiler to identify slow components, then apply memoization strategically.

## Good Luck! ??

Remember: **Understanding > Memorization**
