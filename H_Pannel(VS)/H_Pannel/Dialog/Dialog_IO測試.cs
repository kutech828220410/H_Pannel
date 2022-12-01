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
    public partial class Dialog_IO測試 : Form
    {
        MyThread myThread_program;
        private UDP_Class udP_Class;
        private string IP = "";
        private List<object[]> list_UDP_Rx = new List<object[]>();
        private bool flag_output_buf = false;

        [Serializable]
        public class UDP_READ
        {
            MyConvert myConvert = new MyConvert();
            private string iP = "0.0.0.0";
            private int port = 0;
            private string version = "";
            private int input = 0;
            private int output = 0;
            private int rSSI = -100;
            private int input_dir = 0;
            private int output_dir = 0;

            public string IP { get => iP; set => iP = value; }
            public int Port { get => port; set => port = value; }
            public string Version { get => version; set => version = value; }
            public int Input { get => input; set => input = value; }
            public int Output { get => output; set => output = value; }
            public int RSSI { get => rSSI; set => rSSI = value; }
            public int Input_dir { get => input_dir; set => input_dir = value; }
            public int Output_dir { get => output_dir; set => output_dir = value; }


            public bool Get_Input_dir(int index)
            {
                return this.myConvert.Int32GetBit(Input_dir, index);
            }
            public bool Get_Output_dir(int index)
            {
                return this.myConvert.Int32GetBit(Output_dir, index);
            }
            public bool Get_Input(int index)
            {
                return this.myConvert.Int32GetBit(Input, index);
            }
            public bool Get_Output(int index)
            {
                return this.myConvert.Int32GetBit(Output, index);
            }

        }

        public Dialog_IO測試(UDP_Class udP_Class, string IP, List<object[]> list_UDP_Rx)
        {
            InitializeComponent();
            this.udP_Class = udP_Class;
            this.IP = IP;
            this.list_UDP_Rx = list_UDP_Rx;
        }

        private void Dialog_IO測試_Load(object sender, EventArgs e)
        {
            this.rJ_Button_退出.MouseDownEvent += RJ_Button_退出_MouseDownEvent;

            this.rJ_Button_鎖控輸出.MouseDownEvent += RJ_Button_鎖控輸出_MouseDownEvent;
            this.checkBox_輸入反向.Click += CheckBox_輸入反向_Click;
            this.checkBox_輸出反向.Click += CheckBox_輸出反向_Click;
            this.checkBox_輸出狀態.Click += CheckBox_輸出狀態_Click;
            this.myThread_program = new MyThread();
            this.myThread_program.Add_Method(sub_program);
            this.myThread_program.AutoRun(true);
            this.myThread_program.SetSleepTime(10);
            this.myThread_program.Trigger();
        }



        private void sub_program()
        {
            list_UDP_Rx = list_UDP_Rx.GetRows((int)enum_UDP_DataReceive.IP, IP);
            if (list_UDP_Rx.Count == 0) return;
            string jsonString = list_UDP_Rx[0][(int)enum_UDP_DataReceive.Readline].ObjectToString();
            UDP_READ uDP_READ = jsonString.JsonDeserializet<UDP_READ>();
            if (uDP_READ == null) return;
            this.Invoke(new Action(delegate
            {
                bool flag_output;
                bool flag_input;
                bool flag_input_dir;
                bool flag_output_dir;
                bool flag_rfid_enable;
                flag_output = uDP_READ.Get_Output(0);
                flag_input = uDP_READ.Get_Input(0);
                flag_input_dir = uDP_READ.Get_Input_dir(0);
                flag_output_dir = uDP_READ.Get_Output_dir(0);
                if (flag_output_buf != flag_output)
                {
                    flag_output_buf = flag_output;
                    if (flag_output_buf)
                    {
                        this.rJ_Button_鎖控輸出.BackColor = Color.Black;
                        this.rJ_Button_鎖控輸出.ForeColor = Color.White;
                    }
                    else
                    {
                        this.rJ_Button_鎖控輸出.BackColor = Color.RoyalBlue;
                        this.rJ_Button_鎖控輸出.ForeColor = Color.White;
                    }
                }
                if (this.checkBox_輸入狀態.Checked != flag_input)
                {
                    this.checkBox_輸入狀態.Checked = flag_input;    
                }
                if(this.checkBox_輸入反向.Checked != flag_input_dir)
                {
                    this.checkBox_輸入反向.Checked = flag_input_dir;
                }
                if (this.checkBox_輸出狀態.Checked != flag_output)
                {
                    this.checkBox_輸出狀態.Checked = flag_output;
                }
                if (this.checkBox_輸出反向.Checked != flag_output_dir)
                {
                    this.checkBox_輸出反向.Checked = flag_output_dir;
                }
            }));
        }
        private void CheckBox_輸入反向_Click(object sender, EventArgs e)
        {
            Communication.Set_Input_dir(udP_Class, IP, 1, checkBox_輸入反向.Checked);
        }
        private void CheckBox_輸出反向_Click(object sender, EventArgs e)
        {
            Communication.Set_Output_dir(udP_Class, IP, 1, checkBox_輸出反向.Checked);
        }
        private void CheckBox_輸出狀態_Click(object sender, EventArgs e)
        {
            Communication.Set_OutputPIN(udP_Class, IP, 1, checkBox_輸出狀態.Checked);
        }
        private void RJ_Button_鎖控輸出_MouseDownEvent(MouseEventArgs mevent)
        {
            Communication.Set_OutputPINTrigger(udP_Class, IP, 1, true);
        }
        private void RJ_Button_退出_MouseDownEvent(MouseEventArgs mevent)
        {
            this.myThread_program.Abort();
            this.myThread_program = null;
            this.Invoke(new Action(delegate 
            {
                this.Close();
            }));
        }
    }
}
