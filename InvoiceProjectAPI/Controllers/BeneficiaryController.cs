using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Response;
using BusinessObjectsLayer.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ConvergeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BeneficiaryController : ControllerBase
    {
        private readonly IBeneficiaryService _beneficiaryService;

        public BeneficiaryController(IBeneficiaryService beneficiaryService)
        {
            _beneficiaryService = beneficiaryService;
        }

        [HttpGet("GetBeneficiary")]
        public async Task<IActionResult> GetBeneficiary(int? Id, string? SearchText, bool? IsActive, int? PageNumber = 1, int? PageSize = 20)
        {
            try
            {
                var result = await _beneficiaryService.GetBeneficiary(Id, SearchText, IsActive, PageNumber, PageSize);
                return Ok(ResponseHelper.GetSuccessResponse(result));
            }
            catch (Exception ex)
            {
                return Ok(ResponseHelper.GetFailureResponse());
            }
        }

        [HttpPost("InsertUpdateBeneficiary")]
        public async Task<IActionResult> InsertUpdateBeneficiary(BeneficiaryModel model)
        {
            try
            {
                var result = await _beneficiaryService.InsertUpdateBeneficiary(model);
                return Ok(ResponseHelper.GetSuccessResponse(result));
            }
            catch (Exception ex)
            {
                return Ok(ResponseHelper.GetFailureResponse());
            }
        }

        [HttpGet("DeleteBeneficiary")]
        public async Task<IActionResult> DeleteBeneficiary(int? Id)
        {
            try
            {
                var result = await _beneficiaryService.DeleteBeneficiary(Id);
                return Ok(ResponseHelper.GetSuccessResponse(result));
            }
            catch (Exception ex)
            {
                return Ok(ResponseHelper.GetFailureResponse());
            }
        }
    }
}
