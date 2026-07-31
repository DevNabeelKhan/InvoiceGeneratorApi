using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjectsLayer.Entities
{
    public class ResponseModel
    {
        public bool Response { get; set; }
        public string Message { get; set; } 
        public int StatusCode { get; set; }
        public object Data { get; set; }
        public bool IsSuccess { get; set; } = true;
    }

    public class UsptoLeadModel
    {
        public string? SearchString { get; set; }
        public int? SearchType { get; set; }
        public string? FilingDateFrom { get; set; }
        public string? FilingDateTo { get; set; }
        public string? AbandonedDateFrom { get; set; }
        public string? AbandonedDateTo { get; set; }
        public int? IsAssign { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }
    public class OwnFileSerialNumber
    { 
        public int? Id { get; set; }
        public string? SerialNumber { get; set; }
        public string? OrderId { get; set; }
        public string? Type { get; set; }
        public int? FilingTypeId { get; set; }
        
        public string? Json { get; set; }
    }
    public class OwnLeadModel
    {
        public int? Id { get; set; }
        public string? SerialNumber { get; set; }
        public string? ApplicationStatus { get; set; }
        public string? LastStatus { get; set; }
        public DateTime? StartAppDate { get; set; }
        public DateTime? EndAppDate { get; set; }
        public DateTime? StartUpdatedDate { get; set; }
        public DateTime? EndUpdatedDate { get; set; }
        public int? FilingTypeId { get; set; }
        public int? FetchStatusId { get; set; }
        public int? IsAllFetch { get; set; } = null;
        
        public int? PageNumber { get; set; } = 1;
        public int? PageSize { get; set; } = 100;
    }

}
