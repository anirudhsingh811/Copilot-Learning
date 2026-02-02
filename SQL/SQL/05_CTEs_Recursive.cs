using System;

namespace SQLPreparation
{
    /// <summary>
    /// Day 5-6: CTEs and Recursive Queries
    /// Covers: Common Table Expressions, Recursive CTEs, Multiple CTEs
    /// </summary>
    public class CTEs_Recursive
    {
        public static void RunAllDemos()
        {
            Console.WriteLine("=== CTEs AND RECURSIVE QUERIES DEMO ===\n");

            BasicCTEs();
            MultipleCTEs();
            RecursiveCTEs();
            AdvancedRecursion();
            CTEvsSubquery();
        }

        #region Basic CTEs
        static void BasicCTEs()
        {
            Console.WriteLine("1. Basic Common Table Expressions");
            
            string sql = @"
                -- Simple CTE
                WITH EmployeeCTE AS (
                    SELECT 
                        EmployeeID,
                        FirstName + ' ' + LastName AS FullName,
                        DepartmentID,
                        Salary
                    FROM Employees
                    WHERE IsActive = 1
                )
                SELECT 
                    FullName,
                    Salary
                FROM EmployeeCTE
                WHERE Salary > 70000;

                -- CTE with JOIN
                WITH EmployeeDept AS (
                    SELECT 
                        e.EmployeeID,
                        e.FirstName + ' ' + e.LastName AS FullName,
                        d.DepartmentName,
                        e.Salary
                    FROM Employees e
                    INNER JOIN Departments d ON e.DepartmentID = d.DepartmentID
                )
                SELECT 
                    DepartmentName,
                    COUNT(*) AS EmployeeCount,
                    AVG(Salary) AS AvgSalary
                FROM EmployeeDept
                GROUP BY DepartmentName;

                -- CTE with aggregation
                WITH DeptStats AS (
                    SELECT 
                        DepartmentID,
                        COUNT(*) AS EmployeeCount,
                        AVG(Salary) AS AvgSalary,
                        MAX(Salary) AS MaxSalary,
                        MIN(Salary) AS MinSalary
                    FROM Employees
                    GROUP BY DepartmentID
                )
                SELECT 
                    d.DepartmentName,
                    ds.EmployeeCount,
                    ds.AvgSalary,
                    ds.MaxSalary,
                    ds.MinSalary
                FROM DeptStats ds
                INNER JOIN Departments d ON ds.DepartmentID = d.DepartmentID
                WHERE ds.EmployeeCount > 5;

                -- CTE with window functions
                WITH RankedEmployees AS (
                    SELECT 
                        EmployeeID,
                        FirstName + ' ' + LastName AS FullName,
                        DepartmentID,
                        Salary,
                        ROW_NUMBER() OVER (PARTITION BY DepartmentID ORDER BY Salary DESC) AS SalaryRank
                    FROM Employees
                )
                SELECT 
                    FullName,
                    DepartmentID,
                    Salary,
                    SalaryRank
                FROM RankedEmployees
                WHERE SalaryRank <= 3;

                -- CTE in UPDATE
                WITH EmployeeUpdate AS (
                    SELECT 
                        EmployeeID,
                        Salary
                    FROM Employees
                    WHERE DepartmentID = 1
                )
                UPDATE EmployeeUpdate
                SET Salary = Salary * 1.10;

                -- CTE in DELETE
                WITH InactiveEmployees AS (
                    SELECT EmployeeID
                    FROM Employees
                    WHERE IsActive = 0 
                        AND HireDate < DATEADD(YEAR, -10, GETDATE())
                )
                DELETE FROM InactiveEmployees;
            ";

            Console.WriteLine(sql);
        }
        #endregion

