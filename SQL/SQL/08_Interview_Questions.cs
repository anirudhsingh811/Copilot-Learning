using System;

namespace SQLPreparation
{
    /// <summary>
    /// Day 14: Real-World Interview Questions and Scenarios
    /// Covers: Common interview questions with solutions and explanations
    /// </summary>
    public class Interview_Questions
    {
        public static void RunAllDemos()
        {
            Console.WriteLine("=== SQL INTERVIEW QUESTIONS AND SCENARIOS ===\n");

            NthHighestSalary();
            DuplicatesAndGaps();
            RankingProblems();
            DateProblems();
            StringManipulation();
            BusinessLogicScenarios();
            PerformanceScenarios();
            DataAnalysis();
        }

        #region Nth Highest Salary
        static void NthHighestSalary()
        {
            Console.WriteLine("1. Nth Highest Salary Problems");
            
            string sql = @"
                -- Q1: Find 2nd highest salary
                
                -- Solution 1: Using subquery
                SELECT MAX(Salary) AS SecondHighestSalary
                FROM Employees
                WHERE Salary < (SELECT MAX(Salary) FROM Employees);

                -- Solution 2: Using ROW_NUMBER
                WITH RankedSalaries AS (
                    SELECT 
                        Salary,
                        ROW_NUMBER() OVER (ORDER BY Salary DESC) AS RowNum
                    FROM Employees
                )
                SELECT Salary AS SecondHighestSalary
                FROM RankedSalaries
                WHERE RowNum = 2;

                -- Solution 3: Using OFFSET
                SELECT DISTINCT Salary AS SecondHighestSalary
                FROM Employees
                ORDER BY Salary DESC
                OFFSET 1 ROWS FETCH NEXT 1 ROWS ONLY;

                -- Solution 4: Using DENSE_RANK (handles ties)
                WITH RankedSalaries AS (
                    SELECT 
                        Salary,
                        DENSE_RANK() OVER (ORDER BY Salary DESC) AS Rank
                    FROM Employees
                )
                SELECT DISTINCT Salary AS SecondHighestSalary
                FROM RankedSalaries
                WHERE Rank = 2;

                -- Q2: Find Nth highest salary (parameterized)
                DECLARE @N INT = 3;

                WITH RankedSalaries AS (
                    SELECT 
                        Salary,
                        DENSE_RANK() OVER (ORDER BY Salary DESC) AS Rank
                    FROM Employees
                )
                SELECT DISTINCT Salary AS NthHighestSalary
                FROM RankedSalaries
                WHERE Rank = @N;

                -- Q3: Nth highest salary per department
                DECLARE @N INT = 2;

                WITH RankedSalaries AS (
                    SELECT 
                        e.DepartmentID,
                        d.DepartmentName,
                        e.Salary,
                        DENSE_RANK() OVER (PARTITION BY e.DepartmentID ORDER BY e.Salary DESC) AS Rank
                    FROM Employees e
                    INNER JOIN Departments d ON e.DepartmentID = d.DepartmentID
                )
                SELECT 
                    DepartmentName,
                    Salary AS NthHighestSalary
                FROM RankedSalaries
                WHERE Rank = @N;

                -- Q4: Top N salaries
                SELECT TOP 5
                    EmployeeID,
                    FirstName + ' ' + LastName AS FullName,
                    Salary
                FROM Employees
                ORDER BY Salary DESC;

                -- Q5: Top N salaries per department
                WITH RankedEmployees AS (
                    SELECT 
                        e.EmployeeID,
                        e.FirstName + ' ' + e.LastName AS FullName,
                        d.DepartmentName,
                        e.Salary,
                        ROW_NUMBER() OVER (PARTITION BY e.DepartmentID ORDER BY e.Salary DESC) AS Rank
                    FROM Employees e
                    INNER JOIN Departments d ON e.DepartmentID = d.DepartmentID
                )
                SELECT 
                    FullName,
                    DepartmentName,
                    Salary
                FROM RankedEmployees
                WHERE Rank <= 3
                ORDER BY DepartmentName, Rank;
            ";

            Console.WriteLine(sql);
        }
        #endregion

