namespace QuantityMeasurementBusinessLayer.Services.Interface
{
    /// <summary>AES symmetric encryption for secrets at rest (e.g. password hash envelope).</summary>
    public interface IAesEncryptionService
    {
        string Encrypt(string plainText);
        string Decrypt(string encryptedBase64);
    }
}
