using System;

namespace SQLPreparation
{
    /// <summary>
    /// Day 10-11: Transactions, Concurrency, and Error Handling
    /// Covers: ACID, Isolation Levels, Locks, Deadlocks, TRY-CATCH
    /// </summary>
    public class Transactions_Concurrency
    {
        public static void RunAllDemos()
        {
            Console.WriteLine("=== TRANSACTIONS AND CONCURRENCY DEMO ===\n");

            BasicTransactions();
            IsolationLevels();
            LockingAndBlocking();
            DeadlockHandling();
            ErrorHandling();
            ConcurrencyPatterns();
        }

        #region Basic Transactions
        static void BasicTransactions()
        {
            Console.WriteLine("1. Basic Transactions - ACID Properties");
            
            string sql = @"
                /*
                ACID PROPERTIES:
                - Atomicity: All or nothing
                - Consistency: Database remains in valid state
                - Isolation: Transactions don't interfere with each other
                - Durability: Committed changes persist
                */

                -- Basic transaction
                BEGIN TRANSACTION;
                    UPDATE Employees SET Salary = Salary * 1.10 WHERE DepartmentID = 1;
                    INSERT INTO AuditLog (TableName, Operation) VALUES ('Employees', 'Salary Update');
                COMMIT TRANSACTION;

                -- Transaction with ROLLBACK
                BEGIN TRANSACTION;
                    UPDATE Employees SET Salary = Salary * 1.10 WHERE DepartmentID = 1;
                    -- Something went wrong
                    IF @@ERROR <> 0
                        ROLLBACK TRANSACTION;
                    ELSE
                        COMMIT TRANSACTION;

                -- Named transactions
                BEGIN TRANSACTION SalaryUpdate;
                    UPDATE Employees SET Salary = Salary * 1.10 WHERE DepartmentID = 1;
                COMMIT TRANSACTION SalaryUpdate;

                -- Savepoints
                BEGIN TRANSACTION;
                    UPDATE Employees SET Salary = Salary * 1.10 WHERE DepartmentID = 1;
                    
                    SAVE TRANSACTION SavePoint1;
                    
                    UPDATE Employees SET Salary = Salary * 1.05 WHERE DepartmentID = 2;
                    
                    -- Rollback to savepoint
                    ROLLBACK TRANSACTION SavePoint1;
                    
                COMMIT TRANSACTION;

                -- Nested transactions (nesting level tracked, but rollback affects all)
                BEGIN TRANSACTION OuterTran;
                    UPDATE Employees SET Salary = Salary * 1.10 WHERE EmployeeID = 1;
                    
                    BEGIN TRANSACTION InnerTran;
                        UPDATE Departments SET Budget = Budget * 1.10 WHERE DepartmentID = 1;
                    COMMIT TRANSACTION InnerTran;
                    
                COMMIT TRANSACTION OuterTran;

                -- Check transaction state
                SELECT 
                    @@TRANCOUNT AS TransactionNestingLevel,
                    @@ERROR AS LastError,
                    XACT_STATE() AS TransactionState;
                    -- XACT_STATE: 1 = Committable, 0 = No transaction, -1 = Uncommittable

                -- Implicit transactions
                SET IMPLICIT_TRANSACTIONS ON;
                UPDATE Employees SET Salary = Salary * 1.10 WHERE EmployeeID = 1;
                COMMIT;  -- Required
                SET IMPLICIT_TRANSACTIONS OFF;

                -- Autocommit (default mode)
                UPDATE Employees SET Salary = Salary * 1.10 WHERE EmployeeID = 1;  -- Auto commits
            ";

            Console.WriteLine(sql);
        }
        #endregion

