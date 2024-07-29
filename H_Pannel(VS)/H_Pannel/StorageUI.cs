using System;
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
namespace H_Pannel_lib
{
 
    public partial class StorageUI : DeviceBasicUI
    {
      

        private List<Storage> Storages = new List<Storage>();

        public StorageUI()
        {

        }

        public void Add_NewStorage(string IP, int Port)
        {
            Storages = Storages.Add_NewStorage(IP, Port, new Storage(IP, Port));
        }
        public void Add_NewStorage(Storage storage)
        {
            Storages = Storages.Add_NewStorage(storage);
        }
        public void Add_NewStorage(string IP, int Port, Storage storage)
        {
            Storages = Storages.Add_NewStorage(IP, Port, storage);
        }
        public List<Storage> SortByCode(string Code)
        {
            return Storages.SortByCode(Code);
        }
        public List<Storage> SortByBarCode(string Code)
        {
            return Storages.SortByBarCode(Code);
        }
        public void RemoveByIP(string IP)
        {
            Storages = Storages.RemoveByIP(IP);
        }
        public void ReplacePortByIP(string IP, int Port)
        {
            Storages = Storages.ReplacePortByIP(IP, Port);
        }
        public void ReplaceByIP(Storage storage)
        {
            Storages = Storages.ReplaceByIP(storage);
        }
        public void ClearStorage(string IP)
        {
            Storages = Storages.ClearStorage(IP);
        }
        public void ClearStorage(Storage storage)
        {
            storage.ClearStorage();
        }

        public void SetStorages(List<Storage> Storages)
        {
           this.Storages = Storages;
        }
        public List<Storage> GetStorages()
        {
            return Storages;
        }
        public void StoragesClear()
        {
            this.Storages.Clear();
        }
        public string SQL_GetStorageJsonString(string IP)
        {
            List<object[]> list_value = this.SQL_GetAllDeviceTableRows();
            List<object[]> list_value_buf = new List<object[]>();
            list_value_buf = list_value.GetRows((int)enum_DeviceTable.IP, IP);
            if (list_value_buf.Count == 0) return null;
            return list_value_buf[0][(int)enum_DeviceTable.Value].ObjectToString();
        }


