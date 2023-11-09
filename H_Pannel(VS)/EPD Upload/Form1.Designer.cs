
namespace EPD_Upload
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
            this.comboBox_PortName = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label_狀態 = new System.Windows.Forms.Label();
            this.rJ_Button_上傳 = new MyUI.RJ_Button();
            this.rJ_Button_連接 = new MyUI.RJ_Button();
            this.panel_EPD290 = new System.Windows.Forms.Panel();
            this.rJ_Button_存檔 = new MyUI.RJ_Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button_面板內容_字體_廠牌 = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.comboBox_面板內容_字體顏色_廠牌 = new System.Windows.Forms.ComboBox();
            this.textBox_面板內容_廠牌 = new System.Windows.Forms.TextBox();
            this.rJ_Lable7 = new MyUI.RJ_Lable();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.comboBox_面板內容_背景顏色 = new System.Windows.Forms.ComboBox();
            this.rJ_Button_重繪 = new MyUI.RJ_Button();
            this.textBox_面板內容_GUID = new System.Windows.Forms.TextBox();
            this.label_上傳狀態 = new System.Windows.Forms.Label();
            this.button_面板內容_字體_製造日期 = new System.Windows.Forms.Button();
            this.rJ_ProgressBar_上傳狀態 = new MyUI.RJ_ProgressBar();
            this.groupBox13 = new System.Windows.Forms.GroupBox();
            this.comboBox_面板內容_字體顏色_製造日期 = new System.Windows.Forms.ComboBox();
            this.button_面板內容_字體_批號 = new System.Windows.Forms.Button();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.comboBox_面板內容_字體顏色_批號 = new System.Windows.Forms.ComboBox();
            this.button_面板內容_字體_藥名 = new System.Windows.Forms.Button();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.comboBox_面板內容_字體顏色_藥名 = new System.Windows.Forms.ComboBox();
            this.button_面板內容_字體_包裝單位 = new System.Windows.Forms.Button();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.comboBox_面板內容_字體顏色_包裝單位 = new System.Windows.Forms.ComboBox();
            this.button_面板內容_字體_效期 = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.comboBox_面板內容_字體顏色_效期 = new System.Windows.Forms.ComboBox();
            this.button_面板內容_字體_藥碼 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.comboBox_面板內容_字體顏色_藥碼 = new System.Windows.Forms.ComboBox();
            this.textBox_面板內容_製造日期 = new System.Windows.Forms.TextBox();
            this.textBox_面板內容_批號 = new System.Windows.Forms.TextBox();
            this.textBox_面板內容_藥名 = new System.Windows.Forms.TextBox();
            this.textBox_面板內容_包裝單位 = new System.Windows.Forms.TextBox();
            this.textBox_面板內容_效期 = new System.Windows.Forms.TextBox();
            this.textBox_面板內容_藥碼 = new System.Windows.Forms.TextBox();
            this.rJ_Lable6 = new MyUI.RJ_Lable();
            this.rJ_Lable5 = new MyUI.RJ_Lable();
            this.rJ_Lable4 = new MyUI.RJ_Lable();
            this.rJ_Lable3 = new MyUI.RJ_Lable();
            this.rJ_Lable2 = new MyUI.RJ_Lable();
            this.rJ_Lable1 = new MyUI.RJ_Lable();
            this.sqL_DataGridView_面板列表 = new SQLUI.SQL_DataGridView();
            this.rJ_Button_面板列表_新增 = new MyUI.RJ_Button();
            this.rJ_Button_面板列表_刪除 = new MyUI.RJ_Button();
            this.fontDialog = new System.Windows.Forms.FontDialog();
            this.groupBox1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox13.SuspendLayout();
            this.groupBox11.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBox_PortName
            // 
            this.comboBox_PortName.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.comboBox_PortName.FormattingEnabled = true;
            this.comboBox_PortName.Location = new System.Drawing.Point(31, 62);
            this.comboBox_PortName.Name = "comboBox_PortName";
            this.comboBox_PortName.Size = new System.Drawing.Size(140, 35);
            this.comboBox_PortName.TabIndex = 0;
            this.comboBox_PortName.Text = "--請選擇--";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label2.Location = new System.Drawing.Point(26, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 26);
            this.label2.TabIndex = 3;
            this.label2.Text = "狀態 :";
            // 
            // label_狀態
            // 
            this.label_狀態.AutoSize = true;
            this.label_狀態.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label_狀態.Location = new System.Drawing.Point(91, 26);
            this.label_狀態.Name = "label_狀態";
            this.label_狀態.Size = new System.Drawing.Size(75, 26);
            this.label_狀態.TabIndex = 4;
            this.label_狀態.Text = "未連接";
            // 
            // rJ_Button_上傳
            // 
            this.rJ_Button_上傳.AutoResetState = false;
            this.rJ_Button_上傳.BackColor = System.Drawing.Color.RoyalBlue;
            this.rJ_Button_上傳.BackgroundColor = System.Drawing.Color.RoyalBlue;
            this.rJ_Button_上傳.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.rJ_Button_上傳.BorderRadius = 5;
            this.rJ_Button_上傳.BorderSize = 0;
            this.rJ_Button_上傳.buttonType = MyUI.RJ_Button.ButtonType.Push;
            this.rJ_Button_上傳.Enabled = false;
            this.rJ_Button_上傳.FlatAppearance.BorderSize = 0;
            this.rJ_Button_上傳.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Button_上傳.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_Button_上傳.ForeColor = System.Drawing.Color.White;
            this.rJ_Button_上傳.GUID = "";
            this.rJ_Button_上傳.Location = new System.Drawing.Point(791, 19);
            this.rJ_Button_上傳.Name = "rJ_Button_上傳";
            this.rJ_Button_上傳.Size = new System.Drawing.Size(109, 54);
            this.rJ_Button_上傳.State = false;
            this.rJ_Button_上傳.TabIndex = 6;
            this.rJ_Button_上傳.Text = "上傳";
            this.rJ_Button_上傳.TextColor = System.Drawing.Color.White;
            this.rJ_Button_上傳.UseVisualStyleBackColor = false;
            // 
            // rJ_Button_連接
            // 
            this.rJ_Button_連接.AutoResetState = false;
            this.rJ_Button_連接.BackColor = System.Drawing.Color.RoyalBlue;
            this.rJ_Button_連接.BackgroundColor = System.Drawing.Color.RoyalBlue;
            this.rJ_Button_連接.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.rJ_Button_連接.BorderRadius = 5;
            this.rJ_Button_連接.BorderSize = 0;
            this.rJ_Button_連接.buttonType = MyUI.RJ_Button.ButtonType.Push;
            this.rJ_Button_連接.FlatAppearance.BorderSize = 0;
            this.rJ_Button_連接.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Button_連接.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_Button_連接.ForeColor = System.Drawing.Color.White;
            this.rJ_Button_連接.GUID = "";
            this.rJ_Button_連接.Location = new System.Drawing.Point(178, 51);
            this.rJ_Button_連接.Name = "rJ_Button_連接";
            this.rJ_Button_連接.Size = new System.Drawing.Size(93, 54);
            this.rJ_Button_連接.State = false;
            this.rJ_Button_連接.TabIndex = 7;
            this.rJ_Button_連接.Text = "連接";
            this.rJ_Button_連接.TextColor = System.Drawing.Color.White;
            this.rJ_Button_連接.UseVisualStyleBackColor = false;
            // 
            // panel_EPD290
            // 
            this.panel_EPD290.BackColor = System.Drawing.Color.PapayaWhip;
            this.panel_EPD290.Location = new System.Drawing.Point(163, 119);
            this.panel_EPD290.Name = "panel_EPD290";
            this.panel_EPD290.Size = new System.Drawing.Size(592, 256);
            this.panel_EPD290.TabIndex = 9;
            // 
            // rJ_Button_存檔
            // 
            this.rJ_Button_存檔.AutoResetState = false;
            this.rJ_Button_存檔.BackColor = System.Drawing.Color.RoyalBlue;
            this.rJ_Button_存檔.BackgroundColor = System.Drawing.Color.RoyalBlue;
            this.rJ_Button_存檔.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.rJ_Button_存檔.BorderRadius = 5;
            this.rJ_Button_存檔.BorderSize = 0;
            this.rJ_Button_存檔.buttonType = MyUI.RJ_Button.ButtonType.Push;
            this.rJ_Button_存檔.Enabled = false;
            this.rJ_Button_存檔.FlatAppearance.BorderSize = 0;
            this.rJ_Button_存檔.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Button_存檔.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_Button_存檔.ForeColor = System.Drawing.Color.White;
            this.rJ_Button_存檔.GUID = "";
            this.rJ_Button_存檔.Location = new System.Drawing.Point(791, 329);
            this.rJ_Button_存檔.Name = "rJ_Button_存檔";
            this.rJ_Button_存檔.Size = new System.Drawing.Size(109, 54);
            this.rJ_Button_存檔.State = false;
            this.rJ_Button_存檔.TabIndex = 10;
            this.rJ_Button_存檔.Text = "存檔";
            this.rJ_Button_存檔.TextColor = System.Drawing.Color.White;
            this.rJ_Button_存檔.UseVisualStyleBackColor = false;
            this.rJ_Button_存檔.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.White;
            this.groupBox1.Controls.Add(this.button_面板內容_字體_廠牌);
            this.groupBox1.Controls.Add(this.groupBox4);
            this.groupBox1.Controls.Add(this.textBox_面板內容_廠牌);
            this.groupBox1.Controls.Add(this.rJ_Lable7);
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Controls.Add(this.rJ_Button_重繪);
            this.groupBox1.Controls.Add(this.textBox_面板內容_GUID);
            this.groupBox1.Controls.Add(this.label_上傳狀態);
            this.groupBox1.Controls.Add(this.button_面板內容_字體_製造日期);
            this.groupBox1.Controls.Add(this.rJ_ProgressBar_上傳狀態);
            this.groupBox1.Controls.Add(this.groupBox13);
            this.groupBox1.Controls.Add(this.rJ_Button_上傳);
            this.groupBox1.Controls.Add(this.rJ_Button_存檔);
            this.groupBox1.Controls.Add(this.button_面板內容_字體_批號);
            this.groupBox1.Controls.Add(this.groupBox11);
            this.groupBox1.Controls.Add(this.button_面板內容_字體_藥名);
            this.groupBox1.Controls.Add(this.groupBox9);
            this.groupBox1.Controls.Add(this.button_面板內容_字體_包裝單位);
            this.groupBox1.Controls.Add(this.groupBox7);
            this.groupBox1.Controls.Add(this.button_面板內容_字體_效期);
            this.groupBox1.Controls.Add(this.groupBox5);
            this.groupBox1.Controls.Add(this.button_面板內容_字體_藥碼);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.textBox_面板內容_製造日期);
            this.groupBox1.Controls.Add(this.textBox_面板內容_批號);
            this.groupBox1.Controls.Add(this.textBox_面板內容_藥名);
            this.groupBox1.Controls.Add(this.textBox_面板內容_包裝單位);
            this.groupBox1.Controls.Add(this.textBox_面板內容_效期);
            this.groupBox1.Controls.Add(this.textBox_面板內容_藥碼);
            this.groupBox1.Controls.Add(this.rJ_Lable6);
            this.groupBox1.Controls.Add(this.rJ_Lable5);
            this.groupBox1.Controls.Add(this.rJ_Lable4);
            this.groupBox1.Controls.Add(this.rJ_Lable3);
            this.groupBox1.Controls.Add(this.rJ_Lable2);
            this.groupBox1.Controls.Add(this.rJ_Lable1);
            this.groupBox1.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.groupBox1.Location = new System.Drawing.Point(31, 386);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(906, 543);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "面板內容";
            // 
            // button_面板內容_字體_廠牌
            // 
            this.button_面板內容_字體_廠牌.Location = new System.Drawing.Point(665, 446);
            this.button_面板內容_字體_廠牌.Name = "button_面板內容_字體_廠牌";
            this.button_面板內容_字體_廠牌.Size = new System.Drawing.Size(69, 35);
            this.button_面板內容_字體_廠牌.TabIndex = 37;
            this.button_面板內容_字體_廠牌.Text = "字體";
            this.button_面板內容_字體_廠牌.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.comboBox_面板內容_字體顏色_廠牌);
            this.groupBox4.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.groupBox4.Location = new System.Drawing.Point(566, 435);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(93, 56);
            this.groupBox4.TabIndex = 36;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "字體顏色";
            // 
            // comboBox_面板內容_字體顏色_廠牌
            // 
            this.comboBox_面板內容_字體顏色_廠牌.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_面板內容_字體顏色_廠牌.FormattingEnabled = true;
            this.comboBox_面板內容_字體顏色_廠牌.Items.AddRange(new object[] {
            "紅",
            "白",
            "黑"});
            this.comboBox_面板內容_字體顏色_廠牌.Location = new System.Drawing.Point(6, 21);
            this.comboBox_面板內容_字體顏色_廠牌.Name = "comboBox_面板內容_字體顏色_廠牌";
            this.comboBox_面板內容_字體顏色_廠牌.Size = new System.Drawing.Size(80, 25);
            this.comboBox_面板內容_字體顏色_廠牌.TabIndex = 12;
            // 
            // textBox_面板內容_廠牌
            // 
            this.textBox_面板內容_廠牌.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.textBox_面板內容_廠牌.Location = new System.Drawing.Point(146, 446);
            this.textBox_面板內容_廠牌.Name = "textBox_面板內容_廠牌";
            this.textBox_面板內容_廠牌.Size = new System.Drawing.Size(414, 35);
            this.textBox_面板內容_廠牌.TabIndex = 35;
            // 
            // rJ_Lable7
            // 
            this.rJ_Lable7.BackColor = System.Drawing.Color.Transparent;
            this.rJ_Lable7.BackgroundColor = System.Drawing.Color.Transparent;
            this.rJ_Lable7.BorderColor = System.Drawing.Color.MediumBlue;
            this.rJ_Lable7.BorderRadius = 3;
            this.rJ_Lable7.BorderSize = 2;
            this.rJ_Lable7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Lable7.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_Lable7.ForeColor = System.Drawing.Color.Black;
            this.rJ_Lable7.GUID = "";
            this.rJ_Lable7.Location = new System.Drawing.Point(31, 441);
            this.rJ_Lable7.Name = "rJ_Lable7";
            this.rJ_Lable7.Size = new System.Drawing.Size(109, 45);
            this.rJ_Lable7.TabIndex = 34;
            this.rJ_Lable7.Text = "廠牌";
            this.rJ_Lable7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rJ_Lable7.TextColor = System.Drawing.Color.Black;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.comboBox_面板內容_背景顏色);
            this.groupBox3.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.groupBox3.Location = new System.Drawing.Point(508, 73);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(129, 75);
            this.groupBox3.TabIndex = 33;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "背景顏色";
            // 
            // comboBox_面板內容_背景顏色
            // 
            this.comboBox_面板內容_背景顏色.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_面板內容_背景顏色.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.comboBox_面板內容_背景顏色.FormattingEnabled = true;
            this.comboBox_面板內容_背景顏色.Items.AddRange(new object[] {
            "紅",
            "白",
            "黑"});
            this.comboBox_面板內容_背景顏色.Location = new System.Drawing.Point(24, 30);
            this.comboBox_面板內容_背景顏色.Name = "comboBox_面板內容_背景顏色";
            this.comboBox_面板內容_背景顏色.Size = new System.Drawing.Size(80, 34);
            this.comboBox_面板內容_背景顏色.TabIndex = 12;
            // 
            // rJ_Button_重繪
            // 
            this.rJ_Button_重繪.AutoResetState = false;
            this.rJ_Button_重繪.BackColor = System.Drawing.Color.RoyalBlue;
            this.rJ_Button_重繪.BackgroundColor = System.Drawing.Color.RoyalBlue;
            this.rJ_Button_重繪.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.rJ_Button_重繪.BorderRadius = 5;
            this.rJ_Button_重繪.BorderSize = 0;
            this.rJ_Button_重繪.buttonType = MyUI.RJ_Button.ButtonType.Push;
            this.rJ_Button_重繪.Enabled = false;
            this.rJ_Button_重繪.FlatAppearance.BorderSize = 0;
            this.rJ_Button_重繪.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Button_重繪.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_Button_重繪.ForeColor = System.Drawing.Color.White;
            this.rJ_Button_重繪.GUID = "";
            this.rJ_Button_重繪.Location = new System.Drawing.Point(791, 389);
            this.rJ_Button_重繪.Name = "rJ_Button_重繪";
            this.rJ_Button_重繪.Size = new System.Drawing.Size(109, 54);
            this.rJ_Button_重繪.State = false;
            this.rJ_Button_重繪.TabIndex = 32;
            this.rJ_Button_重繪.Text = "重繪";
            this.rJ_Button_重繪.TextColor = System.Drawing.Color.White;
            this.rJ_Button_重繪.UseVisualStyleBackColor = false;
            // 
            // textBox_面板內容_GUID
            // 
            this.textBox_面板內容_GUID.Enabled = false;
            this.textBox_面板內容_GUID.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.textBox_面板內容_GUID.Location = new System.Drawing.Point(665, 83);
            this.textBox_面板內容_GUID.Name = "textBox_面板內容_GUID";
            this.textBox_面板內容_GUID.Size = new System.Drawing.Size(304, 35);
            this.textBox_面板內容_GUID.TabIndex = 31;
            this.textBox_面板內容_GUID.Visible = false;
            // 
            // label_上傳狀態
            // 
            this.label_上傳狀態.AutoSize = true;
            this.label_上傳狀態.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label_上傳狀態.Location = new System.Drawing.Point(35, 28);
            this.label_上傳狀態.Name = "label_上傳狀態";
            this.label_上傳狀態.Size = new System.Drawing.Size(80, 21);
            this.label_上傳狀態.TabIndex = 14;
            this.label_上傳狀態.Text = "----------";
            // 
            // button_面板內容_字體_製造日期
            // 
            this.button_面板內容_字體_製造日期.Location = new System.Drawing.Point(665, 325);
            this.button_面板內容_字體_製造日期.Name = "button_面板內容_字體_製造日期";
            this.button_面板內容_字體_製造日期.Size = new System.Drawing.Size(69, 35);
            this.button_面板內容_字體_製造日期.TabIndex = 30;
            this.button_面板內容_字體_製造日期.Text = "字體";
            this.button_面板內容_字體_製造日期.UseVisualStyleBackColor = true;
            // 
            // rJ_ProgressBar_上傳狀態
            // 
            this.rJ_ProgressBar_上傳狀態.ChannelColor = System.Drawing.Color.LightSteelBlue;
            this.rJ_ProgressBar_上傳狀態.ChannelHeight = 10;
            this.rJ_ProgressBar_上傳狀態.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_ProgressBar_上傳狀態.ForeBackColor = System.Drawing.Color.RoyalBlue;
            this.rJ_ProgressBar_上傳狀態.ForeColor = System.Drawing.Color.White;
            this.rJ_ProgressBar_上傳狀態.Location = new System.Drawing.Point(35, 28);
            this.rJ_ProgressBar_上傳狀態.Name = "rJ_ProgressBar_上傳狀態";
            this.rJ_ProgressBar_上傳狀態.ShowMaximun = true;
            this.rJ_ProgressBar_上傳狀態.ShowValue = MyUI.TextPosition.Right;
            this.rJ_ProgressBar_上傳狀態.Size = new System.Drawing.Size(734, 34);
            this.rJ_ProgressBar_上傳狀態.SliderColor = System.Drawing.Color.RoyalBlue;
            this.rJ_ProgressBar_上傳狀態.SliderHeight = 6;
            this.rJ_ProgressBar_上傳狀態.SymbolAfter = "";
            this.rJ_ProgressBar_上傳狀態.SymbolBefore = "";
            this.rJ_ProgressBar_上傳狀態.TabIndex = 13;
            // 
            // groupBox13
            // 
            this.groupBox13.Controls.Add(this.comboBox_面板內容_字體顏色_製造日期);
            this.groupBox13.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.groupBox13.Location = new System.Drawing.Point(566, 311);
            this.groupBox13.Name = "groupBox13";
            this.groupBox13.Size = new System.Drawing.Size(93, 56);
            this.groupBox13.TabIndex = 28;
            this.groupBox13.TabStop = false;
            this.groupBox13.Text = "字體顏色";
            // 
            // comboBox_面板內容_字體顏色_製造日期
            // 
            this.comboBox_面板內容_字體顏色_製造日期.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_面板內容_字體顏色_製造日期.FormattingEnabled = true;
            this.comboBox_面板內容_字體顏色_製造日期.Items.AddRange(new object[] {
            "紅",
            "白",
            "黑"});
            this.comboBox_面板內容_字體顏色_製造日期.Location = new System.Drawing.Point(6, 21);
            this.comboBox_面板內容_字體顏色_製造日期.Name = "comboBox_面板內容_字體顏色_製造日期";
            this.comboBox_面板內容_字體顏色_製造日期.Size = new System.Drawing.Size(80, 25);
            this.comboBox_面板內容_字體顏色_製造日期.TabIndex = 12;
            // 
            // button_面板內容_字體_批號
            // 
            this.button_面板內容_字體_批號.Location = new System.Drawing.Point(665, 260);
            this.button_面板內容_字體_批號.Name = "button_面板內容_字體_批號";
            this.button_面板內容_字體_批號.Size = new System.Drawing.Size(69, 35);
            this.button_面板內容_字體_批號.TabIndex = 27;
            this.button_面板內容_字體_批號.Text = "字體";
            this.button_面板內容_字體_批號.UseVisualStyleBackColor = true;
            // 
            // groupBox11
            // 
            this.groupBox11.Controls.Add(this.comboBox_面板內容_字體顏色_批號);
            this.groupBox11.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.groupBox11.Location = new System.Drawing.Point(566, 249);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.Size = new System.Drawing.Size(93, 56);
            this.groupBox11.TabIndex = 25;
            this.groupBox11.TabStop = false;
            this.groupBox11.Text = "字體顏色";
            // 
            // comboBox_面板內容_字體顏色_批號
            // 
            this.comboBox_面板內容_字體顏色_批號.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_面板內容_字體顏色_批號.FormattingEnabled = true;
            this.comboBox_面板內容_字體顏色_批號.Items.AddRange(new object[] {
            "紅",
            "白",
            "黑"});
            this.comboBox_面板內容_字體顏色_批號.Location = new System.Drawing.Point(6, 21);
            this.comboBox_面板內容_字體顏色_批號.Name = "comboBox_面板內容_字體顏色_批號";
            this.comboBox_面板內容_字體顏色_批號.Size = new System.Drawing.Size(80, 25);
            this.comboBox_面板內容_字體顏色_批號.TabIndex = 12;
            // 
            // button_面板內容_字體_藥名
            // 
            this.button_面板內容_字體_藥名.Location = new System.Drawing.Point(665, 201);
            this.button_面板內容_字體_藥名.Name = "button_面板內容_字體_藥名";
            this.button_面板內容_字體_藥名.Size = new System.Drawing.Size(69, 35);
            this.button_面板內容_字體_藥名.TabIndex = 24;
            this.button_面板內容_字體_藥名.Text = "字體";
            this.button_面板內容_字體_藥名.UseVisualStyleBackColor = true;
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.comboBox_面板內容_字體顏色_藥名);
            this.groupBox9.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.groupBox9.Location = new System.Drawing.Point(566, 192);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(93, 56);
            this.groupBox9.TabIndex = 22;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "字體顏色";
            // 
            // comboBox_面板內容_字體顏色_藥名
            // 
            this.comboBox_面板內容_字體顏色_藥名.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_面板內容_字體顏色_藥名.FormattingEnabled = true;
            this.comboBox_面板內容_字體顏色_藥名.Items.AddRange(new object[] {
            "紅",
            "白",
            "黑"});
            this.comboBox_面板內容_字體顏色_藥名.Location = new System.Drawing.Point(6, 21);
            this.comboBox_面板內容_字體顏色_藥名.Name = "comboBox_面板內容_字體顏色_藥名";
            this.comboBox_面板內容_字體顏色_藥名.Size = new System.Drawing.Size(80, 25);
            this.comboBox_面板內容_字體顏色_藥名.TabIndex = 12;
            // 
            // button_面板內容_字體_包裝單位
            // 
            this.button_面板內容_字體_包裝單位.Location = new System.Drawing.Point(414, 139);
            this.button_面板內容_字體_包裝單位.Name = "button_面板內容_字體_包裝單位";
            this.button_面板內容_字體_包裝單位.Size = new System.Drawing.Size(69, 35);
            this.button_面板內容_字體_包裝單位.TabIndex = 21;
            this.button_面板內容_字體_包裝單位.Text = "字體";
            this.button_面板內容_字體_包裝單位.UseVisualStyleBackColor = true;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.comboBox_面板內容_字體顏色_包裝單位);
            this.groupBox7.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.groupBox7.Location = new System.Drawing.Point(315, 128);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(93, 56);
            this.groupBox7.TabIndex = 19;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "字體顏色";
            // 
            // comboBox_面板內容_字體顏色_包裝單位
            // 
            this.comboBox_面板內容_字體顏色_包裝單位.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_面板內容_字體顏色_包裝單位.FormattingEnabled = true;
            this.comboBox_面板內容_字體顏色_包裝單位.Items.AddRange(new object[] {
            "紅",
            "白",
            "黑"});
            this.comboBox_面板內容_字體顏色_包裝單位.Location = new System.Drawing.Point(6, 21);
            this.comboBox_面板內容_字體顏色_包裝單位.Name = "comboBox_面板內容_字體顏色_包裝單位";
            this.comboBox_面板內容_字體顏色_包裝單位.Size = new System.Drawing.Size(80, 25);
            this.comboBox_面板內容_字體顏色_包裝單位.TabIndex = 12;
            // 
            // button_面板內容_字體_效期
            // 
            this.button_面板內容_字體_效期.Location = new System.Drawing.Point(665, 384);
            this.button_面板內容_字體_效期.Name = "button_面板內容_字體_效期";
            this.button_面板內容_字體_效期.Size = new System.Drawing.Size(69, 35);
            this.button_面板內容_字體_效期.TabIndex = 18;
            this.button_面板內容_字體_效期.Text = "字體";
            this.button_面板內容_字體_效期.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.comboBox_面板內容_字體顏色_效期);
            this.groupBox5.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.groupBox5.Location = new System.Drawing.Point(566, 373);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(93, 56);
            this.groupBox5.TabIndex = 16;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "字體顏色";
            // 
            // comboBox_面板內容_字體顏色_效期
            // 
            this.comboBox_面板內容_字體顏色_效期.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_面板內容_字體顏色_效期.FormattingEnabled = true;
            this.comboBox_面板內容_字體顏色_效期.Items.AddRange(new object[] {
            "紅",
            "白",
            "黑"});
            this.comboBox_面板內容_字體顏色_效期.Location = new System.Drawing.Point(6, 21);
            this.comboBox_面板內容_字體顏色_效期.Name = "comboBox_面板內容_字體顏色_效期";
            this.comboBox_面板內容_字體顏色_效期.Size = new System.Drawing.Size(80, 25);
            this.comboBox_面板內容_字體顏色_效期.TabIndex = 12;
            // 
            // button_面板內容_字體_藥碼
            // 
            this.button_面板內容_字體_藥碼.Location = new System.Drawing.Point(414, 79);
            this.button_面板內容_字體_藥碼.Name = "button_面板內容_字體_藥碼";
            this.button_面板內容_字體_藥碼.Size = new System.Drawing.Size(69, 35);
            this.button_面板內容_字體_藥碼.TabIndex = 15;
            this.button_面板內容_字體_藥碼.Text = "字體";
            this.button_面板內容_字體_藥碼.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.comboBox_面板內容_字體顏色_藥碼);
            this.groupBox2.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.groupBox2.Location = new System.Drawing.Point(315, 65);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(93, 56);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "字體顏色";
            // 
            // comboBox_面板內容_字體顏色_藥碼
            // 
            this.comboBox_面板內容_字體顏色_藥碼.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_面板內容_字體顏色_藥碼.FormattingEnabled = true;
            this.comboBox_面板內容_字體顏色_藥碼.Items.AddRange(new object[] {
            "紅",
            "白",
            "黑"});
            this.comboBox_面板內容_字體顏色_藥碼.Location = new System.Drawing.Point(6, 21);
            this.comboBox_面板內容_字體顏色_藥碼.Name = "comboBox_面板內容_字體顏色_藥碼";
            this.comboBox_面板內容_字體顏色_藥碼.Size = new System.Drawing.Size(80, 25);
            this.comboBox_面板內容_字體顏色_藥碼.TabIndex = 12;
            // 
            // textBox_面板內容_製造日期
            // 
            this.textBox_面板內容_製造日期.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.textBox_面板內容_製造日期.Location = new System.Drawing.Point(146, 322);
            this.textBox_面板內容_製造日期.Name = "textBox_面板內容_製造日期";
            this.textBox_面板內容_製造日期.Size = new System.Drawing.Size(414, 35);
            this.textBox_面板內容_製造日期.TabIndex = 11;
            // 
            // textBox_面板內容_批號
            // 
            this.textBox_面板內容_批號.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.textBox_面板內容_批號.Location = new System.Drawing.Point(146, 260);
            this.textBox_面板內容_批號.Name = "textBox_面板內容_批號";
            this.textBox_面板內容_批號.Size = new System.Drawing.Size(414, 35);
            this.textBox_面板內容_批號.TabIndex = 10;
            // 
            // textBox_面板內容_藥名
            // 
            this.textBox_面板內容_藥名.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.textBox_面板內容_藥名.Location = new System.Drawing.Point(146, 203);
            this.textBox_面板內容_藥名.Name = "textBox_面板內容_藥名";
            this.textBox_面板內容_藥名.Size = new System.Drawing.Size(414, 35);
            this.textBox_面板內容_藥名.TabIndex = 9;
            // 
            // textBox_面板內容_包裝單位
            // 
            this.textBox_面板內容_包裝單位.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.textBox_面板內容_包裝單位.Location = new System.Drawing.Point(146, 139);
            this.textBox_面板內容_包裝單位.Name = "textBox_面板內容_包裝單位";
            this.textBox_面板內容_包裝單位.Size = new System.Drawing.Size(163, 35);
            this.textBox_面板內容_包裝單位.TabIndex = 8;
            // 
            // textBox_面板內容_效期
            // 
            this.textBox_面板內容_效期.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.textBox_面板內容_效期.Location = new System.Drawing.Point(146, 384);
            this.textBox_面板內容_效期.Name = "textBox_面板內容_效期";
            this.textBox_面板內容_效期.Size = new System.Drawing.Size(414, 35);
            this.textBox_面板內容_效期.TabIndex = 7;
            // 
            // textBox_面板內容_藥碼
            // 
            this.textBox_面板內容_藥碼.Enabled = false;
            this.textBox_面板內容_藥碼.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.textBox_面板內容_藥碼.Location = new System.Drawing.Point(146, 78);
            this.textBox_面板內容_藥碼.Name = "textBox_面板內容_藥碼";
            this.textBox_面板內容_藥碼.Size = new System.Drawing.Size(163, 35);
            this.textBox_面板內容_藥碼.TabIndex = 6;
            // 
            // rJ_Lable6
            // 
            this.rJ_Lable6.BackColor = System.Drawing.Color.Transparent;
            this.rJ_Lable6.BackgroundColor = System.Drawing.Color.Transparent;
            this.rJ_Lable6.BorderColor = System.Drawing.Color.MediumBlue;
            this.rJ_Lable6.BorderRadius = 3;
            this.rJ_Lable6.BorderSize = 2;
            this.rJ_Lable6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Lable6.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_Lable6.ForeColor = System.Drawing.Color.Black;
            this.rJ_Lable6.GUID = "";
            this.rJ_Lable6.Location = new System.Drawing.Point(31, 134);
            this.rJ_Lable6.Name = "rJ_Lable6";
            this.rJ_Lable6.Size = new System.Drawing.Size(109, 45);
            this.rJ_Lable6.TabIndex = 5;
            this.rJ_Lable6.Text = "包裝單位";
            this.rJ_Lable6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rJ_Lable6.TextColor = System.Drawing.Color.Black;
            // 
            // rJ_Lable5
            // 
            this.rJ_Lable5.BackColor = System.Drawing.Color.Transparent;
            this.rJ_Lable5.BackgroundColor = System.Drawing.Color.Transparent;
            this.rJ_Lable5.BorderColor = System.Drawing.Color.MediumBlue;
            this.rJ_Lable5.BorderRadius = 3;
            this.rJ_Lable5.BorderSize = 2;
            this.rJ_Lable5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Lable5.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_Lable5.ForeColor = System.Drawing.Color.Black;
            this.rJ_Lable5.GUID = "";
            this.rJ_Lable5.Location = new System.Drawing.Point(31, 255);
            this.rJ_Lable5.Name = "rJ_Lable5";
            this.rJ_Lable5.Size = new System.Drawing.Size(109, 45);
            this.rJ_Lable5.TabIndex = 4;
            this.rJ_Lable5.Text = "批號";
            this.rJ_Lable5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rJ_Lable5.TextColor = System.Drawing.Color.Black;
            // 
            // rJ_Lable4
            // 
            this.rJ_Lable4.BackColor = System.Drawing.Color.Transparent;
            this.rJ_Lable4.BackgroundColor = System.Drawing.Color.Transparent;
            this.rJ_Lable4.BorderColor = System.Drawing.Color.MediumBlue;
            this.rJ_Lable4.BorderRadius = 3;
            this.rJ_Lable4.BorderSize = 2;
            this.rJ_Lable4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Lable4.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_Lable4.ForeColor = System.Drawing.Color.Black;
            this.rJ_Lable4.GUID = "";
            this.rJ_Lable4.Location = new System.Drawing.Point(31, 379);
            this.rJ_Lable4.Name = "rJ_Lable4";
            this.rJ_Lable4.Size = new System.Drawing.Size(109, 45);
            this.rJ_Lable4.TabIndex = 3;
            this.rJ_Lable4.Text = "效期";
            this.rJ_Lable4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rJ_Lable4.TextColor = System.Drawing.Color.Black;
            // 
            // rJ_Lable3
            // 
            this.rJ_Lable3.BackColor = System.Drawing.Color.Transparent;
            this.rJ_Lable3.BackgroundColor = System.Drawing.Color.Transparent;
            this.rJ_Lable3.BorderColor = System.Drawing.Color.MediumBlue;
            this.rJ_Lable3.BorderRadius = 3;
            this.rJ_Lable3.BorderSize = 2;
            this.rJ_Lable3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Lable3.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_Lable3.ForeColor = System.Drawing.Color.Black;
            this.rJ_Lable3.GUID = "";
            this.rJ_Lable3.Location = new System.Drawing.Point(31, 317);
            this.rJ_Lable3.Name = "rJ_Lable3";
            this.rJ_Lable3.Size = new System.Drawing.Size(109, 45);
            this.rJ_Lable3.TabIndex = 2;
            this.rJ_Lable3.Text = "製造日期";
            this.rJ_Lable3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rJ_Lable3.TextColor = System.Drawing.Color.Black;
            // 
            // rJ_Lable2
            // 
            this.rJ_Lable2.BackColor = System.Drawing.Color.Transparent;
            this.rJ_Lable2.BackgroundColor = System.Drawing.Color.Transparent;
            this.rJ_Lable2.BorderColor = System.Drawing.Color.MediumBlue;
            this.rJ_Lable2.BorderRadius = 3;
            this.rJ_Lable2.BorderSize = 2;
            this.rJ_Lable2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Lable2.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_Lable2.ForeColor = System.Drawing.Color.Black;
            this.rJ_Lable2.GUID = "";
            this.rJ_Lable2.Location = new System.Drawing.Point(31, 198);
            this.rJ_Lable2.Name = "rJ_Lable2";
            this.rJ_Lable2.Size = new System.Drawing.Size(109, 45);
            this.rJ_Lable2.TabIndex = 1;
            this.rJ_Lable2.Text = "藥名";
            this.rJ_Lable2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rJ_Lable2.TextColor = System.Drawing.Color.Black;
            // 
            // rJ_Lable1
            // 
            this.rJ_Lable1.BackColor = System.Drawing.Color.Transparent;
            this.rJ_Lable1.BackgroundColor = System.Drawing.Color.Transparent;
            this.rJ_Lable1.BorderColor = System.Drawing.Color.MediumBlue;
            this.rJ_Lable1.BorderRadius = 3;
            this.rJ_Lable1.BorderSize = 2;
            this.rJ_Lable1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Lable1.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_Lable1.ForeColor = System.Drawing.Color.Black;
            this.rJ_Lable1.GUID = "";
            this.rJ_Lable1.Location = new System.Drawing.Point(31, 73);
            this.rJ_Lable1.Name = "rJ_Lable1";
            this.rJ_Lable1.Size = new System.Drawing.Size(109, 45);
            this.rJ_Lable1.TabIndex = 0;
            this.rJ_Lable1.Text = "藥碼";
            this.rJ_Lable1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rJ_Lable1.TextColor = System.Drawing.Color.Black;
            // 
            // sqL_DataGridView_面板列表
            // 
            this.sqL_DataGridView_面板列表.AutoSelectToDeep = true;
            this.sqL_DataGridView_面板列表.backColor = System.Drawing.Color.LightBlue;
            this.sqL_DataGridView_面板列表.BorderColor = System.Drawing.Color.LightBlue;
            this.sqL_DataGridView_面板列表.BorderRadius = 10;
            this.sqL_DataGridView_面板列表.BorderSize = 2;
            this.sqL_DataGridView_面板列表.cellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Raised;
            this.sqL_DataGridView_面板列表.cellStylBackColor = System.Drawing.Color.LightBlue;
            this.sqL_DataGridView_面板列表.cellStyleFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.sqL_DataGridView_面板列表.cellStylForeColor = System.Drawing.Color.Black;
            this.sqL_DataGridView_面板列表.columnHeaderBackColor = System.Drawing.Color.SkyBlue;
            this.sqL_DataGridView_面板列表.columnHeaderFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.sqL_DataGridView_面板列表.columnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Raised;
            this.sqL_DataGridView_面板列表.columnHeadersHeight = 18;
            this.sqL_DataGridView_面板列表.columnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.sqL_DataGridView_面板列表.Font = new System.Drawing.Font("新細明體", 9F);
            this.sqL_DataGridView_面板列表.ImageBox = false;
            this.sqL_DataGridView_面板列表.Location = new System.Drawing.Point(943, 26);
            this.sqL_DataGridView_面板列表.Name = "sqL_DataGridView_面板列表";
            this.sqL_DataGridView_面板列表.OnlineState = SQLUI.SQL_DataGridView.OnlineEnum.Online;
            this.sqL_DataGridView_面板列表.Password = "user82822040";
            this.sqL_DataGridView_面板列表.Port = ((uint)(3306u));
            this.sqL_DataGridView_面板列表.rowHeaderBackColor = System.Drawing.Color.LightBlue;
            this.sqL_DataGridView_面板列表.rowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Raised;
            this.sqL_DataGridView_面板列表.RowsColor = System.Drawing.SystemColors.Control;
            this.sqL_DataGridView_面板列表.RowsHeight = 30;
            this.sqL_DataGridView_面板列表.SaveFileName = "SQL_DataGridView";
            this.sqL_DataGridView_面板列表.Server = "127.0.0.0";
            this.sqL_DataGridView_面板列表.Size = new System.Drawing.Size(524, 729);
            this.sqL_DataGridView_面板列表.SSLMode = MySql.Data.MySqlClient.MySqlSslMode.None;
            this.sqL_DataGridView_面板列表.TabIndex = 13;
            this.sqL_DataGridView_面板列表.UserName = "root";
            this.sqL_DataGridView_面板列表.可拖曳欄位寬度 = false;
            this.sqL_DataGridView_面板列表.可選擇多列 = false;
            this.sqL_DataGridView_面板列表.單格樣式 = System.Windows.Forms.DataGridViewCellBorderStyle.Raised;
            this.sqL_DataGridView_面板列表.自動換行 = true;
            this.sqL_DataGridView_面板列表.表單字體 = new System.Drawing.Font("新細明體", 9F);
            this.sqL_DataGridView_面板列表.邊框樣式 = System.Windows.Forms.BorderStyle.Fixed3D;
            this.sqL_DataGridView_面板列表.顯示CheckBox = false;
            this.sqL_DataGridView_面板列表.顯示首列 = true;
            this.sqL_DataGridView_面板列表.顯示首行 = true;
            this.sqL_DataGridView_面板列表.首列樣式 = System.Windows.Forms.DataGridViewHeaderBorderStyle.Raised;
            this.sqL_DataGridView_面板列表.首行樣式 = System.Windows.Forms.DataGridViewHeaderBorderStyle.Raised;
            // 
            // rJ_Button_面板列表_新增
            // 
            this.rJ_Button_面板列表_新增.AutoResetState = false;
            this.rJ_Button_面板列表_新增.BackColor = System.Drawing.Color.RoyalBlue;
            this.rJ_Button_面板列表_新增.BackgroundColor = System.Drawing.Color.RoyalBlue;
            this.rJ_Button_面板列表_新增.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.rJ_Button_面板列表_新增.BorderRadius = 5;
            this.rJ_Button_面板列表_新增.BorderSize = 0;
            this.rJ_Button_面板列表_新增.buttonType = MyUI.RJ_Button.ButtonType.Push;
            this.rJ_Button_面板列表_新增.FlatAppearance.BorderSize = 0;
            this.rJ_Button_面板列表_新增.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Button_面板列表_新增.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_Button_面板列表_新增.ForeColor = System.Drawing.Color.White;
            this.rJ_Button_面板列表_新增.GUID = "";
            this.rJ_Button_面板列表_新增.Location = new System.Drawing.Point(1374, 772);
            this.rJ_Button_面板列表_新增.Name = "rJ_Button_面板列表_新增";
            this.rJ_Button_面板列表_新增.Size = new System.Drawing.Size(93, 54);
            this.rJ_Button_面板列表_新增.State = false;
            this.rJ_Button_面板列表_新增.TabIndex = 14;
            this.rJ_Button_面板列表_新增.Text = "新增";
            this.rJ_Button_面板列表_新增.TextColor = System.Drawing.Color.White;
            this.rJ_Button_面板列表_新增.UseVisualStyleBackColor = false;
            // 
            // rJ_Button_面板列表_刪除
            // 
            this.rJ_Button_面板列表_刪除.AutoResetState = false;
            this.rJ_Button_面板列表_刪除.BackColor = System.Drawing.Color.RoyalBlue;
            this.rJ_Button_面板列表_刪除.BackgroundColor = System.Drawing.Color.RoyalBlue;
            this.rJ_Button_面板列表_刪除.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.rJ_Button_面板列表_刪除.BorderRadius = 5;
            this.rJ_Button_面板列表_刪除.BorderSize = 0;
            this.rJ_Button_面板列表_刪除.buttonType = MyUI.RJ_Button.ButtonType.Push;
            this.rJ_Button_面板列表_刪除.FlatAppearance.BorderSize = 0;
            this.rJ_Button_面板列表_刪除.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Button_面板列表_刪除.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_Button_面板列表_刪除.ForeColor = System.Drawing.Color.White;
            this.rJ_Button_面板列表_刪除.GUID = "";
            this.rJ_Button_面板列表_刪除.Location = new System.Drawing.Point(1275, 772);
            this.rJ_Button_面板列表_刪除.Name = "rJ_Button_面板列表_刪除";
            this.rJ_Button_面板列表_刪除.Size = new System.Drawing.Size(93, 54);
            this.rJ_Button_面板列表_刪除.State = false;
            this.rJ_Button_面板列表_刪除.TabIndex = 15;
            this.rJ_Button_面板列表_刪除.Text = "刪除";
            this.rJ_Button_面板列表_刪除.TextColor = System.Drawing.Color.White;
            this.rJ_Button_面板列表_刪除.UseVisualStyleBackColor = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1479, 941);
            this.Controls.Add(this.rJ_Button_面板列表_刪除);
            this.Controls.Add(this.rJ_Button_面板列表_新增);
            this.Controls.Add(this.sqL_DataGridView_面板列表);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel_EPD290);
            this.Controls.Add(this.rJ_Button_連接);
            this.Controls.Add(this.label_狀態);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBox_PortName);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "EPD 編譯器";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox13.ResumeLayout(false);
            this.groupBox11.ResumeLayout(false);
            this.groupBox9.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox_PortName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label_狀態;
        private MyUI.RJ_Button rJ_Button_上傳;
        private MyUI.RJ_Button rJ_Button_連接;
        private System.Windows.Forms.Panel panel_EPD290;
        private MyUI.RJ_Button rJ_Button_存檔;
        private System.Windows.Forms.GroupBox groupBox1;
        private MyUI.RJ_ProgressBar rJ_ProgressBar_上傳狀態;
        private System.Windows.Forms.Label label_上傳狀態;
        private System.Windows.Forms.Button button_面板內容_字體_製造日期;
        private System.Windows.Forms.GroupBox groupBox13;
        private System.Windows.Forms.ComboBox comboBox_面板內容_字體顏色_製造日期;
        private System.Windows.Forms.Button button_面板內容_字體_批號;
        private System.Windows.Forms.GroupBox groupBox11;
        private System.Windows.Forms.ComboBox comboBox_面板內容_字體顏色_批號;
        private System.Windows.Forms.Button button_面板內容_字體_藥名;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.ComboBox comboBox_面板內容_字體顏色_藥名;
        private System.Windows.Forms.Button button_面板內容_字體_包裝單位;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.ComboBox comboBox_面板內容_字體顏色_包裝單位;
        private System.Windows.Forms.Button button_面板內容_字體_效期;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.ComboBox comboBox_面板內容_字體顏色_效期;
        private System.Windows.Forms.Button button_面板內容_字體_藥碼;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox comboBox_面板內容_字體顏色_藥碼;
        private System.Windows.Forms.TextBox textBox_面板內容_製造日期;
        private System.Windows.Forms.TextBox textBox_面板內容_批號;
        private System.Windows.Forms.TextBox textBox_面板內容_藥名;
        private System.Windows.Forms.TextBox textBox_面板內容_包裝單位;
        private System.Windows.Forms.TextBox textBox_面板內容_效期;
        private System.Windows.Forms.TextBox textBox_面板內容_藥碼;
        private MyUI.RJ_Lable rJ_Lable6;
        private MyUI.RJ_Lable rJ_Lable5;
        private MyUI.RJ_Lable rJ_Lable4;
        private MyUI.RJ_Lable rJ_Lable3;
        private MyUI.RJ_Lable rJ_Lable2;
        private MyUI.RJ_Lable rJ_Lable1;
        private SQLUI.SQL_DataGridView sqL_DataGridView_面板列表;
        private MyUI.RJ_Button rJ_Button_面板列表_新增;
        private MyUI.RJ_Button rJ_Button_面板列表_刪除;
        private System.Windows.Forms.TextBox textBox_面板內容_GUID;
        private MyUI.RJ_Button rJ_Button_重繪;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox comboBox_面板內容_背景顏色;
        private System.Windows.Forms.FontDialog fontDialog;
        private System.Windows.Forms.Button button_面板內容_字體_廠牌;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ComboBox comboBox_面板內容_字體顏色_廠牌;
        private System.Windows.Forms.TextBox textBox_面板內容_廠牌;
        private MyUI.RJ_Lable rJ_Lable7;
    }
}

