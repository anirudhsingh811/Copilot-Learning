# React Learning Application

A comprehensive React learning application designed for senior developers with 13+ years of experience preparing for interviews.

## ?? Getting Started

1. **Run the application:**
   ```bash
   dotnet run
   ```

2. **Navigate to:**
   - Home: `https://localhost:5001/`
   - Fundamentals: `https://localhost:5001/ReactLearning`
   - Advanced Patterns: `https://localhost:5001/ReactAdvanced`
   - Interview Questions: `https://localhost:5001/ReactInterview`

## ?? What's Included

### 1. React Fundamentals (`/ReactLearning`)
Comprehensive examples covering:

#### **Core Hooks**
- `useState` - State management
- `useEffect` - Side effects and lifecycle
- `useContext` - Context consumption
- `useReducer` - Complex state logic
- `useMemo` - Memoizing expensive calculations
- `useCallback` - Memoizing functions
- `useRef` - DOM references and persistent values

#### **Custom Hooks**
- `useFetch` - Data fetching with loading/error states
- `useLocalStorage` - Syncing state with localStorage
- `useDebounce` - Debouncing input values
- `usePrevious` - Tracking previous values

#### **Design Patterns**
- Higher-Order Components (HOC)
- Render Props Pattern
- Compound Components Pattern

#### **State Management**
- Context API implementation
- Provider pattern
- Custom context hooks

#### **Performance Optimization**
- React.memo for component memoization
- useMemo for expensive calculations
- useCallback for function memoization
- Dependency array best practices

#### **Practical Examples**
- Todo app with filtering and localStorage
- Async data fetching from APIs
- Theme toggle with Context
- Mouse tracker with render props
- Controlled vs uncontrolled forms

### 2. Advanced Patterns (`/ReactAdvanced`)
Production-ready patterns including:

#### **Error Handling**
- Error Boundaries (class component)
- Fallback UI
- Error recovery

#### **Advanced Hooks**
- `useIntersectionObserver` - Infinite scroll, lazy loading
- `useWindowSize` - Responsive design
- `useMediaQuery` - Media query matching
- `useOnlineStatus` - Network detection
- `useAsync` - Async operations with abort

#### **Portal Pattern**
- Modal implementation with ReactDOM.createPortal
- Managing focus and body scroll

#### **Code Splitting**
- React.lazy for lazy loading
- Suspense for loading states
- Dynamic imports

#### **Infinite Scroll**
- Intersection Observer API
- Virtual scrolling concepts
- Performance optimization

#### **Form Validation**
- Real-time validation
- Touch tracking
- Error messaging
- Submit handling

#### **Responsive Design**
- Window size detection
- Media query hooks
- Mobile-first approach

### 3. Interview Questions (`/ReactInterview`)
Common interview questions with working code examples:

#### **Questions Covered**
1. **useMemo vs useCallback** - When and why to use each
2. **Preventing Re-renders** - React.memo, optimization strategies
3. **useEffect Cleanup** - Memory leaks, subscriptions, timers
4. **Custom Hooks** - Creating reusable logic
5. **Context API** - When to use, performance considerations
6. **Controlled vs Uncontrolled** - Form input patterns

#### **Additional Topics**
- Virtual DOM and reconciliation
- Keys in lists
- Lifting state up
- Error boundaries
- Code splitting
- Portals

## ?? Interview Preparation Checklist

### Core Concepts
- [ ] Understand all React hooks thoroughly
- [ ] Explain Virtual DOM and reconciliation algorithm
- [ ] Know when to use Context vs prop drilling vs Redux
- [ ] Understand component lifecycle with hooks
- [ ] Can implement custom hooks for common patterns

### Performance
- [ ] Know when and how to use React.memo
- [ ] Understand useMemo vs useCallback
- [ ] Can identify performance bottlenecks
- [ ] Know about code splitting and lazy loading
- [ ] Familiar with React Profiler

### Design Patterns
- [ ] Higher-Order Components (HOC)
- [ ] Render Props
- [ ] Compound Components
- [ ] Container/Presentation pattern
- [ ] Atomic Design principles

### Advanced Topics
- [ ] Error Boundaries
- [ ] Portals
- [ ] Suspense and Concurrent Mode
- [ ] Server Components (React 18+)
- [ ] useTransition and useDeferredValue

### Testing
- [ ] Jest basics
- [ ] React Testing Library
- [ ] Unit vs Integration tests
- [ ] Mocking API calls
- [ ] Testing hooks

### TypeScript (Optional but Recommended)
- [ ] Typing props and state
- [ ] Generic components
- [ ] Event handlers
- [ ] Custom hooks with TypeScript

## ?? Learning Path

### Week 1: Fundamentals
1. Review all hooks examples in `/ReactLearning`
2. Build the Todo app from scratch
3. Implement custom hooks (useFetch, useLocalStorage)
4. Practice Context API

### Week 2: Advanced Patterns
1. Study Error Boundaries implementation
2. Build a modal using Portals
3. Implement infinite scroll
4. Create responsive components
5. Practice form validation

