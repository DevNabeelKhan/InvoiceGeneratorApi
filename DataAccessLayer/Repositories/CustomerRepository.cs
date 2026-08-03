using BusinessObjectsLayer.Entities;
using Dapper;
using DataAccess.DbContext;
using DataAccessLayer.Interface;
using DataAccessLayer.Shared.Helper;
using Microsoft.AspNetCore.Http;
using System.Data;

namespace DataAccessLayer.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly DapperContext _context;
        private readonly HttpContextAccessor _httpContextAccessor;

        public CustomerRepository(HttpContextAccessor httpContextAccessor, DapperContext context)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<dynamic> GetCustomer(int? Id, string? SearchText, bool? IsActive, int? PageNumber = 1, int? PageSize = 20)
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
                var res = (await con.QueryAsync<dynamic>("GetCustomer", param: parameters, commandType: CommandType.StoredProcedure)).ToList();
                return res;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<dynamic> InsertUpdateCustomer(CustomerModel model)
        {
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new
                {
                    Id = model.Id,
                    CustomerName = model.CustomerName,
                    CountryId = model.CountryId,
                    TaxRegistrationNumber = model.TaxRegistrationNumber,
                    City = model.City,
                    StreetAddress = model.StreetAddress,
                    BuildingNumber = model.BuildingNumber,
                    District = model.District,
                    AddressAdditionalNumber = model.AddressAdditionalNumber,
                    PostalCode = model.PostalCode,
                    InvoicingCode = model.InvoicingCode,
                    InvoicingEmail = model.InvoicingEmail,
                    InvoicingPhone = model.InvoicingPhone,
                    InvoicingRelationShipId = model.InvoicingRelationShipId,
                    PaymentTermId = model.PaymentTermId,
                    ContactTypeID = model.ContactTypeID,
                    ContactTypeNumber = model.ContactTypeNumber,
                    SellingRevenueAccountId = model.SellingRevenueAccountId,
                    SellingRevenueCostCenterId = model.SellingRevenueCostCenterId,
                    SellingRevenueTaxRateId = model.SellingRevenueTaxRateId,
                    ArabicName = model.ArabicName,
                    ArabicAddress = model.ArabicAddress,
                    Email = model.Email,
                    Phone = model.Phone,
                    IsActive = model.IsActive,
                    UserId = Helper.UserId(_httpContextAccessor)
                };

                var resp = (await con.QueryAsync<dynamic>("InsertUpdateCustomer", parameters, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                model.Id = resp?.Id;
                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<dynamic> DeleteCustomer(int? Id)
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
                await con.ExecuteAsync("DeleteCustomer", parameters, commandType: CommandType.StoredProcedure);
                result.IsSuccess = true;
                result.Message = "Customer deleted successfully.";
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
