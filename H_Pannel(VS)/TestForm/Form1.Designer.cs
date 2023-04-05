
namespace TestForm
{
    partial class Form1
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.測試單元 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBox_PortName = new System.Windows.Forms.ComboBox();
            this.系統 = new System.Windows.Forms.TabPage();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.EPD290 = new System.Windows.Forms.TabPage();
            this.storageUI_EPD_290 = new H_Pannel_lib.StorageUI_EPD_290();
            this.PLC = new System.Windows.Forms.TabPage();
            this.lowerMachine_Panel = new LadderUI.LowerMachine_Panel();
            this.plC_RJ_GroupBox1 = new MyUI.PLC_RJ_GroupBox();
            this.plC_RJ_Button_EPD290_亮燈 = new MyUI.PLC_RJ_Button();
            this.plC_RJ_Button_EPD290_測試一次 = new MyUI.PLC_RJ_Button();
            this.plC_RJ_Button_EPD290_刷新電子紙 = new MyUI.PLC_RJ_Button();
            this.plC_UI_Init = new MyUI.PLC_UI_Init();
            this.tabControl1.SuspendLayout();
            this.測試單元.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.系統.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.EPD290.SuspendLayout();
            this.PLC.SuspendLayout();
            this.plC_RJ_GroupBox1.ContentsPanel.SuspendLayout();
            this.plC_RJ_GroupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.測試單元);
            this.tabControl1.Controls.Add(this.系統);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1439, 921);
            this.tabControl1.TabIndex = 0;
            // 
            // 測試單元
            // 
            this.測試單元.Controls.Add(this.plC_RJ_GroupBox1);
            this.測試單元.Controls.Add(this.groupBox1);
            this.測試單元.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.測試單元.Location = new System.Drawing.Point(4, 22);
            this.測試單元.Name = "測試單元";
            this.測試單元.Padding = new System.Windows.Forms.Padding(3);
            this.測試單元.Size = new System.Drawing.Size(1431, 895);
            this.測試單元.TabIndex = 0;
            this.測試單元.Text = "測試單元";
            this.測試單元.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBox_PortName);
            this.groupBox1.Font = new System.Drawing.Font("新細明體", 9F);
            this.groupBox1.Location = new System.Drawing.Point(22, 16);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(169, 79);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "選擇串口";
            // 
            // comboBox_PortName
            // 
            this.comboBox_PortName.FormattingEnabled = true;
            this.comboBox_PortName.Location = new System.Drawing.Point(23, 34);
            this.comboBox_PortName.Name = "comboBox_PortName";
            this.comboBox_PortName.Size = new System.Drawing.Size(121, 27);
            this.comboBox_PortName.TabIndex = 1;
            this.comboBox_PortName.Enter += new System.EventHandler(this.comboBox_PortName_Enter);
            // 
            // 系統
            // 
            this.系統.Controls.Add(this.tabControl2);
            this.系統.Location = new System.Drawing.Point(4, 22);
            this.系統.Name = "系統";
            this.系統.Padding = new System.Windows.Forms.Padding(3);
            this.系統.Size = new System.Drawing.Size(1431, 895);
            this.系統.TabIndex = 1;
            this.系統.Text = "系統";
            this.系統.UseVisualStyleBackColor = true;
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.EPD290);
            this.tabControl2.Controls.Add(this.PLC);
            this.tabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl2.Location = new System.Drawing.Point(3, 3);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(1425, 889);
            this.tabControl2.TabIndex = 0;
            // 
            // EPD290
            // 
            this.EPD290.Controls.Add(this.storageUI_EPD_290);
            this.EPD290.Location = new System.Drawing.Point(4, 22);
            this.EPD290.Name = "EPD290";
            this.EPD290.Padding = new System.Windows.Forms.Padding(3);
            this.EPD290.Size = new System.Drawing.Size(1417, 863);
            this.EPD290.TabIndex = 0;
            this.EPD290.Text = "EPD290";
            this.EPD290.UseVisualStyleBackColor = true;
            // 
            // storageUI_EPD_290
            // 
            this.storageUI_EPD_290.DataBaseName = "TEST";
            this.storageUI_EPD_290.Dock = System.Windows.Forms.DockStyle.Fill;
            this.storageUI_EPD_290.IP = "localhost";
            this.storageUI_EPD_290.Location = new System.Drawing.Point(3, 3);
            this.storageUI_EPD_290.Name = "storageUI_EPD_290";
            this.storageUI_EPD_290.Password = "user82822040";
            this.storageUI_EPD_290.Port = ((uint)(3306u));
            this.storageUI_EPD_290.Size = new System.Drawing.Size(1411, 857);
            this.storageUI_EPD_290.TabIndex = 2;
            this.storageUI_EPD_290.TableName = "EPD290_Jsonstring";
            this.storageUI_EPD_290.UDP_LocalPorts = ((System.Collections.Generic.List<string>)(resources.GetObject("storageUI_EPD_290.UDP_LocalPorts")));
            this.storageUI_EPD_290.UDP_ServerPorts = ((System.Collections.Generic.List<string>)(resources.GetObject("storageUI_EPD_290.UDP_ServerPorts")));
            this.storageUI_EPD_290.UserName = "root";
            // 
            // PLC
            // 
            this.PLC.Controls.Add(this.plC_UI_Init);
            this.PLC.Controls.Add(this.lowerMachine_Panel);
            this.PLC.Location = new System.Drawing.Point(4, 22);
            this.PLC.Name = "PLC";
            this.PLC.Size = new System.Drawing.Size(1417, 863);
            this.PLC.TabIndex = 1;
            this.PLC.Text = "PLC";
            this.PLC.UseVisualStyleBackColor = true;
            // 
            // lowerMachine_Panel
            // 
            this.lowerMachine_Panel.Location = new System.Drawing.Point(15, 15);
            this.lowerMachine_Panel.Name = "lowerMachine_Panel";
            this.lowerMachine_Panel.Size = new System.Drawing.Size(869, 565);
            this.lowerMachine_Panel.TabIndex = 0;
            this.lowerMachine_Panel.掃描速度 = 0;
            // 
            // plC_RJ_GroupBox1
            // 
            // 
            // plC_RJ_GroupBox1.ContentsPanel
            // 
            this.plC_RJ_GroupBox1.ContentsPanel.BackColor = System.Drawing.Color.White;
            this.plC_RJ_GroupBox1.ContentsPanel.BorderColor = System.Drawing.Color.SkyBlue;
            this.plC_RJ_GroupBox1.ContentsPanel.BorderRadius = 5;
            this.plC_RJ_GroupBox1.ContentsPanel.BorderSize = 2;
            this.plC_RJ_GroupBox1.ContentsPanel.Controls.Add(this.plC_RJ_Button_EPD290_亮燈);
            this.plC_RJ_GroupBox1.ContentsPanel.Controls.Add(this.plC_RJ_Button_EPD290_測試一次);
            this.plC_RJ_GroupBox1.ContentsPanel.Controls.Add(this.plC_RJ_Button_EPD290_刷新電子紙);
            this.plC_RJ_GroupBox1.ContentsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plC_RJ_GroupBox1.ContentsPanel.ForeColor = System.Drawing.Color.White;
            this.plC_RJ_GroupBox1.ContentsPanel.IsSelected = false;
            this.plC_RJ_GroupBox1.ContentsPanel.Location = new System.Drawing.Point(0, 45);
            this.plC_RJ_GroupBox1.ContentsPanel.Margin = new System.Windows.Forms.Padding(11, 12, 11, 12);
            this.plC_RJ_GroupBox1.ContentsPanel.Name = "ContentsPanel";
            this.plC_RJ_GroupBox1.ContentsPanel.Size = new System.Drawing.Size(532, 91);
            this.plC_RJ_GroupBox1.ContentsPanel.TabIndex = 2;
            this.plC_RJ_GroupBox1.Location = new System.Drawing.Point(200, 16);
            this.plC_RJ_GroupBox1.Margin = new System.Windows.Forms.Padding(6);
            this.plC_RJ_GroupBox1.Name = "plC_RJ_GroupBox1";
            this.plC_RJ_GroupBox1.PannelBackColor = System.Drawing.Color.White;
            this.plC_RJ_GroupBox1.PannelBorderColor = System.Drawing.Color.SkyBlue;
            this.plC_RJ_GroupBox1.PannelBorderRadius = 5;
            this.plC_RJ_GroupBox1.PannelBorderSize = 2;
            this.plC_RJ_GroupBox1.Size = new System.Drawing.Size(532, 136);
            this.plC_RJ_GroupBox1.TabIndex = 41;
            this.plC_RJ_GroupBox1.TitleBackColor = System.Drawing.Color.DeepSkyBlue;
            this.plC_RJ_GroupBox1.TitleBorderColor = System.Drawing.Color.PaleVioletRed;
            this.plC_RJ_GroupBox1.TitleBorderRadius = 5;
            this.plC_RJ_GroupBox1.TitleBorderSize = 0;
            this.plC_RJ_GroupBox1.TitleFont = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.plC_RJ_GroupBox1.TitleForeColor = System.Drawing.Color.White;
            this.plC_RJ_GroupBox1.TitleHeight = 45;
            this.plC_RJ_GroupBox1.TitleTextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.plC_RJ_GroupBox1.TitleTexts = "EPD290";
            // 
            // plC_RJ_Button_EPD290_亮燈
            // 
            this.plC_RJ_Button_EPD290_亮燈.AutoResetState = false;
            this.plC_RJ_Button_EPD290_亮燈.BackgroundColor = System.Drawing.Color.DodgerBlue;
            this.plC_RJ_Button_EPD290_亮燈.Bool = false;
            this.plC_RJ_Button_EPD290_亮燈.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.plC_RJ_Button_EPD290_亮燈.BorderRadius = 5;
            this.plC_RJ_Button_EPD290_亮燈.BorderSize = 0;
            this.plC_RJ_Button_EPD290_亮燈.but_press = false;
            this.plC_RJ_Button_EPD290_亮燈.buttonType = MyUI.RJ_Button.ButtonType.Toggle;
            this.plC_RJ_Button_EPD290_亮燈.FlatAppearance.BorderSize = 0;
            this.plC_RJ_Button_EPD290_亮燈.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.plC_RJ_Button_EPD290_亮燈.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold);
            this.plC_RJ_Button_EPD290_亮燈.Icon = System.Windows.Forms.MessageBoxIcon.Warning;
            this.plC_RJ_Button_EPD290_亮燈.Location = new System.Drawing.Point(19, 10);
            this.plC_RJ_Button_EPD290_亮燈.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.plC_RJ_Button_EPD290_亮燈.Name = "plC_RJ_Button_EPD290_亮燈";
            this.plC_RJ_Button_EPD290_亮燈.OFF_文字內容 = "亮燈";
            this.plC_RJ_Button_EPD290_亮燈.OFF_文字字體 = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold);
            this.plC_RJ_Button_EPD290_亮燈.OFF_文字顏色 = System.Drawing.Color.White;
            this.plC_RJ_Button_EPD290_亮燈.OFF_背景顏色 = System.Drawing.SystemColors.Control;
            this.plC_RJ_Button_EPD290_亮燈.ON_BorderSize = 5;
            this.plC_RJ_Button_EPD290_亮燈.ON_文字內容 = "亮燈";
            this.plC_RJ_Button_EPD290_亮燈.ON_文字字體 = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold);
            this.plC_RJ_Button_EPD290_亮燈.ON_文字顏色 = System.Drawing.Color.Black;
            this.plC_RJ_Button_EPD290_亮燈.ON_背景顏色 = System.Drawing.SystemColors.Control;
            this.plC_RJ_Button_EPD290_亮燈.Size = new System.Drawing.Size(143, 72);
            this.plC_RJ_Button_EPD290_亮燈.State = false;
            this.plC_RJ_Button_EPD290_亮燈.TabIndex = 38;
            this.plC_RJ_Button_EPD290_亮燈.Text = "亮燈";
            this.plC_RJ_Button_EPD290_亮燈.TextColor = System.Drawing.Color.White;
            this.plC_RJ_Button_EPD290_亮燈.Texts = "亮燈";
            this.plC_RJ_Button_EPD290_亮燈.UseVisualStyleBackColor = false;
            this.plC_RJ_Button_EPD290_亮燈.字型鎖住 = false;
            this.plC_RJ_Button_EPD290_亮燈.按鈕型態 = MyUI.PLC_RJ_Button.StatusEnum.保持型;
            this.plC_RJ_Button_EPD290_亮燈.按鍵方式 = MyUI.PLC_RJ_Button.PressEnum.Mouse_左鍵;
            this.plC_RJ_Button_EPD290_亮燈.文字鎖住 = false;
            this.plC_RJ_Button_EPD290_亮燈.讀取位元反向 = false;
            this.plC_RJ_Button_EPD290_亮燈.讀寫鎖住 = false;
            this.plC_RJ_Button_EPD290_亮燈.音效 = true;
            this.plC_RJ_Button_EPD290_亮燈.顯示 = false;
            this.plC_RJ_Button_EPD290_亮燈.顯示狀態 = false;
            // 
            // plC_RJ_Button_EPD290_測試一次
            // 
            this.plC_RJ_Button_EPD290_測試一次.AutoResetState = false;
            this.plC_RJ_Button_EPD290_測試一次.BackgroundColor = System.Drawing.Color.DodgerBlue;
            this.plC_RJ_Button_EPD290_測試一次.Bool = false;
            this.plC_RJ_Button_EPD290_測試一次.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.plC_RJ_Button_EPD290_測試一次.BorderRadius = 5;
            this.plC_RJ_Button_EPD290_測試一次.BorderSize = 0;
            this.plC_RJ_Button_EPD290_測試一次.but_press = false;
            this.plC_RJ_Button_EPD290_測試一次.buttonType = MyUI.RJ_Button.ButtonType.Toggle;
            this.plC_RJ_Button_EPD290_測試一次.FlatAppearance.BorderSize = 0;
            this.plC_RJ_Button_EPD290_測試一次.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.plC_RJ_Button_EPD290_測試一次.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold);
            this.plC_RJ_Button_EPD290_測試一次.Icon = System.Windows.Forms.MessageBoxIcon.Warning;
            this.plC_RJ_Button_EPD290_測試一次.Location = new System.Drawing.Point(320, 10);
            this.plC_RJ_Button_EPD290_測試一次.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.plC_RJ_Button_EPD290_測試一次.Name = "plC_RJ_Button_EPD290_測試一次";
            this.plC_RJ_Button_EPD290_測試一次.OFF_文字內容 = "測試一次";
            this.plC_RJ_Button_EPD290_測試一次.OFF_文字字體 = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold);
            this.plC_RJ_Button_EPD290_測試一次.OFF_文字顏色 = System.Drawing.Color.White;
            this.plC_RJ_Button_EPD290_測試一次.OFF_背景顏色 = System.Drawing.SystemColors.Control;
            this.plC_RJ_Button_EPD290_測試一次.ON_BorderSize = 5;
            this.plC_RJ_Button_EPD290_測試一次.ON_文字內容 = "測試一次";
            this.plC_RJ_Button_EPD290_測試一次.ON_文字字體 = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold);
            this.plC_RJ_Button_EPD290_測試一次.ON_文字顏色 = System.Drawing.Color.Black;
            this.plC_RJ_Button_EPD290_測試一次.ON_背景顏色 = System.Drawing.SystemColors.Control;
            this.plC_RJ_Button_EPD290_測試一次.Size = new System.Drawing.Size(143, 72);
            this.plC_RJ_Button_EPD290_測試一次.State = false;
            this.plC_RJ_Button_EPD290_測試一次.TabIndex = 40;
            this.plC_RJ_Button_EPD290_測試一次.Text = "測試一次";
            this.plC_RJ_Button_EPD290_測試一次.TextColor = System.Drawing.Color.White;
            this.plC_RJ_Button_EPD290_測試一次.Texts = "測試一次";
            this.plC_RJ_Button_EPD290_測試一次.UseVisualStyleBackColor = false;
            this.plC_RJ_Button_EPD290_測試一次.字型鎖住 = false;
            this.plC_RJ_Button_EPD290_測試一次.按鈕型態 = MyUI.PLC_RJ_Button.StatusEnum.保持型;
            this.plC_RJ_Button_EPD290_測試一次.按鍵方式 = MyUI.PLC_RJ_Button.PressEnum.Mouse_左鍵;
            this.plC_RJ_Button_EPD290_測試一次.文字鎖住 = false;
            this.plC_RJ_Button_EPD290_測試一次.讀取位元反向 = false;
            this.plC_RJ_Button_EPD290_測試一次.讀寫鎖住 = false;
            this.plC_RJ_Button_EPD290_測試一次.音效 = true;
            this.plC_RJ_Button_EPD290_測試一次.顯示 = false;
            this.plC_RJ_Button_EPD290_測試一次.顯示狀態 = false;
            // 
            // plC_RJ_Button_EPD290_刷新電子紙
            // 
            this.plC_RJ_Button_EPD290_刷新電子紙.AutoResetState = false;
            this.plC_RJ_Button_EPD290_刷新電子紙.BackgroundColor = System.Drawing.Color.DodgerBlue;
            this.plC_RJ_Button_EPD290_刷新電子紙.Bool = false;
            this.plC_RJ_Button_EPD290_刷新電子紙.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.plC_RJ_Button_EPD290_刷新電子紙.BorderRadius = 5;
            this.plC_RJ_Button_EPD290_刷新電子紙.BorderSize = 0;
            this.plC_RJ_Button_EPD290_刷新電子紙.but_press = false;
            this.plC_RJ_Button_EPD290_刷新電子紙.buttonType = MyUI.RJ_Button.ButtonType.Toggle;
            this.plC_RJ_Button_EPD290_刷新電子紙.FlatAppearance.BorderSize = 0;
            this.plC_RJ_Button_EPD290_刷新電子紙.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.plC_RJ_Button_EPD290_刷新電子紙.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold);
            this.plC_RJ_Button_EPD290_刷新電子紙.Icon = System.Windows.Forms.MessageBoxIcon.Warning;
            this.plC_RJ_Button_EPD290_刷新電子紙.Location = new System.Drawing.Point(165, 10);
            this.plC_RJ_Button_EPD290_刷新電子紙.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.plC_RJ_Button_EPD290_刷新電子紙.Name = "plC_RJ_Button_EPD290_刷新電子紙";
            this.plC_RJ_Button_EPD290_刷新電子紙.OFF_文字內容 = "刷新電子紙";
            this.plC_RJ_Button_EPD290_刷新電子紙.OFF_文字字體 = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold);
            this.plC_RJ_Button_EPD290_刷新電子紙.OFF_文字顏色 = System.Drawing.Color.White;
            this.plC_RJ_Button_EPD290_刷新電子紙.OFF_背景顏色 = System.Drawing.SystemColors.Control;
            this.plC_RJ_Button_EPD290_刷新電子紙.ON_BorderSize = 5;
            this.plC_RJ_Button_EPD290_刷新電子紙.ON_文字內容 = "刷新電子紙";
            this.plC_RJ_Button_EPD290_刷新電子紙.ON_文字字體 = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold);
            this.plC_RJ_Button_EPD290_刷新電子紙.ON_文字顏色 = System.Drawing.Color.Black;
            this.plC_RJ_Button_EPD290_刷新電子紙.ON_背景顏色 = System.Drawing.SystemColors.Control;
            this.plC_RJ_Button_EPD290_刷新電子紙.Size = new System.Drawing.Size(143, 72);
            this.plC_RJ_Button_EPD290_刷新電子紙.State = false;
            this.plC_RJ_Button_EPD290_刷新電子紙.TabIndex = 39;
            this.plC_RJ_Button_EPD290_刷新電子紙.Text = "刷新電子紙";
            this.plC_RJ_Button_EPD290_刷新電子紙.TextColor = System.Drawing.Color.White;
            this.plC_RJ_Button_EPD290_刷新電子紙.Texts = "刷新電子紙";
            this.plC_RJ_Button_EPD290_刷新電子紙.UseVisualStyleBackColor = false;
            this.plC_RJ_Button_EPD290_刷新電子紙.字型鎖住 = false;
            this.plC_RJ_Button_EPD290_刷新電子紙.按鈕型態 = MyUI.PLC_RJ_Button.StatusEnum.保持型;
            this.plC_RJ_Button_EPD290_刷新電子紙.按鍵方式 = MyUI.PLC_RJ_Button.PressEnum.Mouse_左鍵;
            this.plC_RJ_Button_EPD290_刷新電子紙.文字鎖住 = false;
            this.plC_RJ_Button_EPD290_刷新電子紙.讀取位元反向 = false;
            this.plC_RJ_Button_EPD290_刷新電子紙.讀寫鎖住 = false;
            this.plC_RJ_Button_EPD290_刷新電子紙.音效 = true;
            this.plC_RJ_Button_EPD290_刷新電子紙.顯示 = false;
            this.plC_RJ_Button_EPD290_刷新電子紙.顯示狀態 = false;
            // 
            // plC_UI_Init
            // 
            this.plC_UI_Init.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.plC_UI_Init.Location = new System.Drawing.Point(900, 15);
            this.plC_UI_Init.Name = "plC_UI_Init";
            this.plC_UI_Init.Size = new System.Drawing.Size(72, 25);
            this.plC_UI_Init.TabIndex = 1;
            this.plC_UI_Init.光道視覺元件初始化 = false;
            this.plC_UI_Init.全螢幕顯示 = false;
            this.plC_UI_Init.掃描速度 = 100;
            this.plC_UI_Init.起始畫面標題內容 = "鴻森整合機電有限公司";
            this.plC_UI_Init.起始畫面標題字體 = new System.Drawing.Font("標楷體", 20F, System.Drawing.FontStyle.Bold);
            this.plC_UI_Init.起始畫面背景 = ((System.Drawing.Image)(resources.GetObject("plC_UI_Init.起始畫面背景")));
            this.plC_UI_Init.起始畫面顯示 = false;
            this.plC_UI_Init.邁得威視元件初始化 = false;
            this.plC_UI_Init.開機延遲 = 0;
            this.plC_UI_Init.音效 = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1439, 921);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "測試軟體";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.測試單元.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.系統.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.EPD290.ResumeLayout(false);
            this.PLC.ResumeLayout(false);
            this.plC_RJ_GroupBox1.ContentsPanel.ResumeLayout(false);
            this.plC_RJ_GroupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage 測試單元;
        private System.Windows.Forms.TabPage 系統;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox comboBox_PortName;
        private MyUI.RJ_Button rJ_Button_亮燈;
        private MyUI.RJ_Button rJ_Button_刷新電子紙;
        private MyUI.RJ_Button rJ_Button_測試一次;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage EPD290;
        private H_Pannel_lib.StorageUI_EPD_290 storageUI_EPD_290;
        private System.Windows.Forms.TabPage PLC;
        private MyUI.PLC_UI_Init plC_UI_Init;
        private LadderUI.LowerMachine_Panel lowerMachine_Panel;
        private MyUI.PLC_RJ_Button plC_RJ_Button_EPD290_測試一次;
        private MyUI.PLC_RJ_Button plC_RJ_Button_EPD290_刷新電子紙;
        public MyUI.PLC_RJ_Button plC_RJ_Button_EPD290_亮燈;
        private MyUI.PLC_RJ_GroupBox plC_RJ_GroupBox1;
    }
}

