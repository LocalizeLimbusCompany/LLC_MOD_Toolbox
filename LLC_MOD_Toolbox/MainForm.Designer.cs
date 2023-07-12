namespace LLC_MOD_Toolbox
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.note = new Sunny.UI.UILabel();
            this.uiGroupBox_install = new Sunny.UI.UIGroupBox();
            this.installButton = new Sunny.UI.UISymbolButton();
            this.down_Bar = new Sunny.UI.UIProcessBar();
            this.done_Bar = new Sunny.UI.UIProcessBar();
            this.noteLabel = new Sunny.UI.UILabel();
            this.note_2 = new Sunny.UI.UILabel();
            this.status = new Sunny.UI.UILabel();
            this.uiGroupBox1 = new Sunny.UI.UIGroupBox();
            this.enterDoc = new Sunny.UI.UISymbolButton();
            this.uiLabel3 = new Sunny.UI.UILabel();
            this.useGithub = new Sunny.UI.UISwitch();
            this.uiLabel2 = new Sunny.UI.UILabel();
            this.enterWiki = new Sunny.UI.UISymbolButton();
            this.linkline = new Sunny.UI.UILine();
            this.enterGithub = new Sunny.UI.UISymbolButton();
            this.enterAfdian = new Sunny.UI.UISymbolButton();
            this.note_3 = new Sunny.UI.UILabel();
            this.uiLabel1 = new Sunny.UI.UILabel();
            this.useMFL = new Sunny.UI.UISwitch();
            this.uiLine1 = new Sunny.UI.UILine();
            this.dlFromOFB = new Sunny.UI.UIButton();
            this.dlFromLV = new Sunny.UI.UIButton();
            this.dlFromDefault = new Sunny.UI.UIButton();
            this.dlFromLVCDN = new Sunny.UI.UIButton();
            this.uiGroupBox_install.SuspendLayout();
            this.uiGroupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // note
            // 
            this.note.Font = new System.Drawing.Font("微软雅黑", 18F);
            this.note.Location = new System.Drawing.Point(16, 56);
            this.note.Name = "note";
            this.note.Size = new System.Drawing.Size(263, 39);
            this.note.Style = Sunny.UI.UIStyle.Orange;
            this.note.TabIndex = 0;
            this.note.Text = "零协会汉化补丁 工具箱";
            this.note.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // uiGroupBox_install
            // 
            this.uiGroupBox_install.Controls.Add(this.installButton);
            this.uiGroupBox_install.Controls.Add(this.down_Bar);
            this.uiGroupBox_install.Controls.Add(this.done_Bar);
            this.uiGroupBox_install.Controls.Add(this.noteLabel);
            this.uiGroupBox_install.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(249)))), ((int)(((byte)(241)))));
            this.uiGroupBox_install.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(249)))), ((int)(((byte)(241)))));
            this.uiGroupBox_install.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiGroupBox_install.Location = new System.Drawing.Point(22, 109);
            this.uiGroupBox_install.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiGroupBox_install.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiGroupBox_install.Name = "uiGroupBox_install";
            this.uiGroupBox_install.Padding = new System.Windows.Forms.Padding(0, 32, 0, 0);
            this.uiGroupBox_install.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.uiGroupBox_install.Size = new System.Drawing.Size(613, 454);
            this.uiGroupBox_install.Style = Sunny.UI.UIStyle.Orange;
            this.uiGroupBox_install.TabIndex = 1;
            this.uiGroupBox_install.Text = "一键安装";
            this.uiGroupBox_install.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // installButton
            // 
            this.installButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.installButton.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.installButton.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.installButton.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(175)))), ((int)(((byte)(83)))));
            this.installButton.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.installButton.FillSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.installButton.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.installButton.Location = new System.Drawing.Point(232, 187);
            this.installButton.MinimumSize = new System.Drawing.Size(1, 1);
            this.installButton.Name = "installButton";
            this.installButton.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.installButton.RectHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(175)))), ((int)(((byte)(83)))));
            this.installButton.RectPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.installButton.RectSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.installButton.Size = new System.Drawing.Size(122, 55);
            this.installButton.Style = Sunny.UI.UIStyle.Orange;
            this.installButton.Symbol = 361613;
            this.installButton.TabIndex = 5;
            this.installButton.Text = "一键安装";
            this.installButton.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.installButton.Click += new System.EventHandler(this.installButton_Click);
            // 
            // down_Bar
            // 
            this.down_Bar.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(249)))), ((int)(((byte)(241)))));
            this.down_Bar.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.down_Bar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.down_Bar.Location = new System.Drawing.Point(142, 402);
            this.down_Bar.MinimumSize = new System.Drawing.Size(70, 3);
            this.down_Bar.Name = "down_Bar";
            this.down_Bar.RadiusSides = ((Sunny.UI.UICornerRadiusSides)((Sunny.UI.UICornerRadiusSides.RightBottom | Sunny.UI.UICornerRadiusSides.LeftBottom)));
            this.down_Bar.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.down_Bar.Size = new System.Drawing.Size(300, 16);
            this.down_Bar.Style = Sunny.UI.UIStyle.Orange;
            this.down_Bar.TabIndex = 3;
            this.down_Bar.Text = "uiProcessBar1";
            // 
            // done_Bar
            // 
            this.done_Bar.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(249)))), ((int)(((byte)(241)))));
            this.done_Bar.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.done_Bar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.done_Bar.Location = new System.Drawing.Point(142, 377);
            this.done_Bar.MinimumSize = new System.Drawing.Size(70, 3);
            this.done_Bar.Name = "done_Bar";
            this.done_Bar.RadiusSides = ((Sunny.UI.UICornerRadiusSides)((Sunny.UI.UICornerRadiusSides.LeftTop | Sunny.UI.UICornerRadiusSides.RightTop)));
            this.done_Bar.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.done_Bar.Size = new System.Drawing.Size(300, 29);
            this.done_Bar.Style = Sunny.UI.UIStyle.Orange;
            this.done_Bar.TabIndex = 2;
            this.done_Bar.Text = "down_Bar";
            // 
            // noteLabel
            // 
            this.noteLabel.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.noteLabel.Location = new System.Drawing.Point(177, 56);
            this.noteLabel.Name = "noteLabel";
            this.noteLabel.Size = new System.Drawing.Size(241, 23);
            this.noteLabel.Style = Sunny.UI.UIStyle.Orange;
            this.noteLabel.TabIndex = 0;
            this.noteLabel.Text = "点击一键安装，立刻开始！";
            this.noteLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // note_2
            // 
            this.note_2.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.note_2.Location = new System.Drawing.Point(340, 68);
            this.note_2.Name = "note_2";
            this.note_2.Size = new System.Drawing.Size(100, 23);
            this.note_2.Style = Sunny.UI.UIStyle.Orange;
            this.note_2.TabIndex = 2;
            this.note_2.Text = "当前状态：";
            this.note_2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // status
            // 
            this.status.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.status.Location = new System.Drawing.Point(426, 68);
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(444, 23);
            this.status.Style = Sunny.UI.UIStyle.Orange;
            this.status.TabIndex = 3;
            this.status.Text = "空闲中！";
            this.status.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // uiGroupBox1
            // 
            this.uiGroupBox1.Controls.Add(this.dlFromLVCDN);
            this.uiGroupBox1.Controls.Add(this.dlFromDefault);
            this.uiGroupBox1.Controls.Add(this.dlFromLV);
            this.uiGroupBox1.Controls.Add(this.dlFromOFB);
            this.uiGroupBox1.Controls.Add(this.uiLine1);
            this.uiGroupBox1.Controls.Add(this.enterDoc);
            this.uiGroupBox1.Controls.Add(this.uiLabel3);
            this.uiGroupBox1.Controls.Add(this.useGithub);
            this.uiGroupBox1.Controls.Add(this.uiLabel2);
            this.uiGroupBox1.Controls.Add(this.enterWiki);
            this.uiGroupBox1.Controls.Add(this.linkline);
            this.uiGroupBox1.Controls.Add(this.enterGithub);
            this.uiGroupBox1.Controls.Add(this.enterAfdian);
            this.uiGroupBox1.Controls.Add(this.note_3);
            this.uiGroupBox1.Controls.Add(this.uiLabel1);
            this.uiGroupBox1.Controls.Add(this.useMFL);
            this.uiGroupBox1.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(249)))), ((int)(((byte)(241)))));
            this.uiGroupBox1.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(249)))), ((int)(((byte)(241)))));
            this.uiGroupBox1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiGroupBox1.Location = new System.Drawing.Point(678, 109);
            this.uiGroupBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiGroupBox1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiGroupBox1.Name = "uiGroupBox1";
            this.uiGroupBox1.Padding = new System.Windows.Forms.Padding(0, 32, 0, 0);
            this.uiGroupBox1.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.uiGroupBox1.Size = new System.Drawing.Size(523, 454);
            this.uiGroupBox1.Style = Sunny.UI.UIStyle.Orange;
            this.uiGroupBox1.TabIndex = 4;
            this.uiGroupBox1.Text = "其他";
            this.uiGroupBox1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // enterDoc
            // 
            this.enterDoc.Cursor = System.Windows.Forms.Cursors.Hand;
            this.enterDoc.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.enterDoc.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.enterDoc.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(175)))), ((int)(((byte)(83)))));
            this.enterDoc.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.enterDoc.FillSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.enterDoc.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.enterDoc.Location = new System.Drawing.Point(22, 405);
            this.enterDoc.MinimumSize = new System.Drawing.Size(1, 1);
            this.enterDoc.Name = "enterDoc";
            this.enterDoc.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.enterDoc.RectHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(175)))), ((int)(((byte)(83)))));
            this.enterDoc.RectPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.enterDoc.RectSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.enterDoc.Size = new System.Drawing.Size(100, 35);
            this.enterDoc.Style = Sunny.UI.UIStyle.Orange;
            this.enterDoc.Symbol = 61897;
            this.enterDoc.TabIndex = 12;
            this.enterDoc.Text = "文档站";
            this.enterDoc.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.enterDoc.Click += new System.EventHandler(this.enterDoc_Click);
            // 
            // uiLabel3
            // 
            this.uiLabel3.Font = new System.Drawing.Font("微软雅黑", 8F);
            this.uiLabel3.Location = new System.Drawing.Point(19, 166);
            this.uiLabel3.Name = "uiLabel3";
            this.uiLabel3.Size = new System.Drawing.Size(335, 30);
            this.uiLabel3.Style = Sunny.UI.UIStyle.Orange;
            this.uiLabel3.TabIndex = 11;
            this.uiLabel3.Text = "除非你知道你在做什么，否则不要更改。";
            this.uiLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // useGithub
            // 
            this.useGithub.ActiveColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.useGithub.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.useGithub.Location = new System.Drawing.Point(415, 150);
            this.useGithub.MinimumSize = new System.Drawing.Size(1, 1);
            this.useGithub.Name = "useGithub";
            this.useGithub.Size = new System.Drawing.Size(75, 29);
            this.useGithub.Style = Sunny.UI.UIStyle.Orange;
            this.useGithub.TabIndex = 10;
            this.useGithub.Text = "uiSwitch1";
            // 
            // uiLabel2
            // 
            this.uiLabel2.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLabel2.Location = new System.Drawing.Point(18, 143);
            this.uiLabel2.Name = "uiLabel2";
            this.uiLabel2.Size = new System.Drawing.Size(216, 23);
            this.uiLabel2.Style = Sunny.UI.UIStyle.Orange;
            this.uiLabel2.TabIndex = 9;
            this.uiLabel2.Text = "从Github下载文件";
            this.uiLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // enterWiki
            // 
            this.enterWiki.Cursor = System.Windows.Forms.Cursors.Hand;
            this.enterWiki.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.enterWiki.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.enterWiki.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(175)))), ((int)(((byte)(83)))));
            this.enterWiki.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.enterWiki.FillSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.enterWiki.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.enterWiki.Location = new System.Drawing.Point(379, 359);
            this.enterWiki.MinimumSize = new System.Drawing.Size(1, 1);
            this.enterWiki.Name = "enterWiki";
            this.enterWiki.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.enterWiki.RectHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(175)))), ((int)(((byte)(83)))));
            this.enterWiki.RectPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.enterWiki.RectSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.enterWiki.Size = new System.Drawing.Size(111, 35);
            this.enterWiki.Style = Sunny.UI.UIStyle.Orange;
            this.enterWiki.Symbol = 61912;
            this.enterWiki.TabIndex = 8;
            this.enterWiki.Text = "灰机维基";
            this.enterWiki.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.enterWiki.Click += new System.EventHandler(this.enterWiki_Click);
            // 
            // linkline
            // 
            this.linkline.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(249)))), ((int)(((byte)(241)))));
            this.linkline.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.linkline.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.linkline.LineColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(249)))), ((int)(((byte)(241)))));
            this.linkline.Location = new System.Drawing.Point(22, 324);
            this.linkline.MinimumSize = new System.Drawing.Size(1, 1);
            this.linkline.Name = "linkline";
            this.linkline.Size = new System.Drawing.Size(468, 29);
            this.linkline.Style = Sunny.UI.UIStyle.Orange;
            this.linkline.TabIndex = 7;
            this.linkline.Text = "链接";
            // 
            // enterGithub
            // 
            this.enterGithub.Cursor = System.Windows.Forms.Cursors.Hand;
            this.enterGithub.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.enterGithub.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.enterGithub.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(175)))), ((int)(((byte)(83)))));
            this.enterGithub.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.enterGithub.FillSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.enterGithub.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.enterGithub.Location = new System.Drawing.Point(203, 359);
            this.enterGithub.MinimumSize = new System.Drawing.Size(1, 1);
            this.enterGithub.Name = "enterGithub";
            this.enterGithub.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.enterGithub.RectHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(175)))), ((int)(((byte)(83)))));
            this.enterGithub.RectPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.enterGithub.RectSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.enterGithub.Size = new System.Drawing.Size(100, 35);
            this.enterGithub.Style = Sunny.UI.UIStyle.Orange;
            this.enterGithub.Symbol = 61595;
            this.enterGithub.TabIndex = 6;
            this.enterGithub.Text = "Github";
            this.enterGithub.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.enterGithub.Click += new System.EventHandler(this.enterGithub_Click);
            // 
            // enterAfdian
            // 
            this.enterAfdian.Cursor = System.Windows.Forms.Cursors.Hand;
            this.enterAfdian.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.enterAfdian.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.enterAfdian.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(175)))), ((int)(((byte)(83)))));
            this.enterAfdian.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.enterAfdian.FillSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.enterAfdian.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.enterAfdian.Location = new System.Drawing.Point(22, 359);
            this.enterAfdian.MinimumSize = new System.Drawing.Size(1, 1);
            this.enterAfdian.Name = "enterAfdian";
            this.enterAfdian.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.enterAfdian.RectHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(175)))), ((int)(((byte)(83)))));
            this.enterAfdian.RectPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.enterAfdian.RectSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.enterAfdian.Size = new System.Drawing.Size(100, 35);
            this.enterAfdian.Style = Sunny.UI.UIStyle.Orange;
            this.enterAfdian.Symbol = 261597;
            this.enterAfdian.TabIndex = 5;
            this.enterAfdian.Text = "爱发电";
            this.enterAfdian.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.enterAfdian.Click += new System.EventHandler(this.enterAfdian_Click);
            // 
            // note_3
            // 
            this.note_3.Font = new System.Drawing.Font("微软雅黑", 8F);
            this.note_3.Location = new System.Drawing.Point(19, 74);
            this.note_3.Name = "note_3";
            this.note_3.Size = new System.Drawing.Size(335, 69);
            this.note_3.Style = Sunny.UI.UIStyle.Orange;
            this.note_3.TabIndex = 4;
            this.note_3.Text = "MelonLoader-For LLC增加了镜像源，使你能够全程摆脱github安装！\r\n如果你不知道需不需要，则你应该开启。";
            this.note_3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // uiLabel1
            // 
            this.uiLabel1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLabel1.Location = new System.Drawing.Point(18, 51);
            this.uiLabel1.Name = "uiLabel1";
            this.uiLabel1.Size = new System.Drawing.Size(216, 23);
            this.uiLabel1.Style = Sunny.UI.UIStyle.Orange;
            this.uiLabel1.TabIndex = 3;
            this.uiLabel1.Text = "使用MelonLoader-For LLC";
            this.uiLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // useMFL
            // 
            this.useMFL.Active = true;
            this.useMFL.ActiveColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.useMFL.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.useMFL.Location = new System.Drawing.Point(415, 74);
            this.useMFL.MinimumSize = new System.Drawing.Size(1, 1);
            this.useMFL.Name = "useMFL";
            this.useMFL.Size = new System.Drawing.Size(75, 29);
            this.useMFL.Style = Sunny.UI.UIStyle.Orange;
            this.useMFL.TabIndex = 2;
            this.useMFL.Text = "uiSwitch1";
            // 
            // uiLine1
            // 
            this.uiLine1.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(249)))), ((int)(((byte)(241)))));
            this.uiLine1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLine1.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.uiLine1.LineColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(249)))), ((int)(((byte)(241)))));
            this.uiLine1.Location = new System.Drawing.Point(22, 199);
            this.uiLine1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiLine1.Name = "uiLine1";
            this.uiLine1.Size = new System.Drawing.Size(468, 29);
            this.uiLine1.Style = Sunny.UI.UIStyle.Orange;
            this.uiLine1.TabIndex = 13;
            this.uiLine1.Text = "手动选择节点（点击选择）";
            // 
            // dlFromOFB
            // 
            this.dlFromOFB.Cursor = System.Windows.Forms.Cursors.Hand;
            this.dlFromOFB.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.dlFromOFB.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.dlFromOFB.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(175)))), ((int)(((byte)(83)))));
            this.dlFromOFB.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.dlFromOFB.FillSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.dlFromOFB.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dlFromOFB.Location = new System.Drawing.Point(22, 235);
            this.dlFromOFB.MinimumSize = new System.Drawing.Size(1, 1);
            this.dlFromOFB.Name = "dlFromOFB";
            this.dlFromOFB.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.dlFromOFB.RectHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(175)))), ((int)(((byte)(83)))));
            this.dlFromOFB.RectPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.dlFromOFB.RectSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.dlFromOFB.Size = new System.Drawing.Size(468, 24);
            this.dlFromOFB.Style = Sunny.UI.UIStyle.Orange;
            this.dlFromOFB.TabIndex = 14;
            this.dlFromOFB.Text = "从 OneDrive For Business 下载";
            this.dlFromOFB.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dlFromOFB.Click += new System.EventHandler(this.dlFromOFB_Click);
            // 
            // dlFromLV
            // 
            this.dlFromLV.Cursor = System.Windows.Forms.Cursors.Hand;
            this.dlFromLV.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.dlFromLV.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.dlFromLV.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(175)))), ((int)(((byte)(83)))));
            this.dlFromLV.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.dlFromLV.FillSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.dlFromLV.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dlFromLV.Location = new System.Drawing.Point(22, 265);
            this.dlFromLV.MinimumSize = new System.Drawing.Size(1, 1);
            this.dlFromLV.Name = "dlFromLV";
            this.dlFromLV.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.dlFromLV.RectHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(175)))), ((int)(((byte)(83)))));
            this.dlFromLV.RectPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.dlFromLV.RectSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.dlFromLV.Size = new System.Drawing.Size(192, 24);
            this.dlFromLV.Style = Sunny.UI.UIStyle.Orange;
            this.dlFromLV.TabIndex = 15;
            this.dlFromLV.Text = "从 拉斯维加斯服务器 下载";
            this.dlFromLV.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dlFromLV.Click += new System.EventHandler(this.dlFromLV_Click);
            // 
            // dlFromDefault
            // 
            this.dlFromDefault.Cursor = System.Windows.Forms.Cursors.Hand;
            this.dlFromDefault.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.dlFromDefault.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.dlFromDefault.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(175)))), ((int)(((byte)(83)))));
            this.dlFromDefault.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.dlFromDefault.FillSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.dlFromDefault.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dlFromDefault.Location = new System.Drawing.Point(22, 295);
            this.dlFromDefault.MinimumSize = new System.Drawing.Size(1, 1);
            this.dlFromDefault.Name = "dlFromDefault";
            this.dlFromDefault.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.dlFromDefault.RectHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(175)))), ((int)(((byte)(83)))));
            this.dlFromDefault.RectPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.dlFromDefault.RectSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.dlFromDefault.Size = new System.Drawing.Size(468, 24);
            this.dlFromDefault.Style = Sunny.UI.UIStyle.Orange;
            this.dlFromDefault.TabIndex = 16;
            this.dlFromDefault.Text = "恢复默认值";
            this.dlFromDefault.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dlFromDefault.Click += new System.EventHandler(this.dlFromDefault_Click);
            // 
            // dlFromLVCDN
            // 
            this.dlFromLVCDN.Cursor = System.Windows.Forms.Cursors.Hand;
            this.dlFromLVCDN.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.dlFromLVCDN.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.dlFromLVCDN.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(175)))), ((int)(((byte)(83)))));
            this.dlFromLVCDN.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.dlFromLVCDN.FillSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.dlFromLVCDN.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dlFromLVCDN.Location = new System.Drawing.Point(220, 265);
            this.dlFromLVCDN.MinimumSize = new System.Drawing.Size(1, 1);
            this.dlFromLVCDN.Name = "dlFromLVCDN";
            this.dlFromLVCDN.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.dlFromLVCDN.RectHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(175)))), ((int)(((byte)(83)))));
            this.dlFromLVCDN.RectPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.dlFromLVCDN.RectSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.dlFromLVCDN.Size = new System.Drawing.Size(270, 24);
            this.dlFromLVCDN.Style = Sunny.UI.UIStyle.Orange;
            this.dlFromLVCDN.TabIndex = 17;
            this.dlFromLVCDN.Text = "从 拉斯维加斯服务器 with CDN 下载";
            this.dlFromLVCDN.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dlFromLVCDN.Click += new System.EventHandler(this.dlFromLVCDN_Click);
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1223, 583);
            this.ControlBoxFillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(175)))), ((int)(((byte)(83)))));
            this.Controls.Add(this.uiGroupBox1);
            this.Controls.Add(this.status);
            this.Controls.Add(this.note_2);
            this.Controls.Add(this.uiGroupBox_install);
            this.Controls.Add(this.note);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.ShowTitleIcon = true;
            this.Style = Sunny.UI.UIStyle.Orange;
            this.Text = "LLC Mod Toolbox v0.3.6";
            this.TitleColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.ZoomScaleRect = new System.Drawing.Rectangle(15, 15, 800, 450);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.uiGroupBox_install.ResumeLayout(false);
            this.uiGroupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Sunny.UI.UILabel note;
        private Sunny.UI.UIGroupBox uiGroupBox_install;
        private Sunny.UI.UILabel noteLabel;
        private Sunny.UI.UIProcessBar done_Bar;
        private Sunny.UI.UIProcessBar down_Bar;
        private Sunny.UI.UILabel note_2;
        private Sunny.UI.UILabel status;
        private Sunny.UI.UISymbolButton installButton;
        private Sunny.UI.UIGroupBox uiGroupBox1;
        private Sunny.UI.UILabel uiLabel1;
        private Sunny.UI.UISwitch useMFL;
        private Sunny.UI.UILabel note_3;
        private Sunny.UI.UISymbolButton enterWiki;
        private Sunny.UI.UILine linkline;
        private Sunny.UI.UISymbolButton enterGithub;
        private Sunny.UI.UISymbolButton enterAfdian;
        private Sunny.UI.UILabel uiLabel3;
        private Sunny.UI.UISwitch useGithub;
        private Sunny.UI.UILabel uiLabel2;
        private Sunny.UI.UISymbolButton enterDoc;
        private Sunny.UI.UIButton dlFromDefault;
        private Sunny.UI.UIButton dlFromLV;
        private Sunny.UI.UIButton dlFromOFB;
        private Sunny.UI.UILine uiLine1;
        private Sunny.UI.UIButton dlFromLVCDN;
    }
}
