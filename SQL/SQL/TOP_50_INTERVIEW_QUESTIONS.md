# ?? Top 50 SQL Interview Questions - Must Know

## ? Difficulty Legend
- ?? **Easy:** Should answer in 2-3 minutes
- ?? **Medium:** Should answer in 5-7 minutes
- ?? **Hard:** Should answer in 10-15 minutes

---

## ?? SELECT & Basic Queries (10 Questions)

### ?? Q1: Select all employees earning more than 50000
```sql
SELECT * FROM Employees WHERE Salary > 50000;
```

### ?? Q2: Find total number of employees per department
```sql
SELECT DepartmentID, COUNT(*) AS EmployeeCount
FROM Employees
GROUP BY DepartmentID;
```

### ?? Q3: Find employees hired in last 90 days
```sql
SELECT * FROM Employees 
WHERE HireDate >= DATEADD(DAY, -90, GETDATE());
```

### ?? Q4: Select distinct department names
```sql
SELECT DISTINCT DepartmentName FROM Departments;
```

### ?? Q5: Find average salary per department
```sql
SELECT 
    d.DepartmentName,
    AVG(e.Salary) AS AvgSalary
FROM Employees e
INNER JOIN Departments d ON e.DepartmentID = d.DepartmentID
GROUP BY d.DepartmentName;
```

### ?? Q6: Find departments with more than 5 employees
```sql
SELECT DepartmentID, COUNT(*) AS EmployeeCount
FROM Employees
GROUP BY DepartmentID
HAVING COUNT(*) > 5;
```

### ?? Q7: Sort employees by salary in descending order
```sql
SELECT * FROM Employees ORDER BY Salary DESC;
```

### ?? Q8: Find employees whose name starts with 'J'
```sql
SELECT * FROM Employees WHERE FirstName LIKE 'J%';
```

### ?? Q9: Count total active employees
```sql
SELECT COUNT(*) AS ActiveEmployees 
FROM Employees 
WHERE IsActive = 1;
```

### ?? Q10: Find employees with NULL phone numbers
```sql
SELECT * FROM Employees WHERE Phone IS NULL;
```

---

## ?? JOINs (10 Questions)

### ?? Q11: List all employees with their department names (INNER JOIN)
```sql
SELECT 
    e.FirstName,
    e.LastName,
    d.DepartmentName
FROM Employees e
INNER JOIN Departments d ON e.DepartmentID = d.DepartmentID;
```

### ?? Q12: List all departments and employee count (LEFT JOIN)
```sql
SELECT 
    d.DepartmentName,
    COUNT(e.EmployeeID) AS EmployeeCount
FROM Departments d
LEFT JOIN Employees e ON d.DepartmentID = e.DepartmentID
GROUP BY d.DepartmentName;
```

### ?? Q13: Find employees without a manager (SELF JOIN)
```sql
SELECT 
    e.FirstName,
    e.LastName
FROM Employees e
LEFT JOIN Employees m ON e.ManagerID = m.EmployeeID
WHERE m.EmployeeID IS NULL;
```

### ?? Q14: Find employees and their manager's name (SELF JOIN)
```sql
SELECT 
    e.FirstName + ' ' + e.LastName AS Employee,
    ISNULL(m.FirstName + ' ' + m.LastName, 'No Manager') AS Manager
FROM Employees e
LEFT JOIN Employees m ON e.ManagerID = m.EmployeeID;
```

### ?? Q15: Find employees working on multiple projects
```sql
SELECT 
    e.FirstName,
    e.LastName,
    COUNT(ep.ProjectID) AS ProjectCount
FROM Employees e
INNER JOIN EmployeeProjects ep ON e.EmployeeID = ep.EmployeeID
GROUP BY e.EmployeeID, e.FirstName, e.LastName
HAVING COUNT(ep.ProjectID) > 1;
```

### ?? Q16: Find employees not assigned to any project
```sql
SELECT 
    e.FirstName,
    e.LastName
FROM Employees e
LEFT JOIN EmployeeProjects ep ON e.EmployeeID = ep.EmployeeID
WHERE ep.ProjectID IS NULL;
```

