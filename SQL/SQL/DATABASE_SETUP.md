# SQL Preparation Guide - Database Setup

## Prerequisites
- SQL Server 2016 or later (Express Edition is fine)
- SQL Server Management Studio (SSMS) or Azure Data Studio
- .NET 10 SDK

## Database Setup

### Option 1: Quick Setup Script

Run this script in SSMS to set up the demo database:

```sql
-- Create Database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'SQLDemo')
BEGIN
    CREATE DATABASE SQLDemo;
END
GO

USE SQLDemo;
GO

-- Create Tables
-- Departments
CREATE TABLE Departments (
    DepartmentID INT IDENTITY(1,1) PRIMARY KEY,
    DepartmentName NVARCHAR(100) NOT NULL UNIQUE,
    Location NVARCHAR(100),
    Budget DECIMAL(18,2),
    ManagerID INT,
    CreatedDate DATETIME2 DEFAULT SYSDATETIME()
);

-- Employees
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

-- Projects
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

-- EmployeeProjects
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

-- SalaryHistory
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

-- AuditLog
CREATE TABLE AuditLog (
    AuditID BIGINT IDENTITY(1,1) PRIMARY KEY,
    TableName NVARCHAR(100),
    Operation VARCHAR(10),
    PerformedBy NVARCHAR(100),
    PerformedAt DATETIME2 DEFAULT SYSDATETIME(),
    OldValues NVARCHAR(MAX),
    NewValues NVARCHAR(MAX)
);

-- Insert Sample Data
-- Departments
INSERT INTO Departments (DepartmentName, Location, Budget)
VALUES 
    ('IT', 'New York', 1500000),
    ('HR', 'Chicago', 500000),
    ('Finance', 'Boston', 1200000),
    ('Marketing', 'Los Angeles', 800000),
    ('Sales', 'Miami', 1000000),
    ('Engineering', 'Seattle', 2000000);

-- Employees (Sample data)
INSERT INTO Employees (FirstName, LastName, Email, Phone, HireDate, Salary, DepartmentID, IsActive)
VALUES 
    ('John', 'Doe', 'john.doe@company.com', '555-0101', '2020-01-15', 85000, 1, 1),
    ('Jane', 'Smith', 'jane.smith@company.com', '555-0102', '2019-03-20', 92000, 1, 1),
    ('Michael', 'Johnson', 'michael.j@company.com', '555-0103', '2021-05-10', 78000, 2, 1),
    ('Emily', 'Davis', 'emily.d@company.com', '555-0104', '2018-07-22', 95000, 3, 1),
    ('Robert', 'Wilson', 'robert.w@company.com', '555-0105', '2020-09-30', 88000, 1, 1),
    ('Sarah', 'Brown', 'sarah.b@company.com', '555-0106', '2019-11-12', 82000, 4, 1),
    ('David', 'Lee', 'david.l@company.com', '555-0107', '2022-02-14', 75000, 5, 1),
    ('Lisa', 'Anderson', 'lisa.a@company.com', '555-0108', '2017-04-18', 105000, 6, 1),
    ('James', 'Taylor', 'james.t@company.com', '555-0109', '2020-06-25', 79000, 2, 1),
    ('Jennifer', 'Martinez', 'jennifer.m@company.com', '555-0110', '2021-08-30', 91000, 3, 1);

-- Projects
INSERT INTO Projects (ProjectName, StartDate, EndDate, Budget, DepartmentID, Status)
VALUES 
    ('Website Redesign', '2024-01-01', '2024-06-30', 500000, 1, 'Active'),
    ('ERP Implementation', '2024-02-01', '2024-12-31', 1500000, 6, 'Active'),
    ('Marketing Campaign Q1', '2024-01-01', '2024-03-31', 200000, 4, 'Completed'),
    ('Sales Training Program', '2024-03-01', NULL, 150000, 5, 'Active'),
    ('Financial Audit', '2024-01-15', '2024-02-28', 100000, 3, 'Completed');

-- EmployeeProjects
INSERT INTO EmployeeProjects (EmployeeID, ProjectID, Role, HoursAllocated, AssignedDate)
VALUES 
    (1, 1, 'Lead Developer', 160, '2024-01-01'),
    (2, 1, 'Developer', 140, '2024-01-01'),
    (5, 1, 'Developer', 120, '2024-01-15'),
    (8, 2, 'Project Manager', 160, '2024-02-01'),
    (6, 3, 'Marketing Lead', 100, '2024-01-01'),
    (7, 4, 'Trainer', 80, '2024-03-01'),
    (4, 5, 'Financial Analyst', 120, '2024-01-15');

-- SalaryHistory
INSERT INTO SalaryHistory (EmployeeID, OldSalary, NewSalary, ChangeDate, Reason)
VALUES 
    (1, 75000, 85000, '2023-01-01', 'Annual raise'),
    (2, 85000, 92000, '2023-01-01', 'Promotion'),
    (4, 88000, 95000, '2023-06-01', 'Performance bonus'),
    (8, 95000, 105000, '2023-01-01', 'Market adjustment');

PRINT 'Database setup completed successfully!';
PRINT 'Tables created: Departments, Employees, Projects, EmployeeProjects, SalaryHistory, AuditLog';
PRINT 'Sample data inserted.';
GO
```

