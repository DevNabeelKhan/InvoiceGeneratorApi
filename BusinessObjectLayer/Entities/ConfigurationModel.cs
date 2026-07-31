namespace BusinessObjectsLayer.Entities
{
    public class ConfigurationModel : BaseEntity
    {
        public string? TableName { get; set; }
        public string? Title { get; set; }
        public int? UserId { get; set; }

        public string? SearchText { get; set; }
        public int? PageNumber { get; set; } = 1;
        public int? PageSize { get; set; } = 20;
    }
}
