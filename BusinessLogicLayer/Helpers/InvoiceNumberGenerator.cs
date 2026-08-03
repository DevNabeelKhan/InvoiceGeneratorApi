using DataAccessLayer.Interface;

namespace BusinessLogicLayer.Helpers
{
    public interface IInvoiceNumberGenerator
    {
        Task<string> GenerateAsync(int? year = null, string? prefix = "INV-");
    }

    public class InvoiceNumberGenerator : IInvoiceNumberGenerator
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public InvoiceNumberGenerator(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public async Task<string> GenerateAsync(int? year = null, string? prefix = "INV-")
        {
            var result = await _invoiceRepository.GetNextInvoiceNumber(year, prefix);
            return result?.InvoiceNumber ?? $"{prefix}0000/{year ?? DateTime.UtcNow.Year}";
        }
    }
}
