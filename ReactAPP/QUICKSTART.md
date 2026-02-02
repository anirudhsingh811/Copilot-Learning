# ?? Quick Start Guide - React Learning App

## Run the Application

```bash
cd ReactAPP
dotnet run
```

Then open your browser to: `https://localhost:5001`

## ?? Pages Available

| Page | URL | What You'll Learn |
|------|-----|-------------------|
| **Home** | `/` | Overview and navigation |
| **Fundamentals** | `/ReactLearning` | Hooks, Context, Custom Hooks, Performance |
| **Advanced** | `/ReactAdvanced` | Error Boundaries, Portals, Infinite Scroll |
| **Interview Q&A** | `/ReactInterview` | Common questions with working examples |

## ?? Learning Order

### Day 1-2: React Fundamentals
Visit `/ReactLearning` and explore:
1. **Hooks Section** - useState, useEffect, useContext, useReducer
2. **Custom Hooks** - useFetch, useLocalStorage, useDebounce
3. **Performance** - React.memo, useMemo, useCallback
4. **Todo App** - Practice by modifying it
5. **Context API** - Theme toggle example

### Day 3-4: Advanced Patterns
Visit `/ReactAdvanced` and explore:
1. **Error Boundaries** - How to catch errors
2. **Portals** - Modal implementation
3. **Infinite Scroll** - Intersection Observer
4. **Form Validation** - Real-time validation
5. **Responsive Hooks** - Window size, media queries

### Day 5-7: Interview Preparation
Visit `/ReactInterview` and practice:
1. Answer each question without looking
2. Implement the solution from memory
3. Understand the "why" not just the "how"
4. Practice explaining concepts verbally

## ?? Top 10 Interview Questions

1. **What is the difference between useMemo and useCallback?**
   - useMemo returns a memoized VALUE
   - useCallback returns a memoized FUNCTION

2. **How do you prevent unnecessary re-renders?**
   - React.memo for components
   - useMemo for expensive calculations
   - useCallback for functions passed as props

3. **Explain useEffect cleanup**
   - Cleanup runs before next effect
   - Cleanup runs on unmount
   - Prevents memory leaks

4. **When to use Context API vs Redux?**
   - Context: Simple state, infrequent updates (theme, auth)
   - Redux: Complex state, frequent updates, time-travel debugging

5. **What is Virtual DOM?**
   - Lightweight copy of real DOM
   - React compares (reconciliation)
   - Minimal DOM updates for performance

6. **Controlled vs Uncontrolled components?**
   - Controlled: React state is source of truth
   - Uncontrolled: DOM is source of truth (use refs)

7. **What are keys in lists?**
   - Help React identify which items changed
   - Should be stable, unique identifiers
   - Never use index as key for dynamic lists

8. **How does reconciliation work?**
   - Diffing algorithm
   - Keys help identify changes
   - Minimal DOM updates

9. **What are Error Boundaries?**
   - Class components that catch errors
   - Prevent entire app crash
   - Show fallback UI

10. **Explain lifting state up**
    - Move state to closest common ancestor
    - Share state between components
    - Props flow down

## ?? Quick Code Examples

### Custom Hook
```javascript
const useFetch = (url) => {
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    fetch(url)
      .then(res => res.json())
      .then(setData)
      .catch(setError)
      .finally(() => setLoading(false));
  }, [url]);

  return { data, loading, error };
};
```

### Context API
```javascript
const ThemeContext = createContext();

const ThemeProvider = ({ children }) => {
  const [theme, setTheme] = useState('light');
  return (
    <ThemeContext.Provider value={{ theme, setTheme }}>
      {children}
    </ThemeContext.Provider>
  );
};

const useTheme = () => useContext(ThemeContext);
```

### Performance Optimization
```javascript
// Memoize component
const Child = React.memo(({ count, onClick }) => {
  return <button onClick={onClick}>{count}</button>;
});

// Memoize callback
const Parent = () => {
  const [count, setCount] = useState(0);
  const handleClick = useCallback(() => {
    setCount(c => c + 1);
  }, []);
  return <Child count={count} onClick={handleClick} />;
};
```

## ?? Interview Checklist

Before your interview, make sure you can:

- [ ] Explain all hooks with examples
- [ ] Write a custom hook from scratch
- [ ] Implement Context API for state management
- [ ] Optimize a component causing performance issues
- [ ] Handle errors with Error Boundaries
- [ ] Implement form validation
- [ ] Use refs appropriately
- [ ] Explain lifecycle methods with useEffect
- [ ] Describe reconciliation algorithm
- [ ] Compare controlled vs uncontrolled components

## ?? Practice Projects

Build these to solidify your knowledge:

1. **Todo App** - CRUD operations, filtering, localStorage
2. **Weather Dashboard** - API calls, error handling, loading states
3. **E-commerce Cart** - Context API, complex state management
4. **Infinite Scroll Feed** - Intersection Observer, pagination
5. **Form Builder** - Dynamic forms, validation, submission

## ?? Study Materials

All included in this project:
- `wwwroot/react-app/README.md` - Full documentation
- `wwwroot/react-app/LEARNING_GUIDE.md` - Comprehensive guide
- Source code in `wwwroot/react-app/*.jsx` - All examples

## ?? Pro Tips

1. **Open DevTools** - Watch console logs to understand render cycles
2. **Modify Examples** - Change the code and see what happens
3. **Break Things** - Remove dependencies, see errors, understand why
4. **Explain Out Loud** - Teach concepts to solidify understanding
5. **Build From Scratch** - Don't just read, implement yourself

## ?? You're Ready!

With these examples and practice, you'll be prepared for senior-level React interviews. Remember:

- **Understand the "why"** - Not just memorize
- **Practice daily** - Consistency beats intensity
- **Build projects** - Apply what you learn
- **Ask questions** - Deepen understanding
- **Stay confident** - You have 13 years of experience!

Good luck! ??
