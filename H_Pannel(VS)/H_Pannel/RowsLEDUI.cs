﻿using System;
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
using H_Pannel_lib;

namespace H_Pannel_lib
{
    public partial class RowsLEDUI : DeviceBasicUI
    {
        #region 靜態參數
        static public double Lightness = 1.0D;

        static public int NumOfLED = 250;
        static public byte[] Get_Empty_LEDBytes()
        {
            return new byte[NumOfLED * 3];
        }
        static public byte[] Get_Rows_LEDBytes(RowsLED rowsLED, int startNum, int EndNum, Color color , bool ClearAll)
        {
            if(ClearAll)
            {
                rowsLED.LED_Bytes = Get_Empty_LEDBytes();
            }
            return Get_Rows_LEDBytes(ref rowsLED.LED_Bytes, startNum, EndNum, color);
        }
        static public byte[] Get_Rows_LEDBytes(ref byte[] LED_Bytes, int startNum, int EndNum, Color color)
        {
            for (int i = startNum; i < EndNum; i++)
            {
                if (i > LED_Bytes.Length) break;
                LED_Bytes[i * 3 + 0] = (byte)(color.R * Lightness);
                LED_Bytes[i * 3 + 1] = (byte)(color.G * Lightness);
                LED_Bytes[i * 3 + 2] = (byte)(color.B * Lightness);
            }
            return LED_Bytes;
        }
        static public byte[] Get_Rows_LEDBytes(ref byte[] LED_Bytes, RowsDevice rowsDevice, Color color)
        {
            LED_Bytes = Get_Rows_LEDBytes(ref LED_Bytes, rowsDevice.StartLED, rowsDevice.EndLED, color);
            return LED_Bytes;
        }
        static public RowsLED ResetLightStateLEDBytes(RowsLED rowsLED)
        {
            rowsLED.LED_Bytes = Get_Empty_LEDBytes();
            for (int i = 0; i < rowsLED.RowsDevices.Count; i++)
            {
                rowsLED.RowsDevices[i].LightState.State = false;
                rowsLED.RowsDevices[i].LightState.LightColor = Color.Black;
                rowsLED.RowsDevices[i].LightState.Interval = 0;
                rowsLED.RowsDevices[i].LightState.LightOffTime = 0;

            }
            return rowsLED;
        }
        static public byte[] Get_RowsLightStateLEDBytes(RowsLED rowsLED)
        {
            byte[] LED_Bytes = Get_Empty_LEDBytes();
            for (int i = 0; i < rowsLED.RowsDevices.Count; i++)
            {
                if (rowsLED.RowsDevices[i].LightState.State)
                {
                    if(rowsLED.RowsDevices[i].LightState.Interval > 0)
                    {
                        // 計算時間間隔
                        TimeSpan interval = DateTime.Now - rowsLED.RowsDevices[i].LightState.LightingDateTime;
                        // 取得毫秒間隔
                        double milliseconds = interval.TotalMilliseconds;
                        if ((int)(milliseconds / rowsLED.RowsDevices[i].LightState.Interval) % 2 == 0)
                        {
                            continue;
                        }
                    }
                    if (rowsLED.RowsDevices[i].LightState.LightOffTime > 0)
                    {
                        TimeSpan interval = DateTime.Now - rowsLED.RowsDevices[i].LightState.LightingDateTime;
                        double milliseconds = interval.TotalMilliseconds;
                        if (milliseconds > rowsLED.RowsDevices[i].LightState.LightOffTime)
                        {
                            rowsLED.RowsDevices[i].LightState.State = false;
                            continue;
                        }

                    }
                    LED_Bytes = Get_Rows_LEDBytes(ref LED_Bytes, rowsLED.RowsDevices[i].StartLED, rowsLED.RowsDevices[i].EndLED, rowsLED.RowsDevices[i].LightState.LightColor);
                }       
            }

            return LED_Bytes;
        }
        static public byte[] Get_Rows_LEDBytes(ref byte[] LED_Bytes, List<RowsDevice> rowsDevices, Color color)
        {
            for (int i = 0; i < rowsDevices.Count; i++)
            {
                LED_Bytes = Get_Rows_LEDBytes(ref LED_Bytes, rowsDevices[i].StartLED, rowsDevices[i].EndLED, color);
            }
           
            return LED_Bytes;
        }

