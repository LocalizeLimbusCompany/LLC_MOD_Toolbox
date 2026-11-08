using LLC_MOD_Toolbox.Models.Configuration;

namespace LLC_MOD_Toolbox.Services.Configuration;

public interface IConfigurationService
{
    AppSettings Settings { get; }
    void Save();
}
