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
    public class Storage : Device
    {
        public enum enum_DrawType
        {
            type1,
            constom,
        }
    
        private string outputAdress = "";
        private string inputAdress = "";
        private bool alarmEnable = false;
        private bool tOFON = false;
        private enum_DrawType _enum_drawType = Storage.enum_DrawType.type1;

        public byte[] LED_Bytes = new byte[450 * 3];
        [JsonIgnore]
        public byte[] LED_Bytes_buf = new byte[450 * 3];
        [JsonIgnore]
        public byte[] LED_Bytes_temp = new byte[450 * 3];
        [JsonIgnore]
        public bool UploadLED = false;
        public string OutputAdress { get => outputAdress; set => outputAdress = value; }
        public string InputAdress { get => inputAdress; set => inputAdress = value; }      
        public bool TOFON { get => tOFON; set => tOFON = value; }
        public bool AlarmEnable { get => alarmEnable; set => alarmEnable = value; }
        public enum_DrawType Enum_drawType { get => _enum_drawType; set => _enum_drawType = value; }

        public bool ActionDone = false;
        public Storage(int station)
        {
            this.station = station;
            this.SetDeviceType(DeviceType.EPD266_lock);
        }
        public Storage(string IP, int Port)
        {
            this.IP = IP;
            this.Port = Port;
            this.SetDeviceType(DeviceType.EPD266_lock);
        }
        public Storage()
        {
            this.SetDeviceType(DeviceType.EPD266_lock);
        }
        public void SetDeviceType(DeviceType deviceType)
        {
            this.DeviceType = deviceType;
        }

        public override void PasteFormat(object obj)
        {
            if(obj is Storage)
            {
                Storage storage = obj as Storage;
               this.Enum_drawType = storage.Enum_drawType;
            }
            base.PasteFormat(obj);
        }
    }
    static public class StorageMethod
    {
        static public System.Collections.Generic.Dictionary<string, List<Storage>> CoverToDictionaryByCode(this List<Storage> DeviceBasics)
        {
            Dictionary<string, List<Storage>> dictionary = new Dictionary<string, List<Storage>>();

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
                    List<Storage> values = new List<Storage> { item };
                    dictionary[key] = values;
                }
            }

            return dictionary;
        }
        static public List<Storage> SortDictionaryByCode(this System.Collections.Generic.Dictionary<string, List<Storage>> dictionary, string code)
        {
            if (dictionary.ContainsKey(code))
            {
                return dictionary[code];
            }
            return new List<Storage>();
        }
        static public System.Collections.Generic.Dictionary<string, List<Storage>> CoverToDictionaryByIP(this List<Storage> DeviceBasics)
        {
            Dictionary<string, List<Storage>> dictionary = new Dictionary<string, List<Storage>>();

            foreach (var item in DeviceBasics)
            {
                string key = item.IP;

                // 如果字典中已經存在該索引鍵，則將值添加到對應的列表中
                if (dictionary.ContainsKey(key))
                {
                    dictionary[key].Add(item);
                }
                // 否則創建一個新的列表並添加值
                else
                {
                    List<Storage> values = new List<Storage> { item };
                    dictionary[key] = values;
                }
            }

            return dictionary;
        }
        static public List<Storage> SortDictionaryByIP(this System.Collections.Generic.Dictionary<string, List<Storage>> dictionary, string IP)
        {
            if (dictionary.ContainsKey(IP))
            {
                return dictionary[IP];
            }
            return new List<Storage>();
        }

        static public void SQL_ReplaceByIP(SQLUI.SQLControl sQLControl, Storage storage)
        {
            string IP = storage.IP;
            List<object[]> list_value = sQLControl.GetRowsByDefult(null, enum_DeviceTable.IP.GetEnumName(), IP);
            if (list_value.Count == 0) return;
            list_value[0][(int)enum_DeviceTable.Value] = storage.JsonSerializationt<Storage>();
            sQLControl.UpdateByDefulteExtra(null, list_value[0]);
        }
        static public Storage SQL_GetStorageByIP(SQLUI.SQLControl sQLControl, string IP)
        {
            List<object[]> deviceBasicTables = sQLControl.GetRowsByDefult(null, (int)enum_DeviceTable.IP, IP);
            List<Storage> storages = SQL_GetAllStorage(deviceBasicTables);
            if (storages.Count == 0) return null;
            return storages[0];
        }
        static public List<Storage> SQL_GetAllStorage(SQLUI.SQLControl sQLControl)
        {
            List<object[]> deviceBasicTables = sQLControl.GetAllRows(null);
            return SQL_GetAllStorage(deviceBasicTables);
        }   
        static public List<Storage> SQL_GetAllStorage(List<object[]> deviceTables)
        {
            List<Storage> storages = new List<Storage>();


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
            storages = json_result.JsonDeserializet<List<Storage>>();

            return storages;
        }

        static public List<Storage> SQL_GetStorageByCode(SQLUI.SQLControl sQLControl, string Code)
        {
            List<object[]> deviceBasicTables = sQLControl.GetAllRows(null);
            return SQL_GetStorageByCode(deviceBasicTables , Code);
        }
        static public List<Storage> SQL_GetStorageByCode(List<object[]> deviceTables , string Code)
        {
            Code = Code.Replace("*", "");

            List<Storage> storages = new List<Storage>();

            List<string> json_strings = (from temp in deviceTables
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
            storages = json_result.JsonDeserializet<List<Storage>>();
            storages = (from temp in storages
                        where temp.Code == Code
                        select temp).ToList();
            return storages;
        }

        static public List<DeviceBasic> GetAllDeviceBasic(SQLUI.SQLControl sQLControl)
        {
            List<object[]> deviceBasicTables = sQLControl.GetAllRows(null);
            return GetAllDeviceBasic(deviceBasicTables);
        }
        public static List<DeviceBasic> GetAllDeviceBasic(List<object[]> deviceTables)
        {
            List<DeviceBasic> deviceBasics = new List<DeviceBasic>();
            List<object[]> list_value = deviceTables;


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
            deviceBasics = json_result.JsonDeserializet<List<DeviceBasic>>();


            //Parallel.ForEach(list_value, value =>
            //{
            //    string jsonString = value[(int)enum_DeviceTable.Value].ObjectToString();
            //    DeviceBasic deviceBasic = jsonString.JsonDeserializet<DeviceBasic>();
            //    if (deviceBasic != null)
            //    {
            //        deviceBasic.確認效期庫存(true);
            //        deviceBasics.LockAdd(deviceBasic);
            //    }

            //});
            //deviceBasics = (from value in deviceBasics
            //                where value != null
            //            select value).ToList();
            return deviceBasics;
        }


        static public List<DeviceBasic> GetDeviceBasicByCode(SQLUI.SQLControl sQLControl, string Code)
        {
            List<object[]> deviceBasicTables = sQLControl.GetAllRows(null);
            return GetDeviceBasicByCode(deviceBasicTables ,Code);
        }
        public static List<DeviceBasic> GetDeviceBasicByCode(List<object[]> deviceTables , string Code)
        {
            Code = Code.Replace("*", "");


            List<DeviceBasic> deviceBasics = new List<DeviceBasic>();
            List<object[]> list_value = deviceTables;


            List<string> json_strings = (from temp in deviceTables
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
            deviceBasics = json_result.JsonDeserializet<List<DeviceBasic>>();


            //Parallel.ForEach(list_value, value =>
            //{
            //    string jsonString = value[(int)enum_DeviceTable.Value].ObjectToString();
            //    DeviceBasic deviceBasic = jsonString.JsonDeserializet<DeviceBasic>();
            //    if (deviceBasic != null)
            //    {
            //        deviceBasic.確認效期庫存(true);
            //        deviceBasics.LockAdd(deviceBasic);
            //    }

            //});
            //deviceBasics = (from value in deviceBasics
            //                where value != null
            //            select value).ToList();
            return deviceBasics;
        }


        static public List<Storage> Add_NewStorage(this List<Storage> Storages, string IP, int Port)
        {
            return Storages.Add_NewStorage(IP, Port, new Storage(IP, Port));
        }
        static public List<Storage> Add_NewStorage(this List<Storage> Storages, Storage storage)
        {
            for (int i = 0; i < Storages.Count; i++)
            {
                if (Storages[i].IP == storage.IP)
                {
                    Storages[i] = storage;
                    return Storages;
                }
            }
            Storages.Add(storage);
            return Storages;
        }
        static public List<Storage> Add_NewStorage(this List<Storage> Storages, string IP, int Port, Storage storage)
        {
            storage.IP = IP;
            storage.Port = Port;
            if (Storages.SortByIP(IP) == null)
            {
                Storages.Add(storage);
                return Storages;
            }
            else
            {
                Storages = Storages.ReplacePortByIP(IP, Port);
            }
            return Storages;
        }
        static public List<Storage> GetUpToSQL(this List<Storage> Storages)
        {
            List<Storage> storages = new List<Storage>();
            foreach (Storage storage in Storages)
            {
                if (storage.UpToSQL)
                {
                    storage.UpToSQL = false;
                    storages.Add(storage);
                }
            }
            return storages;
        }
        static public List<Storage> SortByCode(this List<Storage> Storages, string Code)
        {
            bool flag_serch = Code.Contains("*");
            Code = Code.Replace("*", "");

            List<Storage> storages = new List<Storage>();
            foreach (Storage storage in Storages)
            {
                if(flag_serch)
                {
                    if (storage.Code.ToUpper().Contains(Code.ToUpper()))
                    {
                        storages.Add(storage);
                    }
                }
                else
                {
                    if (storage.Code.ToUpper() == Code.ToUpper())
                    {
                        storages.Add(storage);
                    }
                }
               
            }
            return storages;
        }
        static public Storage SortByName(this List<Storage> Storages, string StorageName)
        {
            List<Storage> storages = new List<Storage>();
            foreach (Storage storage in Storages)
            {
                if (storage.StorageName.ToUpper() == StorageName.ToUpper())
                {
                    return storage;
                }
            }
            return null;
        }
        static public List<Storage> SortLikeByCode(this List<Storage> Storages, string Code)
        {
            List<Storage> storages = new List<Storage>();
            foreach (Storage storage in Storages)
            {
                if (storage.Code.ToUpper().Contains(Code.ToUpper()))
                {
                    storages.Add(storage);
                }
            }
            return storages;
        }
        static public List<Storage> SortByBarCode(this List<Storage> Storages, string BarCode)
        {
            List<Storage> storages = new List<Storage>();
            foreach (Storage storage in Storages)
            {
                if (storage.BarCode == BarCode)
                {
                    storages.Add(storage);
                }
            }
            return storages;
        }
        static public Storage SortByIP(this List<Storage> Storages, string IP)
        {
            for (int i = 0; i < Storages.Count; i++)
            {
                if (Storages[i].IP == IP) return Storages[i];
            }
            return null;
        }
        static public Storage SortByGUID(this List<Storage> Storages, string GUID)
        {
            foreach (Storage storage in Storages)
            {
                if (storage.GUID == GUID)
                {
                    return storage;
                }
            }
            return null;
        }

        static public List<Storage> RemoveByIP(this List<Storage> Storages, string IP)
        {
            List<Storage> Storages_buf = new List<Storage>();
            foreach (Storage storage in Storages)
            {
                if (storage.IP != IP)
                {
                    Storages_buf.Add(storage);
                }
            }
            Storages = Storages_buf;
            return Storages;
        }
        static public List<Storage> ReplacePortByIP(this List<Storage> Storages, string IP, int Port)
        {
            for (int i = 0; i < Storages.Count; i++)
            {
                if (Storages[i].IP == IP)
                {
                    Storages[i].IP = IP;
                    Storages[i].Port = Port;
                }
            }
            return Storages;
        }
        static public List<Storage> ReplaceByIP(this List<Storage> Storages, Storage storage)
        {
            for (int i = 0; i < Storages.Count; i++)
            {
                if (Storages[i].IP == storage.IP)
                {
                    Storages[i] = storage;
                    return Storages;
                }
            }
            return Storages;
        }
        static public List<Storage> ClearStorage(this List<Storage> Storages, string IP)
        {
            Storage storage = Storages.SortByIP(IP);
            if (storage != null)
            {
                ClearStorage(storage);
            }
            return Storages;
        }
        static public void ClearStorage(this Storage storage)
        {
            storage.Clear();
        }

        static public List<Device> GetAllDevice(this List<Storage> Storages)
        {
            List<Device> devices = new List<Device>();
            for (int i = 0; i < Storages.Count; i++)
            {
                devices.Add(Storages[i]);
            }
      
            return devices;
        }
        static public List<DeviceBasic> GetAllDeviceBasic(this List<Storage> Storages)
        {
            List<DeviceBasic> devices = new List<DeviceBasic>();
            for (int i = 0; i < Storages.Count; i++)
            {
                devices.Add(Storages[i]);
            }

            return devices;
        }


        static public void ReplaceIP(this Storage storage, string IP)
        {
            storage.ReplaceIP(IP, storage.Port);
        }
        static public void ReplaceIP(this Storage storage, string IP, int Port)
        {
            storage.IP = IP;
            storage.Port = Port;
        }

        static public UDP_Class GetUPDClass(this Storage storage)
        {
            if (storage != null)
            {
                UDP_Class uDP_Class = new UDP_Class(storage.IP, storage.Port);
                return uDP_Class;
            }
            return null;
        }


        static public Storage SetMedClass(this Storage storage, HIS_DB_Lib.medClass medClass)
        {
            storage.Code = medClass.藥品碼;
            storage.SKDIACODE = medClass.料號;
            storage.Name = medClass.藥品名稱;
            storage.Scientific_Name = medClass.藥品學名;
            storage.ChineseName = medClass.中文名稱;
            storage.DRUGKIND = medClass.管制級別;
            storage.Package = medClass.包裝單位;

            return storage;
        }
    }
}
