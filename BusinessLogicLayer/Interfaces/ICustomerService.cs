using BusinessObjectsLayer.Entities;

namespace BusinessLogicLayer.Interfaces
{
    public interface ICustomerService
    {
        Task<dynamic> GetCustomer(int? Id, string? SearchText, bool? IsActive, int? PageNumber, int? PageSize);
        Task<dynamic> InsertUpdateCustomer(CustomerModel model);
        Task<dynamic> DeleteCustomer(int? Id);
    }
}
