namespace DataAccessLayer.Shared
{
    // Whitelist of lookup tables that share the common (Id, Title, IsActive, CreatedDate,
    // CreatedBy, UpdatedDate, UpdatedBy) schema and are served by the generic Configuration
    // API. Kept in sync with the corresponding list in the GetConfigurationList /
    // InsertUpdateConfiguration / DeleteConfiguration stored procedures.
    public static class ConfigurationTables
    {
        public static readonly HashSet<string> Allowed = new(StringComparer.OrdinalIgnoreCase)
        {
            "AccountType",
            "BankFeesType",
            "CashFlowType",
            "ContactType",
            "CostCenter",
            "Industry",
            "InvoicingRelationShip",
            "PaymentTerm",
            "RevenueTaxRateType",
            "RevenueRecognitionType",
            "Role",
            "UnitOfMeasure",
            "ProductStatus"
        };

        public static bool IsValid(string? tableName)
        {
            return !string.IsNullOrWhiteSpace(tableName) && Allowed.Contains(tableName);
        }
    }
}
