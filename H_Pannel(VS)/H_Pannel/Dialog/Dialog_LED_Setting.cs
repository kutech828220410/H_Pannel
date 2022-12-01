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
    public partial class Dialog_LED_Setting : Form
    {
        private UDP_Class uDP_Class;
        private string IP;
        private int[] input;
        public int[] Input
        {
            get
            {
                int[] input = new int[5];
                input[0] = (byte)rJ_TextBox_Input01.Texts.StringToInt32();
                input[1] = (byte)rJ_TextBox_Input02.Texts.StringToInt32();
                input[2] = (byte)rJ_TextBox_Input03.Texts.StringToInt32();
                input[3] = (byte)rJ_TextBox_Input04.Texts.StringToInt32();
                input[4] = (byte)rJ_TextBox_Input05.Texts.StringToInt32();
                return input;
            }
            set
            {
                if(value.Length < 5)
                {
                    return;
                }
                rJ_TextBox_Input01.Texts = value[0].ToString();
                rJ_TextBox_Input02.Texts = value[1].ToString();
                rJ_TextBox_Input03.Texts = value[2].ToString();
                rJ_TextBox_Input04.Texts = value[3].ToString();
                rJ_TextBox_Input05.Texts = value[4].ToString();
            }
        }
        private int[] output;
        public int[] Output
        {
            get
            {
                int[] Output = new int[5];
                Output[0] = (byte)rJ_TextBox_Output01.Texts.StringToInt32();
                Output[1] = (byte)rJ_TextBox_Output02.Texts.StringToInt32();
                Output[2] = (byte)rJ_TextBox_Output03.Texts.StringToInt32();
                Output[3] = (byte)rJ_TextBox_Output04.Texts.StringToInt32();
                Output[4] = (byte)rJ_TextBox_Output05.Texts.StringToInt32();
                return Output;
            }
            set
            {
                if (value.Length < 5)
                {
                    return;
                }
                rJ_TextBox_Output01.Texts = value[0].ToString();
                rJ_TextBox_Output02.Texts = value[1].ToString();
                rJ_TextBox_Output03.Texts = value[2].ToString();
                rJ_TextBox_Output04.Texts = value[3].ToString();
                rJ_TextBox_Output05.Texts = value[4].ToString();
            }
        }


        public Dialog_LED_Setting(UDP_Class uDP_Class ,string IP , int[] Input , int[] Output)
        {
            InitializeComponent();
            this.input = Input;
            this.output = Output;
            this.uDP_Class = uDP_Class;
            this.IP = IP;
        }

        private void Dialog_LED_Setting_Load(object sender, EventArgs e)
        {
            this.rJ_Button_退出.MouseDownEvent += RJ_Button_退出_MouseDownEvent;
            this.rJ_Button_設定.MouseDownEvent += RJ_Button_設定_MouseDownEvent;
            this.Input = this.input;
            this.Output = this.output;
        }

        private void RJ_Button_設定_MouseDownEvent(MouseEventArgs mevent)
        {
            Communication.Set_LEDSetting(uDP_Class, IP, this.Input, this.Output);
        }
        private void RJ_Button_退出_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                this.Close();
            }));
        }
    }
}
