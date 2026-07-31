using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Response;
using BusinessObjectsLayer.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ConvergeAPI.Controllers
{
    // Links Customers <-> Beneficiaries via the BeneficiaryCustomerMapping junction table.
    // Used by the reusable Customer/Beneficiary add-edit modals so a Customer can be
    // associated with one or more Beneficiaries (and vice versa) from either modal.
    [Route("api/[controller]")]
    [ApiController]
    public class BeneficiaryCustomerMappingController : ControllerBase
    {
        private readonly IBeneficiaryCustomerMappingService _mappingService;

        public BeneficiaryCustomerMappingController(IBeneficiaryCustomerMappingService mappingService)
        {
            _mappingService = mappingService;
        }

        [HttpGet("GetBeneficiariesByCustomer")]
        public async Task<IActionResult> GetBeneficiariesByCustomer(int customerId)
        {
            try
            {
                var result = await _mappingService.GetBeneficiariesByCustomerId(customerId);
                return Ok(ResponseHelper.GetSuccessResponse(result));
            }
            catch (Exception ex)
            {
                return Ok(ResponseHelper.GetFailureResponse());
            }
        }

        [HttpGet("GetCustomersByBeneficiary")]
        public async Task<IActionResult> GetCustomersByBeneficiary(int beneficiaryId)
        {
            try
            {
                var result = await _mappingService.GetCustomersByBeneficiaryId(beneficiaryId);
                return Ok(ResponseHelper.GetSuccessResponse(result));
            }
            catch (Exception ex)
            {
                return Ok(ResponseHelper.GetFailureResponse());
            }
        }

        [HttpPost("SaveCustomerBeneficiaries")]
        public async Task<IActionResult> SaveCustomerBeneficiaries(BeneficiaryCustomerMappingModel model)
        {
            if (model?.CustomerId == null)
            {
                return Ok(ResponseHelper.GetFailureResponse("CustomerId is required."));
            }
            try
            {
                var result = await _mappingService.SaveCustomerBeneficiaries(model.CustomerId.Value, model.BeneficiaryIds);
                return Ok(ResponseHelper.GetSuccessResponse(result));
            }
            catch (Exception ex)
            {
                return Ok(ResponseHelper.GetFailureResponse());
            }
        }

        [HttpPost("SaveBeneficiaryCustomers")]
        public async Task<IActionResult> SaveBeneficiaryCustomers(BeneficiaryCustomerMappingModel model)
        {
            if (model?.BeneficiaryId == null)
            {
                return Ok(ResponseHelper.GetFailureResponse("BeneficiaryId is required."));
            }
            try
            {
                var result = await _mappingService.SaveBeneficiaryCustomers(model.BeneficiaryId.Value, model.CustomerIds);
                return Ok(ResponseHelper.GetSuccessResponse(result));
            }
            catch (Exception ex)
            {
                return Ok(ResponseHelper.GetFailureResponse());
            }
        }
    }
}