        #region Multiple CTEs
        static void MultipleCTEs()
        {
            Console.WriteLine("\n2. Multiple CTEs - Chaining and Combining");
            
            string sql = @"
                -- Multiple CTEs (comma-separated)
                WITH DeptEmployeeCount AS (
                    SELECT 
                        DepartmentID,
                        COUNT(*) AS EmployeeCount
                    FROM Employees
                    GROUP BY DepartmentID
                ),
                DeptProjectCount AS (
                    SELECT 
                        DepartmentID,
                        COUNT(*) AS ProjectCount,
                        SUM(Budget) AS TotalBudget
                    FROM Projects
                    GROUP BY DepartmentID
                )
                SELECT 
                    d.DepartmentName,
                    ISNULL(dec.EmployeeCount, 0) AS EmployeeCount,
                    ISNULL(dpc.ProjectCount, 0) AS ProjectCount,
                    ISNULL(dpc.TotalBudget, 0) AS TotalBudget
                FROM Departments d
                LEFT JOIN DeptEmployeeCount dec ON d.DepartmentID = dec.DepartmentID
                LEFT JOIN DeptProjectCount dpc ON d.DepartmentID = dpc.DepartmentID;

                -- Chained CTEs (CTE referencing another CTE)
                WITH ActiveEmployees AS (
                    SELECT 
                        EmployeeID,
                        FirstName + ' ' + LastName AS FullName,
                        DepartmentID,
                        Salary
                    FROM Employees
                    WHERE IsActive = 1
                ),
                DeptAvgSalary AS (
                    SELECT 
                        DepartmentID,
                        AVG(Salary) AS AvgSalary
                    FROM ActiveEmployees
                    GROUP BY DepartmentID
                ),
                AboveAvgEmployees AS (
                    SELECT 
                        ae.EmployeeID,
                        ae.FullName,
                        ae.DepartmentID,
                        ae.Salary,
                        das.AvgSalary
                    FROM ActiveEmployees ae
                    INNER JOIN DeptAvgSalary das ON ae.DepartmentID = das.DepartmentID
                    WHERE ae.Salary > das.AvgSalary
                )
                SELECT 
                    d.DepartmentName,
                    aae.FullName,
                    aae.Salary,
                    aae.AvgSalary,
                    aae.Salary - aae.AvgSalary AS DifferenceFromAvg
                FROM AboveAvgEmployees aae
                INNER JOIN Departments d ON aae.DepartmentID = d.DepartmentID
                ORDER BY d.DepartmentName, aae.Salary DESC;

                -- Complex business logic with multiple CTEs
                WITH EmployeeProjects_CTE AS (
                    SELECT 
                        ep.EmployeeID,
                        COUNT(DISTINCT ep.ProjectID) AS ProjectCount,
                        SUM(ep.HoursAllocated) AS TotalHours
                    FROM EmployeeProjects ep
                    INNER JOIN Projects p ON ep.ProjectID = p.ProjectID
                    WHERE p.Status = 'Active'
                    GROUP BY ep.EmployeeID
                ),
                SalaryHistory_CTE AS (
                    SELECT 
                        EmployeeID,
                        MAX(ChangeDate) AS LastSalaryChange,
                        COUNT(*) AS SalaryChanges
                    FROM SalaryHistory
                    GROUP BY EmployeeID
                ),
                EmployeeMetrics AS (
                    SELECT 
                        e.EmployeeID,
                        e.FirstName + ' ' + e.LastName AS FullName,
                        e.Salary,
                        e.HireDate,
                        DATEDIFF(YEAR, e.HireDate, GETDATE()) AS YearsWithCompany,
                        ISNULL(ep.ProjectCount, 0) AS ActiveProjects,
                        ISNULL(ep.TotalHours, 0) AS TotalHours,
                        ISNULL(sh.SalaryChanges, 0) AS SalaryChanges,
                        sh.LastSalaryChange
                    FROM Employees e
                    LEFT JOIN EmployeeProjects_CTE ep ON e.EmployeeID = ep.EmployeeID
                    LEFT JOIN SalaryHistory_CTE sh ON e.EmployeeID = sh.EmployeeID
                    WHERE e.IsActive = 1
                )
                SELECT 
                    FullName,
                    Salary,
                    YearsWithCompany,
                    ActiveProjects,
                    TotalHours,
                    SalaryChanges,
                    LastSalaryChange,
                    CASE 
                        WHEN ActiveProjects = 0 THEN 'Unassigned'
                        WHEN TotalHours > 160 THEN 'Overloaded'
                        WHEN TotalHours BETWEEN 120 AND 160 THEN 'Optimal'
                        ELSE 'Underutilized'
                    END AS WorkloadStatus
                FROM EmployeeMetrics
                ORDER BY Salary DESC;
            ";

            Console.WriteLine(sql);
        }
        #endregion

