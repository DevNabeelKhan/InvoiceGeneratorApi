-- Run against the InvoiceGenerator database.
CREATE OR ALTER PROCEDURE [dbo].[InsertUpdateProduct]
    @Id INT = NULL,
    @Title NVARCHAR(255) = NULL,
    @ProductStatusId INT = NULL,
    @UnitOfMeasureId INT = NULL,
    @ServiceCode NVARCHAR(100) = NULL,
    @ServiceDescription NVARCHAR(MAX) = NULL,
    @SellingPrice DECIMAL(18,2) = NULL,
    @RevenueAccountID INT = NULL,
    @RevenueTaxRateId INT = NULL,
    @PurchaseCost DECIMAL(18,2) = NULL,
    @ExpenseAccountId INT = NULL,
    @PurchaseTaxRateId INT = NULL,
    @IsActive BIT = 1,
    @UserId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Id IS NULL OR @Id = 0
    BEGIN
        INSERT INTO [Product]
        (Title, ProductStatusId, UnitOfMeasureId, ServiceCode, ServiceDescription, SellingPrice,
         RevenueAccountID, RevenueTaxRateId, PurchaseCost, ExpenseAccountId, PurchaseTaxRateId,
         IsActive, CreatedDate, CreatedBy, UserId)
        VALUES
        (@Title, @ProductStatusId, @UnitOfMeasureId, @ServiceCode, @ServiceDescription, @SellingPrice,
         @RevenueAccountID, @RevenueTaxRateId, @PurchaseCost, @ExpenseAccountId, @PurchaseTaxRateId,
         ISNULL(@IsActive, 1), GETDATE(), @UserId, @UserId);

        SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
    END
    ELSE
    BEGIN
        UPDATE [Product] SET
            Title = @Title,
            ProductStatusId = @ProductStatusId,
            UnitOfMeasureId = @UnitOfMeasureId,
            ServiceCode = @ServiceCode,
            ServiceDescription = @ServiceDescription,
            SellingPrice = @SellingPrice,
            RevenueAccountID = @RevenueAccountID,
            RevenueTaxRateId = @RevenueTaxRateId,
            PurchaseCost = @PurchaseCost,
            ExpenseAccountId = @ExpenseAccountId,
            PurchaseTaxRateId = @PurchaseTaxRateId,
            IsActive = ISNULL(@IsActive, IsActive),
            UpdatedDate = GETDATE(),
            UpdatedBy = @UserId
        WHERE Id = @Id;

        SELECT @Id AS Id;
    END
END