        #region Duplicates and Gaps
        static void DuplicatesAndGaps()
        {
            Console.WriteLine("\n2. Find Duplicates and Gaps");
            
            string sql = @"
                -- Q1: Find duplicate emails
                SELECT 
                    Email,
                    COUNT(*) AS DuplicateCount
                FROM Employees
                GROUP BY Email
                HAVING COUNT(*) > 1;

                -- Q2: Find all duplicate records with details
                WITH Duplicates AS (
                    SELECT 
                        *,
                        ROW_NUMBER() OVER (PARTITION BY Email ORDER BY EmployeeID) AS RowNum
                    FROM Employees
                )
                SELECT 
                    EmployeeID,
                    FirstName,
                    LastName,
                    Email
                FROM Duplicates
                WHERE RowNum > 1;

                -- Q3: Delete duplicate records (keep first)
                WITH Duplicates AS (
                    SELECT 
                        *,
                        ROW_NUMBER() OVER (PARTITION BY Email ORDER BY EmployeeID) AS RowNum
                    FROM Employees
                )
                DELETE FROM Duplicates
                WHERE RowNum > 1;

                -- Q4: Find gaps in sequence
                WITH SequenceCheck AS (
                    SELECT 
                        EmployeeID,
                        LAG(EmployeeID) OVER (ORDER BY EmployeeID) AS PrevID
                    FROM Employees
                )
                SELECT 
                    PrevID + 1 AS GapStart,
                    EmployeeID - 1 AS GapEnd,
                    EmployeeID - PrevID - 1 AS GapSize
                FROM SequenceCheck
                WHERE EmployeeID - PrevID > 1;

                -- Q5: Find missing dates in a range
                WITH AllDates AS (
                    SELECT CAST('2024-01-01' AS DATE) AS DateValue
                    UNION ALL
                    SELECT DATEADD(DAY, 1, DateValue)
                    FROM AllDates
                    WHERE DateValue < '2024-01-31'
                ),
                ExistingDates AS (
                    SELECT DISTINCT CAST(HireDate AS DATE) AS HireDate
                    FROM Employees
                    WHERE HireDate BETWEEN '2024-01-01' AND '2024-01-31'
                )
                SELECT ad.DateValue AS MissingDate
                FROM AllDates ad
                LEFT JOIN ExistingDates ed ON ad.DateValue = ed.HireDate
                WHERE ed.HireDate IS NULL
                OPTION (MAXRECURSION 365);

                -- Q6: Find consecutive sequences
                WITH NumberedRows AS (
                    SELECT 
                        EmployeeID,
                        HireDate,
                        ROW_NUMBER() OVER (ORDER BY HireDate) AS RowNum,
                        DATEADD(DAY, -ROW_NUMBER() OVER (ORDER BY HireDate), HireDate) AS GroupDate
                    FROM Employees
                )
                SELECT 
                    MIN(HireDate) AS SequenceStart,
                    MAX(HireDate) AS SequenceEnd,
                    COUNT(*) AS ConsecutiveDays
                FROM NumberedRows
                GROUP BY GroupDate
                HAVING COUNT(*) >= 3;

                -- Q7: Self-join to find duplicates
                SELECT DISTINCT
                    e1.EmployeeID,
                    e1.FirstName,
                    e1.LastName,
                    e1.Email
                FROM Employees e1
                INNER JOIN Employees e2 
                    ON e1.Email = e2.Email 
                    AND e1.EmployeeID < e2.EmployeeID;
            ";

            Console.WriteLine(sql);
        }
        #endregion

