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
namespace TestForm
{
    public partial class Form1 : Form
    {
        private string PortName
        {
            get
            {
                string temp = "";
                this.Invoke(new Action(delegate { temp = this.comboBox_PortName.Text; }));
                return temp;
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            H_Pannel_lib.Communication.ConsoleWrite = true;

            this.storageUI_EPD_290.Init_Offline();
            this.comboBox_PortName_Enter(null, null);
            this.plC_RJ_Button_EPD290_亮燈.MouseDownEvent += PlC_RJ_Button_EPD290_亮燈_MouseDownEvent;
            this.plC_RJ_Button_EPD290_刷新電子紙.MouseDownEvent += PlC_RJ_Button_EPD290_刷新電子紙_MouseDownEvent;
            this.plC_RJ_Button_EPD290_測試一次.MouseDownEvent += PlC_RJ_Button_EPD290_測試一次_MouseDownEvent;

            this.plC_UI_Init.UI_Finished_Event += PlC_UI_Init_UI_Finished_Event;
            this.plC_UI_Init.Run(this.FindForm(), this.lowerMachine_Panel);
            MyMessageBox.音效 = false;
        }

        private void PlC_UI_Init_UI_Finished_Event()
        {

        }

        private void comboBox_PortName_Enter(object sender, EventArgs e)
        {
            this.comboBox_PortName.DataSource = MySerialPort.GetPortNames();
        } 
        private void PlC_RJ_Button_EPD290_刷新電子紙_MouseDownEvent(MouseEventArgs mevent)
        {
            this.plC_RJ_Button_EPD290_刷新電子紙.Bool = true;
            if (!this.storageUI_EPD_290.Set_ClearCanvas_UART(PortName))
            {
                this.plC_RJ_Button_EPD290_刷新電子紙.Bool = false;
                return;
            }
            this.plC_RJ_Button_EPD290_刷新電子紙.Bool = false;
        }
        private void PlC_RJ_Button_EPD290_亮燈_MouseDownEvent(MouseEventArgs mevent)
        {
            this.plC_RJ_Button_EPD290_亮燈.Bool = true;
            if (!this.storageUI_EPD_290.Set_Stroage_LED_UART(PortName, Color.Red))
            {
                this.plC_RJ_Button_EPD290_亮燈.Bool = false;
                return;
            }
            System.Threading.Thread.Sleep(500);
            if (!this.storageUI_EPD_290.Set_Stroage_LED_UART(PortName, Color.Lime))
            {
                this.plC_RJ_Button_EPD290_亮燈.Bool = false;
                return;
            }
            System.Threading.Thread.Sleep(500);
            if (!this.storageUI_EPD_290.Set_Stroage_LED_UART(PortName, Color.Blue))
            {
                this.plC_RJ_Button_EPD290_亮燈.Bool = false;
                return;
            }
            System.Threading.Thread.Sleep(500);
            this.storageUI_EPD_290.Set_Stroage_LED_UART(PortName, Color.Black);
            this.plC_RJ_Button_EPD290_亮燈.Bool = false;
        }
        private void PlC_RJ_Button_EPD290_測試一次_MouseDownEvent(MouseEventArgs mevent)
        {
            this.PlC_RJ_Button_EPD290_刷新電子紙_MouseDownEvent(null);
            this.PlC_RJ_Button_EPD290_亮燈_MouseDownEvent(null);
            MyMessageBox.ShowDialog($"版本 : {this.storageUI_EPD_290.Get_Version_UART(PortName)}");
        }
    }
}
