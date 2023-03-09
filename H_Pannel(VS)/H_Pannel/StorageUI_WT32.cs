using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.IO.Ports;
using System.Diagnostics;
using Basic;
using MyUI;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Net;
namespace H_Pannel_lib
{
    public class StorageUI_WT32 : StorageUI
    {
        #region 靜態參數
        static public int Pannel_Width
        {
            get
            {
                return 480;
            }
        }
        static public int Pannel_Height
        {
            get
            {
                return 320;
            }
        }

        static public bool Set_ClearCanvas(UDP_Class uDP_Class, string IP, Color color)
        {
            if (uDP_Class == null) return false;
            return Communication.Set_ClearCanvas(uDP_Class, IP, 0, 0, Pannel_Width, Pannel_Height, color);

        }
        static public bool Set_Setting_Page(UDP_Class uDP_Class, string IP)
        {
            if (uDP_Class == null) return false;
            return Communication.Set_Setting_Page(uDP_Class, IP);

        }
        static public bool Set_Main_Page(UDP_Class uDP_Class, string IP)
        {
            if (uDP_Class == null) return false;
            return Communication.Set_Main_Page(uDP_Class, IP);

        }
        static public bool Set_LED(UDP_Class uDP_Class, string IP, UDP_READ.LED_Type lED_Type, bool statu)
        {
            if (uDP_Class == null) return false;
            return Communication.Set_PIN(uDP_Class, IP, (int)lED_Type, statu);

        }
        static public bool Set_WS2812_Blink(UDP_Class uDP_Class, string IP, int blinkTime, Color color)
        {
            if (uDP_Class == null) return false;
            return Communication.Set_WS2812_Blink(uDP_Class, IP, blinkTime, color);
        }
        static public bool Set_ScreenPageInit(UDP_Class uDP_Class, string IP, bool enable)
        {
            if (uDP_Class == null) return false;
            return Communication.Set_ScreenPageInit(uDP_Class, IP, enable);

        }
        static public bool Set_ToPage(UDP_Class uDP_Class, string IP, enum_Page enum_Page)
        {
            if (uDP_Class == null) return false;
            return Communication.Set_ToPage(uDP_Class, IP, (int)enum_Page);

        }
        static public bool Set_LockOpen(UDP_Class uDP_Class, string IP)
        {
            return Communication.Set_OutputPINTrigger(uDP_Class, IP, 1, true);
        }
        static public bool GetKeyBoardValue(UDP_Class uDP_Class, string IP, ref int value)
        {
            if (uDP_Class == null) return false;
            return Communication.GetKeyBoardValue(uDP_Class, IP, ref value);
        }
        #endregion
        public enum enum_Page
        {
            設定頁面 = 3,
            主頁面 = 10,
            解鎖頁面 = 11,
            取藥中頁面 = 12,
            數字鍵盤頁面 = 20,
        }

        [Serializable]
        public class UDP_READ
        {

            public enum LED_Type
            {
                GREEN = 25,
                RED = 5,
            }
            public delegate void LEDStateChangeEventHandler(LED_Type lED_Type, bool statu);
            public event LEDStateChangeEventHandler LEDStateChangeEvent;
            public delegate void LockStateChangeEventHandler(bool statu);
            public event LockStateChangeEventHandler LockStateChangeEvent;

            private string iP = "0.0.0.0";
            private int port = 0;
            private string version = "";
            private string command = "";
            private int touch_xPos = 0;
            private int touch_yPos = 0;
            private int touch_touched = 0;
            private int iNPUT_LOCK = 0;
            private int screen_Page = -1;
            private bool screenPage_Init = false;
            private int rSSI = -100;
            private int oUTPUT_LED_RED = 0;
            private int oUTPUT_LED_GREEN = 0;
            private int iNPUT_LOCK_buf = -1;
            private int oUTPUT_LED_RED_buf = -1;
            private int oUTPUT_LED_GREEN_buf = -1;
            private int input = 0;
            private int output = 0;
            private int input_dir = 0;
            private int output_dir = 0;
            private int keyBoard123_value = -1;
            private bool wS2812_State = false;

            public string IP { get => iP; set => iP = value; }
            public int Port { get => port; set => port = value; }
            public string Version { get => version; set => version = value; }
            public string Command { get => command; set => command = value; }
            public int Touch_xPos { get => touch_xPos; set => touch_xPos = value; }
            public int Touch_yPos { get => touch_yPos; set => touch_yPos = value; }
            public int Touch_touched { get => touch_touched; set => touch_touched = value; }
            public int Screen_Page { get => screen_Page; set => screen_Page = value; }
            public bool ScreenPage_Init { get => screenPage_Init; set => screenPage_Init = value; }
            public int RSSI { get => rSSI; set => rSSI = value; }
            public int Input { get => input; set => input = value; }
            public int Output { get => output; set => output = value; }
            public int Input_dir { get => input_dir; set => input_dir = value; }
            public int Output_dir { get => output_dir; set => output_dir = value; }
            public int KeyBoard123_value { get => keyBoard123_value; set => keyBoard123_value = value; }
            public bool WS2812_State { get => wS2812_State; set => wS2812_State = value; }

