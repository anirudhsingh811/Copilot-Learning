using System;

namespace SQLPreparation
{
    /// <summary>
    /// Day 5-6: Window Functions - Advanced Analytics
    /// Covers: ROW_NUMBER, RANK, DENSE_RANK, NTILE, LEAD, LAG, FIRST_VALUE, LAST_VALUE
    /// </summary>
    public class Window_Functions
    {
        public static void RunAllDemos()
        {
            Console.WriteLine("=== WINDOW FUNCTIONS DEMO ===\n");

            RankingFunctions();
            OffsetFunctions();
            AggregateFunctions();
            FrameClauses();
            PracticalScenarios();
        }

        #region Ranking Functions
        static void RankingFunctions()
        {
            Console.WriteLine("1. Ranking Functions - ROW_NUMBER, RANK, DENSE_RANK, NTILE");
            
            string sql = @"
                -- ROW_NUMBER: Unique sequential number
                SELECT 
                    EmployeeID,
                    FirstName + ' ' + LastName AS FullName,
                    DepartmentID,
                    Salary,
                    ROW_NUMBER() OVER (ORDER BY Salary DESC) AS RowNum,
                    ROW_NUMBER() OVER (PARTITION BY DepartmentID ORDER BY Salary DESC) AS DeptRowNum
                FROM Employees;

                -- RANK: Same rank for ties, gaps in sequence
                SELECT 
                    EmployeeID,
                    FirstName + ' ' + LastName AS FullName,
                    Salary,
                    RANK() OVER (ORDER BY Salary DESC) AS SalaryRank,
                    RANK() OVER (PARTITION BY DepartmentID ORDER BY Salary DESC) AS DeptSalaryRank
                FROM Employees;

                -- DENSE_RANK: Same rank for ties, NO gaps
                SELECT 
                    EmployeeID,
                    FirstName + ' ' + LastName AS FullName,
                    Salary,
                    DENSE_RANK() OVER (ORDER BY Salary DESC) AS DenseRank,
                    RANK() OVER (ORDER BY Salary DESC) AS RegularRank
                FROM Employees;

                -- NTILE: Divide rows into N groups
                SELECT 
                    EmployeeID,
                    FirstName + ' ' + LastName AS FullName,
                    Salary,
                    NTILE(4) OVER (ORDER BY Salary DESC) AS Quartile,
                    NTILE(10) OVER (ORDER BY Salary DESC) AS Decile,
                    CASE NTILE(4) OVER (ORDER BY Salary DESC)
                        WHEN 1 THEN 'Top 25%'
                        WHEN 2 THEN 'Upper Middle 25%'
                        WHEN 3 THEN 'Lower Middle 25%'
                        WHEN 4 THEN 'Bottom 25%'
                    END AS SalaryGroup
                FROM Employees;

                -- Practical: Top N per department
                WITH RankedEmployees AS (
                    SELECT 
                        e.EmployeeID,
                        e.FirstName + ' ' + e.LastName AS FullName,
                        d.DepartmentName,
                        e.Salary,
                        ROW_NUMBER() OVER (PARTITION BY e.DepartmentID ORDER BY e.Salary DESC) AS SalaryRank
                    FROM Employees e
                    INNER JOIN Departments d ON e.DepartmentID = d.DepartmentID
                )
                SELECT 
                    FullName,
                    DepartmentName,
                    Salary,
                    SalaryRank
                FROM RankedEmployees
                WHERE SalaryRank <= 3
                ORDER BY DepartmentName, SalaryRank;

                -- Find duplicate emails (using ROW_NUMBER)
                WITH DuplicateCheck AS (
                    SELECT 
                        Email,
                        EmployeeID,
                        ROW_NUMBER() OVER (PARTITION BY Email ORDER BY EmployeeID) AS DupNum
                    FROM Employees
                )
                SELECT 
                    Email,
                    EmployeeID
                FROM DuplicateCheck
                WHERE DupNum > 1;

                -- Median salary using percentiles
                SELECT DISTINCT
                    DepartmentID,
                    PERCENTILE_CONT(0.5) WITHIN GROUP (ORDER BY Salary) OVER (PARTITION BY DepartmentID) AS MedianSalary,
                    PERCENTILE_DISC(0.5) WITHIN GROUP (ORDER BY Salary) OVER (PARTITION BY DepartmentID) AS DiscreteMedian
                FROM Employees;
            ";

            Console.WriteLine(sql);
        }
        #endregion

