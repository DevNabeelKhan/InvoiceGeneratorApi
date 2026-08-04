-- ====================================================================================================
-- Invoice Module Schema Migration
-- Run against the InvoiceGenerator database.
-- ====================================================================================================
-- This script creates the tables, ALTER statements and stored procedures required for a production
-- invoice module.  It intentionally avoids altering columns already used by the existing application
-- and only adds the missing columns/tables that are required for professional invoicing.
-- ====================================================================================================

SET NOCOUNT ON;
GO

-- ----------------------------------------------------------------------------------------------------
-- 1. Missing columns on existing Customer table
-- ----------------------------------------------------------------------------------------------------
-- Why needed:
--   ArabicName      -> Saudi/ZATCA tax invoices must show the customer name in Arabic on the PDF.
--   ArabicAddress   -> Arabic address is mandatory for Arabic tax invoice printouts.
--   Email           -> General customer contact email (separate from the existing invoicing email).
--   Phone           -> General customer phone (separate from the existing invoicing phone).
-- ----------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Customer') AND name = 'ArabicName')
    ALTER TABLE Customer ADD ArabicName NVARCHAR(255) NULL;

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Customer') AND name = 'ArabicAddress')
    ALTER TABLE Customer ADD ArabicAddress NVARCHAR(MAX) NULL;

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Customer') AND name = 'Email')
    ALTER TABLE Customer ADD Email NVARCHAR(255) NULL;

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Customer') AND name = 'Phone')
    ALTER TABLE Customer ADD Phone NVARCHAR(50) NULL;
GO

-- ----------------------------------------------------------------------------------------------------
-- 2. Currency master table
-- ----------------------------------------------------------------------------------------------------
-- Why needed:
--   Invoices can be raised in multiple currencies. Currency master holds the symbol and exchange rate
--   so that the local-currency equivalent can be calculated and displayed.
-- ----------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Currency')
BEGIN
    CREATE TABLE Currency (
        Id              INT IDENTITY(1,1) PRIMARY KEY,
        Code            NVARCHAR(10)    NOT NULL,
        Title           NVARCHAR(100)   NOT NULL,
        Symbol          NVARCHAR(10)    NULL,
        ExchangeRate    DECIMAL(18,6)   NOT NULL DEFAULT 1,
        IsActive        BIT             NOT NULL DEFAULT 1,
        CreatedDate     DATETIME        NULL,
        CreatedBy       INT             NULL,
        UpdatedDate     DATETIME        NULL,
        UpdatedBy       INT             NULL
    );
END
GO

-- ----------------------------------------------------------------------------------------------------
-- 3. Company master table
-- ----------------------------------------------------------------------------------------------------
-- Why needed:
--   The seller/issuer details must appear on every tax invoice.  This table stores the company
--   profile including Arabic/English names, address, VAT number, logo, stamp and bank details.
-- ----------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Company')
BEGIN
    CREATE TABLE Company (
        Id                  INT IDENTITY(1,1) PRIMARY KEY,
        Title               NVARCHAR(255)   NOT NULL,
        ArabicName          NVARCHAR(255)   NULL,
        Address             NVARCHAR(MAX)   NULL,
        ArabicAddress       NVARCHAR(MAX)   NULL,
        Email               NVARCHAR(255)   NULL,
        Phone               NVARCHAR(50)    NULL,
        Website             NVARCHAR(255)   NULL,
        VATNumber           NVARCHAR(100)   NULL,
        LogoUrl             NVARCHAR(500)   NULL,
        LogoPath            NVARCHAR(500)   NULL,
        StampPath           NVARCHAR(500)   NULL,
        BankName            NVARCHAR(255)   NULL,
        BankAccountNumber   NVARCHAR(100)   NULL,
        IBAN                NVARCHAR(100)   NULL,
        SwiftCode           NVARCHAR(50)    NULL,
        AccountCurrency     NVARCHAR(50)    NULL,
        BeneficiaryName     NVARCHAR(255)   NULL,
        Country             NVARCHAR(100)   NULL,
        City                NVARCHAR(100)   NULL,
        IsActive            BIT             NOT NULL DEFAULT 1,
        CreatedDate         DATETIME        NULL,
        CreatedBy           INT             NULL,
        UpdatedDate         DATETIME        NULL,
        UpdatedBy           INT             NULL
    );
