using System;
using System.Threading.Tasks;

namespace WinFIM.Core.Services
{
    public interface IRevalidationService
    {
        Task RunRevalidationAsync(Action<string> progressCallback = null!);
    }
}
