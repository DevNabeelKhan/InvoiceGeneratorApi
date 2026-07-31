using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjectsLayer.Entities
{
    public class SignalRNotification
    {
        public string? NotificationTitle { get; set; }
        public string? NotificationUser { get; set; }
        public int? NotificationCount { get; set; }
        public string? Url { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
