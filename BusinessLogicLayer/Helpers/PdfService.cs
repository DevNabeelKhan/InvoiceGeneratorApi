using BusinessObjectsLayer.Entities;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using System.Globalization;

namespace BusinessLogicLayer.Helpers
{
    public interface IPdfService
    {
        Task<byte[]> GenerateInvoicePdfAsync(InvoiceModel invoice, CompanyModel? company);
        string GenerateInvoiceHtml(InvoiceModel invoice, CompanyModel? company);
    }

    public class PdfService : IPdfService
    {
        public async Task<byte[]> GenerateInvoicePdfAsync(InvoiceModel invoice, CompanyModel? company)
        {
            var html = GenerateInvoiceHtml(invoice, company);

            var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();

            await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                Args = new[] { "--no-sandbox", "--disable-setuid-sandbox" }
            });

            await using var page = await browser.NewPageAsync();
            await page.SetContentAsync(html, new NavigationOptions { WaitUntil = new[] { WaitUntilNavigation.Networkidle0 } });

            var pdf = await page.PdfDataAsync(new PdfOptions
            {
                Format = PaperFormat.A4,
                PrintBackground = true,
                MarginOptions = new MarginOptions
                {
                    Top = "10mm",
                    Right = "10mm",
                    Bottom = "10mm",
                    Left = "10mm"
                }
            });

