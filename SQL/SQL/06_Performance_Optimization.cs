using System;

namespace SQLPreparation
{
    /// <summary>
    /// Day 8-9: Performance Optimization and Indexing
    /// Covers: Indexes, Execution Plans, Query Optimization, Statistics
    /// </summary>
    public class Performance_Optimization
    {
        public static void RunAllDemos()
        {
            Console.WriteLine("=== PERFORMANCE OPTIMIZATION DEMO ===\n");

            IndexTypes();
            IndexStrategies();
            ExecutionPlans();
            QueryOptimization();
            Statistics();
            BestPractices();
        }

        #region Index Types
        static void IndexTypes()
        {
            Console.WriteLine("1. Index Types - Clustered, Non-Clustered, Unique, Filtered");
            
            string sql = @"
                -- CLUSTERED INDEX (only one per table, determines physical storage order)
                CREATE CLUSTERED INDEX IX_Employees_EmployeeID 
                ON Employees(EmployeeID);

                -- NON-CLUSTERED INDEX
                CREATE NONCLUSTERED INDEX IX_Employees_LastName 
                ON Employees(LastName);

                -- COMPOSITE INDEX (multiple columns)
                CREATE NONCLUSTERED INDEX IX_Employees_Dept_Salary 
                ON Employees(DepartmentID, Salary DESC);

                -- UNIQUE INDEX
                CREATE UNIQUE NONCLUSTERED INDEX IX_Employees_Email_Unique 
                ON Employees(Email);

                -- FILTERED INDEX (with WHERE clause)
                CREATE NONCLUSTERED INDEX IX_Employees_Active_Salary 
                ON Employees(Salary)
                WHERE IsActive = 1;

                -- COVERING INDEX (INCLUDE clause)
                CREATE NONCLUSTERED INDEX IX_Employees_Dept_Covering
                ON Employees(DepartmentID)
                INCLUDE (FirstName, LastName, Salary, HireDate);

                -- INDEX with COLUMNSTORE (for analytics)
                CREATE NONCLUSTERED COLUMNSTORE INDEX IX_Employees_Columnstore
                ON Employees(DepartmentID, Salary, HireDate);

                -- FULL-TEXT INDEX (for text search)
                CREATE FULLTEXT CATALOG EmployeeFullTextCatalog;
                CREATE FULLTEXT INDEX ON Employees(FirstName, LastName)
                KEY INDEX PK_Employees
                ON EmployeeFullTextCatalog;

                -- View index information
                SELECT 
                    i.name AS IndexName,
                    i.type_desc AS IndexType,
                    STUFF((
                        SELECT ', ' + c.name
                        FROM sys.index_columns ic
                        INNER JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
                        WHERE ic.object_id = i.object_id AND ic.index_id = i.index_id
                        ORDER BY ic.key_ordinal
                        FOR XML PATH('')
                    ), 1, 2, '') AS IndexColumns,
                    i.is_unique,
                    i.is_primary_key,
                    i.fill_factor
                FROM sys.indexes i
                WHERE i.object_id = OBJECT_ID('Employees')
                    AND i.type > 0; -- Exclude heap

                -- Drop indexes
                DROP INDEX IF EXISTS IX_Employees_LastName ON Employees;
                DROP INDEX IF EXISTS IX_Employees_Dept_Salary ON Employees;

                -- Rebuild index (defragment)
                ALTER INDEX IX_Employees_Email_Unique ON Employees REBUILD;

                -- Reorganize index (less resource intensive)
                ALTER INDEX IX_Employees_Email_Unique ON Employees REORGANIZE;

                -- Disable/Enable index
                ALTER INDEX IX_Employees_Email_Unique ON Employees DISABLE;
                ALTER INDEX IX_Employees_Email_Unique ON Employees REBUILD;  -- Enable
            ";

            Console.WriteLine(sql);
        }
        #endregion