        #region Isolation Levels
        static void IsolationLevels()
        {
            Console.WriteLine("\n2. Transaction Isolation Levels");
            
            string sql = @"
                /*
                ISOLATION LEVELS (least to most restrictive):
                
                1. READ UNCOMMITTED (Level 0)
                   - Dirty reads allowed
                   - No locks acquired for reads
                   - Fastest, least safe
                
                2. READ COMMITTED (Level 1) - DEFAULT
                   - Prevents dirty reads
                   - Shared locks released after read
                   - Non-repeatable reads possible
                
                3. REPEATABLE READ (Level 2)
                   - Prevents dirty and non-repeatable reads
                   - Shared locks held until end of transaction
                   - Phantom reads possible
                
                4. SERIALIZABLE (Level 3)
                   - Prevents dirty, non-repeatable, and phantom reads
                   - Range locks
                   - Slowest, safest
                
                5. SNAPSHOT (special)
                   - Readers don't block writers, writers don't block readers
                   - Uses row versioning in tempdb
                   - No locks for reads
                */

                -- READ UNCOMMITTED (dirty reads)
                SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
                BEGIN TRANSACTION;
                    SELECT * FROM Employees WHERE DepartmentID = 1;
                COMMIT;
                -- Equivalent to:
                SELECT * FROM Employees WITH (NOLOCK) WHERE DepartmentID = 1;

                -- READ COMMITTED (default)
                SET TRANSACTION ISOLATION LEVEL READ COMMITTED;
                BEGIN TRANSACTION;
                    SELECT * FROM Employees WHERE DepartmentID = 1;
                    -- Another transaction can modify data between reads
                    WAITFOR DELAY '00:00:05';
                    SELECT * FROM Employees WHERE DepartmentID = 1;  -- Might be different
                COMMIT;

                -- REPEATABLE READ (consistent reads)
                SET TRANSACTION ISOLATION LEVEL REPEATABLE READ;
                BEGIN TRANSACTION;
                    SELECT * FROM Employees WHERE DepartmentID = 1;
                    WAITFOR DELAY '00:00:05';
                    SELECT * FROM Employees WHERE DepartmentID = 1;  -- Same results
                    -- But new rows can be inserted (phantom reads)
                COMMIT;

                -- SERIALIZABLE (no phantoms)
                SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
                BEGIN TRANSACTION;
                    SELECT * FROM Employees WHERE DepartmentID = 1;
                    WAITFOR DELAY '00:00:05';
                    SELECT * FROM Employees WHERE DepartmentID = 1;  -- Exactly same, no new rows
                COMMIT;

                -- SNAPSHOT Isolation (requires database option)
                ALTER DATABASE SQLDemo SET ALLOW_SNAPSHOT_ISOLATION ON;
                
                SET TRANSACTION ISOLATION LEVEL SNAPSHOT;
                BEGIN TRANSACTION;
                    SELECT * FROM Employees WHERE DepartmentID = 1;
                    -- Sees data as it was at start of transaction
                    -- Other transactions can modify without blocking
                    WAITFOR DELAY '00:00:05';
                    SELECT * FROM Employees WHERE DepartmentID = 1;  -- Consistent snapshot
                COMMIT;

                -- READ COMMITTED SNAPSHOT (row versioning for read committed)
                ALTER DATABASE SQLDemo SET READ_COMMITTED_SNAPSHOT ON;
                -- Now READ COMMITTED uses snapshot semantics by default

                -- Demonstration of concurrency issues
                
                -- Session 1: Dirty Read Example
                BEGIN TRANSACTION;
                    UPDATE Employees SET Salary = 999999 WHERE EmployeeID = 1;
                    WAITFOR DELAY '00:00:10';  -- Wait before rollback
                    ROLLBACK;  -- Changes not committed

                -- Session 2 (concurrent): Sees uncommitted data
                SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
                SELECT Salary FROM Employees WHERE EmployeeID = 1;  -- Sees 999999 (dirty read)

                -- Session 1: Non-Repeatable Read Example
                SET TRANSACTION ISOLATION LEVEL READ COMMITTED;
                BEGIN TRANSACTION;
                    SELECT Salary FROM Employees WHERE EmployeeID = 1;  -- Returns 75000
                    WAITFOR DELAY '00:00:05';
                    -- Another transaction updates salary
                    SELECT Salary FROM Employees WHERE EmployeeID = 1;  -- Returns different value
                COMMIT;

                -- Session 1: Phantom Read Example
                SET TRANSACTION ISOLATION LEVEL REPEATABLE READ;
                BEGIN TRANSACTION;
                    SELECT COUNT(*) FROM Employees WHERE DepartmentID = 1;  -- Returns 10
                    WAITFOR DELAY '00:00:05';
                    -- Another transaction inserts new employee
                    SELECT COUNT(*) FROM Employees WHERE DepartmentID = 1;  -- Returns 11 (phantom)
                COMMIT;

                -- Check current isolation level
                SELECT 
                    CASE transaction_isolation_level 
                        WHEN 0 THEN 'Unspecified'
                        WHEN 1 THEN 'ReadUncommitted'
                        WHEN 2 THEN 'ReadCommitted'
                        WHEN 3 THEN 'RepeatableRead'
                        WHEN 4 THEN 'Serializable'
                        WHEN 5 THEN 'Snapshot'
                    END AS IsolationLevel
                FROM sys.dm_exec_sessions
                WHERE session_id = @@SPID;
            ";

            Console.WriteLine(sql);
        }
        #endregion

