using LLC_MOD_Toolbox.Services.Configuration;
using LLC_MOD_Toolbox.Services.Network;
using Newtonsoft.Json.Linq;
using System.IO;

namespace LLC_MOD_Toolbox.Services.Content
{
    public interface IAnnouncementService
    {
        Task<AnnouncementResult> CheckForAnnouncementAsync();
        void MarkAsRead(int version);
    }

    public record AnnouncementResult(bool HasNew, string Content, string Level, int Version, Exception? Error = null);

    public sealed class AnnouncementService : IAnnouncementService
    {
        private readonly IHttpService _httpService;
        private readonly INodeService _nodeService;
        private readonly ConfigurationManager _config;

        public AnnouncementService(IHttpService httpService, INodeService nodeService, ConfigurationManager config)
        {
            _httpService = httpService;
            _nodeService = nodeService;
            _config = config;
        }

        public async Task<AnnouncementResult> CheckForAnnouncementAsync()
        {
            if (!_config.Settings.announcement.getAnno)
                return new AnnouncementResult(false, string.Empty, string.Empty, 0);

            try
            {
                string annoText = await _httpService.GetTextAsync(_nodeService.ResolveApiUrl("v2/announcement/get_anno"), reportError: false);
                if (string.IsNullOrEmpty(annoText))
                    return new AnnouncementResult(false, string.Empty, string.Empty, 0);

                var annoObject = JObject.Parse(annoText);
                int version = annoObject["version"]!.Value<int>();
                if (version <= _config.Settings.announcement.annoVersion)
                {
                    Log.logger.Info("无新公告。");
                    return new AnnouncementResult(false, string.Empty, string.Empty, 0);
                }

                Log.logger.Info("有新公告。");
                string content = annoObject["anno"]!.Value<string>()!.Replace("\\n", "\n");
                string level = annoObject["level"]!.Value<string>()!;
                return new AnnouncementResult(true, content, level, version);
            }
            catch (Exception ex)
            {
                Log.logger.Error("检查公告失败。", ex);
                return new AnnouncementResult(false, string.Empty, string.Empty, 0, ex);
            }
        }

        public void MarkAsRead(int version)
        {
            _config.Settings.announcement.annoVersion = version;
            _config.SaveConfig();
        }
    }
}
