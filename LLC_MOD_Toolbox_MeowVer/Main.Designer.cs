using System;

namespace LLC_MOD_Toolbox_MeowVer
{
    partial class Main
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.TabControlMenu = new Sunny.UI.UITabControlMenu();
            this.InstallPage = new System.Windows.Forms.TabPage();
            this.TabInstallControlMenu = new Sunny.UI.UITabControl();
            this.AutoInstallPage = new System.Windows.Forms.TabPage();
            this.InstallButton = new Sunny.UI.UISymbolButton();
            this.TotalProcessBar = new Sunny.UI.UIProcessBar();
            this.DownloadBar = new Sunny.UI.UIProcessBar();
            this.ManualInstallPage = new System.Windows.Forms.TabPage();
            this.OverwriteFilePage = new System.Windows.Forms.TabPage();
            this.ConfigPage = new System.Windows.Forms.TabPage();
            this.GreyscaleTestPage = new System.Windows.Forms.TabPage();
            this.SettingsPage = new System.Windows.Forms.TabPage();
            this.DeleteModButton = new Sunny.UI.UIButton();
            this.DeleteModLabel = new Sunny.UI.UILabel();
            this.NodeComboBox = new Sunny.UI.UIComboBox();
            this.AdvancedSettings = new Sunny.UI.UIGroupBox();
            this.UseLegacyDownloadSwitch = new Sunny.UI.UISwitch();
            this.UseLegacyDownload = new Sunny.UI.UILabel();
            this.SettingsLine = new Sunny.UI.UILine();
            this.AboutPage = new System.Windows.Forms.TabPage();
            this.AboutHuijiWikiButton = new Sunny.UI.UISymbolButton();
            this.AboutAfdianButton = new Sunny.UI.UISymbolButton();
            this.uiLine1 = new Sunny.UI.UILine();
            this.AboutMemberButton = new Sunny.UI.UISymbolButton();
            this.uiLabel1 = new Sunny.UI.UILabel();
            this.DebugPage = new System.Windows.Forms.TabPage();
            this.DebugButton1 = new Sunny.UI.UIButton();
            this.StyleManager = new Sunny.UI.UIStyleManager(this.components);
            this.banner = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.TabControlMenu.SuspendLayout();
            this.InstallPage.SuspendLayout();
            this.TabInstallControlMenu.SuspendLayout();
            this.AutoInstallPage.SuspendLayout();
            this.ManualInstallPage.SuspendLayout();
            this.OverwriteFilePage.SuspendLayout();
            this.ConfigPage.SuspendLayout();
            this.GreyscaleTestPage.SuspendLayout();
            this.SettingsPage.SuspendLayout();
            this.AdvancedSettings.SuspendLayout();
            this.AboutPage.SuspendLayout();
            this.DebugPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.banner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            this.SuspendLayout();
            // 
            // TabControlMenu
            // 
            this.TabControlMenu.Alignment = System.Windows.Forms.TabAlignment.Left;
            this.TabControlMenu.Controls.Add(this.InstallPage);
            this.TabControlMenu.Controls.Add(this.ConfigPage);
            this.TabControlMenu.Controls.Add(this.GreyscaleTestPage);
            this.TabControlMenu.Controls.Add(this.SettingsPage);
            this.TabControlMenu.Controls.Add(this.AboutPage);
            this.TabControlMenu.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.TabControlMenu.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.TabControlMenu.Location = new System.Drawing.Point(3, 131);
            this.TabControlMenu.Multiline = true;
            this.TabControlMenu.Name = "TabControlMenu";
            this.TabControlMenu.SelectedIndex = 0;
            this.TabControlMenu.Size = new System.Drawing.Size(1063, 535);
            this.TabControlMenu.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.TabControlMenu.TabIndex = 1;
            // 
            // InstallPage
            // 
            this.InstallPage.Controls.Add(this.TabInstallControlMenu);
            this.InstallPage.Location = new System.Drawing.Point(201, 0);
            this.InstallPage.Name = "InstallPage";
            this.InstallPage.Size = new System.Drawing.Size(862, 535);
            this.InstallPage.TabIndex = 0;
            this.InstallPage.Text = "安装";
            this.InstallPage.UseVisualStyleBackColor = true;
            // 
            // TabInstallControlMenu
            // 
            this.TabInstallControlMenu.Controls.Add(this.AutoInstallPage);
            this.TabInstallControlMenu.Controls.Add(this.ManualInstallPage);
            this.TabInstallControlMenu.Controls.Add(this.OverwriteFilePage);
            this.TabInstallControlMenu.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.TabInstallControlMenu.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.TabInstallControlMenu.ItemSize = new System.Drawing.Size(150, 45);
            this.TabInstallControlMenu.Location = new System.Drawing.Point(0, 0);
            this.TabInstallControlMenu.MainPage = "";
            this.TabInstallControlMenu.Name = "TabInstallControlMenu";
            this.TabInstallControlMenu.SelectedIndex = 0;
            this.TabInstallControlMenu.Size = new System.Drawing.Size(862, 535);
            this.TabInstallControlMenu.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.TabInstallControlMenu.TabIndex = 0;
            this.TabInstallControlMenu.TipsFont = new System.Drawing.Font("微软雅黑", 9F);
            // 
            // AutoInstallPage
            // 
            this.AutoInstallPage.Controls.Add(this.InstallButton);
            this.AutoInstallPage.Controls.Add(this.TotalProcessBar);
            this.AutoInstallPage.Controls.Add(this.DownloadBar);
            this.AutoInstallPage.Location = new System.Drawing.Point(0, 45);
            this.AutoInstallPage.Name = "AutoInstallPage";
            this.AutoInstallPage.Size = new System.Drawing.Size(862, 490);
            this.AutoInstallPage.TabIndex = 0;
            this.AutoInstallPage.Text = "自动安装";
            this.AutoInstallPage.UseVisualStyleBackColor = true;
            // 
            // InstallButton
            // 
            this.InstallButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.InstallButton.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.InstallButton.Location = new System.Drawing.Point(238, 160);
            this.InstallButton.MinimumSize = new System.Drawing.Size(1, 1);
            this.InstallButton.Name = "InstallButton";
            this.InstallButton.Radius = 10;
            this.InstallButton.Size = new System.Drawing.Size(300, 136);
            this.InstallButton.Symbol = 560273;
            this.InstallButton.TabIndex = 2;
            this.InstallButton.Text = "一键安装/更新";
            this.InstallButton.TipsFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.InstallButton.Click += new System.EventHandler(this.InstallButton_Click);
            // 
            // TotalProcessBar
            // 
            this.TotalProcessBar.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.TotalProcessBar.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.TotalProcessBar.Location = new System.Drawing.Point(238, 369);
            this.TotalProcessBar.MinimumSize = new System.Drawing.Size(3, 3);
            this.TotalProcessBar.Name = "TotalProcessBar";
            this.TotalProcessBar.RadiusSides = ((Sunny.UI.UICornerRadiusSides)((Sunny.UI.UICornerRadiusSides.LeftTop | Sunny.UI.UICornerRadiusSides.RightTop)));
            this.TotalProcessBar.Size = new System.Drawing.Size(300, 29);
            this.TotalProcessBar.TabIndex = 1;
            this.TotalProcessBar.Text = "uiProcessBar1";
            // 
            // DownloadBar
            // 
            this.DownloadBar.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.DownloadBar.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.DownloadBar.Location = new System.Drawing.Point(238, 397);
            this.DownloadBar.MinimumSize = new System.Drawing.Size(3, 3);
            this.DownloadBar.Name = "DownloadBar";
            this.DownloadBar.RadiusSides = ((Sunny.UI.UICornerRadiusSides)((Sunny.UI.UICornerRadiusSides.RightBottom | Sunny.UI.UICornerRadiusSides.LeftBottom)));
            this.DownloadBar.Size = new System.Drawing.Size(300, 16);
            this.DownloadBar.TabIndex = 0;
            this.DownloadBar.Text = "下载进度条";
            // 
            // ManualInstallPage
            // 
            this.ManualInstallPage.Controls.Add(this.pictureBox1);
            this.ManualInstallPage.Location = new System.Drawing.Point(0, 45);
            this.ManualInstallPage.Name = "ManualInstallPage";
            this.ManualInstallPage.Size = new System.Drawing.Size(862, 490);
            this.ManualInstallPage.TabIndex = 1;
            this.ManualInstallPage.Text = "手动安装";
            this.ManualInstallPage.UseVisualStyleBackColor = true;
            // 
            // OverwriteFilePage
            // 
            this.OverwriteFilePage.Controls.Add(this.pictureBox2);
            this.OverwriteFilePage.Location = new System.Drawing.Point(0, 45);
            this.OverwriteFilePage.Name = "OverwriteFilePage";
            this.OverwriteFilePage.Size = new System.Drawing.Size(862, 490);
            this.OverwriteFilePage.TabIndex = 2;
            this.OverwriteFilePage.Text = "文件覆盖";
            this.OverwriteFilePage.UseVisualStyleBackColor = true;
            // 
            // ConfigPage
            // 
            this.ConfigPage.Controls.Add(this.pictureBox3);
            this.ConfigPage.Location = new System.Drawing.Point(201, 0);
            this.ConfigPage.Name = "ConfigPage";
            this.ConfigPage.Size = new System.Drawing.Size(862, 535);
            this.ConfigPage.TabIndex = 4;
            this.ConfigPage.Text = "模组配置";
            this.ConfigPage.UseVisualStyleBackColor = true;
            // 
            // GreyscaleTestPage
            // 
            this.GreyscaleTestPage.Controls.Add(this.pictureBox4);
            this.GreyscaleTestPage.Location = new System.Drawing.Point(201, 0);
            this.GreyscaleTestPage.Name = "GreyscaleTestPage";
            this.GreyscaleTestPage.Size = new System.Drawing.Size(862, 535);
            this.GreyscaleTestPage.TabIndex = 3;
            this.GreyscaleTestPage.Text = "灰度测试";
            this.GreyscaleTestPage.UseVisualStyleBackColor = true;
            // 
            // SettingsPage
            // 
            this.SettingsPage.Controls.Add(this.DeleteModButton);
            this.SettingsPage.Controls.Add(this.DeleteModLabel);
            this.SettingsPage.Controls.Add(this.NodeComboBox);
            this.SettingsPage.Controls.Add(this.AdvancedSettings);
            this.SettingsPage.Controls.Add(this.SettingsLine);
            this.SettingsPage.Location = new System.Drawing.Point(201, 0);
            this.SettingsPage.Name = "SettingsPage";
            this.SettingsPage.Size = new System.Drawing.Size(862, 535);
            this.SettingsPage.TabIndex = 2;
            this.SettingsPage.Text = "设置";
            this.SettingsPage.UseVisualStyleBackColor = true;
            // 
            // DeleteModButton
            // 
            this.DeleteModButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.DeleteModButton.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.DeleteModButton.Location = new System.Drawing.Point(291, 60);
            this.DeleteModButton.MinimumSize = new System.Drawing.Size(1, 1);
            this.DeleteModButton.Name = "DeleteModButton";
            this.DeleteModButton.Size = new System.Drawing.Size(100, 35);
            this.DeleteModButton.TabIndex = 5;
            this.DeleteModButton.Text = "卸载";
            this.DeleteModButton.TipsFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.DeleteModButton.Click += new System.EventHandler(this.DeleteModButton_Click);
            // 
            // DeleteModLabel
            // 
            this.DeleteModLabel.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.DeleteModLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.DeleteModLabel.Location = new System.Drawing.Point(17, 61);
            this.DeleteModLabel.Name = "DeleteModLabel";
            this.DeleteModLabel.Size = new System.Drawing.Size(242, 37);
            this.DeleteModLabel.TabIndex = 4;
            this.DeleteModLabel.Text = "卸载模组";
            this.DeleteModLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // NodeComboBox
            // 
            this.NodeComboBox.DataSource = null;
            this.NodeComboBox.FillColor = System.Drawing.Color.White;
            this.NodeComboBox.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.NodeComboBox.ItemHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.NodeComboBox.ItemSelectForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(243)))), ((int)(((byte)(255)))));
            this.NodeComboBox.Location = new System.Drawing.Point(17, 18);
            this.NodeComboBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.NodeComboBox.MinimumSize = new System.Drawing.Size(63, 0);
            this.NodeComboBox.Name = "NodeComboBox";
            this.NodeComboBox.Padding = new System.Windows.Forms.Padding(0, 0, 30, 2);
            this.NodeComboBox.Size = new System.Drawing.Size(374, 34);
            this.NodeComboBox.SymbolSize = 24;
            this.NodeComboBox.TabIndex = 3;
            this.NodeComboBox.Text = "手动选择节点";
            this.NodeComboBox.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            this.NodeComboBox.Watermark = "";
            this.NodeComboBox.SelectedIndexChanged += new System.EventHandler(this.NodeComboBox_SelectedIndexChanged);
            // 
            // AdvancedSettings
            // 
            this.AdvancedSettings.Controls.Add(this.UseLegacyDownloadSwitch);
            this.AdvancedSettings.Controls.Add(this.UseLegacyDownload);
            this.AdvancedSettings.FillColor = System.Drawing.Color.White;
            this.AdvancedSettings.FillColor2 = System.Drawing.Color.White;
            this.AdvancedSettings.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.AdvancedSettings.Location = new System.Drawing.Point(408, 195);
            this.AdvancedSettings.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.AdvancedSettings.MinimumSize = new System.Drawing.Size(1, 1);
            this.AdvancedSettings.Name = "AdvancedSettings";
            this.AdvancedSettings.Padding = new System.Windows.Forms.Padding(0, 32, 0, 0);
            this.AdvancedSettings.Size = new System.Drawing.Size(450, 335);
            this.AdvancedSettings.TabIndex = 2;
            this.AdvancedSettings.Text = "高级选项（请不要轻易改动）";
            this.AdvancedSettings.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // UseLegacyDownloadSwitch
            // 
            this.UseLegacyDownloadSwitch.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.UseLegacyDownloadSwitch.Location = new System.Drawing.Point(319, 45);
            this.UseLegacyDownloadSwitch.MinimumSize = new System.Drawing.Size(1, 1);
            this.UseLegacyDownloadSwitch.Name = "UseLegacyDownloadSwitch";
            this.UseLegacyDownloadSwitch.Size = new System.Drawing.Size(109, 29);
            this.UseLegacyDownloadSwitch.TabIndex = 1;
            this.UseLegacyDownloadSwitch.Text = "uiSwitch1";
            // 
            // UseLegacyDownload
            // 
            this.UseLegacyDownload.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.UseLegacyDownload.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.UseLegacyDownload.Location = new System.Drawing.Point(14, 46);
            this.UseLegacyDownload.Name = "UseLegacyDownload";
            this.UseLegacyDownload.Size = new System.Drawing.Size(185, 29);
            this.UseLegacyDownload.TabIndex = 0;
            this.UseLegacyDownload.Text = "使用旧版下载方式";
            this.UseLegacyDownload.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // SettingsLine
            // 
            this.SettingsLine.BackColor = System.Drawing.Color.Transparent;
            this.SettingsLine.Direction = Sunny.UI.UILine.LineDirection.Vertical;
            this.SettingsLine.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.SettingsLine.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.SettingsLine.Location = new System.Drawing.Point(390, 3);
            this.SettingsLine.MinimumSize = new System.Drawing.Size(1, 1);
            this.SettingsLine.Name = "SettingsLine";
            this.SettingsLine.Size = new System.Drawing.Size(26, 532);
            this.SettingsLine.TabIndex = 1;
            this.SettingsLine.Text = "uiLine2";
            // 
            // AboutPage
            // 
            this.AboutPage.Controls.Add(this.AboutHuijiWikiButton);
            this.AboutPage.Controls.Add(this.AboutAfdianButton);
            this.AboutPage.Controls.Add(this.uiLine1);
            this.AboutPage.Controls.Add(this.AboutMemberButton);
            this.AboutPage.Controls.Add(this.uiLabel1);
            this.AboutPage.Location = new System.Drawing.Point(201, 0);
            this.AboutPage.Name = "AboutPage";
            this.AboutPage.Size = new System.Drawing.Size(862, 535);
            this.AboutPage.TabIndex = 5;
            this.AboutPage.Text = "关于";
            this.AboutPage.UseVisualStyleBackColor = true;
            // 
            // AboutHuijiWikiButton
            // 
            this.AboutHuijiWikiButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.AboutHuijiWikiButton.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.AboutHuijiWikiButton.Location = new System.Drawing.Point(420, 111);
            this.AboutHuijiWikiButton.MinimumSize = new System.Drawing.Size(1, 1);
            this.AboutHuijiWikiButton.Name = "AboutHuijiWikiButton";
            this.AboutHuijiWikiButton.Size = new System.Drawing.Size(417, 35);
            this.AboutHuijiWikiButton.Symbol = 61912;
            this.AboutHuijiWikiButton.TabIndex = 4;
            this.AboutHuijiWikiButton.Text = "想看看边狱公司的维基吗？";
            this.AboutHuijiWikiButton.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.AboutHuijiWikiButton.Click += new System.EventHandler(this.AboutHuijiWikiButton_Click);
            // 
            // AboutAfdianButton
            // 
            this.AboutAfdianButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.AboutAfdianButton.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.AboutAfdianButton.Location = new System.Drawing.Point(420, 70);
            this.AboutAfdianButton.MinimumSize = new System.Drawing.Size(1, 1);
            this.AboutAfdianButton.Name = "AboutAfdianButton";
            this.AboutAfdianButton.Size = new System.Drawing.Size(417, 35);
            this.AboutAfdianButton.Symbol = 61444;
            this.AboutAfdianButton.TabIndex = 3;
            this.AboutAfdianButton.Text = "到爱发电支持我们！";
            this.AboutAfdianButton.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.AboutAfdianButton.Click += new System.EventHandler(this.AboutAfdianButton_Click);
            // 
            // uiLine1
            // 
            this.uiLine1.BackColor = System.Drawing.Color.Transparent;
            this.uiLine1.Direction = Sunny.UI.UILine.LineDirection.Vertical;
            this.uiLine1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLine1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLine1.Location = new System.Drawing.Point(372, 29);
            this.uiLine1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiLine1.Name = "uiLine1";
            this.uiLine1.Size = new System.Drawing.Size(42, 493);
            this.uiLine1.TabIndex = 2;
            this.uiLine1.Text = "uiLine1";
            // 
            // AboutMemberButton
            // 
            this.AboutMemberButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.AboutMemberButton.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.AboutMemberButton.Location = new System.Drawing.Point(421, 29);
            this.AboutMemberButton.MinimumSize = new System.Drawing.Size(1, 1);
            this.AboutMemberButton.Name = "AboutMemberButton";
            this.AboutMemberButton.Size = new System.Drawing.Size(417, 35);
            this.AboutMemberButton.Symbol = 358781;
            this.AboutMemberButton.TabIndex = 1;
            this.AboutMemberButton.Text = "查看零协会的成员们！";
            this.AboutMemberButton.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.AboutMemberButton.Click += new System.EventHandler(this.AboutMemberButton_Click);
            // 
            // uiLabel1
            // 
            this.uiLabel1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiLabel1.Location = new System.Drawing.Point(13, 14);
            this.uiLabel1.Name = "uiLabel1";
            this.uiLabel1.Size = new System.Drawing.Size(401, 508);
            this.uiLabel1.TabIndex = 0;
            this.uiLabel1.Text = "都市零协会汉化组工具箱（LLC_MOD_Toolbox）\r\n汉化文本：全体零协会成员！\r\n零协会组长：北岚\r\n汉化模组：奈芙\r\n汉化字体：茜，北岚\r\n工具箱：曾小" +
    "皮，1029\r\n服务器及镜像支持：曾小皮";
            this.uiLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // DebugPage
            // 
            this.DebugPage.Controls.Add(this.DebugButton1);
            this.DebugPage.Location = new System.Drawing.Point(201, 0);
            this.DebugPage.Name = "DebugPage";
            this.DebugPage.Size = new System.Drawing.Size(862, 535);
            this.DebugPage.TabIndex = 6;
            this.DebugPage.Text = "调试页面";
            this.DebugPage.UseVisualStyleBackColor = true;
            // 
            // DebugButton1
            // 
            this.DebugButton1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.DebugButton1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.DebugButton1.Location = new System.Drawing.Point(4, 4);
            this.DebugButton1.MinimumSize = new System.Drawing.Size(1, 1);
            this.DebugButton1.Name = "DebugButton1";
            this.DebugButton1.Size = new System.Drawing.Size(204, 41);
            this.DebugButton1.TabIndex = 0;
            this.DebugButton1.Text = "下载测试文件";
            this.DebugButton1.TipsFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.DebugButton1.Click += new System.EventHandler(this.DebugButton1_Click);
            // 
            // StyleManager
            // 
            this.StyleManager.GlobalFontName = "微软雅黑";
            // 
            // banner
            // 
            this.banner.Image = global::LLC_MOD_Toolbox_MeowVer.Properties.Resources.bannnnerrr;
            this.banner.Location = new System.Drawing.Point(4, 39);
            this.banner.Name = "banner";
            this.banner.Size = new System.Drawing.Size(1050, 86);
            this.banner.TabIndex = 2;
            this.banner.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::LLC_MOD_Toolbox_MeowVer.Properties.Resources.QvQ;
            this.pictureBox1.Location = new System.Drawing.Point(155, 122);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(466, 172);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::LLC_MOD_Toolbox_MeowVer.Properties.Resources.QvQ;
            this.pictureBox2.Location = new System.Drawing.Point(198, 159);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(466, 172);
            this.pictureBox2.TabIndex = 1;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::LLC_MOD_Toolbox_MeowVer.Properties.Resources.QvQ;
            this.pictureBox3.Location = new System.Drawing.Point(198, 181);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(466, 172);
            this.pictureBox3.TabIndex = 1;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox4
            // 
            this.pictureBox4.Image = global::LLC_MOD_Toolbox_MeowVer.Properties.Resources.QvQ;
            this.pictureBox4.Location = new System.Drawing.Point(198, 181);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(466, 172);
            this.pictureBox4.TabIndex = 1;
            this.pictureBox4.TabStop = false;
            // 
            // Main
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1069, 669);
            this.Controls.Add(this.banner);
            this.Controls.Add(this.TabControlMenu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Main";
            this.ShowIcon = false;
            this.Text = "零协会工具箱";
            this.TitleFont = new System.Drawing.Font("微软雅黑", 12F);
            this.ZoomScaleRect = new System.Drawing.Rectangle(15, 15, 800, 450);
            this.Load += new System.EventHandler(this.Main_Load);
            this.TabControlMenu.ResumeLayout(false);
            this.InstallPage.ResumeLayout(false);
            this.TabInstallControlMenu.ResumeLayout(false);
            this.AutoInstallPage.ResumeLayout(false);
            this.ManualInstallPage.ResumeLayout(false);
            this.OverwriteFilePage.ResumeLayout(false);
            this.ConfigPage.ResumeLayout(false);
            this.GreyscaleTestPage.ResumeLayout(false);
            this.SettingsPage.ResumeLayout(false);
            this.AdvancedSettings.ResumeLayout(false);
            this.AboutPage.ResumeLayout(false);
            this.DebugPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.banner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Sunny.UI.UITabControlMenu TabControlMenu;
        private System.Windows.Forms.TabPage InstallPage;
        private Sunny.UI.UIStyleManager StyleManager;
        private Sunny.UI.UITabControl TabInstallControlMenu;
        private System.Windows.Forms.TabPage AutoInstallPage;
        private System.Windows.Forms.TabPage ManualInstallPage;
        private System.Windows.Forms.TabPage OverwriteFilePage;
        private System.Windows.Forms.TabPage ConfigPage;
        private System.Windows.Forms.TabPage GreyscaleTestPage;
        private System.Windows.Forms.TabPage SettingsPage;
        private System.Windows.Forms.TabPage AboutPage;
        private Sunny.UI.UILabel uiLabel1;
        private System.Windows.Forms.TabPage DebugPage;
        private Sunny.UI.UILine uiLine1;
        private Sunny.UI.UISymbolButton AboutMemberButton;
        private Sunny.UI.UIProcessBar DownloadBar;
        private Sunny.UI.UIButton DebugButton1;
        private Sunny.UI.UIGroupBox AdvancedSettings;
        private Sunny.UI.UISwitch UseLegacyDownloadSwitch;
        private Sunny.UI.UILabel UseLegacyDownload;
        private Sunny.UI.UILine SettingsLine;
        private Sunny.UI.UISymbolButton AboutAfdianButton;
        private Sunny.UI.UIProcessBar TotalProcessBar;
        private Sunny.UI.UISymbolButton InstallButton;
        private System.Windows.Forms.PictureBox banner;
        private Sunny.UI.UISymbolButton AboutHuijiWikiButton;
        private Sunny.UI.UIComboBox NodeComboBox;
        private Sunny.UI.UIButton DeleteModButton;
        private Sunny.UI.UILabel DeleteModLabel;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.PictureBox pictureBox4;
    }
}

