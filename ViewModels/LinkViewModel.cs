using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LLC_MOD_Toolbox.Helpers;

namespace LLC_MOD_Toolbox.ViewModels;

public partial class LinkViewModel() : ObservableObject
{
    [ObservableProperty]
    private Dictionary<string, string> links =
        new(
            [
                new("都市零协会文档站", "https://www.zeroasso.top"),
                new("都市零协会 BiliBili", "https://space.bilibili.com/1247764479"),
                new("LLC on GitHub", "https://github.com/LocalizeLimbusCompany"),
                new("爱发电主页", "https://afdian.com/a/Limbus_zero"),
                new("翻译项目译者", "https://paratranz.cn/projects/6860/leaderboard"),
                new("翻译平台Paratranz", "https://paratranz.cn"),
                new("零协会周边微店(柔造)", "https://weidian.com/?userid=1655827241"),
                new("边狱公司灰机Wiki", "https://limbuscompany.huijiwiki.com"),
                new("免费开服平台-简幻欢", "https://simpfun.cn"),
            ]
        );

    [RelayCommand]
    private static void OpenLink(string link)
    {
        UrlHelper.LaunchUrl(link);
    }
}
