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
        static public string SQL_GetAllRowsLED_JsonStr(SQLUI.SQLControl sQLControl)
        {
            List<object[]> deviceBasicTables = sQLControl.GetAllRows(null);
            return SQL_GetAllRowsLED_JsonStr(deviceBasicTables);
        }
        static public string SQL_GetAllRowsLED_JsonStr(List<object[]> deviceTables)
        {
            List<RowsLED> RowsLEDs = new List<RowsLED>();


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

            return json_result;
        }
        static public string SQL_GetAllRowsLED_ByIP_JsonStr(SQLUI.SQLControl sQLControl , string IP)
        {

            List<object[]> deviceBasicTables = sQLControl.GetRowsByDefult(null, (int)enum_DeviceTable.IP, IP);
            if (deviceBasicTables.Count == 0)
            {
                return null;
            }
            string json_result = deviceBasicTables[0][(int)enum_DeviceTable.Value].ObjectToString();

            return json_result;
        }

        static public void SQL_ReplaceByIP(SQLUI.SQLControl sQLControl, RowsLED rowsLED)
        {
            string IP = rowsLED.IP;
            List<object[]> list_value = sQLControl.GetRowsByDefult(null, enum_DeviceTable.IP.GetEnumName(), IP);
            if (list_value.Count == 0) return;
            list_value[0][(int)enum_DeviceTable.Value] = rowsLED.JsonSerializationt<RowsLED>();
            sQLControl.UpdateByDefulteExtra(null, list_value[0]);
        }

        static public RowsLED SQL_GetRowsLEDByIP(SQLUI.SQLControl sQLControl, string IP)
        {
            List<object[]> deviceBasicTables = sQLControl.GetRowsByDefult(null, (int)enum_DeviceTable.IP, IP);
            List<RowsLED> rowsLEDs = SQL_GetAllRowsLED(deviceBasicTables);
            if (rowsLEDs.Count == 0) return null;
            return rowsLEDs[0];
        }

        static public List<RowsLED> SQL_GetAllRowsLED(SQLUI.SQLControl sQLControl)
        {
            List<object[]> deviceBasicTables = sQLControl.GetAllRows(null);
            return SQL_GetAllRowsLED(deviceBasicTables);
        }
        static public List<RowsLED> SQL_GetAllRowsLED(List<object[]> deviceTables)
        {
            List<RowsLED> RowsLEDs = new List<RowsLED>();


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
            RowsLEDs = json_result.JsonDeserializet<List<RowsLED>>();

            return RowsLEDs;
        }

        static public List<RowsLED> SQL_GetRowsLEDByCode(SQLUI.SQLControl sQLControl, string Code)
        {
            List<object[]> deviceBasicTables = sQLControl.GetAllRows(null);
            return SQL_GetRowsLEDByCode(deviceBasicTables , Code);
        }
        static public List<RowsLED> SQL_GetRowsLEDByCode(List<object[]> deviceTables, string Code)
        {
            Code = Code.Replace("*", "");

            List<RowsLED> RowsLEDs = new List<RowsLED>();
            List<RowsLED> RowsLEDs_src = new List<RowsLED>();


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
            RowsLEDs_src = json_result.JsonDeserializet<List<RowsLED>>();
            for (int i = 0; i < RowsLEDs_src.Count; i++)
            {
                if (RowsLEDs_src[i].SortByCode(Code).Count == 0) continue;

                RowsLEDs.Add(RowsLEDs_src[i]);
            }


            return RowsLEDs;
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
            List<RowsLED> rowsLEDs = json_result.JsonDeserializet<List<RowsLED>>();
            for(int i = 0; i < rowsLEDs.Count; i++)
            {
                for (int k = 0; k < rowsLEDs[i].RowsDevices.Count; k++)
                {
                    deviceBasics.LockAdd(rowsLEDs[i].RowsDevices[k]);
                }
            }

            return deviceBasics;
        }

        static public List<DeviceBasic> GetDeviceBasicByCode(SQLUI.SQLControl sQLControl, string Code)
        {
            List<object[]> deviceBasicTables = sQLControl.GetAllRows(null);
            return GetDeviceBasicByCode(deviceBasicTables, Code);
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
            List<RowsLED> rowsLEDs = json_result.JsonDeserializet<List<RowsLED>>();
            for (int i = 0; i < rowsLEDs.Count; i++)
            {
                for (int k = 0; k < rowsLEDs[i].RowsDevices.Count; k++)
                {
                    if (rowsLEDs[i].RowsDevices[k].Code != Code) continue;
                    deviceBasics.LockAdd(rowsLEDs[i].RowsDevices[k]);
                }
            }

            return deviceBasics;
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
            rowsLEDs.Add(rowsLED);
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
            bool flag_serch = Code.Contains("*");
            Code = Code.Replace("*", "");
            List<RowsDevice> rowsDevices = new List<RowsDevice>();
            for (int i = 0; i < rowsLEDs.Count; i++)
            {
                for (int k = 0; k < rowsLEDs[i].RowsDevices.Count; k++)
                {
                    if(flag_serch)
                    {
                        if (rowsLEDs[i].RowsDevices[k].Code.ToUpper().Contains(Code.ToUpper())) rowsDevices.Add(rowsLEDs[i].RowsDevices[k]);
                    }
                    else
                    {
                        if (rowsLEDs[i].RowsDevices[k].Code.ToUpper() == Code.ToUpper()) rowsDevices.Add(rowsLEDs[i].RowsDevices[k]);
                    }
             
                }
            }
            return rowsDevices;
        }
        static public List<RowsDevice> SortLikeByCode(this List<RowsLED> rowsLEDs, string Code)
        {
            Code = Code.Replace("*", "");
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
            bool flag_serch = Code.Contains("*");
            Code = Code.Replace("*", "");
            List<RowsDevice> rowsDevices = new List<RowsDevice>();
            for (int k = 0; k < rowsLED.RowsDevices.Count; k++)
            {
                if(flag_serch)
                {
                    if (rowsLED.RowsDevices[k].Code.ToUpper().Contains(Code.ToUpper())) rowsDevices.Add(rowsLED.RowsDevices[k]);
                }
                else
                {
                    if (rowsLED.RowsDevices[k].Code.ToUpper() == Code.ToUpper()) rowsDevices.Add(rowsLED.RowsDevices[k]);
                }
          
            }
            return rowsDevices;
        }
        static public List<RowsDevice> SortLikeByCode(this RowsLED rowsLED, string Code)
        {
            Code = Code.Replace("*", "");
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
        static public RowsLED SortByName(this List<RowsLED> rowsLEDs, string SotrageName)
        {
            for (int i = 0; i < rowsLEDs.Count; i++)
            {
                if (rowsLEDs[i].Name == SotrageName) return rowsLEDs[i];
            }
            return null;
        }
        static public RowsLED SQL_GetDevice(SQLUI.SQLControl sQLControl, string IP)
        {
            List<object[]> deviceBasicTables = sQLControl.GetRowsByDefult(sQLControl.TableName, "IP", IP);
            if (deviceBasicTables.Count == 0) return null;
            string jsonString = deviceBasicTables[0][(int)enum_DeviceTable.Value].ObjectToString();
            RowsLED device = jsonString.JsonDeserializet<RowsLED>();
            return device;
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
    public class RowsLEDBasic
    {
        private List<DeviceBasic> rowsDevices = new List<DeviceBasic>();
        public List<DeviceBasic> RowsDevices { get => rowsDevices; set => rowsDevices = value; }
    }
    [Serializable]
    public class RowsLED
    {
        public static int NumOfLED = 250;
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
        private string speaker = "";

        public bool UpToSQL = false;
        public string IP { get => iP; set => iP = value; }
        public int Port { get => port; set => port = value; }
        public string Name { get => name; set => name = value; }
        public List<RowsDevice> RowsDevices { get => rowsDevices; set => rowsDevices = value; }
        public int Maximum { get => maximum; set => maximum = value; }
        public string Speaker { get => speaker; set => speaker = value; }

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


        public class ICP_SortByIP : IComparer<RowsLED>
        {
            public int Compare(RowsLED x, RowsLED y)
            {
                string IP_0 = x.IP;
                string IP_1 = y.IP;
                string[] IP_0_Array = IP_0.Split('.');
                string[] IP_1_Array = IP_1.Split('.');
                IP_0 = "";
                IP_1 = "";
                for (int i = 0; i < 4; i++)
                {
                    if (IP_0_Array[i].Length < 3) IP_0_Array[i] = "0" + IP_0_Array[i];
                    if (IP_0_Array[i].Length < 3) IP_0_Array[i] = "0" + IP_0_Array[i];
                    if (IP_0_Array[i].Length < 3) IP_0_Array[i] = "0" + IP_0_Array[i];

                    if (IP_1_Array[i].Length < 3) IP_1_Array[i] = "0" + IP_1_Array[i];
                    if (IP_1_Array[i].Length < 3) IP_1_Array[i] = "0" + IP_1_Array[i];
                    if (IP_1_Array[i].Length < 3) IP_1_Array[i] = "0" + IP_1_Array[i];

                    IP_0 += IP_0_Array[i];
                    IP_1 += IP_1_Array[i];
                }
                int cmp = IP_0_Array[2].CompareTo(IP_1_Array[2]);
                if (cmp > 0)
                {
                    return 1;
                }
                else if (cmp < 0)
                {
                    return -1;
                }
                else if (cmp == 0)
                {
                    cmp = IP_0_Array[3].CompareTo(IP_1_Array[3]);
                    if (cmp > 0)
                    {
                        return 1;
                    }
                    else if (cmp < 0)
                    {
                        return -1;
                    }
                    else if (cmp == 0)
                    {
                        return 0;
                    }
                }

                return 0;

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