        #region Offset Functions
        static void OffsetFunctions()
        {
            Console.WriteLine("\n2. Offset Functions - LEAD, LAG, FIRST_VALUE, LAST_VALUE");
            
            string sql = @"
                -- LAG: Access previous row
                SELECT 
                    EmployeeID,
                    FirstName + ' ' + LastName AS FullName,
                    HireDate,
                    Salary,
                    LAG(Salary) OVER (ORDER BY HireDate) AS PreviousEmployeeSalary,
                    LAG(HireDate, 1) OVER (ORDER BY HireDate) AS PreviousHireDate
                FROM Employees;

                -- LEAD: Access next row
                SELECT 
                    EmployeeID,
                    FirstName + ' ' + LastName AS FullName,
                    Salary,
                    LEAD(Salary) OVER (ORDER BY HireDate) AS NextEmployeeSalary,
                    LEAD(Salary) OVER (ORDER BY HireDate) - Salary AS SalaryDifference
                FROM Employees;

                -- LAG with default value
                SELECT 
                    EmployeeID,
                    FirstName + ' ' + LastName AS FullName,
                    Salary,
                    LAG(Salary, 1, 0) OVER (ORDER BY EmployeeID) AS PreviousSalary,
                    Salary - LAG(Salary, 1, 0) OVER (ORDER BY EmployeeID) AS Change
                FROM Employees;

                -- FIRST_VALUE: First value in window
                SELECT 
                    e.EmployeeID,
                    e.FirstName + ' ' + e.LastName AS FullName,
                    d.DepartmentName,
                    e.Salary,
                    FIRST_VALUE(e.Salary) OVER (
                        PARTITION BY e.DepartmentID 
                        ORDER BY e.Salary DESC
                        ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW
                    ) AS HighestSalaryInDept,
                    FIRST_VALUE(e.FirstName + ' ' + e.LastName) OVER (
                        PARTITION BY e.DepartmentID 
                        ORDER BY e.Salary DESC
                        ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW
                    ) AS TopEarnerInDept
                FROM Employees e
                INNER JOIN Departments d ON e.DepartmentID = d.DepartmentID;

                -- LAST_VALUE: Last value in window (be careful with frame)
                SELECT 
                    EmployeeID,
                    FirstName + ' ' + LastName AS FullName,
                    DepartmentID,
                    Salary,
                    LAST_VALUE(Salary) OVER (
                        PARTITION BY DepartmentID 
                        ORDER BY Salary DESC
                        ROWS BETWEEN CURRENT ROW AND UNBOUNDED FOLLOWING
                    ) AS LowestSalaryInDept
                FROM Employees;

                -- Salary change tracking
                SELECT 
                    sh.EmployeeID,
                    e.FirstName + ' ' + e.LastName AS FullName,
                    sh.NewSalary AS CurrentSalary,
                    LAG(sh.NewSalary) OVER (PARTITION BY sh.EmployeeID ORDER BY sh.ChangeDate) AS PreviousSalary,
                    sh.NewSalary - LAG(sh.NewSalary) OVER (PARTITION BY sh.EmployeeID ORDER BY sh.ChangeDate) AS SalaryIncrease,
                    CASE 
                        WHEN sh.NewSalary > LAG(sh.NewSalary) OVER (PARTITION BY sh.EmployeeID ORDER BY sh.ChangeDate) 
                        THEN 'Increase'
                        WHEN sh.NewSalary < LAG(sh.NewSalary) OVER (PARTITION BY sh.EmployeeID ORDER BY sh.ChangeDate) 
                        THEN 'Decrease'
                        ELSE 'No Change'
                    END AS ChangeType,
                    sh.ChangeDate
                FROM SalaryHistory sh
                INNER JOIN Employees e ON sh.EmployeeID = e.EmployeeID;

                -- Year-over-year comparison
                SELECT 
                    YEAR(ChangeDate) AS Year,
                    EmployeeID,
                    NewSalary,
                    LAG(NewSalary) OVER (PARTITION BY EmployeeID ORDER BY YEAR(ChangeDate)) AS PreviousYearSalary,
                    NewSalary - LAG(NewSalary) OVER (PARTITION BY EmployeeID ORDER BY YEAR(ChangeDate)) AS YoYIncrease,
                    CAST(
                        ((NewSalary - LAG(NewSalary) OVER (PARTITION BY EmployeeID ORDER BY YEAR(ChangeDate))) * 100.0 / 
                        LAG(NewSalary) OVER (PARTITION BY EmployeeID ORDER BY YEAR(ChangeDate)))
                        AS DECIMAL(5,2)
                    ) AS YoYPercentage
                FROM SalaryHistory;
            ";

            Console.WriteLine(sql);
        }
        #endregion

