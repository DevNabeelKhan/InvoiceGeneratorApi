using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjectsLayer.DTOs
{
    public class InvoiceDto
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Customer is required.")]
        public int? CustomerId { get; set; }

        public int? CompanyId { get; set; }

        [Required(ErrorMessage = "Currency is required.")]
        public int? CurrencyId { get; set; }

        [Required(ErrorMessage = "Invoice date is required.")]
        public DateTime? InvoiceDate { get; set; }

        public DateTime? DueDate { get; set; }

        [StringLength(255, ErrorMessage = "Reference is too long.")]
        public string? Reference { get; set; }

        [StringLength(255, ErrorMessage = "Purchase order number is too long.")]
        public string? PurchaseOrderNumber { get; set; }

        public int? ProjectId { get; set; }

        public bool? PricesIncludeTax { get; set; } = false;

        public string? Notes { get; set; }

        public decimal? ExchangeRate { get; set; } = 1;
        public decimal? DiscountPercentage { get; set; } = 0;
        public decimal? DiscountAmount { get; set; } = 0;
        public decimal? RetentionPercentage { get; set; } = 0;
        public decimal? RetentionAmount { get; set; } = 0;
        public decimal? RoundOffAmount { get; set; } = 0;

        [Required(ErrorMessage = "At least one invoice line is required.")]
        [MinLength(1, ErrorMessage = "At least one invoice line is required.")]
        public List<InvoiceProductDto> Products { get; set; } = new List<InvoiceProductDto>();

        public List<InvoiceAttachmentDto> Attachments { get; set; } = new List<InvoiceAttachmentDto>();
    }

    public class InvoiceProductDto
    {
        public int? Id { get; set; }
        public int? InvoiceId { get; set; }
        public int? ProductId { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string? Description { get; set; }

        public string? Unit { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
        public decimal? Quantity { get; set; } = 1;

        [Required(ErrorMessage = "Price is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Price cannot be negative.")]
        public decimal? Price { get; set; } = 0;

        public decimal? DiscountPercentage { get; set; } = 0;
        public decimal? DiscountAmount { get; set; } = 0;
        public decimal? TaxRate { get; set; } = 0;
        public decimal? TaxableAmount { get; set; } = 0;
        public decimal? VATAmount { get; set; } = 0;
        public decimal? LineTotal { get; set; } = 0;

        [Required(ErrorMessage = "Account is required.")]
        public int? AccountId { get; set; }

        public int? CostCenterId { get; set; }
        public int? RevenueRecognitionId { get; set; }

        public int? SortOrder { get; set; } = 0;
    }

    public class InvoiceAttachmentDto
    {
        public int? Id { get; set; }
        public int? InvoiceId { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public long? FileSize { get; set; }
        public string? ContentType { get; set; }
    }

    public class InvoiceListDto
    {
        public int? Id { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? CustomerName { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal? GrandTotal { get; set; }
        public string? Status { get; set; }
        public string? PaymentStatus { get; set; }
        public string? CurrencyCode { get; set; }
        public int? TotalRecords { get; set; }
    }
}
