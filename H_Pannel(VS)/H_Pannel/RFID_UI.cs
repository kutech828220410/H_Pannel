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
using H_Pannel_lib;

namespace H_Pannel_lib
{
    public partial class RFID_UI : UserControl
    {
        #region 靜態參數
        static public bool Set_LockOpen(UDP_Class uDP_Class, string IP , int Num)
        {
            return Communication.Set_OutputPINTrigger(uDP_Class, IP, Num, true);
        }
        #endregion
        public bool ConsoleWrite = true;
        private bool flag_UDP_Class_Init = false;
        public delegate void DrawerChangeEventHandler(List<RFIDClass> rRIDClass);
        public event DrawerChangeEventHandler DrawerChangeEvent;

        public object Enum_ContextMenuStrip_UDP_DataReceive;
        public object Enum_ContextMenuStrip_DeviceTable;
        public delegate void UDP_DataReceiveMouseDownRightEventHandler(string selectedText, List<IPEndPoint> iPEndPoints);
        public event UDP_DataReceiveMouseDownRightEventHandler UDP_DataReceiveMouseDownRightEvent;
        public delegate void DeviceTableMouseDownRightEventHandler(string selectedText, List<IPEndPoint> iPEndPoints);
        public event DeviceTableMouseDownRightEventHandler DeviceTableMouseDownRightEvent;
        public List<UDP_Class> List_UDP_Server = new List<UDP_Class>();
        public List<UDP_Class> List_UDP_Local = new List<UDP_Class>();

        private string dataBaseName = "TEST";
        private string userName = "root";
        private string password = "user82822040";
        private string iP = "localhost";
        private uint port = 3306;
        private MySql.Data.MySqlClient.MySqlSslMode mySqlSslMode = MySql.Data.MySqlClient.MySqlSslMode.None;
        private string tableName = "RFID_Device_Jsonstring";
        private MyThread MyThread_PING;
        private string IP_UDP_DataReceive_Mask = "XXX.XXX.XXX.XXX";
        private string IP_DeviceTable_Mask = "XXX.XXX.XXX.XXX";
        public int TimeOut = 3000;

