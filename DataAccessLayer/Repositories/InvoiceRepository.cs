using BusinessObjectsLayer.Entities;
using Dapper;
using DataAccess.DbContext;
using DataAccessLayer.Interface;
using DataAccessLayer.Shared.Helper;
using Microsoft.AspNetCore.Http;
using System.Data;

namespace DataAccessLayer.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly DapperContext _context;
        private readonly HttpContextAccessor _httpContextAccessor;

        public InvoiceRepository(HttpContextAccessor httpContextAccessor, DapperContext context)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<dynamic> GetInvoice(int? Id, string? SearchText, int? CustomerId, string? Status, bool? IsActive, int? PageNumber = 1, int? PageSize = 20)
        {
            // Note: intentionally no try/catch here. The controller already wraps this
            // call in its own try/catch and returns a proper failure response - swallowing
            // exceptions here previously caused real errors to be masked as a "successful"
            // response with data: null, making failures indistinguishable from "not found".
            using var con = _context.CreateConnection();
            var parameters = new
            {
                Id = Id,
                SearchText = SearchText,
                CustomerId = CustomerId,
                Status = Status,
                IsActive = IsActive,
                PageNumber = PageNumber,
                PageSize = PageSize
            };

            var invoices = (await con.QueryAsync<dynamic>("GetInvoice", param: parameters, commandType: CommandType.StoredProcedure)).ToList();

            if (Id.HasValue && invoices.Any())
            {
                var invoice = invoices.First();
                var products = (await con.QueryAsync<dynamic>("GetInvoiceProduct",
                    new { InvoiceId = invoice.Id },
                    commandType: CommandType.StoredProcedure)).ToList();
                invoice.Products = products;

                var attachments = (await con.QueryAsync<dynamic>("GetInvoiceAttachments",
                    new { InvoiceId = invoice.Id },
                    commandType: CommandType.StoredProcedure)).ToList();
                invoice.Attachments = attachments;

                return invoice;
            }

            return invoices;
        }

        public async Task<dynamic> InsertUpdateInvoice(InvoiceModel model)
        {
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new
                {
                    Id = model.Id,
                    InvoiceNumber = model.InvoiceNumber,
                    UUID = model.UUID,
                    Reference = model.Reference,
                    PurchaseOrderNumber = model.PurchaseOrderNumber,
                    ProjectId = model.ProjectId,
                    WarehouseId = model.WarehouseId,
                    PricesIncludeTax = model.PricesIncludeTax,
                    CompanyId = model.CompanyId,
                    CustomerId = model.CustomerId,
                    CurrencyId = model.CurrencyId,
                    ExchangeRate = model.ExchangeRate,
                    InvoiceDate = model.InvoiceDate,
                    DueDate = model.DueDate,
                    Notes = model.Notes,
                    Status = model.Status,
                    PaymentStatus = model.PaymentStatus,
                    Draft = model.Draft,
                    Approved = model.Approved,
                    Cancelled = model.Cancelled,
                    Sent = model.Sent,
                    Subtotal = model.Subtotal,
                    DiscountPercentage = model.DiscountPercentage,
                    DiscountAmount = model.DiscountAmount,
                    TaxAmount = model.TaxAmount,
                    GrandTotal = model.GrandTotal,
                    RetentionPercentage = model.RetentionPercentage,
                    RetentionAmount = model.RetentionAmount,
                    RoundOffAmount = model.RoundOffAmount,
                    GeneratedQRCode = model.GeneratedQRCode,
                    QRCodeImagePath = model.QRCodeImagePath,
                    PreviousInvoiceHash = model.PreviousInvoiceHash,
                    XMLPath = model.XMLPath,
                    PDFPath = model.PDFPath,
                    CreatedIP = model.CreatedIP,
                    UserId = Helper.UserId(_httpContextAccessor)
                };

                var resp = (await con.QueryAsync<dynamic>("InsertUpdateInvoice", parameters, commandType: CommandType.StoredProcedure)).FirstOrDefault();

                if (resp != null)
                {
                    model.Id = resp.Id;
                    model.InvoiceNumber = resp.InvoiceNumber;
                    model.UUID = resp.UUID;
                }

                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<dynamic> DeleteInvoice(int? Id, int? UserId)
        {
            var result = new Result();
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new { Id = Id, UserId = UserId };
                await con.ExecuteAsync("DeleteInvoice", parameters, commandType: CommandType.StoredProcedure);
                result.IsSuccess = true;
                result.Message = "Invoice deleted successfully.";
                result.Status = "Success";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                result.IsSuccess = false;
            }
            return result;
        }

        public async Task<dynamic> GetInvoiceProduct(int? Id, int? InvoiceId)
        {
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new { Id = Id, InvoiceId = InvoiceId };
                return (await con.QueryAsync<dynamic>("GetInvoiceProduct", parameters, commandType: CommandType.StoredProcedure)).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<dynamic> InsertUpdateInvoiceProduct(InvoiceProductModel model)
        {
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new
                {
                    Id = model.Id,
                    InvoiceId = model.InvoiceId,
                    ProductId = model.ProductId,
                    Description = model.Description,
                    Unit = model.Unit,
                    Quantity = model.Quantity,
                    Price = model.Price,
                    DiscountPercentage = model.DiscountPercentage,
                    DiscountAmount = model.DiscountAmount,
                    TaxRate = model.TaxRate,
                    TaxableAmount = model.TaxableAmount,
                    VATAmount = model.VATAmount,
                    LineTotal = model.LineTotal,
                    AccountId = model.AccountId,
                    CostCenterId = model.CostCenterId,
                    RevenueRecognitionId = model.RevenueRecognitionId,
                    SortOrder = model.SortOrder,
                    IsActive = model.IsActive,
                    UserId = Helper.UserId(_httpContextAccessor)
                };

                var resp = (await con.QueryAsync<dynamic>("InsertUpdateInvoiceProduct", parameters, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                model.Id = resp?.Id;
                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<dynamic> DeleteInvoiceProduct(int? Id)
        {
            var result = new Result();
            try
            {
                using var con = _context.CreateConnection();
                await con.ExecuteAsync("DeleteInvoiceProduct", new { Id = Id }, commandType: CommandType.StoredProcedure);
                result.IsSuccess = true;
                result.Message = "Invoice line deleted successfully.";
                result.Status = "Success";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                result.IsSuccess = false;
            }
            return result;
        }

        public async Task<dynamic> GetCompany(int? Id)
        {
            try
            {
                using var con = _context.CreateConnection();
                return (await con.QueryAsync<dynamic>("GetCompany", new { Id = Id }, commandType: CommandType.StoredProcedure)).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<dynamic> InsertUpdateCompany(CompanyModel model)
        {
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new
                {
                    Id = model.Id,
                    Title = model.Title,
                    ArabicName = model.ArabicName,
                    Address = model.Address,
                    ArabicAddress = model.ArabicAddress,
                    Email = model.Email,
                    Phone = model.Phone,
                    Website = model.Website,
                    VATNumber = model.VATNumber,
                    LogoUrl = model.LogoUrl, 
                    BankName = model.BankName,
                    BankAccountNumber = model.BankAccountNumber,
                    IBAN = model.IBAN,
                    SwiftCode = model.SwiftCode,
                    AccountCurrency = model.AccountCurrency,
                    BeneficiaryName = model.BeneficiaryName,
                    Country = model.Country,
                    City = model.City,
                    IsActive = model.IsActive,
                    UserId = Helper.UserId(_httpContextAccessor)
                };

                var resp = (await con.QueryAsync<dynamic>("InsertUpdateCompany", parameters, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                model.Id = resp?.Id;
                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<dynamic> GetCurrency(int? Id)
        {
            try
            {
                using var con = _context.CreateConnection();
                return (await con.QueryAsync<dynamic>("GetCurrency", new { Id = Id }, commandType: CommandType.StoredProcedure)).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<dynamic> InsertUpdateCurrency(CurrencyModel model)
        {
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new
                {
                    Id = model.Id,
                    Code = model.Code,
                    Title = model.Title,
                    Symbol = model.Symbol,
                    ExchangeRate = model.ExchangeRate,
                    IsActive = model.IsActive,
                    UserId = Helper.UserId(_httpContextAccessor)
                };

                var resp = (await con.QueryAsync<dynamic>("InsertUpdateCurrency", parameters, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                model.Id = resp?.Id;
                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<dynamic> GetProject(int? Id, string? SearchText, bool? IsActive, int? PageNumber, int? PageSize)
        {
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new
                {
                    Id = Id,
                    SearchText = SearchText,
                    IsActive = IsActive,
                    PageNumber = PageNumber,
                    PageSize = PageSize
                };
                return (await con.QueryAsync<dynamic>("GetProject", parameters, commandType: CommandType.StoredProcedure)).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<dynamic> InsertUpdateProject(ProjectModel model)
        {
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new
                {
                    Id = model.Id,
                    Title = model.Title,
                    IsActive = model.IsActive,
                    UserId = Helper.UserId(_httpContextAccessor)
                };

                var resp = (await con.QueryAsync<dynamic>("InsertUpdateProject", parameters, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                model.Id = resp?.Id;
                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<dynamic> DeleteProject(int? Id, int? UserId)
        {
            var result = new Result();
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new
                {
                    Id = Id,
                    UserId = UserId ?? Helper.UserId(_httpContextAccessor)
                };
                await con.ExecuteAsync("DeleteProject", parameters, commandType: CommandType.StoredProcedure);
                result.IsSuccess = true;
                result.Message = "Project deleted successfully.";
                result.Status = "Success";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                result.IsSuccess = false;
            }
            return result;
        }

        public async Task<dynamic> GetProjectDocument(int? Id, int? ProjectId)
        {
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new { Id = Id, ProjectId = ProjectId };
                return (await con.QueryAsync<dynamic>("GetProjectDocument", parameters, commandType: CommandType.StoredProcedure)).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<dynamic> InsertProjectDocument(ProjectDocumentModel model)
        {
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new
                {
                    ProjectId = model.ProjectId,
                    DocumentTitle = model.DocumentTitle,
                    Url = model.Url,
                    UserId = Helper.UserId(_httpContextAccessor)
                };

                var resp = (await con.QueryAsync<dynamic>("InsertProjectDocument", parameters, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                model.Id = resp?.Id;
                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<dynamic> DeleteProjectDocument(int? Id, int? UserId)
        {
            var result = new Result();
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new
                {
                    Id = Id,
                    UserId = UserId ?? Helper.UserId(_httpContextAccessor)
                };
                await con.ExecuteAsync("DeleteProjectDocument", parameters, commandType: CommandType.StoredProcedure);
                result.IsSuccess = true;
                result.Message = "Document deleted successfully.";
                result.Status = "Success";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                result.IsSuccess = false;
            }
            return result;
        }

        public async Task<dynamic> GetNextInvoiceNumber(int? Year, string? Prefix)
        {
            try
            {
                using var con = _context.CreateConnection();
                return (await con.QueryAsync<dynamic>("GetNextInvoiceNumber",
                    new { Year = Year, Prefix = Prefix },
                    commandType: CommandType.StoredProcedure)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<dynamic> GetWarehouse(int? Id, string? SearchText, bool? IsActive, int? PageNumber, int? PageSize)
        {
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new
                {
                    Id = Id,
                    SearchText = SearchText,
                    IsActive = IsActive,
                    PageNumber = PageNumber,
                    PageSize = PageSize
                };
                return (await con.QueryAsync<dynamic>("GetWarehouse", parameters, commandType: CommandType.StoredProcedure)).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<dynamic> InsertUpdateWarehouse(WarehouseModel model)
        {
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new
                {
                    Id = model.Id,
                    Code = model.Code,
                    Name = model.Name,
                    Phone = model.Phone,
                    StreetAddress = model.StreetAddress,
                    BuildingNumber = model.BuildingNumber,
                    District = model.District,
                    City = model.City,
                    PostalCode = model.PostalCode,
                    IsActive = model.IsActive,
                    UserId = Helper.UserId(_httpContextAccessor)
                };

                var resp = (await con.QueryAsync<dynamic>("InsertUpdateWarehouse", parameters, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                model.Id = resp?.Id;
                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<dynamic> DeleteWarehouse(int? Id, int? UserId)
        {
            var result = new Result();
            try
            {
                using var con = _context.CreateConnection();
                var parameters = new
                {
                    Id = Id,
                    UserId = UserId ?? Helper.UserId(_httpContextAccessor)
                };
                await con.ExecuteAsync("DeleteWarehouse", parameters, commandType: CommandType.StoredProcedure);
                result.IsSuccess = true;
                result.Message = "Warehouse deleted successfully.";
                result.Status = "Success";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                result.IsSuccess = false;
            }
            return result;
        }
    }
}
