# ?? COMPREHENSIVE .NET INTERVIEW QUESTIONS BY CATEGORY

## ?? TABLE OF CONTENTS
1. [C# Language Fundamentals](#c-language-fundamentals)
2. [Memory & Performance](#memory--performance)
3. [Async & Concurrency](#async--concurrency)
4. [Dependency Injection](#dependency-injection)
5. [ASP.NET Core & Web APIs](#aspnet-core--web-apis)
6. [Entity Framework Core](#entity-framework-core)
7. [Design Patterns & Architecture](#design-patterns--architecture)
8. [Testing](#testing)
9. [Security](#security)
10. [DevOps & Cloud](#devops--cloud)
11. [Leadership & System Design](#leadership--system-design)

---

## C# Language Fundamentals

### Basic Level
1. **What's new in C# 12 and how would you use it in production?**
2. **Explain value types vs reference types with memory allocation details**
3. **What are records and when would you use them over classes?**
4. **Difference between `readonly` and `const`?**
5. **Explain nullable reference types and their benefits**

### Intermediate Level
6. **How does pattern matching improve code quality? Give real-world examples**
7. **Explain covariance and contravariance with generic delegates**
8. **What are init-only properties and their use cases?**
9. **Difference between IEnumerable, IQueryable, and ICollection?**
10. **How do expression trees work and when would you use them?**

### Advanced Level
11. **Explain source generators and their performance benefits**
12. **How do you implement custom async iterators with IAsyncEnumerable?**
13. **What are ref structs and their limitations?**
14. **Explain unsafe code and when it's justified**
15. **How do you optimize LINQ queries for performance?**

---

## Memory & Performance

### Basic Level
16. **Explain .NET garbage collection generations**
17. **What is boxing/unboxing and how to avoid it?**
18. **Stack vs heap: how does .NET decide where to allocate?**
19. **What is the IDisposable pattern and when to use it?**
20. **Explain using statement and using declaration**

### Intermediate Level
21. **How does Span<T> improve performance over arrays?**
22. **What is the Large Object Heap and its implications?**
23. **Explain Memory<T> vs Span<T> - when to use each?**
24. **How do you detect and fix memory leaks in .NET?**
25. **What is object pooling and when to implement it?**

### Advanced Level
26. **How do you optimize allocations in hot paths?**
27. **Explain stackalloc and when it's appropriate**
28. **What are memory-mapped files and their use cases?**
29. **How do you analyze GC pressure in production?**
30. **Explain zero-allocation patterns in .NET**

---

## Async & Concurrency

### Basic Level
31. **Difference between async/await and parallel programming?**
32. **What does async/await do behind the scenes?**
33. **When should you use ConfigureAwait(false)?**
34. **Explain Task vs Task<T>**
35. **What are the dangers of async void?**

### Intermediate Level
36. **How do you implement proper cancellation with CancellationToken?**
37. **Explain Task.WhenAll vs Task.WhenAny use cases**
38. **What is SynchronizationContext and why does it matter?**
39. **How do you prevent deadlocks in async code?**
40. **Difference between Task.Run and Task.Factory.StartNew?**

### Advanced Level
41. **When would you use ValueTask over Task?**
42. **How do you implement custom TaskScheduler?**
43. **Explain async state machine transformation**
44. **How do you handle concurrent requests with rate limiting?**
45. **What is ThreadPool exhaustion and how to prevent it?**

---

## Dependency Injection

### Basic Level
46. **What is dependency injection and its benefits?**
47. **Explain service lifetimes: Singleton, Scoped, Transient**
48. **Constructor injection vs property injection - pros/cons?**
49. **How do you register services in .NET?**
50. **What is IServiceProvider and when to use it?**

### Intermediate Level
51. **What is the captive dependency problem?**
52. **How do you register multiple implementations of an interface?**
53. **Explain service descriptors**
54. **How do you implement factory pattern with DI?**
55. **What are keyed services in .NET 8+?**

### Advanced Level
56. **How do you handle circular dependencies?**
57. **Implement a multi-tenant DI strategy**
58. **How do you validate DI container configuration?**
59. **When to use third-party DI containers vs built-in?**
60. **Design a plugin architecture using DI**

---

## ASP.NET Core & Web APIs

### Basic Level
61. **Explain the ASP.NET Core request pipeline**
62. **What is middleware and how to create custom middleware?**
63. **Difference between MVC and Minimal APIs?**
64. **How does model binding work?**
65. **What are action filters and when to use them?**

### Intermediate Level
66. **Implement API versioning strategies**
67. **How do you implement authentication vs authorization?**
68. **Explain JWT and how to validate tokens**
69. **What is CORS and how to configure it properly?**
70. **How do you implement request/response logging?**

### Advanced Level
71. **Design a rate limiting strategy for high-traffic APIs**
72. **Implement API gateway pattern**
73. **How do you handle API backward compatibility?**
74. **Design fault-tolerant API with circuit breakers**
75. **Implement distributed tracing in microservices**

---

## Entity Framework Core

### Basic Level
76. **Code First vs Database First - when to use each?**
77. **What are DbContext and DbSet?**
78. **Explain lazy loading vs eager loading**
79. **How do migrations work in EF Core?**
80. **What is change tracking in EF Core?**

### Intermediate Level
81. **Implement repository pattern with EF Core - is it needed?**
82. **How do you optimize N+1 query problems?**
83. **Explain global query filters**
84. **How do you handle concurrency conflicts?**
85. **What are compiled queries and when to use them?**

### Advanced Level
86. **Design multi-tenant architecture with EF Core**
87. **Implement custom conventions in EF Core**
88. **How do you optimize bulk operations?**
89. **Explain query splitting and its trade-offs**
90. **Design read/write separation (CQRS) with EF Core**

---

## Design Patterns & Architecture

### Basic Level
91. **Explain SOLID principles with C# examples**
92. **What is Repository pattern and when to use it?**
93. **Explain Factory pattern and its variations**
94. **What is Strategy pattern?**
95. **Difference between Adapter and Facade patterns?**

### Intermediate Level
96. **Implement CQRS pattern**
97. **Explain Mediator pattern and MediatR library**
98. **What is the Unit of Work pattern?**
99. **How do you implement Observer pattern in C#?**
100. **Explain Decorator pattern with middleware example**

### Advanced Level
101. **Design Clean Architecture for microservices**
102. **Implement Event Sourcing pattern**
103. **Design saga pattern for distributed transactions**
104. **Implement Domain-Driven Design (DDD) aggregates**
105. **Design event-driven architecture with message brokers**

---

## Testing

### Basic Level
106. **Unit tests vs Integration tests vs E2E tests - when to use each?**
107. **What is AAA (Arrange-Act-Assert) pattern?**
108. **How do you mock dependencies with Moq?**
109. **What is Test-Driven Development (TDD)?**
110. **How do you test async methods?**

### Intermediate Level
111. **Implement integration tests for Web API**
112. **How do you test database operations?**
113. **What is test coverage and what's a good target?**
114. **How do you implement parameterized tests?**
115. **Design testable code - best practices**

### Advanced Level
116. **Implement contract testing for microservices**
117. **How do you test distributed systems?**
118. **Design chaos engineering experiments**
119. **Implement mutation testing strategy**
120. **How do you test resilience patterns (circuit breakers, retries)?**

---

## Security

### Basic Level
121. **Explain authentication vs authorization**
122. **What is JWT and how does it work?**
123. **How do you store passwords securely?**
124. **What is HTTPS and why is it important?**
125. **Explain OWASP Top 10 vulnerabilities**

### Intermediate Level
126. **Implement OAuth 2.0 and OpenID Connect**
127. **How do you prevent SQL injection in C#?**
128. **What is CSRF and how to prevent it?**
129. **Implement role-based vs policy-based authorization**
130. **How do you secure API keys and secrets?**

### Advanced Level
131. **Design zero-trust architecture**
132. **Implement certificate-based authentication**
133. **How do you handle security in microservices?**
134. **Design secure multi-tenant architecture**
135. **Implement data encryption at rest and in transit**

---

## DevOps & Cloud

### Basic Level
136. **What is Docker and why use containers?**
137. **Explain CI/CD pipeline components**
138. **What is Infrastructure as Code (IaC)?**
139. **How do you implement logging in .NET?**
140. **What are health checks and why implement them?**

### Intermediate Level
141. **Design a deployment strategy (blue-green, canary)**
142. **Implement structured logging with Serilog**
143. **How do you monitor application performance?**
144. **What is distributed caching with Redis?**
145. **Implement feature flags for gradual rollout**

### Advanced Level
146. **Design cloud-native architecture on Azure/AWS**
147. **Implement observability (logs, metrics, traces)**
148. **Design disaster recovery strategy**
149. **Implement auto-scaling strategy**
150. **Design cost optimization for cloud resources**

---

## Leadership & System Design

### Engineering Manager Level

#### Technical Leadership
151. **How do you evaluate and adopt new technologies in your team?**
152. **Design a code review process for a team of 10+ developers**
153. **How do you ensure consistent coding standards across teams?**
154. **Describe your approach to technical debt management**
155. **How do you mentor senior developers vs junior developers?**

#### System Design
156. **Design a high-traffic e-commerce platform (1M+ users)**
    - Database design
    - Caching strategy
    - Scalability approach
    - Payment processing
    - Inventory management

157. **Design a real-time notification system**
    - Push notifications
    - Email/SMS fallback
    - Delivery guarantees
    - Scale to millions of users

158. **Design a video streaming platform**
    - Content delivery
    - Adaptive bitrate streaming
    - DRM and security
    - Analytics

159. **Design a microservices architecture for financial services**
    - Service boundaries
    - Data consistency
    - Security
    - Audit trail
    - Compliance

160. **Design a distributed logging and monitoring system**
    - Log aggregation
    - Real-time alerting
    - Metrics collection
    - Distributed tracing

#### Architecture Decisions
161. **Monolith vs Microservices - when to choose which?**
162. **SQL vs NoSQL - decision criteria**
163. **Synchronous vs Asynchronous communication in microservices**
164. **Design patterns for resilient distributed systems**
165. **How do you approach database schema versioning?**

#### Performance & Scalability
166. **Your API response time degraded from 100ms to 2s. Troubleshooting approach?**
167. **Design a system to handle 10x traffic spike**
168. **How do you identify and resolve bottlenecks in production?**
169. **Database is the bottleneck. What are your options?**
170. **Design a caching strategy for complex queries**

#### Production Issues
171. **Production is down. What's your incident response process?**
172. **How do you conduct post-mortem analysis?**
173. **Design a disaster recovery plan**
174. **How do you handle data corruption in production?**
175. **Customer reports data inconsistency. Investigation approach?**

---

## ?? SCENARIO-BASED QUESTIONS FOR ENGINEERING MANAGERS

### Team & Process
176. **Your team is missing deadlines. How do you diagnose and fix?**
177. **Two senior developers have conflicting architectural opinions. How do you resolve?**
178. **Junior developer pushed buggy code to production. How do you handle?**
179. **Team morale is low after several production incidents. Action plan?**
180. **You need to ramp up 5 new developers quickly. What's your onboarding strategy?**

### Architecture & Planning
181. **CTO asks: "Should we migrate our monolith to microservices?" Your analysis?**
182. **Product wants a feature in 1 week that requires 6 weeks. How do you respond?**
183. **Technical debt is accumulating. How do you make the business case for cleanup?**
184. **You inherited a legacy .NET Framework app. Migration strategy to .NET 8?**
185. **Design a modernization roadmap for a 10-year-old application**

### Performance & Scale
186. **API handles 1000 req/sec, needs to scale to 10,000. Approach?**
187. **Database queries are slow. Investigation and optimization strategy?**
188. **Memory usage is growing unbounded. Troubleshooting steps?**
189. **Users report intermittent timeouts. Root cause analysis approach?**
190. **Design load testing strategy for a new system**

### Security & Compliance
191. **Security audit found 50 vulnerabilities. Prioritization approach?**
192. **Need to implement GDPR compliance. Technical strategy?**
193. **API keys were leaked to GitHub. Immediate action plan?**
194. **Design security review process for code changes**
195. **Implement secure development lifecycle in your team**

### Cost & ROI
196. **Cloud costs doubled in 3 months. Investigation and optimization?**
197. **Business wants cheaper alternative to current cloud provider. Analysis?**
198. **Justify infrastructure investment to reduce tech debt**
199. **Build vs buy decision for critical component**
200. **Calculate TCO for new technology adoption**

---

## ?? HOW TO USE THIS GUIDE

### For Interview Preparation:
1. **Start with fundamentals** - Ensure solid understanding of basics
2. **Practice coding** - Implement POCs for each concept
3. **Think out loud** - Practice explaining your thought process
4. **Draw diagrams** - System design questions need visual representation
5. **Ask clarifying questions** - Show you understand requirements

### For Interviewing Candidates:
1. **Start broad, go deep** - Begin with high-level, dive into specifics
2. **Follow-up questions** - Probe understanding vs memorization
3. **Scenario-based** - Real-world problems reveal practical knowledge
4. **Code reviews** - Show them code, ask for improvements
5. **System design** - Whiteboard exercises for senior roles

### For Continuous Learning:
1. **Weekly POC** - Implement one concept thoroughly
2. **Read source code** - ASP.NET Core, EF Core on GitHub
3. **Blog posts** - Write about what you learn
4. **Code reviews** - Learn from others' code
5. **Conference talks** - .NET Conf, NDC, Build

---

## ?? RECOMMENDED RESOURCES

### Books
- **C# in Depth** by Jon Skeet
- **CLR via C#** by Jeffrey Richter
- **Clean Architecture** by Robert C. Martin
- **Domain-Driven Design** by Eric Evans
- **Designing Data-Intensive Applications** by Martin Kleppmann

### Online Resources
- Microsoft Learn
- .NET Blog
- GitHub - dotnet repositories
- Stack Overflow
- Pluralsight / Udemy courses

### Practice Platforms
- LeetCode (algorithms)
- System Design Primer
- Architecture Kata
- Code Review exercises

---

**Remember**: These questions are not just for interviews. They represent the knowledge you need to architect, build, and maintain production systems at scale. Focus on understanding **WHY** over memorizing **WHAT**.

**Good luck with your interviews! ??**
