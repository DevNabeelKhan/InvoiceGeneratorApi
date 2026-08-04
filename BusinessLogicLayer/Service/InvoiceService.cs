using BusinessLogicLayer.Interfaces;
using BusinessObjectsLayer.DTOs;
using BusinessObjectsLayer.Entities;
using DataAccessLayer.Interface;

namespace BusinessLogicLayer.Service
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public InvoiceService(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public async Task<dynamic> GetInvoice(int? Id, string? SearchText, int? CustomerId, string? Status, bool? IsActive, int? PageNumber, int? PageSize)
        {
            return await _invoiceRepository.GetInvoice(Id, SearchText, CustomerId, Status, IsActive, PageNumber, PageSize);
        }

        public async Task<dynamic> SaveInvoice(InvoiceDto dto)
        {
            var invoice = MapToInvoiceModel(dto);

            // Recalculate totals from lines
            CalculateInvoiceTotals(invoice);

            var result = await _invoiceRepository.InsertUpdateInvoice(invoice);

            if (result == null)
                return null;

            // Save line items
            if (invoice.Products != null && invoice.Products.Any())
            {
                foreach (var item in invoice.Products)
                {
                    item.InvoiceId = invoice.Id;
                    await _invoiceRepository.InsertUpdateInvoiceProduct(item);
                }
            }

            return await _invoiceRepository.GetInvoice(invoice.Id, null, null, null, null, 1, 20);
        }

        public async Task<dynamic> DeleteInvoice(int? Id, int? UserId)
        {
            return await _invoiceRepository.DeleteInvoice(Id, UserId);
        }

        public async Task<dynamic> GetInvoiceProduct(int? Id, int? InvoiceId)
        {
            return await _invoiceRepository.GetInvoiceProduct(Id, InvoiceId);
        }

        public async Task<dynamic> GetCompany(int? Id)
        {
            return await _invoiceRepository.GetCompany(Id);
        }

        public async Task<dynamic> SaveCompany(CompanyModel model)
        {
            return await _invoiceRepository.InsertUpdateCompany(model);
        }

        public async Task<dynamic> GetCurrency(int? Id)
        {
            return await _invoiceRepository.GetCurrency(Id);
        }

        public async Task<dynamic> SaveCurrency(CurrencyModel model)
        {
            return await _invoiceRepository.InsertUpdateCurrency(model);
        }

        public async Task<dynamic> GetProject(int? Id, string? SearchText, bool? IsActive, int? PageNumber, int? PageSize)
        {
            return await _invoiceRepository.GetProject(Id, SearchText, IsActive, PageNumber, PageSize);
        }

        public async Task<dynamic> SaveProject(ProjectModel model)
        {
            return await _invoiceRepository.InsertUpdateProject(model);
        }

        public async Task<dynamic> DeleteProject(int? Id, int? UserId)
        {
            return await _invoiceRepository.DeleteProject(Id, UserId);
        }

        public async Task<dynamic> GetProjectDocument(int? Id, int? ProjectId)
        {
            return await _invoiceRepository.GetProjectDocument(Id, ProjectId);
        }

        public async Task<dynamic> SaveProjectDocument(ProjectDocumentModel model)
        {
            return await _invoiceRepository.InsertProjectDocument(model);
        }

        public async Task<dynamic> DeleteProjectDocument(int? Id, int? UserId)
        {
            return await _invoiceRepository.DeleteProjectDocument(Id, UserId);
        }

        private InvoiceModel MapToInvoiceModel(InvoiceDto dto)
        {
            return new InvoiceModel
            {
                Id = dto.Id,
                CustomerId = dto.CustomerId,
                CompanyId = dto.CompanyId,
                CurrencyId = dto.CurrencyId,
                InvoiceDate = dto.InvoiceDate,
                DueDate = dto.DueDate,
                Reference = dto.Reference,
                PurchaseOrderNumber = dto.PurchaseOrderNumber,
                ProjectId = dto.ProjectId,
                PricesIncludeTax = dto.PricesIncludeTax,
                Notes = dto.Notes,
                ExchangeRate = dto.ExchangeRate,
                DiscountPercentage = dto.DiscountPercentage,
                DiscountAmount = dto.DiscountAmount,
                RetentionPercentage = dto.RetentionPercentage,
                RetentionAmount = dto.RetentionAmount,
                RoundOffAmount = dto.RoundOffAmount,
                Products = dto.Products?.Select(p => new InvoiceProductModel
                {
                    Id = p.Id,
                    InvoiceId = p.InvoiceId,
                    ProductId = p.ProductId,
                    Description = p.Description,
                    Unit = p.Unit,
                    Quantity = p.Quantity,
                    Price = p.Price,
                    DiscountPercentage = p.DiscountPercentage,
                    DiscountAmount = p.DiscountAmount,
                    TaxRate = p.TaxRate,
                    TaxableAmount = p.TaxableAmount,
                    VATAmount = p.VATAmount,
                    LineTotal = p.LineTotal,
                    AccountId = p.AccountId,
                    CostCenterId = p.CostCenterId,
                    RevenueRecognitionId = p.RevenueRecognitionId,
                    SortOrder = p.SortOrder
                }).ToList() ?? new List<InvoiceProductModel>()
            };
        }

        private void CalculateInvoiceTotals(InvoiceModel invoice)
        {
            decimal subtotal = 0;
            decimal totalVat = 0;
            var pricesIncludeTax = invoice.PricesIncludeTax ?? false;

            foreach (var line in invoice.Products)
            {
                var qty = line.Quantity ?? 0;
                var price = line.Price ?? 0;
                var lineAmount = qty * price;

                var discountPct = line.DiscountPercentage ?? 0;
                var discountAmt = line.DiscountAmount ?? 0;

                if (discountPct > 0 && discountAmt == 0)
                    discountAmt = Math.Round(lineAmount * discountPct / 100, 2);

                var amountAfterDiscount = lineAmount - discountAmt;
                if (amountAfterDiscount < 0) amountAfterDiscount = 0;

                var rate = line.TaxRate ?? 0;
                decimal taxable, vat, lineTotal;

                if (pricesIncludeTax)
                {
                    // amountAfterDiscount is tax-inclusive (gross)
                    taxable = rate > 0 ? Math.Round(amountAfterDiscount / (1 + rate / 100), 2) : amountAfterDiscount;
                    vat = Math.Round(amountAfterDiscount - taxable, 2);
                    lineTotal = amountAfterDiscount;
                }
                else
                {
                    taxable = amountAfterDiscount;
                    vat = Math.Round(taxable * rate / 100, 2);
                    lineTotal = taxable + vat;
                }

                line.TaxableAmount = taxable;
                line.VATAmount = vat;
                line.LineTotal = lineTotal;
                line.DiscountAmount = discountAmt;

                subtotal += lineAmount;
                totalVat += vat;
            }

            var discountPercentage = invoice.DiscountPercentage ?? 0;
            var invoiceDiscount = invoice.DiscountAmount ?? 0;

            if (discountPercentage > 0 && invoiceDiscount == 0)
                invoiceDiscount = Math.Round(subtotal * discountPercentage / 100, 2);

            var afterLineDiscounts = invoice.Products.Sum(p => (p.TaxableAmount ?? 0));
            var taxableTotal = afterLineDiscounts - invoiceDiscount;
            if (taxableTotal < 0) taxableTotal = 0;

            totalVat = invoice.Products.Sum(p => p.VATAmount ?? 0);

            var retentionAmount = invoice.RetentionAmount ?? 0;
            if ((invoice.RetentionPercentage ?? 0) > 0 && retentionAmount == 0)
                retentionAmount = Math.Round((taxableTotal + totalVat) * (invoice.RetentionPercentage ?? 0) / 100, 2);

            var grandTotal = taxableTotal + totalVat - retentionAmount + (invoice.RoundOffAmount ?? 0);

            invoice.Subtotal = Math.Round(subtotal, 2);
            invoice.DiscountAmount = Math.Round(invoiceDiscount, 2);
            invoice.TaxAmount = Math.Round(totalVat, 2);
            invoice.RetentionAmount = Math.Round(retentionAmount, 2);
            invoice.GrandTotal = Math.Round(grandTotal, 2);
        }
    }
}
