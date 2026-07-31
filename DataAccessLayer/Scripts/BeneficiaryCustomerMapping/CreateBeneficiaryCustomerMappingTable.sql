-- Run once against the InvoiceGenerator database.
-- Junction table linking Customers and Beneficiaries (many-to-many).
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'BeneficiaryCustomerMapping')
BEGIN
    CREATE TABLE [dbo].[BeneficiaryCustomerMapping]
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        CustomerId INT NOT NULL,
        BeneficiaryId INT NOT NULL
    );
END
