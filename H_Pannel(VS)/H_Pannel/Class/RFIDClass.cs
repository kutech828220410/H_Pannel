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
    public static class RFIDMethod
    {
        static public List<RFIDClass> SQL_GetAllRFIDClass(List<object[]> deviceTables)
        {
            List<RFIDClass> rFIDClasses = new List<RFIDClass>();
            Parallel.ForEach(deviceTables, value =>
            {
                string jsonString = value[(int)enum_DeviceTable.Value].ObjectToString();
                RFIDClass rFIDClass = jsonString.JsonDeserializet<RFIDClass>();
                if (rFIDClass != null) rFIDClasses.LockAdd(rFIDClass);

            });
            rFIDClasses = (from value in rFIDClasses
                           where value != null
                           select value).ToList();
            return rFIDClasses;
        }

        static public List<RFIDClass> Add_NewRFIDClass(this List<RFIDClass> rFIDClasses, RFIDDevice rFIDDevice)
        {
            if (rFIDDevice == null) return rFIDClasses;
            for (int i = 0; i < rFIDClasses.Count; i++)
            {
                if (rFIDClasses[i].IP == rFIDDevice.IP)
                {
                    rFIDClasses[i] = rFIDClasses[i].ReplaceRFIDDevice(rFIDDevice);
                    return rFIDClasses;
                }
            }
            return rFIDClasses;

        }
        static public List<RFIDClass> Add_NewRFIDClass(this List<RFIDClass> rFIDClasses, RFIDClass rFIDClass)
        {
            for (int i = 0; i < rFIDClasses.Count; i++)
            {
                if (rFIDClasses[i].IP == rFIDClass.IP)
                {
                    rFIDClasses[i] = rFIDClass;
                    return rFIDClasses;
                }
            }
            rFIDClasses.Add(rFIDClass);
            return rFIDClasses;

        }
        static public RFIDClass ReplaceRFIDDevice(this RFIDClass rFIDClass, RFIDDevice rFIDDevice)
        {
            for (int i = 0; i < rFIDClass.DeviceClasses.Length; i++)
            {
                for (int k = 0; k < rFIDClass.DeviceClasses[i].RFIDDevices.Count; k++)
                {
                    if (rFIDClass.DeviceClasses[i].RFIDDevices[k].GUID == rFIDDevice.GUID)
                    {
                        rFIDClass.DeviceClasses[i].RFIDDevices[k] = rFIDDevice;
                        return rFIDClass;
                    }
                }
            }
            return rFIDClass;
        }
        static public List<RFIDClass> ReplaceRFIDDevice(this List<RFIDClass> rFIDClasses, RFIDDevice rFIDDevice)
        {
            for (int i = 0; i < rFIDClasses.Count; i++)
            {
                for (int k = 0; k < rFIDClasses[i].DeviceClasses.Length; k++)
                {
                    for (int m = 0; m < rFIDClasses[i].DeviceClasses[k].RFIDDevices.Count; m++)
                    {
                        if (rFIDClasses[i].DeviceClasses[k].RFIDDevices[m].GUID == rFIDDevice.GUID)
                        {
                            rFIDClasses[i].DeviceClasses[k].RFIDDevices[m] = rFIDDevice;
                            return rFIDClasses;
                        }
                    }
                }
            }
            return rFIDClasses;
        }
        static public List<Device> GetAllRFIDDevices(this List<RFIDClass> rFIDClasses)
        {
            List<Device> Devices = new List<Device>();
            for (int i = 0; i < rFIDClasses.Count; i++)
            {
                for (int k = 0; k < rFIDClasses[i].DeviceClasses.Length; k++)
                {
                    for (int m = 0; m < rFIDClasses[i].DeviceClasses[k].RFIDDevices.Count; m++)
                    {
                        Devices.Add(rFIDClasses[i].DeviceClasses[k].RFIDDevices[m]);
                    }
                }
            }
            return Devices;
        }
        static public List<RFIDDevice> GetAllRFIDDevices(this RFIDClass rFIDClass)
        {
            List<RFIDDevice> rFIDDevices = new List<RFIDDevice>();
            for (int i = 0; i < rFIDClass.DeviceClasses.Length; i++)
            {
                for (int k = 0; k < rFIDClass.DeviceClasses[i].RFIDDevices.Count; k++)
                {
                    rFIDDevices.Add(rFIDClass.DeviceClasses[i].RFIDDevices[k]);
                }
            }
            return rFIDDevices;
        }
        static public RFIDDevice GetRFIDDevices(this RFIDClass rFIDClass, string GUID)
        {
            return rFIDClass.SortByGUID(GUID);
        }
        static public List<RFIDClass> GetUpToSQL(this List<RFIDClass> rFIDClasses)
        {
            List<RFIDClass> rFIDClasses_buf = new List<RFIDClass>();
            for (int i = 0; i < rFIDClasses.Count; i++)
            {
                if(rFIDClasses[i].UpToSQL)
                {
                    rFIDClasses[i].UpToSQL = false;
                    rFIDClasses_buf.Add(rFIDClasses[i]);          
                }
            }
            return rFIDClasses_buf;
        }
        static public List<RFIDDevice> SortByCode(this List<RFIDClass> rFIDClasses, string Code)
        {
            List<RFIDDevice> rFIDDevices = new List<RFIDDevice>();
            for (int i = 0; i < rFIDClasses.Count; i++)
            {
                for (int k = 0; k < rFIDClasses[i].DeviceClasses.Length; k++)
                {
                    for (int m = 0; m < rFIDClasses[i].DeviceClasses[k].RFIDDevices.Count; m++)
                    {
                        if (rFIDClasses[i].DeviceClasses[k].RFIDDevices[m].Code.ToUpper() == Code.ToUpper())
                        {
                            rFIDDevices.Add(rFIDClasses[i].DeviceClasses[k].RFIDDevices[m]);
                        }
                    }
                }
            }
            return rFIDDevices;
        }
        static public List<RFIDDevice> SortLikeByCode(this List<RFIDClass> rFIDClasses, string Code)
        {
            List<RFIDDevice> rFIDDevices = new List<RFIDDevice>();
            for (int i = 0; i < rFIDClasses.Count; i++)
            {
                for (int k = 0; k < rFIDClasses[i].DeviceClasses.Length; k++)
                {
                    for (int m = 0; m < rFIDClasses[i].DeviceClasses[k].RFIDDevices.Count; m++)
                    {
                        if (rFIDClasses[i].DeviceClasses[k].RFIDDevices[m].Code.ToUpper().Contains(Code.ToUpper()))
                        {
                            rFIDDevices.Add(rFIDClasses[i].DeviceClasses[k].RFIDDevices[m]);
                        }
                    }
                }
            }
            return rFIDDevices;
        }

        static public List<RFIDDevice> SortByCode(this RFIDClass rFIDClass, string Code)
        {
            List<RFIDDevice> rFIDDevices = new List<RFIDDevice>();
            for (int i = 0; i < rFIDClass.DeviceClasses.Length; i++)
            {
                for (int k = 0; k < rFIDClass.DeviceClasses[i].RFIDDevices.Count; k++)
                {
                    if (rFIDClass.DeviceClasses[i].RFIDDevices[k].Code.ToUpper() == Code.ToUpper())
                    {
                        rFIDDevices.Add(rFIDClass.DeviceClasses[i].RFIDDevices[k]);
                    }
                }
            }
            return rFIDDevices;
        }
        static public List<RFIDDevice> SortLikeByCode(this RFIDClass rFIDClass, string Code)
        {
            List<RFIDDevice> rFIDDevices = new List<RFIDDevice>();
            for (int i = 0; i < rFIDClass.DeviceClasses.Length; i++)
            {
                for (int k = 0; k < rFIDClass.DeviceClasses[i].RFIDDevices.Count; k++)
                {
                    if (rFIDClass.DeviceClasses[i].RFIDDevices[k].Code.ToUpper().Contains(Code.ToUpper()))
                    {
                        rFIDDevices.Add(rFIDClass.DeviceClasses[i].RFIDDevices[k]);
                    }
                }
            }
            return rFIDDevices;
        }
        static public RFIDClass SortByIP(this List<RFIDClass> rFIDClasses, string IP)
        {
            for (int i = 0; i < rFIDClasses.Count; i++)
            {
                if (rFIDClasses[i].IP == IP) return rFIDClasses[i];
            }
            return null;
        }

        static public void ReplaceIP(this RFIDClass rFIDClass, string IP)
        {
            rFIDClass.ReplaceIP(IP, rFIDClass.Port);
        }
        static public void ReplaceIP(this RFIDClass rFIDClass, string IP, int Port)
        {
            rFIDClass.IP = IP;
            rFIDClass.Port = Port;

            List<RFIDDevice> rFIDDevices = rFIDClass.GetAllRFIDDevices();
            for (int i = 0; i < rFIDDevices.Count; i++)
            {
                rFIDDevices[i].IP = IP;
                rFIDDevices[i].Port = Port;
            }
        }

        static public List<Device> GetAllDevice(this List<RFIDClass> rFIDClasses)
        {
            List<Device> devices = rFIDClasses.GetAllRFIDDevices();
            return devices;
        }
    }
    [Serializable]
    public class RFIDClass
    {
        public class DeviceClass
        {
            public DeviceClass(string IP, int Port , int index)
            {
                this.gUID = Guid.NewGuid().ToString();
                this.IP = IP;
                this.Port = Port;
                this.Index = index;
                for (int i = 0; i < rFIDDevices.Count; i++)
                {
                    rFIDDevices[i].IP = IP;
                    rFIDDevices[i].Port = Port;
                    rFIDDevices[i].Index = index;

                }
            }
            private string iP = "0.0.0.0";
            private int port = 0;
            private List<RFIDDevice> rFIDDevices = new List<RFIDDevice>();
            private string name = "";
            private string gUID = "";
            private int index = -1;
            private bool enable = true;
            private DateTime unlock_start_dateTime;
            private DateTime unlock_end_dateTime;
            private bool unlockTimeEnable = false;
            private bool isLocker = false;

            public string IP
            {
                get => iP;
                set
                {
                    for (int i = 0; i < rFIDDevices.Count; i++)
                    {
                        if (rFIDDevices[i] != null) rFIDDevices[i].IP = value;
                    }
                    iP = value;
                }
            }
            public int Port { get => port; set => port = value; }
            public List<RFIDDevice> RFIDDevices { get => rFIDDevices; set => rFIDDevices = value; }
            public string Name { get => name; set => name = value; }
            public string GUID { get => gUID; set => gUID = value; }
            public bool Enable { get => enable; set => enable = value; }
            public int Index { get => index; set => index = value; }
            public bool IsLocker { get => isLocker; set => isLocker = value; }

            [JsonIgnore]
            public Font Name_font = new Font("微軟正黑體", 12, FontStyle.Bold);
            [Browsable(false)]
            public string Name_font_Serialize
            {
                get { return FontSerializationHelper.ToString(Name_font); }
                set { Name_font = FontSerializationHelper.FromString(value); }
            }

            public DateTime Unlock_start_dateTime { get => unlock_start_dateTime; set => unlock_start_dateTime = value; }
            public DateTime Unlock_end_dateTime { get => unlock_end_dateTime; set => unlock_end_dateTime = value; }
            public bool UnlockTimeEnable { get => unlockTimeEnable; set => unlockTimeEnable = value; }         


            public void Add()
            {
                rFIDDevices.Add(new RFIDDevice(this.IP, this.Port, this.Index));
                this.DeviceRefresh();
            }
            public void Delete(int rFIDIndex, int deviceIndex)
            {
                if (deviceIndex > RFIDDevices.Count) return;

                for (int i = 0; i < RFIDDevices.Count; i++)
                {
                    if (deviceIndex == RFIDDevices[i].Index)
                    {
                        RFIDDevices.RemoveAt(i);
                    }
                }
                this.DeviceRefresh();
            }
            public void Delete(string GUID)
            {
                for (int i = 0; i < RFIDDevices.Count; i++)
                {
                    if (GUID == RFIDDevices[i].GUID)
                    {
                        RFIDDevices.RemoveAt(i);
                    }
                }
                this.DeviceRefresh();
            }
            public RFIDDevice SortByGUID(string GUID)
            {
                for (int i = 0; i < RFIDDevices.Count; i++)
                {
                    if (GUID == RFIDDevices[i].GUID)
                    {
                        return RFIDDevices[i];
                    }
                }
                return null;
            }

            private void DeviceRefresh()
            {
                for (int i = 0; i < rFIDDevices.Count; i++)
                {
                    rFIDDevices[i].Index = i;
                    rFIDDevices[i].IP = this.IP;
                    rFIDDevices[i].Port = this.Port;
                    rFIDDevices[i].DeviceType = DeviceType.RFID_Device;
                }
            }
        }

        public RFIDClass(string IP, int Port)
        {
            this.gUID = Guid.NewGuid().ToString();
            this.IP = IP;
            this.Port = Port;
            for (int i = 0; i < DeviceClasses.Length; i++)
            {
                this.DeviceClasses[i] = new DeviceClass(this.IP, this.Port , i);
            }    
        }
        private string iP = "0.0.0.0";
        private int port = 0;
        private string name = "";
        private string gUID = "";
        private DeviceClass[] deviceClasses = new DeviceClass[5];

        public bool UpToSQL = false;
        public string IP
        { 
            get => iP;
            set
            {
                for (int i = 0; i < deviceClasses.Length; i++)
                {
                    if (deviceClasses[i] != null) deviceClasses[i].IP = value;
                }
                iP = value;
            }
        }
        public int Port { get => port; set => port = value; }
        public string Name { get => name; set => name = value; }
        public string GUID { get => gUID; set => gUID = value; }
        public DeviceClass[] DeviceClasses { get => deviceClasses; set => deviceClasses = value; }

        public void Add(int rFIDIndex)
        {
            if (rFIDIndex > DeviceClasses.Length) return;
            DeviceClasses[rFIDIndex].RFIDDevices.Add(new RFIDDevice(this.iP, this.port, rFIDIndex));
            this.DeviceRefresh();
        }
        public void Delete(int rFIDIndex , int deviceIndex)
        {
            if (rFIDIndex > DeviceClasses.Length) return;
            if (deviceIndex > DeviceClasses[rFIDIndex].RFIDDevices.Count) return;

            for (int i = 0; i < DeviceClasses[rFIDIndex].RFIDDevices.Count; i++)
            {
                if (deviceIndex == DeviceClasses[rFIDIndex].RFIDDevices[i].Index)
                {
                    DeviceClasses[rFIDIndex].RFIDDevices.RemoveAt(i);
                }
            }
            this.DeviceRefresh();
        }
        public void Delete(string GUID)
        {
            for (int i = 0; i < DeviceClasses.Length; i++)
            {
                for (int k = 0; k < DeviceClasses[i].RFIDDevices.Count; k++)
                {
                    if (DeviceClasses[i].RFIDDevices[k].GUID == GUID)
                    {
                        DeviceClasses[i].RFIDDevices.RemoveAt(k);
                    }
                }
            }
            this.DeviceRefresh();
        }
        public RFIDDevice SortByGUID(string GUID)
        {
            for (int i = 0; i < DeviceClasses.Length; i++)
            {
                for (int k = 0; k < DeviceClasses[i].RFIDDevices.Count; k++)
                {
                    if (DeviceClasses[i].RFIDDevices[k].GUID == GUID)
                    {
                        return DeviceClasses[i].RFIDDevices[k];
                    }
                }
            }
            return null;
        }

        private void DeviceRefresh()
        {
            for (int i = 0; i < DeviceClasses.Length; i++)
            {
                for (int k = 0; k < DeviceClasses[i].RFIDDevices.Count; k++)
                {
                    DeviceClasses[i].RFIDDevices[k].Index = i;
                    DeviceClasses[i].RFIDDevices[k].IP = this.IP;
                    DeviceClasses[i].RFIDDevices[k].Port = this.Port;
                    DeviceClasses[i].RFIDDevices[k].DeviceType = DeviceType.RFID_Device;
                }
            }
        }
    }
    [Serializable]
    public class RFIDDevice : Device
    {
   
        public RFIDDevice(string IP, int Port, int masterIndex)
        {
            this.GUID = Guid.NewGuid().ToString();
            this.IP = IP;
            this.Port = Port;
            this.DeviceType = DeviceType.RFID_Device;
            this.MasterIndex = masterIndex;
        }
    }

}
