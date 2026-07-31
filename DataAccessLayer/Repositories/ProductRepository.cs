using BusinessObjectsLayer.Entities;
using Dapper;
using DataAccess.DbContext;
using DataAccessLayer.Interface;
using DataAccessLayer.Shared.Helper;
using Microsoft.AspNetCore.Http;
using System.Data;

namespace DataAccessLayer.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly DapperContext _context;
        private readonly HttpContextAccessor _httpContextAccessor;

        public ProductRepository(HttpContextAccessor httpContextAccessor, DapperContext context)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<dynamic> GetProduct(int? Id, string? SearchText, bool? IsActive, int? PageNumber = 1, int? PageSize = 20)
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
                var res = (await con.QueryAsync<dynamic>("GetProduct", param: parameters, commandType: CommandType.StoredProcedure)).ToList();
                return res;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<dynamic> InsertUpdateProduct(ProductModel model)
        {
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new
                {
                    Id = model.Id,
                    Title = model.Title,
                    ProductStatusId = model.ProductStatusId,
                    UnitOfMeasureId = model.UnitOfMeasureId,
                    ServiceCode = model.ServiceCode,
                    ServiceDescription = model.ServiceDescription,
                    SellingPrice = model.SellingPrice,
                    RevenueAccountID = model.RevenueAccountID,
                    RevenueTaxRateId = model.RevenueTaxRateId,
                    PurchaseCost = model.PurchaseCost,
                    ExpenseAccountId = model.ExpenseAccountId,
                    PurchaseTaxRateId = model.PurchaseTaxRateId,
                    IsActive = model.IsActive,
                    UserId = Helper.UserId(_httpContextAccessor)
                };

                var resp = (await con.QueryAsync<dynamic>("InsertUpdateProduct", parameters, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                model.Id = resp?.Id;
                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<dynamic> DeleteProduct(int? Id)
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
                await con.ExecuteAsync("DeleteProduct", parameters, commandType: CommandType.StoredProcedure);
                result.IsSuccess = true;
                result.Message = "Product deleted successfully.";
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