            public int OUTPUT_LED_RED
            {
                get => oUTPUT_LED_RED;
                set
                {
                    if (oUTPUT_LED_RED_buf != oUTPUT_LED_RED)
                    {
                        oUTPUT_LED_RED_buf = oUTPUT_LED_RED;

                        if (LEDStateChangeEvent != null) LEDStateChangeEvent(LED_Type.RED, (oUTPUT_LED_RED == 1));
                    }
                    oUTPUT_LED_RED = value;
                }
            }
            public int OUTPUT_LED_GREEN
            {
                get => oUTPUT_LED_GREEN;
                set
                {
                    if (oUTPUT_LED_GREEN_buf != oUTPUT_LED_GREEN)
                    {
                        oUTPUT_LED_GREEN_buf = oUTPUT_LED_GREEN;

                        if (LEDStateChangeEvent != null) LEDStateChangeEvent(LED_Type.GREEN, (oUTPUT_LED_GREEN == 1));
                    }
                    oUTPUT_LED_GREEN = value;
                }
            }
            public int INPUT_LOCK
            {
                get => iNPUT_LOCK;
                set
                {
                    if (iNPUT_LOCK_buf != iNPUT_LOCK)
                    {
                        iNPUT_LOCK_buf = iNPUT_LOCK;

                        if (LockStateChangeEvent != null) LockStateChangeEvent((iNPUT_LOCK == 1));
                    }
                    iNPUT_LOCK = value;
                }
            }

  
        }