## Connection String Configuration

Update the connection string in each demo file:

```csharp
private static string connectionString = "Server=localhost;Database=SQLDemo;Integrated Security=true;TrustServerCertificate=true;";
```

### Connection String Options:

1. **Windows Authentication (Recommended for local development):**
   ```
   Server=localhost;Database=SQLDemo;Integrated Security=true;TrustServerCertificate=true;
   ```

2. **SQL Server Authentication:**
   ```
   Server=localhost;Database=SQLDemo;User Id=sa;Password=YourPassword;TrustServerCertificate=true;
   ```

3. **Named Instance:**
   ```
   Server=localhost\SQLEXPRESS;Database=SQLDemo;Integrated Security=true;TrustServerCertificate=true;
   ```

4. **Azure SQL Database:**
   ```
   Server=your-server.database.windows.net;Database=SQLDemo;User Id=username;Password=password;Encrypt=true;
   ```

## Running the Project

1. **Restore NuGet packages** (if needed):
   ```bash
   dotnet restore
   ```

2. **Build the project:**
   ```bash
   dotnet build
   ```

3. **Run the application:**
   ```bash
   dotnet run
   ```

## Troubleshooting

### Connection Issues

**Problem:** "Cannot connect to SQL Server"
- **Solution:** Ensure SQL Server is running. Check Services (services.msc) for "SQL Server (MSSQLSERVER)"

**Problem:** "Login failed for user"
- **Solution:** Use Windows Authentication or check SQL Server login credentials

**Problem:** "Database does not exist"
- **Solution:** Run the setup script above to create the database

### SQL Server Not Installed

1. Download SQL Server Express (free): https://www.microsoft.com/en-us/sql-server/sql-server-downloads
2. Download SSMS: https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms

## Additional Resources

### Learning Resources
- Microsoft SQL Server Documentation: https://docs.microsoft.com/en-us/sql/
- SQL Performance Explained: https://use-the-index-luke.com/
- LeetCode SQL Problems: https://leetcode.com/problemset/database/
- HackerRank SQL: https://www.hackerrank.com/domains/sql

### Practice Datasets
- AdventureWorks: https://github.com/Microsoft/sql-server-samples/tree/master/samples/databases/adventure-works
- Northwind: https://github.com/Microsoft/sql-server-samples/tree/master/samples/databases/northwind-pubs

### Interview Preparation
- Review all 8 demo files in order
- Focus on understanding concepts, not memorizing syntax
- Practice writing queries from scratch
- Review execution plans and optimization techniques
- Study real-world scenarios in Interview_Questions.cs

## Study Schedule

### Week 1: Core Concepts
- **Day 1-2:** DDL Operations - Run demos, practice creating tables with various constraints
- **Day 3-4:** DML + JOINs - Practice INSERT/UPDATE/DELETE and all JOIN types
- **Day 5-6:** Window Functions + CTEs - Master ROW_NUMBER, RANK, recursive queries
- **Day 7:** Review Week 1, practice complex queries

### Week 2: Advanced Topics
- **Day 8-9:** Performance - Learn indexing strategies, read execution plans
- **Day 10-11:** Transactions - Understand ACID, isolation levels, deadlocks
- **Day 12-13:** Review all concepts, practice challenging problems
- **Day 14:** Interview Questions - Practice all scenarios, time yourself

## Tips for Success

1. **Hands-on Practice:** Type queries yourself, don't just read
2. **Understand, Don't Memorize:** Focus on concepts and when to use each technique
3. **Execution Plans:** Always check query performance
4. **Real-World Context:** Think about business problems
5. **Daily Practice:** 1-2 hours per day for consistent improvement
6. **Mock Interviews:** Practice explaining your solutions out loud

## Support

For issues or questions:
- Check the inline comments in each demo file
- Review the interview questions section in 08_Interview_Questions.cs
- Refer to Microsoft SQL Server documentation
- Practice on LeetCode SQL section

Good luck with your SQL preparation! ??