            return pdf;
        }

        public string GenerateInvoiceHtml(InvoiceModel invoice, CompanyModel? company)
        {
            var culture = new CultureInfo("en-SA");
            var qrBase64 = invoice.GeneratedQRCode;

            if (!string.IsNullOrEmpty(qrBase64))
            {
                try
                {
                    var qrImage = ZatcaQrHelper.GenerateQrImageBase64(qrBase64, 4);
                    qrBase64 = $"data:image/png;base64,{qrImage}";
                }
                catch { qrBase64 = null; }
            }

            var logoHtml = !string.IsNullOrEmpty(company?.LogoPath)
                ? $"<img src=\"{company.LogoPath}\" class=\"company-logo\" alt=\"logo\" />"
                : "<div class=\"company-logo-placeholder\"></div>";

            var stampHtml = !string.IsNullOrEmpty(company?.StampPath)
                ? $"<img src=\"{company.StampPath}\" class=\"stamp\" alt=\"stamp\" />"
                : "";

            var customerName = invoice.CustomerName ?? string.Empty;
            var customerArabic = invoice.CustomerArabicName ?? string.Empty;
            var companyName = invoice.CompanyName ?? company?.Name ?? string.Empty;
            var companyArabic = invoice.CompanyArabicName ?? company?.ArabicName ?? string.Empty;
            var companyVat = invoice.CompanyVATNumber ?? company?.VATNumber ?? string.Empty;

            var rowsHtml = string.Empty;
            int index = 1;
            foreach (var line in invoice.Products?.Where(p => p.IsActive) ?? new List<InvoiceProductModel>())
            {
                rowsHtml += $@"
                <tr>
                    <td class='text-center'>{index}</td>
                    <td>{line.Description}</td>
                    <td class='text-center'>{line.Unit}</td>
                    <td class='text-end'>{line.Quantity:N2}</td>
                    <td class='text-end'>{line.Price:N2}</td>
                    <td class='text-end'>{line.DiscountAmount:N2}</td>
                    <td class='text-end'>{line.TaxRate:N0}%</td>
                    <td class='text-end'>{line.TaxableAmount:N2}</td>
                    <td class='text-end'>{line.VATAmount:N2}</td>
                    <td class='text-end'>{line.LineTotal:N2}</td>
                </tr>";
                index++;
            }

            var html = $@"<!DOCTYPE html>
<html lang='ar' dir='rtl'>
<head>
    <meta charset='UTF-8'>
    <title>Tax Invoice / فاتورة ضريبية</title>
    <style>
        @page {{ size: A4; margin: 10mm; }}
        body {{ font-family: 'Segoe UI', Tahoma, Arial, sans-serif; font-size: 11px; color: #333; margin: 0; padding: 0; }}
        .invoice-container {{ width: 100%; max-width: 210mm; margin: 0 auto; border: 1px solid #ccc; padding: 15px; box-sizing: border-box; }}
        .header {{ display: flex; justify-content: space-between; align-items: flex-start; border-bottom: 2px solid #1e3a8a; padding-bottom: 12px; margin-bottom: 12px; }}
        .header-left {{ text-align: right; width: 60%; }}
        .header-right {{ text-align: left; width: 35%; }}
        .company-name {{ font-size: 18px; font-weight: bold; color: #1e3a8a; }}
        .company-name-ar {{ font-size: 16px; font-weight: bold; color: #1e3a8a; }}
        .vat-label {{ font-size: 10px; color: #666; }}
        .invoice-title {{ font-size: 22px; font-weight: bold; color: #1e3a8a; margin-bottom: 4px; }}
        .invoice-title-ar {{ font-size: 18px; font-weight: bold; color: #1e3a8a; margin-bottom: 8px; }}
        .invoice-meta {{ margin-top: 6px; font-size: 10px; }}
        .meta-row {{ display: flex; justify-content: space-between; margin-bottom: 3px; }}
        .meta-label {{ font-weight: bold; color: #555; }}
        .company-logo {{ max-width: 90px; max-height: 70px; }}
        .two-col {{ display: flex; justify-content: space-between; margin: 10px 0; }}
        .box {{ border: 1px solid #ddd; padding: 8px; width: 48%; }}
        .box-title {{ font-weight: bold; margin-bottom: 6px; color: #1e3a8a; border-bottom: 1px solid #eee; padding-bottom: 4px; }}
        table.invoice-table {{ width: 100%; border-collapse: collapse; margin: 12px 0; }}
        table.invoice-table th {{ background: #1e3a8a; color: #fff; padding: 6px; text-align: center; font-size: 10px; }}
        table.invoice-table td {{ border-bottom: 1px solid #ddd; padding: 6px; vertical-align: top; }}
        .text-center {{ text-align: center; }}
        .text-end {{ text-align: right; }}
        .totals {{ display: flex; justify-content: flex-end; margin: 12px 0; }}
        .totals-table {{ width: 280px; border-collapse: collapse; }}
        .totals-table td {{ padding: 5px; border-bottom: 1px solid #eee; }}
        .totals-table .grand {{ font-weight: bold; font-size: 13px; background: #f3f4f6; }}
        .footer {{ display: flex; justify-content: space-between; margin-top: 20px; border-top: 2px solid #1e3a8a; padding-top: 12px; }}
        .bank-details {{ font-size: 10px; line-height: 1.6; }}
        .qr-section {{ text-align: center; }}
        .qr-section img {{ width: 110px; height: 110px; }}
        .stamp-section {{ text-align: center; margin-top: 8px; }}
        .stamp-section img {{ max-width: 90px; }}
        .footer-note {{ text-align: center; font-size: 9px; color: #666; margin-top: 12px; }}
    </style>
</head>
<body>
<div class='invoice-container'>
    <div class='header'>
        <div class='header-left'>
            <div class='company-name'>{companyName}</div>
            <div class='company-name-ar'>{companyArabic}</div>
            <div>{invoice.CompanyAddress ?? company?.Address}</div>
            <div>{invoice.CompanyArabicAddress ?? company?.ArabicAddress}</div>
            <div class='vat-label'>VAT / الضريبة: {companyVat}</div>
        </div>
        <div class='header-right'>
            {logoHtml}
            <div class='invoice-title'>Tax Invoice</div>
            <div class='invoice-title-ar'>فاتورة ضريبية</div>
            <div class='invoice-meta'>
                <div class='meta-row'><span class='meta-label'>Invoice # / رقم الفاتورة</span><span>{invoice.InvoiceNumber}</span></div>
                <div class='meta-row'><span class='meta-label'>Issue Date / تاريخ الإصدار</span><span>{invoice.InvoiceDate:yyyy-MM-dd}</span></div>
                <div class='meta-row'><span class='meta-label'>Due Date / تاريخ الاستحقاق</span><span>{invoice.DueDate:yyyy-MM-dd}</span></div>
                <div class='meta-row'><span class='meta-label'>Reference / المرجع</span><span>{invoice.Reference}</span></div>
                <div class='meta-row'><span class='meta-label'>PO / أمر شراء</span><span>{invoice.PurchaseOrderNumber}</span></div>
            </div>
        </div>
    </div>

    <div class='two-col'>
        <div class='box'>
            <div class='box-title'>Bill To / العميل</div>
            <div><strong>{customerName}</strong></div>
            <div>{customerArabic}</div>
            <div>{invoice.CustomerAddress}</div>
            <div>{invoice.CustomerArabicAddress}</div>
            <div>VAT: {invoice.CustomerVATNumber}</div>
        </div>
        <div class='box'>
            <div class='box-title'>Project / المشروع</div>
            <div>{invoice.ProjectName}</div>
        </div>
    </div>

    <table class='invoice-table'>
        <thead>
            <tr>
                <th>#</th>
                <th>Description / البيان</th>
                <th>Unit</th>
                <th>Qty</th>
                <th>Price</th>
                <th>Discount</th>
                <th>VAT %</th>
                <th>Taxable</th>
                <th>VAT</th>
                <th>Total</th>
            </tr>
        </thead>
        <tbody>
            {rowsHtml}
        </tbody>
    </table>

    <div class='totals'>
        <table class='totals-table'>
            <tr><td>Subtotal / الإجمالي</td><td class='text-end'>{invoice.Subtotal:N2}</td></tr>
            <tr><td>Discount / الخصم</td><td class='text-end'>{invoice.DiscountAmount:N2}</td></tr>
            <tr><td>VAT / ضريبة القيمة المضافة</td><td class='text-end'>{invoice.TaxAmount:N2}</td></tr>
            <tr><td>Retention / الاستقطاع</td><td class='text-end'>{invoice.RetentionAmount:N2}</td></tr>
            <tr class='grand'><td>Grand Total / الإجمالي الكلي</td><td class='text-end'>{invoice.GrandTotal:N2} {invoice.CurrencyCode}</td></tr>
        </table>
    </div>

    <div class='footer'>
        <div class='bank-details'>
            <strong>Bank Details / تفاصيل البنك</strong><br/>
            Beneficiary: {company?.BeneficiaryName}<br/>
            Bank: {company?.BankName}<br/>
            Account: {company?.BankAccountNumber}<br/>
            IBAN: {company?.IBAN}<br/>
            SWIFT: {company?.SwiftCode}
        </div>
        <div class='qr-section'>
            {(string.IsNullOrEmpty(qrBase64) ? "" : $"<img src='{qrBase64}' alt='QR'/>")}
            <div class='stamp-section'>{stampHtml}</div>
        </div>
    </div>

    <div class='footer-note'>This invoice was generated electronically and is valid without signature.</div>
</div>
</body>
</html>";

            return html;
        }
    }
}