        #region Index Strategies
        static void IndexStrategies()
        {
            Console.WriteLine("\n2. Index Strategies - When and What to Index");
            
            string sql = @"
                -- Find missing indexes (from DMV)
                SELECT 
                    migs.avg_total_user_cost * (migs.avg_user_impact / 100.0) * (migs.user_seeks + migs.user_scans) AS improvement_measure,
                    'CREATE NONCLUSTERED INDEX IX_' + 
                        OBJECT_NAME(mid.object_id) + '_' +
                        REPLACE(REPLACE(REPLACE(ISNULL(mid.equality_columns,''),', ','_'),'[',''),']','') + '_' +
                        REPLACE(REPLACE(REPLACE(ISNULL(mid.inequality_columns,''),', ','_'),'[',''),']','') +
                    ' ON ' + mid.statement + ' (' + ISNULL(mid.equality_columns,'') +
                        CASE WHEN mid.equality_columns IS NOT NULL AND mid.inequality_columns IS NOT NULL THEN ',' ELSE '' END +
                        ISNULL(mid.inequality_columns, '') + ')' +
                        ISNULL(' INCLUDE (' + mid.included_columns + ')', '') AS create_index_statement,
                    migs.user_seeks,
                    migs.user_scans,
                    migs.last_user_seek,
                    migs.last_user_scan
                FROM sys.dm_db_missing_index_groups mig
                INNER JOIN sys.dm_db_missing_index_group_stats migs ON migs.group_handle = mig.index_group_handle
                INNER JOIN sys.dm_db_missing_index_details mid ON mig.index_handle = mid.index_handle
                WHERE migs.avg_total_user_cost * (migs.avg_user_impact / 100.0) * (migs.user_seeks + migs.user_scans) > 10
                    AND mid.database_id = DB_ID()
                ORDER BY improvement_measure DESC;

                -- Find unused indexes
                SELECT 
                    OBJECT_NAME(i.object_id) AS TableName,
                    i.name AS IndexName,
                    i.type_desc AS IndexType,
                    ius.user_seeks,
                    ius.user_scans,
                    ius.user_lookups,
                    ius.user_updates,
                    ius.last_user_seek,
                    ius.last_user_scan
                FROM sys.indexes i
                LEFT JOIN sys.dm_db_index_usage_stats ius 
                    ON i.object_id = ius.object_id AND i.index_id = ius.index_id AND ius.database_id = DB_ID()
                WHERE OBJECTPROPERTY(i.object_id, 'IsUserTable') = 1
                    AND i.type_desc <> 'HEAP'
                    AND i.is_primary_key = 0
                    AND i.is_unique_constraint = 0
                    AND (ius.user_seeks + ius.user_scans + ius.user_lookups) = 0
                ORDER BY ius.user_updates DESC;

                -- Index fragmentation
                SELECT 
                    OBJECT_NAME(ips.object_id) AS TableName,
                    i.name AS IndexName,
                    ips.index_type_desc,
                    ips.avg_fragmentation_in_percent,
                    ips.page_count,
                    CASE 
                        WHEN ips.avg_fragmentation_in_percent > 30 THEN 'REBUILD'
                        WHEN ips.avg_fragmentation_in_percent > 10 THEN 'REORGANIZE'
                        ELSE 'No Action Needed'
                    END AS Recommendation
                FROM sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, 'LIMITED') ips
                INNER JOIN sys.indexes i ON ips.object_id = i.object_id AND ips.index_id = i.index_id
                WHERE ips.page_count > 100
                ORDER BY ips.avg_fragmentation_in_percent DESC;

                -- Duplicate indexes
                WITH IndexColumns AS (
                    SELECT 
                        i.object_id,
                        i.index_id,
                        i.name AS IndexName,
                        STUFF((
                            SELECT ',' + c.name
                            FROM sys.index_columns ic
                            INNER JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
                            WHERE ic.object_id = i.object_id AND ic.index_id = i.index_id AND ic.is_included_column = 0
                            ORDER BY ic.key_ordinal
                            FOR XML PATH('')
                        ), 1, 1, '') AS KeyColumns
                    FROM sys.indexes i
                    WHERE i.type IN (1, 2) -- Clustered and Non-clustered only
                )
                SELECT 
                    OBJECT_NAME(ic1.object_id) AS TableName,
                    ic1.IndexName AS Index1,
                    ic2.IndexName AS Index2,
                    ic1.KeyColumns
                FROM IndexColumns ic1
                INNER JOIN IndexColumns ic2 
                    ON ic1.object_id = ic2.object_id 
                    AND ic1.KeyColumns = ic2.KeyColumns
                    AND ic1.index_id < ic2.index_id;

                -- Indexing strategy examples
                
