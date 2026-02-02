using System;
using System.Data.SqlClient;

namespace SQLPreparation
{
    /// <summary>
    /// Day 3-4: JOINs and Subqueries - Complex Query Patterns
    /// Covers: All JOIN types, Subqueries, Correlated Subqueries, Derived Tables
    /// </summary>
    public class Joins_Subqueries
    {
        private static string connectionString = "Server=localhost;Database=SQLDemo;Integrated Security=true;TrustServerCertificate=true;";

        public static void RunAllDemos()
        {
            Console.WriteLine("=== JOINS AND SUBQUERIES DEMO ===\n");

            InnerJoin();
            LeftRightJoin();
            FullOuterJoin();
            CrossJoin();
            SelfJoin();
            MultipleJoins();
            Subqueries();
            CorrelatedSubqueries();
            DerivedTables();
        }

        #region INNER JOIN
        static void InnerJoin()
        {
            Console.WriteLine("1. INNER JOIN - Returns matching records from both tables");
            
            string sql = @"
                -- Basic INNER JOIN
                SELECT 
                    e.EmployeeID,
                    e.FirstName,
                    e.LastName,
                    d.DepartmentName
                FROM Employees e
                INNER JOIN Departments d ON e.DepartmentID = d.DepartmentID;

                -- INNER JOIN with multiple conditions
                SELECT 
                    e.EmployeeID,
                    e.FirstName + ' ' + e.LastName AS FullName,
                    d.DepartmentName,
                    e.Salary
                FROM Employees e
                INNER JOIN Departments d 
                    ON e.DepartmentID = d.DepartmentID 
                    AND e.IsActive = 1
                WHERE d.Budget > 500000;

                -- INNER JOIN with aggregate
                SELECT 
                    d.DepartmentName,
                    COUNT(e.EmployeeID) AS EmployeeCount,
                    AVG(e.Salary) AS AvgSalary,
                    SUM(e.Salary) AS TotalSalary
                FROM Departments d
                INNER JOIN Employees e ON d.DepartmentID = e.DepartmentID
                GROUP BY d.DepartmentName
                HAVING COUNT(e.EmployeeID) > 5;

                -- INNER JOIN with ORDER BY
                SELECT 
                    e.FirstName,
                    e.LastName,
                    d.DepartmentName,
                    e.Salary
                FROM Employees e
                INNER JOIN Departments d ON e.DepartmentID = d.DepartmentID
                ORDER BY d.DepartmentName, e.Salary DESC;
            ";

            Console.WriteLine(sql);
        }
        #endregion

        #region LEFT and RIGHT JOIN
        static void LeftRightJoin()
        {
            Console.WriteLine("\n2. LEFT and RIGHT JOIN - Include unmatched records");
            
            string sql = @"
                -- LEFT JOIN (All employees, even without department)
                SELECT 
                    e.EmployeeID,
                    e.FirstName,
                    e.LastName,
                    ISNULL(d.DepartmentName, 'No Department') AS DepartmentName
                FROM Employees e
                LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID;

                -- LEFT JOIN to find unmatched records (employees without department)
                SELECT 
                    e.EmployeeID,
                    e.FirstName,
                    e.LastName
                FROM Employees e
                LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
                WHERE d.DepartmentID IS NULL;

                -- RIGHT JOIN (All departments, even without employees)
                SELECT 
                    ISNULL(e.FirstName + ' ' + e.LastName, 'No Employees') AS Employee,
                    d.DepartmentName
                FROM Employees e
                RIGHT JOIN Departments d ON e.DepartmentID = d.DepartmentID;

                -- LEFT JOIN with aggregate (include departments with 0 employees)
                SELECT 
                    d.DepartmentName,
                    COUNT(e.EmployeeID) AS EmployeeCount,
                    ISNULL(AVG(e.Salary), 0) AS AvgSalary
                FROM Departments d
                LEFT JOIN Employees e ON d.DepartmentID = e.DepartmentID
                GROUP BY d.DepartmentID, d.DepartmentName;

                -- LEFT JOIN with multiple conditions
                SELECT 
                    e.FirstName,
                    e.LastName,
                    p.ProjectName,
                    ep.Role
                FROM Employees e
                LEFT JOIN EmployeeProjects ep ON e.EmployeeID = ep.EmployeeID
                LEFT JOIN Projects p ON ep.ProjectID = p.ProjectID AND p.Status = 'Active'
                WHERE e.IsActive = 1;

                -- Find employees not assigned to any project
                SELECT 
                    e.EmployeeID,
                    e.FirstName,
                    e.LastName
                FROM Employees e
                LEFT JOIN EmployeeProjects ep ON e.EmployeeID = ep.EmployeeID
                WHERE ep.ProjectID IS NULL;
            ";

            Console.WriteLine(sql);
        }
        #endregion