        static public bool Set_Rows_LED_UDP(UDP_Class uDP_Class, RowsLED rowsLED, int startNum, int EndNum, Color color, bool ClearAll)
        {
            return Set_Rows_LED_UDP(uDP_Class, rowsLED, startNum, EndNum, color, ClearAll, false);
        }
        static public bool Set_Rows_LED_UDP(UDP_Class uDP_Class, RowsLED rowsLED, int startNum, int EndNum, Color color, bool ClearAll, bool check_buf_diff)
        {
            if (ClearAll)
            {
                rowsLED.LED_Bytes = Get_Empty_LEDBytes();
            }
            rowsLED.LED_Bytes = Get_Rows_LEDBytes(ref rowsLED.LED_Bytes, startNum, EndNum, color);
            return Set_Rows_LED_UDP(uDP_Class, rowsLED, check_buf_diff);
        }
        static public bool Set_Rows_LED_UDP(UDP_Class uDP_Class, RowsLED rowsLED)
        {
            return Set_Rows_LED_UDP(uDP_Class, rowsLED, false);
        }
        static public bool Set_Rows_LED_UDP(UDP_Class uDP_Class, RowsLED rowsLED , bool check_buf_diff)
        {
            bool flag_buf_diff = false;
            if(rowsLED.LED_Bytes_buf.Length != rowsLED.LED_Bytes.Length)
            {
                rowsLED.LED_Bytes_buf = new byte[rowsLED.LED_Bytes.Length];
            }


            for (int i = 0; i < rowsLED.LED_Bytes.Length; i++)
            {
                if (rowsLED.LED_Bytes_buf[i] != rowsLED.LED_Bytes[i])
                {
                    rowsLED.LED_Bytes_buf[i] = rowsLED.LED_Bytes[i];
                    flag_buf_diff = true;
                }
                  
            }
            if (check_buf_diff)
            {
                if(flag_buf_diff)
                {
                    Set_Rows_LED_UDP(uDP_Class, rowsLED.IP, rowsLED.LED_Bytes);
                }
                return true;
            }
            return Set_Rows_LED_UDP(uDP_Class, rowsLED.IP, rowsLED.LED_Bytes);
        }
        static public bool Set_Rows_LED_UDP(UDP_Class uDP_Class, string IP, int startNum, int EndNum, Color color)
        {
            byte[] LED_Bytes = Get_Empty_LEDBytes();
            LED_Bytes = Get_Rows_LEDBytes(ref LED_Bytes, startNum, EndNum, color);
            return Set_Rows_LED_UDP(uDP_Class, IP, LED_Bytes);
        }
        static public bool Set_Rows_LED_UDP(UDP_Class uDP_Class, string IP, byte[] LED_Bytes)
        {
            if (uDP_Class != null)
            {
                return Communication.Set_WS2812_Buffer(uDP_Class, IP, 0, LED_Bytes);
            }
            return false;
        }

        static public bool Set_Rows_LED_UDP_Ex(UDP_Class uDP_Class, string IP, int startNum, int EndNum, Color color)
        {
            if (uDP_Class != null)
            {
                int len = EndNum - startNum;
                if (len <= 0) return false;
                byte[] LED_Bytes = new byte[len * 3];
                for (int i = 0; i < len; i++)
                {
                    LED_Bytes[i * 3 + 0] = color.R;
                    LED_Bytes[i * 3 + 1] = color.G;
                    LED_Bytes[i * 3 + 2] = color.B;
                }
                return Communication.Set_WS2812_Buffer(uDP_Class, IP, startNum * 3, LED_Bytes);
            }
            return false;
        }
        static public bool Set_WS2812B_breathing(UDP_Class uDP_Class, string IP, byte WS2812B_breathing_onAddVal, byte WS2812B_breathing_offSubVal, Color color)
        {
            if (uDP_Class != null)
            {
                return Communication.Set_WS2812B_breathing(uDP_Class, IP, WS2812B_breathing_onAddVal, WS2812B_breathing_offSubVal, color);
            }
            return false;
        }
        static public byte[] Get_RowsLED_LEDBytes_UDP(UDP_Class uDP_Class, string IP)
        {
            byte[] LED_Bytes = new byte[NumOfLED * 3];
            if (uDP_Class != null)
            {
                return Communication.Get_WS2812_Buffer(uDP_Class, IP, NumOfLED * 3);
            }
            return LED_Bytes;
        }

