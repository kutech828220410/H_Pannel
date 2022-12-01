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
    public partial class Dialog_Pannel35 : Form
    {
        public Storage CurrentStorage;
        public List<UDP_Class> list_UDP_Local;
        public Dialog_Pannel35(Storage storage , List<UDP_Class> List_UDP_Local)
        {
            InitializeComponent();
            this.CurrentStorage = storage;
            this.list_UDP_Local = List_UDP_Local;
        }

        private void Dialog_Pannel35_Load(object sender, EventArgs e)
        {
            this.rJ_Button_退出.MouseDownEvent += RJ_Button_退出_MouseDownEvent;
            this.wT32_GPADC.Init(this.list_UDP_Local);
            this.wT32_GPADC.Set_Stroage(this.CurrentStorage);
        }

        private void RJ_Button_退出_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                this.wT32_GPADC.Dispose();
                this.Close();
            }));
        }
    }
}
