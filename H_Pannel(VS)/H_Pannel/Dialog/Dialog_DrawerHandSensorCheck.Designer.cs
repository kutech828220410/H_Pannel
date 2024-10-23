
namespace H_Pannel_lib
{
    partial class Dialog_DrawerHandSensorCheck
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
            this.panel_paint = new System.Windows.Forms.Panel();
            this.rJ_Lable_雷射距離 = new MyUI.RJ_Lable();
            this.SuspendLayout();
            // 
            // panel_paint
            // 
            this.panel_paint.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel_paint.Location = new System.Drawing.Point(42, 92);
            this.panel_paint.Name = "panel_paint";
            this.panel_paint.Size = new System.Drawing.Size(600, 600);
            this.panel_paint.TabIndex = 0;
            // 
            // rJ_Lable_雷射距離
            // 
            this.rJ_Lable_雷射距離.BackColor = System.Drawing.Color.White;
            this.rJ_Lable_雷射距離.BackgroundColor = System.Drawing.Color.Red;
            this.rJ_Lable_雷射距離.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.rJ_Lable_雷射距離.BorderRadius = 10;
            this.rJ_Lable_雷射距離.BorderSize = 0;
            this.rJ_Lable_雷射距離.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Lable_雷射距離.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rJ_Lable_雷射距離.ForeColor = System.Drawing.Color.Transparent;
            this.rJ_Lable_雷射距離.GUID = "";
            this.rJ_Lable_雷射距離.Location = new System.Drawing.Point(40, 34);
            this.rJ_Lable_雷射距離.Name = "rJ_Lable_雷射距離";
            this.rJ_Lable_雷射距離.ShadowColor = System.Drawing.Color.DimGray;
            this.rJ_Lable_雷射距離.ShadowSize = 0;
            this.rJ_Lable_雷射距離.Size = new System.Drawing.Size(178, 50);
            this.rJ_Lable_雷射距離.TabIndex = 1;
            this.rJ_Lable_雷射距離.Text = "0";
            this.rJ_Lable_雷射距離.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rJ_Lable_雷射距離.TextColor = System.Drawing.Color.White;
            // 
            // Dialog_DrawerHandSensorCheck
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CaptionHeight = 40;
            this.ClientSize = new System.Drawing.Size(693, 708);
            this.CloseBoxSize = new System.Drawing.Size(40, 40);
            this.ControlBox = true;
            this.Controls.Add(this.rJ_Lable_雷射距離);
            this.Controls.Add(this.panel_paint);
            this.MaximizeBox = false;
            this.MaxSize = new System.Drawing.Size(40, 40);
            this.MiniSize = new System.Drawing.Size(40, 40);
            this.Name = "Dialog_DrawerHandSensorCheck";
            this.Text = "手勢感測檢查";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel_paint;
        private MyUI.RJ_Lable rJ_Lable_雷射距離;
    }
}