        #region FULL OUTER JOIN
        static void FullOuterJoin()
        {
            Console.WriteLine("\n3. FULL OUTER JOIN - All records from both tables");
            
            string sql = @"
                -- FULL OUTER JOIN
                SELECT 
                    ISNULL(e.FirstName + ' ' + e.LastName, 'N/A') AS Employee,
                    ISNULL(d.DepartmentName, 'N/A') AS Department
                FROM Employees e
                FULL OUTER JOIN Departments d ON e.DepartmentID = d.DepartmentID;

                -- FULL OUTER JOIN to find mismatches
                SELECT 
                    e.EmployeeID,
                    e.FirstName,
                    e.LastName,
                    d.DepartmentID,
                    d.DepartmentName,
                    CASE 
                        WHEN e.EmployeeID IS NULL THEN 'Department without employees'
                        WHEN d.DepartmentID IS NULL THEN 'Employee without department'
                        ELSE 'Match'
                    END AS Status
                FROM Employees e
                FULL OUTER JOIN Departments d ON e.DepartmentID = d.DepartmentID
                WHERE e.EmployeeID IS NULL OR d.DepartmentID IS NULL;

                -- FULL OUTER JOIN with aggregates
                SELECT 
                    COALESCE(d.DepartmentName, 'No Department') AS Department,
                    COUNT(e.EmployeeID) AS EmployeeCount
                FROM Employees e
                FULL OUTER JOIN Departments d ON e.DepartmentID = d.DepartmentID
                GROUP BY d.DepartmentName;
            ";

            Console.WriteLine(sql);
        }
        #endregion

        #region CROSS JOIN
        static void CrossJoin()
        {
            Console.WriteLine("\n4. CROSS JOIN - Cartesian Product");
            
            string sql = @"
                -- Basic CROSS JOIN (all combinations)
                SELECT 
                    e.FirstName,
                    e.LastName,
                    d.DepartmentName
                FROM Employees e
                CROSS JOIN Departments d;

                -- CROSS JOIN for generating combinations
                SELECT 
                    e.EmployeeID,
                    e.FirstName + ' ' + e.LastName AS Employee,
                    p.ProjectName
                FROM Employees e
                CROSS JOIN Projects p
                WHERE NOT EXISTS (
                    SELECT 1 
                    FROM EmployeeProjects ep 
                    WHERE ep.EmployeeID = e.EmployeeID 
                        AND ep.ProjectID = p.ProjectID
                );

                -- CROSS JOIN with number table (generate dates)
                WITH Numbers AS (
                    SELECT TOP 365 ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) - 1 AS n
                    FROM sys.objects s1, sys.objects s2
                )
                SELECT 
                    DATEADD(DAY, n, '2024-01-01') AS DateValue
                FROM Numbers;

                -- CROSS JOIN for all employee-department combinations
                SELECT 
                    e.FirstName + ' ' + e.LastName AS Employee,
                    d.DepartmentName,
                    CASE 
                        WHEN e.DepartmentID = d.DepartmentID THEN 'Current Department'
                        ELSE 'Potential Transfer'
                    END AS Status
                FROM Employees e
                CROSS JOIN Departments d;
            ";

            Console.WriteLine(sql);
        }
        #endregion

