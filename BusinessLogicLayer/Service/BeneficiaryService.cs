using BusinessLogicLayer.Interfaces;
using BusinessObjectsLayer.Entities;
using DataAccessLayer.Interface;

namespace BusinessLogicLayer.Service
{
    public class BeneficiaryService : IBeneficiaryService
    {
        private readonly IBeneficiaryRepository _beneficiaryRepository;

        public BeneficiaryService(IBeneficiaryRepository beneficiaryRepository)
        {
            _beneficiaryRepository = beneficiaryRepository;
        }

        public async Task<dynamic> GetBeneficiary(int? Id, string? SearchText, bool? IsActive, int? PageNumber, int? PageSize)
        {
            var res = await _beneficiaryRepository.GetBeneficiary(Id, SearchText, IsActive, PageNumber, PageSize);
            return res;
        }

        public async Task<dynamic> InsertUpdateBeneficiary(BeneficiaryModel model)
        {
            var res = await _beneficiaryRepository.InsertUpdateBeneficiary(model);
            return res;
        }

        public async Task<dynamic> DeleteBeneficiary(int? Id)
        {
            var res = await _beneficiaryRepository.DeleteBeneficiary(Id);
            return res;
        }
    }
}