        #region Aggregate Window Functions
        static void AggregateFunctions()
        {
            Console.WriteLine("\n3. Aggregate Window Functions - Running Totals, Moving Averages");
            
            string sql = @"
                -- Running total
                SELECT 
                    EmployeeID,
                    FirstName + ' ' + LastName AS FullName,
                    HireDate,
                    Salary,
                    SUM(Salary) OVER (ORDER BY HireDate ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) AS RunningTotal,
                    AVG(Salary) OVER (ORDER BY HireDate ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) AS RunningAverage
                FROM Employees;

                -- Moving average (last 3 rows)
                SELECT 
                    EmployeeID,
                    FirstName + ' ' + LastName AS FullName,
                    HireDate,
                    Salary,
                    AVG(Salary) OVER (
                        ORDER BY HireDate 
                        ROWS BETWEEN 2 PRECEDING AND CURRENT ROW
                    ) AS MovingAvg3,
                    COUNT(*) OVER (
                        ORDER BY HireDate 
                        ROWS BETWEEN 2 PRECEDING AND CURRENT ROW
                    ) AS WindowSize
                FROM Employees;

                -- Cumulative sum by department
                SELECT 
                    e.EmployeeID,
                    e.FirstName + ' ' + e.LastName AS FullName,
                    d.DepartmentName,
                    e.Salary,
                    SUM(e.Salary) OVER (
                        PARTITION BY e.DepartmentID 
                        ORDER BY e.HireDate
                        ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW
                    ) AS CumulativeDeptSalary,
                    COUNT(*) OVER (
                        PARTITION BY e.DepartmentID 
                        ORDER BY e.HireDate
                        ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW
                    ) AS EmployeeCount
                FROM Employees e
                INNER JOIN Departments d ON e.DepartmentID = d.DepartmentID;

                -- Percentage of total
                SELECT 
                    e.EmployeeID,
                    e.FirstName + ' ' + e.LastName AS FullName,
                    d.DepartmentName,
                    e.Salary,
                    SUM(e.Salary) OVER () AS TotalCompanySalary,
                    SUM(e.Salary) OVER (PARTITION BY e.DepartmentID) AS TotalDeptSalary,
                    CAST((e.Salary * 100.0 / SUM(e.Salary) OVER (PARTITION BY e.DepartmentID)) AS DECIMAL(5,2)) AS PctOfDeptSalary,
                    CAST((e.Salary * 100.0 / SUM(e.Salary) OVER ()) AS DECIMAL(5,2)) AS PctOfCompanySalary
                FROM Employees e
                INNER JOIN Departments d ON e.DepartmentID = d.DepartmentID;

                -- Statistical measures
                SELECT DISTINCT
                    DepartmentID,
                    COUNT(*) OVER (PARTITION BY DepartmentID) AS EmployeeCount,
                    AVG(Salary) OVER (PARTITION BY DepartmentID) AS AvgSalary,
                    MIN(Salary) OVER (PARTITION BY DepartmentID) AS MinSalary,
                    MAX(Salary) OVER (PARTITION BY DepartmentID) AS MaxSalary,
                    STDEV(Salary) OVER (PARTITION BY DepartmentID) AS StdDevSalary,
                    VAR(Salary) OVER (PARTITION BY DepartmentID) AS VarianceSalary
                FROM Employees;

                -- Difference from average
                SELECT 
                    EmployeeID,
                    FirstName + ' ' + LastName AS FullName,
                    DepartmentID,
                    Salary,
                    AVG(Salary) OVER (PARTITION BY DepartmentID) AS DeptAvgSalary,
                    Salary - AVG(Salary) OVER (PARTITION BY DepartmentID) AS DiffFromAvg,
                    CAST(
                        ((Salary - AVG(Salary) OVER (PARTITION BY DepartmentID)) * 100.0 / 
                        AVG(Salary) OVER (PARTITION BY DepartmentID))
                        AS DECIMAL(5,2)
                    ) AS PctDiffFromAvg
                FROM Employees;
            ";

            Console.WriteLine(sql);
        }
        #endregion

