using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace WinFIM.Core.Services
{
    public class HashingService : IHashingService
    {
        public async Task<string> CalculateSha256Async(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return string.Empty;

            try
            {
                using var sha256 = SHA256.Create();
                using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 4096, true);
                var hashBytes = await sha256.ComputeHashAsync(stream);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
            catch (Exception)
            {
                // In case file is locked or cannot be read, we return an empty hash or a specific error string.
                // For MVP, we'll return empty string to indicate hash could not be calculated.
                return string.Empty;
            }
        }
    }
}
