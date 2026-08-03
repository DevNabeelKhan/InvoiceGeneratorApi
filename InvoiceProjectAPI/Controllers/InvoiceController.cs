using AutoMapper;
using BusinessLogicLayer.Helpers;
using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Response;
using BusinessObjectsLayer.DTOs;
using BusinessObjectsLayer.Entities;
using DataAccessLayer.Shared.Helper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConvergeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;
        private readonly IPdfService _pdfService;
        private readonly IXmlService _xmlService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public InvoiceController(IInvoiceService invoiceService, IPdfService pdfService, IXmlService xmlService, IMapper mapper, IConfiguration configuration)
        {
            _invoiceService = invoiceService;
            _pdfService = pdfService;
            _xmlService = xmlService;
            _mapper = mapper;
            _configuration = configuration;
        }

        [HttpGet("GetInvoice")]
        public async Task<IActionResult> GetInvoice(int? Id, string? SearchText, int? CustomerId, string? Status, bool? IsActive, int? PageNumber = 1, int? PageSize = 20)
        {
            try
            {
                var result = await _invoiceService.GetInvoice(Id, SearchText, CustomerId, Status, IsActive, PageNumber, PageSize);
                return Ok(ResponseHelper.GetSuccessResponse(result));
            }
            catch (Exception ex)
            {
                return Ok(ResponseHelper.GetFailureResponse(ex.Message));
            }
        }

        [HttpPost("InsertUpdateInvoice")]
        public async Task<IActionResult> InsertUpdateInvoice(InvoiceDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Ok(ResponseHelper.GetValidationFailureResponse(ModelState));

                var result = await _invoiceService.SaveInvoice(dto);
                return Ok(ResponseHelper.GetSuccessResponse(result));
            }
            catch (Exception ex)
            {
                return Ok(ResponseHelper.GetFailureResponse(ex.Message));
            }
        }

        [HttpGet("DeleteInvoice")]
        public async Task<IActionResult> DeleteInvoice(int? Id)
        {
            try
            {
                int? userId = null;
                var userIdClaim = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out var parsedUserId))
                    userId = parsedUserId;

                var result = await _invoiceService.DeleteInvoice(Id, userId);
                return Ok(ResponseHelper.GetSuccessResponse(result));
            }
            catch (Exception ex)
            {
                return Ok(ResponseHelper.GetFailureResponse(ex.Message));
            }
        }

        [HttpGet("GetInvoiceProduct")]
        public async Task<IActionResult> GetInvoiceProduct(int? Id, int? InvoiceId)
        {
            try
            {
                var result = await _invoiceService.GetInvoiceProduct(Id, InvoiceId);
                return Ok(ResponseHelper.GetSuccessResponse(result));
            }
            catch (Exception ex)
            {
                return Ok(ResponseHelper.GetFailureResponse(ex.Message));
            }
        }

        [HttpGet("GetCompany")]
        public async Task<IActionResult> GetCompany(int? Id)
        {
            try
            {
                var result = await _invoiceService.GetCompany(Id);
                return Ok(ResponseHelper.GetSuccessResponse(result));
            }
            catch (Exception ex)
            {
                return Ok(ResponseHelper.GetFailureResponse(ex.Message));
            }
        }

        [HttpPost("InsertUpdateCompany")]
        public async Task<IActionResult> InsertUpdateCompany(CompanyModel model)
        {
            try
            {
                var result = await _invoiceService.SaveCompany(model);
                return Ok(ResponseHelper.GetSuccessResponse(result));
            }
            catch (Exception ex)
            {
                return Ok(ResponseHelper.GetFailureResponse(ex.Message));
            }
        }

        [HttpGet("GetCurrency")]
        public async Task<IActionResult> GetCurrency(int? Id)
        {
            try
            {
                var result = await _invoiceService.GetCurrency(Id);
                return Ok(ResponseHelper.GetSuccessResponse(result));
            }
            catch (Exception ex)
            {
                return Ok(ResponseHelper.GetFailureResponse(ex.Message));
            }
        }

        [HttpPost("InsertUpdateCurrency")]
        public async Task<IActionResult> InsertUpdateCurrency(CurrencyModel model)
        {
            try
            {
                var result = await _invoiceService.SaveCurrency(model);
                return Ok(ResponseHelper.GetSuccessResponse(result));
            }
            catch (Exception ex)
            {
                return Ok(ResponseHelper.GetFailureResponse(ex.Message));
            }
        }

        [HttpGet("PreviewInvoice")]
        public async Task<IActionResult> PreviewInvoice(int Id)
        {
            try
            {
                var invoice = await _invoiceService.GetInvoice(Id, null, null, null, null, 1, 20);
                return Ok(ResponseHelper.GetSuccessResponse(invoice));
            }
            catch (Exception ex)
            {
                return Ok(ResponseHelper.GetFailureResponse(ex.Message));
            }
        }

        [HttpPost("GeneratePdf")]
        public async Task<IActionResult> GeneratePdf(int Id)
        {
            try
            {
                var invoiceObj = await _invoiceService.GetInvoice(Id, null, null, null, null, 1, 20);
                if (invoiceObj == null)
                    return Ok(ResponseHelper.GetNotFoundResponse());

                var invoice = _mapper.Map<InvoiceModel>(invoiceObj);
                var companyList = await _invoiceService.GetCompany(invoice.CompanyId);
                object? firstCompany = companyList != null && ((IEnumerable<object>)companyList).Any() ? ((IEnumerable<object>)companyList).First() : null;
                var company = firstCompany != null ? _mapper.Map<CompanyModel>(firstCompany) : null;

                var pdfBytes = await _pdfService.GenerateInvoicePdfAsync(invoice, company);
                var fileName = $"Invoice-{invoice.InvoiceNumber}.pdf";

                var outputDir = Path.Combine(Directory.GetCurrentDirectory(), "GeneratedFiles");
                if (!Directory.Exists(outputDir)) Directory.CreateDirectory(outputDir);

                var filePath = Path.Combine(outputDir, fileName);
                await System.IO.File.WriteAllBytesAsync(filePath, pdfBytes);

                invoice.PDFPath = filePath;
                // Update the invoice record with PDF path (re-calculation not needed, but use service method?)

                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                return Ok(ResponseHelper.GetFailureResponse(ex.Message));
            }
        }

        [HttpGet("DownloadPdf")]
        public async Task<IActionResult> DownloadPdf(int Id)
        {
            var invoiceObj = await _invoiceService.GetInvoice(Id, null, null, null, null, 1, 20);
            if (invoiceObj == null)
                return Ok(ResponseHelper.GetNotFoundResponse());

            var invoice = _mapper.Map<InvoiceModel>(invoiceObj);
            var companyList = await _invoiceService.GetCompany(invoice.CompanyId);
            object? firstCompany = companyList != null && ((IEnumerable<object>)companyList).Any() ? ((IEnumerable<object>)companyList).First() : null;
            var company = firstCompany != null ? _mapper.Map<CompanyModel>(firstCompany) : null;

            var pdfBytes = await _pdfService.GenerateInvoicePdfAsync(invoice, company);
            return File(pdfBytes, "application/pdf", $"Invoice-{invoice.InvoiceNumber}.pdf");
        }

        [HttpGet("GenerateQr")]
        public async Task<IActionResult> GenerateQr(int Id)
        {
            try
            {
                var invoiceObj = await _invoiceService.GetInvoice(Id, null, null, null, null, 1, 20);
                if (invoiceObj == null)
                    return Ok(ResponseHelper.GetNotFoundResponse());

                var invoice = _mapper.Map<InvoiceModel>(invoiceObj);
                var companyList = await _invoiceService.GetCompany(invoice.CompanyId);
                object? firstCompany = companyList != null && ((IEnumerable<object>)companyList).Any() ? ((IEnumerable<object>)companyList).First() : null;
                var company = firstCompany != null ? _mapper.Map<CompanyModel>(firstCompany) : null;

                var sellerName = invoice.CompanyName ?? company?.Name ?? string.Empty;
                var vatNumber = invoice.CompanyVATNumber ?? company?.VATNumber ?? string.Empty;
                var invoiceTotal = invoice.GrandTotal ?? 0;
                var vatTotal = invoice.TaxAmount ?? 0;

                var qrBase64 = ZatcaQrHelper.GenerateBase64(sellerName, vatNumber, invoice.InvoiceDate ?? DateTime.UtcNow, invoiceTotal, vatTotal);
                var qrImageBase64 = $"data:image/png;base64,{ZatcaQrHelper.GenerateQrImageBase64(qrBase64)}";

                return Ok(ResponseHelper.GetSuccessResponse(new
                {
                    TlvBase64 = qrBase64,
                    QrImageBase64 = qrImageBase64
                }));
            }
            catch (Exception ex)
            {
                return Ok(ResponseHelper.GetFailureResponse(ex.Message));
            }
        }

        [HttpGet("GenerateXml")]
        public async Task<IActionResult> GenerateXml(int Id)
        {
            try
            {
                var invoiceObj = await _invoiceService.GetInvoice(Id, null, null, null, null, 1, 20);
                if (invoiceObj == null)
                    return Ok(ResponseHelper.GetNotFoundResponse());

                var invoice = _mapper.Map<InvoiceModel>(invoiceObj);
                var companyList = await _invoiceService.GetCompany(invoice.CompanyId);
                object? firstCompany = companyList != null && ((IEnumerable<object>)companyList).Any() ? ((IEnumerable<object>)companyList).First() : null;
                var company = firstCompany != null ? _mapper.Map<CompanyModel>(firstCompany) : null;

                var xmlDir = Path.Combine(Directory.GetCurrentDirectory(), "GeneratedFiles", "Xml");
                var path = _xmlService.SaveInvoiceXml(invoice, company, xmlDir);

                var xmlBytes = await System.IO.File.ReadAllBytesAsync(path);
                return File(xmlBytes, "application/xml", $"Invoice-{invoice.InvoiceNumber}.xml");
            }
            catch (Exception ex)
            {
                return Ok(ResponseHelper.GetFailureResponse(ex.Message));
            }
        }

        [HttpPost("UploadAttachment")]
        public async Task<IActionResult> UploadAttachment(int invoiceId, IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return Ok(ResponseHelper.GetFailureResponse("No file received."));

                var (fileName, fileUrl) = await Helper.AttachFileToS3Async(file, _configuration);

                if (string.IsNullOrEmpty(fileName))
                    return Ok(ResponseHelper.GetFailureResponse("Upload failed."));

                // Save attachment to DB via a direct repository call or extend service; for now return the URL.
                return Ok(ResponseHelper.GetSuccessResponse(new
                {
                    InvoiceId = invoiceId,
                    FileName = fileName,
                    FilePath = fileUrl,
                    FileSize = file.Length,
                    ContentType = file.ContentType
                }));
            }
            catch (Exception ex)
            {
                return Ok(ResponseHelper.GetFailureResponse(ex.Message));
            }
        }
    }
}
