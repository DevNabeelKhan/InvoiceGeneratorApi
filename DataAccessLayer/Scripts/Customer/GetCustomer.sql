-- Run against the InvoiceGenerator database.
-- Returns a single row when @Id is provided (used to populate the Edit modal),
-- otherwise returns a filtered, paged list with a TotalRecords column (COUNT(*) OVER())
-- so the Angular grid can render server-side pagination without a second round-trip.
CREATE OR ALTER PROCEDURE [dbo].[GetCustomer]
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
            Id, CustomerName, CountryId, TaxRegistrationNumber, City, StreetAddress,
            BuildingNumber, District, AddressAdditionalNumber, PostalCode, InvoicingCode,
            InvoicingEmail, InvoicingPhone, InvoicingRelationShipId, PaymentTermId,
            ContactTypeID, ContactTypeNumber, SellingRevenueAccountId, SellingRevenueCostCenterId,
            SellingRevenueTaxRateId, IsActive,
            CreatedDate AS CreatedOn, CreatedBy, UpdatedDate AS UpdatedOn, UpdatedBy, UserId,
            CAST(1 AS INT) AS TotalRecords
        FROM [Customer]
        WHERE Id = @Id;
        RETURN;
    END

    SELECT
        Id, CustomerName, CountryId, TaxRegistrationNumber, City, StreetAddress,
        BuildingNumber, District, AddressAdditionalNumber, PostalCode, InvoicingCode,
        InvoicingEmail, InvoicingPhone, InvoicingRelationShipId, PaymentTermId,
        ContactTypeID, ContactTypeNumber, SellingRevenueAccountId, SellingRevenueCostCenterId,
        SellingRevenueTaxRateId, IsActive,
        CreatedDate AS CreatedOn, CreatedBy, UpdatedDate AS UpdatedOn, UpdatedBy, UserId,
        COUNT(*) OVER() AS TotalRecords
    FROM [Customer]
    WHERE (@SearchText IS NULL
            OR CustomerName LIKE '%' + @SearchText + '%'
            OR InvoicingEmail LIKE '%' + @SearchText + '%'
            OR TaxRegistrationNumber LIKE '%' + @SearchText + '%')
      AND (@IsActive IS NULL OR IsActive = @IsActive)
    ORDER BY Id DESC
    OFFSET (ISNULL(@PageNumber, 1) - 1) * ISNULL(@PageSize, 20) ROWS
    FETCH NEXT ISNULL(@PageSize, 20) ROWS ONLY;
END