        private enum ContextMenuStrip_Main
        {
            畫面設置,
            IO設定,
            設為有鎖控,
            設為無鎖控,
        }
        private enum ContextMenuStrip_DeviceTable_畫面設置
        {
            換至設定頁,
            換至主畫面,
            換至數字鍵盤頁面,
            清除畫布,
        }
        private enum ContextMenuStrip_DeviceTable_IO設定
        {
            背板LED設定,
            紅燈ON,
            紅燈OFF,
            綠燈ON,
            綠燈OFF,
            IO測試,
        }
        public StorageUI_WT32()
        {
            this.DeviceTableMouseDownRightEvent += StorageUI_WT32_DeviceTableMouseDownRightEvent;
            this.UDP_DataReceiveMouseDownRightEvent += StorageUI_WT32_UDP_DataReceiveMouseDownRightEvent;
            Enum_ContextMenuStrip_DeviceTable = new ContextMenuStrip_Main();
            Enum_ContextMenuStrip_UDP_DataReceive = new ContextMenuStrip_Main();
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
                storage.DeviceType = DeviceType.Pannel35_lock;
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
        protected override void Import()
        {
            if (this.openFileDialog_LoadExcel.ShowDialog() == DialogResult.OK)
            {
                this.Cursor = Cursors.WaitCursor;
                DataTable dataTable = new DataTable();
                CSVHelper.LoadFile(this.openFileDialog_LoadExcel.FileName, 0, dataTable);
                DataTable datatable_buf = dataTable.ReorderTable(new enum_DeviceTable_匯入());
                if (datatable_buf == null)
                {
                    MyMessageBox.ShowDialog("匯入檔案,資料錯誤!");
                    return;
                }
                List<object[]> list_LoadValue = datatable_buf.DataTableToRowList();
                List<object[]> list_SQL_Value = this.sqL_DataGridView_DeviceTable.SQL_GetAllRows(false);
                List<object[]> list_Add = new List<object[]>();
                List<object[]> list_Delete_ColumnName = new List<object[]>();
                List<object[]> list_Delete_SerchValue = new List<object[]>();
                List<string> list_Replace_SerchValue = new List<string>();
                List<object[]> list_Replace_Value = new List<object[]>();
                List<object[]> list_SQL_Value_buf = new List<object[]>();
                List<object[]> list_LoadValue_buf = new List<object[]>();

                for (int i = 0; i < list_LoadValue.Count; i++)
                {
                    object[] value_load = list_LoadValue[i];
                    value_load = value_load.CopyRow(new enum_DeviceTable_匯入(), new enum_DeviceTable());
                    list_SQL_Value_buf = list_SQL_Value.GetRows((int)enum_DeviceTable.IP, value_load[(int)enum_DeviceTable.IP].ObjectToString());
                    string IP = value_load[(int)enum_DeviceTable.IP].ObjectToString();
                    int Port = value_load[(int)enum_DeviceTable.Port].ObjectToString().StringToInt32();
                    if (list_SQL_Value_buf.Count > 0)
                    {
                        object[] value_SQL = list_SQL_Value_buf[0];
                        value_load[(int)enum_DeviceTable.GUID] = value_SQL[(int)enum_DeviceTable.GUID];
                        value_load[(int)enum_DeviceTable.Value] = value_SQL[(int)enum_DeviceTable.Value];
                        bool flag_Equal = value_load.IsEqual(value_SQL);
                        if (value_load[(int)enum_DeviceTable.Value].ObjectToString().StringIsEmpty())
                        {
                            Storage storage = new Storage(IP, Port);
                            storage.DeviceType = DeviceType.Pannel35;
                            value_load[(int)enum_DeviceTable.Value] = storage.JsonSerializationt<Storage>();
                            flag_Equal = false;
                        }
                        if (!flag_Equal)
                        {
                            list_Replace_SerchValue.Add(value_load[(int)enum_DeviceTable.GUID].ObjectToString());
                            list_Replace_Value.Add(value_load);
                        }
                    }
                    else
                    {
                        value_load[(int)enum_DeviceTable.GUID] = Guid.NewGuid().ToString();
                        Storage storage = new Storage(IP, Port);
                        storage.DeviceType = DeviceType.Pannel35;
                        value_load[(int)enum_DeviceTable.Value] = storage.JsonSerializationt<Storage>();
                        list_Add.Add(value_load);
                    }
                }
                for (int i = 0; i < list_SQL_Value.Count; i++)
                {
                    list_LoadValue_buf = list_LoadValue.GetRows((int)enum_DeviceTable_匯入.IP, list_SQL_Value[i][(int)enum_DeviceTable.IP].ObjectToString());
                    if (list_LoadValue_buf.Count == 0)
                    {
                        list_Delete_ColumnName.Add(new string[] { enum_DeviceTable.GUID.GetEnumName() });
                        list_Delete_SerchValue.Add(new string[] { list_SQL_Value[i][(int)enum_DeviceTable.GUID].ObjectToString() });
                    }
                }
                this.sqL_DataGridView_DeviceTable.SQL_AddRows(list_Add, false);
                this.sqL_DataGridView_DeviceTable.SQL_ReplaceExtra(enum_DeviceTable.GUID.GetEnumName(), list_Replace_SerchValue, list_Replace_Value, false);
                this.sqL_DataGridView_DeviceTable.SQL_DeleteExtra(list_Delete_ColumnName, list_Delete_SerchValue, false);
                this.sqL_DataGridView_DeviceTable.SQL_GetAllRows(true);
                this.Cursor = Cursors.Default;
            }
        }
        protected override void Export()
        {
            if (this.saveFileDialog_SaveExcel.ShowDialog() == DialogResult.OK)
            {
                DataTable dataTable = this.sqL_DataGridView_DeviceTable.GetDataTable().DeepClone();
                dataTable = dataTable.ReorderTable(new enum_DeviceTable_匯出());
                CSVHelper.SaveFile(dataTable, this.saveFileDialog_SaveExcel.FileName);
            }
        }

        protected override void SQL_ReplaceDevice(object[] value, string IP, int Port)
        {
            List<object[]> list_SQL_Value = this.sqL_DataGridView_DeviceTable.SQL_GetAllRows(false);
            List<object[]> list_SQL_Value_buf = new List<object[]>();
            list_SQL_Value_buf = list_SQL_Value.GetRows((int)enum_DeviceTable.IP, IP);
            if (list_SQL_Value_buf.Count == 0)
            {
                string GUID = value[(int)enum_DeviceTable.GUID].ObjectToString();
                value[(int)enum_DeviceTable.IP] = IP;
                value[(int)enum_DeviceTable.Port] = Port;
                string jsonString = value[(int)enum_DeviceTable.Value].ObjectToString();
                if (jsonString.StringIsEmpty()) return;
                Storage storage = jsonString.JsonDeserializet<Storage>();
                if (storage == null) return;
                storage.IP = IP;
                value[(int)enum_DeviceTable.Value] = storage.JsonSerializationt();
                this.sqL_DataGridView_DeviceTable.SQL_Replace(enum_DeviceTable.GUID.GetEnumName(), GUID, value, true);
            }
            else
            {
                string GUID = list_SQL_Value_buf[0][(int)enum_DeviceTable.GUID].ObjectToString();
                this.sqL_DataGridView_DeviceTable.SQL_Delete(enum_DeviceTable.GUID.GetEnumName(), GUID, false);

                GUID = value[(int)enum_DeviceTable.GUID].ObjectToString();
                value[(int)enum_DeviceTable.IP] = IP;
                value[(int)enum_DeviceTable.Port] = Port;
                string jsonString = value[(int)enum_DeviceTable.Value].ObjectToString();
                if (jsonString.StringIsEmpty()) return;
                Storage storage = jsonString.JsonDeserializet<Storage>();
                if (storage == null) return;
                storage.IP = IP;
                storage.Port = Port;
                value[(int)enum_DeviceTable.Value] = storage.JsonSerializationt();
                this.sqL_DataGridView_DeviceTable.SQL_AddRow(value, true);
            }
        }


        public bool Set_ClearCanvas(string IP, int Port, Color color)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
            return Set_ClearCanvas(uDP_Class, IP, color);
        }
        public bool Set_Setting_Page(string IP, int Port)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
            return Set_Setting_Page(uDP_Class, IP);
        }
        public bool Set_Main_Page(string IP, int Port)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
            return Set_Main_Page(uDP_Class, IP);
        }
        public bool Set_LED(string IP, int Port, UDP_READ.LED_Type lED_Type, bool statu)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
            return Set_LED(uDP_Class, IP, lED_Type, statu);
        }
        public bool Set_WS2812_Blink(Storage storage, int blinkTime, Color color)
        {
            if (storage == null) return false;
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(storage.Port);
            return Set_WS2812_Blink(uDP_Class, storage.IP, blinkTime, color);
        }
        public bool Set_WS2812_Blink(string IP, int Port, int blinkTime, Color color)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
            return Set_WS2812_Blink(uDP_Class, IP, blinkTime, color);
        }
        public bool Set_ScreenPageInit(Storage storage, bool enable)
        {
            return this.Set_ScreenPageInit(storage.IP, storage.Port, enable);
        }
        public bool Set_ScreenPageInit(string IP, int Port, bool enable)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
            return Set_ScreenPageInit(uDP_Class, IP, enable);
        }


        public bool Set_ToPage(Storage storage, enum_Page enum_Page, bool enforce)
        {
            return this.Set_ToPage(storage.IP, storage.Port, enum_Page, enforce);
        }
        public bool Set_ToPage(Storage storage, enum_Page enum_Page)
        {
            return this.Set_ToPage(storage.IP, storage.Port, enum_Page, false);
        }
        public bool Set_ToPage(string IP, int Port, enum_Page enum_Page)
        {
            return this.Set_ToPage(IP, Port, enum_Page, false);
        }
        public bool Set_ToPage(string IP, int Port, enum_Page enum_Page ,bool enforce)
        {
            UDP_READ uDP_READ = this.GerUDP_READ(IP);
            if (uDP_READ == null) return false;
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
            if(!enforce)
            {
                if (uDP_READ.Screen_Page == (int)enum_Page)
                {
                    return true;
                }
            }
            
            bool flag_OK = Set_ToPage(uDP_Class, IP, enum_Page);
            return flag_OK;

        }
        public bool Set_TextEx(string IP, int Port, string Text, int x, int y, Font font, Color FontColor, Color FontBackColor)
        {
            Size string_size = TextRenderer.MeasureText(Text, font);
            return this.Set_TextEx(IP, Port, Text, x, y, string_size.Width, string_size.Height, font, FontColor, FontBackColor, 0, Color.Black, HorizontalAlignment.Left);
        }
        public bool Set_TextEx(Storage storage, string Text, int x, int y, Font font, Color FontColor, Color FontBackColor, int BorderSize, Color BorderColor)
        {
            return this.Set_TextEx(storage.IP, storage.Port, Text, x, y, font, FontColor, FontBackColor, BorderSize, BorderColor, HorizontalAlignment.Left);
        }
        public bool Set_TextEx(string IP, int Port, string Text, int x, int y, Font font, Color FontColor, Color FontBackColor, int BorderSize, Color BorderColor)
        {
            Size string_size = TextRenderer.MeasureText(Text, font);
            return this.Set_TextEx(IP, Port, Text, x, y, string_size.Width, string_size.Height, font, FontColor, FontBackColor, BorderSize, BorderColor, HorizontalAlignment.Left);
        }
        public bool Set_TextEx(string IP, int Port, string Text, int x, int y, Font font, Color FontColor, Color FontBackColor, int BorderSize, Color BorderColor, HorizontalAlignment horizontalAlignment)
        {
            Size string_size = TextRenderer.MeasureText(Text, font);
            return this.Set_TextEx(IP, Port, Text, x, y, string_size.Width, string_size.Height, font, FontColor, FontBackColor, BorderSize, BorderColor, horizontalAlignment);
        }
        public bool Set_TextEx(string IP, int Port, string Text, int x, int y, int width, int height, Font font, Color FontColor, Color FontBackColor, int BorderSize, Color BorderColor, HorizontalAlignment horizontalAlignment)
        {
            bool flag = false;
            for (int i = 0; i < List_UDP_Local.Count; i++)
            {
                if (List_UDP_Local[i].Port == Port)
                {
                    flag = Communication.Set_TextEx(List_UDP_Local[i], IP, Text, x, y, width, height, font, FontColor, FontBackColor, horizontalAlignment);
                    if (!flag) return false;
                    if (BorderSize > 0)
                    {
                        flag = this.Set_DrawRect(IP, Port, x, y, width, height, BorderSize, BorderColor);
                    }
                }

            }
            return flag;
        }
        public bool Set_TextEx(Storage storage, Storage.ValueName valueName, int x, int y, Font font, Color FontColor, Color FontBackColor, int BorderSize, Color BorderColor)
        {
            Storage.VlaueClass vlaueClass = storage.GetValue(valueName);
            string IP = storage.IP;
            int Port = storage.Port;
            string text = vlaueClass.StringValue;
            Size text_size = DrawingClass.Draw.MeasureText(text, font);
            int width = text_size.Width;
            int height = text_size.Height;
            return this.Set_TextEx(IP, Port, text, x, y, width, height, font, FontColor, FontBackColor, BorderSize, BorderColor, HorizontalAlignment.Center);
        }

        public bool Set_BarCodeEx(string IP, int Port, string content, int x, int y, int width, int height)
        {

            bool flag = false;
            for (int i = 0; i < List_UDP_Local.Count; i++)
            {
                if (List_UDP_Local[i].Port == Port)
                {
                    flag = Communication.Set_BarCodeEx(List_UDP_Local[i], IP, content, x, y, width, height);
                }

            }
            return flag;
        }  
        public bool Set_DrawRect(string IP, int Port, int x, int y, int width, int height, int pen_width, Color color)
        {
            bool flag = false;
            for (int i = 0; i < List_UDP_Local.Count; i++)
            {
                if (List_UDP_Local[i].Port == Port)
                {
                    flag = Communication.Set_DrawRect(List_UDP_Local[i], IP, x, y, width, height, pen_width, color);
                }

            }
            return flag;
        }

        public bool Set_DrawPannel(Storage storage, Storage.ValueName valueName)
        {
            Storage.VlaueClass vlaueClass = storage.GetValue(valueName);
            return this.Set_DrawPannel(storage.IP, storage.Port, vlaueClass);
        }
        public bool Set_DrawPannel(string IP, int Port, Storage.VlaueClass vlaueClass)
        {
            if (vlaueClass.Visable)
            {
                if (vlaueClass.valueName != Storage.ValueName.BarCode)
                {
                    this.Set_TextEx(IP, Port, vlaueClass.StringValue, vlaueClass.Position.X, vlaueClass.Position.Y, vlaueClass.Width, vlaueClass.Height, vlaueClass.Font, vlaueClass.ForeColor, vlaueClass.BackColor, vlaueClass.BorderSize, vlaueClass.BorderColor, vlaueClass.HorizontalAlignment);
                }
                else
                {
                    this.Set_BarCodeEx(IP, Port, vlaueClass.Value, vlaueClass.Position.X, vlaueClass.Position.Y, vlaueClass.Width, vlaueClass.Height);
                }
            }
            return true;
        }
        public bool Set_DrawPannel(Storage storage)
        {
            Storage.VlaueClass vlaueClass;
            this.Set_ClearCanvas(storage.IP, storage.Port, storage.BackColor);
            for (int i = 0; i < new Storage.ValueName().GetEnumNames().Length; i++)
            {
                vlaueClass = storage.GetValue((Storage.ValueName)i);
                if (vlaueClass.Visable)
                {
                    if (vlaueClass.valueName != Storage.ValueName.BarCode)
                    {
                        this.Set_TextEx(storage.IP, storage.Port, vlaueClass.StringValue, vlaueClass.Position.X, vlaueClass.Position.Y, vlaueClass.Width, vlaueClass.Height, vlaueClass.Font, vlaueClass.ForeColor, vlaueClass.BackColor, vlaueClass.BorderSize, vlaueClass.BorderColor, vlaueClass.HorizontalAlignment);
                    }
                    else
                    {
                        this.Set_BarCodeEx(storage.IP, storage.Port, vlaueClass.Value, vlaueClass.Position.X, vlaueClass.Position.Y, vlaueClass.Width, vlaueClass.Height);
                    }
                }

            }
            this.Set_ScreenPageInit(storage.IP, storage.Port, false);
            return true;
        }
        public bool Set_DrawPannelJEPG(Storage storage)
        {
            List<byte> list_bytes = new List<byte>();
            byte[] bytes = WT32_GPADC.Get_Storage_JepgBytes(storage);
            Set_Jpegbuffer(storage.IP, storage.Port, bytes);
            this.Set_ScreenPageInit(storage.IP, storage.Port, false);
            return true;
        }
        public bool Set_Framebuffer(string IP, int Port, long start_ptr, byte[] value)
        {
            UDP_READ uDP_READ = this.GerUDP_READ(IP);
            if (uDP_READ == null) return false;
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
            return Communication.Set_Framebuffer(uDP_Class, IP, start_ptr , value);

        }
        public bool Set_Jpegbuffer(string IP, int Port, byte[] value)
        {
            UDP_READ uDP_READ = this.GerUDP_READ(IP);
            if (uDP_READ == null) return false;
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
            return Communication.Set_Jpegbuffer(uDP_Class, IP, value);

        }

        public bool GetKeyBoardValue(Storage storage, ref int value)
        {
            return this.GetKeyBoardValue(storage.IP, storage.Port, ref value);
        }
        public bool GetKeyBoardValue(string IP, int Port, ref int value)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
            return GetKeyBoardValue(uDP_Class, IP , ref value);
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
            return ((uDP_READ.Input % 2) == 1);
        }
        public bool Set_Stroage_LED_UDP(Storage storage, Color color)
        {
            if(color == Color.Black) return this.Set_WS2812_Blink(storage, 0, color);
            return this.Set_WS2812_Blink(storage, 1, color);
        }


        public UDP_READ GerUDP_READ(string IP)
        {
            string jsonString = this.GetUDPJsonString(IP);
            if (jsonString.StringIsEmpty()) return null;
            UDP_READ uDP_READ = jsonString.JsonDeserializet<UDP_READ>();
            return uDP_READ;
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

        #region Event
        private void StorageUI_WT32_UDP_DataReceiveMouseDownRightEvent(string selectedText, List<IPEndPoint> iPEndPoints)
        {
            if (selectedText == ContextMenuStrip_Main.畫面設置.GetEnumName())
            {
                Dialog_ContextMenuStrip dialog_ContextMenuStrip = new Dialog_ContextMenuStrip(new ContextMenuStrip_DeviceTable_畫面設置().GetEnumNames());
                if (dialog_ContextMenuStrip.ShowDialog() == System.Windows.Forms.DialogResult.Yes)
                {
                    if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable_畫面設置.換至主畫面.GetEnumName())
                    {
                        List<Task> taskList = new List<Task>();
                        for (int i = 0; i < iPEndPoints.Count; i++)
                        {
                            string IP = iPEndPoints[i].Address.ToString();
                            int Port = iPEndPoints[i].Port;

                            taskList.Add(Task.Run(() =>
                            {
                                Set_Main_Page(IP, Port);
                            }));
                        }
                        Task allTask = Task.WhenAll(taskList);
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable_畫面設置.換至設定頁.GetEnumName())
                    {
                        List<Task> taskList = new List<Task>();
                        for (int i = 0; i < iPEndPoints.Count; i++)
                        {
                            string IP = iPEndPoints[i].Address.ToString();
                            int Port = iPEndPoints[i].Port;

                            taskList.Add(Task.Run(() =>
                            {
                                Set_Setting_Page(IP, Port);
                            }));
                        }
                        Task allTask = Task.WhenAll(taskList);
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable_畫面設置.換至數字鍵盤頁面.GetEnumName())
                    {
                        List<Task> taskList = new List<Task>();
                        for (int i = 0; i < iPEndPoints.Count; i++)
                        {
                            string IP = iPEndPoints[i].Address.ToString();
                            int Port = iPEndPoints[i].Port;

                            taskList.Add(Task.Run(() =>
                            {
                                Set_ToPage(IP, Port, enum_Page.數字鍵盤頁面);
                            }));
                        }
                        Task allTask = Task.WhenAll(taskList);
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable_畫面設置.清除畫布.GetEnumName())
                    {
                        ColorDialog colorDialog = new ColorDialog();
                        if (colorDialog.ShowDialog() == DialogResult.OK)
                        {
                            List<Task> taskList = new List<Task>();
                            for (int i = 0; i < iPEndPoints.Count; i++)
                            {
                                string IP = iPEndPoints[i].Address.ToString();
                                int Port = iPEndPoints[i].Port;

                                taskList.Add(Task.Run(() =>
                                {
                                    Set_ClearCanvas(IP, Port, colorDialog.Color);
                                }));
                            }
                            Task allTask = Task.WhenAll(taskList);
                        }
                    }

                }
            }
            else if (selectedText == ContextMenuStrip_Main.IO設定.GetEnumName())
            {
                Dialog_ContextMenuStrip dialog_ContextMenuStrip = new Dialog_ContextMenuStrip(new ContextMenuStrip_DeviceTable_IO設定().GetEnumNames());
                if (dialog_ContextMenuStrip.ShowDialog() == DialogResult.Yes)
                {
                    if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable_IO設定.紅燈ON.GetEnumName())
                    {
                        List<Task> taskList = new List<Task>();
                        for (int i = 0; i < iPEndPoints.Count; i++)
                        {
                            string IP = iPEndPoints[i].Address.ToString();
                            int Port = iPEndPoints[i].Port;

                            taskList.Add(Task.Run(() =>
                            {
                                Set_LED(IP, Port, UDP_READ.LED_Type.RED, true);
                            }));
                        }
                        Task allTask = Task.WhenAll(taskList);
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable_IO設定.紅燈OFF.GetEnumName())
                    {
                        List<Task> taskList = new List<Task>();
                        for (int i = 0; i < iPEndPoints.Count; i++)
                        {
                            string IP = iPEndPoints[i].Address.ToString();
                            int Port = iPEndPoints[i].Port;

                            taskList.Add(Task.Run(() =>
                            {
                                Set_LED(IP, Port, UDP_READ.LED_Type.RED, false);
                            }));
                        }
                        Task allTask = Task.WhenAll(taskList);
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable_IO設定.綠燈ON.GetEnumName())
                    {
                        List<Task> taskList = new List<Task>();
                        for (int i = 0; i < iPEndPoints.Count; i++)
                        {
                            string IP = iPEndPoints[i].Address.ToString();
                            int Port = iPEndPoints[i].Port;

                            taskList.Add(Task.Run(() =>
                            {
                                Set_LED(IP, Port, UDP_READ.LED_Type.GREEN, true);
                            }));
                        }
                        Task allTask = Task.WhenAll(taskList);
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable_IO設定.綠燈OFF.GetEnumName())
                    {
                        List<Task> taskList = new List<Task>();
                        for (int i = 0; i < iPEndPoints.Count; i++)
                        {
                            string IP = iPEndPoints[i].Address.ToString();
                            int Port = iPEndPoints[i].Port;

                            taskList.Add(Task.Run(() =>
                            {
                                Set_LED(IP, Port, UDP_READ.LED_Type.GREEN, false);
                            }));
                        }
                        Task allTask = Task.WhenAll(taskList);
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable_IO設定.背板LED設定.GetEnumName())
                    {
                        List<Task> taskList = new List<Task>();
                        ColorDialog colorDialog = new ColorDialog();
                        if (colorDialog.ShowDialog() != DialogResult.OK) return;
                        Dialog_NumPannel dialog_NumPannel = new Dialog_NumPannel();
                        if (dialog_NumPannel.ShowDialog() != DialogResult.Yes) return;
                        for (int i = 0; i < iPEndPoints.Count; i++)
                        {
                            string IP = iPEndPoints[i].Address.ToString();
                            int Port = iPEndPoints[i].Port;

                            taskList.Add(Task.Run(() =>
                            {
                                Set_WS2812_Blink(IP, Port, dialog_NumPannel.Value, colorDialog.Color);
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
                    storage.SetDeviceType(DeviceType.Pannel35_lock);
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
                    storage.SetDeviceType(DeviceType.Pannel35);
                    taskList.Add(Task.Run(() =>
                    {
                        this.SQL_ReplaceStorage(storage);
                    }));
                }
                Task allTask = Task.WhenAll(taskList);
                this.sqL_DataGridView_DeviceTable.SQL_GetAllRows(true);
            }
        }
        private void StorageUI_WT32_DeviceTableMouseDownRightEvent(string selectedText, List<System.Net.IPEndPoint> iPEndPoints)
        {
            if (selectedText == ContextMenuStrip_Main.畫面設置.GetEnumName())
            {
                Dialog_ContextMenuStrip dialog_ContextMenuStrip = new Dialog_ContextMenuStrip(new ContextMenuStrip_DeviceTable_畫面設置().GetEnumNames());
                if (dialog_ContextMenuStrip.ShowDialog() == System.Windows.Forms.DialogResult.Yes)
                {
                    if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable_畫面設置.換至主畫面.GetEnumName())
                    {
                        List<Task> taskList = new List<Task>();
                        for (int i = 0; i < iPEndPoints.Count; i++)
                        {
                            string IP = iPEndPoints[i].Address.ToString();
                            int Port = iPEndPoints[i].Port;

                            taskList.Add(Task.Run(() =>
                            {
                                Set_Main_Page(IP, Port);
                            }));
                        }
                        Task allTask = Task.WhenAll(taskList);
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable_畫面設置.換至設定頁.GetEnumName())
                    {
                        List<Task> taskList = new List<Task>();
                        for (int i = 0; i < iPEndPoints.Count; i++)
                        {
                            string IP = iPEndPoints[i].Address.ToString();
                            int Port = iPEndPoints[i].Port;

                            taskList.Add(Task.Run(() =>
                            {
                                Set_Setting_Page(IP, Port);
                            }));
                        }
                        Task allTask = Task.WhenAll(taskList);
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable_畫面設置.換至數字鍵盤頁面.GetEnumName())
                    {
                        List<Task> taskList = new List<Task>();
                        for (int i = 0; i < iPEndPoints.Count; i++)
                        {
                            string IP = iPEndPoints[i].Address.ToString();
                            int Port = iPEndPoints[i].Port;

                            taskList.Add(Task.Run(() =>
                            {
                                Set_ToPage(IP, Port, enum_Page.數字鍵盤頁面);
                            }));
                        }
                        Task allTask = Task.WhenAll(taskList);
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable_畫面設置.清除畫布.GetEnumName())
                    {
                        ColorDialog colorDialog = new ColorDialog();
                        if (colorDialog.ShowDialog() == DialogResult.OK)
                        {
                            List<Task> taskList = new List<Task>();
                            for (int i = 0; i < iPEndPoints.Count; i++)
                            {
                                string IP = iPEndPoints[i].Address.ToString();
                                int Port = iPEndPoints[i].Port;

                                taskList.Add(Task.Run(() =>
                                {
                                    Set_ClearCanvas(IP, Port , colorDialog.Color);
                                }));
                            }
                            Task allTask = Task.WhenAll(taskList);
                        }                    
                    }
                   
                }
            }
            else if (selectedText == ContextMenuStrip_Main.IO設定.GetEnumName())
            {
                Dialog_ContextMenuStrip dialog_ContextMenuStrip = new Dialog_ContextMenuStrip(new ContextMenuStrip_DeviceTable_IO設定().GetEnumNames());
                if (dialog_ContextMenuStrip.ShowDialog() == DialogResult.Yes)
                {
                    if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable_IO設定.紅燈ON.GetEnumName())
                    {
                        List<Task> taskList = new List<Task>();
                        for (int i = 0; i < iPEndPoints.Count; i++)
                        {
                            string IP = iPEndPoints[i].Address.ToString();
                            int Port = iPEndPoints[i].Port;

                            taskList.Add(Task.Run(() =>
                            {
                                Set_LED(IP, Port, UDP_READ.LED_Type.RED, true);
                            }));
                        }
                        Task allTask = Task.WhenAll(taskList);
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable_IO設定.紅燈OFF.GetEnumName())
                    {
                        List<Task> taskList = new List<Task>();
                        for (int i = 0; i < iPEndPoints.Count; i++)
                        {
                            string IP = iPEndPoints[i].Address.ToString();
                            int Port = iPEndPoints[i].Port;

                            taskList.Add(Task.Run(() =>
                            {
                                Set_LED(IP, Port, UDP_READ.LED_Type.RED, false);
                            }));
                        }
                        Task allTask = Task.WhenAll(taskList);
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable_IO設定.綠燈ON.GetEnumName())
                    {
                        List<Task> taskList = new List<Task>();
                        for (int i = 0; i < iPEndPoints.Count; i++)
                        {
                            string IP = iPEndPoints[i].Address.ToString();
                            int Port = iPEndPoints[i].Port;

                            taskList.Add(Task.Run(() =>
                            {
                                Set_LED(IP, Port, UDP_READ.LED_Type.GREEN, true);
                            }));
                        }
                        Task allTask = Task.WhenAll(taskList);
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable_IO設定.綠燈OFF.GetEnumName())
                    {
                        List<Task> taskList = new List<Task>();
                        for (int i = 0; i < iPEndPoints.Count; i++)
                        {
                            string IP = iPEndPoints[i].Address.ToString();
                            int Port = iPEndPoints[i].Port;

                            taskList.Add(Task.Run(() =>
                            {
                                Set_LED(IP, Port, UDP_READ.LED_Type.GREEN, false);
                            }));
                        }
                        Task allTask = Task.WhenAll(taskList);
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable_IO設定.背板LED設定.GetEnumName())
                    {
                        List<Task> taskList = new List<Task>();
                        ColorDialog colorDialog = new ColorDialog();
                        if (colorDialog.ShowDialog() != DialogResult.OK) return;
                        Dialog_NumPannel dialog_NumPannel = new Dialog_NumPannel();
                        if (dialog_NumPannel.ShowDialog() != DialogResult.Yes) return;
                        for (int i = 0; i < iPEndPoints.Count; i++)
                        {
                            string IP = iPEndPoints[i].Address.ToString();
                            int Port = iPEndPoints[i].Port;

                            taskList.Add(Task.Run(() =>
                            {
                                Set_WS2812_Blink(IP, Port, dialog_NumPannel.Value, colorDialog.Color);
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
                    storage.SetDeviceType(DeviceType.Pannel35_lock);
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
                    storage.SetDeviceType(DeviceType.Pannel35);
                    taskList.Add(Task.Run(() =>
                    {
                        this.SQL_ReplaceStorage(storage);
                    }));
                }
                Task allTask = Task.WhenAll(taskList);
                this.sqL_DataGridView_DeviceTable.SQL_GetAllRows(true);
            }
        }
        #endregion

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // sqL_DataGridView_DeviceTable
            // 
            this.sqL_DataGridView_DeviceTable.columnHeadersHeight = 19;
            // 
            // sqL_DataGridView_UDP_DataReceive
            // 
            this.sqL_DataGridView_UDP_DataReceive.columnHeadersHeight = 18;
            // 
            // StorageUI_WT32
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.Name = "StorageUI_WT32";
            this.TableName = "WT32_Jsonstring";
            this.ResumeLayout(false);

        }
    }
}