### ?? Q17: List employees with department and project info
```sql
SELECT 
    e.FirstName + ' ' + e.LastName AS Employee,
    d.DepartmentName,
    p.ProjectName
FROM Employees e
INNER JOIN Departments d ON e.DepartmentID = d.DepartmentID
LEFT JOIN EmployeeProjects ep ON e.EmployeeID = ep.EmployeeID
LEFT JOIN Projects p ON ep.ProjectID = p.ProjectID;
```

### ?? Q18: Find departments without active projects
```sql
SELECT 
    d.DepartmentName
FROM Departments d
LEFT JOIN Projects p ON d.DepartmentID = p.DepartmentID AND p.Status = 'Active'
WHERE p.ProjectID IS NULL;
```

### ?? Q19: Cross join to generate all employee-project combinations
```sql
SELECT 
    e.FirstName + ' ' + e.LastName AS Employee,
    p.ProjectName
FROM Employees e
CROSS JOIN Projects p;
```

### ?? Q20: Full outer join to find mismatches
```sql
SELECT 
    ISNULL(e.FirstName, 'N/A') AS Employee,
    ISNULL(d.DepartmentName, 'N/A') AS Department
FROM Employees e
FULL OUTER JOIN Departments d ON e.DepartmentID = d.DepartmentID
WHERE e.EmployeeID IS NULL OR d.DepartmentID IS NULL;
```

---

## ?? Ranking & Window Functions (10 Questions)

### ?? Q21: Find 2nd highest salary
```sql
WITH RankedSalaries AS (
    SELECT 
        Salary,
        DENSE_RANK() OVER (ORDER BY Salary DESC) AS Rank
    FROM Employees
)
SELECT DISTINCT Salary 
FROM RankedSalaries 
WHERE Rank = 2;
```

### ?? Q22: Find Nth highest salary per department
```sql
DECLARE @N INT = 2;
WITH RankedSalaries AS (
    SELECT 
        DepartmentID,
        Salary,
        DENSE_RANK() OVER (PARTITION BY DepartmentID ORDER BY Salary DESC) AS Rank
    FROM Employees
)
SELECT 
    DepartmentID,
    Salary
FROM RankedSalaries
WHERE Rank = @N;
```

### ?? Q23: Rank employees by salary
```sql
SELECT 
    FirstName,
    LastName,
    Salary,
    ROW_NUMBER() OVER (ORDER BY Salary DESC) AS RowNum,
    RANK() OVER (ORDER BY Salary DESC) AS Rank,
    DENSE_RANK() OVER (ORDER BY Salary DESC) AS DenseRank
FROM Employees;
```

### ?? Q24: Top 3 employees per department by salary
```sql
WITH RankedEmployees AS (
    SELECT 
        FirstName,
        LastName,
        DepartmentID,
        Salary,
        ROW_NUMBER() OVER (PARTITION BY DepartmentID ORDER BY Salary DESC) AS Rank
    FROM Employees
)
SELECT * FROM RankedEmployees WHERE Rank <= 3;
```

### ?? Q25: Calculate running total of salaries
```sql
SELECT 
    FirstName,
    LastName,
    HireDate,
    Salary,
    SUM(Salary) OVER (ORDER BY HireDate ROWS UNBOUNDED PRECEDING) AS RunningTotal
FROM Employees;
```

### ?? Q26: Calculate moving average of last 3 salaries
```sql
SELECT 
    FirstName,
    LastName,
    HireDate,
    Salary,
    AVG(Salary) OVER (
        ORDER BY HireDate 
        ROWS BETWEEN 2 PRECEDING AND CURRENT ROW
    ) AS MovingAvg
FROM Employees;
```

### ?? Q27: Find salary differences from previous employee
```sql
SELECT 
    FirstName,
    LastName,
    Salary,
    LAG(Salary) OVER (ORDER BY HireDate) AS PrevSalary,
    Salary - LAG(Salary) OVER (ORDER BY HireDate) AS Difference
FROM Employees;
```

### ?? Q28: Find employees earning more than department average
```sql
WITH DeptAvg AS (
    SELECT 
        DepartmentID,
        AVG(Salary) AS AvgSalary
    FROM Employees
    GROUP BY DepartmentID
)
SELECT 
    e.FirstName,
    e.LastName,
    e.Salary,
    da.AvgSalary
FROM Employees e
INNER JOIN DeptAvg da ON e.DepartmentID = da.DepartmentID
WHERE e.Salary > da.AvgSalary;
```

