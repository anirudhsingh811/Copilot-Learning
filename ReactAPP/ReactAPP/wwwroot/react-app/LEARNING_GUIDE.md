# React Learning Guide for Senior Developers

## ?? Complete Learning Path

### 1. **Core Concepts Covered**

#### Hooks Deep Dive
- **useState**: Managing component state
- **useEffect**: Side effects, cleanup, dependencies
- **useContext**: Consuming context without prop drilling
- **useReducer**: Complex state logic
- **useMemo**: Memoizing expensive calculations
- **useCallback**: Memoizing functions to prevent re-renders
- **useRef**: Accessing DOM elements and persisting values

#### Custom Hooks
- `useFetch`: Reusable data fetching with loading/error states
- `useLocalStorage`: Syncing state with localStorage
- `useDebounce`: Debouncing input values
- `usePrevious`: Tracking previous values

### 2. **Design Patterns**

#### Higher-Order Components (HOC)
```javascript
const withLogger = (WrappedComponent) => {
    return (props) => {
        // Add logging logic
        return <WrappedComponent {...props} />;
    };
};
```

#### Render Props
```javascript
<MouseTracker render={(position) => (
    <div>X: {position.x}, Y: {position.y}</div>
)} />
```

#### Compound Components
```javascript
<Accordion>
    <AccordionItem title="Title">Content</AccordionItem>
</Accordion>
```

### 3. **State Management**

- Context API with useReducer for global state
- Provider pattern
- Custom hooks for consuming context

### 4. **Performance Optimization**

- `React.memo`: Prevent unnecessary re-renders
- `useMemo`: Memoize expensive calculations
- `useCallback`: Memoize callback functions
- Proper dependency arrays in useEffect

### 5. **Best Practices**

#### Component Organization
```
src/
  components/
    common/       # Reusable components
    features/     # Feature-specific components
  hooks/          # Custom hooks
  context/        # Context providers
  utils/          # Helper functions
```

#### Error Boundaries
```javascript
class ErrorBoundary extends React.Component {
    componentDidCatch(error, errorInfo) {
        // Log error
    }
    render() {
        if (this.state.hasError) {
            return <FallbackComponent />;
        }
        return this.props.children;
    }
}
```

### 6. **Common Interview Questions**

#### Q1: What is the difference between useMemo and useCallback?
- `useMemo`: Returns a memoized **value**
- `useCallback`: Returns a memoized **function**

#### Q2: When to use useReducer vs useState?
- useReducer: Complex state logic, multiple related state values
- useState: Simple state, single values

#### Q3: How to prevent unnecessary re-renders?
- React.memo for components
- useMemo for expensive calculations
- useCallback for event handlers passed as props

#### Q4: What are the rules of hooks?
- Only call hooks at the top level
- Only call hooks from React functions
- Don't call hooks inside loops, conditions, or nested functions

#### Q5: How does useEffect cleanup work?
```javascript
useEffect(() => {
    const subscription = subscribeToAPI();
    return () => {
        // Cleanup: runs before next effect and on unmount
        subscription.unsubscribe();
    };
}, [dependencies]);
```

### 7. **Modern React Features**

#### Concurrent Features (React 18+)
- `useTransition`: Mark state updates as non-urgent
- `useDeferredValue`: Defer updating less important parts
- Automatic batching

#### Server Components
- Run on server, reduce bundle size
- Direct database access
- Zero client-side JavaScript

### 8. **Testing Strategies**

#### Unit Tests with React Testing Library
```javascript
import { render, screen, fireEvent } from '@testing-library/react';

test('button click increments counter', () => {
    render(<Counter />);
    const button = screen.getByRole('button');
    fireEvent.click(button);
    expect(screen.getByText('Count: 1')).toBeInTheDocument();
});
```

### 9. **TypeScript Integration**

#### Typing Components
```typescript
interface Props {
    title: string;
    count?: number;
    onClick: (id: number) => void;
}

const MyComponent: React.FC<Props> = ({ title, count = 0, onClick }) => {
    // Component logic
};
```

#### Typing Hooks
```typescript
const [state, setState] = useState<string | null>(null);
const inputRef = useRef<HTMLInputElement>(null);
```

### 10. **Architecture Patterns**

#### Container/Presentation Pattern
- Container: Handles logic, state
- Presentation: Receives props, renders UI

#### Atomic Design
- Atoms ? Molecules ? Organisms ? Templates ? Pages

### 11. **Key Takeaways for Interviews**

1. **Understand Virtual DOM**: How React reconciliation works
2. **Lifecycle**: How useEffect maps to class lifecycle methods
3. **State Management**: When to use Context vs Redux
4. **Performance**: Profiling with React DevTools
5. **Code Splitting**: React.lazy, Suspense
6. **Accessibility**: ARIA attributes, semantic HTML
7. **Security**: XSS prevention, sanitizing user input

### 12. **Practice Projects**

Build these to solidify knowledge:
1. Todo app with filtering and localStorage
2. Dashboard with API calls and error handling
3. E-commerce cart with Context API
4. Form with validation and custom hooks
5. Infinite scroll with intersection observer

### 13. **Resources**

- React Official Docs: https://react.dev
- React Patterns: https://reactpatterns.com
- Kent C. Dodds Blog: https://kentcdodds.com/blog
- React TypeScript Cheatsheet: https://react-typescript-cheatsheet.netlify.app

## ?? Interview Preparation Checklist

- [ ] Understand all hooks thoroughly
- [ ] Can explain Virtual DOM and reconciliation
- [ ] Know when to use Context vs prop drilling
- [ ] Understand performance optimization techniques
- [ ] Can implement custom hooks
- [ ] Know design patterns (HOC, Render Props, Compound Components)
- [ ] Understand React lifecycle and useEffect
- [ ] Can write tests with React Testing Library
- [ ] Familiar with TypeScript in React
- [ ] Know about React 18 features
- [ ] Understand code splitting and lazy loading
- [ ] Can debug with React DevTools

## ?? Pro Tips

1. **Always think about performance** - Don't over-optimize, but know when to optimize
2. **Composition over inheritance** - React favors composition
3. **Keep components small** - Single responsibility principle
4. **Use meaningful names** - Component and hook names should be descriptive
5. **Handle edge cases** - Loading states, errors, empty states
6. **Write testable code** - Separate logic from UI when possible
