﻿using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using Basic;
using MyUI;
using System.Reflection;
using CCWin.SkinControl;
using SkiaSharp;
namespace H_Pannel_lib
{
    public partial class StorageUI_EPD_266 : StorageUI
    {
        IpLockManager ipLockManager = new IpLockManager();
        static public double Lightness = 1.0D;

        [Serializable]
        public class UDP_READ : UDP_READ_basic
        {
            MyConvert myConvert = new MyConvert();
          

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

        #region 靜態參數
        public delegate Bitmap Get_Storage_bmpEventHandler(Storage storage);
        static public event Get_Storage_bmpEventHandler Get_Storage_bmpChangeEvent;

        static public int Pannel_Width
        {
            get
            {
                return 296;
            }
        }
        static public int Pannel_Height
        {
            get
            {
                return 152;
            }
        }
        static public int NumOfLED = 10;
        static public byte[] Get_Empty_LEDBytes()
        {
            return new byte[NumOfLED * 3];
        }
        static public byte[] Get_Pannel_LEDBytes(Color color)
        {
            byte[] LED_Bytes = Get_Empty_LEDBytes();

            for (int i = 0; i < NumOfLED; i++)
            {
                LED_Bytes[i * 3 + 0] = (byte)(color.R * Lightness);
                LED_Bytes[i * 3 + 1] = (byte)(color.G * Lightness);
                LED_Bytes[i * 3 + 2] = (byte)(color.B * Lightness);
            }

            return LED_Bytes;
        }
        static public byte[] Get_LightStateLEDBytes(Storage storage)
        {
            byte[] LED_Bytes = Get_Empty_LEDBytes();

            if (storage.LightState.State)
            {
                if (storage.LightState.Interval > 0)
                {
                    // 計算時間間隔
                    TimeSpan interval = DateTime.Now - storage.LightState.LightingDateTime;
                    // 取得毫秒間隔
                    double milliseconds = interval.TotalMilliseconds;
                    if ((int)(milliseconds / storage.LightState.Interval) % 2 == 0)
                    {
                        return Get_Empty_LEDBytes(); 
                    }
                }
                if (storage.LightState.LightOffTime > 0)
                {
                    TimeSpan interval = DateTime.Now - storage.LightState.LightingDateTime;
                    double milliseconds = interval.TotalMilliseconds;
                    if (milliseconds > storage.LightState.LightOffTime)
                    {
                        storage.LightState.State = false;
                        return Get_Empty_LEDBytes();
                    }

                }
                LED_Bytes = Get_Pannel_LEDBytes(storage.LightState.LightColor);
            }
            return LED_Bytes;
        }

        static public bool Set_Stroage_TOF(UDP_Class uDP_Class, string IP, bool value)
        {
            return Communication.Set_TOF(uDP_Class, IP, value);
        }
        static public bool Set_Stroage_LED_UDP(UDP_Class uDP_Class, string IP, Color color)
        {
            byte[] LED_Bytes = Get_Pannel_LEDBytes(color);
            return Set_Stroage_LED_UDP(uDP_Class, IP, LED_Bytes);
        }
        static public bool Set_WS2812B_breathing(UDP_Class uDP_Class, string IP, byte WS2812B_breathing_onAddVal, byte WS2812B_breathing_offSubVal, Color color)
        {
            if (uDP_Class != null)
            {
                return Communication.Set_WS2812B_breathing(uDP_Class, IP, WS2812B_breathing_onAddVal, WS2812B_breathing_offSubVal, color);
            }
            return false;
        }
        static public bool Set_Stroage_LED_UDP(UDP_Class uDP_Class, string IP, byte[] LED_Bytes)
        {
            if (uDP_Class != null)
            {
                bool flag = Communication.Set_WS2812_Buffer(uDP_Class, IP, 0, LED_Bytes);
                //System.Threading.Thread.Sleep(50);
                return flag;
            }
            return false;
        }
        static public bool DrawToEpd_UDP(UDP_Class uDP_Class, Storage storage)
        {
            using (Bitmap bitmap = Get_Storage_bmp(storage))
            {
                return DrawToEpd_UDP(uDP_Class, storage.IP, bitmap);
            }
        }
        static public bool DrawToEpd_UDP(UDP_Class uDP_Class, string IP, Bitmap bitmap)
        {
          
            if (uDP_Class != null)
            {
                bool flag_ok = false;
                if (bitmap.Width == 250 && bitmap.Height == 122)
                {
                    flag_ok = Communication.EPD_213_DrawImage(uDP_Class, IP, bitmap);
                    return flag_ok;
                }
                if (bitmap.Width == 296 && bitmap.Height == 128)
                {
                    flag_ok = Communication.EPD_290_DrawImage(uDP_Class, IP, bitmap);
                    return flag_ok;
                }
                if (bitmap.Width == 296 && bitmap.Height == 152)
                {
                    flag_ok = Communication.EPD_266_DrawImage(uDP_Class, IP, bitmap);
                    return flag_ok;
                }
                if (bitmap.Width == 400 && bitmap.Height == 300)
                {
                    flag_ok = Communication.EPD_420_DrawImage(uDP_Class, IP, bitmap);
                    return flag_ok;
                }

            }
            return false;
        }

        static public bool Set_LockOpen(UDP_Class uDP_Class, string IP)
        {
            return Communication.Set_OutputPINTrigger(uDP_Class, IP, 1, true);
        }

