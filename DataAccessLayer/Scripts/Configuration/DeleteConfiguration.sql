-- Run against the InvoiceGenerator database.
-- Generic soft-delete procedure for all Configuration lookup tables.
CREATE OR ALTER PROCEDURE [dbo].[DeleteConfiguration]
    @TableName NVARCHAR(100),
    @Id INT,
    @UserId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @TableName NOT IN (N'AccountType', N'BankFeesType', N'CashFlowType', N'ContactType', N'CostCenter',
        N'Industry', N'InvoicingRelationShip', N'PaymentTerm', N'RevenueTaxRateType', N'RevenueRecognitionType', N'Role',
        N'UnitOfMeasure', N'ProductStatus')
    BEGIN
        RAISERROR('Invalid table name.', 16, 1);
        RETURN;
    END

    DECLARE @sql NVARCHAR(MAX);
    SET @sql = N'
        UPDATE ' + QUOTENAME(@TableName) + N'
        SET IsActive = 0,
            UpdatedDate = GETDATE(),
            UpdatedBy = @UserId
        WHERE Id = @Id;';

    EXEC sp_executesql @sql, N'@Id INT, @UserId INT', @Id = @Id, @UserId = @UserId;

    SELECT @Id AS Id;
END
