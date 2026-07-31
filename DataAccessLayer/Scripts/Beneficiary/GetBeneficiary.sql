-- Run against the InvoiceGenerator database.
CREATE OR ALTER PROCEDURE [dbo].[GetBeneficiary]
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
            Id, IBAN, CurrencyId, BeneficiaryName, BeneficiaryAddress, BankName, Swift,
            CountryId, BankFeesTypeId, IsActive,
            CreatedDate AS CreatedOn, CreatedBy, UpdatedDate AS UpdatedOn, UpdatedBy, UserId,
            CAST(1 AS INT) AS TotalRecords
        FROM [Beneficiary]
        WHERE Id = @Id;
        RETURN;
    END

    SELECT
        Id, IBAN, CurrencyId, BeneficiaryName, BeneficiaryAddress, BankName, Swift,
        CountryId, BankFeesTypeId, IsActive,
        CreatedDate AS CreatedOn, CreatedBy, UpdatedDate AS UpdatedOn, UpdatedBy, UserId,
        COUNT(*) OVER() AS TotalRecords
    FROM [Beneficiary]
    WHERE (@SearchText IS NULL
            OR BeneficiaryName LIKE '%' + @SearchText + '%'
            OR IBAN LIKE '%' + @SearchText + '%'
            OR BankName LIKE '%' + @SearchText + '%')
      AND (@IsActive IS NULL OR IsActive = @IsActive)
    ORDER BY Id DESC
    OFFSET (ISNULL(@PageNumber, 1) - 1) * ISNULL(@PageSize, 20) ROWS
    FETCH NEXT ISNULL(@PageSize, 20) ROWS ONLY;
END
