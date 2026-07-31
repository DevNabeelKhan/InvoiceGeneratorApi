using BusinessObjectsLayer.Entities;

namespace DataAccessLayer.Interface
{
    public interface IProductRepository
    {
        Task<dynamic> GetProduct(int? Id, string? SearchText, bool? IsActive, int? PageNumber = 1, int? PageSize = 20);
        Task<dynamic> InsertUpdateProduct(ProductModel model);
        Task<dynamic> DeleteProduct(int? Id);
    }
}
