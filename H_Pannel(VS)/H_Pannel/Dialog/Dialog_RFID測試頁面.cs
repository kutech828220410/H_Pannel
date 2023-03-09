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
namespace H_Pannel_lib
{
    public partial class Dialog_RFID測試頁面 : Form
    {
        public enum enum_RFID_Type
        {
            Normal,
            HighFQ,
        }
        public enum_RFID_Type RFID_Type = enum_RFID_Type.Normal;
        private UDP_Class udP_Class;
        private string IP = "";
        private List<object[]> list_UDP_Rx = new List<object[]>();
        private List<MyUI.RJ_Button> rJ_Buttons_鎖控輸出 = new List<MyUI.RJ_Button>();
        private List<MyUI.RJ_TextBox> rJ_TextBoxes_CardD = new List<MyUI.RJ_TextBox>();
        private List<CheckBox> checkBoxes_輸入狀態 = new List<CheckBox>();
        private List<CheckBox> checkBoxes_輸入方向 = new List<CheckBox>();
        private List<CheckBox> checkBoxes_輸出狀態 = new List<CheckBox>();
        private List<CheckBox> checkBoxes_輸出方向 = new List<CheckBox>();
        private List<CheckBox> checkBoxes_RFID_Enable = new List<CheckBox>();

        private bool[] flag_output_buf = new bool[10];
        MyThread myThread_program;
        public Dialog_RFID測試頁面(UDP_Class udP_Class , string IP ,List<object[]> list_UDP_Rx)
        {
            InitializeComponent();
            this.udP_Class = udP_Class;
            this.IP = IP;
            this.list_UDP_Rx = list_UDP_Rx;
       
        }
        private void sub_program()
        {
            list_UDP_Rx = list_UDP_Rx.GetRows((int)RFID_UI.enum_UDP_DataReceive.IP, IP);
            if (list_UDP_Rx.Count == 0) return;
            string jsonString = list_UDP_Rx[0][(int)RFID_UI.enum_UDP_DataReceive.Readline].ObjectToString();
            RFID_UI.UDP_READ uDP_READ = jsonString.JsonDeserializet<RFID_UI.UDP_READ>();
            if (uDP_READ == null) return;
            this.Invoke(new Action(delegate
            {
                bool flag_output;
                bool flag_input;
                bool flag_input_dir;
                bool flag_output_dir;
                bool flag_rfid_enable;
                for (int i = 0; i < 10; i++)
                {
                    flag_output = uDP_READ.Get_Output(i);
                    if (flag_output_buf[i] != flag_output)
                    {
                        flag_output_buf[i] = flag_output;
                        if (flag_output_buf[i])
                        {
                            this.rJ_Buttons_鎖控輸出[i].BackColor = Color.Black;
                            this.rJ_Buttons_鎖控輸出[i].ForeColor = Color.White;
                        }
                        else
                        {
                            this.rJ_Buttons_鎖控輸出[i].BackColor = Color.RoyalBlue;
                            this.rJ_Buttons_鎖控輸出[i].ForeColor = Color.White;
                        }                    
                    }
                }
                for (int i = 0; i < 5; i++)
                {
                    flag_output = uDP_READ.Get_Output(i);
                    if (this.rJ_TextBoxes_CardD[i].Texts != uDP_READ.Get_CardID(i))
                    {
                        this.rJ_TextBoxes_CardD[i].Texts = uDP_READ.Get_CardID(i);
                    }
                }
                for (int i = 0; i < 5; i++)
                {
                    flag_rfid_enable = uDP_READ.Get_RFID_Enable(i);
                    if (checkBoxes_RFID_Enable[i].Checked != flag_rfid_enable)
                    {
                        checkBoxes_RFID_Enable[i].Checked = flag_rfid_enable;
                    }
                }
                for (int i = 0; i < 10; i++)
                {
                    flag_input = uDP_READ.Get_Input(i);
                    if (checkBoxes_輸入狀態[i].Checked != flag_input)
                    {
                        checkBoxes_輸入狀態[i].Checked = flag_input;
                    }
                }
                for (int i = 0; i < 10; i++)
                {
                    flag_output = uDP_READ.Get_Output(i);
                    if (checkBoxes_輸出狀態[i].Checked != flag_output)
                    {
                        checkBoxes_輸出狀態[i].Checked = flag_output;
                    }
                }
                for (int i = 0; i < 10; i++)
                {
                    flag_input_dir = uDP_READ.Get_Input_dir(i);
                    if (checkBoxes_輸入方向[i].Checked != flag_input_dir)
                    {
                        checkBoxes_輸入方向[i].Checked = flag_input_dir;
                    }
                }
                for (int i = 0; i < 10; i++)
                {
                    flag_output_dir = uDP_READ.Get_Output_dir(i);
                    if (checkBoxes_輸出方向[i].Checked != flag_output_dir)
                    {
                        checkBoxes_輸出方向[i].Checked = flag_output_dir;
                    }
                }
            }));
         
            
        }
        private void Dialog_RFID測試頁面_Load(object sender, EventArgs e)
        {
            this.rJ_Button_輸出01.MouseDownEvent += RJ_Button_輸出01_MouseDownEvent;
            this.rJ_Button_輸出02.MouseDownEvent += RJ_Button_輸出02_MouseDownEvent;
            this.rJ_Button_輸出03.MouseDownEvent += RJ_Button_輸出03_MouseDownEvent;
            this.rJ_Button_輸出04.MouseDownEvent += RJ_Button_輸出04_MouseDownEvent;
            this.rJ_Button_輸出05.MouseDownEvent += RJ_Button_輸出05_MouseDownEvent;
            this.rJ_Button_輸出06.MouseDownEvent += RJ_Button_輸出06_MouseDownEvent;
            this.rJ_Button_輸出07.MouseDownEvent += RJ_Button_輸出07_MouseDownEvent;
            this.rJ_Button_輸出08.MouseDownEvent += RJ_Button_輸出08_MouseDownEvent;
            this.rJ_Button_輸出09.MouseDownEvent += RJ_Button_輸出09_MouseDownEvent;
            this.rJ_Button_輸出10.MouseDownEvent += RJ_Button_輸出10_MouseDownEvent;

            this.rJ_Button_退出.MouseDownEvent += RJ_Button_退出_MouseDownEvent;

            this.rJ_Buttons_鎖控輸出.Add(this.rJ_Button_輸出01);
            this.rJ_Buttons_鎖控輸出.Add(this.rJ_Button_輸出02);
            this.rJ_Buttons_鎖控輸出.Add(this.rJ_Button_輸出03);
            this.rJ_Buttons_鎖控輸出.Add(this.rJ_Button_輸出04);
            this.rJ_Buttons_鎖控輸出.Add(this.rJ_Button_輸出05);
            this.rJ_Buttons_鎖控輸出.Add(this.rJ_Button_輸出06);
            this.rJ_Buttons_鎖控輸出.Add(this.rJ_Button_輸出07);
            this.rJ_Buttons_鎖控輸出.Add(this.rJ_Button_輸出08);
            this.rJ_Buttons_鎖控輸出.Add(this.rJ_Button_輸出09);
            this.rJ_Buttons_鎖控輸出.Add(this.rJ_Button_輸出10);

            this.rJ_TextBoxes_CardD.Add(rJ_TextBox_Card01_ID);
            this.rJ_TextBoxes_CardD.Add(rJ_TextBox_Card02_ID);
            this.rJ_TextBoxes_CardD.Add(rJ_TextBox_Card03_ID);
            this.rJ_TextBoxes_CardD.Add(rJ_TextBox_Card04_ID);
            this.rJ_TextBoxes_CardD.Add(rJ_TextBox_Card05_ID);

            this.checkBoxes_輸入狀態.Add(checkBox_輸入狀態01);
            this.checkBoxes_輸入狀態.Add(checkBox_輸入狀態02);
            this.checkBoxes_輸入狀態.Add(checkBox_輸入狀態03);
            this.checkBoxes_輸入狀態.Add(checkBox_輸入狀態04);
            this.checkBoxes_輸入狀態.Add(checkBox_輸入狀態05);
            this.checkBoxes_輸入狀態.Add(checkBox_輸入狀態06);
            this.checkBoxes_輸入狀態.Add(checkBox_輸入狀態07);
            this.checkBoxes_輸入狀態.Add(checkBox_輸入狀態08);
            this.checkBoxes_輸入狀態.Add(checkBox_輸入狀態09);
            this.checkBoxes_輸入狀態.Add(checkBox_輸入狀態10);

            this.checkBoxes_輸出狀態.Add(checkBox_輸出狀態01);
            this.checkBoxes_輸出狀態.Add(checkBox_輸出狀態02);
            this.checkBoxes_輸出狀態.Add(checkBox_輸出狀態03);
            this.checkBoxes_輸出狀態.Add(checkBox_輸出狀態04);
            this.checkBoxes_輸出狀態.Add(checkBox_輸出狀態05);
            this.checkBoxes_輸出狀態.Add(checkBox_輸出狀態06);
            this.checkBoxes_輸出狀態.Add(checkBox_輸出狀態07);
            this.checkBoxes_輸出狀態.Add(checkBox_輸出狀態08);
            this.checkBoxes_輸出狀態.Add(checkBox_輸出狀態09);
            this.checkBoxes_輸出狀態.Add(checkBox_輸出狀態10);

            this.checkBoxes_輸入方向.Add(checkBox_輸入方向01);
            this.checkBoxes_輸入方向.Add(checkBox_輸入方向02);
            this.checkBoxes_輸入方向.Add(checkBox_輸入方向03);
            this.checkBoxes_輸入方向.Add(checkBox_輸入方向04);
            this.checkBoxes_輸入方向.Add(checkBox_輸入方向05);
            this.checkBoxes_輸入方向.Add(checkBox_輸入方向06);
            this.checkBoxes_輸入方向.Add(checkBox_輸入方向07);
            this.checkBoxes_輸入方向.Add(checkBox_輸入方向08);
            this.checkBoxes_輸入方向.Add(checkBox_輸入方向09);
            this.checkBoxes_輸入方向.Add(checkBox_輸入方向10);

            this.checkBoxes_輸出方向.Add(checkBox_輸出方向01);
            this.checkBoxes_輸出方向.Add(checkBox_輸出方向02);
            this.checkBoxes_輸出方向.Add(checkBox_輸出方向03);
            this.checkBoxes_輸出方向.Add(checkBox_輸出方向04);
            this.checkBoxes_輸出方向.Add(checkBox_輸出方向05);
            this.checkBoxes_輸出方向.Add(checkBox_輸出方向06);
            this.checkBoxes_輸出方向.Add(checkBox_輸出方向07);
            this.checkBoxes_輸出方向.Add(checkBox_輸出方向08);
            this.checkBoxes_輸出方向.Add(checkBox_輸出方向09);
            this.checkBoxes_輸出方向.Add(checkBox_輸出方向10);

            this.checkBoxes_RFID_Enable.Add(checkBox_RFID_Enable_01);
            this.checkBoxes_RFID_Enable.Add(checkBox_RFID_Enable_02);
            this.checkBoxes_RFID_Enable.Add(checkBox_RFID_Enable_03);
            this.checkBoxes_RFID_Enable.Add(checkBox_RFID_Enable_04);
            this.checkBoxes_RFID_Enable.Add(checkBox_RFID_Enable_05);

            this.checkBox_輸出狀態01.Click += checkBox_輸出狀態01_Click;
            this.checkBox_輸出狀態02.Click += checkBox_輸出狀態02_Click;
            this.checkBox_輸出狀態03.Click += checkBox_輸出狀態03_Click;
            this.checkBox_輸出狀態04.Click += checkBox_輸出狀態04_Click;
            this.checkBox_輸出狀態05.Click += checkBox_輸出狀態05_Click;
            this.checkBox_輸出狀態06.Click += checkBox_輸出狀態06_Click;
            this.checkBox_輸出狀態07.Click += checkBox_輸出狀態07_Click;
            this.checkBox_輸出狀態08.Click += checkBox_輸出狀態08_Click;
            this.checkBox_輸出狀態09.Click += checkBox_輸出狀態09_Click;
            this.checkBox_輸出狀態10.Click += checkBox_輸出狀態10_Click;

            this.checkBox_輸入方向01.Click += CheckBox_輸入方向01_Click;
            this.checkBox_輸入方向02.Click += CheckBox_輸入方向02_Click;
            this.checkBox_輸入方向03.Click += CheckBox_輸入方向03_Click;
            this.checkBox_輸入方向04.Click += CheckBox_輸入方向04_Click;
            this.checkBox_輸入方向05.Click += CheckBox_輸入方向05_Click;
            this.checkBox_輸入方向06.Click += CheckBox_輸入方向06_Click;
            this.checkBox_輸入方向07.Click += CheckBox_輸入方向07_Click;
            this.checkBox_輸入方向08.Click += CheckBox_輸入方向08_Click;
            this.checkBox_輸入方向09.Click += CheckBox_輸入方向09_Click;
            this.checkBox_輸入方向10.Click += CheckBox_輸入方向10_Click;

            this.checkBox_輸出方向01.Click += CheckBox_輸出方向01_Click;
            this.checkBox_輸出方向02.Click += CheckBox_輸出方向02_Click;
            this.checkBox_輸出方向03.Click += CheckBox_輸出方向03_Click;
            this.checkBox_輸出方向04.Click += CheckBox_輸出方向04_Click;
            this.checkBox_輸出方向05.Click += CheckBox_輸出方向05_Click;
            this.checkBox_輸出方向06.Click += CheckBox_輸出方向06_Click;
            this.checkBox_輸出方向07.Click += CheckBox_輸出方向07_Click;
            this.checkBox_輸出方向08.Click += CheckBox_輸出方向08_Click;
            this.checkBox_輸出方向09.Click += CheckBox_輸出方向09_Click;
            this.checkBox_輸出方向10.Click += CheckBox_輸出方向10_Click;

            this.checkBox_RFID_Enable_01.Click += CheckBox_RFID_Enable_01_Click;
            this.checkBox_RFID_Enable_02.Click += CheckBox_RFID_Enable_02_Click;
            this.checkBox_RFID_Enable_03.Click += CheckBox_RFID_Enable_03_Click;
            this.checkBox_RFID_Enable_04.Click += CheckBox_RFID_Enable_04_Click;
            this.checkBox_RFID_Enable_05.Click += CheckBox_RFID_Enable_05_Click;

            this.rJ_Button_RFID_Beep_01.MouseDownEvent += RJ_Button_RFID_Beep_01_MouseDownEvent;
            this.rJ_Button_RFID_Beep_02.MouseDownEvent += RJ_Button_RFID_Beep_02_MouseDownEvent;
            this.rJ_Button_RFID_Beep_03.MouseDownEvent += RJ_Button_RFID_Beep_03_MouseDownEvent;
            this.rJ_Button_RFID_Beep_04.MouseDownEvent += RJ_Button_RFID_Beep_04_MouseDownEvent;
            this.rJ_Button_RFID_Beep_05.MouseDownEvent += RJ_Button_RFID_Beep_05_MouseDownEvent;


            myThread_program = new MyThread();
            myThread_program.AutoRun(true);
            myThread_program.Add_Method(sub_program);
            myThread_program.SetSleepTime(10);
            myThread_program.Trigger();
        }

