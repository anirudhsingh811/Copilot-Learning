# SQL Quick Reference Guide - Interview Cheat Sheet

## ?? Table of Contents
1. [SELECT Basics](#select-basics)
2. [JOINs Quick Reference](#joins-quick-reference)
3. [Window Functions](#window-functions)
4. [Common Table Expressions](#ctes)
5. [Performance Tips](#performance-tips)
6. [Interview Favorites](#interview-favorites)

---

## SELECT Basics

```sql
-- Basic SELECT
SELECT column1, column2 FROM table WHERE condition ORDER BY column1;

-- DISTINCT
SELECT DISTINCT column FROM table;

-- TOP/LIMIT
SELECT TOP 10 * FROM table;  -- SQL Server
SELECT * FROM table LIMIT 10;  -- MySQL/PostgreSQL

-- Aggregate Functions
SELECT 
    COUNT(*) AS Total,
    SUM(column) AS Sum,
    AVG(column) AS Average,
    MIN(column) AS Minimum,
    MAX(column) AS Maximum
FROM table
GROUP BY group_column
HAVING COUNT(*) > 1;

-- CASE Statement
SELECT 
    column,
    CASE 
        WHEN condition1 THEN result1
        WHEN condition2 THEN result2
        ELSE default_result
    END AS new_column
FROM table;
```

---

## JOINs Quick Reference

```sql
-- INNER JOIN (only matching records)
SELECT a.*, b.*
FROM table_a a
INNER JOIN table_b b ON a.id = b.id;

-- LEFT JOIN (all from left + matches from right)
SELECT a.*, b.*
FROM table_a a
LEFT JOIN table_b b ON a.id = b.id;

-- RIGHT JOIN (all from right + matches from left)
SELECT a.*, b.*
FROM table_a a
RIGHT JOIN table_b b ON a.id = b.id;

-- FULL OUTER JOIN (all records from both)
SELECT a.*, b.*
FROM table_a a
FULL OUTER JOIN table_b b ON a.id = b.id;

-- CROSS JOIN (Cartesian product)
SELECT a.*, b.*
FROM table_a a
CROSS JOIN table_b b;

-- SELF JOIN (join table to itself)
SELECT e1.name AS Employee, e2.name AS Manager
FROM employees e1
LEFT JOIN employees e2 ON e1.manager_id = e2.id;
```

---

## Window Functions

```sql
-- ROW_NUMBER (unique sequential number)
SELECT 
    column,
    ROW_NUMBER() OVER (ORDER BY column) AS row_num
FROM table;

-- RANK (gaps in ranking)
SELECT 
    column,
    RANK() OVER (ORDER BY column DESC) AS rank
FROM table;

-- DENSE_RANK (no gaps)
SELECT 
    column,
    DENSE_RANK() OVER (ORDER BY column DESC) AS dense_rank
FROM table;

-- PARTITION BY (window within groups)
SELECT 
    department,
    salary,
    ROW_NUMBER() OVER (PARTITION BY department ORDER BY salary DESC) AS dept_rank
FROM employees;

-- LAG (previous row)
SELECT 
    column,
    LAG(column, 1) OVER (ORDER BY date_column) AS previous_value
FROM table;

-- LEAD (next row)
SELECT 
    column,
    LEAD(column, 1) OVER (ORDER BY date_column) AS next_value
FROM table;

-- Running Total
SELECT 
    date_column,
    amount,
    SUM(amount) OVER (ORDER BY date_column ROWS UNBOUNDED PRECEDING) AS running_total
FROM transactions;

-- Moving Average (last 3 rows)
SELECT 
    date_column,
    amount,
    AVG(amount) OVER (ORDER BY date_column ROWS BETWEEN 2 PRECEDING AND CURRENT ROW) AS moving_avg
FROM transactions;
```

---

## CTEs

```sql
-- Basic CTE
WITH cte_name AS (
    SELECT column1, column2
    FROM table
    WHERE condition
)
SELECT * FROM cte_name;

-- Multiple CTEs
WITH 
cte1 AS (SELECT ...),
cte2 AS (SELECT ... FROM cte1),
cte3 AS (SELECT ... FROM cte2)
SELECT * FROM cte3;

-- Recursive CTE (hierarchy)
WITH EmployeeHierarchy AS (
    -- Anchor member
    SELECT id, name, manager_id, 0 AS level
    FROM employees
    WHERE manager_id IS NULL
    
    UNION ALL
    
    -- Recursive member
    SELECT e.id, e.name, e.manager_id, eh.level + 1
    FROM employees e
    INNER JOIN EmployeeHierarchy eh ON e.manager_id = eh.id
)
SELECT * FROM EmployeeHierarchy;

-- Generate sequence (1 to 100)
WITH Numbers AS (
    SELECT 1 AS n
    UNION ALL
    SELECT n + 1 FROM Numbers WHERE n < 100
)
SELECT n FROM Numbers
OPTION (MAXRECURSION 100);
```

---

## Performance Tips

### ? DO's

```sql
-- Use specific columns instead of *
SELECT id, name, salary FROM employees;  -- ? Good

-- Use indexes on WHERE/JOIN columns
CREATE INDEX idx_employee_dept ON employees(department_id);  -- ?

-- Use EXISTS for existence checks
IF EXISTS (SELECT 1 FROM employees WHERE dept_id = 1)  -- ?

-- Use covering indexes
CREATE INDEX idx_covering ON employees(dept_id) INCLUDE (name, salary);  -- ?

-- Use proper data types
WHERE hire_date >= '2024-01-01'  -- ? Good

-- Batch large operations
UPDATE TOP (1000) table SET ...  -- ?
```

### ? DON'Ts

```sql
-- Avoid SELECT *
SELECT * FROM employees;  -- ? Bad

-- Avoid functions on indexed columns in WHERE
WHERE YEAR(hire_date) = 2024  -- ? Bad
WHERE hire_date >= '2024-01-01' AND hire_date < '2025-01-01'  -- ? Good

-- Avoid OR with different columns
WHERE dept_id = 1 OR salary > 100000  -- ? Can't use single index

-- Avoid NOT IN with nullable columns
WHERE id NOT IN (SELECT manager_id FROM employees)  -- ? NULL issues
WHERE NOT EXISTS (SELECT 1 FROM employees e2 WHERE e2.manager_id = e.id)  -- ?

-- Avoid implicit conversions
WHERE employee_id = '123'  -- ? If employee_id is INT

-- Avoid cursors for set-based operations
DECLARE cursor_name CURSOR FOR ...  -- ? Slow
-- Use set-based approach instead  -- ?
```

---

## Interview Favorites

### 1. Nth Highest Salary

```sql
-- 2nd highest salary
WITH RankedSalaries AS (
    SELECT salary, DENSE_RANK() OVER (ORDER BY salary DESC) AS rank
    FROM employees
)
SELECT DISTINCT salary FROM RankedSalaries WHERE rank = 2;
```

### 2. Find Duplicates

```sql
-- Find duplicate emails
SELECT email, COUNT(*) as count
FROM employees
GROUP BY email
HAVING COUNT(*) > 1;
```

### 3. Remove Duplicates (keep first)

```sql
WITH Duplicates AS (
    SELECT *, ROW_NUMBER() OVER (PARTITION BY email ORDER BY id) AS rn
    FROM employees
)
DELETE FROM Duplicates WHERE rn > 1;
```

### 4. Employees Earning More Than Manager

```sql
SELECT e.name AS Employee
FROM employees e
INNER JOIN employees m ON e.manager_id = m.id
WHERE e.salary > m.salary;
```

### 5. Department with Highest Average Salary

```sql
SELECT TOP 1
    department_id,
    AVG(salary) AS avg_salary
FROM employees
GROUP BY department_id
ORDER BY avg_salary DESC;
```

### 6. Running Total

```sql
SELECT 
    date,
    amount,
    SUM(amount) OVER (ORDER BY date ROWS UNBOUNDED PRECEDING) AS running_total
FROM transactions;
```

### 7. Top 3 Per Category

```sql
WITH RankedProducts AS (
    SELECT 
        category,
        product_name,
        sales,
        ROW_NUMBER() OVER (PARTITION BY category ORDER BY sales DESC) AS rank
    FROM products
)
SELECT * FROM RankedProducts WHERE rank <= 3;
```

### 8. Year-Over-Year Growth

```sql
WITH YearlyRevenue AS (
    SELECT 
        YEAR(date) AS year,
        SUM(revenue) AS total_revenue
    FROM sales
    GROUP BY YEAR(date)
)
SELECT 
    year,
    total_revenue,
    LAG(total_revenue) OVER (ORDER BY year) AS prev_year_revenue,
    total_revenue - LAG(total_revenue) OVER (ORDER BY year) AS growth
FROM YearlyRevenue;
```

### 9. Find Gaps in Sequence

```sql
WITH SequenceCheck AS (
    SELECT 
        id,
        LAG(id) OVER (ORDER BY id) AS prev_id
    FROM employees
)
SELECT 
    prev_id + 1 AS gap_start,
    id - 1 AS gap_end
FROM SequenceCheck
WHERE id - prev_id > 1;
```

### 10. Pivot Table

```sql
SELECT 
    department,
    [2021], [2022], [2023], [2024]
FROM (
    SELECT department, YEAR(hire_date) AS year, salary
    FROM employees
) AS SourceTable
PIVOT (
    AVG(salary)
    FOR year IN ([2021], [2022], [2023], [2024])
) AS PivotTable;
```

---

## Common String Functions

```sql
-- Concatenation
SELECT FirstName + ' ' + LastName AS FullName;  -- SQL Server
SELECT CONCAT(FirstName, ' ', LastName) AS FullName;  -- Universal

-- Substring
SELECT SUBSTRING(column, start, length);
SELECT LEFT(column, 5);
SELECT RIGHT(column, 5);

-- String manipulation
SELECT UPPER(column);
SELECT LOWER(column);
SELECT LTRIM(RTRIM(column));  -- Remove leading/trailing spaces
SELECT REPLACE(column, 'old', 'new');
SELECT LEN(column);  -- Length
SELECT CHARINDEX('substring', column);  -- Find position

-- String aggregation (SQL Server 2017+)
SELECT STRING_AGG(name, ', ') AS names
FROM employees
GROUP BY department_id;
```

---

## Common Date Functions

```sql
-- Current date/time
SELECT GETDATE();  -- SQL Server
SELECT SYSDATETIME();  -- Higher precision

-- Date parts
SELECT YEAR(date_column);
SELECT MONTH(date_column);
SELECT DAY(date_column);
SELECT DATEPART(QUARTER, date_column);

-- Date arithmetic
SELECT DATEADD(DAY, 7, date_column);  -- Add 7 days
SELECT DATEADD(MONTH, 1, date_column);  -- Add 1 month
SELECT DATEDIFF(DAY, start_date, end_date);  -- Difference in days

-- Date formatting
SELECT FORMAT(date_column, 'yyyy-MM-dd');
SELECT CONVERT(VARCHAR, date_column, 101);  -- MM/DD/YYYY

-- First/Last day of month
SELECT DATEFROMPARTS(YEAR(date_column), MONTH(date_column), 1);
SELECT EOMONTH(date_column);
```

---

## Transaction & Isolation Quick Guide

```sql
-- Basic transaction
BEGIN TRANSACTION;
    UPDATE table SET column = value WHERE condition;
    INSERT INTO audit_log VALUES (...);
COMMIT TRANSACTION;

-- Rollback on error
BEGIN TRANSACTION;
BEGIN TRY
    UPDATE ...
    INSERT ...
    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    THROW;
END CATCH;

-- Isolation levels
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;  -- Dirty reads
SET TRANSACTION ISOLATION LEVEL READ COMMITTED;     -- Default
SET TRANSACTION ISOLATION LEVEL REPEATABLE READ;    -- No non-repeatable
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;       -- Strictest
SET TRANSACTION ISOLATION LEVEL SNAPSHOT;           -- Row versioning
```

---

## Index Quick Reference

```sql
-- Create index
CREATE NONCLUSTERED INDEX idx_name ON table(column);

-- Composite index
CREATE INDEX idx_name ON table(col1, col2);

-- Covering index
CREATE INDEX idx_name ON table(col1) INCLUDE (col2, col3);

-- Unique index
CREATE UNIQUE INDEX idx_name ON table(column);

-- Filtered index
CREATE INDEX idx_name ON table(column) WHERE condition;

-- Drop index
DROP INDEX idx_name ON table;

-- Rebuild index
ALTER INDEX idx_name ON table REBUILD;
```

---

## Common Pitfalls to Avoid

1. ? **SELECT * in production** ? ? List specific columns
2. ? **Missing WHERE in UPDATE/DELETE** ? ? Always use WHERE
3. ? **No indexes on JOIN/WHERE columns** ? ? Create appropriate indexes
4. ? **Not handling NULL properly** ? ? Use IS NULL, ISNULL(), COALESCE()
5. ? **Function on indexed column in WHERE** ? ? Rewrite to make it SARGable
6. ? **Using cursors for set operations** ? ? Use set-based queries
7. ? **No error handling in transactions** ? ? Use TRY-CATCH
8. ? **Implicit data type conversions** ? ? Use correct data types
9. ? **Large transactions** ? ? Break into smaller batches
10. ? **Not checking execution plans** ? ? Always review performance

---

## Interview Preparation Checklist

- [ ] Can explain all JOIN types with examples
- [ ] Comfortable with window functions (ROW_NUMBER, RANK, LAG, LEAD)
- [ ] Can write recursive CTEs for hierarchical data
- [ ] Understand indexing strategies and when to use each type
- [ ] Know transaction isolation levels and their trade-offs
- [ ] Can optimize slow queries using execution plans
- [ ] Familiar with handling NULL values correctly
- [ ] Can write complex subqueries and correlate them
- [ ] Understand deadlock causes and prevention
- [ ] Practice common interview questions (Nth highest, duplicates, etc.)
- [ ] Can explain ACID properties
- [ ] Comfortable with date/string manipulation functions
- [ ] Know difference between UNION and UNION ALL
- [ ] Understand when to use EXISTS vs IN
- [ ] Can write efficient pagination queries

---

## Final Tips

1. **Practice Daily:** 30-60 minutes on LeetCode SQL or HackerRank
2. **Understand Concepts:** Don't just memorize, understand WHY
3. **Think Performance:** Always consider scalability
4. **Explain Your Thinking:** Talk through your approach in interviews
5. **Handle Edge Cases:** NULL values, empty sets, duplicates
6. **Know Your Environment:** Syntax may vary (SQL Server, MySQL, PostgreSQL)
7. **Use Aliases:** Make queries readable
8. **Test Your Queries:** Verify results with different data sets
9. **Read Execution Plans:** Understand how queries execute
10. **Stay Current:** SQL evolves, learn new features

Good luck! ??
