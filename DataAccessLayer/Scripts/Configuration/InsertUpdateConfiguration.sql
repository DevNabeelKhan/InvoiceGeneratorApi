-- Run against the InvoiceGenerator database.
-- Generic insert/update procedure for all Configuration lookup tables.
CREATE OR ALTER PROCEDURE [dbo].[InsertUpdateConfiguration]
    @TableName NVARCHAR(100),
    @Id INT = NULL,
    @Title NVARCHAR(255),
    @IsActive BIT = 1,
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
    DECLARE @OutId INT;

    IF @Id IS NULL OR @Id = 0
    BEGIN
        SET @sql = N'
            INSERT INTO ' + QUOTENAME(@TableName) + N' (Title, IsActive, CreatedDate, CreatedBy)
            VALUES (@Title, @IsActive, GETDATE(), @UserId);
            SET @OutId = SCOPE_IDENTITY();';

        EXEC sp_executesql @sql,
            N'@Title NVARCHAR(255), @IsActive BIT, @UserId INT, @OutId INT OUTPUT',
            @Title = @Title, @IsActive = @IsActive, @UserId = @UserId, @OutId = @OutId OUTPUT;

        SELECT @OutId AS Id;
    END
    ELSE
    BEGIN
        SET @sql = N'
            UPDATE ' + QUOTENAME(@TableName) + N'
            SET Title = @Title,
                IsActive = ISNULL(@IsActive, IsActive),
                UpdatedDate = GETDATE(),
                UpdatedBy = @UserId
            WHERE Id = @Id;';

        EXEC sp_executesql @sql,
            N'@Title NVARCHAR(255), @IsActive BIT, @UserId INT, @Id INT',
            @Title = @Title, @IsActive = @IsActive, @UserId = @UserId, @Id = @Id;

        SELECT @Id AS Id;
    END
END
