using LLC_MOD_Toolbox.Models;

namespace LLC_MOD_Toolbox.Services.Gacha
{
    public interface IGachaService
    {
        bool IsInitialized { get; }
        bool InitializationFailed { get; }
        int RollCount { get; }
        Task InitializeAsync();
        GachaRollResult Roll();
    }
}
