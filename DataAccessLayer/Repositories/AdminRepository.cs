using Amazon.S3;
using BusinessObjectLayer.Entities;
using BusinessObjectsLayer.Entities;
using Dapper;
using DataAccess.DbContext;
using DataAccessLayer.Interface;
using DataAccessLayer.Shared.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Dynamic;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace DataAccessLayer.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly DapperContext _context;
        private readonly HttpContextAccessor _httpContextAccessor;
        public AdminRepository(HttpContextAccessor httpContextAccessor, DapperContext context)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<dynamic> GetProfile(int UserId)
        {
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new
                {
                    UserId = UserId
                };
                using var multi = await con.QueryMultipleAsync("GetProfile", param: parameters, commandType: CommandType.StoredProcedure);
                var Facilities = (await multi.ReadAsync<dynamic>()).ToList();
                var Poc = (await multi.ReadAsync<dynamic>()).FirstOrDefault();
                var Providers = (await multi.ReadAsync<dynamic>()).ToList();
                var License = (await multi.ReadAsync<dynamic>()).ToList();
                var PortalLogin = (await multi.ReadAsync<dynamic>()).ToList();

                return new { Facilities, Poc, Providers, License, PortalLogin };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return (null);
        }

        public async Task<dynamic> GetFacilities(int? DoctorId, string? Text, int? PageNumber = 1, int? PageSize = 20)
        {
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new
                {
                    DoctorId = DoctorId,
                    Text = Text,
                    UserId = Helper.UserId(_httpContextAccessor),
                    PageNumber = PageNumber,
                    PageSize = PageSize
                };
                return await con.QueryAsync("GetFacilities", param: parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<List<SmsProviderModel>> GetProviderAccount(int? Id, int? ProviderId)
        {
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new
                {
                    Id = Id,
                    ProviderId = ProviderId
                };
                return (await con.QueryAsync<SmsProviderModel>("GetProviderAccount", param: parameters, commandType: CommandType.StoredProcedure)).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }
        public async Task<dynamic> GetNote(int? Id)
        {
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new
                {
                    Id = Id
                };
                return await con.QueryAsync("GetScreenData", param: parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }
        public async Task<dynamic> GetUsersDetails(int? Id)
        {
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new
                {
                    Id = Id
                };
                return await con.QueryAsync("GetUsersDetails", param: parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }
        public async Task<dynamic> InsertUpdateFacility(FacilityModel model)
        {
            try
            {
                using var con = _context.CreateConnection();
                if (model.Facility != null)
                {
                    model.Facility.UserBy = Helper.UserId(_httpContextAccessor);
                    model.Facility.Id = (await con.QueryAsync<int>("InsertUpdateFacility", model.Facility, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                }
                if (model.PointOfContact != null)
                {
                    model.PointOfContact.UserBy = Helper.UserId(_httpContextAccessor);
                    model.PointOfContact.Id = (await con.QueryAsync<int>("InsertUpdatePointOfContact", model.PointOfContact, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                }
                if (model.Provider != null)
                {
                    model.Provider.UserBy = Helper.UserId(_httpContextAccessor);
                    model.Provider.Id = (await con.QueryAsync<int>("InsertUpdateProvider", model.Provider, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                }

                if (model.License?.Count() > 0)
                {
                    foreach (var item in model.License)
                    {
                        item.UserBy = Helper.UserId(_httpContextAccessor);
                        item.Id = (await con.QueryAsync<int>("InsertUpdateLicense", item, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                    }
                }

                if (model.Portal?.Count() > 0)
                {
                    foreach (var item in model.Portal)
                    {
                        item.UserBy = Helper.UserId(_httpContextAccessor);
                        item.Id = (await con.QueryAsync<int>("InsertUpdatePortal", item, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                    }
                }
                //foreach (var item in model.Document)
                //{
                //    item.FacilityId = model.Facility.Id;
                //    item.Id = (await con.QueryAsync<int>("InsertUpdateDocument", item, commandType: CommandType.StoredProcedure)).FirstOrDefault();

                //}
                return model;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return (null);
        }

        public async Task<dynamic> InsertUpdateDocument(DocumentModel model)
        {
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new
                {
                    UserBy = Helper.UserId(_httpContextAccessor),
                    Url = model.Url,
                    DocumentTypeId = model.DocumentTypeId,
                    FacilityId = model.FacilityId,
                };
                return await con.QueryAsync("InsertUpdateDocument", param: parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<dynamic> InsertUpdateRecentUpdate(RecentUpdateModel model)
        {
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new
                {
                    Id = model.Id,
                    Date = model.Date,
                    Title = model.Title,
                    Description = model.Description,
                    Image = model.Image,
                    Sequence = model.Sequence,
                    UserBy = Helper.UserId(_httpContextAccessor)
                };
                return await con.QueryAsync("InsertUpdateRecentUpdate", param: parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<dynamic> InsertUpdateApplication(ApplicationModel model)
        {
            try
            {
                using var con = _context.CreateConnection();
                model.UserBy = Helper.UserId(_httpContextAccessor);
                return await con.QueryAsync("InsertUpdateApplication", model, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }
        public async Task<dynamic> GetOgTable()
        {
            try
            {
                using var con = _context.CreateConnection();
                using var multi = await con.QueryMultipleAsync("GetOgTable", new { UserId = Helper.UserId(_httpContextAccessor) }, commandType: CommandType.StoredProcedure);
                var PortalType = (await multi.ReadAsync<dynamic>()).ToList();
                var DocumentType = (await multi.ReadAsync<dynamic>()).ToList();
                var LicenseType = (await multi.ReadAsync<dynamic>()).ToList();
                return new { PortalType, DocumentType, LicenseType };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<dynamic> GetApplicationStatus()
        {
            try
            {
                using var con = _context.CreateConnection();
                return (await con.QueryAsync("GetApplicationStatus", new { UserId = Helper.UserId(_httpContextAccessor) }, commandType: CommandType.StoredProcedure)).ToList();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }




        public async Task<dynamic> GetFacilityByUser(int? UserId, string? Text, int? PageNumber = 1, int? PageSize = 20)
        {
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new
                {
                    UserId = UserId,
                    Text = Text,
                    PageNumber = PageNumber,
                    PageSize = PageSize
                };
                return await con.QueryAsync("GetFacilityByUser", param: parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }
        public async Task<dynamic> GetFacilityInfo(int? FacilityId)
        {
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new
                {
                    FacilityId = FacilityId,
                    UserId = Helper.UserId(_httpContextAccessor)
                };
                using var multi = await con.QueryMultipleAsync("GetFacilityInfo", param: parameters, commandType: CommandType.StoredProcedure);
                var Facilities = (await multi.ReadAsync<dynamic>()).FirstOrDefault();
                var Poc = (await multi.ReadAsync<dynamic>()).FirstOrDefault();
                var Providers = (await multi.ReadAsync<dynamic>()).FirstOrDefault();
                var License = (await multi.ReadAsync<dynamic>()).ToList();
                var PortalLogin = (await multi.ReadAsync<dynamic>()).ToList();
                var Document = (await multi.ReadAsync<dynamic>()).ToList();

                return new { Facilities, Poc, Providers, License, PortalLogin, Document };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return (null);
        }
        public async Task<dynamic> GetApplicationByUser(int? UserId, int? FacilityId, string? Text, int? StatusId, int? PageNumber = 1, int? PageSize = 20)
        {
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new
                {
                    UserId = UserId,
                    FacilityId = FacilityId,
                    Text = Text,
                    StatusId = StatusId,
                    PageNumber = PageNumber,
                    PageSize = PageSize
                };
                using var multi = await con.QueryMultipleAsync("GetApplicationByUser", param: parameters, commandType: CommandType.StoredProcedure);
                var Count = (await multi.ReadAsync<dynamic>()).ToList();
                var Application = (await multi.ReadAsync<dynamic>()).ToList();

                return new { Count, Application };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return (null);
        }
        public async Task<dynamic> GetDoctorDashboard(int? UserId, int? FacilityId, int? IsNullFacility)
        {
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new
                {
                    UserId = UserId,
                    FacilityId = FacilityId,
                    IsNullFacility = IsNullFacility
                };
                using var multi = await con.QueryMultipleAsync("GetDoctorDashboard", param: parameters, commandType: CommandType.StoredProcedure);
                var Facility = (await multi.ReadAsync<dynamic>()).FirstOrDefault();
                var Provider = (await multi.ReadAsync<dynamic>()).FirstOrDefault();
                var RecentUpdate = new List<dynamic>();
                if (IsNullFacility == null)
                {
                    RecentUpdate = (await multi.ReadAsync<dynamic>()).ToList();
                }
                return new { Facility, Provider, RecentUpdate };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return (null);
        }
        public async Task<dynamic> GetDoctorUser()
        {
            try
            {
                using var con = _context.CreateConnection();

                return await con.QueryAsync("GetDoctorUser", new { UserId = Helper.UserId(_httpContextAccessor) }, commandType: CommandType.StoredProcedure);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return (null);
        }
        public async Task<dynamic> DeleteApplication(int? Id)
        {
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new
                {
                    UserId = Helper.UserId(_httpContextAccessor),
                    Id = Id
                };
                return await con.QueryAsync("DeleteApplication", param: parameters, commandType: CommandType.StoredProcedure);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return (null);
        }
        public async Task<dynamic> DeleteRecentUpdate(int? Id)
        {
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new
                {
                    UserId = Helper.UserId(_httpContextAccessor),
                    Id = Id
                };
                return await con.QueryAsync("DeleteRecentUpdate", param: parameters, commandType: CommandType.StoredProcedure);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return (null);
        }
        public async Task<dynamic> ActiveRecentUpdate(int? Id, bool? IsActive)
        {
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new
                {
                    UserId = Helper.UserId(_httpContextAccessor),
                    Id = Id,
                    IsActive = IsActive
                };
                return await con.QueryAsync("ActiveRecentUpdate", param: parameters, commandType: CommandType.StoredProcedure);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return (null);
        }
        public async Task<dynamic> GetProfileFacility(int? UserId, int? FacilityId)
        {

            try
            {
                var param = new
                {
                    UserId = UserId,
                    FacilityId = FacilityId
                };
                using var connection = _context.CreateConnection();
                return (await connection.QueryAsync<dynamic>("GetProfileFacility", param: param)).ToList();


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return (null);
        }
        public async Task<dynamic> GetDoctors()
        {

            try
            {
                using var con = _context.CreateConnection();
                return await con.QueryAsync("GetDoctors", new { UserId = Helper.UserId(_httpContextAccessor) }, commandType: CommandType.StoredProcedure);


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return (null);
        }

        public async Task<dynamic> GetProvider_OG()
        {

            try
            {
                using var con = _context.CreateConnection();
                return await con.QueryAsync("GetProvider_OG", commandType: CommandType.StoredProcedure);


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return (null);
        }

        public async Task<dynamic> GetProvider()
        {

            try
            {
                using var con = _context.CreateConnection();
                return await con.QueryAsync("GetProvider", commandType: CommandType.StoredProcedure);


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return (null);
        }

        public async Task<dynamic> GetSmsNumber()
        {

            try
            {
                using var con = _context.CreateConnection();
                return await con.QueryAsync("GetSmsNumber", commandType: CommandType.StoredProcedure);


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return (null);
        }


        public async Task<dynamic> GetTemplates_OG()
        {

            try
            {
                using var con = _context.CreateConnection();
                return await con.QueryAsync("GetTemplates_OG", commandType: CommandType.StoredProcedure);


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return (null);
        }



        public async Task<dynamic> GetOwnNumberList()
        {

            try
            {
                using var con = _context.CreateConnection();
                return await con.QueryAsync("GetOwnNumberList", commandType: CommandType.StoredProcedure);


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return (null);
        }

        public async Task<dynamic> GetOwnNumber()
        {

            try
            {
                using var con = _context.CreateConnection();
                return await con.QueryAsync("GetOwnNumber", commandType: CommandType.StoredProcedure);


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return (null);
        }




        public async Task<dynamic> GetRecentUpdate(int? Id, string Text, int? PageNumber = 1, int? PageSize = 20)
        {
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new
                {
                    Id = Id,
                    Text = Text,
                    UserId = Helper.UserId(_httpContextAccessor),
                    PageNumber = PageNumber,
                    PageSize = PageSize
                };
                return await con.QueryAsync("GetRecentUpdate", param: parameters, commandType: CommandType.StoredProcedure);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return (null);
        }

        public async Task<dynamic> GetUserById(int? UserId, int? RoleId, int? PageNumber, int? PageSize, string? SearchText = null)
        {
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new
                {
                    UserId = UserId,
                    RoleId = RoleId,
                    PageNumber = PageNumber,
                    PageSize = PageSize,
                    SearchText = SearchText
                };
                var res = (await con.QueryAsync<dynamic>("GetUsers", param: parameters, commandType: CommandType.StoredProcedure)).ToList();
                return res;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return (null);
        }
        public async Task<dynamic> GetInsurance(int? Id, int? PageNumber = 1, int? PageSize = 20, string? SearchText = null)
        {
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new
                {
                    Id = Id,
                    UserId = Helper.UserId(_httpContextAccessor),
                    PageNumber = PageNumber,
                    PageSize = PageSize,
                    SearchText = SearchText
                };
                var res = (await con.QueryAsync<dynamic>("GetInsurance", param: parameters, commandType: CommandType.StoredProcedure)).ToList();
                return res;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return (null);
        }

        public async Task<dynamic> DeleteUser(int? Id)
        {
            var result = new Result();
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var parameters = new
                    {
                        UserId = Helper.UserId(_httpContextAccessor),
                        Id = Id
                    };
                    return (await connection.QueryAsync<dynamic>("DeleteUser", param: parameters, commandType: CommandType.StoredProcedure)).ToList();


                }

            }
            catch (Exception ex)
            {
                result.Data = null;
            }
            return result;
        }
        public async Task<dynamic> InsertUpdateUser(UserDto user)
        {
            try
            {
                using (IDbConnection con = _context.CreateConnection())
                {
                    var parameters = new
                    {
                        Id = user.Id,
                        FullName = user.FullName,
                        Email = user.Email,
                        RoleId = user.RoleId,
                        Password = user.Password,
                        UserName = user.UserName,
                        PictureUrl = user.PictureUrl,
                        UserId = Helper.UserId(_httpContextAccessor)
                    };

                    var resp = (await con.QueryAsync<dynamic>("InsertUpdateUser", parameters, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                    user.Id = resp.Id;
                    return user;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<dynamic> GetRole()
        {
            var result = new Result();
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var res = (await connection.QueryAsync<dynamic>("GetRole", new { UserId = Helper.UserId(_httpContextAccessor) }, commandType: CommandType.StoredProcedure)).ToList();
                    return res;
                }

            }
            catch (Exception ex)
            {
                result.Data = null;
            }
            return result;
        }
        public async Task<dynamic> ChangeSequence(int? Id, int? Sequence)
        {
            var result = new Result();
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var parameters = new
                    {
                        Id = Id,
                        UserId = Helper.UserId(_httpContextAccessor),
                        Sequence = Sequence
                    };
                    var res = (await connection.QueryAsync<dynamic>("ChangeSequence", param: parameters, commandType: CommandType.StoredProcedure)).ToList();
                    return res;
                }

            }
            catch (Exception ex)
            {
                result.Data = null;
            }
            return result;
        }
        public async Task<dynamic> DeleteInsurance(int? Id)
        {
            var result = new Result();
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var parameters = new
                    {
                        UserId = Helper.UserId(_httpContextAccessor),
                        Id = Id
                    };
                    return (await connection.QueryAsync<dynamic>("DeleteInsurance", param: parameters, commandType: CommandType.StoredProcedure)).ToList();


                }

            }
            catch (Exception ex)
            {
                result.Data = null;
            }
            return result;
        }
        public async Task<dynamic> InsertUpdateInsurance(InsuranceModel user)
        {
            try
            {
                using (IDbConnection con = _context.CreateConnection())
                {
                    var parameters = new
                    {
                        Id = user.Id,
                        Title = user.Title,
                        UserBy = Helper.UserId(_httpContextAccessor)
                    };

                    var resp = (await con.QueryAsync<dynamic>("InsertUpdateInsurance", parameters, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                    user.Id = resp.Id;
                    return user;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<dynamic> InsertUpdateNote(NoteModel user)
        {
            try
            {
                using (IDbConnection con = _context.CreateConnection())
                {
                    var parameters = new
                    {
                        Id = user.Id,
                        Title = user.Title,
                        Text = user.Text,
                        UserId = Helper.UserId(_httpContextAccessor)
                    };

                    var resp = (await con.QueryAsync<dynamic>("InsertUpdateNote", parameters, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                    user.Id = resp.Id;
                    return user;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<dynamic> InsertUpdateUsersDetails(UserDetailModel user)
        {
            try
            {
                using (IDbConnection con = _context.CreateConnection())
                {
                    var parameters = new
                    {
                        Id = user.Id,
                        IP = user.IP,
                        Name = user.Name,
                        Password = user.Password,
                        Comment = user.Comment,
                        UserId = Helper.UserId(_httpContextAccessor)
                    };

                    var resp = (await con.QueryAsync<dynamic>("InsertUpdateUsersDetails", parameters, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                    user.Id = resp.Id;
                    return user;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<dynamic> DeleteTableRow(string TableName, int? Id)
        {
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new
                {
                    tableName = TableName,
                    Id = Id
                };
                return (await con.QueryAsync("DeleteTableRow", param: parameters, commandType: CommandType.StoredProcedure)).ToList();
            }
            catch (Exception ex)
            {
                return (null);
            }

        }


        public async Task<dynamic> DeleteNumber(int? Id)
        {
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new
                {

                    Id = Id
                };
                return (await con.QueryAsync("sp_Delete_OwnNumber", param: parameters, commandType: CommandType.StoredProcedure)).ToList();
            }
            catch (Exception ex)
            {
                return (null);
            }

        }

        public async Task<dynamic> GetSendNumber(int? Id)
        {
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new
                {
                    Id = Id
                };
                return (await con.QueryAsync("ProviderTable", param: parameters, commandType: CommandType.StoredProcedure)).ToList();
            }
            catch (Exception ex)
            {
                return (null);
            }

        }




        public async Task<dynamic> GetUserName()
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    return (await connection.QueryAsync<dynamic>("GetUserName", new { UserId = Helper.UserId(_httpContextAccessor) }, commandType: CommandType.StoredProcedure)).ToList();
                }

            }
            catch (Exception ex)
            {
                return ex;
            }
        }
        public async Task<dynamic> GetAppFacilityByUser()
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    return (await connection.QueryAsync<dynamic>("GetAppFacilityByUser", new { UserId = Helper.UserId(_httpContextAccessor) }, commandType: CommandType.StoredProcedure)).ToList();
                }

            }
            catch (Exception ex)
            {
                return ex;
            }
        }
        public async Task<dynamic> SendSms(SmsModel model)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    return (await connection.QueryAsync<dynamic>("InsertUpdateSendSms", model, commandType: CommandType.StoredProcedure)).ToList();
                }

            }
            catch (Exception ex)
            {
                return ex;
            }
        }
        public async Task<dynamic> ReportFile(ReportModel model)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    return (await connection.QueryAsync<dynamic>("Insert_Campaign", model, commandType: CommandType.StoredProcedure)).ToList();
                }

            }
            catch (Exception ex)
            {
                return ex;
            }
        }



        public async Task<dynamic> GetDashboard(int? Year, int? Month)
        {
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new
                {
                    UserId = Helper.UserId(_httpContextAccessor),
                    Year = Year,
                    Month = Month
                };
                using var multi = await con.QueryMultipleAsync("GetDashboard", param: parameters, commandType: CommandType.StoredProcedure);
                var TopCount = new List<dynamic>();
                var Facility = new List<dynamic>();
                var RecentUpdate = new List<dynamic>();
                if (Year == null && Month == null)
                {
                    TopCount = (await multi.ReadAsync<dynamic>()).ToList();
                    Facility = (await multi.ReadAsync<dynamic>()).ToList();
                    RecentUpdate = (await multi.ReadAsync<dynamic>()).ToList();

                }
                var Application = (await multi.ReadAsync<dynamic>()).ToList();
                return new { TopCount, Facility, Application, RecentUpdate };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return (null);
        }

        public async Task InsertSmsMessagesAsync(IEnumerable<SmsEntity> messages)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("[From]", typeof(string));
            dataTable.Columns.Add("[To]", typeof(string));
            dataTable.Columns.Add("[Message]", typeof(string));
            dataTable.Columns.Add("[Status]", typeof(string));
            dataTable.Columns.Add("[Date]", typeof(DateTime));

            foreach (var msg in messages)
            {

                dataTable.Rows.Add(msg.Origin ?? "Unknown", msg.Destination ?? "Unknown", msg.Message ?? "", msg.Status ?? "Unknown", msg.DateTime);
                if (msg.DateTime.ToString() == "")
                {

                }

            }

            try
            {
                using var connection = (SqlConnection)_context.CreateConnection();
                await connection.OpenAsync();

                using var bulkCopy = new SqlBulkCopy(connection)
                {
                    DestinationTableName = "SMS"
                };

                await bulkCopy.WriteToServerAsync(dataTable);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Bulk insert failed: {ex.Message}");
            }
        }
        public async Task<dynamic> InsertUpdateCampaignSms(CampaignSmsModel model)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    return (await connection.QueryAsync<dynamic>("InsertUpdateCampaignSms", model, commandType: CommandType.StoredProcedure)).ToList();
                }

            }
            catch (Exception ex)
            {
                return ex;
            }
        }





        public async Task<dynamic> InsertUpdateOwnNumber(OwnNumber model)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var parameters = new
                    {
                        Id = model.Id,
                        Number = model.Number,
                        Title = model.Title,
                    };

                    var resp = (await connection.QueryAsync<OwnNumber>(
                        "sp_InsertUpdate_OwnNumber",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    )).FirstOrDefault();

                    if (resp != null)
                    {
                        model.Id = resp.Id;
                        return model;
                    }
                    else
                    {
                        return new { success = false, message = "Stored procedure did not return any data." };
                    }
                }
            }
            catch (Exception ex)
            {
                return new { success = false, message = "Error occurred: " + ex.Message };
            }
        }




        public async Task<dynamic> GetSmsDetail(string? message, DateTime? startDate, DateTime? endDate, string? smsType, string? status, string? statusText,int? pageNumber = 1, int? pageSize = 10)
        {
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new
                {
                    message = message,
                    startDate = startDate,
                    endDate = endDate,
                    smsType = smsType,
                    status = status,
                    pageNumber = pageNumber,
                    pageSize = pageSize,
                    statusText= statusText

                };
                using var multi = await con.QueryMultipleAsync("GetSmsDetail", param: parameters, commandType: CommandType.StoredProcedure);
                var List = (await multi.ReadAsync<dynamic>()).ToList();
                var Count = (await multi.ReadAsync<dynamic>()).ToList();
                var TotalCount = (await multi.ReadAsync<dynamic>()).FirstOrDefault();

                return new { List, Count, TotalCount };
            }
            catch (Exception ex)
            {
                return (null);
            }

        }
        public async Task<dynamic> GetDashboard(string? message, DateTime? startDate, DateTime? endDate, string? smsType, string? status, int? pageNumber = 1, int? pageSize = 10)
        {
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new
                {
                    message = message,
                    startDate = startDate,
                    endDate = endDate,
                    smsType = smsType,
                    status = status,
                    pageNumber = pageNumber,
                    pageSize = pageSize

                };
                using var multi = await con.QueryMultipleAsync("GetDashboard", param: parameters, commandType: CommandType.StoredProcedure);
               // var List = (await multi.ReadAsync<dynamic>()).ToList();
                var Count = (await multi.ReadAsync<dynamic>()).ToList();

                return new {  Count };
            }
            catch (Exception ex)
            {
                return (null);
            }

        }



        public async Task<dynamic> GetContact(int? Id, string? SearchText, int? pageNumber = 1, int? pageSize = 10)
        {
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new
                {
                    Id = Id,
                    SearchText = @SearchText,
                    pageNumber = pageNumber,
                    pageSize = pageSize

                };
                using var multi = await con.QueryMultipleAsync("sp_GetContacts", param: parameters, commandType: CommandType.StoredProcedure);
                var List = (await multi.ReadAsync<dynamic>()).ToList();


                return new { List };
            }
            catch (Exception ex)
            {
                return (null);
            }

        }

        public async Task<dynamic> GetContactDetails(string? To, string? Text, int? PageNumber = 1, int? PageSize = 20)
        {
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new
                {
                    To = To,
                    Text = Text,
                    PageNumber = PageNumber,
                    PageSize = PageSize
                };
                return await con.QueryAsync("GetContactDetails", param: parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }



        public async Task<dynamic> GetAllCampaigns(string? Text, int? PageNumber = 1, int? PageSize = 20)
        {
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new
                {

                    Text = Text,
                    PageNumber = PageNumber,
                    PageSize = PageSize
                };
                return await con.QueryAsync("GetAllCampaigns", param: parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }


        public async Task<dynamic> InsertUpdateContactGroup(ContactGroupModel user)
        {
            try
            {
                using (IDbConnection con = _context.CreateConnection())
                {
                    var parameters = new
                    {
                        ID = user.Id,
                        GroupName = user.GroupName

                    };
                    var resp = (await con.QueryAsync<dynamic>("InsertUpdateContactGroup", parameters, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                    user.Id = resp.Id;
                    return user;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<dynamic> InsertUpdateTemplate(TemplateModel user)
        {
            try
            {
                using (IDbConnection con = _context.CreateConnection())
                {
                    var parameters = new
                    {
                        Id = user.Id,
                        Title = user.Title,
                        Text = user.Text,
                        Is3cx = user.Is3cx,
                        IsAutoReply = user.IsAutoReply
                    };
                    var resp = (await con.QueryAsync<dynamic>("InsertUpdateTemplate", parameters, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                    user.Id = resp.Id;
                    return user;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<dynamic> SentSMSContactDetail(int ProviderId, string From, string To, string Message, int AgentId)
        {
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new
                {
                    ProviderId = ProviderId,
                    From = From,
                    To = To,
                    Message = Message,
                    AgentId = AgentId
                };
                return await con.ExecuteAsync("SentSMSContactDetail", parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<dynamic> InsertUpdateBulkSms(BulkSmsModel model)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    return (await connection.QueryAsync<dynamic>("InsertUpdateBulkSms", model, commandType: CommandType.StoredProcedure)).ToList();
                }

            }
            catch (Exception ex)
            {
                return ex;
            }

        }

        public async Task<dynamic> GetCampaignContactDetail(int? CampaignId, int? PageNumber = 1, int? PageSize = 20)
        {
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new
                {
                    CampaignId = CampaignId,
                    PageNumber = PageNumber,
                    PageSize = PageSize
                };
                return await con.QueryAsync("GetCampaignContactDetail", param: parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<dynamic> GetContactGroup(int? Id)
        {
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new
                {
                    Id = Id
                };
                return await con.QueryAsync("GetContactGroup", param: parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }


        public async Task<dynamic> InsertUpdateUsers(UserModel user)
        {
            try
            {
                using (IDbConnection con = _context.CreateConnection())
                {
                    var parameters = new
                    {
                        Id = user.Id,
                        FullName = user.FullName,
                        UserName = user.UserName,
                        Password = user.Password,
                        RoleId = user.RoleId,

                    };

                    var resp = (await con.QueryAsync<dynamic>("InsertUpdateUsers", parameters, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                    user.Id = resp.Id;
                    return user;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<dynamic> GetUser_OG()
        {

            try
            {
                using var con = _context.CreateConnection();
                return await con.QueryAsync("API_Select_User", commandType: CommandType.StoredProcedure);


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return (null);
        }

        public async Task<dynamic> GetUser(int? Id)

        {
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new
                {

                    Id = Id
                };
                return await con.QueryAsync("GetUsers", param: parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                return (null);
            }

        }
        public async Task<dynamic> Insert_Update_ProviderAccount(ProviderAccountModel user)
        {
            try
            {
                using (IDbConnection con = _context.CreateConnection())
                {
                    var parameters = new
                    {
                        ID = user.Id,
                        ProviderID = user.ProviderID,
                        Number = user.Number,
                        LoginID = user.LoginID,
                        LoginPassword = user.LoginPassword,
                        ApiKey = user.ApiKey,
                        ApiSecret = user.APISecret
                    };
                    var resp = (await con.QueryAsync<dynamic>("Insert_Update_ProviderAccount", parameters, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                    user.Id = resp.Id;
                    return user;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<dynamic> GetCampaigns(string? searchTerm = null)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var parameters = new { SearchTerm = searchTerm };
                    return (await connection.QueryAsync<dynamic>("GetCampaigns", parameters, commandType: CommandType.StoredProcedure)).ToList();
                }
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
        public async Task<dynamic> GetTemplate(int? Id)

        {
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new
                {

                    Id = Id
                };
                return await con.QueryAsync("GetTemplate", param: parameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                return (null);
            }

        }
    }
}





