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
            this.disable = new Sunny.UI.UIButton();
            this.canable = new Sunny.UI.UIButton();
            this.useMFL = new Sunny.UI.UISwitch();
            this.uiLabel1 = new Sunny.UI.UILabel();
            this.note_3 = new Sunny.UI.UILabel();
            this.enterAfdian = new Sunny.UI.UISymbolButton();
            this.enterGithub = new Sunny.UI.UISymbolButton();
            this.linkline = new Sunny.UI.UILine();
            this.enterWiki = new Sunny.UI.UISymbolButton();
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
            this.uiGroupBox_install.Size = new System.Drawing.Size(613, 377);
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
            this.installButton.Location = new System.Drawing.Point(233, 147);
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
            this.down_Bar.Location = new System.Drawing.Point(142, 314);
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
            this.done_Bar.Location = new System.Drawing.Point(142, 288);
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
            this.uiGroupBox1.Controls.Add(this.enterWiki);
            this.uiGroupBox1.Controls.Add(this.linkline);
            this.uiGroupBox1.Controls.Add(this.enterGithub);
            this.uiGroupBox1.Controls.Add(this.enterAfdian);
            this.uiGroupBox1.Controls.Add(this.note_3);
            this.uiGroupBox1.Controls.Add(this.uiLabel1);
            this.uiGroupBox1.Controls.Add(this.useMFL);
            this.uiGroupBox1.Controls.Add(this.canable);
            this.uiGroupBox1.Controls.Add(this.disable);
            this.uiGroupBox1.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(249)))), ((int)(((byte)(241)))));
            this.uiGroupBox1.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(249)))), ((int)(((byte)(241)))));
            this.uiGroupBox1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiGroupBox1.Location = new System.Drawing.Point(678, 109);
            this.uiGroupBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiGroupBox1.MinimumSize = new System.Drawing.Size(1, 1);
            this.uiGroupBox1.Name = "uiGroupBox1";
            this.uiGroupBox1.Padding = new System.Windows.Forms.Padding(0, 32, 0, 0);
            this.uiGroupBox1.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.uiGroupBox1.Size = new System.Drawing.Size(414, 377);
            this.uiGroupBox1.Style = Sunny.UI.UIStyle.Orange;
            this.uiGroupBox1.TabIndex = 4;
            this.uiGroupBox1.Text = "其他";
            this.uiGroupBox1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // disable
            // 
            this.disable.Cursor = System.Windows.Forms.Cursors.Hand;
            this.disable.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.disable.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.disable.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(175)))), ((int)(((byte)(83)))));
            this.disable.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.disable.FillSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.disable.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.disable.Location = new System.Drawing.Point(22, 46);
            this.disable.MinimumSize = new System.Drawing.Size(1, 1);
            this.disable.Name = "disable";
            this.disable.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.disable.RectHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(175)))), ((int)(((byte)(83)))));
            this.disable.RectPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.disable.RectSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.disable.Size = new System.Drawing.Size(100, 35);
            this.disable.Style = Sunny.UI.UIStyle.Orange;
            this.disable.TabIndex = 0;
            this.disable.Text = "禁用模组";
            this.disable.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.disable.Click += new System.EventHandler(this.disable_Click);
            // 
            // canable
            // 
            this.canable.Cursor = System.Windows.Forms.Cursors.Hand;
            this.canable.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.canable.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.canable.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(175)))), ((int)(((byte)(83)))));
            this.canable.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.canable.FillSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.canable.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.canable.Location = new System.Drawing.Point(163, 46);
            this.canable.MinimumSize = new System.Drawing.Size(1, 1);
            this.canable.Name = "canable";
            this.canable.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.canable.RectHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(175)))), ((int)(((byte)(83)))));
            this.canable.RectPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.canable.RectSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.canable.Size = new System.Drawing.Size(100, 35);
            this.canable.Style = Sunny.UI.UIStyle.Orange;
            this.canable.TabIndex = 1;
            this.canable.Text = "启用模组";
            this.canable.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.canable.Click += new System.EventHandler(this.canable_Click);
            // 
            // useMFL
            // 
            this.useMFL.Active = true;
            this.useMFL.ActiveColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.useMFL.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.useMFL.Location = new System.Drawing.Point(255, 109);
            this.useMFL.MinimumSize = new System.Drawing.Size(1, 1);
            this.useMFL.Name = "useMFL";
            this.useMFL.Size = new System.Drawing.Size(75, 29);
            this.useMFL.Style = Sunny.UI.UIStyle.Orange;
            this.useMFL.TabIndex = 2;
            this.useMFL.Text = "uiSwitch1";
            // 
            // uiLabel1
            // 
            this.uiLabel1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiLabel1.Location = new System.Drawing.Point(18, 111);
            this.uiLabel1.Name = "uiLabel1";
            this.uiLabel1.Size = new System.Drawing.Size(216, 23);
            this.uiLabel1.Style = Sunny.UI.UIStyle.Orange;
            this.uiLabel1.TabIndex = 3;
            this.uiLabel1.Text = "使用MelonLoader-For LLC";
            this.uiLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // note_3
            // 
            this.note_3.Font = new System.Drawing.Font("微软雅黑", 8F);
            this.note_3.Location = new System.Drawing.Point(19, 141);
            this.note_3.Name = "note_3";
            this.note_3.Size = new System.Drawing.Size(335, 69);
            this.note_3.Style = Sunny.UI.UIStyle.Orange;
            this.note_3.TabIndex = 4;
            this.note_3.Text = "MelonLoader-For LLC增加了镜像源，使你能够全程摆脱github安装！\r\n如果你不知道需不需要，则你应该开启。";
            this.note_3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            this.enterAfdian.Location = new System.Drawing.Point(22, 268);
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
            // enterGithub
            // 
            this.enterGithub.Cursor = System.Windows.Forms.Cursors.Hand;
            this.enterGithub.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.enterGithub.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.enterGithub.FillHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(175)))), ((int)(((byte)(83)))));
            this.enterGithub.FillPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.enterGithub.FillSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.enterGithub.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.enterGithub.Location = new System.Drawing.Point(147, 268);
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
            // linkline
            // 
            this.linkline.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(249)))), ((int)(((byte)(241)))));
            this.linkline.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.linkline.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.linkline.LineColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(249)))), ((int)(((byte)(241)))));
            this.linkline.Location = new System.Drawing.Point(22, 214);
            this.linkline.MinimumSize = new System.Drawing.Size(1, 1);
            this.linkline.Name = "linkline";
            this.linkline.Size = new System.Drawing.Size(360, 29);
            this.linkline.Style = Sunny.UI.UIStyle.Orange;
            this.linkline.TabIndex = 7;
            this.linkline.Text = "链接";
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
            this.enterWiki.Location = new System.Drawing.Point(271, 268);
            this.enterWiki.MinimumSize = new System.Drawing.Size(1, 1);
            this.enterWiki.Name = "enterWiki";
            this.enterWiki.RectColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(155)))), ((int)(((byte)(40)))));
            this.enterWiki.RectHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(175)))), ((int)(((byte)(83)))));
            this.enterWiki.RectPressColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.enterWiki.RectSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(124)))), ((int)(((byte)(32)))));
            this.enterWiki.Size = new System.Drawing.Size(111, 35);
            this.enterWiki.Style = Sunny.UI.UIStyle.Orange;
            this.enterWiki.Symbol = 162056;
            this.enterWiki.TabIndex = 8;
            this.enterWiki.Text = "灰机维基";
            this.enterWiki.TipsFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.enterWiki.Click += new System.EventHandler(this.enterWiki_Click);
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1118, 520);
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
            this.Text = "LLC Mod Toolbox";
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
        private Sunny.UI.UIButton disable;
        private Sunny.UI.UIButton canable;
        private Sunny.UI.UILabel uiLabel1;
        private Sunny.UI.UISwitch useMFL;
        private Sunny.UI.UILabel note_3;
        private Sunny.UI.UISymbolButton enterWiki;
        private Sunny.UI.UILine linkline;
        private Sunny.UI.UISymbolButton enterGithub;
        private Sunny.UI.UISymbolButton enterAfdian;
    }
}

