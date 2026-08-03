-- ====================================================================================================
-- Adds any missing columns to Company / Currency / Invoice / InvoiceProduct / InvoiceAttachment.
-- Use this if those tables already existed (e.g. created empty/partial before) so that
-- CreateInvoiceSchema.sql's "CREATE TABLE IF NOT EXISTS" skipped them and columns were never added.
-- Safe to re-run any number of times.
-- Run against the InvoiceGenerator database.
-- ====================================================================================================

SET NOCOUNT ON;
GO

-- ----------------------------------------------------------------------------------------------------
-- Company
-- ----------------------------------------------------------------------------------------------------
IF OBJECT_ID('Company') IS NULL
BEGIN
    CREATE TABLE Company (
        Id INT IDENTITY(1,1) PRIMARY KEY
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Company') AND name = 'Name')
    ALTER TABLE Company ADD Name NVARCHAR(255) NOT NULL DEFAULT '';
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Company') AND name = 'ArabicName')
    ALTER TABLE Company ADD ArabicName NVARCHAR(255) NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Company') AND name = 'Address')
    ALTER TABLE Company ADD [Address] NVARCHAR(MAX) NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Company') AND name = 'ArabicAddress')
    ALTER TABLE Company ADD ArabicAddress NVARCHAR(MAX) NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Company') AND name = 'Email')
    ALTER TABLE Company ADD Email NVARCHAR(255) NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Company') AND name = 'Phone')
    ALTER TABLE Company ADD Phone NVARCHAR(50) NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Company') AND name = 'Website')
    ALTER TABLE Company ADD Website NVARCHAR(255) NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Company') AND name = 'VATNumber')
    ALTER TABLE Company ADD VATNumber NVARCHAR(100) NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Company') AND name = 'LogoPath')
    ALTER TABLE Company ADD LogoPath NVARCHAR(500) NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Company') AND name = 'StampPath')
    ALTER TABLE Company ADD StampPath NVARCHAR(500) NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Company') AND name = 'BankName')
    ALTER TABLE Company ADD BankName NVARCHAR(255) NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Company') AND name = 'BankAccountNumber')
    ALTER TABLE Company ADD BankAccountNumber NVARCHAR(100) NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Company') AND name = 'IBAN')
    ALTER TABLE Company ADD IBAN NVARCHAR(100) NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Company') AND name = 'SwiftCode')
    ALTER TABLE Company ADD SwiftCode NVARCHAR(50) NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Company') AND name = 'AccountCurrency')
    ALTER TABLE Company ADD AccountCurrency NVARCHAR(50) NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Company') AND name = 'BeneficiaryName')
    ALTER TABLE Company ADD BeneficiaryName NVARCHAR(255) NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Company') AND name = 'Country')
    ALTER TABLE Company ADD Country NVARCHAR(100) NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Company') AND name = 'City')
    ALTER TABLE Company ADD City NVARCHAR(100) NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Company') AND name = 'IsActive')
    ALTER TABLE Company ADD IsActive BIT NOT NULL DEFAULT 1;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Company') AND name = 'CreatedDate')
    ALTER TABLE Company ADD CreatedDate DATETIME NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Company') AND name = 'CreatedBy')
    ALTER TABLE Company ADD CreatedBy INT NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Company') AND name = 'UpdatedDate')
    ALTER TABLE Company ADD UpdatedDate DATETIME NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Company') AND name = 'UpdatedBy')
    ALTER TABLE Company ADD UpdatedBy INT NULL;
GO

-- ----------------------------------------------------------------------------------------------------
-- Currency
-- ----------------------------------------------------------------------------------------------------
IF OBJECT_ID('Currency') IS NULL
BEGIN
    CREATE TABLE Currency (
        Id INT IDENTITY(1,1) PRIMARY KEY
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Currency') AND name = 'Code')
    ALTER TABLE Currency ADD Code NVARCHAR(10) NOT NULL DEFAULT '';
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Currency') AND name = 'Name')
    ALTER TABLE Currency ADD Name NVARCHAR(100) NOT NULL DEFAULT '';
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Currency') AND name = 'Symbol')
    ALTER TABLE Currency ADD Symbol NVARCHAR(10) NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Currency') AND name = 'ExchangeRate')
    ALTER TABLE Currency ADD ExchangeRate DECIMAL(18,6) NOT NULL DEFAULT 1;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Currency') AND name = 'IsActive')
    ALTER TABLE Currency ADD IsActive BIT NOT NULL DEFAULT 1;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Currency') AND name = 'CreatedDate')
    ALTER TABLE Currency ADD CreatedDate DATETIME NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Currency') AND name = 'CreatedBy')
    ALTER TABLE Currency ADD CreatedBy INT NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Currency') AND name = 'UpdatedDate')
    ALTER TABLE Currency ADD UpdatedDate DATETIME NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Currency') AND name = 'UpdatedBy')
    ALTER TABLE Currency ADD UpdatedBy INT NULL;
GO

-- ----------------------------------------------------------------------------------------------------
-- InvoiceNumberSequence
-- ----------------------------------------------------------------------------------------------------
IF OBJECT_ID('InvoiceNumberSequence') IS NULL
BEGIN
    CREATE TABLE InvoiceNumberSequence (
        Id          INT IDENTITY(1,1) PRIMARY KEY,
        [Year]      INT NOT NULL,
        LastNumber  INT NOT NULL DEFAULT 0,
        CONSTRAINT UQ_InvoiceNumberSequence_Year UNIQUE ([Year])
    );
END
GO

-- ----------------------------------------------------------------------------------------------------
-- Invoice
-- ----------------------------------------------------------------------------------------------------
IF OBJECT_ID('Invoice') IS NULL
BEGIN
    CREATE TABLE Invoice (
        Id INT IDENTITY(1,1) PRIMARY KEY
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Invoice') AND name = 'InvoiceNumber')
    ALTER TABLE Invoice ADD InvoiceNumber NVARCHAR(50) NOT NULL DEFAULT '';
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Invoice') AND name = 'UUID')
    ALTER TABLE Invoice ADD [UUID] NVARCHAR(100) NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Invoice') AND name = 'Reference')
    ALTER TABLE Invoice ADD Reference NVARCHAR(255) NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Invoice') AND name = 'PurchaseOrderNumber')
    ALTER TABLE Invoice ADD PurchaseOrderNumber NVARCHAR(255) NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Invoice') AND name = 'ProjectName')
    ALTER TABLE Invoice ADD ProjectName NVARCHAR(500) NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Invoice') AND name = 'CompanyId')
    ALTER TABLE Invoice ADD CompanyId INT NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Invoice') AND name = 'CustomerId')
    ALTER TABLE Invoice ADD CustomerId INT NOT NULL DEFAULT 0;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Invoice') AND name = 'CurrencyId')
    ALTER TABLE Invoice ADD CurrencyId INT NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Invoice') AND name = 'ExchangeRate')
    ALTER TABLE Invoice ADD ExchangeRate DECIMAL(18,6) NOT NULL DEFAULT 1;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Invoice') AND name = 'InvoiceDate')
    ALTER TABLE Invoice ADD InvoiceDate DATE NOT NULL DEFAULT GETDATE();
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Invoice') AND name = 'DueDate')
    ALTER TABLE Invoice ADD DueDate DATE NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Invoice') AND name = 'Notes')
    ALTER TABLE Invoice ADD Notes NVARCHAR(MAX) NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Invoice') AND name = 'Status')
    ALTER TABLE Invoice ADD [Status] NVARCHAR(50) NOT NULL DEFAULT 'Draft';
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Invoice') AND name = 'PaymentStatus')
    ALTER TABLE Invoice ADD PaymentStatus NVARCHAR(50) NOT NULL DEFAULT 'Unpaid';
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Invoice') AND name = 'Draft')
    ALTER TABLE Invoice ADD Draft BIT NOT NULL DEFAULT 1;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Invoice') AND name = 'Approved')
    ALTER TABLE Invoice ADD Approved BIT NOT NULL DEFAULT 0;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Invoice') AND name = 'Cancelled')
    ALTER TABLE Invoice ADD Cancelled BIT NOT NULL DEFAULT 0;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Invoice') AND name = 'Sent')
    ALTER TABLE Invoice ADD Sent BIT NOT NULL DEFAULT 0;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Invoice') AND name = 'Subtotal')
    ALTER TABLE Invoice ADD Subtotal DECIMAL(18,2) NOT NULL DEFAULT 0;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Invoice') AND name = 'DiscountPercentage')
    ALTER TABLE Invoice ADD DiscountPercentage DECIMAL(18,2) NULL DEFAULT 0;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Invoice') AND name = 'DiscountAmount')
    ALTER TABLE Invoice ADD DiscountAmount DECIMAL(18,2) NULL DEFAULT 0;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Invoice') AND name = 'TaxAmount')
    ALTER TABLE Invoice ADD TaxAmount DECIMAL(18,2) NOT NULL DEFAULT 0;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Invoice') AND name = 'GrandTotal')
    ALTER TABLE Invoice ADD GrandTotal DECIMAL(18,2) NOT NULL DEFAULT 0;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Invoice') AND name = 'RetentionPercentage')
    ALTER TABLE Invoice ADD RetentionPercentage DECIMAL(18,2) NULL DEFAULT 0;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Invoice') AND name = 'RetentionAmount')
    ALTER TABLE Invoice ADD RetentionAmount DECIMAL(18,2) NULL DEFAULT 0;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Invoice') AND name = 'RoundOffAmount')
    ALTER TABLE Invoice ADD RoundOffAmount DECIMAL(18,2) NULL DEFAULT 0;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Invoice') AND name = 'GeneratedQRCode')
    ALTER TABLE Invoice ADD GeneratedQRCode NVARCHAR(MAX) NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Invoice') AND name = 'QRCodeImagePath')
    ALTER TABLE Invoice ADD QRCodeImagePath NVARCHAR(500) NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Invoice') AND name = 'PreviousInvoiceHash')
    ALTER TABLE Invoice ADD PreviousInvoiceHash NVARCHAR(500) NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Invoice') AND name = 'XMLPath')
    ALTER TABLE Invoice ADD XMLPath NVARCHAR(500) NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Invoice') AND name = 'PDFPath')
    ALTER TABLE Invoice ADD PDFPath NVARCHAR(500) NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Invoice') AND name = 'CreatedIP')
    ALTER TABLE Invoice ADD CreatedIP NVARCHAR(100) NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Invoice') AND name = 'IsActive')
    ALTER TABLE Invoice ADD IsActive BIT NOT NULL DEFAULT 1;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Invoice') AND name = 'CreatedDate')
    ALTER TABLE Invoice ADD CreatedDate DATETIME NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Invoice') AND name = 'CreatedBy')
    ALTER TABLE Invoice ADD CreatedBy INT NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Invoice') AND name = 'UpdatedDate')
    ALTER TABLE Invoice ADD UpdatedDate DATETIME NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Invoice') AND name = 'UpdatedBy')
    ALTER TABLE Invoice ADD UpdatedBy INT NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Invoice') AND name = 'UserId')
    ALTER TABLE Invoice ADD UserId INT NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UQ_Invoice_InvoiceNumber' AND object_id = OBJECT_ID('Invoice'))
    ALTER TABLE Invoice ADD CONSTRAINT UQ_Invoice_InvoiceNumber UNIQUE (InvoiceNumber);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Invoice_Customer')
    ALTER TABLE Invoice ADD CONSTRAINT FK_Invoice_Customer FOREIGN KEY (CustomerId) REFERENCES Customer(Id);
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Invoice_Currency')
    ALTER TABLE Invoice ADD CONSTRAINT FK_Invoice_Currency FOREIGN KEY (CurrencyId) REFERENCES Currency(Id);
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Invoice_Company')
    ALTER TABLE Invoice ADD CONSTRAINT FK_Invoice_Company FOREIGN KEY (CompanyId) REFERENCES Company(Id);
GO

-- ----------------------------------------------------------------------------------------------------
-- InvoiceProduct
-- ----------------------------------------------------------------------------------------------------
IF OBJECT_ID('InvoiceProduct') IS NULL
BEGIN
    CREATE TABLE InvoiceProduct (
        Id INT IDENTITY(1,1) PRIMARY KEY
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('InvoiceProduct') AND name = 'InvoiceId')
    ALTER TABLE InvoiceProduct ADD InvoiceId INT NOT NULL DEFAULT 0;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('InvoiceProduct') AND name = 'ProductId')
    ALTER TABLE InvoiceProduct ADD ProductId INT NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('InvoiceProduct') AND name = 'Description')
    ALTER TABLE InvoiceProduct ADD [Description] NVARCHAR(MAX) NOT NULL DEFAULT '';
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('InvoiceProduct') AND name = 'Unit')
    ALTER TABLE InvoiceProduct ADD Unit NVARCHAR(50) NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('InvoiceProduct') AND name = 'Quantity')
    ALTER TABLE InvoiceProduct ADD Quantity DECIMAL(18,2) NOT NULL DEFAULT 1;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('InvoiceProduct') AND name = 'Price')
    ALTER TABLE InvoiceProduct ADD Price DECIMAL(18,2) NOT NULL DEFAULT 0;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('InvoiceProduct') AND name = 'DiscountPercentage')
    ALTER TABLE InvoiceProduct ADD DiscountPercentage DECIMAL(18,2) NULL DEFAULT 0;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('InvoiceProduct') AND name = 'DiscountAmount')
    ALTER TABLE InvoiceProduct ADD DiscountAmount DECIMAL(18,2) NULL DEFAULT 0;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('InvoiceProduct') AND name = 'TaxRate')
    ALTER TABLE InvoiceProduct ADD TaxRate DECIMAL(18,2) NOT NULL DEFAULT 0;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('InvoiceProduct') AND name = 'TaxableAmount')
    ALTER TABLE InvoiceProduct ADD TaxableAmount DECIMAL(18,2) NOT NULL DEFAULT 0;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('InvoiceProduct') AND name = 'VATAmount')
    ALTER TABLE InvoiceProduct ADD VATAmount DECIMAL(18,2) NOT NULL DEFAULT 0;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('InvoiceProduct') AND name = 'LineTotal')
    ALTER TABLE InvoiceProduct ADD LineTotal DECIMAL(18,2) NOT NULL DEFAULT 0;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('InvoiceProduct') AND name = 'AccountId')
    ALTER TABLE InvoiceProduct ADD AccountId INT NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('InvoiceProduct') AND name = 'SortOrder')
    ALTER TABLE InvoiceProduct ADD SortOrder INT NOT NULL DEFAULT 0;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('InvoiceProduct') AND name = 'IsActive')
    ALTER TABLE InvoiceProduct ADD IsActive BIT NOT NULL DEFAULT 1;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('InvoiceProduct') AND name = 'CreatedDate')
    ALTER TABLE InvoiceProduct ADD CreatedDate DATETIME NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('InvoiceProduct') AND name = 'CreatedBy')
    ALTER TABLE InvoiceProduct ADD CreatedBy INT NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('InvoiceProduct') AND name = 'UpdatedDate')
    ALTER TABLE InvoiceProduct ADD UpdatedDate DATETIME NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('InvoiceProduct') AND name = 'UpdatedBy')
    ALTER TABLE InvoiceProduct ADD UpdatedBy INT NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_InvoiceProduct_Invoice')
    ALTER TABLE InvoiceProduct ADD CONSTRAINT FK_InvoiceProduct_Invoice FOREIGN KEY (InvoiceId) REFERENCES Invoice(Id) ON DELETE CASCADE;
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_InvoiceProduct_Product')
    ALTER TABLE InvoiceProduct ADD CONSTRAINT FK_InvoiceProduct_Product FOREIGN KEY (ProductId) REFERENCES Product(Id);
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_InvoiceProduct_Account')
    ALTER TABLE InvoiceProduct ADD CONSTRAINT FK_InvoiceProduct_Account FOREIGN KEY (AccountId) REFERENCES AccountType(Id);
