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
namespace H_Pannel_lib
{
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
            Parallel.ForEach(deviceBasicTables, value =>
            {
                string jsonString = value[(int)enum_DeviceTable.Value].ObjectToString();
                DeviceBasic deviceBasic = jsonString.JsonDeserializet<DeviceBasic>();
                if (deviceBasic != null)
                {
                    deviceBasics.LockAdd(deviceBasic);
                }

            });
            deviceBasics = (from value in deviceBasics
                            where value != null
                            select value).ToList();
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
            List<DeviceBasic> DeviceBasics_buf = new List<DeviceBasic>();
            foreach (DeviceBasic deviceBasic in DeviceBasics)
            {
                if (deviceBasic.Code == Code)
                {
                    DeviceBasics_buf.Add(deviceBasic);
                }
            }
            return DeviceBasics_buf;
        }

   
    }
    [Serializable]
    public class DeviceBasicClass
    {
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
            DeviceBasicMethod.SQL_Init(sQLControl);
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
    public class DeviceBasic
    {
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
        public DeviceType DeviceType { get => deviceType; set => deviceType = value; }
        private DeviceType deviceType = DeviceType.None;
        private string _Code = "";
        public string Code { get => _Code; set => _Code = value; }
        private string _StorageName = "";
        public string StorageName { get => _StorageName; set => _StorageName = value; }
        private string _IP = "";
        public string IP { get => _IP; set => _IP = value; }
        public DeviceBasic()
        {
            this.GUID = Guid.NewGuid().ToString();
        }

        public string Inventory
        {
            get
            {
                return this.取得庫存().ToString();
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


        public void 效期庫存異動(string 效期, int 異動量)
        {
            this.效期庫存異動(效期, 異動量.ToString());
        }
        public void 效期庫存異動(string 效期, string 異動量)
        {
            if (this.List_Validity_period == null) this.List_Validity_period = new List<string>();
            if (this.List_Inventory == null) this.List_Inventory = new List<string>();
            if (this.List_Lot_number == null) this.List_Lot_number = new List<string>();
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
            if (異動量.StringToInt32() == 0) return;
            if (!效期.Check_Date_String()) return;
            if (this.List_Validity_period == null) this.List_Validity_period = new List<string>();
            if (this.List_Inventory == null) this.List_Inventory = new List<string>();
            if (this.List_Lot_number == null) this.List_Lot_number = new List<string>();
            for (int i = 0; i < this.List_Validity_period.Count; i++)
            {
                if (this.List_Validity_period[i].ToDateString("/") == 效期.ToDateString("/"))
                {
                    int 現有庫存 = this.List_Inventory[i].StringToInt32();
                    int 異動庫存 = 異動量.StringToInt32();
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
            if (異動量.StringToInt32() < 0) return;
            if (!效期.Check_Date_String()) return;
            if (this.List_Validity_period == null) this.List_Validity_period = new List<string>();
            if (this.List_Inventory == null) this.List_Inventory = new List<string>();
            if (this.List_Lot_number == null) this.List_Lot_number = new List<string>();
            for (int i = 0; i < this.List_Validity_period.Count; i++)
            {
                if (this.List_Validity_period[i].ToDateString("/") == 效期.ToDateString("/"))
                {
                    int 異動庫存 = 異動量.StringToInt32();
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
        public void 確認效期庫存()
        {
            this.確認效期庫存(false);
        }
        public void 確認效期庫存(bool ClearAll)
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

            int 庫存 = 0;
            for (int i = 0; i < this.list_Validity_period.Count; i++)
            {
                庫存 = this.list_Inventory[i].StringToInt32();
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
        public void 新增效期(string 效期, string 庫存)
        {
            this.新增效期(效期, 效期, 庫存);
        }
        public void 新增效期(string 效期, string 批號, string 庫存)
        {
            if (庫存.StringToInt32() == -1 && 庫存 != "00") return;
            if (庫存.StringToInt32() < 0 && 庫存 != "00") return;
            if (!效期.Check_Date_String()) return;
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
        public int 庫存異動(int 總異動量, out List<string> 效期, out List<string> 異動量)
        {
            return this.庫存異動(總異動量.ToString(), out 效期, out 異動量, true);
        }
        public int 庫存異動(string 總異動量, out List<string> 效期, out List<string> 異動量, bool 要寫入)
        {
            效期 = new List<string>();
            異動量 = new List<string>();
            if (總異動量.StringIsEmpty()) return -1;
            if (總異動量.StringToInt32() == 0) return -1;
            int int_總異動量 = 總異動量.StringToInt32();
            int 總庫存 = 取得庫存();
            int 剩餘庫存數量 = 0;
            int 效期庫存 = 0;
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
                        異動量.Add(int_總異動量.ToString());
                        break;
                    }
                    else
                    {
                        效期.Add(this.List_Validity_period[i]);
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
                int_總異動量 += 異動量[i].StringToInt32();
            }
            return int_總異動量;


        }
        public int 取得庫存(string 效期)
        {
            if (!效期.Check_Date_String()) return -1;
            if (this.List_Validity_period == null) this.List_Validity_period = new List<string>();
            if (this.List_Inventory == null) this.List_Inventory = new List<string>();
            if (this.List_Lot_number == null) this.List_Lot_number = new List<string>();
            for (int i = 0; i < this.List_Validity_period.Count; i++)
            {
                if (this.List_Validity_period[i].ToDateString("/") == 效期.ToDateString("/"))
                {
                    return this.List_Inventory[i].StringToInt32();
                }
            }
            return -1;
        }
        public string 取得批號(string 效期)
        {
            if (!效期.Check_Date_String()) return "";
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
        public int 取得庫存()
        {
            int 庫存 = 0;
            int temp = 0;
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
        public void 清除所有庫存資料()
        {
            list_Validity_period.Clear();
            list_Lot_number.Clear();
            list_Inventory.Clear();
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
            List<object[]> deviceTables = sQLControl.GetAllRows(null);
            List<Device> devices = new List<Device>();
            Parallel.ForEach(deviceTables, value =>
            {
                string jsonString = value[(int)enum_DeviceTable.Value].ObjectToString();
                Device device = jsonString.JsonDeserializet<Device>();
                if (device != null)
                {
                    device.Port = value[(int)enum_DeviceTable.Port].ObjectToString().StringToInt32();
                    devices.LockAdd(device);
                }

            });
            devices = (from value in devices
                       where value != null
                       select value).ToList();
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
            Parallel.ForEach(deviceTables, value =>
            {
                string jsonString = value[(int)enum_DeviceTable.Value].ObjectToString();
                Device device = jsonString.JsonDeserializet<Device>();
                if (device != null)
                {
                    device.Port = value[(int)enum_DeviceTable.Port].ObjectToString().StringToInt32();
                    devices.LockAdd(device);
                }

            });
            devices = (from value in devices
                        where value != null
                        select value).ToList();
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
        Pannel35= 7,
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
            儲位名稱,
            IP,
            Port,
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
            public Color BorderColor = Color.Black;
            public bool Visable = false;

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
                case ValueName.庫存:
                    {
                        if (valueType == ValueType.Value)
                        {
                            //if (Value.StringToInt32() < 0) return;
                            //this.新增效期(Default_Validity_period.ToDateString(), (string)Value);
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
                        vlaueClass.BorderColor = this.Name_BorderColor;
                        vlaueClass.Visable = this.Name_Visable;

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
                        vlaueClass.BorderColor = this.ChineseName_BorderColor;
                        vlaueClass.Visable = this.ChineseName_Visable;

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
                        vlaueClass.BorderColor = this.Inventory_BorderColor;
                        vlaueClass.Visable = this.Inventory_Visable;

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
            return vlaueClass;
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
                Bitmap bitmap = Communication.TextToBitmap(vlaueClass.StringValue, vlaueClass.Font, bmp_Scale, vlaueClass.Width, vlaueClass.Height, vlaueClass.ForeColor, vlaueClass.BackColor, vlaueClass.BorderSize, vlaueClass.BorderColor, vlaueClass.HorizontalAlignment);
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

        #region Name
        private string _Name_Title = "";
        public string Name_Title { get => _Name_Title; set => _Name_Title = value; }
        private string _Name = "";
        public string Name { get => _Name; set => _Name = value; }
        [JsonIgnore]
        public Font Name_font = new Font("微軟正黑體", 12, FontStyle.Bold);
        [Browsable(false)]
        public string Name_font_Serialize
        {
            get { return FontSerializationHelper.ToString(Name_font); }
            set { Name_font = FontSerializationHelper.FromString(value); }
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
        private string _ChineseName = "";
        public string ChineseName { get => _ChineseName; set => _ChineseName = value; }
        [JsonIgnore]
        public Font ChineseName_font = new Font("微軟正黑體", 12, FontStyle.Bold);
        [Browsable(false)]
        public string ChineseName_font_Serialize
        {
            get { return FontSerializationHelper.ToString(ChineseName_font); }
            set { ChineseName_font = FontSerializationHelper.FromString(value); }
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
        public Font Package_font = new Font("微軟正黑體", 12, FontStyle.Bold);
        [Browsable(false)]
        public string Package_font_Serialize
        {
            get { return FontSerializationHelper.ToString(Package_font); }
            set { Package_font = FontSerializationHelper.FromString(value); }
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
        private string _Scientific_Name = "";
        public string Scientific_Name { get => _Scientific_Name; set => _Scientific_Name = value; }
        [JsonIgnore]
        public Font Scientific_Name_font = new Font("微軟正黑體", 12, FontStyle.Bold);
        [Browsable(false)]
        public string Scientific_Name_font_Serialize
        {
            get { return FontSerializationHelper.ToString(Scientific_Name_font); }
            set { Scientific_Name_font = FontSerializationHelper.FromString(value); }
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
        public Font Code_font = new Font("微軟正黑體", 12, FontStyle.Bold);
        [Browsable(false)]
        public string Code_font_Serialize
        {
            get { return FontSerializationHelper.ToString(Code_font); }
            set { Code_font = FontSerializationHelper.FromString(value); }
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
        public Font Label_font = new Font("微軟正黑體", 12, FontStyle.Bold);
        [Browsable(false)]
        public string Label_font_Serialize
        {
            get { return FontSerializationHelper.ToString(Label_font); }
            set { Label_font = FontSerializationHelper.FromString(value); }
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
        public Font Validity_period_font = new Font("微軟正黑體", 12, FontStyle.Bold);
        [Browsable(false)]
        public string Validity_period_font_Serialize
        {
            get { return FontSerializationHelper.ToString(Validity_period_font); }
            set { Validity_period_font = FontSerializationHelper.FromString(value); }
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
        public Font StorageName_font = new Font("微軟正黑體", 12, FontStyle.Bold);
        [Browsable(false)]
        public string StorageName_font_Serialize
        {
            get { return FontSerializationHelper.ToString(StorageName_font); }
            set { StorageName_font = FontSerializationHelper.FromString(value); }
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
        public Font MinPackage_font = new Font("微軟正黑體", 12, FontStyle.Bold);
        [Browsable(false)]
        public string MinPackage_font_Serialize
        {
            get { return FontSerializationHelper.ToString(MinPackage_font); }
            set { MinPackage_font = FontSerializationHelper.FromString(value); }
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
        private string _Min_Package_Num = "";
        public string Min_Package_Num { get => _Min_Package_Num; set => _Min_Package_Num = value; }
        [JsonIgnore]
        public Font Min_Package_Num_font = new Font("微軟正黑體", 12, FontStyle.Bold);
        [Browsable(false)]
        public string Min_Package_Num_font_Serialize
        {
            get { return FontSerializationHelper.ToString(Min_Package_Num_font); }
            set { Min_Package_Num_font = FontSerializationHelper.FromString(value); }
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
        private string _BarCode = "";
        public string BarCode { get => _BarCode; set => _BarCode = value; }
        [JsonIgnore]
        public Font BarCode_font = new Font("微軟正黑體", 12, FontStyle.Bold);
        [Browsable(false)]
        public string BarCode_font_Serialize
        {
            get { return FontSerializationHelper.ToString(BarCode_font); }
            set { BarCode_font = FontSerializationHelper.FromString(value); }
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
        public Font Inventory_font = new Font("微軟正黑體", 12, FontStyle.Bold);
        [Browsable(false)]
        public string Inventory_font_Serialize
        {
            get { return FontSerializationHelper.ToString(Inventory_font); }
            set { Inventory_font = FontSerializationHelper.FromString(value); }
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
        #region IP
        private string _IP_Title = "";
        public string IP_Title { get => _IP_Title; set => _IP_Title = value; }

        [JsonIgnore]
        public Font IP_font = new Font("微軟正黑體", 12, FontStyle.Bold);
        [Browsable(false)]
        public string IP_font_Serialize
        {
            get { return FontSerializationHelper.ToString(IP_font); }
            set { IP_font = FontSerializationHelper.FromString(value); }
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
        public Font Port_font = new Font("微軟正黑體", 12, FontStyle.Bold);
        [Browsable(false)]
        public string Port_font_Serialize
        {
            get { return FontSerializationHelper.ToString(Port_font); }
            set { Port_font = FontSerializationHelper.FromString(value); }
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

     
   
        public int MasterIndex { get => masterIndex; set => masterIndex = value; }
        public int Index { get => index; set => index = value; }

    
    
        private int masterIndex = -1;
        private int index = 0;

    

        public void PasteFormat(Device device)
        {
            this.BackColor = device.BackColor;
            this.ForeColor = device.ForeColor;
            for (int i = 0; i < new ValueName().GetEnumNames().Length; i++)
            {
                for (int k = 0; k < new ValueType().GetEnumNames().Length; k++)
                {
                    if ((ValueType)k == ValueType.StringValue) continue;
                    if ((ValueType)k == ValueType.Value) continue;
                    if ((ValueType)k == ValueType.Title) continue;
                    object value = device.GetValue((ValueName)i, (ValueType)k);
                    this.SetValue((ValueName)i, (ValueType)k, value);
                }
            }
        }
        public void Paste(Device device)
        {
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
