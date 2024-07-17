using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace H_Pannel_lib
{
    public partial class Dialog_選取顯示網段 : Form
    {
        public string Value
        {
            get
            {
                return this.rJ_ComboBox.Texts;
            }
        }

        public Dialog_選取顯示網段(object dataSource)
        {
            InitializeComponent();
            this.rJ_ComboBox.DataSource = dataSource;
            this.rJ_Button_Cancel.MouseDownEvent += RJ_Button_Cancel_MouseDownEvent;
            this.rJ_Button_OK.MouseDownEvent += RJ_Button_OK_MouseDownEvent;
        }

        private void RJ_Button_OK_MouseDownEvent(MouseEventArgs mevent)
        {
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }
        private void RJ_Button_Cancel_MouseDownEvent(MouseEventArgs mevent)
        {
            this.DialogResult = DialogResult.No;
            this.Close();
        }

        private void rJ_Button_OK_Click(object sender, EventArgs e)
        {
     
        }
        private void rJ_Button_Cancel_Click(object sender, EventArgs e)
        {
       
        }
    }
}
