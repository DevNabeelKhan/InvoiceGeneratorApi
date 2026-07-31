-- Run against the InvoiceGenerator database.
-- Soft-delete: marks the record inactive instead of physically deleting it.
CREATE OR ALTER PROCEDURE [dbo].[DeleteBeneficiary]
    @Id INT,
    @UserId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [Beneficiary]
    SET IsActive = 0,
        UpdatedDate = GETDATE(),
        UpdatedBy = @UserId
    WHERE Id = @Id;

    SELECT @Id AS Id;
END