        #region SELF JOIN
        static void SelfJoin()
        {
            Console.WriteLine("\n5. SELF JOIN - Join table to itself");
            
            string sql = @"
                -- Find employee and their manager
                SELECT 
                    e.EmployeeID,
                    e.FirstName + ' ' + e.LastName AS Employee,
                    ISNULL(m.FirstName + ' ' + m.LastName, 'No Manager') AS Manager
                FROM Employees e
                LEFT JOIN Employees m ON e.ManagerID = m.EmployeeID;

                -- Find employees in same department
                SELECT DISTINCT
                    e1.FirstName + ' ' + e1.LastName AS Employee1,
                    e2.FirstName + ' ' + e2.LastName AS Employee2,
                    d.DepartmentName
                FROM Employees e1
                INNER JOIN Employees e2 ON e1.DepartmentID = e2.DepartmentID 
                    AND e1.EmployeeID < e2.EmployeeID
                INNER JOIN Departments d ON e1.DepartmentID = d.DepartmentID;

                -- Find employees with same salary
                SELECT 
                    e1.FirstName + ' ' + e1.LastName AS Employee1,
                    e2.FirstName + ' ' + e2.LastName AS Employee2,
                    e1.Salary
                FROM Employees e1
                INNER JOIN Employees e2 ON e1.Salary = e2.Salary 
                    AND e1.EmployeeID < e2.EmployeeID
                ORDER BY e1.Salary DESC;

                -- Hierarchical query (manager chain)
                WITH ManagerHierarchy AS (
                    SELECT 
                        EmployeeID,
                        FirstName + ' ' + LastName AS Name,
                        ManagerID,
                        0 AS Level
                    FROM Employees
                    WHERE ManagerID IS NULL
                    
                    UNION ALL
                    
                    SELECT 
                        e.EmployeeID,
                        e.FirstName + ' ' + e.LastName,
                        e.ManagerID,
                        mh.Level + 1
                    FROM Employees e
                    INNER JOIN ManagerHierarchy mh ON e.ManagerID = mh.EmployeeID
                )
                SELECT 
                    REPLICATE('  ', Level) + Name AS EmployeeHierarchy,
                    Level
                FROM ManagerHierarchy
                ORDER BY Level, Name;

                -- Find employees hired in same year
                SELECT 
                    e1.FirstName + ' ' + e1.LastName AS Employee1,
                    e2.FirstName + ' ' + e2.LastName AS Employee2,
                    YEAR(e1.HireDate) AS HireYear
                FROM Employees e1
                INNER JOIN Employees e2 
                    ON YEAR(e1.HireDate) = YEAR(e2.HireDate)
                    AND e1.EmployeeID < e2.EmployeeID
                ORDER BY HireYear;
            ";

            Console.WriteLine(sql);
        }
        #endregion