        private void RJ_Button_RFID_Beep_05_MouseDownEvent(MouseEventArgs mevent)
        {
            Communication.Set_RFID_Beep(udP_Class, IP, 5);
        }
        private void RJ_Button_RFID_Beep_04_MouseDownEvent(MouseEventArgs mevent)
        {
            Communication.Set_RFID_Beep(udP_Class, IP, 4);
        }
        private void RJ_Button_RFID_Beep_03_MouseDownEvent(MouseEventArgs mevent)
        {
            Communication.Set_RFID_Beep(udP_Class, IP, 3);
        }
        private void RJ_Button_RFID_Beep_02_MouseDownEvent(MouseEventArgs mevent)
        {
            Communication.Set_RFID_Beep(udP_Class, IP, 2);
        }
        private void RJ_Button_RFID_Beep_01_MouseDownEvent(MouseEventArgs mevent)
        {
            Communication.Set_RFID_Beep(udP_Class, IP, 1);
        }

        private void CheckBox_RFID_Enable_05_Click(object sender, EventArgs e)
        {
            Communication.Set_RFID_Enable(udP_Class, IP, 4, this.checkBox_RFID_Enable_05.Checked);
        }
        private void CheckBox_RFID_Enable_04_Click(object sender, EventArgs e)
        {
            Communication.Set_RFID_Enable(udP_Class, IP, 3, this.checkBox_RFID_Enable_04.Checked);
        }
        private void CheckBox_RFID_Enable_03_Click(object sender, EventArgs e)
        {
            Communication.Set_RFID_Enable(udP_Class, IP, 2, this.checkBox_RFID_Enable_03.Checked);
        }
        private void CheckBox_RFID_Enable_02_Click(object sender, EventArgs e)
        {
            Communication.Set_RFID_Enable(udP_Class, IP, 1, this.checkBox_RFID_Enable_02.Checked);
        }
        private void CheckBox_RFID_Enable_01_Click(object sender, EventArgs e)
        {
            Communication.Set_RFID_Enable(udP_Class, IP, 0, this.checkBox_RFID_Enable_01.Checked);
        }

