using QRCoder;
using System;
using System.IO;
using System.Text;

namespace BusinessLogicLayer.Helpers
{
    /// <summary>
    /// Generates a ZATCA Phase-2 compliant QR code (Simplified Tax Invoice).
    /// TLV format: tag (1 byte) + length (1 byte) + value (UTF-8 bytes).
    /// The resulting bytes are encoded to Base64 and rendered as a PNG.
    /// </summary>
    public static class ZatcaQrHelper
    {
        public const int TagSellerName = 1;
        public const int TagVatNumber = 2;
        public const int TagTimestamp = 3;
        public const int TagInvoiceTotal = 4;
        public const int TagVatTotal = 5;

        public static string GenerateBase64(string sellerName, string vatNumber, DateTime invoiceTimestamp,
                                            decimal invoiceTotal, decimal vatTotal)
        {
            var bytes = new StringBuilder();
            bytes.Append(EncodeTlv(TagSellerName, sellerName));
            bytes.Append(EncodeTlv(TagVatNumber, vatNumber));
            bytes.Append(EncodeTlv(TagTimestamp, invoiceTimestamp.ToString("yyyy-MM-ddTHH:mm:ssZ")));
            bytes.Append(EncodeTlv(TagInvoiceTotal, invoiceTotal.ToString("0.00")));
            bytes.Append(EncodeTlv(TagVatTotal, vatTotal.ToString("0.00")));

            var data = Encoding.UTF8.GetBytes(bytes.ToString());
            return Convert.ToBase64String(data);
        }

        public static byte[] GenerateQrImage(string base64Tlv, int pixelsPerModule = 5)
        {
            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(base64Tlv, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(qrCodeData);
            return qrCode.GetGraphic(pixelsPerModule);
        }

        public static string GenerateQrImageBase64(string base64Tlv, int pixelsPerModule = 5)
        {
            var image = GenerateQrImage(base64Tlv, pixelsPerModule);
            return Convert.ToBase64String(image);
        }

        private static string EncodeTlv(int tag, string value)
        {
            var valueBytes = Encoding.UTF8.GetBytes(value ?? string.Empty);
            var tagByte = (byte)tag;
            var lengthByte = (byte)valueBytes.Length;

            using var ms = new MemoryStream();
            ms.WriteByte(tagByte);
            ms.WriteByte(lengthByte);
            ms.Write(valueBytes, 0, valueBytes.Length);

            return Encoding.UTF8.GetString(ms.ToArray());
        }
    }
}
