using Basic;
using MyUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS_DB_Lib;
using SQLUI;
namespace H_Pannel_lib
{
    public enum enum_UDP_DataReceive
    {
        [Description("GUID,VARCHAR,50,PRIMARY")]
        GUID,
        [Description("編號,VARCHAR,10,NONE")]
        編號,
        [Description("IP,VARCHAR,50,NONE")]
        IP,
        [Description("Port,VARCHAR,10,NONE")]
        Port,
        [Description("Readline,VARCHAR,500,NONE")]
        Readline,
        [Description("StartTime,DATETIME,20,INDEX")]
        StartTime,
        [Description("Time,DATETIME,20,INDEX")]
        Time,
        [Description("State,VARCHAR,10,NONE")]
        State,
    }
    public enum enum_DeviceTable
    {
        [Description("GUID,VARCHAR,50,PRIMARY")]
        GUID,
        [Description("IP,VARCHAR,50,NONE")]
        IP,
        [Description("Port,VARCHAR,50,NONE")]
        Port,
        [Description("Value,MEDIUMTEXT,50,NONE")]
        Value,
    }
    public enum enum_IP_PING
    {
        [Description("IP,VARCHAR,50,NONE")]
        IP,
    }
    public partial class DeviceBasicUI : UserControl
    {
        static public int TimeOut = 5000;


        public bool ConsoleWrite = true;
        public List<object[]> list_UDP_Rx = new List<object[]>();
        public object Enum_ContextMenuStrip_UDP_DataReceive;
        public object Enum_ContextMenuStrip_DeviceTable;
        public delegate void UDP_DataReceiveMouseDownRightEventHandler(string selectedText, List<IPEndPoint> iPEndPoints);
        public event UDP_DataReceiveMouseDownRightEventHandler UDP_DataReceiveMouseDownRightEvent;
        public delegate void DeviceTableMouseDownRightEventHandler(string selectedText, List<IPEndPoint> iPEndPoints);
        public event DeviceTableMouseDownRightEventHandler DeviceTableMouseDownRightEvent;
        public List<UDP_Class> List_UDP_Server = new List<UDP_Class>();
        public List<UDP_Class> List_UDP_Local = new List<UDP_Class>();

        private bool flag_UDP_Class_Init = false;
        private string dataBaseName = "TEST";
        private string userName = "root";
        private string password = "user82822040";
        private string iP = "localhost";
        private uint port = 3306;
        private MySql.Data.MySqlClient.MySqlSslMode mySqlSslMode = MySql.Data.MySqlClient.MySqlSslMode.None;
        private string tableName = "Device_Jsonstring";
        private MyThread MyThread_PING;
        private string IP_UDP_DataReceive_Mask = "XXX.XXX.XXX.XXX";
        private string IP_DeviceTable_Mask = "XXX.XXX.XXX.XXX";

        public int Panel_Width
        {
            get
            {
                return 480;
            }
        }
        public int Panel_Height
        {
            get
            {
                return 320;
            }
        }
        public bool ComIsOpen
        {
            get
            {
                return this.mySerialPort.IsConnected;
            }
        }

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
        public enum enum_UDP_DataReceive_匯出
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
            匯出,
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
            刷新,
         
        }


        private Stopwatch stopwatch = new Stopwatch();
        private MyThread MyThread_SqlDataRefrsh;
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
        public String UDP_SendTime
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
        public MySerialPort mySerialPort = new MySerialPort();