END
GO

-- ----------------------------------------------------------------------------------------------------
-- 4. Invoice header table
-- ----------------------------------------------------------------------------------------------------
-- Why needed:
--   Stores the complete invoice header including ZATCA Phase 2 fields (UUID, QR code, hash),
--   status tracking, totals and file paths for XML/PDF.
-- ----------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Invoice')
BEGIN
    CREATE TABLE Invoice (
        Id                      INT IDENTITY(1,1) PRIMARY KEY,
        InvoiceNumber           NVARCHAR(50)    NOT NULL,
        [UUID]                  NVARCHAR(100)   NULL,
        Reference               NVARCHAR(255)   NULL,
        PurchaseOrderNumber     NVARCHAR(255)   NULL,
        ProjectName             NVARCHAR(500)   NULL,
        CompanyId               INT             NULL,
        CustomerId              INT             NOT NULL,
        CurrencyId              INT             NULL,
        ExchangeRate            DECIMAL(18,6)   NOT NULL DEFAULT 1,
        InvoiceDate             DATE            NOT NULL,
        DueDate                 DATE            NULL,
        Notes                   NVARCHAR(MAX)   NULL,
        [Status]                NVARCHAR(50)    NOT NULL DEFAULT 'Draft', -- Draft | Approved | Sent | Cancelled
        PaymentStatus           NVARCHAR(50)    NOT NULL DEFAULT 'Unpaid', -- Unpaid | Partial | Paid
        Draft                   BIT             NOT NULL DEFAULT 1,
        Approved                BIT             NOT NULL DEFAULT 0,
        Cancelled               BIT             NOT NULL DEFAULT 0,
        Sent                    BIT             NOT NULL DEFAULT 0,
        Subtotal                DECIMAL(18,2)   NOT NULL DEFAULT 0,
        DiscountPercentage      DECIMAL(18,2)   NULL DEFAULT 0,
        DiscountAmount          DECIMAL(18,2)   NULL DEFAULT 0,
        TaxAmount               DECIMAL(18,2)   NOT NULL DEFAULT 0,
        GrandTotal              DECIMAL(18,2)   NOT NULL DEFAULT 0,
        RetentionPercentage     DECIMAL(18,2)   NULL DEFAULT 0,
        RetentionAmount         DECIMAL(18,2)   NULL DEFAULT 0,
        RoundOffAmount          DECIMAL(18,2)   NULL DEFAULT 0,
        GeneratedQRCode         NVARCHAR(MAX)   NULL,   -- Base64 TLV string
        QRCodeImagePath         NVARCHAR(500)   NULL,
        PreviousInvoiceHash     NVARCHAR(500)   NULL,
        XMLPath                 NVARCHAR(500)   NULL,
        PDFPath                 NVARCHAR(500)   NULL,
        CreatedIP               NVARCHAR(100)   NULL,
        IsActive                BIT             NOT NULL DEFAULT 1,
        CreatedDate             DATETIME        NULL,
        CreatedBy               INT             NULL,
        UpdatedDate             DATETIME        NULL,
        UpdatedBy               INT             NULL,
        UserId                  INT             NULL
    );

    ALTER TABLE Invoice ADD CONSTRAINT UQ_Invoice_InvoiceNumber UNIQUE (InvoiceNumber);
    ALTER TABLE Invoice ADD CONSTRAINT FK_Invoice_Customer FOREIGN KEY (CustomerId) REFERENCES Customer(Id);
    ALTER TABLE Invoice ADD CONSTRAINT FK_Invoice_Currency FOREIGN KEY (CurrencyId) REFERENCES Currency(Id);
    ALTER TABLE Invoice ADD CONSTRAINT FK_Invoice_Company  FOREIGN KEY (CompanyId)  REFERENCES Company(Id);
END
GO

