using System;
using System.Data.SqlClient;

namespace SQLPreparation
{
    /// <summary>
    /// Day 3-4: DML Operations - Data Manipulation Language
    /// Covers: INSERT, UPDATE, DELETE, MERGE, OUTPUT clause
    /// </summary>
    public class DML_Operations
    {
        private static string connectionString = "Server=localhost;Database=SQLDemo;Integrated Security=true;TrustServerCertificate=true;";

        public static void RunAllDemos()
        {
            Console.WriteLine("=== DML OPERATIONS DEMO ===\n");

            InsertOperations();
            UpdateOperations();
            DeleteOperations();
            MergeOperations();
            OutputClause();
            BulkOperations();
        }

        #region INSERT Operations
        static void InsertOperations()
        {
            Console.WriteLine("1. INSERT Operations - Various Techniques");
            
            string sql = @"
                -- Basic INSERT
                INSERT INTO Departments (DepartmentName, Location, Budget)
                VALUES ('IT', 'New York', 1000000);

                -- Multiple rows INSERT
                INSERT INTO Departments (DepartmentName, Location, Budget)
                VALUES 
                    ('HR', 'Chicago', 500000),
                    ('Finance', 'Boston', 800000),
                    ('Marketing', 'Los Angeles', 600000),
                    ('Sales', 'Miami', 750000);

                -- INSERT with SELECT
                INSERT INTO Departments (DepartmentName, Location, Budget)
                SELECT 'Engineering', 'Seattle', 1200000;

                -- INSERT from another table
                INSERT INTO AuditLog (TableName, Operation, PerformedBy)
                SELECT 'Departments', 'INSERT', SYSTEM_USER;

                -- INSERT with DEFAULT values
                INSERT INTO Employees (FirstName, LastName, Email, DepartmentID)
                VALUES ('John', 'Doe', 'john.doe@company.com', 1);

                -- INSERT with explicit NULL
                INSERT INTO Employees (FirstName, LastName, Email, Phone, DepartmentID)
                VALUES ('Jane', 'Smith', 'jane.smith@company.com', NULL, 2);

                -- INSERT with IDENTITY_INSERT (override auto-increment)
                SET IDENTITY_INSERT Employees ON;
                INSERT INTO Employees (EmployeeID, FirstName, LastName, Email, DepartmentID)
                VALUES (100, 'Admin', 'User', 'admin@company.com', 1);
                SET IDENTITY_INSERT Employees OFF;

                -- INSERT with SELECT and WHERE
                INSERT INTO SalaryHistory (EmployeeID, NewSalary, Reason)
                SELECT EmployeeID, Salary, 'Initial Salary'
                FROM Employees
                WHERE Salary IS NOT NULL;

                -- INSERT with TOP
                INSERT INTO AuditLog (TableName, Operation)
                SELECT TOP 10 'Employees', 'BULK_INSERT'
                FROM Employees;

                -- INSERT with UNION (multiple sources)
                INSERT INTO Employees (FirstName, LastName, Email, DepartmentID, Salary)
                SELECT 'Alice', 'Johnson', 'alice.j@company.com', 1, 75000
                UNION ALL
                SELECT 'Bob', 'Williams', 'bob.w@company.com', 2, 68000
                UNION ALL
                SELECT 'Charlie', 'Brown', 'charlie.b@company.com', 3, 82000;

                -- INSERT with computed values
                INSERT INTO Projects (ProjectName, StartDate, EndDate, Budget, DepartmentID)
                VALUES 
                    ('Project Alpha', GETDATE(), DATEADD(MONTH, 6, GETDATE()), 500000, 1),
                    ('Project Beta', GETDATE(), DATEADD(YEAR, 1, GETDATE()), 750000, 1);
            ";

            Console.WriteLine(sql);
        }
        #endregion

