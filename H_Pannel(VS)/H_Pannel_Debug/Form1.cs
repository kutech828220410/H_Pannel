using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;
using H_Pannel_lib;
using System.Text.RegularExpressions;
using Basic;
namespace WT32_SC01
{
    public partial class Form1 : Form
    {
        Basic.MyThread MyThread_Program;
        public Form1()
        {
            InitializeComponent();
        }
        #region ToolStripMenuItem

        private void 顯示測試資訊ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> list_IP = this.storageUI_WT32.GetAllDeviceTableIP();
            List<Task> taskList = new List<Task>();
            Console.Clear();
            for (int i = 0; i < list_IP.Count; i++)
            {
                string IP = list_IP[i];
            
                taskList.Add(Task.Run(() =>
                {
                    int index = 0;
                    if (this.wT32_GPADC.Set_ClearCanvas(IP, 29000, Color.White)) index++;
                    if (this.wT32_GPADC.Set_TextEx(IP, 29000, "藥品名稱 : Ketoprofen I.M. INJ. 25MG/ML", 5, 10, new Font("標楷體", 15, FontStyle.Bold), Color.White, Color.Black, 2, Color.Red)) index++;
                    if (this.wT32_GPADC.Set_TextEx(IP, 29000, "藥品學名 : Ketoprofen 50 mg/2mL", 5, 40, new Font("新細明體", 15, FontStyle.Bold), Color.White, Color.Black)) index++;
                    if (this.wT32_GPADC.Set_TextEx(IP, 29000, "廠牌　　 : 南光化學製藥", 5, 70, new Font("微軟正黑體", 15, FontStyle.Bold), Color.White, Color.Black)) index++;
                    if (this.wT32_GPADC.Set_TextEx(IP, 29000, "藥品碼　 : KET01", 5, 100, new Font("微軟正黑體", 15, FontStyle.Bold), Color.White, Color.Black)) index++;
                    if (this.wT32_GPADC.Set_TextEx(IP, 29000, "庫存 <155>", 10, 130, new Font("微軟正黑體", 15, FontStyle.Bold), Color.Red, Color.Yellow)) index++;
                    if (this.wT32_GPADC.Set_TextEx(IP, 29000, IP, 10, 160, new Font("微軟正黑體", 15, FontStyle.Bold), Color.Red, Color.Yellow)) index++;
                    if (this.wT32_GPADC.Set_BarCodeEx(IP, 29000, "KET01", 90, 230, 300, 80)) index++;
                    if (index == 8)
                    {
                        Console.WriteLine("{0} 完成繪製! <{1}>", IP, index.ToString());
                    }
                    else
                    {
                        Console.WriteLine("{0} 繪製失敗! <{1}>", IP, index.ToString());
                    }
                 

                    //this.wT32_GPADC.Set_DrawRect(JsonString, 10, 10, 100, 200, 5, Color.Red);
                    //this.wT32_GPADC.Set_TextEx(JsonString, IP, 0, 0, new Font("微軟正黑體", 30), Color.White, Color.Black);
                }));
            }
            Task allTask = Task.WhenAll(taskList);
            //allTask.Wait();
        }
        #endregion

        private void Form1_Load(object sender, EventArgs e)
        {
            this.MyThread_Program = new Basic.MyThread();
            this.MyThread_Program.SetSleepTime(1);
            this.MyThread_Program.AutoRun(true);
            this.MyThread_Program.Trigger();

            this.rJ_Button_WT32_上傳畫面.MouseDownEvent += RJ_Button_WT32_上傳畫面_MouseDownEvent;
            this.rJ_Button_WT32_測試.MouseDownEvent += RJ_Button_WT32_測試_MouseDownEvent;

            this.drawerUI_EPD_583.sqL_DataGridView_UDP_DataReceive.RowDoubleClickEvent += SqL_DataGridView_UDP_DataReceive_EPD583_RowDoubleClickEvent;
            this.drawerUI_EPD_583.sqL_DataGridView_DeviceTable.RowDoubleClickEvent += SqL_DataGridView_DeviceTable_EPD583_RowDoubleClickEvent;
            this.rJ_Button_epD_583_Pannel_儲位亮燈.MouseDownEvent += RJ_Button_epD_583_Pannel_儲位亮燈_MouseDownEvent;
            this.rJ_Button_epD_583_Pannel_全部滅燈.MouseDownEvent += RJ_Button_epD_583_Pannel_全部滅燈_MouseDownEvent;

            this.storageUI_EPD_266.sqL_DataGridView_DeviceTable.RowDoubleClickEvent += SqL_DataGridView_DeviceTable_RowDoubleClickEvent;

            this.rJ_Button_RFID_初始化.MouseDownEvent += RJ_Button_RFID_初始化_MouseDownEvent;
        }

  

