using LLC_MOD_Toolbox.Models;
using Newtonsoft.Json.Linq;
using System.Windows;

namespace LLC_MOD_Toolbox
{
    public partial class MainWindow : Window
    {
        #region MirrorChyan
        internal void CheckMirrorChyan()
        {
            if (configuation.Settings.mirrorChyan.enable && SecureStringStorage.HasSavedData())
            {
                HandleExistingSetup();
            }
        }

        private void HandleExistingSetup()
        {
            mirrorChyanToken = SecureStringStorage.LoadToken();
            if (!string.IsNullOrWhiteSpace(mirrorChyanToken))
            {
                Log.logger.Info("设置Mirror酱模式。");
                this.Dispatcher.Invoke(() =>
                {
                    MirrorChyanLogo.Visibility = Visibility.Visible;
                });
                isMirrorChyanMode = true;
                MirrorChyanConfigButtonLabelChanger(isMirrorChyanMode);
                return;
            }

            bool result = UniversalDialog.ShowConfirm("读取Mirror酱秘钥失败，你想要再输入一次秘钥吗？", "提示", this);
            if (result)
            {
                HandleTokenReInput();
            }
            else
            {
                DisableMirrorChyanMode();
            }
        }

        private void HandleTokenReInput()
        {
            var token = ShowMirrorChyanDialog();
            if (string.IsNullOrWhiteSpace(token))
            {
                return;
            }

            SetupMirrorChyanMode(token);
            Log.logger.Info("MirrorChyan Mode 已开启。");
        }

        private void SetupMirrorChyanMode(string token)
        {
            Log.logger.Info("设置Mirror酱模式。");
            this.Dispatcher.Invoke(() =>
            {
                MirrorChyanLogo.Visibility = Visibility.Visible;
            });
            isMirrorChyanMode = true;
            MirrorChyanConfigButtonLabelChanger(isMirrorChyanMode);
            mirrorChyanToken = token.Trim();
            SecureStringStorage.SaveToken(mirrorChyanToken);
            configuation.Settings.mirrorChyan.enable = true;
            configuation.SaveConfig();
        }

        private void DisableMirrorChyanMode()
        {
            configuation.Settings.mirrorChyan.enable = false;
            configuation.SaveConfig();
            Log.logger.Info("MirrorChyan Mode 已关闭。");
        }

        private string? ShowMirrorChyanDialog()
        {
            const string message = "Mirror酱是一个第三方应用分发平台，让开源应用的更新更简单。\n" +
                                  "用户付费使用，收益与开发者共享。\n" +
                                  "如果你拥有Mirror酱秘钥，能够缓解你在使用本软件时可能遇到的网络问题。\n" +
                                  "没有？没关系，你也可以忽略本提示，零协会仍然提供免费镜像源。\n" +
                                  "想了解一下？点击右下角按钮。";

            const string title = "等下，你有Mirror酱秘钥吗？";

            return InputMirrorChyan.ShowDialog(message, title, "Mirror酱秘钥", true, this);
        }

        internal JObject ParseMirrorChyanJson(string json)
        {
            JObject parsed = JObject.Parse(json);
            int code = parsed["code"].Value<int>();
            if (code != 0)
            {
                throw new MirrorChyanException(code);
            }
            return parsed;
        }

        internal void MirrorChyanConfigButtonLabelChanger(bool mirrorChyanMode)
        {
            Dispatcher.Invoke(() =>
            {
                if (mirrorChyanMode)
                {
                    ViewModel.MirrorChyanButtonText = "禁用";
                }
                else
                {
                    ViewModel.MirrorChyanButtonText = "填写秘钥";
                }
            });
        }

        internal void HandleMirrorChyanConfigRequested()
        {
            if (isMirrorChyanMode)
            {
                bool result = UniversalDialog.ShowConfirm("确定要禁用Mirror酱吗？\n关闭后，你可以在设置重新开启Mirror酱的服务。", "提示", this);
                if (!result)
                {
                    return;
                }
                SecureStringStorage.DeleteSecretFile();
                configuation.Settings.mirrorChyan.enable = false;
                configuation.SaveConfig();
                UniversalDialog.ShowMessage("已禁用Mirror酱并删除你的Mirror酱CDK。\n为了处理，软件将关闭，再次启动后效果生效。", "提示", null, this);
                Application.Current.Shutdown();
            }
            else
            {
                var result = UniversalDialog.ShowInput(
                    "请输入你的 Mirror 酱 CDK。\n你可以在 Mirror 酱官网购买。",
                    "输入秘钥",
                    "Mirror 酱 CDK",
                    InputType.Password,
                    [new DialogButton("确定", true, false), new DialogButton("取消", false, true)],
                    this);
                if (result.IsCanceled)
                {
                    return;
                }
                if (result.IsSuccess && !string.IsNullOrEmpty(result.Input))
                {
                    SetupMirrorChyanMode(result.Input);
                    UniversalDialog.ShowMessage("Mirror酱秘钥设置成功。\n为了处理，软件将关闭，再次启动后效果生效。", "提示", null, this);
                    Application.Current.Shutdown();
                }
                else
                {
                    UniversalDialog.ShowMessage("设置失败。", "提示", null, this);
                }
            }
        }
        #endregion
    }
}
