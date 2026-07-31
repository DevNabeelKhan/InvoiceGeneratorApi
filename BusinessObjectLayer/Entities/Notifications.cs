using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BusinessObjectsLayer.Entities
{
    public class NotificationDetail : BaseEntity
    {
        [Required(ErrorMessage = "Audience Type Id is required.")]
        public int? AudienceTypeId { get; set; }

        [Required(ErrorMessage = "Notification Type Id is required.")]
        public int? NotificationTypeId { get; set; }

        [StringLength(200, ErrorMessage = "Notification Title cannot exceed 200 characters.")]
        public string? NotificationTitle { get; set; }

        [StringLength(1000, ErrorMessage = "Summary cannot exceed 1000 characters.")]
        public string? Summary { get; set; }
        public string? FileUrl { get; set; }

        [Required(ErrorMessage = "Status Id is required.")]
        public int? StatusId { get; set; }

        public DateTime? ScheduledDate { get; set; }

        public string? ScheduledTime { get; set; }

        [Required(ErrorMessage = "Batche is required.")]
        public virtual List<int>? BatchIds { get; set; } = new List<int>();
        public int? EntityId { get; set; }
    }

    public class AudienceType
    {
        public int? Id { get; set; }
        public string? Type { get; set; }
    }
    public class NotificationType
    {
        public int Id { get; set; }
        public string? Type { get; set; }
    }
    public class NotificationStatus
    {
        public int Id { get; set; }
        public string? Status { get; set; }
    }
    /* public class NotificationDto
     {
         public int? Id { get; set; }
         public int AudienceTypeId { get; set; }
         public int NotificationTypeId { get; set; }
         public string? NotificationTitle { get; set; }
         public string? Summary { get; set; }
         public string? FileUrl { get; set; }
         public int? StatusId { get; set; }
         public DateTime? ScheduledDate { get; set; }
         public string? ScheduledTime { get; set; }
         public virtual List<int>? BatchIds { get; set; }
         public int? EntityId { get; set; }
         public string? Individual { get; set; }
         public virtual int? IndividualId { get; set; }
         public DateTime? CreatedOn { get; set; }
         public int? CreatedBy { get; set; }
         public DateTime? ModifiedOn { get; set; }
         public int? ModifiedBy { get; set; }
         public string? AudienceType { get; set; }
         public string? NotificationType { get; set; }
         public string? IconUrl { get; set; }
         public int? TotalRecords { get; set; }

         public string? Topic { get; set; }

     }*/
    public class NotificationDto
    {
        public string? Topic { get; set; }
        public string? NotificationTitle { get; set; }
        public string? Summary { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool IsRead { get; set; }
        public int? NotificationTypeId { get; set; }
        public string? IconUrl { get; set; }
        public int? Id { get; set; }
        public string? NotificationType { get; set; }
    }

    public class GetNotificationDetail
    {
        public int? Id { get; set; }
        public string? NotificationTitle { get; set; }
        public string? Summary { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool IsRead { get; set; }
        public string? IconUrl { get; set; }
        public string? NotificationType { get; set; }
        public int TotalRecords { get; set; }
    }

    public class GetAdminNotification
    {
        public int? Id { get; set; }
        public string? IconUrl { get; set; }
        public string? NotificationTitle { get; set; }
        public string? Summary { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? NotificationType { get; set; }
        public string? AudienceType { get; set; }
        public string? FileUrl { get; set; }
        public int TotalRecords { get; set; }
    }

    public class GetNotificationById
    {
        public int? Id { get; set; }
        public string? NotificationTitle { get; set; }
        public string? Summary { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? NotificationTypeId { get; set; }
        public int? AudienceTypeId { get; set; }
        public List<int>? BatchIds { get; set; } = new List<int>();
        public string? StudentName { get; set; }
        public string? FacultyName { get; set; }
        public int? EntityId { get; set; }
        public string? FileUrl { get; set; }

    }
}