        private void rJ_Button_WT32_初始化_Click(object sender, EventArgs e)
        {
            this.storageUI_WT32.Init();
            this.wT32_GPADC.Init(this.storageUI_WT32.GetLoacalUDP_Class());
            this.pannel35_Pannel.Init(this.storageUI_WT32.GetLoacalUDP_Class());
            this.pannel35_Pannel.EditFinishedEvent += Pannel35_Pannel_EditFinishedEvent;
        }
        private void RJ_Button_WT32_測試_MouseDownEvent(MouseEventArgs mevent)
        {
            List<string> list_selectedIP = this.storageUI_WT32.GetAllSelectDeviceTableIP();
            if (list_selectedIP.Count > 0)
            {
                Storage storage = this.storageUI_WT32.SQL_GetStorage(list_selectedIP[0]);
                if (storage != null)
                {
                    List<byte> list_bytes = new List<byte>();
                    byte[] bytes = WT32_GPADC.Get_Storage_JepgBytes(storage);
       
                    this.storageUI_WT32.Set_Jpegbuffer(storage.IP, storage.Port, bytes);
                }
            }
            else
            {
                MyMessageBox.ShowDialog("未選擇儲位!");
            }
            
        }
        private void RJ_Button_WT32_上傳畫面_MouseDownEvent(MouseEventArgs mevent)
        {
            List<string> list_selectedIP = this.storageUI_WT32.GetAllSelectDeviceTableIP();
            if (list_selectedIP.Count > 0)
            {
                Storage storage = this.storageUI_WT32.SQL_GetStorage(list_selectedIP[0]);
                if (storage != null)
                {
                    this.wT32_GPADC.Set_DrawPannel(storage);
                }
            }
            else
            {
                MyMessageBox.ShowDialog("未選擇儲位!");
            }
        }

        private void Pannel35_Pannel_EditFinishedEvent(Storage storage)
        {
            this.wT32_GPADC.Set_Stroage(storage);
            this.storageUI_WT32.SQL_ReplaceStorage(storage);
        }

        private void rJ_Button_WT32_讀取選擇儲位_Click(object sender, EventArgs e)
        {
            List<string> list_selectedIP = this.storageUI_WT32.GetAllSelectDeviceTableIP();
            if (list_selectedIP.Count > 0)
            {
                Storage storage = this.storageUI_WT32.SQL_GetStorage(list_selectedIP[0]);
                if (storage != null)
                {
                    this.wT32_GPADC.Set_Stroage(storage);
                    this.pannel35_Pannel.Set_Stroage(storage);
                }
            }
            else
            {
                MyMessageBox.ShowDialog("未選擇儲位!");
            }
        }
        private void rJ_Button_WT32_上傳選擇儲位_Click(object sender, EventArgs e)
        {
            List<string> list_selectedIP = this.storageUI_WT32.GetAllSelectDeviceTableIP();
            if (list_selectedIP.Count > 0)
            {
                if (this.wT32_GPADC.CurrentStorage != null)
                {
                    this.storageUI_WT32.SQL_ReplaceStorage(this.wT32_GPADC.CurrentStorage);
                }
            }
            else
            {
                MyMessageBox.ShowDialog("未選擇儲位!");
            }
        }
        private void rJ_Button_WT32_資料寫入_Click(object sender, EventArgs e)
        {
            if (this.wT32_GPADC.CurrentStorage != null)
            {
                this.wT32_GPADC.CurrentStorage.SetValue(Storage.ValueName.藥品碼, Storage.ValueType.Value, this.rJ_TextBox_WT32_藥品碼.Text);
                this.wT32_GPADC.CurrentStorage.SetValue(Storage.ValueName.藥品名稱, Storage.ValueType.Value, this.rJ_TextBox_WT32_藥品名稱.Text);
                this.wT32_GPADC.CurrentStorage.SetValue(Storage.ValueName.藥品中文名稱, Storage.ValueType.Value, this.rJ_TextBox_WT32_中文名稱.Text);
                this.wT32_GPADC.CurrentStorage.SetValue(Storage.ValueName.藥品學名, Storage.ValueType.Value, this.rJ_TextBox_WT32_藥品學名.Text);
                this.wT32_GPADC.CurrentStorage.SetValue(Storage.ValueName.廠牌, Storage.ValueType.Value, this.rJ_TextBox_WT32_廠牌.Text);
                this.wT32_GPADC.CurrentStorage.SetValue(Storage.ValueName.包裝單位, Storage.ValueType.Value, this.rJ_TextBox_WT32_包裝單位.Text);
                this.wT32_GPADC.CurrentStorage.SetValue(Storage.ValueName.最小包裝單位, Storage.ValueType.Value, this.rJ_TextBox_WT32_最小包裝單位.Text);
                this.wT32_GPADC.CurrentStorage.SetValue(Storage.ValueName.最小包裝單位數量, Storage.ValueType.Value, this.rJ_TextBox_WT32_最小包裝數量.Text);
                this.wT32_GPADC.CurrentStorage.SetValue(Storage.ValueName.BarCode, Storage.ValueType.Value, this.rJ_TextBox_WT32_一維碼.Text);
                this.wT32_GPADC.CurrentStorage.SetValue(Storage.ValueName.庫存, Storage.ValueType.Value, this.rJ_TextBox_WT32_庫存.Text);

            }
        }