        #region Locking and Blocking
        static void LockingAndBlocking()
        {
            Console.WriteLine("\n3. Locking and Blocking");
            
            string sql = @"
                /*
                LOCK TYPES:
                - Shared (S): Read locks, multiple transactions can hold
                - Exclusive (X): Write locks, only one transaction can hold
                - Update (U): Intent to update, prevents deadlocks
                - Intent (IS, IX, IU): Intent to lock at lower level
                - Schema (Sch-S, Sch-M): Schema stability or modification
                - Bulk Update (BU): For bulk operations
                - Key-range: For serializable isolation
                
                LOCK GRANULARITY:
                - RID: Row lock
                - KEY: Index key lock
                - PAGE: Page lock (8KB)
                - EXTENT: Extent lock (64KB, 8 pages)
                - TABLE: Entire table lock
                - DATABASE: Database lock
                */

                -- View current locks
                SELECT 
                    request_session_id AS SPID,
                    resource_type AS ResourceType,
                    resource_database_id AS DatabaseID,
                    resource_description AS Resource,
                    resource_associated_entity_id AS EntityID,
                    request_mode AS LockMode,
                    request_status AS Status
                FROM sys.dm_tran_locks
                WHERE request_session_id <> @@SPID
                ORDER BY request_session_id;

                -- Detailed lock information
                SELECT 
                    tl.request_session_id,
                    tl.resource_type,
                    tl.resource_database_id,
                    tl.resource_associated_entity_id,
                    tl.request_mode,
                    tl.request_status,
                    OBJECT_NAME(tl.resource_associated_entity_id) AS ObjectName
                FROM sys.dm_tran_locks tl
                LEFT JOIN sys.objects o ON tl.resource_associated_entity_id = o.object_id
                WHERE tl.resource_type IN ('OBJECT', 'PAGE', 'KEY', 'RID');

                -- Find blocking sessions
                SELECT 
                    blocking.session_id AS BlockingSessionID,
                    blocked.session_id AS BlockedSessionID,
                    blocking_text.text AS BlockingQuery,
                    blocked_text.text AS BlockedQuery,
                    blocked.wait_type,
                    blocked.wait_time,
                    blocked.wait_resource
                FROM sys.dm_exec_requests blocked
                INNER JOIN sys.dm_exec_requests blocking ON blocked.blocking_session_id = blocking.session_id
                CROSS APPLY sys.dm_exec_sql_text(blocking.sql_handle) blocking_text
                CROSS APPLY sys.dm_exec_sql_text(blocked.sql_handle) blocked_text
                WHERE blocked.blocking_session_id > 0;

                -- Lock hints
                
                -- NOLOCK: Read uncommitted (no shared locks)
                SELECT * FROM Employees WITH (NOLOCK) WHERE DepartmentID = 1;

                -- HOLDLOCK: Hold shared lock until end of transaction (equivalent to SERIALIZABLE)
                SELECT * FROM Employees WITH (HOLDLOCK) WHERE DepartmentID = 1;

                -- ROWLOCK: Force row-level locks
                UPDATE Employees WITH (ROWLOCK) SET Salary = Salary * 1.10 WHERE EmployeeID = 1;

                -- PAGLOCK: Force page-level locks
                UPDATE Employees WITH (PAGLOCK) SET Salary = Salary * 1.10 WHERE DepartmentID = 1;

                -- TABLOCK: Force table-level lock
                UPDATE Employees WITH (TABLOCK) SET Salary = Salary * 1.10;

                -- TABLOCKX: Exclusive table lock
                SELECT * FROM Employees WITH (TABLOCKX);

                -- UPDLOCK: Update lock (prevents deadlocks in update scenarios)
                BEGIN TRANSACTION;
                    SELECT * FROM Employees WITH (UPDLOCK) WHERE EmployeeID = 1;
                    UPDATE Employees SET Salary = 80000 WHERE EmployeeID = 1;
                COMMIT;

                -- XLOCK: Exclusive lock
                SELECT * FROM Employees WITH (XLOCK, HOLDLOCK) WHERE EmployeeID = 1;

                -- READPAST: Skip locked rows
                SELECT * FROM Employees WITH (READPAST) WHERE DepartmentID = 1;

                -- READCOMMITTEDLOCK: Force locking even with READ_COMMITTED_SNAPSHOT
                SELECT * FROM Employees WITH (READCOMMITTEDLOCK) WHERE DepartmentID = 1;

                -- Set lock timeout
                SET LOCK_TIMEOUT 5000;  -- 5 seconds
                SELECT * FROM Employees WHERE DepartmentID = 1;
                SET LOCK_TIMEOUT -1;  -- Wait forever (default)

                -- Kill blocking session (use with caution)
                -- KILL 52;  -- Replace 52 with actual SPID

                -- Wait statistics
                SELECT 
                    wait_type,
                    waiting_tasks_count,
                    wait_time_ms,
                    max_wait_time_ms,
                    signal_wait_time_ms
                FROM sys.dm_os_wait_stats
                WHERE wait_type LIKE 'LCK%'
                ORDER BY wait_time_ms DESC;
            ";

            Console.WriteLine(sql);
        }
        #endregion