        public List<Storage> SQL_GetAllStorage()
        {
            lock (this)
            {
                List<Storage> storages = new List<Storage>();
                List<object[]> list_value = this.SQL_GetAllDeviceTableRows();

                Parallel.ForEach(list_value, value =>
                {
                    string jsonString = value[(int)enum_DeviceTable.Value].ObjectToString();
                    Storage storage = jsonString.JsonDeserializet<Storage>();
                    if (storage != null)
                    {
                        storage.確認效期庫存(true);
                        storage.Port = value[(int)enum_DeviceTable.Port].ObjectToString().StringToInt32();
                        storages.LockAdd(storage);
                    }

                });
                storages = (from value in storages
                            where value != null
                            select value).ToList();
                return storages;
            }
           
        }
        public List<Device> SQL_GetAllDevice()
        {
            List<Device> devices = new List<Device>();
            List<Storage> storages = this.SQL_GetAllStorage();
            for (int i = 0; i < storages.Count; i++)
            {
                devices.Add((Device)storages[i]);
            }
            return devices;
        }
        public List<DeviceBasic> SQL_GetAllDeviceBasic()
        {
            lock (this)
            {
                List<DeviceBasic> storages = new List<DeviceBasic>();
                List<object[]> list_value = this.SQL_GetAllDeviceTableRows();

                Parallel.ForEach(list_value, value =>
                {
                    string jsonString = value[(int)enum_DeviceTable.Value].ObjectToString();
                    DeviceBasic storage = jsonString.JsonDeserializet<DeviceBasic>();
                    if (storage != null)
                    {
                        storage.確認效期庫存(true);
                        storages.LockAdd(storage);
                    }

                });
                storages = (from value in storages
                            where value != null
                            select value).ToList();
                return storages;
            }
        }
        public Storage SQL_GetStorage(Storage storage)
        {
            return SQL_GetStorage(storage.IP);
        }
        public Storage SQL_GetStorage(string IP)
        {
            //List<object[]> list_value = this.SQL_GetAllDeviceTableRows();
            List<object[]> list_value_buf = new List<object[]>();       
            list_value_buf = this.sqL_DataGridView_DeviceTable.SQL_GetRows((int)enum_DeviceTable.IP, IP, false);
            if (list_value_buf.Count == 0) return null;

            string jsonString = list_value_buf[0][(int)enum_DeviceTable.Value].ObjectToString();
            if (jsonString.StringIsEmpty()) return null;
            Storage storage = jsonString.JsonDeserializet<Storage>();
            storage.Port = list_value_buf[0][(int)enum_DeviceTable.Port].ObjectToString().StringToInt32();
            return storage;
        }
        public bool SQL_ReplaceStorage(Storage storage)
        {
            string IP = storage.IP;
            List<object[]> list_value = this.sqL_DataGridView_DeviceTable.SQL_GetRows(enum_DeviceTable.IP.GetEnumName(), IP, false);
            if (list_value.Count == 0) return false;
            storage.Port = list_value[0][(int)enum_DeviceTable.Port].ObjectToString().StringToInt32();
            list_value[0][(int)enum_DeviceTable.Value] = storage.JsonSerializationt<Storage>();
            this.sqL_DataGridView_DeviceTable.SQL_Replace(enum_DeviceTable.GUID.GetEnumName(), list_value[0][(int)enum_DeviceTable.GUID].ObjectToString(), list_value[0], false);
            return true;
        }
        public bool SQL_ReplaceStorage(List<Storage> storage)
        {
            List<object[]> list_value = this.sqL_DataGridView_DeviceTable.SQL_GetAllRows(false);
            List<object[]> list_value_buf = new List<object[]>();
            List<string> list_Replace_SerchValue = new List<string>();
            List<object[]> list_Replace_Value = new List<object[]>();
            if (list_value.Count == 0) return false;
            for (int i = 0; i < storage.Count; i++)
            {
                list_value_buf = list_value.GetRows((int)enum_DeviceTable.IP, storage[i].IP);
                if (list_value_buf.Count > 0)
                {
                    storage[i].Port = list_value_buf[0][(int)enum_DeviceTable.Port].ObjectToString().StringToInt32();
                    list_value_buf[0][(int)enum_DeviceTable.Value] = storage[i].JsonSerializationt<Storage>();
                    list_Replace_SerchValue.Add(list_value_buf[0][(int)enum_DeviceTable.GUID].ObjectToString());
                    list_Replace_Value.Add(list_value_buf[0]);
                }
            }
            this.sqL_DataGridView_DeviceTable.SQL_ReplaceExtra(enum_DeviceTable.GUID.GetEnumName(), list_Replace_SerchValue, list_Replace_Value, false);
            return true;
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
                            value_load[(int)enum_DeviceTable.Value] = new Storage(IP, Port).JsonSerializationt<Storage>();
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
                        value_load[(int)enum_DeviceTable.Value] = new Storage(IP, Port).JsonSerializationt<Storage>();
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
                Storage storage = jsonString.JsonDeserializet<Storage>();
                if (storage == null) return;
                storage.IP = IP;
                value[(int)enum_DeviceTable.Value] = storage.JsonSerializationt();
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
                Storage storage = jsonString.JsonDeserializet<Storage>();
                if (storage == null) return;
                storage.IP = IP;
                storage.Port = Port;
                value[(int)enum_DeviceTable.Value] = storage.JsonSerializationt();
                this.sqL_DataGridView_DeviceTable.SQL_AddRow(value, true);
            }
        }


        static public bool Check_LEDBytesBuf_Diff(Storage storage)
        {
            return Check_LEDBytesBuf_Diff(storage.LED_Bytes, storage.LED_Bytes_buf);
        }
        static public bool Check_LEDBytesBuf_Diff(Storage storage, byte[] bytes2)
        {
            return Check_LEDBytesBuf_Diff(storage.LED_Bytes, bytes2);
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
    }



}
