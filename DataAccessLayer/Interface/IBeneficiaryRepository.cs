using BusinessObjectsLayer.Entities;

namespace DataAccessLayer.Interface
{
    public interface IBeneficiaryRepository
    {
        Task<dynamic> GetBeneficiary(int? Id, string? SearchText, bool? IsActive, int? PageNumber = 1, int? PageSize = 20);
        Task<dynamic> InsertUpdateBeneficiary(BeneficiaryModel model);
        Task<dynamic> DeleteBeneficiary(int? Id);
    }
}
