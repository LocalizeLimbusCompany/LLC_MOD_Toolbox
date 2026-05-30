using LLC_MOD_Toolbox.Models;
using LLC_MOD_Toolbox.Services.Gacha;
using LLC_MOD_Toolbox.Services.Skin;
using LLC_MOD_Toolbox.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace LLC_MOD_Toolbox
{
    public partial class MainWindow : Window
    {
        public MainWindowViewModel ViewModel { get; }
        private readonly ISkinService _skinService;
        private DispatcherTimer? _gachaTimer;
        private int _gachaRevealIndex;
        private bool _gachaRevealing;
        private GachaRollResult? _pendingGachaResult;
        private bool _hitMillionthEgg;
        private bool _millionthEggBroken;

        public MainWindow(MainWindowViewModel viewModel, ISkinService skinService)
        {
            ViewModel = viewModel;
            _skinService = skinService;
            InitializeComponent();
            DataContext = ViewModel;

            ViewModel.CloseRequested += () => Application.Current.Shutdown();
            ViewModel.MinimizeRequested += () => WindowState = WindowState.Minimized;
            ViewModel.ApplySkinRequested += () => _skinService.ApplySkinToWindow(this);
            ViewModel.FontPreviewRequested += ApplyFontPreview;
            ViewModel.GachaRollExecuted += HandleGachaRollResult;
            ViewModel.EasterEggImageRequested += LoadEasterEggImage;
            ViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainWindowViewModel.ProgressPercentage))
                UpdateProgressClip(ViewModel.ProgressPercentage);
        }

        private void UpdateProgressClip(float value)
        {
            value = (float)Math.Round(value, 1);
            var rectGeometry = new RectangleGeometry
            {
                Rect = new Rect(0, 0, 6.24 * value, 50)
            };
            Dispatcher.BeginInvoke(() => AutoInstallPage.ProgressClip = rectGeometry);
        }

        private async void LoadEasterEggImage(string url)
        {
            try
            {
                using var client = new HttpClient();
                var bytes = await client.GetByteArrayAsync(url);
                using var stream = new MemoryStream(bytes);
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
                bitmap.Freeze();
                Dispatcher.BeginInvoke(() => EEPage.PageImageSource = bitmap);
            }
            catch (Exception ex)
            {
                Log.logger.Error("更改彩蛋图片失败：" + ex.Message);
            }
        }

        private async void WindowLoaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.InitializeAsync();
        }

        private void WindowDragMove(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Cursor = Cursors.ScrollAll;
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Cursor = Cursors.Arrow;
        }

        private void ApplyFontPreview(string fontPath, double fontSize)
        {
            var fontUri = new System.Uri(fontPath);
            string familyName = new Services.Font.FontService().GetFontFamilyName(fontPath);
            var customFont = new FontFamily(fontUri.AbsoluteUri + "#" + familyName);
            Dispatcher.BeginInvoke(() =>
            {
                Resources["GlobalPreviewFont"] = customFont;
                Resources["GlobalPreviewFontSize"] = fontSize;
                Resources["GlobalPreviewSmallFontSize"] = fontSize / 16 * 12;
            });
        }

        private void HandleGachaRollResult(GachaRollResult result)
        {
            if (_gachaRevealing) return;
            _gachaRevealing = true;
            _pendingGachaResult = result;

            GachaPage.CollapseAllResults();
            GachaPage.SetButtonHitTestVisible(false);

            var random = new Random();
            bool vergilShown = false;
            for (int i = 0; i < 10 && i < result.Personals.Count; i++)
            {
                var personal = result.Personals[i];
                if (GachaPage.GetResultLabel(i).Content is not TextBlock textBlock) continue;

                if (result.HasVergil && !vergilShown && random.Next(1, 10) == 1)
                {
                    textBlock.Text = "[★★★★★★] 猩红凝视 维吉里乌斯";
                    textBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9B0101"));
                    vergilShown = true;
                    continue;
                }

                (textBlock.Text, string color) = personal.Unique switch
                {
                    1 => ("[★]" + personal.Name, "#B88345"),
                    2 => ("[★★]" + personal.Name, "#CA1400"),
                    _ => ("[★★★]" + personal.Name, "#FCC404"),
                };
                textBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
            }

            _gachaRevealVergilShown = vergilShown;
            _gachaRevealIndex = 0;
            _gachaTimer ??= CreateGachaTimer();
            _gachaTimer.Start();
        }

        private bool _gachaRevealVergilShown;

        private DispatcherTimer CreateGachaTimer()
        {
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(0.05) };
            timer.Tick += GachaTimerTick;
            return timer;
        }

        private void GachaTimerTick(object? sender, EventArgs e)
        {
            if (_gachaRevealIndex < 10)
            {
                GachaPage.GetResultLabel(_gachaRevealIndex).Visibility = Visibility.Visible;
                _gachaRevealIndex++;
                return;
            }

            _gachaTimer?.Stop();
            GachaPage.SetButtonHitTestVisible(true);
            ShowGachaResultDialog(_pendingGachaResult);
            _gachaRevealing = false;
        }

        private void ShowGachaResultDialog(GachaRollResult? result)
        {
            if (result is null) return;
            var random = new Random();

            if (random.Next(1, 100001) == 100000 && !_millionthEggBroken)
            {
                ShowMillionthEgg();
                return;
            }
            if (_gachaRevealVergilShown)
            {
                ViewModel.ShowGachaMessage("当你不见前路，不知应去往何方时……\n向导会为你指引方向。\n但丁。", "？？？");
                return;
            }
            if (random.Next(1, 51) == 50)
            {
                ViewModel.ShowGachaMessage("你知道吗？\n按Enter可以快速抽奖。", "提示");
                return;
            }
            if (ShowRollCountMilestone(result.RollCount))
                return;

            ShowUniqueCountComment(result.UniqueCounts, random);
        }

        private bool ShowRollCountMilestone(int count)
        {
            string? message = count switch
            {
                10 => "你已经抽了100抽了，你上头了？",
                20 => "恭喜你，你已经抽了一个井了！\n珍爱生命，远离抽卡啊亲！",
                40 => "两个井了，你算算已经砸了多少狂气了？",
                60 => "收手吧！你不算砸了多少狂气我算了！\n你已经砸了60x1300=78000狂气了！",
                100 => "我是来恭喜你，你已经扔进去1000抽，简称130000狂气了。\n你花了多少时间到这里？",
                200 => "2000抽。\n有这个毅力你做什么都会成功的。",
                _ => null,
            };
            if (message is null) return false;
            ViewModel.ShowGachaMessage(message, "提示");
            return true;
        }

        private void ShowMillionthEgg()
        {
            if (!_hitMillionthEgg)
            {
                ViewModel.ShowGachaMessage("Bro。\n这是一个十万分之一概率的弹窗。\n你怎么会触发它呢，它几乎是一个不可能事件——\n算了，无所谓了。\n总之你现在可以向朋友炫耀一下了，我猜。\n比如说你中了一个零协会的彩蛋？\n当然我不会给你什么的，例如狂气又或者合成玉之类。\n不过我倒是可以告诉你一个秘密：", "什么？？？");
                ViewModel.ShowGachaMessage("凯尔希很可爱。", "嗯。");
                ViewModel.ShowGachaMessage("不满意？\n那随便你，总之彩蛋我写完了。", "你应该不会再触发一次这个文本，对吧？");
                _hitMillionthEgg = true;
                return;
            }
            ViewModel.ShowGachaMessage("你真再触发了一次啊？", "哇靠");
            ViewModel.ShowGachaMessage("额，我可以说我没什么能告诉你了吗。\n放心，合成玉或者源石什么的我还是不会给的。", "什么？？？");
            ViewModel.ShowGachaMessage("这样吧，你先下载明日方舟，然后前往中坚卡池抽取凯尔希，后面忘了", "嗯，即使这样我也什么都不会说了");
            ViewModel.ShowGachaMessage("然后你可以不用抽了，没文本了。", "真的没有了！！！");
            ViewModel.ShowGachaMessage("当然你仍然可能会觉得：\n工具箱一定在骗我，一定还有彩蛋！\n如果你真的这么想然后抽1000抽没有结果，然后往网上传图，我会不留余力嘲笑你的。\n就这样。", "真的没彩蛋了，实在不行你双击一下小黑小白？");
            _millionthEggBroken = true;
        }

        private void ShowUniqueCountComment(int[] counts, Random random)
        {
            int choice = random.Next(1, 4);
            string message;
            if (counts[0] == 9 && counts[1] == 1)
                message = choice switch { 1 => "恭喜九白一红~！", 2 => "正常发挥正常发挥~", _ => "还好没拿真狂气抽吧！" };
            else if (counts[0] == 8 && counts[1] == 2)
                message = choice switch { 1 => "至少比九白一红好一点，不是么？", 2 => "你要不先去洗洗手？", _ => "真是可惜，看来这次运气没有站在你这边.jpg" };
            else if (counts[0] == 7 && counts[1] == 3)
                message = choice switch { 1 => "三个二星！这是多少碎片来着？", 2 => "工具箱的概率可是十分严谨的！\n所以肯定不是工具箱的问题！", _ => "要是抽不中就算了吧，散伙散伙！" };
            else if (counts[2] == 1)
                message = choice switch { 1 => "金色传说！虽然说就一个。", 2 => "恭喜恭喜~不知道抽了多少次了？", _ => "ALL IN！" };
            else if (counts[2] == 2)
                message = choice switch { 1 => "双黄蛋？希望你瓦夜的时候也能这样。", 2 => "100碎片而已，我一点都不羡慕！", _ => "恭喜恭喜~" };
            else if (counts[2] == 3)
                message = choice switch { 1 => "真的假的三黄。。？", 2 => "你平时运气也这么好？！", _ => "爽了，再来再来！" };
            else if (counts[2] >= 4)
                message = choice switch { 1 => "不可能……不可能啊？！", 2 => "欧吃矛！", _ => "再抽池子就要空了！" };
            else
                message = choice switch { 1 => "怎么样？再来一次么？", 2 => "冷知识：概率真的完全真实。", _ => "你平时抽卡也这个结果吗？" };

            ViewModel.ShowGachaMessage(message, "提示");
        }

        private void WhiteBlackClickDouble(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount < 2) return;
            if (!ViewModel.IsEasterEggUnlocked)
            {
                Log.logger.Info("不要点了>_<");
                ViewModel.IsEasterEggUnlocked = true;
                e.Handled = true;
            }
        }
    }
}