        #region Ranking Problems
        static void RankingProblems()
        {
            Console.WriteLine("\n3. Ranking and Grouping Problems");
            
            string sql = @"
                -- Q1: Rank employees by salary (handle ties)
                SELECT 
                    EmployeeID,
                    FirstName + ' ' + LastName AS FullName,
                    Salary,
                    ROW_NUMBER() OVER (ORDER BY Salary DESC) AS RowNum,
                    RANK() OVER (ORDER BY Salary DESC) AS Rank,
                    DENSE_RANK() OVER (ORDER BY Salary DESC) AS DenseRank
                FROM Employees;

                -- Q2: Find employees earning more than department average
                WITH DeptAvg AS (
                    SELECT 
                        DepartmentID,
                        AVG(Salary) AS AvgSalary
                    FROM Employees
                    GROUP BY DepartmentID
                )
                SELECT 
                    e.EmployeeID,
                    e.FirstName + ' ' + e.LastName AS FullName,
                    d.DepartmentName,
                    e.Salary,
                    da.AvgSalary,
                    e.Salary - da.AvgSalary AS DifferenceFromAvg
                FROM Employees e
                INNER JOIN Departments d ON e.DepartmentID = d.DepartmentID
                INNER JOIN DeptAvg da ON e.DepartmentID = da.DepartmentID
                WHERE e.Salary > da.AvgSalary
                ORDER BY d.DepartmentName, e.Salary DESC;

                -- Q3: Running total by department
                SELECT 
                    e.DepartmentID,
                    d.DepartmentName,
                    e.FirstName + ' ' + e.LastName AS FullName,
                    e.Salary,
                    SUM(e.Salary) OVER (
                        PARTITION BY e.DepartmentID 
                        ORDER BY e.HireDate
                        ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW
                    ) AS RunningTotal
                FROM Employees e
                INNER JOIN Departments d ON e.DepartmentID = d.DepartmentID
                ORDER BY e.DepartmentID, e.HireDate;

                -- Q4: Find top 3 products in each category
                /*
                Assuming Products table with columns: ProductID, CategoryID, ProductName, Sales
                */
                /*
                WITH RankedProducts AS (
                    SELECT 
                        ProductID,
                        CategoryID,
                        ProductName,
                        Sales,
                        ROW_NUMBER() OVER (PARTITION BY CategoryID ORDER BY Sales DESC) AS Rank
                    FROM Products
                )
                SELECT 
                    CategoryID,
                    ProductName,
                    Sales
                FROM RankedProducts
                WHERE Rank <= 3;
                */

                -- Q5: Percentile calculation
                SELECT 
                    EmployeeID,
                    FirstName + ' ' + LastName AS FullName,
                    Salary,
                    PERCENT_RANK() OVER (ORDER BY Salary) AS PercentRank,
                    CUME_DIST() OVER (ORDER BY Salary) AS CumulativeDistribution,
                    NTILE(4) OVER (ORDER BY Salary) AS Quartile
                FROM Employees;

                -- Q6: Year-over-year growth
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
                    LAG(YearEndSalary) OVER (PARTITION BY EmployeeID ORDER BY Year) AS PreviousYearSalary,
                    YearEndSalary - LAG(YearEndSalary) OVER (PARTITION BY EmployeeID ORDER BY Year) AS YoYGrowth,
                    CASE 
                        WHEN LAG(YearEndSalary) OVER (PARTITION BY EmployeeID ORDER BY Year) IS NOT NULL
                        THEN CAST(((YearEndSalary - LAG(YearEndSalary) OVER (PARTITION BY EmployeeID ORDER BY Year)) * 100.0 / 
                              LAG(YearEndSalary) OVER (PARTITION BY EmployeeID ORDER BY Year)) AS DECIMAL(5,2))
                    END AS YoYPercentage
                FROM YearlySalary;
            ";

            Console.WriteLine(sql);
        }
        #endregion