        #region UPDATE Operations
        static void UpdateOperations()
        {
            Console.WriteLine("\n2. UPDATE Operations - Various Techniques");
            
            string sql = @"
                -- Basic UPDATE
                UPDATE Employees
                SET Salary = 80000
                WHERE EmployeeID = 1;

                -- UPDATE multiple columns
                UPDATE Employees
                SET 
                    Salary = Salary * 1.10,  -- 10% raise
                    ModifiedDate = GETDATE()
                WHERE DepartmentID = 1;

                -- UPDATE with calculation
                UPDATE Employees
                SET Salary = Salary + (Salary * 0.05)
                WHERE HireDate < DATEADD(YEAR, -5, GETDATE());

                -- UPDATE with CASE
                UPDATE Employees
                SET Salary = CASE 
                    WHEN DepartmentID = 1 THEN Salary * 1.15
                    WHEN DepartmentID = 2 THEN Salary * 1.10
                    WHEN DepartmentID = 3 THEN Salary * 1.12
                    ELSE Salary * 1.08
                END;

                -- UPDATE from JOIN
                UPDATE e
                SET e.Salary = e.Salary * 1.05
                FROM Employees e
                INNER JOIN Departments d ON e.DepartmentID = d.DepartmentID
                WHERE d.DepartmentName = 'IT' AND d.Budget > 1000000;

                -- UPDATE with subquery
                UPDATE Employees
                SET Salary = (
                    SELECT AVG(Salary)
                    FROM Employees e2
                    WHERE e2.DepartmentID = Employees.DepartmentID
                )
                WHERE Salary IS NULL;

                -- UPDATE with TOP
                UPDATE TOP (10) Employees
                SET IsActive = 0
                WHERE HireDate < DATEADD(YEAR, -20, GETDATE());

                -- UPDATE all rows in department
                UPDATE Employees
                SET DepartmentID = 1
                WHERE DepartmentID IN (
                    SELECT DepartmentID 
                    FROM Departments 
                    WHERE Budget < 600000
                );

                -- UPDATE with JOIN to multiple tables
                UPDATE ep
                SET ep.HoursAllocated = ep.HoursAllocated * 1.2
                FROM EmployeeProjects ep
                INNER JOIN Projects p ON ep.ProjectID = p.ProjectID
                INNER JOIN Departments d ON p.DepartmentID = d.DepartmentID
                WHERE p.Status = 'Active' AND d.Budget > 800000;

                -- Conditional UPDATE
                UPDATE Employees
                SET Salary = CASE
                    WHEN Salary < 50000 THEN 50000
                    WHEN Salary > 200000 THEN 200000
                    ELSE Salary
                END;
            ";

            Console.WriteLine(sql);
        }
        #endregion

        #region DELETE Operations
        static void DeleteOperations()
        {
            Console.WriteLine("\n3. DELETE Operations - Various Techniques");
            
            string sql = @"
                -- Basic DELETE
                DELETE FROM Employees
                WHERE EmployeeID = 100;

                -- DELETE with WHERE clause
                DELETE FROM Employees
                WHERE IsActive = 0 AND HireDate < DATEADD(YEAR, -25, GETDATE());

                -- DELETE with JOIN
                DELETE e
                FROM Employees e
                INNER JOIN Departments d ON e.DepartmentID = d.DepartmentID
                WHERE d.DepartmentName = 'Temp';

                -- DELETE with subquery
                DELETE FROM Employees
                WHERE DepartmentID IN (
                    SELECT DepartmentID 
                    FROM Departments 
                    WHERE Budget = 0
                );

                -- DELETE with TOP
                DELETE TOP (10) FROM AuditLog
                WHERE PerformedAt < DATEADD(DAY, -90, GETDATE());

                -- DELETE with EXISTS
                DELETE FROM Employees
                WHERE EXISTS (
                    SELECT 1 
                    FROM EmployeeProjects ep
                    INNER JOIN Projects p ON ep.ProjectID = p.ProjectID
                    WHERE ep.EmployeeID = Employees.EmployeeID
                        AND p.Status = 'Cancelled'
                );

                -- DELETE all rows (better to use TRUNCATE if possible)
                DELETE FROM AuditLog;

                -- Self-referencing DELETE (remove duplicates)
                DELETE e1
                FROM Employees e1
                INNER JOIN Employees e2 ON e1.Email = e2.Email
                WHERE e1.EmployeeID > e2.EmployeeID;

                -- DELETE with NOT EXISTS (orphaned records)
                DELETE FROM EmployeeProjects
                WHERE NOT EXISTS (
                    SELECT 1 
                    FROM Employees e 
                    WHERE e.EmployeeID = EmployeeProjects.EmployeeID
                );
            ";

            Console.WriteLine(sql);
        }
        #endregion

        #region MERGE Operations
        static void MergeOperations()
        {
            Console.WriteLine("\n4. MERGE Operations - UPSERT Pattern");
            
            string sql = @"
                -- Create staging table for demo
                CREATE TABLE #EmployeeStaging (
                    EmployeeID INT,
                    FirstName NVARCHAR(50),
                    LastName NVARCHAR(50),
                    Email NVARCHAR(100),
                    Salary DECIMAL(18,2),
                    DepartmentID INT
                );

