-- Replaces all Customer mappings for the given Beneficiary with the supplied
-- comma-separated list of Customer Ids (NULL/empty clears all mappings).
CREATE OR ALTER PROCEDURE [dbo].[SaveBeneficiaryCustomers]
    @BeneficiaryId INT,
    @CustomerIds NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM [BeneficiaryCustomerMapping] WHERE BeneficiaryId = @BeneficiaryId;

    IF @CustomerIds IS NOT NULL AND LEN(@CustomerIds) > 0
    BEGIN
        INSERT INTO [BeneficiaryCustomerMapping] (CustomerId, BeneficiaryId)
        SELECT CAST(value AS INT), @BeneficiaryId
        FROM STRING_SPLIT(@CustomerIds, ',')
        WHERE LEN(TRIM(value)) > 0;
    END
END
