
namespace H_Pannel_lib
{
    partial class EPD_583_Pannel
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

        #region 元件設計工具產生的程式碼

        /// <summary> 
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.saveFileDialog_bmp = new System.Windows.Forms.SaveFileDialog();
            this.saveFileDialog_jpg = new System.Windows.Forms.SaveFileDialog();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // saveFileDialog_bmp
            // 
            this.saveFileDialog_bmp.DefaultExt = "bmp";
            this.saveFileDialog_bmp.Filter = "bmp File (*.bmp)|*.bmp;";
            // 
            // saveFileDialog_jpg
            // 
            this.saveFileDialog_jpg.DefaultExt = "jpg";
            this.saveFileDialog_jpg.Filter = "jpg File (*.jpg)|*.jpg;";
            // 
            // pictureBox
            // 
            this.pictureBox.BackColor = System.Drawing.SystemColors.Info;
            this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox.Location = new System.Drawing.Point(0, 0);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(648, 480);
            this.pictureBox.TabIndex = 23;
            this.pictureBox.TabStop = false;
            // 
            // EPD_583_Pannel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pictureBox);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "EPD_583_Pannel";
            this.Size = new System.Drawing.Size(648, 480);
            this.Load += new System.EventHandler(this.EPD_583_Pannel_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.SaveFileDialog saveFileDialog_bmp;
        private System.Windows.Forms.SaveFileDialog saveFileDialog_jpg;
        private System.Windows.Forms.PictureBox pictureBox;
    }
}