        #region Multiple JOINs
        static void MultipleJoins()
        {
            Console.WriteLine("\n6. Multiple JOINs - Complex Relationships");
            
            string sql = @"
                -- Employee, Department, and Projects
                SELECT 
                    e.FirstName + ' ' + e.LastName AS Employee,
                    d.DepartmentName,
                    p.ProjectName,
                    ep.Role,
                    ep.HoursAllocated
                FROM Employees e
                INNER JOIN Departments d ON e.DepartmentID = d.DepartmentID
                LEFT JOIN EmployeeProjects ep ON e.EmployeeID = ep.EmployeeID
                LEFT JOIN Projects p ON ep.ProjectID = p.ProjectID;

                -- Complex join with multiple tables and aggregates
                SELECT 
                    d.DepartmentName,
                    COUNT(DISTINCT e.EmployeeID) AS EmployeeCount,
                    COUNT(DISTINCT p.ProjectID) AS ProjectCount,
                    ISNULL(SUM(p.Budget), 0) AS TotalProjectBudget,
                    ISNULL(AVG(e.Salary), 0) AS AvgSalary
                FROM Departments d
                LEFT JOIN Employees e ON d.DepartmentID = e.DepartmentID
                LEFT JOIN Projects p ON d.DepartmentID = p.DepartmentID
                GROUP BY d.DepartmentID, d.DepartmentName;

                -- Employee with manager, department, and project info
                SELECT 
                    e.FirstName + ' ' + e.LastName AS Employee,
                    m.FirstName + ' ' + m.LastName AS Manager,
                    d.DepartmentName,
                    p.ProjectName,
                    p.Status AS ProjectStatus
                FROM Employees e
                LEFT JOIN Employees m ON e.ManagerID = m.EmployeeID
                INNER JOIN Departments d ON e.DepartmentID = d.DepartmentID
                LEFT JOIN EmployeeProjects ep ON e.EmployeeID = ep.EmployeeID
                LEFT JOIN Projects p ON ep.ProjectID = p.ProjectID
                WHERE e.IsActive = 1;

                -- Complex business query: Department performance
                SELECT 
                    d.DepartmentName,
                    d.Budget AS DepartmentBudget,
                    COUNT(DISTINCT e.EmployeeID) AS EmployeeCount,
                    SUM(e.Salary) AS TotalSalaries,
                    COUNT(DISTINCT p.ProjectID) AS ActiveProjects,
                    SUM(p.Budget) AS ProjectBudget,
                    d.Budget - SUM(e.Salary) AS RemainingBudget
                FROM Departments d
                LEFT JOIN Employees e ON d.DepartmentID = e.DepartmentID AND e.IsActive = 1
                LEFT JOIN Projects p ON d.DepartmentID = p.DepartmentID AND p.Status = 'Active'
                GROUP BY d.DepartmentID, d.DepartmentName, d.Budget
                HAVING SUM(e.Salary) < d.Budget;

                -- Join with derived table
                SELECT 
                    e.FirstName + ' ' + e.LastName AS Employee,
                    e.Salary,
                    dept.AvgSalary,
                    e.Salary - dept.AvgSalary AS DifferenceFromAvg
                FROM Employees e
                INNER JOIN (
                    SELECT 
                        DepartmentID,
                        AVG(Salary) AS AvgSalary
                    FROM Employees
                    GROUP BY DepartmentID
                ) dept ON e.DepartmentID = dept.DepartmentID;
            ";

            Console.WriteLine(sql);
        }
        #endregion

        #region Subqueries
        static void Subqueries()
        {
            Console.WriteLine("\n7. Subqueries - Nested SELECT statements");
            
            string sql = @"
                -- Subquery in WHERE clause
                SELECT 
                    FirstName,
                    LastName,
                    Salary
                FROM Employees
                WHERE Salary > (SELECT AVG(Salary) FROM Employees);

                -- Subquery with IN
                SELECT 
                    FirstName,
                    LastName,
                    DepartmentID
                FROM Employees
                WHERE DepartmentID IN (
                    SELECT DepartmentID 
                    FROM Departments 
                    WHERE Budget > 800000
                );

                -- Subquery with NOT IN
                SELECT 
                    DepartmentName
                FROM Departments
                WHERE DepartmentID NOT IN (
                    SELECT DISTINCT DepartmentID 
                    FROM Employees 
                    WHERE DepartmentID IS NOT NULL
                );

                -- Subquery with EXISTS
                SELECT 
                    d.DepartmentName,
                    d.Budget
                FROM Departments d
                WHERE EXISTS (
                    SELECT 1 
                    FROM Employees e 
                    WHERE e.DepartmentID = d.DepartmentID 
                        AND e.Salary > 80000
                );

                -- Subquery with NOT EXISTS
                SELECT 
                    e.FirstName,
                    e.LastName
                FROM Employees e
                WHERE NOT EXISTS (
                    SELECT 1 
                    FROM EmployeeProjects ep 
                    WHERE ep.EmployeeID = e.EmployeeID
                );

                -- Subquery in SELECT (scalar subquery)
                SELECT 
                    e.FirstName,
                    e.LastName,
                    e.Salary,
                    (SELECT AVG(Salary) FROM Employees WHERE DepartmentID = e.DepartmentID) AS DeptAvgSalary,
                    (SELECT COUNT(*) FROM EmployeeProjects WHERE EmployeeID = e.EmployeeID) AS ProjectCount
                FROM Employees e;

                -- Multiple subqueries
                SELECT 
                    FirstName,
                    LastName,
                    Salary
                FROM Employees
                WHERE Salary > (SELECT AVG(Salary) FROM Employees)
                    AND DepartmentID IN (
                        SELECT DepartmentID 
                        FROM Departments 
                        WHERE Location = 'New York'
                    );

                -- Subquery with ANY/SOME
                SELECT 
                    FirstName,
                    LastName,
                    Salary
                FROM Employees
                WHERE Salary > ANY (
                    SELECT Salary 
                    FROM Employees 
                    WHERE DepartmentID = 1
                );

                -- Subquery with ALL
                SELECT 
                    FirstName,
                    LastName,
                    Salary
                FROM Employees
                WHERE Salary > ALL (
                    SELECT Salary 
                    FROM Employees 
                    WHERE DepartmentID = 2
                );
            ";

            Console.WriteLine(sql);
        }
        #endregion

