using System;
using System.Collections.Generic;
using System.Linq;

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
using System.Text;
using System.Reflection;
using SkiaSharp;
using System.Threading;

namespace H_Pannel_lib
{
    public class Driver_IO_Board
    {
        public delegate void ProgramEventHandler(Driver_IO_Board driver_IO_Board);
        public event ProgramEventHandler ProgramEvent;
        public int SleepTime = 0;
        public StationClass this[byte station]
        {
            get
            {
                for (int i = 0; i < stationClasses.Count; i++)
                {
                    if (stationClasses[i].station == station) return stationClasses[i];
                }
                return null;
            }
        }
        public class StationClass
        {
            public StationClass(int station)
            {
                this.station = station;
            }
            public class InputClass
            {
                public int Port { get; set; }
                public bool this[int index]
                {
                    get
                    {
                        return Port.GetBit(index);
                    }
                }
            }
            public class OutputClass
            {
                public int Port { get; set; }
                private int port_Refresh = 0;
                public int Port_Refresh
                {
                    get
                    {
                        return port_Refresh;
                    }
                    set
                    {
                        port_Refresh = value;
                    }
                }
                private int port_Refresh_state = 0;
                public int Port_Refresh_state
                {
                    get
                    {
                        return port_Refresh_state;
                    }
                    set
                    {
                        port_Refresh_state = value;
                    }
                }

                public bool this[int index]
                {
                    get
                    {
                        return Port.GetBit(index);
                    }
                    set
                    {
                        port_Refresh_state = port_Refresh_state.SetBit(index, value);
                        port_Refresh = port_Refresh.SetBit(index, true);
                    }
                }

            }
            public InputClass Input = new InputClass();
            public OutputClass Output = new OutputClass();
            public int station { get; set; }
            public bool flag_Setoutput = false;
            public int Setoutput_Value = 0;
            public void SetOutput(int index, bool state)
            {
                int Port_Refresh_state_temp = Output.Port_Refresh_state;
                Output.Port_Refresh_state = Port_Refresh_state_temp.SetBit(index, state);

                int Port_Refresh_temp = Output.Port_Refresh;
                Output.Port_Refresh = Port_Refresh_temp.SetBit(index, state);
            }
        }
        public List<StationClass> stationClasses = new List<StationClass>();
        private MyThread myThread;
        private MySerialPort mySerialPort;
        public void Init(MySerialPort MySerialPort, params byte[] stations)
        {
            mySerialPort = MySerialPort;
            for (int i = 0; i < stations.Length; i++)
            {
                this.stationClasses.Add(new StationClass(stations[i]));
            }
            myThread = new MyThread();
            myThread.Add_Method(sub_program);
            myThread.AutoRun(true);
            myThread.SetSleepTime(SleepTime);
            myThread.Trigger();
        }
        private void sub_program()
        {
            for (int i = 0; i < stationClasses.Count; i++)
            {
                StationClass stationClass = stationClasses[i];
                int station = stationClass.station;
                int input = 0;
                int output = 0;
                if (Communication.UART_Command_RS485_GetIO(mySerialPort, station, ref input, ref output))
                {
                    stationClass.Input.Port = input;
                    stationClass.Output.Port = output;
                }


            }
            if (ProgramEvent != null) ProgramEvent(this);
            for (int i = 0; i < stationClasses.Count; i++)
            {
                StationClass stationClass = stationClasses[i];
                int station = stationClass.station;
                int output = stationClass.Output.Port;
                if (stationClass.Output.Port_Refresh > 0)
                {
                    for (int k = 0; k < 16; k++)
                    {
                        if (stationClass.Output.Port_Refresh.GetBit(k))
                        {
                            output.SetBit(k, stationClass.Output.Port_Refresh_state.GetBit(k));
                        }
                    }
                    Communication.UART_Command_RS485_SetOutput(mySerialPort, station, output);
                    stationClass.Output.Port_Refresh = 0;
                }
            }
        }
    }
}
