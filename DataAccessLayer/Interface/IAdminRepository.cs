using BusinessObjectLayer.Entities;
using BusinessObjectsLayer.Entities;

namespace DataAccessLayer.Interface
{
    public interface IAdminRepository
    {
        Task<dynamic> GetProfile(int UserId);
        Task<dynamic> GetSmsDetail(string? message, DateTime? startDate, DateTime? endDate, string? smsType, string? status, string? statusText, int? pageNumber = 1, int? pageSize = 10);
        Task<dynamic> GetDashboard(string? message, DateTime? startDate, DateTime? endDate, string? smsType, string? status, int? pageNumber = 1, int? pageSize = 10);

        Task<dynamic> GetContact(int? Id, string? SearchText, int? pageNumber = 1, int? pageSize = 10);
        Task<dynamic> GetFacilities(int? DoctorId, string? Text, int? PageNumber = 1, int? PageSize = 20);
        Task<List<SmsProviderModel>> GetProviderAccount(int? Id, int? ProviderId);
        Task<dynamic> GetNote(int? Id);
        Task<dynamic> GetUsersDetails(int? Id);
        Task<dynamic> InsertUpdateFacility(FacilityModel model);
        Task<dynamic> InsertUpdateDocument(DocumentModel model);
        Task<dynamic> GetApplicationByUser(int? UserId, int? FacilityId, string? Text, int? StatusId, int? PageNumber = 1, int? PageSize = 20);
        Task<dynamic> InsertUpdateRecentUpdate(RecentUpdateModel model);
        Task<dynamic> InsertUpdateApplication(ApplicationModel model);
        Task<dynamic> GetOgTable();
        Task<dynamic> GetApplicationStatus();
        Task<dynamic> GetFacilityByUser(int? UserId, string? Text, int? PageNumber = 1, int? PageSize = 20);
        Task<dynamic> GetFacilityInfo(int? FacilityId);
        Task<dynamic> GetDoctorDashboard(int? UserId, int? FacilityId, int? IsNullFacility);
        
        Task<dynamic> GetDoctorUser();
        Task<dynamic> DeleteApplication(int? Id);
        Task<dynamic> DeleteRecentUpdate(int? Id);
        Task<dynamic> ActiveRecentUpdate(int? Id, bool? IsActive);
        Task<dynamic> GetRecentUpdate(int? Id, string Text, int? PageNumber = 1, int? PageSize = 20);
        Task<dynamic> GetProfileFacility(int? UserId, int? FacilityId);
        Task<dynamic> GetDoctors();
        Task<dynamic> GetProvider_OG();
        Task<dynamic> GetProvider();

        Task<dynamic> GetUser_OG();

        Task<dynamic> GetSmsNumber();

        Task<dynamic> GetTemplates_OG();
        Task<dynamic> GetOwnNumber();
        Task<dynamic> GetUserById(int? UserId, int? UserTypeId, int? PageNumber, int? PageSize, string? SearchText = null);
        Task<dynamic> DeleteUser(int? Id);
        Task<dynamic> DeleteInsurance(int? Id);
        Task<dynamic> GetRole();
        Task<dynamic> GetUserName();
        Task<dynamic> GetAppFacilityByUser();
        Task<dynamic> ChangeSequence(int? Id, int? Sequence);
        Task<dynamic> InsertUpdateUser(UserDto user);
        Task<dynamic> InsertUpdateInsurance(InsuranceModel user);
        Task<dynamic> InsertUpdateNote(NoteModel user);
        Task<dynamic> InsertUpdateUsersDetails(UserDetailModel user);
        Task<dynamic> DeleteTableRow(string? tableName, int? Id);
        Task<dynamic> DeleteNumber(int? Id);

        Task<dynamic> GetOwnNumberList();
        Task<dynamic> GetSendNumber(int? Id);
        Task<dynamic> GetInsurance(int? Id, int? PageNumber = 1, int? PageSize = 20, string? SearchText = null);
        Task InsertSmsMessagesAsync(IEnumerable<SmsEntity> messages);
        Task<dynamic> SendSms(SmsModel model);
        Task<dynamic> ReportFile(ReportModel user);
        Task<dynamic> InsertUpdateCampaignSms(CampaignSmsModel model);

        Task<dynamic> GetUser(int? Id);
        Task<dynamic> InsertUpdateUsers(UserModel user);
        Task<dynamic> GetContactGroup(int? Id);
        Task<dynamic> Insert_Update_ProviderAccount(ProviderAccountModel user);
        Task<dynamic> InsertUpdateContactGroup(ContactGroupModel user);
        Task<dynamic> InsertUpdateOwnNumber(OwnNumber model);
        Task<dynamic> InsertUpdateBulkSms(BulkSmsModel model);
        Task<dynamic> GetContactDetails(string? To, string? Text, int? PageNumber = 1, int? PageSize = 20);
        Task<dynamic> GetCampaignContactDetail(int? CampaignId, int? PageNumber = 1, int? PageSize = 20);
        Task<dynamic> GetAllCampaigns(string? Text, int? PageNumber = 1, int? PageSize = 20);
        Task<dynamic> GetTemplate(int? Id);
        Task<dynamic> InsertUpdateTemplate(TemplateModel user);
        Task<dynamic> SentSMSContactDetail(int ProviderId, string From, string To, string Message, int AgentId);
        Task<dynamic> GetCampaigns(string? searchTerm = null);

        Task<dynamic> GetDashboard(int? Year, int? Month);

    }
}