                -- 1. Index for Foreign Keys
                CREATE NONCLUSTERED INDEX IX_Employees_DepartmentID 
                ON Employees(DepartmentID);

                -- 2. Index for frequent WHERE clauses
                CREATE NONCLUSTERED INDEX IX_Employees_IsActive_HireDate
                ON Employees(IsActive, HireDate)
                WHERE IsActive = 1;

                -- 3. Index for JOIN columns
                CREATE NONCLUSTERED INDEX IX_EmployeeProjects_EmployeeID 
                ON EmployeeProjects(EmployeeID);

                -- 4. Covering index for specific query
                CREATE NONCLUSTERED INDEX IX_Employees_DeptSalary_Covering
                ON Employees(DepartmentID, Salary DESC)
                INCLUDE (FirstName, LastName, Email);

                -- 5. Index for ORDER BY
                CREATE NONCLUSTERED INDEX IX_Employees_HireDate_Desc
                ON Employees(HireDate DESC);
            ";

            Console.WriteLine(sql);
        }
        #endregion

        #region Execution Plans
        static void ExecutionPlans()
        {
            Console.WriteLine("\n3. Execution Plans - Reading and Analyzing");
            
            string sql = @"
                -- Enable actual execution plan
                SET STATISTICS IO ON;
                SET STATISTICS TIME ON;

                -- Example query to analyze
                SELECT 
                    e.EmployeeID,
                    e.FirstName + ' ' + e.LastName AS FullName,
                    d.DepartmentName,
                    e.Salary
                FROM Employees e
                INNER JOIN Departments d ON e.DepartmentID = d.DepartmentID
                WHERE e.Salary > 70000
                ORDER BY e.Salary DESC;

                SET STATISTICS IO OFF;
                SET STATISTICS TIME OFF;

                -- Show estimated execution plan
                SET SHOWPLAN_ALL ON;
                SELECT * FROM Employees WHERE Salary > 70000;
                SET SHOWPLAN_ALL OFF;

                -- Query Store (SQL Server 2016+)
                -- View top queries by execution time
                SELECT 
                    qt.query_sql_text,
                    q.query_id,
                    rs.avg_duration / 1000.0 AS avg_duration_ms,
                    rs.avg_cpu_time / 1000.0 AS avg_cpu_ms,
                    rs.avg_logical_io_reads,
                    rs.avg_physical_io_reads,
                    rs.count_executions
                FROM sys.query_store_query q
                INNER JOIN sys.query_store_query_text qt ON q.query_text_id = qt.query_text_id
                INNER JOIN sys.query_store_plan p ON q.query_id = p.query_id
                INNER JOIN sys.query_store_runtime_stats rs ON p.plan_id = rs.plan_id
                WHERE qt.query_sql_text LIKE '%Employees%'
                ORDER BY rs.avg_duration DESC;

                -- Operators to watch for in execution plans:
                /*
                SCAN vs SEEK:
                - Index Scan: Reads entire index (slower)
                - Index Seek: Reads specific rows using index (faster)
                - Table Scan: Reads entire table (worst)

                JOIN types:
                - Nested Loops: Good for small datasets
                - Hash Match: Good for large datasets
                - Merge Join: Good for sorted data

                Other operators:
                - Key Lookup: Additional lookup after index seek (consider covering index)
                - Sort: Expensive operation (consider index on ORDER BY columns)
                - Filter: Better to use WHERE clause with index
                - Compute Scalar: Usually fine
                - Stream Aggregate: Good for grouped operations
                */

                -- Query hints (use with caution)
                
                -- Force specific index
                SELECT * FROM Employees WITH (INDEX(IX_Employees_DepartmentID))
                WHERE DepartmentID = 1;

                -- Force index seek
                SELECT * FROM Employees WITH (FORCESEEK)
                WHERE DepartmentID = 1;

                -- Force table scan
                SELECT * FROM Employees WITH (INDEX(0))
                WHERE Salary > 50000;

                -- No lock (dirty read)
                SELECT * FROM Employees WITH (NOLOCK)
                WHERE IsActive = 1;

                -- Optimize for specific parameter value
                DECLARE @DeptID INT = 1;
                SELECT * FROM Employees 
                WHERE DepartmentID = @DeptID
                OPTION (OPTIMIZE FOR (@DeptID = 1));

                -- Force recompile
                SELECT * FROM Employees 
                WHERE DepartmentID = 1
                OPTION (RECOMPILE);

                -- Maxdop (limit parallelism)
                SELECT * FROM Employees 
                WHERE Salary > 50000
                OPTION (MAXDOP 4);
            ";

            Console.WriteLine(sql);
        }
        #endregion

