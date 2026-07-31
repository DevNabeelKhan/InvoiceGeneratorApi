-- Run against the InvoiceGenerator database.
-- Generic list/get-by-id procedure for all Configuration lookup tables.
CREATE OR ALTER PROCEDURE [dbo].[GetConfigurationList]
    @TableName NVARCHAR(100),
    @Id INT = NULL,
    @SearchText NVARCHAR(200) = NULL,
    @IsActive BIT = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 20
AS
BEGIN
    SET NOCOUNT ON;

    IF @TableName NOT IN (N'AccountType', N'BankFeesType', N'CashFlowType', N'ContactType', N'CostCenter',
        N'Industry', N'InvoicingRelationShip', N'PaymentTerm', N'RevenueTaxRateType', N'Role',
        N'UnitOfMeasure', N'ProductStatus')
    BEGIN
        RAISERROR('Invalid table name.', 16, 1);
        RETURN;
    END

    DECLARE @sql NVARCHAR(MAX);

    IF @Id IS NOT NULL
    BEGIN
        SET @sql = N'
            SELECT Id, Title, IsActive,
                   CreatedDate AS CreatedOn, CreatedBy, UpdatedDate AS UpdatedOn, UpdatedBy,
                   CAST(1 AS INT) AS TotalRecords
            FROM ' + QUOTENAME(@TableName) + N'
            WHERE Id = @Id;';

        EXEC sp_executesql @sql, N'@Id INT', @Id = @Id;
        RETURN;
    END

    SET @sql = N'
        SELECT Id, Title, IsActive,
               CreatedDate AS CreatedOn, CreatedBy, UpdatedDate AS UpdatedOn, UpdatedBy,
               COUNT(*) OVER() AS TotalRecords
        FROM ' + QUOTENAME(@TableName) + N'
        WHERE (@SearchText IS NULL OR Title LIKE ''%'' + @SearchText + ''%'')
          AND (@IsActive IS NULL OR IsActive = @IsActive)
        ORDER BY Id DESC
        OFFSET (ISNULL(@PageNumber, 1) - 1) * ISNULL(@PageSize, 20) ROWS
        FETCH NEXT ISNULL(@PageSize, 20) ROWS ONLY;';

    EXEC sp_executesql @sql,
        N'@SearchText NVARCHAR(200), @IsActive BIT, @PageNumber INT, @PageSize INT',
        @SearchText = @SearchText, @IsActive = @IsActive, @PageNumber = @PageNumber, @PageSize = @PageSize;
END