                -- Basic MERGE (INSERT, UPDATE, DELETE)
                MERGE INTO Employees AS Target
                USING #EmployeeStaging AS Source
                ON Target.EmployeeID = Source.EmployeeID
                WHEN MATCHED THEN
                    UPDATE SET 
                        Target.FirstName = Source.FirstName,
                        Target.LastName = Source.LastName,
                        Target.Email = Source.Email,
                        Target.Salary = Source.Salary,
                        Target.ModifiedDate = GETDATE()
                WHEN NOT MATCHED BY TARGET THEN
                    INSERT (FirstName, LastName, Email, Salary, DepartmentID)
                    VALUES (Source.FirstName, Source.LastName, Source.Email, 
                            Source.Salary, Source.DepartmentID)
                WHEN NOT MATCHED BY SOURCE THEN
                    DELETE;

                -- MERGE with conditional logic
                MERGE INTO Employees AS Target
                USING #EmployeeStaging AS Source
                ON Target.Email = Source.Email
                WHEN MATCHED AND Source.Salary > Target.Salary THEN
                    UPDATE SET 
                        Target.Salary = Source.Salary,
                        Target.ModifiedDate = GETDATE()
                WHEN MATCHED AND Source.Salary < Target.Salary THEN
                    UPDATE SET Target.ModifiedDate = GETDATE()
                WHEN NOT MATCHED BY TARGET THEN
                    INSERT (FirstName, LastName, Email, Salary, DepartmentID)
                    VALUES (Source.FirstName, Source.LastName, Source.Email, 
                            Source.Salary, Source.DepartmentID);

                -- MERGE with OUTPUT clause
                MERGE INTO Employees AS Target
                USING #EmployeeStaging AS Source
                ON Target.EmployeeID = Source.EmployeeID
                WHEN MATCHED THEN
                    UPDATE SET Target.Salary = Source.Salary
                WHEN NOT MATCHED THEN
                    INSERT (FirstName, LastName, Email, Salary, DepartmentID)
                    VALUES (Source.FirstName, Source.LastName, Source.Email, 
                            Source.Salary, Source.DepartmentID)
                OUTPUT 
                    $action AS Action,
                    INSERTED.EmployeeID,
                    INSERTED.FirstName,
                    DELETED.Salary AS OldSalary,
                    INSERTED.Salary AS NewSalary;

                -- MERGE with complex source
                MERGE INTO SalaryHistory AS Target
                USING (
                    SELECT 
                        e.EmployeeID,
                        e.Salary AS NewSalary,
                        sh.NewSalary AS OldSalary
                    FROM Employees e
                    LEFT JOIN (
                        SELECT EmployeeID, NewSalary,
                               ROW_NUMBER() OVER (PARTITION BY EmployeeID ORDER BY ChangeDate DESC) AS rn
                        FROM SalaryHistory
                    ) sh ON e.EmployeeID = sh.EmployeeID AND sh.rn = 1
                    WHERE e.Salary != ISNULL(sh.NewSalary, 0)
                ) AS Source
                ON Target.EmployeeID = Source.EmployeeID
                WHEN NOT MATCHED BY TARGET THEN
                    INSERT (EmployeeID, OldSalary, NewSalary, Reason)
                    VALUES (Source.EmployeeID, Source.OldSalary, Source.NewSalary, 'Salary Change');
            ";

            Console.WriteLine(sql);
        }
        #endregion

        #region OUTPUT Clause
        static void OutputClause()
        {
            Console.WriteLine("\n5. OUTPUT Clause - Track Changes");
            
            string sql = @"
                -- INSERT with OUTPUT
                INSERT INTO Employees (FirstName, LastName, Email, DepartmentID, Salary)
                OUTPUT 
                    INSERTED.EmployeeID,
                    INSERTED.FirstName,
                    INSERTED.LastName,
                    INSERTED.Email,
                    INSERTED.HireDate
                VALUES ('Mike', 'Davis', 'mike.d@company.com', 1, 75000);

                -- UPDATE with OUTPUT
                UPDATE Employees
                SET Salary = Salary * 1.10
                OUTPUT 
                    INSERTED.EmployeeID,
                    DELETED.Salary AS OldSalary,
                    INSERTED.Salary AS NewSalary,
                    INSERTED.Salary - DELETED.Salary AS Increase
                WHERE DepartmentID = 1;

                -- DELETE with OUTPUT
                DELETE FROM AuditLog
                OUTPUT 
                    DELETED.AuditID,
                    DELETED.TableName,
                    DELETED.Operation,
                    DELETED.PerformedAt
                WHERE PerformedAt < DATEADD(DAY, -30, GETDATE());

                -- OUTPUT INTO table
                DECLARE @DeletedEmployees TABLE (
                    EmployeeID INT,
                    FullName NVARCHAR(101),
                    Email NVARCHAR(100),
                    DeletedDate DATETIME2
                );

                DELETE FROM Employees
                OUTPUT 
                    DELETED.EmployeeID,
                    DELETED.FirstName + ' ' + DELETED.LastName,
                    DELETED.Email,
                    GETDATE()
                INTO @DeletedEmployees
                WHERE IsActive = 0;

                SELECT * FROM @DeletedEmployees;

                -- MERGE with OUTPUT
                MERGE INTO Employees AS Target
                USING (SELECT 1 AS EmployeeID, 'Updated' AS FirstName) AS Source
                ON Target.EmployeeID = Source.EmployeeID
                WHEN MATCHED THEN
                    UPDATE SET Target.FirstName = Source.FirstName
                OUTPUT 
                    $action AS Action,
                    DELETED.FirstName AS OldFirstName,
                    INSERTED.FirstName AS NewFirstName;
            ";

            Console.WriteLine(sql);
        }
        #endregion

