using LLC_MOD_Toolbox.Models;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace LLC_MOD_Toolbox
{
    public partial class MainWindow : Window
    {
        #region 抽卡模拟器
        private static bool isInitGacha = false;
        private static bool isInitGachaFailed = false;
        private static int gachaCount = 0;
        private static List<PersonalInfo> personalInfos1star = [];
        private static List<PersonalInfo> personalInfos2star = [];
        private static List<PersonalInfo> personalInfos3star = [];
        private DispatcherTimer? gachaTimer;
        private int _currentIndex = 0;
        private int[]? uniqueCount;
        private bool hasVergil = false;
        private bool alreadyHasVergil = false;
        private bool gachaing = false;
        private bool _isTicking = false;
        bool someEggIGuess = false;
        bool someEggBroken = false;

        private async Task InitGacha()
        {
            await DisableGlobalOperations();
            string gachaText = await GetURLText("https://download.zeroasso.top/wiki/wiki_personal.json");
            gachaing = false;
            if (string.IsNullOrEmpty(gachaText))
            {
                Log.logger.Error("初始化失败。");
                UniversalDialog.ShowMessage("初始化失败。请检查网络情况。", "", null, this);
                isInitGachaFailed = true;
                hasVergil = false;
                await EnableGlobalOperations();
                return;
            }

            gachaTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(0.05)
            };
            gachaTimer.Tick += GachaTimerTick;
            List<PersonalInfo> personalInfos = TranformTextToList(gachaText);
            Log.logger.Info("人格数量：" + personalInfos.Count);
            personalInfos1star = personalInfos.Where(p => p.Unique == 1).ToList();
            personalInfos2star = personalInfos.Where(p => p.Unique == 2).ToList();
            personalInfos3star = personalInfos.Where(p => p.Unique == 3).ToList();
            UniversalDialog.ShowMessage("初始化完成。", "提示", null, this);
            isInitGacha = true;
            await EnableGlobalOperations();
        }

        private async void InGachaButtonClick()
        {
            if (gachaing) return;
            Log.logger.Info("点击抽卡。");
            gachaing = true;
            await CollapsedAllGacha();
            if (isInitGachaFailed)
            {
                Log.logger.Info("初始化失败。");
                UniversalDialog.ShowMessage("初始化失败，无法进行抽卡操作。", "提示", null, this);
                gachaing = false;
                return;
            }
            Random random = new();
            if (random.Next(1, 101) == 100)
            {
                hasVergil = true;
            }
            try
            {
                List<PersonalInfo> personals = GenPersonalList();
                if (personals.Count < 10)
                {
                    Log.logger.Info("人格数量不足。\n尝试重新生成。");
                    personals = GenPersonalList();
                }
                await StartChangeLabel(personals);
            }
            catch (Exception ex)
            {
                Log.logger.Info("出现了问题。", ex);
                UniversalDialog.ShowMessage("出了点小问题！\n要不再试一次？\n————————\n" + ex.ToString(), "啊呀，出了点小问题", null, this);
                gachaTimer?.Stop();
                _currentIndex = 0;
                await this.Dispatcher.BeginInvoke(() =>
                {
                    GachaPage.SetButtonHitTestVisible(true);
                });
                gachaing = false;
                return;
            }
            if (gachaTimer != null)
            {
                _currentIndex = 0;
                gachaTimer.Start();
            }
        }

        private static int[] GetPersonalUniqueCount(List<PersonalInfo> personals)
        {
            int[] uniqueCount = [0, 0, 0];
            foreach (PersonalInfo personal in personals)
            {
                uniqueCount[personal.Unique - 1] += 1;
            }
            return uniqueCount;
        }

        private async Task StartChangeLabel(List<PersonalInfo> personals)
        {
            for (int i = 0; i < 10; i++)
            {
                await ChangeLabelColorAndPersonal(personals[i], GachaPage.GetResultLabel(i));
            }
        }

        private async Task ChangeLabelColorAndPersonal(PersonalInfo personal, Label label)
        {
            await this.Dispatcher.BeginInvoke(() =>
            {
                if (label.Content is TextBlock textBlock)
                {
                    if (personal.Unique == 1)
                    {
                        textBlock.Text = "[★]" + personal.Name;
                        textBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B88345"));
                    }
                    if (personal.Unique == 2)
                    {
                        textBlock.Text = "[★★]" + personal.Name;
                        textBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CA1400"));
                    }
                    if (personal.Unique == 3)
                    {
                        textBlock.Text = "[★★★]" + personal.Name;
                        textBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FCC404"));
                    }
                    Random random = new();
                    if (hasVergil && random.Next(1, 10) == 1)
                    {
                        textBlock.Text = "[★★★★★★] 猩红凝视 维吉里乌斯";
                        textBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9B0101"));
                        hasVergil = false;
                        alreadyHasVergil = true;
                    }
                }
            });
        }

        private List<PersonalInfo> GenPersonalList()
        {
            Random random = new();
            List<PersonalInfo> genPersonalInfos = [];
            for (int i = 0; i < 10; i++)
            {
                int chance = random.Next(1, 101);
                if (i != 9)
                {
                    if (chance <= 84)
                    {
                        if (personalInfos1star.Count > 0)
                        {
                            genPersonalInfos.Add(personalInfos1star[random.Next(personalInfos1star.Count)]);
                        }
                        else if (personalInfos2star.Count > 0)
                        {
                            genPersonalInfos.Add(personalInfos2star[random.Next(personalInfos2star.Count)]);
                        }
                        else
                        {
                            genPersonalInfos.Add(personalInfos3star[random.Next(personalInfos3star.Count)]);
                        }
                    }
                    else if (chance <= 97)
                    {
                        if (personalInfos2star.Count > 0)
                        {
                            genPersonalInfos.Add(personalInfos2star[random.Next(personalInfos2star.Count)]);
                        }
                        else
                        {
                            genPersonalInfos.Add(personalInfos3star[random.Next(personalInfos3star.Count)]);
                        }
                    }
                    else
                    {
                        genPersonalInfos.Add(personalInfos3star[random.Next(personalInfos3star.Count)]);
                    }
                }
                else
                {
                    if (chance <= 84)
                    {
                        if (personalInfos2star.Count > 0)
                        {
                            genPersonalInfos.Add(personalInfos2star[random.Next(personalInfos2star.Count)]);
                        }
                        else
                        {
                            genPersonalInfos.Add(personalInfos3star[random.Next(personalInfos3star.Count)]);
                        }
                    }
                    else
                    {
                        genPersonalInfos.Add(personalInfos3star[random.Next(personalInfos3star.Count)]);
                    }
                }
            }
            uniqueCount = GetPersonalUniqueCount(genPersonalInfos);
            return genPersonalInfos;
        }

        private static List<PersonalInfo> TranformTextToList(string gachaText)
        {
            Log.logger.Info("开始转换文本。");
            var gachaObject = JObject.Parse(gachaText);
            List<PersonalInfo> personalInfoList = [];
            for (int i = 0; i < gachaObject["data"].Count(); i++)
            {
                string characterName = BeautifyText(gachaObject["data"][i][0].Value<string>(), gachaObject["data"][i][1].Value<string>());
                if (!string.IsNullOrWhiteSpace(characterName))
                {
                    PersonalInfo personalInfo = new()
                    {
                        Name = characterName,
                        Unique = gachaObject["data"][i][7].Value<int>(),
                    };
                    personalInfoList.Add(personalInfo);
                }
                else
                {
                    Log.logger.Warn($"Character at index {i} has an empty name and will be skipped.");
                }
            }
            return personalInfoList;
        }

        private static string BeautifyText(string input, string prefix)
        {
            if (input.StartsWith(prefix))
            {
                string title = input[prefix.Length..];
                return $"{title} {prefix}";
            }
            else
            {
                return input;
            }
        }

        private async void GachaTimerTick(object? sender, EventArgs? e)
        {
            if (_isTicking) return;
            _isTicking = true;

            try
            {
                if (_currentIndex < 10)
                {
                    var label = GachaPage.GetResultLabel(_currentIndex);
                    await this.Dispatcher.BeginInvoke(() =>
                    {
                        label.Visibility = Visibility.Visible;
                    });
                    _currentIndex++;
                }
                else if (gachaTimer != null)
                {
                    gachaTimer.Stop();
                    await this.Dispatcher.BeginInvoke(() =>
                    {
                        GachaPage.SetButtonHitTestVisible(true);
                    });
                    Random random = new();
                    gachaCount += 1;
                    if (random.Next(1, 100001) == 100000 && !someEggBroken)
                    {
                        if (!someEggIGuess)
                        {
                            UniversalDialog.ShowMessage("Bro。\n这是一个十万分之一概率的弹窗。\n你怎么会触发它呢，它几乎是一个不可能事件——\n算了，无所谓了。\n总之你现在可以向朋友炫耀一下了，我猜。\n比如说你中了一个零协会的彩蛋？\n当然我不会给你什么的，例如狂气又或者合成玉之类。\n不过我倒是可以告诉你一个秘密：", "什么？？？", null, this);
                            UniversalDialog.ShowMessage("凯尔希很可爱。", "嗯。", null, this);
                            UniversalDialog.ShowMessage("不满意？\n那随便你，总之彩蛋我写完了。", "你应该不会再触发一次这个文本，对吧？", null, this);
                            someEggIGuess = true;
                            gachaing = false;
                            return;
                        }
                        else
                        {
                            UniversalDialog.ShowMessage("你真再触发了一次啊？", "哇靠", null, this);
                            UniversalDialog.ShowMessage("额，我可以说我没什么能告诉你了吗。\n放心，合成玉或者源石什么的我还是不会给的。", "什么？？？", null, this);
                            UniversalDialog.ShowMessage("这样吧，你先下载明日方舟，然后前往中坚卡池抽取凯尔希，后面忘了", "嗯，即使这样我也什么都不会说了");
                            UniversalDialog.ShowMessage("然后你可以不用抽了，没文本了。", "真的没有了！！！");
                            UniversalDialog.ShowMessage("当然你仍然可能会觉得：\n工具箱一定在骗我，一定还有彩蛋！\n如果你真的这么想然后抽1000抽没有结果，然后往网上传图，我会不留余力嘲笑你的。\n就这样。", "真的没彩蛋了，实在不行你双击一下小黑小白？", null, this);
                            File.WriteAllText(Path.Combine(currentDir, "W0wS0meE99.DOYOUWANTCHECKME"), Convert.ToBase64String(Encoding.UTF8.GetBytes("是吧，我还是骗你了，不过你能发现这个彩蛋也算你厉害了。\n这条信息会被BASE64加密，如果你能成功解密这条消息……\n呃，呃……\n谢谢喜欢？")));
                            someEggBroken = true;
                            gachaing = false;
                            return;
                        }
                    }
                    if (alreadyHasVergil)
                    {
                        UniversalDialog.ShowMessage("当你不见前路，不知应去往何方时……\n向导会为你指引方向。\n但丁。", "？？？", null, this);
                        alreadyHasVergil = false;
                        gachaing = false;
                        return;
                    }
                    if (random.Next(1, 51) == 50)
                    {
                        UniversalDialog.ShowMessage("你知道吗？\n按Enter可以快速抽奖。", "提示", null, this);
                        gachaing = false;
                        return;
                    }
                    switch (gachaCount)
                    {
                        case 10:
                            UniversalDialog.ShowMessage("你已经抽了100抽了，你上头了？", "提示", null, this);
                            gachaing = false;
                            return;
                        case 20:
                            UniversalDialog.ShowMessage("恭喜你，你已经抽了一个井了！\n珍爱生命，远离抽卡啊亲！", "提示", null, this);
                            gachaing = false;
                            return;
                        case 40:
                            UniversalDialog.ShowMessage("两个井了，你算算已经砸了多少狂气了？", "提示", null, this);
                            gachaing = false;
                            return;
                        case 60:
                            UniversalDialog.ShowMessage("收手吧！你不算砸了多少狂气我算了！\n你已经砸了60x1300=78000狂气了！", "提示", null, this);
                            gachaing = false;
                            return;
                        case 100:
                            UniversalDialog.ShowMessage("我是来恭喜你，你已经扔进去1000抽，简称130000狂气了。\n你花了多少时间到这里？", "提示", null, this);
                            gachaing = false;
                            return;
                        case 200:
                            UniversalDialog.ShowMessage("2000抽。\n有这个毅力你做什么都会成功的。", "提示", null, this);
                            gachaing = false;
                            return;
                    }
                    if (uniqueCount == null)
                    {
                        Log.logger.Error("uniqueCount为空。");
                        UniversalDialog.ShowMessage("抽卡完成。", "提示", null, this);
                        gachaing = false;
                        return;
                    }
                    else if (uniqueCount[0] == 9 && uniqueCount[1] == 1)
                    {
                        int choice = random.Next(1, 4);
                        if (choice == 1)
                        {
                            UniversalDialog.ShowMessage("恭喜九白一红~！", "提示", null, this);
                        }
                        else if (choice == 2)
                        {
                            UniversalDialog.ShowMessage("正常发挥正常发挥~", "提示", null, this);
                        }
                        else
                        {
                            UniversalDialog.ShowMessage("还好没拿真狂气抽吧！", "提示", null, this);
                        }
                    }
                    else if (uniqueCount[0] == 8 && uniqueCount[1] == 2)
                    {
                        int choice = random.Next(1, 4);
                        if (choice == 1)
                        {
                            UniversalDialog.ShowMessage("至少比九白一红好一点，不是么？", "提示", null, this);
                        }
                        else if (choice == 2)
                        {
                            UniversalDialog.ShowMessage("你要不先去洗洗手？", "提示", null, this);
                        }
                        else
                        {
                            UniversalDialog.ShowMessage("真是可惜，看来这次运气没有站在你这边.jpg", "提示", null, this);
                        }
                    }
                    else if (uniqueCount[0] == 7 && uniqueCount[1] == 3)
                    {
                        int choice = random.Next(1, 4);
                        if (choice == 1)
                        {
                            UniversalDialog.ShowMessage("三个二星！这是多少碎片来着？", "提示", null, this);
                        }
                        else if (choice == 2)
                        {
                            UniversalDialog.ShowMessage("工具箱的概率可是十分严谨的！\n所以肯定不是工具箱的问题！", "提示", null, this);
                        }
                        else
                        {
                            UniversalDialog.ShowMessage("要是抽不中就算了吧，散伙散伙！", "提示", null, this);
                        }
                    }
                    else if (uniqueCount[2] == 1)
                    {
                        int choice = random.Next(1, 4);
                        if (choice == 1)
                        {
                            UniversalDialog.ShowMessage("金色传说！虽然说就一个。", "提示", null, this);
                        }
                        else if (choice == 2)
                        {
                            UniversalDialog.ShowMessage("恭喜恭喜~不知道抽了多少次了？", "提示", null, this);
                        }
                        else
                        {
                            UniversalDialog.ShowMessage("ALL IN！", "提示", null, this);
                        }
                    }
                    else if (uniqueCount[2] == 2)
                    {
                        int choice = random.Next(1, 4);
                        if (choice == 1)
                        {
                            UniversalDialog.ShowMessage("双黄蛋？希望你瓦夜的时候也能这样。", "提示", null, this);
                        }
                        else if (choice == 2)
                        {
                            UniversalDialog.ShowMessage("100碎片而已，我一点都不羡慕！", "提示", null, this);
                        }
                        else
                        {
                            UniversalDialog.ShowMessage("恭喜恭喜~", "提示", null, this);
                        }
                    }
                    else if (uniqueCount[2] == 3)
                    {
                        int choice = random.Next(1, 4);
                        if (choice == 1)
                        {
                            UniversalDialog.ShowMessage("真的假的三黄。。？", "提示", null, this);
                        }
                        else if (choice == 2)
                        {
                            UniversalDialog.ShowMessage("你平时运气也这么好？！", "提示", null, this);
                        }
                        else
                        {
                            UniversalDialog.ShowMessage("爽了，再来再来！", "提示", null, this);
                        }
                    }
                    else if (uniqueCount[2] >= 4)
                    {
                        int choice = random.Next(1, 4);
                        switch (choice)
                        {
                            case 1:
                                UniversalDialog.ShowMessage("不可能……不可能啊？！", "提示", null, this);
                                break;
                            case 2:
                                UniversalDialog.ShowMessage("欧吃矛！", "提示", null, this);
                                break;
                            case 3:
                                UniversalDialog.ShowMessage("再抽池子就要空了！", "提示", null, this);
                                break;
                        }
                    }
                    else
                    {
                        int choice = random.Next(1, 4);
                        if (choice == 1)
                        {
                            UniversalDialog.ShowMessage("怎么样？再来一次么？", "提示", null, this);
                        }
                        else if (choice == 2)
                        {
                            UniversalDialog.ShowMessage("冷知识：概率真的完全真实。", "提示", null, this);
                        }
                        else
                        {
                            UniversalDialog.ShowMessage("你平时抽卡也这个结果吗？", "提示", null, this);
                        }
                    }
                    gachaing = false;
                }
            }
            finally
            {
                _isTicking = false;
            }
        }

        private async Task CollapsedAllGacha()
        {
            await this.Dispatcher.BeginInvoke(() =>
            {
                GachaPage.CollapseAllResults();
                GachaPage.SetButtonHitTestVisible(false);
            });
        }
        #endregion
    }
}
