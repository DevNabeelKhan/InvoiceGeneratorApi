using BusinessLogicLayer.Interfaces;
using BusinessObjectsLayer.Entities;
using DataAccessLayer.Interface;

namespace BusinessLogicLayer.Service
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<dynamic> GetProduct(int? Id, string? SearchText, bool? IsActive, int? PageNumber, int? PageSize)
        {
            var res = await _productRepository.GetProduct(Id, SearchText, IsActive, PageNumber, PageSize);
            return res;
        }

        public async Task<dynamic> InsertUpdateProduct(ProductModel model)
        {
            var res = await _productRepository.InsertUpdateProduct(model);
            return res;
        }

        public async Task<dynamic> DeleteProduct(int? Id)
        {
            var res = await _productRepository.DeleteProduct(Id);
            return res;
        }
    }
}