        private void CheckBox_輸出方向10_Click(object sender, EventArgs e)
        {
            Communication.Set_Output_dir(udP_Class, IP, 9, checkBox_輸出方向10.Checked);
        }
        private void CheckBox_輸出方向09_Click(object sender, EventArgs e)
        {
            Communication.Set_Output_dir(udP_Class, IP, 8, checkBox_輸出方向09.Checked);
        }
        private void CheckBox_輸出方向08_Click(object sender, EventArgs e)
        {
            Communication.Set_Output_dir(udP_Class, IP, 7, checkBox_輸出方向08.Checked);
        }
        private void CheckBox_輸出方向07_Click(object sender, EventArgs e)
        {
            Communication.Set_Output_dir(udP_Class, IP, 6, checkBox_輸出方向07.Checked);
        }
        private void CheckBox_輸出方向06_Click(object sender, EventArgs e)
        {
            Communication.Set_Output_dir(udP_Class, IP, 5, checkBox_輸出方向06.Checked);
        }
        private void CheckBox_輸出方向05_Click(object sender, EventArgs e)
        {
            Communication.Set_Output_dir(udP_Class, IP, 4, checkBox_輸出方向05.Checked);
        }
        private void CheckBox_輸出方向04_Click(object sender, EventArgs e)
        {
            Communication.Set_Output_dir(udP_Class, IP, 3, checkBox_輸出方向04.Checked);
        }
        private void CheckBox_輸出方向03_Click(object sender, EventArgs e)
        {
            Communication.Set_Output_dir(udP_Class, IP, 2, checkBox_輸出方向03.Checked);
        }
        private void CheckBox_輸出方向02_Click(object sender, EventArgs e)
        {
            Communication.Set_Output_dir(udP_Class, IP, 1, checkBox_輸出方向02.Checked);
        }
        private void CheckBox_輸出方向01_Click(object sender, EventArgs e)
        {
            Communication.Set_Output_dir(udP_Class, IP, 0, checkBox_輸出方向01.Checked);
        }

