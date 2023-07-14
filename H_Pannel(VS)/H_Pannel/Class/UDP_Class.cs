using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;
using Basic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using System.Drawing.Drawing2D;


namespace H_Pannel_lib
{
    public static class UDP_ClassMethod
    {
        public static UDP_Class SortByPort(this List<UDP_Class> uDP_Classes , int Port)
        {
            List<UDP_Class> uDP_Classes_buf = new List<UDP_Class>();
            uDP_Classes_buf = (from value in uDP_Classes
                               where value.Port == Port
                               select value).ToList();
            if (uDP_Classes_buf.Count == 0) return null;
            return uDP_Classes_buf[0];
        }
        public static UDP_Class SortByIP(this List<UDP_Class> uDP_Classes, string IP)
        {
            List<UDP_Class> uDP_Classes_buf = new List<UDP_Class>();
            uDP_Classes_buf = (from value in uDP_Classes
                               where value.IP == IP
                               select value).ToList();
            if (uDP_Classes_buf.Count == 0) return null;
            return uDP_Classes_buf[0];
        }
    }
    public class UDP_Class
    {
        public enum UDP_Rx
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
        private string iP = "";
        public string IP
        {
            get
            {
                return iP;
            }
            private set
            {
                iP = value;
            }
        }
        private int port = 0;
        public int Port
        {
            get
            {
                return port;
            }
            private set
            {
                port = value;
            }
        }
        public bool ConsoleWrite = false;
        public delegate void DataReciveEventHandler(string IP, int Port, string readline);
        public event DataReciveEventHandler DataReciveEvent;
        public List<object[]> List_UDP_Rx = new List<object[]>();
        public List<string> List_Readline = new List<string>();
        private MyThread MyThread_Program;
        private IPEndPoint remoteIP;
        private IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
        private UdpClient udpClient;
        private Stopwatch stopwatch = new Stopwatch();
        private byte[] data = new byte[4096 * 10]; //存放接收的資料
        private MyConvert myConvert = new MyConvert();
        public bool IsDataRecive
        {
            get
            {
                return (_readline != "");
            }
        }
        private string _readline = "";
        public string readline
        {
            get
            {

                return this._readline;
            }
            set
            {
                this._readline = value;
            }
        }
        private double WriteByteStartTime = 0;
        static public double WriteByteTime = 25;
        public UDP_Class(string IP, int port)
        {
            this.Port = port;
            udpClient =new UdpClient(Port);
            udpClient.Client.SendBufferSize = 409600;
            udpClient.Client.ReceiveBufferSize = 409600;

            udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), udpClient);
            this.MyThread_Program = new MyThread();
            this.MyThread_Program.Add_Method(sub_ReadByte);
            this.MyThread_Program.AutoRun(true);
            this.MyThread_Program.SetSleepTime(1);
            this.MyThread_Program.Trigger();
            this.stopwatch.Start();
        }
        private void ReceiveCallback(IAsyncResult result)
        {
            UdpClient ts = (UdpClient)result.AsyncState;
            IPEndPoint remoteIP = new IPEndPoint(IPAddress.Any, 0);
            try
            {
                byte[] buffer = ts.EndReceive(result, ref remoteIP);
                string returnData = Encoding.GetEncoding("BIG5").GetString(buffer);
                returnData += $"${remoteIP.Address.ToString()}";
                returnData += $"${remoteIP.Port.ToString()}";
                result.AsyncWaitHandle.Close();
                udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), udpClient);
                List_Readline.Insert(0, returnData);
            }
            catch
            {
                try
                {
                    udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), udpClient);
                }
                catch
                {

                }    
            }
        }
        public void WriteByte(byte[] value, string IP)
        {
            this.WriteByte(value, IP, this.port);
        }
        public void WriteByte(byte[] value , string IP ,int Port)
        {
            lock(this)
            {
                while(true)
                {
                    if ((stopwatch.Elapsed.TotalMilliseconds - WriteByteStartTime) >= WriteByteTime) break;
                }
                this.remoteIP = new IPEndPoint(IPAddress.Parse(IP), Port); //定義一個位址 (伺服器位址)
                List<byte[]> list_bytes = separateByte(value);
                for (int i = 0; i < list_bytes.Count; i++)
                {
                    while (true)
                    {
                        if ((stopwatch.Elapsed.TotalMilliseconds - WriteByteStartTime) >= WriteByteTime) break;
                    }
                    int recv = this.udpClient.Send(list_bytes[i], list_bytes[i].Length, remoteIP); //送出的資料跟目的 
                    //Console.WriteLine("{0}:{1}發送消息：{2}", IP, port, list_bytes[i].Length);
                    this.WriteByteStartTime = stopwatch.Elapsed.TotalMilliseconds;
                }

                //int recv = this.udpClient.Send(value, value.Length, remoteIP); //送出的資料跟目的 
                //if (ConsoleWrite) Console.WriteLine("{0}:{1}發送消息：{2}", IP,port, value.Length);
                this.WriteByteStartTime = stopwatch.Elapsed.TotalMilliseconds;
            }
                 
        }
        public List<byte[]> separateByte(byte[] bytes)
        {
            List<byte[]> list_bytes_out = new List<byte[]>();

            int len = bytes.Length;
            List<byte> list_header = new List<byte>();
            list_header.Add((byte)len);
            list_header.Add((byte)(len >> 8));
            list_header.Add((byte)(len >> 16));
            list_header.Add((byte)(len >> 24));
            list_bytes_out.Add(list_header.ToArray());

            int WritePackageSize = 1400;
            int div = bytes.Length / WritePackageSize;
            int mod = bytes.Length % WritePackageSize;
            for (int i = 0; i < div; i++)
            {
                byte[] bytes_div = new byte[WritePackageSize];
                for (int k = 0; k < WritePackageSize; k++)
                {
                    bytes_div[k] = bytes[i * WritePackageSize + k];
                }
                list_bytes_out.Add(bytes_div);
            }
            byte[] bytes_mod = new byte[mod];
            for (int i = 0; i < mod; i++)
            {
                bytes_mod[i] = bytes[div * WritePackageSize + i];
            }
            list_bytes_out.Add(bytes_mod);
            return list_bytes_out;
        }
        private void sub_ReadByte()
        {
       
            while(true)
            {
                if (List_Readline.Count == 0) return;
                string returnData = List_Readline[List_Readline.Count - 1];
                List_Readline.RemoveAt(List_Readline.Count - 1);

                double CycleTime_start = 0;
                string IP = "";
                string[] str_Array = myConvert.分解分隔號字串(returnData, '$');
                if (str_Array.Length == 3)
                {
                    IP = str_Array[1];


                    List<object[]> List_UDP_Rx_buf = new List<object[]>();
                    List_UDP_Rx_buf = (from value in List_UDP_Rx
                                       where value[(int)UDP_Rx.IP].ObjectToString() == IP
                                       select value).ToList();
                    if (List_UDP_Rx_buf.Count == 1)
                    {
                        double.TryParse(List_UDP_Rx_buf[0][(int)UDP_Rx.StartTime].ObjectToString(), out CycleTime_start);
                        List_UDP_Rx_buf[0][(int)UDP_Rx.Readline] = str_Array[0];
                        List_UDP_Rx_buf[0][(int)UDP_Rx.Port] = str_Array[2];
                        List_UDP_Rx_buf[0][(int)UDP_Rx.StartTime] = Time.GetTotalMilliseconds().ToString("0.000000");
                        List_UDP_Rx_buf[0][(int)UDP_Rx.Time] = (Time.GetTotalMilliseconds() - CycleTime_start).ToString("0.000000");
                        List_UDP_Rx_buf[0][(int)UDP_Rx.State] = "OK";
                    }
                    else
                    {
                        object[] value = new object[new UDP_Rx().GetLength()];
                        value[(int)UDP_Rx.GUID] = Guid.NewGuid().ToString();
                        value[(int)UDP_Rx.編號] = "0";
                        value[(int)UDP_Rx.IP] = IP;
                        value[(int)UDP_Rx.Port] = Port;
                        value[(int)UDP_Rx.Readline] = str_Array[0];
                        value[(int)UDP_Rx.StartTime] = stopwatch.Elapsed.TotalMilliseconds.ToString("0.000000");
                        value[(int)UDP_Rx.Time] = "0";
                        value[(int)UDP_Rx.State] = "OK";

                        this.List_UDP_Rx.LockAdd(value);
                    }

                    if (this.DataReciveEvent != null) this.DataReciveEvent(IP, ((IPEndPoint)(remoteIpEndPoint)).Port, str_Array[0]);

                }
            }
         

        }
        public void Set_ReadLineClearByIP(string IP)
        {
            List<object[]> List_UDP_Rx_buf = new List<object[]>();
            List_UDP_Rx_buf = (from value in List_UDP_Rx
                               where value[(int)UDP_Rx.IP].ObjectToString() == IP
                               select value).ToList();
            if (List_UDP_Rx_buf.Count > 0)
            {
                List_UDP_Rx_buf[0][(int)UDP_Rx.Readline] = "";
            }
        }
        public string Get_ReadLineByIP(string IP)
        {
            string _readline = "";
            List<object[]> list_UDP_Rx = new List<object[]>();
            for (int i = 0; i < List_UDP_Rx.Count; i++)
            {
                object[] value = new object[List_UDP_Rx[i].Length];
                for(int k = 0; k < value.Length; k++)
                {
                    value[k] = List_UDP_Rx[i][k];
                }
                list_UDP_Rx.Add(value);
            }

            List<object[]> List_UDP_Rx_buf = new List<object[]>();
            List_UDP_Rx_buf = (from value in list_UDP_Rx
                               where value[(int)UDP_Rx.IP].ObjectToString() == IP
                               select value).ToList();
            if (List_UDP_Rx_buf.Count > 0)
            {
                _readline = List_UDP_Rx_buf[0][(int)UDP_Rx.Readline].ObjectToString();
                List_UDP_Rx_buf[0][(int)UDP_Rx.Readline] = "";
            }
            return _readline;
        }
        public string Get_ReadLineByGUID(string GUID)
        {
            string _readline = "";
            List<object[]> list_UDP_Rx = new List<object[]>();
            for (int i = 0; i < List_UDP_Rx.Count; i++)
            {
                object[] value = new object[List_UDP_Rx[i].Length];
                for (int k = 0; k < value.Length; k++)
                {
                    value[k] = List_UDP_Rx[i][k];
                }
                list_UDP_Rx.Add(value);
            }
            List<object[]> List_UDP_Rx_buf = new List<object[]>();
            List_UDP_Rx_buf = (from value in list_UDP_Rx
                               where value[(int)UDP_Rx.GUID].ObjectToString() == GUID
                               select value).ToList();
            if (List_UDP_Rx_buf.Count > 0)
            {
                _readline = List_UDP_Rx_buf[0][(int)UDP_Rx.Readline].ObjectToString();
                List_UDP_Rx_buf[0][(int)UDP_Rx.Readline] = "";
            }
            return _readline;
        }
        public void Dispose()
        {
            if (MyThread_Program != null) MyThread_Program.Stop();
            if (this.udpClient != null)
            {
                this.udpClient.Close();
                this.udpClient.Dispose();
            }
        }

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
                int cmp = IP_0_Array[3].CompareTo(IP_1_Array[3]);
                if (cmp > 0)
                {
                    return 1;
                }
                else if (cmp < 0)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }



            }
        }
    }
    class MyTimer
    {
        private bool OnTick = false;
        private double TickTime = 0;
        static private Stopwatch stopwatch = new Stopwatch();
        public MyTimer()
        {
            stopwatch.Start();
        }
        public MyTimer(string adress)
        {
            stopwatch.Start();
        }
        private double CycleTime_start;
        public void StartTickTime(double TickTime)
        {
            this.TickTime = TickTime;
            if (!OnTick)
            {
                CycleTime_start = stopwatch.Elapsed.TotalMilliseconds;
                OnTick = true;
            }
        }

        public double GetTickTime()
        {
            return stopwatch.Elapsed.TotalMilliseconds - CycleTime_start;
        }
        public void TickStop()
        {
            this.OnTick = false;
        }
        public bool IsTimeOut()
        {
            if ((stopwatch.Elapsed.TotalMilliseconds - CycleTime_start) >= TickTime)
            {
                OnTick = false;
                return true;
            }
            else return false;
        }

    }
}
