using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Response;
using BusinessObjectsLayer.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ConvergeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet("GetCustomer")]
        public async Task<IActionResult> GetCustomer(int? Id, string? SearchText, bool? IsActive, int? PageNumber = 1, int? PageSize = 20)
        {
            try
            {
                var result = await _customerService.GetCustomer(Id, SearchText, IsActive, PageNumber, PageSize);
                return Ok(ResponseHelper.GetSuccessResponse(result));
            }
            catch (Exception ex)
            {
                return Ok(ResponseHelper.GetFailureResponse());
            }
        }

        [HttpPost("InsertUpdateCustomer")]
        public async Task<IActionResult> InsertUpdateCustomer(CustomerModel model)
        {
            try
            {
                var result = await _customerService.InsertUpdateCustomer(model);
                return Ok(ResponseHelper.GetSuccessResponse(result));
            }
            catch (Exception ex)
            {
                return Ok(ResponseHelper.GetFailureResponse());
            }
        }

        [HttpGet("DeleteCustomer")]
        public async Task<IActionResult> DeleteCustomer(int? Id)
        {
            try
            {
                var result = await _customerService.DeleteCustomer(Id);
                return Ok(ResponseHelper.GetSuccessResponse(result));
            }
            catch (Exception ex)
            {
                return Ok(ResponseHelper.GetFailureResponse());
            }
        }
    }
}
