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
    public class DrawerUI : DeviceBasicUI
    {
        public delegate void DrawerChangeEventHandler(List<Drawer> drawers);
        public event DrawerChangeEventHandler DrawerChangeEvent;

        public static int NumOfColumn = 4;
        public static int NumOfRow = 8;

        private List<Drawer> Drawers = new List<Drawer>();
        public DrawerUI()
        {

        }
        public void Add_NewDrawer(Drawer drawer)
        {
            Drawers = Drawers.Add_NewDrawer(drawer);
        }
        public void Add_NewDrawer(Box box)
        {
            Drawers = Drawers.Add_NewDrawer(box);
        }
        public void Add_NewDrawer(string IP, int Port)
        {
            Drawers = Drawers.Add_NewDrawer(IP, Port);
        }
        public List<Box> SortByCode(string Code)
        {
            return Drawers.SortByCode(Code);
        }
        public List<Box> SortByBarCode(string Code)
        {
            return Drawers.SortByBarCode(Code);
        }
        public void RemoveByIP(string IP)
        {
            Drawers = Drawers.RemoveByIP(IP);
        }
        public void ReplacePortByIP(string IP, int Port)
        {
            Drawers = Drawers.ReplacePortByIP(IP, Port);
        }
        public void ReplaceByIP(Drawer drawer)
        {
            Drawers = Drawers.ReplaceByIP(drawer);
        }
        public void ClearDrawer(string IP)
        {
            Drawers = Drawers.ClearDrawer(IP);
        }

        public void SetDrawers(List<Drawer> Drawers)
        {
            this.Drawers = Drawers;
        }
        public void DrawersClear()
        {
            this.Drawers.Clear();
        }
    
        public string SQL_GetDrawerJsonString(string IP)
        {
            List<object[]> list_value = this.SQL_GetDeviceTableRows(IP);
            if (list_value.Count == 0) return null;
            return list_value[0][(int)enum_DeviceTable.Value].ObjectToString();
        }
        public Drawer SQL_GetDrawer(Box box)
        {
            return SQL_GetDrawer(box.IP);
        }
        public Drawer SQL_GetDrawer(Drawer drawer)
        {
            return SQL_GetDrawer(drawer.IP);
        }
        public Drawer SQL_GetDrawer(string IP)
        {
            string jsonString = this.SQL_GetDrawerJsonString(IP);
            if (jsonString.StringIsEmpty()) return null;
            Drawer Drawer = jsonString.JsonDeserializet<Drawer>();
            Drawer.BoxInit();
            if (DrawerChangeEvent != null)
            {
                List<Drawer> drawers = new List<Drawer>();
                drawers.Add(Drawer);
                DrawerChangeEvent(drawers);
            }
            return Drawer;
        }
        virtual public Box SQL_GetBox(Box box)
        {
            return SQL_GetBox(box.IP, box.Column, box.Row);
        }
        public Box SQL_GetBox(string IP , int col , int row)
        {
            string jsonString = this.SQL_GetDrawerJsonString(IP);
            if (jsonString.StringIsEmpty()) return null;
            Drawer Drawer = jsonString.JsonDeserializet<Drawer>();
            Drawer.BoxInit();
            return Drawer.Boxes[col][row];
        }



        public List<Drawer> SQL_GetAllDrawers()
        {
            List<Drawer> drawers = new List<Drawer>();
            List<object[]> list_value = new List<object[]>();
            list_value = this.SQL_GetAllDeviceTableRows();
            drawers = DrawerMethod.SQL_GetAllDrawers(list_value);
            foreach (Drawer drawer in drawers)
            {
                drawer.BoxInit();
            }
            if (DrawerChangeEvent != null)
            {
                DrawerChangeEvent(drawers);
            }
            return drawers;
        }
        public List<Device> SQL_GetAllDevice()
        {
            List<Device> devices = new List<Device>();
            List<Drawer> drawers = this.SQL_GetAllDrawers();
            devices = drawers.GetAllDevice();
            return devices;
        }
        public List<DeviceBasic> SQL_GetAllDeviceBasic()
        {
            List<DeviceBasic> deviceBasics = new List<DeviceBasic>();
            List<object[]> list_value = this.SQL_GetAllDeviceTableRows();
            Parallel.ForEach(list_value, value =>
            {
                string jsonString = value[(int)enum_DeviceTable.Value].ObjectToString();
                DrawerBasic drawer = jsonString.JsonDeserializet<DrawerBasic>();
                for (int i = 0; i < drawer.Boxes.Count; i++)
                {
                    for (int k = 0; k < drawer.Boxes[i].Length; k++)
                    {
                        deviceBasics.LockAdd(drawer.Boxes[i][k]);
                    }
                }
            });

            deviceBasics = (from value in deviceBasics
                            where value != null
                            select value).ToList();
            return deviceBasics;
            
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
                            value_load[(int)enum_DeviceTable.Value] = new Drawer(IP, Port).JsonSerializationt<Drawer>();
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
                        value_load[(int)enum_DeviceTable.Value] = new Drawer(IP, Port).JsonSerializationt<Drawer>();
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
                value[(int)enum_DeviceTable.Value] = new Drawer(IP, Port).JsonSerializationt<Drawer>();
                this.sqL_DataGridView_DeviceTable.SQL_AddRow(value, true);
            }
            else
            {
                string GUID = list_SQL_Value_buf[0][(int)enum_DeviceTable.GUID].ObjectToString();
                list_SQL_Value_buf[0][(int)enum_DeviceTable.Port] = Port;
                string jsonString = list_SQL_Value_buf[0][(int)enum_DeviceTable.Value].ObjectToString();
                if (jsonString.StringIsEmpty()) return;
                Drawer Drawer = jsonString.JsonDeserializet<Drawer>();
                if (Drawer == null) return;
                Drawer.Port = Port;
                list_SQL_Value_buf[0][(int)enum_DeviceTable.Value] = Drawer.JsonSerializationt();
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
                Drawer Drawer = jsonString.JsonDeserializet<Drawer>();
                if (Drawer == null) return;
                Drawer.IP = IP;
                value[(int)enum_DeviceTable.Value] = Drawer.JsonSerializationt();
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
                Drawer Drawer = jsonString.JsonDeserializet<Drawer>();
                if (Drawer == null) return;
                Drawer.IP = IP;
                Drawer.Port = Port;
                value[(int)enum_DeviceTable.Value] = Drawer.JsonSerializationt();
                this.sqL_DataGridView_DeviceTable.SQL_AddRow(value, true);
            }
        }
        public bool SQL_ReplaceDrawer(Drawer Drawer)
        {
            string IP = Drawer.IP;
            List<object[]> list_value = this.sqL_DataGridView_DeviceTable.SQL_GetRows(enum_DeviceTable.IP.GetEnumName(), IP, false);
            if (list_value.Count == 0) return false;
            list_value[0][(int)enum_DeviceTable.Value] = Drawer.JsonSerializationt<Drawer>();
            this.sqL_DataGridView_DeviceTable.SQL_Replace(enum_DeviceTable.GUID.GetEnumName(), list_value[0][(int)enum_DeviceTable.GUID].ObjectToString(), list_value[0], false);
            if (DrawerChangeEvent != null)
            {
                List<Drawer> drawers = new List<Drawer>();
                drawers.Add(Drawer);
                DrawerChangeEvent(drawers);
            }
            return true;
        }
        public bool SQL_ReplaceDrawer(List<Drawer> Drawer)
        {
            List<object[]> list_value = this.sqL_DataGridView_DeviceTable.SQL_GetAllRows(false);
            List<object[]> list_value_buf = new List<object[]>();
            List<string> list_Replace_SerchValue = new List<string>();
            List<object[]> list_Replace_Value = new List<object[]>();
            if (list_value.Count == 0) return false;
            for (int i = 0; i < Drawer.Count; i++)
            {
                list_value_buf = list_value.GetRows((int)enum_DeviceTable.IP, Drawer[i].IP);
                if (list_value_buf.Count > 0)
                {
                    list_value_buf[0][(int)enum_DeviceTable.Value] = Drawer[i].JsonSerializationt<Drawer>();
                    list_Replace_SerchValue.Add(list_value_buf[0][(int)enum_DeviceTable.GUID].ObjectToString());
                    list_Replace_Value.Add(list_value_buf[0]);
                }
            }
            this.sqL_DataGridView_DeviceTable.SQL_ReplaceExtra(enum_DeviceTable.GUID.GetEnumName(), list_Replace_SerchValue, list_Replace_Value, false);
            if (DrawerChangeEvent != null) DrawerChangeEvent(Drawer);
            return true;
        }
   
    }
}
