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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace WT32_SC01
{
    public partial class Form1 : Form
    {
        Driver_IO_Board driver_IO_Board = new Driver_IO_Board();
        MySerialPort mySerialPort_IO_Board = new MySerialPort();
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
            string _Name_font_Serialize = FontSerializationHelper.ToString(new Font("微軟正黑體", 14, FontStyle.Bold));
            MyMessageBox.form = this.FindForm();
            MyMessageBox.音效 = false;
            this.MyThread_Program = new Basic.MyThread();
            this.MyThread_Program.SetSleepTime(10);
            this.MyThread_Program.AutoRun(true);
            this.MyThread_Program.Add_Method(this.sub_H_RFID);
            this.MyThread_Program.Trigger();

            this.rJ_Button_WT32_上傳畫面.MouseDownEvent += RJ_Button_WT32_上傳畫面_MouseDownEvent;
            this.rJ_Button_WT32_測試.MouseDownEvent += RJ_Button_WT32_測試_MouseDownEvent;

            this.drawerUI_EPD_583.sqL_DataGridView_UDP_DataReceive.RowDoubleClickEvent += SqL_DataGridView_UDP_DataReceive_EPD583_RowDoubleClickEvent;
            this.drawerUI_EPD_583.sqL_DataGridView_DeviceTable.RowDoubleClickEvent += SqL_DataGridView_DeviceTable_EPD583_RowDoubleClickEvent;
            this.rJ_Button_epD_583_Pannel_儲位亮燈.MouseDownEvent += RJ_Button_epD_583_Pannel_儲位亮燈_MouseDownEvent;
            this.rJ_Button_epD_583_Pannel_全部滅燈.MouseDownEvent += RJ_Button_epD_583_Pannel_全部滅燈_MouseDownEvent;
            this.rJ_Button_epD_583_Pannel_TEST.MouseDownEvent += RJ_Button_epD_583_Pannel_TEST_MouseDownEvent;

            this.storageUI_EPD_266.sqL_DataGridView_DeviceTable.RowDoubleClickEvent += SqL_DataGridView_DeviceTable_EPD266_RowDoubleClickEvent1;
            this.rJ_Button_EPD266_TEST.MouseDownEvent += RJ_Button_EPD266_TEST_MouseDownEvent;
           

            this.rJ_Button_EPD_290_初始化.MouseDownEvent += RJ_Button_EPD_290_初始化_MouseDownEvent;
            this.rJ_Button_EPD_290_TEST.MouseDownEvent += RJ_Button_EPD_290_TEST_MouseDownEvent;

            this.rJ_Button_RFID_初始化.MouseDownEvent += RJ_Button_RFID_初始化_MouseDownEvent;

            this.rJ_Button_H_RFID_初始化.MouseDownEvent += RJ_Button_H_RFID_初始化_MouseDownEvent;


            this.storageUI_EPD_266.Init();
            this.epD_266_Pannel.Init(this.storageUI_EPD_266.List_UDP_Local);
            this.epD_266_Pannel.MouseDown += EpD_266_Pannel_MouseDown;
            this.storagePanel.SureClick += StoragePanel_SureClick;

            this.storageUI_EPD_290.Init();
            this.epD_290_Pannel.Init(this.storageUI_EPD_290.List_UDP_Local);

            this.drawerUI_EPD_420.Init();
            this.epD_420_Pannel.Init(this.drawerUI_EPD_420.List_UDP_Local);
            this.rJ_Button_EPD_420_填入測試畫面.MouseDownEvent += RJ_Button_EPD_420_填入測試畫面_MouseDownEvent;

            this.drawerUI_EPD_1020.Init();
            this.epD_1020_Pannel.Init(this.drawerUI_EPD_1020.List_UDP_Local);


            this.rJ_Button_EPD_1020_填入測試畫面.MouseDownEvent += RJ_Button_EPD_1020_填入測試畫面_MouseDownEvent;
            this.rJ_Button_EPD1020_初始化.MouseDownEvent += RJ_Button_EPD1020_初始化_MouseDownEvent;
            this.rJ_Button_EPD_1020_門片畫面測試.MouseDownEvent += RJ_Button_EPD_1020_門片畫面測試_MouseDownEvent;
            this.rfiD_UI.Init();

            MyUI.數字鍵盤.音效 = false;
            H_Pannel_lib.Communication.UART_ConsoletWrite = true;

            storageUI_LCD_114.Init();
            lcD114_Panel.Init(this.storageUI_LCD_114.List_UDP_Local);
            this.rJ_Button_lcD114_Panel_Write.MouseDownEvent += RJ_Button_lcD114_Panel_Write_MouseDownEvent;
        }
        int index = 0;
        private void RJ_Button_lcD114_Panel_Write_MouseDownEvent(MouseEventArgs mevent)
        {
            Font font = new Font("標楷體", 80, FontStyle.Bold);
            lcD114_Panel.DrawImage("192.168.40.200", 29000, index.ToString("000"), font, Color.White, Color.Red);
            index++;
        }



        #region WT32
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
        #endregion
        private void Pannel35_Pannel_EditFinishedEvent(Storage storage)
        {
            this.wT32_GPADC.Set_Stroage(storage);
            this.storageUI_WT32.SQL_ReplaceStorage(storage);
        }

        #region EPD266
        private void EpD_266_Pannel_MouseDown(object sender, MouseEventArgs e)
        {
            EPD266_Paint_Form ePD266_Paint_Form = new EPD266_Paint_Form(this.epD_266_Pannel.CurrentStorage);
            if (ePD266_Paint_Form.ShowDialog() != DialogResult.Yes) return;

            Storage storage = ePD266_Paint_Form.CurrentStorage;

            storageUI_EPD_266.SQL_ReplaceStorage(storage);
        }
        private void rJ_Button_EPD_266_初始化_Click(object sender, EventArgs e)
        {
            this.storageUI_EPD_266.Init();
            this.epD_266_Pannel.Init(this.storageUI_EPD_266.List_UDP_Local);        
        }
        private void RJ_Button_EPD266_TEST_MouseDownEvent(MouseEventArgs mevent)
        {
            MyUI.MyTimer myTimer = new MyUI.MyTimer();
            myTimer.StartTickTime(5000);
           Storage storage = this.storageUI_EPD_266.SQL_GetStorage("192.168.10.180");
            //StorageUI_EPD_266.Get_Storage_bmp(storage);

            using (Bitmap bitmap = StorageUI_EPD_266.Get_Storage_bmp(storage))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.SmoothingMode = SmoothingMode.HighQuality; //使繪圖質量最高，即消除鋸齒
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
                    Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                  //  g.FillRectangle(new SolidBrush(Color.White), rect);
                    this.storageUI_EPD_266.DrawToEpd_UDP("192.168.1.155", 29000, bitmap);
                }
            }
                 
        }
        private void RJ_Button_EPD_290_初始化_MouseDownEvent(MouseEventArgs mevent)
        {
          
        }
        private void RJ_Button_EPD_290_TEST_MouseDownEvent(MouseEventArgs mevent)
        {
            MyUI.MyTimer myTimer = new MyUI.MyTimer();
            myTimer.StartTickTime(5000);
            Storage storage = this.storageUI_EPD_266.SQL_GetStorage("192.168.10.180");
            //StorageUI_EPD_290.Get_Storage_bmp(storage);

            using (Bitmap bitmap = StorageUI_EPD_290.Get_Storage_bmp(storage))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.SmoothingMode = SmoothingMode.HighQuality; //使繪圖質量最高，即消除鋸齒
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
                    Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                    //  g.FillRectangle(new SolidBrush(Color.White), rect);
                    this.storageUI_EPD_290.DrawToEpd_UDP("192.168.1.205", 29015, bitmap);
                }
            }
        }
        private void StoragePanel_SureClick(Storage storage)
        {
            storageUI_EPD_266.SQL_ReplaceStorage(storage);
            this.storagePanel.DrawToPictureBox(storage);
        }
        private void SqL_DataGridView_DeviceTable_EPD266_RowDoubleClickEvent1(object[] RowValue)
        {
            string IP = RowValue[(int)enum_DeviceTable.IP].ObjectToString();
            Storage storage = this.storageUI_EPD_266.SQL_GetStorage(IP);
            if (storage == null) return;
            this.epD_266_Pannel.DrawToPictureBox(storage);
            this.storagePanel.DrawToPictureBox(storage);
        }
        #endregion
        #region EPD420
        private void RJ_Button_EPD_420_填入測試畫面_MouseDownEvent(MouseEventArgs mevent)
        {
            Drawer drawer = new Drawer("192.168.1.230", 29013, DrawerUI_EPD_420.Pannel_Width, DrawerUI_EPD_420.Pannel_Height);
            Bitmap bitmap = new Bitmap(DrawerUI_EPD_420.Pannel_Width, DrawerUI_EPD_420.Pannel_Height);
            Graphics g = Graphics.FromImage(bitmap);

            g.FillRectangle(new SolidBrush(Color.White), new RectangleF(0, 0, DrawerUI_EPD_420.Pannel_Width, DrawerUI_EPD_420.Pannel_Height));
            Rectangle rectangle_姓名 = new Rectangle(0, 0, DrawerUI_EPD_420.Pannel_Width, 150);
            g.FillRectangle(new SolidBrush(Color.Black), rectangle_姓名);
            DrawingClass.Draw.文字中心繪製("5B", rectangle_姓名, rectangle_姓名.Width - 200, new Font("標楷體", 70, FontStyle.Bold), Color.White, g);

            Rectangle rectangle_生日 = new Rectangle(0, 150, DrawerUI_EPD_420.Pannel_Width, 150);
            g.FillRectangle(new SolidBrush(Color.White), rectangle_生日);
            DrawingClass.Draw.文字中心繪製("16-01", rectangle_生日, rectangle_生日.Width - 30, new Font("微軟正黑體", 50, FontStyle.Bold), Color.Red, g);




            this.epD_420_Pannel.DrawToPictureBox(bitmap);
            string BW = "";
            string RW = "";
            //this.drawerUI_EPD_420.DrawToEpd_UDP("192.168.1.230", 29013, bitmap);
           // bitmap.RotateFlip(RotateFlipType.);
            Communication.BitmapToHexString(bitmap, ref BW, ref RW, EPD_Type.EPD420);
            bitmap.Dispose();
            g.Dispose();
        }
        #endregion
        #region EPD583
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
        private void RJ_Button_epD_583_Pannel_TEST_MouseDownEvent(MouseEventArgs mevent)
        {
            MyUI.MyTimer myTimer = new MyUI.MyTimer();
            myTimer.StartTickTime(5000);
            this.drawerUI_EPD_583.SQL_GetAllDevice();
            Console.WriteLine($"SQL_GetAllDevice ,耗時{myTimer.ToString()}");

            this.drawerUI_EPD_583.SQL_GetAllDeviceBasic();
            Console.WriteLine($"SQL_GetAllDeviceBasic ,耗時{myTimer.ToString()}");

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
        #endregion
        private void RJ_Button_RFID_初始化_MouseDownEvent(MouseEventArgs mevent)
        {
            this.rfiD_UI.Init();
        }
        private void RJ_Button_H_RFID_初始化_MouseDownEvent(MouseEventArgs mevent)
        {
            this.h_RFID_UI.Init();
        }
        private enum enum_H_RFID_Datas
        {
            GUID,
            IP,
            CardID,
            RSSI,
        }
        #region EPD1020
        private void RJ_Button_EPD1020_初始化_MouseDownEvent(MouseEventArgs mevent)
        {
           // this.drawerUI_EPD_1020.Init();
        }
        private void RJ_Button_EPD_1020_填入測試畫面_MouseDownEvent(MouseEventArgs mevent)
        {
            Drawer drawer = new Drawer("192.168.43.230", 29012, 960, 640);
            Bitmap bitmap = new Bitmap(DrawerUI_EPD_1020.Pannel_Width, DrawerUI_EPD_1020.Pannel_Height);

            Graphics g = Graphics.FromImage(bitmap);

            g.FillRectangle(new SolidBrush(Color.White), new RectangleF(0, 0, 960, 640));
            //g.DrawRectangle(new Pen(Color.Black, _box.Pen_Width), rect);
            SizeF size_Name;
            g.SmoothingMode = SmoothingMode.HighQuality; //使繪圖質量最高，即消除鋸齒
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

            Rectangle rectangle_LOGO = new Rectangle(0, 0, 200, 100);

            Rectangle rectangle_病房代碼 = new Rectangle(rectangle_LOGO.Right + 5, 0, 200, 100);
            g.DrawRectangle(new Pen(Color.Black, 1), rectangle_病房代碼);
            g.FillRectangle(new SolidBrush(Color.Black), new RectangleF(rectangle_病房代碼.X, rectangle_病房代碼.Y, rectangle_病房代碼.Width, 30));

            DrawingClass.Draw.文字中心繪製("病房號碼", new Rectangle(rectangle_病房代碼.X, rectangle_病房代碼.Y, rectangle_病房代碼.Width, 30), rectangle_病房代碼.Width - 20, new Font("微軟正黑體", 14, FontStyle.Bold), Color.White, g);
            DrawingClass.Draw.文字中心繪製("501-G", new Rectangle(rectangle_病房代碼.X, rectangle_病房代碼.Y + 30, rectangle_病房代碼.Width, rectangle_病房代碼.Height - 30), rectangle_病房代碼.Width - 50, new Font("微軟正黑體", 20, FontStyle.Bold), Color.Black, g);


            Rectangle rectangle_出院日期 = new Rectangle(rectangle_病房代碼.Right + 5, 0, 300, 100);
            g.DrawRectangle(new Pen(Color.Black, 1), rectangle_出院日期);
            g.FillRectangle(new SolidBrush(Color.Black), new RectangleF(rectangle_出院日期.X, rectangle_出院日期.Y, rectangle_出院日期.Width, 30));
            DrawingClass.Draw.文字中心繪製("出院日期", new Rectangle(rectangle_出院日期.X, rectangle_出院日期.Y, rectangle_出院日期.Width, 30), rectangle_出院日期.Width - 20, new Font("微軟正黑體", 14, FontStyle.Bold), Color.White, g);
            DrawingClass.Draw.文字中心繪製("2023/05/28 週五", new Rectangle(rectangle_出院日期.X, rectangle_出院日期.Y + 30, rectangle_出院日期.Width, rectangle_出院日期.Height - 30), rectangle_出院日期.Width - 50, new Font("微軟正黑體", 20, FontStyle.Bold), Color.Black, g);

            Rectangle rectangle_病人資訊 = new Rectangle(0, rectangle_病房代碼.Bottom + 5, rectangle_出院日期.Right, 300);
            g.DrawRectangle(new Pen(Color.Black, 1), rectangle_病人資訊);

            DrawingClass.Draw.文字左上繪製("病人姓名", 100, new Point(rectangle_病人資訊.X + 20, rectangle_病人資訊.Y + 20), new Font("標楷體", 12), Color.Black, g);
            DrawingClass.Draw.文字左上繪製("李偉豪", 250, new Point(rectangle_病人資訊.X + 30, rectangle_病人資訊.Y + 50), new Font("微軟正黑體", 30,FontStyle.Bold), Color.Black, g);

            DrawingClass.Draw.文字左上繪製("性別", 50, new Point(rectangle_病人資訊.X + 350, rectangle_病人資訊.Y + 20), new Font("標楷體", 12), Color.Black, g);
            DrawingClass.Draw.文字左上繪製("男", 50, new Point(rectangle_病人資訊.X + 380, rectangle_病人資訊.Y + 50), new Font("微軟正黑體", 30, FontStyle.Bold), Color.Black, g);

            DrawingClass.Draw.文字左上繪製("年齡", 50, new Point(rectangle_病人資訊.X + 500, rectangle_病人資訊.Y + 20), new Font("標楷體", 12), Color.Black, g);
            DrawingClass.Draw.文字左上繪製("35歲", 150, new Point(rectangle_病人資訊.X + 530, rectangle_病人資訊.Y + 50), new Font("微軟正黑體", 30, FontStyle.Bold), Color.Black, g);

            g.DrawLine(new Pen(Color.Black, 1), 30, (rectangle_病人資訊.Y + rectangle_病人資訊.Height / 2) , rectangle_病人資訊.Right - 30, (rectangle_病人資訊.Y + rectangle_病人資訊.Height / 2));

            DrawingClass.Draw.文字左上繪製("病歷號碼", 100, new Point(rectangle_病人資訊.X + 20, (rectangle_病人資訊.Y + rectangle_病人資訊.Height / 2) + 20), new Font("標楷體", 12), Color.Black, g);
            DrawingClass.Draw.文字左上繪製("A5665", 180, new Point(rectangle_病人資訊.X + 30, (rectangle_病人資訊.Y + rectangle_病人資訊.Height / 2) + 50), new Font("微軟正黑體", 30, FontStyle.Bold), Color.Black, g);

            DrawingClass.Draw.文字左上繪製("科別", 50, new Point(rectangle_病人資訊.X + 250, (rectangle_病人資訊.Y + rectangle_病人資訊.Height / 2) + 20), new Font("標楷體", 12), Color.Black, g);
            DrawingClass.Draw.文字左上繪製("耳鼻喉科", 180, new Point(rectangle_病人資訊.X + 255, (rectangle_病人資訊.Y + rectangle_病人資訊.Height / 2) + 50), new Font("微軟正黑體", 30, FontStyle.Bold), Color.Black, g);

            DrawingClass.Draw.文字左上繪製("入住日期", 100, new Point(rectangle_病人資訊.X + 400, (rectangle_病人資訊.Y + rectangle_病人資訊.Height / 2) + 20), new Font("標楷體", 12), Color.Black, g);
            DrawingClass.Draw.文字左上繪製("2021/11/25", 300, new Point(rectangle_病人資訊.X + 420, (rectangle_病人資訊.Y + rectangle_病人資訊.Height / 2) + 50), new Font("微軟正黑體", 30, FontStyle.Bold), Color.Black, g);


            Rectangle rectangle_入住資訊 = new Rectangle(rectangle_出院日期.Right + 5, 0, 960 - (rectangle_出院日期.Right + 11), rectangle_病人資訊.Bottom);
            g.DrawRectangle(new Pen(Color.Black, 1), rectangle_入住資訊);
            g.FillRectangle(new SolidBrush(Color.Black), new RectangleF(rectangle_入住資訊.X, rectangle_入住資訊.Y, rectangle_入住資訊.Width, 30));
            DrawingClass.Draw.文字中心繪製("入住資訊", new Rectangle(rectangle_入住資訊.X, rectangle_入住資訊.Y, rectangle_入住資訊.Width, 30), rectangle_入住資訊.Width - 20, new Font("微軟正黑體", 14, FontStyle.Bold), Color.White, g);
            DrawingClass.Draw.文字中心繪製("主治醫師", new Rectangle(rectangle_入住資訊.X, rectangle_入住資訊.Y + 50, rectangle_入住資訊.Width, 30), 120, new Font("標楷體", 12), Color.Black, g);
            DrawingClass.Draw.文字中心繪製("林冠宇", new Rectangle(rectangle_入住資訊.X, rectangle_入住資訊.Y + 90, rectangle_入住資訊.Width, 30), 180, new Font("微軟正黑體", 25, FontStyle.Bold), Color.Black, g);
          
            DrawingClass.Draw.文字中心繪製("臨床藥師", new Rectangle(rectangle_入住資訊.X, rectangle_入住資訊.Y + 180, rectangle_入住資訊.Width, 30), 120, new Font("標楷體", 12), Color.Black, g);
            DrawingClass.Draw.文字中心繪製("傑恩", new Rectangle(rectangle_入住資訊.X, rectangle_入住資訊.Y + 220, rectangle_入住資訊.Width, 30), 180, new Font("微軟正黑體", 25, FontStyle.Bold), Color.Black, g);

            DrawingClass.Draw.文字中心繪製("護理師", new Rectangle(rectangle_入住資訊.X, rectangle_入住資訊.Y + 310, rectangle_入住資訊.Width, 30), 120, new Font("標楷體", 12), Color.Black, g);
            DrawingClass.Draw.文字中心繪製("澄程", new Rectangle(rectangle_入住資訊.X, rectangle_入住資訊.Y + 350, rectangle_入住資訊.Width, 30), 180, new Font("微軟正黑體", 25, FontStyle.Bold), Color.Black, g);

            Rectangle rectangle_溫馨提醒 = new Rectangle(rectangle_病人資訊.X, rectangle_病人資訊.Bottom + 5, rectangle_病人資訊.Width, 640 - ( rectangle_病人資訊.Bottom + 10));
            g.DrawRectangle(new Pen(Color.Black, 1), rectangle_溫馨提醒);

            Image image_buf;
            Bitmap bitmap_buf;
            image_buf = Bitmap.FromFile(@"C:\Users\User\Desktop\H_Pannel\10.2inch ICON\防止跌倒.bmp");
            bitmap_buf = EPD_1020_Pannel.ScaleImage((Bitmap)image_buf, (int)(image_buf.Width * 0.13), (int)(image_buf.Height * 0.13));
            g.DrawImage(bitmap_buf, new Point(rectangle_溫馨提醒.X + 20, rectangle_溫馨提醒.Y + (int)((rectangle_溫馨提醒.Height - image_buf.Height * 0.13) / 2)));
            bitmap_buf.Dispose();
            image_buf.Dispose();

            image_buf = Bitmap.FromFile(@"C:\Users\User\Desktop\H_Pannel\10.2inch ICON\禁止下床.bmp");
            bitmap_buf = EPD_1020_Pannel.ScaleImage((Bitmap)image_buf, (int)(image_buf.Width * 0.13), (int)(image_buf.Height * 0.13));
            g.DrawImage(bitmap_buf, new Point(rectangle_溫馨提醒.X + 20  + (int)((image_buf.Width * 0.13 + 20) * 1), rectangle_溫馨提醒.Y + (int)((rectangle_溫馨提醒.Height - image_buf.Height * 0.13) / 2)));
            bitmap_buf.Dispose();
            image_buf.Dispose();

            image_buf = Bitmap.FromFile(@"C:\Users\User\Desktop\H_Pannel\10.2inch ICON\禁食.bmp");
            bitmap_buf = EPD_1020_Pannel.ScaleImage((Bitmap)image_buf, (int)(image_buf.Width * 0.13), (int)(image_buf.Height * 0.13));
            g.DrawImage(bitmap_buf, new Point(rectangle_溫馨提醒.X + 20 + (int)((image_buf.Width * 0.13 + 20) * 2), rectangle_溫馨提醒.Y + (int)((rectangle_溫馨提醒.Height - image_buf.Height * 0.13) / 2)));
            bitmap_buf.Dispose();
            image_buf.Dispose();

            Rectangle rectangle_注意事項 = new Rectangle(rectangle_溫馨提醒.Right + 5, rectangle_溫馨提醒.Y, rectangle_入住資訊.Width, rectangle_溫馨提醒.Height);
            g.FillRectangle(new SolidBrush(Color.Red), new RectangleF(rectangle_注意事項.X, rectangle_注意事項.Y, rectangle_注意事項.Width, rectangle_注意事項.Height));


            image_buf = Bitmap.FromFile(@"C:\Users\User\Desktop\H_Pannel\10.2inch ICON\佩戴口罩.bmp");
            bitmap_buf = EPD_1020_Pannel.ScaleImage((Bitmap)image_buf, (int)(image_buf.Width * 0.12), (int)(image_buf.Height * 0.12));;
            g.DrawImage(bitmap_buf, new Point((int)(rectangle_注意事項.X + (rectangle_注意事項.Width - bitmap_buf.Width) / 2), (int)(rectangle_注意事項.Y + (rectangle_注意事項.Height - bitmap_buf.Height) / 2)));

            bitmap_buf.Dispose();
            image_buf.Dispose();
            string BW = "";
            string RW = "";
  
           
            this.epD_1020_Pannel.DrawToPictureBox(bitmap);


            this.drawerUI_EPD_1020.DrawToEpd_UDP("192.168.0.150", 29012, bitmap);
            bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
            Communication.BitmapToHexString(bitmap, ref BW, ref RW, EPD_Type.EPD1020);
            bitmap.Dispose();
            g.Dispose();
            

        }
        private void RJ_Button_EPD_1020_門片畫面測試_MouseDownEvent(MouseEventArgs mevent)
        {
            Drawer drawer = new Drawer();
            for (int i = 0; i < 15; i++)
            {
                Box box0 = new Box();
                box0.Code = $"0001A-{i}";
                box0.Name = $"測試{i}";
                drawer.Add_NewBox(box0);
            }
            this.epD_1020_Pannel.DrawToPictureBox(drawer);
        }
        #endregion
        private void sub_H_RFID()
        {
            this.sqL_DataGridView_h_RFID_Datas.Init();
            List<H_RFID_UI.UDP_READ.RFID_Data> rFID_UID_Classes = this.h_RFID_UI.GetRFID();
            List<object[]> list_value = new List<object[]>();
            for(int i = 0; i < rFID_UID_Classes.Count; i++)
            {
                object[] value = new object[new enum_H_RFID_Datas().GetLength()];
                value[(int)enum_H_RFID_Datas.GUID] = Guid.NewGuid().ToString();
                value[(int)enum_H_RFID_Datas.IP] = rFID_UID_Classes[i].IP;
                value[(int)enum_H_RFID_Datas.CardID] = rFID_UID_Classes[i].Card_ID;
                value[(int)enum_H_RFID_Datas.RSSI] = rFID_UID_Classes[i].RSSI;
                list_value.Add(value);
            }
            this.sqL_DataGridView_h_RFID_Datas.RefreshGrid(list_value);
        }

    }
}
