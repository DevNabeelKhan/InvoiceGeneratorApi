-- ====================================================================================================
-- Invoice Module Stored Procedures
-- Run against the InvoiceGenerator database.
-- ====================================================================================================

SET NOCOUNT ON;
GO

-- ----------------------------------------------------------------------------------------------------
-- Insert / Update Company
-- ----------------------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[InsertUpdateCompany]
    @Id                  INT = NULL,
    @Name                NVARCHAR(255) = NULL,
    @ArabicName          NVARCHAR(255) = NULL,
    @Address             NVARCHAR(MAX) = NULL,
    @ArabicAddress       NVARCHAR(MAX) = NULL,
    @Email               NVARCHAR(255) = NULL,
    @Phone               NVARCHAR(50)  = NULL,
    @Website             NVARCHAR(255) = NULL,
    @VATNumber           NVARCHAR(100) = NULL,
    @LogoPath            NVARCHAR(500) = NULL,
    @StampPath           NVARCHAR(500) = NULL,
    @BankName            NVARCHAR(255) = NULL,
    @BankAccountNumber   NVARCHAR(100) = NULL,
    @IBAN                NVARCHAR(100) = NULL,
    @SwiftCode           NVARCHAR(50)  = NULL,
    @AccountCurrency     NVARCHAR(50)  = NULL,
    @BeneficiaryName     NVARCHAR(255) = NULL,
    @Country             NVARCHAR(100) = NULL,
    @City                NVARCHAR(100) = NULL,
    @IsActive            BIT = 1,
    @UserId              INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Id IS NULL OR @Id = 0
    BEGIN
        INSERT INTO Company
        (Name, ArabicName, [Address], ArabicAddress, Email, Phone, Website, VATNumber,
         LogoPath, StampPath, BankName, BankAccountNumber, IBAN, SwiftCode, AccountCurrency,
         BeneficiaryName, Country, City, IsActive, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
        VALUES
        (@Name, @ArabicName, @Address, @ArabicAddress, @Email, @Phone, @Website, @VATNumber,
         @LogoPath, @StampPath, @BankName, @BankAccountNumber, @IBAN, @SwiftCode, @AccountCurrency,
         @BeneficiaryName, @Country, @City, ISNULL(@IsActive, 1), GETDATE(), @UserId, GETDATE(), @UserId);

        SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
    END
    ELSE
    BEGIN
        UPDATE Company SET
            Name                = @Name,
            ArabicName          = @ArabicName,
            [Address]           = @Address,
            ArabicAddress       = @ArabicAddress,
            Email               = @Email,
            Phone               = @Phone,
            Website             = @Website,
            VATNumber           = @VATNumber,
            LogoPath            = @LogoPath,
            StampPath           = @StampPath,
            BankName            = @BankName,
            BankAccountNumber   = @BankAccountNumber,
            IBAN                = @IBAN,
            SwiftCode           = @SwiftCode,
            AccountCurrency     = @AccountCurrency,
            BeneficiaryName     = @BeneficiaryName,
            Country             = @Country,
            City                = @City,
            IsActive            = ISNULL(@IsActive, IsActive),
            UpdatedDate         = GETDATE(),
            UpdatedBy           = @UserId
        WHERE Id = @Id;

        SELECT @Id AS Id;
    END
END
GO

-- ----------------------------------------------------------------------------------------------------
-- Get Company
-- ----------------------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[GetCompany]
    @Id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Id IS NOT NULL
    BEGIN
        SELECT *, CAST(1 AS INT) AS TotalRecords FROM Company WHERE Id = @Id;
        RETURN;
    END

    SELECT *, COUNT(*) OVER() AS TotalRecords FROM Company WHERE IsActive = 1;
END
GO

-- ----------------------------------------------------------------------------------------------------
-- Insert / Update Currency
-- ----------------------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[InsertUpdateCurrency]
    @Id             INT = NULL,
    @Code           NVARCHAR(10) = NULL,
    @Name           NVARCHAR(100) = NULL,
    @Symbol         NVARCHAR(10) = NULL,
    @ExchangeRate   DECIMAL(18,6) = 1,
    @IsActive       BIT = 1,
    @UserId         INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Id IS NULL OR @Id = 0
    BEGIN
        INSERT INTO Currency (Code, Name, Symbol, ExchangeRate, IsActive, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
        VALUES (@Code, @Name, @Symbol, @ExchangeRate, ISNULL(@IsActive,1), GETDATE(), @UserId, GETDATE(), @UserId);

        SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
    END
    ELSE
    BEGIN
        UPDATE Currency SET
            Code         = @Code,
            Name         = @Name,
            Symbol       = @Symbol,
            ExchangeRate = @ExchangeRate,
            IsActive     = ISNULL(@IsActive, IsActive),
            UpdatedDate  = GETDATE(),
            UpdatedBy    = @UserId
        WHERE Id = @Id;

        SELECT @Id AS Id;
    END
END
GO

-- ----------------------------------------------------------------------------------------------------
-- Get Currency
-- ----------------------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[GetCurrency]
    @Id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Id IS NOT NULL
    BEGIN
        SELECT *, CAST(1 AS INT) AS TotalRecords FROM Currency WHERE Id = @Id;
        RETURN;
    END

    SELECT *, COUNT(*) OVER() AS TotalRecords FROM Currency WHERE IsActive = 1;
END
GO

-- ----------------------------------------------------------------------------------------------------
-- Get next invoice number for a given year
-- ----------------------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[GetNextInvoiceNumber]
    @Year     INT = NULL,
    @Prefix   NVARCHAR(10) = 'INV-'
AS
BEGIN
    SET NOCOUNT ON;

    IF @Year IS NULL SET @Year = YEAR(GETDATE());

    DECLARE @NextNumber INT;

    IF NOT EXISTS (SELECT 1 FROM InvoiceNumberSequence WHERE [Year] = @Year)
    BEGIN
        INSERT INTO InvoiceNumberSequence ([Year], LastNumber) VALUES (@Year, 0);
    END

    UPDATE InvoiceNumberSequence
    SET @NextNumber = LastNumber + 1, LastNumber = LastNumber + 1
    WHERE [Year] = @Year;

    SELECT @Prefix + RIGHT('0000' + CAST(@NextNumber AS NVARCHAR(10)), 4) + '/' + CAST(@Year AS NVARCHAR(4)) AS InvoiceNumber,
           @NextNumber AS Number;
END
GO

-- ----------------------------------------------------------------------------------------------------
-- Insert / Update Invoice (with full header and line-item TVP support simulated via JSON)
-- ----------------------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[InsertUpdateInvoice]
    @Id                      INT = NULL,
    @InvoiceNumber           NVARCHAR(50) = NULL,
    @UUID                    NVARCHAR(100) = NULL,
    @Reference               NVARCHAR(255) = NULL,
    @PurchaseOrderNumber     NVARCHAR(255) = NULL,
    @ProjectName             NVARCHAR(500) = NULL,
    @CompanyId               INT = NULL,
    @CustomerId              INT = NULL,
    @CurrencyId              INT = NULL,
    @ExchangeRate            DECIMAL(18,6) = 1,
    @InvoiceDate             DATE = NULL,
    @DueDate                 DATE = NULL,
    @Notes                   NVARCHAR(MAX) = NULL,
    @Status                  NVARCHAR(50) = NULL,
    @PaymentStatus           NVARCHAR(50) = NULL,
    @Draft                   BIT = NULL,
    @Approved                BIT = NULL,
    @Cancelled               BIT = NULL,
    @Sent                    BIT = NULL,
    @Subtotal                DECIMAL(18,2) = NULL,
    @DiscountPercentage      DECIMAL(18,2) = NULL,
    @DiscountAmount          DECIMAL(18,2) = NULL,
    @TaxAmount               DECIMAL(18,2) = NULL,
    @GrandTotal              DECIMAL(18,2) = NULL,
    @RetentionPercentage     DECIMAL(18,2) = NULL,
    @RetentionAmount         DECIMAL(18,2) = NULL,
    @RoundOffAmount          DECIMAL(18,2) = NULL,
    @GeneratedQRCode         NVARCHAR(MAX) = NULL,
    @QRCodeImagePath         NVARCHAR(500) = NULL,
    @PreviousInvoiceHash     NVARCHAR(500) = NULL,
    @XMLPath                 NVARCHAR(500) = NULL,
    @PDFPath                 NVARCHAR(500) = NULL,
    @CreatedIP               NVARCHAR(100) = NULL,
    @UserId                  INT = NULL,
    @IsActive                BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    IF @Status IS NULL SET @Status = 'Draft';

    IF @Id IS NULL OR @Id = 0
    BEGIN
        IF @InvoiceNumber IS NULL
        BEGIN
            DECLARE @NewNumber NVARCHAR(50);
            EXEC @NewNumber = dbo.GetNextInvoiceNumber @Year = NULL, @Prefix = 'INV-';
            SET @InvoiceNumber = @NewNumber;
        END

        IF @UUID IS NULL SET @UUID = CONVERT(NVARCHAR(100), NEWID());

        INSERT INTO Invoice
        (InvoiceNumber, [UUID], Reference, PurchaseOrderNumber, ProjectName, CompanyId, CustomerId,
         CurrencyId, ExchangeRate, InvoiceDate, DueDate, Notes, [Status], PaymentStatus,
         Draft, Approved, Cancelled, Sent, Subtotal, DiscountPercentage, DiscountAmount,
         TaxAmount, GrandTotal, RetentionPercentage, RetentionAmount, RoundOffAmount,
         GeneratedQRCode, QRCodeImagePath, PreviousInvoiceHash, XMLPath, PDFPath, CreatedIP,
         IsActive, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, UserId)
        VALUES
        (@InvoiceNumber, @UUID, @Reference, @PurchaseOrderNumber, @ProjectName, @CompanyId, @CustomerId,
         @CurrencyId, @ExchangeRate, @InvoiceDate, @DueDate, @Notes, @Status, @PaymentStatus,
         ISNULL(@Draft, 1), ISNULL(@Approved, 0), ISNULL(@Cancelled, 0), ISNULL(@Sent, 0),
         @Subtotal, @DiscountPercentage, @DiscountAmount, @TaxAmount, @GrandTotal,
         @RetentionPercentage, @RetentionAmount, @RoundOffAmount, @GeneratedQRCode, @QRCodeImagePath,
         @PreviousInvoiceHash, @XMLPath, @PDFPath, @CreatedIP, ISNULL(@IsActive,1), GETDATE(), @UserId,
         GETDATE(), @UserId, @UserId);

        SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id, @InvoiceNumber AS InvoiceNumber, @UUID AS [UUID];
    END
    ELSE
    BEGIN
        UPDATE Invoice SET
            InvoiceNumber         = @InvoiceNumber,
            Reference             = @Reference,
            PurchaseOrderNumber   = @PurchaseOrderNumber,
            ProjectName           = @ProjectName,
            CompanyId             = @CompanyId,
            CustomerId            = @CustomerId,
            CurrencyId            = @CurrencyId,
            ExchangeRate          = @ExchangeRate,
            InvoiceDate           = @InvoiceDate,
            DueDate               = @DueDate,
            Notes                 = @Notes,
            [Status]              = @Status,
            PaymentStatus         = @PaymentStatus,
            Draft                 = @Draft,
            Approved              = @Approved,
            Cancelled             = @Cancelled,
            Sent                  = @Sent,
            Subtotal              = @Subtotal,
            DiscountPercentage    = @DiscountPercentage,
            DiscountAmount        = @DiscountAmount,
            TaxAmount             = @TaxAmount,
            GrandTotal            = @GrandTotal,
            RetentionPercentage   = @RetentionPercentage,
            RetentionAmount       = @RetentionAmount,
            RoundOffAmount        = @RoundOffAmount,
            GeneratedQRCode       = @GeneratedQRCode,
            QRCodeImagePath       = @QRCodeImagePath,
            PreviousInvoiceHash   = @PreviousInvoiceHash,
            XMLPath               = @XMLPath,
            PDFPath               = @PDFPath,
            CreatedIP             = @CreatedIP,
            IsActive              = ISNULL(@IsActive, IsActive),
            UpdatedDate           = GETDATE(),
            UpdatedBy             = @UserId
        WHERE Id = @Id;

        SELECT @Id AS Id, @InvoiceNumber AS InvoiceNumber, @UUID AS [UUID];
    END
END
GO

-- ----------------------------------------------------------------------------------------------------
-- Get Invoice
-- ----------------------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[GetInvoice]
    @Id          INT = NULL,
    @SearchText  NVARCHAR(200) = NULL,
    @CustomerId  INT = NULL,
    @Status      NVARCHAR(50) = NULL,
    @IsActive    BIT = 1,
    @PageNumber  INT = 1,
    @PageSize    INT = 20
AS
BEGIN
    SET NOCOUNT ON;

    IF @Id IS NOT NULL
    BEGIN
        SELECT
            i.*,
            c.CustomerName,
            c.ArabicName AS CustomerArabicName,
            c.TaxRegistrationNumber AS CustomerVATNumber,
            c.StreetAddress AS CustomerAddress,
            c.ArabicAddress AS CustomerArabicAddress,
            c.Email AS CustomerEmail,
            c.Phone AS CustomerPhone,
            c.City AS CustomerCity,
            cmp.Name AS CompanyName,
            cmp.ArabicName AS CompanyArabicName,
            cmp.VATNumber AS CompanyVATNumber,
            cmp.[Address] AS CompanyAddress,
            cmp.ArabicAddress AS CompanyArabicAddress,
            cmp.BankName AS CompanyBankName,
            cmp.BankAccountNumber,
            cmp.IBAN,
            cmp.SwiftCode,
            cmp.AccountCurrency,
            cmp.LogoPath,
            cmp.StampPath,
            cur.Code AS CurrencyCode,
            cur.Symbol AS CurrencySymbol,
            CAST(1 AS INT) AS TotalRecords
        FROM Invoice i
        LEFT JOIN Customer c  ON c.Id  = i.CustomerId
        LEFT JOIN Company cmp ON cmp.Id = i.CompanyId
        LEFT JOIN Currency cur ON cur.Id = i.CurrencyId
        WHERE i.Id = @Id AND i.IsActive = ISNULL(@IsActive, i.IsActive);

        RETURN;
    END

    SELECT
        i.*,
        c.CustomerName,
        cmp.Name AS CompanyName,
        cur.Code AS CurrencyCode,
        COUNT(*) OVER() AS TotalRecords
    FROM Invoice i
    LEFT JOIN Customer c  ON c.Id = i.CustomerId
    LEFT JOIN Company cmp ON cmp.Id = i.CompanyId
    LEFT JOIN Currency cur ON cur.Id = i.CurrencyId
    WHERE i.IsActive = ISNULL(@IsActive, i.IsActive)
      AND (@CustomerId IS NULL OR i.CustomerId = @CustomerId)
      AND (@Status IS NULL OR i.[Status] = @Status)
      AND (@SearchText IS NULL
           OR i.InvoiceNumber LIKE '%' + @SearchText + '%'
           OR i.Reference LIKE '%' + @SearchText + '%'
           OR i.ProjectName LIKE '%' + @SearchText + '%'
           OR c.CustomerName LIKE '%' + @SearchText + '%')
    ORDER BY i.Id DESC
    OFFSET (ISNULL(@PageNumber, 1) - 1) * ISNULL(@PageSize, 20) ROWS
    FETCH NEXT ISNULL(@PageSize, 20) ROWS ONLY;
END
GO

-- ----------------------------------------------------------------------------------------------------
-- Delete Invoice (soft)
-- ----------------------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[DeleteInvoice]
    @Id     INT = NULL,
    @UserId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Invoice SET
        IsActive = 0,
        UpdatedDate = GETDATE(),
        UpdatedBy = @UserId
    WHERE Id = @Id;
END
GO

-- ----------------------------------------------------------------------------------------------------
-- Insert / Update Invoice Product line
-- ----------------------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[InsertUpdateInvoiceProduct]
    @Id                  INT = NULL,
    @InvoiceId           INT = NULL,
    @ProductId           INT = NULL,
    @Description         NVARCHAR(MAX) = NULL,
    @Unit                NVARCHAR(50) = NULL,
    @Quantity            DECIMAL(18,2) = 1,
    @Price               DECIMAL(18,2) = 0,
    @DiscountPercentage  DECIMAL(18,2) = 0,
    @DiscountAmount      DECIMAL(18,2) = 0,
    @TaxRate             DECIMAL(18,2) = 0,
    @TaxableAmount       DECIMAL(18,2) = 0,
    @VATAmount           DECIMAL(18,2) = 0,
    @LineTotal           DECIMAL(18,2) = 0,
    @AccountId           INT = NULL,
    @SortOrder           INT = 0,
    @IsActive            BIT = 1,
    @UserId              INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Id IS NULL OR @Id = 0
    BEGIN
        INSERT INTO InvoiceProduct
        (InvoiceId, ProductId, [Description], Unit, Quantity, Price, DiscountPercentage, DiscountAmount,
         TaxRate, TaxableAmount, VATAmount, LineTotal, AccountId, SortOrder, IsActive,
         CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
        VALUES
        (@InvoiceId, @ProductId, @Description, @Unit, @Quantity, @Price, @DiscountPercentage, @DiscountAmount,
         @TaxRate, @TaxableAmount, @VATAmount, @LineTotal, @AccountId, @SortOrder, ISNULL(@IsActive,1),
         GETDATE(), @UserId, GETDATE(), @UserId);

        SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
    END
    ELSE
    BEGIN
        UPDATE InvoiceProduct SET
            InvoiceId           = @InvoiceId,
            ProductId           = @ProductId,
            [Description]       = @Description,
            Unit                = @Unit,
            Quantity            = @Quantity,
            Price               = @Price,
            DiscountPercentage  = @DiscountPercentage,
            DiscountAmount      = @DiscountAmount,
            TaxRate             = @TaxRate,
            TaxableAmount       = @TaxableAmount,
            VATAmount           = @VATAmount,
            LineTotal           = @LineTotal,
            AccountId           = @AccountId,
            SortOrder           = @SortOrder,
            IsActive            = ISNULL(@IsActive, IsActive),
            UpdatedDate         = GETDATE(),
            UpdatedBy           = @UserId
        WHERE Id = @Id;

        SELECT @Id AS Id;
    END
END
GO

-- ----------------------------------------------------------------------------------------------------
-- Get Invoice Products
-- ----------------------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[GetInvoiceProduct]
    @Id        INT = NULL,
    @InvoiceId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Id IS NOT NULL
    BEGIN
        SELECT *, CAST(1 AS INT) AS TotalRecords FROM InvoiceProduct WHERE Id = @Id;
        RETURN;
    END

    SELECT
        ip.*,
        p.Title AS ProductTitle,
        p.ServiceCode,
        a.Title AS AccountTitle,
        CAST(1 AS INT) AS TotalRecords
    FROM InvoiceProduct ip
    LEFT JOIN Product p ON p.Id = ip.ProductId
    LEFT JOIN AccountType a ON a.Id = ip.AccountId
    WHERE (@InvoiceId IS NULL OR ip.InvoiceId = @InvoiceId)
      AND ip.IsActive = 1
    ORDER BY ip.SortOrder, ip.Id;
END
GO

-- ----------------------------------------------------------------------------------------------------
-- Delete Invoice Product line
-- ----------------------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[DeleteInvoiceProduct]
    @Id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE InvoiceProduct SET IsActive = 0 WHERE Id = @Id;
END
GO

-- ----------------------------------------------------------------------------------------------------
-- Get Invoice Attachments
-- ----------------------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[GetInvoiceAttachments]
    @InvoiceId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        InvoiceId,
        FileName,
        FilePath,
        FileSize,
        ContentType,
        CreatedDate AS CreatedOn,
        CreatedBy,
        CAST(1 AS INT) AS TotalRecords
    FROM InvoiceAttachment
    WHERE (@InvoiceId IS NULL OR InvoiceId = @InvoiceId)
      AND IsActive = 1
    ORDER BY CreatedDate DESC;
END
GO

-- ----------------------------------------------------------------------------------------------------
-- Insert / Update Invoice Attachment
-- ----------------------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[InsertUpdateInvoiceAttachment]
    @Id          INT = NULL,
    @InvoiceId   INT = NULL,
    @FileName    NVARCHAR(255) = NULL,
    @FilePath    NVARCHAR(500) = NULL,
    @FileSize    BIGINT = NULL,
    @ContentType NVARCHAR(100) = NULL,
    @UserId      INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Id IS NULL OR @Id = 0
    BEGIN
        INSERT INTO InvoiceAttachment
        (InvoiceId, FileName, FilePath, FileSize, ContentType, IsActive, CreatedDate, CreatedBy)
        VALUES
        (@InvoiceId, @FileName, @FilePath, @FileSize, @ContentType, 1, GETDATE(), @UserId);

        SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
    END
    ELSE
    BEGIN
        UPDATE InvoiceAttachment SET
            InvoiceId   = @InvoiceId,
            FileName    = @FileName,
            FilePath    = @FilePath,
            FileSize    = @FileSize,
            ContentType = @ContentType
        WHERE Id = @Id;

        SELECT @Id AS Id;
    END
END
GO
