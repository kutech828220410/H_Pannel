using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using Basic;
using System.Collections.Concurrent;
namespace H_Pannel_lib
{
    public class UDP_READ_basic
    {
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
        private float _dht_h = 0;
        private float _dht_t = 0;

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
        public float dht_h { get => _dht_h; set => _dht_h = value; }
        public float dht_t { get => _dht_t; set => _dht_t = value; }

    }

    public enum enum_PictureType
    {
        無,
        藥品圖片,
        高警訊_1,
        高警訊_2,
        高警訊_3,
        高警訊_4,
        LASA_1,
        LASA_2,

    }
    public class StockClass
    {

        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("validity_period")]
        public string Validity_period { get; set; }
        [JsonPropertyName("lot_number")]
        public string Lot_number { get; set; }
        [JsonPropertyName("qty")]
        public string Qty { get; set; }

    }
  
    static public class StockClassMethod
    {
        static public double GetTolalQty(this List<StockClass> stockClasses)
        {
            double qty = 0;
            for(int i = 0; i < stockClasses.Count; i++)
            {
                qty += stockClasses[i].Qty.StringToDouble();
            }
            return qty;
        }
        static public List<StockClass> QtyAbs(this List<StockClass> stockClasses)
        {
            for (int i = 0; i < stockClasses.Count; i++)
            {
                stockClasses[i].Qty = (stockClasses[i].Qty.StringToDouble() * -1).ToString();
            }
            return stockClasses;
        }
    }

    static public class DeviceBasicMethod
    {
        static public void SQL_Init(SQLUI.SQL_DataGridView.ConnentionClass connentionClass, string TableName)
        {
            connentionClass.TableName = TableName;
            SQL_Init(connentionClass);
        }
        static public void SQL_Init(SQLUI.SQL_DataGridView.ConnentionClass connentionClass)
        {
            SQLUI.SQLControl sQLControl = new SQLUI.SQLControl(connentionClass.IP, connentionClass.DataBaseName, connentionClass.TableName, connentionClass.UserName, connentionClass.Password, connentionClass.Port, connentionClass.MySqlSslMode);
            SQL_Init(sQLControl);
        }
        static public void SQL_Init(SQLUI.SQLControl sQLControl)
        {
            if (!sQLControl.IsTableCreat(null))
            {
                SQLUI.Table table = new SQLUI.Table(sQLControl.TableName);
                table.AddColumnList(enum_DeviceTable.GUID.GetEnumName(), SQLUI.Table.StringType.VARCHAR, SQLUI.Table.IndexType.PRIMARY);
                table.AddColumnList(enum_DeviceTable.IP.GetEnumName(), SQLUI.Table.StringType.VARCHAR, SQLUI.Table.IndexType.None);
                table.AddColumnList(enum_DeviceTable.Port.GetEnumName(), SQLUI.Table.StringType.VARCHAR, SQLUI.Table.IndexType.None);
                table.AddColumnList(enum_DeviceTable.Value.GetEnumName(), SQLUI.Table.StringType.MEDIUMTEXT, SQLUI.Table.IndexType.None);
                sQLControl.CreatTable(table);
            }
        }
        static public List<DeviceBasic> SQL_GetAllDeviceBasic(SQLUI.SQL_DataGridView.ConnentionClass connentionClass, string TableName)
        {
            SQLUI.SQLControl sQLControl = new SQLUI.SQLControl(connentionClass.IP, connentionClass.DataBaseName, connentionClass.TableName, connentionClass.UserName, connentionClass.Password, connentionClass.Port, connentionClass.MySqlSslMode);
            return SQL_GetAllDeviceBasic(sQLControl);
        }
        static public List<DeviceBasic> SQL_GetAllDeviceBasic(SQLUI.SQLControl sQLControl)
        {
            List<object[]> deviceBasicTables = sQLControl.GetAllRows(null);
            List<DeviceBasic> deviceBasics = new List<DeviceBasic>();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < deviceBasicTables.Count; i++)
            {
                string jsonString = deviceBasicTables[i][(int)enum_DeviceTable.Value].ObjectToString();
                if (i == 0) sb.Append("[");

                sb.Append($"{jsonString}");

                if (i != deviceBasicTables.Count - 1)
                {
                    sb.Append($",");
                }
                if (i == deviceBasicTables.Count - 1) sb.Append("]");
            }
      
            string json_result = sb.ToString();
            if (json_result.StringIsEmpty()) json_result = "[]";
            deviceBasics = json_result.JsonDeserializet<List<DeviceBasic>>();
            return deviceBasics;
        }
        static public List<DeviceSimple> SQL_GetAllDeviceSimple(SQLUI.SQLControl sQLControl)
        {
            List<object[]> deviceBasicTables = sQLControl.GetAllRows(null);
            List<DeviceSimple> deviceBasics = new List<DeviceSimple>();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < deviceBasicTables.Count; i++)
            {
                string jsonString = deviceBasicTables[i][(int)enum_DeviceTable.Value].ObjectToString();
                if (i == 0) sb.Append("[");

                sb.Append($"{jsonString}");

                if (i != deviceBasicTables.Count - 1)
                {
                    sb.Append($",");
                }
                if (i == deviceBasicTables.Count - 1) sb.Append("]");
            }

            string json_result = sb.ToString();
            if (json_result.StringIsEmpty()) json_result = "[]";
            deviceBasics = json_result.JsonDeserializet<List<DeviceSimple>>();
            return deviceBasics;
       
        }
        static public DeviceBasic SQL_GetDeviceBasic(SQLUI.SQL_DataGridView.ConnentionClass connentionClass, string TableName, DeviceBasic deviceBasic)
        {
            return SQL_GetDeviceBasic(connentionClass, TableName, deviceBasic.GUID);
        }
        static public DeviceBasic SQL_GetDeviceBasic(SQLUI.SQLControl sQLControl, DeviceBasic deviceBasic)
        {
            return SQL_GetDeviceBasic(sQLControl, deviceBasic.GUID);
        }
        static public DeviceBasic SQL_GetDeviceBasic(SQLUI.SQL_DataGridView.ConnentionClass connentionClass, string TableName , string GUID)
        {
            SQLUI.SQLControl sQLControl = new SQLUI.SQLControl(connentionClass.IP, connentionClass.DataBaseName, connentionClass.TableName, connentionClass.UserName, connentionClass.Password, connentionClass.Port, connentionClass.MySqlSslMode);
            return SQL_GetDeviceBasic(sQLControl, GUID);
        }
        static public DeviceBasic SQL_GetDeviceBasic(SQLUI.SQLControl sQLControl, string GUID)
        {
            List<object[]> deviceBasicTables = sQLControl.GetRowsByDefult(sQLControl.TableName, "GUID", GUID);
            if (deviceBasicTables.Count == 0) return null;
            string jsonString = deviceBasicTables[0][(int)enum_DeviceTable.Value].ObjectToString();
            DeviceBasic deviceBasic = jsonString.JsonDeserializet<DeviceBasic>();

            deviceBasic.serverIP = sQLControl.Server;
            deviceBasic.serverPort = sQLControl.Port;
            deviceBasic.dbName = sQLControl.Database;

            return deviceBasic;
        }

        static public void SQL_ReplaceDeviceBasic(SQLUI.SQL_DataGridView.ConnentionClass connentionClass, string TableName, List<DeviceBasic> deviceBasics)
        {
            SQLUI.SQLControl sQLControl = new SQLUI.SQLControl(connentionClass.IP, connentionClass.DataBaseName, connentionClass.TableName, connentionClass.UserName, connentionClass.Password, connentionClass.Port, connentionClass.MySqlSslMode);
            SQL_ReplaceDeviceBasic(sQLControl, deviceBasics);
        }
        static public void SQL_ReplaceDeviceBasic(SQLUI.SQLControl sQLControl, List<DeviceBasic> deviceBasics)
        {
            List<object[]> list_value = new List<object[]>();
            Parallel.ForEach(deviceBasics, value =>
            {
                object[] SQLvalue = new object[new enum_DeviceTable().GetLength()];
                SQLvalue[(int)enum_DeviceTable.GUID] = value.GUID;
                SQLvalue[(int)enum_DeviceTable.IP] = "";
                SQLvalue[(int)enum_DeviceTable.Port] = "";
                SQLvalue[(int)enum_DeviceTable.Value] = value.JsonSerializationt();
                list_value.LockAdd(SQLvalue);

            });
            sQLControl.UpdateByDefulteExtra(sQLControl.TableName, list_value);
        }

        static public void SQL_AddDeviceBasic(SQLUI.SQL_DataGridView.ConnentionClass connentionClass, string TableName, List<DeviceBasic> deviceBasics)
        {
            SQLUI.SQLControl sQLControl = new SQLUI.SQLControl(connentionClass.IP, connentionClass.DataBaseName, connentionClass.TableName, connentionClass.UserName, connentionClass.Password, connentionClass.Port, connentionClass.MySqlSslMode);
            SQL_AddDeviceBasic(sQLControl, deviceBasics);
        }
        static public void SQL_AddDeviceBasic(SQLUI.SQLControl sQLControl, List<DeviceBasic> deviceBasics)
        {
            List<object[]> list_value = new List<object[]>();
            Parallel.ForEach(deviceBasics, value =>
            {
                object[] SQLvalue = new object[new enum_DeviceTable().GetLength()];
                SQLvalue[(int)enum_DeviceTable.GUID] = value.GUID;
                SQLvalue[(int)enum_DeviceTable.IP] = "";
                SQLvalue[(int)enum_DeviceTable.Port] = "";
                SQLvalue[(int)enum_DeviceTable.Value] = value.JsonSerializationt();
                list_value.LockAdd(SQLvalue);

            });
            sQLControl.AddRows(sQLControl.TableName, list_value);
        }


        static public List<DeviceBasic> Add_NewDeviceBasic(this List<DeviceBasic> DeviceBasics, DeviceBasic deviceBasic)
        {
            for (int i = 0; i < DeviceBasics.Count; i++)
            {
                if (DeviceBasics[i].GUID == deviceBasic.GUID)
                {
                    DeviceBasics[i] = deviceBasic;
                    return DeviceBasics;
                }
            }
            DeviceBasics.Add(deviceBasic);
            return DeviceBasics;
        }
        static public DeviceBasic SortByGUID(this List<DeviceBasic> DeviceBasics, string GUID)
        {
            foreach (DeviceBasic deviceBasic in DeviceBasics)
            {
                if (deviceBasic.GUID == GUID)
                {
                    return deviceBasic;
                }
            }
            return null;
        }
        static public List<DeviceBasic> SortByCode(this List<DeviceBasic> DeviceBasics, string Code)
        {
            bool flag_serch = Code.Contains("*");
            Code = Code.Replace("*", "");
            List<DeviceBasic> DeviceBasics_buf = new List<DeviceBasic>();
            foreach (DeviceBasic deviceBasic in DeviceBasics)
            {
                if(flag_serch == false)
                {
                    if (deviceBasic.Code == Code)
                    {
                        DeviceBasics_buf.Add(deviceBasic);
                    }
                }
              
            }
            return DeviceBasics_buf;
        }

        static public System.Collections.Generic.Dictionary<string, List<DeviceBasic>> CoverToDictionaryByCode(this List<DeviceBasic> DeviceBasics)
        {
            Dictionary<string, List<DeviceBasic>> dictionary = new Dictionary<string, List<DeviceBasic>>();

            foreach (var item in DeviceBasics)
            {
                string key = item.Code;

                // 如果字典中已經存在該索引鍵，則將值添加到對應的列表中
                if (dictionary.ContainsKey(key))
                {
                    dictionary[key].Add(item);
                }
                // 否則創建一個新的列表並添加值
                else
                {
                    List<DeviceBasic> values = new List<DeviceBasic> { item };
                    dictionary[key] = values;
                }
            }

            return dictionary;
        }
        static public List<DeviceBasic> SortDictionaryByCode(this System.Collections.Generic.Dictionary<string, List<DeviceBasic>> dictionary, string code)
        {
            code = code.Replace("*", "");
            if (dictionary.ContainsKey(code))
            {
                return dictionary[code];
            }
            return new List<DeviceBasic>();
        }
        public static List<DeviceBasic> SearchDictionaryByCode(this Dictionary<string, List<DeviceBasic>> dictionary, string code)
        {
            code = code.Replace("*", "");
            // 找到所有包含特定子字符串的鍵
            var matchingKeys = dictionary.Keys.Where(key => key.Contains(code)).ToList();

            // 聚合所有匹配鍵的值
            var result = new List<DeviceBasic>();
            foreach (var key in matchingKeys)
            {
                result.AddRange(dictionary[key]);
            }

            return result;
        }
    }

    [Serializable]
    public class DeviceBasicClass
    {
        private bool flag_UDP_Class_Init = false;
        private string tableName = "";
        private SQLUI.SQLControl sQLControl;
        public void Init(SQLUI.SQL_DataGridView.ConnentionClass connentionClass,string tableName)
        {
            Init(connentionClass.IP, connentionClass.DataBaseName, tableName, connentionClass.UserName, connentionClass.Password, connentionClass.Port, connentionClass.MySqlSslMode);
        }
        public void Init(SQLUI.SQL_DataGridView.ConnentionClass connentionClass)
        {
            Init(connentionClass.IP, connentionClass.DataBaseName, connentionClass.TableName, connentionClass.UserName, connentionClass.Password, connentionClass.Port, connentionClass.MySqlSslMode);
        }
        public void Init(string IP, string DataBaseName, string TableName, string UserName, string Password, uint Port, MySql.Data.MySqlClient.MySqlSslMode mySqlSslMode)
        {
            this.tableName = TableName;
            sQLControl = new SQLUI.SQLControl(IP, DataBaseName, TableName, UserName, Password, Port, mySqlSslMode);
            if(flag_UDP_Class_Init == false)
            {
                DeviceBasicMethod.SQL_Init(sQLControl);
            }
            flag_UDP_Class_Init = true;
        }
        public List<DeviceBasic> SQL_GetAllDeviceBasic()
        {
            if (sQLControl == null) return null;
            return DeviceBasicMethod.SQL_GetAllDeviceBasic(sQLControl);
        }
        public DeviceBasic SQL_GetDeviceBasic(DeviceBasic deviceBasic)
        {
            if (sQLControl == null) return null;
            return DeviceBasicMethod.SQL_GetDeviceBasic(sQLControl, deviceBasic);
        }
        public DeviceBasic SQL_GetDeviceBasic(string GUID)
        {
            if (sQLControl == null) return null;
            return DeviceBasicMethod.SQL_GetDeviceBasic(sQLControl, GUID);
        }
        public void SQL_ReplaceDeviceBasic(DeviceBasic deviceBasic)
        {
            List<DeviceBasic> deviceBasics = new List<DeviceBasic>();
            deviceBasics.Add(deviceBasic);
            this.SQL_ReplaceDeviceBasic(deviceBasics);
        }
        public void SQL_ReplaceDeviceBasic(List<DeviceBasic> deviceBasics)
        {
            if (sQLControl == null) return;
            DeviceBasicMethod.SQL_ReplaceDeviceBasic(sQLControl ,deviceBasics);
        }
        public void SQL_AddDeviceBasic(List<DeviceBasic> deviceBasics)
        {
            if (sQLControl == null) return;
            DeviceBasicMethod.SQL_AddDeviceBasic(sQLControl, deviceBasics);
        }
    }
    [Serializable]
    public class DeviceSimple
    {
        public bool flag_replace = false;
        public string GUID
        {
            get
            {
                if (this.gUID.StringIsEmpty()) gUID = Guid.NewGuid().ToString();
                return gUID;
            }
            set => gUID = value;
        }
        private string gUID = "";
        public string Master_GUID { get => master_GUID; set => master_GUID = value; }
        private string master_GUID = "";
        private string _IP = "";
        public string IP
        {
            get => _IP;
            set
            {
                if (_IP != value) flag_replace = true;
                _IP = value;
            }
        }
        private string _Code = "";
        public string Code
        {
            get
            {
                return _Code.Trim();
            }
            set
            {
                if (_Code != value) flag_replace = true;
                _Code = value.Trim();
                
            }
        }
        private string _SKDIACODE = "";
        public string SKDIACODE
        {
            get => _SKDIACODE;
            set
            {
                if (_SKDIACODE != value) flag_replace = true;
                _SKDIACODE = value;
            }
        }
        private string _Name = "";
        public string Name
        {
            get => _Name;
            set
            {
                if (_Name != value) flag_replace = true;
                _Name = value;
            }
        }
        private string _Inventory = "";
        [JsonInclude]
        public string Inventory
        {
            get
            {
                return this.取得庫存().ToString();
            }
            set
            {
                _Inventory = this.取得庫存().ToString();
            }
        }

        private int _SortIndex = 0;
        public int SortIndex
        {
            get => _SortIndex;
            set
            {
                if (_SortIndex != value) flag_replace = true;
                _SortIndex = value;
            }
        }

        public List<string> List_Validity_period
        {
            get
            {
                this.確認效期庫存();
                return list_Validity_period;
            }
            set => list_Validity_period = value;
        }
        public List<string> List_Inventory
        {
            get
            {
                this.確認效期庫存();
                return list_Inventory;
            }
            set => list_Inventory = value;
        }
        public List<string> List_Lot_number
        {
            get
            {
                this.確認效期庫存();
                return list_Lot_number;
            }
            set => list_Lot_number = value;
        }
        protected List<string> list_Validity_period = new List<string>();
        protected List<string> list_Lot_number = new List<string>();
        protected List<string> list_Inventory = new List<string>();
        private object _lock = new object();
        public void 確認效期庫存()
        {
            this.確認效期庫存(false);
        }
        public void 確認效期庫存(bool ClearAll)
        {
            lock(_lock)
            {
                List<string> 效期_temp = new List<string>();
                List<string> 庫存_temp = new List<string>();
                List<string> 批號_temp = new List<string>();
                if (this.list_Validity_period == null) this.list_Validity_period = new List<string>();
                if (this.list_Inventory == null) this.list_Inventory = new List<string>();
                if (this.list_Lot_number == null) this.list_Lot_number = new List<string>();

                while (true)
                {
                    bool flag_break = true;
                    if (this.list_Validity_period.Count > this.list_Inventory.Count)
                    {
                        this.list_Inventory.Add("0");
                        flag_break = false;
                    }
                    if (this.list_Validity_period.Count > this.list_Lot_number.Count)
                    {
                        this.list_Lot_number.Add("None");
                        flag_break = false;
                    }
                    if (flag_break) break;
                }

                double 庫存 = 0;
                for (int i = 0; i < this.list_Validity_period.Count; i++)
                {
                    庫存 = this.list_Inventory[i].StringToDouble();
                    if (庫存 > 0 || (this.list_Inventory[i] == "00" && !ClearAll))
                    {
                        效期_temp.Add(this.list_Validity_period[i].ToDateString("/"));
                        批號_temp.Add(this.list_Lot_number[i]);
                        庫存_temp.Add(this.list_Inventory[i]);
                    }
                }


                this.list_Validity_period = 效期_temp;
                this.list_Lot_number = 批號_temp;
                this.list_Inventory = 庫存_temp;

                List<object[]> list_value = new List<object[]>();
                for (int i = 0; i < this.list_Validity_period.Count; i++)
                {
                    list_value.Add(new object[] { list_Validity_period[i], list_Lot_number[i], list_Inventory[i] });
                }
                list_value.Sort(new DateComparerby());
                效期_temp.Clear();
                批號_temp.Clear();
                庫存_temp.Clear();
                for (int i = 0; i < list_value.Count; i++)
                {
                    效期_temp.Add(list_value[i][0].ObjectToString());
                    批號_temp.Add(list_value[i][1].ObjectToString());
                    庫存_temp.Add(list_value[i][2].ObjectToString());
                }
                this.list_Validity_period = 效期_temp;
                this.list_Lot_number = 批號_temp;
                this.list_Inventory = 庫存_temp;
            }
          

        }
        public double 取得庫存(string 效期)
        {
            if (!效期.Check_Date_String()) return -1;
            效期 = 效期.StringToDateTime().ToDateString("/");
            if (this.List_Validity_period == null) this.List_Validity_period = new List<string>();
            if (this.List_Inventory == null) this.List_Inventory = new List<string>();
            if (this.List_Lot_number == null) this.List_Lot_number = new List<string>();
            for (int i = 0; i < this.List_Validity_period.Count; i++)
            {
                if (this.List_Validity_period[i].ToDateString("/") == 效期.ToDateString("/"))
                {
                    return this.List_Inventory[i].StringToDouble();
                }
            }
            return -1;
        }
        public double 取得庫存()
        {
            double 庫存 = 0;
            double temp = 0;
            if (this.List_Validity_period == null) this.List_Validity_period = new List<string>();
            if (this.List_Inventory == null) this.List_Inventory = new List<string>();
            if (this.List_Lot_number == null) this.List_Lot_number = new List<string>();
            for (int i = 0; i < this.List_Validity_period.Count; i++)
            {
                temp = this.取得庫存(this.List_Validity_period[i]);
                if (temp > 0) 庫存 += temp;
            }
            return 庫存;
        }
    }
    [Serializable]
    public class DeviceBasic : DeviceSimple
    {
        [Serializable]
        public class LightStateClass
        {
            public bool State = false;
            public double Interval = 0;
            public double LightOffTime = 0;
            public Color LightColor = Color.Black;
            public DateTime LightingDateTime = DateTime.Now;
            public bool IsLightOn = false;

        }
        private class CachedBitmapInfo
        {
            public Bitmap Original;
            public Bitmap Dithered;
            public bool IsDithered;
        }

        private static Dictionary<string, CachedBitmapInfo> bitmapCache = new Dictionary<string, CachedBitmapInfo>();

        public string serverIP = "";
        public uint serverPort = 0;
        public string dbName = "";

        public DeviceType DeviceType
        {
            get => deviceType;
            set
            {
                if (deviceType != value) flag_replace = true;
                 deviceType = value;
            }
        }
        private DeviceType deviceType = DeviceType.None;

        private string _ChineseName = "";
        public string ChineseName
        {
            get => _ChineseName;
            set
            {
                if (_ChineseName != value) flag_replace = true;
                _ChineseName = value;
            }
        }
        private string _Scientific_Name = "";
        public string Scientific_Name { get => _Scientific_Name; set => _Scientific_Name = value; }
        private string _BarCode = "";
        public string BarCode
        {
            get
            {
                if (_BarCode.StringIsEmpty()) return Code;
                return _BarCode;
            }
            set
            {
                if (_BarCode != value) flag_replace = true;
                _BarCode = value;
            }
        }
        private string _BarCode1 = "";
        public string BarCode1
        {
            get => _BarCode1;
            set
            {
                if (_BarCode1 != value) flag_replace = true;
                _BarCode1 = value;
            }
        }
        private string _BarCode2 = "";
        public string BarCode2 
        {
            get => _BarCode2;
            set
            {
                if (_BarCode2 != value) flag_replace = true;
                _BarCode2 = value;
            }
        }
        private string _Package = "";
        public string Package
        {
            get => _Package;
            set
            {
                if (_Package != value) flag_replace = true;
                _Package = value;
            }
        }

        private string _CustomText1 = "";
        public string CustomText1 
        {
            get => _CustomText1; 
            set => _CustomText1 = value;
        }
        private string _CustomText2 = "";
        public string CustomText2 { get => _CustomText2; set => _CustomText2 = value; }
        private string _CustomText3 = "";
        public string CustomText3 { get => _CustomText3; set => _CustomText3 = value; }

        private int max_shipping = 1;
        public int Max_shipping
        {
            get => max_shipping;
            set
            {
                if (max_shipping != value) flag_replace = true;
                max_shipping = value;
            }
        }
        private double max_Inventory = 0;
        public double Max_Inventory
        { 
            get => max_Inventory;
            set
            {
                if (max_Inventory != value) flag_replace = true;
                max_Inventory = value;
            }
        }
        private string _StorageName = "";
        public string StorageName
        {
            get => _StorageName;
            set
            {
                if (_StorageName != value) flag_replace = true;
                _StorageName = value;
            }
        }
 
        private bool _isWarning = false;
        public bool IsWarning
        {
            get => _isWarning;
            set
            {
                if (_isWarning != value) flag_replace = true;
                _isWarning = value;
            }
        }
        private string dRUGKIND = "";
        public string DRUGKIND { get => dRUGKIND; set => dRUGKIND = value; }
        private bool isAnesthetic = false;
        public bool IsAnesthetic { get => isAnesthetic; set => isAnesthetic = value; }
        private bool isShapeSimilar = false;
        public bool IsShapeSimilar { get => isShapeSimilar; set => isShapeSimilar = value; }
        private bool isSoundSimilar = false;
        public bool IsSoundSimilar { get => isSoundSimilar; set => isSoundSimilar = value; }


        private string _Min_Package_Num = "";
        public string Min_Package_Num
        {
            get => _Min_Package_Num;
            set
            {
                if (_Min_Package_Num != value) flag_replace = true;
                _Min_Package_Num = value;
            }
        }


        private string speaker = "";
        public string Speaker { get => speaker; set => speaker = value; }

        private string area = "";
        public string Area { get => area; set => area = value; }

        public LightStateClass LightState = new LightStateClass();


        public void SetLight(bool state , Color color , double intervlal , double light_off_time)
        {
            LightState.State = state;
            if (color != Color.Transparent) LightState.LightColor = color;
            if (intervlal > 0 || intervlal == -1) LightState.Interval = intervlal;
            if (light_off_time > 0 || light_off_time == -1) LightState.LightOffTime = light_off_time;
            LightState.LightingDateTime = DateTime.Now;
        }


        [JsonIgnore]   
        public List<StockClass> stockClasses
        {
            get
            {
                List<StockClass> temp = new List<StockClass>();
                for (int i = 0; i < list_Validity_period.Count; i++)
                {
                    StockClass stockClass = new StockClass();
                    stockClass.Code = this.Code;
                    stockClass.Validity_period = list_Validity_period[i];
                    stockClass.Lot_number = list_Lot_number[i];
                    stockClass.Qty = list_Inventory[i];
                    temp.Add(stockClass);
                }
                return temp;
            }
            set
            {

            }
        }

     

        public void 效期庫存異動(string 效期, double 異動量)
        {
            this.效期庫存異動(效期, 異動量.ToString());
        }
        public void 效期庫存異動(string 效期, string 異動量)
        {
            if (this.List_Validity_period == null) this.List_Validity_period = new List<string>();
            if (this.List_Inventory == null) this.List_Inventory = new List<string>();
            if (this.List_Lot_number == null) this.List_Lot_number = new List<string>();
            if (!效期.Check_Date_String()) return;
            效期 = 效期.StringToDateTime().ToDateString("/");
            string 批號 = "";
            for (int i = 0; i < this.List_Validity_period.Count; i++)
            {
                if (this.List_Validity_period[i].ToDateString("/") == 效期.ToDateString("/"))
                {
                    批號 = this.List_Lot_number[i];
                }
            }

            this.效期庫存異動(效期, 批號, 異動量, true);
        }
        public void 效期庫存異動(string 效期, string 異動量, bool 清除無庫存效期)
        {
            this.效期庫存異動(效期, "", 異動量, 清除無庫存效期);
        }
        public void 效期庫存異動(string 效期, string 批號, string 異動量)
        {
            this.效期庫存異動(效期, 批號, 異動量, true);
        }
        public void 效期庫存異動(string 效期, string 批號, string 異動量, bool 清除無庫存效期)
        {
            if (異動量.StringToDouble() == 0) return;
            if (!效期.Check_Date_String()) return;
            效期 = 效期.StringToDateTime().ToDateString("/");
            if (this.List_Validity_period == null) this.List_Validity_period = new List<string>();
            if (this.List_Inventory == null) this.List_Inventory = new List<string>();
            if (this.List_Lot_number == null) this.List_Lot_number = new List<string>();
            for (int i = 0; i < this.List_Validity_period.Count; i++)
            {
                if (this.List_Validity_period[i].ToDateString("/") == 效期.ToDateString("/"))
                {
                    if (批號.StringIsEmpty() == false) this.List_Lot_number[i] = 批號;
                    double 現有庫存 = this.List_Inventory[i].StringToDouble();
                    double 異動庫存 = 異動量.StringToDouble();
                    this.List_Inventory[i] = (現有庫存 + 異動庫存).ToString();
                    this.確認效期庫存();
                    return;
                }
            }
            this.list_Validity_period.Add(效期);
            this.list_Lot_number.Add(批號);
            this.list_Inventory.Add(異動量);
            this.確認效期庫存();
        }
        public void 效期庫存覆蓋(string 效期, string 異動量)
        {
            if (this.List_Validity_period == null) this.List_Validity_period = new List<string>();
            if (this.List_Inventory == null) this.List_Inventory = new List<string>();
            if (this.List_Lot_number == null) this.List_Lot_number = new List<string>();
            if (!效期.Check_Date_String()) return;
            效期 = 效期.StringToDateTime().ToDateString("/");
            string 批號 = "";
            for (int i = 0; i < this.List_Validity_period.Count; i++)
            {
                if (this.List_Validity_period[i].ToDateString("/") == 效期.ToDateString("/"))
                {
                    批號 = this.List_Lot_number[i];
                }
            }
            this.效期庫存覆蓋(效期, 批號, 異動量, true);
        }
        public void 效期庫存覆蓋(string 效期, string 異動量, bool 清除無庫存效期)
        {
            this.效期庫存覆蓋(效期, "", 異動量, 清除無庫存效期);
        }
        public void 效期庫存覆蓋(string 效期, string 批號, string 異動量)
        {
            this.效期庫存覆蓋(效期, 批號, 異動量, true);
        }
        public void 效期庫存覆蓋(string 效期, string 批號, string 異動量, bool 清除無庫存效期)
        {
            if (異動量.StringToDouble() < 0) return;
            if (!效期.Check_Date_String()) return;
            效期 = 效期.StringToDateTime().ToDateString("/");
            if (this.List_Validity_period == null) this.List_Validity_period = new List<string>();
            if (this.List_Inventory == null) this.List_Inventory = new List<string>();
            if (this.List_Lot_number == null) this.List_Lot_number = new List<string>();
            for (int i = 0; i < this.List_Validity_period.Count; i++)
            {
                if (this.List_Validity_period[i].ToDateString("/") == 效期.ToDateString("/"))
                {
                    double 異動庫存 = 異動量.StringToDouble();
                    this.List_Inventory[i] = (異動庫存).ToString();
                    this.list_Lot_number[i] = 批號;
                    this.確認效期庫存();
                    return;
                }
            }
            this.list_Validity_period.Add(效期);
            this.list_Lot_number.Add(批號);
            this.list_Inventory.Add(異動量);
            this.確認效期庫存();
        }

        public void 新增效期(string 效期, string 庫存)
        {
            this.新增效期(效期, 效期, 庫存);
        }
        public void 新增效期(string 效期, string 批號, string 庫存)
        {
            if (庫存.StringToDouble() == -1 && 庫存 != "00") return;
            if (庫存.StringToDouble() < 0 && 庫存 != "00") return;
            if (!效期.Check_Date_String()) return;
            效期 = 效期.StringToDateTime().ToDateString("/");
            if (this.List_Validity_period == null) this.List_Validity_period = new List<string>();
            if (this.List_Inventory == null) this.List_Inventory = new List<string>();
            if (this.List_Lot_number == null) this.List_Lot_number = new List<string>();
            for (int i = 0; i < this.List_Validity_period.Count; i++)
            {
                if (this.List_Validity_period[i].ToDateString("/") == 效期.ToDateString("/"))
                {
                    this.List_Lot_number[i] = 批號;
                    this.List_Inventory[i] = 庫存;
                    return;
                }
            }
            this.list_Validity_period.Add(效期);
            this.list_Lot_number.Add(批號);
            this.list_Inventory.Add(庫存);
        }
        public void 清除效期(string 效期)
        {
            if (!效期.Check_Date_String()) return;
            效期 = 效期.StringToDateTime().ToDateString("/");
            if (this.List_Validity_period == null) this.List_Validity_period = new List<string>();
            if (this.List_Inventory == null) this.List_Inventory = new List<string>();
            if (this.List_Lot_number == null) this.List_Lot_number = new List<string>();
            for (int i = 0; i < this.List_Validity_period.Count; i++)
            {
                if (this.List_Validity_period[i] == 效期)
                {
                    this.list_Inventory.RemoveAt(i);
                    this.list_Validity_period.RemoveAt(i);
                    this.list_Lot_number.RemoveAt(i);
                    return;
                }
            }
        }

        public double 庫存異動(double 總異動量, out List<string> 效期, out List<string> 批號, out List<string> 異動量)
        {
            return 庫存異動(總異動量.ToString(), out 效期, out 批號, out 異動量, true);
        }
        public double 庫存異動(double 總異動量, out List<string> 效期, out List<string> 異動量)
        {
            return this.庫存異動(總異動量.ToString(), out 效期, out 異動量, true);
        }
        public double 庫存異動(string 總異動量, out List<string> 效期, out List<string> 異動量, bool 要寫入)
        {
            List<string> 批號 = new List<string>();
            return 庫存異動(總異動量, out 效期, out 批號, out 異動量, 要寫入);
        }
        public double 庫存異動(string 總異動量, out List<string> 效期, out List<string> 批號, out List<string> 異動量, bool 要寫入)
        {
            效期 = new List<string>();
            批號 = new List<string>();
            異動量 = new List<string>();
            if (總異動量.StringIsEmpty()) return -1;
            if (總異動量.StringToDouble() == 0) return -1;
            double int_總異動量 = 總異動量.StringToDouble();
            double 總庫存 = 取得庫存();
            double 剩餘庫存數量 = 0;
            double 效期庫存 = 0;
            if (this.List_Validity_period == null) this.List_Validity_period = new List<string>();
            if (this.List_Inventory == null) this.List_Inventory = new List<string>();
            this.List_Validity_period = this.List_Validity_period.OrderBy(r => DateTime.Parse(r.ToDateString())).ToList();
            for (int i = 0; i < this.List_Validity_period.Count; i++)
            {
                if (總庫存 > 0)
                {
                    效期庫存 = 取得庫存(this.List_Validity_period[i]);
                    剩餘庫存數量 = 效期庫存 + int_總異動量;
                    if (剩餘庫存數量 >= 0)
                    {
                        效期.Add(this.List_Validity_period[i]);
                        批號.Add(this.List_Lot_number[i]);
                        異動量.Add(int_總異動量.ToString());
                        break;
                    }
                    else
                    {
                        效期.Add(this.List_Validity_period[i]);
                        批號.Add(this.List_Lot_number[i]);
                        異動量.Add((效期庫存 * -1).ToString());

                        int_總異動量 = int_總異動量 + (效期庫存);
                    }
                }
            }
            if (要寫入)
            {
                for (int i = 0; i < 效期.Count; i++)
                {
                    this.效期庫存異動(效期[i], 異動量[i]);
                }
            }

            int_總異動量 = 0;
            for (int i = 0; i < 效期.Count; i++)
            {
                int_總異動量 += 異動量[i].StringToDouble();
            }
            return int_總異動量;


        }

        public List<StockClass> 庫存異動(double 總異動量)
        {
            return 庫存異動(總異動量, true);
        }
        public List<StockClass> 庫存異動(string 總異動量)
        {
            return 庫存異動(總異動量.StringToDouble(), true);
        }
        public List<StockClass> 庫存異動(double 總異動量, bool 要寫入)
        {

            List<StockClass> stockClasses = new List<StockClass>();
            double int_總異動量 = 總異動量;
            double 總庫存 = 取得庫存();
            double 剩餘庫存數量 = 0;
            double 效期庫存 = 0;
            if (this.List_Validity_period == null) this.List_Validity_period = new List<string>();
            if (this.List_Inventory == null) this.List_Inventory = new List<string>();
            this.List_Validity_period = this.List_Validity_period.OrderBy(r => DateTime.Parse(r.ToDateString())).ToList();
            for (int i = 0; i < this.List_Validity_period.Count; i++)
            {
                StockClass stockClass = new StockClass();
                if (總庫存 > 0)
                {
                    效期庫存 = 取得庫存(this.List_Validity_period[i]);
                    剩餘庫存數量 = 效期庫存 + int_總異動量;
                    if (剩餘庫存數量 >= 0)
                    {
                        stockClass.Validity_period = this.List_Validity_period[i];
                        stockClass.Lot_number = this.List_Lot_number[i];
                        stockClass.Qty = int_總異動量.ToString();
                        stockClasses.Add(stockClass);
                        break;
                    }
                    else
                    {
                        stockClass.Validity_period = this.List_Validity_period[i];
                        stockClass.Lot_number = this.List_Lot_number[i];
                        stockClass.Qty = int_總異動量.ToString();
                        stockClasses.Add(stockClass);
                        int_總異動量 = int_總異動量 + (效期庫存);
                    }
                }
            }
            if (要寫入)
            {
                for (int i = 0; i < stockClasses.Count; i++)
                {
                    this.效期庫存異動(stockClasses[i].Validity_period, stockClasses[i].Qty.StringToDouble());
                }
            }
            return stockClasses;

        }


        public string 取得批號(string 效期)
        {
            if (!效期.Check_Date_String()) return "";
            效期 = 效期.StringToDateTime().ToDateString("/");
            if (this.List_Validity_period == null) this.List_Validity_period = new List<string>();
            if (this.List_Inventory == null) this.List_Inventory = new List<string>();
            if (this.List_Lot_number == null) this.List_Lot_number = new List<string>();
            for (int i = 0; i < this.List_Validity_period.Count; i++)
            {
                if (this.List_Validity_period[i].ToDateString("/") == 效期.ToDateString("/"))
                {
                    return this.List_Lot_number[i];
                }
            }
            return "";
        }
        public void 修正批號(string 效期, string 批號)
        {
            if (!效期.Check_Date_String()) return;
            效期 = 效期.StringToDateTime().ToDateString("/");
            if (this.List_Validity_period == null) this.List_Validity_period = new List<string>();
            if (this.List_Inventory == null) this.List_Inventory = new List<string>();
            if (this.List_Lot_number == null) this.List_Lot_number = new List<string>();
            for (int i = 0; i < this.List_Validity_period.Count; i++)
            {
                if (this.List_Validity_period[i].ToDateString("/") == 效期.ToDateString("/"))
                {
                    this.List_Lot_number[i] = 批號;
                }
            }
            return;
        }
  
        public void 清除所有庫存資料()
        {
            list_Validity_period.Clear();
            list_Lot_number.Clear();
            list_Inventory.Clear();
        }


        public static void SetBitmapToCache(string drugCode, Bitmap bmp)
        {
            if (string.IsNullOrWhiteSpace(drugCode) || bmp == null) return;

            if (bitmapCache.ContainsKey(drugCode))
            {
                bitmapCache[drugCode].Original?.Dispose();
                bitmapCache[drugCode].Dithered?.Dispose();
                bitmapCache[drugCode] = new CachedBitmapInfo
                {
                    Original = bmp,
                    Dithered = null,
                    IsDithered = false
                };
            }
            else
            {
                bitmapCache.Add(drugCode, new CachedBitmapInfo
                {
                    Original = bmp,
                    Dithered = null,
                    IsDithered = false
                });
            }
        }

        public static Bitmap GetDitheredBitmapFromCache(string drugCode)
        {
            if (!bitmapCache.ContainsKey(drugCode)) return null;

            var info = bitmapCache[drugCode];

            if (!info.IsDithered)
            {
                info.Dithered = info.Original?.ApplyFloydSteinbergDitheringSixColor();
                info.IsDithered = true;
            }

            return info.Dithered;
        }
        public static void ClearBitmapCache()
        {
            foreach (var entry in bitmapCache.Values)
            {
                entry.Original?.Dispose();
                entry.Dithered?.Dispose();
            }
            bitmapCache.Clear();
        }

        public static bool ContainsBitmap(string drugCode)
        {
            return bitmapCache.ContainsKey(drugCode);
        }

    }



    static public class DeviceMethod
    {
        static public void SQL_Init(SQLUI.SQL_DataGridView.ConnentionClass connentionClass , string TableName)
        {
            connentionClass.TableName = TableName;
            SQL_Init(connentionClass);
        }
        static public void SQL_Init(SQLUI.SQL_DataGridView.ConnentionClass connentionClass)
        {
            SQLUI.SQLControl sQLControl = new SQLUI.SQLControl(connentionClass.IP, connentionClass.DataBaseName, connentionClass.TableName, connentionClass.UserName, connentionClass.Password, connentionClass.Port, connentionClass.MySqlSslMode);
            if(!sQLControl.IsTableCreat(null))
            {
                SQLUI.Table table = new SQLUI.Table(connentionClass.TableName);
                table.AddColumnList(enum_DeviceTable.GUID.GetEnumName(), SQLUI.Table.StringType.VARCHAR, SQLUI.Table.IndexType.PRIMARY);
                table.AddColumnList(enum_DeviceTable.IP.GetEnumName(), SQLUI.Table.StringType.VARCHAR, SQLUI.Table.IndexType.None);
                table.AddColumnList(enum_DeviceTable.Port.GetEnumName(), SQLUI.Table.StringType.VARCHAR, SQLUI.Table.IndexType.None);
                table.AddColumnList(enum_DeviceTable.Value.GetEnumName(), SQLUI.Table.StringType.MEDIUMTEXT, SQLUI.Table.IndexType.None);
                sQLControl.CreatTable(table);
            }
            sQLControl = null;
        }

        static public List<Device> SQL_GetAllDevice(SQLUI.SQL_DataGridView.ConnentionClass connentionClass, string TableName)
        {
            SQLUI.SQLControl sQLControl = new SQLUI.SQLControl(connentionClass.IP, connentionClass.DataBaseName, connentionClass.TableName, connentionClass.UserName, connentionClass.Password, connentionClass.Port, connentionClass.MySqlSslMode);

            return SQL_GetAllDevice(sQLControl);
        }
        static public List<Device> SQL_GetAllDevice(SQLUI.SQLControl sQLControl)
        {
            List<object[]> deviceTables = sQLControl.GetAllRows(null);
            List<Device> devices = SQL_GetAllDevice(deviceTables);
            return devices;
        }
     
        static public void SQL_ReplaceDevice(SQLUI.SQL_DataGridView.ConnentionClass connentionClass, string TableName, List<Device> devices)
        {
            SQLUI.SQLControl sQLControl = new SQLUI.SQLControl(connentionClass.IP, connentionClass.DataBaseName, connentionClass.TableName, connentionClass.UserName, connentionClass.Password, connentionClass.Port, connentionClass.MySqlSslMode);
            List<object[]> list_value = new List<object[]>();
            Parallel.ForEach(devices, value =>
            {
                object[] SQLvalue = new object[new enum_DeviceTable().GetLength()];
                SQLvalue[(int)enum_DeviceTable.GUID] = value.GUID;
                SQLvalue[(int)enum_DeviceTable.IP] = value.IP;
                SQLvalue[(int)enum_DeviceTable.Port] = value.Port;
                SQLvalue[(int)enum_DeviceTable.Value] = value.JsonSerializationt();
                list_value.LockAdd(SQLvalue);

            });
            sQLControl.UpdateByDefulteExtra(TableName ,list_value);
        }
        static public void SQL_AddDevice(SQLUI.SQL_DataGridView.ConnentionClass connentionClass, string TableName, List<Device> devices)
        {
            SQLUI.SQLControl sQLControl = new SQLUI.SQLControl(connentionClass.IP, connentionClass.DataBaseName, connentionClass.TableName, connentionClass.UserName, connentionClass.Password, connentionClass.Port, connentionClass.MySqlSslMode);
            List<object[]> list_value = new List<object[]>();
            Parallel.ForEach(devices, value =>
            {
                object[] SQLvalue = new object[new enum_DeviceTable().GetLength()];
                SQLvalue[(int)enum_DeviceTable.GUID] = value.GUID;
                SQLvalue[(int)enum_DeviceTable.IP] = value.IP;
                SQLvalue[(int)enum_DeviceTable.Port] = value.Port;
                SQLvalue[(int)enum_DeviceTable.Value] = value.JsonSerializationt();
                list_value.LockAdd(SQLvalue);

            });
            sQLControl.AddRows(TableName, list_value);
        }
    
        static public List<Device> SQL_GetAllDevice(List<object[]> deviceTables)
        {
            List<Device> devices = new List<Device>();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < deviceTables.Count; i++)
            {
                string jsonString = deviceTables[i][(int)enum_DeviceTable.Value].ObjectToString();
                if (i == 0) sb.Append("[");

                sb.Append($"{jsonString}");

                if (i != deviceTables.Count - 1)
                {
                    sb.Append($",");
                }
                if (i == deviceTables.Count - 1) sb.Append("]");
            }

            string json_result = sb.ToString();
            if (json_result.StringIsEmpty()) json_result = "[]";
            devices = json_result.JsonDeserializet<List<Device>>();
            return devices;
         
        }

        static public List<Device> Add_NewDevice(this List<Device> Devices, string IP, int Port)
        {
            return Devices.Add_NewDevice(IP, Port, new Device(IP, Port));
        }
        static public List<Device> Add_NewDevice(this List<Device> Devices, Device device)
        {
            for (int i = 0; i < Devices.Count; i++)
            {
                if (Devices[i].IP == device.IP)
                {
                    Devices[i] = device;
                    return Devices;
                }
            }
            Devices.Add(device);
            return Devices;
        }
        static public List<Device> Add_NewDevice(this List<Device> Devices, string IP, int Port, Device device)
        {
            device.IP = IP;
            device.Port = Port;
            if (Devices.SortByIP(IP) == null)
            {
                Devices.Add(device);
                return Devices;
            }
            else
            {
                Devices = Devices.ReplacePortByIP(IP, Port);
            }
            return Devices;
        }
        static public List<Device> SortByCode(this List<Device> Devices, string Code)
        {
            List<Device> devices = new List<Device>();
            foreach (Device device in Devices)
            {
                if (device.Code == Code)
                {
                    devices.Add(device);
                }
            }
            return devices;
        }
        static public List<Device> SortByBarCode(this List<Device> Devices, string BarCode)
        {
            List<Device> devices = new List<Device>();
            foreach (Device device in Devices)
            {
                if (device.BarCode == BarCode)
                {
                    devices.Add(device);
                }
            }
            return devices;
        }
        static public Device SortByIP(this List<Device> Devices, string IP)
        {
            for (int i = 0; i < Devices.Count; i++)
            {
                if (Devices[i].IP == IP) return Devices[i];
            }
            return null;
        }
        static public List<Device> RemoveByIP(this List<Device> Devices, string IP)
        {
            List<Device> Devices_buf = new List<Device>();
            foreach (Device device in Devices)
            {
                if (device.IP != IP)
                {
                    Devices_buf.Add(device);
                }
            }
            Devices = Devices_buf;
            return Devices;
        }
        static public List<Device> ReplacePortByIP(this List<Device> Devices, string IP, int Port)
        {
            for (int i = 0; i < Devices.Count; i++)
            {
                if (Devices[i].IP == IP)
                {
                    Devices[i].IP = IP;
                    Devices[i].Port = Port;
                }
            }
            return Devices;
        }
        static public List<Device> ReplaceByIP(this List<Device> Devices, Device device)
        {
            for (int i = 0; i < Devices.Count; i++)
            {
                if (Devices[i].IP == device.IP)
                {
                    Devices[i] = device;
                    return Devices;
                }
            }
            return Devices;
        }
        static public List<Device> ClearDevice(this List<Device> Devices, string IP)
        {
            Device device = Devices.SortByIP(IP);
            if (device != null)
            {
                ClearDevice(device);
            }
            return Devices;
        }
        static public void ClearDevice(this Device device)
        {
            device.Clear();
        }

        static public List<Device> GetAllDevice(this List<Device> Devices)
        {
            List<Device> devices = new List<Device>();
            for (int i = 0; i < Devices.Count; i++)
            {
                devices.Add(Devices[i]);
            }

            return devices;
        }

        static public void ReplaceIP(this Device device, string IP)
        {
            device.ReplaceIP(IP, device.Port);
        }
        static public void ReplaceIP(this Device device, string IP, int Port)
        {
            device.IP = IP;
            device.Port = Port;
        }

        static public UDP_Class GetUPDClass(this Device device)
        {
            if (device != null)
            {
                UDP_Class uDP_Class = new UDP_Class(device.IP, device.Port);
                return uDP_Class;
            }
            return null;
        }
        static public bool DeviceIsStorage(this Device device)
        {
            bool flag = (device.DeviceType == DeviceType.EPD266 || device.DeviceType == DeviceType.EPD266_lock
                        || device.DeviceType == DeviceType.EPD290 || device.DeviceType == DeviceType.EPD290_lock
                        || device.DeviceType == DeviceType.EPD420 || device.DeviceType == DeviceType.EPD420_lock
                        || device.DeviceType == DeviceType.EPD213 || device.DeviceType == DeviceType.EPD213_lock
                         || device.DeviceType == DeviceType.EPD360E || device.DeviceType == DeviceType.EPD360E_lock);
            return flag;
        }
        
        static public double GetInventory(this List<DeviceBasic> deviceBasics)
        {
            double 庫存 = 0;
            for (int i = 0; i < deviceBasics.Count; i++)
            {
                庫存 += deviceBasics[i].Inventory.StringToDouble();
            }

            return 庫存;
        }

        static public Device SetMedClass(this Device device, HIS_DB_Lib.medClass medClass)
        {
            device.Code = medClass.藥品碼;
            device.SKDIACODE = medClass.料號;
            device.Name = medClass.藥品名稱;
            device.Scientific_Name = medClass.藥品學名;
            device.ChineseName = medClass.中文名稱;
            device.DRUGKIND = medClass.管制級別;
            device.Package = medClass.包裝單位;

            return device;
        }   



    
    }
  

    public delegate void PaintHandler();
    public enum DeviceType
    {
        None = 0,
        EPD266_lock = 1,
        EPD266 = 2,
        EPD583_lock = 3,
        EPD583 = 4,   
        RowsLED = 5,
        RFID_Device = 6,
        Pannel35_lock = 7,
        Pannel35= 8,
        EPD290_lock = 9,
        EPD290 = 10,
        EPD1020 = 11,
        EPD1020_lock = 12,
        EPD420 = 13,
        EPD420_lock = 14,
        EPD730F_lock = 15,
        EPD730F = 16,
        EPD213_lock = 17,
        EPD420_D = 18,
        EPD420_D_lock = 19,
        EPD730E_lock = 20,
        EPD730E = 21,
        EPD360E_lock = 22,
        EPD360E = 23,
        EPD213 = 118,
    }
    public enum HorizontalAlignment
    {
        Left = 0,
        Right = 1,
        Center = 2
    }
    #region Serialization
    static public class ColorSerializationHelper
    {
        static public Color FromString(string value)
        {
            var parts = value.Split(':');

            int A = 0;
            int R = 0;
            int G = 0;
            int B = 0;
            int.TryParse(parts[0], out A);
            int.TryParse(parts[1], out R);
            int.TryParse(parts[2], out G);
            int.TryParse(parts[3], out B);
            return Color.FromArgb(A, R, G, B);
        }
        static public string ToString(Color color)
        {
            return color.A + ":" + color.R + ":" + color.G + ":" + color.B;

        }
    }
    [TypeConverter(typeof(FontConverter))]
    static public class FontSerializationHelper
    {
        static public Font FromString(string value)
        {
            var parts = value.Split(':');
            return new Font(
                parts[0],                                                   // FontFamily.Name
                float.Parse(parts[1]),                                      // Size
                EnumSerializationHelper.FromString<FontStyle>(parts[2]),    // Style
                EnumSerializationHelper.FromString<GraphicsUnit>(parts[3]), // Unit
                byte.Parse(parts[4]),                                       // GdiCharSet
                bool.Parse(parts[5])                                        // GdiVerticalFont
            );
        }
        static public string ToString(Font font)
        {
            return font.FontFamily.Name
                    + ":" + font.Size
                    + ":" + font.Style
                    + ":" + font.Unit
                    + ":" + font.GdiCharSet
                    + ":" + font.GdiVerticalFont
                    ;
        }
    }
    [TypeConverter(typeof(EnumConverter))]
    static public class EnumSerializationHelper
    {
        static public T FromString<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
    }
    #endregion
    [Serializable]
    public class Device :DeviceBasic
    {
        public bool input = false;

        public Size PanelSize
        {
            get
            {
                if (DeviceType == DeviceType.EPD213 || DeviceType == DeviceType.EPD213_lock)
                {
                    return new Size(250, 122);
                }
                if (DeviceType == DeviceType.EPD266 || DeviceType == DeviceType.EPD266_lock)
                {
                    return new Size(296, 152);
                }
                if (DeviceType == DeviceType.EPD290 || DeviceType == DeviceType.EPD290_lock)
                {
                    return new Size(296, 128);
                }
                if (DeviceType.GetEnumName().Contains("EPD420"))
                {
                    return new Size(400, 300);
                }
                if (DeviceType.GetEnumName().Contains("EPD360E"))
                {
                    return new Size(600, 400);
                }
                if (DeviceType == DeviceType.Pannel35 || DeviceType == DeviceType.Pannel35_lock)
                {
                    return new Size(360, 240);
                }
                return new Size(0, 0);
            }
        }

        public enum ValueName
        {
            藥品碼,
            藥品名稱,
            藥品中文名稱,
            藥品學名,
            廠牌,
            效期,
            庫存,
            包裝單位,
            最小包裝單位,
            最小包裝單位數量,
            BarCode,
            CustomText1,
            CustomText2,
            CustomText3,
            圖片1,
            圖片2,
            儲位名稱,
            IP,
            Port,
            最大存量,
            None,
        }
        public enum ValueType
        {
            StringValue,
            Title,
            Value,
            Font,
            ForeColor,
            BackColor,
            Position,
            Width,
            Height,
            HorizontalAlignment,
            BorderSize,
            BorderColor,
            Visable,
            BorderRadius,

        }
        public class VlaueClass
        {
            public string StringValue
            {
                get
                {
                    if(valueName == ValueName.效期)
                    {
                        return this.Value;
                    }
                    if (valueName == ValueName.藥品碼)
                    {
                        return this.Value;
                    }
                    if (this.Title.StringIsEmpty()) this.Title = this.valueName.GetEnumName();
                    if (this.Title == "None")
                    {
                        return string.Format("{0}", this.Value);
                    }
                    return string.Format("{0}:{1}", this.Title, this.Value);
                }
            }
            public ValueName valueName = new ValueName();
            public string Title = "";
            public string Value = "";
            public Font Font = new Font("標楷體", 14);
            public Color ForeColor = Color.Black;
            public Color BackColor = Color.White;
            public Point Position = new Point();
            public int Width = 0;
            public int Height = 0;
            public HorizontalAlignment HorizontalAlignment = HorizontalAlignment.Left;
            public int BorderSize = 0;
            public int BorderRadius = 0;
            public Color BorderColor = Color.Black;
            public bool Visable = false;
            public bool flag_breathing = false;
            public object Getvalue(ValueType valueType)
            {
                if(valueType == ValueType.StringValue)
                {
                    return StringValue;
                }
                if (valueType == ValueType.Title)
                {
                    return Title;
                }
                if (valueType == ValueType.Value)
                {
                    return Value;
                }
                if (valueType == ValueType.Font)
                {
                    return Font;
                }
                if (valueType == ValueType.ForeColor)
                {
                    return ForeColor;
                }
                if (valueType == ValueType.BackColor)
                {
                    return BackColor;
                }
                if (valueType == ValueType.Position)
                {
                    return Position;
                }
                if (valueType == ValueType.Width)
                {
                    return Width;
                }
                if (valueType == ValueType.Height)
                {
                    return Height;
                }
                if (valueType == ValueType.HorizontalAlignment)
                {
                    return HorizontalAlignment;
                }
                if (valueType == ValueType.BorderSize)
                {
                    return BorderSize;
                }
                if (valueType == ValueType.BorderColor)
                {
                    return BorderColor;
                }
                if (valueType == ValueType.Visable)
                {
                    return Visable;
                }
                return null;
            }

        }
        private DateTime Default_Validity_period = new DateTime(2020, 01, 01);
        public Device(int station)
        {
            this.station = station;
            this.GUID = Guid.NewGuid().ToString();
        }
        public Device(string IP, int Port)
        {
            this.IP = IP;
            this.Port = Port;
            this.GUID = Guid.NewGuid().ToString();
        }
        public Device()
        {
            this.GUID = Guid.NewGuid().ToString();
        }
        public void SetValue(VlaueClass vlaueClass)
        {
            for (int i = 0; i < new ValueType().GetLength(); i++)
            {
                this.SetValue(vlaueClass.valueName, (ValueType)i, vlaueClass.Getvalue((ValueType)i));
            }
        }
        public void SetValue(ValueName valueName, ValueType valueType, object Value)
        {

            switch (valueName)
            {
                case ValueName.藥品名稱:
                    {
                        if (valueType == ValueType.Value)
                        {
                            if (Value is string) this.Name = (string)Value;
                        }
                        else if (valueType == ValueType.Title)
                        {
                            if (Value is string) this.Name_Title = (string)Value;
                        }
                        else if (valueType == ValueType.Font)
                        {
                            if (Value is Font) this.Name_font = (Font)Value;
                        }
                        else if (valueType == ValueType.ForeColor)
                        {
                            if (Value is Color) this.Name_ForeColor = (Color)Value;
                        }
                        else if (valueType == ValueType.BackColor)
                        {
                            if (Value is Color) this.Name_BackColor = (Color)Value;
                        }
                        else if (valueType == ValueType.Position)
                        {
                            if (Value is Point) this.Name_Position = (Point)Value;
                        }
                        else if (valueType == ValueType.Width)
                        {
                            if (Value is int) this.Name_Width = (int)Value;
                        }
                        else if (valueType == ValueType.Height)
                        {
                            if (Value is int) this.Name_Height = (int)Value;
                        }
                        else if (valueType == ValueType.BorderSize)
                        {
                            if (Value is int) this.Name_BorderSize = (int)Value;
                        }
                        else if (valueType == ValueType.BorderRadius)
                        {
                            if (Value is int) this.Name_BorderRadius = (int)Value;
                        }
                        else if (valueType == ValueType.BorderColor)
                        {
                            if (Value is Color) this.Name_BorderColor = (Color)Value;
                        }
                        else if (valueType == ValueType.HorizontalAlignment)
                        {
                            if (Value is HorizontalAlignment) this.Name_HorizontalAlignment = (HorizontalAlignment)Value;
                        }
                        else if (valueType == ValueType.Visable)
                        {
                            if (Value is bool) this.Name_Visable = (bool)Value;
                        }

                        break;
                    }
                case ValueName.藥品中文名稱:
                    {

                        if (valueType == ValueType.Value)
                        {
                            if (Value is string) this.ChineseName = (string)Value;
                        }
                        else if (valueType == ValueType.Title)
                        {
                            if (Value is string) this.ChineseName_Title = (string)Value;
                        }
                        else if (valueType == ValueType.Font)
                        {
                            if (Value is Font) this.ChineseName_font = (Font)Value;
                        }
                        else if (valueType == ValueType.ForeColor)
                        {
                            if (Value is Color) this.ChineseName_ForeColor = (Color)Value;
                        }
                        else if (valueType == ValueType.BackColor)
                        {
                            if (Value is Color) this.ChineseName_BackColor = (Color)Value;
                        }
                        else if (valueType == ValueType.Position)
                        {
                            if (Value is Point) this.ChineseName_Position = (Point)Value;
                        }
                        else if (valueType == ValueType.Width)
                        {
                            if (Value is int) this.ChineseName_Width = (int)Value;
                        }
                        else if (valueType == ValueType.Height)
                        {
                            if (Value is int) this.ChineseName_Height = (int)Value;
                        }
                        else if (valueType == ValueType.BorderSize)
                        {
                            if (Value is int) this.ChineseName_BorderSize = (int)Value;
                        }
                        else if (valueType == ValueType.BorderRadius)
                        {
                            if (Value is int) this.ChineseName_BorderRadius = (int)Value;
                        }
                        else if (valueType == ValueType.BorderColor)
                        {
                            if (Value is Color) this.ChineseName_BorderColor = (Color)Value;
                        }
                        else if (valueType == ValueType.HorizontalAlignment)
                        {
                            if (Value is HorizontalAlignment) this.ChineseName_HorizontalAlignment = (HorizontalAlignment)Value;
                        }
                        else if (valueType == ValueType.Visable)
                        {
                            if (Value is bool) this.ChineseName_Visable = (bool)Value;
                        }

                        break;
                    }
                case ValueName.包裝單位:
                    {

                        if (valueType == ValueType.Value)
                        {
                            if (Value is string) this.Package = (string)Value;
                        }
                        else if (valueType == ValueType.Title)
                        {
                            if (Value is string) this.Package_Title = (string)Value;
                        }
                        else if (valueType == ValueType.Font)
                        {
                            if (Value is Font) this.Package_font = (Font)Value;
                        }
                        else if (valueType == ValueType.ForeColor)
                        {
                            if (Value is Color) this.Package_ForeColor = (Color)Value;
                        }
                        else if (valueType == ValueType.BackColor)
                        {
                            if (Value is Color) this.Package_BackColor = (Color)Value;
                        }
                        else if (valueType == ValueType.Position)
                        {
                            if (Value is Point) this.Package_Position = (Point)Value;
                        }
                        else if (valueType == ValueType.Width)
                        {
                            if (Value is int) this.Package_Width = (int)Value;
                        }
                        else if (valueType == ValueType.Height)
                        {
                            if (Value is int) this.Package_Height = (int)Value;
                        }
                        else if (valueType == ValueType.BorderSize)
                        {
                            if (Value is int) this.Package_BorderSize = (int)Value;
                        }
                        else if (valueType == ValueType.BorderRadius)
                        {
                            if (Value is int) this.Package_BorderRadius = (int)Value;
                        }
                        else if (valueType == ValueType.BorderColor)
                        {
                            if (Value is Color) this.Package_BorderColor = (Color)Value;
                        }
                        else if (valueType == ValueType.HorizontalAlignment)
                        {
                            if (Value is HorizontalAlignment) this.Package_HorizontalAlignment = (HorizontalAlignment)Value;
                        }
                        else if (valueType == ValueType.Visable)
                        {
                            if (Value is bool) this.Package_Visable = (bool)Value;
                        }
                        break;
                    }
                case ValueName.藥品學名:
                    {

                        if (valueType == ValueType.Value)
                        {
                            if (Value is string) this.Scientific_Name = (string)Value;
                        }
                        else if (valueType == ValueType.Title)
                        {
                            if (Value is string) this.Scientific_Name_Title = (string)Value;
                        }
                        else if (valueType == ValueType.Font)
                        {
                            if (Value is Font) this.Scientific_Name_font = (Font)Value;
                        }
                        else if (valueType == ValueType.ForeColor)
                        {
                            if (Value is Color) this.Scientific_Name_ForeColor = (Color)Value;
                        }
                        else if (valueType == ValueType.BackColor)
                        {
                            if (Value is Color) this.Scientific_Name_BackColor = (Color)Value;
                        }
                        else if (valueType == ValueType.Position)
                        {
                            if (Value is Point) this.Scientific_Name_Position = (Point)Value;
                        }
                        else if (valueType == ValueType.Width)
                        {
                            if (Value is int) this.Scientific_Name_Width = (int)Value;
                        }
                        else if (valueType == ValueType.Height)
                        {
                            if (Value is int) this.Scientific_Name_Height = (int)Value;
                        }
                        else if (valueType == ValueType.BorderSize)
                        {
                            if (Value is int) this.Scientific_Name_BorderSize = (int)Value;
                        }
                        else if (valueType == ValueType.BorderRadius)
                        {
                            if (Value is int) this.Scientific_Name_BorderRadius = (int)Value;
                        }
                        else if (valueType == ValueType.BorderColor)
                        {
                            if (Value is Color) this.Scientific_Name_BorderColor = (Color)Value;
                        }
                        else if (valueType == ValueType.HorizontalAlignment)
                        {
                            if (Value is HorizontalAlignment) this.Scientific_Name_HorizontalAlignment = (HorizontalAlignment)Value;
                        }
                        else if (valueType == ValueType.Visable)
                        {
                            if (Value is bool) this.Scientific_Name_Visable = (bool)Value;
                        }
                        break;
                    }
                case ValueName.藥品碼:
                    {

                        if (valueType == ValueType.Value)
                        {
                            if (Value is string) this.Code = (string)Value;
                        }
                        else if (valueType == ValueType.Title)
                        {
                            if (Value is string) this.Code_Title = (string)Value;
                        }
                        else if (valueType == ValueType.Font)
                        {
                            if (Value is Font) this.Code_font = (Font)Value;
                        }
                        else if (valueType == ValueType.ForeColor)
                        {
                            if (Value is Color) this.Code_ForeColor = (Color)Value;
                        }
                        else if (valueType == ValueType.BackColor)
                        {
                            if (Value is Color) this.Code_BackColor = (Color)Value;
                        }
                        else if (valueType == ValueType.Position)
                        {
                            if (Value is Point) this.Code_Position = (Point)Value;
                        }
                        else if (valueType == ValueType.Width)
                        {
                            if (Value is int) this.Code_Width = (int)Value;
                        }
                        else if (valueType == ValueType.Height)
                        {
                            if (Value is int) this.Code_Height = (int)Value;
                        }
                        else if (valueType == ValueType.BorderSize)
                        {
                            if (Value is int) this.Code_BorderSize = (int)Value;
                        }
                        else if (valueType == ValueType.BorderRadius)
                        {
                            if (Value is int) this.Code_BorderRadius = (int)Value;
                        }
                        else if (valueType == ValueType.BorderColor)
                        {
                            if (Value is Color) this.Code_BorderColor = (Color)Value;
                        }
                        else if (valueType == ValueType.HorizontalAlignment)
                        {
                            if (Value is HorizontalAlignment) this.Code_HorizontalAlignment = (HorizontalAlignment)Value;
                        }
                        else if (valueType == ValueType.Visable)
                        {
                            if (Value is bool) this.Code_Visable = (bool)Value;
                        }
                        break;
                    }
                case ValueName.廠牌:
                    {

                        if (valueType == ValueType.Value)
                        {
                            if (Value is string) this.Label = (string)Value;
                        }
                        else if (valueType == ValueType.Title)
                        {
                            if (Value is string) this.Label_Title = (string)Value;
                        }
                        else if (valueType == ValueType.Font)
                        {
                            if (Value is Font) this.Label_font = (Font)Value;
                        }
                        else if (valueType == ValueType.ForeColor)
                        {
                            if (Value is Color) this.Label_ForeColor = (Color)Value;
                        }
                        else if (valueType == ValueType.BackColor)
                        {
                            if (Value is Color) this.Label_BackColor = (Color)Value;
                        }
                        else if (valueType == ValueType.Position)
                        {
                            if (Value is Point) this.Label_Position = (Point)Value;
                        }
                        else if (valueType == ValueType.Width)
                        {
                            if (Value is int) this.Label_Width = (int)Value;
                        }
                        else if (valueType == ValueType.Height)
                        {
                            if (Value is int) this.Label_Height = (int)Value;
                        }
                        else if (valueType == ValueType.BorderSize)
                        {
                            if (Value is int) this.Label_BorderSize = (int)Value;
                        }
                        else if (valueType == ValueType.BorderRadius)
                        {
                            if (Value is int) this.Label_BorderRadius = (int)Value;
                        }
                        else if (valueType == ValueType.BorderColor)
                        {
                            if (Value is Color) this.Label_BorderColor = (Color)Value;
                        }
                        else if (valueType == ValueType.HorizontalAlignment)
                        {
                            if (Value is HorizontalAlignment) this.Label_HorizontalAlignment = (HorizontalAlignment)Value;
                        }
                        else if (valueType == ValueType.Visable)
                        {
                            if (Value is bool) this.Label_Visable = (bool)Value;
                        }
                        break;
                    }
                case ValueName.效期:
                    {

                        if (valueType == ValueType.Value)
                        {
                            if (Value is string) this.Validity_period = (string)Value;
                        }
                        else if (valueType == ValueType.Title)
                        {
                            if (Value is string) this.Validity_period_Title = (string)Value;
                        }
                        else if (valueType == ValueType.Font)
                        {
                            if (Value is Font) this.Validity_period_font = (Font)Value;
                        }
                        else if (valueType == ValueType.ForeColor)
                        {
                            if (Value is Color) this.Validity_period_ForeColor = (Color)Value;
                        }
                        else if (valueType == ValueType.BackColor)
                        {
                            if (Value is Color) this.Validity_period_BackColor = (Color)Value;
                        }
                        else if (valueType == ValueType.Position)
                        {
                            if (Value is Point) this.Validity_period_Position = (Point)Value;
                        }
                        else if (valueType == ValueType.Width)
                        {
                            if (Value is int) this.Validity_period_Width = (int)Value;
                        }
                        else if (valueType == ValueType.Height)
                        {
                            if (Value is int) this.Validity_period_Height = (int)Value;
                        }
                        else if (valueType == ValueType.BorderSize)
                        {
                            if (Value is int) this.Validity_period_BorderSize = (int)Value;
                        }
                        else if (valueType == ValueType.BorderRadius)
                        {
                            if (Value is int) this.Validity_period_BorderRadius = (int)Value;
                        }
                        else if (valueType == ValueType.BorderColor)
                        {
                            if (Value is Color) this.Validity_period_BorderColor = (Color)Value;
                        }
                        else if (valueType == ValueType.HorizontalAlignment)
                        {
                            if (Value is HorizontalAlignment) this.Validity_period_HorizontalAlignment = (HorizontalAlignment)Value;
                        }
                        else if (valueType == ValueType.Visable)
                        {
                            if (Value is bool) this.Validity_period_Visable = (bool)Value;
                        }
                        break;
                    }
                case ValueName.儲位名稱:
                    {

                        if (valueType == ValueType.Value)
                        {
                            if (Value is string) this.StorageName = (string)Value;
                        }
                        else if (valueType == ValueType.Title)
                        {
                            if (Value is string) this.StorageName_Title = (string)Value;
                        }
                        else if (valueType == ValueType.Font)
                        {
                            if (Value is Font) this.StorageName_font = (Font)Value;
                        }
                        else if (valueType == ValueType.ForeColor)
                        {
                            if (Value is Color) this.StorageName_ForeColor = (Color)Value;
                        }
                        else if (valueType == ValueType.BackColor)
                        {
                            if (Value is Color) this.StorageName_BackColor = (Color)Value;
                        }
                        else if (valueType == ValueType.Position)
                        {
                            if (Value is Point) this.StorageName_Position = (Point)Value;
                        }
                        else if (valueType == ValueType.Width)
                        {
                            if (Value is int) this.StorageName_Width = (int)Value;
                        }
                        else if (valueType == ValueType.Height)
                        {
                            if (Value is int) this.StorageName_Height = (int)Value;
                        }
                        else if (valueType == ValueType.BorderSize)
                        {
                            if (Value is int) this.StorageName_BorderSize = (int)Value;
                        }
                        else if (valueType == ValueType.BorderRadius)
                        {
                            if (Value is int) this.StorageName_BorderRadius = (int)Value;
                        }
                        else if (valueType == ValueType.BorderColor)
                        {
                            if (Value is Color) this.StorageName_BorderColor = (Color)Value;
                        }
                        else if (valueType == ValueType.HorizontalAlignment)
                        {
                            if (Value is HorizontalAlignment) this.StorageName_HorizontalAlignment = (HorizontalAlignment)Value;
                        }
                        else if (valueType == ValueType.Visable)
                        {
                            if (Value is bool) this.StorageName_Visable = (bool)Value;
                        }
                        break;
                    }
                case ValueName.最小包裝單位:
                    {
                        if (valueType == ValueType.Value)
                        {
                            if (Value is string) this.MinPackage = (string)Value;
                        }
                        else if (valueType == ValueType.Title)
                        {
                            if (Value is string) this.MinPackage_Title = (string)Value;
                        }
                        else if (valueType == ValueType.Font)
                        {
                            if (Value is Font) this.MinPackage_font = (Font)Value;
                        }
                        else if (valueType == ValueType.ForeColor)
                        {
                            if (Value is Color) this.MinPackage_ForeColor = (Color)Value;
                        }
                        else if (valueType == ValueType.BackColor)
                        {
                            if (Value is Color) this.MinPackage_BackColor = (Color)Value;
                        }
                        else if (valueType == ValueType.Position)
                        {
                            if (Value is Point) this.MinPackage_Position = (Point)Value;
                        }
                        else if (valueType == ValueType.Width)
                        {
                            if (Value is int) this.MinPackage_Width = (int)Value;
                        }
                        else if (valueType == ValueType.Height)
                        {
                            if (Value is int) this.MinPackage_Height = (int)Value;
                        }
                        else if (valueType == ValueType.BorderSize)
                        {
                            if (Value is int) this.MinPackage_BorderSize = (int)Value;
                        }
                        else if (valueType == ValueType.BorderRadius)
                        {
                            if (Value is int) this.MinPackage_BorderRadius = (int)Value;
                        }
                        else if (valueType == ValueType.BorderColor)
                        {
                            if (Value is Color) this.MinPackage_BorderColor = (Color)Value;
                        }
                        else if (valueType == ValueType.HorizontalAlignment)
                        {
                            if (Value is HorizontalAlignment) this.MinPackage_HorizontalAlignment = (HorizontalAlignment)Value;
                        }
                        else if (valueType == ValueType.Visable)
                        {
                            if (Value is bool) this.MinPackage_Visable = (bool)Value;
                        }
                        break;
                    }
                case ValueName.最小包裝單位數量:
                    {
                        if (valueType == ValueType.Value)
                        {
                            if (Value is string) this.Min_Package_Num = (string)Value;
                        }
                        else if (valueType == ValueType.Title)
                        {
                            if (Value is string) this.Min_Package_Num_Title = (string)Value;
                        }
                        else if (valueType == ValueType.Font)
                        {
                            if (Value is Font) this.Min_Package_Num_font = (Font)Value;
                        }
                        else if (valueType == ValueType.ForeColor)
                        {
                            if (Value is Color) this.Min_Package_Num_ForeColor = (Color)Value;
                        }
                        else if (valueType == ValueType.BackColor)
                        {
                            if (Value is Color) this.Min_Package_Num_BackColor = (Color)Value;
                        }
                        else if (valueType == ValueType.Position)
                        {
                            if (Value is Point) this.Min_Package_Num_Position = (Point)Value;
                        }
                        else if (valueType == ValueType.Width)
                        {
                            if (Value is int) this.Min_Package_Num_Width = (int)Value;
                        }
                        else if (valueType == ValueType.Height)
                        {
                            if (Value is int) this.Min_Package_Num_Height = (int)Value;
                        }
                        else if (valueType == ValueType.BorderSize)
                        {
                            if (Value is int) this.Min_Package_Num_BorderSize = (int)Value;
                        }
                        else if (valueType == ValueType.BorderRadius)
                        {
                            if (Value is int) this.Min_Package_Num_BorderRadius = (int)Value;
                        }
                        else if (valueType == ValueType.BorderColor)
                        {
                            if (Value is Color) this.Min_Package_Num_BorderColor = (Color)Value;
                        }
                        else if (valueType == ValueType.HorizontalAlignment)
                        {
                            if (Value is HorizontalAlignment) this.Min_Package_Num_HorizontalAlignment = (HorizontalAlignment)Value;
                        }
                        else if (valueType == ValueType.Visable)
                        {
                            if (Value is bool) this.Min_Package_Num_Visable = (bool)Value;
                        }
                        break;
                    }
                case ValueName.BarCode:
                    {
                        if (valueType == ValueType.Value)
                        {
                            if (Value is string) this.BarCode = (string)Value;
                        }
                        else if (valueType == ValueType.Title)
                        {
                            if (Value is string) this.BarCode_Title = (string)Value;
                        }
                        else if (valueType == ValueType.Font)
                        {
                            if (Value is Font) this.BarCode_font = (Font)Value;
                        }
                        else if (valueType == ValueType.ForeColor)
                        {
                            if (Value is Color) this.BarCode_ForeColor = (Color)Value;
                        }
                        else if (valueType == ValueType.BackColor)
                        {
                            if (Value is Color) this.BarCode_BackColor = (Color)Value;
                        }
                        else if (valueType == ValueType.Position)
                        {
                            if (Value is Point) this.BarCode_Position = (Point)Value;
                        }
                        else if (valueType == ValueType.Width)
                        {
                            if (Value is int) this.BarCode_Width = (int)Value;
                        }
                        else if (valueType == ValueType.Height)
                        {
                            if (Value is int) this.BarCode_Height = (int)Value;
                        }
                        else if (valueType == ValueType.BorderSize)
                        {
                            if (Value is int) this.BarCode_BorderSize = (int)Value;
                        }
                        else if (valueType == ValueType.BorderRadius)
                        {
                            if (Value is int) this.BarCode_BorderRadius = (int)Value;
                        }
                        else if (valueType == ValueType.BorderColor)
                        {
                            if (Value is Color) this.BarCode_BorderColor = (Color)Value;
                        }
                        else if (valueType == ValueType.HorizontalAlignment)
                        {
                            if (Value is HorizontalAlignment) this.BarCode_HorizontalAlignment = (HorizontalAlignment)Value;
                        }
                        else if (valueType == ValueType.Visable)
                        {
                            if (Value is bool) this.BarCode_Visable = (bool)Value;
                        }
                        break;
                    }
                case ValueName.圖片1:
                    {

                        if (valueType == ValueType.Title)
                        {
                            if (Value is string) this.Picture1_Title = (string)Value;
                        }
                        else if (valueType == ValueType.Value)
                        {
                            if (Value is string) this.Picture1_Name = (string)Value;
                        }
                        else if (valueType == ValueType.Font)
                        {
                            if (Value is Font) this.Picture1_font = (Font)Value;
                        }
                        else if (valueType == ValueType.ForeColor)
                        {
                            if (Value is Color) this.Picture1_ForeColor = (Color)Value;
                        }
                        else if (valueType == ValueType.BackColor)
                        {
                            if (Value is Color) this.Picture1_BackColor = (Color)Value;
                        }
                        else if (valueType == ValueType.Position)
                        {
                            if (Value is Point) this.Picture1_Position = (Point)Value;
                        }
                        else if (valueType == ValueType.Width)
                        {
                            if (Value is int) this.Picture1_Width = (int)Value;
                        }
                        else if (valueType == ValueType.Height)
                        {
                            if (Value is int) this.Picture1_Height = (int)Value;
                        }
                        else if (valueType == ValueType.BorderSize)
                        {
                            if (Value is int) this.Picture1_BorderSize = (int)Value;
                        }
                        else if (valueType == ValueType.BorderColor)
                        {
                            if (Value is Color) this.Picture1_BorderColor = (Color)Value;
                        }
                        else if (valueType == ValueType.HorizontalAlignment)
                        {
                            if (Value is HorizontalAlignment) this.Picture1_HorizontalAlignment = (HorizontalAlignment)Value;
                        }
                        else if (valueType == ValueType.Visable)
                        {
                            if (Value is bool) this.Picture1_Visable = (bool)Value;
                        }
                        break;
                    }
                case ValueName.圖片2:
                    {

                        if (valueType == ValueType.Title)
                        {
                            if (Value is string) this.Picture2_Title = (string)Value;
                        }
                        else if (valueType == ValueType.Value)
                        {
                            if (Value is string) this.Picture2_Name = (string)Value;
                        }
                        else if (valueType == ValueType.Font)
                        {
                            if (Value is Font) this.Picture2_font = (Font)Value;
                        }
                        else if (valueType == ValueType.ForeColor)
                        {
                            if (Value is Color) this.Picture2_ForeColor = (Color)Value;
                        }
                        else if (valueType == ValueType.BackColor)
                        {
                            if (Value is Color) this.Picture2_BackColor = (Color)Value;
                        }
                        else if (valueType == ValueType.Position)
                        {
                            if (Value is Point) this.Picture2_Position = (Point)Value;
                        }
                        else if (valueType == ValueType.Width)
                        {
                            if (Value is int) this.Picture2_Width = (int)Value;
                        }
                        else if (valueType == ValueType.Height)
                        {
                            if (Value is int) this.Picture2_Height = (int)Value;
                        }
                        else if (valueType == ValueType.BorderSize)
                        {
                            if (Value is int) this.Picture2_BorderSize = (int)Value;
                        }
                        else if (valueType == ValueType.BorderColor)
                        {
                            if (Value is Color) this.Picture2_BorderColor = (Color)Value;
                        }
                        else if (valueType == ValueType.HorizontalAlignment)
                        {
                            if (Value is HorizontalAlignment) this.Picture2_HorizontalAlignment = (HorizontalAlignment)Value;
                        }
                        else if (valueType == ValueType.Visable)
                        {
                            if (Value is bool) this.Picture2_Visable = (bool)Value;
                        }
                        break;
                    }
                case ValueName.庫存:
                    {
                        if (valueType == ValueType.Value)
                        {
                         
                        }
                        else if (valueType == ValueType.Title)
                        {
                            if (Value is string) this.Inventory_Title = (string)Value;
                        }
                        else if (valueType == ValueType.Font)
                        {
                            if (Value is Font) this.Inventory_font = (Font)Value;
                        }
                        else if (valueType == ValueType.ForeColor)
                        {
                            if (Value is Color) this.Inventory_ForeColor = (Color)Value;
                        }
                        else if (valueType == ValueType.BackColor)
                        {
                            if (Value is Color) this.Inventory_BackColor = (Color)Value;
                        }
                        else if (valueType == ValueType.Position)
                        {
                            if (Value is Point) this.Inventory_Position = (Point)Value;
                        }
                        else if (valueType == ValueType.Width)
                        {
                            if (Value is int) this.Inventory_Width = (int)Value;
                        }
                        else if (valueType == ValueType.Height)
                        {
                            if (Value is int) this.Inventory_Height = (int)Value;
                        }
                        else if (valueType == ValueType.BorderSize)
                        {
                            if (Value is int) this.Inventory_BorderSize = (int)Value;
                        }
                        else if (valueType == ValueType.BorderRadius)
                        {
                            if (Value is int) this.Inventory_BorderRadius = (int)Value;
                        }
                        else if (valueType == ValueType.BorderColor)
                        {
                            if (Value is Color) this.Inventory_BorderColor = (Color)Value;
                        }
                        else if (valueType == ValueType.HorizontalAlignment)
                        {
                            if (Value is HorizontalAlignment) this.Inventory_HorizontalAlignment = (HorizontalAlignment)Value;
                        }
                        else if (valueType == ValueType.Visable)
                        {
                            if (Value is bool) this.Inventory_Visable = (bool)Value;
                        }
                        break;
                    }
                case ValueName.最大存量:
                    {
                        if (valueType == ValueType.Value)
                        {
                            if (Value is string)
                            {
                                if (Value.ObjectToString().StringIsEmpty()) Value = "0";
                                this.Max_Inventory = Value.ObjectToString().StringToDouble();

                            }
                        }
                        else if (valueType == ValueType.Title)
                        {
                            if (Value is string) this.Max_Inventory_Title = (string)Value;
                        }
                        else if (valueType == ValueType.Font)
                        {
                            if (Value is Font) this.Max_Inventory_font = (Font)Value;
                        }
                        else if (valueType == ValueType.ForeColor)
                        {
                            if (Value is Color) this.Max_Inventory_ForeColor = (Color)Value;
                        }
                        else if (valueType == ValueType.BackColor)
                        {
                            if (Value is Color) this.Max_Inventory_BackColor = (Color)Value;
                        }
                        else if (valueType == ValueType.Position)
                        {
                            if (Value is Point) this.Max_Inventory_Position = (Point)Value;
                        }
                        else if (valueType == ValueType.Width)
                        {
                            if (Value is int) this.Max_Inventory_Width = (int)Value;
                        }
                        else if (valueType == ValueType.Height)
                        {
                            if (Value is int) this.Max_Inventory_Height = (int)Value;
                        }
                        else if (valueType == ValueType.BorderSize)
                        {
                            if (Value is int) this.Max_Inventory_BorderSize = (int)Value;
                        }
                        else if (valueType == ValueType.BorderRadius)
                        {
                            if (Value is int) this.Max_Inventory_BorderRadius = (int)Value;
                        }
                        else if (valueType == ValueType.BorderColor)
                        {
                            if (Value is Color) this.Max_Inventory_BorderColor = (Color)Value;
                        }
                        else if (valueType == ValueType.HorizontalAlignment)
                        {
                            if (Value is HorizontalAlignment) this.Max_Inventory_HorizontalAlignment = (HorizontalAlignment)Value;
                        }
                        else if (valueType == ValueType.Visable)
                        {
                            if (Value is bool) this.Max_Inventory_Visable = (bool)Value;
                        }
                        break;
                    }
                case ValueName.IP:
                    {
                        if (valueType == ValueType.Value)
                        {
                            if (Value is string) this.IP = (string)Value;
                        }
                        else if (valueType == ValueType.Title)
                        {
                            if (Value is string) this.IP_Title = (string)Value;
                        }
                        else if (valueType == ValueType.Font)
                        {
                            if (Value is Font) this.IP_font = (Font)Value;
                        }
                        else if (valueType == ValueType.ForeColor)
                        {
                            if (Value is Color) this.IP_ForeColor = (Color)Value;
                        }
                        else if (valueType == ValueType.BackColor)
                        {
                            if (Value is Color) this.IP_BackColor = (Color)Value;
                        }
                        else if (valueType == ValueType.Position)
                        {
                            if (Value is Point) this.IP_Position = (Point)Value;
                        }
                        else if (valueType == ValueType.Width)
                        {
                            if (Value is int) this.IP_Width = (int)Value;
                        }
                        else if (valueType == ValueType.Height)
                        {
                            if (Value is int) this.IP_Height = (int)Value;
                        }
                        else if (valueType == ValueType.BorderSize)
                        {
                            if (Value is int) this.IP_BorderSize = (int)Value;
                        }
                        else if (valueType == ValueType.BorderColor)
                        {
                            if (Value is Color) this.IP_BorderColor = (Color)Value;
                        }
                        else if (valueType == ValueType.HorizontalAlignment)
                        {
                            if (Value is HorizontalAlignment) this.IP_HorizontalAlignment = (HorizontalAlignment)Value;
                        }
                        else if (valueType == ValueType.Visable)
                        {
                            if (Value is bool) this.IP_Visable = (bool)Value;
                        }
                        break;
                    }
                case ValueName.Port:
                    {
                        if (valueType == ValueType.Value)
                        {
                            if (Value is string) this.Port = ((string)Value).StringToInt32();
                        }
                        else if (valueType == ValueType.Title)
                        {
                            if (Value is string) this.Port_Title = (string)Value;
                        }
                        else if (valueType == ValueType.Font)
                        {
                            if (Value is Font) this.Port_font = (Font)Value;
                        }
                        else if (valueType == ValueType.ForeColor)
                        {
                            if (Value is Color) this.Port_ForeColor = (Color)Value;
                        }
                        else if (valueType == ValueType.BackColor)
                        {
                            if (Value is Color) this.Port_BackColor = (Color)Value;
                        }
                        else if (valueType == ValueType.Position)
                        {
                            if (Value is Point) this.Port_Position = (Point)Value;
                        }
                        else if (valueType == ValueType.Width)
                        {
                            if (Value is int) this.Port_Width = (int)Value;
                        }
                        else if (valueType == ValueType.Height)
                        {
                            if (Value is int) this.Port_Height = (int)Value;
                        }
                        else if (valueType == ValueType.BorderSize)
                        {
                            if (Value is int) this.Port_BorderSize = (int)Value;
                        }
                        else if (valueType == ValueType.BorderColor)
                        {
                            if (Value is Color) this.Port_BorderColor = (Color)Value;
                        }
                        else if (valueType == ValueType.HorizontalAlignment)
                        {
                            if (Value is HorizontalAlignment) this.Port_HorizontalAlignment = (HorizontalAlignment)Value;
                        }
                        else if (valueType == ValueType.Visable)
                        {
                            if (Value is bool) this.Port_Visable = (bool)Value;
                        }
                        break;
                    }
                case ValueName.CustomText1:
                    {

                        if (valueType == ValueType.Value)
                        {
                            if (Value is string) this.CustomText1 = (string)Value;
                        }
                        else if (valueType == ValueType.Title)
                        {
                            if (Value is string) this.CustomText1_Title = (string)Value;
                        }
                        else if (valueType == ValueType.Font)
                        {
                            if (Value is Font) this.CustomText1_font = (Font)Value;
                        }
                        else if (valueType == ValueType.ForeColor)
                        {
                            if (Value is Color) this.CustomText1_ForeColor = (Color)Value;
                        }
                        else if (valueType == ValueType.BackColor)
                        {
                            if (Value is Color) this.CustomText1_BackColor = (Color)Value;
                        }
                        else if (valueType == ValueType.Position)
                        {
                            if (Value is Point) this.CustomText1_Position = (Point)Value;
                        }
                        else if (valueType == ValueType.Width)
                        {
                            if (Value is int) this.CustomText1_Width = (int)Value;
                        }
                        else if (valueType == ValueType.Height)
                        {
                            if (Value is int) this.CustomText1_Height = (int)Value;
                        }
                        else if (valueType == ValueType.BorderSize)
                        {
                            if (Value is int) this.CustomText1_BorderSize = (int)Value;
                        }
                        else if (valueType == ValueType.BorderRadius)
                        {
                            if (Value is int) this.CustomText1_BorderRadius = (int)Value;
                        }
                        else if (valueType == ValueType.BorderColor)
                        {
                            if (Value is Color) this.CustomText1_BorderColor = (Color)Value;
                        }
                        else if (valueType == ValueType.HorizontalAlignment)
                        {
                            if (Value is HorizontalAlignment) this.CustomText1_HorizontalAlignment = (HorizontalAlignment)Value;
                        }
                        else if (valueType == ValueType.Visable)
                        {
                            if (Value is bool) this.CustomText1_Visable = (bool)Value;
                        }

                        break;
                    }
                case ValueName.CustomText2:
                    {

                        if (valueType == ValueType.Value)
                        {
                            if (Value is string) this.CustomText2 = (string)Value;
                        }
                        else if (valueType == ValueType.Title)
                        {
                            if (Value is string) this.CustomText2_Title = (string)Value;
                        }
                        else if (valueType == ValueType.Font)
                        {
                            if (Value is Font) this.CustomText2_font = (Font)Value;
                        }
                        else if (valueType == ValueType.ForeColor)
                        {
                            if (Value is Color) this.CustomText2_ForeColor = (Color)Value;
                        }
                        else if (valueType == ValueType.BackColor)
                        {
                            if (Value is Color) this.CustomText2_BackColor = (Color)Value;
                        }
                        else if (valueType == ValueType.Position)
                        {
                            if (Value is Point) this.CustomText2_Position = (Point)Value;
                        }
                        else if (valueType == ValueType.Width)
                        {
                            if (Value is int) this.CustomText2_Width = (int)Value;
                        }
                        else if (valueType == ValueType.Height)
                        {
                            if (Value is int) this.CustomText2_Height = (int)Value;
                        }
                        else if (valueType == ValueType.BorderSize)
                        {
                            if (Value is int) this.CustomText2_BorderSize = (int)Value;
                        }
                        else if (valueType == ValueType.BorderRadius)
                        {
                            if (Value is int) this.CustomText2_BorderRadius = (int)Value;
                        }
                        else if (valueType == ValueType.BorderColor)
                        {
                            if (Value is Color) this.CustomText2_BorderColor = (Color)Value;
                        }
                        else if (valueType == ValueType.HorizontalAlignment)
                        {
                            if (Value is HorizontalAlignment) this.CustomText2_HorizontalAlignment = (HorizontalAlignment)Value;
                        }
                        else if (valueType == ValueType.Visable)
                        {
                            if (Value is bool) this.CustomText2_Visable = (bool)Value;
                        }

                        break;
                    }
                case ValueName.CustomText3:
                    {

                        if (valueType == ValueType.Value)
                        {
                            if (Value is string) this.CustomText3 = (string)Value;
                        }
                        else if (valueType == ValueType.Title)
                        {
                            if (Value is string) this.CustomText3_Title = (string)Value;
                        }
                        else if (valueType == ValueType.Font)
                        {
                            if (Value is Font) this.CustomText3_font = (Font)Value;
                        }
                        else if (valueType == ValueType.ForeColor)
                        {
                            if (Value is Color) this.CustomText3_ForeColor = (Color)Value;
                        }
                        else if (valueType == ValueType.BackColor)
                        {
                            if (Value is Color) this.CustomText3_BackColor = (Color)Value;
                        }
                        else if (valueType == ValueType.Position)
                        {
                            if (Value is Point) this.CustomText3_Position = (Point)Value;
                        }
                        else if (valueType == ValueType.Width)
                        {
                            if (Value is int) this.CustomText3_Width = (int)Value;
                        }
                        else if (valueType == ValueType.Height)
                        {
                            if (Value is int) this.CustomText3_Height = (int)Value;
                        }
                        else if (valueType == ValueType.BorderSize)
                        {
                            if (Value is int) this.CustomText3_BorderSize = (int)Value;
                        }
                        else if (valueType == ValueType.BorderRadius)
                        {
                            if (Value is int) this.CustomText3_BorderRadius = (int)Value;
                        }
                        else if (valueType == ValueType.BorderColor)
                        {
                            if (Value is Color) this.CustomText3_BorderColor = (Color)Value;
                        }
                        else if (valueType == ValueType.HorizontalAlignment)
                        {
                            if (Value is HorizontalAlignment) this.CustomText3_HorizontalAlignment = (HorizontalAlignment)Value;
                        }
                        else if (valueType == ValueType.Visable)
                        {
                            if (Value is bool) this.CustomText3_Visable = (bool)Value;
                        }

                        break;
                    }
            }
        }
        public object GetValue(ValueName valueName, ValueType valueType)
        {
            Storage.VlaueClass vlaueClass = this.GetValue(valueName);
            if (valueType == ValueType.Value)
            {
                return vlaueClass.Value;
            }
            else if (valueType == ValueType.StringValue)
            {
                return vlaueClass.StringValue;
            }
            else if (valueType == ValueType.Font)
            {
                return vlaueClass.Font;
            }
            else if (valueType == ValueType.ForeColor)
            {
                return vlaueClass.ForeColor;
            }
            else if (valueType == ValueType.BackColor)
            {
                return vlaueClass.BackColor;
            }
            else if (valueType == ValueType.Position)
            {
                return vlaueClass.Position;
            }
            else if (valueType == ValueType.HorizontalAlignment)
            {
                return vlaueClass.HorizontalAlignment;
            }
            else if (valueType == ValueType.Width)
            {
                return vlaueClass.Width;
            }
            else if (valueType == ValueType.Height)
            {
                return vlaueClass.Height;
            }
            else if (valueType == ValueType.BorderSize)
            {
                return vlaueClass.BorderSize;
            }
            else if (valueType == ValueType.BorderRadius)
            {
                return vlaueClass.BorderRadius;
            }
            else if (valueType == ValueType.BorderColor)
            {
                return vlaueClass.BorderColor;
            }
            else if (valueType == ValueType.Visable)
            {
                return vlaueClass.Visable;
            }
            return null;
        }
        public VlaueClass GetValue(ValueName valueName)
        {
            VlaueClass vlaueClass = new VlaueClass();

            switch (valueName)
            {
                case ValueName.None:
                    {
                        vlaueClass.valueName = valueName;
                        break;
                    }
                case ValueName.藥品名稱:
                    {
                        vlaueClass.valueName = valueName;
                        vlaueClass.Title = this.Name_Title;
                        vlaueClass.Value = this.Name;
                        vlaueClass.Font = this.Name_font;
                        vlaueClass.ForeColor = this.Name_ForeColor;
                        vlaueClass.BackColor = this.Name_BackColor;
                        vlaueClass.Position = this.Name_Position;
                        vlaueClass.Width = this.Name_Width;
                        vlaueClass.Height = this.Name_Height;
                        vlaueClass.HorizontalAlignment = this.Name_HorizontalAlignment;
                        vlaueClass.BorderSize = this.Name_BorderSize;
                        vlaueClass.BorderRadius = this.Name_BorderRadius;
                        vlaueClass.BorderColor = this.Name_BorderColor;
                        vlaueClass.Visable = this.Name_Visable;
                        if (vlaueClass.Width > this.PanelSize.Width) vlaueClass.Width = this.PanelSize.Width;
                        if (vlaueClass.Height > this.PanelSize.Height) vlaueClass.Height = this.PanelSize.Height;

                        //Size size = TextRenderer.MeasureText(vlaueClass.StringValue, vlaueClass.Font);
                        //if (vlaueClass.Width < size.Width) vlaueClass.Width = size.Width;
                        //if (vlaueClass.Height < size.Height) vlaueClass.Height = size.Height;
                        break;
                    }
                case ValueName.藥品中文名稱:
                    {
                        vlaueClass.valueName = valueName;
                        vlaueClass.Title = this.ChineseName_Title;
                        vlaueClass.Value = this.ChineseName;
                        vlaueClass.Font = this.ChineseName_font;
                        vlaueClass.ForeColor = this.ChineseName_ForeColor;
                        vlaueClass.BackColor = this.ChineseName_BackColor;
                        vlaueClass.Position = this.ChineseName_Position;
                        vlaueClass.Width = this.ChineseName_Width;
                        vlaueClass.Height = this.ChineseName_Height;
                        vlaueClass.HorizontalAlignment = this.ChineseName_HorizontalAlignment;
                        vlaueClass.BorderSize = this.ChineseName_BorderSize;
                        vlaueClass.BorderRadius = this.ChineseName_BorderRadius;
                        vlaueClass.BorderColor = this.ChineseName_BorderColor;
                        vlaueClass.Visable = this.ChineseName_Visable;

                        //Size size = TextRenderer.MeasureText(vlaueClass.StringValue, vlaueClass.Font);
                        //if (vlaueClass.Width < size.Width) vlaueClass.Width = size.Width;
                        //if (vlaueClass.Height < size.Height) vlaueClass.Height = size.Height;
                        break;
                    }
                case ValueName.CustomText1:
                    {
                        vlaueClass.valueName = valueName;
                        vlaueClass.Title = this.CustomText1_Title;
                        vlaueClass.Value = this.CustomText1;
                        vlaueClass.Font = this.CustomText1_font;
                        vlaueClass.ForeColor = this.CustomText1_ForeColor;
                        vlaueClass.BackColor = this.CustomText1_BackColor;
                        vlaueClass.Position = this.CustomText1_Position;
                        vlaueClass.Width = this.CustomText1_Width;
                        vlaueClass.Height = this.CustomText1_Height;
                        vlaueClass.HorizontalAlignment = this.CustomText1_HorizontalAlignment;
                        vlaueClass.BorderSize = this.CustomText1_BorderSize;
                        vlaueClass.BorderRadius = this.CustomText1_BorderRadius;
                        vlaueClass.BorderColor = this.CustomText1_BorderColor;
                        vlaueClass.Visable = this.CustomText1_Visable;

                        //Size size = TextRenderer.MeasureText(vlaueClass.StringValue, vlaueClass.Font);
                        //if (vlaueClass.Width < size.Width) vlaueClass.Width = size.Width;
                        //if (vlaueClass.Height < size.Height) vlaueClass.Height = size.Height;
                        break;
                    }
                case ValueName.CustomText2:
                    {
                        vlaueClass.valueName = valueName;
                        vlaueClass.Title = this.CustomText2_Title;
                        vlaueClass.Value = this.CustomText2;
                        vlaueClass.Font = this.CustomText2_font;
                        vlaueClass.ForeColor = this.CustomText2_ForeColor;
                        vlaueClass.BackColor = this.CustomText2_BackColor;
                        vlaueClass.Position = this.CustomText2_Position;
                        vlaueClass.Width = this.CustomText2_Width;
                        vlaueClass.Height = this.CustomText2_Height;
                        vlaueClass.HorizontalAlignment = this.CustomText2_HorizontalAlignment;
                        vlaueClass.BorderSize = this.CustomText2_BorderSize;
                        vlaueClass.BorderRadius = this.CustomText2_BorderRadius;
                        vlaueClass.BorderColor = this.CustomText2_BorderColor;
                        vlaueClass.Visable = this.CustomText2_Visable;

                        //Size size = TextRenderer.MeasureText(vlaueClass.StringValue, vlaueClass.Font);
                        //if (vlaueClass.Width < size.Width) vlaueClass.Width = size.Width;
                        //if (vlaueClass.Height < size.Height) vlaueClass.Height = size.Height;
                        break;
                    }
                case ValueName.CustomText3:
                    {
                        vlaueClass.valueName = valueName;
                        vlaueClass.Title = this.CustomText3_Title;
                        vlaueClass.Value = this.CustomText3;
                        vlaueClass.Font = this.CustomText3_font;
                        vlaueClass.ForeColor = this.CustomText3_ForeColor;
                        vlaueClass.BackColor = this.CustomText3_BackColor;
                        vlaueClass.Position = this.CustomText3_Position;
                        vlaueClass.Width = this.CustomText3_Width;
                        vlaueClass.Height = this.CustomText3_Height;
                        vlaueClass.HorizontalAlignment = this.CustomText3_HorizontalAlignment;
                        vlaueClass.BorderSize = this.CustomText3_BorderSize;
                        vlaueClass.BorderRadius = this.CustomText3_BorderRadius;
                        vlaueClass.BorderColor = this.CustomText3_BorderColor;
                        vlaueClass.Visable = this.CustomText3_Visable;

                        //Size size = TextRenderer.MeasureText(vlaueClass.StringValue, vlaueClass.Font);
                        //if (vlaueClass.Width < size.Width) vlaueClass.Width = size.Width;
                        //if (vlaueClass.Height < size.Height) vlaueClass.Height = size.Height;
                        break;
                    }
                case ValueName.包裝單位:
                    {
                        vlaueClass.valueName = valueName;
                        vlaueClass.Title = this.Package_Title;
                        vlaueClass.Value = this.Package;
                        vlaueClass.Font = this.Package_font;
                        vlaueClass.ForeColor = this.Package_ForeColor;
                        vlaueClass.BackColor = this.Package_BackColor;
                        vlaueClass.Position = this.Package_Position;
                        vlaueClass.Width = this.Package_Width;
                        vlaueClass.Height = this.Package_Height;
                        vlaueClass.HorizontalAlignment = this.Package_HorizontalAlignment;
                        vlaueClass.BorderSize = this.Package_BorderSize;
                        vlaueClass.BorderRadius = this.Package_BorderRadius;
                        vlaueClass.BorderColor = this.Package_BorderColor;
                        vlaueClass.Visable = this.Package_Visable;
                        //Size size = TextRenderer.MeasureText(vlaueClass.StringValue, vlaueClass.Font);
                        //if (vlaueClass.Width < size.Width) vlaueClass.Width = size.Width;
                        //if (vlaueClass.Height < size.Height) vlaueClass.Height = size.Height;
                        break;
                    }
                case ValueName.藥品學名:
                    {
                        vlaueClass.valueName = valueName;
                        vlaueClass.Title = this.Scientific_Name_Title;
                        vlaueClass.Value = this.Scientific_Name;
                        vlaueClass.Font = this.Scientific_Name_font;
                        vlaueClass.ForeColor = this.Scientific_Name_ForeColor;
                        vlaueClass.BackColor = this.Scientific_Name_BackColor;
                        vlaueClass.Position = this.Scientific_Name_Position;
                        vlaueClass.Width = this.Scientific_Name_Width;
                        vlaueClass.Height = this.Scientific_Name_Height;
                        vlaueClass.HorizontalAlignment = this.Scientific_Name_HorizontalAlignment;
                        vlaueClass.BorderSize = this.Scientific_Name_BorderSize;
                        vlaueClass.BorderRadius = this.Scientific_Name_BorderRadius;
                        vlaueClass.BorderColor = this.Scientific_Name_BorderColor;
                        vlaueClass.Visable = this.Scientific_Name_Visable;

                        //Size size = TextRenderer.MeasureText(vlaueClass.StringValue, vlaueClass.Font);
                        //if (vlaueClass.Width < size.Width) vlaueClass.Width = size.Width;
                        //if (vlaueClass.Height < size.Height) vlaueClass.Height = size.Height;
                        break;
                    }
                case ValueName.藥品碼:
                    {
                        vlaueClass.valueName = valueName;
                        vlaueClass.Title = this.Code_Title;
                        vlaueClass.Value = this.Code;
                        vlaueClass.Font = this.Code_font;
                        vlaueClass.ForeColor = this.Code_ForeColor;
                        vlaueClass.BackColor = this.Code_BackColor;
                        vlaueClass.Position = this.Code_Position;
                        vlaueClass.Width = this.Code_Width;
                        vlaueClass.Height = this.Code_Height;
                        vlaueClass.HorizontalAlignment = this.Code_HorizontalAlignment;
                        vlaueClass.BorderSize = this.Code_BorderSize;
                        vlaueClass.BorderRadius = this.Code_BorderRadius;
                        vlaueClass.BorderColor = this.Code_BorderColor;
                        vlaueClass.Visable = this.Code_Visable;

                        //Size size = TextRenderer.MeasureText(vlaueClass.StringValue, vlaueClass.Font);
                        //if (vlaueClass.Width < size.Width) vlaueClass.Width = size.Width;
                        //if (vlaueClass.Height < size.Height) vlaueClass.Height = size.Height;
                        break;
                    }
                case ValueName.廠牌:
                    {
                        vlaueClass.valueName = valueName;
                        vlaueClass.Title = this.Label_Title;
                        vlaueClass.Value = this.Label;
                        vlaueClass.Font = this.Label_font;
                        vlaueClass.ForeColor = this.Label_ForeColor;
                        vlaueClass.BackColor = this.Label_BackColor;
                        vlaueClass.Position = this.Label_Position;
                        vlaueClass.Width = this.Label_Width;
                        vlaueClass.Height = this.Label_Height;
                        vlaueClass.HorizontalAlignment = this.Label_HorizontalAlignment;
                        vlaueClass.BorderSize = this.Label_BorderSize;
                        vlaueClass.BorderRadius = this.Label_BorderRadius;
                        vlaueClass.BorderColor = this.Label_BorderColor;
                        vlaueClass.Visable = this.Label_Visable;

                        //Size size = TextRenderer.MeasureText(vlaueClass.StringValue, vlaueClass.Font);
                        //if (vlaueClass.Width < size.Width) vlaueClass.Width = size.Width;
                        //if (vlaueClass.Height < size.Height) vlaueClass.Height = size.Height;
                        break;
                    }
                case ValueName.效期:
                    {
                        vlaueClass.valueName = valueName;
                        vlaueClass.Title = this.Validity_period_Title;
                        vlaueClass.Value = this.Validity_period;

                        vlaueClass.Value = "";
                        for (int i = 0; i < this.list_Validity_period.Count; i++)
                        {
                            if (i != 0) vlaueClass.Value += "\n";
                            vlaueClass.Value += $"效期:{list_Validity_period[i]} 庫存:{this.list_Inventory[i]}";
                        }
                        
                        vlaueClass.Font = this.Validity_period_font;
                        vlaueClass.ForeColor = this.Validity_period_ForeColor;
                        vlaueClass.BackColor = this.Validity_period_BackColor;
                        vlaueClass.Position = this.Validity_period_Position;
                        vlaueClass.Width = this.Validity_period_Width;
                        vlaueClass.Height = this.Validity_period_Height;
                        vlaueClass.HorizontalAlignment = this.Validity_period_HorizontalAlignment;
                        vlaueClass.BorderSize = this.Validity_period_BorderSize;
                        vlaueClass.BorderRadius = this.Validity_period_BorderRadius;
                        vlaueClass.BorderColor = this.Validity_period_BorderColor;
                        vlaueClass.Visable = this.Validity_period_Visable;

                        //Size size = TextRenderer.MeasureText(vlaueClass.StringValue, vlaueClass.Font);
                        //if (vlaueClass.Width < size.Width) vlaueClass.Width = size.Width;
                        //if (vlaueClass.Height < size.Height) vlaueClass.Height = size.Height;
                        break;
                    }
                case ValueName.儲位名稱:
                    {
                        vlaueClass.valueName = valueName;
                        vlaueClass.Title = this.StorageName_Title;
                        vlaueClass.Value = this.StorageName;
                        vlaueClass.Font = this.StorageName_font;
                        vlaueClass.ForeColor = this.StorageName_ForeColor;
                        vlaueClass.BackColor = this.StorageName_BackColor;
                        vlaueClass.Position = this.StorageName_Position;
                        vlaueClass.Width = this.StorageName_Width;
                        vlaueClass.Height = this.StorageName_Height;
                        vlaueClass.HorizontalAlignment = this.StorageName_HorizontalAlignment;
                        vlaueClass.BorderSize = this.StorageName_BorderSize;
                        vlaueClass.BorderRadius = this.StorageName_BorderRadius;
                        vlaueClass.BorderColor = this.StorageName_BorderColor;
                        vlaueClass.Visable = this.StorageName_Visable;

                        //Size size = TextRenderer.MeasureText(vlaueClass.StringValue, vlaueClass.Font);
                        //if (vlaueClass.Width < size.Width) vlaueClass.Width = size.Width;
                        //if (vlaueClass.Height < size.Height) vlaueClass.Height = size.Height;
                        break;
                    }
                case ValueName.最小包裝單位:
                    {
                        vlaueClass.valueName = valueName;
                        vlaueClass.Title = this.MinPackage_Title;
                        vlaueClass.Value = this.MinPackage;
                        vlaueClass.Font = this.MinPackage_font;
                        vlaueClass.ForeColor = this.MinPackage_ForeColor;
                        vlaueClass.BackColor = this.MinPackage_BackColor;
                        vlaueClass.Position = this.MinPackage_Position;
                        vlaueClass.Width = this.MinPackage_Width;
                        vlaueClass.Height = this.MinPackage_Height;
                        vlaueClass.HorizontalAlignment = this.MinPackage_HorizontalAlignment;
                        vlaueClass.BorderSize = this.MinPackage_BorderSize;
                        vlaueClass.BorderRadius = this.MinPackage_BorderRadius;
                        vlaueClass.BorderColor = this.MinPackage_BorderColor;
                        vlaueClass.Visable = this.MinPackage_Visable;

                        //Size size = TextRenderer.MeasureText(vlaueClass.StringValue, vlaueClass.Font);
                        //if (vlaueClass.Width < size.Width) vlaueClass.Width = size.Width;
                        //if (vlaueClass.Height < size.Height) vlaueClass.Height = size.Height;
                        break;
                    }
                case ValueName.最小包裝單位數量:
                    {
                        vlaueClass.valueName = valueName;
                        vlaueClass.Title = this.Min_Package_Num_Title;
                        vlaueClass.Value = this.Min_Package_Num;
                        vlaueClass.Font = this.Min_Package_Num_font;
                        vlaueClass.ForeColor = this.Min_Package_Num_ForeColor;
                        vlaueClass.BackColor = this.Min_Package_Num_BackColor;
                        vlaueClass.Position = this.Min_Package_Num_Position;
                        vlaueClass.Width = this.Min_Package_Num_Width;
                        vlaueClass.Height = this.Min_Package_Num_Height;
                        vlaueClass.HorizontalAlignment = this.Min_Package_Num_HorizontalAlignment;
                        vlaueClass.BorderSize = this.Min_Package_Num_BorderSize;
                        vlaueClass.BorderRadius = this.Min_Package_Num_BorderRadius;
                        vlaueClass.BorderColor = this.Min_Package_Num_BorderColor;
                        vlaueClass.Visable = this.Min_Package_Num_Visable;

                        //Size size = TextRenderer.MeasureText(vlaueClass.StringValue, vlaueClass.Font);
                        //if (vlaueClass.Width < size.Width) vlaueClass.Width = size.Width;
                        //if (vlaueClass.Height < size.Height) vlaueClass.Height = size.Height;
                        break;
                    }
                case ValueName.BarCode:
                    {
                        vlaueClass.valueName = valueName;
                        vlaueClass.Title = this.BarCode_Title;
                        vlaueClass.Value = this.BarCode;
                        vlaueClass.Font = this.BarCode_font;
                        vlaueClass.ForeColor = this.BarCode_ForeColor;
                        vlaueClass.BackColor = this.BarCode_BackColor;
                        vlaueClass.Position = this.BarCode_Position;
                        vlaueClass.Width = this.BarCode_Width;
                        vlaueClass.Height = this.BarCode_Height;
                        vlaueClass.HorizontalAlignment = this.BarCode_HorizontalAlignment;
                        vlaueClass.BorderSize = this.BarCode_BorderSize;
                        vlaueClass.BorderRadius = this.BarCode_BorderRadius;
                        vlaueClass.BorderColor = this.BarCode_BorderColor;
                        vlaueClass.Visable = this.BarCode_Visable;

                        if (vlaueClass.Value.StringIsEmpty())
                        {
                            vlaueClass.Value = "None";
                        }
                        ////Size size = TextRenderer.MeasureText(vlaueClass.Value, vlaueClass.Font);
                        ////if (vlaueClass.Width < size.Width) vlaueClass.Width = size.Width;
                        ////if (vlaueClass.Height < size.Height) vlaueClass.Height = size.Height;
                        break;
                    }
                case ValueName.圖片1:
                    {
                        vlaueClass.valueName = valueName;
                        vlaueClass.Value = this.Picture1_Name;
                        vlaueClass.Title = this.Picture1_Title;
                        vlaueClass.Font = this.Picture1_font;
                        vlaueClass.ForeColor = this.Picture1_ForeColor;
                        vlaueClass.BackColor = this.Picture1_BackColor;
                        vlaueClass.Position = this.Picture1_Position;
                        vlaueClass.Width = this.Picture1_Width;
                        vlaueClass.Height = this.Picture1_Height;
                        vlaueClass.HorizontalAlignment = this.Picture1_HorizontalAlignment;
                        vlaueClass.BorderSize = this.Picture1_BorderSize;
                        vlaueClass.BorderColor = this.Picture1_BorderColor;
                        vlaueClass.Visable = this.Picture1_Visable;

                        if (vlaueClass.Value.StringIsEmpty())
                        {
                            vlaueClass.Value = "None";
                        }
                        ////Size size = TextRenderer.MeasureText(vlaueClass.Value, vlaueClass.Font);
                        ////if (vlaueClass.Width < size.Width) vlaueClass.Width = size.Width;
                        ////if (vlaueClass.Height < size.Height) vlaueClass.Height = size.Height;
                        break;
                    }
                case ValueName.圖片2:
                    {
                        vlaueClass.valueName = valueName;
                        vlaueClass.Value = this.Picture2_Name;
                        vlaueClass.Title = this.Picture2_Title;
                        vlaueClass.Font = this.Picture2_font;
                        vlaueClass.ForeColor = this.Picture2_ForeColor;
                        vlaueClass.BackColor = this.Picture2_BackColor;
                        vlaueClass.Position = this.Picture2_Position;
                        vlaueClass.Width = this.Picture2_Width;
                        vlaueClass.Height = this.Picture2_Height;
                        vlaueClass.HorizontalAlignment = this.Picture2_HorizontalAlignment;
                        vlaueClass.BorderSize = this.Picture2_BorderSize;
                        vlaueClass.BorderColor = this.Picture2_BorderColor;
                        vlaueClass.Visable = this.Picture2_Visable;

                        if (vlaueClass.Value.StringIsEmpty())
                        {
                            vlaueClass.Value = "None";
                        }
                        ////Size size = TextRenderer.MeasureText(vlaueClass.Value, vlaueClass.Font);
                        ////if (vlaueClass.Width < size.Width) vlaueClass.Width = size.Width;
                        ////if (vlaueClass.Height < size.Height) vlaueClass.Height = size.Height;
                        break;
                    }
                case ValueName.庫存:
                    {
                        vlaueClass.valueName = valueName;
                        vlaueClass.Title = this.Inventory_Title;
                        vlaueClass.Value = this.Inventory;
                        vlaueClass.Font = this.Inventory_font;
                        vlaueClass.ForeColor = this.Inventory_ForeColor;
                        vlaueClass.BackColor = this.Inventory_BackColor;
                        vlaueClass.Position = this.Inventory_Position;
                        vlaueClass.Width = this.Inventory_Width;
                        vlaueClass.Height = this.Inventory_Height;
                        vlaueClass.HorizontalAlignment = this.Inventory_HorizontalAlignment;
                        vlaueClass.BorderSize = this.Inventory_BorderSize;
                        vlaueClass.BorderRadius = this.Inventory_BorderRadius;
                        vlaueClass.BorderColor = this.Inventory_BorderColor;
                        vlaueClass.Visable = this.Inventory_Visable;

                        //Size size = TextRenderer.MeasureText(vlaueClass.StringValue, vlaueClass.Font);
                        //if (vlaueClass.Width < size.Width) vlaueClass.Width = size.Width;
                        //if (vlaueClass.Height < size.Height) vlaueClass.Height = size.Height;
                        break;
                    }
                case ValueName.最大存量:
                    {
                        vlaueClass.valueName = valueName;
                        vlaueClass.Title = this.Max_Inventory_Title;
                        vlaueClass.Value = this.Max_Inventory.ToString();
                        vlaueClass.Font = this.Max_Inventory_font;
                        vlaueClass.ForeColor = this.Max_Inventory_ForeColor;
                        vlaueClass.BackColor = this.Max_Inventory_BackColor;
                        vlaueClass.Position = this.Max_Inventory_Position;
                        vlaueClass.Width = this.Max_Inventory_Width;
                        vlaueClass.Height = this.Max_Inventory_Height;
                        vlaueClass.HorizontalAlignment = this.Max_Inventory_HorizontalAlignment;
                        vlaueClass.BorderSize = this.Max_Inventory_BorderSize;
                        vlaueClass.BorderRadius = this.Max_Inventory_BorderRadius;
                        vlaueClass.BorderColor = this.Max_Inventory_BorderColor;
                        vlaueClass.Visable = this.Max_Inventory_Visable;

                        //Size size = TextRenderer.MeasureText(vlaueClass.StringValue, vlaueClass.Font);
                        //if (vlaueClass.Width < size.Width) vlaueClass.Width = size.Width;
                        //if (vlaueClass.Height < size.Height) vlaueClass.Height = size.Height;
                        break;
                    }
                case ValueName.IP:
                    {
                        vlaueClass.valueName = valueName;
                        vlaueClass.Title = this.IP_Title;
                        vlaueClass.Value = this.IP;
                        vlaueClass.Font = this.IP_font;
                        vlaueClass.ForeColor = this.IP_ForeColor;
                        vlaueClass.BackColor = this.IP_BackColor;
                        vlaueClass.Position = this.IP_Position;
                        vlaueClass.Width = this.IP_Width;
                        vlaueClass.Height = this.IP_Height;
                        vlaueClass.HorizontalAlignment = this.IP_HorizontalAlignment;
                        vlaueClass.BorderSize = this.IP_BorderSize;
                        vlaueClass.BorderColor = this.IP_BorderColor;
                        vlaueClass.Visable = this.IP_Visable;

                        //Size size = TextRenderer.MeasureText(vlaueClass.StringValue, vlaueClass.Font);
                        //if (vlaueClass.Width < size.Width) vlaueClass.Width = size.Width;
                        //if (vlaueClass.Height < size.Height) vlaueClass.Height = size.Height;
                        break;
                    }
                case ValueName.Port:
                    {
                        vlaueClass.valueName = valueName;
                        vlaueClass.Title = this.Port_Title;
                        vlaueClass.Value = this.Port.ToString();
                        vlaueClass.Font = this.Port_font;
                        vlaueClass.ForeColor = this.Port_ForeColor;
                        vlaueClass.BackColor = this.Port_BackColor;
                        vlaueClass.Position = this.Port_Position;
                        vlaueClass.Width = this.Port_Width;
                        vlaueClass.Height = this.Port_Height;
                        vlaueClass.HorizontalAlignment = this.Port_HorizontalAlignment;
                        vlaueClass.BorderSize = this.Port_BorderSize;
                        vlaueClass.BorderColor = this.Port_BorderColor;
                        vlaueClass.Visable = this.Port_Visable;

                        //Size size = TextRenderer.MeasureText(vlaueClass.StringValue, vlaueClass.Font);
                        //if (vlaueClass.Width < size.Width) vlaueClass.Width = size.Width;
                        //if (vlaueClass.Height < size.Height) vlaueClass.Height = size.Height;
                        break;
                    }
            }
            if (vlaueClass.Width > this.PanelSize.Width) vlaueClass.Width = this.PanelSize.Width;
            if (vlaueClass.Height > this.PanelSize.Height) vlaueClass.Height = this.PanelSize.Height;
            return vlaueClass;
        }
        public Bitmap Get_EPD_Bitmap(ValueName valueName, double bmp_Scale)
        {
            return this.Get_EPD_Bitmap(valueName, bmp_Scale, null, 0, false);
        }
        public Bitmap Get_EPD_Bitmap(ValueName valueName, double bmp_Scale, Color? color, int BorderSize, bool dash)
        {
            VlaueClass vlaueClass = this.GetValue(valueName);

            if (valueName == ValueName.BarCode)
            {

                Size Rect_Size = new Size((int)(vlaueClass.Width * bmp_Scale), (int)(vlaueClass.Height * bmp_Scale));

                Bitmap bitmap = Communication.CreateBarCode(vlaueClass.Value, Rect_Size.Width, Rect_Size.Height);
                if (bitmap != null)
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        if (color != null)
                        {
                            g.SmoothingMode = SmoothingMode.HighQuality; //使繪圖質量最高，即消除鋸齒
                            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            g.CompositingQuality = CompositingQuality.HighQuality;
                            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                            float[] dashValues = { 2, 2, 2, 2 };
                            Pen pen = new Pen((Color)color, BorderSize);
                            if (dash) pen.DashPattern = dashValues;

                            g.DrawRectangle(pen, BorderSize / 2, BorderSize / 2, (int)(vlaueClass.Width * bmp_Scale - BorderSize), (int)(vlaueClass.Height * bmp_Scale - BorderSize));
                        }
                    }
                }
                return bitmap;          
            }
            else if (valueName == ValueName.圖片1|| valueName == ValueName.圖片2)
            {
                // 計算縮放後的尺寸
                Size rectSize = new Size((int)(vlaueClass.Width * bmp_Scale),
                                         (int)(vlaueClass.Height * bmp_Scale));

                // 將字串 Value 轉換成 enum_PictureType
                if (!Enum.TryParse<enum_PictureType>(vlaueClass.Value, out var picType))
                {
                    return null; // 無效的值就直接回傳 null
                }

                // 呼叫封裝好的靜態函式
                Bitmap bitmap = Communication.GetPictureBitmap(picType, rectSize.Width, rectSize.Height, color, BorderSize, dash, Code);

                return bitmap;

            }
            else
            {
                Bitmap bitmap = Communication.TextToBitmap(vlaueClass.Value, vlaueClass.Font, bmp_Scale, vlaueClass.Width, vlaueClass.Height, vlaueClass.ForeColor, vlaueClass.BackColor, vlaueClass.BorderSize , vlaueClass.BorderRadius, vlaueClass.BorderColor, vlaueClass.HorizontalAlignment);
                if (bitmap == null) return null;
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.SmoothingMode = SmoothingMode.HighQuality; //使繪圖質量最高，即消除鋸齒
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                    if (color != null)
                    {
                        float[] dashValues = { 2, 2, 2, 2 };
                        Pen pen = new Pen((Color)color, BorderSize);
                        if (dash) pen.DashPattern = dashValues;

                        g.DrawRectangle(pen, BorderSize / 2, BorderSize / 2, (int)(bitmap.Width - BorderSize), (int)(bitmap.Height - BorderSize));

                    }
                }
                return bitmap;
            }


        }
        public Bitmap GetBitmap(ValueName valueName, double bmp_Scale)
        {
            return this.GetBitmap(valueName, bmp_Scale, null , 0 , false);
        }
        public Bitmap GetBitmap(ValueName valueName, double bmp_Scale, Color? color , int  BorderSize , bool dash)
        {
            VlaueClass vlaueClass = this.GetValue(valueName);

            if (valueName != ValueName.BarCode)
            {
                Bitmap bitmap = Communication.TextToBitmap(vlaueClass.StringValue, vlaueClass.Font, bmp_Scale, vlaueClass.Width, vlaueClass.Height, vlaueClass.ForeColor, vlaueClass.BackColor, vlaueClass.BorderSize, vlaueClass.BorderRadius, vlaueClass.BorderColor, vlaueClass.HorizontalAlignment);
                if (bitmap == null) return null;
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.SmoothingMode = SmoothingMode.HighQuality; //使繪圖質量最高，即消除鋸齒
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                    if (color != null)
                    {                      
                        float[] dashValues = { 2, 2, 2, 2 };
                        Pen pen = new Pen((Color)color, BorderSize);
                        if (dash) pen.DashPattern = dashValues;

                        g.DrawRectangle(pen, BorderSize/ 2, BorderSize / 2, (int)(bitmap.Width - BorderSize), (int)(bitmap.Height - BorderSize));                  
                    }
                }
                return bitmap;
            }
            else
            {
                Size Rect_Size = new Size((int)(vlaueClass.Width * bmp_Scale), (int)(vlaueClass.Height * bmp_Scale));

                Bitmap bitmap = Communication.CreateBarCode(vlaueClass.Value, Rect_Size.Width, Rect_Size.Height);
                if (bitmap != null)
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        if (color != null)
                        {
                            g.SmoothingMode = SmoothingMode.HighQuality; //使繪圖質量最高，即消除鋸齒
                            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            g.CompositingQuality = CompositingQuality.HighQuality;
                            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                            float[] dashValues = { 2, 2, 2, 2 };
                            Pen pen = new Pen((Color)color, BorderSize);
                            if (dash) pen.DashPattern = dashValues;

                            g.DrawRectangle(pen, BorderSize / 2, BorderSize /  2, (int)(vlaueClass.Width * bmp_Scale - BorderSize), (int)(vlaueClass.Height * bmp_Scale - BorderSize));
                        }
                    }
                }

                return bitmap;
            }


        }
        public Bitmap CreateBarCode(string content, int Width, int Height)
        {
            if (content.StringIsEmpty()) return null;
            ZXing.BarcodeWriter barcodeWriter = new ZXing.BarcodeWriter
            {
                Format = ZXing.BarcodeFormat.CODE_128,
                Options = new ZXing.Common.EncodingOptions
                {
                    Height = Height,
                    Width = Width,
                    PureBarcode = true,
                    //CharacterCommand = "UTF-8",
                    Margin = 0
                }
            };
            var barCode = barcodeWriter.Write(content);
            return barCode;
        }

        static public ValueName GetValueName(string text)
        {
            if(text.StringIsEmpty())return ValueName.None;
            if (text == "藥碼" || text == "藥品碼") return ValueName.藥品碼;
            if (text == "藥品名稱" || text == "藥名") return ValueName.藥品名稱;
            if (text == "藥品中文名稱" || text.Contains("中文名")) return ValueName.藥品中文名稱;
            if (text == "藥品學名" || text == "商品名") return ValueName.藥品學名;
            if (text == "廠牌") return ValueName.廠牌;
            if (text == "效期") return ValueName.效期;
            if (text == "庫存") return ValueName.庫存;
            if (text == "包裝單位" || text == "單位") return ValueName.包裝單位;
            if (text == "最小包裝單位" || text == "最小單位") return ValueName.最小包裝單位;
            if (text == "最小包裝單位數量" || text == "最小單位數量") return ValueName.最小包裝單位數量;
            if (text.ToUpper() == "BarCode".ToUpper() || text.Contains("條碼")) return ValueName.BarCode;
            if (text == "儲位名稱") return ValueName.儲位名稱;
            if (text == "IP") return ValueName.IP;
            if (text == "Port") return ValueName.Port;
            if (text == "最大存量") return ValueName.最大存量;
            if (text == "圖片1") return ValueName.圖片1;
            if (text == "圖片2") return ValueName.圖片2;
            if (text == "CustomText1" || text.Contains("文本1"))
            {
                return ValueName.CustomText1;
            }
            if (text == "CustomText2" || text.Contains("文本2")) return ValueName.CustomText2;
            if (text == "CustomText3" || text.Contains("文本3")) return ValueName.CustomText3;


            return ValueName.None;
        }

        #region Name
        private string _Name_Title = "";
        public string Name_Title { get => _Name_Title; set => _Name_Title = value; }
        private string _Name = "";
        public string Name { get => _Name; set => _Name = value; }
        [JsonIgnore]
        public Font Name_font
        {
            get
            {
                return FontSerializationHelper.FromString(_Name_font_Serialize);
            }
            set
            {
                _Name_font_Serialize = FontSerializationHelper.ToString(value);
            }
        }
        private string _Name_font_Serialize = "微軟正黑體:14:Bold:Point:1:False";
        [Browsable(false)]
        public string Name_font_Serialize
        {
            get { return _Name_font_Serialize; }
            set { _Name_font_Serialize = value; }
        }
        [JsonIgnore]
        public Color Name_BackColor = Color.White;
        [Browsable(false)]
        public string Name_BackColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(Name_BackColor); }
            set { Name_BackColor = ColorSerializationHelper.FromString(value); }
        }
        [JsonIgnore]
        public Color Name_ForeColor = Color.Black;
        [Browsable(false)]
        public string Name_ForeColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(Name_ForeColor); }
            set { Name_ForeColor = ColorSerializationHelper.FromString(value); }
        }
        private Point _Name_Position = new Point();
        public Point Name_Position { get => _Name_Position; set => _Name_Position = value; }
        private bool _Name_Visable = true;
        public bool Name_Visable { get => _Name_Visable; set => _Name_Visable = value; }
        private int _Name_Width = 0;
        public int Name_Width { get => _Name_Width; set => _Name_Width = value; }
        private int _Name_Height = 0;
        public int Name_Height { get => _Name_Height; set => _Name_Height = value; }
        private int _Name_BorderSize = 2;
        public int Name_BorderSize { get => _Name_BorderSize; set => _Name_BorderSize = value; }
        private int _Name_BorderRadius = 0;
        public int Name_BorderRadius { get => _Name_BorderRadius; set => _Name_BorderRadius = value; }
        [JsonIgnore]
        public Color Name_BorderColor = Color.Black;
        [Browsable(false)]
        public string Name_BorderColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(Name_BorderColor); }
            set { Name_BorderColor = ColorSerializationHelper.FromString(value); }
        }
        private HorizontalAlignment _Name_HorizontalAlignment = HorizontalAlignment.Left;
        public HorizontalAlignment Name_HorizontalAlignment { get => _Name_HorizontalAlignment; set => _Name_HorizontalAlignment = value; }
        #endregion
        #region ChineseName
        private string _ChineseName_Title = "";
        public string ChineseName_Title { get => _ChineseName_Title; set => _ChineseName_Title = value; }

        [JsonIgnore]
        public Font ChineseName_font
        {
            get
            {
                return FontSerializationHelper.FromString(_ChineseName_font_Serialize);
            }
            set
            {
                _ChineseName_font_Serialize = FontSerializationHelper.ToString(value);
            }
        }
        private string _ChineseName_font_Serialize = "微軟正黑體:14:Bold:Point:1:False";
        [Browsable(false)]
        public string ChineseName_font_Serialize
        {
            get { return _ChineseName_font_Serialize; }
            set { _ChineseName_font_Serialize = value; }
        }
        [JsonIgnore]
        public Color ChineseName_BackColor = Color.White;
        [Browsable(false)]
        public string ChineseName_BackColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(ChineseName_BackColor); }
            set { ChineseName_BackColor = ColorSerializationHelper.FromString(value); }
        }
        [JsonIgnore]
        public Color ChineseName_ForeColor = Color.Black;
        [Browsable(false)]
        public string ChineseName_ForeColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(ChineseName_ForeColor); }
            set { ChineseName_ForeColor = ColorSerializationHelper.FromString(value); }
        }
        private Point _ChineseName_Position = new Point();
        public Point ChineseName_Position { get => _ChineseName_Position; set => _ChineseName_Position = value; }
        private bool _ChineseName_Visable = true;
        public bool ChineseName_Visable { get => _ChineseName_Visable; set => _ChineseName_Visable = value; }
        private int _ChineseName_Width = 0;
        public int ChineseName_Width { get => _ChineseName_Width; set => _ChineseName_Width = value; }
        private int _ChineseName_Height = 0;
        public int ChineseName_Height { get => _ChineseName_Height; set => _ChineseName_Height = value; }
        private int _ChineseName_BorderSize = 2;
        public int ChineseName_BorderSize { get => _ChineseName_BorderSize; set => _ChineseName_BorderSize = value; }
        private int _ChineseName_BorderRadius = 0;
        public int ChineseName_BorderRadius { get => _ChineseName_BorderRadius; set => _ChineseName_BorderRadius = value; }
        [JsonIgnore]
        public Color ChineseName_BorderColor = Color.Black;
        [Browsable(false)]
        public string ChineseName_BorderColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(ChineseName_BorderColor); }
            set { ChineseName_BorderColor = ColorSerializationHelper.FromString(value); }
        }
        private HorizontalAlignment _ChineseName_HorizontalAlignment = HorizontalAlignment.Left;
        public HorizontalAlignment ChineseName_HorizontalAlignment { get => _ChineseName_HorizontalAlignment; set => _ChineseName_HorizontalAlignment = value; }
        #endregion
        #region Package
        private string _Package_Title = "";
        public string Package_Title { get => _Package_Title; set => _Package_Title = value; }
        private string _Package = "";
        public string Package { get => _Package; set => _Package = value; }
        [JsonIgnore]
        public Font Package_font
        {
            get
            {
                return FontSerializationHelper.FromString(_Package_font_Serialize);
            }
            set
            {
                _Package_font_Serialize = FontSerializationHelper.ToString(value);
            }
        }
        private string _Package_font_Serialize = "微軟正黑體:14:Bold:Point:1:False";
        [Browsable(false)]
        public string Package_font_Serialize
        {
            get { return _Package_font_Serialize; }
            set { _Package_font_Serialize = value; }
        }
        [JsonIgnore]
        public Color Package_BackColor = Color.White;
        [Browsable(false)]
        public string Package_BackColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(Package_BackColor); }
            set { Package_BackColor = ColorSerializationHelper.FromString(value); }
        }
        [JsonIgnore]
        public Color Package_ForeColor = Color.Black;
        [Browsable(false)]
        public string Package_ForeColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(Package_ForeColor); }
            set { Package_ForeColor = ColorSerializationHelper.FromString(value); }
        }
        private Point _Package_Position = new Point();
        public Point Package_Position { get => _Package_Position; set => _Package_Position = value; }
        private bool _Package_Visable = true;
        public bool Package_Visable { get => _Package_Visable; set => _Package_Visable = value; }
        private int _Package_Width = 0;
        public int Package_Width { get => _Package_Width; set => _Package_Width = value; }
        private int _Package_Height = 0;
        public int Package_Height { get => _Package_Height; set => _Package_Height = value; }
        private int _Package_BorderSize = 2;
        public int Package_BorderSize { get => _Package_BorderSize; set => _Package_BorderSize = value; }
        private int _Package_BorderRadius = 0;
        public int Package_BorderRadius { get => _Package_BorderRadius; set => _Package_BorderRadius = value; }
        [JsonIgnore]
        public Color Package_BorderColor = Color.Black;
        [Browsable(false)]
        public string Package_BorderColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(Package_BorderColor); }
            set { Package_BorderColor = ColorSerializationHelper.FromString(value); }
        }
        private HorizontalAlignment _Package_HorizontalAlignment = HorizontalAlignment.Left;
        public HorizontalAlignment Package_HorizontalAlignment { get => _Package_HorizontalAlignment; set => _Package_HorizontalAlignment = value; }
        #endregion
        #region Scientific_Name
        private string _Scientific_Name_Title = "";
        public string Scientific_Name_Title { get => _Scientific_Name_Title; set => _Scientific_Name_Title = value; }

        [JsonIgnore]
        public Font Scientific_Name_font
        {
            get
            {
                return FontSerializationHelper.FromString(_Scientific_Name_font_Serialize);
            }
            set
            {
                _Scientific_Name_font_Serialize = FontSerializationHelper.ToString(value);
            }
        }
        private string _Scientific_Name_font_Serialize = "微軟正黑體:14:Bold:Point:1:False";
        [Browsable(false)]
        public string Scientific_Name_font_Serialize
        {
            get { return _Scientific_Name_font_Serialize; }
            set { _Scientific_Name_font_Serialize = value; }
        }
        [JsonIgnore]
        public Color Scientific_Name_BackColor = Color.White;
        [Browsable(false)]
        public string Scientific_Name_BackColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(Scientific_Name_BackColor); }
            set { Scientific_Name_BackColor = ColorSerializationHelper.FromString(value); }
        }
        [JsonIgnore]
        public Color Scientific_Name_ForeColor = Color.Black;
        [Browsable(false)]
        public string Scientific_Name_ForeColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(Scientific_Name_ForeColor); }
            set { Scientific_Name_ForeColor = ColorSerializationHelper.FromString(value); }
        }
        private Point _Scientific_Name_Position = new Point();
        public Point Scientific_Name_Position { get => _Scientific_Name_Position; set => _Scientific_Name_Position = value; }
        private bool _Scientific_Name_Visable = true;
        public bool Scientific_Name_Visable { get => _Scientific_Name_Visable; set => _Scientific_Name_Visable = value; }
        private int _Scientific_Name_Width = 0;
        public int Scientific_Name_Width { get => _Scientific_Name_Width; set => _Scientific_Name_Width = value; }
        private int _Scientific_Name_Height = 0;
        public int Scientific_Name_Height { get => _Scientific_Name_Height; set => _Scientific_Name_Height = value; }
        private int _Scientific_Name_BorderSize = 2;
        public int Scientific_Name_BorderSize { get => _Scientific_Name_BorderSize; set => _Scientific_Name_BorderSize = value; }
        private int _Scientific_Name_BorderRadius = 0;
        public int Scientific_Name_BorderRadius { get => _Scientific_Name_BorderRadius; set => _Scientific_Name_BorderRadius = value; }
        [JsonIgnore]
        public Color Scientific_Name_BorderColor = Color.Black;
        [Browsable(false)]
        public string Scientific_Name_BorderColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(Scientific_Name_BorderColor); }
            set { Scientific_Name_BorderColor = ColorSerializationHelper.FromString(value); }
        }
        private HorizontalAlignment _Scientific_Name_HorizontalAlignment = HorizontalAlignment.Left;
        public HorizontalAlignment Scientific_Name_HorizontalAlignment { get => _Scientific_Name_HorizontalAlignment; set => _Scientific_Name_HorizontalAlignment = value; }
        #endregion
        #region Code
        private string _Code_Title = "";
        public string Code_Title { get => _Code_Title; set => _Code_Title = value; }

        [JsonIgnore]
        public Font Code_font
        {
            get
            {
                return FontSerializationHelper.FromString(_Code_font_Serialize);
            }
            set
            {
                _Code_font_Serialize = FontSerializationHelper.ToString(value);
            }
        }
        private string _Code_font_Serialize = "微軟正黑體:14:Bold:Point:1:False";
        [Browsable(false)]
        public string Code_font_Serialize
        {
            get { return _Code_font_Serialize; }
            set { _Code_font_Serialize = value; }
        }
        [JsonIgnore]
        public Color Code_BackColor = Color.White;
        [Browsable(false)]
        public string Code_BackColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(Code_BackColor); }
            set { Code_BackColor = ColorSerializationHelper.FromString(value); }
        }
        [JsonIgnore]
        public Color Code_ForeColor = Color.Black;
        [Browsable(false)]
        public string Code_ForeColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(Code_ForeColor); }
            set { Code_ForeColor = ColorSerializationHelper.FromString(value); }
        }
        private Point _Code_Position = new Point();
        public Point Code_Position { get => _Code_Position; set => _Code_Position = value; }
        private bool _Code_Visable = true;
        public bool Code_Visable { get => _Code_Visable; set => _Code_Visable = value; }
        private int _Code_Width = 0;
        public int Code_Width { get => _Code_Width; set => _Code_Width = value; }
        private int _Code_Height = 0;
        public int Code_Height { get => _Code_Height; set => _Code_Height = value; }
        private int _Code_BorderSize = 2;
        public int Code_BorderSize { get => _Code_BorderSize; set => _Code_BorderSize = value; }
        private int _Code_BorderRadius = 0;
        public int Code_BorderRadius { get => _Code_BorderRadius; set => _Code_BorderRadius = value; }
        [JsonIgnore]
        public Color Code_BorderColor = Color.Black;
        [Browsable(false)]
        public string Code_BorderColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(Code_BorderColor); }
            set { Code_BorderColor = ColorSerializationHelper.FromString(value); }
        }
        private HorizontalAlignment _Code_HorizontalAlignment = HorizontalAlignment.Left;
        public HorizontalAlignment Code_HorizontalAlignment { get => _Code_HorizontalAlignment; set => _Code_HorizontalAlignment = value; }
        #endregion
        #region Label
        private string _Label_Title = "";
        public string Label_Title { get => _Label_Title; set => _Label_Title = value; }
        private string _Label = "";
        public string Label { get => _Label; set => _Label = value; }
        [JsonIgnore]
        public Font Label_font
        {
            get
            {
                return FontSerializationHelper.FromString(_Label_font_Serialize);
            }
            set
            {
                _Label_font_Serialize = FontSerializationHelper.ToString(value);
            }
        }
        private string _Label_font_Serialize = "微軟正黑體:14:Bold:Point:1:False";
        [Browsable(false)]
        public string Label_font_Serialize
        {
            get { return _Label_font_Serialize; }
            set { _Label_font_Serialize = value; }
        }
        [JsonIgnore]
        public Color Label_BackColor = Color.White;
        [Browsable(false)]
        public string Label_BackColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(Label_BackColor); }
            set { Label_BackColor = ColorSerializationHelper.FromString(value); }
        }
        [JsonIgnore]
        public Color Label_ForeColor = Color.Black;
        [Browsable(false)]
        public string Label_ForeColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(Label_ForeColor); }
            set { Label_ForeColor = ColorSerializationHelper.FromString(value); }
        }
        private Point _Label_Position = new Point();
        public Point Label_Position { get => _Label_Position; set => _Label_Position = value; }
        private bool _Label_Visable = true;
        public bool Label_Visable { get => _Label_Visable; set => _Label_Visable = value; }
        private int _Label_Width = 0;
        public int Label_Width { get => _Label_Width; set => _Label_Width = value; }
        private int _Label_Height = 0;
        public int Label_Height { get => _Label_Height; set => _Label_Height = value; }
        private int _Label_BorderSize = 2;
        public int Label_BorderSize { get => _Label_BorderSize; set => _Label_BorderSize = value; }
        private int _Label_BorderRadius = 0;
        public int Label_BorderRadius { get => _Label_BorderRadius; set => _Label_BorderRadius = value; }
        [JsonIgnore]
        public Color Label_BorderColor = Color.Black;
        [Browsable(false)]
        public string Label_BorderColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(Label_BorderColor); }
            set { Label_BorderColor = ColorSerializationHelper.FromString(value); }
        }
        private HorizontalAlignment _Label_HorizontalAlignment = HorizontalAlignment.Left;
        public HorizontalAlignment Label_HorizontalAlignment { get => _Label_HorizontalAlignment; set => _Label_HorizontalAlignment = value; }
        #endregion
        #region Validity_period
        private string _Validity_period_Title = "";
        public string Validity_period_Title { get => _Validity_period_Title; set => _Validity_period_Title = value; }
        private string _Validity_period = "";
        public string Validity_period { get => _Validity_period; set => _Validity_period = value; }
        [JsonIgnore]
        public Font Validity_period_font
        {
            get
            {
                return FontSerializationHelper.FromString(_Validity_period_font_Serialize);
            }
            set
            {
                _Validity_period_font_Serialize = FontSerializationHelper.ToString(value);
            }
        }
        private string _Validity_period_font_Serialize = "微軟正黑體:14:Bold:Point:1:False";
        [Browsable(false)]
        public string Validity_period_font_Serialize
        {
            get { return _Validity_period_font_Serialize; }
            set { _Validity_period_font_Serialize = value; }
        }
        [JsonIgnore]
        public Color Validity_period_BackColor = Color.White;
        [Browsable(false)]
        public string Validity_period_BackColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(Validity_period_BackColor); }
            set { Validity_period_BackColor = ColorSerializationHelper.FromString(value); }
        }
        [JsonIgnore]
        public Color Validity_period_ForeColor = Color.Black;
        [Browsable(false)]
        public string Validity_period_ForeColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(Validity_period_ForeColor); }
            set { Validity_period_ForeColor = ColorSerializationHelper.FromString(value); }
        }
        private Point _Validity_period_Position = new Point();
        public Point Validity_period_Position { get => _Validity_period_Position; set => _Validity_period_Position = value; }
        private bool _Validity_period_Visable = true;
        public bool Validity_period_Visable { get => _Validity_period_Visable; set => _Validity_period_Visable = value; }
        private int _Validity_period_Width = 0;
        public int Validity_period_Width { get => _Validity_period_Width; set => _Validity_period_Width = value; }
        private int _Validity_period_Height = 0;
        public int Validity_period_Height { get => _Validity_period_Height; set => _Validity_period_Height = value; }
        private int _Validity_period_BorderSize = 2;
        public int Validity_period_BorderSize { get => _Validity_period_BorderSize; set => _Validity_period_BorderSize = value; }
        private int _Validity_period_BorderRadius = 0;
        public int Validity_period_BorderRadius { get => _Validity_period_BorderRadius; set => _Validity_period_BorderRadius = value; }
        [JsonIgnore]
        public Color Validity_period_BorderColor = Color.Black;
        [Browsable(false)]
        public string Validity_period_BorderColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(Validity_period_BorderColor); }
            set { Validity_period_BorderColor = ColorSerializationHelper.FromString(value); }
        }
        private HorizontalAlignment _Validity_period_HorizontalAlignment = HorizontalAlignment.Left;
        public HorizontalAlignment Validity_period_HorizontalAlignment { get => _Validity_period_HorizontalAlignment; set => _Validity_period_HorizontalAlignment = value; }
        #endregion
        #region StorageName
        private string _StorageName_Title = "";
        public string StorageName_Title { get => _StorageName_Title; set => _StorageName_Title = value; }
    
       [JsonIgnore]
        public Font StorageName_font
        {
            get
            {
                return FontSerializationHelper.FromString(_StorageName_font_Serialize);
            }
            set
            {
                _StorageName_font_Serialize = FontSerializationHelper.ToString(value);
            }
        }
        private string _StorageName_font_Serialize = "微軟正黑體:14:Bold:Point:1:False";
        [Browsable(false)]
        public string StorageName_font_Serialize
        {
            get { return _StorageName_font_Serialize; }
            set { _StorageName_font_Serialize = value; }
        }
        [JsonIgnore]
        public Color StorageName_BackColor = Color.White;
        [Browsable(false)]
        public string StorageName_BackColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(StorageName_BackColor); }
            set { StorageName_BackColor = ColorSerializationHelper.FromString(value); }
        }
        [JsonIgnore]
        public Color StorageName_ForeColor = Color.Black;
        [Browsable(false)]
        public string StorageName_ForeColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(StorageName_ForeColor); }
            set { StorageName_ForeColor = ColorSerializationHelper.FromString(value); }
        }
        private Point _StorageName_Position = new Point();
        public Point StorageName_Position { get => _StorageName_Position; set => _StorageName_Position = value; }
        private bool _StorageName_Visable = true;
        public bool StorageName_Visable { get => _StorageName_Visable; set => _StorageName_Visable = value; }
        private int _StorageName_Width = 0;
        public int StorageName_Width { get => _StorageName_Width; set => _StorageName_Width = value; }
        private int _StorageName_Height = 0;
        public int StorageName_Height { get => _StorageName_Height; set => _StorageName_Height = value; }
        private int _StorageName_BorderSize = 2;
        public int StorageName_BorderSize { get => _StorageName_BorderSize; set => _StorageName_BorderSize = value; }
        private int _StorageName_BorderRadius = 0;
        public int StorageName_BorderRadius { get => _StorageName_BorderRadius; set => _StorageName_BorderRadius = value; }
        [JsonIgnore]
        public Color StorageName_BorderColor = Color.Black;
        [Browsable(false)]
        public string StorageName_BorderColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(StorageName_BorderColor); }
            set { StorageName_BorderColor = ColorSerializationHelper.FromString(value); }
        }
        private HorizontalAlignment _StorageName_HorizontalAlignment = HorizontalAlignment.Left;
        public HorizontalAlignment StorageName_HorizontalAlignment { get => _StorageName_HorizontalAlignment; set => _StorageName_HorizontalAlignment = value; }
        #endregion
        #region MinPackage
        private string _MinPackage_Title = "";
        public string MinPackage_Title { get => _MinPackage_Title; set => _MinPackage_Title = value; }
        private string _MinPackage = "";
        public string MinPackage { get => _MinPackage; set => _MinPackage = value; }
        [JsonIgnore]
        public Font MinPackage_font
        {
            get
            {
                return FontSerializationHelper.FromString(_MinPackage_font_Serialize);
            }
            set
            {
                _MinPackage_font_Serialize = FontSerializationHelper.ToString(value);
            }
        }
        private string _MinPackage_font_Serialize = "微軟正黑體:14:Bold:Point:1:False";
        [Browsable(false)]
        public string MinPackage_font_Serialize
        {
            get { return _MinPackage_font_Serialize; }
            set { _MinPackage_font_Serialize = value; }
        }
        [JsonIgnore]
        public Color MinPackage_BackColor = Color.White;
        [Browsable(false)]
        public string MinPackage_BackColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(MinPackage_BackColor); }
            set { MinPackage_BackColor = ColorSerializationHelper.FromString(value); }
        }
        [JsonIgnore]
        public Color MinPackage_ForeColor = Color.Black;
        [Browsable(false)]
        public string MinPackage_ForeColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(MinPackage_ForeColor); }
            set { MinPackage_ForeColor = ColorSerializationHelper.FromString(value); }
        }
        private Point _MinPackage_Position = new Point();
        public Point MinPackage_Position { get => _MinPackage_Position; set => _MinPackage_Position = value; }
        private bool _MinPackage_Visable = true;
        public bool MinPackage_Visable { get => _MinPackage_Visable; set => _MinPackage_Visable = value; }
        private int _MinPackage_Width = 0;
        public int MinPackage_Width { get => _MinPackage_Width; set => _MinPackage_Width = value; }
        private int _MinPackage_Height = 0;
        public int MinPackage_Height { get => _MinPackage_Height; set => _MinPackage_Height = value; }
        private int _MinPackage_BorderSize = 2;
        public int MinPackage_BorderSize { get => _MinPackage_BorderSize; set => _MinPackage_BorderSize = value; }
        private int _MinPackage_BorderRadius = 0;
        public int MinPackage_BorderRadius { get => _MinPackage_BorderRadius; set => _MinPackage_BorderRadius = value; }
        [JsonIgnore]
        public Color MinPackage_BorderColor = Color.Black;
        [Browsable(false)]
        public string MinPackage_BorderColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(MinPackage_BorderColor); }
            set { MinPackage_BorderColor = ColorSerializationHelper.FromString(value); }
        }
        private HorizontalAlignment _MinPackage_HorizontalAlignment = HorizontalAlignment.Left;
        public HorizontalAlignment MinPackage_HorizontalAlignment { get => _MinPackage_HorizontalAlignment; set => _MinPackage_HorizontalAlignment = value; }
        #endregion
        #region Min_Package_Num
        private string _Min_Package_Num_Title = "";
        public string Min_Package_Num_Title { get => _Min_Package_Num_Title; set => _Min_Package_Num_Title = value; }

        [JsonIgnore]
        public Font Min_Package_Num_font
        {
            get
            {
                return FontSerializationHelper.FromString(_Min_Package_Num_font_Serialize);
            }
            set
            {
                _Min_Package_Num_font_Serialize = FontSerializationHelper.ToString(value);
            }
        }
        private string _Min_Package_Num_font_Serialize = "微軟正黑體:14:Bold:Point:1:False";
        [Browsable(false)]
        public string Min_Package_Num_font_Serialize
        {
            get { return _Min_Package_Num_font_Serialize; }
            set { _Min_Package_Num_font_Serialize = value; }
        }
        [JsonIgnore]
        public Color Min_Package_Num_BackColor = Color.White;
        [Browsable(false)]
        public string Min_Package_Num_BackColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(Min_Package_Num_BackColor); }
            set { Min_Package_Num_BackColor = ColorSerializationHelper.FromString(value); }
        }
        [JsonIgnore]
        public Color Min_Package_Num_ForeColor = Color.Black;
        [Browsable(false)]
        public string Min_Package_Num_ForeColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(Min_Package_Num_ForeColor); }
            set { Min_Package_Num_ForeColor = ColorSerializationHelper.FromString(value); }
        }
        private Point _Min_Package_Num_Position = new Point();
        public Point Min_Package_Num_Position { get => _Min_Package_Num_Position; set => _Min_Package_Num_Position = value; }
        private bool _Min_Package_Num_Visable = true;
        public bool Min_Package_Num_Visable { get => _Min_Package_Num_Visable; set => _Min_Package_Num_Visable = value; }
        private int _Min_Package_Num_Width = 0;
        public int Min_Package_Num_Width { get => _Min_Package_Num_Width; set => _Min_Package_Num_Width = value; }
        private int _Min_Package_Num_Height = 0;
        public int Min_Package_Num_Height { get => _Min_Package_Num_Height; set => _Min_Package_Num_Height = value; }
        private int _Min_Package_Num_BorderSize = 2;
        public int Min_Package_Num_BorderSize { get => _Min_Package_Num_BorderSize; set => _Min_Package_Num_BorderSize = value; }
        private int _Min_Package_Num_BorderRadius = 0;
        public int Min_Package_Num_BorderRadius { get => _Min_Package_Num_BorderRadius; set => _Min_Package_Num_BorderRadius = value; }
        [JsonIgnore]
        public Color Min_Package_Num_BorderColor = Color.Black;
        [Browsable(false)]
        public string Min_Package_Num_BorderColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(Min_Package_Num_BorderColor); }
            set { Min_Package_Num_BorderColor = ColorSerializationHelper.FromString(value); }
        }
        private HorizontalAlignment _Min_Package_Num_HorizontalAlignment = HorizontalAlignment.Left;
        public HorizontalAlignment Min_Package_Num_HorizontalAlignment { get => _Min_Package_Num_HorizontalAlignment; set => _Min_Package_Num_HorizontalAlignment = value; }
        #endregion
        #region BarCode
        private string _BarCode_Title = "";
        public string BarCode_Title { get => _BarCode_Title; set => _BarCode_Title = value; }

        [JsonIgnore]
        public Font BarCode_font
        {
            get
            {
                return FontSerializationHelper.FromString(_BarCode_font_Serialize);
            }
            set
            {
                _BarCode_font_Serialize = FontSerializationHelper.ToString(value);
            }
        }
        private string _BarCode_font_Serialize = "微軟正黑體:14:Bold:Point:1:False";
        [Browsable(false)]
        public string BarCode_font_Serialize
        {
            get { return _BarCode_font_Serialize; }
            set { _BarCode_font_Serialize = value; }
        }
        [JsonIgnore]
        public Color BarCode_BackColor = Color.White;
        [Browsable(false)]
        public string BarCode_BackColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(BarCode_BackColor); }
            set { BarCode_BackColor = ColorSerializationHelper.FromString(value); }
        }
        [JsonIgnore]
        public Color BarCode_ForeColor = Color.Black;
        [Browsable(false)]
        public string BarCode_ForeColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(BarCode_ForeColor); }
            set { BarCode_ForeColor = ColorSerializationHelper.FromString(value); }
        }
        private Point _BarCode_Position = new Point();
        public Point BarCode_Position { get => _BarCode_Position; set => _BarCode_Position = value; }
        private bool _BarCode_Visable = true;
        public bool BarCode_Visable { get => _BarCode_Visable; set => _BarCode_Visable = value; }
        private int _BarCode_Width = 200;
        public int BarCode_Width { get => _BarCode_Width; set => _BarCode_Width = value; }
        private int _BarCode_Height = 100;
        public int BarCode_Height { get => _BarCode_Height; set => _BarCode_Height = value; }
        private int _BarCode_BorderSize = 2;
        public int BarCode_BorderSize { get => _BarCode_BorderSize; set => _BarCode_BorderSize = value; }
        private int _BarCode_BorderRadius = 0;
        public int BarCode_BorderRadius { get => _BarCode_BorderRadius; set => _BarCode_BorderRadius = value; }
        [JsonIgnore]
        public Color BarCode_BorderColor = Color.Black;
        [Browsable(false)]
        public string BarCode_BorderColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(BarCode_BorderColor); }
            set { BarCode_BorderColor = ColorSerializationHelper.FromString(value); }
        }
        private HorizontalAlignment _BarCode_HorizontalAlignment = HorizontalAlignment.Left;
        public HorizontalAlignment BarCode_HorizontalAlignment { get => _BarCode_HorizontalAlignment; set => _BarCode_HorizontalAlignment = value; }
        #endregion
        #region Inventory
        private string _Inventory_Title = "";
        public string Inventory_Title { get => _Inventory_Title; set => _Inventory_Title = value; }
        private string _Inventory = "";
        [JsonIgnore]
        public Font Inventory_font
        {
            get
            {
                return FontSerializationHelper.FromString(_Inventory_font_Serialize);
            }
            set
            {
                _Inventory_font_Serialize = FontSerializationHelper.ToString(value);
            }
        }
        private string _Inventory_font_Serialize = "微軟正黑體:14:Bold:Point:1:False";
        [Browsable(false)]
        public string Inventory_font_Serialize
        {
            get { return _Inventory_font_Serialize; }
            set { _Inventory_font_Serialize = value; }
        }
        [JsonIgnore]
        public Color Inventory_BackColor = Color.White;
        [Browsable(false)]
        public string Inventory_BackColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(Inventory_BackColor); }
            set { Inventory_BackColor = ColorSerializationHelper.FromString(value); }
        }
        [JsonIgnore]
        public Color Inventory_ForeColor = Color.Black;
        [Browsable(false)]
        public string Inventory_ForeColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(Inventory_ForeColor); }
            set { Inventory_ForeColor = ColorSerializationHelper.FromString(value); }
        }
        private Point _Inventory_Position = new Point();
        public Point Inventory_Position { get => _Inventory_Position; set => _Inventory_Position = value; }
        private bool _Inventory_Visable = true;
        public bool Inventory_Visable { get => _Inventory_Visable; set => _Inventory_Visable = value; }
        private int _Inventory_Width = 0;
        public int Inventory_Width { get => _Inventory_Width; set => _Inventory_Width = value; }
        private int _Inventory_Height = 0;
        public int Inventory_Height { get => _Inventory_Height; set => _Inventory_Height = value; }
        private int _Inventory_BorderSize = 2;
        public int Inventory_BorderSize { get => _Inventory_BorderSize; set => _Inventory_BorderSize = value; }
        private int _Inventory_BorderRadius = 0;
        public int Inventory_BorderRadius { get => _Inventory_BorderRadius; set => _Inventory_BorderRadius = value; }
        [JsonIgnore]
        public Color Inventory_BorderColor = Color.Black;
        [Browsable(false)]
        public string Inventory_BorderColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(Inventory_BorderColor); }
            set { Inventory_BorderColor = ColorSerializationHelper.FromString(value); }
        }
        private HorizontalAlignment _Inventory_HorizontalAlignment = HorizontalAlignment.Left;
        public HorizontalAlignment Inventory_HorizontalAlignment { get => _Inventory_HorizontalAlignment; set => _Inventory_HorizontalAlignment = value; }
        #endregion
        #region Max_Inventory
        private string _Max_Inventory_Title = "";
        public string Max_Inventory_Title { get => _Max_Inventory_Title; set => _Max_Inventory_Title = value; }
        private string _Max_Inventory = "";
        [JsonIgnore]
        public Font Max_Inventory_font
        {
            get
            {
                return FontSerializationHelper.FromString(_Max_Inventory_font_Serialize);
            }
            set
            {
                _Max_Inventory_font_Serialize = FontSerializationHelper.ToString(value);
            }
        }
        private string _Max_Inventory_font_Serialize = "微軟正黑體:14:Bold:Point:1:False";
        [Browsable(false)]
        public string Max_Inventory_font_Serialize
        {
            get { return _Max_Inventory_font_Serialize; }
            set { _Max_Inventory_font_Serialize = value; }
        }
        [JsonIgnore]
        public Color Max_Inventory_BackColor = Color.White;
        [Browsable(false)]
        public string Max_Inventory_BackColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(Max_Inventory_BackColor); }
            set { Max_Inventory_BackColor = ColorSerializationHelper.FromString(value); }
        }
        [JsonIgnore]
        public Color Max_Inventory_ForeColor = Color.Black;
        [Browsable(false)]
        public string Max_Inventory_ForeColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(Max_Inventory_ForeColor); }
            set { Max_Inventory_ForeColor = ColorSerializationHelper.FromString(value); }
        }
        private Point _Max_Inventory_Position = new Point();
        public Point Max_Inventory_Position { get => _Max_Inventory_Position; set => _Max_Inventory_Position = value; }
        private bool _Max_Inventory_Visable = true;
        public bool Max_Inventory_Visable { get => _Max_Inventory_Visable; set => _Max_Inventory_Visable = value; }
        private int _Max_Inventory_Width = 0;
        public int Max_Inventory_Width { get => _Max_Inventory_Width; set => _Max_Inventory_Width = value; }
        private int _Max_Inventory_Height = 0;
        public int Max_Inventory_Height { get => _Max_Inventory_Height; set => _Max_Inventory_Height = value; }
        private int _Max_Inventory_BorderSize = 2;
        public int Max_Inventory_BorderSize { get => _Max_Inventory_BorderSize; set => _Max_Inventory_BorderSize = value; }
        private int _Max_Inventory_BorderRadius = 0;
        public int Max_Inventory_BorderRadius { get => _Max_Inventory_BorderRadius; set => _Max_Inventory_BorderRadius = value; }
        [JsonIgnore]
        public Color Max_Inventory_BorderColor = Color.Black;
        [Browsable(false)]
        public string Max_Inventory_BorderColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(Max_Inventory_BorderColor); }
            set { Max_Inventory_BorderColor = ColorSerializationHelper.FromString(value); }
        }
        private HorizontalAlignment _Max_Inventory_HorizontalAlignment = HorizontalAlignment.Left;
        public HorizontalAlignment Max_Inventory_HorizontalAlignment { get => _Max_Inventory_HorizontalAlignment; set => _Max_Inventory_HorizontalAlignment = value; }
        #endregion
        #region IP
        private string _IP_Title = "";
        public string IP_Title { get => _IP_Title; set => _IP_Title = value; }

        [JsonIgnore]
        public Font IP_font
        {
            get
            {
                return FontSerializationHelper.FromString(_IP_font_Serialize);
            }
            set
            {
                _IP_font_Serialize = FontSerializationHelper.ToString(value);
            }
        }
        private string _IP_font_Serialize = "微軟正黑體:12:Bold:Point:1:False";
        [Browsable(false)]
        public string IP_font_Serialize
        {
            get { return _IP_font_Serialize; }
            set { _IP_font_Serialize = value; }
        }
        [JsonIgnore]
        public Color IP_BackColor = Color.White;
        [Browsable(false)]
        public string IP_BackColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(IP_BackColor); }
            set { IP_BackColor = ColorSerializationHelper.FromString(value); }
        }
        [JsonIgnore]
        public Color IP_ForeColor = Color.Black;
        [Browsable(false)]
        public string IP_ForeColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(IP_ForeColor); }
            set { IP_ForeColor = ColorSerializationHelper.FromString(value); }
        }
        private Point _IP_Position = new Point();
        public Point IP_Position { get => _IP_Position; set => _IP_Position = value; }
        private bool _IP_Visable = true;
        public bool IP_Visable { get => _IP_Visable; set => _IP_Visable = value; }
        private int _IP_Width = 0;
        public int IP_Width { get => _IP_Width; set => _IP_Width = value; }
        private int _IP_Height = 0;
        public int IP_Height { get => _IP_Height; set => _IP_Height = value; }
        private int _IP_BorderSize = 2;
        public int IP_BorderSize { get => _IP_BorderSize; set => _IP_BorderSize = value; }
        [JsonIgnore]
        public Color IP_BorderColor = Color.Black;
        [Browsable(false)]
        public string IP_BorderColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(IP_BorderColor); }
            set { IP_BorderColor = ColorSerializationHelper.FromString(value); }
        }
        private HorizontalAlignment _IP_HorizontalAlignment = HorizontalAlignment.Left;
        public HorizontalAlignment IP_HorizontalAlignment { get => _IP_HorizontalAlignment; set => _IP_HorizontalAlignment = value; }
        #endregion
        #region Port
        private string _Port_Title = "";
        public string Port_Title { get => _Port_Title; set => _Port_Title = value; }
        private int _Port = 4000;
        public int Port { get => _Port; set => _Port = value; }
        [JsonIgnore]
        public Font Port_font
        {
            get
            {
                return FontSerializationHelper.FromString(_Port_font_Serialize);
            }
            set
            {
                _Port_font_Serialize = FontSerializationHelper.ToString(value);
            }
        }
        private string _Port_font_Serialize = "微軟正黑體:12:Bold:Point:1:False";
        [Browsable(false)]
        public string Port_font_Serialize
        {
            get { return _Port_font_Serialize; }
            set { _Port_font_Serialize = value; }
        }
        [JsonIgnore]
        public Color Port_BackColor = Color.White;
        [Browsable(false)]
        public string Port_BackColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(Port_BackColor); }
            set { Port_BackColor = ColorSerializationHelper.FromString(value); }
        }
        [JsonIgnore]
        public Color Port_ForeColor = Color.Black;
        [Browsable(false)]
        public string Port_ForeColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(Port_ForeColor); }
            set { Port_ForeColor = ColorSerializationHelper.FromString(value); }
        }
        private Point _Port_Position = new Point();
        public Point Port_Position { get => _Port_Position; set => _Port_Position = value; }
        private bool _Port_Visable = true;
        public bool Port_Visable { get => _Port_Visable; set => _Port_Visable = value; }
        private int _Port_Width = 0;
        public int Port_Width { get => _Port_Width; set => _Port_Width = value; }
        private int _Port_Height = 0;
        public int Port_Height { get => _Port_Height; set => _Port_Height = value; }
        private int _Port_BorderSize = 2;
        public int Port_BorderSize { get => _Port_BorderSize; set => _Port_BorderSize = value; }
        [JsonIgnore]
        public Color Port_BorderColor = Color.Black;
        [Browsable(false)]
        public string Port_BorderColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(Port_BorderColor); }
            set { Port_BorderColor = ColorSerializationHelper.FromString(value); }
        }
        private HorizontalAlignment _Port_HorizontalAlignment = HorizontalAlignment.Left;
        public HorizontalAlignment Port_HorizontalAlignment { get => _Port_HorizontalAlignment; set => _Port_HorizontalAlignment = value; }
        #endregion
        #region Picture1
        private string _Picture1_Title = "";
        public string Picture1_Title { get => _Picture1_Title; set => _Picture1_Title = value; }
        private string _Picture1_Name = "";
        public string Picture1_Name { get => _Picture1_Name; set => _Picture1_Name = value; }
        [JsonIgnore]
        public Font Picture1_font
        {
            get
            {
                return FontSerializationHelper.FromString(_Picture1_font_Serialize);
            }
            set
            {
                _Picture1_font_Serialize = FontSerializationHelper.ToString(value);
            }
        }
        private string _Picture1_font_Serialize = "微軟正黑體:14:Bold:Point:1:False";
        [Browsable(false)]
        public string Picture1_font_Serialize
        {
            get { return _Picture1_font_Serialize; }
            set { _Picture1_font_Serialize = value; }
        }
        [JsonIgnore]
        public Color Picture1_BackColor = Color.White;
        [Browsable(false)]
        public string Picture1_BackColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(Picture1_BackColor); }
            set { Picture1_BackColor = ColorSerializationHelper.FromString(value); }
        }
        [JsonIgnore]
        public Color Picture1_ForeColor = Color.Black;
        [Browsable(false)]
        public string Picture1_ForeColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(Picture1_ForeColor); }
            set { Picture1_ForeColor = ColorSerializationHelper.FromString(value); }
        }
        private Point _Picture1_Position = new Point();
        public Point Picture1_Position { get => _Picture1_Position; set => _Picture1_Position = value; }
        private bool _Picture1_Visable = true;
        public bool Picture1_Visable { get => _Picture1_Visable; set => _Picture1_Visable = value; }
        private int _Picture1_Width = 50;
        public int Picture1_Width { get => _Picture1_Width; set => _Picture1_Width = value; }
        private int _Picture1_Height = 50;
        public int Picture1_Height { get => _Picture1_Height; set => _Picture1_Height = value; }
        private int _Picture1_BorderSize = 1;
        public int Picture1_BorderSize { get => _Picture1_BorderSize; set => _Picture1_BorderSize = value; }
        [JsonIgnore]
        public Color Picture1_BorderColor = Color.Black;
        [Browsable(false)]
        public string Picture1_BorderColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(Picture1_BorderColor); }
            set { Picture1_BorderColor = ColorSerializationHelper.FromString(value); }
        }
        private HorizontalAlignment _Picture1_HorizontalAlignment = HorizontalAlignment.Left;
        public HorizontalAlignment Picture1_HorizontalAlignment { get => _Picture1_HorizontalAlignment; set => _Picture1_HorizontalAlignment = value; }
        #endregion
        #region Picture2
        private string _Picture2_Title = "";
        public string Picture2_Title { get => _Picture2_Title; set => _Picture2_Title = value; }
        private string _Picture2_Name = "";
        public string Picture2_Name { get => _Picture2_Name; set => _Picture2_Name = value; }
        [JsonIgnore]
        public Font Picture2_font
        {
            get
            {
                return FontSerializationHelper.FromString(_Picture2_font_Serialize);
            }
            set
            {
                _Picture2_font_Serialize = FontSerializationHelper.ToString(value);
            }
        }
        private string _Picture2_font_Serialize = "微軟正黑體:14:Bold:Point:1:False";
        [Browsable(false)]
        public string Picture2_font_Serialize
        {
            get { return _Picture2_font_Serialize; }
            set { _Picture2_font_Serialize = value; }
        }
        [JsonIgnore]
        public Color Picture2_BackColor = Color.White;
        [Browsable(false)]
        public string Picture2_BackColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(Picture2_BackColor); }
            set { Picture2_BackColor = ColorSerializationHelper.FromString(value); }
        }
        [JsonIgnore]
        public Color Picture2_ForeColor = Color.Black;
        [Browsable(false)]
        public string Picture2_ForeColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(Picture2_ForeColor); }
            set { Picture2_ForeColor = ColorSerializationHelper.FromString(value); }
        }
        private Point _Picture2_Position = new Point();
        public Point Picture2_Position { get => _Picture2_Position; set => _Picture2_Position = value; }
        private bool _Picture2_Visable = true;
        public bool Picture2_Visable { get => _Picture2_Visable; set => _Picture2_Visable = value; }
        private int _Picture2_Width = 50;
        public int Picture2_Width { get => _Picture2_Width; set => _Picture2_Width = value; }
        private int _Picture2_Height = 50;
        public int Picture2_Height { get => _Picture2_Height; set => _Picture2_Height = value; }
        private int _Picture2_BorderSize = 1;
        public int Picture2_BorderSize { get => _Picture2_BorderSize; set => _Picture2_BorderSize = value; }
        [JsonIgnore]
        public Color Picture2_BorderColor = Color.Black;
        [Browsable(false)]
        public string Picture2_BorderColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(Picture2_BorderColor); }
            set { Picture2_BorderColor = ColorSerializationHelper.FromString(value); }
        }
        private HorizontalAlignment _Picture2_HorizontalAlignment = HorizontalAlignment.Left;
        public HorizontalAlignment Picture2_HorizontalAlignment { get => _Picture2_HorizontalAlignment; set => _Picture2_HorizontalAlignment = value; }
        #endregion
        #region CustomText1
        private string _CustomText1_Title = "";
        public string CustomText1_Title { get => _CustomText1_Title; set => _CustomText1_Title = value; }

        [JsonIgnore]
        public Font CustomText1_font
        {
            get
            {
                return FontSerializationHelper.FromString(_CustomText1_font_Serialize);
            }
            set
            {
                _CustomText1_font_Serialize = FontSerializationHelper.ToString(value);
            }
        }
        private string _CustomText1_font_Serialize = "微軟正黑體:14:Bold:Point:1:False";
        [Browsable(false)]
        public string CustomText1_font_Serialize
        {
            get { return _CustomText1_font_Serialize; }
            set { _CustomText1_font_Serialize = value; }
        }
        [JsonIgnore]
        public Color CustomText1_BackColor = Color.White;
        [Browsable(false)]
        public string CustomText1_BackColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(CustomText1_BackColor); }
            set { CustomText1_BackColor = ColorSerializationHelper.FromString(value); }
        }
        [JsonIgnore]
        public Color CustomText1_ForeColor = Color.Black;
        [Browsable(false)]
        public string CustomText1_ForeColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(CustomText1_ForeColor); }
            set { CustomText1_ForeColor = ColorSerializationHelper.FromString(value); }
        }
        private Point _CustomText1_Position = new Point();
        public Point CustomText1_Position { get => _CustomText1_Position; set => _CustomText1_Position = value; }
        private bool _CustomText1_Visable = true;
        public bool CustomText1_Visable { get => _CustomText1_Visable; set => _CustomText1_Visable = value; }
        private int _CustomText1_Width = 0;
        public int CustomText1_Width { get => _CustomText1_Width; set => _CustomText1_Width = value; }
        private int _CustomText1_Height = 0;
        public int CustomText1_Height { get => _CustomText1_Height; set => _CustomText1_Height = value; }
        private int _CustomText1_BorderSize = 2;
        public int CustomText1_BorderSize { get => _CustomText1_BorderSize; set => _CustomText1_BorderSize = value; }
        private int _CustomText1_BorderRadius = 0;
        public int CustomText1_BorderRadius { get => _CustomText1_BorderRadius; set => _CustomText1_BorderRadius = value; }
        [JsonIgnore]
        public Color CustomText1_BorderColor = Color.Black;
        [Browsable(false)]
        public string CustomText1_BorderColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(CustomText1_BorderColor); }
            set { CustomText1_BorderColor = ColorSerializationHelper.FromString(value); }
        }
        private HorizontalAlignment _CustomText1_HorizontalAlignment = HorizontalAlignment.Left;
        public HorizontalAlignment CustomText1_HorizontalAlignment { get => _CustomText1_HorizontalAlignment; set => _CustomText1_HorizontalAlignment = value; }
        #endregion
        #region CustomText2
        private string _CustomText2_Title = "";
        public string CustomText2_Title { get => _CustomText2_Title; set => _CustomText2_Title = value; }

        [JsonIgnore]
        public Font CustomText2_font
        {
            get
            {
                return FontSerializationHelper.FromString(_CustomText2_font_Serialize);
            }
            set
            {
                _CustomText2_font_Serialize = FontSerializationHelper.ToString(value);
            }
        }
        private string _CustomText2_font_Serialize = "微軟正黑體:14:Bold:Point:1:False";
        [Browsable(false)]
        public string CustomText2_font_Serialize
        {
            get { return _CustomText2_font_Serialize; }
            set { _CustomText2_font_Serialize = value; }
        }
        [JsonIgnore]
        public Color CustomText2_BackColor = Color.White;
        [Browsable(false)]
        public string CustomText2_BackColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(CustomText2_BackColor); }
            set { CustomText2_BackColor = ColorSerializationHelper.FromString(value); }
        }
        [JsonIgnore]
        public Color CustomText2_ForeColor = Color.Black;
        [Browsable(false)]
        public string CustomText2_ForeColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(CustomText2_ForeColor); }
            set { CustomText2_ForeColor = ColorSerializationHelper.FromString(value); }
        }
        private Point _CustomText2_Position = new Point();
        public Point CustomText2_Position { get => _CustomText2_Position; set => _CustomText2_Position = value; }
        private bool _CustomText2_Visable = true;
        public bool CustomText2_Visable { get => _CustomText2_Visable; set => _CustomText2_Visable = value; }
        private int _CustomText2_Width = 0;
        public int CustomText2_Width { get => _CustomText2_Width; set => _CustomText2_Width = value; }
        private int _CustomText2_Height = 0;
        public int CustomText2_Height { get => _CustomText2_Height; set => _CustomText2_Height = value; }
        private int _CustomText2_BorderSize = 2;
        public int CustomText2_BorderSize { get => _CustomText2_BorderSize; set => _CustomText2_BorderSize = value; }
        private int _CustomText2_BorderRadius = 0;
        public int CustomText2_BorderRadius { get => _CustomText2_BorderRadius; set => _CustomText2_BorderRadius = value; }
        [JsonIgnore]
        public Color CustomText2_BorderColor = Color.Black;
        [Browsable(false)]
        public string CustomText2_BorderColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(CustomText2_BorderColor); }
            set { CustomText2_BorderColor = ColorSerializationHelper.FromString(value); }
        }
        private HorizontalAlignment _CustomText2_HorizontalAlignment = HorizontalAlignment.Left;
        public HorizontalAlignment CustomText2_HorizontalAlignment { get => _CustomText2_HorizontalAlignment; set => _CustomText2_HorizontalAlignment = value; }
        #endregion
        #region CustomText3
        private string _CustomText3_Title = "";
        public string CustomText3_Title { get => _CustomText3_Title; set => _CustomText3_Title = value; }

        [JsonIgnore]
        public Font CustomText3_font
        {
            get
            {
                return FontSerializationHelper.FromString(_CustomText3_font_Serialize);
            }
            set
            {
                _CustomText3_font_Serialize = FontSerializationHelper.ToString(value);
            }
        }
        private string _CustomText3_font_Serialize = "微軟正黑體:14:Bold:Point:1:False";
        [Browsable(false)]
        public string CustomText3_font_Serialize
        {
            get { return _CustomText3_font_Serialize; }
            set { _CustomText3_font_Serialize = value; }
        }
        [JsonIgnore]
        public Color CustomText3_BackColor = Color.White;
        [Browsable(false)]
        public string CustomText3_BackColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(CustomText3_BackColor); }
            set { CustomText3_BackColor = ColorSerializationHelper.FromString(value); }
        }
        [JsonIgnore]
        public Color CustomText3_ForeColor = Color.Black;
        [Browsable(false)]
        public string CustomText3_ForeColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(CustomText3_ForeColor); }
            set { CustomText3_ForeColor = ColorSerializationHelper.FromString(value); }
        }
        private Point _CustomText3_Position = new Point();
        public Point CustomText3_Position { get => _CustomText3_Position; set => _CustomText3_Position = value; }
        private bool _CustomText3_Visable = true;
        public bool CustomText3_Visable { get => _CustomText3_Visable; set => _CustomText3_Visable = value; }
        private int _CustomText3_Width = 0;
        public int CustomText3_Width { get => _CustomText3_Width; set => _CustomText3_Width = value; }
        private int _CustomText3_Height = 0;
        public int CustomText3_Height { get => _CustomText3_Height; set => _CustomText3_Height = value; }
        private int _CustomText3_BorderSize = 2;
        public int CustomText3_BorderSize { get => _CustomText3_BorderSize; set => _CustomText3_BorderSize = value; }
        private int _CustomText3_BorderRadius = 0;
        public int CustomText3_BorderRadius { get => _CustomText3_BorderRadius; set => _CustomText3_BorderRadius = value; }
        [JsonIgnore]
        public Color CustomText3_BorderColor = Color.Black;
        [Browsable(false)]
        public string CustomText3_BorderColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(CustomText3_BorderColor); }
            set { CustomText3_BorderColor = ColorSerializationHelper.FromString(value); }
        }
        private HorizontalAlignment _CustomText3_HorizontalAlignment = HorizontalAlignment.Left;
        public HorizontalAlignment CustomText3_HorizontalAlignment { get => _CustomText3_HorizontalAlignment; set => _CustomText3_HorizontalAlignment = value; }
        #endregion

        [JsonIgnore]
        public Color BackColor = Color.White;
        [Browsable(false)]
        public string BackColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(BackColor); }
            set { BackColor = ColorSerializationHelper.FromString(value); }
        }
        [JsonIgnore]
        public Color ForeColor = Color.Black;
        [Browsable(false)]
        public string ForeColor_Serialize
        {
            get { return ColorSerializationHelper.ToString(ForeColor); }
            set { ForeColor = ColorSerializationHelper.FromString(value); }
        }
      
        public bool UpToSQL = false;
        public int station = -1;  
        public int Station { get => station; set => station = value; }   

        private int masterIndex = -1;
        public int MasterIndex { get => masterIndex; set => masterIndex = value; }
        public int Index { get => index; set => index = value; }
       
        private int index = 0;
        public bool LightOn = false;

        virtual public void PasteFormat(object obj)
        {
            if(obj is Device)
            {
                Device device = obj as Device;

                this.BackColor = device.BackColor;
                this.ForeColor = device.ForeColor;
                for (int i = 0; i < new ValueName().GetEnumNames().Length; i++)
                {
                    ValueName valueName = (ValueName)i;
                    for (int k = 0; k < new ValueType().GetEnumNames().Length; k++)
                    {
                        ValueType valueType = (ValueType)k;


                        if (valueName != ValueName.圖片1 && valueName != ValueName.圖片2 )
                        {
                            if (valueType == ValueType.StringValue) continue;
                            if (valueType == ValueType.Value) continue;
                            if (valueType == ValueType.Title) continue;
                        }
                        object value = device.GetValue(valueName, valueType);
                        this.SetValue(valueName, valueType, value);
                    }
                }
                this.Speaker = device.Speaker;
            }
            
        }
        virtual public void Paste(object obj)
        {
            if (obj is Device)
            {
                Device device = obj as Device;

                this.BackColor = device.BackColor;
                this.ForeColor = device.ForeColor;

                for (int i = 0; i < new ValueName().GetEnumNames().Length; i++)
                {
                    for (int k = 0; k < new ValueType().GetEnumNames().Length; k++)
                    {
                        object value = device.GetValue((ValueName)i, (ValueType)k);
                        this.SetValue((ValueName)i, (ValueType)k, value);
                    }
                }
                this.Speaker = device.Speaker;
            }
          
        }
        public void Clear()
        {
            this.Name = "";
            this.ChineseName = "";
            this.Package = "";
            this.Scientific_Name = "";
            this.Code = "";
            this.Label = "";
            this.StorageName = "";
            this.MinPackage = "";
            this.Min_Package_Num = "0";
            this.BarCode = "";
            //this.BackColor = Color.White;
            //this.ForeColor = Color.Black;

            this.list_Inventory.Clear();
            this.list_Lot_number.Clear();
            this.list_Validity_period.Clear();
        }


    


    }
    //建立一個排序類別
    public class DateComparerby : IComparer<object[]>
    {
        //實作Compare方法
        //依Speed由小排到大。
        public int Compare(object[] x, object[] y)
        {
            DateTime datetime1 = x[0].ObjectToString().StringToDateTime();
            DateTime datetime2 = y[0].ObjectToString().StringToDateTime();
            return DateTime.Compare(datetime1, datetime2);


        }
    }
}