        #region Date Problems
        static void DateProblems()
        {
            Console.WriteLine("\n4. Date and Time Problems");
            
            string sql = @"
                -- Q1: Find employees hired in last 90 days
                SELECT 
                    EmployeeID,
                    FirstName + ' ' + LastName AS FullName,
                    HireDate,
                    DATEDIFF(DAY, HireDate, GETDATE()) AS DaysEmployed
                FROM Employees
                WHERE HireDate >= DATEADD(DAY, -90, GETDATE());

                -- Q2: Find employees by hire year
                SELECT 
                    YEAR(HireDate) AS HireYear,
                    COUNT(*) AS EmployeesHired,
                    AVG(Salary) AS AvgStartingSalary
                FROM Employees
                GROUP BY YEAR(HireDate)
                ORDER BY HireYear;

                -- Q3: Get first and last day of month
                SELECT 
                    EmployeeID,
                    HireDate,
                    DATEFROMPARTS(YEAR(HireDate), MONTH(HireDate), 1) AS FirstDayOfMonth,
                    EOMONTH(HireDate) AS LastDayOfMonth
                FROM Employees;

                -- Q4: Calculate tenure
                SELECT 
                    EmployeeID,
                    FirstName + ' ' + LastName AS FullName,
                    HireDate,
                    DATEDIFF(YEAR, HireDate, GETDATE()) AS YearsOfService,
                    DATEDIFF(MONTH, HireDate, GETDATE()) % 12 AS AdditionalMonths,
                    DATEDIFF(DAY, HireDate, GETDATE()) AS TotalDays
                FROM Employees;

                -- Q5: Find employees with anniversary this month
                SELECT 
                    EmployeeID,
                    FirstName + ' ' + LastName AS FullName,
                    HireDate,
                    DATEDIFF(YEAR, HireDate, GETDATE()) AS YearsOfService
                FROM Employees
                WHERE MONTH(HireDate) = MONTH(GETDATE())
                ORDER BY DAY(HireDate);

                -- Q6: Business days between dates (excluding weekends)
                DECLARE @StartDate DATE = '2024-01-01';
                DECLARE @EndDate DATE = '2024-01-31';

                WITH DateRange AS (
                    SELECT @StartDate AS DateValue
                    UNION ALL
                    SELECT DATEADD(DAY, 1, DateValue)
                    FROM DateRange
                    WHERE DateValue < @EndDate
                )
                SELECT 
                    COUNT(*) AS BusinessDays
                FROM DateRange
                WHERE DATEPART(WEEKDAY, DateValue) NOT IN (1, 7)  -- Not Sunday or Saturday
                OPTION (MAXRECURSION 365);

                -- Q7: Quarter and fiscal year
                SELECT 
                    EmployeeID,
                    HireDate,
                    DATEPART(QUARTER, HireDate) AS Quarter,
                    YEAR(HireDate) AS CalendarYear,
                    CASE 
                        WHEN MONTH(HireDate) >= 4 THEN YEAR(HireDate)
                        ELSE YEAR(HireDate) - 1
                    END AS FiscalYear  -- Assuming fiscal year starts in April
                FROM Employees;

                -- Q8: Age calculation from birthdate
                /*
                Assuming Employees has BirthDate column
                SELECT 
                    EmployeeID,
                    BirthDate,
                    DATEDIFF(YEAR, BirthDate, GETDATE()) - 
                        CASE 
                            WHEN MONTH(BirthDate) > MONTH(GETDATE()) OR 
                                 (MONTH(BirthDate) = MONTH(GETDATE()) AND DAY(BirthDate) > DAY(GETDATE()))
                            THEN 1
                            ELSE 0
                        END AS Age
                FROM Employees;
                */

                -- Q9: Days between consecutive hires
                SELECT 
                    EmployeeID,
                    FirstName + ' ' + LastName AS FullName,
                    HireDate,
                    LAG(HireDate) OVER (ORDER BY HireDate) AS PreviousHireDate,
                    DATEDIFF(DAY, LAG(HireDate) OVER (ORDER BY HireDate), HireDate) AS DaysSincePreviousHire
                FROM Employees
                ORDER BY HireDate;

                -- Q10: Working days in current month
                WITH DateSeries AS (
                    SELECT DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1) AS DateValue
                    UNION ALL
                    SELECT DATEADD(DAY, 1, DateValue)
                    FROM DateSeries
                    WHERE DateValue < EOMONTH(GETDATE())
                )
                SELECT 
                    COUNT(*) AS WorkingDaysInMonth
                FROM DateSeries
                WHERE DATEPART(WEEKDAY, DateValue) NOT IN (1, 7)
                OPTION (MAXRECURSION 31);
            ";

            Console.WriteLine(sql);
        }
        #endregion

