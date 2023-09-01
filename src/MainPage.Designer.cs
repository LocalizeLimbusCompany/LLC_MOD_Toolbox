namespace LLC_MOD_Toolbox
{
    partial class MainPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainPage));
            this.uiTabControl = new Sunny.UI.UITabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.statu = new Sunny.UI.UILabel();
            this.DownloadBar = new Sunny.UI.UIProcessBar();
            this.TotalBar = new Sunny.UI.UIProcessBar();
            this.installButton = new Sunny.UI.UISymbolButton();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.EnterAfdian = new Sunny.UI.UISymbolButton();
            this.EnterQuestion = new Sunny.UI.UISymbolButton();
            this.EnterLLCG = new Sunny.UI.UISymbolButton();
            this.EnterParatranz = new Sunny.UI.UISymbolButton();
            this.EnterBilibili = new Sunny.UI.UISymbolButton();
            this.EnterWebsite = new Sunny.UI.UISymbolButton();
            this.EnterSteampp = new Sunny.UI.UISymbolButton();
            this.EnterLLCGithub = new Sunny.UI.UISymbolButton();
            this.EnterWiki = new Sunny.UI.UISymbolButton();
            this.EnterToolBoxGithub = new Sunny.UI.UISymbolButton();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.uiGroupBox2 = new Sunny.UI.UIGroupBox();
            this.deleteButton = new Sunny.UI.UIButton();
            this.uiLabel7 = new Sunny.UI.UILabel();
            this.useGithub = new Sunny.UI.UISwitch();
            this.uiLabel2 = new Sunny.UI.UILabel();
            this.uiGroupBox1 = new Sunny.UI.UIGroupBox();
            this.dlFromDefault = new Sunny.UI.UIButton();
            this.uiLabel6 = new Sunny.UI.UILabel();
            this.dlFromLVCDN = new Sunny.UI.UIButton();
            this.uiLabel5 = new Sunny.UI.UILabel();
            this.dlFromLV = new Sunny.UI.UIButton();
            this.uiLabel4 = new Sunny.UI.UILabel();
            this.dlFromOFB = new Sunny.UI.UIButton();
            this.uiLabel3 = new Sunny.UI.UILabel();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.uiLabel9 = new Sunny.UI.UILabel();
            this.uiLabel8 = new Sunny.UI.UILabel();
            this.uiTabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.uiGroupBox2.SuspendLayout();
            this.uiGroupBox1.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.SuspendLayout();
            // 
            // uiTabControl
            // 
            this.uiTabControl.Controls.Add(this.tabPage1);
            this.uiTabControl.Controls.Add(this.tabPage2);
            this.uiTabControl.Controls.Add(this.tabPage3);
            this.uiTabControl.Controls.Add(this.tabPage4);
            this.uiTabControl.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.uiTabControl.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(248)))), ((int)(((byte)(248)))));
            this.uiTabControl.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiTabControl.Frame = this;
            this.uiTabControl.ItemSize = new System.Drawing.Size(150, 40);
            this.uiTabControl.Location = new System.Drawing.Point(3, 43);
            this.uiTabControl.MainPage = "";
            this.uiTabControl.MenuStyle = Sunny.UI.UIMenuStyle.White;
            this.uiTabControl.Name = "uiTabControl";
            this.uiTabControl.SelectedIndex = 0;
            this.uiTabControl.Size = new System.Drawing.Size(1030, 448);
            this.uiTabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.uiTabControl.Style = Sunny.UI.UIStyle.LayuiGreen;
            this.uiTabControl.TabBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.uiTabControl.TabIndex = 0;
            this.uiTabControl.TabSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.uiTabControl.TabSelectedForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.uiTabControl.TabSelectedHighColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.uiTabControl.TabUnSelectedForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.uiTabControl.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(248)))), ((int)(((byte)(248)))));
            this.tabPage1.Controls.Add(this.statu);
            this.tabPage1.Controls.Add(this.DownloadBar);
            this.tabPage1.Controls.Add(this.TotalBar);
            this.tabPage1.Controls.Add(this.installButton);
            this.tabPage1.Location = new System.Drawing.Point(0, 40);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(1030, 408);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "安装页面";
            // 
            // statu
            // 
            this.statu.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.statu.Location = new System.Drawing.Point(237, 21);
            this.statu.Name = "statu";
            this.statu.Size = new System.Drawing.Size(498, 63);
            this.statu.Style = Sunny.UI.UIStyle.LayuiGreen;
            this.statu.TabIndex = 3;
            this.statu.Text = "正在加载中……";
            this.statu.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DownloadBar
            // 
            this.DownloadBar.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(248)))), ((int)(((byte)(248)))));
            this.DownloadBar.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.DownloadBar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.DownloadBar.Location = new System.Drawing.Point(330, 303);
            this.DownloadBar.MinimumSize = new System.Drawing.Size(70, 3);
            this.DownloadBar.Name = "DownloadBar";
            this.DownloadBar.RadiusSides = ((Sunny.UI.UICornerRadiusSides)((Sunny.UI.UICornerRadiusSides.RightBottom | Sunny.UI.UICornerRadiusSides.LeftBottom)));
            this.DownloadBar.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.DownloadBar.Size = new System.Drawing.Size(312, 19);
            this.DownloadBar.Style = Sunny.UI.UIStyle.LayuiGreen;
            this.DownloadBar.TabIndex = 2;
            this.DownloadBar.Text = "DownloadBar";
            // 
            // TotalBar
            // 
            this.TotalBar.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(248)))), ((int)(((byte)(248)))));
            this.TotalBar.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.TotalBar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.TotalBar.Location = new System.Drawing.Point(330, 275);
            this.TotalBar.MinimumSize = new System.Drawing.Size(70, 3);
            this.TotalBar.Name = "TotalBar";
            this.TotalBar.RadiusSides = ((Sunny.UI.UICornerRadiusSides)((Sunny.UI.UICornerRadiusSides.LeftTop | Sunny.UI.UICornerRadiusSides.RightTop)));
            this.TotalBar.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.TotalBar.Size = new System.Drawing.Size(312, 29);
            this.TotalBar.Style = Sunny.UI.UIStyle.LayuiGreen;
            this.TotalBar.TabIndex = 1;
            this.TotalBar.Text = "TotalBar";
            // 
            // installButton
            // 
            this.installButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.installButton.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.installButton.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.installButton.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(171)))), ((int)(((byte)(160)))));
            this.installButton.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.installButton.FillSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.installButton.Font = new System.Drawing.Font("微软雅黑", 15F);
            this.installButton.Location = new System.Drawing.Point(330, 102);
            this.installButton.MinimumSize = new System.Drawing.Size(1, 1);
            this.installButton.Name = "installButton";
            this.installButton.Radius = 20;
            this.installButton.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.installButton.RectHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(171)))), ((int)(((byte)(160)))));
            this.installButton.RectPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.installButton.RectSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.installButton.Size = new System.Drawing.Size(312, 121);
            this.installButton.Style = Sunny.UI.UIStyle.LayuiGreen;
            this.installButton.TabIndex = 0;
            this.installButton.Text = "立刻安装/更新";
            this.installButton.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.installButton.Click += new System.EventHandler(this.installButton_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(248)))), ((int)(((byte)(248)))));
            this.tabPage2.Controls.Add(this.EnterAfdian);
            this.tabPage2.Controls.Add(this.EnterQuestion);
            this.tabPage2.Controls.Add(this.EnterLLCG);
            this.tabPage2.Controls.Add(this.EnterParatranz);
            this.tabPage2.Controls.Add(this.EnterBilibili);
            this.tabPage2.Controls.Add(this.EnterWebsite);
            this.tabPage2.Controls.Add(this.EnterSteampp);
            this.tabPage2.Controls.Add(this.EnterLLCGithub);
            this.tabPage2.Controls.Add(this.EnterWiki);
            this.tabPage2.Controls.Add(this.EnterToolBoxGithub);
            this.tabPage2.Location = new System.Drawing.Point(0, 40);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(200, 60);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "链接";
            // 
            // EnterAfdian
            // 
            this.EnterAfdian.Cursor = System.Windows.Forms.Cursors.Hand;
            this.EnterAfdian.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.EnterAfdian.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.EnterAfdian.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(171)))), ((int)(((byte)(160)))));
            this.EnterAfdian.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.EnterAfdian.FillSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.EnterAfdian.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.EnterAfdian.Location = new System.Drawing.Point(281, 211);
            this.EnterAfdian.MinimumSize = new System.Drawing.Size(1, 1);
            this.EnterAfdian.Name = "EnterAfdian";
            this.EnterAfdian.Radius = 10;
            this.EnterAfdian.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.EnterAfdian.RectHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(171)))), ((int)(((byte)(160)))));
            this.EnterAfdian.RectPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.EnterAfdian.RectSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.EnterAfdian.Size = new System.Drawing.Size(194, 46);
            this.EnterAfdian.Style = Sunny.UI.UIStyle.LayuiGreen;
            this.EnterAfdian.Symbol = 61654;
            this.EnterAfdian.TabIndex = 11;
            this.EnterAfdian.Text = "爱发电（赞助渠道）";
            this.EnterAfdian.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.EnterAfdian.Click += new System.EventHandler(this.EnterAfdian_Click);
            // 
            // EnterQuestion
            // 
            this.EnterQuestion.Cursor = System.Windows.Forms.Cursors.Hand;
            this.EnterQuestion.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.EnterQuestion.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.EnterQuestion.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(171)))), ((int)(((byte)(160)))));
            this.EnterQuestion.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.EnterQuestion.FillSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.EnterQuestion.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.EnterQuestion.Location = new System.Drawing.Point(562, 119);
            this.EnterQuestion.MinimumSize = new System.Drawing.Size(1, 1);
            this.EnterQuestion.Name = "EnterQuestion";
            this.EnterQuestion.Radius = 10;
            this.EnterQuestion.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.EnterQuestion.RectHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(171)))), ((int)(((byte)(160)))));
            this.EnterQuestion.RectPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.EnterQuestion.RectSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.EnterQuestion.Size = new System.Drawing.Size(176, 46);
            this.EnterQuestion.Style = Sunny.UI.UIStyle.LayuiGreen;
            this.EnterQuestion.Symbol = 61736;
            this.EnterQuestion.TabIndex = 10;
            this.EnterQuestion.Text = "常见问题";
            this.EnterQuestion.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.EnterQuestion.Click += new System.EventHandler(this.EnterQuestion_Click);
            // 
            // EnterLLCG
            // 
            this.EnterLLCG.Cursor = System.Windows.Forms.Cursors.Hand;
            this.EnterLLCG.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.EnterLLCG.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.EnterLLCG.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(171)))), ((int)(((byte)(160)))));
            this.EnterLLCG.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.EnterLLCG.FillSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.EnterLLCG.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.EnterLLCG.Location = new System.Drawing.Point(832, 119);
            this.EnterLLCG.MinimumSize = new System.Drawing.Size(1, 1);
            this.EnterLLCG.Name = "EnterLLCG";
            this.EnterLLCG.Radius = 10;
            this.EnterLLCG.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.EnterLLCG.RectHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(171)))), ((int)(((byte)(160)))));
            this.EnterLLCG.RectPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.EnterLLCG.RectSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.EnterLLCG.Size = new System.Drawing.Size(176, 46);
            this.EnterLLCG.Style = Sunny.UI.UIStyle.LayuiGreen;
            this.EnterLLCG.Symbol = 61910;
            this.EnterLLCG.TabIndex = 9;
            this.EnterLLCG.Text = "LLCG";
            this.EnterLLCG.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.EnterLLCG.Click += new System.EventHandler(this.EnterLLCG_Click);
            // 
            // EnterParatranz
            // 
            this.EnterParatranz.Cursor = System.Windows.Forms.Cursors.Hand;
            this.EnterParatranz.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.EnterParatranz.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.EnterParatranz.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(171)))), ((int)(((byte)(160)))));
            this.EnterParatranz.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.EnterParatranz.FillSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.EnterParatranz.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.EnterParatranz.Location = new System.Drawing.Point(29, 211);
            this.EnterParatranz.MinimumSize = new System.Drawing.Size(1, 1);
            this.EnterParatranz.Name = "EnterParatranz";
            this.EnterParatranz.Radius = 10;
            this.EnterParatranz.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.EnterParatranz.RectHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(171)))), ((int)(((byte)(160)))));
            this.EnterParatranz.RectPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.EnterParatranz.RectSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.EnterParatranz.Size = new System.Drawing.Size(176, 46);
            this.EnterParatranz.Style = Sunny.UI.UIStyle.LayuiGreen;
            this.EnterParatranz.Symbol = 300080;
            this.EnterParatranz.TabIndex = 8;
            this.EnterParatranz.Text = "Paratranz";
            this.EnterParatranz.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.EnterParatranz.Click += new System.EventHandler(this.EnterParatranz_Click);
            // 
            // EnterBilibili
            // 
            this.EnterBilibili.Cursor = System.Windows.Forms.Cursors.Hand;
            this.EnterBilibili.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.EnterBilibili.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.EnterBilibili.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(171)))), ((int)(((byte)(160)))));
            this.EnterBilibili.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.EnterBilibili.FillSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.EnterBilibili.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.EnterBilibili.Location = new System.Drawing.Point(832, 28);
            this.EnterBilibili.MinimumSize = new System.Drawing.Size(1, 1);
            this.EnterBilibili.Name = "EnterBilibili";
            this.EnterBilibili.Radius = 10;
            this.EnterBilibili.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.EnterBilibili.RectHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(171)))), ((int)(((byte)(160)))));
            this.EnterBilibili.RectPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.EnterBilibili.RectSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.EnterBilibili.Size = new System.Drawing.Size(176, 46);
            this.EnterBilibili.Style = Sunny.UI.UIStyle.LayuiGreen;
            this.EnterBilibili.Symbol = 158329;
            this.EnterBilibili.TabIndex = 5;
            this.EnterBilibili.Text = "我们的BiliBili";
            this.EnterBilibili.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.EnterBilibili.Click += new System.EventHandler(this.EnterBilibili_Click);
            // 
            // EnterWebsite
            // 
            this.EnterWebsite.Cursor = System.Windows.Forms.Cursors.Hand;
            this.EnterWebsite.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.EnterWebsite.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.EnterWebsite.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(171)))), ((int)(((byte)(160)))));
            this.EnterWebsite.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.EnterWebsite.FillSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.EnterWebsite.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.EnterWebsite.Location = new System.Drawing.Point(562, 28);
            this.EnterWebsite.MinimumSize = new System.Drawing.Size(1, 1);
            this.EnterWebsite.Name = "EnterWebsite";
            this.EnterWebsite.Radius = 10;
            this.EnterWebsite.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.EnterWebsite.RectHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(171)))), ((int)(((byte)(160)))));
            this.EnterWebsite.RectPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.EnterWebsite.RectSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.EnterWebsite.Size = new System.Drawing.Size(176, 46);
            this.EnterWebsite.Style = Sunny.UI.UIStyle.LayuiGreen;
            this.EnterWebsite.Symbol = 362844;
            this.EnterWebsite.TabIndex = 4;
            this.EnterWebsite.Text = "官网";
            this.EnterWebsite.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.EnterWebsite.Click += new System.EventHandler(this.EnterWebsite_Click);
            // 
            // EnterSteampp
            // 
            this.EnterSteampp.Cursor = System.Windows.Forms.Cursors.Hand;
            this.EnterSteampp.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.EnterSteampp.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.EnterSteampp.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(171)))), ((int)(((byte)(160)))));
            this.EnterSteampp.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.EnterSteampp.FillSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.EnterSteampp.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.EnterSteampp.Location = new System.Drawing.Point(290, 119);
            this.EnterSteampp.MinimumSize = new System.Drawing.Size(1, 1);
            this.EnterSteampp.Name = "EnterSteampp";
            this.EnterSteampp.Radius = 10;
            this.EnterSteampp.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.EnterSteampp.RectHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(171)))), ((int)(((byte)(160)))));
            this.EnterSteampp.RectPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.EnterSteampp.RectSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.EnterSteampp.Size = new System.Drawing.Size(176, 46);
            this.EnterSteampp.Style = Sunny.UI.UIStyle.LayuiGreen;
            this.EnterSteampp.Symbol = 357527;
            this.EnterSteampp.TabIndex = 3;
            this.EnterSteampp.Text = "Watt Toolkit";
            this.EnterSteampp.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.EnterSteampp.Click += new System.EventHandler(this.EnterSteampp_Click);
            // 
            // EnterLLCGithub
            // 
            this.EnterLLCGithub.Cursor = System.Windows.Forms.Cursors.Hand;
            this.EnterLLCGithub.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.EnterLLCGithub.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.EnterLLCGithub.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(171)))), ((int)(((byte)(160)))));
            this.EnterLLCGithub.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.EnterLLCGithub.FillSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.EnterLLCGithub.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.EnterLLCGithub.Location = new System.Drawing.Point(290, 28);
            this.EnterLLCGithub.MinimumSize = new System.Drawing.Size(1, 1);
            this.EnterLLCGithub.Name = "EnterLLCGithub";
            this.EnterLLCGithub.Radius = 10;
            this.EnterLLCGithub.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.EnterLLCGithub.RectHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(171)))), ((int)(((byte)(160)))));
            this.EnterLLCGithub.RectPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.EnterLLCGithub.RectSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.EnterLLCGithub.Size = new System.Drawing.Size(176, 46);
            this.EnterLLCGithub.Style = Sunny.UI.UIStyle.LayuiGreen;
            this.EnterLLCGithub.Symbol = 161595;
            this.EnterLLCGithub.TabIndex = 2;
            this.EnterLLCGithub.Text = "汉化补丁 Github";
            this.EnterLLCGithub.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.EnterLLCGithub.Click += new System.EventHandler(this.EnterLLCGithub_Click);
            // 
            // EnterWiki
            // 
            this.EnterWiki.Cursor = System.Windows.Forms.Cursors.Hand;
            this.EnterWiki.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.EnterWiki.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.EnterWiki.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(171)))), ((int)(((byte)(160)))));
            this.EnterWiki.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.EnterWiki.FillSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.EnterWiki.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.EnterWiki.Location = new System.Drawing.Point(29, 119);
            this.EnterWiki.MinimumSize = new System.Drawing.Size(1, 1);
            this.EnterWiki.Name = "EnterWiki";
            this.EnterWiki.Radius = 10;
            this.EnterWiki.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.EnterWiki.RectHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(171)))), ((int)(((byte)(160)))));
            this.EnterWiki.RectPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.EnterWiki.RectSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.EnterWiki.Size = new System.Drawing.Size(176, 46);
            this.EnterWiki.Style = Sunny.UI.UIStyle.LayuiGreen;
            this.EnterWiki.Symbol = 361912;
            this.EnterWiki.TabIndex = 1;
            this.EnterWiki.Text = "边狱公司 灰机Wiki";
            this.EnterWiki.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.EnterWiki.Click += new System.EventHandler(this.EnterWiki_Click);
            // 
            // EnterToolBoxGithub
            // 
            this.EnterToolBoxGithub.Cursor = System.Windows.Forms.Cursors.Hand;
            this.EnterToolBoxGithub.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.EnterToolBoxGithub.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.EnterToolBoxGithub.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(171)))), ((int)(((byte)(160)))));
            this.EnterToolBoxGithub.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.EnterToolBoxGithub.FillSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.EnterToolBoxGithub.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.EnterToolBoxGithub.Location = new System.Drawing.Point(29, 28);
            this.EnterToolBoxGithub.MinimumSize = new System.Drawing.Size(1, 1);
            this.EnterToolBoxGithub.Name = "EnterToolBoxGithub";
            this.EnterToolBoxGithub.Radius = 10;
            this.EnterToolBoxGithub.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.EnterToolBoxGithub.RectHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(171)))), ((int)(((byte)(160)))));
            this.EnterToolBoxGithub.RectPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.EnterToolBoxGithub.RectSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.EnterToolBoxGithub.Size = new System.Drawing.Size(176, 46);
            this.EnterToolBoxGithub.Style = Sunny.UI.UIStyle.LayuiGreen;
            this.EnterToolBoxGithub.Symbol = 161595;
            this.EnterToolBoxGithub.TabIndex = 0;
            this.EnterToolBoxGithub.Text = "工具箱 Github";
            this.EnterToolBoxGithub.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.EnterToolBoxGithub.Click += new System.EventHandler(this.EnterToolBoxGithub_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(248)))), ((int)(((byte)(248)))));
            this.tabPage3.Controls.Add(this.uiGroupBox2);
            this.tabPage3.Controls.Add(this.uiGroupBox1);
            this.tabPage3.Location = new System.Drawing.Point(0, 40);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(1030, 408);
            this.tabPage3.TabIndex = 1;
            this.tabPage3.Text = "设置";
            // 
            // uiGroupBox2
            // 
            this.uiGroupBox2.Controls.Add(this.deleteButton);
            this.uiGroupBox2.Controls.Add(this.uiLabel7);
            this.uiGroupBox2.Controls.Add(this.useGithub);
            this.uiGroupBox2.Controls.Add(this.uiLabel2);
            this.uiGroupBox2.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(248)))), ((int)(((byte)(248)))));
            this.uiGroupBox2.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(248)))), ((int)(((byte)(248)))));
            this.uiGroupBox2.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiGroupBox2.Location = new System.Drawing.Point(521, 17);
            this.uiGroupBox2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiGroupBox2.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiGroupBox2.Name = "uiGroupBox2";
            this.uiGroupBox2.Padding = new System.Windows.Forms.Padding(0, 32, 0, 0);
            this.uiGroupBox2.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.uiGroupBox2.Size = new System.Drawing.Size(490, 374);
            this.uiGroupBox2.Style = Sunny.UI.UIStyle.LayuiGreen;
            this.uiGroupBox2.TabIndex = 3;
            this.uiGroupBox2.Text = "设置";
            this.uiGroupBox2.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // deleteButton
            // 
            this.deleteButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.deleteButton.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.deleteButton.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.deleteButton.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(171)))), ((int)(((byte)(160)))));
            this.deleteButton.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.deleteButton.FillSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.deleteButton.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.deleteButton.Location = new System.Drawing.Point(371, 104);
            this.deleteButton.MinimumSize = new System.Drawing.Size(1, 1);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.deleteButton.RectHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(171)))), ((int)(((byte)(160)))));
            this.deleteButton.RectPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.deleteButton.RectSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.deleteButton.Size = new System.Drawing.Size(100, 35);
            this.deleteButton.Style = Sunny.UI.UIStyle.LayuiGreen;
            this.deleteButton.TabIndex = 5;
            this.deleteButton.Text = "删除";
            this.deleteButton.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // uiLabel7
            // 
            this.uiLabel7.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.uiLabel7.Location = new System.Drawing.Point(17, 110);
            this.uiLabel7.Name = "uiLabel7";
            this.uiLabel7.Size = new System.Drawing.Size(357, 23);
            this.uiLabel7.Style = Sunny.UI.UIStyle.LayuiGreen;
            this.uiLabel7.TabIndex = 4;
            this.uiLabel7.Text = "删除汉化补丁";
            this.uiLabel7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // useGithub
            // 
            this.useGithub.ActiveColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.useGithub.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.useGithub.Location = new System.Drawing.Point(380, 58);
            this.useGithub.MinimumSize = new System.Drawing.Size(1, 1);
            this.useGithub.Name = "useGithub";
            this.useGithub.Size = new System.Drawing.Size(78, 29);
            this.useGithub.Style = Sunny.UI.UIStyle.LayuiGreen;
            this.useGithub.SwitchShape = Sunny.UI.UISwitch.UISwitchShape.Square;
            this.useGithub.TabIndex = 3;
            this.useGithub.Text = "uiSwitch1";
            // 
            // uiLabel2
            // 
            this.uiLabel2.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.uiLabel2.Location = new System.Drawing.Point(17, 58);
            this.uiLabel2.Name = "uiLabel2";
            this.uiLabel2.Size = new System.Drawing.Size(357, 23);
            this.uiLabel2.Style = Sunny.UI.UIStyle.LayuiGreen;
            this.uiLabel2.TabIndex = 2;
            this.uiLabel2.Text = "从 Github 下载文件";
            this.uiLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // uiGroupBox1
            // 
            this.uiGroupBox1.Controls.Add(this.dlFromDefault);
            this.uiGroupBox1.Controls.Add(this.uiLabel6);
            this.uiGroupBox1.Controls.Add(this.dlFromLVCDN);
            this.uiGroupBox1.Controls.Add(this.uiLabel5);
            this.uiGroupBox1.Controls.Add(this.dlFromLV);
            this.uiGroupBox1.Controls.Add(this.uiLabel4);
            this.uiGroupBox1.Controls.Add(this.dlFromOFB);
            this.uiGroupBox1.Controls.Add(this.uiLabel3);
            this.uiGroupBox1.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(248)))), ((int)(((byte)(248)))));
            this.uiGroupBox1.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(248)))), ((int)(((byte)(248)))));
            this.uiGroupBox1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiGroupBox1.Location = new System.Drawing.Point(19, 17);
            this.uiGroupBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiGroupBox1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiGroupBox1.Name = "uiGroupBox1";
            this.uiGroupBox1.Padding = new System.Windows.Forms.Padding(0, 32, 0, 0);
            this.uiGroupBox1.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.uiGroupBox1.Size = new System.Drawing.Size(469, 374);
            this.uiGroupBox1.Style = Sunny.UI.UIStyle.LayuiGreen;
            this.uiGroupBox1.TabIndex = 2;
            this.uiGroupBox1.Text = "手动选择节点";
            this.uiGroupBox1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // dlFromDefault
            // 
            this.dlFromDefault.Cursor = System.Windows.Forms.Cursors.Hand;
            this.dlFromDefault.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.dlFromDefault.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.dlFromDefault.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(171)))), ((int)(((byte)(160)))));
            this.dlFromDefault.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.dlFromDefault.FillSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.dlFromDefault.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dlFromDefault.Location = new System.Drawing.Point(344, 324);
            this.dlFromDefault.MinimumSize = new System.Drawing.Size(1, 1);
            this.dlFromDefault.Name = "dlFromDefault";
            this.dlFromDefault.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.dlFromDefault.RectHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(171)))), ((int)(((byte)(160)))));
            this.dlFromDefault.RectPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.dlFromDefault.RectSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.dlFromDefault.Size = new System.Drawing.Size(100, 35);
            this.dlFromDefault.Style = Sunny.UI.UIStyle.LayuiGreen;
            this.dlFromDefault.TabIndex = 11;
            this.dlFromDefault.Text = "选择";
            this.dlFromDefault.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dlFromDefault.Click += new System.EventHandler(this.dlFromDefault_Click);
            // 
            // uiLabel6
            // 
            this.uiLabel6.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.uiLabel6.Location = new System.Drawing.Point(23, 329);
            this.uiLabel6.Name = "uiLabel6";
            this.uiLabel6.Size = new System.Drawing.Size(357, 23);
            this.uiLabel6.Style = Sunny.UI.UIStyle.LayuiGreen;
            this.uiLabel6.TabIndex = 10;
            this.uiLabel6.Text = "恢复默认值";
            this.uiLabel6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dlFromLVCDN
            // 
            this.dlFromLVCDN.Cursor = System.Windows.Forms.Cursors.Hand;
            this.dlFromLVCDN.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.dlFromLVCDN.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.dlFromLVCDN.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(171)))), ((int)(((byte)(160)))));
            this.dlFromLVCDN.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.dlFromLVCDN.FillSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.dlFromLVCDN.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dlFromLVCDN.Location = new System.Drawing.Point(344, 161);
            this.dlFromLVCDN.MinimumSize = new System.Drawing.Size(1, 1);
            this.dlFromLVCDN.Name = "dlFromLVCDN";
            this.dlFromLVCDN.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.dlFromLVCDN.RectHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(171)))), ((int)(((byte)(160)))));
            this.dlFromLVCDN.RectPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.dlFromLVCDN.RectSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.dlFromLVCDN.Size = new System.Drawing.Size(100, 35);
            this.dlFromLVCDN.Style = Sunny.UI.UIStyle.LayuiGreen;
            this.dlFromLVCDN.TabIndex = 9;
            this.dlFromLVCDN.Text = "选择";
            this.dlFromLVCDN.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dlFromLVCDN.Click += new System.EventHandler(this.dlFromLVCDN_Click);
            // 
            // uiLabel5
            // 
            this.uiLabel5.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.uiLabel5.Location = new System.Drawing.Point(23, 165);
            this.uiLabel5.Name = "uiLabel5";
            this.uiLabel5.Size = new System.Drawing.Size(357, 23);
            this.uiLabel5.Style = Sunny.UI.UIStyle.LayuiGreen;
            this.uiLabel5.TabIndex = 8;
            this.uiLabel5.Text = "从 拉斯维加斯服务器 with CDN 下载";
            this.uiLabel5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dlFromLV
            // 
            this.dlFromLV.Cursor = System.Windows.Forms.Cursors.Hand;
            this.dlFromLV.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.dlFromLV.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.dlFromLV.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(171)))), ((int)(((byte)(160)))));
            this.dlFromLV.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.dlFromLV.FillSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.dlFromLV.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dlFromLV.Location = new System.Drawing.Point(344, 104);
            this.dlFromLV.MinimumSize = new System.Drawing.Size(1, 1);
            this.dlFromLV.Name = "dlFromLV";
            this.dlFromLV.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.dlFromLV.RectHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(171)))), ((int)(((byte)(160)))));
            this.dlFromLV.RectPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.dlFromLV.RectSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.dlFromLV.Size = new System.Drawing.Size(100, 35);
            this.dlFromLV.Style = Sunny.UI.UIStyle.LayuiGreen;
            this.dlFromLV.TabIndex = 7;
            this.dlFromLV.Text = "选择";
            this.dlFromLV.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dlFromLV.Click += new System.EventHandler(this.dlFromLV_Click);
            // 
            // uiLabel4
            // 
            this.uiLabel4.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.uiLabel4.Location = new System.Drawing.Point(23, 110);
            this.uiLabel4.Name = "uiLabel4";
            this.uiLabel4.Size = new System.Drawing.Size(357, 23);
            this.uiLabel4.Style = Sunny.UI.UIStyle.LayuiGreen;
            this.uiLabel4.TabIndex = 6;
            this.uiLabel4.Text = "从 拉斯维加斯服务器 下载";
            this.uiLabel4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dlFromOFB
            // 
            this.dlFromOFB.Cursor = System.Windows.Forms.Cursors.Hand;
            this.dlFromOFB.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.dlFromOFB.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.dlFromOFB.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(171)))), ((int)(((byte)(160)))));
            this.dlFromOFB.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.dlFromOFB.FillSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.dlFromOFB.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dlFromOFB.Location = new System.Drawing.Point(344, 46);
            this.dlFromOFB.MinimumSize = new System.Drawing.Size(1, 1);
            this.dlFromOFB.Name = "dlFromOFB";
            this.dlFromOFB.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.dlFromOFB.RectHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(171)))), ((int)(((byte)(160)))));
            this.dlFromOFB.RectPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.dlFromOFB.RectSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(109)))));
            this.dlFromOFB.Size = new System.Drawing.Size(100, 35);
            this.dlFromOFB.Style = Sunny.UI.UIStyle.LayuiGreen;
            this.dlFromOFB.TabIndex = 5;
            this.dlFromOFB.Text = "选择";
            this.dlFromOFB.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dlFromOFB.Click += new System.EventHandler(this.dlFromOFB_Click);
            // 
            // uiLabel3
            // 
            this.uiLabel3.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.uiLabel3.Location = new System.Drawing.Point(23, 52);
            this.uiLabel3.Name = "uiLabel3";
            this.uiLabel3.Size = new System.Drawing.Size(357, 23);
            this.uiLabel3.Style = Sunny.UI.UIStyle.LayuiGreen;
            this.uiLabel3.TabIndex = 4;
            this.uiLabel3.Text = "从 Onedrive For Business 下载";
            this.uiLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tabPage4
            // 
            this.tabPage4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(248)))), ((int)(((byte)(248)))));
            this.tabPage4.Controls.Add(this.uiLabel9);
            this.tabPage4.Controls.Add(this.uiLabel8);
            this.tabPage4.Location = new System.Drawing.Point(0, 40);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(200, 60);
            this.tabPage4.TabIndex = 1;
            this.tabPage4.Text = "关于";
            // 
            // uiLabel9
            // 
            this.uiLabel9.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLabel9.Location = new System.Drawing.Point(18, 16);
            this.uiLabel9.Name = "uiLabel9";
            this.uiLabel9.Size = new System.Drawing.Size(502, 36);
            this.uiLabel9.Style = Sunny.UI.UIStyle.LayuiGreen;
            this.uiLabel9.TabIndex = 1;
            this.uiLabel9.Text = "LLC_Toolbox 版本0.4.1";
            this.uiLabel9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // uiLabel8
            // 
            this.uiLabel8.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLabel8.Location = new System.Drawing.Point(18, 52);
            this.uiLabel8.Name = "uiLabel8";
            this.uiLabel8.Size = new System.Drawing.Size(996, 278);
            this.uiLabel8.Style = Sunny.UI.UIStyle.LayuiGreen;
            this.uiLabel8.TabIndex = 0;
            this.uiLabel8.Text = resources.GetString("uiLabel8.Text");
            this.uiLabel8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // MainPage
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1036, 494);
            this.ControlBoxFillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(171)))), ((int)(((byte)(160)))));
            this.Controls.Add(this.uiTabControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainTabControl = this.uiTabControl;
            this.MaximizeBox = false;
            this.Name = "MainPage";
            this.Padding = new System.Windows.Forms.Padding(0, 40, 0, 0);
            this.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.ShowRadius = false;
            this.ShowRect = false;
            this.ShowTitleIcon = true;
            this.Style = Sunny.UI.UIStyle.LayuiGreen;
            this.Text = "零协会工具箱 v" + MainPage.VERSION;
            this.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.TitleHeight = 40;
            this.ZoomScaleRect = new System.Drawing.Rectangle(15, 15, 800, 450);
            this.Load += new System.EventHandler(this.MainPage_Load);
            this.uiTabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.uiGroupBox2.ResumeLayout(false);
            this.uiGroupBox1.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Sunny.UI.UITabControl uiTabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private Sunny.UI.UIProcessBar DownloadBar;
        private Sunny.UI.UIProcessBar TotalBar;
        private Sunny.UI.UISymbolButton installButton;
        private Sunny.UI.UILabel statu;
        private Sunny.UI.UIGroupBox uiGroupBox2;
        private Sunny.UI.UISwitch useGithub;
        private Sunny.UI.UILabel uiLabel2;
        private Sunny.UI.UIGroupBox uiGroupBox1;
        private Sunny.UI.UIButton dlFromDefault;
        private Sunny.UI.UILabel uiLabel6;
        private Sunny.UI.UIButton dlFromLVCDN;
        private Sunny.UI.UILabel uiLabel5;
        private Sunny.UI.UIButton dlFromLV;
        private Sunny.UI.UILabel uiLabel4;
        private Sunny.UI.UIButton dlFromOFB;
        private Sunny.UI.UILabel uiLabel3;
        private Sunny.UI.UIButton deleteButton;
        private Sunny.UI.UILabel uiLabel7;
        private Sunny.UI.UISymbolButton EnterQuestion;
        private Sunny.UI.UISymbolButton EnterLLCG;
        private Sunny.UI.UISymbolButton EnterParatranz;
        private Sunny.UI.UISymbolButton EnterBilibili;
        private Sunny.UI.UISymbolButton EnterWebsite;
        private Sunny.UI.UISymbolButton EnterSteampp;
        private Sunny.UI.UISymbolButton EnterLLCGithub;
        private Sunny.UI.UISymbolButton EnterWiki;
        private Sunny.UI.UISymbolButton EnterToolBoxGithub;
        private Sunny.UI.UISymbolButton EnterAfdian;
        private Sunny.UI.UILabel uiLabel8;
        private Sunny.UI.UILabel uiLabel9;
    }
}