        public DeviceBasicUI()
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
        public void Init(string DataBaseName, string UserName, string Password, string IP, uint Port, MySql.Data.MySqlClient.MySqlSslMode SSLMode, int serverPort, int localPort)
        {
            this.DataBaseName = DataBaseName;
            this.UserName = UserName;
            this.Password = Password;
            this.IP = IP;
            this.Port = Port;
            this.mySqlSslMode = SSLMode;

            Communication.ConsoleWrite = ConsoleWrite;
            this.stopwatch.Start();
            SQLUI.SQL_DataGridView.SQL_Set_Properties(this.sqL_DataGridView_DeviceTable, this.TableName, DataBaseName, UserName, Password, IP, Port, mySqlSslMode);

            SQLUI.Table table_UDP_DataReceive = new SQLUI.Table(new enum_UDP_DataReceive());
            SQLUI.Table table_DeviceTable = new SQLUI.Table(new enum_DeviceTable());

            this.sqL_DataGridView_UDP_DataReceive.Init(table_UDP_DataReceive, this.TableName);
            this.sqL_DataGridView_DeviceTable.Init(table_DeviceTable, this.TableName);


            if (flag_UDP_Class_Init == false)
            {
                flag_UDP_Class_Init = true;
                this.Invoke(new Action(delegate
                {
                    this.sqL_DataGridView_UDP_DataReceive.DataGridRowsChangeEvent += SqL_DataGridView_UDP_DataReceive_DataGridRowsChangeEvent;
                    this.sqL_DataGridView_UDP_DataReceive.DataGridRefreshEvent += SqL_DataGridView_UDP_DataReceive_DataGridRefreshEvent;
                    this.sqL_DataGridView_UDP_DataReceive.DataGridRowsChangeRefEvent += SqL_DataGridView_UDP_DataReceive_DataGridRowsChangeRefEvent;
                    this.sqL_DataGridView_UDP_DataReceive.MouseDown += SqL_DataGridView_UDP_DataReceive_MouseDown;
                    this.sqL_DataGridView_UDP_DataReceive.Set_ColumnVisible(false, new enum_UDP_DataReceive().GetEnumNames());
                    this.sqL_DataGridView_UDP_DataReceive.Set_ColumnWidth(60, enum_UDP_DataReceive.編號);
                    this.sqL_DataGridView_UDP_DataReceive.Set_ColumnWidth(150, enum_UDP_DataReceive.IP);
                    this.sqL_DataGridView_UDP_DataReceive.Set_ColumnWidth(60, enum_UDP_DataReceive.Port);
                    this.sqL_DataGridView_UDP_DataReceive.Set_ColumnWidth(400, enum_UDP_DataReceive.Readline);
                    this.sqL_DataGridView_UDP_DataReceive.Set_ColumnWidth(100, enum_UDP_DataReceive.Time);

                    if (!this.sqL_DataGridView_DeviceTable.SQL_IsTableCreat()) this.sqL_DataGridView_DeviceTable.SQL_CreateTable();
                    this.sqL_DataGridView_DeviceTable.DataGridRowsChangeEvent += SqL_DataGridView_DeviceTable_DataGridRowsChangeEvent;
                    this.sqL_DataGridView_DeviceTable.DataGridRowsChangeRefEvent += SqL_DataGridView_DeviceTable_DataGridRowsChangeRefEvent;
                    this.sqL_DataGridView_DeviceTable.MouseDown += SqL_DataGridView_DeviceTable_MouseDown;
                    this.sqL_DataGridView_DeviceTable.Set_ColumnVisible(false, new enum_DeviceTable().GetEnumNames());
                    this.sqL_DataGridView_DeviceTable.Set_ColumnWidth(100, enum_DeviceTable.GUID);
                    this.sqL_DataGridView_DeviceTable.Set_ColumnWidth(150, enum_DeviceTable.IP);
                    this.sqL_DataGridView_DeviceTable.Set_ColumnWidth(60, enum_DeviceTable.Port);
                    this.sqL_DataGridView_DeviceTable.Set_ColumnWidth(600, enum_DeviceTable.Value);
                    this.sqL_DataGridView_DeviceTable.SQL_GetAllRows(true);
                }));




                this.UDP_Class_Init(serverPort, localPort);
                this.Init_Offline();

                this.MyThread_SqlDataRefrsh = new MyThread();
                this.MyThread_SqlDataRefrsh.AutoRun(true);
                this.MyThread_SqlDataRefrsh.Add_Method(this.sub_SqlDataRefrsh);
                this.MyThread_SqlDataRefrsh.SetSleepTime(50);
                this.MyThread_SqlDataRefrsh.Trigger();

                SQLUI.Table table_IP_PING = new SQLUI.Table(new enum_IP_PING());

                this.sqL_DataGridView_PING.Init(table_IP_PING);
                this.MyThread_PING = new MyThread();
                this.MyThread_PING = new MyThread();
                this.MyThread_PING.AutoRun(true);
                this.MyThread_PING.Add_Method(this.sub_PING);
                this.MyThread_PING.SetSleepTime(100);

            }

        }
        public void Console_Init(string DataBaseName, string UserName, string Password, string IP, uint Port, MySql.Data.MySqlClient.MySqlSslMode SSLMode ,int serverPort , int localPort)
        {
            this.DataBaseName = DataBaseName;
            this.UserName = UserName;
            this.Password = Password;
            this.IP = IP;
            this.Port = Port;
            this.mySqlSslMode = SSLMode;

            SQLUI.SQL_DataGridView.SQL_Set_Properties(this.sqL_DataGridView_DeviceTable, this.TableName, DataBaseName, UserName, Password, IP, Port, mySqlSslMode);

            SQLUI.Table table_UDP_DataReceive = new SQLUI.Table(new enum_UDP_DataReceive());
            SQLUI.Table table_DeviceTable = new SQLUI.Table(new enum_DeviceTable());

            this.sqL_DataGridView_UDP_DataReceive.Init(table_UDP_DataReceive, this.TableName);
            this.sqL_DataGridView_DeviceTable.Init(table_DeviceTable, this.TableName);
            this.UDP_Class_Init(serverPort , localPort);
        }
        virtual public void Init()
        {
            Communication.ConsoleWrite = ConsoleWrite;
            this.stopwatch.Start();
            SQLUI.SQL_DataGridView.SQL_Set_Properties(this.sqL_DataGridView_DeviceTable, this.TableName, DataBaseName, UserName, Password, IP, Port, mySqlSslMode);

            SQLUI.Table table_UDP_DataReceive = new SQLUI.Table(new enum_UDP_DataReceive());
            SQLUI.Table table_DeviceTable = new SQLUI.Table(new enum_DeviceTable());

            this.sqL_DataGridView_UDP_DataReceive.Init(table_UDP_DataReceive, this.TableName);
            this.sqL_DataGridView_DeviceTable.Init(table_DeviceTable, this.TableName);


            if (flag_UDP_Class_Init == false)
            {
                flag_UDP_Class_Init = true;
                this.Invoke(new Action(delegate
                {
                    this.sqL_DataGridView_UDP_DataReceive.DataGridRowsChangeEvent += SqL_DataGridView_UDP_DataReceive_DataGridRowsChangeEvent;
                    this.sqL_DataGridView_UDP_DataReceive.DataGridRefreshEvent += SqL_DataGridView_UDP_DataReceive_DataGridRefreshEvent;
                    this.sqL_DataGridView_UDP_DataReceive.DataGridRowsChangeRefEvent += SqL_DataGridView_UDP_DataReceive_DataGridRowsChangeRefEvent;
                    this.sqL_DataGridView_UDP_DataReceive.MouseDown += SqL_DataGridView_UDP_DataReceive_MouseDown;
                    this.sqL_DataGridView_UDP_DataReceive.Set_ColumnVisible(false, new enum_UDP_DataReceive().GetEnumNames());
                    this.sqL_DataGridView_UDP_DataReceive.Set_ColumnWidth(60, enum_UDP_DataReceive.編號);
                    this.sqL_DataGridView_UDP_DataReceive.Set_ColumnWidth(150, enum_UDP_DataReceive.IP);
                    this.sqL_DataGridView_UDP_DataReceive.Set_ColumnWidth(60, enum_UDP_DataReceive.Port);
                    this.sqL_DataGridView_UDP_DataReceive.Set_ColumnWidth(400, enum_UDP_DataReceive.Readline);
                    this.sqL_DataGridView_UDP_DataReceive.Set_ColumnWidth(100, enum_UDP_DataReceive.Time);

                    if (!this.sqL_DataGridView_DeviceTable.SQL_IsTableCreat()) this.sqL_DataGridView_DeviceTable.SQL_CreateTable();
                    this.sqL_DataGridView_DeviceTable.DataGridRowsChangeEvent += SqL_DataGridView_DeviceTable_DataGridRowsChangeEvent;
                    this.sqL_DataGridView_DeviceTable.DataGridRowsChangeRefEvent += SqL_DataGridView_DeviceTable_DataGridRowsChangeRefEvent;
                    this.sqL_DataGridView_DeviceTable.MouseDown += SqL_DataGridView_DeviceTable_MouseDown;
                    this.sqL_DataGridView_DeviceTable.Set_ColumnVisible(false, new enum_DeviceTable().GetEnumNames());
                    this.sqL_DataGridView_DeviceTable.Set_ColumnWidth(100, enum_DeviceTable.GUID);
                    this.sqL_DataGridView_DeviceTable.Set_ColumnWidth(150, enum_DeviceTable.IP);
                    this.sqL_DataGridView_DeviceTable.Set_ColumnWidth(60, enum_DeviceTable.Port);
                    this.sqL_DataGridView_DeviceTable.Set_ColumnWidth(600, enum_DeviceTable.Value);
                    this.sqL_DataGridView_DeviceTable.SQL_GetAllRows(true);
                }));

            


                this.UDP_Class_Init();
                this.Init_Offline();

                this.MyThread_SqlDataRefrsh = new MyThread();
                this.MyThread_SqlDataRefrsh.AutoRun(true);
                this.MyThread_SqlDataRefrsh.Add_Method(this.sub_SqlDataRefrsh);
                this.MyThread_SqlDataRefrsh.SetSleepTime(50);
                this.MyThread_SqlDataRefrsh.Trigger();

                SQLUI.Table table_IP_PING = new SQLUI.Table(new enum_IP_PING());

                this.sqL_DataGridView_PING.Init(table_IP_PING);
                this.MyThread_PING = new MyThread();
                this.MyThread_PING = new MyThread();
                this.MyThread_PING.AutoRun(true);
                this.MyThread_PING.Add_Method(this.sub_PING);
                this.MyThread_PING.SetSleepTime(100);

            }
  

        }
        public void UDP_Class_Init(int serverPort, int localPort)
        {
            if(serverPort > 0)
            {
                UDP_Class uDP_Class_serverPort = new UDP_Class("0.0.0.0", serverPort);
                uDP_Class_serverPort.ConsoleWrite = false;
                List_UDP_Server.Add(uDP_Class_serverPort);
            }
            if (localPort > 0)
            {
                UDP_Class uDP_Class_localPort = new UDP_Class("0.0.0.0", localPort);
                uDP_Class_localPort.ConsoleWrite = false;
                List_UDP_Local.Add(uDP_Class_localPort);
            }

          
        }
        public void UDP_Class_Init()
        {
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
        }
        public void Set_UDP_WriteTime(int ms)
        {
            for (int i = 0; i < this.List_UDP_Server.Count; i++)
            {
                List_UDP_Server[i].WriteByteTime = ms;
            }
            for (int i = 0; i < this.List_UDP_Local.Count; i++)
            {
                List_UDP_Local[i].WriteByteTime = ms;
            }
        }

