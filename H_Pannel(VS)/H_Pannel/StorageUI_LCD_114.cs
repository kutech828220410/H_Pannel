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
using System.Reflection;
namespace H_Pannel_lib
{

    public partial class StorageUI_LCD_114 : StorageUI
    {
        public enum enum_InputName
        {
            X0,
            X1,
            X2,
            X3,
            Y0,
            Y1,
            Y2,
            Y3,
        }
        public List<UDP_READ> UDP_READs = new List<UDP_READ>();
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
            private bool lASER_ON = false;

            public string IP { get => iP; set => iP = value; }
            public int Port { get => port; set => port = value; }
            public string Version { get => version; set => version = value; }
            public int Input { get => input; set => input = value; }
            public int Output { get => output; set => output = value; }
            public int RSSI { get => rSSI; set => rSSI = value; }
            public int Input_dir { get => input_dir; set => input_dir = value; }
            public int Output_dir { get => output_dir; set => output_dir = value; }
            public int LaserDistance { get => laserDistance; set => laserDistance = value; }
            public bool WS2812_State { get; set; }
            public bool LASER_ON { get => lASER_ON; set => lASER_ON = value; }

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
            public bool Get_Input(enum_InputName point_name)
            {
                return Get_Input(point_name.GetEnumName());
            }
            public bool Get_Input(string point_name)
            {
                if (point_name == "X0")
                {
                    return Get_Input(3);
                }
                else if (point_name == "X1")
                {
                    return Get_Input(2);
                }
                else if (point_name == "X2")
                {
                    return Get_Input(1);
                }
                else if (point_name == "X3")
                {
                    return Get_Input(0);
                }
                else if (point_name == "Y0")
                {
                    return Get_Input(4);
                }
                else if (point_name == "Y1")
                {
                    return Get_Input(5);
                }
                else if (point_name == "Y2")
                {
                    return Get_Input(6);
                }
                else if (point_name == "Y3")
                {
                    return Get_Input(7);
                }
                return false;
            }


            public bool IsSensorOn(DrawerUI_EPD_583.LightSensorClass lightSensorClass)
            {
                bool flag_H = false;
                bool flag_V = false;
                for (int i = 0; i < lightSensorClass.H_Sensor_Check.Count; i++)
                {
                    flag_H = Get_Input(lightSensorClass.H_Sensor_Check[i]);
                    if (flag_H) break;
                }
                for (int i = 0; i < lightSensorClass.V_Sensor_Check.Count; i++)
                {
                    flag_V = Get_Input(lightSensorClass.V_Sensor_Check[i]);
                    if (flag_V) break;
                }
                //Console.WriteLine($"IsSensorOn : {(flag_H && flag_V)} , [flag_H : {flag_H}],[flag_V : {flag_V}]");
                return (flag_H && flag_V);
            }
        }

        #region 靜態參數
        public delegate Bitmap Get_Storage_bmpEventHandler(Storage storage);
        static public event Get_Storage_bmpEventHandler Get_Storage_bmpChangeEvent;