        #region Frame Clauses
        static void FrameClauses()
        {
            Console.WriteLine("\n4. Frame Clauses - ROWS vs RANGE");
            
            string sql = @"
                -- ROWS: Physical offset
                SELECT 
                    EmployeeID,
                    FirstName + ' ' + LastName AS FullName,
                    Salary,
                    AVG(Salary) OVER (
                        ORDER BY EmployeeID 
                        ROWS BETWEEN 1 PRECEDING AND 1 FOLLOWING
                    ) AS AvgSalary_Rows
                FROM Employees;

                -- RANGE: Logical offset
                SELECT 
                    EmployeeID,
                    FirstName + ' ' + LastName AS FullName,
                    Salary,
                    AVG(Salary) OVER (
                        ORDER BY Salary 
                        RANGE BETWEEN 1000 PRECEDING AND 1000 FOLLOWING
                    ) AS AvgSalary_Range
                FROM Employees;

                -- Different frame specifications
                SELECT 
                    EmployeeID,
                    Salary,
                    -- Current row only
                    SUM(Salary) OVER (ORDER BY EmployeeID ROWS CURRENT ROW) AS CurrentRowSum,
                    
                    -- Unbounded preceding to current
                    SUM(Salary) OVER (ORDER BY EmployeeID ROWS UNBOUNDED PRECEDING) AS CumulativeSum,
                    
                    -- Current to unbounded following
                    SUM(Salary) OVER (ORDER BY EmployeeID ROWS BETWEEN CURRENT ROW AND UNBOUNDED FOLLOWING) AS ReverseSum,
                    
                    -- 2 preceding to 2 following
                    AVG(Salary) OVER (ORDER BY EmployeeID ROWS BETWEEN 2 PRECEDING AND 2 FOLLOWING) AS MovingAvg5,
                    
                    -- Entire partition
                    SUM(Salary) OVER (ORDER BY EmployeeID ROWS BETWEEN UNBOUNDED PRECEDING AND UNBOUNDED FOLLOWING) AS TotalSum
                FROM Employees;

                -- Frame with PARTITION BY
                SELECT 
                    e.EmployeeID,
                    d.DepartmentName,
                    e.Salary,
                    e.HireDate,
                    COUNT(*) OVER (
                        PARTITION BY e.DepartmentID 
                        ORDER BY e.HireDate 
                        ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW
                    ) AS EmployeeNumber,
                    AVG(e.Salary) OVER (
                        PARTITION BY e.DepartmentID 
                        ORDER BY e.HireDate 
                        ROWS BETWEEN 2 PRECEDING AND CURRENT ROW
                    ) AS RecentAvgSalary
                FROM Employees e
                INNER JOIN Departments d ON e.DepartmentID = d.DepartmentID;
            ";

            Console.WriteLine(sql);
        }
        #endregion

