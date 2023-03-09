using System;
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
    public partial class H_RFID_UI : DeviceBasicUI
    {
        private enum ContextMenuStrip_Main
        {
            RFID測試頁面,
        }


        [Serializable]
        public class UDP_READ
        {
            private List<RFID_Data> rFID_Datas = new List<RFID_Data>();
            [JsonIgnore]
            public List<RFID_Data> RFID_Datas
            {
                get
                {
                    rFID_Datas.Clear();
                    if (cardID_01.StringIsEmpty() == false)
                    {
                        rFID_Datas.Add(new RFID_Data(this.IP, cardID_01, RSSI_01.StringToInt32()));
                    }
                    if (cardID_02.StringIsEmpty() == false)
                    {
                        rFID_Datas.Add(new RFID_Data(this.IP, cardID_02, RSSI_02.StringToInt32()));
                    }
                    if (cardID_03.StringIsEmpty() == false)
                    {
                        rFID_Datas.Add(new RFID_Data(this.IP, cardID_03, RSSI_03.StringToInt32()));
                    }
                    if (cardID_04.StringIsEmpty() == false)
                    {
                        rFID_Datas.Add(new RFID_Data(this.IP, cardID_04, RSSI_04.StringToInt32()));
                    }
                    if (cardID_05.StringIsEmpty() == false)
                    {
                        rFID_Datas.Add(new RFID_Data(this.IP, cardID_05, RSSI_05.StringToInt32()));
                    }
                    return rFID_Datas;
                }
            }
            public class RFID_Data
            {
                private string iP = "";
                private string card_ID = "";
                private int rSSI = 0;

                public string Card_ID { get => card_ID; set => card_ID = value; }
                public int RSSI { get => rSSI; set => rSSI = value; }
                public string IP { get => iP; set => iP = value; }

                public RFID_Data(string IP ,string Card_ID ,int RSSI)
                {
                    this.IP = IP;
                    this.Card_ID = Card_ID;
                    this.RSSI = RSSI;
                }
            }
            public class BlinkEnable_Data
            {
                private string iP = "";
                private int pinNum = 0;

                public string IP { get => iP; set => iP = value; }
                public int PinNum { get => pinNum; set => pinNum = value; }

                public BlinkEnable_Data(string IP , int PinNum)
                {
                    this.IP = IP;
                    this.PinNum = PinNum;
                }
            }
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
            private int rSSI_01 = 0;
            private int rSSI_02 = 0;
            private int rSSI_03 = 0;
            private int rSSI_04 = 0;
            private int rSSI_05 = 0;

            private bool blinkEnable01 = false;
            private bool blinkEnable02 = false;
            private bool blinkEnable03 = false;
            private bool blinkEnable04 = false;
            private bool blinkEnable05 = false;
            private bool blinkEnable06 = false;
            private bool blinkEnable07 = false;
            private bool blinkEnable08 = false;
            private bool blinkEnable09 = false;
            private bool blinkEnable10 = false;

            private int input_dir = 0;
            private int output_dir = 0;

            public string IP { get => iP; set => iP = value; }
            public int Port { get => port; set => port = value; }
            public string Version { get => version; set => version = value; }
            public int Input { get => input; set => input = value; }
            public int Output { get => output; set => output = value; }
            public int RSSI { get => rSSI; set => rSSI = value; }
        
            public int Input_dir { get => input_dir; set => input_dir = value; }
            public int Output_dir { get => output_dir; set => output_dir = value; }
            public string CardID_01 { get => cardID_01; set => cardID_01 = value; }
            public string CardID_02 { get => cardID_02; set => cardID_02 = value; }
            public string CardID_03 { get => cardID_03; set => cardID_03 = value; }
            public string CardID_04 { get => cardID_04; set => cardID_04 = value; }
            public string CardID_05 { get => cardID_05; set => cardID_05 = value; }
            public int RSSI_01 { get => rSSI_01; set => rSSI_01 = value; }
            public int RSSI_02 { get => rSSI_02; set => rSSI_02 = value; }
            public int RSSI_03 { get => rSSI_03; set => rSSI_03 = value; }
            public int RSSI_04 { get => rSSI_04; set => rSSI_04 = value; }
            public int RSSI_05 { get => rSSI_05; set => rSSI_05 = value; }
            public bool BlinkEnable01 { get => blinkEnable01; set => blinkEnable01 = value; }
            public bool BlinkEnable02 { get => blinkEnable02; set => blinkEnable02 = value; }
            public bool BlinkEnable03 { get => blinkEnable03; set => blinkEnable03 = value; }
            public bool BlinkEnable04 { get => blinkEnable04; set => blinkEnable04 = value; }
            public bool BlinkEnable05 { get => blinkEnable05; set => blinkEnable05 = value; }
            public bool BlinkEnable06 { get => blinkEnable06; set => blinkEnable06 = value; }
            public bool BlinkEnable07 { get => blinkEnable07; set => blinkEnable07 = value; }
            public bool BlinkEnable08 { get => blinkEnable08; set => blinkEnable08 = value; }
            public bool BlinkEnable09 { get => blinkEnable09; set => blinkEnable09 = value; }
            public bool BlinkEnable10 { get => blinkEnable10; set => blinkEnable10 = value; }

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
            public int Get_RSSI(int index)
            {
                if (index == 0)
                {
                    return RSSI_01;
                }
                if (index == 1)
                {
                    return RSSI_02;
                }
                if (index == 2)
                {
                    return RSSI_03;
                }
                if (index == 3)
                {
                    return RSSI_04;
                }
                if (index == 4)
                {
                    return RSSI_05;
                }
                return 0;
            }
            public string Get_CardID(int index)
            {
                if (index == 0)
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
            public bool Get_BlinkEnable(int index)
            {
                if (index == 0)
                {
                    return BlinkEnable01;
                }
                if (index == 1)
                {
                    return BlinkEnable02;
                }
                if (index == 2)
                {
                    return BlinkEnable03;
                }
                if (index == 3)
                {
                    return BlinkEnable04;
                }
                if (index == 4)
                {
                    return BlinkEnable05;
                }
                if (index == 5)
                {
                    return BlinkEnable06;
                }
                if (index == 6)
                {
                    return BlinkEnable07;
                }
                if (index == 7)
                {
                    return BlinkEnable08;
                }
                if (index == 8)
                {
                    return BlinkEnable09;
                }
                if (index == 9)
                {
                    return BlinkEnable10;
                }
           
                return false;
            }
        }
        public H_RFID_UI()
        {
            Enum_ContextMenuStrip_UDP_DataReceive = new ContextMenuStrip_Main();
            this.DeviceTableMouseDownRightEvent += H_RFID_UI_DeviceTableMouseDownRightEvent;
            this.UDP_DataReceiveMouseDownRightEvent += H_RFID_UI_UDP_DataReceiveMouseDownRightEvent;
        }



        public UDP_READ GerUDP_READ(string IP)
        {
            string jsonString = this.GetUDPJsonString(IP);
            if (jsonString.StringIsEmpty()) return null;
            UDP_READ uDP_READ = jsonString.JsonDeserializet<UDP_READ>();
            return uDP_READ;
        }
        public List<UDP_READ> GerAllUDP_READ()
        {
            List<UDP_READ> uDP_READs = new List<UDP_READ>();

            List<string> list_UDPJsonString = this.GetAllUDPJsonString();

            Parallel.ForEach(list_UDPJsonString, value =>
            {
                string jsonString = value;
                UDP_READ uDP_READ = jsonString.JsonDeserializet<UDP_READ>();
                if (uDP_READ != null) uDP_READs.LockAdd(uDP_READ);
            });

            return uDP_READs;
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
        virtual public bool Set_OutputPIN(string IP, int Port, int Num, bool value)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
            if (uDP_Class == null) return false;
            return Communication.Set_OutputPIN(uDP_Class, IP, Num, value);
        }
        virtual public bool Set_BlinkEnable(string IP, int Port, int Num, bool value, int blinkTime)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
            if (uDP_Class == null) return false;
            return Communication.Set_BlinkEnable(uDP_Class, IP, Num, value, blinkTime);
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
            if (flag)
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
        public bool GetInput(string IP, int Num)
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
        public string GetRFID(string IP, int CardNum)
        {
            string jsonString = GetUDPJsonString(IP);
            if (jsonString.StringIsEmpty()) return "";
            UDP_READ uDP_READ = jsonString.JsonDeserializet<UDP_READ>();
            if (uDP_READ == null) return "";
            return uDP_READ.Get_CardID(CardNum);
        }
        public List<UDP_READ.RFID_Data> GetRFID()
        {
            List<UDP_READ.RFID_Data> RFID_Datas = new List<UDP_READ.RFID_Data>();
            List<UDP_READ> uDP_READs = this.GerAllUDP_READ();
            for (int i = 0; i < uDP_READs.Count; i++)
            {
                for (int k = 0; k < uDP_READs[i].RFID_Datas.Count; k++)
                {
                    RFID_Datas.Add(uDP_READs[i].RFID_Datas[k]);
                }
            }           
            return RFID_Datas;
        }
        public bool GetBlinkEnable(string IP , int PinNum)
        {
            List<UDP_READ> uDP_READs = this.GerAllUDP_READ();
            for (int i = 0; i < uDP_READs.Count; i++)
            {
                if (uDP_READs[i].IP == IP)
                {
                    return uDP_READs[i].Get_BlinkEnable(PinNum);
                }
            }

            return false;
        }
        public List<UDP_READ.BlinkEnable_Data> GetBlinkEnable()
        {
            List<UDP_READ.BlinkEnable_Data> blinkEnable_Datas = new List<UDP_READ.BlinkEnable_Data>();
            List<UDP_READ> uDP_READs = this.GerAllUDP_READ();
            for (int i = 0; i < uDP_READs.Count; i++)
            {
                for (int k = 0; k < 10; k++)
                {
                    if(uDP_READs[i].Get_BlinkEnable(k))
                    {
                        UDP_READ.BlinkEnable_Data blinkEnable_Data = new UDP_READ.BlinkEnable_Data(uDP_READs[i].IP, k);
                        blinkEnable_Datas.Add(blinkEnable_Data);
                    }
                }
            }
            return blinkEnable_Datas;
        }
        #region Event
        private void H_RFID_UI_UDP_DataReceiveMouseDownRightEvent(string selectedText, List<IPEndPoint> iPEndPoints)
        {
            if (selectedText == ContextMenuStrip_Main.RFID測試頁面.GetEnumName())
            {
                if (iPEndPoints.Count == 0) return;
                string IP = iPEndPoints[0].Address.ToString();
                int Port = iPEndPoints[0].Port;
                UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
                if (uDP_Class == null) return;
                Dialog_RFID測試頁面 dialog_RFID測試頁面 = new Dialog_RFID測試頁面(uDP_Class, IP, list_UDP_Rx);
                dialog_RFID測試頁面.ShowDialog();
            }
        }
        private void H_RFID_UI_DeviceTableMouseDownRightEvent(string selectedText, List<IPEndPoint> iPEndPoints)
        {
            if (selectedText == ContextMenuStrip_Main.RFID測試頁面.GetEnumName())
            {
                if (iPEndPoints.Count == 0) return;
                string IP = iPEndPoints[0].Address.ToString();
                int Port = iPEndPoints[0].Port;
                UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
                if (uDP_Class == null) return;
                Dialog_RFID測試頁面 dialog_RFID測試頁面 = new Dialog_RFID測試頁面(uDP_Class, IP, list_UDP_Rx);
                dialog_RFID測試頁面.ShowDialog();
            }
        }
        #endregion
    }
}
