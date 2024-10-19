
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
            this.SuspendLayout();
            // 
            // panel_paint
            // 
            this.panel_paint.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel_paint.Location = new System.Drawing.Point(41, 47);
            this.panel_paint.Name = "panel_paint";
            this.panel_paint.Size = new System.Drawing.Size(600, 600);
            this.panel_paint.TabIndex = 0;
            // 
            // Dialog_DrawerHandSensorCheck
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CaptionHeight = 40;
            this.ClientSize = new System.Drawing.Size(693, 675);
            this.CloseBoxSize = new System.Drawing.Size(40, 40);
            this.ControlBox = true;
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
    }
}