        #region Bulk Operations
        static void BulkOperations()
        {
            Console.WriteLine("\n6. Bulk Operations - Performance Optimization");
            
            string sql = @"
                -- Bulk INSERT with transaction
                BEGIN TRANSACTION;
                BEGIN TRY
                    INSERT INTO Employees (FirstName, LastName, Email, DepartmentID, Salary)
                    SELECT 
                        'FirstName' + CAST(number AS VARCHAR),
                        'LastName' + CAST(number AS VARCHAR),
                        'email' + CAST(number AS VARCHAR) + '@company.com',
                        (number % 5) + 1,
                        50000 + (number * 1000)
                    FROM master..spt_values
                    WHERE type = 'P' AND number BETWEEN 1 AND 100;
                    
                    COMMIT TRANSACTION;
                END TRY
                BEGIN CATCH
                    ROLLBACK TRANSACTION;
                    THROW;
                END CATCH;

                -- Batch UPDATE for performance
                DECLARE @BatchSize INT = 1000;
                DECLARE @RowsAffected INT = @BatchSize;

                WHILE @RowsAffected = @BatchSize
                BEGIN
                    UPDATE TOP (@BatchSize) Employees
                    SET ModifiedDate = GETDATE()
                    WHERE ModifiedDate IS NULL;
                    
                    SET @RowsAffected = @@ROWCOUNT;
                END;

                -- Batch DELETE
                WHILE EXISTS (SELECT 1 FROM AuditLog WHERE PerformedAt < DATEADD(YEAR, -1, GETDATE()))
                BEGIN
                    DELETE TOP (1000) FROM AuditLog
                    WHERE PerformedAt < DATEADD(YEAR, -1, GETDATE());
                    
                    WAITFOR DELAY '00:00:01'; -- Prevent blocking
                END;
            ";

            Console.WriteLine(sql);
        }
        #endregion

        #region Interview Questions
        /*
        COMMON INTERVIEW QUESTIONS:

        1. What is the difference between DELETE and TRUNCATE?
           - DELETE: Row-by-row, WHERE clause, can rollback, logs, triggers fire
           - TRUNCATE: All rows, faster, minimal log, resets IDENTITY, no triggers

        2. What is MERGE statement and when to use it?
           - Combines INSERT, UPDATE, DELETE in single statement (UPSERT)
           - Useful for synchronization scenarios

        3. How does OUTPUT clause work?
           - Returns data from DML operations (INSERT, UPDATE, DELETE, MERGE)
           - INSERTED: New values, DELETED: Old values
           - Can output to table variable or table

        4. How to insert data from one table to another?
           - INSERT INTO table1 SELECT * FROM table2 WHERE condition

        5. How to update multiple tables in one query?
           - Cannot directly update multiple tables
           - Use UPDATE with JOIN or MERGE statement

        6. What is IDENTITY_INSERT?
           - Allows explicit insert into IDENTITY column
           - Must SET IDENTITY_INSERT ON before insert

        7. How to handle concurrency in DML operations?
           - Use transactions with appropriate isolation levels
           - Implement optimistic/pessimistic locking
           - Use ROWVERSION for concurrency detection

        8. What is @@ROWCOUNT?
           - Returns number of rows affected by last statement
           - Used for validation and batch processing

        9. Best practices for bulk inserts?
           - Use BULK INSERT for large CSV files
           - Batch operations for better performance
           - Disable indexes/constraints temporarily
           - Use appropriate transaction scope

        10. How to prevent duplicate inserts?
            - Use UNIQUE constraint
            - Check with IF NOT EXISTS
            - Use MERGE with proper ON condition
        */
        #endregion
    }
}
