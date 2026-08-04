using System;

namespace BusinessObjectsLayer.Entities
{
    public class ProjectDocumentModel : BaseEntity
    {
        public int? ProjectId { get; set; }
        public string? DocumentTitle { get; set; }
        public string? Url { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
    }
}
