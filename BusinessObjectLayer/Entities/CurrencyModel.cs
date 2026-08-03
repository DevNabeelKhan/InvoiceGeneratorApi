namespace BusinessObjectsLayer.Entities
{
    public class CurrencyModel : BaseEntity
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Symbol { get; set; }
        public decimal? ExchangeRate { get; set; } = 1;
        public int? UserId { get; set; }

        public string? SearchText { get; set; }
        public int? PageNumber { get; set; } = 1;
        public int? PageSize { get; set; } = 20;
    }
}