        #region Deadlock Handling
        static void DeadlockHandling()
        {
            Console.WriteLine("\n4. Deadlock Detection and Resolution");
            
            string sql = @"
                /*
                DEADLOCK: Two or more transactions waiting for each other
                SQL Server detects and kills one transaction (deadlock victim)
                */

                -- Example deadlock scenario:
                
                -- Session 1:
                BEGIN TRANSACTION;
                    UPDATE Employees SET Salary = 80000 WHERE EmployeeID = 1;
                    WAITFOR DELAY '00:00:05';
                    UPDATE Departments SET Budget = 1000000 WHERE DepartmentID = 1;  -- Will deadlock
                COMMIT;

                -- Session 2 (concurrent):
                BEGIN TRANSACTION;
                    UPDATE Departments SET Budget = 1000000 WHERE DepartmentID = 1;
                    WAITFOR DELAY '00:00:05';
                    UPDATE Employees SET Salary = 80000 WHERE EmployeeID = 1;  -- Will deadlock
                COMMIT;

                -- Enable deadlock trace flags
                DBCC TRACEON(1204, -1);  -- Basic deadlock information
                DBCC TRACEON(1222, -1);  -- Detailed deadlock information (XML)

                -- Disable trace flags
                DBCC TRACEOFF(1204, -1);
                DBCC TRACEOFF(1222, -1);

                -- Deadlock prevention strategies:
                
                -- 1. Access objects in same order
                BEGIN TRANSACTION;
                    -- Always access Employees first, then Departments
                    UPDATE Employees SET Salary = 80000 WHERE EmployeeID = 1;
                    UPDATE Departments SET Budget = 1000000 WHERE DepartmentID = 1;
                COMMIT;

                -- 2. Keep transactions short
                BEGIN TRANSACTION;
                    UPDATE Employees SET Salary = 80000 WHERE EmployeeID = 1;
                    -- Don't do other work here
                COMMIT;

                -- 3. Use lower isolation level
                SET TRANSACTION ISOLATION LEVEL READ COMMITTED;

                -- 4. Use SNAPSHOT isolation
                SET TRANSACTION ISOLATION LEVEL SNAPSHOT;

                -- 5. Use UPDLOCK hint
                BEGIN TRANSACTION;
                    SELECT * FROM Employees WITH (UPDLOCK) WHERE EmployeeID = 1;
                    UPDATE Employees SET Salary = 80000 WHERE EmployeeID = 1;
                COMMIT;

                -- 6. Set deadlock priority (lower priority becomes victim)
                SET DEADLOCK_PRIORITY LOW;  -- -5 to 10, default 0
                -- or
                SET DEADLOCK_PRIORITY -5;

                -- Handle deadlock in application
                BEGIN TRY
                    BEGIN TRANSACTION;
                        UPDATE Employees SET Salary = 80000 WHERE EmployeeID = 1;
                        UPDATE Departments SET Budget = 1000000 WHERE DepartmentID = 1;
                    COMMIT TRANSACTION;
                END TRY
                BEGIN CATCH
                    IF ERROR_NUMBER() = 1205  -- Deadlock error number
                    BEGIN
                        ROLLBACK TRANSACTION;
                        -- Retry logic here
                        RAISERROR('Deadlock detected. Transaction rolled back.', 16, 1);
                    END
                    ELSE
                    BEGIN
                        ROLLBACK TRANSACTION;
                        THROW;
                    END
                END CATCH;

                -- View deadlock history (SQL Server 2008+)
                SELECT 
                    xdr.deadlock_xml
                FROM (
                    SELECT CAST(target_data AS XML) AS target_data
                    FROM sys.dm_xe_session_targets st
                    INNER JOIN sys.dm_xe_sessions s ON s.address = st.event_session_address
                    WHERE s.name = 'system_health'
                        AND st.target_name = 'ring_buffer'
                ) AS ring_buffer
                CROSS APPLY target_data.nodes('/RingBufferTarget/event[@name=""xml_deadlock_report""]') AS xdr(deadlock_xml);

                -- Deadlock graph (SQL Server Management Studio)
                -- Tools > SQL Server Profiler > Deadlock Graph event
            ";

            Console.WriteLine(sql);
        }
        #endregion