        #region Recursive CTEs
        static void RecursiveCTEs()
        {
            Console.WriteLine("\n3. Recursive CTEs - Hierarchical Data");
            
            string sql = @"
                -- Basic recursive CTE: Employee hierarchy
                WITH EmployeeHierarchy AS (
                    -- Anchor member (top-level managers)
                    SELECT 
                        EmployeeID,
                        FirstName + ' ' + LastName AS FullName,
                        ManagerID,
                        0 AS Level,
                        CAST(FirstName + ' ' + LastName AS NVARCHAR(1000)) AS HierarchyPath
                    FROM Employees
                    WHERE ManagerID IS NULL
                    
                    UNION ALL
                    
                    -- Recursive member
                    SELECT 
                        e.EmployeeID,
                        e.FirstName + ' ' + e.LastName,
                        e.ManagerID,
                        eh.Level + 1,
                        CAST(eh.HierarchyPath + ' -> ' + e.FirstName + ' ' + e.LastName AS NVARCHAR(1000))
                    FROM Employees e
                    INNER JOIN EmployeeHierarchy eh ON e.ManagerID = eh.EmployeeID
                )
                SELECT 
                    REPLICATE('  ', Level) + FullName AS EmployeeHierarchy,
                    Level,
                    HierarchyPath
                FROM EmployeeHierarchy
                ORDER BY HierarchyPath;

                -- Count subordinates
                WITH EmployeeTree AS (
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
                        et.Level + 1
                    FROM Employees e
                    INNER JOIN EmployeeTree et ON e.ManagerID = et.EmployeeID
                )
                SELECT 
                    e.EmployeeID,
                    e.FirstName + ' ' + e.LastName AS Manager,
                    COUNT(et.EmployeeID) - 1 AS TotalSubordinates
                FROM Employees e
                LEFT JOIN EmployeeTree et ON e.EmployeeID = et.EmployeeID OR e.EmployeeID IN (
                    SELECT ManagerID 
                    FROM EmployeeTree 
                    WHERE EmployeeID = et.EmployeeID
                )
                WHERE e.EmployeeID IN (SELECT DISTINCT ManagerID FROM Employees WHERE ManagerID IS NOT NULL)
                GROUP BY e.EmployeeID, e.FirstName, e.LastName
                ORDER BY TotalSubordinates DESC;

                -- Number sequence generation
                WITH Numbers AS (
                    SELECT 1 AS n
                    UNION ALL
                    SELECT n + 1
                    FROM Numbers
                    WHERE n < 100
                )
                SELECT n
                FROM Numbers
                OPTION (MAXRECURSION 100);

                -- Date series generation
                WITH DateSeries AS (
                    SELECT CAST('2024-01-01' AS DATE) AS DateValue
                    UNION ALL
                    SELECT DATEADD(DAY, 1, DateValue)
                    FROM DateSeries
                    WHERE DateValue < '2024-12-31'
                )
                SELECT 
                    DateValue,
                    DATENAME(WEEKDAY, DateValue) AS DayName,
                    DATEPART(WEEK, DateValue) AS WeekNumber
                FROM DateSeries
                OPTION (MAXRECURSION 366);

                -- Fibonacci sequence
                WITH Fibonacci AS (
                    SELECT 
                        1 AS Level,
                        CAST(0 AS BIGINT) AS Fib,
                        CAST(1 AS BIGINT) AS NextFib
                    UNION ALL
                    SELECT 
                        Level + 1,
                        NextFib,
                        Fib + NextFib
                    FROM Fibonacci
                    WHERE Level < 20
                )
                SELECT 
                    Level,
                    Fib AS FibonacciNumber
                FROM Fibonacci
                OPTION (MAXRECURSION 20);
            ";

            Console.WriteLine(sql);
        }
        #endregion

