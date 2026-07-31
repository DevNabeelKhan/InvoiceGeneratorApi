using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Interface;

namespace BusinessLogicLayer.Service
{
    public class BeneficiaryCustomerMappingService : IBeneficiaryCustomerMappingService
    {
        private readonly IBeneficiaryCustomerMappingRepository _mappingRepository;

        public BeneficiaryCustomerMappingService(IBeneficiaryCustomerMappingRepository mappingRepository)
        {
            _mappingRepository = mappingRepository;
        }

        public async Task<dynamic> GetBeneficiariesByCustomerId(int customerId)
        {
            return await _mappingRepository.GetBeneficiariesByCustomerId(customerId);
        }

        public async Task<dynamic> GetCustomersByBeneficiaryId(int beneficiaryId)
        {
            return await _mappingRepository.GetCustomersByBeneficiaryId(beneficiaryId);
        }

        public async Task<dynamic> SaveCustomerBeneficiaries(int customerId, List<int>? beneficiaryIds)
        {
            return await _mappingRepository.SaveCustomerBeneficiaries(customerId, beneficiaryIds);
        }

        public async Task<dynamic> SaveBeneficiaryCustomers(int beneficiaryId, List<int>? customerIds)
        {
            return await _mappingRepository.SaveBeneficiaryCustomers(beneficiaryId, customerIds);
        }
    }
}
