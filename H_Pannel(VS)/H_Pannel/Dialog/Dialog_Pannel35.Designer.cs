
namespace H_Pannel_lib
{
    partial class Dialog_Pannel35
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
            this.wT32_GPADC = new H_Pannel_lib.WT32_GPADC();
            this.rJ_Button_退出 = new MyUI.RJ_Button();
            this.SuspendLayout();
            // 
            // wT32_GPADC
            // 
            this.wT32_GPADC.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.wT32_GPADC.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.wT32_GPADC.Cursor = System.Windows.Forms.Cursors.Default;
            this.wT32_GPADC.Dock = System.Windows.Forms.DockStyle.Left;
            this.wT32_GPADC.Location = new System.Drawing.Point(0, 0);
            this.wT32_GPADC.MouseDownType = H_Pannel_lib.WT32_GPADC.TxMouseDownType.NONE;
            this.wT32_GPADC.Name = "wT32_GPADC";
            this.wT32_GPADC.Pannel_Green_Visible = false;
            this.wT32_GPADC.Pannel_Lock_Visible = false;
            this.wT32_GPADC.Pannel_Red_Visible = false;
            this.wT32_GPADC.Size = new System.Drawing.Size(1048, 964);
            this.wT32_GPADC.TabIndex = 0;
            // 
            // rJ_Button_退出
            // 
            this.rJ_Button_退出.BackColor = System.Drawing.Color.Gray;
            this.rJ_Button_退出.BackgroundColor = System.Drawing.Color.Gray;
            this.rJ_Button_退出.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.rJ_Button_退出.BorderRadius = 5;
            this.rJ_Button_退出.BorderSize = 0;
            this.rJ_Button_退出.buttonType = MyUI.RJ_Button.ButtonType.Push;
            this.rJ_Button_退出.FlatAppearance.BorderSize = 0;
            this.rJ_Button_退出.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Button_退出.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_Button_退出.ForeColor = System.Drawing.Color.White;
            this.rJ_Button_退出.Location = new System.Drawing.Point(1052, 881);
            this.rJ_Button_退出.Name = "rJ_Button_退出";
            this.rJ_Button_退出.Size = new System.Drawing.Size(146, 78);
            this.rJ_Button_退出.State = false;
            this.rJ_Button_退出.TabIndex = 12;
            this.rJ_Button_退出.Text = "退出";
            this.rJ_Button_退出.TextColor = System.Drawing.Color.White;
            this.rJ_Button_退出.UseVisualStyleBackColor = false;
            // 
            // Dialog_Pannel35
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1201, 964);
            this.ControlBox = false;
            this.Controls.Add(this.rJ_Button_退出);
            this.Controls.Add(this.wT32_GPADC);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "Dialog_Pannel35";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.Dialog_Pannel35_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private WT32_GPADC wT32_GPADC;
        private MyUI.RJ_Button rJ_Button_退出;
    }
}