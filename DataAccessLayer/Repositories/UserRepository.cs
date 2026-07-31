
using Amazon.S3; 
using BusinessObjectsLayer.Entities;
using Dapper;
using DataAccess.DbContext;
using DataAccessLayer.Interface;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace DataAccessLayer.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DapperContext _context;
        private readonly HttpContextAccessor _httpContextAccessor;
        public UserRepository(HttpContextAccessor httpContextAccessor,DapperContext context)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<BusinessObjectsLayer.Entities.User> Authenticate(string userId,string password)
        {
            try
            {
                using var connection = _context.CreateConnection();
                var user = await connection.QueryFirstOrDefaultAsync<User>("Login", new { UserId = userId, Password = password });
                if (user != null)
                {
                    return user;
                }
            }
            catch (Exception ex)
            {
            }
            return null;    
        }

        public async Task<dynamic> ChangePassword(ChangePassword changePassword)
        {
            var resp = new Object();
            try
            {
                var param = new
                {
                    UserId = changePassword.Id,
                    CurrentPassword = changePassword.CurrentPassword,
                    NewPassword = changePassword.NewPassword
                };
                using var connection = _context.CreateConnection();
                resp = await connection.QueryFirstOrDefaultAsync<dynamic>("ChangePassword", param: param);
                return resp;

            }
            catch (Exception ex)
            {
            }
            return null;
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
                        Id = Id,
                        //UserId = Helper.UserId(_httpContextAccessor)
                    };
                    await connection.ExecuteAsync("DeleteUser", parameters, commandType: CommandType.StoredProcedure);
                    result.IsSuccess = true;
                    result.Message = result.IsSuccess ? "User deleted successfully." : "failed to delete!";
                    result.Status = result.IsSuccess ? "Success" : "Error";
                }

            }
            catch (Exception ex)
            {
                result.Data = null;
            }
            return result;
        }
        public async Task<dynamic> GetRole()
        {
            var result = new Result();
            try
            {
                using (var connection = _context.CreateConnection()) 
                        return  (await connection.QueryAsync("GetRole", commandType: CommandType.StoredProcedure)).ToList();
                     

            }
            catch (Exception ex)
            {
                result.Data = null;
            }
            return result;
        }
        
        public async Task<dynamic> GetUserById(int? UserId, int? RoleId, int?PageNumber = 1,int?PageSize = 100, string? SearchText = null)
        {
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new
                {
                    UserId = UserId,
                    RoleId = RoleId,
                    PageNumber =PageNumber,
                    PageSize=PageSize,
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

        public async Task<List<BusinessObjectsLayer.Entities.User>> GetUserId(int UserId, List<int> BatchIds)
        {
            try
            {
                string BatchId = BatchIds != null ? "string" : null;
                using var connection = _context.CreateConnection();
                var user = await connection.QueryAsync<BusinessObjectsLayer.Entities.User>("GetUserId UserId,BatchIds", new { UserId = UserId, BatchIds = BatchId });
                if (user != null)
                {
                    return user.ToList();
                }
            }
            catch (Exception ex)
            {
            }
            return null;
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
                        RoleId = user.RoleId,
                        CompanyId = user.CompanyId,
                        Email = user.Email,
                        Password = user.Password,
                        UserName = user.UserName,
                        PictureUrl = user.PictureUrl,
                        UserId =1  
                    };

                    var resp = (await con.QueryAsync<dynamic>("InsertUpdateUser", parameters, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                    user.Id = resp.Id;
                    return user;
                }
                return user;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


    }
}
