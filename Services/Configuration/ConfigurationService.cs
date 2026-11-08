using System.IO;
using LLC_MOD_Toolbox.Models.Configuration;
using Newtonsoft.Json;

namespace LLC_MOD_Toolbox.Services.Configuration;

/// <summary>Owns the configuration file. CLR property names are mapped to the existing JSON schema.</summary>
public sealed class ConfigurationService : IConfigurationService
{
    private static readonly JsonSerializerSettings SerializerSettings = new()
    {
        MissingMemberHandling = MissingMemberHandling.Ignore,
        NullValueHandling = NullValueHandling.Ignore
    };

    private readonly object _saveLock = new();
    private readonly string _filePath;

    public AppSettings Settings { get; }

    public ConfigurationService(string filePath)
    {
        _filePath = Path.GetFullPath(filePath);
        Settings = Load();
        if (!File.Exists(_filePath))
            Save();
    }

    private AppSettings Load()
    {
        try
        {
            return JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText(_filePath), SerializerSettings)
                ?? new AppSettings();
        }
        catch (FileNotFoundException)
        {
            return new AppSettings();
        }
        catch (DirectoryNotFoundException)
        {
            return new AppSettings();
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"JSON解析错误: {ex.Message}", ex);
        }
    }

    public void Save()
    {
        lock (_saveLock)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
            string temporaryPath = _filePath + "." + Guid.NewGuid().ToString("N") + ".tmp";
            try
            {
                File.WriteAllText(temporaryPath, JsonConvert.SerializeObject(Settings, Formatting.Indented));
                File.Move(temporaryPath, _filePath, overwrite: true);
            }
            finally
            {
                if (File.Exists(temporaryPath))
                    File.Delete(temporaryPath);
            }
        }
    }
}