        #region Advanced Recursion
        static void AdvancedRecursion()
        {
            Console.WriteLine("\n4. Advanced Recursive Patterns");
            
            string sql = @"
                -- Find all paths in organizational structure
                WITH OrgPaths AS (
                    SELECT 
                        EmployeeID,
                        FirstName + ' ' + LastName AS FullName,
                        ManagerID,
                        CAST(EmployeeID AS VARCHAR(MAX)) AS Path,
                        0 AS Depth
                    FROM Employees
                    WHERE ManagerID IS NULL
                    
                    UNION ALL
                    
                    SELECT 
                        e.EmployeeID,
                        e.FirstName + ' ' + e.LastName,
                        e.ManagerID,
                        CAST(op.Path + ',' + CAST(e.EmployeeID AS VARCHAR(10)) AS VARCHAR(MAX)),
                        op.Depth + 1
                    FROM Employees e
                    INNER JOIN OrgPaths op ON e.ManagerID = op.EmployeeID
                    WHERE op.Depth < 10  -- Prevent infinite loops
                )
                SELECT 
                    FullName,
                    Path,
                    Depth,
                    LEN(Path) - LEN(REPLACE(Path, ',', '')) + 1 AS PathLength
                FROM OrgPaths
                ORDER BY Path;

                -- Calculate aggregates up the hierarchy
                WITH EmployeeSalaryTree AS (
                    SELECT 
                        e.EmployeeID,
                        e.FirstName + ' ' + e.LastName AS FullName,
                        e.ManagerID,
                        e.Salary,
                        CAST(e.Salary AS DECIMAL(18,2)) AS TotalSalaryBelowManaged
                    FROM Employees e
                    WHERE NOT EXISTS (
                        SELECT 1 FROM Employees sub WHERE sub.ManagerID = e.EmployeeID
                    )
                    
                    UNION ALL
                    
                    SELECT 
                        e.EmployeeID,
                        e.FirstName + ' ' + e.LastName,
                        e.ManagerID,
                        e.Salary,
                        e.Salary + SUM(est.TotalSalaryBelowManaged)
                    FROM Employees e
                    INNER JOIN EmployeeSalaryTree est ON e.EmployeeID = est.ManagerID
                    GROUP BY e.EmployeeID, e.FirstName, e.LastName, e.ManagerID, e.Salary
                )
                SELECT 
                    FullName,
                    Salary AS OwnSalary,
                    TotalSalaryBelowManaged AS TotalTeamSalary,
                    TotalSalaryBelowManaged - Salary AS SubordinatesTotalSalary
                FROM EmployeeSalaryTree
                WHERE ManagerID IS NOT NULL
                ORDER BY TotalSalaryBelowManaged DESC;

                -- Find all ancestors
                WITH Ancestors AS (
                    SELECT 
                        e.EmployeeID AS ChildID,
                        e.FirstName + ' ' + e.LastName AS ChildName,
                        m.EmployeeID AS AncestorID,
                        m.FirstName + ' ' + m.LastName AS AncestorName,
                        1 AS Distance
                    FROM Employees e
                    INNER JOIN Employees m ON e.ManagerID = m.EmployeeID
                    
                    UNION ALL
                    
                    SELECT 
                        a.ChildID,
                        a.ChildName,
                        m.EmployeeID,
                        m.FirstName + ' ' + m.LastName,
                        a.Distance + 1
                    FROM Ancestors a
                    INNER JOIN Employees m ON a.AncestorID = m.ManagerID
                    WHERE a.Distance < 10
                )
                SELECT 
                    ChildName AS Employee,
                    AncestorName AS Manager,
                    Distance AS LevelsAbove,
                    CASE Distance
                        WHEN 1 THEN 'Direct Manager'
                        WHEN 2 THEN 'Manager of Manager'
                        ELSE CAST(Distance AS VARCHAR) + ' levels above'
                    END AS Relationship
                FROM Ancestors
                ORDER BY ChildID, Distance;

                -- Bill of Materials (hierarchical product structure)
                /*
                CREATE TABLE ProductBOM (
                    ProductID INT,
                    ComponentID INT,
                    Quantity INT
                );
                */

                WITH BillOfMaterials AS (
                    SELECT 
                        ProductID,
                        ComponentID,
                        Quantity,
                        0 AS Level
                    FROM ProductBOM
                    WHERE ProductID = 100  -- Top level product
                    
                    UNION ALL
                    
                    SELECT 
                        bom.ProductID,
                        pb.ComponentID,
                        bom.Quantity * pb.Quantity,
                        bom.Level + 1
                    FROM BillOfMaterials bom
                    INNER JOIN ProductBOM pb ON bom.ComponentID = pb.ProductID
                    WHERE bom.Level < 10
                )
                SELECT 
                    REPLICATE('  ', Level) + CAST(ComponentID AS VARCHAR) AS Component,
                    Quantity AS TotalQuantityNeeded,
                    Level
                FROM BillOfMaterials;
            ";

            Console.WriteLine(sql);
        }
        #endregion