        #region String Manipulation
        static void StringManipulation()
        {
            Console.WriteLine("\n5. String Manipulation Problems");
            
            string sql = @"
                -- Q1: Split full name into first and last
                SELECT 
                    EmployeeID,
                    FirstName + ' ' + LastName AS FullName,
                    LEFT(FirstName + ' ' + LastName, CHARINDEX(' ', FirstName + ' ' + LastName) - 1) AS First,
                    RIGHT(FirstName + ' ' + LastName, LEN(FirstName + ' ' + LastName) - CHARINDEX(' ', FirstName + ' ' + LastName)) AS Last
                FROM Employees;

                -- Q2: Capitalize first letter of each word
                SELECT 
                    EmployeeID,
                    UPPER(LEFT(FirstName, 1)) + LOWER(SUBSTRING(FirstName, 2, LEN(FirstName))) AS ProperFirstName,
                    UPPER(LEFT(LastName, 1)) + LOWER(SUBSTRING(LastName, 2, LEN(LastName))) AS ProperLastName
                FROM Employees;

                -- Q3: Extract domain from email
                SELECT 
                    EmployeeID,
                    Email,
                    RIGHT(Email, LEN(Email) - CHARINDEX('@', Email)) AS EmailDomain
                FROM Employees;

                -- Q4: Count words in a string
                SELECT 
                    EmployeeID,
                    FirstName + ' ' + LastName AS FullName,
                    LEN(FirstName + ' ' + LastName) - LEN(REPLACE(FirstName + ' ' + LastName, ' ', '')) + 1 AS WordCount
                FROM Employees;

                -- Q5: Remove special characters
                SELECT 
                    EmployeeID,
                    Email,
                    REPLACE(REPLACE(REPLACE(Email, '.', ''), '-', ''), '_', '') AS CleanedEmail
                FROM Employees;

                -- Q6: Pad string with zeros
                SELECT 
                    EmployeeID,
                    RIGHT('00000' + CAST(EmployeeID AS VARCHAR(10)), 5) AS PaddedID
                FROM Employees;

                -- Q7: Reverse string (recursive CTE)
                DECLARE @String VARCHAR(100) = 'Hello World';

                WITH ReversedString AS (
                    SELECT 
                        LEN(@String) AS Position,
                        SUBSTRING(@String, LEN(@String), 1) AS Character
                    UNION ALL
                    SELECT 
                        Position - 1,
                        SUBSTRING(@String, Position - 1, 1)
                    FROM ReversedString
                    WHERE Position > 1
                )
                SELECT STRING_AGG(Character, '') WITHIN GROUP (ORDER BY Position DESC) AS Reversed
                FROM ReversedString;

                -- Q8: Find and replace multiple values
                SELECT 
                    EmployeeID,
                    FirstName,
                    REPLACE(REPLACE(REPLACE(FirstName, 'John', 'Jonathan'), 'Bob', 'Robert'), 'Mike', 'Michael') AS ExpandedName
                FROM Employees;

                -- Q9: Concatenate with separator (using STRING_AGG)
                SELECT 
                    DepartmentID,
                    STRING_AGG(FirstName + ' ' + LastName, ', ') AS EmployeeList
                FROM Employees
                GROUP BY DepartmentID;

                -- Q10: Check if string contains substring
                SELECT 
                    EmployeeID,
                    Email,
                    CASE 
                        WHEN Email LIKE '%@gmail.com' THEN 'Gmail'
                        WHEN Email LIKE '%@yahoo.com' THEN 'Yahoo'
                        WHEN Email LIKE '%@company.com' THEN 'Company'
                        ELSE 'Other'
                    END AS EmailProvider
                FROM Employees;

                -- Q11: Split string into multiple rows (using STRING_SPLIT - SQL Server 2016+)
                /*
                SELECT 
                    value AS Item
                FROM STRING_SPLIT('Apple,Banana,Orange,Mango', ',');
                */

                -- Q12: Get initials
                SELECT 
                    EmployeeID,
                    FirstName + ' ' + LastName AS FullName,
                    LEFT(FirstName, 1) + LEFT(LastName, 1) AS Initials
                FROM Employees;
            ";

            Console.WriteLine(sql);
        }
        #endregion

