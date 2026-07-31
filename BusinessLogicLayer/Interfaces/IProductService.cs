using BusinessObjectsLayer.Entities;

namespace BusinessLogicLayer.Interfaces
{
    public interface IProductService
    {
        Task<dynamic> GetProduct(int? Id, string? SearchText, bool? IsActive, int? PageNumber, int? PageSize);
        Task<dynamic> InsertUpdateProduct(ProductModel model);
        Task<dynamic> DeleteProduct(int? Id);
    }
}