        private void CheckBox_輸入方向10_Click(object sender, EventArgs e)
        {
            Communication.Set_Input_dir(udP_Class, IP, 9, checkBox_輸入方向10.Checked);
        }
        private void CheckBox_輸入方向09_Click(object sender, EventArgs e)
        {
            Communication.Set_Input_dir(udP_Class, IP, 8, checkBox_輸入方向09.Checked);
        }
        private void CheckBox_輸入方向08_Click(object sender, EventArgs e)
        {
            Communication.Set_Input_dir(udP_Class, IP, 7, checkBox_輸入方向08.Checked);
        }
        private void CheckBox_輸入方向07_Click(object sender, EventArgs e)
        {
            Communication.Set_Input_dir(udP_Class, IP, 6, checkBox_輸入方向07.Checked);
        }
        private void CheckBox_輸入方向06_Click(object sender, EventArgs e)
        {
            Communication.Set_Input_dir(udP_Class, IP, 5, checkBox_輸入方向06.Checked);
        }
        private void CheckBox_輸入方向05_Click(object sender, EventArgs e)
        {
            Communication.Set_Input_dir(udP_Class, IP, 4, checkBox_輸入方向05.Checked);
        }
        private void CheckBox_輸入方向04_Click(object sender, EventArgs e)
        {
            Communication.Set_Input_dir(udP_Class, IP, 3, checkBox_輸入方向04.Checked);
        }
        private void CheckBox_輸入方向03_Click(object sender, EventArgs e)
        {
            Communication.Set_Input_dir(udP_Class, IP, 2, checkBox_輸入方向03.Checked);
        }
        private void CheckBox_輸入方向02_Click(object sender, EventArgs e)
        {
            Communication.Set_Input_dir(udP_Class, IP, 1, checkBox_輸入方向02.Checked);
        }
        private void CheckBox_輸入方向01_Click(object sender, EventArgs e)
        {
            Communication.Set_Input_dir(udP_Class, IP, 0, checkBox_輸入方向01.Checked);
        }

