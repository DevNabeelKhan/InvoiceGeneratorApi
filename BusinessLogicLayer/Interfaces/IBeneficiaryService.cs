using BusinessObjectsLayer.Entities;

namespace BusinessLogicLayer.Interfaces
{
    public interface IBeneficiaryService
    {
        Task<dynamic> GetBeneficiary(int? Id, string? SearchText, bool? IsActive, int? PageNumber, int? PageSize);
        Task<dynamic> InsertUpdateBeneficiary(BeneficiaryModel model);
        Task<dynamic> DeleteBeneficiary(int? Id);
    }
}