### ?? Q29: Divide employees into salary quartiles
```sql
SELECT 
    FirstName,
    LastName,
    Salary,
    NTILE(4) OVER (ORDER BY Salary) AS Quartile
FROM Employees;
```

### ?? Q30: Find first hired employee per department
```sql
SELECT 
    DepartmentID,
    FirstName,
    LastName,
    HireDate
FROM (
    SELECT 
        *,
        ROW_NUMBER() OVER (PARTITION BY DepartmentID ORDER BY HireDate) AS RowNum
    FROM Employees
) AS Ranked
WHERE RowNum = 1;
```

---

## ?? Duplicates & Data Issues (5 Questions)

### ?? Q31: Find duplicate email addresses
```sql
SELECT 
    Email,
    COUNT(*) AS DuplicateCount
FROM Employees
GROUP BY Email
HAVING COUNT(*) > 1;
```

### ?? Q32: Delete duplicate records (keep first)
```sql
WITH Duplicates AS (
    SELECT 
        *,
        ROW_NUMBER() OVER (PARTITION BY Email ORDER BY EmployeeID) AS RowNum
    FROM Employees
)
DELETE FROM Duplicates WHERE RowNum > 1;
```

### ?? Q33: Find gaps in employee ID sequence
```sql
WITH SequenceCheck AS (
    SELECT 
        EmployeeID,
        LAG(EmployeeID) OVER (ORDER BY EmployeeID) AS PrevID
    FROM Employees
)
SELECT 
    PrevID + 1 AS GapStart,
    EmployeeID - 1 AS GapEnd
FROM SequenceCheck
WHERE EmployeeID - PrevID > 1;
```

### ?? Q34: Find employees with same name in same department
```sql
SELECT 
    e1.FirstName,
    e1.LastName,
    e1.DepartmentID,
    COUNT(*) AS DuplicateCount
FROM Employees e1
INNER JOIN Employees e2 
    ON e1.FirstName = e2.FirstName 
    AND e1.LastName = e2.LastName
    AND e1.DepartmentID = e2.DepartmentID
    AND e1.EmployeeID < e2.EmployeeID
GROUP BY e1.FirstName, e1.LastName, e1.DepartmentID;
```

### ?? Q35: Find records with NULL in required fields
```sql
SELECT * FROM Employees
WHERE FirstName IS NULL 
   OR LastName IS NULL 
   OR Email IS NULL;
```

---

## ?? Date & Time Problems (5 Questions)

### ?? Q36: Find employees hired this year
```sql
SELECT * FROM Employees
WHERE YEAR(HireDate) = YEAR(GETDATE());
```

### ?? Q37: Calculate employee tenure in years
```sql
SELECT 
    FirstName,
    LastName,
    HireDate,
    DATEDIFF(YEAR, HireDate, GETDATE()) AS YearsOfService
FROM Employees;
```

### ?? Q38: Find employees with anniversary this month
```sql
SELECT 
    FirstName,
    LastName,
    HireDate,
    DATEDIFF(YEAR, HireDate, GETDATE()) AS YearsOfService
FROM Employees
WHERE MONTH(HireDate) = MONTH(GETDATE());
```

### ?? Q39: Calculate business days between two dates
```sql
DECLARE @StartDate DATE = '2024-01-01';
DECLARE @EndDate DATE = '2024-01-31';

WITH DateRange AS (
    SELECT @StartDate AS DateValue
    UNION ALL
    SELECT DATEADD(DAY, 1, DateValue)
    FROM DateRange
    WHERE DateValue < @EndDate
)
SELECT COUNT(*) AS BusinessDays
FROM DateRange
WHERE DATEPART(WEEKDAY, DateValue) NOT IN (1, 7)
OPTION (MAXRECURSION 365);
```

