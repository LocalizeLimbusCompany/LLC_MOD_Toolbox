using LLC_MOD_Toolbox.Models;
using LLC_MOD_Toolbox.Services.Network;
using Newtonsoft.Json.Linq;

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

    public record GachaRollResult(List<PersonalInfo> Personals, int[] UniqueCounts, bool HasVergil, int RollCount);

    public sealed class GachaService : IGachaService
    {
        private const string PersonalityDataUrl = "https://download.zeroasso.top/wiki/personality.json";

        private readonly IHttpService _httpService;
        private List<PersonalInfo> _personalInfos1star = [];
        private List<PersonalInfo> _personalInfos2star = [];
        private List<PersonalInfo> _personalInfos3star = [];
        private bool _hasVergil;
        private readonly Random _random = new();

        public bool IsInitialized { get; private set; }
        public bool InitializationFailed { get; private set; }
        public int RollCount { get; private set; }

        public GachaService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task InitializeAsync()
        {
            InitializationFailed = false;

            string gachaText = await _httpService.GetTextAsync(PersonalityDataUrl);
            if (string.IsNullOrEmpty(gachaText))
            {
                Log.logger.Error("初始化失败。");
                InitializationFailed = true;
                return;
            }

            List<PersonalInfo> personalInfos = TransformTextToList(gachaText);
            if (personalInfos.Count == 0)
            {
                Log.logger.Error("初始化失败，未解析到人格数据。");
                InitializationFailed = true;
                return;
            }

            Log.logger.Info("人格数量：" + personalInfos.Count);
            _personalInfos1star = personalInfos.Where(p => p.Unique == 1).ToList();
            _personalInfos2star = personalInfos.Where(p => p.Unique == 2).ToList();
            _personalInfos3star = personalInfos.Where(p => p.Unique == 3).ToList();
            IsInitialized = true;
        }

        public GachaRollResult Roll()
        {
            if (_random.Next(1, 101) == 100)
                _hasVergil = true;

            List<PersonalInfo> personals = GeneratePersonalList();
            if (personals.Count < 10)
                personals = GeneratePersonalList();

            int[] uniqueCount = GetPersonalUniqueCount(personals);
            RollCount++;

            bool showVergil = false;
            if (_hasVergil && _random.Next(1, 10) == 1)
            {
                showVergil = true;
                _hasVergil = false;
            }

            return new GachaRollResult(personals, uniqueCount, showVergil, RollCount);
        }

        private List<PersonalInfo> GeneratePersonalList()
        {
            List<PersonalInfo> result = [];
            for (int i = 0; i < 10; i++)
            {
                int chance = _random.Next(1, 101);
                if (i != 9)
                {
                    if (chance <= 84)
                        result.Add(Pick1Star());
                    else if (chance <= 97)
                        result.Add(Pick2Star());
                    else
                        result.Add(_personalInfos3star[_random.Next(_personalInfos3star.Count)]);
                }
                else
                {
                    if (chance <= 84)
                        result.Add(Pick2Star());
                    else
                        result.Add(_personalInfos3star[_random.Next(_personalInfos3star.Count)]);
                }
            }
            return result;
        }

        private PersonalInfo Pick1Star()
        {
            if (_personalInfos1star.Count > 0)
                return _personalInfos1star[_random.Next(_personalInfos1star.Count)];
            if (_personalInfos2star.Count > 0)
                return _personalInfos2star[_random.Next(_personalInfos2star.Count)];
            return _personalInfos3star[_random.Next(_personalInfos3star.Count)];
        }

        private PersonalInfo Pick2Star()
        {
            if (_personalInfos2star.Count > 0)
                return _personalInfos2star[_random.Next(_personalInfos2star.Count)];
            return _personalInfos3star[_random.Next(_personalInfos3star.Count)];
        }

        private static int[] GetPersonalUniqueCount(List<PersonalInfo> personals)
        {
            int[] counts = [0, 0, 0];
            foreach (var p in personals)
                counts[p.Unique - 1]++;
            return counts;
        }

        private static List<PersonalInfo> TransformTextToList(string gachaText)
        {
            Log.logger.Info("开始转换文本。");
            var gachaObject = JObject.Parse(gachaText.TrimStart('\uFEFF'));
            List<PersonalInfo> list = [];

            JToken? personalityData = gachaObject["personalityData"];
            if (personalityData is not JArray personalityArray)
            {
                Log.logger.Error("人格数据格式错误，未找到 personalityData 数组。");
                return list;
            }

            foreach (JToken item in personalityArray)
            {
                string? name = item["name"]?.Value<string>();
                string? starsText = item["stars"]?.Value<string>();
                if (string.IsNullOrWhiteSpace(name) ||
                    !int.TryParse(starsText, out int stars) ||
                    stars is < 1 or > 3)
                {
                    continue;
                }

                list.Add(new PersonalInfo
                {
                    Name = name.Trim(),
                    Unique = stars,
                });
            }

            return list;
        }
    }
}
