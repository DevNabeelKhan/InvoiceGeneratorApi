-- Run against the InvoiceGenerator database.
-- Same procedure handles both Insert (Id NULL/0) and Update (Id provided).
CREATE OR ALTER PROCEDURE [dbo].[InsertUpdateCustomer]
    @Id INT = NULL,
    @CustomerName NVARCHAR(255) = NULL,
    @CountryId INT = NULL,
    @TaxRegistrationNumber NVARCHAR(100) = NULL,
    @City NVARCHAR(150) = NULL,
    @StreetAddress NVARCHAR(255) = NULL,
    @BuildingNumber NVARCHAR(50) = NULL,
    @District NVARCHAR(150) = NULL,
    @AddressAdditionalNumber NVARCHAR(50) = NULL,
    @PostalCode NVARCHAR(50) = NULL,
    @InvoicingCode NVARCHAR(100) = NULL,
    @InvoicingEmail NVARCHAR(255) = NULL,
    @InvoicingPhone NVARCHAR(50) = NULL,
    @InvoicingRelationShipId INT = NULL,
    @PaymentTermId INT = NULL,
    @ContactTypeID INT = NULL,
    @ContactTypeNumber NVARCHAR(100) = NULL,
    @SellingRevenueAccountId INT = NULL,
    @SellingRevenueCostCenterId INT = NULL,
    @SellingRevenueTaxRateId INT = NULL,
    @ArabicName NVARCHAR(255) = NULL,
    @ArabicAddress NVARCHAR(MAX) = NULL,
    @Email NVARCHAR(255) = NULL,
    @Phone NVARCHAR(50) = NULL,
    @IsActive BIT = 1,
    @UserId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Id IS NULL OR @Id = 0
    BEGIN
        INSERT INTO [Customer]
        (CustomerName, CountryId, TaxRegistrationNumber, City, StreetAddress, BuildingNumber,
         District, AddressAdditionalNumber, PostalCode, InvoicingCode, InvoicingEmail, InvoicingPhone,
         InvoicingRelationShipId, PaymentTermId, ContactTypeID, ContactTypeNumber,
         SellingRevenueAccountId, SellingRevenueCostCenterId, SellingRevenueTaxRateId,
         ArabicName, ArabicAddress, Email, Phone,
         IsActive, CreatedDate, CreatedBy, UserId)
        VALUES
        (@CustomerName, @CountryId, @TaxRegistrationNumber, @City, @StreetAddress, @BuildingNumber,
         @District, @AddressAdditionalNumber, @PostalCode, @InvoicingCode, @InvoicingEmail, @InvoicingPhone,
         @InvoicingRelationShipId, @PaymentTermId, @ContactTypeID, @ContactTypeNumber,
         @SellingRevenueAccountId, @SellingRevenueCostCenterId, @SellingRevenueTaxRateId,
         @ArabicName, @ArabicAddress, @Email, @Phone,
         ISNULL(@IsActive, 1), GETDATE(), @UserId, @UserId);

        SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
    END
    ELSE
    BEGIN
        UPDATE [Customer] SET
            CustomerName = @CustomerName,
            CountryId = @CountryId,
            TaxRegistrationNumber = @TaxRegistrationNumber,
            City = @City,
            StreetAddress = @StreetAddress,
            BuildingNumber = @BuildingNumber,
            District = @District,
            AddressAdditionalNumber = @AddressAdditionalNumber,
            PostalCode = @PostalCode,
            InvoicingCode = @InvoicingCode,
            InvoicingEmail = @InvoicingEmail,
            InvoicingPhone = @InvoicingPhone,
            InvoicingRelationShipId = @InvoicingRelationShipId,
            PaymentTermId = @PaymentTermId,
            ContactTypeID = @ContactTypeID,
            ContactTypeNumber = @ContactTypeNumber,
            SellingRevenueAccountId = @SellingRevenueAccountId,
            SellingRevenueCostCenterId = @SellingRevenueCostCenterId,
            SellingRevenueTaxRateId = @SellingRevenueTaxRateId,
            ArabicName = @ArabicName,
            ArabicAddress = @ArabicAddress,
            Email = @Email,
            Phone = @Phone,
            IsActive = ISNULL(@IsActive, IsActive),
            UpdatedDate = GETDATE(),
            UpdatedBy = @UserId
        WHERE Id = @Id;

        SELECT @Id AS Id;
    END
END
