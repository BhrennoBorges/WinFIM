using System.Threading.Tasks;

namespace WinFIM.Core.Services
{
    public interface IHashingService
    {
        Task<string> CalculateSha256Async(string filePath);
    }
}
