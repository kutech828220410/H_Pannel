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
using MyUI;

namespace H_Pannel_lib
{
    public partial class Dialog_燈數設定 : Form
    {
        MyThread myThread_program;
        private int Num_buf = 0;
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
        public int Value
        {
            get
            {
                return this.plC_NumBox_燈數.Value;
            }
            set
            {
                this.plC_NumBox_燈數.Value = value;
            }
        }
        public Dialog_燈數設定(int Num)
        {
            InitializeComponent();
            this.Num_buf = Num;
        }

        private void Dialog_燈數設定_Load(object sender, EventArgs e)
        {
            this.rJ_Button_退出.MouseDownEvent += RJ_Button_退出_MouseDownEvent;
            this.myThread_program = new MyThread();
            this.myThread_program.Add_Method(sub_program);
            this.myThread_program.AutoRun(true);
            this.myThread_program.SetSleepTime(10);
            this.myThread_program.Trigger();
            this.plC_NumBox_燈數.Value = Num_buf;
            MyUI.數字鍵盤.音效 = false;
        }
        private void Dialog_燈數設定_Shown(object sender, EventArgs e)
        {
           
        }
        private void RJ_Button_退出_MouseDownEvent(MouseEventArgs mevent)
        {
            this.myThread_program.Abort();
            this.myThread_program = null;
            this.Invoke(new Action(delegate
            {
                this.DialogResult = DialogResult.Yes;
                this.Close();
            }));
        }
        private void sub_program()
        {
            this.plC_NumBox_燈數.Run();
        }


    }
}