        static public bool Check_LEDBytesBuf_Diff(RowsLED rowsLED)
        {
            return Check_LEDBytesBuf_Diff(rowsLED.LED_Bytes, rowsLED.LED_Bytes_buf);
        }
        static public bool Check_LEDBytesBuf_Diff(RowsLED rowsLED, byte[] bytes2)
        {
            return Check_LEDBytesBuf_Diff(rowsLED.LED_Bytes, bytes2);
        }
        static public bool Check_LEDBytesBuf_Diff(byte[] bytes1, byte[] bytes2)
        {
            bool flag_diff = true;
            try
            {
                if (bytes1.Length < bytes2.Length)
                {
                    flag_diff = true;
                    return flag_diff;
                }
                for (int i = 0; i < bytes1.Length; i++)
                {
                    if (i >= bytes2.Length) break;
                    if (bytes1[i] != bytes2[i])
                    {
                        flag_diff = true;
                        return flag_diff;
                    }
                }
                flag_diff = false;
                return flag_diff;
            }
            catch
            {
                flag_diff = false;
                return flag_diff;
            }
            finally
            {

            }
         
        }
        #endregion

        private enum ContextMenuStrip_Main
        {
            設定亮燈,
            設定呼吸燈,
            設定燈數,
        }

        public RowsLEDUI()
        {
            this.TableName = "RowsLED_Jsonstring";
            this.DeviceTableMouseDownRightEvent += RowsLEDUI_DeviceTableMouseDownRightEvent;
            this.UDP_DataReceiveMouseDownRightEvent += RowsLEDUI_UDP_DataReceiveMouseDownRightEvent;

            Enum_ContextMenuStrip_DeviceTable = new ContextMenuStrip_Main();
            Enum_ContextMenuStrip_UDP_DataReceive = new ContextMenuStrip_Main();
        }

        public bool Set_Rows_LED_Clear_UDP(RowsLED rowsLED, RowsDevice rowsDevice, bool check_buf_diff)
        {
            return Set_Rows_LED_UDP(rowsLED, rowsDevice, Color.Black, check_buf_diff);
        }
        public bool Set_Rows_LED_UDP(RowsLED rowsLED, RowsDevice rowsDevice, Color color)
        {
            return Set_Rows_LED_UDP(rowsLED, rowsDevice.StartLED, rowsDevice.EndLED, color, true, false);
        }
        public bool Set_Rows_LED_UDP(RowsLED rowsLED, RowsDevice rowsDevice, Color color, bool check_buf_diff)
        {
            return Set_Rows_LED_UDP(rowsLED, rowsDevice.StartLED, rowsDevice.EndLED, color, true, check_buf_diff);
        }
        public bool Set_Rows_LED_UDP(RowsLED rowsLED, RowsDevice rowsDevice, Color color, bool ClearAll, bool check_buf_diff)
        {
            return Set_Rows_LED_UDP(rowsLED, rowsDevice.StartLED, rowsDevice.EndLED, color, ClearAll, check_buf_diff);
        }


        public bool Set_Rows_LED_UDP(RowsLED rowsLED, int startNum, int EndNum, Color color)
        {
            return Set_Rows_LED_UDP(rowsLED, startNum, EndNum, color, true, false);
        }
        public bool Set_Rows_LED_UDP(RowsLED rowsLED, int startNum, int EndNum, Color color, bool check_buf_diff)
        {
            return Set_Rows_LED_UDP(rowsLED, startNum, EndNum, color, true, check_buf_diff);
        }        
        public bool Set_Rows_LED_UDP(RowsLED rowsLED, int startNum, int EndNum, Color color, bool ClearAll, bool check_buf_diff)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(rowsLED.Port);
            return Set_Rows_LED_UDP(uDP_Class, rowsLED, startNum, EndNum, color, ClearAll, check_buf_diff);
        }

