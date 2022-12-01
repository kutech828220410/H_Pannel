
namespace H_Pannel_lib
{
    partial class Dialog_RowsLED
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
            this.rJ_Button_OK = new MyUI.RJ_Button();
            this.rJ_TrackBar = new MyUI.RJ_TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // rJ_Button_OK
            // 
            this.rJ_Button_OK.BackColor = System.Drawing.Color.RoyalBlue;
            this.rJ_Button_OK.BackgroundColor = System.Drawing.Color.RoyalBlue;
            this.rJ_Button_OK.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.rJ_Button_OK.BorderRadius = 5;
            this.rJ_Button_OK.BorderSize = 0;
            this.rJ_Button_OK.buttonType = MyUI.RJ_Button.ButtonType.Push;
            this.rJ_Button_OK.FlatAppearance.BorderSize = 0;
            this.rJ_Button_OK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rJ_Button_OK.ForeColor = System.Drawing.Color.White;
            this.rJ_Button_OK.Location = new System.Drawing.Point(548, 35);
            this.rJ_Button_OK.Name = "rJ_Button_OK";
            this.rJ_Button_OK.Size = new System.Drawing.Size(76, 40);
            this.rJ_Button_OK.State = false;
            this.rJ_Button_OK.TabIndex = 15;
            this.rJ_Button_OK.Text = "OK";
            this.rJ_Button_OK.TextColor = System.Drawing.Color.White;
            this.rJ_Button_OK.UseVisualStyleBackColor = false;
            this.rJ_Button_OK.Click += new System.EventHandler(this.rJ_Button_OK_Click);
            // 
            // rJ_TrackBar
            // 
            this.rJ_TrackBar.BarColor = System.Drawing.Color.LightGray;
            this.rJ_TrackBar.BarSize = 50;
            this.rJ_TrackBar.BottomSliderColor = System.Drawing.Color.Red;
            this.rJ_TrackBar.Location = new System.Drawing.Point(40, 19);
            this.rJ_TrackBar.Maximum = 100;
            this.rJ_TrackBar.MaxValue = 0;
            this.rJ_TrackBar.Minimum = 0;
            this.rJ_TrackBar.MinValue = 0;
            this.rJ_TrackBar.Name = "rJ_TrackBar";
            this.rJ_TrackBar.Size = new System.Drawing.Size(493, 70);
            this.rJ_TrackBar.SliderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.rJ_TrackBar.SliderSize = 10;
            this.rJ_TrackBar.TabIndex = 16;
            this.rJ_TrackBar.Text = "rJ_TrackBar";
            this.rJ_TrackBar.TopSliderColor = System.Drawing.Color.Red;
            this.rJ_TrackBar.ValueChanged += new MyUI.RJ_TrackBar.ValueChangedEventHandler(this.rJ_TrackBar_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 77);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(24, 12);
            this.label2.TabIndex = 18;
            this.label2.Text = "Min";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 12);
            this.label1.TabIndex = 17;
            this.label1.Text = "Max";
            // 
            // Dialog_RowsLED
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(639, 109);
            this.ControlBox = false;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rJ_TrackBar);
            this.Controls.Add(this.rJ_Button_OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "Dialog_RowsLED";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private MyUI.RJ_Button rJ_Button_OK;
        private MyUI.RJ_TrackBar rJ_TrackBar;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}