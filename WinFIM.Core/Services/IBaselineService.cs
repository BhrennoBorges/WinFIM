using System;
using System.Threading.Tasks;

namespace WinFIM.Core.Services
{
    public interface IBaselineService
    {
        Task CreateBaselineAsync(Action<string> progressCallback = null!);
    }
}
