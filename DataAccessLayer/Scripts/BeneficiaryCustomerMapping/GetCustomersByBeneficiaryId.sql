-- Returns the active Customers mapped to a given Beneficiary.
CREATE OR ALTER PROCEDURE [dbo].[GetCustomersByBeneficiaryId]
    @BeneficiaryId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT c.Id, c.CustomerName, c.InvoicingEmail, c.InvoicingPhone, c.IsActive
    FROM [Customer] c
    INNER JOIN [BeneficiaryCustomerMapping] m ON m.CustomerId = c.Id
    WHERE m.BeneficiaryId = @BeneficiaryId AND c.IsActive = 1
    ORDER BY c.CustomerName;
END