        #region Query Optimization
        static void QueryOptimization()
        {
            Console.WriteLine("\n4. Query Optimization Techniques");
            
            string sql = @"
                -- Bad: SELECT *
                SELECT * FROM Employees WHERE DepartmentID = 1;

                -- Good: SELECT specific columns
                SELECT EmployeeID, FirstName, LastName, Salary 
                FROM Employees 
                WHERE DepartmentID = 1;

                -- Bad: Function on indexed column prevents index usage
                SELECT * FROM Employees 
                WHERE YEAR(HireDate) = 2023;

                -- Good: Rewrite to use index
                SELECT * FROM Employees 
                WHERE HireDate >= '2023-01-01' AND HireDate < '2024-01-01';

                -- Bad: OR with different columns
                SELECT * FROM Employees 
                WHERE DepartmentID = 1 OR Salary > 100000;

                -- Good: Use UNION (if appropriate)
                SELECT * FROM Employees WHERE DepartmentID = 1
                UNION
                SELECT * FROM Employees WHERE Salary > 100000;

                -- Bad: NOT IN with NULL possibility
                SELECT * FROM Employees 
                WHERE DepartmentID NOT IN (SELECT DepartmentID FROM Departments WHERE Budget = 0);

                -- Good: NOT EXISTS (handles NULLs correctly)
                SELECT * FROM Employees e
                WHERE NOT EXISTS (
                    SELECT 1 FROM Departments d 
                    WHERE d.DepartmentID = e.DepartmentID AND d.Budget = 0
                );

                -- Bad: Implicit conversion
                SELECT * FROM Employees WHERE EmployeeID = '123';  -- EmployeeID is INT

                -- Good: Explicit types
                SELECT * FROM Employees WHERE EmployeeID = 123;

                -- Bad: Multiple SCALAR functions
                SELECT 
                    EmployeeID,
                    dbo.GetDepartmentName(DepartmentID) AS DeptName,  -- Called for each row
                    dbo.GetManagerName(ManagerID) AS ManagerName
                FROM Employees;

                -- Good: Use JOINs
                SELECT 
                    e.EmployeeID,
                    d.DepartmentName,
                    m.FirstName + ' ' + m.LastName AS ManagerName
                FROM Employees e
                LEFT JOIN Departments d ON e.DepartmentID = d.DepartmentID
                LEFT JOIN Employees m ON e.ManagerID = m.EmployeeID;

                -- Bad: Correlated subquery in SELECT
                SELECT 
                    e.EmployeeID,
                    (SELECT COUNT(*) FROM EmployeeProjects WHERE EmployeeID = e.EmployeeID) AS ProjectCount,
                    (SELECT AVG(Salary) FROM Employees WHERE DepartmentID = e.DepartmentID) AS AvgSalary
                FROM Employees e;

                -- Good: Use JOINs or window functions
                SELECT 
                    e.EmployeeID,
                    COUNT(ep.ProjectID) AS ProjectCount,
                    AVG(e2.Salary) OVER (PARTITION BY e.DepartmentID) AS AvgSalary
                FROM Employees e
                LEFT JOIN EmployeeProjects ep ON e.EmployeeID = ep.EmployeeID
                CROSS JOIN Employees e2 WHERE e2.DepartmentID = e.DepartmentID
                GROUP BY e.EmployeeID, e.DepartmentID;

                -- Bad: DISTINCT when not needed
                SELECT DISTINCT e.EmployeeID, e.FirstName 
                FROM Employees e;

                -- Good: Remove DISTINCT if primary key is selected
                SELECT e.EmployeeID, e.FirstName 
                FROM Employees e;

                -- Pagination optimization
                
                -- Bad: OFFSET/FETCH with large offsets
                SELECT EmployeeID, FirstName, LastName
                FROM Employees
                ORDER BY EmployeeID
                OFFSET 100000 ROWS FETCH NEXT 10 ROWS ONLY;

                -- Good: Use keyset pagination
                DECLARE @LastEmployeeID INT = 100000;
                SELECT TOP 10 EmployeeID, FirstName, LastName
                FROM Employees
                WHERE EmployeeID > @LastEmployeeID
                ORDER BY EmployeeID;

                -- Use proper data types
                -- Bad: VARCHAR for dates
                -- Good: DATE or DATETIME2

                -- Avoid leading wildcards in LIKE
                -- Bad: LIKE '%smith'
                -- Good: LIKE 'smith%' or Full-Text Search

                -- Use EXISTS instead of COUNT
                -- Bad: IF (SELECT COUNT(*) FROM Employees WHERE DepartmentID = 1) > 0
                -- Good: IF EXISTS (SELECT 1 FROM Employees WHERE DepartmentID = 1)
            ";

            Console.WriteLine(sql);
        }
        #endregion

