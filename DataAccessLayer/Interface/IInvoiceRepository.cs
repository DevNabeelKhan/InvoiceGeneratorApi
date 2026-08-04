using BusinessObjectsLayer.DTOs;
using BusinessObjectsLayer.Entities;

namespace DataAccessLayer.Interface
{
    public interface IInvoiceRepository
    {
        Task<dynamic> GetInvoice(int? Id, string? SearchText, int? CustomerId, string? Status, bool? IsActive, int? PageNumber, int? PageSize);
        Task<dynamic> InsertUpdateInvoice(InvoiceModel model);
        Task<dynamic> DeleteInvoice(int? Id, int? UserId);
        Task<dynamic> GetInvoiceProduct(int? Id, int? InvoiceId);
        Task<dynamic> InsertUpdateInvoiceProduct(InvoiceProductModel model);
        Task<dynamic> DeleteInvoiceProduct(int? Id);
        Task<dynamic> GetCompany(int? Id);
        Task<dynamic> InsertUpdateCompany(CompanyModel model);
        Task<dynamic> GetCurrency(int? Id);
        Task<dynamic> InsertUpdateCurrency(CurrencyModel model);
        Task<dynamic> GetNextInvoiceNumber(int? Year, string? Prefix);
        Task<dynamic> GetProject(int? Id, string? SearchText, bool? IsActive, int? PageNumber, int? PageSize);
        Task<dynamic> InsertUpdateProject(ProjectModel model);
        Task<dynamic> DeleteProject(int? Id, int? UserId);
        Task<dynamic> GetProjectDocument(int? Id, int? ProjectId);
        Task<dynamic> InsertProjectDocument(ProjectDocumentModel model);
        Task<dynamic> DeleteProjectDocument(int? Id, int? UserId);
    }
}