GO

-- ----------------------------------------------------------------------------------------------------
-- InvoiceAttachment
-- ----------------------------------------------------------------------------------------------------
IF OBJECT_ID('InvoiceAttachment') IS NULL
BEGIN
    CREATE TABLE InvoiceAttachment (
        Id INT IDENTITY(1,1) PRIMARY KEY
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('InvoiceAttachment') AND name = 'InvoiceId')
    ALTER TABLE InvoiceAttachment ADD InvoiceId INT NOT NULL DEFAULT 0;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('InvoiceAttachment') AND name = 'FileName')
    ALTER TABLE InvoiceAttachment ADD FileName NVARCHAR(255) NOT NULL DEFAULT '';
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('InvoiceAttachment') AND name = 'FilePath')
    ALTER TABLE InvoiceAttachment ADD FilePath NVARCHAR(500) NOT NULL DEFAULT '';
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('InvoiceAttachment') AND name = 'FileSize')
    ALTER TABLE InvoiceAttachment ADD FileSize BIGINT NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('InvoiceAttachment') AND name = 'ContentType')
    ALTER TABLE InvoiceAttachment ADD ContentType NVARCHAR(100) NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('InvoiceAttachment') AND name = 'IsActive')
    ALTER TABLE InvoiceAttachment ADD IsActive BIT NOT NULL DEFAULT 1;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('InvoiceAttachment') AND name = 'CreatedDate')
    ALTER TABLE InvoiceAttachment ADD CreatedDate DATETIME NULL;
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('InvoiceAttachment') AND name = 'CreatedBy')
    ALTER TABLE InvoiceAttachment ADD CreatedBy INT NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_InvoiceAttachment_Invoice')
    ALTER TABLE InvoiceAttachment ADD CONSTRAINT FK_InvoiceAttachment_Invoice FOREIGN KEY (InvoiceId) REFERENCES Invoice(Id) ON DELETE CASCADE;
GO

-- ----------------------------------------------------------------------------------------------------
-- Customer (Arabic/Email/Phone columns needed by Invoice module)
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
