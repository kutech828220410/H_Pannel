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
            MyMessageBox.音效 = false;
            H_Pannel_lib.Communication.ConsoleWrite = true;

            this.storageUI_EPD_290.Init_Offline();
            this.storageUI_EPD_266.Init_Offline();

            this.rJ_TextBox_EPD266_IP.Texts = this.storageUI_EPD_266.IP_Adress;
            this.rJ_TextBox_EPD266_SSID.Texts = this.storageUI_EPD_266.SSID;
            this.rJ_TextBox_EPD266_Password.Texts = this.storageUI_EPD_266._Password;
            this.rJ_TextBox_EPD266_ServerIP.Texts = this.storageUI_EPD_266.Server_IP_Adress;


            this.comboBox_PortName_Enter(null, null);
            this.plC_RJ_Button_EPD290_亮燈.MouseDownEvent += PlC_RJ_Button_EPD290_亮燈_MouseDownEvent;
            this.plC_RJ_Button_EPD290_刷新電子紙.MouseDownEvent += PlC_RJ_Button_EPD290_刷新電子紙_MouseDownEvent;
            this.plC_RJ_Button_EPD290_測試一次.MouseDownEvent += PlC_RJ_Button_EPD290_測試一次_MouseDownEvent;


            this.plC_RJ_Button_EPD266_亮燈.MouseDownEvent += PlC_RJ_Button_EPD266_亮燈_MouseDownEvent;
            this.plC_RJ_Button_EPD266_刷新電子紙.MouseDownEvent += PlC_RJ_Button_EPD266_刷新電子紙_MouseDownEvent;
            this.plC_RJ_Button_EPD266_測試一次.MouseDownEvent += PlC_RJ_Button_EPD266_測試一次_MouseDownEvent;
            this.plC_RJ_Button_EPD266_寫入參數.MouseDownEvent += PlC_RJ_Button_EPD266_寫入參數_MouseDownEvent;
            this.plC_RJ_Button_EPD266_OTA.MouseDownEvent += PlC_RJ_Button_EPD266_OTA_MouseDownEvent;
            this.plC_RJ_Button_EPD266_IP加1.MouseDownEvent += PlC_RJ_Button_EPD266_IP加1_MouseDownEvent;
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

        #region EPD290 Event
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
        #endregion
        #region EPD266 Event
        private void PlC_RJ_Button_EPD266_亮燈_MouseDownEvent(MouseEventArgs mevent)
        {
            this.plC_RJ_Button_EPD266_亮燈.Bool = true;
            if (!this.storageUI_EPD_266.Set_Stroage_LED_UART(PortName, Color.Red))
            {
                this.plC_RJ_Button_EPD266_亮燈.Bool = false;
                return;
            }
            System.Threading.Thread.Sleep(500);
            if (!this.storageUI_EPD_266.Set_Stroage_LED_UART(PortName, Color.Lime))
            {
                this.plC_RJ_Button_EPD266_亮燈.Bool = false;
                return;
            }
            System.Threading.Thread.Sleep(500);
            if (!this.storageUI_EPD_266.Set_Stroage_LED_UART(PortName, Color.Blue))
            {
                this.plC_RJ_Button_EPD266_亮燈.Bool = false;
                return;
            }
            System.Threading.Thread.Sleep(500);
            this.storageUI_EPD_266.Set_Stroage_LED_UART(PortName, Color.Black);
            this.plC_RJ_Button_EPD266_亮燈.Bool = false;
        }
        private void PlC_RJ_Button_EPD266_刷新電子紙_MouseDownEvent(MouseEventArgs mevent)
        {
            this.plC_RJ_Button_EPD266_刷新電子紙.Bool = true;
            if (!this.storageUI_EPD_266.Set_ClearCanvas_UART(PortName))
            {
                this.plC_RJ_Button_EPD266_刷新電子紙.Bool = false;
                return;
            }
            this.plC_RJ_Button_EPD266_刷新電子紙.Bool = false;
        }
        private void PlC_RJ_Button_EPD266_測試一次_MouseDownEvent(MouseEventArgs mevent)
        {
            this.PlC_RJ_Button_EPD266_刷新電子紙_MouseDownEvent(null);
            this.PlC_RJ_Button_EPD266_亮燈_MouseDownEvent(null);
            MyMessageBox.ShowDialog($"版本 : {this.storageUI_EPD_266.Get_Version_UART(PortName)}");
        }  
        private void PlC_RJ_Button_EPD266_寫入參數_MouseDownEvent(MouseEventArgs mevent)
        {
            this.storageUI_EPD_266.IP_Adress = this.rJ_TextBox_EPD266_IP.Text;
            this.storageUI_EPD_266.SSID = this.rJ_TextBox_EPD266_SSID.Text;
            this.storageUI_EPD_266._Password = this.rJ_TextBox_EPD266_Password.Text;
            this.storageUI_EPD_266.Server_IP_Adress = this.rJ_TextBox_EPD266_ServerIP.Text;

            if(this.storageUI_EPD_266.WriteConfig(PortName))
            {
                if(this.checkBox_EPD266_自動加1.Checked)
                PlC_RJ_Button_EPD266_IP加1_MouseDownEvent(null);
            }
        }
        private void PlC_RJ_Button_EPD266_OTA_MouseDownEvent(MouseEventArgs mevent)
        {
            this.storageUI_EPD_266.UDP_Class_Init();
            if (this.storageUI_EPD_266.Ping(this.storageUI_EPD_266.IP_Adress))
            {
                this.storageUI_EPD_266.Set_OTAUpdate(this.storageUI_EPD_266.IP_Adress, 29000);
            }
   
        }
        private void PlC_RJ_Button_EPD266_IP加1_MouseDownEvent(MouseEventArgs mevent)
        {
            string IP = rJ_TextBox_EPD266_IP.Text;
            string[] IP_Ary = IP.Split('.');
            if (IP_Ary.Length != 4) return;
            int temp = IP_Ary[3].StringToInt32();
            this.Invoke(new Action(delegate 
            {
                rJ_TextBox_EPD266_IP.Text = $"{IP_Ary[0]}.{IP_Ary[1]}.{IP_Ary[2]}.{temp + 1}";
            }));
        }
        #endregion
    }
}
