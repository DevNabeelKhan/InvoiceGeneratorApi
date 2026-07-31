using BusinessObjectsLayer.Entities;

namespace DataAccessLayer.Interface
{
    public interface ICustomerRepository
    {
        Task<dynamic> GetCustomer(int? Id, string? SearchText, bool? IsActive, int? PageNumber = 1, int? PageSize = 20);
        Task<dynamic> InsertUpdateCustomer(CustomerModel model);
        Task<dynamic> DeleteCustomer(int? Id);
    }
}
