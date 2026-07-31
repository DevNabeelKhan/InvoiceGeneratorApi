using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BusinessObjectsLayer.Entities
{
    public class BaseEntity
    {
        public int? Id { get; set; }
        [JsonIgnore]
        public DateTime CreatedOn { get; set; }
        [JsonIgnore]
        public int? CreatedBy { get; set; }
        //[JsonIgnore]
        //public string? CreatedBy { get; set; }
        //[JsonIgnore]
        public DateTime? UpdatedOn { get; set; }
        [JsonIgnore]
        public int? UpdatedBy { get; set; }
        //[JsonIgnore]
        //public string? ModifiedBy { get; set; }
        //[JsonIgnore]
        public bool IsActive { get; set; } = true;
    }
}