        #region CTE vs Subquery
        static void CTEvsSubquery()
        {
            Console.WriteLine("\n5. CTE vs Subquery - When to Use Each");
            
            string sql = @"
                -- Same query: Subquery version
                SELECT 
                    e.FirstName + ' ' + e.LastName AS FullName,
                    e.Salary,
                    dept_avg.AvgSalary
                FROM Employees e
                INNER JOIN (
                    SELECT 
                        DepartmentID,
                        AVG(Salary) AS AvgSalary
                    FROM Employees
                    GROUP BY DepartmentID
                ) dept_avg ON e.DepartmentID = dept_avg.DepartmentID
                WHERE e.Salary > dept_avg.AvgSalary;

                -- Same query: CTE version (more readable)
                WITH DeptAvgSalary AS (
                    SELECT 
                        DepartmentID,
                        AVG(Salary) AS AvgSalary
                    FROM Employees
                    GROUP BY DepartmentID
                )
                SELECT 
                    e.FirstName + ' ' + e.LastName AS FullName,
                    e.Salary,
                    das.AvgSalary
                FROM Employees e
                INNER JOIN DeptAvgSalary das ON e.DepartmentID = das.DepartmentID
                WHERE e.Salary > das.AvgSalary;

                -- CTE advantage: Reusability
                WITH HighPerformers AS (
                    SELECT 
                        EmployeeID,
                        FirstName + ' ' + LastName AS FullName,
                        DepartmentID,
                        Salary
                    FROM Employees
                    WHERE Salary > (SELECT AVG(Salary) FROM Employees) * 1.2
                )
                SELECT 
                    hp1.FullName AS Employee1,
                    hp2.FullName AS Employee2,
                    hp1.Salary + hp2.Salary AS CombinedSalary
                FROM HighPerformers hp1
                CROSS JOIN HighPerformers hp2
                WHERE hp1.EmployeeID < hp2.EmployeeID
                    AND hp1.DepartmentID = hp2.DepartmentID;

                -- CTE for complex UPDATE
                WITH SalaryAdjustment AS (
                    SELECT 
                        e.EmployeeID,
                        e.Salary,
                        CASE 
                            WHEN DATEDIFF(YEAR, e.HireDate, GETDATE()) >= 10 THEN 1.15
                            WHEN DATEDIFF(YEAR, e.HireDate, GETDATE()) >= 5 THEN 1.10
                            ELSE 1.05
                        END AS AdjustmentFactor
                    FROM Employees e
                    WHERE e.IsActive = 1
                )
                UPDATE sa
                SET Salary = Salary * AdjustmentFactor
                FROM SalaryAdjustment sa;
            ";

            Console.WriteLine(sql);
        }
        #endregion

        #region Interview Questions
        /*
        COMMON INTERVIEW QUESTIONS:

        1. What is a CTE?
           - Common Table Expression: Named temporary result set
           - Defined within execution scope of SELECT, INSERT, UPDATE, DELETE
           - More readable than subqueries

        2. CTE vs Temporary Table vs Table Variable?
           - CTE: Not persisted, scope limited to single statement, no statistics
           - Temp Table: Persisted in tempdb, can have indexes, statistics
           - Table Variable: In-memory, faster for small datasets, no statistics

        3. Can you update data through a CTE?
           - Yes, CTE can be used in UPDATE, DELETE, MERGE statements

        4. What is a recursive CTE?
           - CTE that references itself
           - Must have anchor member and recursive member
           - Use MAXRECURSION hint to prevent infinite loops

        5. What is MAXRECURSION?
           - Limits number of recursive iterations
           - Default: 100
           - Set to 0 for unlimited (use with caution)

        6. Can you have multiple CTEs in one query?
           - Yes, separate with commas
           - Later CTEs can reference earlier ones

        7. Performance: CTE vs Subquery?
           - Usually similar performance
           - Query optimizer may treat them the same
           - CTE better for readability and reusability

        8. When to use recursive CTE?
           - Hierarchical data (org charts, bill of materials)
           - Graph traversal
           - Generating sequences
           - Finding all paths in tree structure

        9. Can CTE improve performance?
           - Not directly (not materialized)
           - Improves readability which aids optimization
           - Consider materialized view or temp table for reuse

        10. Common pitfalls with CTEs?
            - Thinking they're materialized (they're not)
            - Infinite recursion without MAXRECURSION
            - Using instead of temp table when multiple references needed
            - Not realizing CTE is reevaluated each time referenced
        */
        #endregion
    }
}
