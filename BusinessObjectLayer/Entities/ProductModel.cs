namespace BusinessObjectsLayer.Entities
{
    public class ProductModel : BaseEntity
    {
        public string? Title { get; set; }
        public int? ProductStatusId { get; set; }
        public int? UnitOfMeasureId { get; set; }
        public string? ServiceCode { get; set; }
        public string? ServiceDescription { get; set; }
        public decimal? SellingPrice { get; set; }
        public int? RevenueAccountID { get; set; }
        public int? RevenueTaxRateId { get; set; }
        public decimal? RevenueTaxRatePercentage { get; set; }
        public decimal? PurchaseCost { get; set; }
        public int? ExpenseAccountId { get; set; }
        public int? PurchaseTaxRateId { get; set; }
        public int? UserId { get; set; }

        public string? SearchText { get; set; }
        public int? PageNumber { get; set; } = 1;
        public int? PageSize { get; set; } = 20;
    }
}