        #region Statistics
        static void Statistics()
        {
            Console.WriteLine("\n5. Statistics - Query Optimizer's Friend");
            
            string sql = @"
                -- View statistics
                DBCC SHOW_STATISTICS('Employees', 'IX_Employees_DepartmentID');

                -- Update statistics
                UPDATE STATISTICS Employees;

                -- Update statistics with full scan
                UPDATE STATISTICS Employees WITH FULLSCAN;

                -- Update statistics for specific index
                UPDATE STATISTICS Employees IX_Employees_DepartmentID WITH FULLSCAN;

                -- Create statistics
                CREATE STATISTICS Stats_Employees_Salary ON Employees(Salary);

                -- Drop statistics
                DROP STATISTICS Employees.Stats_Employees_Salary;

                -- Auto update statistics (database level)
                ALTER DATABASE SQLDemo SET AUTO_UPDATE_STATISTICS ON;
                ALTER DATABASE SQLDemo SET AUTO_CREATE_STATISTICS ON;

                -- View statistics info
                SELECT 
                    OBJECT_NAME(s.object_id) AS TableName,
                    s.name AS StatisticName,
                    s.auto_created,
                    s.user_created,
                    s.has_filter,
                    sp.last_updated,
                    sp.rows,
                    sp.rows_sampled,
                    sp.modification_counter
                FROM sys.stats s
                CROSS APPLY sys.dm_db_stats_properties(s.object_id, s.stats_id) sp
                WHERE s.object_id = OBJECT_ID('Employees')
                ORDER BY sp.modification_counter DESC;

                -- Check if statistics are outdated
                SELECT 
                    OBJECT_NAME(object_id) AS TableName,
                    name AS StatisticName,
                    STATS_DATE(object_id, stats_id) AS LastUpdated,
                    DATEDIFF(DAY, STATS_DATE(object_id, stats_id), GETDATE()) AS DaysSinceUpdate
                FROM sys.stats
                WHERE STATS_DATE(object_id, stats_id) < DATEADD(DAY, -7, GETDATE())
                    AND OBJECTPROPERTY(object_id, 'IsUserTable') = 1;

                -- Filtered statistics
                CREATE STATISTICS Stats_ActiveEmployees_Salary 
                ON Employees(Salary) 
                WHERE IsActive = 1;
            ";

            Console.WriteLine(sql);
        }
        #endregion

