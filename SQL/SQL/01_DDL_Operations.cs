using System;
using System.Data.SqlClient;

namespace SQLPreparation
{
    /// <summary>
    /// Day 1-2: DDL Operations - Data Definition Language
    /// Covers: CREATE, ALTER, DROP, TRUNCATE, Constraints, Data Types
    /// </summary>
    public class DDL_Operations
    {
        private static string connectionString = "Server=localhost;Database=SQLDemo;Integrated Security=true;TrustServerCertificate=true;";

        public static void RunAllDemos()
        {
            Console.WriteLine("=== DDL OPERATIONS DEMO ===\n");

            CreateDatabase();
            CreateTables();
            AlterTableOperations();
            ConstraintExamples();
            DropOperations();
        }

        #region Database Operations
        static void CreateDatabase()
        {
            Console.WriteLine("1. CREATE DATABASE");
            string sql = @"
                -- Create database if not exists
                IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'SQLDemo')
                BEGIN
                    CREATE DATABASE SQLDemo;
                END
                
                -- With specific options
                /*
                CREATE DATABASE SQLDemoAdvanced
                ON PRIMARY 
                (
                    NAME = SQLDemo_Data,
                    FILENAME = 'C:\SQLData\SQLDemo.mdf',
                    SIZE = 10MB,
                    MAXSIZE = 100MB,
                    FILEGROWTH = 5MB
                )
                LOG ON
                (
                    NAME = SQLDemo_Log,
                    FILENAME = 'C:\SQLData\SQLDemo_log.ldf',
                    SIZE = 5MB,
                    MAXSIZE = 50MB,
                    FILEGROWTH = 5MB
                );
                */
            ";
            Console.WriteLine(sql);
        }
        #endregion

        #region Table Creation
        static void CreateTables()
        {
            Console.WriteLine("\n2. CREATE TABLE - Multiple Examples");
            
            string sql = @"
                USE SQLDemo;
                GO

                -- Employees Table with Various Constraints
                CREATE TABLE Employees (
                    EmployeeID INT IDENTITY(1,1) PRIMARY KEY,
                    FirstName NVARCHAR(50) NOT NULL,
                    LastName NVARCHAR(50) NOT NULL,
                    Email NVARCHAR(100) UNIQUE NOT NULL,
                    Phone VARCHAR(20),
                    HireDate DATE DEFAULT GETDATE(),
                    Salary DECIMAL(18,2) CHECK (Salary > 0),
                    DepartmentID INT,
                    ManagerID INT,
                    IsActive BIT DEFAULT 1,
                    CreatedDate DATETIME2 DEFAULT SYSDATETIME(),
                    ModifiedDate DATETIME2,
                    SSN VARCHAR(11) UNIQUE,
                    CONSTRAINT FK_Employee_Department FOREIGN KEY (DepartmentID) 
                        REFERENCES Departments(DepartmentID),
                    CONSTRAINT FK_Employee_Manager FOREIGN KEY (ManagerID) 
                        REFERENCES Employees(EmployeeID),
                    CONSTRAINT CHK_Email_Format CHECK (Email LIKE '%@%.%')
                );

                -- Departments Table
                CREATE TABLE Departments (
                    DepartmentID INT IDENTITY(1,1) PRIMARY KEY,
                    DepartmentName NVARCHAR(100) NOT NULL UNIQUE,
                    Location NVARCHAR(100),
                    Budget DECIMAL(18,2),
                    ManagerID INT,
                    CreatedDate DATETIME2 DEFAULT SYSDATETIME()
                );

                -- Projects Table
                CREATE TABLE Projects (
                    ProjectID INT IDENTITY(1,1) PRIMARY KEY,
                    ProjectName NVARCHAR(200) NOT NULL,
                    StartDate DATE NOT NULL,
                    EndDate DATE,
                    Budget DECIMAL(18,2),
                    DepartmentID INT,
                    Status VARCHAR(20) DEFAULT 'Active',
                    CONSTRAINT FK_Project_Department FOREIGN KEY (DepartmentID) 
                        REFERENCES Departments(DepartmentID),
                    CONSTRAINT CHK_EndDate CHECK (EndDate IS NULL OR EndDate >= StartDate),
                    CONSTRAINT CHK_Status CHECK (Status IN ('Active', 'Completed', 'OnHold', 'Cancelled'))
                );

                -- EmployeeProjects (Many-to-Many relationship)
                CREATE TABLE EmployeeProjects (
                    EmployeeID INT,
                    ProjectID INT,
                    Role NVARCHAR(50),
                    HoursAllocated DECIMAL(5,2),
                    AssignedDate DATE DEFAULT GETDATE(),
                    PRIMARY KEY (EmployeeID, ProjectID),
                    CONSTRAINT FK_EP_Employee FOREIGN KEY (EmployeeID) 
                        REFERENCES Employees(EmployeeID) ON DELETE CASCADE,
                    CONSTRAINT FK_EP_Project FOREIGN KEY (ProjectID) 
                        REFERENCES Projects(ProjectID) ON DELETE CASCADE
                );

                -- Salaries History Table
                CREATE TABLE SalaryHistory (
                    SalaryHistoryID INT IDENTITY(1,1) PRIMARY KEY,
                    EmployeeID INT NOT NULL,
                    OldSalary DECIMAL(18,2),
                    NewSalary DECIMAL(18,2),
                    ChangeDate DATETIME2 DEFAULT SYSDATETIME(),
                    Reason NVARCHAR(200),
                    CONSTRAINT FK_SalaryHistory_Employee FOREIGN KEY (EmployeeID) 
                        REFERENCES Employees(EmployeeID)
                );

                -- Audit Log Table
                CREATE TABLE AuditLog (
                    AuditID BIGINT IDENTITY(1,1) PRIMARY KEY,
                    TableName NVARCHAR(100),
                    Operation VARCHAR(10),
                    PerformedBy NVARCHAR(100),
                    PerformedAt DATETIME2 DEFAULT SYSDATETIME(),
                    OldValues NVARCHAR(MAX),
                    NewValues NVARCHAR(MAX)
                );
            ";

            ExecuteNonQuery(sql);
            Console.WriteLine("Tables created successfully!");
        }
        #endregion

