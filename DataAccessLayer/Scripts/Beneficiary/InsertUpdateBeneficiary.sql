-- Run against the InvoiceGenerator database.
CREATE OR ALTER PROCEDURE [dbo].[InsertUpdateBeneficiary]
    @Id INT = NULL,
    @IBAN NVARCHAR(50) = NULL,
    @CurrencyId INT = NULL,
    @BeneficiaryName NVARCHAR(255) = NULL,
    @BeneficiaryAddress NVARCHAR(255) = NULL,
    @BankName NVARCHAR(150) = NULL,
    @Swift NVARCHAR(50) = NULL,
    @CountryId INT = NULL,
    @BankFeesTypeId INT = NULL,
    @IsActive BIT = 1,
    @UserId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Id IS NULL OR @Id = 0
    BEGIN
        INSERT INTO [Beneficiary]
        (IBAN, CurrencyId, BeneficiaryName, BeneficiaryAddress, BankName, Swift, CountryId,
         BankFeesTypeId, IsActive, CreatedDate, CreatedBy, UserId)
        VALUES
        (@IBAN, @CurrencyId, @BeneficiaryName, @BeneficiaryAddress, @BankName, @Swift, @CountryId,
         @BankFeesTypeId, ISNULL(@IsActive, 1), GETDATE(), @UserId, @UserId);

        SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
    END
    ELSE
    BEGIN
        UPDATE [Beneficiary] SET
            IBAN = @IBAN,
            CurrencyId = @CurrencyId,
            BeneficiaryName = @BeneficiaryName,
            BeneficiaryAddress = @BeneficiaryAddress,
            BankName = @BankName,
            Swift = @Swift,
            CountryId = @CountryId,
            BankFeesTypeId = @BankFeesTypeId,
            IsActive = ISNULL(@IsActive, IsActive),
            UpdatedDate = GETDATE(),
            UpdatedBy = @UserId
        WHERE Id = @Id;

        SELECT @Id AS Id;
    END
END
