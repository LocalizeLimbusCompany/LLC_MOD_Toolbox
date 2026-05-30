using LLC_MOD_Toolbox.Services.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace LLC_MOD_Toolbox.Services.Installation
{
    public sealed class UninstallService : IUninstallService
    {
        private readonly AppState _appState;
        private readonly IFileService _fileService;

        public UninstallService(AppState appState, IFileService fileService)
        {
            _appState = appState;
            _fileService = fileService;
        }

        public void DeleteLanguagePack()
        {
            _fileService.DeleteDirectory(Path.Combine(_appState.LimbusCompanyDir, "LimbusCompany_Data", "Lang", "LLC_zh-CN"));
            WriteLCBLangConfig("");
        }

        public void DeleteBepInEx()
        {
            string dir = _appState.LimbusCompanyDir;
            _fileService.DeleteDirectory(Path.Combine(dir, "BepInEx"));
            _fileService.DeleteDirectory(Path.Combine(dir, "dotnet"));
            _fileService.DeleteFile(Path.Combine(dir, "doorstop_config.ini"));
            _fileService.DeleteFile(Path.Combine(dir, "Latest(框架日志).log"));
            _fileService.DeleteFile(Path.Combine(dir, "Player(游戏日志).log"));
            _fileService.DeleteFile(Path.Combine(dir, "winhttp.dll"));
            _fileService.DeleteFile(Path.Combine(dir, "winhttp.dll.disabled"));
            _fileService.DeleteFile(Path.Combine(dir, "changelog.txt"));
            _fileService.DeleteFile(Path.Combine(dir, "BepInEx-IL2CPP-x64.7z"));
            _fileService.DeleteFile(Path.Combine(dir, "LimbusLocalize_BIE.7z"));
            _fileService.DeleteFile(Path.Combine(dir, "tmpchinese_BIE.7z"));
        }

        public void DeleteMelonLoader()
        {
            string dir = _appState.LimbusCompanyDir;
            _fileService.DeleteDirectory(Path.Combine(dir, "MelonLoader"));
            _fileService.DeleteDirectory(Path.Combine(dir, "Mods"));
            _fileService.DeleteDirectory(Path.Combine(dir, "Plugins"));
            _fileService.DeleteDirectory(Path.Combine(dir, "UserData"));
            _fileService.DeleteDirectory(Path.Combine(dir, "UserLibs"));
            _fileService.DeleteFile(Path.Combine(dir, "dobby.dll"));
            _fileService.DeleteFile(Path.Combine(dir, "version.dll"));
        }

        public void UninstallAll()
        {
            DeleteLanguagePack();
            DeleteBepInEx();
            DeleteMelonLoader();
        }

        private void WriteLCBLangConfig(string value)
        {
            try
            {
                string configPath = Path.Combine(_appState.LimbusCompanyDir, "LimbusCompany_Data", "Lang", "config.json");
                if (File.Exists(configPath))
                {
                    string configJson = File.ReadAllText(configPath);
                    var configObject = JObject.Parse(configJson);
                    configObject["lang"] = value;
                    File.WriteAllText(configPath, configObject.ToString());
                }
            }
            catch (JsonReaderException ex)
            {
                Log.logger.Warn("修改LCB lang config失败: " + ex.Message);
                File.WriteAllText(
                    Path.Combine(_appState.LimbusCompanyDir, "LimbusCompany_Data", "Lang", "config.json"),
                    "{\"lang\": \"\",\"titleFont\": \"\",\"contextFont\": \"\",\"samplingPointSize\": 78,\"padding\": 5}");
                WriteLCBLangConfig(value);
            }
        }
    }
}
