-- Run against the InvoiceGenerator database.
-- Creates the 12 lookup/configuration tables that share the same schema:
-- Id, Title, IsActive, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy.
-- Safe to re-run: each table is only created if it does not already exist.

DECLARE @tables TABLE (Name NVARCHAR(100));
INSERT INTO @tables (Name) VALUES
    ('AccountType'),
    ('BankFeesType'),
    ('CashFlowType'),
    ('ContactType'),
    ('CostCenter'),
    ('Industry'),
    ('InvoicingRelationShip'),
    ('PaymentTerm'),
    ('RevenueTaxRateType'),
    ('RevenueRecognitionType'),
    ('Role'),
    ('UnitOfMeasure'),
    ('ProductStatus');

DECLARE @tableName NVARCHAR(100);
DECLARE @sql NVARCHAR(MAX);

DECLARE tbl_cursor CURSOR FOR SELECT Name FROM @tables;
OPEN tbl_cursor;
FETCH NEXT FROM tbl_cursor INTO @tableName;

WHILE @@FETCH_STATUS = 0
BEGIN
    IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = @tableName)
    BEGIN
        SET @sql = N'
            CREATE TABLE ' + QUOTENAME(@tableName) + N' (
                Id INT IDENTITY(1,1) PRIMARY KEY,
                Title NVARCHAR(255) NOT NULL,
                IsActive BIT NOT NULL DEFAULT 1,
                CreatedDate DATETIME NULL,
                CreatedBy INT NULL,
                UpdatedDate DATETIME NULL,
                UpdatedBy INT NULL
            );';
        EXEC sp_executesql @sql;
    END

    FETCH NEXT FROM tbl_cursor INTO @tableName;
END

CLOSE tbl_cursor;
DEALLOCATE tbl_cursor;