        #region Business Logic Scenarios
        static void BusinessLogicScenarios()
        {
            Console.WriteLine("\n6. Business Logic and Real-World Scenarios");
            
            string sql = @"
                -- Q1: Calculate employee bonus based on performance
                SELECT 
                    e.EmployeeID,
                    e.FirstName + ' ' + e.LastName AS FullName,
                    e.Salary,
                    DATEDIFF(YEAR, e.HireDate, GETDATE()) AS YearsOfService,
                    CASE 
                        WHEN DATEDIFF(YEAR, e.HireDate, GETDATE()) >= 10 THEN e.Salary * 0.15
                        WHEN DATEDIFF(YEAR, e.HireDate, GETDATE()) >= 5 THEN e.Salary * 0.10
                        WHEN DATEDIFF(YEAR, e.HireDate, GETDATE()) >= 2 THEN e.Salary * 0.05
                        ELSE 0
                    END AS Bonus
                FROM Employees e
                WHERE e.IsActive = 1;

                -- Q2: Department budget utilization
                SELECT 
                    d.DepartmentID,
                    d.DepartmentName,
                    d.Budget AS AllocatedBudget,
                    ISNULL(SUM(e.Salary), 0) AS UsedBudget,
                    d.Budget - ISNULL(SUM(e.Salary), 0) AS RemainingBudget,
                    CAST((ISNULL(SUM(e.Salary), 0) * 100.0 / d.Budget) AS DECIMAL(5,2)) AS UtilizationPercentage,
                    CASE 
                        WHEN ISNULL(SUM(e.Salary), 0) > d.Budget THEN 'Over Budget'
                        WHEN ISNULL(SUM(e.Salary), 0) > d.Budget * 0.90 THEN 'Near Limit'
                        ELSE 'Within Budget'
                    END AS BudgetStatus
                FROM Departments d
                LEFT JOIN Employees e ON d.DepartmentID = e.DepartmentID AND e.IsActive = 1
                GROUP BY d.DepartmentID, d.DepartmentName, d.Budget;

                -- Q3: Employee hierarchy depth
                WITH EmployeeHierarchy AS (
                    SELECT 
                        EmployeeID,
                        FirstName + ' ' + LastName AS FullName,
                        ManagerID,
                        0 AS Level
                    FROM Employees
                    WHERE ManagerID IS NULL
                    
                    UNION ALL
                    
                    SELECT 
                        e.EmployeeID,
                        e.FirstName + ' ' + e.LastName,
                        e.ManagerID,
                        eh.Level + 1
                    FROM Employees e
                    INNER JOIN EmployeeHierarchy eh ON e.ManagerID = eh.EmployeeID
                )
                SELECT 
                    FullName,
                    Level,
                    CASE 
                        WHEN Level = 0 THEN 'C-Level'
                        WHEN Level = 1 THEN 'Director'
                        WHEN Level = 2 THEN 'Manager'
                        ELSE 'Staff'
                    END AS Role
                FROM EmployeeHierarchy
                ORDER BY Level, FullName;

                -- Q4: Project resource allocation
                SELECT 
                    p.ProjectID,
                    p.ProjectName,
                    p.Budget AS ProjectBudget,
                    COUNT(DISTINCT ep.EmployeeID) AS AssignedEmployees,
                    SUM(ep.HoursAllocated) AS TotalHoursAllocated,
                    AVG(e.Salary) AS AvgEmployeeSalary,
                    CASE 
                        WHEN COUNT(DISTINCT ep.EmployeeID) = 0 THEN 'Not Started'
                        WHEN SUM(ep.HoursAllocated) < 100 THEN 'Under-resourced'
                        WHEN SUM(ep.HoursAllocated) BETWEEN 100 AND 500 THEN 'Well-resourced'
                        ELSE 'Over-resourced'
                    END AS ResourceStatus
                FROM Projects p
                LEFT JOIN EmployeeProjects ep ON p.ProjectID = ep.ProjectID
                LEFT JOIN Employees e ON ep.EmployeeID = e.EmployeeID
                WHERE p.Status = 'Active'
                GROUP BY p.ProjectID, p.ProjectName, p.Budget;

                -- Q5: Customer cohort analysis (adapt for your schema)
                /*
                WITH MonthlySignups AS (
                    SELECT 
                        CustomerID,
                        DATEFROMPARTS(YEAR(SignupDate), MONTH(SignupDate), 1) AS CohortMonth,
                        SignupDate
                    FROM Customers
                ),
                MonthlyActivity AS (
                    SELECT 
                        o.CustomerID,
                        DATEFROMPARTS(YEAR(o.OrderDate), MONTH(o.OrderDate), 1) AS ActivityMonth,
                        SUM(o.OrderAmount) AS MonthlySpend
                    FROM Orders o
                    GROUP BY o.CustomerID, DATEFROMPARTS(YEAR(o.OrderDate), MONTH(o.OrderDate), 1)
                )
                SELECT 
                    ms.CohortMonth,
                    ma.ActivityMonth,
                    DATEDIFF(MONTH, ms.CohortMonth, ma.ActivityMonth) AS MonthNumber,
                    COUNT(DISTINCT ms.CustomerID) AS CohortSize,
                    COUNT(DISTINCT ma.CustomerID) AS ActiveCustomers,
                    CAST(COUNT(DISTINCT ma.CustomerID) * 100.0 / COUNT(DISTINCT ms.CustomerID) AS DECIMAL(5,2)) AS RetentionRate,
                    AVG(ma.MonthlySpend) AS AvgSpend
                FROM MonthlySignups ms
                LEFT JOIN MonthlyActivity ma ON ms.CustomerID = ma.CustomerID
                GROUP BY ms.CohortMonth, ma.ActivityMonth
                ORDER BY ms.CohortMonth, ma.ActivityMonth;
                */

                -- Q6: Salary distribution by quartile
                WITH SalaryQuartiles AS (
                    SELECT 
                        EmployeeID,
                        FirstName + ' ' + LastName AS FullName,
                        Salary,
                        NTILE(4) OVER (ORDER BY Salary) AS Quartile
                    FROM Employees
                )
                SELECT 
                    Quartile,
                    COUNT(*) AS EmployeeCount,
                    MIN(Salary) AS MinSalary,
                    MAX(Salary) AS MaxSalary,
                    AVG(Salary) AS AvgSalary
                FROM SalaryQuartiles
                GROUP BY Quartile
                ORDER BY Quartile;

                -- Q7: Find managers with most direct reports
                SELECT 
                    m.EmployeeID AS ManagerID,
                    m.FirstName + ' ' + m.LastName AS ManagerName,
                    COUNT(e.EmployeeID) AS DirectReports,
                    AVG(e.Salary) AS AvgTeamSalary,
                    SUM(e.Salary) AS TotalTeamSalary
                FROM Employees m
                INNER JOIN Employees e ON m.EmployeeID = e.ManagerID
                GROUP BY m.EmployeeID, m.FirstName, m.LastName
                HAVING COUNT(e.EmployeeID) >= 3
                ORDER BY DirectReports DESC;
            ";

            Console.WriteLine(sql);
        }
        #endregion

