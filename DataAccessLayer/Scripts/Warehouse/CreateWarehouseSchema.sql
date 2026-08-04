-- Run against the InvoiceGenerator database. Safe to re-run.
-- ----------------------------------------------------------------------------------------------------
-- Warehouse table
-- ----------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Warehouse')
BEGIN
    CREATE TABLE Warehouse
    (
        Id              INT IDENTITY(1,1) PRIMARY KEY,
        Code            NVARCHAR(250)   NULL,
        [Name]          NVARCHAR(250)   NULL,
        Phone           NVARCHAR(250)   NULL,
        StreetAddress   NVARCHAR(250)   NULL,
        BuildingNumber  NVARCHAR(250)   NULL,
        District        NVARCHAR(250)   NULL,
        City            NVARCHAR(250)   NULL,
        PostalCode      NVARCHAR(250)   NULL,
        UserId          INT             NULL,
        IsActive        BIT             DEFAULT(1),
        CreatedDate     DATETIME        DEFAULT(GETUTCDATE()),
        CreatedBy       INT             NULL,
        UpdatedDate     DATETIME        NULL,
        UpdatedBy       INT             NULL
    );
END
GO
