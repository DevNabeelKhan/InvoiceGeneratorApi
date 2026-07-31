using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjectLayer.Entities
{
    public class SmsEntity
    {
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public DateTime DateTime { get; set; }
        // Add other properties as per your table schema
    }
   public class CampaignSmsModel
    {
        public string? ContactJson { get; set; }
        public string? CampaignTemplateJson { get; set; }
        public string? KeywordJson { get; set; } 
        public string? GroupName { get; set; }
        public string? OwnReplyJson { get; set; }
        public string? CampaignName { get; set; }
        public string? Agent { get; set; }
        public string? BrandName { get; set; }
        public string? ParalegalName { get; set; }
        public string? CustomerBrand { get; set; }
        public int? CampaignType { get; set; }
        public bool? IsAutoReply { get; set; }
        public bool? Is3cxReply { get; set; }
        public int? Id { get; set; }
        public int? ProviderId { get; set; }
        public int? CampaignId { get; set; }
        public int? AgentId { get; set; }
        // Add other properties as per your table schema

    }
    public class ContactJson
    {
        public string? Name { get; set; }
        public string? Number { get; set; }
        public string? SerialNumber { get; set; }
        public string? CustomerBrand { get; set; }
        public bool? IsOptOut { get; set; }
    }
    public class BulkSmsModel
    {
        public string? ContactJson { get; set; }
        public string? Message { get; set; }
        public string? KeywordJson { get; set; }
        public string? GroupName { get; set; }
        public string? OwnReplyJson { get; set; }
        public string? CampaignName { get; set; }
        public string? Agent { get; set; }
        public string? BrandName { get; set; }
        public string? ParalegalName { get; set; }
        public string? CustomerBrand { get; set; }
        public int? CampaignType { get; set; }
        public bool? IsAutoReply { get; set; }
        public bool? Is3cxReply { get; set; }
        public int? Id { get; set; }
        public int? ProviderId { get; set; }
        public int? CampaignId { get; set; }
        public int? AgentId { get; set; }
        // Add other properties as per your table schema

    }

    public class ContactDetailModel
    {
        public int? Id { get; set; }
        public DateTime DateTime { get; set; }
        public string? From	 { get; set; }
        public string? To	 { get; set; }
        public string? Message	 { get; set; }
        public string? SMSType	 { get; set; }
        public string? Status { get; set; }
      
    }

    public class SentSMSContactDetailModel
    {
        public int ProviderId { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Message { get; set; }
        public int AgentId { get; set; }
    }



    public class OwnNumber
    {

        public int? Id { get; set; }
        public string? Number { get; set; }
            
        public string? Title  { get; set; }

    }

    //public class Owndetail
    //{

    //    public int? Id { get; set; }
    //    public string? Number { get; set; }

    //    public bool? isActive { get; set; }
    //    public string? Title { get; set; }

    //}


    public class AllCampaignsModel
    {
        public int? Id{ get; set; }
        public string? CampaignName { get; set; }
        public string? GroupName{ get; set; }
        public int? CampaignType{ get; set; }
        public int? TotalContact{ get; set; }
        public bool? IsAutoReply{ get; set; }
        public bool? Is3cx{ get; set; }
        public int? ProviderId{ get; set; }
        public string? Agent{ get; set; }
        public string? BrandName{ get; set; }
        public string? CustomerBrand{ get; set; }
        public string? ParalegalName{ get; set; }
        public bool? IsActive{ get; set; }
        public int? TotalRecords { get; set; }

    }


}