        #region Performance Scenarios
        static void PerformanceScenarios()
        {
            Console.WriteLine("\n7. Performance Optimization Scenarios");
            
            string sql = @"
                -- Q1: Optimize this slow query
                -- BAD:
                SELECT *
                FROM Employees
                WHERE YEAR(HireDate) = 2023
                    AND UPPER(FirstName) LIKE '%JOHN%';

                -- GOOD:
                SELECT EmployeeID, FirstName, LastName, Email, HireDate, Salary
                FROM Employees
                WHERE HireDate >= '2023-01-01' AND HireDate < '2024-01-01'
                    AND FirstName LIKE '%John%';

                -- Q2: Rewrite subquery as JOIN
                -- BAD:
                SELECT 
                    EmployeeID,
                    FirstName,
                    LastName,
                    (SELECT DepartmentName FROM Departments WHERE DepartmentID = Employees.DepartmentID) AS DeptName,
                    (SELECT COUNT(*) FROM EmployeeProjects WHERE EmployeeID = Employees.EmployeeID) AS ProjectCount
                FROM Employees;

                -- GOOD:
                SELECT 
                    e.EmployeeID,
                    e.FirstName,
                    e.LastName,
                    d.DepartmentName,
                    COUNT(ep.ProjectID) AS ProjectCount
                FROM Employees e
                LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
                LEFT JOIN EmployeeProjects ep ON e.EmployeeID = ep.EmployeeID
                GROUP BY e.EmployeeID, e.FirstName, e.LastName, d.DepartmentName;

                -- Q3: Pagination without OFFSET (better performance)
                -- BAD (slow for large offsets):
                SELECT EmployeeID, FirstName, LastName
                FROM Employees
                ORDER BY EmployeeID
                OFFSET 100000 ROWS FETCH NEXT 10 ROWS ONLY;

                -- GOOD (keyset pagination):
                DECLARE @LastID INT = 100000;
                SELECT TOP 10 EmployeeID, FirstName, LastName
                FROM Employees
                WHERE EmployeeID > @LastID
                ORDER BY EmployeeID;

                -- Q4: Batch large updates
                DECLARE @BatchSize INT = 1000;
                DECLARE @RowsAffected INT = @BatchSize;

                WHILE @RowsAffected >= @BatchSize
                BEGIN
                    UPDATE TOP (@BatchSize) Employees
                    SET ModifiedDate = GETDATE()
                    WHERE ModifiedDate IS NULL;
                    
                    SET @RowsAffected = @@ROWCOUNT;
                    
                    WAITFOR DELAY '00:00:01';  -- Prevent blocking
                END;

                -- Q5: Use EXISTS instead of COUNT
                -- BAD:
                IF (SELECT COUNT(*) FROM Employees WHERE DepartmentID = 1) > 0
                    PRINT 'Department has employees';

                -- GOOD:
                IF EXISTS (SELECT 1 FROM Employees WHERE DepartmentID = 1)
                    PRINT 'Department has employees';

                -- Q6: Avoid DISTINCT when not needed
                -- BAD:
                SELECT DISTINCT e.EmployeeID, e.FirstName, e.LastName
                FROM Employees e
                INNER JOIN Departments d ON e.DepartmentID = d.DepartmentID;

                -- GOOD (if EmployeeID is unique):
                SELECT e.EmployeeID, e.FirstName, e.LastName
                FROM Employees e
                INNER JOIN Departments d ON e.DepartmentID = d.DepartmentID;
            ";

            Console.WriteLine(sql);
        }
        #endregion

