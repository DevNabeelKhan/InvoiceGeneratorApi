using System.Security.Cryptography;
using System.Text;

namespace DataAccessLayer.Shared
{
    // Simple AES-256-CBC helper used to encrypt/decrypt the configuration TableName
    // that travels between the Angular client and this API. Key/IV must stay in
    // sync with the EncryptionService on the Angular side (src/services/encryption.service.ts).
    public static class EncryptionHelper
    {
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("InvoiceGenerator@2025-AesKey!!!!"); // 32 bytes -> AES-256
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("InvoiceGenIV2025"); // 16 bytes

        public static string Encrypt(string? plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return string.Empty;
            using var aes = Aes.Create();
            aes.Key = Key;
            aes.IV = IV;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            using var encryptor = aes.CreateEncryptor();
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
            return Convert.ToBase64String(cipherBytes);
        }

        public static string? Decrypt(string? cipherText)
        {
            if (string.IsNullOrEmpty(cipherText)) return null;
            try
            {
                using var aes = Aes.Create();
                aes.Key = Key;
                aes.IV = IV;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                using var decryptor = aes.CreateDecryptor();
                var cipherBytes = Convert.FromBase64String(cipherText);
                var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                return Encoding.UTF8.GetString(plainBytes);
            }
            catch
            {
                return null;
            }
        }
    }
}
