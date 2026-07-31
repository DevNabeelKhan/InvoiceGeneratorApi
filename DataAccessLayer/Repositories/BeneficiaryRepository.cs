using BusinessObjectsLayer.Entities;
using Dapper;
using DataAccess.DbContext;
using DataAccessLayer.Interface;
using DataAccessLayer.Shared.Helper;
using Microsoft.AspNetCore.Http;
using System.Data;

namespace DataAccessLayer.Repositories
{
    public class BeneficiaryRepository : IBeneficiaryRepository
    {
        private readonly DapperContext _context;
        private readonly HttpContextAccessor _httpContextAccessor;

        public BeneficiaryRepository(HttpContextAccessor httpContextAccessor, DapperContext context)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<dynamic> GetBeneficiary(int? Id, string? SearchText, bool? IsActive, int? PageNumber = 1, int? PageSize = 20)
        {
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new
                {
                    Id = Id,
                    SearchText = SearchText,
                    IsActive = IsActive,
                    PageNumber = PageNumber,
                    PageSize = PageSize
                };
                var res = (await con.QueryAsync<dynamic>("GetBeneficiary", param: parameters, commandType: CommandType.StoredProcedure)).ToList();
                return res;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<dynamic> InsertUpdateBeneficiary(BeneficiaryModel model)
        {
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new
                {
                    Id = model.Id,
                    IBAN = model.IBAN,
                    CurrencyId = model.CurrencyId,
                    BeneficiaryName = model.BeneficiaryName,
                    BeneficiaryAddress = model.BeneficiaryAddress,
                    BankName = model.BankName,
                    Swift = model.Swift,
                    CountryId = model.CountryId,
                    BankFeesTypeId = model.BankFeesTypeId,
                    IsActive = model.IsActive,
                    UserId = Helper.UserId(_httpContextAccessor)
                };

                var resp = (await con.QueryAsync<dynamic>("InsertUpdateBeneficiary", parameters, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                model.Id = resp?.Id;
                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<dynamic> DeleteBeneficiary(int? Id)
        {
            var result = new Result();
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new
                {
                    Id = Id,
                    UserId = Helper.UserId(_httpContextAccessor)
                };
                await con.ExecuteAsync("DeleteBeneficiary", parameters, commandType: CommandType.StoredProcedure);
                result.IsSuccess = true;
                result.Message = "Beneficiary deleted successfully.";
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
