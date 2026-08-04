namespace BusinessObjectsLayer.Entities
{
    public class InvoiceProductModel : BaseEntity
    {
        public int? InvoiceId { get; set; }
        public int? ProductId { get; set; }
        public string? Description { get; set; }
        public string? Unit { get; set; }
        public decimal? Quantity { get; set; } = 1;
        public decimal? Price { get; set; } = 0;
        public decimal? DiscountPercentage { get; set; } = 0;
        public decimal? DiscountAmount { get; set; } = 0;
        public decimal? TaxRate { get; set; } = 0;
        public decimal? TaxableAmount { get; set; } = 0;
        public decimal? VATAmount { get; set; } = 0;
        public decimal? LineTotal { get; set; } = 0;
        public int? AccountId { get; set; }
        public int? CostCenterId { get; set; }
        public int? RevenueRecognitionId { get; set; }
        public int? SortOrder { get; set; } = 0;

        // Related data
        public string? ProductTitle { get; set; }
        public string? ServiceCode { get; set; }
        public string? AccountTitle { get; set; }
        public string? CostCenterTitle { get; set; }
        public string? RevenueRecognitionTitle { get; set; }
    }
}