        static public int Get_LaserDistance(UDP_Class uDP_Class, string IP)
        {
            if (uDP_Class != null)
            {
                return Communication.EPD_Get_LaserDistance(uDP_Class, IP);
            }
            return -999;
        }
        public List<UDP_READ> GerAllUDP_READ()
        {
            List<UDP_READ> uDP_READs = new List<UDP_READ>();

            List<string> list_UDPJsonString = this.GetAllUDPJsonString();

            Parallel.ForEach(list_UDPJsonString, value =>
            {
                string jsonString = value;
                UDP_READ uDP_READ = jsonString.JsonDeserializet<UDP_READ>();
                if (uDP_READ != null) uDP_READs.LockAdd(uDP_READ);
            });

            return uDP_READs;
        }
        static public Bitmap Get_Storage_bmp(Storage storage)
        {
            if (Get_Storage_bmpChangeEvent != null) return Get_Storage_bmpChangeEvent(storage);
            return Communication.Storage_GetBitmap(storage);

        }
        static private void DrawStorageString(Graphics g, Storage storage, Device.ValueName valueName, float x, float y)
        {
            string str = (string)storage.GetValue(valueName, Device.ValueType.Value);
            Font font = (Font)storage.GetValue(valueName, Device.ValueType.Font);
            Color foreColor = (Color)storage.GetValue(valueName, Device.ValueType.ForeColor);
            int borderSize = (int)storage.GetValue(valueName, Device.ValueType.BorderSize);
            Color borderColor = (Color)storage.GetValue(valueName, Device.ValueType.BorderColor);

            DrawStorageString(g, str, font, foreColor, borderSize, borderColor, x, y);
        }
        static private void DrawStorageString(Graphics g, string str, Font font, Color ForeColor, int BorderSize, Color BorderColor, float x, float y)
        {
            SizeF size_font = TextRenderer.MeasureText(str, font);

            g.SmoothingMode = SmoothingMode.HighQuality; //使繪圖質量最高，即消除鋸齒
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

            g.DrawString(str, font, new SolidBrush(ForeColor), x, y);
            if (BorderSize > 0)
            {
                Pen pen = new Pen(new SolidBrush(BorderColor), 1);
                g.DrawRectangle(pen, x, y, size_font.Width, size_font.Height);
            }            
        }
        #endregion

        private enum ContextMenuStrip_Main
        {
            畫面設置,
            IO設定,
            面板種類,
        }
        private enum ContextMenuStrip_DeviceTable_畫面設置
        {
            清除畫布,
            測試資訊,
            繪製面板,
            
        }
        private enum ContextMenuStrip_DeviceTable_IO設定
        {
            面板亮燈,
            IO測試,
            雷射開啟,
            雷射關閉,
        }
        private enum ContextMenuStrip_UDP_DataReceive_畫面設置
        {
            清除畫布,
            測試資訊,
            測試資訊_UART,
        }
        private enum ContextMenuStrip_UDP_DataReceive_IO設定
        {
            面板亮燈,
            IO測試,
            雷射開啟,
            雷射關閉,
        }
        private enum ContextMenuStrip_UDP_DataReceive_面板種類
        {
            設為EPD213有鎖控,
            設為EPD213無鎖控,
            設為EPD266有鎖控,
            設為EPD266無鎖控,
            設為EPD290有鎖控,
            設為EPD290無鎖控,
            設為EPD420有鎖控,
            設為EPD420無鎖控,
        }
        public StorageUI_EPD_266()
        {
            this.TableName = "EPD266_Jsonstring";
            this.DeviceTableMouseDownRightEvent += StorageUI_EPD_266_DeviceTableMouseDownRightEvent;
            this.UDP_DataReceiveMouseDownRightEvent += StorageUI_EPD_266_UDP_DataReceiveMouseDownRightEvent;

            this.sqL_DataGridView_DeviceTable.DataGridRefreshEvent += SqL_DataGridView_DeviceTable_DataGridRefreshEvent;

            Enum_ContextMenuStrip_DeviceTable = new ContextMenuStrip_Main();
            Enum_ContextMenuStrip_UDP_DataReceive = new ContextMenuStrip_Main();
        }

        private void SqL_DataGridView_DeviceTable_DataGridRefreshEvent()
        {
            for (int i = 0; i < this.sqL_DataGridView_DeviceTable.dataGridView.Rows.Count; i++)
            {
                string jsonString = this.sqL_DataGridView_DeviceTable.dataGridView.Rows[i].Cells[(int)enum_DeviceTable.Value].Value.ObjectToString();
                Storage storage = jsonString.JsonDeserializet<Storage>();
                if (storage != null)
                {
                    Color color = Color.White;
                    if (storage.DeviceType == DeviceType.EPD266_lock || storage.DeviceType == DeviceType.EPD266)
                    {
                        this.sqL_DataGridView_DeviceTable.dataGridView.Rows[i].DefaultCellStyle.BackColor = Color.Yellow;
                        this.sqL_DataGridView_DeviceTable.dataGridView.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
                    }
                    if (storage.DeviceType == DeviceType.EPD290_lock || storage.DeviceType == DeviceType.EPD290)
                    {
                        this.sqL_DataGridView_DeviceTable.dataGridView.Rows[i].DefaultCellStyle.BackColor = Color.HotPink;
                        this.sqL_DataGridView_DeviceTable.dataGridView.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
                    }
                }
            }
        }

