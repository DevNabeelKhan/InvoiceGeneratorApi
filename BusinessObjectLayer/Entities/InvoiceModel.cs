using System;
using System.Collections.Generic;

namespace BusinessObjectsLayer.Entities
{
    public class InvoiceModel : BaseEntity
    {
        public string? InvoiceNumber { get; set; }
        public string? UUID { get; set; }
        public string? Reference { get; set; }
        public string? PurchaseOrderNumber { get; set; }
        public int? ProjectId { get; set; }
        public bool? PricesIncludeTax { get; set; } = false;
        public int? CompanyId { get; set; }
        public int? CustomerId { get; set; }
        public int? CurrencyId { get; set; }
        public decimal? ExchangeRate { get; set; } = 1;
        public DateTime? InvoiceDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Notes { get; set; }
        public string? Status { get; set; } = "Draft";
        public string? PaymentStatus { get; set; } = "Unpaid";
        public bool? Draft { get; set; } = true;
        public bool? Approved { get; set; } = false;
        public bool? Cancelled { get; set; } = false;
        public bool? Sent { get; set; } = false;
        public decimal? Subtotal { get; set; } = 0;
        public decimal? DiscountPercentage { get; set; } = 0;
        public decimal? DiscountAmount { get; set; } = 0;
        public decimal? TaxAmount { get; set; } = 0;
        public decimal? GrandTotal { get; set; } = 0;
        public decimal? RetentionPercentage { get; set; } = 0;
        public decimal? RetentionAmount { get; set; } = 0;
        public decimal? RoundOffAmount { get; set; } = 0;
        public string? GeneratedQRCode { get; set; }
        public string? QRCodeImagePath { get; set; }
        public string? PreviousInvoiceHash { get; set; }
        public string? XMLPath { get; set; }
        public string? PDFPath { get; set; }
        public string? CreatedIP { get; set; }
        public int? UserId { get; set; }

        // Related data (not DB columns)
        public string? CustomerName { get; set; }
        public string? CustomerArabicName { get; set; }
        public string? CustomerVATNumber { get; set; }
        public string? CustomerAddress { get; set; }
        public string? CustomerArabicAddress { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerPhone { get; set; }
        public string? CustomerCity { get; set; }

        public string? CompanyName { get; set; }
        public string? CompanyArabicName { get; set; }
        public string? CompanyVATNumber { get; set; }
        public string? CompanyAddress { get; set; }
        public string? CompanyArabicAddress { get; set; }
        public string? CompanyBankName { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? IBAN { get; set; }
        public string? SwiftCode { get; set; }
        public string? AccountCurrency { get; set; }
        public string? LogoPath { get; set; }
        public string? StampPath { get; set; }

        public string? CurrencyCode { get; set; }
        public string? CurrencySymbol { get; set; }

        public string? ProjectName { get; set; }

        public List<InvoiceProductModel> Products { get; set; } = new List<InvoiceProductModel>();
        public List<InvoiceAttachmentModel> Attachments { get; set; } = new List<InvoiceAttachmentModel>();

        // Search/paging helpers
        public string? SearchText { get; set; }
        public int? PageNumber { get; set; } = 1;
        public int? PageSize { get; set; } = 20;
    }
}