        private void RJ_Button_退出_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                myThread_program.Abort();
                myThread_program = null;
                this.Close();
            }));

        }

        private void checkBox_輸出狀態10_Click(object sender, EventArgs e)
        {
            Communication.Set_OutputPIN(udP_Class, IP, 9, checkBox_輸出狀態10.Checked);
        }
        private void checkBox_輸出狀態09_Click(object sender, EventArgs e)
        {
            Communication.Set_OutputPIN(udP_Class, IP, 8, checkBox_輸出狀態09.Checked);
        }
        private void checkBox_輸出狀態08_Click(object sender, EventArgs e)
        {
            Communication.Set_OutputPIN(udP_Class, IP, 7, checkBox_輸出狀態08.Checked);
        }
        private void checkBox_輸出狀態07_Click(object sender, EventArgs e)
        {
            Communication.Set_OutputPIN(udP_Class, IP, 6, checkBox_輸出狀態07.Checked);
        }
        private void checkBox_輸出狀態06_Click(object sender, EventArgs e)
        {
            Communication.Set_OutputPIN(udP_Class, IP, 5, checkBox_輸出狀態06.Checked);
        }
        private void checkBox_輸出狀態05_Click(object sender, EventArgs e)
        {
            Communication.Set_OutputPIN(udP_Class, IP, 4, checkBox_輸出狀態05.Checked);
        }
        private void checkBox_輸出狀態04_Click(object sender, EventArgs e)
        {
            Communication.Set_OutputPIN(udP_Class, IP, 3, checkBox_輸出狀態04.Checked);
        }
        private void checkBox_輸出狀態03_Click(object sender, EventArgs e)
        {
            Communication.Set_OutputPIN(udP_Class, IP, 2, checkBox_輸出狀態03.Checked);
        }
        private void checkBox_輸出狀態02_Click(object sender, EventArgs e)
        {
            Communication.Set_OutputPIN(udP_Class, IP, 1, checkBox_輸出狀態02.Checked);
        }
        private void checkBox_輸出狀態01_Click(object sender, EventArgs e)
        {
            Communication.Set_OutputPIN(udP_Class, IP, 0, checkBox_輸出狀態01.Checked);
        }

       
        private void RJ_Button_輸出10_MouseDownEvent(MouseEventArgs mevent)
        {
            Communication.Set_OutputPINTrigger(udP_Class, IP, 9, true);
        }
        private void RJ_Button_輸出09_MouseDownEvent(MouseEventArgs mevent)
        {
            Communication.Set_OutputPINTrigger(udP_Class, IP, 8, true);
        }
        private void RJ_Button_輸出08_MouseDownEvent(MouseEventArgs mevent)
        {
            Communication.Set_OutputPINTrigger(udP_Class, IP, 7, true);
        }
        private void RJ_Button_輸出07_MouseDownEvent(MouseEventArgs mevent)
        {
            Communication.Set_OutputPINTrigger(udP_Class, IP, 6, true);
        }
        private void RJ_Button_輸出06_MouseDownEvent(MouseEventArgs mevent)
        {
            Communication.Set_OutputPINTrigger(udP_Class, IP, 5, true);
        }
        private void RJ_Button_輸出05_MouseDownEvent(MouseEventArgs mevent)
        {
            Communication.Set_OutputPINTrigger(udP_Class, IP, 4, true);
        }
        private void RJ_Button_輸出04_MouseDownEvent(MouseEventArgs mevent)
        {
            Communication.Set_OutputPINTrigger(udP_Class, IP, 3, true);
        }
        private void RJ_Button_輸出03_MouseDownEvent(MouseEventArgs mevent)
        {
            Communication.Set_OutputPINTrigger(udP_Class, IP, 2, true);
        }
        private void RJ_Button_輸出02_MouseDownEvent(MouseEventArgs mevent)
        {
            Communication.Set_OutputPINTrigger(udP_Class, IP, 1, true);
        }
        private void RJ_Button_輸出01_MouseDownEvent(MouseEventArgs mevent)
        {
            Communication.Set_OutputPINTrigger(udP_Class, IP, 0, true);
        }
    }
}
