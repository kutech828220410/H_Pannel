
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
            this.plC_RJ_GroupBox2 = new MyUI.PLC_RJ_GroupBox();
            this.rJ_TextBox_EPD266_ServerIP = new MyUI.RJ_TextBox();
            this.rJ_Lable2 = new MyUI.RJ_Lable();
            this.rJ_TextBox_EPD266_Password = new MyUI.RJ_TextBox();
            this.rJ_Lable = new MyUI.RJ_Lable();
            this.rJ_TextBox_EPD266_SSID = new MyUI.RJ_TextBox();
            this.rJ_Lable_EPD266_SSID = new MyUI.RJ_Lable();
            this.rJ_TextBox_EPD266_IP = new MyUI.RJ_TextBox();
            this.rJ_Lable1 = new MyUI.RJ_Lable();
            this.plC_RJ_Button_EPD266_OTA = new MyUI.PLC_RJ_Button();
            this.plC_RJ_Button_EPD266_寫入參數 = new MyUI.PLC_RJ_Button();
            this.plC_RJ_Button_EPD266_亮燈 = new MyUI.PLC_RJ_Button();
            this.plC_RJ_Button_EPD266_測試一次 = new MyUI.PLC_RJ_Button();
            this.plC_RJ_Button_EPD266_刷新電子紙 = new MyUI.PLC_RJ_Button();
            this.plC_RJ_GroupBox1 = new MyUI.PLC_RJ_GroupBox();
            this.plC_RJ_Button_EPD290_亮燈 = new MyUI.PLC_RJ_Button();
            this.plC_RJ_Button_EPD290_測試一次 = new MyUI.PLC_RJ_Button();
            this.plC_RJ_Button_EPD290_刷新電子紙 = new MyUI.PLC_RJ_Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBox_PortName = new System.Windows.Forms.ComboBox();
            this.系統 = new System.Windows.Forms.TabPage();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.EPD290 = new System.Windows.Forms.TabPage();
            this.storageUI_EPD_290 = new H_Pannel_lib.StorageUI_EPD_290();
            this.EPD266 = new System.Windows.Forms.TabPage();
            this.storageUI_EPD_266 = new H_Pannel_lib.StorageUI_EPD_266();
            this.PLC = new System.Windows.Forms.TabPage();
            this.plC_UI_Init = new MyUI.PLC_UI_Init();
            this.lowerMachine_Panel = new LadderUI.LowerMachine_Panel();
            this.plC_RJ_Button_EPD266_IP加1 = new MyUI.PLC_RJ_Button();
            this.tabControl1.SuspendLayout();
            this.測試單元.SuspendLayout();
            this.plC_RJ_GroupBox2.ContentsPanel.SuspendLayout();
            this.plC_RJ_GroupBox2.SuspendLayout();
            this.plC_RJ_GroupBox1.ContentsPanel.SuspendLayout();
            this.plC_RJ_GroupBox1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.系統.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.EPD290.SuspendLayout();
            this.EPD266.SuspendLayout();
            this.PLC.SuspendLayout();
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
            this.測試單元.Controls.Add(this.plC_RJ_GroupBox2);
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
            // plC_RJ_GroupBox2
            // 
            // 
            // plC_RJ_GroupBox2.ContentsPanel
            // 
            this.plC_RJ_GroupBox2.ContentsPanel.BackColor = System.Drawing.Color.White;
            this.plC_RJ_GroupBox2.ContentsPanel.BorderColor = System.Drawing.Color.SkyBlue;
            this.plC_RJ_GroupBox2.ContentsPanel.BorderRadius = 5;
            this.plC_RJ_GroupBox2.ContentsPanel.BorderSize = 2;
            this.plC_RJ_GroupBox2.ContentsPanel.Controls.Add(this.plC_RJ_Button_EPD266_IP加1);
            this.plC_RJ_GroupBox2.ContentsPanel.Controls.Add(this.rJ_TextBox_EPD266_ServerIP);
            this.plC_RJ_GroupBox2.ContentsPanel.Controls.Add(this.rJ_Lable2);
            this.plC_RJ_GroupBox2.ContentsPanel.Controls.Add(this.rJ_TextBox_EPD266_Password);
            this.plC_RJ_GroupBox2.ContentsPanel.Controls.Add(this.rJ_Lable);
            this.plC_RJ_GroupBox2.ContentsPanel.Controls.Add(this.rJ_TextBox_EPD266_SSID);
            this.plC_RJ_GroupBox2.ContentsPanel.Controls.Add(this.rJ_Lable_EPD266_SSID);
            this.plC_RJ_GroupBox2.ContentsPanel.Controls.Add(this.rJ_TextBox_EPD266_IP);
            this.plC_RJ_GroupBox2.ContentsPanel.Controls.Add(this.rJ_Lable1);
            this.plC_RJ_GroupBox2.ContentsPanel.Controls.Add(this.plC_RJ_Button_EPD266_OTA);
            this.plC_RJ_GroupBox2.ContentsPanel.Controls.Add(this.plC_RJ_Button_EPD266_寫入參數);
            this.plC_RJ_GroupBox2.ContentsPanel.Controls.Add(this.plC_RJ_Button_EPD266_亮燈);
            this.plC_RJ_GroupBox2.ContentsPanel.Controls.Add(this.plC_RJ_Button_EPD266_測試一次);
            this.plC_RJ_GroupBox2.ContentsPanel.Controls.Add(this.plC_RJ_Button_EPD266_刷新電子紙);
            this.plC_RJ_GroupBox2.ContentsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plC_RJ_GroupBox2.ContentsPanel.ForeColor = System.Drawing.Color.White;
            this.plC_RJ_GroupBox2.ContentsPanel.IsSelected = false;
            this.plC_RJ_GroupBox2.ContentsPanel.Location = new System.Drawing.Point(0, 45);
            this.plC_RJ_GroupBox2.ContentsPanel.Margin = new System.Windows.Forms.Padding(11, 12, 11, 12);
            this.plC_RJ_GroupBox2.ContentsPanel.Name = "ContentsPanel";
            this.plC_RJ_GroupBox2.ContentsPanel.Size = new System.Drawing.Size(678, 460);
            this.plC_RJ_GroupBox2.ContentsPanel.TabIndex = 2;
            this.plC_RJ_GroupBox2.Location = new System.Drawing.Point(200, 164);
            this.plC_RJ_GroupBox2.Margin = new System.Windows.Forms.Padding(6);
            this.plC_RJ_GroupBox2.Name = "plC_RJ_GroupBox2";
            this.plC_RJ_GroupBox2.PannelBackColor = System.Drawing.Color.White;
            this.plC_RJ_GroupBox2.PannelBorderColor = System.Drawing.Color.SkyBlue;
            this.plC_RJ_GroupBox2.PannelBorderRadius = 5;
            this.plC_RJ_GroupBox2.PannelBorderSize = 2;
            this.plC_RJ_GroupBox2.Size = new System.Drawing.Size(678, 505);
            this.plC_RJ_GroupBox2.TabIndex = 42;
            this.plC_RJ_GroupBox2.TitleBackColor = System.Drawing.Color.DeepSkyBlue;
            this.plC_RJ_GroupBox2.TitleBorderColor = System.Drawing.Color.PaleVioletRed;
            this.plC_RJ_GroupBox2.TitleBorderRadius = 5;
            this.plC_RJ_GroupBox2.TitleBorderSize = 0;
            this.plC_RJ_GroupBox2.TitleFont = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.plC_RJ_GroupBox2.TitleForeColor = System.Drawing.Color.White;
            this.plC_RJ_GroupBox2.TitleHeight = 45;
            this.plC_RJ_GroupBox2.TitleTextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.plC_RJ_GroupBox2.TitleTexts = "EPD266 [Port:29000,30000]";
            // 
            // rJ_TextBox_EPD266_ServerIP
            // 
            this.rJ_TextBox_EPD266_ServerIP.BackColor = System.Drawing.SystemColors.Window;
            this.rJ_TextBox_EPD266_ServerIP.BorderColor = System.Drawing.Color.MediumSlateBlue;
            this.rJ_TextBox_EPD266_ServerIP.BorderFocusColor = System.Drawing.Color.HotPink;
            this.rJ_TextBox_EPD266_ServerIP.BorderRadius = 0;
            this.rJ_TextBox_EPD266_ServerIP.BorderSize = 2;
            this.rJ_TextBox_EPD266_ServerIP.Font = new System.Drawing.Font("新細明體", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_TextBox_EPD266_ServerIP.ForeColor = System.Drawing.Color.DimGray;
            this.rJ_TextBox_EPD266_ServerIP.Location = new System.Drawing.Point(177, 299);
            this.rJ_TextBox_EPD266_ServerIP.Multiline = false;
            this.rJ_TextBox_EPD266_ServerIP.Name = "rJ_TextBox_EPD266_ServerIP";
            this.rJ_TextBox_EPD266_ServerIP.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.rJ_TextBox_EPD266_ServerIP.PassWordChar = false;
            this.rJ_TextBox_EPD266_ServerIP.PlaceholderColor = System.Drawing.Color.DarkGray;
            this.rJ_TextBox_EPD266_ServerIP.PlaceholderText = "";
            this.rJ_TextBox_EPD266_ServerIP.ShowTouchPannel = false;
            this.rJ_TextBox_EPD266_ServerIP.Size = new System.Drawing.Size(250, 40);
            this.rJ_TextBox_EPD266_ServerIP.TabIndex = 50;
            this.rJ_TextBox_EPD266_ServerIP.TextAlgin = System.Windows.Forms.HorizontalAlignment.Left;
            this.rJ_TextBox_EPD266_ServerIP.Texts = "";
            this.rJ_TextBox_EPD266_ServerIP.UnderlineStyle = false;
            // 
            // rJ_Lable2
            // 
            this.rJ_Lable2.BackColor = System.Drawing.Color.RoyalBlue;
            this.rJ_Lable2.BackgroundColor = System.Drawing.Color.RoyalBlue;
            this.rJ_Lable2.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.rJ_Lable2.BorderRadius = 8;
            this.rJ_Lable2.BorderSize = 0;
            this.rJ_Lable2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Lable2.Font = new System.Drawing.Font("新細明體", 16F);
            this.rJ_Lable2.ForeColor = System.Drawing.Color.White;
            this.rJ_Lable2.Location = new System.Drawing.Point(39, 294);
            this.rJ_Lable2.Name = "rJ_Lable2";
            this.rJ_Lable2.Size = new System.Drawing.Size(123, 51);
            this.rJ_Lable2.TabIndex = 49;
            this.rJ_Lable2.Text = "Server IP";
            this.rJ_Lable2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rJ_Lable2.TextColor = System.Drawing.Color.White;
            // 
            // rJ_TextBox_EPD266_Password
            // 
            this.rJ_TextBox_EPD266_Password.BackColor = System.Drawing.SystemColors.Window;
            this.rJ_TextBox_EPD266_Password.BorderColor = System.Drawing.Color.MediumSlateBlue;
            this.rJ_TextBox_EPD266_Password.BorderFocusColor = System.Drawing.Color.HotPink;
            this.rJ_TextBox_EPD266_Password.BorderRadius = 0;
            this.rJ_TextBox_EPD266_Password.BorderSize = 2;
            this.rJ_TextBox_EPD266_Password.Font = new System.Drawing.Font("新細明體", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_TextBox_EPD266_Password.ForeColor = System.Drawing.Color.DimGray;
            this.rJ_TextBox_EPD266_Password.Location = new System.Drawing.Point(177, 238);
            this.rJ_TextBox_EPD266_Password.Multiline = false;
            this.rJ_TextBox_EPD266_Password.Name = "rJ_TextBox_EPD266_Password";
            this.rJ_TextBox_EPD266_Password.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.rJ_TextBox_EPD266_Password.PassWordChar = false;
            this.rJ_TextBox_EPD266_Password.PlaceholderColor = System.Drawing.Color.DarkGray;
            this.rJ_TextBox_EPD266_Password.PlaceholderText = "";
            this.rJ_TextBox_EPD266_Password.ShowTouchPannel = false;
            this.rJ_TextBox_EPD266_Password.Size = new System.Drawing.Size(250, 40);
            this.rJ_TextBox_EPD266_Password.TabIndex = 48;
            this.rJ_TextBox_EPD266_Password.TextAlgin = System.Windows.Forms.HorizontalAlignment.Left;
            this.rJ_TextBox_EPD266_Password.Texts = "";
            this.rJ_TextBox_EPD266_Password.UnderlineStyle = false;
            // 
            // rJ_Lable
            // 
            this.rJ_Lable.BackColor = System.Drawing.Color.RoyalBlue;
            this.rJ_Lable.BackgroundColor = System.Drawing.Color.RoyalBlue;
            this.rJ_Lable.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.rJ_Lable.BorderRadius = 8;
            this.rJ_Lable.BorderSize = 0;
            this.rJ_Lable.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Lable.Font = new System.Drawing.Font("新細明體", 16F);
            this.rJ_Lable.ForeColor = System.Drawing.Color.White;
            this.rJ_Lable.Location = new System.Drawing.Point(39, 233);
            this.rJ_Lable.Name = "rJ_Lable";
            this.rJ_Lable.Size = new System.Drawing.Size(123, 51);
            this.rJ_Lable.TabIndex = 47;
            this.rJ_Lable.Text = "Password";
            this.rJ_Lable.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rJ_Lable.TextColor = System.Drawing.Color.White;
            // 
            // rJ_TextBox_EPD266_SSID
            // 
            this.rJ_TextBox_EPD266_SSID.BackColor = System.Drawing.SystemColors.Window;
            this.rJ_TextBox_EPD266_SSID.BorderColor = System.Drawing.Color.MediumSlateBlue;
            this.rJ_TextBox_EPD266_SSID.BorderFocusColor = System.Drawing.Color.HotPink;
            this.rJ_TextBox_EPD266_SSID.BorderRadius = 0;
            this.rJ_TextBox_EPD266_SSID.BorderSize = 2;
            this.rJ_TextBox_EPD266_SSID.Font = new System.Drawing.Font("新細明體", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_TextBox_EPD266_SSID.ForeColor = System.Drawing.Color.DimGray;
            this.rJ_TextBox_EPD266_SSID.Location = new System.Drawing.Point(177, 176);
            this.rJ_TextBox_EPD266_SSID.Multiline = false;
            this.rJ_TextBox_EPD266_SSID.Name = "rJ_TextBox_EPD266_SSID";
            this.rJ_TextBox_EPD266_SSID.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.rJ_TextBox_EPD266_SSID.PassWordChar = false;
            this.rJ_TextBox_EPD266_SSID.PlaceholderColor = System.Drawing.Color.DarkGray;
            this.rJ_TextBox_EPD266_SSID.PlaceholderText = "";
            this.rJ_TextBox_EPD266_SSID.ShowTouchPannel = false;
            this.rJ_TextBox_EPD266_SSID.Size = new System.Drawing.Size(250, 40);
            this.rJ_TextBox_EPD266_SSID.TabIndex = 46;
            this.rJ_TextBox_EPD266_SSID.TextAlgin = System.Windows.Forms.HorizontalAlignment.Left;
            this.rJ_TextBox_EPD266_SSID.Texts = "";
            this.rJ_TextBox_EPD266_SSID.UnderlineStyle = false;
            // 
            // rJ_Lable_EPD266_SSID
            // 
            this.rJ_Lable_EPD266_SSID.BackColor = System.Drawing.Color.RoyalBlue;
            this.rJ_Lable_EPD266_SSID.BackgroundColor = System.Drawing.Color.RoyalBlue;
            this.rJ_Lable_EPD266_SSID.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.rJ_Lable_EPD266_SSID.BorderRadius = 8;
            this.rJ_Lable_EPD266_SSID.BorderSize = 0;
            this.rJ_Lable_EPD266_SSID.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Lable_EPD266_SSID.Font = new System.Drawing.Font("新細明體", 16F);
            this.rJ_Lable_EPD266_SSID.ForeColor = System.Drawing.Color.White;
            this.rJ_Lable_EPD266_SSID.Location = new System.Drawing.Point(39, 171);
            this.rJ_Lable_EPD266_SSID.Name = "rJ_Lable_EPD266_SSID";
            this.rJ_Lable_EPD266_SSID.Size = new System.Drawing.Size(123, 51);
            this.rJ_Lable_EPD266_SSID.TabIndex = 45;
            this.rJ_Lable_EPD266_SSID.Text = "SSID";
            this.rJ_Lable_EPD266_SSID.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rJ_Lable_EPD266_SSID.TextColor = System.Drawing.Color.White;
            // 
            // rJ_TextBox_EPD266_IP
            // 
            this.rJ_TextBox_EPD266_IP.BackColor = System.Drawing.SystemColors.Window;
            this.rJ_TextBox_EPD266_IP.BorderColor = System.Drawing.Color.MediumSlateBlue;
            this.rJ_TextBox_EPD266_IP.BorderFocusColor = System.Drawing.Color.HotPink;
            this.rJ_TextBox_EPD266_IP.BorderRadius = 0;
            this.rJ_TextBox_EPD266_IP.BorderSize = 2;
            this.rJ_TextBox_EPD266_IP.Font = new System.Drawing.Font("新細明體", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_TextBox_EPD266_IP.ForeColor = System.Drawing.Color.DimGray;
            this.rJ_TextBox_EPD266_IP.Location = new System.Drawing.Point(177, 116);
            this.rJ_TextBox_EPD266_IP.Multiline = false;
            this.rJ_TextBox_EPD266_IP.Name = "rJ_TextBox_EPD266_IP";
            this.rJ_TextBox_EPD266_IP.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.rJ_TextBox_EPD266_IP.PassWordChar = false;
            this.rJ_TextBox_EPD266_IP.PlaceholderColor = System.Drawing.Color.DarkGray;
            this.rJ_TextBox_EPD266_IP.PlaceholderText = "";
            this.rJ_TextBox_EPD266_IP.ShowTouchPannel = false;
            this.rJ_TextBox_EPD266_IP.Size = new System.Drawing.Size(250, 40);
            this.rJ_TextBox_EPD266_IP.TabIndex = 44;
            this.rJ_TextBox_EPD266_IP.TextAlgin = System.Windows.Forms.HorizontalAlignment.Left;
            this.rJ_TextBox_EPD266_IP.Texts = "";
            this.rJ_TextBox_EPD266_IP.UnderlineStyle = false;
            // 
            // rJ_Lable1
            // 
            this.rJ_Lable1.BackColor = System.Drawing.Color.RoyalBlue;
            this.rJ_Lable1.BackgroundColor = System.Drawing.Color.RoyalBlue;
            this.rJ_Lable1.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.rJ_Lable1.BorderRadius = 8;
            this.rJ_Lable1.BorderSize = 0;
            this.rJ_Lable1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Lable1.Font = new System.Drawing.Font("新細明體", 16F);
            this.rJ_Lable1.ForeColor = System.Drawing.Color.White;
            this.rJ_Lable1.Location = new System.Drawing.Point(39, 111);
            this.rJ_Lable1.Name = "rJ_Lable1";
            this.rJ_Lable1.Size = new System.Drawing.Size(123, 51);
            this.rJ_Lable1.TabIndex = 43;
            this.rJ_Lable1.Text = "IP";
            this.rJ_Lable1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rJ_Lable1.TextColor = System.Drawing.Color.White;
            // 
            // plC_RJ_Button_EPD266_OTA
            // 
            this.plC_RJ_Button_EPD266_OTA.AutoResetState = false;
            this.plC_RJ_Button_EPD266_OTA.BackgroundColor = System.Drawing.Color.DodgerBlue;
            this.plC_RJ_Button_EPD266_OTA.Bool = false;
            this.plC_RJ_Button_EPD266_OTA.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.plC_RJ_Button_EPD266_OTA.BorderRadius = 5;
            this.plC_RJ_Button_EPD266_OTA.BorderSize = 0;
            this.plC_RJ_Button_EPD266_OTA.but_press = false;
            this.plC_RJ_Button_EPD266_OTA.buttonType = MyUI.RJ_Button.ButtonType.Toggle;
            this.plC_RJ_Button_EPD266_OTA.FlatAppearance.BorderSize = 0;
            this.plC_RJ_Button_EPD266_OTA.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.plC_RJ_Button_EPD266_OTA.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold);
            this.plC_RJ_Button_EPD266_OTA.Icon = System.Windows.Forms.MessageBoxIcon.Warning;
            this.plC_RJ_Button_EPD266_OTA.Location = new System.Drawing.Point(165, 370);
            this.plC_RJ_Button_EPD266_OTA.Margin = new System.Windows.Forms.Padding(6);
            this.plC_RJ_Button_EPD266_OTA.Name = "plC_RJ_Button_EPD266_OTA";
            this.plC_RJ_Button_EPD266_OTA.OFF_文字內容 = "OTA";
            this.plC_RJ_Button_EPD266_OTA.OFF_文字字體 = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold);
            this.plC_RJ_Button_EPD266_OTA.OFF_文字顏色 = System.Drawing.Color.White;
            this.plC_RJ_Button_EPD266_OTA.OFF_背景顏色 = System.Drawing.SystemColors.Control;
            this.plC_RJ_Button_EPD266_OTA.ON_BorderSize = 5;
            this.plC_RJ_Button_EPD266_OTA.ON_文字內容 = "OTA";
            this.plC_RJ_Button_EPD266_OTA.ON_文字字體 = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold);
            this.plC_RJ_Button_EPD266_OTA.ON_文字顏色 = System.Drawing.Color.Black;
            this.plC_RJ_Button_EPD266_OTA.ON_背景顏色 = System.Drawing.SystemColors.Control;
            this.plC_RJ_Button_EPD266_OTA.Size = new System.Drawing.Size(143, 72);
            this.plC_RJ_Button_EPD266_OTA.State = false;
            this.plC_RJ_Button_EPD266_OTA.TabIndex = 42;
            this.plC_RJ_Button_EPD266_OTA.Text = "OTA";
            this.plC_RJ_Button_EPD266_OTA.TextColor = System.Drawing.Color.White;
            this.plC_RJ_Button_EPD266_OTA.Texts = "OTA";
            this.plC_RJ_Button_EPD266_OTA.UseVisualStyleBackColor = false;
            this.plC_RJ_Button_EPD266_OTA.字型鎖住 = false;
            this.plC_RJ_Button_EPD266_OTA.按鈕型態 = MyUI.PLC_RJ_Button.StatusEnum.保持型;
            this.plC_RJ_Button_EPD266_OTA.按鍵方式 = MyUI.PLC_RJ_Button.PressEnum.Mouse_左鍵;
            this.plC_RJ_Button_EPD266_OTA.文字鎖住 = false;
            this.plC_RJ_Button_EPD266_OTA.讀取位元反向 = false;
            this.plC_RJ_Button_EPD266_OTA.讀寫鎖住 = false;
            this.plC_RJ_Button_EPD266_OTA.音效 = true;
            this.plC_RJ_Button_EPD266_OTA.顯示 = false;
            this.plC_RJ_Button_EPD266_OTA.顯示狀態 = false;
            // 
            // plC_RJ_Button_EPD266_寫入參數
            // 
            this.plC_RJ_Button_EPD266_寫入參數.AutoResetState = false;
            this.plC_RJ_Button_EPD266_寫入參數.BackgroundColor = System.Drawing.Color.DodgerBlue;
            this.plC_RJ_Button_EPD266_寫入參數.Bool = false;
            this.plC_RJ_Button_EPD266_寫入參數.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.plC_RJ_Button_EPD266_寫入參數.BorderRadius = 5;
            this.plC_RJ_Button_EPD266_寫入參數.BorderSize = 0;
            this.plC_RJ_Button_EPD266_寫入參數.but_press = false;
            this.plC_RJ_Button_EPD266_寫入參數.buttonType = MyUI.RJ_Button.ButtonType.Toggle;
            this.plC_RJ_Button_EPD266_寫入參數.FlatAppearance.BorderSize = 0;
            this.plC_RJ_Button_EPD266_寫入參數.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.plC_RJ_Button_EPD266_寫入參數.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold);
            this.plC_RJ_Button_EPD266_寫入參數.Icon = System.Windows.Forms.MessageBoxIcon.Warning;
            this.plC_RJ_Button_EPD266_寫入參數.Location = new System.Drawing.Point(320, 370);
            this.plC_RJ_Button_EPD266_寫入參數.Margin = new System.Windows.Forms.Padding(6);
            this.plC_RJ_Button_EPD266_寫入參數.Name = "plC_RJ_Button_EPD266_寫入參數";
            this.plC_RJ_Button_EPD266_寫入參數.OFF_文字內容 = "寫入參數";
            this.plC_RJ_Button_EPD266_寫入參數.OFF_文字字體 = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold);
            this.plC_RJ_Button_EPD266_寫入參數.OFF_文字顏色 = System.Drawing.Color.White;
            this.plC_RJ_Button_EPD266_寫入參數.OFF_背景顏色 = System.Drawing.SystemColors.Control;
            this.plC_RJ_Button_EPD266_寫入參數.ON_BorderSize = 5;
            this.plC_RJ_Button_EPD266_寫入參數.ON_文字內容 = "寫入參數";
            this.plC_RJ_Button_EPD266_寫入參數.ON_文字字體 = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold);
            this.plC_RJ_Button_EPD266_寫入參數.ON_文字顏色 = System.Drawing.Color.Black;
            this.plC_RJ_Button_EPD266_寫入參數.ON_背景顏色 = System.Drawing.SystemColors.Control;
            this.plC_RJ_Button_EPD266_寫入參數.Size = new System.Drawing.Size(143, 72);
            this.plC_RJ_Button_EPD266_寫入參數.State = false;
            this.plC_RJ_Button_EPD266_寫入參數.TabIndex = 41;
            this.plC_RJ_Button_EPD266_寫入參數.Text = "寫入參數";
            this.plC_RJ_Button_EPD266_寫入參數.TextColor = System.Drawing.Color.White;
            this.plC_RJ_Button_EPD266_寫入參數.Texts = "寫入參數";
            this.plC_RJ_Button_EPD266_寫入參數.UseVisualStyleBackColor = false;
            this.plC_RJ_Button_EPD266_寫入參數.字型鎖住 = false;
            this.plC_RJ_Button_EPD266_寫入參數.按鈕型態 = MyUI.PLC_RJ_Button.StatusEnum.保持型;
            this.plC_RJ_Button_EPD266_寫入參數.按鍵方式 = MyUI.PLC_RJ_Button.PressEnum.Mouse_左鍵;
            this.plC_RJ_Button_EPD266_寫入參數.文字鎖住 = false;
            this.plC_RJ_Button_EPD266_寫入參數.讀取位元反向 = false;
            this.plC_RJ_Button_EPD266_寫入參數.讀寫鎖住 = false;
            this.plC_RJ_Button_EPD266_寫入參數.音效 = true;
            this.plC_RJ_Button_EPD266_寫入參數.顯示 = false;
            this.plC_RJ_Button_EPD266_寫入參數.顯示狀態 = false;
            // 
            // plC_RJ_Button_EPD266_亮燈
            // 
            this.plC_RJ_Button_EPD266_亮燈.AutoResetState = false;
            this.plC_RJ_Button_EPD266_亮燈.BackgroundColor = System.Drawing.Color.DodgerBlue;
            this.plC_RJ_Button_EPD266_亮燈.Bool = false;
            this.plC_RJ_Button_EPD266_亮燈.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.plC_RJ_Button_EPD266_亮燈.BorderRadius = 5;
            this.plC_RJ_Button_EPD266_亮燈.BorderSize = 0;
            this.plC_RJ_Button_EPD266_亮燈.but_press = false;
            this.plC_RJ_Button_EPD266_亮燈.buttonType = MyUI.RJ_Button.ButtonType.Toggle;
            this.plC_RJ_Button_EPD266_亮燈.FlatAppearance.BorderSize = 0;
            this.plC_RJ_Button_EPD266_亮燈.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.plC_RJ_Button_EPD266_亮燈.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold);
            this.plC_RJ_Button_EPD266_亮燈.Icon = System.Windows.Forms.MessageBoxIcon.Warning;
            this.plC_RJ_Button_EPD266_亮燈.Location = new System.Drawing.Point(19, 10);
            this.plC_RJ_Button_EPD266_亮燈.Margin = new System.Windows.Forms.Padding(6);
            this.plC_RJ_Button_EPD266_亮燈.Name = "plC_RJ_Button_EPD266_亮燈";
            this.plC_RJ_Button_EPD266_亮燈.OFF_文字內容 = "亮燈";
            this.plC_RJ_Button_EPD266_亮燈.OFF_文字字體 = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold);
            this.plC_RJ_Button_EPD266_亮燈.OFF_文字顏色 = System.Drawing.Color.White;
            this.plC_RJ_Button_EPD266_亮燈.OFF_背景顏色 = System.Drawing.SystemColors.Control;
            this.plC_RJ_Button_EPD266_亮燈.ON_BorderSize = 5;
            this.plC_RJ_Button_EPD266_亮燈.ON_文字內容 = "亮燈";
            this.plC_RJ_Button_EPD266_亮燈.ON_文字字體 = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold);
            this.plC_RJ_Button_EPD266_亮燈.ON_文字顏色 = System.Drawing.Color.Black;
            this.plC_RJ_Button_EPD266_亮燈.ON_背景顏色 = System.Drawing.SystemColors.Control;
            this.plC_RJ_Button_EPD266_亮燈.Size = new System.Drawing.Size(143, 72);
            this.plC_RJ_Button_EPD266_亮燈.State = false;
            this.plC_RJ_Button_EPD266_亮燈.TabIndex = 38;
            this.plC_RJ_Button_EPD266_亮燈.Text = "亮燈";
            this.plC_RJ_Button_EPD266_亮燈.TextColor = System.Drawing.Color.White;
            this.plC_RJ_Button_EPD266_亮燈.Texts = "亮燈";
            this.plC_RJ_Button_EPD266_亮燈.UseVisualStyleBackColor = false;
            this.plC_RJ_Button_EPD266_亮燈.字型鎖住 = false;
            this.plC_RJ_Button_EPD266_亮燈.按鈕型態 = MyUI.PLC_RJ_Button.StatusEnum.保持型;
            this.plC_RJ_Button_EPD266_亮燈.按鍵方式 = MyUI.PLC_RJ_Button.PressEnum.Mouse_左鍵;
            this.plC_RJ_Button_EPD266_亮燈.文字鎖住 = false;
            this.plC_RJ_Button_EPD266_亮燈.讀取位元反向 = false;
            this.plC_RJ_Button_EPD266_亮燈.讀寫鎖住 = false;
            this.plC_RJ_Button_EPD266_亮燈.音效 = true;
            this.plC_RJ_Button_EPD266_亮燈.顯示 = false;
            this.plC_RJ_Button_EPD266_亮燈.顯示狀態 = false;
            // 
            // plC_RJ_Button_EPD266_測試一次
            // 
            this.plC_RJ_Button_EPD266_測試一次.AutoResetState = false;
            this.plC_RJ_Button_EPD266_測試一次.BackgroundColor = System.Drawing.Color.DodgerBlue;
            this.plC_RJ_Button_EPD266_測試一次.Bool = false;
            this.plC_RJ_Button_EPD266_測試一次.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.plC_RJ_Button_EPD266_測試一次.BorderRadius = 5;
            this.plC_RJ_Button_EPD266_測試一次.BorderSize = 0;
            this.plC_RJ_Button_EPD266_測試一次.but_press = false;
            this.plC_RJ_Button_EPD266_測試一次.buttonType = MyUI.RJ_Button.ButtonType.Toggle;
            this.plC_RJ_Button_EPD266_測試一次.FlatAppearance.BorderSize = 0;
            this.plC_RJ_Button_EPD266_測試一次.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.plC_RJ_Button_EPD266_測試一次.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold);
            this.plC_RJ_Button_EPD266_測試一次.Icon = System.Windows.Forms.MessageBoxIcon.Warning;
            this.plC_RJ_Button_EPD266_測試一次.Location = new System.Drawing.Point(320, 10);
            this.plC_RJ_Button_EPD266_測試一次.Margin = new System.Windows.Forms.Padding(6);
            this.plC_RJ_Button_EPD266_測試一次.Name = "plC_RJ_Button_EPD266_測試一次";
            this.plC_RJ_Button_EPD266_測試一次.OFF_文字內容 = "測試一次";
            this.plC_RJ_Button_EPD266_測試一次.OFF_文字字體 = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold);
            this.plC_RJ_Button_EPD266_測試一次.OFF_文字顏色 = System.Drawing.Color.White;
            this.plC_RJ_Button_EPD266_測試一次.OFF_背景顏色 = System.Drawing.SystemColors.Control;
            this.plC_RJ_Button_EPD266_測試一次.ON_BorderSize = 5;
            this.plC_RJ_Button_EPD266_測試一次.ON_文字內容 = "測試一次";
            this.plC_RJ_Button_EPD266_測試一次.ON_文字字體 = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold);
            this.plC_RJ_Button_EPD266_測試一次.ON_文字顏色 = System.Drawing.Color.Black;
            this.plC_RJ_Button_EPD266_測試一次.ON_背景顏色 = System.Drawing.SystemColors.Control;
            this.plC_RJ_Button_EPD266_測試一次.Size = new System.Drawing.Size(143, 72);
            this.plC_RJ_Button_EPD266_測試一次.State = false;
            this.plC_RJ_Button_EPD266_測試一次.TabIndex = 40;
            this.plC_RJ_Button_EPD266_測試一次.Text = "測試一次";
            this.plC_RJ_Button_EPD266_測試一次.TextColor = System.Drawing.Color.White;
            this.plC_RJ_Button_EPD266_測試一次.Texts = "測試一次";
            this.plC_RJ_Button_EPD266_測試一次.UseVisualStyleBackColor = false;
            this.plC_RJ_Button_EPD266_測試一次.字型鎖住 = false;
            this.plC_RJ_Button_EPD266_測試一次.按鈕型態 = MyUI.PLC_RJ_Button.StatusEnum.保持型;
            this.plC_RJ_Button_EPD266_測試一次.按鍵方式 = MyUI.PLC_RJ_Button.PressEnum.Mouse_左鍵;
            this.plC_RJ_Button_EPD266_測試一次.文字鎖住 = false;
            this.plC_RJ_Button_EPD266_測試一次.讀取位元反向 = false;
            this.plC_RJ_Button_EPD266_測試一次.讀寫鎖住 = false;
            this.plC_RJ_Button_EPD266_測試一次.音效 = true;
            this.plC_RJ_Button_EPD266_測試一次.顯示 = false;
            this.plC_RJ_Button_EPD266_測試一次.顯示狀態 = false;
            // 
            // plC_RJ_Button_EPD266_刷新電子紙
            // 
            this.plC_RJ_Button_EPD266_刷新電子紙.AutoResetState = false;
            this.plC_RJ_Button_EPD266_刷新電子紙.BackgroundColor = System.Drawing.Color.DodgerBlue;
            this.plC_RJ_Button_EPD266_刷新電子紙.Bool = false;
            this.plC_RJ_Button_EPD266_刷新電子紙.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.plC_RJ_Button_EPD266_刷新電子紙.BorderRadius = 5;
            this.plC_RJ_Button_EPD266_刷新電子紙.BorderSize = 0;
            this.plC_RJ_Button_EPD266_刷新電子紙.but_press = false;
            this.plC_RJ_Button_EPD266_刷新電子紙.buttonType = MyUI.RJ_Button.ButtonType.Toggle;
            this.plC_RJ_Button_EPD266_刷新電子紙.FlatAppearance.BorderSize = 0;
            this.plC_RJ_Button_EPD266_刷新電子紙.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.plC_RJ_Button_EPD266_刷新電子紙.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold);
            this.plC_RJ_Button_EPD266_刷新電子紙.Icon = System.Windows.Forms.MessageBoxIcon.Warning;
            this.plC_RJ_Button_EPD266_刷新電子紙.Location = new System.Drawing.Point(165, 10);
            this.plC_RJ_Button_EPD266_刷新電子紙.Margin = new System.Windows.Forms.Padding(6);
            this.plC_RJ_Button_EPD266_刷新電子紙.Name = "plC_RJ_Button_EPD266_刷新電子紙";
            this.plC_RJ_Button_EPD266_刷新電子紙.OFF_文字內容 = "刷新電子紙";
            this.plC_RJ_Button_EPD266_刷新電子紙.OFF_文字字體 = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold);
            this.plC_RJ_Button_EPD266_刷新電子紙.OFF_文字顏色 = System.Drawing.Color.White;
            this.plC_RJ_Button_EPD266_刷新電子紙.OFF_背景顏色 = System.Drawing.SystemColors.Control;
            this.plC_RJ_Button_EPD266_刷新電子紙.ON_BorderSize = 5;
            this.plC_RJ_Button_EPD266_刷新電子紙.ON_文字內容 = "刷新電子紙";
            this.plC_RJ_Button_EPD266_刷新電子紙.ON_文字字體 = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold);
            this.plC_RJ_Button_EPD266_刷新電子紙.ON_文字顏色 = System.Drawing.Color.Black;
            this.plC_RJ_Button_EPD266_刷新電子紙.ON_背景顏色 = System.Drawing.SystemColors.Control;
            this.plC_RJ_Button_EPD266_刷新電子紙.Size = new System.Drawing.Size(143, 72);
            this.plC_RJ_Button_EPD266_刷新電子紙.State = false;
            this.plC_RJ_Button_EPD266_刷新電子紙.TabIndex = 39;
            this.plC_RJ_Button_EPD266_刷新電子紙.Text = "刷新電子紙";
            this.plC_RJ_Button_EPD266_刷新電子紙.TextColor = System.Drawing.Color.White;
            this.plC_RJ_Button_EPD266_刷新電子紙.Texts = "刷新電子紙";
            this.plC_RJ_Button_EPD266_刷新電子紙.UseVisualStyleBackColor = false;
            this.plC_RJ_Button_EPD266_刷新電子紙.字型鎖住 = false;
            this.plC_RJ_Button_EPD266_刷新電子紙.按鈕型態 = MyUI.PLC_RJ_Button.StatusEnum.保持型;
            this.plC_RJ_Button_EPD266_刷新電子紙.按鍵方式 = MyUI.PLC_RJ_Button.PressEnum.Mouse_左鍵;
            this.plC_RJ_Button_EPD266_刷新電子紙.文字鎖住 = false;
            this.plC_RJ_Button_EPD266_刷新電子紙.讀取位元反向 = false;
            this.plC_RJ_Button_EPD266_刷新電子紙.讀寫鎖住 = false;
            this.plC_RJ_Button_EPD266_刷新電子紙.音效 = true;
            this.plC_RJ_Button_EPD266_刷新電子紙.顯示 = false;
            this.plC_RJ_Button_EPD266_刷新電子紙.顯示狀態 = false;
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
            this.plC_RJ_Button_EPD290_亮燈.Margin = new System.Windows.Forms.Padding(6);
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
            this.plC_RJ_Button_EPD290_測試一次.Margin = new System.Windows.Forms.Padding(6);
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
            this.plC_RJ_Button_EPD290_刷新電子紙.Margin = new System.Windows.Forms.Padding(6);
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
            this.comboBox_PortName.Font = new System.Drawing.Font("新細明體", 20F);
            this.comboBox_PortName.FormattingEnabled = true;
            this.comboBox_PortName.Location = new System.Drawing.Point(18, 27);
            this.comboBox_PortName.Name = "comboBox_PortName";
            this.comboBox_PortName.Size = new System.Drawing.Size(121, 35);
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
            this.tabControl2.Controls.Add(this.EPD266);
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
            this.storageUI_EPD_290._Password = "";
            this.storageUI_EPD_290.DataBaseName = "TEST";
            this.storageUI_EPD_290.DNS = "0.0.0.0";
            this.storageUI_EPD_290.Dock = System.Windows.Forms.DockStyle.Fill;
            this.storageUI_EPD_290.Gateway = "0.0.0.0";
            this.storageUI_EPD_290.IP = "localhost";
            this.storageUI_EPD_290.IP_Adress = "0.0.0.0";
            this.storageUI_EPD_290.Local_Port = "0";
            this.storageUI_EPD_290.Location = new System.Drawing.Point(3, 3);
            this.storageUI_EPD_290.Name = "storageUI_EPD_290";
            this.storageUI_EPD_290.Password = "user82822040";
            this.storageUI_EPD_290.Port = ((uint)(3306u));
            this.storageUI_EPD_290.Server_IP_Adress = "0.0.0.0";
            this.storageUI_EPD_290.Server_Port = "0";
            this.storageUI_EPD_290.Size = new System.Drawing.Size(1411, 857);
            this.storageUI_EPD_290.SSID = "";
            this.storageUI_EPD_290.Station = "0";
            this.storageUI_EPD_290.Subnet = "0.0.0.0";
            this.storageUI_EPD_290.TabIndex = 2;
            this.storageUI_EPD_290.TableName = "EPD290_Jsonstring";
            this.storageUI_EPD_290.UDP_LocalPorts = ((System.Collections.Generic.List<string>)(resources.GetObject("storageUI_EPD_290.UDP_LocalPorts")));
            this.storageUI_EPD_290.UDP_SendTime = "0";
            this.storageUI_EPD_290.UDP_ServerPorts = ((System.Collections.Generic.List<string>)(resources.GetObject("storageUI_EPD_290.UDP_ServerPorts")));
            this.storageUI_EPD_290.UserName = "root";
            // 
            // EPD266
            // 
            this.EPD266.Controls.Add(this.storageUI_EPD_266);
            this.EPD266.Location = new System.Drawing.Point(4, 22);
            this.EPD266.Name = "EPD266";
            this.EPD266.Size = new System.Drawing.Size(178, 42);
            this.EPD266.TabIndex = 2;
            this.EPD266.Text = "EPD266";
            this.EPD266.UseVisualStyleBackColor = true;
            // 
            // storageUI_EPD_266
            // 
            this.storageUI_EPD_266._Password = "";
            this.storageUI_EPD_266.DataBaseName = "TEST";
            this.storageUI_EPD_266.DNS = "0.0.0.0";
            this.storageUI_EPD_266.Dock = System.Windows.Forms.DockStyle.Fill;
            this.storageUI_EPD_266.Gateway = "0.0.0.0";
            this.storageUI_EPD_266.IP = "localhost";
            this.storageUI_EPD_266.IP_Adress = "0.0.0.0";
            this.storageUI_EPD_266.Local_Port = "0";
            this.storageUI_EPD_266.Location = new System.Drawing.Point(0, 0);
            this.storageUI_EPD_266.Name = "storageUI_EPD_266";
            this.storageUI_EPD_266.Password = "user82822040";
            this.storageUI_EPD_266.Port = ((uint)(3306u));
            this.storageUI_EPD_266.Server_IP_Adress = "0.0.0.0";
            this.storageUI_EPD_266.Server_Port = "0";
            this.storageUI_EPD_266.Size = new System.Drawing.Size(178, 42);
            this.storageUI_EPD_266.SSID = "";
            this.storageUI_EPD_266.Station = "0";
            this.storageUI_EPD_266.Subnet = "0.0.0.0";
            this.storageUI_EPD_266.TabIndex = 0;
            this.storageUI_EPD_266.TableName = "EPD266_Jsonstring";
            this.storageUI_EPD_266.UDP_LocalPorts = ((System.Collections.Generic.List<string>)(resources.GetObject("storageUI_EPD_266.UDP_LocalPorts")));
            this.storageUI_EPD_266.UDP_SendTime = "0";
            this.storageUI_EPD_266.UDP_ServerPorts = ((System.Collections.Generic.List<string>)(resources.GetObject("storageUI_EPD_266.UDP_ServerPorts")));
            this.storageUI_EPD_266.UserName = "root";
            // 
            // PLC
            // 
            this.PLC.Controls.Add(this.plC_UI_Init);
            this.PLC.Controls.Add(this.lowerMachine_Panel);
            this.PLC.Location = new System.Drawing.Point(4, 22);
            this.PLC.Name = "PLC";
            this.PLC.Size = new System.Drawing.Size(178, 42);
            this.PLC.TabIndex = 1;
            this.PLC.Text = "PLC";
            this.PLC.UseVisualStyleBackColor = true;
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
            // lowerMachine_Panel
            // 
            this.lowerMachine_Panel.Location = new System.Drawing.Point(15, 15);
            this.lowerMachine_Panel.Name = "lowerMachine_Panel";
            this.lowerMachine_Panel.Size = new System.Drawing.Size(869, 565);
            this.lowerMachine_Panel.TabIndex = 0;
            this.lowerMachine_Panel.掃描速度 = 0;
            // 
            // plC_RJ_Button_EPD266_IP加1
            // 
            this.plC_RJ_Button_EPD266_IP加1.AutoResetState = false;
            this.plC_RJ_Button_EPD266_IP加1.BackgroundColor = System.Drawing.Color.DodgerBlue;
            this.plC_RJ_Button_EPD266_IP加1.Bool = false;
            this.plC_RJ_Button_EPD266_IP加1.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.plC_RJ_Button_EPD266_IP加1.BorderRadius = 5;
            this.plC_RJ_Button_EPD266_IP加1.BorderSize = 0;
            this.plC_RJ_Button_EPD266_IP加1.but_press = false;
            this.plC_RJ_Button_EPD266_IP加1.buttonType = MyUI.RJ_Button.ButtonType.Toggle;
            this.plC_RJ_Button_EPD266_IP加1.FlatAppearance.BorderSize = 0;
            this.plC_RJ_Button_EPD266_IP加1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.plC_RJ_Button_EPD266_IP加1.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold);
            this.plC_RJ_Button_EPD266_IP加1.Icon = System.Windows.Forms.MessageBoxIcon.Warning;
            this.plC_RJ_Button_EPD266_IP加1.Location = new System.Drawing.Point(436, 116);
            this.plC_RJ_Button_EPD266_IP加1.Margin = new System.Windows.Forms.Padding(6);
            this.plC_RJ_Button_EPD266_IP加1.Name = "plC_RJ_Button_EPD266_IP加1";
            this.plC_RJ_Button_EPD266_IP加1.OFF_文字內容 = "+1";
            this.plC_RJ_Button_EPD266_IP加1.OFF_文字字體 = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold);
            this.plC_RJ_Button_EPD266_IP加1.OFF_文字顏色 = System.Drawing.Color.White;
            this.plC_RJ_Button_EPD266_IP加1.OFF_背景顏色 = System.Drawing.SystemColors.Control;
            this.plC_RJ_Button_EPD266_IP加1.ON_BorderSize = 5;
            this.plC_RJ_Button_EPD266_IP加1.ON_文字內容 = "+1";
            this.plC_RJ_Button_EPD266_IP加1.ON_文字字體 = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold);
            this.plC_RJ_Button_EPD266_IP加1.ON_文字顏色 = System.Drawing.Color.Black;
            this.plC_RJ_Button_EPD266_IP加1.ON_背景顏色 = System.Drawing.SystemColors.Control;
            this.plC_RJ_Button_EPD266_IP加1.Size = new System.Drawing.Size(78, 40);
            this.plC_RJ_Button_EPD266_IP加1.State = false;
            this.plC_RJ_Button_EPD266_IP加1.TabIndex = 51;
            this.plC_RJ_Button_EPD266_IP加1.Text = "+1";
            this.plC_RJ_Button_EPD266_IP加1.TextColor = System.Drawing.Color.White;
            this.plC_RJ_Button_EPD266_IP加1.Texts = "+1";
            this.plC_RJ_Button_EPD266_IP加1.UseVisualStyleBackColor = false;
            this.plC_RJ_Button_EPD266_IP加1.字型鎖住 = false;
            this.plC_RJ_Button_EPD266_IP加1.按鈕型態 = MyUI.PLC_RJ_Button.StatusEnum.保持型;
            this.plC_RJ_Button_EPD266_IP加1.按鍵方式 = MyUI.PLC_RJ_Button.PressEnum.Mouse_左鍵;
            this.plC_RJ_Button_EPD266_IP加1.文字鎖住 = false;
            this.plC_RJ_Button_EPD266_IP加1.讀取位元反向 = false;
            this.plC_RJ_Button_EPD266_IP加1.讀寫鎖住 = false;
            this.plC_RJ_Button_EPD266_IP加1.音效 = true;
            this.plC_RJ_Button_EPD266_IP加1.顯示 = false;
            this.plC_RJ_Button_EPD266_IP加1.顯示狀態 = false;
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
            this.plC_RJ_GroupBox2.ContentsPanel.ResumeLayout(false);
            this.plC_RJ_GroupBox2.ResumeLayout(false);
            this.plC_RJ_GroupBox1.ContentsPanel.ResumeLayout(false);
            this.plC_RJ_GroupBox1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.系統.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.EPD290.ResumeLayout(false);
            this.EPD266.ResumeLayout(false);
            this.PLC.ResumeLayout(false);
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
        private System.Windows.Forms.TabPage EPD266;
        private H_Pannel_lib.StorageUI_EPD_266 storageUI_EPD_266;
        private MyUI.PLC_RJ_GroupBox plC_RJ_GroupBox2;
        private MyUI.PLC_RJ_Button plC_RJ_Button_EPD266_寫入參數;
        public MyUI.PLC_RJ_Button plC_RJ_Button_EPD266_亮燈;
        private MyUI.PLC_RJ_Button plC_RJ_Button_EPD266_測試一次;
        private MyUI.PLC_RJ_Button plC_RJ_Button_EPD266_刷新電子紙;
        private MyUI.PLC_RJ_Button plC_RJ_Button_EPD266_OTA;
        private MyUI.RJ_TextBox rJ_TextBox_EPD266_IP;
        private MyUI.RJ_Lable rJ_Lable1;
        private MyUI.RJ_TextBox rJ_TextBox_EPD266_Password;
        private MyUI.RJ_Lable rJ_Lable;
        private MyUI.RJ_TextBox rJ_TextBox_EPD266_SSID;
        private MyUI.RJ_Lable rJ_Lable_EPD266_SSID;
        private MyUI.RJ_TextBox rJ_TextBox_EPD266_ServerIP;
        private MyUI.RJ_Lable rJ_Lable2;
        private MyUI.PLC_RJ_Button plC_RJ_Button_EPD266_IP加1;
    }
}