        static public int Pannel_Width
        {
            get
            {
                return 235;
            }
        }
        static public int Pannel_Height
        {
            get
            {
                return 140;
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
     
        
        #endregion

        private enum ContextMenuStrip_Main
        {
            畫面設置,
            IO設定,
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
        }

        public StorageUI_LCD_114()
        {
            this.TableName = "EPD266_Jsonstring";
            this.DeviceTableMouseDownRightEvent += StorageUI_LCD_114_DeviceTableMouseDownRightEvent;
            this.UDP_DataReceiveMouseDownRightEvent += StorageUI_LCD_114_UDP_DataReceiveMouseDownRightEvent;

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
        static public int Get_LaserDistance(UDP_Class uDP_Class, string IP)
        {
            if (uDP_Class != null)
            {
                return Communication.EPD_Get_LaserDistance(uDP_Class, IP);
            }
            return -999;
        }


        public UDP_READ Get_UDP_READ(string IP)
        {
            string json = this.GetUDPJsonString(IP);
            if (json.StringIsEmpty()) return null;
            UDP_READ uDP_READ = json.JsonDeserializet<UDP_READ>();
            return uDP_READ;
        }
        public void UDP_READ_Update()
        {
            UDP_READs = GerAllUDP_READ();
        }
        public bool Get_LASER_ON(string IP)
        {
            List<UDP_READ> uDP_READs_buf = (from temp in UDP_READs
                                            where temp.IP == IP
                                            select temp).ToList();
            if (uDP_READs_buf.Count == 0) return false;
            return uDP_READs_buf[0].LASER_ON;
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

        public bool DrawImage(string IP, int port, string text, Font font, Color ForeColoe, Color BackColor)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(port);
            if (uDP_Class == null) return false;
            using (Bitmap bmp = Communication.Get_LCD_144_bmp(text, font, Color.White, Color.Black))
            {
                return Communication.LCD_144_DrawImageEx(uDP_Class, IP, bmp, ForeColoe, BackColor);
            }

        }
        public bool DrawImage(string IP, int port, Color BackColor)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(port);
            if (uDP_Class == null) return false;
            return Communication.LCD_144_DrawImageEx(uDP_Class, IP, BackColor);

        }
        public bool ClearCanvas(string IP, int port)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(port);
            if (uDP_Class == null) return false;
            return Communication.Command_ClearCanvas(uDP_Class, IP, Color.DimGray);

        }
        private void StorageUI_LCD_114_DeviceTableMouseDownRightEvent(string selectedText, List<System.Net.IPEndPoint> iPEndPoints)
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
                 
                            taskList.Add(Task.Run(() =>
                            {
                                ClearCanvas(IP, Port);
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
                            taskList.Add(Task.Run(() =>
                            {
                                DrawImage(IP, Port, "---", new Font("標楷體", 70, FontStyle.Bold), Color.White, Color.Blue);
                            }));

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

                    if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable_IO設定.IO測試.GetEnumName())
                    {
                        //List<object[]> list_value = this.sqL_DataGridView_DeviceTable.Get_All_Select_RowsValues();
                        //if (list_value.Count == 0) return;
                        //string IP = list_value[0][(int)enum_DeviceTable.IP].ObjectToString();
                        //int Port = list_value[0][(int)enum_DeviceTable.Port].ObjectToString().StringToInt32();
                        //UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
                        //if (uDP_Class == null) return;
                        Dialog_DrawerHandSensorCheck dialog_DrawerHandSensorCheck = new Dialog_DrawerHandSensorCheck( IP, list_UDP_Rx);
                        dialog_DrawerHandSensorCheck.ShowDialog();
                    }


                }


            }
        }
        private void StorageUI_LCD_114_UDP_DataReceiveMouseDownRightEvent(string selectedText, List<System.Net.IPEndPoint> iPEndPoints)
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

                            taskList.Add(Task.Run(() =>
                            {
                                ClearCanvas(IP, Port);
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
                            taskList.Add(Task.Run(() =>
                            {
                                DrawImage(IP, Port, "---", new Font("標楷體", 70, FontStyle.Bold), Color.White, Color.Blue);
                            }));

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

                    if (dialog_ContextMenuStrip.Value == ContextMenuStrip_UDP_DataReceive_IO設定.IO測試.GetEnumName())
                    {
                        List<object[]> list_value = this.sqL_DataGridView_UDP_DataReceive.Get_All_Select_RowsValues();
                        if (list_value.Count == 0) return;
                        string IP = list_value[0][(int)enum_UDP_DataReceive.IP].ObjectToString();
                        int Port = list_value[0][(int)enum_UDP_DataReceive.Port].ObjectToString().StringToInt32();
                        UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
                        if (uDP_Class == null) return;
                        Dialog_DrawerHandSensorCheck dialog_DrawerHandSensorCheck = new Dialog_DrawerHandSensorCheck(IP, list_UDP_Rx);
                        dialog_DrawerHandSensorCheck.ShowDialog();
                    }


                }


            }

        }

            
    }

    static public class StorageUI_LCD_114_Method
    {
        static public bool Get_LASER_ON(this List<StorageUI_LCD_114.UDP_READ> uDP_READs, string IP)
        {
            List<StorageUI_LCD_114.UDP_READ> uDP_READs_buf = (from temp in uDP_READs
                                                              where temp.IP == IP
                                                              select temp).ToList();
            if (uDP_READs_buf.Count == 0) return false;
            return uDP_READs_buf[0].LASER_ON;
        }
        static public int Get_LaserDistance(this List<StorageUI_LCD_114.UDP_READ> uDP_READs, string IP)
        {
            List<StorageUI_LCD_114.UDP_READ> uDP_READs_buf = (from temp in uDP_READs
                                                              where temp.IP == IP
                                                              select temp).ToList();
            if (uDP_READs_buf.Count == 0) return -999;
            return uDP_READs_buf[0].LaserDistance;
        }
    }
}

