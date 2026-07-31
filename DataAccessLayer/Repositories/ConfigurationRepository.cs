using BusinessObjectsLayer.Entities;
using Dapper;
using DataAccess.DbContext;
using DataAccessLayer.Interface;
using DataAccessLayer.Shared;
using DataAccessLayer.Shared.Helper;
using Microsoft.AspNetCore.Http;
using System.Data;

namespace DataAccessLayer.Repositories
{
    public class ConfigurationRepository : IConfigurationRepository
    {
        private readonly DapperContext _context;
        private readonly HttpContextAccessor _httpContextAccessor;

        public ConfigurationRepository(HttpContextAccessor httpContextAccessor, DapperContext context)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<dynamic> GetConfiguration(string TableName, int? Id, string? SearchText, bool? IsActive, int? PageNumber = 1, int? PageSize = 20)
        {
            if (!ConfigurationTables.IsValid(TableName)) return null;
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new
                {
                    TableName = TableName,
                    Id = Id,
                    SearchText = SearchText,
                    IsActive = IsActive,
                    PageNumber = PageNumber,
                    PageSize = PageSize
                };
                var res = (await con.QueryAsync<dynamic>("GetConfigurationList", param: parameters, commandType: CommandType.StoredProcedure)).ToList();
                return res;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<dynamic> InsertUpdateConfiguration(ConfigurationModel model)
        {
            if (!ConfigurationTables.IsValid(model.TableName)) return null;
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new
                {
                    TableName = model.TableName,
                    Id = model.Id,
                    Title = model.Title,
                    IsActive = model.IsActive,
                    UserId = Helper.UserId(_httpContextAccessor)
                };

                var resp = (await con.QueryAsync<dynamic>("InsertUpdateConfiguration", parameters, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                model.Id = resp?.Id;
                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<dynamic> DeleteConfiguration(string TableName, int? Id)
        {
            var result = new Result();
            if (!ConfigurationTables.IsValid(TableName))
            {
                result.IsSuccess = false;
                result.Message = "Invalid table name.";
                return result;
            }
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new
                {
                    TableName = TableName,
                    Id = Id,
                    UserId = Helper.UserId(_httpContextAccessor)
                };
                await con.ExecuteAsync("DeleteConfiguration", parameters, commandType: CommandType.StoredProcedure);
                result.IsSuccess = true;
                result.Message = "Deleted successfully.";
                result.Status = "Success";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                result.IsSuccess = false;
            }
            return result;
        }
    }
}