        #region Correlated Subqueries
        static void CorrelatedSubqueries()
        {
            Console.WriteLine("\n8. Correlated Subqueries - Depends on outer query");
            
            string sql = @"
                -- Find employees earning more than department average
                SELECT 
                    e1.FirstName,
                    e1.LastName,
                    e1.Salary,
                    d.DepartmentName
                FROM Employees e1
                INNER JOIN Departments d ON e1.DepartmentID = d.DepartmentID
                WHERE e1.Salary > (
                    SELECT AVG(e2.Salary)
                    FROM Employees e2
                    WHERE e2.DepartmentID = e1.DepartmentID
                );

                -- Find employees with above-average project count
                SELECT 
                    e.FirstName,
                    e.LastName,
                    (SELECT COUNT(*) FROM EmployeeProjects ep WHERE ep.EmployeeID = e.EmployeeID) AS ProjectCount
                FROM Employees e
                WHERE (
                    SELECT COUNT(*) 
                    FROM EmployeeProjects ep 
                    WHERE ep.EmployeeID = e.EmployeeID
                ) > (
                    SELECT AVG(ProjectCnt)
                    FROM (
                        SELECT COUNT(*) AS ProjectCnt
                        FROM EmployeeProjects
                        GROUP BY EmployeeID
                    ) AS AvgProjects
                );

                -- Correlated EXISTS
                SELECT 
                    d.DepartmentName,
                    d.Budget
                FROM Departments d
                WHERE EXISTS (
                    SELECT 1
                    FROM Employees e
                    WHERE e.DepartmentID = d.DepartmentID
                        AND e.Salary > (
                            SELECT AVG(Salary)
                            FROM Employees
                            WHERE DepartmentID = d.DepartmentID
                        ) * 1.2
                );

                -- Find employees with latest salary change
                SELECT 
                    e.FirstName,
                    e.LastName,
                    sh.NewSalary,
                    sh.ChangeDate
                FROM Employees e
                INNER JOIN SalaryHistory sh ON e.EmployeeID = sh.EmployeeID
                WHERE sh.ChangeDate = (
                    SELECT MAX(ChangeDate)
                    FROM SalaryHistory
                    WHERE EmployeeID = e.EmployeeID
                );

                -- Row number simulation with correlated subquery
                SELECT 
                    e1.FirstName,
                    e1.LastName,
                    e1.Salary,
                    (
                        SELECT COUNT(*) + 1
                        FROM Employees e2
                        WHERE e2.DepartmentID = e1.DepartmentID
                            AND e2.Salary > e1.Salary
                    ) AS Rank
                FROM Employees e1
                ORDER BY e1.DepartmentID, Rank;
            ";

            Console.WriteLine(sql);
        }
        #endregion

