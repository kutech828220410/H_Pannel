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
        private string outputAdress = "";
        private string inputAdress = "";
        private string master_GUID = "";

        private bool tOFON = false;

        public byte[] LED_Bytes = new byte[450 * 3];

        public string OutputAdress { get => outputAdress; set => outputAdress = value; }
        public string InputAdress { get => inputAdress; set => inputAdress = value; }
        public string Master_GUID { get => master_GUID; set => master_GUID = value; }
        public bool TOFON { get => tOFON; set => tOFON = value; }

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

    }
    static public class StorageMethod
    {
        static public List<Storage> SQL_GetAllStorage(SQLUI.SQLControl sQLControl)
        {
            List<object[]> deviceTables = sQLControl.GetAllRows(null);
            List<Storage> storages = new List<Storage>();
            Parallel.ForEach(deviceTables, value =>
            {
                string jsonString = value[(int)enum_DeviceTable.Value].ObjectToString();
                Storage storage = jsonString.JsonDeserializet<Storage>();
                if (storage != null)
                {
                    storage.Port = value[(int)enum_DeviceTable.Port].ObjectToString().StringToInt32();
                    storages.LockAdd(storage);
                }

            });
            storages = (from value in storages
                        where value != null
                        select value).ToList();
            return storages;
        }
        static public List<Storage> SQL_GetAllStorage(List<object[]> deviceTables)
        {
            List<Storage> storages = new List<Storage>();
            Parallel.ForEach(deviceTables, value =>
            {
                string jsonString = value[(int)enum_DeviceTable.Value].ObjectToString();
                Storage storage = jsonString.JsonDeserializet<Storage>();
                if (storage != null)
                {
                    storage.Port = value[(int)enum_DeviceTable.Port].ObjectToString().StringToInt32();
                    storages.LockAdd(storage);
                }

            });
            storages = (from value in storages
                        where value != null
                        select value).ToList();
            return storages;
        }
        public static List<DeviceBasic> GetAllDeviceBasic(List<object[]> deviceTables)
        {
            List<DeviceBasic> deviceBasics = new List<DeviceBasic>();
            List<object[]> list_value = deviceTables;
            Parallel.ForEach(list_value, value =>
            {
                string jsonString = value[(int)enum_DeviceTable.Value].ObjectToString();
                DeviceBasic deviceBasic = jsonString.JsonDeserializet<DeviceBasic>();
                if (deviceBasic != null)
                {
                    deviceBasic.確認效期庫存(true);
                    deviceBasics.LockAdd(deviceBasic);
                }

            });
            deviceBasics = (from value in deviceBasics
                            where value != null
                        select value).ToList();
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
            List<Storage> storages = new List<Storage>();
            foreach (Storage storage in Storages)
            {
                if (storage.Code.ToUpper() == Code.ToUpper())
                {
                    storages.Add(storage);
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
    }
}