-- ----------------------------------------------------------------------------------------------------
-- 5. Invoice product line items table
-- ----------------------------------------------------------------------------------------------------
-- Why needed:
--   Invoice lines must be stored independently from the product master so that historical prices,
--   discounts, tax and line totals are preserved even if the product master changes.
-- ----------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'InvoiceProduct')
BEGIN
    CREATE TABLE InvoiceProduct (
        Id                  INT IDENTITY(1,1) PRIMARY KEY,
        InvoiceId           INT             NOT NULL,
        ProductId           INT             NULL,
        [Description]       NVARCHAR(MAX)   NOT NULL,
        Unit                NVARCHAR(50)    NULL,
        Quantity            DECIMAL(18,2)   NOT NULL DEFAULT 1,
        Price               DECIMAL(18,2)   NOT NULL DEFAULT 0,
        DiscountPercentage  DECIMAL(18,2)   NULL DEFAULT 0,
        DiscountAmount      DECIMAL(18,2)   NULL DEFAULT 0,
        TaxRate             DECIMAL(18,2)   NOT NULL DEFAULT 0,    -- VAT %
        TaxableAmount       DECIMAL(18,2)   NOT NULL DEFAULT 0,
        VATAmount           DECIMAL(18,2)   NOT NULL DEFAULT 0,
        LineTotal           DECIMAL(18,2)   NOT NULL DEFAULT 0,
        AccountId           INT             NULL,
        SortOrder           INT             NOT NULL DEFAULT 0,
        IsActive            BIT             NOT NULL DEFAULT 1,
        CreatedDate         DATETIME        NULL,
        CreatedBy           INT             NULL,
        UpdatedDate         DATETIME        NULL,
        UpdatedBy           INT             NULL
    );

    ALTER TABLE InvoiceProduct ADD CONSTRAINT FK_InvoiceProduct_Invoice FOREIGN KEY (InvoiceId) REFERENCES Invoice(Id) ON DELETE CASCADE;
    ALTER TABLE InvoiceProduct ADD CONSTRAINT FK_InvoiceProduct_Product  FOREIGN KEY (ProductId)  REFERENCES Product(Id);
    ALTER TABLE InvoiceProduct ADD CONSTRAINT FK_InvoiceProduct_Account  FOREIGN KEY (AccountId)  REFERENCES AccountType(Id);
END
GO

-- ----------------------------------------------------------------------------------------------------
-- 6. Invoice attachments table
-- ----------------------------------------------------------------------------------------------------
-- Why needed:
--   A professional invoice system must support supporting documents (PO, delivery notes, etc.).
-- ----------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'InvoiceAttachment')
BEGIN
    CREATE TABLE InvoiceAttachment (
        Id          INT IDENTITY(1,1) PRIMARY KEY,
        InvoiceId   INT             NOT NULL,
        FileName    NVARCHAR(255)   NOT NULL,
        FilePath    NVARCHAR(500)   NOT NULL,
        FileSize    BIGINT          NULL,
        ContentType NVARCHAR(100)   NULL,
        IsActive    BIT             NOT NULL DEFAULT 1,
        CreatedDate DATETIME        NULL,
        CreatedBy   INT             NULL
    );

    ALTER TABLE InvoiceAttachment ADD CONSTRAINT FK_InvoiceAttachment_Invoice FOREIGN KEY (InvoiceId) REFERENCES Invoice(Id) ON DELETE CASCADE;
END
GO

-- ----------------------------------------------------------------------------------------------------
-- 7. Invoice number sequence helper table
-- ----------------------------------------------------------------------------------------------------
-- Why needed:
--   Provides a thread-safe way to generate gap-free invoice numbers per year (e.g. INV-0001/2026).
-- ----------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'InvoiceNumberSequence')
BEGIN
    CREATE TABLE InvoiceNumberSequence (
        Id          INT IDENTITY(1,1) PRIMARY KEY,
        [Year]      INT             NOT NULL,
        LastNumber  INT             NOT NULL DEFAULT 0,
        CONSTRAINT UQ_InvoiceNumberSequence_Year UNIQUE ([Year])
    );
END
GO
