using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Response;
using BusinessObjectsLayer.Entities;
using DataAccessLayer.Shared;
using Microsoft.AspNetCore.Mvc;

namespace ConvergeAPI.Controllers
{
    // Generic CRUD controller for all simple lookup/configuration tables that share the
    // (Id, Title, IsActive, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy) schema:
    // AccountType, BankFeesType, CashFlowType, ContactType, CostCenter, Industry,
    // InvoicingRelationShip, PaymentTerm, RevenueTaxRateType, Role, UnitOfMeasure, ProductStatus.
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        private readonly IConfigurationService _configurationService;

        public ConfigurationController(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        [HttpGet("GetConfiguration")]
        public async Task<IActionResult> GetConfiguration(string TableName, int? Id, string? SearchText, bool? IsActive, int? PageNumber = 1, int? PageSize = 20)
        {
            var decryptedTableName = EncryptionHelper.Decrypt(TableName);
            if (!ConfigurationTables.IsValid(decryptedTableName))
            {
                return Ok(ResponseHelper.GetFailureResponse("Invalid table name."));
            }
            try
            {
                var result = await _configurationService.GetConfiguration(decryptedTableName, Id, SearchText, IsActive, PageNumber, PageSize);
                return Ok(ResponseHelper.GetSuccessResponse(result));
            }
            catch (Exception ex)
            {
                return Ok(ResponseHelper.GetFailureResponse());
            }
        }

        [HttpPost("InsertUpdateConfiguration")]
        public async Task<IActionResult> InsertUpdateConfiguration(ConfigurationModel model)
        {
            var decryptedTableName = EncryptionHelper.Decrypt(model?.TableName);
            if (!ConfigurationTables.IsValid(decryptedTableName))
            {
                return Ok(ResponseHelper.GetFailureResponse("Invalid table name."));
            }
            model.TableName = decryptedTableName;
            try
            {
                var result = await _configurationService.InsertUpdateConfiguration(model);
                return Ok(ResponseHelper.GetSuccessResponse(result));
            }
            catch (Exception ex)
            {
                return Ok(ResponseHelper.GetFailureResponse());
            }
        }

        [HttpGet("DeleteConfiguration")]
        public async Task<IActionResult> DeleteConfiguration(string TableName, int? Id)
        {
            var decryptedTableName = EncryptionHelper.Decrypt(TableName);
            if (!ConfigurationTables.IsValid(decryptedTableName))
            {
                return Ok(ResponseHelper.GetFailureResponse("Invalid table name."));
            }
            try
            {
                var result = await _configurationService.DeleteConfiguration(decryptedTableName, Id);
                return Ok(ResponseHelper.GetSuccessResponse(result));
            }
            catch (Exception ex)
            {
                return Ok(ResponseHelper.GetFailureResponse());
            }
        }
    }
}