        #region Error Handling
        static void ErrorHandling()
        {
            Console.WriteLine("\n5. Error Handling - TRY-CATCH");
            
            string sql = @"
                -- Basic TRY-CATCH
                BEGIN TRY
                    BEGIN TRANSACTION;
                        UPDATE Employees SET Salary = 80000 WHERE EmployeeID = 1;
                        -- Intentional error
                        SELECT 1/0;
                    COMMIT TRANSACTION;
                END TRY
                BEGIN CATCH
                    IF @@TRANCOUNT > 0
                        ROLLBACK TRANSACTION;
                    
                    SELECT 
                        ERROR_NUMBER() AS ErrorNumber,
                        ERROR_MESSAGE() AS ErrorMessage,
                        ERROR_SEVERITY() AS ErrorSeverity,
                        ERROR_STATE() AS ErrorState,
                        ERROR_LINE() AS ErrorLine,
                        ERROR_PROCEDURE() AS ErrorProcedure;
                END CATCH;

                -- THROW (SQL Server 2012+) - re-throws error
                BEGIN TRY
                    SELECT 1/0;
                END TRY
                BEGIN CATCH
                    THROW;  -- Re-throws original error
                END CATCH;

                -- THROW with custom error
                BEGIN TRY
                    DECLARE @Salary DECIMAL(18,2) = -1000;
                    IF @Salary < 0
                        THROW 50001, 'Salary cannot be negative', 1;
                END TRY
                BEGIN CATCH
                    SELECT ERROR_MESSAGE();
                END CATCH;

                -- RAISERROR (older method)
                BEGIN TRY
                    RAISERROR('Custom error message', 16, 1);
                END TRY
                BEGIN CATCH
                    SELECT ERROR_MESSAGE();
                END CATCH;

                -- Custom error messages
                -- Add custom error
                EXEC sp_addmessage 
                    @msgnum = 50001,
                    @severity = 16,
                    @msgtext = 'Salary must be between %d and %d';

                -- Use custom error
                BEGIN TRY
                    RAISERROR(50001, 16, 1, 30000, 200000);
                END TRY
                BEGIN CATCH
                    SELECT ERROR_MESSAGE();
                END CATCH;

                -- Nested TRY-CATCH
                BEGIN TRY
                    BEGIN TRANSACTION;
                        UPDATE Employees SET Salary = 80000 WHERE EmployeeID = 1;
                        
                        BEGIN TRY
                            UPDATE Departments SET Budget = 1000000 WHERE DepartmentID = 1;
                        END TRY
                        BEGIN CATCH
                            PRINT 'Inner error: ' + ERROR_MESSAGE();
                            THROW;  -- Re-throw to outer catch
                        END CATCH;
                        
                    COMMIT TRANSACTION;
                END TRY
                BEGIN CATCH
                    IF @@TRANCOUNT > 0
                        ROLLBACK TRANSACTION;
                    PRINT 'Outer error: ' + ERROR_MESSAGE();
                END CATCH;

                -- Error handling with OUTPUT
                DECLARE @ErrorOccurred BIT = 0;
                DECLARE @ErrorMessage NVARCHAR(4000);

                BEGIN TRY
                    BEGIN TRANSACTION;
                        UPDATE Employees SET Salary = 80000 WHERE EmployeeID = 1;
                    COMMIT TRANSACTION;
                END TRY
                BEGIN CATCH
                    SET @ErrorOccurred = 1;
                    SET @ErrorMessage = ERROR_MESSAGE();
                    
                    IF @@TRANCOUNT > 0
                        ROLLBACK TRANSACTION;
                END CATCH;

                IF @ErrorOccurred = 1
                    PRINT 'Error: ' + @ErrorMessage;
                ELSE
                    PRINT 'Success';

                -- Logging errors
                CREATE TABLE ErrorLog (
                    ErrorLogID INT IDENTITY(1,1) PRIMARY KEY,
                    ErrorNumber INT,
                    ErrorMessage NVARCHAR(4000),
                    ErrorSeverity INT,
                    ErrorState INT,
                    ErrorLine INT,
                    ErrorProcedure NVARCHAR(128),
                    ErrorTime DATETIME2 DEFAULT SYSDATETIME()
                );

                BEGIN TRY
                    SELECT 1/0;
                END TRY
                BEGIN CATCH
                    INSERT INTO ErrorLog (ErrorNumber, ErrorMessage, ErrorSeverity, ErrorState, ErrorLine, ErrorProcedure)
                    VALUES (ERROR_NUMBER(), ERROR_MESSAGE(), ERROR_SEVERITY(), ERROR_STATE(), ERROR_LINE(), ERROR_PROCEDURE());
                END CATCH;

                -- XACT_STATE() usage
                BEGIN TRY
                    BEGIN TRANSACTION;
                        UPDATE Employees SET Salary = 80000 WHERE EmployeeID = 1;
                        SELECT 1/0;
                    COMMIT TRANSACTION;
                END TRY
                BEGIN CATCH
                    IF XACT_STATE() = 1  -- Committable
                    BEGIN
                        COMMIT TRANSACTION;
                        PRINT 'Transaction committed';
                    END
                    ELSE IF XACT_STATE() = -1  -- Uncommittable
                    BEGIN
                        ROLLBACK TRANSACTION;
                        PRINT 'Transaction rolled back';
                    END
                    ELSE IF XACT_STATE() = 0  -- No transaction
                    BEGIN
                        PRINT 'No active transaction';
                    END
                END CATCH;
            ";

            Console.WriteLine(sql);
        }
        #endregion

