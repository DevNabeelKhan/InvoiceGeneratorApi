using System;

namespace BusinessObjectsLayer.Entities
{
    public class CompanyModel : BaseEntity
    {
        public string? Name { get; set; }
        public string? ArabicName { get; set; }
        public string? Address { get; set; }
        public string? ArabicAddress { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Website { get; set; }
        public string? VATNumber { get; set; }
        public string? LogoPath { get; set; }
        public string? StampPath { get; set; }
        public string? BankName { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? IBAN { get; set; }
        public string? SwiftCode { get; set; }
        public string? AccountCurrency { get; set; }
        public string? BeneficiaryName { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public int? UserId { get; set; }

        // Search/paging helpers (not DB columns)
        public string? SearchText { get; set; }
        public int? PageNumber { get; set; } = 1;
        public int? PageSize { get; set; } = 20;
    }
}
