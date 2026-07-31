using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjectsLayer.Entities
{
    public class User : BaseEntity
    { 
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }  
        public string RoleTitle { get; set; }
        public string Token { get; set; }
        public DateTime Expiry { get; set; }
        public string PictureUrl { get; set; }
    }
    public class UserDto:BaseEntity
    {
        public string? FullName { get; set; } 
        public int? RoleId { get; set; }
        public int? CompanyId { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? UserName { get; set; }
        public string? PictureUrl { get; set; }
    }
    public class InsuranceModel : BaseEntity
    {
        public string? Title { get; set; } 

    }
    public class NoteModel : BaseEntity
    {
        public string? Title { get; set; } 
        public string? Text { get; set; } 

    }
    public class ProviderAccountModel : BaseEntity
    { 
        
        public int? ProviderID { get; set; } 
        public string? Number { get; set; } 
        public string? LoginID { get; set; } 
        public string? LoginPassword { get; set; } 
        public string? ApiKey { get; set; } 
        public string? APISecret { get; set; }

    }
    public class UserDetailModel : BaseEntity
    {
        public string? IP { get; set; } 
        public string? Name { get; set; } 
        public string? Password { get; set; } 
        public string? Comment { get; set; } 

    }
    public class UserModel : BaseEntity
    {
       
        public string? FullName { get; set; } 
        public string? UserName { get; set; } 
        public string? Password { get; set; } 
        public int? RoleId { get; set; } 

    }
    public class SmsModel  
    {
        public int? Id { get; set; }
        public int? ProviderId { get; set; }
        public string? From { get; set; }
        public string? To { get; set; }
        public string? Message { get; set; }
        public int? AgentId { get; set; }

    }

    public class ReportModel
    {
        public int? Id { get; set; }
       
        public string? CampaignName { get; set; }
        public int? CampaignType { get; set; }
        public int? TotalAutoReply { get; set; }

    }
    public class ContactGroupModel : BaseEntity
    { 
        public string? GroupName { get; set; } 

    }

    public class Owndetail
    {

        public int? Id { get; set; }
        public string? Number { get; set; }

        public bool? isActive { get; set; }
        public string? Title { get; set; }

    }

    public class TemplateModel : BaseEntity
    {
        public string? Title { get; set; }
        public string? Text { get; set; }
        public bool? Is3cx { get; set; }
        public bool? IsAutoReply { get; set; }

    }

}