        private List<string> _UDP_ServerPorts = new List<string>();
        [Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design", "System.Drawing.Design.UITypeEditor, System.Drawing")]
        [ReadOnly(false), Browsable(true), Category("UDP Config"), Description(""), DefaultValue("")]
        public List<string> UDP_ServerPorts
        {
            get { return _UDP_ServerPorts; }
            set
            {
                _UDP_ServerPorts = value;
            }
        }
        private List<string> _UDP_LocalPorts = new List<string>();
        [Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design", "System.Drawing.Design.UITypeEditor, System.Drawing")]
        [ReadOnly(false), Browsable(true), Category("UDP Config"), Description(""), DefaultValue("")]
        public List<string> UDP_LocalPorts
        {
            get { return _UDP_LocalPorts; }
            set
            {
                _UDP_LocalPorts = value;
            }
        }


        [ReadOnly(false), Browsable(true), Category("DataBase"), Description(""), DefaultValue("")]
        public string DataBaseName { get => dataBaseName; set => dataBaseName = value; }
        [ReadOnly(false), Browsable(true), Category("DataBase"), Description(""), DefaultValue("")]
        public string UserName { get => userName; set => userName = value; }
        [ReadOnly(false), Browsable(true), Category("DataBase"), Description(""), DefaultValue("")]
        public string Password { get => password; set => password = value; }
        [ReadOnly(false), Browsable(true), Category("DataBase"), Description(""), DefaultValue("")]
        public string IP { get => iP; set => iP = value; }
        [ReadOnly(false), Browsable(true), Category("DataBase"), Description(""), DefaultValue("")]
        public uint Port { get => port; set => port = value; }
        [ReadOnly(false), Browsable(true), Category("DataBase"), Description(""), DefaultValue("")]
        public string TableName { get => tableName; set => tableName = value; }

        public String IP_Adress
        {
            get
            {
                int[] temp = new int[4];
                int.TryParse(this.textBox_IP_Adress_A.Text, out temp[0]);
                int.TryParse(this.textBox_IP_Adress_B.Text, out temp[1]);
                int.TryParse(this.textBox_IP_Adress_C.Text, out temp[2]);
                int.TryParse(this.textBox_IP_Adress_D.Text, out temp[3]);
                return temp[0].ToString() + "." + temp[1].ToString() + "." + temp[2].ToString() + "." + temp[3].ToString();
            }
            set
            {
                string[] str = value.Split('.');
                if (str.Length == 4)
                {
                    bool flag_OK = true;
                    int[] temp = new int[4];
                    if (!int.TryParse(str[0], out temp[0])) flag_OK = false;
                    if (!int.TryParse(str[1], out temp[1])) flag_OK = false;
                    if (!int.TryParse(str[2], out temp[2])) flag_OK = false;
                    if (!int.TryParse(str[3], out temp[3])) flag_OK = false;
                    if (flag_OK)
                    {
                        this.textBox_IP_Adress_A.Text = temp[0].ToString();
                        this.textBox_IP_Adress_B.Text = temp[1].ToString();
                        this.textBox_IP_Adress_C.Text = temp[2].ToString();
                        this.textBox_IP_Adress_D.Text = temp[3].ToString();
                    }
                }
            }
        }
        public String Subnet
        {
            get
            {
                int[] temp = new int[4];
                int.TryParse(this.textBox_Subnet_A.Text, out temp[0]);
                int.TryParse(this.textBox_Subnet_B.Text, out temp[1]);
                int.TryParse(this.textBox_Subnet_C.Text, out temp[2]);
                int.TryParse(this.textBox_Subnet_D.Text, out temp[3]);
                return temp[0].ToString() + "." + temp[1].ToString() + "." + temp[2].ToString() + "." + temp[3].ToString();
            }
            set
            {
                string[] str = value.Split('.');
                if (str.Length == 4)
                {
                    bool flag_OK = true;
                    int[] temp = new int[4];
                    if (!int.TryParse(str[0], out temp[0])) flag_OK = false;
                    if (!int.TryParse(str[1], out temp[1])) flag_OK = false;
                    if (!int.TryParse(str[2], out temp[2])) flag_OK = false;
                    if (!int.TryParse(str[3], out temp[3])) flag_OK = false;
                    if (flag_OK)
                    {
                        this.textBox_Subnet_A.Text = temp[0].ToString();
                        this.textBox_Subnet_B.Text = temp[1].ToString();
                        this.textBox_Subnet_C.Text = temp[2].ToString();
                        this.textBox_Subnet_D.Text = temp[3].ToString();
                    }
                }
            }
        }
        public String Gateway
        {
            get
            {
                int[] temp = new int[4];
                int.TryParse(this.textBox_Gateway_A.Text, out temp[0]);
                int.TryParse(this.textBox_Gateway_B.Text, out temp[1]);
                int.TryParse(this.textBox_Gateway_C.Text, out temp[2]);
                int.TryParse(this.textBox_Gateway_D.Text, out temp[3]);
                return temp[0].ToString() + "." + temp[1].ToString() + "." + temp[2].ToString() + "." + temp[3].ToString();
            }
            set
            {
                string[] str = value.Split('.');
                if (str.Length == 4)
                {
                    bool flag_OK = true;
                    int[] temp = new int[4];
                    if (!int.TryParse(str[0], out temp[0])) flag_OK = false;
                    if (!int.TryParse(str[1], out temp[1])) flag_OK = false;
                    if (!int.TryParse(str[2], out temp[2])) flag_OK = false;
                    if (!int.TryParse(str[3], out temp[3])) flag_OK = false;
                    if (flag_OK)
                    {
                        this.textBox_Gateway_A.Text = temp[0].ToString();
                        this.textBox_Gateway_B.Text = temp[1].ToString();
                        this.textBox_Gateway_C.Text = temp[2].ToString();
                        this.textBox_Gateway_D.Text = temp[3].ToString();
                    }
                }
            }
        }
        public String DNS
        {
            get
            {
                int[] temp = new int[4];
                int.TryParse(this.textBox_DNS_A.Text, out temp[0]);
                int.TryParse(this.textBox_DNS_B.Text, out temp[1]);
                int.TryParse(this.textBox_DNS_C.Text, out temp[2]);
                int.TryParse(this.textBox_DNS_D.Text, out temp[3]);
                return temp[0].ToString() + "." + temp[1].ToString() + "." + temp[2].ToString() + "." + temp[3].ToString();
            }
            set
            {
                string[] str = value.Split('.');
                if (str.Length == 4)
                {
                    bool flag_OK = true;
                    int[] temp = new int[4];
                    if (!int.TryParse(str[0], out temp[0])) flag_OK = false;
                    if (!int.TryParse(str[1], out temp[1])) flag_OK = false;
                    if (!int.TryParse(str[2], out temp[2])) flag_OK = false;
                    if (!int.TryParse(str[3], out temp[3])) flag_OK = false;
                    if (flag_OK)
                    {
                        this.textBox_DNS_A.Text = temp[0].ToString();
                        this.textBox_DNS_B.Text = temp[1].ToString();
                        this.textBox_DNS_C.Text = temp[2].ToString();
                        this.textBox_DNS_D.Text = temp[3].ToString();
                    }
                }
            }
        }
        public String Server_IP_Adress
        {
            get
            {
                int[] temp = new int[4];
                int.TryParse(this.textBox_Server_IP_Adress_A.Text, out temp[0]);
                int.TryParse(this.textBox_Server_IP_Adress_B.Text, out temp[1]);
                int.TryParse(this.textBox_Server_IP_Adress_C.Text, out temp[2]);
                int.TryParse(this.textBox_Server_IP_Adress_D.Text, out temp[3]);
                return temp[0].ToString() + "." + temp[1].ToString() + "." + temp[2].ToString() + "." + temp[3].ToString();
            }
            set
            {
                string[] str = value.Split('.');
                if (str.Length == 4)
                {
                    bool flag_OK = true;
                    int[] temp = new int[4];
                    if (!int.TryParse(str[0], out temp[0])) flag_OK = false;
                    if (!int.TryParse(str[1], out temp[1])) flag_OK = false;
                    if (!int.TryParse(str[2], out temp[2])) flag_OK = false;
                    if (!int.TryParse(str[3], out temp[3])) flag_OK = false;
                    if (flag_OK)
                    {
                        this.textBox_Server_IP_Adress_A.Text = temp[0].ToString();
                        this.textBox_Server_IP_Adress_B.Text = temp[1].ToString();
                        this.textBox_Server_IP_Adress_C.Text = temp[2].ToString();
                        this.textBox_Server_IP_Adress_D.Text = temp[3].ToString();
                    }
                }
            }
        }
        public String Local_Port
        {
            get
            {
                UInt16 temp = 0;
                UInt16.TryParse(this.textBox_Local_Port.Text, out temp);
                return temp.ToString();
            }
            set
            {
                UInt16 temp = 0;
                if (UInt16.TryParse(value, out temp))
                {
                    this.textBox_Local_Port.Text = value;
                }
            }
        }
        public String Server_Port
        {
            get
            {
                UInt16 temp = 0;
                UInt16.TryParse(this.textBox_Server_Port.Text, out temp);
                return temp.ToString();
            }
            set
            {
                UInt16 temp = 0;
                if (UInt16.TryParse(value, out temp))
                {
                    this.textBox_Server_Port.Text = value;
                }
            }
        }
        public String SSID
        {
            get
            {
                return this.textBox_SSID.Text;
            }
            set
            {
                this.textBox_SSID.Text = value;
            }
        }
        public String _Password
        {
            get
            {
                return this.textBox_Password.Text;
            }
            set
            {
                this.textBox_Password.Text = value;
            }
        }
        public String Station
        {
            get
            {
                UInt16 temp = 0;
                UInt16.TryParse(this.textBox_Station.Text, out temp);
                return temp.ToString();
            }
            set
            {
                UInt16 temp = 0;
                if (UInt16.TryParse(value, out temp))
                {
                    this.textBox_Station.Text = value;
                }
            }
        }
        private String UDP_SendTime
        {
            get
            {
                UInt16 temp = 0;
                UInt16.TryParse(this.textBox_UDP_SendTime.Text, out temp);
                return temp.ToString();
            }
            set
            {
                UInt16 temp = 0;
                if (UInt16.TryParse(value, out temp))
                {
                    this.textBox_UDP_SendTime.Text = value;
                }
            }
        }
        public String RFID_Enable
        {
            set
            {
                int temp = 0;
                int.TryParse(value, out temp);
                if (this.IsHandleCreated)
                {
                    this.Invoke(new Action(delegate
                    {
                        this.checkBox_RFID_Enable_01.Checked = myConvert.Int32GetBit(temp, 0);
                        this.checkBox_RFID_Enable_02.Checked = myConvert.Int32GetBit(temp, 1);
                        this.checkBox_RFID_Enable_03.Checked = myConvert.Int32GetBit(temp, 2);
                        this.checkBox_RFID_Enable_04.Checked = myConvert.Int32GetBit(temp, 3);
                        this.checkBox_RFID_Enable_05.Checked = myConvert.Int32GetBit(temp, 4);
                        this.checkBox_RFID_Enable_06.Checked = myConvert.Int32GetBit(temp, 5);
                        this.checkBox_RFID_Enable_07.Checked = myConvert.Int32GetBit(temp, 6);
                        this.checkBox_RFID_Enable_08.Checked = myConvert.Int32GetBit(temp, 7);
                    }));
                }
            }
            get
            {
                int temp = 0;
                temp = myConvert.Int32SetBit(this.checkBox_RFID_Enable_01.Checked, temp, 0);
                temp = myConvert.Int32SetBit(this.checkBox_RFID_Enable_02.Checked, temp, 1);
                temp = myConvert.Int32SetBit(this.checkBox_RFID_Enable_03.Checked, temp, 2);
                temp = myConvert.Int32SetBit(this.checkBox_RFID_Enable_04.Checked, temp, 3);
                temp = myConvert.Int32SetBit(this.checkBox_RFID_Enable_05.Checked, temp, 4);
                temp = myConvert.Int32SetBit(this.checkBox_RFID_Enable_06.Checked, temp, 5);
                temp = myConvert.Int32SetBit(this.checkBox_RFID_Enable_07.Checked, temp, 6);
                temp = myConvert.Int32SetBit(this.checkBox_RFID_Enable_08.Checked, temp, 7);
                return temp.ToString();
            }
        }

        [Serializable]
        public class UDP_READ
        {
            MyConvert myConvert = new MyConvert();
            private string iP = "0.0.0.0";
            private int port = 0;
            private string version = "";
            private int input = 0;
            private int output = 0;
            private int rSSI = -100;
            private string cardID_01 = "";
            private string cardID_02 = "";
            private string cardID_03 = "";
            private string cardID_04 = "";
            private string cardID_05 = "";
            private int input_dir = 0;
            private int output_dir = 0;
            private int rFID_Enable = 0;

            public string IP { get => iP; set => iP = value; }
            public int Port { get => port; set => port = value; }
            public string Version { get => version; set => version = value; }
            public int Input { get => input; set => input = value; }
            public int Output { get => output; set => output = value; }
            public int RSSI { get => rSSI; set => rSSI = value; }
            public string CardID_01 { get => cardID_01; set => cardID_01 = value; }
            public string CardID_02 { get => cardID_02; set => cardID_02 = value; }
            public string CardID_03 { get => cardID_03; set => cardID_03 = value; }
            public string CardID_04 { get => cardID_04; set => cardID_04 = value; }
            public string CardID_05 { get => cardID_05; set => cardID_05 = value; }
            public int Input_dir { get => input_dir; set => input_dir = value; }
            public int Output_dir { get => output_dir; set => output_dir = value; }
            public int RFID_Enable { get => rFID_Enable; set => rFID_Enable = value; }

            public bool Get_RFID_Enable(int index)
            {
                return this.myConvert.Int32GetBit(RFID_Enable, index);
            }
            public bool Get_Input_dir(int index)
            {
                return this.myConvert.Int32GetBit(Input_dir, index);
            }
            public bool Get_Output_dir(int index)
            {
                return this.myConvert.Int32GetBit(Output_dir, index);
            }
            public bool Get_Input(int index)
            {
                return this.myConvert.Int32GetBit(Input, index);
            }
            public bool Get_Output(int index)
            {
                return this.myConvert.Int32GetBit(Output, index);
            }
            public string Get_CardID(int index)
            {
                if(index == 0)
                {
                    return CardID_01;
                }
                if (index == 1)
                {
                    return CardID_02;
                }
                if (index == 2)
                {
                    return CardID_03;
                }
                if (index == 3)
                {
                    return CardID_04;
                }
                if (index == 4)
                {
                    return CardID_05;
                }
                return "";
            }

        }

        public enum enum_UDP_DataReceive
        {
            GUID,
            編號,
            IP,
            Port,
            Readline,
            StartTime,
            Time,
            State,
        }
        public enum enum_DeviceTable
        {
            GUID,
            IP,
            Port,
            Value,
        }
        public enum enum_DeviceTable_匯入
        {
            IP,
            Port,
        }
        public enum enum_DeviceTable_匯出
        {
            IP,
            Port,
        }
        public enum ContextMenuStrip_UDP_DataReceive
        {
            重新啟動,
            OTA線上更新,
            設置發送時間,
            設定ServerConfig,
            設定GatewayConfig,
            設定LocalPort,
            選取顯示網段,
            RFID測試頁面,
            設定LED參數,
        }
        public enum ContextMenuStrip_DeviceTable
        {
            匯入,
            匯出,
            新增儲位,
            修改儲位,
            刪除選取資料,
            重新啟動,
            OTA線上更新,
            設置發送時間,
            設定ServerConfig,
            設定GatewayConfig,
            設定LocalPort,
            選取顯示網段,
            儲位設定,
            刷新,
        }
        public class RFID_UID_Class
        {
            private string uID = "";
            private string iP = "";
            private int num = -1;

            public RFID_UID_Class(string UID, string IP, int Num)
            {
                this.uID = UID;
                this.iP = IP;
                this.num = Num;
            }

            public string UID { get => uID; set => uID = value; }
            public string IP { get => iP; set => iP = value; }
            public int Num { get => num; set => num = value; }
        }
        private Stopwatch stopwatch = new Stopwatch();
        private MyThread MyThread_SqlDataRefrsh;
        private MyConvert myConvert = new MyConvert();
        private MySerialPort mySerialPort = new MySerialPort();
        private List<object[]> list_UDP_Rx = new List<object[]>();

        public RFID_UI()
        {
            InitializeComponent();
        }
        public void Init(SQLUI.SQL_DataGridView.ConnentionClass connentionClass)
        {
            this.Init(connentionClass.DataBaseName, connentionClass.UserName, connentionClass.Password, connentionClass.IP, connentionClass.Port, connentionClass.MySqlSslMode);
        }
        public void Init(string DataBaseName, string UserName, string Password, string IP, uint Port, MySql.Data.MySqlClient.MySqlSslMode SSLMode)
        {
            this.DataBaseName = DataBaseName;
            this.UserName = UserName;
            this.Password = Password;
            this.IP = IP;
            this.Port = Port;
            this.mySqlSslMode = SSLMode;
            this.Init();
        }
        public void Init()
        {
            this.stopwatch.Start();
            
            SQLUI.SQL_DataGridView.SQL_Set_Properties(this.sqL_DataGridView_DeviceTable, this.TableName, DataBaseName, UserName, Password, IP, Port, mySqlSslMode);
            this.sqL_DataGridView_UDP_DataReceive.Init();
            this.sqL_DataGridView_DeviceTable.Init();
            if (flag_UDP_Class_Init == false)
            {
      
                this.sqL_DataGridView_UDP_DataReceive.DataGridRowsChangeEvent += SqL_DataGridView_UDP_DataReceive_DataGridRowsChangeEvent;
                this.sqL_DataGridView_UDP_DataReceive.DataGridRefreshEvent += SqL_DataGridView_UDP_DataReceive_DataGridRefreshEvent;
                this.sqL_DataGridView_UDP_DataReceive.DataGridRowsChangeRefEvent += SqL_DataGridView_UDP_DataReceive_DataGridRowsChangeRefEvent;
                this.sqL_DataGridView_UDP_DataReceive.MouseDown += SqL_DataGridView_UDP_DataReceive_MouseDown;
               
                if (!this.sqL_DataGridView_DeviceTable.SQL_IsTableCreat()) this.sqL_DataGridView_DeviceTable.SQL_CreateTable();
                this.sqL_DataGridView_DeviceTable.DataGridRowsChangeEvent += SqL_DataGridView_DeviceTable_DataGridRowsChangeEvent;
                this.sqL_DataGridView_DeviceTable.DataGridRowsChangeRefEvent += SqL_DataGridView_DeviceTable_DataGridRowsChangeRefEvent;
                this.sqL_DataGridView_DeviceTable.MouseDown += SqL_DataGridView_DeviceTable_MouseDown;
                this.sqL_DataGridView_DeviceTable.SQL_GetAllRows(true);

                for (int i = 0; i < this.UDP_ServerPorts.Count; i++)
                {
                    UDP_Class uDP_Class = new UDP_Class("0.0.0.0", this.UDP_ServerPorts[i].StringToInt32());
                    uDP_Class.ConsoleWrite = false;
                    List_UDP_Server.Add(uDP_Class);
                }
                for (int i = 0; i < this.UDP_LocalPorts.Count; i++)
                {
                    UDP_Class uDP_Class = new UDP_Class("0.0.0.0", this.UDP_LocalPorts[i].StringToInt32());
                    uDP_Class.ConsoleWrite = false;
                    List_UDP_Local.Add(uDP_Class);
                }
                Communication.ConsoleWrite = ConsoleWrite;
                this.FindForm().FormClosing += RFID_UI_FormClosing;
             

                this.rJ_Button_Station_Write.MouseDownEvent += RJ_Button_Station_Write_MouseDownEvent;
                this.rJ_Button_Read.MouseDownEvent += RJ_Button_Read_MouseDownEvent;
                this.rJ_Button_Write.MouseDownEvent += RJ_Button_Write_MouseDownEvent;
 
                this.rJ_Button_輸出方向_讀取.MouseDownEvent += RJ_Button_輸出方向_讀取_MouseDownEvent;
                this.rJ_Button_輸出方向_寫入.MouseDownEvent += RJ_Button_輸出方向_寫入_MouseDownEvent;
                this.rJ_Button_輸入方向_讀取.MouseDownEvent += RJ_Button_輸入方向_讀取_MouseDownEvent;
                this.rJ_Button_輸入方向_寫入.MouseDownEvent += RJ_Button_輸入方向_寫入_MouseDownEvent;
                this.rJ_Button_輸入_讀取.MouseDownEvent += RJ_Button_輸入_讀取_MouseDownEvent;
                this.rJ_Button_輸出_寫入.MouseDownEvent += RJ_Button_輸出_寫入_MouseDownEvent;
                this.checkBox_輸出01.Click += CheckBox_輸出01_Click;
                this.checkBox_輸出02.Click += CheckBox_輸出02_Click;
                this.checkBox_輸出03.Click += CheckBox_輸出03_Click;
                this.checkBox_輸出04.Click += CheckBox_輸出04_Click;
                this.checkBox_輸出05.Click += CheckBox_輸出05_Click;
                this.checkBox_輸出06.Click += CheckBox_輸出06_Click;
                this.checkBox_輸出07.Click += CheckBox_輸出07_Click;
                this.checkBox_輸出08.Click += CheckBox_輸出08_Click;
                this.checkBox_輸出09.Click += CheckBox_輸出09_Click;
                this.checkBox_輸出10.Click += CheckBox_輸出10_Click;

                mySerialPort.Init(this.textBox_COM.Text, 115200, 8, Parity.None, StopBits.One, false);

                this.MyThread_SqlDataRefrsh = new MyThread();
                this.MyThread_SqlDataRefrsh.AutoRun(true);
                this.MyThread_SqlDataRefrsh.Add_Method(this.sub_SqlDataRefrsh);
                this.MyThread_SqlDataRefrsh.SetSleepTime(50);
                this.MyThread_SqlDataRefrsh.Trigger();

                this.sqL_DataGridView_PING.Init();
                this.MyThread_PING = new MyThread();
                this.MyThread_PING = new MyThread();
                this.MyThread_PING.AutoRun(true);
                this.MyThread_PING.Add_Method(this.sub_PING);
                this.MyThread_PING.SetSleepTime(100);

            }

            flag_UDP_Class_Init = true;

        }

       

        virtual public bool Set_ESP32_Restart(string IP, int Port)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
            if (uDP_Class != null)
            {
                return Communication.Set_ESP32_Restart(uDP_Class, IP);
            }
            return false;
        }
        virtual public bool Set_OTAUpdate(string IP, int Port)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
            if (uDP_Class != null)
            {
                return Communication.Set_OTAUpdate(uDP_Class, IP);
            }
            return false;
        }
        virtual public bool Set_UDP_SendTime(string IP, int Port, int ms)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
            if (uDP_Class != null)
            {
                return Communication.Set_UDP_SendTime(uDP_Class, IP, ms);
            }
            return false;
        }
        virtual public bool Set_ServerConfig(string IP, int Port, string ServerIP, int ServerPort)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
            if (uDP_Class != null)
            {
                return Communication.Set_ServerConfig(uDP_Class, IP, ServerIP, ServerPort);
            }
            return false;
        }
        virtual public bool Set_GatewayConfig(string IP, int Port, string Geteway)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
            if (uDP_Class != null)
            {
                return Communication.Set_GatewayConfig(uDP_Class, IP, Geteway);
            }
            return false;
        }
        virtual public bool Set_LocalPort(string IP, int Port, int LocalPort)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
            if (uDP_Class != null)
            {
                return Communication.Set_LocalPort(uDP_Class, IP, LocalPort);
            }
            return false;
        }
        virtual public bool Set_OutputPINTrigger(string IP, int Port, int PIN_Num, bool value)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
            if (uDP_Class != null)
            {
                return Communication.Set_OutputPINTrigger(uDP_Class, IP, PIN_Num, value);
            }
            return false;
        }
        virtual public bool Set_OutputTrigger(string IP, int Port, int value)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
            if (uDP_Class != null)
            {
                return Communication.Set_OutputTrigger(uDP_Class, IP, value);
            }
            return false;
        }
        virtual public bool Set_LockOpen(RFIDClass rFIDClass, int Num)
        {
            return Set_LockOpen(rFIDClass.IP, rFIDClass.Port, Num);
        }
        virtual public bool Set_LockOpen(string IP, int Port, int Num)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
            return Set_LockOpen(uDP_Class, IP, Num);
        }
        virtual public bool Set_OutputPIN(string IP, int Port, int Num, bool value)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
            if (uDP_Class == null) return false;
            return Communication.Set_OutputPIN(uDP_Class, IP, Num, value);
        }
        virtual public bool Get_IO(string IP, int Port, out int intput, out int output)
        {
            intput = 0;
            output = 0;
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
            if (uDP_Class == null) return false;
            return Communication.Get_IO(uDP_Class, IP, out intput, out output);
        }

        public bool Get_IO_Input(string IP, int Port, int Num)
        {
            int intput = 0;
            int output = 0;
            bool flag = this.Get_IO(IP, Port, out intput, out output);
            if(flag)
            {
                return ((intput >> Num) % 2 == 1); 
            }
            return false;
        }
        public bool Get_IO_Output(string IP, int Port, int Num)
        {
            int intput = 0;
            int output = 0;
            bool flag = this.Get_IO(IP, Port, out intput, out output);
            if (flag)
            {
                return ((output >> Num) % 2 == 1);
            }
            return false;
        }
        public bool GetInput(string IP , int Num)
        {
            string jsonString = GetUDPJsonString(IP);
            if (jsonString.StringIsEmpty()) return false;
            UDP_READ uDP_READ = jsonString.JsonDeserializet<UDP_READ>();
            if (uDP_READ == null) return false;
            return uDP_READ.Get_Input(Num);
        }
        public bool GetOutput(string IP, int Num)
        {
            string jsonString = GetUDPJsonString(IP);
            if (jsonString.StringIsEmpty()) return false;
            UDP_READ uDP_READ = jsonString.JsonDeserializet<UDP_READ>();
            if (uDP_READ == null) return false;
            return uDP_READ.Get_Output(Num);
        }
        public string GetRFID(string IP , int Num)
        {
            string jsonString = GetUDPJsonString(IP);
            if (jsonString.StringIsEmpty()) return "";
            UDP_READ uDP_READ = jsonString.JsonDeserializet<UDP_READ>();
            if (uDP_READ == null) return "";
            return uDP_READ.Get_CardID(Num);
        }
        public List<RFID_UID_Class> GetRFID()
        {
            List<RFID_UID_Class> list_RFID_UID_Class = new List<RFID_UID_Class>();
            List<string> list_jsonstring = this.GetAllUDPJsonString();

            Parallel.ForEach(list_jsonstring, value =>
            {
                string jsonString = value;
                try
                {
                    UDP_READ uDP_READ = jsonString.JsonDeserializet<UDP_READ>();
                    if (uDP_READ != null)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            if (uDP_READ.Get_RFID_Enable(i))
                            {
                                string card_ID = uDP_READ.Get_CardID(i);
                                if (card_ID.StringIsEmpty() == false)
                                {
                                    if (card_ID.StringToInt32() != 0)
                                    {
                                        RFID_UID_Class rFID_UID_Class = new RFID_UID_Class(card_ID, uDP_READ.IP, i);
                                        list_RFID_UID_Class.LockAdd(rFID_UID_Class);
                                    }
                                }
                            }
                        }
                    }
                }
                catch
                {

                }
             
               

            });

            return list_RFID_UID_Class;
        }

        public bool Get_LEDSetting(string IP, int Port, ref int[] InputPIN, ref int[] OutputPIN)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
            if(uDP_Class != null) return Communication.Get_LEDSetting(uDP_Class, IP, ref InputPIN, ref OutputPIN);
            return false;
        }

        public string GetUDPJsonString(string IP)
        {
            string JsonString = "";
            List<object[]> list_value = this.Get_UDP_RX().DeepClone();
            list_value = (from value in list_value
                          where value[(int)UDP_Class.UDP_Rx.IP].ObjectToString() == IP
                          select value).ToList();
            if (list_value.Count > 0)
            {
                JsonString = list_value[0][(int)UDP_Class.UDP_Rx.Readline].ObjectToString();
            }
            return JsonString;
        }
        public List<object[]> GetAllSelectUDPRowsValues()
        {
            return this.sqL_DataGridView_UDP_DataReceive.Get_All_Select_RowsValues();
        }
        public List<string> GetAllSelectUDPJsonString()
        {
            List<string> list_JsonString = new List<string>();
            List<object[]> list_value = this.sqL_DataGridView_UDP_DataReceive.Get_All_Select_RowsValues();
            List<object[]> list_UDP_RX = this.Get_UDP_RX().DeepClone();
            List<object[]> list_UDP_RX_buf = new List<object[]>();
            for (int i = 0; i < list_value.Count; i++)
            {
                list_UDP_RX_buf = list_UDP_RX.GetRows((int)UDP_Class.UDP_Rx.IP, list_value[i][(int)UDP_Class.UDP_Rx.IP].ObjectToString());
                if (list_UDP_RX_buf.Count > 0)
                {
                    list_JsonString.Add(list_UDP_RX_buf[0][(int)UDP_Class.UDP_Rx.Readline].ObjectToString());
                }
            }



            return list_JsonString;
        }
        public List<string> GetAllUDPJsonString()
        {
            List<string> list_JsonString = new List<string>();
            List<object[]> list_value = this.Get_UDP_RX();
            for (int i = 0; i < list_value.Count; i++)
            {
                list_JsonString.Add(list_value[i][(int)UDP_Class.UDP_Rx.Readline].ObjectToString());
            }
            return list_JsonString;
        }
        public List<string> GetAllDeviceTableIP()
        {
            List<object[]> list_value = this.sqL_DataGridView_DeviceTable.GetAllRows();
            List<string> list_IP = new List<string>();
            for (int i = 0; i < list_value.Count; i++)
            {
                list_IP.Add(list_value[i][(int)enum_DeviceTable.IP].ObjectToString());
            }
            return list_IP;
        }

        public List<object[]> SQL_GetAllDeviceTableRows()
        {
            List<object[]> list_value = this.sqL_DataGridView_DeviceTable.SQL_GetAllRows(false);
            return list_value;
        }
        public List<string> SQL_GetAllDeviceTableValue()
        {
            List<object[]> list_value = this.SQL_GetAllDeviceTableRows();
            List<string> list_DeviceTableValue = new List<string>();

            for (int i = 0; i < list_value.Count; i++)
            {

            }
            return list_DeviceTableValue;
        }

        public string SQL_GetDrawerJsonString(string IP)
        {
            List<object[]> list_value = this.SQL_GetAllDeviceTableRows();
            List<object[]> list_value_buf = new List<object[]>();
            list_value_buf = list_value.GetRows((int)enum_DeviceTable.IP, IP);
            if (list_value_buf.Count == 0) return null;
            return list_value_buf[0][(int)enum_DeviceTable.Value].ObjectToString();
        }
        public List<RFIDClass> SQL_GetAllRFIDClass()
        {
            List<RFIDClass> rFIDClasses = new List<RFIDClass>();
            List<object[]> list_value = this.SQL_GetAllDeviceTableRows();

            Parallel.ForEach(list_value, value =>
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
        public List<DeviceBasic> SQL_GetAllDeviceBasic()
        {
            List<DeviceBasic> deviceBasics = new List<DeviceBasic>();
            List<RFIDClassBasic> rFIDClassBasics = new List<RFIDClassBasic>();
            List<object[]> list_value = this.SQL_GetAllDeviceTableRows();
            Parallel.ForEach(list_value, value =>
            {
                string jsonString = value[(int)enum_DeviceTable.Value].ObjectToString();
                RFIDClassBasic rFIDClassBasic = jsonString.JsonDeserializet<RFIDClassBasic>();
                for (int i = 0; i < rFIDClassBasic.RFIDDevices.Count; i++)
                {
                    deviceBasics.LockAdd(rFIDClassBasic.RFIDDevices[i]);
                }
            });

            deviceBasics = (from value in deviceBasics
                            where value != null
                            select value).ToList();
            return deviceBasics;

        }

        public RFIDClass SQL_GetRFIDClass(RFIDClass rFIDClass)
        {
            return SQL_GetRFIDClass(rFIDClass.IP);
        }
        public RFIDClass SQL_GetRFIDClass(string IP)
        {
            string jsonString = this.SQL_GetDrawerJsonString(IP);
            if (jsonString.StringIsEmpty()) return null;
            RFIDClass rFIDClass = jsonString.JsonDeserializet<RFIDClass>();
            if (DrawerChangeEvent != null)
            {
                List<RFIDClass> list_rFIDClass = new List<RFIDClass>();
                list_rFIDClass.Add(rFIDClass);
                DrawerChangeEvent(list_rFIDClass);
            }
            return rFIDClass;
        }
        public RFIDDevice SQL_GetDevice(RFIDDevice rFIDDevice)
        {
            RFIDClass rFIDClass = this.SQL_GetRFIDClass(rFIDDevice.IP);
            if (rFIDClass == null) return null;
            rFIDDevice = rFIDClass.SortByGUID(rFIDDevice.GUID);
            return rFIDDevice;
        }
        public bool SQL_ReplaceRFIDClass(RFIDClass rFIDClass)
        {
            string IP = rFIDClass.IP;
            List<object[]> list_value = this.sqL_DataGridView_DeviceTable.SQL_GetRows(enum_DeviceTable.IP.GetEnumName(), IP, false);
            if (list_value.Count == 0) return false;
            list_value[0][(int)enum_DeviceTable.Value] = rFIDClass.JsonSerializationt<RFIDClass>();
            this.sqL_DataGridView_DeviceTable.SQL_Replace(list_value[0], false);
            return true;
        }
        public bool SQL_ReplaceRFIDClass(List<RFIDClass> rFIDClasses)
        {
            List<object[]> list_value = this.sqL_DataGridView_DeviceTable.SQL_GetAllRows(false);
            List<object[]> list_value_buf = new List<object[]>();
            List<string> list_Replace_SerchValue = new List<string>();
            List<object[]> list_Replace_Value = new List<object[]>();
            if (list_value.Count == 0) return false;
            for (int i = 0; i < rFIDClasses.Count; i++)
            {
                list_value_buf = list_value.GetRows((int)enum_DeviceTable.IP, rFIDClasses[i].IP);
                if (list_value_buf.Count > 0)
                {
                    list_value_buf[0][(int)enum_DeviceTable.Value] = rFIDClasses[i].JsonSerializationt<RFIDClass>();
                    list_Replace_SerchValue.Add(list_value_buf[0][(int)enum_DeviceTable.GUID].ObjectToString());
                    list_Replace_Value.Add(list_value_buf[0]);
                }
            }
            this.sqL_DataGridView_DeviceTable.SQL_ReplaceExtra(list_Replace_Value, false);
            return true;
        }


        virtual protected void Export()
        {
            if (this.saveFileDialog_SaveExcel.ShowDialog() == DialogResult.OK)
            {
                DataTable dataTable = this.sqL_DataGridView_DeviceTable.GetDataTable().DeepClone();
                dataTable = dataTable.ReorderTable(new enum_DeviceTable_匯出());
                CSVHelper.SaveFile(dataTable, this.saveFileDialog_SaveExcel.FileName);
            }
        }
        virtual protected void Import()
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
                            value_load[(int)enum_DeviceTable.Value] = new RFIDClass(IP, Port).JsonSerializationt<RFIDClass>();
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
                        value_load[(int)enum_DeviceTable.Value] = new RFIDClass(IP, Port).JsonSerializationt<RFIDClass>();
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
        virtual protected void SQL_AddDevice(string IP, int Port)
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
                value[(int)enum_DeviceTable.Value] = new RFIDClass(IP, Port).JsonSerializationt<RFIDClass>();
                this.sqL_DataGridView_DeviceTable.SQL_AddRow(value, true);
            }
            else
            {
                string GUID = list_SQL_Value_buf[0][(int)enum_DeviceTable.GUID].ObjectToString();
                list_SQL_Value_buf[0][(int)enum_DeviceTable.Port] = Port;
                string jsonString = list_SQL_Value_buf[0][(int)enum_DeviceTable.Value].ObjectToString();
                if (jsonString.StringIsEmpty()) return;
                RFIDClass rFIDClass = jsonString.JsonDeserializet<RFIDClass>();
                if (rFIDClass == null) return;
                rFIDClass.Port = Port;
                list_SQL_Value_buf[0][(int)enum_DeviceTable.Value] = rFIDClass.JsonSerializationt();
                this.sqL_DataGridView_DeviceTable.SQL_Replace(list_SQL_Value_buf[0], true);
            }
        }
        virtual protected void SQL_ReplaceDevice(object[] value, string IP, int Port)
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
                RFIDClass rFIDClass = jsonString.JsonDeserializet<RFIDClass>();
                if (rFIDClass == null) return;
                rFIDClass.IP = IP;
                for(int i = 0; i < rFIDClass.DeviceClasses.Length; i++)
                {
                    rFIDClass.DeviceClasses[i].IP = IP;
                }
                value[(int)enum_DeviceTable.Value] = rFIDClass.JsonSerializationt();
                this.sqL_DataGridView_DeviceTable.SQL_Replace(value, true);
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
                RFIDClass rFIDClass = jsonString.JsonDeserializet<RFIDClass>();
                if (rFIDClass == null) return;
                rFIDClass.IP = IP;
                rFIDClass.Port = Port;
                value[(int)enum_DeviceTable.Value] = rFIDClass.JsonSerializationt();
                this.sqL_DataGridView_DeviceTable.SQL_AddRow(value, true);
            }
        }
        public List<UDP_Class> GetLoacalUDP_Class()
        {
            return this.List_UDP_Local;
        }
        protected List<object[]> Get_UDP_RX()
        {
            List<List<object[]>> list_list_UDP_Rx = new List<List<object[]>>();
            List<object[]> list_UDP_Rx = new List<object[]>();
            for (int i = 0; i < List_UDP_Server.Count; i++)
            {
                list_list_UDP_Rx.Add(List_UDP_Server[i].List_UDP_Rx);
            }
            for (int i = 0; i < list_list_UDP_Rx.Count; i++)
            {
                for (int k = 0; k < list_list_UDP_Rx[i].Count; k++)
                {
                    list_UDP_Rx.Add(list_list_UDP_Rx[i][k]);
                }
            }
            list_UDP_Rx.Sort(new ICP_UDP_Rx());
            return list_UDP_Rx;
        }

        private void sub_SqlDataRefrsh()
        {
            this.list_UDP_Rx = this.Get_UDP_RX();
            if (this.CanSelect)
            {               
                this.sqL_DataGridView_UDP_DataReceive.RefreshGrid(list_UDP_Rx);
            }
        }
        private void sub_PING()
        {
            List<string> list_IP = this.GetAllDeviceTableIP();
            List<object[]> list_disconnected_IP = new List<object[]>();
            for (int i = 0; i < list_IP.Count; i++)
            {
                if (!Basic.Net.Ping(list_IP[i], 3, 200))
                {
                    list_disconnected_IP.Add(new object[] { list_IP[i] });
                }
            }
            if (this.CanSelect)
            {
                this.sqL_DataGridView_PING.RefreshGrid(list_disconnected_IP);
            }
          
        }

        #region Event
        private void RJ_Button_Write_MouseDownEvent(MouseEventArgs mevent)
        {
            bool flag_OK = true;
            int localport = this.Local_Port.StringToInt32();
            int serverport = this.Server_Port.StringToInt32();
            int udp_sendtime = this.UDP_SendTime.StringToInt32();
            int RFID_Enable = this.RFID_Enable.StringToInt32();

            if (localport < 0)
            {
                MyMessageBox.ShowDialog("localport is invalid!");
                return;
            }
            if (serverport < 0)
            {
                MyMessageBox.ShowDialog("serverport is invalid!");
                return;
            }
            if (udp_sendtime < 0)
            {
                MyMessageBox.ShowDialog("udp_sendtime is invalid!");
                return;
            }
            if (RFID_Enable < 0)
            {
                MyMessageBox.ShowDialog("RFID_Enable is invalid!");
            }
            if (!Communication.UART_Command_Set_IP_Adress(mySerialPort, this.IP_Adress))
            {
                flag_OK = false;
            }
            if (!Communication.UART_Command_Set_Subnet(mySerialPort, this.Subnet))
            {
                flag_OK = false;
            }
            if (!Communication.UART_Command_Set_Gateway(mySerialPort, this.Gateway)) flag_OK = false;
            if (!Communication.UART_Command_Set_DNS(mySerialPort, this.DNS)) flag_OK = false;
            if (!Communication.UART_Command_Set_Server_IP_Adress(mySerialPort, this.Server_IP_Adress)) flag_OK = false;
            if (!Communication.UART_Command_Set_Local_Port(mySerialPort, localport)) flag_OK = false;
            if (!Communication.UART_Command_Set_Server_Port(mySerialPort, serverport)) flag_OK = false;
            if (!Communication.UART_Command_Set_SSID(mySerialPort, this.SSID)) flag_OK = false;
            if (!Communication.UART_Command_Set_Password(mySerialPort, this._Password)) flag_OK = false;
            if (!Communication.UART_Command_Set_UDP_SendTime(mySerialPort, udp_sendtime)) flag_OK = false;
            if (!Communication.UART_Command_Set_RFID_Enable(mySerialPort, RFID_Enable)) flag_OK = false;
            if (flag_OK)
            {
                MyMessageBox.ShowDialog("Write data sucessed!");
            }
            else
            {
                MyMessageBox.ShowDialog("Write data failed!");
            }
        }
        private void RJ_Button_Read_MouseDownEvent(MouseEventArgs mevent)
        {
            mySerialPort.PortName = this.textBox_COM.Text;
            string IP_Adress = "";
            string Subnet = "";
            string Gateway = "";
            string DNS = "";
            string Server_IP_Adress = "";
            string Local_Port = "";
            string Server_Port = "";
            string SSID = "";
            string Password = "";
            string Station = "";
            string UDP_SendTime = "";
            string RFID_Enable = "";
            if (Communication.UART_Command_Get_Setting(mySerialPort, out IP_Adress, out Subnet, out Gateway, out DNS, out Server_IP_Adress, out Local_Port, out Server_Port, out SSID, out Password, out Station, out UDP_SendTime, out RFID_Enable))
            {
                this.Invoke(new Action(delegate
                {
                    this.IP_Adress = IP_Adress;
                    this.Subnet = Subnet;
                    this.Gateway = Gateway;
                    this.DNS = DNS;
                    this.Server_IP_Adress = Server_IP_Adress;
                    this.Local_Port = Local_Port;
                    this.Server_Port = Server_Port;
                    this.SSID = SSID;
                    this._Password = Password;
                    this.Station = Station;
                    this.UDP_SendTime = UDP_SendTime;
                    this.RFID_Enable = RFID_Enable;
                }));
          
                MyMessageBox.ShowDialog("Receive data sucessed!");
            }
            else
            {
                MyMessageBox.ShowDialog("Receive data failed!");
            }
        }
        private void RJ_Button_Station_Write_MouseDownEvent(MouseEventArgs mevent)
        {
            int station = this.textBox_Station.Text.StringToInt32();
            if (station < 0)
            {
                MyMessageBox.ShowDialog("非法的字元!");
                return;
            }
            if (Communication.UART_Command_Set_Station(mySerialPort, station))
            {
                MyMessageBox.ShowDialog("站號修改成功!");
            }
            else
            {
                MyMessageBox.ShowDialog("站號修改失敗!");
            }
        }
        private void CheckBox_輸出01_Click(object sender, EventArgs e)
        {
            int station = this.textBox_Station.Text.StringToInt32();
            if (station < 0)
            {
                MyMessageBox.ShowDialog("非法的字元!");
                return;
            }
            Communication.UART_Command_RS485_SetOutputPIN(mySerialPort, station, 0, ((CheckBox)sender).Checked);
        }
        private void CheckBox_輸出02_Click(object sender, EventArgs e)
        {
            int station = this.textBox_Station.Text.StringToInt32();
            if (station < 0)
            {
                MyMessageBox.ShowDialog("非法的字元!");
                return;
            }
            Communication.UART_Command_RS485_SetOutputPIN(mySerialPort, station, 1, ((CheckBox)sender).Checked);
        }
        private void CheckBox_輸出03_Click(object sender, EventArgs e)
        {
            int station = this.textBox_Station.Text.StringToInt32();
            if (station < 0)
            {
                MyMessageBox.ShowDialog("非法的字元!");
                return;
            }
            Communication.UART_Command_RS485_SetOutputPIN(mySerialPort, station, 2, ((CheckBox)sender).Checked);
        }
        private void CheckBox_輸出04_Click(object sender, EventArgs e)
        {
            int station = this.textBox_Station.Text.StringToInt32();
            if (station < 0)
            {
                MyMessageBox.ShowDialog("非法的字元!");
                return;
            }
            Communication.UART_Command_RS485_SetOutputPIN(mySerialPort, station, 3, ((CheckBox)sender).Checked);
        }
        private void CheckBox_輸出05_Click(object sender, EventArgs e)
        {
            int station = this.textBox_Station.Text.StringToInt32();
            if (station < 0)
            {
                MyMessageBox.ShowDialog("非法的字元!");
                return;
            }
            Communication.UART_Command_RS485_SetOutputPIN(mySerialPort, station, 4, ((CheckBox)sender).Checked);
        }
        private void CheckBox_輸出06_Click(object sender, EventArgs e)
        {
            int station = this.textBox_Station.Text.StringToInt32();
            if (station < 0)
            {
                MyMessageBox.ShowDialog("非法的字元!");
                return;
            }
            Communication.UART_Command_RS485_SetOutputPIN(mySerialPort, station, 5, ((CheckBox)sender).Checked);
        }
        private void CheckBox_輸出07_Click(object sender, EventArgs e)
        {
            int station = this.textBox_Station.Text.StringToInt32();
            if (station < 0)
            {
                MyMessageBox.ShowDialog("非法的字元!");
                return;
            }
            Communication.UART_Command_RS485_SetOutputPIN(mySerialPort, station, 6, ((CheckBox)sender).Checked);
        }
        private void CheckBox_輸出08_Click(object sender, EventArgs e)
        {
            int station = this.textBox_Station.Text.StringToInt32();
            if (station < 0)
            {
                MyMessageBox.ShowDialog("非法的字元!");
                return;
            }
            Communication.UART_Command_RS485_SetOutputPIN(mySerialPort, station, 7, ((CheckBox)sender).Checked);
        }
        private void CheckBox_輸出09_Click(object sender, EventArgs e)
        {
            int station = this.textBox_Station.Text.StringToInt32();
            if (station < 0)
            {
                MyMessageBox.ShowDialog("非法的字元!");
                return;
            }
            Communication.UART_Command_RS485_SetOutputPIN(mySerialPort, station, 8, ((CheckBox)sender).Checked);
        }
        private void CheckBox_輸出10_Click(object sender, EventArgs e)
        {
            int station = this.textBox_Station.Text.StringToInt32();
            if (station < 0)
            {
                MyMessageBox.ShowDialog("非法的字元!");
                return;
            }
            Communication.UART_Command_RS485_SetOutputPIN(mySerialPort, station, 9, ((CheckBox)sender).Checked);
        }

        private void RJ_Button_輸出方向_寫入_MouseDownEvent(MouseEventArgs mevent)
        {
            int station = this.textBox_Station.Text.StringToInt32();
            if (station < 0)
            {
                MyMessageBox.ShowDialog("非法的字元!");
                return;
            }
            int output_dir = 0;
            if (checkBox_輸出方向_01.Checked) output_dir = output_dir.SetBit(0, true);
            if (checkBox_輸出方向_02.Checked) output_dir = output_dir.SetBit(1, true);
            if (checkBox_輸出方向_03.Checked) output_dir = output_dir.SetBit(2, true);
            if (checkBox_輸出方向_04.Checked) output_dir = output_dir.SetBit(3, true);
            if (checkBox_輸出方向_05.Checked) output_dir = output_dir.SetBit(4, true);
            if (checkBox_輸出方向_06.Checked) output_dir = output_dir.SetBit(5, true);
            if (checkBox_輸出方向_07.Checked) output_dir = output_dir.SetBit(6, true);
            if (checkBox_輸出方向_08.Checked) output_dir = output_dir.SetBit(7, true);
            if (checkBox_輸出方向_09.Checked) output_dir = output_dir.SetBit(8, true);
            if (checkBox_輸出方向_10.Checked) output_dir = output_dir.SetBit(9, true);

            if (!Communication.UART_Command_RS485_SetOutputDir(mySerialPort, station, output_dir))
            {
                MyMessageBox.ShowDialog("寫入失敗!");
                return;
            }
            MyMessageBox.ShowDialog("寫入成功!");
        }
        private void RJ_Button_輸出方向_讀取_MouseDownEvent(MouseEventArgs mevent)
        {
            int station = this.textBox_Station.Text.StringToInt32();
            if (station < 0)
            {
                MyMessageBox.ShowDialog("非法的字元!");
                return;
            }
            int output_dir = 0;
            if (!Communication.UART_Command_RS485_GetOutputDir(mySerialPort, station, ref output_dir))
            {
                MyMessageBox.ShowDialog("讀取失敗!");
                return;
            }
            this.Invoke(new Action(delegate 
            {
                checkBox_輸出方向_01.Checked = output_dir.GetBit(0);
                checkBox_輸出方向_02.Checked = output_dir.GetBit(1);
                checkBox_輸出方向_03.Checked = output_dir.GetBit(2);
                checkBox_輸出方向_04.Checked = output_dir.GetBit(3);
                checkBox_輸出方向_05.Checked = output_dir.GetBit(4);
                checkBox_輸出方向_06.Checked = output_dir.GetBit(5);
                checkBox_輸出方向_07.Checked = output_dir.GetBit(6);
                checkBox_輸出方向_08.Checked = output_dir.GetBit(7);
                checkBox_輸出方向_09.Checked = output_dir.GetBit(8);
                checkBox_輸出方向_10.Checked = output_dir.GetBit(9);
            }));
            MyMessageBox.ShowDialog("讀取成功!");
        }
        private void RJ_Button_輸入方向_寫入_MouseDownEvent(MouseEventArgs mevent)
        {
            int station = this.textBox_Station.Text.StringToInt32();
            if (station < 0)
            {
                MyMessageBox.ShowDialog("非法的字元!");
                return;
            }
            int input_dir = 0;
            if (checkBox_輸入方向_01.Checked) input_dir = input_dir.SetBit(0, true);
            if (checkBox_輸入方向_02.Checked) input_dir = input_dir.SetBit(1, true);
            if (checkBox_輸入方向_03.Checked) input_dir = input_dir.SetBit(2, true);
            if (checkBox_輸入方向_04.Checked) input_dir = input_dir.SetBit(3, true);
            if (checkBox_輸入方向_05.Checked) input_dir = input_dir.SetBit(4, true);
            if (checkBox_輸入方向_06.Checked) input_dir = input_dir.SetBit(5, true);
            if (checkBox_輸入方向_07.Checked) input_dir = input_dir.SetBit(6, true);
            if (checkBox_輸入方向_08.Checked) input_dir = input_dir.SetBit(7, true);
            if (checkBox_輸入方向_09.Checked) input_dir = input_dir.SetBit(8, true);
            if (checkBox_輸入方向_10.Checked) input_dir = input_dir.SetBit(9, true);

            if (!Communication.UART_Command_RS485_SetInputDir(mySerialPort, station, input_dir))
            {
                MyMessageBox.ShowDialog("寫入失敗!");
                return;
            }
            MyMessageBox.ShowDialog("寫入成功!");
        }
        private void RJ_Button_輸入方向_讀取_MouseDownEvent(MouseEventArgs mevent)
        {
            int station = this.textBox_Station.Text.StringToInt32();
            if (station < 0)
            {
                MyMessageBox.ShowDialog("非法的字元!");
                return;
            }
            int input_dir = 0;
            if (!Communication.UART_Command_RS485_GetInputDir(mySerialPort, station, ref input_dir))
            {
                MyMessageBox.ShowDialog("讀取失敗!");
                return;
            }
            this.Invoke(new Action(delegate
            {
                checkBox_輸入方向_01.Checked = input_dir.GetBit(0);
                checkBox_輸入方向_02.Checked = input_dir.GetBit(1);
                checkBox_輸入方向_03.Checked = input_dir.GetBit(2);
                checkBox_輸入方向_04.Checked = input_dir.GetBit(3);
                checkBox_輸入方向_05.Checked = input_dir.GetBit(4);
                checkBox_輸入方向_06.Checked = input_dir.GetBit(5);
                checkBox_輸入方向_07.Checked = input_dir.GetBit(6);
                checkBox_輸入方向_08.Checked = input_dir.GetBit(7);
                checkBox_輸入方向_09.Checked = input_dir.GetBit(8);
                checkBox_輸入方向_10.Checked = input_dir.GetBit(9);
            }));
            MyMessageBox.ShowDialog("讀取成功!");
        }
        private void RJ_Button_輸入_讀取_MouseDownEvent(MouseEventArgs mevent)
        {
            int station = this.textBox_Station.Text.StringToInt32();
            if (station < 0)
            {
                MyMessageBox.ShowDialog("非法的字元!");
                return;
            }
            int input = 0;
            int output = 0;
            if(!Communication.UART_Command_RS485_GetIO(mySerialPort, station, ref input, ref output))
            {
                MyMessageBox.ShowDialog("讀取失敗!");
                return;
            }
            this.Invoke(new Action(delegate 
            {
                checkBox_輸入01.Checked = input.GetBit(0);
                checkBox_輸入02.Checked = input.GetBit(1);
                checkBox_輸入03.Checked = input.GetBit(2);
                checkBox_輸入04.Checked = input.GetBit(3);
                checkBox_輸入05.Checked = input.GetBit(4);
                checkBox_輸入06.Checked = input.GetBit(5);
                checkBox_輸入07.Checked = input.GetBit(6);
                checkBox_輸入08.Checked = input.GetBit(7);
                checkBox_輸入09.Checked = input.GetBit(8);
                checkBox_輸入10.Checked = input.GetBit(9);

                checkBox_輸出01.Checked = output.GetBit(0);
                checkBox_輸出02.Checked = output.GetBit(1);
                checkBox_輸出03.Checked = output.GetBit(2);
                checkBox_輸出04.Checked = output.GetBit(3);
                checkBox_輸出05.Checked = output.GetBit(4);
                checkBox_輸出06.Checked = output.GetBit(5);
                checkBox_輸出07.Checked = output.GetBit(6);
                checkBox_輸出08.Checked = output.GetBit(7);
                checkBox_輸出09.Checked = output.GetBit(8);
                checkBox_輸出10.Checked = output.GetBit(9);
            }));
            MyMessageBox.ShowDialog("讀取成功!");
        }
        private void RJ_Button_輸出_寫入_MouseDownEvent(MouseEventArgs mevent)
        {
            int station = this.textBox_Station.Text.StringToInt32();
            if (station < 0)
            {
                MyMessageBox.ShowDialog("非法的字元!");
                return;
            }
            int output = 0;
            output = output.SetBit(0, checkBox_輸出01.Checked);
            output = output.SetBit(1, checkBox_輸出02.Checked);
            output = output.SetBit(2, checkBox_輸出03.Checked);
            output = output.SetBit(3, checkBox_輸出04.Checked);
            output = output.SetBit(4, checkBox_輸出05.Checked);
            output = output.SetBit(5, checkBox_輸出06.Checked);
            output = output.SetBit(6, checkBox_輸出07.Checked);
            output = output.SetBit(7, checkBox_輸出08.Checked);
            output = output.SetBit(8, checkBox_輸出09.Checked);
            output = output.SetBit(9, checkBox_輸出10.Checked);

            if(!Communication.UART_Command_RS485_SetOutput(mySerialPort, station, output))
            {
                MyMessageBox.ShowDialog("寫入失敗!");
                return;
            }
        }

        private void RFID_UI_FormClosing(object sender, FormClosingEventArgs e)
        {
            for (int i = 0; i < List_UDP_Server.Count; i++)
            {
                List_UDP_Server[i].Dispose();
            }
        }

        private void SqL_DataGridView_UDP_DataReceive_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                List<object[]> list_value = this.sqL_DataGridView_UDP_DataReceive.Get_All_Select_RowsValues();
                List<string> list_ContextMenuStrip = new List<string>();
                string[] contextMenuStrip_UDP_DataReceive = new ContextMenuStrip_UDP_DataReceive().GetEnumNames();
                string[] enum_ContextMenuStrip_UDP_DataReceive = Enum_ContextMenuStrip_UDP_DataReceive.GetEnumNames();
                for (int i = 0; i < contextMenuStrip_UDP_DataReceive.Length; i++)
                {
                    list_ContextMenuStrip.Add(contextMenuStrip_UDP_DataReceive[i]);
                }
                for (int i = 0; i < enum_ContextMenuStrip_UDP_DataReceive.Length; i++)
                {
                    list_ContextMenuStrip.Add(enum_ContextMenuStrip_UDP_DataReceive[i]);
                }

                Dialog_ContextMenuStrip dialog_ContextMenuStrip = new Dialog_ContextMenuStrip(list_ContextMenuStrip.ToArray());
                dialog_ContextMenuStrip.TitleText = "接收資料功能選單";
                if (dialog_ContextMenuStrip.ShowDialog() == DialogResult.Yes)
                {
                    if (dialog_ContextMenuStrip.Value == ContextMenuStrip_UDP_DataReceive.重新啟動.GetEnumName())
                    {
                        List<Task> taskList = new List<Task>();
                        for (int i = 0; i < list_value.Count; i++)
                        {
                            string IP = list_value[i][(int)enum_UDP_DataReceive.IP].ObjectToString();
                            int Port = list_value[i][(int)enum_UDP_DataReceive.Port].ObjectToString().StringToInt32();
                            taskList.Add(Task.Run(() =>
                            {
                                Set_ESP32_Restart(IP, Port);
                            }));
                        }
                        Task allTask = Task.WhenAll(taskList);
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_UDP_DataReceive.OTA線上更新.GetEnumName())
                    {
                        List<Task> taskList = new List<Task>();
                        if (MyMessageBox.ShowDialog("是否線上更新?", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) == DialogResult.Yes)
                        {
                            for (int i = 0; i < list_value.Count; i++)
                            {
                                string IP = list_value[i][(int)enum_UDP_DataReceive.IP].ObjectToString();
                                int Port = list_value[i][(int)enum_UDP_DataReceive.Port].ObjectToString().StringToInt32();
                                taskList.Add(Task.Run(() =>
                                {
                                    Set_OTAUpdate(IP, Port);
                                }));
                            }
                            Task allTask = Task.WhenAll(taskList);
                        }
                        else
                        {
                            SqL_DataGridView_UDP_DataReceive_MouseDown(sender, e);
                        }
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_UDP_DataReceive.設置發送時間.GetEnumName())
                    {

                        Dialog_NumPannel dialog_NumPannel = new Dialog_NumPannel();
                        if (dialog_NumPannel.ShowDialog() == DialogResult.Yes)
                        {
                            int num = dialog_NumPannel.Value;
                            List<Task> taskList = new List<Task>();
                            for (int i = 0; i < list_value.Count; i++)
                            {
                                string IP = list_value[i][(int)enum_UDP_DataReceive.IP].ObjectToString();
                                int Port = list_value[i][(int)enum_UDP_DataReceive.Port].ObjectToString().StringToInt32();
                                taskList.Add(Task.Run(() =>
                                {
                                    Set_UDP_SendTime(IP, Port, num);
                                }));
                            }
                            Task allTask = Task.WhenAll(taskList);
                        }
                        else
                        {
                            SqL_DataGridView_UDP_DataReceive_MouseDown(sender, e);
                        }

                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_UDP_DataReceive.設定ServerConfig.GetEnumName())
                    {
                        Dialog_IP_Config dialog_IP_Config = new Dialog_IP_Config();
                        if (dialog_IP_Config.ShowDialog() == DialogResult.Yes)
                        {
                            string setting_IP = dialog_IP_Config.IP;
                            int setting_Port = dialog_IP_Config.Port;
                            List<Task> taskList = new List<Task>();
                            for (int i = 0; i < list_value.Count; i++)
                            {
                                string IP = list_value[i][(int)enum_UDP_DataReceive.IP].ObjectToString();
                                int Port = list_value[i][(int)enum_UDP_DataReceive.Port].ObjectToString().StringToInt32();
                                taskList.Add(Task.Run(() =>
                                {
                                    Set_ServerConfig(IP, Port, setting_IP, setting_Port);
                                }));
                            }
                            Task allTask = Task.WhenAll(taskList);
                        }
                        else
                        {
                            SqL_DataGridView_UDP_DataReceive_MouseDown(sender, e);
                        }

                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_UDP_DataReceive.設定GatewayConfig.GetEnumName())
                    {
                        Dialog_IP_Config dialog_IP_Config = new Dialog_IP_Config();
                        dialog_IP_Config.PortVisibale = false;
                        if (dialog_IP_Config.ShowDialog() == DialogResult.Yes)
                        {
                            string setting_IP = dialog_IP_Config.IP;
                            List<Task> taskList = new List<Task>();
                            for (int i = 0; i < list_value.Count; i++)
                            {
                                string IP = list_value[i][(int)enum_UDP_DataReceive.IP].ObjectToString();
                                int Port = list_value[i][(int)enum_UDP_DataReceive.Port].ObjectToString().StringToInt32();
                                taskList.Add(Task.Run(() =>
                                {
                                    Set_GatewayConfig(IP, Port, setting_IP);
                                }));
                            }
                            Task allTask = Task.WhenAll(taskList);
                        }
                        else
                        {
                            SqL_DataGridView_UDP_DataReceive_MouseDown(sender, e);
                        }

                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_UDP_DataReceive.設定LocalPort.GetEnumName())
                    {

                        Dialog_NumPannel dialog_NumPannel = new Dialog_NumPannel();
                        if (dialog_NumPannel.ShowDialog() == DialogResult.Yes)
                        {
                            int num = dialog_NumPannel.Value;
                            List<Task> taskList = new List<Task>();
                            for (int i = 0; i < list_value.Count; i++)
                            {
                                string IP = list_value[i][(int)enum_UDP_DataReceive.IP].ObjectToString();
                                int Port = list_value[i][(int)enum_UDP_DataReceive.Port].ObjectToString().StringToInt32();
                                taskList.Add(Task.Run(() =>
                                {
                                    Set_LocalPort(IP, Port, num);
                                }));
                            }
                            Task allTask = Task.WhenAll(taskList);
                        }
                        else
                        {
                            SqL_DataGridView_UDP_DataReceive_MouseDown(sender, e);
                        }

                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_UDP_DataReceive.選取顯示網段.GetEnumName())
                    {
                        List<object[]> list_UDP_RX = Get_UDP_RX();
                        List<string> list_datasource = new List<string>();

                        for (int i = 0; i < list_UDP_RX.Count; i++)
                        {
                            bool flag_IsContinue = false;
                            string IP = list_UDP_RX[i][(int)UDP_Class.UDP_Rx.IP].ObjectToString();
                            string[] IP_0_Array = IP.Split('.');
                            IP = $"{IP_0_Array[0]}.{IP_0_Array[1]}.{IP_0_Array[2]}.XXX";
                            for (int k = 0; k < list_datasource.Count; k++)
                            {
                                if (list_datasource[k] == IP)
                                {
                                    flag_IsContinue = true;
                                    break;
                                }
                            }
                            if (flag_IsContinue) continue;
                            list_datasource.Add(IP);
                        }
                        list_datasource.Add("XXX.XXX.XXX.XXX");

                        Dialog_選取顯示網段 dialog_選取顯示網段 = new Dialog_選取顯示網段(list_datasource);
                        if (dialog_選取顯示網段.ShowDialog() == DialogResult.Yes)
                        {
                            this.IP_UDP_DataReceive_Mask = dialog_選取顯示網段.Value;
                        }
                        else
                        {
                            SqL_DataGridView_UDP_DataReceive_MouseDown(sender, e);
                        }
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_UDP_DataReceive.RFID測試頁面.GetEnumName())
                    {
                        if (list_value.Count == 0) return;
                        string IP = list_value[0][(int)enum_UDP_DataReceive.IP].ObjectToString();
                        int Port = list_value[0][(int)enum_UDP_DataReceive.Port].ObjectToString().StringToInt32();
                        UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
                        if (uDP_Class == null) return;
                        Dialog_RFID測試頁面 dialog_RFID測試頁面 = new Dialog_RFID測試頁面(uDP_Class , IP, list_UDP_Rx);
                        dialog_RFID測試頁面.ShowDialog();
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_UDP_DataReceive.設定LED參數.GetEnumName())
                    {
                        if (list_value.Count == 0) return;
                        string IP = list_value[0][(int)enum_UDP_DataReceive.IP].ObjectToString();
                        int Port = list_value[0][(int)enum_UDP_DataReceive.Port].ObjectToString().StringToInt32();
                        UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
                        if (uDP_Class == null) return;
                        int[] input_PIN = new int[5];
                        int[] output_PIN = new int[5];
                        if (Get_LEDSetting(IP, Port, ref input_PIN, ref output_PIN))
                        {
                            Dialog_LED_Setting dialog_LED_Setting = new Dialog_LED_Setting(uDP_Class, IP, input_PIN, output_PIN);
                            this.Invoke(new Action(delegate 
                            {
                                dialog_LED_Setting.ShowDialog();
                            }));
                         
                        }
                    }


                    for (int i = 0; i < enum_ContextMenuStrip_UDP_DataReceive.Length; i++)
                    {
                        if (enum_ContextMenuStrip_UDP_DataReceive[i] == dialog_ContextMenuStrip.Value)
                        {
                            List<IPEndPoint> list_IPEndPoint = new List<IPEndPoint>();
                            for (int k = 0; k < list_value.Count; k++)
                            {

                                IPAddress address = list_value[k][(int)enum_UDP_DataReceive.IP].ObjectToString().StrinToIPAddress();
                                int Port = list_value[k][(int)enum_UDP_DataReceive.Port].ObjectToString().StringToInt32();
                                if (address != null && Port > 0)
                                {
                                    IPEndPoint iPEndPoint = new IPEndPoint(address, Port);
                                    list_IPEndPoint.Add(iPEndPoint);
                                }

                            }
                            if (UDP_DataReceiveMouseDownRightEvent != null) UDP_DataReceiveMouseDownRightEvent(dialog_ContextMenuStrip.Value, list_IPEndPoint);
                        }
                    }

                }


            }
        }
        private void SqL_DataGridView_UDP_DataReceive_DataGridRowsChangeEvent(List<object[]> RowsList)
        {
            double CycleTime_start = 0;
            for (int i = 0; i < RowsList.Count; i++)
            {
                RowsList[i][(int)UDP_Class.UDP_Rx.編號] = (i + 1).ToString("000");

                double.TryParse(RowsList[i][(int)UDP_Class.UDP_Rx.StartTime].ObjectToString(), out CycleTime_start);
                if ((Time.GetTotalMilliseconds() - CycleTime_start) >= this.TimeOut)
                {
                    RowsList[i][(int)UDP_Class.UDP_Rx.State] = "NG";
                }
            }

        }
        private void SqL_DataGridView_UDP_DataReceive_DataGridRowsChangeRefEvent(ref List<object[]> RowsList)
        {
            List<object[]> RowsList_buf = new List<object[]>();
            string[] IP_UDP_DataReceive_Mask_Array = IP_UDP_DataReceive_Mask.Split('.');
            string[] IP_Array;
            for (int i = 0; i < RowsList.Count; i++)
            {
                bool flag_add = true;
                string IP = RowsList[i][(int)UDP_Class.UDP_Rx.IP].ObjectToString();
                IP_Array = IP.Split('.');
                for (int k = 0; k < 4; k++)
                {
                    if (IP_Array[k] != IP_UDP_DataReceive_Mask_Array[k] && IP_UDP_DataReceive_Mask_Array[k] != "XXX") flag_add = false;
                }
                if (flag_add)
                {
                    RowsList[i][(int)UDP_Class.UDP_Rx.編號] = (RowsList_buf.Count + 1).ToString("000");
                    RowsList_buf.Add(RowsList[i]);
                }
            }
            RowsList = RowsList_buf;
        }
        private void SqL_DataGridView_UDP_DataReceive_DataGridRefreshEvent()
        {
            string 狀態 = "";
            for (int i = 0; i < this.sqL_DataGridView_UDP_DataReceive.dataGridView.Rows.Count; i++)
            {
                狀態 = this.sqL_DataGridView_UDP_DataReceive.dataGridView.Rows[i].Cells[(int)UDP_Class.UDP_Rx.State].Value.ToString();
                if (狀態 == "OK")
                {
                    this.sqL_DataGridView_UDP_DataReceive.dataGridView.Rows[i].DefaultCellStyle.BackColor = Color.Lime;
                    this.sqL_DataGridView_UDP_DataReceive.dataGridView.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
                }
                else if (狀態 == "NG")
                {
                    this.sqL_DataGridView_UDP_DataReceive.dataGridView.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                    this.sqL_DataGridView_UDP_DataReceive.dataGridView.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
                }
            }
        }
        private void SqL_DataGridView_DeviceTable_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                List<object[]> list_value = this.sqL_DataGridView_DeviceTable.Get_All_Select_RowsValues();
                List<string> list_ContextMenuStrip = new List<string>();
                string[] contextMenuStrip_DeviceTable = new ContextMenuStrip_DeviceTable().GetEnumNames();
                string[] enum_ContextMenuStrip_DeviceTable = Enum_ContextMenuStrip_DeviceTable.GetEnumNames();
                for (int i = 0; i < contextMenuStrip_DeviceTable.Length; i++)
                {
                    list_ContextMenuStrip.Add(contextMenuStrip_DeviceTable[i]);
                }
                for (int i = 0; i < enum_ContextMenuStrip_DeviceTable.Length; i++)
                {
                    list_ContextMenuStrip.Add(enum_ContextMenuStrip_DeviceTable[i]);
                }

                Dialog_ContextMenuStrip dialog_ContextMenuStrip = new Dialog_ContextMenuStrip(list_ContextMenuStrip.ToArray());
                dialog_ContextMenuStrip.TitleText = "儲位列表功能選單";
                if (dialog_ContextMenuStrip.ShowDialog() == DialogResult.Yes)
                {
                    if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable.重新啟動.GetEnumName())
                    {
                        List<Task> taskList = new List<Task>();
                        for (int i = 0; i < list_value.Count; i++)
                        {
                            string IP = list_value[i][(int)enum_DeviceTable.IP].ObjectToString();
                            int Port = list_value[i][(int)enum_DeviceTable.Port].ObjectToString().StringToInt32();
                            taskList.Add(Task.Run(() =>
                            {
                                Set_ESP32_Restart(IP, Port);
                            }));
                        }
                        Task allTask = Task.WhenAll(taskList);
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable.OTA線上更新.GetEnumName())
                    {
                        List<Task> taskList = new List<Task>();
                        if (MyMessageBox.ShowDialog("是否線上更新?", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) == DialogResult.Yes)
                        {
                            for (int i = 0; i < list_value.Count; i++)
                            {
                                string IP = list_value[i][(int)enum_DeviceTable.IP].ObjectToString();
                                int Port = list_value[i][(int)enum_DeviceTable.Port].ObjectToString().StringToInt32();
                                taskList.Add(Task.Run(() =>
                                {
                                    Set_OTAUpdate(IP, Port);
                                }));
                            }
                            Task allTask = Task.WhenAll(taskList);
                        }
                        else
                        {
                            SqL_DataGridView_DeviceTable_MouseDown(sender, e);
                        }
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable.設置發送時間.GetEnumName())
                    {

                        Dialog_NumPannel dialog_NumPannel = new Dialog_NumPannel();
                        if (dialog_NumPannel.ShowDialog() == DialogResult.Yes)
                        {
                            int num = dialog_NumPannel.Value;
                            List<Task> taskList = new List<Task>();
                            for (int i = 0; i < list_value.Count; i++)
                            {
                                string IP = list_value[i][(int)enum_DeviceTable.IP].ObjectToString();
                                int Port = list_value[i][(int)enum_DeviceTable.Port].ObjectToString().StringToInt32();
                                taskList.Add(Task.Run(() =>
                                {
                                    Set_UDP_SendTime(IP, Port, num);
                                }));
                            }
                            Task allTask = Task.WhenAll(taskList);
                        }
                        else
                        {
                            SqL_DataGridView_DeviceTable_MouseDown(sender, e);
                        }

                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable.設定ServerConfig.GetEnumName())
                    {
                        Dialog_IP_Config dialog_IP_Config = new Dialog_IP_Config();
                        if (dialog_IP_Config.ShowDialog() == DialogResult.Yes)
                        {
                            string setting_IP = dialog_IP_Config.IP;
                            int setting_Port = dialog_IP_Config.Port;
                            List<Task> taskList = new List<Task>();
                            for (int i = 0; i < list_value.Count; i++)
                            {
                                string IP = list_value[i][(int)enum_DeviceTable.IP].ObjectToString();
                                int Port = list_value[i][(int)enum_DeviceTable.Port].ObjectToString().StringToInt32();
                                taskList.Add(Task.Run(() =>
                                {
                                    Set_ServerConfig(IP, Port, setting_IP, setting_Port);
                                }));
                            }
                            Task allTask = Task.WhenAll(taskList);
                        }
                        else
                        {
                            SqL_DataGridView_DeviceTable_MouseDown(sender, e);
                        }

                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable.設定GatewayConfig.GetEnumName())
                    {
                        Dialog_IP_Config dialog_IP_Config = new Dialog_IP_Config();
                        dialog_IP_Config.PortVisibale = false;
                        if (dialog_IP_Config.ShowDialog() == DialogResult.Yes)
                        {
                            string setting_IP = dialog_IP_Config.IP;
                            List<Task> taskList = new List<Task>();
                            for (int i = 0; i < list_value.Count; i++)
                            {
                                string IP = list_value[i][(int)enum_DeviceTable.IP].ObjectToString();
                                int Port = list_value[i][(int)enum_DeviceTable.Port].ObjectToString().StringToInt32();
                                taskList.Add(Task.Run(() =>
                                {
                                    Set_GatewayConfig(IP, Port, setting_IP);
                                }));
                            }
                            Task allTask = Task.WhenAll(taskList);
                        }
                        else
                        {
                            SqL_DataGridView_UDP_DataReceive_MouseDown(sender, e);
                        }

                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable.設定LocalPort.GetEnumName())
                    {

                        Dialog_NumPannel dialog_NumPannel = new Dialog_NumPannel();
                        if (dialog_NumPannel.ShowDialog() == DialogResult.Yes)
                        {
                            int num = dialog_NumPannel.Value;
                            List<Task> taskList = new List<Task>();
                            for (int i = 0; i < list_value.Count; i++)
                            {
                                string IP = list_value[i][(int)enum_DeviceTable.IP].ObjectToString();
                                int Port = list_value[i][(int)enum_DeviceTable.Port].ObjectToString().StringToInt32();
                                taskList.Add(Task.Run(() =>
                                {
                                    Set_LocalPort(IP, Port, num);
                                }));
                            }
                            Task allTask = Task.WhenAll(taskList);
                        }
                        else
                        {
                            SqL_DataGridView_DeviceTable_MouseDown(sender, e);
                        }

                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable.匯出.GetEnumName())
                    {
                        this.Export();
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable.匯入.GetEnumName())
                    {
                        this.Import();
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable.刪除選取資料.GetEnumName())
                    {
                        if (MyMessageBox.ShowDialog("確認刪除選取資料?", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) == DialogResult.Yes)
                        {
                            for (int i = 0; i < list_value.Count; i++)
                            {
                                this.sqL_DataGridView_DeviceTable.SQL_Delete(enum_DeviceTable.GUID.GetEnumName(), list_value[i][(int)enum_DeviceTable.GUID].ObjectToString(), false);
                            }
                            this.sqL_DataGridView_DeviceTable.SQL_GetAllRows(true);
                        }
                        else
                        {
                            this.SqL_DataGridView_DeviceTable_MouseDown(sender, e);
                        }
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable.新增儲位.GetEnumName())
                    {
                        Dialog_IP_Config dialog_IP_Config = new Dialog_IP_Config();
                        if (dialog_IP_Config.ShowDialog() == DialogResult.Yes)
                        {
                            this.SQL_AddDevice(dialog_IP_Config.IP, dialog_IP_Config.Port);
                        }
                        else
                        {
                            SqL_DataGridView_DeviceTable_MouseDown(sender, e);
                        }
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable.修改儲位.GetEnumName())
                    {
                        Dialog_IP_Config dialog_IP_Config = new Dialog_IP_Config();
                        if (list_value.Count == 0) return;
                        if (dialog_IP_Config.ShowDialog() == DialogResult.Yes)
                        {
                            this.SQL_ReplaceDevice(list_value[0], dialog_IP_Config.IP, dialog_IP_Config.Port);

                        }
                        else
                        {
                            SqL_DataGridView_DeviceTable_MouseDown(sender, e);
                        }
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable.選取顯示網段.GetEnumName())
                    {
                        List<object[]> list_UDP_RX = list_value;
                        List<string> list_datasource = new List<string>();

                        for (int i = 0; i < list_UDP_RX.Count; i++)
                        {
                            bool flag_IsContinue = false;
                            string IP = list_UDP_RX[i][(int)enum_DeviceTable.IP].ObjectToString();
                            string[] IP_0_Array = IP.Split('.');
                            IP = $"{IP_0_Array[0]}.{IP_0_Array[1]}.{IP_0_Array[2]}.XXX";
                            for (int k = 0; k < list_datasource.Count; k++)
                            {
                                if (list_datasource[k] == IP)
                                {
                                    flag_IsContinue = true;
                                    break;
                                }
                            }
                            if (flag_IsContinue) continue;
                            list_datasource.Add(IP);
                        }
                        list_datasource.Add("XXX.XXX.XXX.XXX");

                        Dialog_選取顯示網段 dialog_選取顯示網段 = new Dialog_選取顯示網段(list_datasource);
                        if (dialog_選取顯示網段.ShowDialog() == DialogResult.Yes)
                        {
                            this.IP_DeviceTable_Mask = dialog_選取顯示網段.Value;

                            MyTimer myTimer = new MyTimer();
                            myTimer.StartTickTime(50000);
                            List<object[]> list_value_buf = this.sqL_DataGridView_DeviceTable.SQL_GetAllRows(false);
                            if (ConsoleWrite) Console.WriteLine($"Get data form SQL Time : {myTimer.GetTickTime().ToString("0.000")} ms");
                            this.sqL_DataGridView_DeviceTable.RefreshGrid(list_value_buf);
                        }
                        else
                        {
                            SqL_DataGridView_UDP_DataReceive_MouseDown(sender, e);
                        }
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable.儲位設定.GetEnumName())
                    {
                        if (list_value.Count == 0) return;
                        string jsonString = list_value[0][(int)enum_DeviceTable.Value].ObjectToString();
                        RFIDClass rFIDClass = jsonString.JsonDeserializet<RFIDClass>();
                        if (rFIDClass == null) return;
                        rFIDClass = this.SQL_GetRFIDClass(rFIDClass);
                        Dialog_RFID_DeviceSetting dialog_RFID_DeviceSetting = new Dialog_RFID_DeviceSetting(rFIDClass);
                        dialog_RFID_DeviceSetting.ShowDialog();
                        this.SQL_ReplaceRFIDClass(dialog_RFID_DeviceSetting.rFIDClass);
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable.刷新.GetEnumName())
                    {
                        MyTimer myTimer = new MyTimer();
                        myTimer.StartTickTime(50000);
                        List<object[]> list_value_buf = this.sqL_DataGridView_DeviceTable.SQL_GetAllRows(false);
                        if (ConsoleWrite) Console.WriteLine($"Get data form SQL Time : {myTimer.GetTickTime().ToString("0.000")} ms");
                        this.sqL_DataGridView_DeviceTable.RefreshGrid(list_value_buf);
                    }

                    for (int i = 0; i < enum_ContextMenuStrip_DeviceTable.Length; i++)
                    {
                        if (enum_ContextMenuStrip_DeviceTable[i] == dialog_ContextMenuStrip.Value)
                        {
                            List<IPEndPoint> list_IPEndPoint = new List<IPEndPoint>();
                            for (int k = 0; k < list_value.Count; k++)
                            {

                                IPAddress address = list_value[k][(int)enum_DeviceTable.IP].ObjectToString().StrinToIPAddress();
                                int Port = list_value[k][(int)enum_DeviceTable.Port].ObjectToString().StringToInt32();
                                if (address != null && Port > 0)
                                {
                                    IPEndPoint iPEndPoint = new IPEndPoint(address, Port);
                                    list_IPEndPoint.Add(iPEndPoint);
                                }
                            }
                            if (DeviceTableMouseDownRightEvent != null) DeviceTableMouseDownRightEvent(dialog_ContextMenuStrip.Value, list_IPEndPoint);
                        }
                    }
                }
            }
        }
        private void SqL_DataGridView_DeviceTable_DataGridRowsChangeEvent(List<object[]> RowsList)
        {
            RowsList.Sort(new ICP_DeviceTable());
        }
        private void SqL_DataGridView_DeviceTable_DataGridRowsChangeRefEvent(ref List<object[]> RowsList)
        {
            List<object[]> RowsList_buf = new List<object[]>();
            string[] IP_DeviceTable_Mask_Array = IP_DeviceTable_Mask.Split('.');
            string[] IP_Array;
            for (int i = 0; i < RowsList.Count; i++)
            {
                bool flag_add = true;
                string IP = RowsList[i][(int)enum_DeviceTable.IP].ObjectToString();
                IP_Array = IP.Split('.');
                for (int k = 0; k < 4; k++)
                {
                    if (IP_Array[k] != IP_DeviceTable_Mask_Array[k] && IP_DeviceTable_Mask_Array[k] != "XXX") flag_add = false;
                }
                if (flag_add) RowsList_buf.Add(RowsList[i]);
            }
            RowsList = RowsList_buf;
        }
        #endregion

        private class ICP_UDP_Rx : IComparer<object[]>
        {
            public int Compare(object[] x, object[] y)
            {
                string IP_0 = x[(int)UDP_Class.UDP_Rx.IP].ObjectToString();
                string IP_1 = y[(int)UDP_Class.UDP_Rx.IP].ObjectToString();
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
        private class ICP_DeviceTable : IComparer<object[]>
        {
            public int Compare(object[] x, object[] y)
            {
                string IP_0 = x[(int)enum_DeviceTable.IP].ObjectToString();
                string IP_1 = y[(int)enum_DeviceTable.IP].ObjectToString();
                string port0 = x[(int)enum_DeviceTable.Port].ObjectToString();
                string port1 = y[(int)enum_DeviceTable.Port].ObjectToString();
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
                        if (port0.StringToInt32() > port1.StringToInt32()) return 1;
                        else if (port0.StringToInt32() < port1.StringToInt32()) return -1;
                        else
                        {
                            return 0;
                        }
                    }
                }

                return 0;

            }
        }
    }
}