        #region Derived Tables
        static void DerivedTables()
        {
            Console.WriteLine("\n9. Derived Tables - Inline views");
            
            string sql = @"
                -- Basic derived table
                SELECT 
                    DeptName,
                    EmployeeCount,
                    AvgSalary
                FROM (
                    SELECT 
                        d.DepartmentName AS DeptName,
                        COUNT(e.EmployeeID) AS EmployeeCount,
                        AVG(e.Salary) AS AvgSalary
                    FROM Departments d
                    LEFT JOIN Employees e ON d.DepartmentID = e.DepartmentID
                    GROUP BY d.DepartmentName
                ) AS DeptStats
                WHERE EmployeeCount > 5;

                -- Multiple derived tables with JOIN
                SELECT 
                    dept.DepartmentName,
                    dept.EmployeeCount,
                    proj.ProjectCount
                FROM (
                    SELECT 
                        DepartmentID,
                        DepartmentName,
                        COUNT(*) AS EmployeeCount
                    FROM Departments d
                    INNER JOIN Employees e ON d.DepartmentID = e.DepartmentID
                    GROUP BY d.DepartmentID, d.DepartmentName
                ) dept
                LEFT JOIN (
                    SELECT 
                        DepartmentID,
                        COUNT(*) AS ProjectCount
                    FROM Projects
                    GROUP BY DepartmentID
                ) proj ON dept.DepartmentID = proj.DepartmentID;

                -- Derived table with window function
                SELECT 
                    FirstName,
                    LastName,
                    Salary,
                    AvgSalary,
                    Salary - AvgSalary AS Difference
                FROM (
                    SELECT 
                        FirstName,
                        LastName,
                        Salary,
                        AVG(Salary) OVER (PARTITION BY DepartmentID) AS AvgSalary
                    FROM Employees
                ) AS EmpWithAvg
                WHERE Salary > AvgSalary;

                -- Complex derived table for ranking
                SELECT TOP 5
                    DepartmentName,
                    TotalSalary,
                    EmployeeCount,
                    AvgSalary
                FROM (
                    SELECT 
                        d.DepartmentName,
                        SUM(e.Salary) AS TotalSalary,
                        COUNT(e.EmployeeID) AS EmployeeCount,
                        AVG(e.Salary) AS AvgSalary,
                        ROW_NUMBER() OVER (ORDER BY SUM(e.Salary) DESC) AS rn
                    FROM Departments d
                    INNER JOIN Employees e ON d.DepartmentID = e.DepartmentID
                    GROUP BY d.DepartmentID, d.DepartmentName
                ) AS RankedDepts
                WHERE rn <= 5;
            ";

            Console.WriteLine(sql);
        }
        #endregion

        #region Interview Questions
        /*
        COMMON INTERVIEW QUESTIONS:

        1. Difference between INNER JOIN and LEFT JOIN?
           - INNER: Only matching records
           - LEFT: All from left + matching from right (NULL if no match)

        2. When to use EXISTS vs IN?
           - EXISTS: Better for large datasets, stops when found, returns boolean
           - IN: Better for small lists, returns all matches

        3. What is a correlated subquery?
           - Subquery that references columns from outer query
           - Executed once for each row in outer query
           - Usually slower than JOINs

        4. JOIN vs Subquery - which is faster?
           - Generally JOIN is faster (set-based)
           - Subquery can be slower (row-by-row)
           - Optimizer may rewrite subquery as JOIN

        5. What is CROSS JOIN used for?
           - Cartesian product of two tables
           - Useful for generating combinations or test data

        6. How to find records in Table A but not in Table B?
           - LEFT JOIN with WHERE B.key IS NULL
           - NOT EXISTS
           - NOT IN (watch for NULLs)
           - EXCEPT

        7. Difference between WHERE and HAVING?
           - WHERE: Filters before grouping
           - HAVING: Filters after grouping

        8. Can you use subquery in JOIN condition?
           - Yes, but usually better to use derived table or CTE

        9. What is a SELF JOIN?
           - Table joined to itself
           - Used for hierarchical data, comparing rows within same table

        10. Best practices for writing JOINs?
            - Use appropriate JOIN type
            - Index JOIN columns
            - Avoid SELECT *
            - Consider execution plan
            - Use aliases for readability
        */
        #endregion
    }
}
