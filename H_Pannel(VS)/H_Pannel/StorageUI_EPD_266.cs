using System;
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

namespace H_Pannel_lib
{
    public partial class StorageUI_EPD_266 : StorageUI
    {
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
            private int laserDistance = 0;

            public string IP { get => iP; set => iP = value; }
            public int Port { get => port; set => port = value; }
            public string Version { get => version; set => version = value; }
            public int Input { get => input; set => input = value; }
            public int Output { get => output; set => output = value; }
            public int RSSI { get => rSSI; set => rSSI = value; }
            public int Input_dir { get => input_dir; set => input_dir = value; }
            public int Output_dir { get => output_dir; set => output_dir = value; }
            public int LaserDistance { get => laserDistance; set => laserDistance = value; }

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
        static public int NumOfLED = 100;
        static public byte[] Get_Empty_LEDBytes()
        {
            return new byte[NumOfLED * 3];
        }
        static public byte[] Get_Pannel_LEDBytes(Color color)
        {
            byte[] LED_Bytes = Get_Empty_LEDBytes();

            for (int i = 0; i < NumOfLED; i++)
            {
                LED_Bytes[i * 3 + 0] = color.R;
                LED_Bytes[i * 3 + 1] = color.G;
                LED_Bytes[i * 3 + 2] = color.B;
            }

            return LED_Bytes;
        }
        static public bool Set_Stroage_LED_UDP(UDP_Class uDP_Class, string IP, Color color)
        {
            byte[] LED_Bytes = Get_Pannel_LEDBytes(color);
            return Set_Stroage_LED_UDP(uDP_Class, IP, LED_Bytes);
        }
        static public bool Set_Stroage_LED_UDP(UDP_Class uDP_Class, string IP, byte[] LED_Bytes)
        {
            if (uDP_Class != null)
            {
                return Communication.Set_WS2812_Buffer(uDP_Class, IP, 0, LED_Bytes);
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
                return Communication.EPD_266_DrawImage(uDP_Class, IP, bitmap);
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

        static public Bitmap Get_Storage_bmp(Storage storage)
        {
            //storage.Name = "Teicoplanin";
            //storage.ChineseName = "康可明靜脈注射劑";
            //storage.Scientific_Name = "Targocid";
            //storage.Code = "862755";
            //storage.Package = "200mg/vial";
            //storage.BarCode = "862755";

            storage.SetValue(Storage.ValueName.藥品名稱, Storage.ValueType.BackColor, Color.Red);
            storage.SetValue(Storage.ValueName.藥品碼, Storage.ValueType.ForeColor, Color.Red);
            Bitmap bitmap = new Bitmap(Pannel_Width, Pannel_Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.HighQuality; //使繪圖質量最高，即消除鋸齒
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

                Rectangle rect = new Rectangle(0, 0, Pannel_Width, Pannel_Height);
                int Line_Height = (Pannel_Height / 3) * 2;
                g.FillRectangle(new SolidBrush(storage.BackColor), rect);

                //this.Graphics_Draw_Bitmap.DrawLine(new Pen(storage.ForeColor, 2), new Point(0, Line_Height), new Point(Pannel_Width, Line_Height));

                storage.SetValue(Device.ValueName.藥品名稱, Device.ValueType.ForeColor, storage.ForeColor);
                storage.SetValue(Device.ValueName.藥品學名, Device.ValueType.ForeColor, storage.ForeColor);
                storage.SetValue(Device.ValueName.藥品中文名稱, Device.ValueType.ForeColor, storage.ForeColor);
                storage.SetValue(Device.ValueName.藥品碼, Device.ValueType.ForeColor, storage.ForeColor);
                storage.SetValue(Device.ValueName.包裝單位, Device.ValueType.ForeColor, storage.ForeColor);

                storage.SetValue(Device.ValueName.藥品名稱, Device.ValueType.BackColor, storage.BackColor);
                storage.SetValue(Device.ValueName.藥品學名, Device.ValueType.BackColor, storage.BackColor);
                storage.SetValue(Device.ValueName.藥品中文名稱, Device.ValueType.BackColor, storage.BackColor);
                storage.SetValue(Device.ValueName.藥品碼, Device.ValueType.BackColor, storage.BackColor);
                storage.SetValue(Device.ValueName.包裝單位, Device.ValueType.BackColor, storage.BackColor);
                float posy = 0;
                SizeF size_name = TextRenderer.MeasureText(storage.Name, storage.Name_font);
                g.DrawString(storage.Name, storage.Name_font, new SolidBrush((Color)storage.GetValue(Storage.ValueName.藥品名稱, Storage.ValueType.ForeColor)), 0, 0 + posy);
                posy += size_name.Height;

                //SizeF size_Scientific_Name_font = TextRenderer.MeasureText(storage.Scientific_Name, storage.Scientific_Name_font);
                //g.DrawString(storage.Scientific_Name, storage.Scientific_Name_font, new SolidBrush((Color)storage.GetValue(Storage.ValueName.藥品學名, Storage.ValueType.ForeColor)), 0, 0 + posy);
                //posy += size_Scientific_Name_font.Height;

                SizeF size_ChineseName = TextRenderer.MeasureText(storage.ChineseName, storage.ChineseName_font);
                g.DrawString(storage.ChineseName, storage.ChineseName_font, new SolidBrush((Color)storage.GetValue(Storage.ValueName.藥品中文名稱, Storage.ValueType.ForeColor)), 0, 0 + posy);
                posy += size_ChineseName.Height;

                for (int i = 0; i < storage.List_Validity_period.Count; i++)
                {
                    if (storage.List_Inventory[i] == "00") continue;
                    string str = $"{i+1}.效期 : {storage.List_Validity_period[i]}   庫存 : {storage.List_Inventory[i]}";
                    storage.Validity_period_font = new Font(storage.Validity_period_font, FontStyle.Bold);
                    SizeF size_Validity_period = TextRenderer.MeasureText(str, storage.Validity_period_font);   
                    g.DrawString(str, storage.Validity_period_font, new SolidBrush((Color)storage.GetValue(Storage.ValueName.效期, Storage.ValueType.ForeColor)), 0, 0 + posy);
                    g.DrawRectangle(new Pen(new SolidBrush(Color.Red), 1), 0, 0 + posy, size_Validity_period.Width, size_Validity_period.Height);
                    posy += size_Validity_period.Height;

                }

                SizeF size_Code_font = TextRenderer.MeasureText(storage.Code, storage.Code_font);
                g.DrawString(storage.Code, storage.Code_font, new SolidBrush((Color)storage.GetValue(Storage.ValueName.藥品碼, Storage.ValueType.ForeColor)), 0, Pannel_Height - size_Code_font.Height);

                SizeF size_Package_font = TextRenderer.MeasureText(storage.Package, storage.Package_font);
                DrawStorageString(g, storage, Device.ValueName.包裝單位, 0, Pannel_Height - size_Code_font.Height - size_Package_font.Height);

                //g.DrawString(storage.Package, storage.Package_font, new SolidBrush((Color)storage.GetValue(Storage.ValueName.包裝單位, Storage.ValueType.ForeColor)), 0, Pannel_Height - size_Code_font.Height - size_Package_font.Height);
                //g.DrawRectangle(new Pen(new SolidBrush((Color)storage.GetValue(Storage.ValueName.包裝單位, Storage.ValueType.ForeColor)), 1), 0, Pannel_Height - size_Code_font.Height - size_Package_font.Height, size_Package_font.Width, size_Package_font.Height);


                Bitmap bitmap_barcode = Communication.CreateBarCode(storage.BarCode, 150, 45);
                g.DrawImage(bitmap_barcode, (Pannel_Width - 150) / 2, Pannel_Height - 45);
                bitmap_barcode.Dispose();

                string[] ip_array = storage.IP.Split('.');
                if (ip_array.Length == 4)
                {
                    string ip = ip_array[2] + "." + ip_array[3];
                    SizeF size_IP = TextRenderer.MeasureText(ip, new Font("微軟正黑體", 10, FontStyle.Bold));
                    g.DrawString(ip, new Font("微軟正黑體", 8, FontStyle.Bold), new SolidBrush((Color)storage.GetValue(Storage.ValueName.IP, Storage.ValueType.ForeColor)), (Pannel_Width - size_IP.Width), (Pannel_Height - size_IP.Height));
                }
            }
            return bitmap;
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
            設為有鎖控,
            設為無鎖控,
        }
        private enum ContextMenuStrip_DeviceTable_畫面設置
        {
            清除畫布,
            測試資訊,
        }
        private enum ContextMenuStrip_DeviceTable_IO設定
        {
            面板亮燈,
            IO測試,
        }
        private enum ContextMenuStrip_UDP_DataReceive_畫面設置
        {
            清除畫布,
            測試資訊,
        }
        private enum ContextMenuStrip_UDP_DataReceive_IO設定
        {
            面板亮燈,
            IO測試,
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
                    if (storage.DeviceType == DeviceType.EPD266_lock)
                    {
                        this.sqL_DataGridView_DeviceTable.dataGridView.Rows[i].DefaultCellStyle.BackColor = Color.Yellow;
                        this.sqL_DataGridView_DeviceTable.dataGridView.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
                    }
                }
            }
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
            return this.Set_Stroage_LED_UDP(IP, Port , LED_Bytes);
        }
        public bool Set_Stroage_LED_UDP(string IP, int Port, byte[] LED_Bytes)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
            byte[] temp = Get_Stroage_LED_UDP(IP, Port);
            return Set_Stroage_LED_UDP(uDP_Class,IP, LED_Bytes);
        }
        public byte[] Get_Stroage_LED_UDP(string IP, int Port)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
            return Communication.Get_WS2812_Buffer(uDP_Class, IP , NumOfLED * 3);
        }

        public bool DrawToEpd_UDP(string IP)
        {
            Storage storage = this.SQL_GetStorage(IP);
            using (Bitmap bitmap = Get_Storage_bmp(storage))
            {
                return DrawToEpd_UDP(IP, storage.Port, bitmap);
            }
        }
        public bool DrawToEpd_UDP(Storage storage)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(storage.Port);
            return DrawToEpd_UDP(uDP_Class, storage);
        }
        public bool DrawToEpd_UDP(string IP, int Port, Bitmap bitmap)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
            return DrawToEpd_UDP(uDP_Class, IP, bitmap);
        }

        public bool Set_LockOpen(Storage storage)
        {
            return Set_LockOpen(storage.IP, storage.Port);
        }
        public bool Set_LockOpen(string IP, int Port)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
            return Set_LockOpen(uDP_Class, IP);
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
                            Bitmap bmp = new Bitmap(Pannel_Width, Pannel_Height);
                            Graphics g = Graphics.FromImage(bmp);
                            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                            g.SmoothingMode = SmoothingMode.HighQuality; //使繪圖質量最高，即消除鋸齒
                            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            g.CompositingQuality = CompositingQuality.HighQuality;
                            g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
                            g.FillRectangle(new SolidBrush(Color.Red), rect);
                            g.Dispose();
                            string IP = iPEndPoints[i].Address.ToString();
                            int Port = iPEndPoints[i].Port;
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
                            Storage storage = new Storage(IP, Port);
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

                }

            }
            else if (selectedText == ContextMenuStrip_Main.設為有鎖控.GetEnumName())
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
            else if (selectedText == ContextMenuStrip_Main.設為無鎖控.GetEnumName())
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
                            Bitmap bmp = new Bitmap(Pannel_Width, Pannel_Height);
                            Graphics g = Graphics.FromImage(bmp);
                            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                            g.SmoothingMode = SmoothingMode.HighQuality; //使繪圖質量最高，即消除鋸齒
                            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            g.CompositingQuality = CompositingQuality.HighQuality;
                            g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
                            g.FillRectangle(new SolidBrush(Color.Red), rect);
                            g.Dispose();
                            string IP = iPEndPoints[i].Address.ToString();
                            int Port = iPEndPoints[i].Port;
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


                }
            }
            else if (selectedText == ContextMenuStrip_Main.設為有鎖控.GetEnumName())
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
            else if (selectedText == ContextMenuStrip_Main.設為無鎖控.GetEnumName())
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
        }
    }
}
