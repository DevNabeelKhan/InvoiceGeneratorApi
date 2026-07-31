using BusinessLogicLayer.Interfaces;
using BusinessObjectsLayer.Entities;
using DataAccessLayer.Interface;

namespace BusinessLogicLayer.Service
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<dynamic> GetCustomer(int? Id, string? SearchText, bool? IsActive, int? PageNumber, int? PageSize)
        {
            var res = await _customerRepository.GetCustomer(Id, SearchText, IsActive, PageNumber, PageSize);
            return res;
        }

        public async Task<dynamic> InsertUpdateCustomer(CustomerModel model)
        {
            var res = await _customerRepository.InsertUpdateCustomer(model);
            return res;
        }

        public async Task<dynamic> DeleteCustomer(int? Id)
        {
            var res = await _customerRepository.DeleteCustomer(Id);
            return res;
        }
    }
}