### ?? Q40: Find employees hired in each quarter
```sql
SELECT 
    DATEPART(QUARTER, HireDate) AS Quarter,
    YEAR(HireDate) AS Year,
    COUNT(*) AS EmployeesHired
FROM Employees
GROUP BY YEAR(HireDate), DATEPART(QUARTER, HireDate)
ORDER BY Year, Quarter;
```

---

## ?? Hierarchical & Recursive Queries (5 Questions)

### ?? Q41: Display employee hierarchy
```sql
WITH EmployeeHierarchy AS (
    -- Top level
    SELECT 
        EmployeeID,
        FirstName + ' ' + LastName AS FullName,
        ManagerID,
        0 AS Level
    FROM Employees
    WHERE ManagerID IS NULL
    
    UNION ALL
    
    -- Recursive
    SELECT 
        e.EmployeeID,
        e.FirstName + ' ' + e.LastName,
        e.ManagerID,
        eh.Level + 1
    FROM Employees e
    INNER JOIN EmployeeHierarchy eh ON e.ManagerID = eh.EmployeeID
)
SELECT 
    REPLICATE('  ', Level) + FullName AS Hierarchy,
    Level
FROM EmployeeHierarchy;
```

### ?? Q42: Count subordinates for each manager
```sql
WITH EmployeeTree AS (
    SELECT 
        EmployeeID,
        ManagerID,
        0 AS Level
    FROM Employees
    WHERE ManagerID IS NULL
    
    UNION ALL
    
    SELECT 
        e.EmployeeID,
        e.ManagerID,
        et.Level + 1
    FROM Employees e
    INNER JOIN EmployeeTree et ON e.ManagerID = et.EmployeeID
)
SELECT 
    m.EmployeeID AS ManagerID,
    m.FirstName + ' ' + m.LastName AS ManagerName,
    COUNT(et.EmployeeID) - 1 AS TotalSubordinates
FROM Employees m
LEFT JOIN EmployeeTree et ON m.EmployeeID IN (
    SELECT EmployeeID FROM EmployeeTree WHERE ManagerID = m.EmployeeID
)
GROUP BY m.EmployeeID, m.FirstName, m.LastName
HAVING COUNT(et.EmployeeID) > 1;
```

### ?? Q43: Generate number sequence 1 to 100
```sql
WITH Numbers AS (
    SELECT 1 AS n
    UNION ALL
    SELECT n + 1 FROM Numbers WHERE n < 100
)
SELECT n FROM Numbers
OPTION (MAXRECURSION 100);
```

### ?? Q44: Find all ancestors of an employee
```sql
WITH Ancestors AS (
    SELECT 
        e.EmployeeID,
        e.FirstName + ' ' + e.LastName AS EmployeeName,
        m.EmployeeID AS AncestorID,
        m.FirstName + ' ' + m.LastName AS AncestorName,
        1 AS Distance
    FROM Employees e
    INNER JOIN Employees m ON e.ManagerID = m.EmployeeID
    WHERE e.EmployeeID = 10  -- Specific employee
    
    UNION ALL
    
    SELECT 
        a.EmployeeID,
        a.EmployeeName,
        m.EmployeeID,
        m.FirstName + ' ' + m.LastName,
        a.Distance + 1
    FROM Ancestors a
    INNER JOIN Employees m ON a.AncestorID = m.ManagerID
)
SELECT * FROM Ancestors;
```

### ?? Q45: Generate date series for current month
```sql
WITH DateSeries AS (
    SELECT DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1) AS DateValue
    UNION ALL
    SELECT DATEADD(DAY, 1, DateValue)
    FROM DateSeries
    WHERE DateValue < EOMONTH(GETDATE())
)
SELECT DateValue, DATENAME(WEEKDAY, DateValue) AS DayName
FROM DateSeries
OPTION (MAXRECURSION 31);
```

---

## ?? Business Logic & Complex Scenarios (5 Questions)

### ?? Q46: Calculate employee bonus based on tenure
```sql
SELECT 
    EmployeeID,
    FirstName + ' ' + LastName AS FullName,
    Salary,
    DATEDIFF(YEAR, HireDate, GETDATE()) AS Years,
    CASE 
        WHEN DATEDIFF(YEAR, HireDate, GETDATE()) >= 10 THEN Salary * 0.15
        WHEN DATEDIFF(YEAR, HireDate, GETDATE()) >= 5 THEN Salary * 0.10
        WHEN DATEDIFF(YEAR, HireDate, GETDATE()) >= 2 THEN Salary * 0.05
        ELSE 0
    END AS Bonus
FROM Employees;
```

