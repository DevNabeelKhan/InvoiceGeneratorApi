using BusinessObjectsLayer.DTOs;
using BusinessObjectsLayer.Entities;

namespace BusinessLogicLayer.Interfaces
{
    public interface IInvoiceService
    {
        Task<dynamic> GetInvoice(int? Id, string? SearchText, int? CustomerId, string? Status, bool? IsActive, int? PageNumber, int? PageSize);
        Task<dynamic> SaveInvoice(InvoiceDto dto);
        Task<dynamic> DeleteInvoice(int? Id, int? UserId);
        Task<dynamic> GetInvoiceProduct(int? Id, int? InvoiceId);
        Task<dynamic> GetCompany(int? Id);
        Task<dynamic> SaveCompany(CompanyModel model);
        Task<dynamic> GetCurrency(int? Id);
        Task<dynamic> SaveCurrency(CurrencyModel model);
        Task<dynamic> GetProject(int? Id, string? SearchText, bool? IsActive, int? PageNumber, int? PageSize);
        Task<dynamic> SaveProject(ProjectModel model);
        Task<dynamic> DeleteProject(int? Id, int? UserId);
        Task<dynamic> GetProjectDocument(int? Id, int? ProjectId);
        Task<dynamic> SaveProjectDocument(ProjectDocumentModel model);
        Task<dynamic> DeleteProjectDocument(int? Id, int? UserId);
        Task<dynamic> GetWarehouse(int? Id, string? SearchText, bool? IsActive, int? PageNumber, int? PageSize);
        Task<dynamic> SaveWarehouse(WarehouseModel model);
        Task<dynamic> DeleteWarehouse(int? Id, int? UserId);
    }
}
