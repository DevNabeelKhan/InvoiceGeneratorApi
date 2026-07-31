-- Replaces all Beneficiary mappings for the given Customer with the supplied
-- comma-separated list of Beneficiary Ids (NULL/empty clears all mappings).
CREATE OR ALTER PROCEDURE [dbo].[SaveCustomerBeneficiaries]
    @CustomerId INT,
    @BeneficiaryIds NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM [BeneficiaryCustomerMapping] WHERE CustomerId = @CustomerId;

    IF @BeneficiaryIds IS NOT NULL AND LEN(@BeneficiaryIds) > 0
    BEGIN
        INSERT INTO [BeneficiaryCustomerMapping] (CustomerId, BeneficiaryId)
        SELECT @CustomerId, CAST(value AS INT)
        FROM STRING_SPLIT(@BeneficiaryIds, ',')
        WHERE LEN(TRIM(value)) > 0;
    END
END