        private void pictureBox_2_66_Click(object sender, EventArgs e)
        {
        }
        private void rJ_Button_EPD_266_初始化_Click(object sender, EventArgs e)
        {
            this.storageUI_EPD_266.Init();
            this.epD_266_Pannel.Init(this.storageUI_EPD_266.List_UDP_Local);
        }
        private void SqL_DataGridView_DeviceTable_RowDoubleClickEvent(object[] RowValue)
        {
            string IP = RowValue[(int)enum_DeviceTable.IP].ObjectToString();
            Storage storage = this.storageUI_EPD_266.SQL_GetStorage(IP);
            if (storage == null) return;
            this.epD_266_Pannel.DrawToPictureBox(storage);
        }

        string epD_583_Pannel_IP = "";
        async private void rJ_Button_EPD_583_初始化_Click(object sender, EventArgs e)
        {
            Task task = Task.Factory.StartNew(new Action(delegate 
            {
                this.drawerUI_EPD_583.Init();
                this.epD_583_Pannel.Init(this.drawerUI_EPD_583.List_UDP_Local);
                if (!epD_583_Pannel_IP.Check_IP_Adress()) return;

            }));
            await task;     
        }
        private void SqL_DataGridView_DeviceTable_EPD583_RowDoubleClickEvent(object[] RowValue)
        {
            string IP = RowValue[(int)enum_DeviceTable.IP].ObjectToString();
            epD_583_Pannel_IP = IP;
            Drawer drawer = this.drawerUI_EPD_583.SQL_GetDrawer(epD_583_Pannel_IP);
            if (drawer == null) return;
            this.epD_583_Pannel.DrawToPictureBox(drawer);
        }
        private void SqL_DataGridView_UDP_DataReceive_EPD583_RowDoubleClickEvent(object[] RowValue)
        {
            string IP = RowValue[(int)enum_UDP_DataReceive.IP].ObjectToString();
            epD_583_Pannel_IP = IP;
            Drawer drawer = this.drawerUI_EPD_583.SQL_GetDrawer(epD_583_Pannel_IP);
            if (drawer == null) return;
            this.epD_583_Pannel.DrawToPictureBox(drawer);
        }

        private void rJ_Button_EPD_583_設定IP_Click(object sender, EventArgs e)
        {
            Dialog_IP_Config dialog_IP_Config = new Dialog_IP_Config();
            dialog_IP_Config.PortVisibale = false;
            if (dialog_IP_Config.ShowDialog() == DialogResult.Yes)
            {
                epD_583_Pannel_IP = dialog_IP_Config.IP;
            }
        }
        private void rJ_Button_epD_583_Pannel_上傳_Click(object sender, EventArgs e)
        {
            Drawer drawer = this.epD_583_Pannel.CurrentDrawer;
            if (drawer == null) return;
            this.drawerUI_EPD_583.SQL_ReplaceDrawer(drawer);
        }
        private void RJ_Button_epD_583_Pannel_儲位亮燈_MouseDownEvent(MouseEventArgs mevent)
        {
            this.epD_583_Pannel.BoxLightOn(Color.Blue);
        }

        private void rJ_Button_epD_583_Pannel_儲位滅燈_Click(object sender, EventArgs e)
        {
            this.epD_583_Pannel.BoxLightOff();
        }
        private void rJ_Button_epD_583_Pannel_面板亮燈_Click(object sender, EventArgs e)
        {
            this.epD_583_Pannel.PannelLightOn(Color.Red);
        }
        private void rJ_Button_epD_583_Pannel_面板滅燈_Click(object sender, EventArgs e)
        {
            this.epD_583_Pannel.PannelLightOff();
        }
        private void rJ_Button_epD_583_Pannel_全部亮燈_Click(object sender, EventArgs e)
        {
            this.epD_583_Pannel.AllLightOn(Color.White);
        }
        
        private void RJ_Button_epD_583_Pannel_全部滅燈_MouseDownEvent(MouseEventArgs mevent)
        {
            this.epD_583_Pannel.AllLightOn(Color.Black);
        }
        private void rJ_Button_RowsLED_初始化_Click(object sender, EventArgs e)
        {
            this.rowsLEDUI.Init();
        }



        private void RJ_Button_RFID_初始化_MouseDownEvent(MouseEventArgs mevent)
        {
            this.rfiD_UI.Init();
        }

        private void rJ_Button1_Click(object sender, EventArgs e)
        {
            List<RFID_UI.RFID_UID_Class> rFID_UID_Classes = this.rfiD_UI.GetRFID();
        }


    }
}
