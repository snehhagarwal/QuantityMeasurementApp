using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using QuantityMeasurementBusinessLayer.Services.Interface;

namespace QuantityMeasurementBusinessLayer.Services.Implementation
{
    /// <summary>AES-256-CBC. Stored format: Base64( IV[16] + CipherText ). Key from configuration (SHA256-derived).</summary>
    public class AesEncryptionService : IAesEncryptionService
    {
        private readonly byte[] _key;

        public AesEncryptionService(IConfiguration config)
        {
            var rawKey = config["Encryption:Key"]
                ?? throw new InvalidOperationException("Encryption:Key is missing from appsettings.json.");
            _key = SHA256.HashData(Encoding.UTF8.GetBytes(rawKey));
        }

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentException("Nothing to encrypt.", nameof(plainText));

            using var aes = Aes.Create();
            aes.Key = _key;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor();
            byte[] cipherBytes = encryptor.TransformFinalBlock(
                Encoding.UTF8.GetBytes(plainText), 0, plainText.Length);

            byte[] result = new byte[aes.IV.Length + cipherBytes.Length];
            Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
            Buffer.BlockCopy(cipherBytes, 0, result, aes.IV.Length, cipherBytes.Length);
            return Convert.ToBase64String(result);
        }

        public string Decrypt(string encryptedBase64)
        {
            if (string.IsNullOrEmpty(encryptedBase64))
                throw new ArgumentException("Nothing to decrypt.", nameof(encryptedBase64));

            byte[] combined = Convert.FromBase64String(encryptedBase64);
            const int ivLength = 16;
            if (combined.Length <= ivLength) throw new ArgumentException("Data is too short.");

            byte[] iv = new byte[ivLength];
            byte[] cipherBytes = new byte[combined.Length - ivLength];
            Buffer.BlockCopy(combined, 0, iv, 0, ivLength);
            Buffer.BlockCopy(combined, ivLength, cipherBytes, 0, cipherBytes.Length);

            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var decryptor = aes.CreateDecryptor();
            byte[] plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
            return Encoding.UTF8.GetString(plainBytes);
        }
    }
}
