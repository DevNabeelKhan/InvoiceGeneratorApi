-- ====================================================================================================
-- Adds a numeric RatePercentage column to RevenueTaxRateType so that selecting a Product's
-- RevenueTaxRateId can auto-fill the actual VAT % on an invoice line (functional requirement:
-- "Auto Fill VAT"). Without this column the lookup only carries a display Title (e.g. "Standard 15%")
-- with no machine-readable percentage value.
-- Run against the InvoiceGenerator database. Safe to re-run.
-- ====================================================================================================

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('RevenueTaxRateType') AND name = 'RatePercentage')
    ALTER TABLE RevenueTaxRateType ADD RatePercentage DECIMAL(5,2) NOT NULL DEFAULT 0;
GO

-- Seed a sensible default for the common Saudi standard VAT rate if the table is empty or has no
-- percentage set yet. Adjust/insert rows as needed for your actual tax rate titles.
IF EXISTS (SELECT 1 FROM RevenueTaxRateType WHERE Title LIKE '%15%' AND RatePercentage = 0)
    UPDATE RevenueTaxRateType SET RatePercentage = 15 WHERE Title LIKE '%15%';

IF EXISTS (SELECT 1 FROM RevenueTaxRateType WHERE Title LIKE '%Zero%' AND RatePercentage = 0)
    UPDATE RevenueTaxRateType SET RatePercentage = 0 WHERE Title LIKE '%Zero%';

IF EXISTS (SELECT 1 FROM RevenueTaxRateType WHERE Title LIKE '%Exempt%' AND RatePercentage = 0)
    UPDATE RevenueTaxRateType SET RatePercentage = 0 WHERE Title LIKE '%Exempt%';
GO
