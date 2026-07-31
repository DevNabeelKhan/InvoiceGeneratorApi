using Dapper;
using DataAccess.DbContext;
using DataAccessLayer.Interface;
using System.Data;

namespace DataAccessLayer.Repositories
{
    public class BeneficiaryCustomerMappingRepository : IBeneficiaryCustomerMappingRepository
    {
        private readonly DapperContext _context;

        public BeneficiaryCustomerMappingRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<dynamic> GetBeneficiariesByCustomerId(int customerId)
        {
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new { CustomerId = customerId };
                var res = (await con.QueryAsync<dynamic>("GetBeneficiariesByCustomerId", param: parameters, commandType: CommandType.StoredProcedure)).ToList();
                return res;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<dynamic> GetCustomersByBeneficiaryId(int beneficiaryId)
        {
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new { BeneficiaryId = beneficiaryId };
                var res = (await con.QueryAsync<dynamic>("GetCustomersByBeneficiaryId", param: parameters, commandType: CommandType.StoredProcedure)).ToList();
                return res;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<dynamic> SaveCustomerBeneficiaries(int customerId, List<int>? beneficiaryIds)
        {
            var result = new BusinessObjectsLayer.Entities.Result();
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new
                {
                    CustomerId = customerId,
                    BeneficiaryIds = beneficiaryIds != null && beneficiaryIds.Count > 0 ? string.Join(",", beneficiaryIds) : null
                };
                await con.ExecuteAsync("SaveCustomerBeneficiaries", parameters, commandType: CommandType.StoredProcedure);
                result.IsSuccess = true;
                result.Status = "Success";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                result.IsSuccess = false;
            }
            return result;
        }

        public async Task<dynamic> SaveBeneficiaryCustomers(int beneficiaryId, List<int>? customerIds)
        {
            var result = new BusinessObjectsLayer.Entities.Result();
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new
                {
                    BeneficiaryId = beneficiaryId,
                    CustomerIds = customerIds != null && customerIds.Count > 0 ? string.Join(",", customerIds) : null
                };
                await con.ExecuteAsync("SaveBeneficiaryCustomers", parameters, commandType: CommandType.StoredProcedure);
                result.IsSuccess = true;
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