        #region Best Practices
        static void BestPractices()
        {
            Console.WriteLine("\n6. Performance Best Practices");
            
            string sql = @"
                /*
                INDEXING BEST PRACTICES:
                1. Index foreign key columns
                2. Index columns in WHERE, JOIN, ORDER BY clauses
                3. Don't over-index (slows INSERT/UPDATE/DELETE)
                4. Use covering indexes for frequently run queries
                5. Regularly maintain indexes (rebuild/reorganize)
                6. Monitor and remove unused indexes
                7. Use filtered indexes for subset queries
                8. Consider columnstore for analytics workloads

                QUERY WRITING BEST PRACTICES:
                1. SELECT only needed columns (avoid SELECT *)
                2. Use appropriate JOINs instead of subqueries when possible
                3. Avoid functions on indexed columns in WHERE
                4. Use EXISTS instead of IN for large datasets
                5. Use proper data types (avoid implicit conversions)
                6. Parameterize queries to prevent SQL injection and aid plan reuse
                7. Use WITH (NOLOCK) carefully (dirty reads)
                8. Batch large operations
                9. Use appropriate transaction isolation levels
                10. Consider query hints only when necessary

                TABLE DESIGN BEST PRACTICES:
                1. Normalize to reduce redundancy
                2. Denormalize for read performance when necessary
                3. Use appropriate data types (smallest that fits)
                4. Define primary keys on all tables
                5. Use clustered index on most accessed column
                6. Consider partitioning for very large tables
                7. Use computed columns wisely
                8. Archive old data regularly

                MONITORING AND MAINTENANCE:
                1. Regular index maintenance schedule
                2. Update statistics regularly
                3. Monitor query performance with Query Store
                4. Review execution plans for slow queries
                5. Monitor wait statistics
                6. Check for blocking and deadlocks
                7. Monitor disk I/O and memory usage
                8. Set up alerts for performance degradation
                9. Maintain database backups
                10. Test performance in production-like environment

                COMMON PERFORMANCE KILLERS:
                1. Table scans on large tables
                2. Excessive blocking/deadlocks
                3. Missing indexes
                4. Outdated statistics
                5. Parameter sniffing issues
                6. Implicit conversions
                7. Nested views
                8. Cursor usage (consider set-based alternatives)
                9. Large transactions
                10. TempDB contention
                */

                -- Performance monitoring queries
                
                -- Current sessions and blocking
                SELECT 
                    session_id,
                    blocking_session_id,
                    wait_type,
                    wait_time,
                    wait_resource,
                    cpu_time,
                    logical_reads,
                    reads,
                    writes
                FROM sys.dm_exec_requests
                WHERE session_id > 50
                ORDER BY cpu_time DESC;

                -- Top CPU queries
                SELECT TOP 10
                    qs.total_worker_time / qs.execution_count AS avg_cpu_time,
                    qs.execution_count,
                    SUBSTRING(st.text, (qs.statement_start_offset/2) + 1,
                        ((CASE qs.statement_end_offset WHEN -1 THEN DATALENGTH(st.text)
                          ELSE qs.statement_end_offset END - qs.statement_start_offset)/2) + 1) AS query_text
                FROM sys.dm_exec_query_stats qs
                CROSS APPLY sys.dm_exec_sql_text(qs.sql_handle) st
                ORDER BY avg_cpu_time DESC;

                -- Wait statistics
                SELECT 
                    wait_type,
                    wait_time_ms / 1000.0 AS wait_time_seconds,
                    waiting_tasks_count,
                    wait_time_ms / waiting_tasks_count AS avg_wait_ms
                FROM sys.dm_os_wait_stats
                WHERE wait_type NOT LIKE '%SLEEP%'
                    AND waiting_tasks_count > 0
                ORDER BY wait_time_ms DESC;
            ";

            Console.WriteLine(sql);
        }
        #endregion

        #region Interview Questions
        /*
        COMMON INTERVIEW QUESTIONS:

        1. Difference between clustered and non-clustered index?
           - Clustered: Physical order, one per table, leaf nodes contain data
           - Non-clustered: Logical order, multiple allowed, leaf nodes contain pointers

        2. What is a covering index?
           - Index that contains all columns needed for a query
           - Avoids key lookup, improves performance

        3. When should you rebuild vs reorganize an index?
           - Rebuild: Fragmentation > 30%, offline operation, resets statistics
           - Reorganize: Fragmentation 10-30%, online, compacts pages

        4. What is index seek vs index scan?
           - Seek: Uses index to find specific rows (fast)
           - Scan: Reads entire index or table (slow)

        5. What causes parameter sniffing?
           - Query plan cached based on first parameter value
           - May not be optimal for other parameter values
           - Solutions: OPTION (RECOMPILE), OPTIMIZE FOR, plan guides

        6. How to identify missing indexes?
           - sys.dm_db_missing_index_* DMVs
           - Execution plan recommendations
           - Query Store

        7. What is statistics and why important?
           - Histograms of data distribution
           - Query optimizer uses to choose execution plan
           - Outdated statistics lead to poor plans

        8. What causes key lookup?
           - Index seek followed by lookup to get non-indexed columns
           - Solution: Covering index with INCLUDE

        9. Best practices for indexing?
           - Index foreign keys, WHERE/JOIN columns
           - Don't over-index
           - Regular maintenance
           - Monitor usage

        10. How to troubleshoot slow query?
            - Check execution plan
            - Look for scans, sorts, key lookups
            - Check statistics
            - Review indexes
            - Consider query rewrite
        */
        #endregion
    }
}
