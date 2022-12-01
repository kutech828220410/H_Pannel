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
    public static class RowsLEDMethod
    {
        static public List<RowsLED> SQL_GetAllRowsLED(List<object[]> deviceTables)
        {
            List<RowsLED> RowsLEDs = new List<RowsLED>();

            Parallel.ForEach(deviceTables, value =>
            {
                string jsonString = value[(int)enum_DeviceTable.Value].ObjectToString();
                RowsLED RowsLED = jsonString.JsonDeserializet<RowsLED>();
                if (RowsLED != null) RowsLEDs.LockAdd(RowsLED);

            });
            RowsLEDs = (from value in RowsLEDs
                        where value != null
                        select value).ToList();
            return RowsLEDs;
        }


        static public List<RowsLED> Add_NewRowsLED(this List<RowsLED> rowsLEDs, RowsDevice rowsDevice)
        {
            if (rowsDevice == null) return rowsLEDs;
            for (int i = 0; i < rowsLEDs.Count; i++)
            {
                if (rowsLEDs[i].IP == rowsDevice.IP)
                {
                    rowsLEDs[i] = rowsLEDs[i].ReplaceRowsDevice(rowsDevice);
                    return rowsLEDs;
                }
            }
            return rowsLEDs;

        }
        static public RowsLED Add_NewRowsLED(this RowsLED rowsLED, RowsDevice rowsDevice)
        {
            if (rowsDevice == null) return rowsLED;
            rowsDevice.IP = rowsLED.IP;
            rowsDevice.Port = rowsLED.Port;
            rowsLED.RowsDevices.Add(rowsDevice);
            return rowsLED;
        }
        static public List<RowsLED> Add_NewRowsLED(this List<RowsLED> rowsLEDs, RowsLED rowsLED)
        {
            for (int i = 0; i < rowsLEDs.Count; i++)
            {
                if (rowsLEDs[i].IP == rowsLED.IP)
                {
                    rowsLEDs[i] = rowsLED;
                    return rowsLEDs;
                }
            }
            return rowsLEDs;

        }
        static public RowsLED ReplaceRowsDevice(this RowsLED rowsLED, RowsDevice rowsDevice)
        {
            for (int i = 0; i < rowsLED.RowsDevices.Count; i++)
            {
                if (rowsLED.RowsDevices[i].GUID == rowsDevice.GUID) rowsLED.RowsDevices[i] = rowsDevice;
            }
            return rowsLED;
        }
        static public List<RowsDevice> GetAllRowsDevices(this List<RowsLED> rowsLEDs)
        {
            List<RowsDevice> rowsDevices = new List<RowsDevice>();
            for (int i = 0; i < rowsLEDs.Count; i++)
            {
                for (int k = 0; k < rowsLEDs[i].RowsDevices.Count; k++)
                {
                    rowsDevices.Add(rowsLEDs[i].RowsDevices[k]);
                }
            }
            return rowsDevices;
        }
        static public List<RowsDevice> GetAllRowsDevices(this RowsLED rowsLEDs)
        {
            List<RowsDevice> rowsDevices = new List<RowsDevice>();
            for (int i = 0; i < rowsLEDs.RowsDevices.Count; i++)
            {
                rowsDevices.Add(rowsLEDs.RowsDevices[i]);
            }
            return rowsDevices;
        }
        static public RowsDevice GetRowsDevice(this List<RowsLED> rowsLEDs, string GUID)
        {
            RowsDevice rowsDevice = null;
            for (int i = 0; i < rowsLEDs.Count; i++)
            {
                rowsDevice = GetRowsDevice(rowsLEDs[i], GUID);
                if (rowsDevice != null) return rowsDevice;
            }
            return null;
        }
        static public RowsDevice GetRowsDevice(this RowsLED rowsLED ,string GUID)
        {
            return rowsLED.SortByGUID(GUID);
        }
        static public List<RowsLED> GetUpToSQL(this List<RowsLED> rowsLEDs)
        {
            List<RowsLED> rowsLEDs_buf = new List<RowsLED>();
            for (int i = 0; i < rowsLEDs.Count; i++)
            {
                if(rowsLEDs[i].UpToSQL)
                {
                    rowsLEDs[i].UpToSQL = false;
                    rowsLEDs_buf.Add(rowsLEDs[i]);
                }
            }
            return rowsLEDs_buf;
        }
        static public List<RowsDevice> SortByCode(this List<RowsLED> rowsLEDs, string Code)
        {
            List<RowsDevice> rowsDevices = new List<RowsDevice>();
            for (int i = 0; i < rowsLEDs.Count; i++)
            {
                for (int k = 0; k < rowsLEDs[i].RowsDevices.Count; k++)
                {
                    if (rowsLEDs[i].RowsDevices[k].Code.ToUpper() == Code.ToUpper()) rowsDevices.Add(rowsLEDs[i].RowsDevices[k]);
                }
            }
            return rowsDevices;
        }
        static public List<RowsDevice> SortLikeByCode(this List<RowsLED> rowsLEDs, string Code)
        {
            List<RowsDevice> rowsDevices = new List<RowsDevice>();
            for (int i = 0; i < rowsLEDs.Count; i++)
            {
                for (int k = 0; k < rowsLEDs[i].RowsDevices.Count; k++)
                {
                    if (rowsLEDs[i].RowsDevices[k].Code.ToUpper().Contains(Code.ToUpper())) rowsDevices.Add(rowsLEDs[i].RowsDevices[k]);
                }
            }
            return rowsDevices;
        }
        static public List<RowsDevice> SortByCode(this RowsLED rowsLED, string Code)
        {
            List<RowsDevice> rowsDevices = new List<RowsDevice>();
            for (int k = 0; k < rowsLED.RowsDevices.Count; k++)
            {
                if (rowsLED.RowsDevices[k].Code.ToUpper() == Code.ToUpper()) rowsDevices.Add(rowsLED.RowsDevices[k]);
            }
            return rowsDevices;
        }
        static public List<RowsDevice> SortLikeByCode(this RowsLED rowsLED, string Code)
        {
            List<RowsDevice> rowsDevices = new List<RowsDevice>();
            for (int k = 0; k < rowsLED.RowsDevices.Count; k++)
            {
                if (rowsLED.RowsDevices[k].Code.ToUpper().Contains(Code.ToUpper())) rowsDevices.Add(rowsLED.RowsDevices[k]);
            }
            return rowsDevices;
        }
        static public RowsLED SortByIP(this List<RowsLED> rowsLEDs, string IP)
        {
            for (int i = 0; i < rowsLEDs.Count; i++)
            {
                if (rowsLEDs[i].IP == IP) return rowsLEDs[i];
            }
            return null;
        }

    
        static public void ReplaceIP(this RowsLED rowsLED, string IP)
        {
            rowsLED.ReplaceIP(IP, rowsLED.Port);
        }
        static public void ReplaceIP(this RowsLED rowsLED, string IP, int Port)
        {
            List<RowsDevice> rowsDevices = rowsLED.GetAllRowsDevices();
            rowsLED.IP = IP;
            rowsLED.Port = Port;
            for (int i = 0; i < rowsDevices.Count; i++)
            {
                rowsDevices[i].IP = IP;
                rowsDevices[i].Port = Port;
            }
        }

        static public List<Device> GetAllDevice(this List<RowsLED> rowsLEDs)
        {
            List<RowsDevice> rowsDevices = rowsLEDs.GetAllRowsDevices();
            List<Device> devices = new List<Device>();
            for (int i = 0; i < rowsDevices.Count; i++)
            {
                devices.Add(rowsDevices[i]);
            }

            return devices;
        }
    }
    [Serializable]
    public class RowsLED
    {
        public static int NumOfLED = 100;
        [JsonIgnore]
        public byte[] LED_Bytes = new byte[NumOfLED * 3];
        public RowsLED(string IP, int Port)
        {
            this.IP = IP;
            this.Port = Port;
        }
        private string iP = "0.0.0.0";
        private int port = 0;
        private string name = "";
        private int maximum = 0;
        private List<RowsDevice> rowsDevices = new List<RowsDevice>();

        public bool UpToSQL = false;
        public string IP { get => iP; set => iP = value; }
        public int Port { get => port; set => port = value; }
        public string Name { get => name; set => name = value; }
        public List<RowsDevice> RowsDevices { get => rowsDevices; set => rowsDevices = value; }
        public int Maximum { get => maximum; set => maximum = value; }

        public void Add(int startLED, int endLED)
        {
            RowsDevices.Add(new RowsDevice(this.IP, this.Port, startLED, endLED));
            this.DeviceRefresh();
        }
        public void Delete(int index)
        {
            for (int i = 0; i < RowsDevices.Count; i++)
            {
                if(index == RowsDevices[i].Index)
                {
                    RowsDevices.RemoveAt(i);
                }
            }
            this.DeviceRefresh();
        }
        public void Delete(string GUID)
        {
            for (int i = 0; i < RowsDevices.Count; i++)
            {
                if (GUID == RowsDevices[i].GUID)
                {
                    RowsDevices.RemoveAt(i);
                }
            }
            this.DeviceRefresh();
        }
        public RowsDevice SortByGUID(string GUID)
        {
            for (int i = 0; i < RowsDevices.Count; i++)
            {
                if (RowsDevices[i].GUID == GUID) return RowsDevices[i];
            }
            return null;
        }
        private void DeviceRefresh()
        {
            for (int i = 0; i < RowsDevices.Count; i++)
            {
                RowsDevices[i].Index = i;
                RowsDevices[i].IP = this.IP;
                RowsDevices[i].Port = this.Port;
                RowsDevices[i].DeviceType = DeviceType.RowsLED;
            }
        }

    }

    [Serializable]
    public class RowsDevice : Device
    {
        private int startLED = 0;
        private int endLED = 0;

        public int StartLED { get => startLED; set => startLED = value; }
        public int EndLED { get => endLED; set => endLED = value; }

        public RowsDevice(string IP, int Port, int startLED, int endLED)
        {
            this.GUID = Guid.NewGuid().ToString();
            this.IP = IP;
            this.Port = Port;
            this.startLED = startLED;
            this.endLED = endLED;
            this.DeviceType = DeviceType.RowsLED;
        }
    }
}
