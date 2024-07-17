
namespace H_Pannel_lib
{
    partial class Dialog_燈數設定
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
            this.rJ_Lable1 = new MyUI.RJ_Lable();
            this.rJ_Button_退出 = new MyUI.RJ_Button();
            this.plC_NumBox_燈數 = new MyUI.PLC_NumBox();
            this.SuspendLayout();
            // 
            // rJ_Lable1
            // 
            this.rJ_Lable1.BackColor = System.Drawing.Color.White;
            this.rJ_Lable1.BackgroundColor = System.Drawing.Color.RoyalBlue;
            this.rJ_Lable1.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.rJ_Lable1.BorderRadius = 10;
            this.rJ_Lable1.BorderSize = 0;
            this.rJ_Lable1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Lable1.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_Lable1.ForeColor = System.Drawing.Color.White;
            this.rJ_Lable1.GUID = "";
            this.rJ_Lable1.Location = new System.Drawing.Point(12, 17);
            this.rJ_Lable1.Name = "rJ_Lable1";
            this.rJ_Lable1.ShadowColor = System.Drawing.Color.DimGray;
            this.rJ_Lable1.ShadowSize = 0;
            this.rJ_Lable1.Size = new System.Drawing.Size(105, 46);
            this.rJ_Lable1.TabIndex = 0;
            this.rJ_Lable1.Text = "燈 數";
            this.rJ_Lable1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rJ_Lable1.TextColor = System.Drawing.Color.White;
            // 
            // rJ_Button_退出
            // 
            this.rJ_Button_退出.AutoResetState = false;
            this.rJ_Button_退出.BackColor = System.Drawing.Color.Transparent;
            this.rJ_Button_退出.BackgroundColor = System.Drawing.Color.Gray;
            this.rJ_Button_退出.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.rJ_Button_退出.BorderRadius = 10;
            this.rJ_Button_退出.BorderSize = 0;
            this.rJ_Button_退出.buttonType = MyUI.RJ_Button.ButtonType.Push;
            this.rJ_Button_退出.DisenableColor = System.Drawing.Color.Gray;
            this.rJ_Button_退出.FlatAppearance.BorderSize = 0;
            this.rJ_Button_退出.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Button_退出.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_Button_退出.ForeColor = System.Drawing.Color.White;
            this.rJ_Button_退出.GUID = "";
            this.rJ_Button_退出.Image_padding = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.rJ_Button_退出.Location = new System.Drawing.Point(269, 15);
            this.rJ_Button_退出.Name = "rJ_Button_退出";
            this.rJ_Button_退出.ProhibitionBorderLineWidth = 1;
            this.rJ_Button_退出.ProhibitionLineWidth = 4;
            this.rJ_Button_退出.ProhibitionSymbolSize = 30;
            this.rJ_Button_退出.ShadowColor = System.Drawing.Color.DimGray;
            this.rJ_Button_退出.ShadowSize = 0;
            this.rJ_Button_退出.ShowLoadingForm = false;
            this.rJ_Button_退出.Size = new System.Drawing.Size(106, 51);
            this.rJ_Button_退出.State = false;
            this.rJ_Button_退出.TabIndex = 12;
            this.rJ_Button_退出.Text = "退出";
            this.rJ_Button_退出.TextColor = System.Drawing.Color.White;
            this.rJ_Button_退出.TextHeight = 0;
            this.rJ_Button_退出.UseVisualStyleBackColor = false;
            // 
            // plC_NumBox_燈數
            // 
            this.plC_NumBox_燈數.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.plC_NumBox_燈數.Location = new System.Drawing.Point(135, 25);
            this.plC_NumBox_燈數.mBackColor = System.Drawing.SystemColors.Window;
            this.plC_NumBox_燈數.mForeColor = System.Drawing.SystemColors.WindowText;
            this.plC_NumBox_燈數.Name = "plC_NumBox_燈數";
            this.plC_NumBox_燈數.ReadOnly = false;
            this.plC_NumBox_燈數.Size = new System.Drawing.Size(110, 35);
            this.plC_NumBox_燈數.TabIndex = 13;
            this.plC_NumBox_燈數.Value = 0;
            this.plC_NumBox_燈數.字元長度 = MyUI.PLC_NumBox.WordLengthEnum.單字元;
            this.plC_NumBox_燈數.密碼欄位 = false;
            this.plC_NumBox_燈數.小數點位置 = 0;
            this.plC_NumBox_燈數.微調數值 = 1;
            this.plC_NumBox_燈數.音效 = true;
            this.plC_NumBox_燈數.顯示微調按鈕 = false;
            this.plC_NumBox_燈數.顯示螢幕小鍵盤 = true;
            // 
            // Dialog_燈數設定
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(394, 76);
            this.ControlBox = false;
            this.Controls.Add(this.plC_NumBox_燈數);
            this.Controls.Add(this.rJ_Button_退出);
            this.Controls.Add(this.rJ_Lable1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "Dialog_燈數設定";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.Dialog_燈數設定_Load);
            this.Shown += new System.EventHandler(this.Dialog_燈數設定_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private MyUI.RJ_Lable rJ_Lable1;
        private MyUI.RJ_Button rJ_Button_退出;
        private MyUI.PLC_NumBox plC_NumBox_燈數;
    }
}