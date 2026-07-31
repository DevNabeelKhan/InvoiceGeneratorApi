using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjectsLayer.Entities
{
    public class FacilityModel 
    {
        public Facility? Facility { get; set; } 
        public PointOfContactModel? PointOfContact { get; set; } 
        public ProviderModel? Provider { get; set; } 
        public List<LicenseModel>? License { get; set; }
        public List<PortalModel>? Portal { get; set; } 
    }
    public class Facility 
    {
        public int? Id { get; set; }
        public int? UserId { get; set; }
        public string? FacilityName { get; set; }
        public string? NameonIRS { get; set; }
        public string? TaxID { get; set; }
        public string? GroupNPI { get; set; }
        public string? PracticingAddress { get; set; }
        public string? MailingAddress { get; set; }
        public string? Availablehours { get; set; }
        public string? Medicare { get; set; }
        public string? MedicalId { get; set; }
        public int? UserBy { get; set; }
    }


    public class PointOfContactModel
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Title { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Fax { get; set; }
        public bool? IsActive { get; set; }
        public int? FacilityId { get; set; }
        public int? UserBy { get; set; }
    }
    public class ProviderModel
    {
        public int? Id { get; set; } 
        public int? FacilityId { get; set; }
        public string? ProviderName { get; set; }
        public string? Title { get; set; }
        public string? IndividualNPI { get; set; }
        public DateTime? DOB { get; set; }
        public string? SSN { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? PrimarySpecialty { get; set; }
        public string? PcpSpecialty { get; set; }
        public bool? IsActive { get; set; }
        public int? UserBy { get; set; }
    }

    public class LicenseModel
    {
        public int? Id { get; set; }
        public int? LicenseTypeId{ get; set; }
        public string? Number { get; set; }
        public DateTime?  Effective { get; set; }
        public DateTime?  Expiry { get; set; } 
        public bool? IsActive { get; set; }
        public int? FacilityId { get; set; } 
        public int? UserBy { get; set; }
    }
    public class PortalModel
    {
        public int? Id { get; set; }
        public int? PortalTypeId { get; set; }
        public string? UserName { get; set; }
        public string? TypeValue { get; set; } 
        public string? Password { get; set; }
        public int? FacilityId { get; set; }
        public int? UserBy { get; set; }
    }
  
    public class DocumentModel
    { 
        public int? Id { get; set; } 
        public int? DocumentTypeId { get; set; } 
        public string? Url { get; set; } 
        public IFormFile? FileUpload { get; set; } 
        public int? FacilityId { get; set; } 
        public int? UserBy { get; set; }
    }

    public class RecentUpdateModel
    {
        public int? Id { get; set; }
        public DateTime? Date { get; set; }
        public IFormFile? FileUpload { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public bool? IsActive { get; set; }
        public int? UserBy { get; set; }
        public int? Sequence { get; set; }
    }
    public class ApplicationModel
    {
        public int? Id { get; set; } 
        public int? FacilityId { get; set; } 
        public int? ApplicationStatusId { get; set; }
        public string? NameOfInsurance { get; set; }
        public int? InsuranceId { get; set; }
        public DateTime? SubmissionDate { get; set; }
        public string? RemarkComment { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Fax { get; set; }
        public bool? IsActive { get; set; }
        public int? UserBy { get; set; }
    }
    public class SmsProviderModel
    {
        public int ID { get; set; }
        public int ProviderId { get; set; }
        public string Provider { get; set; }
        public string Number { get; set; }
        public string LoginID { get; set; }
        public string LoginPassword { get; set; }
        public string ApiKey { get; set; }
        public string APISecret { get; set; }
        public DateTime LastOutgoingUpdatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }

}
