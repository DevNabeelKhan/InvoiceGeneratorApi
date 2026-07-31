namespace BusinessLogicLayer.Interfaces
{
    public interface IBeneficiaryCustomerMappingService
    {
        Task<dynamic> GetBeneficiariesByCustomerId(int customerId);
        Task<dynamic> GetCustomersByBeneficiaryId(int beneficiaryId);
        Task<dynamic> SaveCustomerBeneficiaries(int customerId, List<int>? beneficiaryIds);
        Task<dynamic> SaveBeneficiaryCustomers(int beneficiaryId, List<int>? customerIds);
    }
}