        public void Init_Offline()
        {
            this.FindForm().FormClosing += FormClosing;
            this.LoadConfig();
            this.rJ_Button_Station_Write.MouseDownEvent += RJ_Button_Station_Write_MouseDownEvent;
            this.rJ_Button_Read.MouseDownEvent += RJ_Button_Read_MouseDownEvent;
            this.rJ_Button_Write.MouseDownEvent += RJ_Button_Write_MouseDownEvent;
            this.rJ_Button_Lock_On.MouseDownEvent += RJ_Button_Lock_On_MouseDownEvent;
            this.rJ_Button_Lock_Off.MouseDownEvent += RJ_Button_Lock_Off_MouseDownEvent;
            this.rJ_Button_Save.MouseDownEvent += RJ_Button_Save_MouseDownEvent;
            this.rJ_Button_Load.MouseDownEvent += RJ_Button_Load_MouseDownEvent;

            mySerialPort.Init(this.textBox_COM.Text, 115200, 8, Parity.None, StopBits.One, false);
        }

     

        public void SerialPortOpen(string PortName)
        {
            mySerialPort.SerialPortClose();
            mySerialPort.PortName = PortName;
            mySerialPort.SerialPortOpen();
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

        public string Get_Version_UART()
        {
            return this.Get_Version_UART(mySerialPort.PortName);
        }
        public string Get_Version_UART(string PortName)
        {
            string ver = "";
            mySerialPort.PortName = PortName;
            Communication.UART_Command_Get_Version(mySerialPort, out ver);
            return ver;
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
        public List<string> GetAllSelectDeviceTableIP()
        {
            List<object[]> list_value = this.sqL_DataGridView_DeviceTable.Get_All_Select_RowsValues();
            List<string> list_IP = new List<string>();
            for (int i = 0; i < list_value.Count; i++)
            {
                list_IP.Add(list_value[i][(int)enum_DeviceTable.IP].ObjectToString());
            }
            return list_IP;
        }
        public List<object[]> SQL_GetDeviceTableRows(string IP)
        {
            List<object[]> list_value = this.sqL_DataGridView_DeviceTable.SQL_GetRows((int)enum_DeviceTable.IP, IP, false);
            return list_value;
        }
        public List<object[]> SQL_GetDeviceTableRows(List<ServerSettingClass> serverSettingClasses, string IP)
        {
            List<object[]> list_value = new List<object[]>();
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < serverSettingClasses.Count; i++)
            {
                ServerSettingClass serverSettingClass = serverSettingClasses[i];
                tasks.Add(Task.Run(new Action(delegate
                {
                    string Server = serverSettingClass.Server;
                    string DB = serverSettingClass.DBName;
                    string UserName = serverSettingClass.User;
                    string Password = serverSettingClass.Password;
                    uint Port = (uint)serverSettingClass.Port.StringToInt32();
                    SQLControl sQLControl = new SQLControl(Server, DB, this.sqL_DataGridView_DeviceTable.TableName, UserName, Password, Port, mySqlSslMode);
                    List<object[]> list_value_buf = sQLControl.GetRowsByDefult(null, (int)enum_DeviceTable.IP, IP);
                    list_value.LockAdd(list_value_buf);
                })));
            }

            return list_value;
        }
        public List<object[]> SQL_GetAllDeviceTableRows()
        {
            List<object[]> list_value = this.sqL_DataGridView_DeviceTable.SQL_GetAllRows(false);
            return list_value;
        }
        public List<object[]> SQL_GetAllDeviceTableRows(List<ServerSettingClass> serverSettingClasses)
        {
            List<object[]> list_value = new List<object[]>();
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < serverSettingClasses.Count; i++)
            {
                ServerSettingClass serverSettingClass = serverSettingClasses[i];
                tasks.Add(Task.Run(new Action(delegate
                {
                    string Server = serverSettingClass.Server;
                    string DB = serverSettingClass.DBName;
                    string UserName = serverSettingClass.User;
                    string Password = serverSettingClass.Password;
                    uint Port = (uint)serverSettingClass.Port.StringToInt32();
                    SQLControl sQLControl = new SQLControl(Server, DB, this.sqL_DataGridView_DeviceTable.TableName, UserName, Password, Port, mySqlSslMode);
                    List<object[]> list_value_buf = sQLControl.GetAllRows(null);
                    list_value.LockAdd(list_value_buf);
                })));
            }
          
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
        public List<string> SQL_GetAllDeviceTableValue(List<ServerSettingClass> serverSettingClasses)
        {
            List<object[]> list_value = this.SQL_GetAllDeviceTableRows(serverSettingClasses);
            List<string> list_DeviceTableValue = new List<string>();

            for (int i = 0; i < list_value.Count; i++)
            {

            }
            return list_DeviceTableValue;
        }



        virtual protected void Export()
        {

        }
        virtual protected void Import()
        {

        }
        virtual protected void SQL_AddDevice(string IP, int Port)
        {

        }
        virtual protected void SQL_ReplaceDevice(object[] value, string IP, int Port)
        {

        }
        public List<UDP_Class> GetLoacalUDP_Class()
        {
            return this.List_UDP_Local;
        }

        public bool WriteConfig(string PortName)
        {
            mySerialPort.PortName = PortName;
      

            bool flag_OK = true;
            int localport = this.Local_Port.StringToInt32();
            int serverport = this.Server_Port.StringToInt32();
            int udp_sendtime = this.UDP_SendTime.StringToInt32();
            if (localport < 0)
            {
                MyMessageBox.ShowDialog("localport is invalid!");
                return false;
            }
            if (serverport < 0)
            {
                MyMessageBox.ShowDialog("serverport is invalid!");
                return false;
            }
            if (udp_sendtime < 0)
            {
                MyMessageBox.ShowDialog("udp_sendtime is invalid!");
                return false;
            }
            if (!Communication.UART_Command_Set_IP_Adress(mySerialPort, this.IP_Adress))
            {
                return false;
            }
            if (!Communication.UART_Command_Set_Subnet(mySerialPort, this.Subnet))
            {
                return false;
            }
            if (!Communication.UART_Command_Set_Gateway(mySerialPort, this.Gateway)) return false;
            if (!Communication.UART_Command_Set_DNS(mySerialPort, this.DNS)) return false;
            if (!Communication.UART_Command_Set_Server_IP_Adress(mySerialPort, this.Server_IP_Adress)) return false;
            if (!Communication.UART_Command_Set_Local_Port(mySerialPort, localport)) return false;
            if (!Communication.UART_Command_Set_Server_Port(mySerialPort, serverport)) return false;
            if (!Communication.UART_Command_Set_SSID(mySerialPort, this.SSID)) return false;
            if (!Communication.UART_Command_Set_Password(mySerialPort, this._Password)) return false;
            if (!Communication.UART_Command_Set_UDP_SendTime(mySerialPort, udp_sendtime)) return false;
            return flag_OK;
        }
        public bool Ping(string IP)
        {
            return Basic.Net.Ping(IP, 3, 200);
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
                if (!Ping(list_IP[i]))
                {
                    list_disconnected_IP.Add(new object[] { list_IP[i] });
                }
            }
            this.sqL_DataGridView_PING.RefreshGrid(list_disconnected_IP);
        }