        public bool Set_Rows_LED_UDP(RowsLED rowsLED)
        {
            return Set_Rows_LED_UDP(rowsLED , false);
        }
        public bool Set_Rows_LED_UDP(RowsLED rowsLED, bool check_buf_diff)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(rowsLED.Port);
            return Set_Rows_LED_UDP(uDP_Class, rowsLED, check_buf_diff);
        }

        public bool Set_Rows_LED_UDP(string IP, int Port, byte[] LED_Bytes)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
            return Set_Rows_LED_UDP(uDP_Class, IP, LED_Bytes);
        }
        public bool Set_Rows_LED_Clear_UDP(RowsLED rowsLED)
        {
            return Set_Rows_LED_Clear_UDP(rowsLED, false);
        }
        public bool Set_Rows_LED_Clear_UDP(RowsLED rowsLED, bool check_buf_diff)
        {
            rowsLED.LED_Bytes = Get_Empty_LEDBytes();
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(rowsLED.Port);
            return Set_Rows_LED_UDP(uDP_Class, rowsLED, check_buf_diff);
        }
    
        public bool Set_Rows_LED_UDP_Ex(RowsLED rowsLED, int startNum, int EndNum, Color color)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(rowsLED.Port);
            return Set_Rows_LED_UDP_Ex(uDP_Class, rowsLED.IP, startNum, EndNum, color);
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
                            value_load[(int)enum_DeviceTable.Value] = new RowsLED(IP, Port).JsonSerializationt<RowsLED>();
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
                        value_load[(int)enum_DeviceTable.Value] = new RowsLED(IP, Port).JsonSerializationt<RowsLED>();
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
        protected override void SQL_AddDevice(string IP, int Port)
        {
            List<object[]> list_SQL_Value = this.sqL_DataGridView_DeviceTable.SQL_GetAllRows(false);
            List<object[]> list_SQL_Value_buf = new List<object[]>();
            list_SQL_Value_buf = list_SQL_Value.GetRows((int)enum_DeviceTable.IP, IP);
            if (list_SQL_Value_buf.Count == 0)
            {
                object[] value = new object[new enum_DeviceTable().GetEnumNames().Length];
                value[(int)enum_DeviceTable.GUID] = Guid.NewGuid().ToString();
                value[(int)enum_DeviceTable.IP] = IP;
                value[(int)enum_DeviceTable.Port] = Port;
                value[(int)enum_DeviceTable.Value] = new RowsLED(IP, Port).JsonSerializationt<RowsLED>();
                this.sqL_DataGridView_DeviceTable.SQL_AddRow(value, true);
            }
            else
            {
                string GUID = list_SQL_Value_buf[0][(int)enum_DeviceTable.GUID].ObjectToString();
                list_SQL_Value_buf[0][(int)enum_DeviceTable.Port] = Port;
                string jsonString = list_SQL_Value_buf[0][(int)enum_DeviceTable.Value].ObjectToString();
                if (jsonString.StringIsEmpty()) return;
                RowsLED rowsLED = jsonString.JsonDeserializet<RowsLED>();
                if (rowsLED == null) return;
                rowsLED.Port = Port;
                list_SQL_Value_buf[0][(int)enum_DeviceTable.Value] = rowsLED.JsonSerializationt();
                this.sqL_DataGridView_DeviceTable.SQL_Replace(enum_DeviceTable.GUID.GetEnumName(), GUID, list_SQL_Value_buf[0], true);
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
                RowsLED RowsLED = jsonString.JsonDeserializet<RowsLED>();
                if (RowsLED == null) return;
                RowsLED.IP = IP;
                value[(int)enum_DeviceTable.Value] = RowsLED.JsonSerializationt();
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
                RowsLED RowsLED = jsonString.JsonDeserializet<RowsLED>();
                if (RowsLED == null) return;
                RowsLED.IP = IP;
                RowsLED.Port = Port;
                value[(int)enum_DeviceTable.Value] = RowsLED.JsonSerializationt();
                this.sqL_DataGridView_DeviceTable.SQL_AddRow(value, true);
            }
        }

        public string SQL_GetRowsLEDJsonString(string IP)
        {
            List<object[]> list_value = this.SQL_GetAllDeviceTableRows();
            List<object[]> list_value_buf = new List<object[]>();
            list_value_buf = list_value.GetRows((int)enum_DeviceTable.IP, IP);
            if (list_value_buf.Count == 0) return null;
            return list_value_buf[0][(int)enum_DeviceTable.Value].ObjectToString();
        }
        public List<RowsLED> SQL_GetAllRowsLED()
        {
            List<RowsLED> RowsLEDs = new List<RowsLED>();
            List<object[]> list_value = this.SQL_GetAllDeviceTableRows();

            Parallel.ForEach(list_value, value =>
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
        public List<Device> SQL_GetAllDevice()
        {
            List<Device> devices = new List<Device>();
            List<RowsLED> RowsLEDs = this.SQL_GetAllRowsLED();
            devices = devices.GetAllDevice();
            return devices;
        }
        public List<DeviceBasic> SQL_GetAllDeviceBasic()
        {
            List<DeviceBasic> deviceBasics = new List<DeviceBasic>();
            List<RowsLEDBasic> rowsLEDs = new List<RowsLEDBasic>();
            List<object[]> list_value = this.SQL_GetAllDeviceTableRows();
            Parallel.ForEach(list_value, value =>
            {
                string jsonString = value[(int)enum_DeviceTable.Value].ObjectToString();
                RowsLEDBasic rowsLEDBasic = jsonString.JsonDeserializet<RowsLEDBasic>();
                for (int i = 0; i < rowsLEDBasic.RowsDevices.Count; i++)
                {
                    deviceBasics.LockAdd(rowsLEDBasic.RowsDevices[i]);
                }
            });

            deviceBasics = (from value in deviceBasics
                            where value != null
                            select value).ToList();
            return deviceBasics;

        }
        public RowsLED SQL_GetRowsLED(RowsLED RowsLED)
        {
            return SQL_GetRowsLED(RowsLED.IP);
        }
        public RowsLED SQL_GetRowsLED(string IP)
        {
            string jsonString = this.SQL_GetRowsLEDJsonString(IP);
            if (jsonString.StringIsEmpty()) return null;
            RowsLED RowsLED = jsonString.JsonDeserializet<RowsLED>();
            return RowsLED;
        }
        public RowsDevice SQL_GetRowsDevice(RowsDevice rowsDevice)
        {
            RowsLED rowsLED = this.SQL_GetRowsLED(rowsDevice.IP);
            if (rowsLED == null) return null;
            rowsDevice = rowsLED.SortByGUID(rowsDevice.GUID);
            return rowsDevice;
        }
        public bool SQL_ReplaceRowsLED(RowsLED RowsLED)
        {
            string IP = RowsLED.IP;
            List<object[]> list_value = this.sqL_DataGridView_DeviceTable.SQL_GetRows(enum_DeviceTable.IP.GetEnumName(), IP, false);
            if (list_value.Count == 0) return false;
            list_value[0][(int)enum_DeviceTable.Value] = RowsLED.JsonSerializationt<RowsLED>();
            this.sqL_DataGridView_DeviceTable.SQL_Replace(enum_DeviceTable.GUID.GetEnumName(), list_value[0][(int)enum_DeviceTable.GUID].ObjectToString(), list_value[0], false);
            return true;
        }
        public bool SQL_ReplaceRowsLED(List<RowsLED> RowsLED)
        {
            List<object[]> list_value = this.sqL_DataGridView_DeviceTable.SQL_GetAllRows(false);
            List<object[]> list_value_buf = new List<object[]>();
            List<string> list_Replace_SerchValue = new List<string>();
            List<object[]> list_Replace_Value = new List<object[]>();
            if (list_value.Count == 0) return false;
            for (int i = 0; i < RowsLED.Count; i++)
            {
                list_value_buf = list_value.GetRows((int)enum_DeviceTable.IP, RowsLED[i].IP);
                if (list_value_buf.Count > 0)
                {
                    list_value_buf[0][(int)enum_DeviceTable.Value] = RowsLED[i].JsonSerializationt<RowsLED>();
                    list_Replace_SerchValue.Add(list_value_buf[0][(int)enum_DeviceTable.GUID].ObjectToString());
                    list_Replace_Value.Add(list_value_buf[0]);
                }
            }
            this.sqL_DataGridView_DeviceTable.SQL_ReplaceExtra(enum_DeviceTable.GUID.GetEnumName(), list_Replace_SerchValue, list_Replace_Value, false);
            return true;
        }

        public byte[] Get_RowsLED_LED_UDP(RowsLED rowsLED)
        {
            rowsLED.LED_Bytes = Get_RowsLED_LED_UDP(rowsLED.IP, rowsLED.Port);
            return rowsLED.LED_Bytes;
        }
        public byte[] Get_RowsLED_LED_UDP(string IP, int Port)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
            return Get_RowsLED_LEDBytes_UDP(uDP_Class, IP);
        }


        #region Event
        private void RowsLEDUI_UDP_DataReceiveMouseDownRightEvent(string selectedText, List<IPEndPoint> iPEndPoints)
        {
            if (selectedText == ContextMenuStrip_Main.設定亮燈.GetEnumName())
            {
                List<string> list_IP = new List<string>();
                List<UDP_Class> list_UDP_Class = new List<UDP_Class>();
                for(int i = 0; i < iPEndPoints.Count; i++)
                {
                    string IP = iPEndPoints[i].Address.ToString();
                    UDP_Class uDP_Class = List_UDP_Local.SortByPort(iPEndPoints[i].Port);
                    if(uDP_Class != null)
                    {
                        list_IP.Add(IP);
                        list_UDP_Class.Add(uDP_Class);
                    }
                }

                ColorDialog colorDialog = new ColorDialog();
                colorDialog.Color = Color.Red;
                if (colorDialog.ShowDialog() != DialogResult.OK) return;

                Dialog_RowsLED dialog_RowsLED = new Dialog_RowsLED(list_UDP_Class, list_IP, colorDialog.Color);
                dialog_RowsLED.ShowDialog();
            }
            if (selectedText == ContextMenuStrip_Main.設定呼吸燈.GetEnumName())
            {
                List<string> list_IP = new List<string>();
                List<UDP_Class> list_UDP_Class = new List<UDP_Class>();
                for (int i = 0; i < iPEndPoints.Count; i++)
                {
                    string IP = iPEndPoints[i].Address.ToString();
                    UDP_Class uDP_Class = List_UDP_Local.SortByPort(iPEndPoints[i].Port);
                    if (uDP_Class != null)
                    {
                        list_IP.Add(IP);
                        list_UDP_Class.Add(uDP_Class);
                    }
                }
                if (list_IP.Count == 0) return;
                ColorDialog colorDialog = new ColorDialog();
                colorDialog.Color = Color.Red;
                if (colorDialog.ShowDialog() != DialogResult.OK) return;
                Dialog_NumPannel dialog_NumPannel = new Dialog_NumPannel("亮燈亮度增量大小");
                if (dialog_NumPannel.ShowDialog() != DialogResult.Yes) return;
                int Add_val = (int)dialog_NumPannel.Value;

                dialog_NumPannel = new Dialog_NumPannel("亮燈亮度減量大小");
                if (dialog_NumPannel.ShowDialog() != DialogResult.Yes) return;
                int Sub_val = (int)dialog_NumPannel.Value;
                Set_WS2812B_breathing(list_UDP_Class[0], iPEndPoints[0].Address.ToString(), (byte)Add_val, (byte)Sub_val, colorDialog.Color);
            }
        }
        private void RowsLEDUI_DeviceTableMouseDownRightEvent(string selectedText, List<IPEndPoint> iPEndPoints)
        {
            if (selectedText == ContextMenuStrip_Main.設定亮燈.GetEnumName())
            {
                List<string> list_IP = new List<string>();
                List<UDP_Class> list_UDP_Class = new List<UDP_Class>();
                for (int i = 0; i < iPEndPoints.Count; i++)
                {
                    string IP = iPEndPoints[i].Address.ToString();
                    UDP_Class uDP_Class = List_UDP_Local.SortByPort(iPEndPoints[i].Port);
                    if (uDP_Class != null)
                    {
                        list_IP.Add(IP);
                        list_UDP_Class.Add(uDP_Class);
                    }
                }

                ColorDialog colorDialog = new ColorDialog();
                colorDialog.Color = Color.Red;
                if (colorDialog.ShowDialog() != DialogResult.OK) return;

                Dialog_RowsLED dialog_RowsLED = new Dialog_RowsLED(list_UDP_Class, list_IP, colorDialog.Color);
                dialog_RowsLED.ShowDialog();
            }
            if (selectedText == ContextMenuStrip_Main.設定燈數.GetEnumName())
            {
                if (iPEndPoints.Count == 0) return;

                Dialog_NumPannel dialog_NumPannel = new Dialog_NumPannel("請輸入[燈數]");
                if (dialog_NumPannel.ShowDialog() != DialogResult.Yes) return;
                if (dialog_NumPannel.Value < 50) dialog_NumPannel.Value = 50;
              
                List<Task> taskList = new List<Task>();
                List<RowsLED> rowsLEDs = new List<RowsLED>();
                for (int i = 0; i < iPEndPoints.Count; i++)
                {
                    double value = dialog_NumPannel.Value;
                    string IP = iPEndPoints[i].Address.ToString();
                    int Port = iPEndPoints[i].Port;
                    taskList.Add(Task.Run(() =>
                    {
                        RowsLED rowsLED = this.SQL_GetRowsLED(IP);
                        if (rowsLED != null)
                        {
                            rowsLED.Maximum = (int)value;
                            rowsLEDs.LockAdd(rowsLED);
                        }                   
                    }));
                }
                Task allTask = Task.WhenAll(taskList);
                allTask.Wait();
                this.SQL_ReplaceRowsLED(rowsLEDs);
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
            // RowsLEDUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.Name = "RowsLEDUI";
            this.ResumeLayout(false);

        }
    }
}
