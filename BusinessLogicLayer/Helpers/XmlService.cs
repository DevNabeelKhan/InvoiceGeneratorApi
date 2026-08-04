using BusinessObjectsLayer.Entities;
using System.Text;
using System.Xml.Linq;

namespace BusinessLogicLayer.Helpers
{
    public interface IXmlService
    {
        /// <summary>
        /// Builds a ZATCA Phase-2 UBL XML placeholder.  The XML is not signed in this version; it is
        /// structured so that the electronic signing step can be plugged in later.
        /// </summary>
        XDocument BuildInvoiceXml(InvoiceModel invoice, CompanyModel? company);
        string SaveInvoiceXml(InvoiceModel invoice, CompanyModel? company, string outputFolder);
    }

    public class XmlService : IXmlService
    {
        public XDocument BuildInvoiceXml(InvoiceModel invoice, CompanyModel? company)
        {
            var ns = XNamespace.Get("urn:oasis:names:specification:ubl:schema:xsd:Invoice-2");
            var cac = XNamespace.Get("urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
            var cbc = XNamespace.Get("urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");

            var root = new XElement(ns + "Invoice",
                new XAttribute(XNamespace.Xmlns + "cac", cac.NamespaceName),
                new XAttribute(XNamespace.Xmlns + "cbc", cbc.NamespaceName),
                new XElement(cbc + "ID", invoice.InvoiceNumber),
                new XElement(cbc + "UUID", invoice.UUID),
                new XElement(cbc + "IssueDate", invoice.InvoiceDate?.ToString("yyyy-MM-dd")),
                new XElement(cbc + "IssueTime", invoice.InvoiceDate?.ToString("HH:mm:ss")),
                new XElement(cbc + "InvoiceTypeCode", "388"),
                new XElement(cbc + "DocumentCurrencyCode", invoice.CurrencyCode),
                new XElement(cbc + "TaxCurrencyCode", invoice.CurrencyCode),
                new XElement(cac + "AccountingSupplierParty",
                    new XElement(cac + "Party",
                        new XElement(cac + "PartyLegalEntity",
                            new XElement(cbc + "RegistrationName", invoice.CompanyName ?? company?.Title)
                        ),
                        new XElement(cac + "PartyTaxScheme",
                            new XElement(cbc + "CompanyID", invoice.CompanyVATNumber ?? company?.VATNumber),
                            new XElement(cac + "TaxScheme", new XElement(cbc + "ID", "VAT"))
                        )
                    )
                ),
                new XElement(cac + "AccountingCustomerParty",
                    new XElement(cac + "Party",
                        new XElement(cac + "PartyLegalEntity",
                            new XElement(cbc + "RegistrationName", invoice.CustomerName)
                        ),
                        new XElement(cac + "PartyTaxScheme",
                            new XElement(cbc + "CompanyID", invoice.CustomerVATNumber),
                            new XElement(cac + "TaxScheme", new XElement(cbc + "ID", "VAT"))
                        )
                    )
                ),
                new XElement(cac + "LegalMonetaryTotal",
                    new XElement(cbc + "LineExtensionAmount", invoice.Subtotal?.ToString("0.00")),
                    new XElement(cbc + "TaxExclusiveAmount", (invoice.Subtotal - invoice.DiscountAmount)?.ToString("0.00")),
                    new XElement(cbc + "TaxInclusiveAmount", invoice.GrandTotal?.ToString("0.00")),
                    new XElement(cbc + "PayableAmount", invoice.GrandTotal?.ToString("0.00"))
                )
            );

            foreach (var line in invoice.Products?.Where(p => p.IsActive) ?? new List<InvoiceProductModel>())
            {
                var lineXml = new XElement(cac + "InvoiceLine",
                    new XElement(cbc + "ID", line.SortOrder),
                    new XElement(cbc + "InvoicedQuantity", new XAttribute("unitCode", line.Unit ?? "EA"), line.Quantity?.ToString("0.00")),
                    new XElement(cbc + "LineExtensionAmount", (line.TaxableAmount)?.ToString("0.00")),
                    new XElement(cac + "TaxTotal",
                        new XElement(cbc + "TaxAmount", line.VATAmount?.ToString("0.00")),
                        new XElement(cac + "TaxSubtotal",
                            new XElement(cbc + "TaxableAmount", line.TaxableAmount?.ToString("0.00")),
                            new XElement(cbc + "TaxAmount", line.VATAmount?.ToString("0.00")),
                            new XElement(cac + "TaxScheme",
                                new XElement(cbc + "ID", "VAT"),
                                new XElement(cbc + "TaxTypeCode", "S")
                            )
                        )
                    ),
                    new XElement(cac + "Item",
                        new XElement(cbc + "Name", line.Description)
                    ),
                    new XElement(cac + "Price",
                        new XElement(cbc + "PriceAmount", line.Price?.ToString("0.00"))
                    )
                );

                root.Add(lineXml);
            }

            return new XDocument(new XDeclaration("1.0", "utf-8", "yes"), root);
        }

        public string SaveInvoiceXml(InvoiceModel invoice, CompanyModel? company, string outputFolder)
        {
            if (!Directory.Exists(outputFolder))
                Directory.CreateDirectory(outputFolder);

            var fileName = $"invoice-{invoice.InvoiceNumber}.xml"
                .Replace("/", "-")
                .Replace("\\", "-");

            var path = Path.Combine(outputFolder, fileName);
            var doc = BuildInvoiceXml(invoice, company);
            doc.Save(path, SaveOptions.None);
            return path;
        }
    }
}
