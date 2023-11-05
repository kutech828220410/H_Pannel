using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Basic;

namespace EPD_Upload
{
    public partial class Dialog_藥碼輸入 : Form
    {
        public string Value
        {
            get
            {
                return this.rJ_TextBox_Value.Texts;
            }
        }
        public static Form form;
        public DialogResult ShowDialog()
        {
            if (form == null)
            {
                base.ShowDialog();
            }
            else
            {
                form.Invoke(new Action(delegate
                {
                    base.ShowDialog();
                }));
            }

            return this.DialogResult;
        }

        public Dialog_藥碼輸入()
        {
            InitializeComponent();
            this.Load += Dialog_藥碼輸入_Load;
        }

        private void Dialog_藥碼輸入_Load(object sender, EventArgs e)
        {
            rJ_Button_確認.MouseDownEvent += RJ_Button_確認_MouseDownEvent;
            rJ_TextBox_Value.KeyPress += RJ_TextBox_Value_KeyPress;
        }

        private void RJ_TextBox_Value_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                RJ_Button_確認_MouseDownEvent(null);
            }
        }

        private void RJ_Button_確認_MouseDownEvent(MouseEventArgs mevent)
        {
            if(this.rJ_TextBox_Value.Texts == "")
            {
                MyMessageBox.ShowDialog("未輸入藥碼");
                return;
            }
            this.Invoke(new Action(delegate
            {
                this.DialogResult = DialogResult.Yes;
                this.Close();
                return;
            }));
        }
    }
}