### ?? Q47: Department budget utilization report
```sql
SELECT 
    d.DepartmentName,
    d.Budget AS AllocatedBudget,
    ISNULL(SUM(e.Salary), 0) AS UsedBudget,
    d.Budget - ISNULL(SUM(e.Salary), 0) AS RemainingBudget,
    CAST((ISNULL(SUM(e.Salary), 0) * 100.0 / d.Budget) AS DECIMAL(5,2)) AS UtilizationPct,
    CASE 
        WHEN ISNULL(SUM(e.Salary), 0) > d.Budget THEN 'Over Budget'
        WHEN ISNULL(SUM(e.Salary), 0) > d.Budget * 0.90 THEN 'Near Limit'
        ELSE 'Within Budget'
    END AS Status
FROM Departments d
LEFT JOIN Employees e ON d.DepartmentID = e.DepartmentID AND e.IsActive = 1
GROUP BY d.DepartmentID, d.DepartmentName, d.Budget;
```

### ?? Q48: Pivot salaries by department and year
```sql
SELECT 
    DepartmentID,
    [2021], [2022], [2023], [2024]
FROM (
    SELECT 
        DepartmentID,
        YEAR(HireDate) AS HireYear,
        Salary
    FROM Employees
) AS SourceTable
PIVOT (
    AVG(Salary)
    FOR HireYear IN ([2021], [2022], [2023], [2024])
) AS PivotTable;
```

### ?? Q49: Find employees earning more than their manager
```sql
SELECT 
    e.FirstName + ' ' + e.LastName AS Employee,
    e.Salary AS EmployeeSalary,
    m.FirstName + ' ' + m.LastName AS Manager,
    m.Salary AS ManagerSalary
FROM Employees e
INNER JOIN Employees m ON e.ManagerID = m.EmployeeID
WHERE e.Salary > m.Salary;
```

### ?? Q50: Year-over-year salary growth analysis
```sql
WITH YearlySalary AS (
    SELECT 
        EmployeeID,
        YEAR(ChangeDate) AS Year,
        MAX(NewSalary) AS YearEndSalary
    FROM SalaryHistory
    GROUP BY EmployeeID, YEAR(ChangeDate)
)
SELECT 
    EmployeeID,
    Year,
    YearEndSalary,
    LAG(YearEndSalary) OVER (PARTITION BY EmployeeID ORDER BY Year) AS PrevYear,
    YearEndSalary - LAG(YearEndSalary) OVER (PARTITION BY EmployeeID ORDER BY Year) AS YoYGrowth,
    CAST(
        ((YearEndSalary - LAG(YearEndSalary) OVER (PARTITION BY EmployeeID ORDER BY Year)) * 100.0 / 
        LAG(YearEndSalary) OVER (PARTITION BY EmployeeID ORDER BY Year))
        AS DECIMAL(5,2)
    ) AS YoYPercentage
FROM YearlySalary;
```

---

## ?? Interview Tips for These Questions

### Preparation Strategy
1. **Master the patterns** - Most questions are variations of these 50
2. **Practice explaining** - Talk through your approach
3. **Handle edge cases** - NULLs, empty sets, duplicates
4. **Optimize** - Always consider performance
5. **Ask questions** - Clarify requirements before coding

### Common Follow-ups
- "How would you optimize this query?"
- "What if there are millions of records?"
- "What indexes would you create?"
- "How do you handle NULL values?"
- "Can you write this without a subquery?"

### Time Management
- ?? Easy: 2-3 minutes
- ?? Medium: 5-7 minutes  
- ?? Hard: 10-15 minutes

### Remember
? Write clean, readable SQL
? Use meaningful aliases
? Comment complex logic
? Consider NULL values
? Think about performance
? Explain your approach
? Test edge cases

---

## ?? Master These 50 Questions

If you can solve all 50 questions confidently and explain your approach, you're ready for 90% of SQL interviews!

**Practice daily and good luck!** ??