### Week 3: Interview Prep
1. Go through all questions in `/ReactInterview`
2. Answer each question without looking at the code
3. Implement solutions from memory
4. Read the LEARNING_GUIDE.md thoroughly

### Week 4: Practice
1. Build a small project combining all concepts
2. Review performance optimization techniques
3. Practice explaining concepts out loud
4. Mock interviews

## ?? Key Interview Topics

### Must-Know Questions
1. **What is React?** - Library for building UIs using component-based architecture
2. **Virtual DOM?** - Lightweight copy of real DOM for efficient updates
3. **JSX?** - JavaScript XML syntax extension
4. **Props vs State?** - Props are immutable, state is mutable
5. **Lifecycle Methods?** - How they map to useEffect
6. **Hooks Rules?** - Only at top level, only in React functions
7. **Keys in Lists?** - Help React identify which items changed
8. **Lifting State Up?** - Move state to common ancestor
9. **Error Boundaries?** - Catch errors in component tree
10. **Code Splitting?** - Split bundle into smaller chunks

### Performance Questions
1. **How to optimize React app?** - Memoization, code splitting, lazy loading
2. **When to use useMemo?** - Expensive calculations that don't change often
3. **When to use useCallback?** - Memoize functions passed to child components
4. **React.memo?** - Prevent re-renders when props haven't changed
5. **Profiling?** - Use React DevTools Profiler

### State Management
1. **Context API vs Redux?** - Context for simple state, Redux for complex
2. **When to use Context?** - Theme, auth, language - infrequent updates
3. **Context performance issues?** - Can cause unnecessary re-renders
4. **Solutions?** - Split contexts, use multiple contexts, memoization

## ?? Project Structure

```
ReactAPP/
??? Pages/
?   ??? Index.cshtml                 # Home page with navigation
?   ??? ReactLearning.cshtml         # Fundamentals page
?   ??? ReactAdvanced.cshtml         # Advanced patterns page
?   ??? ReactInterview.cshtml        # Interview questions page
?   ??? *.cshtml.cs                  # Page models
??? wwwroot/
?   ??? react-app/
?       ??? app.jsx                  # Main fundamentals app
?       ??? advanced.jsx             # Advanced patterns app
?       ??? interview-questions.jsx  # Interview Q&A app
?       ??? LEARNING_GUIDE.md        # Comprehensive guide
?       ??? README.md                # This file
??? Program.cs                       # ASP.NET Core configuration
```

## ??? Technologies Used

- **React 18** - Latest version with concurrent features
- **React DOM 18** - Rendering library
- **Babel Standalone** - JSX transformation
- **Bootstrap 5** - Styling and components
- **ASP.NET Core (NET 10)** - Backend hosting

## ?? Best Practices Demonstrated

1. **Component Organization** - Logical separation of concerns
2. **Custom Hooks** - Reusable logic extraction
3. **Performance Optimization** - Strategic use of memoization
4. **Error Handling** - Error boundaries and try-catch
5. **Accessibility** - Semantic HTML and ARIA attributes
6. **Code Quality** - Clean, readable, maintainable code

## ?? Next Steps

After mastering these concepts:

1. **Learn TypeScript** - Type-safe React development
2. **State Management Libraries** - Redux, Zustand, Recoil
3. **Testing** - Jest, React Testing Library
4. **Next.js** - Server-side rendering, static generation
5. **React Native** - Mobile app development
6. **Build Tools** - Vite, Webpack, esbuild

## ?? Additional Resources

- [Official React Docs](https://react.dev)
- [React TypeScript Cheatsheet](https://react-typescript-cheatsheet.netlify.app)
- [Kent C. Dodds Blog](https://kentcdodds.com/blog)
- [React Patterns](https://reactpatterns.com)
- [Awesome React](https://github.com/enaqx/awesome-react)

## ?? Tips for Success

1. **Practice Daily** - Code every day, even 30 minutes
2. **Build Projects** - Apply concepts in real applications
3. **Read Code** - Study open-source React projects
4. **Explain Concepts** - Teach others to solidify understanding
5. **Stay Updated** - Follow React blog, RFCs, and discussions
6. **Mock Interviews** - Practice with peers or online platforms

## ?? Common Mistakes to Avoid

1. **Mutating State Directly** - Always use setState
2. **Missing Dependencies** - useEffect dependency array
3. **Unnecessary Re-renders** - Overusing state
4. **Prop Drilling** - Use Context or state management
5. **Not Cleaning Up** - useEffect cleanup functions
6. **Inline Functions** - In render causing re-renders
7. **Index as Key** - Use unique, stable identifiers

## ?? Interview Success Criteria

For a senior position with 13 years experience, demonstrate:

1. **Deep Understanding** - Not just "how" but "why"
2. **Architecture Skills** - Component design, project structure
3. **Performance Awareness** - When and how to optimize
4. **Best Practices** - Industry standards and patterns
5. **Problem-Solving** - Debug and solve complex issues
6. **Communication** - Explain concepts clearly
7. **Experience** - Real-world examples and trade-offs

Good luck with your interviews! ??
