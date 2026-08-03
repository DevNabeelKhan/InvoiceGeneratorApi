using System;

namespace BusinessObjectsLayer.Entities
{
    public class InvoiceAttachmentModel : BaseEntity
    {
        public int? InvoiceId { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public long? FileSize { get; set; }
        public string? ContentType { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
    }
}