        #region Practical Scenarios
        static void PracticalScenarios()
        {
            Console.WriteLine("\n5. Practical Scenarios - Real-world Applications");
            
            string sql = @"
                -- 1. Find gaps in sequence
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

                -- 2. Top N with ties (Olympic ranking)
                WITH RankedEmployees AS (
                    SELECT 
                        EmployeeID,
                        FirstName + ' ' + LastName AS FullName,
                        Salary,
                        DENSE_RANK() OVER (ORDER BY Salary DESC) AS SalaryRank
                    FROM Employees
                )
                SELECT 
                    FullName,
                    Salary,
                    SalaryRank
                FROM RankedEmployees
                WHERE SalaryRank <= 10;

                -- 3. Running difference
                SELECT 
                    EmployeeID,
                    FirstName + ' ' + LastName AS FullName,
                    HireDate,
                    Salary,
                    Salary - LAG(Salary) OVER (ORDER BY HireDate) AS SalaryDiffFromPrevHire,
                    DATEDIFF(DAY, LAG(HireDate) OVER (ORDER BY HireDate), HireDate) AS DaysSinceLastHire
                FROM Employees;

                -- 4. Percentile ranks
                SELECT 
                    EmployeeID,
                    FirstName + ' ' + LastName AS FullName,
                    Salary,
                    PERCENT_RANK() OVER (ORDER BY Salary) AS PercentRank,
                    CUME_DIST() OVER (ORDER BY Salary) AS CumulativeDistribution,
                    CAST(PERCENT_RANK() OVER (ORDER BY Salary) * 100 AS DECIMAL(5,2)) AS Percentile
                FROM Employees;

                -- 5. Department ranking comparison
                WITH DeptStats AS (
                    SELECT 
                        e.EmployeeID,
                        e.FirstName + ' ' + e.LastName AS FullName,
                        e.DepartmentID,
                        e.Salary,
                        RANK() OVER (PARTITION BY e.DepartmentID ORDER BY e.Salary DESC) AS DeptRank,
                        RANK() OVER (ORDER BY e.Salary DESC) AS CompanyRank,
                        NTILE(4) OVER (PARTITION BY e.DepartmentID ORDER BY e.Salary DESC) AS DeptQuartile
                    FROM Employees e
                )
                SELECT 
                    FullName,
                    DepartmentID,
                    Salary,
                    DeptRank,
                    CompanyRank,
                    CASE DeptQuartile
                        WHEN 1 THEN 'Top Performers in Dept'
                        WHEN 2 THEN 'Above Average in Dept'
                        WHEN 3 THEN 'Below Average in Dept'
                        WHEN 4 THEN 'Need Improvement'
                    END AS Performance
                FROM DeptStats;

                -- 6. Consecutive days analysis
                WITH DateDiffs AS (
                    SELECT 
                        EmployeeID,
                        HireDate,
                        DATEDIFF(DAY, 
                            LAG(HireDate) OVER (ORDER BY HireDate), 
                            HireDate
                        ) AS DaysDiff,
                        ROW_NUMBER() OVER (ORDER BY HireDate) AS RowNum
                    FROM Employees
                )
                SELECT 
                    EmployeeID,
                    HireDate,
                    DaysDiff,
                    CASE 
                        WHEN DaysDiff <= 7 THEN 'Hired within a week'
                        WHEN DaysDiff <= 30 THEN 'Hired within a month'
                        ELSE 'More than a month gap'
                    END AS HiringPattern
                FROM DateDiffs
                WHERE DaysDiff IS NOT NULL;

                -- 7. Market basket analysis pattern
                WITH ProjectAssignments AS (
                    SELECT 
                        ep.EmployeeID,
                        STRING_AGG(CAST(ep.ProjectID AS VARCHAR), ',') WITHIN GROUP (ORDER BY ep.ProjectID) AS ProjectList,
                        COUNT(*) AS ProjectCount,
                        ROW_NUMBER() OVER (PARTITION BY COUNT(*) ORDER BY ep.EmployeeID) AS GroupRank
                    FROM EmployeeProjects ep
                    GROUP BY ep.EmployeeID
                )
                SELECT 
                    EmployeeID,
                    ProjectList,
                    ProjectCount,
                    GroupRank
                FROM ProjectAssignments;
            ";

            Console.WriteLine(sql);
        }
        #endregion

        #region Interview Questions
        /*
        COMMON INTERVIEW QUESTIONS:

        1. What is the difference between ROW_NUMBER, RANK, and DENSE_RANK?
           - ROW_NUMBER: Unique sequential numbers, no ties
           - RANK: Allows ties, creates gaps (1,2,2,4)
           - DENSE_RANK: Allows ties, no gaps (1,2,2,3)

        2. When to use LAG vs LEAD?
           - LAG: Access previous row (historical comparison)
           - LEAD: Access next row (forward-looking analysis)

        3. What is PARTITION BY in window functions?
           - Divides result set into partitions
           - Window function applied to each partition independently

        4. Difference between ROWS and RANGE?
           - ROWS: Physical row offset
           - RANGE: Logical value-based offset

        5. Can you use window functions in WHERE clause?
           - No, must use in SELECT or ORDER BY
           - Use CTE or subquery to filter on window function results

        6. What is FIRST_VALUE vs MIN?
           - FIRST_VALUE: Returns first value in ordered set
           - MIN: Returns minimum value (not order-dependent)

        7. How to calculate running total?
           - SUM() OVER (ORDER BY column ROWS UNBOUNDED PRECEDING)

        8. What is NTILE used for?
           - Divide rows into N equal groups
           - Useful for quartiles, deciles, percentiles

        9. Performance considerations for window functions?
           - May require sorting (expensive)
           - Index on PARTITION BY and ORDER BY columns
           - Consider materializing results for complex calculations

        10. Window functions vs GROUP BY?
            - GROUP BY: Reduces rows (aggregation)
            - Window functions: Keeps all rows, adds calculated columns
            - Window functions can reference individual row data
        */
        #endregion
    }
}