        #region Concurrency Patterns
        static void ConcurrencyPatterns()
        {
            Console.WriteLine("\n6. Concurrency Control Patterns");
            
            string sql = @"
                -- Optimistic Concurrency (ROWVERSION)
                ALTER TABLE Employees ADD RowVersion ROWVERSION;

                -- Update with optimistic concurrency check
                DECLARE @OriginalRowVersion BINARY(8);
                DECLARE @EmployeeID INT = 1;

                -- Read data and row version
                SELECT @OriginalRowVersion = RowVersion
                FROM Employees
                WHERE EmployeeID = @EmployeeID;

                -- Update only if row version hasn't changed
                UPDATE Employees
                SET Salary = 85000
                WHERE EmployeeID = @EmployeeID
                    AND RowVersion = @OriginalRowVersion;

                IF @@ROWCOUNT = 0
                    RAISERROR('Concurrency conflict: Record was modified by another user', 16, 1);

                -- Pessimistic Concurrency (explicit locks)
                BEGIN TRANSACTION;
                    -- Lock row for update
                    SELECT * FROM Employees WITH (UPDLOCK, ROWLOCK)
                    WHERE EmployeeID = 1;
                    
                    -- Perform business logic
                    WAITFOR DELAY '00:00:02';
                    
                    -- Update
                    UPDATE Employees SET Salary = 85000 WHERE EmployeeID = 1;
                COMMIT TRANSACTION;

                -- Queue-based pattern (using table as queue)
                CREATE TABLE WorkQueue (
                    QueueID INT IDENTITY(1,1) PRIMARY KEY,
                    TaskData NVARCHAR(MAX),
                    Status VARCHAR(20) DEFAULT 'Pending',
                    ProcessedBy INT NULL,
                    ProcessedAt DATETIME2 NULL
                );

                -- Worker process picks up next task
                BEGIN TRANSACTION;
                    DECLARE @QueueID INT;
                    
                    -- Get next available task with lock
                    SELECT TOP 1 @QueueID = QueueID
                    FROM WorkQueue WITH (UPDLOCK, READPAST)
                    WHERE Status = 'Pending'
                    ORDER BY QueueID;
                    
                    IF @QueueID IS NOT NULL
                    BEGIN
                        UPDATE WorkQueue
                        SET Status = 'Processing',
                            ProcessedBy = @@SPID,
                            ProcessedAt = SYSDATETIME()
                        WHERE QueueID = @QueueID;
                    END
                COMMIT TRANSACTION;

                -- Retry pattern for deadlocks
                DECLARE @RetryCount INT = 0;
                DECLARE @MaxRetries INT = 3;
                DECLARE @Success BIT = 0;

                WHILE @RetryCount < @MaxRetries AND @Success = 0
                BEGIN
                    BEGIN TRY
                        BEGIN TRANSACTION;
                            UPDATE Employees SET Salary = 85000 WHERE EmployeeID = 1;
                        COMMIT TRANSACTION;
                        SET @Success = 1;
                    END TRY
                    BEGIN CATCH
                        IF ERROR_NUMBER() = 1205  -- Deadlock
                        BEGIN
                            SET @RetryCount = @RetryCount + 1;
                            WAITFOR DELAY '00:00:01';  -- Wait before retry
                            IF @@TRANCOUNT > 0
                                ROLLBACK TRANSACTION;
                        END
                        ELSE
                        BEGIN
                            IF @@TRANCOUNT > 0
                                ROLLBACK TRANSACTION;
                            THROW;
                        END
                    END CATCH;
                END;

                IF @Success = 0
                    RAISERROR('Operation failed after %d retries', 16, 1, @MaxRetries);

                -- Application locks for custom synchronization
                BEGIN TRANSACTION;
                    DECLARE @Result INT;
                    
                    -- Acquire application lock
                    EXEC @Result = sp_getapplock 
                        @Resource = 'EmployeeSalaryUpdate',
                        @LockMode = 'Exclusive',
                        @LockOwner = 'Transaction',
                        @LockTimeout = 5000;
                    
                    IF @Result >= 0  -- Lock acquired
                    BEGIN
                        UPDATE Employees SET Salary = 85000 WHERE EmployeeID = 1;
                        
                        -- Release lock (automatic on commit, but can be explicit)
                        EXEC sp_releaseapplock @Resource = 'EmployeeSalaryUpdate', @LockOwner = 'Transaction';
                    END
                    ELSE
                        RAISERROR('Could not acquire application lock', 16, 1);
                        
                COMMIT TRANSACTION;
            ";

            Console.WriteLine(sql);
        }
        #endregion

