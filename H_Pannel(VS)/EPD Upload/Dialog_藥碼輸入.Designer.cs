
namespace EPD_Upload
{
    partial class Dialog_藥碼輸入
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.rJ_TextBox_Value = new MyUI.RJ_TextBox();
            this.rJ_Button_確認 = new MyUI.RJ_Button();
            this.SuspendLayout();
            // 
            // rJ_TextBox_Value
            // 
            this.rJ_TextBox_Value.BackColor = System.Drawing.SystemColors.Window;
            this.rJ_TextBox_Value.BorderColor = System.Drawing.Color.DarkBlue;
            this.rJ_TextBox_Value.BorderFocusColor = System.Drawing.Color.HotPink;
            this.rJ_TextBox_Value.BorderRadius = 0;
            this.rJ_TextBox_Value.BorderSize = 2;
            this.rJ_TextBox_Value.Font = new System.Drawing.Font("微軟正黑體", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_TextBox_Value.ForeColor = System.Drawing.Color.DimGray;
            this.rJ_TextBox_Value.GUID = "";
            this.rJ_TextBox_Value.Location = new System.Drawing.Point(24, 12);
            this.rJ_TextBox_Value.Multiline = false;
            this.rJ_TextBox_Value.Name = "rJ_TextBox_Value";
            this.rJ_TextBox_Value.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.rJ_TextBox_Value.PassWordChar = false;
            this.rJ_TextBox_Value.PlaceholderColor = System.Drawing.Color.DarkGray;
            this.rJ_TextBox_Value.PlaceholderText = "請輸入藥碼...";
            this.rJ_TextBox_Value.ShowTouchPannel = false;
            this.rJ_TextBox_Value.Size = new System.Drawing.Size(362, 50);
            this.rJ_TextBox_Value.TabIndex = 0;
            this.rJ_TextBox_Value.TextAlgin = System.Windows.Forms.HorizontalAlignment.Left;
            this.rJ_TextBox_Value.Texts = "";
            this.rJ_TextBox_Value.UnderlineStyle = false;
            // 
            // rJ_Button_確認
            // 
            this.rJ_Button_確認.AutoResetState = false;
            this.rJ_Button_確認.BackColor = System.Drawing.Color.RoyalBlue;
            this.rJ_Button_確認.BackgroundColor = System.Drawing.Color.RoyalBlue;
            this.rJ_Button_確認.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.rJ_Button_確認.BorderRadius = 5;
            this.rJ_Button_確認.BorderSize = 0;
            this.rJ_Button_確認.buttonType = MyUI.RJ_Button.ButtonType.Push;
            this.rJ_Button_確認.FlatAppearance.BorderSize = 0;
            this.rJ_Button_確認.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Button_確認.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_Button_確認.ForeColor = System.Drawing.Color.White;
            this.rJ_Button_確認.GUID = "";
            this.rJ_Button_確認.Location = new System.Drawing.Point(398, 12);
            this.rJ_Button_確認.Name = "rJ_Button_確認";
            this.rJ_Button_確認.Size = new System.Drawing.Size(96, 51);
            this.rJ_Button_確認.State = false;
            this.rJ_Button_確認.TabIndex = 1;
            this.rJ_Button_確認.Text = "確認";
            this.rJ_Button_確認.TextColor = System.Drawing.Color.White;
            this.rJ_Button_確認.UseVisualStyleBackColor = false;
            // 
            // Dialog_藥碼輸入
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(507, 78);
            this.Controls.Add(this.rJ_Button_確認);
            this.Controls.Add(this.rJ_TextBox_Value);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Dialog_藥碼輸入";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "請輸入藥碼...";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        private MyUI.RJ_TextBox rJ_TextBox_Value;
        private MyUI.RJ_Button rJ_Button_確認;
    }
}