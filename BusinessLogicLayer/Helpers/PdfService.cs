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

            // Use the TLV already persisted on the invoice if available; otherwise generate it on the
            // fly from the invoice/company data so the QR code always renders in the PDF, even if
            // "Generate QR" was never explicitly saved back to the invoice record.
            var tlvBase64 = invoice.GeneratedQRCode;
            if (string.IsNullOrEmpty(tlvBase64))
            {
                var sellerName = invoice.CompanyName ?? company?.Title ?? string.Empty;
                var vatNumber = invoice.CompanyVATNumber ?? company?.VATNumber ?? string.Empty;
                var invoiceTotal = invoice.GrandTotal ?? 0;
                var vatTotal = invoice.TaxAmount ?? 0;
                tlvBase64 = ZatcaQrHelper.GenerateBase64(sellerName, vatNumber, invoice.InvoiceDate ?? DateTime.UtcNow, invoiceTotal, vatTotal);
            }

            string? qrBase64 = null;
            try
            {
                var qrImage = ZatcaQrHelper.GenerateQrImageBase64(tlvBase64, 4);
                qrBase64 = $"data:image/png;base64,{qrImage}";
            }
            catch { qrBase64 = null; }

            var logoSrc = !string.IsNullOrEmpty(invoice.LogoPath) ? invoice.LogoPath
                : !string.IsNullOrEmpty(company?.LogoPath) ? company!.LogoPath : company?.LogoUrl;

            var logoHtml = !string.IsNullOrEmpty(logoSrc)
                ? $"<img src=\"{logoSrc}\" class=\"preview-logo\" alt=\"logo\" />"
                : "";

            var stampSrc = !string.IsNullOrEmpty(invoice.StampPath) ? invoice.StampPath : company?.StampPath;
            var stampHtml = !string.IsNullOrEmpty(stampSrc)
                ? $"<img src=\"{stampSrc}\" class=\"preview-stamp\" alt=\"stamp\" />"
                : "";

            var customerName = invoice.CustomerName ?? string.Empty;
            var companyName = invoice.CompanyName ?? company?.Title ?? string.Empty;
            var companyArabic = invoice.CompanyArabicName ?? company?.ArabicName ?? string.Empty;
            var companyVat = invoice.CompanyVATNumber ?? company?.VATNumber ?? string.Empty;
            var companyAddress = invoice.CompanyAddress ?? company?.Address ?? string.Empty;
            var companyArabicAddress = invoice.CompanyArabicAddress ?? company?.ArabicAddress ?? string.Empty;
            var currency = !string.IsNullOrEmpty(invoice.CurrencySymbol) ? invoice.CurrencySymbol : invoice.CurrencyCode;

            var lines = invoice.Products?.Where(p => p.IsActive != false).OrderBy(p => p.SortOrder).ToList() ?? new List<InvoiceProductModel>();
            var hasLineDiscounts = lines.Any(l => (l.DiscountAmount ?? 0) > 0 || (l.DiscountPercentage ?? 0) > 0);

            var rowsHtml = string.Empty;
            int index = 1;
            foreach (var line in lines)
            {
                var discountCell = hasLineDiscounts
                    ? $@"<td class='col-num-val'>{line.DiscountAmount:N2}{(line.DiscountPercentage > 0 ? $"<span class='sub-rate'>{line.DiscountPercentage:N0}%</span>" : "")}</td>"
                    : "";
                rowsHtml += $@"
                <tr>
                    <td class='col-num'>{index}</td>
                    <td class='col-desc'>{line.Description}</td>
                    <td class='col-num-val'>{line.Quantity:N2}</td>
                    <td class='col-num-val'>{line.Price:N2}</td>
                    <td class='col-num-val'>{line.TaxableAmount:N2}</td>
                    <td class='col-num-val'>{line.VATAmount:N2}{(line.TaxRate > 0 ? $"<span class='sub-rate'>{line.TaxRate:N0}%</span>" : "")}</td>
                    {discountCell}
                    <td class='col-num-val'>{line.LineTotal:N2}</td>
                </tr>";
                index++;
            }

            var discountHeaderCell = hasLineDiscounts ? "<th class='col-num-val'>Discount <span class='ar'>خصم</span></th>" : "";

            var discountTotalRow = (invoice.DiscountAmount ?? 0) > 0
                ? $@"<tr><td class='t-label'>Discount <span class='ar'>الخصم</span></td><td class='t-cur'>{currency}</td><td class='t-amt'>{invoice.DiscountAmount:N2}</td></tr>"
                : "";

            var retentionTotalRow = (invoice.RetentionAmount ?? 0) > 0
                ? $@"<tr><td class='t-label'>Retention <span class='ar'>الاحتجاز</span></td><td class='t-cur'>{currency}</td><td class='t-amt'>{invoice.RetentionAmount:N2}</td></tr>"
                : "";

            var notesHtml = !string.IsNullOrEmpty(invoice.Notes)
                ? $@"<div class='preview-notes'><strong>Notes <span class='ar'>ملاحظات</span></strong><p>{invoice.Notes}</p></div>"
                : "";

            var qrHtml = !string.IsNullOrEmpty(qrBase64)
                ? $"<img src='{qrBase64}' alt='QR'/>"
                : "<div class='preview-qr-placeholder'><span>QR not available</span></div>";

            var referenceRow = !string.IsNullOrEmpty(invoice.Reference)
                ? $@"<div class='meta-row-inner'><span class='meta-key'>Reference</span><span class='meta-val'>{invoice.Reference}</span><span class='meta-key-ar'>رقم المرجع</span></div>"
                : "";
            var dueDateRow = invoice.DueDate.HasValue
                ? $@"<div class='meta-row-inner'><span class='meta-key'>Due date</span><span class='meta-val'>{invoice.DueDate:yyyy-MM-dd}</span><span class='meta-key-ar'>تاريخ الاستحقاق</span></div>"
                : "";
            var projectRow = !string.IsNullOrEmpty(invoice.ProjectName)
                ? $@"<div class='meta-row-inner'><span class='meta-key'>Project</span><span class='meta-val'>{invoice.ProjectName}</span><span class='meta-key-ar'>المشروع</span></div>"
                : "";
            var warehouseRow = !string.IsNullOrEmpty(invoice.WarehouseName)
                ? $@"<div class='meta-row-inner'><span class='meta-key'>Warehouse</span><span class='meta-val'>{invoice.WarehouseName}</span><span class='meta-key-ar'>المستودع</span></div>"
                : "";
            var customerPhoneRowEn = !string.IsNullOrEmpty(invoice.CustomerPhone) ? $"<div class='preview-small'>{invoice.CustomerPhone}</div>" : "";
            var customerPhoneMeta = !string.IsNullOrEmpty(invoice.CustomerPhone)
                ? $@"<div class='meta-row'><span class='meta-key'>Phone</span><span class='meta-val'>{invoice.CustomerPhone}</span><span class='meta-key-ar'>الهاتف</span></div>"
                : "";

            var html = $@"<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <title>Tax Invoice / فاتورة ضريبية</title>
    <style>
        @page {{ size: A4; margin: 10mm; }}
        * {{ box-sizing: border-box; }}
        body {{ font-family: 'Segoe UI', Tahoma, Arial, sans-serif; margin: 0; padding: 0; background: #fff; }}
        .ar {{ font-family: 'Segoe UI', Tahoma, sans-serif; direction: rtl; unicode-bidi: isolate; color: inherit; font-weight: inherit; }}

        .preview-card {{ background: #fff; padding: 4px 6px 4px; font-size: 13px; color: #1d2939; line-height: 1.5; }}

        .preview-header {{ display: flex; justify-content: space-between; align-items: flex-start; gap: 12px; }}
        .preview-header-left {{ flex: 1; }}
        .preview-header-right {{ flex: 1; text-align: right; direction: rtl; }}
        .preview-company-name, .preview-company-name-ar {{ font-weight: 700; color: #101828; font-size: 17px; margin-bottom: 3px; }}
        .preview-small {{ font-size: 11px; color: #2f4368; line-height: 1.55; }}
        .preview-logo {{ max-width: 70px; max-height: 60px; object-fit: contain; flex-shrink: 0; align-self: center; }}

        .preview-divider {{ height: 1px; background: #e4e7ec; margin: 16px 0 18px; }}

        .preview-title-row {{ display: flex; align-items: baseline; justify-content: center; gap: 12px; margin-bottom: 18px; }}
        .title-en {{ font-weight: 700; color: #1f2d45; font-size: 26px; letter-spacing: -0.2px; }}
        .title-ar {{ font-weight: 700; color: #1f2d45; font-size: 24px; direction: rtl; }}

        .preview-meta-box {{ border: 1px solid #dfe3e8; border-radius: 2px; margin-bottom: 18px; background: #fff; }}
        .meta-row {{ display: grid; grid-template-columns: 110px 1fr auto; align-items: baseline; gap: 8px; padding: 6px 14px; font-size: 11.5px; border-bottom: 1px solid #e8ebef; }}
        .meta-row-split {{ display: grid; grid-template-columns: 1fr 1fr; }}
        .meta-split-col {{ padding: 6px 14px; }}
        .meta-split-col + .meta-split-col {{ border-left: 1px solid #e8ebef; }}
        .meta-row-inner {{ display: grid; grid-template-columns: 92px 1fr auto; align-items: baseline; gap: 8px; padding: 2px 0; font-size: 11.5px; }}
        .meta-key {{ color: #1f2d45; font-weight: 600; white-space: nowrap; }}
        .meta-val {{ color: #2f4368; text-align: center; }}
        .meta-key-ar {{ color: #3f5878; font-weight: 500; font-size: 11px; direction: rtl; text-align: right; white-space: nowrap; }}

        .preview-lines-table {{ width: 100%; border-collapse: collapse; border: 1px solid #dfe3e8; margin-bottom: 14px; }}
        .preview-lines-table th {{ background: #fff; color: #1f2d45; padding: 8px 6px; font-size: 11px; font-weight: 600; line-height: 1.4; text-align: right; vertical-align: top; border-bottom: 1px solid #dfe3e8; }}
        .preview-lines-table th.col-num, .preview-lines-table td.col-num {{ width: 24px; text-align: left; }}
        .preview-lines-table th.col-desc, .preview-lines-table td.col-desc {{ text-align: left; }}
        .preview-lines-table th.col-num-val, .preview-lines-table td.col-num-val {{ text-align: right; white-space: nowrap; }}
        .preview-lines-table th .ar {{ display: block; font-size: 10px; font-weight: 500; color: #3f5878; margin-top: 1px; }}
        .preview-lines-table td {{ padding: 9px 6px; font-size: 11.5px; color: #2f4368; vertical-align: top; }}
        .sub-rate {{ display: block; font-size: 9.5px; color: #8fa0b8; margin-top: 1px; }}

        .preview-qr-totals-row {{ display: flex; justify-content: space-between; align-items: flex-start; gap: 14px; margin-bottom: 14px; }}
        .preview-qr {{ flex-shrink: 0; max-width: 170px; }}
        .preview-qr img {{ width: 100px; height: 100px; }}
        .preview-qr-placeholder {{ width: 100px; height: 100px; border: 1px dashed #d0d5dd; border-radius: 4px; display: flex; align-items: center; justify-content: center; color: #98a2b3; font-size: 9px; text-align: center; }}
        .qr-caption {{ font-size: 9px; color: #8fa0b8; line-height: 1.45; margin: 5px 0 0; }}
        .qr-caption .ar {{ display: block; margin-top: 1px; }}

        .preview-totals-table {{ flex: 1; max-width: 320px; border-collapse: collapse; margin-left: auto; }}
        .preview-totals-table td {{ padding: 5px 0; font-size: 12px; }}
        .preview-totals-table td.t-label {{ text-align: right; color: #1f2d45; font-weight: 600; white-space: nowrap; }}
        .preview-totals-table td.t-label .ar {{ margin-left: 8px; font-weight: 500; color: #3f5878; }}
        .preview-totals-table td.t-cur {{ text-align: center; color: #3f5878; width: 38px; font-size: 11px; }}
        .preview-totals-table td.t-amt {{ text-align: right; font-weight: 700; color: #1f2d45; width: 78px; }}
        .preview-totals-table tr.grand td {{ font-size: 13.5px; padding-top: 7px; }}
        .preview-totals-table tr.grand td.t-label, .preview-totals-table tr.grand td.t-amt {{ font-weight: 700; color: #101828; }}

        .preview-notes {{ border-top: 1px solid #eef0f3; padding-top: 10px; margin-bottom: 10px; font-size: 11.5px; color: #475467; }}
        .preview-notes p {{ margin: 4px 0 0; white-space: pre-line; }}

        .preview-stamp {{ max-width: 70px; margin-top: 10px; }}
    </style>
</head>
<body>
<div class='preview-card'>

    <div class='preview-header'>
        <div class='preview-header-left'>
            <div class='preview-company-name'>{companyName}</div>
            <div class='preview-small'>{companyAddress}</div>
            {customerPhoneRowEn}
            <div class='preview-small'>VAT number {companyVat}</div>
        </div>
        {logoHtml}
        <div class='preview-header-right'>
            <div class='preview-company-name-ar'>{companyArabic}</div>
            <div class='preview-small'>{companyArabicAddress}</div>
            {customerPhoneRowEn}
            <div class='preview-small'>رقم التسجيل الضريبي {companyVat}</div>
        </div>
    </div>

    <div class='preview-divider'></div>

    <div class='preview-title-row'>
        <span class='title-en'>Tax Invoice</span>
        <span class='title-ar'>فاتورة ضريبية</span>
    </div>

    <div class='preview-meta-box'>
        <div class='meta-row'>
            <span class='meta-key'>Customer</span>
            <span class='meta-val'>{customerName}</span>
            <span class='meta-key-ar'>العميل</span>
        </div>
        <div class='meta-row'>
            <span class='meta-key'>Address</span>
            <span class='meta-val'>{invoice.CustomerAddress}</span>
            <span class='meta-key-ar'>العنوان</span>
        </div>
        {customerPhoneMeta}
        <div class='meta-row'>
            <span class='meta-key'>VAT number</span>
            <span class='meta-val'>{invoice.CustomerVATNumber}</span>
            <span class='meta-key-ar'>رقم التسجيل الضريبي</span>
        </div>
        <div class='meta-row-split'>
            <div class='meta-split-col'>
                <div class='meta-row-inner'>
                    <span class='meta-key'>Invoice number</span>
                    <span class='meta-val'>{invoice.InvoiceNumber}</span>
                    <span class='meta-key-ar'>رقم الفاتورة</span>
                </div>
                {referenceRow}
                {dueDateRow}
            </div>
            <div class='meta-split-col'>
                <div class='meta-row-inner'>
                    <span class='meta-key'>Date</span>
                    <span class='meta-val'>{invoice.InvoiceDate:yyyy-MM-dd}</span>
                    <span class='meta-key-ar'>التاريخ</span>
                </div>
                {projectRow}
                {warehouseRow}
            </div>
        </div>
    </div>

    <table class='preview-lines-table'>
        <thead>
            <tr>
                <th class='col-num'>#</th>
                <th class='col-desc'>Description <span class='ar'>الوصف</span></th>
                <th class='col-num-val'>Qty <span class='ar'>الكمية</span></th>
                <th class='col-num-val'>Price <span class='ar'>السعر</span></th>
                <th class='col-num-val'>Taxable amount <span class='ar'>المبلغ الخاضع للضريبة</span></th>
                <th class='col-num-val'>VAT amount <span class='ar'>القيمة المضافة</span></th>
                {discountHeaderCell}
                <th class='col-num-val'>Line amount <span class='ar'>المجموع</span></th>
            </tr>
        </thead>
        <tbody>
            {rowsHtml}
        </tbody>
    </table>

    <div class='preview-qr-totals-row'>
        <div class='preview-qr'>
            {qrHtml}
            <p class='qr-caption'>
                This QR code is encoded as per ZATCA e-invoicing requirements
                <span class='ar'>تم ترميز هذا الرمز وفقاً لمتطلبات هيئة الزكاة والضريبة والجمارك للفوترة الإلكترونية</span>
            </p>
        </div>

        <table class='preview-totals-table'>
            <tr>
                <td class='t-label'>Subtotal <span class='ar'>المجموع الفرعي</span></td>
                <td class='t-cur'>{currency}</td>
                <td class='t-amt'>{invoice.Subtotal:N2}</td>
            </tr>
            {discountTotalRow}
            <tr>
                <td class='t-label'>Total VAT <span class='ar'>إجمالي ضريبة القيمة المضافة</span></td>
                <td class='t-cur'>{currency}</td>
                <td class='t-amt'>{invoice.TaxAmount:N2}</td>
            </tr>
            {retentionTotalRow}
            <tr class='grand'>
                <td class='t-label'>Total <span class='ar'>المجموع شامل القيمة المضافة</span></td>
                <td class='t-cur'>{currency}</td>
                <td class='t-amt'>{invoice.GrandTotal:N2}</td>
            </tr>
        </table>
    </div>

    {notesHtml}

    {stampHtml}
</div>
</body>
</html>";

            return html;
        }
    }
}
