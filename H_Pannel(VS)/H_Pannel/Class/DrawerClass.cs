using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using Basic;
namespace H_Pannel_lib
{
    [Serializable]
    public static class DrawerMethod
    {
        static public void SQL_ReplaceByIP(SQLUI.SQLControl sQLControl, Drawer drawer)
        {
            string IP = drawer.IP;
            List<object[]> list_value = sQLControl.GetRowsByDefult(null, enum_DeviceTable.IP.GetEnumName(), IP);
            if (list_value.Count == 0) return;
            list_value[0][(int)enum_DeviceTable.Value] = drawer.JsonSerializationt<Drawer>();
            sQLControl.UpdateByDefulteExtra(null, list_value[0]);
        }

        static public Drawer SQL_GetDrawerByIP(SQLUI.SQLControl sQLControl,string IP)
        {
            List<object[]> deviceBasicTables = sQLControl.GetRowsByDefult(null, (int)enum_DeviceTable.IP, IP);
            List<Drawer> drawers = SQL_GetAllDrawers(deviceBasicTables);
            if (drawers.Count == 0) return null;
            return drawers[0];
        }

        static public List<Drawer> SQL_GetAllDrawers(SQLUI.SQLControl sQLControl)
        {
            List<object[]> deviceBasicTables = sQLControl.GetAllRows(null);
            return SQL_GetAllDrawers(deviceBasicTables);
        }
        static public List<Drawer> SQL_GetAllDrawers(List<object[]> deviceTables)
        {
            List<Drawer> drawers = new List<Drawer>();
            List<object[]> list_value = deviceTables;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < list_value.Count; i++)
            {
                string jsonString = list_value[i][(int)enum_DeviceTable.Value].ObjectToString();
                if (i == 0) sb.Append("[");

                sb.Append($"{jsonString}");

                if (i != list_value.Count - 1)
                {
                    sb.Append($",");
                }

                if (i == list_value.Count - 1) sb.Append("]");
            }

            string json_result = sb.ToString();
            if (json_result.StringIsEmpty()) json_result = "[]";
            drawers = json_result.JsonDeserializet<List<Drawer>>();

            return drawers;
        }

        static public List<Drawer> SQL_GetDrawersByCode(SQLUI.SQLControl sQLControl, string Code)
        {

            List<object[]> deviceBasicTables = sQLControl.GetAllRows(null);
            return SQL_GetDrawersByCode(deviceBasicTables , Code);
        }
        static public  List<Drawer> SQL_GetDrawersByCode(List<object[]> deviceTables , string Code)
        {
            Code = Code.Replace("*", "");
            List<Drawer> drawers = new List<Drawer>();
            List<object[]> list_value = deviceTables;

            List<string> json_strings = (from temp in list_value
                                         where temp[(int)enum_DeviceTable.Value].ObjectToString().Contains(Code)
                                         select temp[(int)enum_DeviceTable.Value].ObjectToString()).ToList();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < json_strings.Count; i++)
            {
                string jsonString = json_strings[i];
                if (i == 0) sb.Append("[");
                if (jsonString.Contains(Code) == false) continue;

                sb.Append($"{jsonString}");

                if (i != json_strings.Count - 1)
                {
                    sb.Append($",");
                }
                if (i == json_strings.Count - 1) sb.Append("]");
            }

            string json_result = sb.ToString();
            if (json_result.StringIsEmpty()) json_result = "[]";
            drawers = json_result.JsonDeserializet<List<Drawer>>();