        #region Data Analysis
        static void DataAnalysis()
        {
            Console.WriteLine("\n8. Data Analysis Questions");
            
            string sql = @"
                -- Q1: Moving average of salaries
                SELECT 
                    EmployeeID,
                    FirstName + ' ' + LastName AS FullName,
                    HireDate,
                    Salary,
                    AVG(Salary) OVER (
                        ORDER BY HireDate 
                        ROWS BETWEEN 2 PRECEDING AND CURRENT ROW
                    ) AS MovingAvg3
                FROM Employees
                ORDER BY HireDate;

                -- Q2: Cumulative sum by department
                SELECT 
                    e.DepartmentID,
                    d.DepartmentName,
                    e.FirstName + ' ' + e.LastName AS FullName,
                    e.HireDate,
                    e.Salary,
                    SUM(e.Salary) OVER (
                        PARTITION BY e.DepartmentID 
                        ORDER BY e.HireDate
                        ROWS UNBOUNDED PRECEDING
                    ) AS CumulativeSalary
                FROM Employees e
                INNER JOIN Departments d ON e.DepartmentID = d.DepartmentID
                ORDER BY e.DepartmentID, e.HireDate;

                -- Q3: Percentage contribution to total
                SELECT 
                    d.DepartmentName,
                    SUM(e.Salary) AS DeptTotal,
                    SUM(SUM(e.Salary)) OVER () AS CompanyTotal,
                    CAST(SUM(e.Salary) * 100.0 / SUM(SUM(e.Salary)) OVER () AS DECIMAL(5,2)) AS PercentOfTotal
                FROM Employees e
                INNER JOIN Departments d ON e.DepartmentID = d.DepartmentID
                GROUP BY d.DepartmentName;

                -- Q4: Statistical measures
                SELECT 
                    DepartmentID,
                    COUNT(*) AS EmployeeCount,
                    AVG(Salary) AS AvgSalary,
                    MIN(Salary) AS MinSalary,
                    MAX(Salary) AS MaxSalary,
                    STDEV(Salary) AS StdDeviation,
                    VAR(Salary) AS Variance
                FROM Employees
                GROUP BY DepartmentID;

                -- Q5: Pivot table - salaries by department and year
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

                -- Q6: Unpivot example
                /*
                SELECT 
                    DepartmentID,
                    Quarter,
                    Revenue
                FROM (
                    SELECT DepartmentID, Q1, Q2, Q3, Q4
                    FROM QuarterlyRevenue
                ) AS SourceTable
                UNPIVOT (
                    Revenue FOR Quarter IN (Q1, Q2, Q3, Q4)
                ) AS UnpivotTable;
                */

                -- Q7: Correlation analysis (basic)
                WITH SalaryStats AS (
                    SELECT 
                        AVG(Salary) AS AvgSalary,
                        AVG(CAST(DATEDIFF(YEAR, HireDate, GETDATE()) AS DECIMAL(10,2))) AS AvgTenure
                    FROM Employees
                )
                SELECT 
                    e.EmployeeID,
                    e.Salary - ss.AvgSalary AS SalaryDeviation,
                    DATEDIFF(YEAR, e.HireDate, GETDATE()) - ss.AvgTenure AS TenureDeviation
                FROM Employees e
                CROSS JOIN SalaryStats ss;
            ";

            Console.WriteLine(sql);
        }
        #endregion

        #region Interview Tips
        /*
        TOP INTERVIEW TIPS:

        1. PROBLEM-SOLVING APPROACH:
           - Clarify requirements
           - Discuss edge cases
           - Start with simple solution
           - Optimize if needed
           - Explain your thinking

        2. COMMON QUESTION TYPES:
           - Nth highest value
           - Find duplicates
           - Ranking problems
           - Running totals
           - Date calculations
           - String manipulation
           - JOINs and relationships
           - Performance optimization

        3. KEY SQL CONCEPTS TO KNOW:
           - All JOIN types
           - Window functions
           - CTEs and recursive queries
           - Indexes and performance
           - Transactions and isolation
           - Subqueries vs JOINs
           - Set operations (UNION, INTERSECT, EXCEPT)
           - Aggregate functions
           - CASE expressions
           - Date functions

        4. PERFORMANCE CONSIDERATIONS:
           - Avoid SELECT *
           - Use appropriate indexes
           - Minimize subqueries
           - Use EXISTS instead of IN for large datasets
           - Avoid functions on indexed columns in WHERE
           - Consider execution plans
           - Batch large operations

        5. BEST PRACTICES:
           - Use meaningful aliases
           - Format SQL readable
           - Handle NULLs appropriately
           - Use transactions for data integrity
           - Parameterize queries
           - Comment complex logic
           - Test edge cases

        6. RED FLAGS TO AVOID:
           - Not checking for NULL values
           - Using SELECT * in production code
           - Not considering performance
           - Ignoring data types
           - Not handling errors
           - Using cursors when set-based solution exists

        7. COMMUNICATION:
           - Explain your approach
           - Discuss trade-offs
           - Ask clarifying questions
           - Think out loud
           - Be honest about limitations

        8. PRACTICE AREAS:
           - LeetCode SQL problems
           - HackerRank SQL challenges
           - Real database schemas
           - Performance tuning
           - Writing stored procedures
           - Complex business logic
        */
        #endregion
    }
}
