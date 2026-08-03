-- Run against the InvoiceGenerator database.
CREATE OR ALTER PROCEDURE [dbo].[GetProduct]
    @Id INT = NULL,
    @SearchText NVARCHAR(200) = NULL,
    @IsActive BIT = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 20
AS
BEGIN
    SET NOCOUNT ON;

    IF @Id IS NOT NULL
    BEGIN
        SELECT
            p.Id, p.Title, p.ProductStatusId, p.UnitOfMeasureId, p.ServiceCode, p.ServiceDescription,
            p.SellingPrice, p.RevenueAccountID, p.RevenueTaxRateId, p.PurchaseCost, p.ExpenseAccountId,
            p.PurchaseTaxRateId, p.IsActive,
            rtr.RatePercentage AS RevenueTaxRatePercentage,
            p.CreatedDate AS CreatedOn, p.CreatedBy, p.UpdatedDate AS UpdatedOn, p.UpdatedBy, p.UserId,
            CAST(1 AS INT) AS TotalRecords
        FROM [Product] p
        LEFT JOIN RevenueTaxRateType rtr ON rtr.Id = p.RevenueTaxRateId
        WHERE p.Id = @Id;
        RETURN;
    END

    SELECT
        p.Id, p.Title, p.ProductStatusId, p.UnitOfMeasureId, p.ServiceCode, p.ServiceDescription,
        p.SellingPrice, p.RevenueAccountID, p.RevenueTaxRateId, p.PurchaseCost, p.ExpenseAccountId,
        p.PurchaseTaxRateId, p.IsActive,
        rtr.RatePercentage AS RevenueTaxRatePercentage,
        p.CreatedDate AS CreatedOn, p.CreatedBy, p.UpdatedDate AS UpdatedOn, p.UpdatedBy, p.UserId,
        COUNT(*) OVER() AS TotalRecords
    FROM [Product] p
    LEFT JOIN RevenueTaxRateType rtr ON rtr.Id = p.RevenueTaxRateId
    WHERE (@SearchText IS NULL
            OR p.Title LIKE '%' + @SearchText + '%'
            OR p.ServiceCode LIKE '%' + @SearchText + '%')
      AND (@IsActive IS NULL OR p.IsActive = @IsActive)
    ORDER BY p.Id DESC
    OFFSET (ISNULL(@PageNumber, 1) - 1) * ISNULL(@PageSize, 20) ROWS
    FETCH NEXT ISNULL(@PageSize, 20) ROWS ONLY;
END