            return drawers;
        }

 
        static public  List<DeviceBasic> GetAllDeviceBasic(SQLUI.SQLControl sQLControl)
        {
            List<object[]> deviceBasicTables = sQLControl.GetAllRows(null);
            return GetAllDeviceBasic(deviceBasicTables);
        }
        static public List<DeviceBasic> GetAllDeviceBasic(List<object[]> deviceTables)
        {
            List<DeviceBasic> deviceBasics = new List<DeviceBasic>();
            List<object[]> list_value = deviceTables;


            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < list_value.Count; i++)
            {
                string jsonString = list_value[i][(int)enum_DeviceTable.Value].ObjectToString();
                if (i == 0) sb.Append("[");

                sb.Append($"{jsonString}");

                if (i != list_value.Count - 1)
                {
                    sb.Append($",");
                }
                if (i == list_value.Count - 1) sb.Append("]");
            }

            string json_result = sb.ToString();
            List<Drawer> drawers = json_result.JsonDeserializet<List<Drawer>>();
            List<Box> boxes = drawers.GetAllBoxes();
            for (int i = 0; i < boxes.Count; i++)
            {
                deviceBasics.Add(boxes[i]);
            }

            //Parallel.ForEach(list_value, value =>
            //{
            //    string jsonString = value[(int)enum_DeviceTable.Value].ObjectToString();
            //    DrawerBasic drawer = jsonString.JsonDeserializet<DrawerBasic>();
            //    for (int i = 0; i < drawer.Boxes.Count; i++)
            //    {
            //        for (int k = 0; k < drawer.Boxes[i].Length; k++)
            //        {
            //            deviceBasics.LockAdd(drawer.Boxes[i][k]);
            //        }
            //    }
            //});

            //deviceBasics = (from value in deviceBasics
            //                where value != null
            //                select value).ToList();
            return deviceBasics;
        }

        static public List<DeviceBasic> GetDeviceBasicByCode(SQLUI.SQLControl sQLControl, string Code)
        {
            List<object[]> deviceBasicTables = sQLControl.GetAllRows(null);
            return GetDeviceBasicByCode(deviceBasicTables, Code);
        }
        static public List<DeviceBasic> GetDeviceBasicByCode(List<object[]> deviceTables , string Code)
        {
            Code = Code.Replace("*", "");

            List<DeviceBasic> deviceBasics = new List<DeviceBasic>();
            List<object[]> list_value = deviceTables;
            List<string> json_strings = (from temp in list_value
                                         where temp[(int)enum_DeviceTable.Value].ObjectToString().Contains(Code)
                                         select temp[(int)enum_DeviceTable.Value].ObjectToString()).ToList();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < json_strings.Count; i++)
            {
                string jsonString = json_strings[i];  
                if (i == 0) sb.Append("[");
                if (jsonString.Contains(Code) == false) continue;

                sb.Append($"{jsonString}");

                if (i != json_strings.Count - 1)
                {
                    sb.Append($",");
                }
                if (i == json_strings.Count - 1) sb.Append("]");
            }

            string json_result = sb.ToString();
            if (json_result.StringIsEmpty()) json_result = "[]";
            List<Drawer> drawers = json_result.JsonDeserializet<List<Drawer>>();
            List<Box> boxes = drawers.GetAllBoxes();
            for (int i = 0; i < boxes.Count; i++)
            {
                if (boxes[i].Code != Code) continue;
                deviceBasics.Add(boxes[i]);
            }

 
            return deviceBasics;
        }

        static public Drawer SQL_GetDevice(SQLUI.SQLControl sQLControl, string IP)
        {
            List<object[]> deviceBasicTables = sQLControl.GetRowsByDefult(sQLControl.TableName, "IP", IP);
            if (deviceBasicTables.Count == 0) return null;
            string jsonString = deviceBasicTables[0][(int)enum_DeviceTable.Value].ObjectToString();
            Drawer device = jsonString.JsonDeserializet<Drawer>();
            return device;
        }

        static public List<Drawer> Add_NewDrawer(this List<Drawer> Drawers, string IP, int Port)
        {
            return Drawers.Add_NewDrawer(IP, Port, new Drawer(IP, Port, Drawer.Enum_DrawerType._4X8));
        }
        static public List<Drawer> Add_NewDrawer(this List<Drawer> Drawers, string IP, int Port, Drawer.Enum_DrawerType enum_DrawerType)
        {
            return Drawers.Add_NewDrawer(IP, Port, new Drawer(IP, Port, enum_DrawerType));
        }
        static public List<Drawer> Add_NewDrawer(this List<Drawer> Drawers, string IP, int Port, Drawer Drawer)
        {
            Drawer.IP = IP;
            Drawer.Port = Port;
            if (Drawers.SortByIP(IP) == null)
            {
                Drawers.Add(Drawer);
                return Drawers;
            }
            else
            {
                Drawers = Drawers.ReplacePortByIP(IP, Port);
            }
            return Drawers;
        }
        static public List<Drawer> Add_NewDrawer(this List<Drawer> Drawers, Drawer drawer)
        {
            for (int i = 0; i < Drawers.Count; i++)
            {
               if( Drawers[i].IP == drawer.IP)
                {
                    Drawers[i] = drawer;
                    return Drawers;
                }
            }
            Drawers.Add(drawer);
            return Drawers;

        }
        static public List<Drawer> Add_NewDrawer(this List<Drawer> Drawers, Box box)
        {
            if (box == null) return Drawers;
            for (int i = 0; i < Drawers.Count; i++)
            {
                if (Drawers[i].IP == box.IP)
                {
                    Drawers[i] = Drawers[i].ReplaceBox(box);
                    return Drawers;
                }
            }
            return Drawers;

        }
        static public Drawer Add_NewBox(this Drawer drawer, Box box)
        {
            box.IP = drawer.IP;
            drawer.Boxes.Add(new Box[] { box });
            return drawer;
        }
        static public List<Drawer> ReplaceByIP(this List<Drawer> Drawers, Drawer drawer)
        {
            for (int i = 0; i < Drawers.Count; i++)
            {
                if (Drawers[i].IP == drawer.IP)
                {
                    Drawers[i] = drawer;
                    return Drawers;
                }
            }
            return Drawers;
        }
        static public List<Drawer> ReplacePortByIP(this List<Drawer> Drawers, string IP, int Port)
        {
            for (int i = 0; i < Drawers.Count; i++)
            {
                if (Drawers[i].IP == IP)
                {
                    Drawers[i].IP = IP;
                    Drawers[i].Port = Port;
                }
            }
            return Drawers;
        }
        static public List<Drawer> RemoveByIP(this List<Drawer> Drawers, string IP)
        {
            List<Drawer> Drawers_buf = new List<Drawer>();
            foreach (Drawer drawer in Drawers)
            {
                if (drawer.IP != IP)
                {
                    Drawers_buf.Add(drawer);
                }
            }
            Drawers = Drawers_buf;
            return Drawers;
        }
        static public List<Drawer> GetUpToSQL(this List<Drawer> Drawers)
        {
            List<Drawer> Drawers_buf = new List<Drawer>();
            for (int i = 0; i < Drawers.Count; i++)
            {
                if (Drawers[i].UpToSQL)
                {
                    Drawers[i].UpToSQL = false;
                    Drawers_buf.Add(Drawers[i]);
                }
            }
            return Drawers_buf;
        }
        static public List<Box> SortByCode(this List<Drawer> Drawers, string Code)
        {    
            bool flag_serch = Code.Contains("*");
            Code = Code.Replace("*", "");
            List<Box> boxes = new List<Box>();
            for (int i = 0; i < Drawers.Count; i++)
            {
                for (int col = 0; col < Drawers[i].Boxes.Count; col++)
                {
                    for(int row = 0; row < Drawers[i].Boxes[col].Length; row ++)
                    {
                        if(flag_serch)
                        {
                            if (Drawers[i].Boxes[col][row].Code.ToUpper().Contains(Code.ToUpper())) boxes.Add(Drawers[i].Boxes[col][row]);
                        }
                        else
                        {
                            if (Drawers[i].Boxes[col][row].Code.ToUpper() == Code.ToUpper()) boxes.Add(Drawers[i].Boxes[col][row]);
                        }
                      
                    }
               
                }
            }
            return boxes;
        }
        static public List<Box> SortLikeByCode(this List<Drawer> Drawers, string Code)
        {
            Code = Code.Replace("*", "");
            List<Box> boxes = new List<Box>();
            for (int i = 0; i < Drawers.Count; i++)
            {
                for (int col = 0; col < Drawers[i].Boxes.Count; col++)
                {
                    for (int row = 0; row < Drawers[i].Boxes[col].Length; row++)
                    {
                        if (Drawers[i].Boxes[col][row].Code.ToUpper().Contains(Code.ToUpper())) boxes.Add(Drawers[i].Boxes[col][row]);
                    }

                }
            }
            return boxes;
        }
        static public List<Box> SortByCode(this Drawer drawer, string Code)
        {
            List<Drawer> drawers = new List<Drawer>();
            drawers.Add(drawer);
            return SortByCode(drawers, Code.ToUpper());
        }
        static public List<Box> SortLikeByCode(this Drawer drawer, string Code)
        {
            List<Drawer> drawers = new List<Drawer>();
            drawers.Add(drawer);
            return SortLikeByCode(drawers, Code.ToUpper());
        }
        static public List<Box> SortByBarCode(this List<Drawer> Drawers, string Code)
        {
            Code = Code.Replace("*", "");
            List<Box> boxes = new List<Box>();
            for (int i = 0; i < Drawers.Count; i++)
            {
                for (int col = 0; col < Drawers[i].Boxes.Count; col++)
                {
                    for (int row = 0; row < Drawers[i].Boxes[col].Length; row++)
                    {
                        if (Drawers[i].Boxes[col][row].BarCode == Code) boxes.Add(Drawers[i].Boxes[col][row]);
                    }

                }
            }
            return boxes;
        }
        static public Drawer SortByIP(this List<Drawer> Drawers ,string IP)
        {
            if (Drawers == null) return null;
            for (int i = 0; i < Drawers.Count; i++)
            {
                if (Drawers[i].IP == IP) return Drawers[i];
            }

            return null;
        }
    
        static public Drawer SortByName(this List<Drawer> Drawers, string Name)
        {
            for (int i = 0; i < Drawers.Count; i++)
            {
                if (Drawers[i].Name == Name) return Drawers[i];
            }

            return null;
        }
        static public Drawer ReplaceBox(this List<Drawer> Drawers, Box box)
        {
            Drawer drawer = Drawers.SortByIP(box.IP);
            if (drawer == null) return null;
            return drawer.ReplaceBox(box);
        }
        static public Drawer ReplaceBox(this Drawer drawer, Box box)
        {
            for (int col = 0; col < drawer.Boxes.Count; col++)
            {
                for (int row = 0; row < drawer.Boxes[col].Length; row++)
                {
                    if (drawer.Boxes[col][row].Column == box.Column && drawer.Boxes[col][row].Row == box.Row)
                    {
                        drawer.Boxes[col][row] = box;
                        return drawer;
                    }
                }

            }
            return drawer;
        }
        static public List<Drawer> ClearDrawer(this List<Drawer> Drawers, string IP)
        {
            Drawers.SortByIP(IP).Clear();
            return Drawers;
        }
        static public List<Box> GetAllBoxes(this List<Drawer> Drawers)
        {
            List<Box> boxes = new List<Box>();
            List<Box> boxes_buf = new List<Box>();
            for (int i = 0; i < Drawers.Count; i++)
            {
                boxes_buf = Drawers[i].GetAllBoxes();
                for (int k = 0; k < boxes_buf.Count; k++)
                {
                    boxes.Add(boxes_buf[k]);
                }
            }
            return boxes;
        }
        static public List<Box> GetAllBoxes(this Drawer drawer)
        {
            List<Box> boxes = new List<Box>();
            for (int i = 0; i < drawer.Boxes.Count; i++)
            {
                for (int k = 0; k < drawer.Boxes[i].Length; k++)
                {
                    boxes.Add(drawer.Boxes[i][k]);
                }
            }
            return boxes;
        }
        static public List<Box> GetMasterBoxes(this Drawer drawer)
        {
            List<Box> boxes = GetAllBoxes(drawer);
            List<Box> boxes_buf = new List<Box>();
            for (int i = 0; i < boxes.Count; i++)
            {
                if (boxes[i].Slave == false) boxes_buf.Add(boxes[i]);
            }
            return boxes_buf;
        }
        static public Box GetBox(this Drawer drawer, string GUID)
        {
            List<Box> boxes = drawer.GetAllBoxes();
            for (int i = 0; i < boxes.Count; i++)
            {
                if (boxes[i].GUID == GUID) return boxes[i];
            }
            return null;
        }
        static public Box GetByGUID(this Drawer drawer, string GUID)
        {
            List<Box> boxes = drawer.GetAllBoxes();
            for (int i = 0; i < boxes.Count; i++)
            {
                if (boxes[i].GUID == GUID) return boxes[i];
            }
            return null;
        }
        static public Drawer ReplaceByGUID(this List<Drawer> Drawers, Box box)
        {
            Drawer drawer = Drawers.SortByIP(box.IP);
            if (drawer == null) return null;
            return drawer.ReplaceByGUID(box);
        }
        static public Drawer ReplaceByGUID(this Drawer drawer, Box box)
        {
            for (int i = 0; i < drawer.Boxes.Count; i++)
            {
                for (int k = 0; k < drawer.Boxes[i].Length; k++)
                {
                    if (drawer.Boxes[i][k].GUID == box.GUID)
                    {
                        drawer.Boxes[i][k] = box;
                    }
                }
            }
            return drawer;
        }
        static public void RemoveByGUID(this Drawer drawer, string GUID)
        {
            List<Box[]> boxes_buf = new List<Box[]>();
            for (int i = 0; i < drawer.Boxes.Count; i++)
            {
                List<Box> boxe_ary = new List<Box>();
                for (int k = 0; k < drawer.Boxes[i].Length; k++)
                {
                    if (drawer.Boxes[i][k].GUID != GUID)
                    {
                        boxe_ary.Add(drawer.Boxes[i][k]);
                    }              
                }
                if (boxe_ary.Count > 0)
                {
                    boxes_buf.Add(boxe_ary.ToArray());
                }
    
            }
            drawer.Boxes = boxes_buf;
        }
        static public void ReplaceIP(this Drawer drawer, string IP)
        {
            drawer.ReplaceIP(IP, drawer.Port);
        }
        static public void ReplaceIP(this Drawer drawer , string IP , int Port)
        {
            List<Box> boxes = drawer.GetAllBoxes();
            drawer.IP = IP;
            drawer.Port = Port;
            for (int i = 0; i < boxes.Count; i++)
            {
                boxes[i].IP = IP;
                boxes[i].Port = Port;
            }
        }

        static public List<Device> GetAllDevice(this List<Drawer> Drawers)
        {
            List<Box> boxes = new List<Box>();
            List<Box> boxes_buf = new List<Box>();
            for (int i = 0; i < Drawers.Count; i++)
            {
                boxes_buf = Drawers[i].GetAllBoxes();
                for (int k = 0; k < boxes_buf.Count; k++)
                {
                    boxes.Add(boxes_buf[k]);
                }
            }
            List<Device> devices = new List<Device>();
            for (int i = 0; i < boxes.Count; i++)
            {
                devices.Add(boxes[i]);
            }
            return devices;
        }

        static public UDP_Class GetUPDClass(this Drawer drawer)
        {
            if (drawer != null)
            {
                UDP_Class uDP_Class = new UDP_Class(drawer.IP, drawer.Port);
                return uDP_Class;
            }
            return null;
        }
    }

    [Serializable]
    public class DrawerBasic
    {
        private List<DeviceBasic[]> boxes = new List<DeviceBasic[]>();
        public List<DeviceBasic[]> Boxes { get => boxes; set => boxes = value; }
    }
    [Serializable]
    public class Drawer
    {
        public string serverIP = "";
        public uint serverPort = 0;
        public string dbName = "";

        public bool ActionDone = false;
        public bool UpToSQL = false;
        public static int NumOfLED = 450;
        public bool input = false;


        private string area = "";
        public string Area { get => area; set => area = value; }

        [JsonIgnore]
        public byte[] LED_Bytes = new byte[NumOfLED * 3];
        [JsonIgnore]
        public Color[] LED_Colors
        {
            get
            {
                Color[] colors = new Color[NumOfLED];
                for(int i = 0; i < colors.Length; i++)
                {
                    Color color = new Color();
                    color = Color.FromArgb(LED_Bytes[i * 3 + 0], LED_Bytes[i * 3 + 1], LED_Bytes[i * 3 + 2]);
                    if (color.R == 0 && color.G == 0 && color.B == 0) color = Color.Black;
                    colors[i] = color;
                }
                return colors;
            }
        }
        private Enum_DrawerType drawerType = Enum_DrawerType._4X8;
        public enum Enum_DrawerType
        {
            //原始版本
            _4X8,
            _3X8,
            //大抽屜版本
            _5X8_A,
            _4X8_A,
            //ADC抽屜版本
            _1X8_ADC,
            _3X8_ADC,
        }
        public Drawer()
        {
            this.BoxInit();
        }
        public Drawer(string IP, int Port, int PannelWidth, int PannelHeight)
        {
            this.IP = IP;
            this.Port = Port;
            this.PannelWidth = PannelWidth;
            this.PannelHeight = PannelHeight;
            this.drawerType = Enum_DrawerType._4X8;
            if (this.drawerType == Enum_DrawerType._4X8)
            {
                this.Num_Of_Columns = 4;
                this.Num_Of_Rows = 8;
            }
            else if (this.drawerType == Enum_DrawerType._3X8)
            {
                this.Num_Of_Columns = 3;
                this.Num_Of_Rows = 8;
            }
            else if (this.drawerType == Enum_DrawerType._5X8_A)
            {
                this.Num_Of_Columns = 5;
                this.Num_Of_Rows = 8;
            }
            else if (this.drawerType == Enum_DrawerType._4X8_A)
            {
                this.Num_Of_Columns = 4;
                this.Num_Of_Rows = 8;
            }
            this.BoxInit();
        }
        public Drawer(string IP, int Port)
        {
            this.IP = IP;
            this.Port = Port;
            this.PannelWidth = DrawerUI_EPD_583.Pannel_Width;
            this.PannelHeight = DrawerUI_EPD_583.Pannel_Height;
            this.drawerType =  Enum_DrawerType._4X8;
            if (this.drawerType == Enum_DrawerType._4X8)
            {
                this.Num_Of_Columns = 4;
                this.Num_Of_Rows = 8;
            }
            else if (this.drawerType == Enum_DrawerType._3X8)
            {
                this.Num_Of_Columns = 3;
                this.Num_Of_Rows = 8;
            }
            this.BoxInit();
        }
        public Drawer(string IP, int Port, Enum_DrawerType drawerType)
        {
            this.IP = IP;
            this.Port = Port;
            this.PannelWidth = DrawerUI_EPD_583.Pannel_Width;
            this.PannelHeight = DrawerUI_EPD_583.Pannel_Height;
            this.SetDrawerType(drawerType);
        }
        public Drawer(string IP, int Port,int pannelWidth , int pannelHeight, Enum_DrawerType drawerType)
        {
            this.IP = IP;
            this.Port = Port;
            this.PannelWidth = pannelWidth;
            this.PannelHeight = pannelHeight;
            this.SetDrawerType(drawerType);
        }

        private List<Box[]> boxes = new List<Box[]>();

        private int station = -1;
        private string iP = "0.0.0.0";
        private string name = "";
        private int port = 4000;
        private int num_Of_Columns = 0;
        private int num_Of_Rows = 0;
        private int pannelWidth = 0;
        private int pannelHeight = 0;
        private string outputAdress = "";
        private string inputAdress = "";
        private DateTime unlock_start_dateTime;
        private DateTime unlock_end_dateTime;
        private bool unlockTimeEnable = false;
        private bool isAllLight = true;
        private string speaker = "";
        private bool alarmEnable = false;
        private bool breathLight = false;

        public List<Box[]> Boxes { get => boxes; set => boxes = value; }
        public int Station { get => station; set => station = value; }
        public string IP
        {
            get => iP;
            set
            {
                iP = value;
                for (int col = 0; col < boxes.Count; col++)
                {
                    for (int row = 0; row < boxes[col].Length; row++)
                    {
                        boxes[col][row].IP = iP;
                    }
                }
            }
        }
        public string Name { get => name; set => name = value; }
        public int Port
        {
            get => port;
            set
            {
                port = value;
                for (int col = 0; col < boxes.Count; col++)
                {
                    for (int row = 0; row < boxes[col].Length; row++)
                    {
                        boxes[col][row].Port = port;
                    }
                }
            }
        }
        public int Num_Of_Columns { get => num_Of_Columns; set => num_Of_Columns = value; }
        public int Num_Of_Rows { get => num_Of_Rows; set => num_Of_Rows = value; }
        public int PannelWidth { get => pannelWidth; set => pannelWidth = value; }
        public int PannelHeight { get => pannelHeight; set => pannelHeight = value; }
        public string OutputAdress { get => outputAdress; set => outputAdress = value; }
        public string InputAdress { get => inputAdress; set => inputAdress = value; }
        public DateTime Unlock_start_dateTime { get => unlock_start_dateTime; set => unlock_start_dateTime = value; }
        public DateTime Unlock_end_dateTime { get => unlock_end_dateTime; set => unlock_end_dateTime = value; }
        public bool UnlockTimeEnable { get => unlockTimeEnable; set => unlockTimeEnable = value; }
        public bool IsAllLight { get => isAllLight; set => isAllLight = value; }
        public string Speaker { get => speaker; set => speaker = value; }
        public bool AlarmEnable { get => alarmEnable; set => alarmEnable = value; }
        public bool BreathLight { get => breathLight; set => breathLight = value; }


        public Enum_DrawerType DrawerType 
        { 
            get => drawerType;
            set
            {
                drawerType = value;
            }
        }
        public DeviceType DeviceType
        {
            get
            {
                List<Box> list_Boxes = this.GetAllBoxes();
                for (int i = 0; i < list_Boxes.Count; i++)
                {
                    return list_Boxes[i].DeviceType;
                }
                return DeviceType.None;
            }
        }


        public void SetDeviceType(DeviceType deviceType)
        {
            if (deviceType == DeviceType.EPD583 || deviceType == DeviceType.EPD583_lock)
            {
                this.pannelWidth = 648;
                this.pannelHeight = 480;
            }
            if (deviceType == DeviceType.EPD730 || deviceType == DeviceType.EPD730_lock)
            {
                this.pannelWidth = 800;
                this.pannelHeight = 480;
            }
            if (deviceType == DeviceType.EPD420_D_lock || deviceType == DeviceType.EPD420_D)
            {
                this.pannelWidth = 400;
                this.pannelHeight = 300;
            }
            List<Box> list_Boxes = this.GetAllBoxes();
            for (int i = 0; i < list_Boxes.Count; i++)
            {
                list_Boxes[i].DeviceType = deviceType;
            }
        }
        public void SetDrawerType(Enum_DrawerType drawerType)
        {
            this.drawerType = drawerType;
            if (this.drawerType == Enum_DrawerType._4X8)
            {
                this.Num_Of_Columns = 4;
                this.Num_Of_Rows = 8;
            }
            else if (this.drawerType == Enum_DrawerType._3X8)
            {
                this.Num_Of_Columns = 3;
                this.Num_Of_Rows = 8;
            }
            else if (this.drawerType == Enum_DrawerType._5X8_A)
            {
                this.Num_Of_Columns = 5;
                this.Num_Of_Rows = 8;
            }
            else if (this.drawerType == Enum_DrawerType._4X8_A)
            {
                this.Num_Of_Columns = 4;
                this.Num_Of_Rows = 8;
            }
            else if (this.drawerType == Enum_DrawerType._3X8_ADC)
            {
                this.Num_Of_Columns = 3;
                this.Num_Of_Rows = 8;
            }
            else if (this.drawerType == Enum_DrawerType._1X8_ADC)
            {
                this.Num_Of_Columns = 1;
                this.Num_Of_Rows = 8;
            }
            this.BoxInit();
        }
        public void Clear()
        {
            for (int c = 0; c < Boxes.Count; c++)
            {
                for (int r = 0; r < Boxes[c].Length; r++)
                {
                    Boxes[c][r].Clear();
                }
            }
        }
        public void BoxResize()
        {
            DeviceType deviceType = this.DeviceType;
            if (this.DeviceType == DeviceType.EPD583 || this.DeviceType == DeviceType.EPD583_lock)
            {
                this.pannelWidth = 648;
                this.pannelHeight = 480;
            }
            if (this.DeviceType == DeviceType.EPD730 || this.DeviceType == DeviceType.EPD730_lock)
            {
                this.pannelWidth = 800;
                this.pannelHeight = 480;
            }
            if (this.DeviceType == DeviceType.EPD420 || this.DeviceType == DeviceType.EPD420_lock)
            {
                this.pannelWidth = 400;
                this.pannelHeight = 300;
            }
            int x, y, width, height;
            Size Box_size = this.GetBoxSize();
            for (int c = 0; c < this.Boxes.Count; c++)
            {
                x = c * Box_size.Width;
                y = 0;
                width = Box_size.Width;
                height = 0;
                for (int r = 0; r < this.Boxes[c].Length; r++)
                {
                    this.Boxes[c][r].DeviceType = deviceType;
                    y += height;
                    height = Box_size.Height;
                    this.Boxes[c][r].X = x;
                    this.Boxes[c][r].Y = y;
                    this.Boxes[c][r].Width = width;
                    this.Boxes[c][r].Height = height;
                }
            }
        }
        public void BoxInit()
        {
            DeviceType deviceType = this.DeviceType;
            if (this.DeviceType == DeviceType.EPD583 || this.DeviceType == DeviceType.EPD583_lock)
            {
                this.pannelWidth = 648;
                this.pannelHeight = 480;
            }
            if (this.DeviceType == DeviceType.EPD730 || this.DeviceType == DeviceType.EPD730_lock)
            {
                this.pannelWidth = 800;
                this.pannelHeight = 480;
            }
            if (this.DeviceType == DeviceType.EPD420 || this.DeviceType == DeviceType.EPD420_lock)
            {
                this.pannelWidth = 400;
                this.pannelHeight = 300;
            }
            int x, y, width, height;
            height = 0;
            Size Box_size = this.GetBoxSize();
            Box box;
            List<Box> list_colOfBox = new List<Box>();
            List<Box[]> list_RectOfBox = new List<Box[]>();
            for (int i = 0; i < this.Num_Of_Columns; i++)
            {
                list_colOfBox.Clear();
                x = i * Box_size.Width;
                y = 0;
                width = Box_size.Width;
                height = 0;                          
                for (int k = 0; k < this.Num_Of_Rows; k++)
                {
                    box = new Box(this.IP, this.Port, i, k);
                    box.DeviceType = deviceType;
                    y += height;
                    height = Box_size.Height;
                    box.X = x;
                    box.Y = y;
                    box.Width = width;
                    box.Height = height;
                    list_colOfBox.Add(box);
                }
                list_RectOfBox.Add(list_colOfBox.ToArray());
            }
            this.Boxes = list_RectOfBox;
       
    
        }
        public void SetAllBoxes_LightOff()
        {
            for (int i = 0; i < this.Boxes.Count; i++)
            {
                for (int k = 0; k < this.Boxes[i].Length; k++)
                {
                    this.Boxes[i][k].LightOn = false;
                }
            }
        }
        private Size GetBoxSize()
        {
            return this.GetBoxSize(this.Num_Of_Columns, this.Num_Of_Rows);
        }
        private Size GetBoxSize(int num_Of_Columns ,int num_Of_Rows)
        {
            int width = (int)((double)this.PannelWidth / (double)num_Of_Columns);
            int height = (int)((double)this.PannelHeight / (double)num_Of_Rows);
            return new Size(width, height);
        }
    }
    [Serializable]
    public class Box : Device
    {
        private int slaveBox_Column = -1;
        private int slaveBox_Row = -1;
        private int masterBox_Column = -1;
        private int masterBox_Row = -1;
        private bool slave = false;
        private int x = 0;
        private int y = 0;
        private int width = 0;
        private int height = 0;
        private int column = 0;
        private int row = 0;
        private bool enable = false;
        private int pen_Width = 2;

        public int SlaveBox_Column { get => slaveBox_Column; set => slaveBox_Column = value; }
        public int SlaveBox_Row { get => slaveBox_Row; set => slaveBox_Row = value; }
        public int MasterBox_Column { get => masterBox_Column; set => masterBox_Column = value; }
        public int MasterBox_Row { get => masterBox_Row; set => masterBox_Row = value; }
        public bool Slave { get => slave; set => slave = value; }
        public int X { get => x; set => x = value; }
        public int Y { get => y; set => y = value; }
        public int Width { get => width; set => width = value; }
        public int Height { get => height; set => height = value; }
        public int Column { get => column; set => column = value; }
        public int Row { get => row; set => row = value; }
        public int Pen_Width { get => pen_Width; set => pen_Width = value; }
        public bool Enable { get => enable; set => enable = value; }


        public Box(int station)
        {
            this.GUID = Guid.NewGuid().ToString();
            this.station = station;
        }
        public Box(string IP, int Port, int column, int row)
        {
            this.GUID = Guid.NewGuid().ToString();
            this.IP = IP;
            this.Port = Port;
            this.column = column;
            this.row = row;
        }
        public Box()
        {
            this.GUID = Guid.NewGuid().ToString();

        }

    }
}
