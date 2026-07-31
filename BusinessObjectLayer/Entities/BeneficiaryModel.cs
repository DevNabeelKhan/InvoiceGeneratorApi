namespace BusinessObjectsLayer.Entities
{
    public class BeneficiaryModel : BaseEntity
    {
        public string? IBAN { get; set; }
        public int? CurrencyId { get; set; }
        public string? BeneficiaryName { get; set; }
        public string? BeneficiaryAddress { get; set; }
        public string? BankName { get; set; }
        public string? Swift { get; set; }
        public int? CountryId { get; set; }
        public int? BankFeesTypeId { get; set; }
        public int? UserId { get; set; }

        public string? SearchText { get; set; }
        public int? PageNumber { get; set; } = 1;
        public int? PageSize { get; set; } = 20;
    }
}