        #region Alter Operations
        static void AlterTableOperations()
        {
            Console.WriteLine("\n3. ALTER TABLE Operations");
            
            string sql = @"
                -- Add new column
                ALTER TABLE Employees ADD MiddleName NVARCHAR(50) NULL;

                -- Add column with default value
                ALTER TABLE Employees ADD Country NVARCHAR(50) DEFAULT 'USA';

                -- Modify column data type
                ALTER TABLE Employees ALTER COLUMN Phone VARCHAR(25);

                -- Add constraint
                ALTER TABLE Employees ADD CONSTRAINT CHK_Salary_Range 
                    CHECK (Salary BETWEEN 30000 AND 500000);

                -- Drop constraint
                ALTER TABLE Employees DROP CONSTRAINT IF EXISTS CHK_Salary_Range;

                -- Add computed column
                ALTER TABLE Employees ADD FullName AS (FirstName + ' ' + LastName) PERSISTED;

                -- Add default constraint
                ALTER TABLE Employees ADD CONSTRAINT DF_Country DEFAULT 'USA' FOR Country;

                -- Drop column
                ALTER TABLE Employees DROP COLUMN IF EXISTS MiddleName;

                -- Rename column (using sp_rename)
                EXEC sp_rename 'Employees.Phone', 'PhoneNumber', 'COLUMN';

                -- Add index
                CREATE INDEX IX_Employees_LastName ON Employees(LastName);
                CREATE INDEX IX_Employees_Email ON Employees(Email);
                CREATE INDEX IX_Employees_DepartmentID ON Employees(DepartmentID);
            ";

            ExecuteNonQuery(sql);
            Console.WriteLine("ALTER operations completed!");
        }
        #endregion