        #region Interview Questions
        /*
        COMMON INTERVIEW QUESTIONS:

        1. What are ACID properties?
           - Atomicity: All or nothing
           - Consistency: Valid state
           - Isolation: Concurrent transactions don't interfere
           - Durability: Committed changes persist

        2. Explain isolation levels?
           - READ UNCOMMITTED: Dirty reads allowed
           - READ COMMITTED: No dirty reads (default)
           - REPEATABLE READ: No dirty or non-repeatable reads
           - SERIALIZABLE: No dirty, non-repeatable, or phantom reads
           - SNAPSHOT: Row versioning, no locks for reads

        3. What is a deadlock?
           - Two or more transactions waiting for each other
           - SQL Server detects and kills one (victim)
           - Prevention: access objects in same order, keep transactions short

        4. Dirty read vs non-repeatable read vs phantom read?
           - Dirty: Reading uncommitted data
           - Non-repeatable: Reading different values in same transaction
           - Phantom: New rows appearing in same transaction

        5. When to use SNAPSHOT isolation?
           - High read concurrency needed
           - Readers shouldn't block writers
           - Have tempdb capacity

        6. Pessimistic vs Optimistic concurrency?
           - Pessimistic: Lock resources (UPDLOCK)
           - Optimistic: Check version on update (ROWVERSION)

        7. How to handle deadlocks?
           - Retry transaction
           - Access resources in same order
           - Keep transactions short
           - Use lower isolation level

        8. What is XACT_STATE()?
           - Returns transaction state
           - 1: Committable
           - 0: No transaction
           - -1: Uncommittable (must rollback)

        9. Difference between THROW and RAISERROR?
           - THROW: SQL Server 2012+, simpler syntax, re-throws errors
           - RAISERROR: Older, more formatting options

        10. Best practices for transactions?
            - Keep short
            - Handle errors properly
            - Use appropriate isolation level
            - Avoid user interaction during transaction
            - Consider retry logic for deadlocks
        */
        #endregion
    }
}