        public UDP_READ Get_UDP_READ(string IP)
        {
            string json = this.GetUDPJsonString(IP);
            if (json.StringIsEmpty()) return null;
            UDP_READ uDP_READ = json.JsonDeserializet<UDP_READ>();
            return uDP_READ;
        }
        protected override void SQL_AddDevice(string IP, int Port)
        {
            List<object[]> list_SQL_Value = this.sqL_DataGridView_DeviceTable.SQL_GetAllRows(false);
            List<object[]> list_SQL_Value_buf = new List<object[]>();
            list_SQL_Value_buf = list_SQL_Value.GetRows((int)enum_DeviceTable.IP, IP);
            if (list_SQL_Value_buf.Count == 0)
            {
                object[] value = new object[new enum_DeviceTable().GetEnumNames().Length];
                Storage storage = new Storage(IP, Port);
                storage.DeviceType = DeviceType.EPD266_lock;
                value[(int)enum_DeviceTable.GUID] = Guid.NewGuid().ToString();
                value[(int)enum_DeviceTable.IP] = IP;
                value[(int)enum_DeviceTable.Port] = Port;
                value[(int)enum_DeviceTable.Value] = storage.JsonSerializationt<Storage>();
                this.sqL_DataGridView_DeviceTable.SQL_AddRow(value, true);
            }
            else
            {
                string GUID = list_SQL_Value_buf[0][(int)enum_DeviceTable.GUID].ObjectToString();
                list_SQL_Value_buf[0][(int)enum_DeviceTable.Port] = Port;
                string jsonString = list_SQL_Value_buf[0][(int)enum_DeviceTable.Value].ObjectToString();
                if (jsonString.StringIsEmpty()) return;
                Storage storage = jsonString.JsonDeserializet<Storage>();
                if (storage == null) return;
                storage.Port = Port;
                list_SQL_Value_buf[0][(int)enum_DeviceTable.Value] = storage.JsonSerializationt();
                this.sqL_DataGridView_DeviceTable.SQL_Replace(enum_DeviceTable.GUID.GetEnumName(), GUID, list_SQL_Value_buf[0], true);
            }
        }
        public bool Set_Stroage_LED_UDP(Storage storage, Color color)
        {
            byte[] LED_Bytes = Get_Pannel_LEDBytes(color);
            return this.Set_Stroage_LED_UDP(storage.IP, storage.Port, LED_Bytes);
        }
        public bool Set_Stroage_LED_UDP(string IP, int Port, Color color)
        {
            byte[] LED_Bytes = Get_Pannel_LEDBytes(color);
            return this.Set_Stroage_LED_UDP(IP, Port, LED_Bytes);

        }
        public bool Set_TOF(string IP, int Port, bool statu)
        {
            string ipAddress = IP;
            using (var lockHandle = ipLockManager.LockIp(ipAddress))
            {
                Console.WriteLine($"IP {ipAddress} 已成功鎖定");
                // 執行針對該 IP 的相關操作...
                UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
                if (uDP_Class == null) return false;
                return Set_Stroage_TOF(uDP_Class, IP, statu);
            }

         
        }
        public bool Set_Stroage_LED_UDP(string IP, int Port, byte[] LED_Bytes)
        {
            string ipAddress = IP;
            using (var lockHandle = ipLockManager.LockIp(ipAddress))
            {
                Console.WriteLine($"IP {ipAddress} 已成功鎖定");
                // 執行針對該 IP 的相關操作...
                UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
                return Set_Stroage_LED_UDP(uDP_Class, IP, LED_Bytes);
            }

         
        }
        public byte[] Get_Stroage_LED_UDP(string IP, int Port)
        {
            string ipAddress = IP;
            using (var lockHandle = ipLockManager.LockIp(ipAddress))
            {
                Console.WriteLine($"IP {ipAddress} 已成功鎖定");
                // 執行針對該 IP 的相關操作...
                UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
                return Communication.Get_WS2812_Buffer(uDP_Class, IP, NumOfLED * 3);
            }

           
        }
        public bool Set_ClearCanvas_UART()
        {
            return this.Set_ClearCanvas_UART(mySerialPort.PortName);
        }
        public bool Set_ClearCanvas_UART(string PortName)
        {
            mySerialPort.PortName = PortName;
            return Communication.UART_Command_Set_ClearCanvas(mySerialPort);
        }
        public bool EPD_290_DrawImages_UART(Bitmap bitmap)
        {
            return EPD_290_DrawImages_UART(mySerialPort.PortName, bitmap);
        }
        public bool EPD_290_DrawImages_UART(string PortName , Bitmap bitmap)
        {
            mySerialPort.PortName = PortName;
            return Communication.UART_EPD_290_DrawImage(mySerialPort, bitmap);
        }
        public bool Set_Stroage_LED_UART(Color color)
        {
            return this.Set_Stroage_LED_UART(mySerialPort.PortName, color);
        }
        public bool Set_Stroage_LED_UART(string PortName, Color color)
        {
            mySerialPort.PortName = PortName;
            byte[] LED_Bytes = Get_Pannel_LEDBytes(color);
            return Communication.UART_Command_Set_WS2812_Buffer(mySerialPort, 0, LED_Bytes);
        }
        public bool DrawToEpd_UDP(string IP)
        {
            string ipAddress = IP;
            using (var lockHandle = ipLockManager.LockIp(ipAddress))
            {
                Console.WriteLine($"IP {ipAddress} 已成功鎖定");
                // 執行針對該 IP 的相關操作...
                Storage storage = this.SQL_GetStorage(IP);
                using (Bitmap bitmap = Get_Storage_bmp(storage))
                {
                    return DrawToEpd_UDP(IP, storage.Port, bitmap);
                }
            }

        
        }
        public bool DrawToEpd_UDP(Storage storage)
        {
            string ipAddress = storage.IP;
            using (var lockHandle = ipLockManager.LockIp(ipAddress))
            {
                Console.WriteLine($"IP {ipAddress} 已成功鎖定");
                UDP_Class uDP_Class = List_UDP_Local.SortByPort(storage.Port);
                return DrawToEpd_UDP(uDP_Class, storage);
            }


         
        }
        public bool DrawToEpd_UDP(string IP, int Port, Bitmap bitmap)
        {
            string ipAddress = IP;
            using (var lockHandle = ipLockManager.LockIp(ipAddress))
            {
                Console.WriteLine($"IP {ipAddress} 已成功鎖定");
                UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
                return DrawToEpd_UDP(uDP_Class, IP, bitmap);
            }

         
        }
        public bool Set_WS2812B_breathing(Storage storage, byte WS2812B_breathing_onAddVal, byte WS2812B_breathing_offSubVal, Color color)
        {
            return Set_WS2812B_breathing(storage.IP, storage.Port, WS2812B_breathing_onAddVal, WS2812B_breathing_offSubVal, color);
        }
        public bool Set_WS2812B_breathing(string IP, int Port, byte WS2812B_breathing_onAddVal, byte WS2812B_breathing_offSubVal, Color color)
        {
            string ipAddress = IP;
            using (var lockHandle = ipLockManager.LockIp(ipAddress))
            {
                Console.WriteLine($"IP {ipAddress} 已成功鎖定");
                UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
                return Set_WS2812B_breathing(uDP_Class, IP, WS2812B_breathing_onAddVal, WS2812B_breathing_offSubVal, color);
            }

      
        }
        public bool Set_LockOpen(Storage storage)
        {
            return Set_LockOpen(storage.IP, storage.Port);
        }
        public bool Set_LockOpen(string IP, int Port)
        {
            string ipAddress = IP;
            using (var lockHandle = ipLockManager.LockIp(ipAddress))
            {
                Console.WriteLine($"IP {ipAddress} 已成功鎖定");
                UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
                return Set_LockOpen(uDP_Class, IP);

            }
           
        }
        public bool Set_OutputPIN(Storage storage, bool state)
        {
            return Set_OutputPIN(storage.IP, storage.Port, state);
        }
        public bool Set_OutputPIN(string IP, int Port, bool state)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
            return Communication.Set_OutputPIN(uDP_Class, IP, 1, state);
        }
        public bool Set_ADCMotorTrigger(Storage storage, int time_ms)
        {
            return Set_ADCMotorTrigger(storage.IP, storage.Port, time_ms);
        }
        public bool Set_ADCMotorTrigger(string IP, int Port, int time_ms)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
            return Communication.Command_Set_ADCMotorTrigger(uDP_Class, IP, 0, time_ms);
        }
        public bool Get_OutputPIN(Storage storage)
        {
            return Get_OutputPIN(storage.IP);
        }
        public bool Get_OutputPIN(string IP)
        {
            string jsonString = GetUDPJsonString(IP);
            if (jsonString.StringIsEmpty()) return false;
            UDP_READ uDP_READ = jsonString.JsonDeserializet<UDP_READ>();
            if (uDP_READ == null) return false;
            return ((uDP_READ.Output % 2) == 1);
        }
        public bool GetInput(string IP)
        {
            string jsonString = GetUDPJsonString(IP);
            if (jsonString.StringIsEmpty()) return false;
            UDP_READ uDP_READ = jsonString.JsonDeserializet<UDP_READ>();
            if (uDP_READ == null) return false;
            return ((uDP_READ.Input % 2 )==  1);
        }