        #region Constraints
        static void ConstraintExamples()
        {
            Console.WriteLine("\n4. Constraint Examples");
            
            string sql = @"
                -- PRIMARY KEY Constraint
                -- Already shown in CREATE TABLE

                -- UNIQUE Constraint
                ALTER TABLE Employees ADD CONSTRAINT UQ_Employee_SSN UNIQUE (SSN);

                -- CHECK Constraint
                ALTER TABLE Employees ADD CONSTRAINT CHK_HireDate 
                    CHECK (HireDate <= GETDATE());

                -- DEFAULT Constraint
                ALTER TABLE Projects ADD CONSTRAINT DF_Project_Status 
                    DEFAULT 'Active' FOR Status;

                -- FOREIGN KEY with CASCADE options
                ALTER TABLE EmployeeProjects DROP CONSTRAINT IF EXISTS FK_EP_Employee;
                ALTER TABLE EmployeeProjects ADD CONSTRAINT FK_EP_Employee 
                    FOREIGN KEY (EmployeeID) REFERENCES Employees(EmployeeID)
                    ON DELETE CASCADE
                    ON UPDATE CASCADE;

                -- NOT NULL Constraint (requires ALTER COLUMN)
                ALTER TABLE Employees ALTER COLUMN FirstName NVARCHAR(50) NOT NULL;

                -- View constraints
                SELECT 
                    OBJECT_NAME(parent_object_id) AS TableName,
                    name AS ConstraintName,
                    type_desc AS ConstraintType
                FROM sys.objects
                WHERE type_desc LIKE '%CONSTRAINT'
                    AND OBJECT_NAME(parent_object_id) = 'Employees';
            ";

            Console.WriteLine(sql);
        }
        #endregion

        #region Drop Operations
        static void DropOperations()
        {
            Console.WriteLine("\n5. DROP Operations");
            
            string sql = @"
                -- Drop constraint
                ALTER TABLE Employees DROP CONSTRAINT IF EXISTS CHK_HireDate;

                -- Drop index
                DROP INDEX IF EXISTS IX_Employees_Email ON Employees;

                -- Drop table (with dependencies check)
                -- DROP TABLE IF EXISTS SalaryHistory;

                -- Truncate table (removes all data, keeps structure)
                TRUNCATE TABLE AuditLog;

                -- Drop database
                -- USE master;
                -- DROP DATABASE IF EXISTS SQLDemo;

                -- Key Differences:
                -- DELETE: Removes rows, logs each deletion, can rollback, slower
                -- TRUNCATE: Removes all rows, minimal logging, cannot rollback, faster, resets IDENTITY
                -- DROP: Removes entire table structure and data
            ";

            Console.WriteLine(sql);
        }
        #endregion

        #region Helper Methods
        static void ExecuteNonQuery(string sql)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        #endregion

        #region Interview Questions
        /* 
        COMMON INTERVIEW QUESTIONS:

        1. What is the difference between DELETE, TRUNCATE, and DROP?
           - DELETE: DML, row-by-row, logs, rollback, triggers fire, WHERE clause
           - TRUNCATE: DDL, all rows, minimal log, no rollback, no triggers, faster
           - DROP: DDL, removes table structure

        2. What are constraints and their types?
           - PRIMARY KEY, FOREIGN KEY, UNIQUE, CHECK, DEFAULT, NOT NULL

        3. What is the difference between PRIMARY KEY and UNIQUE?
           - PRIMARY KEY: Only one per table, cannot be NULL, clustered index by default
           - UNIQUE: Multiple allowed, can be NULL (one NULL), non-clustered index

        4. What is CASCADE in FOREIGN KEY?
           - ON DELETE CASCADE: Deletes child rows when parent is deleted
           - ON UPDATE CASCADE: Updates child rows when parent key is updated

        5. What is a COMPUTED COLUMN?
           - Virtual column calculated from other columns
           - PERSISTED: Stored physically, faster queries

        6. Difference between CHAR and VARCHAR?
           - CHAR(n): Fixed length, pads with spaces, faster for fixed-length data
           - VARCHAR(n): Variable length, saves space, better for variable-length data

        7. What is IDENTITY column?
           - Auto-incrementing column, typically used for PRIMARY KEY
           - IDENTITY(seed, increment)

        8. Can we have multiple PRIMARY KEYs?
           - No, only one PRIMARY KEY per table
           - But can be composite (multiple columns)

        9. What is the purpose of CHECK constraint?
           - Enforces domain integrity by limiting values

        10. How to view table structure?
            - sp_help TableName
            - INFORMATION_SCHEMA.COLUMNS
            - sys.columns
        */
        #endregion
    }
}