        #region Event
        private void RJ_Button_Load_MouseDownEvent(MouseEventArgs mevent)
        {
            this.LoadConfig();
            MyMessageBox.ShowDialog("讀取成功!");
        }
        private void RJ_Button_Save_MouseDownEvent(MouseEventArgs mevent)
        {
            this.SaveConfig();
            MyMessageBox.ShowDialog("儲存成功!");

        }  
        private void RJ_Button_Write_MouseDownEvent(MouseEventArgs mevent)
        {
            string Version = "";
            mySerialPort.PortName = this.textBox_COM.Text;
            if (Communication.UART_Command_Get_Version(mySerialPort, out Version))
            {

            }
            else
            {
                MyMessageBox.ShowDialog("UART Command get Version failed!");
                return;
            }

            bool flag_OK = this.WriteConfig(mySerialPort.PortName);

            if (flag_OK)
            {
                MyMessageBox.ShowDialog($"Write data sucessed! {Version}");
            }
            else
            {
                MyMessageBox.ShowDialog($"Write data failed! {Version}");
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
            if (Communication.UART_Command_Get_Setting(mySerialPort, out IP_Adress, out Subnet, out Gateway, out DNS, out Server_IP_Adress, out Local_Port, out Server_Port, out SSID, out Password, out Station, out UDP_SendTime))
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
        private void RJ_Button_Lock_On_MouseDownEvent(MouseEventArgs mevent)
        {
            string result = "";
            Communication.UART_Command_Set_Locker(mySerialPort, true, out result);
            MyMessageBox.ShowDialog($"{result}");
        }
        private void RJ_Button_Lock_Off_MouseDownEvent(MouseEventArgs mevent)
        {
            string result = "";
            Communication.UART_Command_Set_Locker(mySerialPort, false, out result);
            MyMessageBox.ShowDialog($"{result}");
        }
        private void FormClosing(object sender, FormClosingEventArgs e)
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
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_UDP_DataReceive.匯出.GetEnumName())
                    {
                        if (this.saveFileDialog_SaveExcel.ShowDialog() == DialogResult.OK)
                        {
                            DataTable dataTable = this.sqL_DataGridView_UDP_DataReceive.GetDataTable().DeepClone();
                            dataTable = dataTable.ReorderTable(new enum_UDP_DataReceive_匯出());
                            CSVHelper.SaveFile(dataTable, this.saveFileDialog_SaveExcel.FileName);
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
                if ((Time.GetTotalMilliseconds() - CycleTime_start) >= TimeOut)
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
                    RowsList_buf.Add(RowsList[i]);
                }
            }
            RowsList = RowsList_buf;
            RowsList.Sort(new ICP_UDP_Rx());
            for (int i = 0; i < RowsList.Count; i++)
            {
                RowsList[i][(int)UDP_Class.UDP_Rx.編號] = (i + 1).ToString("000");
            }
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
                            
                        }
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable.刷新.GetEnumName())
                    {
                        MyTimer myTimer = new MyTimer();
                        myTimer.StartTickTime(50000);
                        List<object[]> list_value_buf = this.sqL_DataGridView_DeviceTable.SQL_GetAllRows(false);
                        List<object[]> list_value_delete = (from value in list_value_buf
                                                            where !value[(int)enum_DeviceTable.IP].ObjectToString().Check_IP_Adress()
                                                            select value).ToList();
                        if (ConsoleWrite) Console.WriteLine($"Get data form SQL Time : {myTimer.GetTickTime().ToString("0.000")} ms");
                        this.sqL_DataGridView_DeviceTable.RefreshGrid(list_value_buf);
                        this.sqL_DataGridView_DeviceTable.SQL_DeleteExtra(list_value_delete, false);
                        this.sqL_DataGridView_DeviceTable.DeleteExtra(list_value_delete, true);
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
                if (IP_Array.Length < 4) continue;
                for (int k = 0; k < 4; k++)
                {
                    if (IP_Array[k] != IP_DeviceTable_Mask_Array[k] && IP_DeviceTable_Mask_Array[k] != "XXX") flag_add = false;
                }
                if (flag_add) RowsList_buf.Add(RowsList[i]);
            }
            RowsList = RowsList_buf;
        }
        #endregion
        #region StreamIO
        [Serializable]
        private class SaveConfigClass
        {
            private string iP_Adress = "";
            private string subnet = "";
            private string gateway = "";
            private string dNS = "";
            private string server_IP_Adress = "";
            private string local_Port = "";
            private string server_Port = "";
            private string sSID = "";
            private string password = "";
            private string station = "";
            private string uDP_SendTime = "";

            public string IP_Adress { get => iP_Adress; set => iP_Adress = value; }
            public string Subnet { get => subnet; set => subnet = value; }
            public string Gateway { get => gateway; set => gateway = value; }
            public string DNS { get => dNS; set => dNS = value; }
            public string Server_IP_Adress { get => server_IP_Adress; set => server_IP_Adress = value; }
            public string Local_Port { get => local_Port; set => local_Port = value; }
            public string Server_Port { get => server_Port; set => server_Port = value; }
            public string SSID { get => sSID; set => sSID = value; }
            public string Password { get => password; set => password = value; }
            public string Station { get => station; set => station = value; }
            public string UDP_SendTime { get => uDP_SendTime; set => uDP_SendTime = value; }
        }
        public void SaveConfig()
        {
            string StreamName = @".\\" + $"{this.Name}_Config" + ".pro";
            SaveConfigClass saveConfig = new SaveConfigClass();
            saveConfig.IP_Adress = this.IP_Adress;
            saveConfig.Subnet = this.Subnet;
            saveConfig.Gateway = this.Gateway;
            saveConfig.DNS = this.DNS;
            saveConfig.Server_IP_Adress = this.Server_IP_Adress;
            saveConfig.Local_Port = this.Local_Port;
            saveConfig.Server_Port = this.Server_Port;
            saveConfig.SSID = this.SSID;
            saveConfig.Password = this._Password;
            saveConfig.Station = this.Station;
            saveConfig.UDP_SendTime = this.UDP_SendTime;

            Basic.FileIO.SaveProperties(saveConfig, StreamName);
        }
        public void LoadConfig()
        {
            string StreamName = @".\\" + $"{this.Name}_Config" + ".pro";
            object temp = new object();
            Basic.FileIO.LoadProperties(ref temp, StreamName);
            if (temp is SaveConfigClass)
            {
                SaveConfigClass saveConfigClass = temp as SaveConfigClass;
                this.Invoke(new Action(delegate
                {
                    this.IP_Adress = saveConfigClass.IP_Adress;
                    this.Subnet = saveConfigClass.Subnet;
                    this.Gateway = saveConfigClass.Gateway;
                    this.DNS = saveConfigClass.DNS;
                    this.Server_IP_Adress = saveConfigClass.Server_IP_Adress;
                    this.Local_Port = saveConfigClass.Local_Port;
                    this.Server_Port = saveConfigClass.Server_Port;
                    this.SSID = saveConfigClass.SSID;
                    this._Password = saveConfigClass.Password;
                    this.Station = saveConfigClass.Station;
                    this.UDP_SendTime = saveConfigClass.UDP_SendTime;

                }));
            }

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
                if (IP_0_Array.Length < 4) return 0;
                if (IP_1_Array.Length < 4) return 0;
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
