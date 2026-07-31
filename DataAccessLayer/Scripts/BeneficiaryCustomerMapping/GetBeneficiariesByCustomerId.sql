-- Returns the active Beneficiaries mapped to a given Customer.
CREATE OR ALTER PROCEDURE [dbo].[GetBeneficiariesByCustomerId]
    @CustomerId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT b.Id, b.BeneficiaryName, b.IBAN, b.BankName, b.Swift, b.IsActive
    FROM [Beneficiary] b
    INNER JOIN [BeneficiaryCustomerMapping] m ON m.BeneficiaryId = b.Id
    WHERE m.CustomerId = @CustomerId AND b.IsActive = 1
    ORDER BY b.BeneficiaryName;
END
