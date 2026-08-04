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
    @Title               NVARCHAR(255) = NULL,
    @ArabicName          NVARCHAR(255) = NULL,
    @Address             NVARCHAR(MAX) = NULL,
    @ArabicAddress       NVARCHAR(MAX) = NULL,
    @Email               NVARCHAR(255) = NULL,
    @Phone               NVARCHAR(50)  = NULL,
    @Website             NVARCHAR(255) = NULL,
    @VATNumber           NVARCHAR(100) = NULL,
    @LogoUrl             NVARCHAR(500) = NULL,
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
        (Title, ArabicName, [Address], ArabicAddress, Email, Phone, Website, VATNumber,
         LogoUrl, LogoPath, StampPath, BankName, BankAccountNumber, IBAN, SwiftCode, AccountCurrency,
         BeneficiaryName, Country, City, IsActive, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
        VALUES
        (@Title, @ArabicName, @Address, @ArabicAddress, @Email, @Phone, @Website, @VATNumber,
         @LogoUrl, @LogoPath, @StampPath, @BankName, @BankAccountNumber, @IBAN, @SwiftCode, @AccountCurrency,
         @BeneficiaryName, @Country, @City, ISNULL(@IsActive, 1), GETDATE(), @UserId, GETDATE(), @UserId);

        SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
    END
    ELSE
    BEGIN
        UPDATE Company SET
            Title               = @Title,
            ArabicName          = @ArabicName,
            [Address]           = @Address,
            ArabicAddress       = @ArabicAddress,
            Email               = @Email,
            Phone               = @Phone,
            Website             = @Website,
            VATNumber           = @VATNumber,
            LogoUrl             = @LogoUrl,
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
    @Title          NVARCHAR(100) = NULL,
    @Symbol         NVARCHAR(10) = NULL,
    @ExchangeRate   DECIMAL(18,6) = 1,
    @IsActive       BIT = 1,
    @UserId         INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Id IS NULL OR @Id = 0
    BEGIN
        INSERT INTO Currency (Code, Title, Symbol, ExchangeRate, IsActive, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
        VALUES (@Code, @Title, @Symbol, @ExchangeRate, ISNULL(@IsActive,1), GETDATE(), @UserId, GETDATE(), @UserId);

        SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
    END
    ELSE
    BEGIN
        UPDATE Currency SET
            Code         = @Code,
            Title        = @Title,
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
    @Year           INT = NULL,
    @Prefix         NVARCHAR(10) = 'INV-',
    @InvoiceNumber  NVARCHAR(50) = NULL OUTPUT
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

    SET @InvoiceNumber = @Prefix + RIGHT('0000' + CAST(@NextNumber AS NVARCHAR(10)), 4) + '/' + CAST(@Year AS NVARCHAR(4));

    -- Result set kept for any callers that still consume it directly (e.g. ad-hoc testing);
    -- the OUTPUT parameter above is what InsertUpdateInvoice actually relies on.
    SELECT @InvoiceNumber AS InvoiceNumber, @NextNumber AS Number;
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
    @ProjectId               INT = NULL,
    @WarehouseId             INT = NULL,
    @PricesIncludeTax        BIT = 0,
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
            EXEC dbo.GetNextInvoiceNumber @Year = NULL, @Prefix = 'INV-', @InvoiceNumber = @NewNumber OUTPUT;
            SET @InvoiceNumber = @NewNumber;
        END

        IF @UUID IS NULL SET @UUID = CONVERT(NVARCHAR(100), NEWID());

        INSERT INTO Invoice
        (InvoiceNumber, [UUID], Reference, PurchaseOrderNumber, ProjectId, WarehouseId, PricesIncludeTax, CompanyId, CustomerId,
         CurrencyId, ExchangeRate, InvoiceDate, DueDate, Notes, [Status], PaymentStatus,
         Draft, Approved, Cancelled, Sent, Subtotal, DiscountPercentage, DiscountAmount,
         TaxAmount, GrandTotal, RetentionPercentage, RetentionAmount, RoundOffAmount,
         GeneratedQRCode, QRCodeImagePath, PreviousInvoiceHash, XMLPath, PDFPath, CreatedIP,
         IsActive, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, UserId)
        VALUES
        (@InvoiceNumber, @UUID, @Reference, @PurchaseOrderNumber, @ProjectId, @WarehouseId, ISNULL(@PricesIncludeTax,0), @CompanyId, @CustomerId,
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
            InvoiceNumber         = ISNULL(@InvoiceNumber, InvoiceNumber),
            Reference             = @Reference,
            PurchaseOrderNumber   = @PurchaseOrderNumber,
            ProjectId             = @ProjectId,
            WarehouseId           = @WarehouseId,
            PricesIncludeTax      = ISNULL(@PricesIncludeTax, PricesIncludeTax),
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

        SELECT Id, InvoiceNumber, [UUID] FROM Invoice WHERE Id = @Id;
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
            cmp.Title AS CompanyName,
            cmp.ArabicName AS CompanyArabicName,
            cmp.VATNumber AS CompanyVATNumber,
            cmp.[Address] AS CompanyAddress,
            cmp.ArabicAddress AS CompanyArabicAddress,
            cmp.BankName AS CompanyBankName,
            cmp.BankAccountNumber,
            cmp.IBAN,
            cmp.SwiftCode,
            cmp.AccountCurrency,
            cmp.BeneficiaryName,
            ISNULL(NULLIF(cmp.LogoPath, ''), cmp.LogoUrl) AS LogoPath,
            cmp.StampPath,
            cur.Code AS CurrencyCode,
            cur.Symbol AS CurrencySymbol,
            pj.Title AS ProjectName,
            wh.[Name] AS WarehouseName,
            CAST(1 AS INT) AS TotalRecords
        FROM Invoice i
        LEFT JOIN Customer c  ON c.Id  = i.CustomerId
        LEFT JOIN Company cmp ON cmp.Id = i.CompanyId
        LEFT JOIN Currency cur ON cur.Id = i.CurrencyId
        LEFT JOIN Project pj ON pj.Id = i.ProjectId
        LEFT JOIN Warehouse wh ON wh.Id = i.WarehouseId
        WHERE i.Id = @Id AND i.IsActive = ISNULL(@IsActive, i.IsActive);

        RETURN;
    END

    SELECT
        i.*,
        c.CustomerName,
        cmp.Title AS CompanyName,
        cur.Code AS CurrencyCode,
        pj.Title AS ProjectName,
        wh.[Name] AS WarehouseName,
        COUNT(*) OVER() AS TotalRecords
    FROM Invoice i
    LEFT JOIN Customer c  ON c.Id = i.CustomerId
    LEFT JOIN Company cmp ON cmp.Id = i.CompanyId
    LEFT JOIN Currency cur ON cur.Id = i.CurrencyId
    LEFT JOIN Project pj ON pj.Id = i.ProjectId
    LEFT JOIN Warehouse wh ON wh.Id = i.WarehouseId
    WHERE i.IsActive = ISNULL(@IsActive, i.IsActive)
      AND (@CustomerId IS NULL OR i.CustomerId = @CustomerId)
      AND (@Status IS NULL OR i.[Status] = @Status)
      AND (@SearchText IS NULL
           OR i.InvoiceNumber LIKE '%' + @SearchText + '%'
           OR i.Reference LIKE '%' + @SearchText + '%'
           OR pj.Title LIKE '%' + @SearchText + '%'
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
    @CostCenterId        INT = NULL,
    @RevenueRecognitionId INT = NULL,
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
         TaxRate, TaxableAmount, VATAmount, LineTotal, AccountId, CostCenterId, RevenueRecognitionId, SortOrder, IsActive,
         CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
        VALUES
        (@InvoiceId, @ProductId, @Description, @Unit, @Quantity, @Price, @DiscountPercentage, @DiscountAmount,
         @TaxRate, @TaxableAmount, @VATAmount, @LineTotal, @AccountId, @CostCenterId, @RevenueRecognitionId, @SortOrder, ISNULL(@IsActive,1),
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
            CostCenterId        = @CostCenterId,
            RevenueRecognitionId = @RevenueRecognitionId,
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
        cc.Title AS CostCenterTitle,
        rr.Title AS RevenueRecognitionTitle,
        CAST(1 AS INT) AS TotalRecords
    FROM InvoiceProduct ip
    LEFT JOIN Product p ON p.Id = ip.ProductId
    LEFT JOIN AccountType a ON a.Id = ip.AccountId
    LEFT JOIN CostCenter cc ON cc.Id = ip.CostCenterId
    LEFT JOIN RevenueRecognitionType rr ON rr.Id = ip.RevenueRecognitionId
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

-- ----------------------------------------------------------------------------------------------------
-- Insert / Update Project
-- ----------------------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[InsertUpdateProject]
    @Id       INT = NULL,
    @Title    NVARCHAR(500) = NULL,
    @IsActive BIT = 1,
    @UserId   INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Id IS NULL OR @Id = 0
    BEGIN
        INSERT INTO Project
        (Title, IsActive, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, UserId)
        VALUES
        (@Title, ISNULL(@IsActive, 1), GETDATE(), @UserId, GETDATE(), @UserId, @UserId);

        SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
    END
    ELSE
    BEGIN
        UPDATE Project SET
            Title       = @Title,
            IsActive    = ISNULL(@IsActive, IsActive),
            UpdatedDate = GETDATE(),
            UpdatedBy   = @UserId
        WHERE Id = @Id;

        SELECT @Id AS Id;
    END
END
GO

-- ----------------------------------------------------------------------------------------------------
-- Get Project
-- ----------------------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[GetProject]
    @Id         INT = NULL,
    @SearchText NVARCHAR(200) = NULL,
    @IsActive   BIT = NULL,
    @PageNumber INT = 1,
    @PageSize   INT = 20
AS
BEGIN
    SET NOCOUNT ON;

    IF @Id IS NOT NULL
    BEGIN
        SELECT *, CAST(1 AS INT) AS TotalRecords FROM Project WHERE Id = @Id;
        RETURN;
    END

    SELECT *, COUNT(*) OVER() AS TotalRecords
    FROM Project
    WHERE (@IsActive IS NULL OR IsActive = @IsActive)
      AND (@SearchText IS NULL OR Title LIKE '%' + @SearchText + '%')
    ORDER BY Id DESC
    OFFSET (ISNULL(@PageNumber, 1) - 1) * ISNULL(@PageSize, 20) ROWS
    FETCH NEXT ISNULL(@PageSize, 20) ROWS ONLY;
END
GO

-- ----------------------------------------------------------------------------------------------------
-- Delete Project (soft)
-- ----------------------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[DeleteProject]
    @Id     INT,
    @UserId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Project SET
        IsActive = 0,
        UpdatedDate = GETDATE(),
        UpdatedBy = @UserId
    WHERE Id = @Id;

    SELECT @Id AS Id;
END
GO

-- ----------------------------------------------------------------------------------------------------
-- Get Project Documents
-- ----------------------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[GetProjectDocument]
    @Id        INT = NULL,
    @ProjectId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        ProjectId,
        DocumentTitle,
        Url,
        CreatedDate AS CreatedOn,
        CreatedBy,
        CAST(1 AS INT) AS TotalRecords
    FROM ProjectDocument
    WHERE (@Id IS NULL OR Id = @Id)
      AND (@ProjectId IS NULL OR ProjectId = @ProjectId)
      AND IsActive = 1
    ORDER BY CreatedDate DESC;
END
GO

-- ----------------------------------------------------------------------------------------------------
-- Insert Project Document
-- ----------------------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[InsertProjectDocument]
    @ProjectId     INT,
    @DocumentTitle NVARCHAR(255) = NULL,
    @Url           NVARCHAR(500) = NULL,
    @UserId        INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO ProjectDocument
    (ProjectId, DocumentTitle, Url, IsActive, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, UserId)
    VALUES
    (@ProjectId, @DocumentTitle, @Url, 1, GETDATE(), @UserId, GETDATE(), @UserId, @UserId);

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
END
GO

-- ----------------------------------------------------------------------------------------------------
-- Delete Project Document (soft)
-- ----------------------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[DeleteProjectDocument]
    @Id     INT,
    @UserId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE ProjectDocument SET
        IsActive = 0,
        UpdatedDate = GETDATE(),
        UpdatedBy = @UserId
    WHERE Id = @Id;

    SELECT @Id AS Id;
END
GO

-- ----------------------------------------------------------------------------------------------------
-- Insert / Update Warehouse
-- ----------------------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[InsertUpdateWarehouse]
    @Id             INT = NULL,
    @Code           NVARCHAR(250) = NULL,
    @Name           NVARCHAR(250) = NULL,
    @Phone          NVARCHAR(250) = NULL,
    @StreetAddress  NVARCHAR(250) = NULL,
    @BuildingNumber NVARCHAR(250) = NULL,
    @District       NVARCHAR(250) = NULL,
    @City           NVARCHAR(250) = NULL,
    @PostalCode     NVARCHAR(250) = NULL,
    @IsActive       BIT = 1,
    @UserId         INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Id IS NULL OR @Id = 0
    BEGIN
        INSERT INTO Warehouse
        (Code, [Name], Phone, StreetAddress, BuildingNumber, District, City, PostalCode,
         IsActive, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, UserId)
        VALUES
        (@Code, @Name, @Phone, @StreetAddress, @BuildingNumber, @District, @City, @PostalCode,
         ISNULL(@IsActive, 1), GETUTCDATE(), @UserId, GETUTCDATE(), @UserId, @UserId);

        SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
    END
    ELSE
    BEGIN
        UPDATE Warehouse SET
            Code           = @Code,
            [Name]         = @Name,
            Phone          = @Phone,
            StreetAddress  = @StreetAddress,
            BuildingNumber = @BuildingNumber,
            District       = @District,
            City           = @City,
            PostalCode     = @PostalCode,
            IsActive       = ISNULL(@IsActive, IsActive),
            UpdatedDate    = GETUTCDATE(),
            UpdatedBy      = @UserId
        WHERE Id = @Id;

        SELECT @Id AS Id;
    END
END
GO

-- ----------------------------------------------------------------------------------------------------
-- Get Warehouse
-- ----------------------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[GetWarehouse]
    @Id         INT = NULL,
    @SearchText NVARCHAR(200) = NULL,
    @IsActive   BIT = NULL,
    @PageNumber INT = 1,
    @PageSize   INT = 20
AS
BEGIN
    SET NOCOUNT ON;

    IF @Id IS NOT NULL
    BEGIN
        SELECT *, CAST(1 AS INT) AS TotalRecords FROM Warehouse WHERE Id = @Id;
        RETURN;
    END

    SELECT *, COUNT(*) OVER() AS TotalRecords
    FROM Warehouse
    WHERE (@IsActive IS NULL OR IsActive = @IsActive)
      AND (@SearchText IS NULL OR [Name] LIKE '%' + @SearchText + '%' OR Code LIKE '%' + @SearchText + '%')
    ORDER BY Id DESC
    OFFSET (ISNULL(@PageNumber, 1) - 1) * ISNULL(@PageSize, 20) ROWS
    FETCH NEXT ISNULL(@PageSize, 20) ROWS ONLY;
END
GO

-- ----------------------------------------------------------------------------------------------------
-- Delete Warehouse (soft)
-- ----------------------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[DeleteWarehouse]
    @Id     INT,
    @UserId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Warehouse SET
        IsActive = 0,
        UpdatedDate = GETUTCDATE(),
        UpdatedBy = @UserId
    WHERE Id = @Id;

    SELECT @Id AS Id;
END
GO
