using System;

namespace BusinessObjectsLayer.Entities
{
    public class CustomerModel : BaseEntity
    {
        public string? CustomerName { get; set; }
        public int? CountryId { get; set; }
        public string? TaxRegistrationNumber { get; set; }
        public string? City { get; set; }
        public string? StreetAddress { get; set; }
        public string? BuildingNumber { get; set; }
        public string? District { get; set; }
        public string? AddressAdditionalNumber { get; set; }
        public string? PostalCode { get; set; }
        public string? InvoicingCode { get; set; }
        public string? InvoicingEmail { get; set; }
        public string? InvoicingPhone { get; set; }
        public int? InvoicingRelationShipId { get; set; }
        public int? PaymentTermId { get; set; }
        public int? ContactTypeID { get; set; }
        public string? ContactTypeNumber { get; set; }
        public int? SellingRevenueAccountId { get; set; }
        public int? SellingRevenueCostCenterId { get; set; }
        public int? SellingRevenueTaxRateId { get; set; }
        public int? UserId { get; set; }

        public string? SearchText { get; set; }
        public int? PageNumber { get; set; } = 1;
        public int? PageSize { get; set; } = 20;
    }
}
