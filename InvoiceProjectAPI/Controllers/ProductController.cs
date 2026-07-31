using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Response;
using BusinessObjectsLayer.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ConvergeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("GetProduct")]
        public async Task<IActionResult> GetProduct(int? Id, string? SearchText, bool? IsActive, int? PageNumber = 1, int? PageSize = 20)
        {
            try
            {
                var result = await _productService.GetProduct(Id, SearchText, IsActive, PageNumber, PageSize);
                return Ok(ResponseHelper.GetSuccessResponse(result));
            }
            catch (Exception ex)
            {
                return Ok(ResponseHelper.GetFailureResponse());
            }
        }

        [HttpPost("InsertUpdateProduct")]
        public async Task<IActionResult> InsertUpdateProduct(ProductModel model)
        {
            try
            {
                var result = await _productService.InsertUpdateProduct(model);
                return Ok(ResponseHelper.GetSuccessResponse(result));
            }
            catch (Exception ex)
            {
                return Ok(ResponseHelper.GetFailureResponse());
            }
        }

        [HttpGet("DeleteProduct")]
        public async Task<IActionResult> DeleteProduct(int? Id)
        {
            try
            {
                var result = await _productService.DeleteProduct(Id);
                return Ok(ResponseHelper.GetSuccessResponse(result));
            }
            catch (Exception ex)
            {
                return Ok(ResponseHelper.GetFailureResponse());
            }
        }
    }
}