        public int Get_LaserDistance(Storage storage)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(storage.Port);
            return Get_LaserDistance(uDP_Class, storage.IP);
        }
        public int Get_LaserDistance(string IP, int Port)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
            return Get_LaserDistance(uDP_Class, IP);
        }

        private void StorageUI_EPD_266_DeviceTableMouseDownRightEvent(string selectedText, List<System.Net.IPEndPoint> iPEndPoints)
        {
            if (selectedText == ContextMenuStrip_Main.畫面設置.GetEnumName())
            {
                Dialog_ContextMenuStrip dialog_ContextMenuStrip = new Dialog_ContextMenuStrip(new ContextMenuStrip_DeviceTable_畫面設置().GetEnumNames());
                if (dialog_ContextMenuStrip.ShowDialog() == System.Windows.Forms.DialogResult.Yes)
                {
                    if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable_畫面設置.清除畫布.GetEnumName())
                    {
                        List<Task> taskList = new List<Task>();
                        for (int i = 0; i < iPEndPoints.Count; i++)
                        {
                            string IP = iPEndPoints[i].Address.ToString();
                            int Port = iPEndPoints[i].Port;
                            Storage storage = this.SQL_GetStorage(IP);
                            if (storage == null) continue;
                            Bitmap bmp = null;
                            bmp = new Bitmap(storage.PanelSize.Width, storage.PanelSize.Height);
                            Graphics g = Graphics.FromImage(bmp);
                            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                            g.SmoothingMode = SmoothingMode.HighQuality; //使繪圖質量最高，即消除鋸齒
                            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            g.CompositingQuality = CompositingQuality.HighQuality;
                            g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
                            g.FillRectangle(new SolidBrush(Color.Red), rect);
                            g.Dispose();
                   
                            taskList.Add(Task.Run(() =>
                            {
                                DrawToEpd_UDP(IP, Port, bmp);
                                bmp.Dispose();
                            }));
                        }
                        Task allTask = Task.WhenAll(taskList);
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable_畫面設置.測試資訊.GetEnumName())
                    {
                        List<Task> taskList = new List<Task>();
                        for (int i = 0; i < iPEndPoints.Count; i++)
                        {
                            string IP = iPEndPoints[i].Address.ToString();
                            int Port = iPEndPoints[i].Port;
                            Storage storage = this.SQL_GetStorage(IP);
                            if (storage != null)
                            {
                                Bitmap bmp = Get_Storage_bmp(storage);
                                taskList.Add(Task.Run(() =>
                                {
                                    DrawToEpd_UDP(IP, Port, bmp);
                                    bmp.Dispose();
                                }));
                            }
                            
                        }
                        Task allTask = Task.WhenAll(taskList);
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable_畫面設置.繪製面板.GetEnumName())
                    {
                        List<Task> taskList = new List<Task>();
                        for (int i = 0; i < iPEndPoints.Count; i++)
                        {
                            string IP = iPEndPoints[i].Address.ToString();
                            int Port = iPEndPoints[i].Port;
                            Storage storage = this.SQL_GetStorage(IP);
                            if (storage != null)
                            {
                                Bitmap bmp = Get_Storage_bmp(storage);
                                int width = bmp.Width;
                                int height = bmp.Height;
                                bmp.RotateFlip(RotateFlipType.Rotate90FlipXY);
                                byte[] bytes_BW = new byte[(height / 8) * width];
                                byte[] bytes_RW = new byte[(height / 8) * width];
                                Communication.BitmapToByte(bmp, ref bytes_BW, ref bytes_RW, EPD_Type.EPD290_V2);
                                string jsonString_RW = ByteToStringHex(bytes_RW);
                                string jsonString_BW = ByteToStringHex(bytes_BW);
                               
                                taskList.Add(Task.Run(() =>
                                {
                                    DrawToEpd_UDP(IP, Port, bmp);
                                    bmp.Dispose();
                                }));
                            }

                        }
                        Task allTask = Task.WhenAll(taskList);
                    }
                }
            }
            else if (selectedText == ContextMenuStrip_Main.IO設定.GetEnumName())
            {
                Dialog_ContextMenuStrip dialog_ContextMenuStrip = new Dialog_ContextMenuStrip(new ContextMenuStrip_DeviceTable_IO設定().GetEnumNames());
                if (dialog_ContextMenuStrip.ShowDialog() == DialogResult.Yes)
                {
                    if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable_IO設定.面板亮燈.GetEnumName())
                    {
                        List<Task> taskList = new List<Task>();
                        ColorDialog colorDialog = new ColorDialog();
                        if (colorDialog.ShowDialog() != DialogResult.OK) return;
                        for (int i = 0; i < iPEndPoints.Count; i++)
                        {
                            string IP = iPEndPoints[i].Address.ToString();
                            int Port = iPEndPoints[i].Port;
                            Color color = colorDialog.Color;
                            taskList.Add(Task.Run(() =>
                            {
                                Set_Stroage_LED_UDP(IP, Port, color);
                            }));
                        }
                        Task allTask = Task.WhenAll(taskList);
                    }
                    if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable_IO設定.IO測試.GetEnumName())
                    {
                        List<object[]> list_value = this.sqL_DataGridView_DeviceTable.Get_All_Select_RowsValues();
                        if (list_value.Count == 0) return;
                        string IP = list_value[0][(int)enum_DeviceTable.IP].ObjectToString();
                        int Port = list_value[0][(int)enum_DeviceTable.Port].ObjectToString().StringToInt32();
                        UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
                        if (uDP_Class == null) return;
                        Dialog_IO測試 Dialog_IO測試 = new Dialog_IO測試(uDP_Class, IP, list_UDP_Rx);
                        Dialog_IO測試.ShowDialog();
                    }
                    if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable_IO設定.雷射開啟.GetEnumName())
                    {
                        LoadingForm.ShowLoadingForm();
                        List<Storage> storages_replace = new List<Storage>();
                        List<Task> taskList = new List<Task>();
                        for (int i = 0; i < iPEndPoints.Count; i++)
                        {
                            string IP = iPEndPoints[i].Address.ToString();
                            int Port = iPEndPoints[i].Port;


                            taskList.Add(Task.Run(() =>
                            {
                                Storage storage = this.SQL_GetStorage(IP);
                                if (storage == null) return;
                                storage.TOFON = true;
                                this.SQL_ReplaceStorage(storage);
                            }));
                        }
                        Task allTask = Task.WhenAll(taskList);
                        LoadingForm.CloseLoadingForm();
                    }
                    if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable_IO設定.雷射關閉.GetEnumName())
                    {
                        LoadingForm.ShowLoadingForm();
                        List<Storage> storages_replace = new List<Storage>();
                        List<Task> taskList = new List<Task>();
                        for (int i = 0; i < iPEndPoints.Count; i++)
                        {
                            string IP = iPEndPoints[i].Address.ToString();
                            int Port = iPEndPoints[i].Port;


                            taskList.Add(Task.Run(() =>
                            {
                                Storage storage = this.SQL_GetStorage(IP);
                                if (storage == null) return;
                                storage.TOFON = false;
                                this.SQL_ReplaceStorage(storage);
                                //Set_TOF(IP, Port, false);
                            }));
                        }
                        Task allTask = Task.WhenAll(taskList);
                        LoadingForm.CloseLoadingForm();

                    }
                }

            }
            else if (selectedText == ContextMenuStrip_Main.面板種類.GetEnumName())
            {
                Dialog_ContextMenuStrip dialog_ContextMenuStrip = new Dialog_ContextMenuStrip(new ContextMenuStrip_UDP_DataReceive_面板種類().GetEnumNames());
                if (dialog_ContextMenuStrip.ShowDialog() != DialogResult.Yes) return;
                if (dialog_ContextMenuStrip.Value == ContextMenuStrip_UDP_DataReceive_面板種類.設為EPD213有鎖控.GetEnumName())
                {
                    List<Task> taskList = new List<Task>();
                    for (int i = 0; i < iPEndPoints.Count; i++)
                    {
                        string IP = iPEndPoints[i].Address.ToString();
                        int Port = iPEndPoints[i].Port;
                        Storage storage = this.SQL_GetStorage(IP);
                        if (storage == null) continue;
                        storage.SetDeviceType(DeviceType.EPD213_lock);
                        taskList.Add(Task.Run(() =>
                        {
                            this.SQL_ReplaceStorage(storage);
                        }));
                    }
                    Task allTask = Task.WhenAll(taskList);
                    this.sqL_DataGridView_DeviceTable.SQL_GetAllRows(true);
                }
                else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_UDP_DataReceive_面板種類.設為EPD213無鎖控.GetEnumName())
                {
                    List<Task> taskList = new List<Task>();
                    for (int i = 0; i < iPEndPoints.Count; i++)
                    {
                        string IP = iPEndPoints[i].Address.ToString();
                        int Port = iPEndPoints[i].Port;
                        Storage storage = this.SQL_GetStorage(IP);
                        if (storage == null) continue;
                        storage.SetDeviceType(DeviceType.EPD213);
                        taskList.Add(Task.Run(() =>
                        {
                            this.SQL_ReplaceStorage(storage);
                        }));
                    }
                    Task allTask = Task.WhenAll(taskList);
                    this.sqL_DataGridView_DeviceTable.SQL_GetAllRows(true);
                }
                else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_UDP_DataReceive_面板種類.設為EPD266有鎖控.GetEnumName())
                {
                    List<Task> taskList = new List<Task>();
                    for (int i = 0; i < iPEndPoints.Count; i++)
                    {
                        string IP = iPEndPoints[i].Address.ToString();
                        int Port = iPEndPoints[i].Port;
                        Storage storage = this.SQL_GetStorage(IP);
                        if (storage == null) continue;
                        storage.SetDeviceType(DeviceType.EPD266_lock);
                        taskList.Add(Task.Run(() =>
                        {
                            this.SQL_ReplaceStorage(storage);
                        }));
                    }
                    Task allTask = Task.WhenAll(taskList);
                    this.sqL_DataGridView_DeviceTable.SQL_GetAllRows(true);
                }
                else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_UDP_DataReceive_面板種類.設為EPD266無鎖控.GetEnumName())
                {
                    List<Task> taskList = new List<Task>();
                    for (int i = 0; i < iPEndPoints.Count; i++)
                    {
                        string IP = iPEndPoints[i].Address.ToString();
                        int Port = iPEndPoints[i].Port;
                        Storage storage = this.SQL_GetStorage(IP);
                        if (storage == null) continue;
                        storage.SetDeviceType(DeviceType.EPD266);
                        taskList.Add(Task.Run(() =>
                        {
                            this.SQL_ReplaceStorage(storage);
                        }));
                    }
                    Task allTask = Task.WhenAll(taskList);
                    this.sqL_DataGridView_DeviceTable.SQL_GetAllRows(true);
                }
                else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_UDP_DataReceive_面板種類.設為EPD290有鎖控.GetEnumName())
                {
                    List<Task> taskList = new List<Task>();
                    for (int i = 0; i < iPEndPoints.Count; i++)
                    {
                        string IP = iPEndPoints[i].Address.ToString();
                        int Port = iPEndPoints[i].Port;
                        Storage storage = this.SQL_GetStorage(IP);
                        if (storage == null) continue;
                        storage.SetDeviceType(DeviceType.EPD290_lock);
                        taskList.Add(Task.Run(() =>
                        {
                            this.SQL_ReplaceStorage(storage);
                        }));
                    }
                    Task allTask = Task.WhenAll(taskList);
                    this.sqL_DataGridView_DeviceTable.SQL_GetAllRows(true);
                }
                else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_UDP_DataReceive_面板種類.設為EPD290無鎖控.GetEnumName())
                {
                    List<Task> taskList = new List<Task>();
                    for (int i = 0; i < iPEndPoints.Count; i++)
                    {
                        string IP = iPEndPoints[i].Address.ToString();
                        int Port = iPEndPoints[i].Port;
                        Storage storage = this.SQL_GetStorage(IP);
                        if (storage == null) continue;
                        storage.SetDeviceType(DeviceType.EPD290);
                        taskList.Add(Task.Run(() =>
                        {
                            this.SQL_ReplaceStorage(storage);
                        }));
                    }
                    Task allTask = Task.WhenAll(taskList);
                    this.sqL_DataGridView_DeviceTable.SQL_GetAllRows(true);
                }
                else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_UDP_DataReceive_面板種類.設為EPD290有鎖控.GetEnumName())
                {
                    List<Task> taskList = new List<Task>();
                    for (int i = 0; i < iPEndPoints.Count; i++)
                    {
                        string IP = iPEndPoints[i].Address.ToString();
                        int Port = iPEndPoints[i].Port;
                        Storage storage = this.SQL_GetStorage(IP);
                        if (storage == null) continue;
                        storage.SetDeviceType(DeviceType.EPD290_lock);
                        taskList.Add(Task.Run(() =>
                        {
                            this.SQL_ReplaceStorage(storage);
                        }));
                    }
                    Task allTask = Task.WhenAll(taskList);
                    this.sqL_DataGridView_DeviceTable.SQL_GetAllRows(true);
                }
                else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_UDP_DataReceive_面板種類.設為EPD420無鎖控.GetEnumName())
                {
                    List<Task> taskList = new List<Task>();
                    for (int i = 0; i < iPEndPoints.Count; i++)
                    {
                        string IP = iPEndPoints[i].Address.ToString();
                        int Port = iPEndPoints[i].Port;
                        Storage storage = this.SQL_GetStorage(IP);
                        if (storage == null) continue;
                        storage.SetDeviceType(DeviceType.EPD420);
                        taskList.Add(Task.Run(() =>
                        {
                            this.SQL_ReplaceStorage(storage);
                        }));
                    }
                    Task allTask = Task.WhenAll(taskList);
                    this.sqL_DataGridView_DeviceTable.SQL_GetAllRows(true);
                }
                else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_UDP_DataReceive_面板種類.設為EPD420有鎖控.GetEnumName())
                {
                    List<Task> taskList = new List<Task>();
                    for (int i = 0; i < iPEndPoints.Count; i++)
                    {
                        string IP = iPEndPoints[i].Address.ToString();
                        int Port = iPEndPoints[i].Port;
                        Storage storage = this.SQL_GetStorage(IP);
                        if (storage == null) continue;
                        storage.SetDeviceType(DeviceType.EPD420_lock);
                        taskList.Add(Task.Run(() =>
                        {
                            this.SQL_ReplaceStorage(storage);
                        }));
                    }
                    Task allTask = Task.WhenAll(taskList);
                    this.sqL_DataGridView_DeviceTable.SQL_GetAllRows(true);
                }
            }
           
        }
        private void StorageUI_EPD_266_UDP_DataReceiveMouseDownRightEvent(string selectedText, List<System.Net.IPEndPoint> iPEndPoints)
        {
            if (selectedText == ContextMenuStrip_Main.畫面設置.GetEnumName())
            {
                Dialog_ContextMenuStrip dialog_ContextMenuStrip = new Dialog_ContextMenuStrip(new ContextMenuStrip_UDP_DataReceive_畫面設置().GetEnumNames());
                if (dialog_ContextMenuStrip.ShowDialog() == System.Windows.Forms.DialogResult.Yes)
                {
                    if (dialog_ContextMenuStrip.Value == ContextMenuStrip_UDP_DataReceive_畫面設置.清除畫布.GetEnumName())
                    {
                        List<Task> taskList = new List<Task>();
                        for (int i = 0; i < iPEndPoints.Count; i++)
                        {
                            string IP = iPEndPoints[i].Address.ToString();
                            int Port = iPEndPoints[i].Port;
                            Storage storage = this.SQL_GetStorage(IP);
                            if (storage == null) continue;
                            Bitmap bmp = null;
                            bmp = new Bitmap(storage.PanelSize.Width, storage.PanelSize.Height);
                            Graphics g = Graphics.FromImage(bmp);
                            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                            g.SmoothingMode = SmoothingMode.HighQuality; //使繪圖質量最高，即消除鋸齒
                            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            g.CompositingQuality = CompositingQuality.HighQuality;
                            g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
                            g.FillRectangle(new SolidBrush(Color.Red), rect);
                            g.Dispose();
                     
                            taskList.Add(Task.Run(() =>
                            {
                                DrawToEpd_UDP(IP, Port, bmp);
                                bmp.Dispose();
                            }));
                        }
                        Task allTask = Task.WhenAll(taskList);
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_UDP_DataReceive_畫面設置.測試資訊.GetEnumName())
                    {
                        List<Task> taskList = new List<Task>();
                        for (int i = 0; i < iPEndPoints.Count; i++)
                        {
                            string IP = iPEndPoints[i].Address.ToString();
                            int Port = iPEndPoints[i].Port;
                            Storage storage = this.SQL_GetStorage(IP);
                            if (storage != null)
                            {
                                Bitmap bmp = Get_Storage_bmp(storage);
                                taskList.Add(Task.Run(() =>
                                {
                                    DrawToEpd_UDP(IP, Port, bmp);
                                    bmp.Dispose();
                                }));
                            }

                        }
                        Task allTask = Task.WhenAll(taskList);
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_UDP_DataReceive_畫面設置.測試資訊_UART.GetEnumName())
                    {
                        List<Task> taskList = new List<Task>();
                        for (int i = 0; i < iPEndPoints.Count; i++)
                        {
                            string IP = iPEndPoints[i].Address.ToString();
                            int Port = iPEndPoints[i].Port;
                            Storage storage = this.SQL_GetStorage(IP);
                            if (storage != null)
                            {
                                Bitmap bmp = Get_Storage_bmp(storage);
                                taskList.Add(Task.Run(() =>
                                {
                                    EPD_290_DrawImages_UART(bmp);
                                    bmp.Dispose();
                                }));
                            }

                        }
                        Task allTask = Task.WhenAll(taskList);
                    }
                }
            }
            else if (selectedText == ContextMenuStrip_Main.IO設定.GetEnumName())
            {
                Dialog_ContextMenuStrip dialog_ContextMenuStrip = new Dialog_ContextMenuStrip(new ContextMenuStrip_UDP_DataReceive_IO設定().GetEnumNames());
                if (dialog_ContextMenuStrip.ShowDialog() == DialogResult.Yes)
                {
                    if (dialog_ContextMenuStrip.Value == ContextMenuStrip_UDP_DataReceive_IO設定.面板亮燈.GetEnumName())
                    {
                        List<Task> taskList = new List<Task>();
                        ColorDialog colorDialog = new ColorDialog();
                        if (colorDialog.ShowDialog() != DialogResult.OK) return;
                        for (int i = 0; i < iPEndPoints.Count; i++)
                        {
                            string IP = iPEndPoints[i].Address.ToString();
                            int Port = iPEndPoints[i].Port;
                            Color color = colorDialog.Color;
                            taskList.Add(Task.Run(() =>
                            {
                                Set_Stroage_LED_UDP(IP, Port, color);
                            }));
                        }
                        Task allTask = Task.WhenAll(taskList);
                    }
                    if (dialog_ContextMenuStrip.Value == ContextMenuStrip_UDP_DataReceive_IO設定.IO測試.GetEnumName())
                    {
                        if (iPEndPoints.Count == 0) return;
                        string IP = iPEndPoints[0].Address.ToString();
                        int Port = iPEndPoints[0].Port;
                        UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
                        if (uDP_Class == null) return;
                        Dialog_IO測試 Dialog_IO測試 = new Dialog_IO測試(uDP_Class, IP, list_UDP_Rx);
                        Dialog_IO測試.ShowDialog();
                    }
                    if (dialog_ContextMenuStrip.Value == ContextMenuStrip_UDP_DataReceive_IO設定.雷射開啟.GetEnumName())
                    {
                        List<Storage> storages_replace = new List<Storage>();
                        List<Task> taskList = new List<Task>();
                        for (int i = 0; i < iPEndPoints.Count; i++)
                        {
                            string IP = iPEndPoints[i].Address.ToString();
                            int Port = iPEndPoints[i].Port;

                   
                            taskList.Add(Task.Run(() =>
                            {
                                Storage storage = this.SQL_GetStorage(IP);
                                if (storage == null) return;
                                storage.TOFON = true;
                                storages_replace.LockAdd(storage);
                                //Set_TOF(IP, Port, true);
                            }));
                        }
                        Task allTask = Task.WhenAll(taskList);

                        this.SQL_ReplaceStorage(storages_replace);
                    }
                    if (dialog_ContextMenuStrip.Value == ContextMenuStrip_UDP_DataReceive_IO設定.雷射關閉.GetEnumName())
                    {
                        List<Storage> storages_replace = new List<Storage>();
                        List<Task> taskList = new List<Task>();
                        for (int i = 0; i < iPEndPoints.Count; i++)
                        {
                            string IP = iPEndPoints[i].Address.ToString();
                            int Port = iPEndPoints[i].Port;


                            taskList.Add(Task.Run(() =>
                            {
                                Storage storage = this.SQL_GetStorage(IP);
                                if (storage == null) return;
                                storage.TOFON = false;
                                storages_replace.LockAdd(storage);
                                //Set_TOF(IP, Port, false);
                            }));
                        }
                        Task allTask = Task.WhenAll(taskList);

                        this.SQL_ReplaceStorage(storages_replace);
                    }

                }
            }
            else if (selectedText == ContextMenuStrip_Main.面板種類.GetEnumName())
            {
                Dialog_ContextMenuStrip dialog_ContextMenuStrip = new Dialog_ContextMenuStrip(new ContextMenuStrip_UDP_DataReceive_面板種類().GetEnumNames());
                if (dialog_ContextMenuStrip.ShowDialog() != DialogResult.Yes) return;
                if (dialog_ContextMenuStrip.Value == ContextMenuStrip_UDP_DataReceive_面板種類.設為EPD213有鎖控.GetEnumName())
                {
                    List<Task> taskList = new List<Task>();
                    for (int i = 0; i < iPEndPoints.Count; i++)
                    {
                        string IP = iPEndPoints[i].Address.ToString();
                        int Port = iPEndPoints[i].Port;
                        Storage storage = this.SQL_GetStorage(IP);
                        if (storage == null) continue;
                        storage.SetDeviceType(DeviceType.EPD213_lock);
                        taskList.Add(Task.Run(() =>
                        {
                            this.SQL_ReplaceStorage(storage);
                        }));
                    }
                    Task allTask = Task.WhenAll(taskList);
                    this.sqL_DataGridView_DeviceTable.SQL_GetAllRows(true);
                }
                else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_UDP_DataReceive_面板種類.設為EPD213無鎖控.GetEnumName())
                {
                    List<Task> taskList = new List<Task>();
                    for (int i = 0; i < iPEndPoints.Count; i++)
                    {
                        string IP = iPEndPoints[i].Address.ToString();
                        int Port = iPEndPoints[i].Port;
                        Storage storage = this.SQL_GetStorage(IP);
                        if (storage == null) continue;
                        storage.SetDeviceType(DeviceType.EPD213);
                        taskList.Add(Task.Run(() =>
                        {
                            this.SQL_ReplaceStorage(storage);
                        }));
                    }
                    Task allTask = Task.WhenAll(taskList);
                    this.sqL_DataGridView_DeviceTable.SQL_GetAllRows(true);
                }
                else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_UDP_DataReceive_面板種類.設為EPD266有鎖控.GetEnumName())
                {
                    List<Task> taskList = new List<Task>();
                    for (int i = 0; i < iPEndPoints.Count; i++)
                    {
                        string IP = iPEndPoints[i].Address.ToString();
                        int Port = iPEndPoints[i].Port;
                        Storage storage = this.SQL_GetStorage(IP);
                        if (storage == null) continue;
                        storage.SetDeviceType(DeviceType.EPD266_lock);
                        taskList.Add(Task.Run(() =>
                        {
                            this.SQL_ReplaceStorage(storage);
                        }));
                    }
                    Task allTask = Task.WhenAll(taskList);
                    this.sqL_DataGridView_DeviceTable.SQL_GetAllRows(true);
                }
                else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_UDP_DataReceive_面板種類.設為EPD266無鎖控.GetEnumName())
                {
                    List<Task> taskList = new List<Task>();
                    for (int i = 0; i < iPEndPoints.Count; i++)
                    {
                        string IP = iPEndPoints[i].Address.ToString();
                        int Port = iPEndPoints[i].Port;
                        Storage storage = this.SQL_GetStorage(IP);
                        if (storage == null) continue;
                        storage.SetDeviceType(DeviceType.EPD266);
                        taskList.Add(Task.Run(() =>
                        {
                            this.SQL_ReplaceStorage(storage);
                        }));
                    }
                    Task allTask = Task.WhenAll(taskList);
                    this.sqL_DataGridView_DeviceTable.SQL_GetAllRows(true);
                }
                else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_UDP_DataReceive_面板種類.設為EPD290有鎖控.GetEnumName())
                {
                    List<Task> taskList = new List<Task>();
                    for (int i = 0; i < iPEndPoints.Count; i++)
                    {
                        string IP = iPEndPoints[i].Address.ToString();
                        int Port = iPEndPoints[i].Port;
                        Storage storage = this.SQL_GetStorage(IP);
                        if (storage == null) continue;
                        storage.SetDeviceType(DeviceType.EPD290_lock);
                        taskList.Add(Task.Run(() =>
                        {
                            this.SQL_ReplaceStorage(storage);
                        }));
                    }
                    Task allTask = Task.WhenAll(taskList);
                    this.sqL_DataGridView_DeviceTable.SQL_GetAllRows(true);
                }
                else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_UDP_DataReceive_面板種類.設為EPD290無鎖控.GetEnumName())
                {
                    List<Task> taskList = new List<Task>();
                    for (int i = 0; i < iPEndPoints.Count; i++)
                    {
                        string IP = iPEndPoints[i].Address.ToString();
                        int Port = iPEndPoints[i].Port;
                        Storage storage = this.SQL_GetStorage(IP);
                        if (storage == null) continue;
                        storage.SetDeviceType(DeviceType.EPD290);
                        taskList.Add(Task.Run(() =>
                        {
                            this.SQL_ReplaceStorage(storage);
                        }));
                    }
                    Task allTask = Task.WhenAll(taskList);
                    this.sqL_DataGridView_DeviceTable.SQL_GetAllRows(true);
                }
                else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_UDP_DataReceive_面板種類.設為EPD420有鎖控.GetEnumName())
                {
                    List<Task> taskList = new List<Task>();
                    for (int i = 0; i < iPEndPoints.Count; i++)
                    {
                        string IP = iPEndPoints[i].Address.ToString();
                        int Port = iPEndPoints[i].Port;
                        Storage storage = this.SQL_GetStorage(IP);
                        if (storage == null) continue;
                        storage.SetDeviceType(DeviceType.EPD420_lock);
                        taskList.Add(Task.Run(() =>
                        {
                            this.SQL_ReplaceStorage(storage);
                        }));
                    }
                    Task allTask = Task.WhenAll(taskList);
                    this.sqL_DataGridView_DeviceTable.SQL_GetAllRows(true);
                }
                else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_UDP_DataReceive_面板種類.設為EPD420無鎖控.GetEnumName())
                {
                    List<Task> taskList = new List<Task>();
                    for (int i = 0; i < iPEndPoints.Count; i++)
                    {
                        string IP = iPEndPoints[i].Address.ToString();
                        int Port = iPEndPoints[i].Port;
                        Storage storage = this.SQL_GetStorage(IP);
                        if (storage == null) continue;
                        storage.SetDeviceType(DeviceType.EPD420);
                        taskList.Add(Task.Run(() =>
                        {
                            this.SQL_ReplaceStorage(storage);
                        }));
                    }
                    Task allTask = Task.WhenAll(taskList);
                    this.sqL_DataGridView_DeviceTable.SQL_GetAllRows(true);
                }
            }         
            else if (selectedText == ContextMenuStrip_Main.IO設定.GetEnumName())
            {

            }
        }
        public string ByteToStringHex(byte[] value)
        {
            return string.Join(",", value.Select(b => $"0x{b:X2}"));
        }
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // StorageUI_EPD_266
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.Name = "StorageUI_EPD_266";
            this.ResumeLayout(false);

        }
    }
}
