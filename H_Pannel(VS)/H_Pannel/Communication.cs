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
namespace H_Pannel_lib
{
    public enum Hex_Type
    {
        H_L,
        L_H,
    }
    public enum ChipType
    {
        ESP32,
        BW16,
    }
    public enum EPD_Type
    {
        EPD213_B,
        EPD266_B,
        EPD583,
        EPD290_V2,
        EPD290_V3,
        EPD420,
        EPD1020,
        EPD730F
    }
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
                if(Communication.UART_Command_RS485_GetIO(mySerialPort, station, ref input, ref output))
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
    public class Communication
    {
        public delegate void ProcessBarEventHandler(int value, int max, string info);
        static public event ProcessBarEventHandler ProcessBarEvent;

        static public bool ConsoleWrite = false;
        static public int UDP_TimeOut = 1000;
        static public int UDP_RetryNum = 5;
        static public readonly int Image_Buffer_SIZE = 1200;
        static public ChipType Chip_Type = ChipType.BW16;
        //static public int EPD583_frameDIV = 10;
        public enum enum_LED_Type : int
        {
            WHITE, RED, BLUE, YELLOW, WARM_WHITE
        }
        private enum UDP_Command
        {
            Cusor_posituin = (byte)'0',
            ClearCanvas = (byte)'1',
            Framebuffer = (byte)'2',
            DrawCanvas = (byte)'3',
            DrawLine = (byte)'4',
            WS2812_Blink = (byte)'5',
            FramebufferEx = (byte)'6',
            FontColor = (byte)'7',
            Code_FrameWrite = (byte)'8',
            ESP_Restart = (byte)'9',
            DrawCanvasEx = (byte)'a',
            GetKeyBoardValue = (byte)'b',
            Set_Jpegbuffer = (byte)'j',


            Set_PIN = (byte)'A',
            OTAUpdate = (byte)'B',
            SetToPage = (byte)'C',
            Set_TOF = (byte)'D',
            DrawRect = (byte)'E',
            Set_UDP_SendTime = (byte)'F',
            Set_ScreenPageInit = (byte)'G',       
            Set_ServerConfig = (byte)'H',
            Set_JsonStringSend = (byte)'I',
            Set_Output = (byte)'J',
            Set_OutputPIN = (byte)'K',
            Set_WS2812_Buffer = (byte)'L',
            Set_GetewayConfig = (byte)'M',
            Set_LocalPort = (byte)'N',
            Get_WS2812_Buffer = (byte)'O',
            Set_OutputTrigger = (byte)'P',
            Set_OutputPINTrigger = (byte)'Q',
     
            Set_Input_dir = (byte)'R',
            Set_Output_dir = (byte)'S',
            Set_RFID_Enable = (byte)'T',
            Set_RFID_Beep = (byte)'U',
            Get_LEDSetting =(byte)'V',
            Set_LEDSetting = (byte)'W',
            Get_IO = (byte)'X',
            Set_BlinkEnable = (byte)'Y',       
            SendTestData = (byte)'Z',

            Set_WS2812B_breathing = (byte)'@',
            Set_ADCMotorTrigger = (byte)'!',

            EPD_Set_Sleep = (byte)'a',
            EPD_Set_WakeUp = (byte)'b',
            EPD_DrawFrame_RW = (byte)'c',
            EPD_DrawFrame_BW = (byte)'d',
            EPD_Send_Framebuffer = (byte)'e',
            EPD_RefreshCanvas = (byte)'f',
            EPD_Get_LaserDistance = (byte)'g',
            EPD_BW_Command = (byte)'h',
            EPD_RW_Command = (byte)'i',
            EPD_SendSPI = (byte)'j',
            EPD_GetType = (byte)'k',

        }

        static public bool Set_TOF(UDP_Class uDP_Class, string IP, bool Statu)
        {
            return Command_Set_TOF(uDP_Class, IP, Statu);
        }

        static public bool Set_ClearCanvas(UDP_Class uDP_Class, string IP, int x, int y, int width, int height, Color color)
        {
            if (!Command_Set_Cusor_position(uDP_Class, IP, x, y, width, height))
            {
                return false;
            }
            if (!Command_ClearCanvas(uDP_Class, IP, color)) return false;
            return true;
        }
        static public bool Set_BarCode(UDP_Class uDP_Class, string IP, string content, int x, int y, int width, int height)
        {
            return Set_SetBitmap(uDP_Class, IP, CreateBarCode(content, width, height), x, y);           
        }
        static public bool Set_BarCodeEx(UDP_Class uDP_Class, string IP, string content, int x, int y, int width, int height)
        {
            return Set_SetBitmapEx(uDP_Class, IP, CreateBarCode(content, width, height), x, y);
        }
        static public bool Set_Text(UDP_Class uDP_Class, string IP, string Text, int x, int y, Font font)
        {
            return Set_Text(uDP_Class, IP, Text, x, y, font, Color.Black, Color.White);
        }
        static public bool Set_Text(UDP_Class uDP_Class, string IP, string Text, int x, int y, Font font, Color FontColor, Color FontBackColor)
        {
            using (Bitmap bitmap = TextToBitmap(Text, font, FontColor, FontBackColor))
            {
                if (bitmap == null) return true;
                List<byte[]> list_bmp_byte = SplitImage(BitmapToByte(bitmap), Image_Buffer_SIZE);

                if (!Command_Set_Cusor_position(uDP_Class, IP, x, y, bitmap.Width, bitmap.Height))
                {
                    return false;
                }
    
                for (int i = 0; i < list_bmp_byte.Count; i++)
                {
                    long StatrPtr = i * Image_Buffer_SIZE / 2;
                    if (!Command_Framebuffer(uDP_Class, IP, StatrPtr, list_bmp_byte[i]))
                    {
                        return false;
                    }
                }
                if (!Command_DrawCanvas(uDP_Class, IP))
                {
                    return false;
                }

            }

            return true;

        }
        static public bool Set_TextEx(UDP_Class uDP_Class,string IP, string Text, int x, int y, Font font, Color FontColor, Color FontBackColor)
        {
            Size string_size = TextRenderer.MeasureText(Text, font);
            return Set_TextEx(uDP_Class, IP, Text, x, y, string_size.Width, string_size.Height, font, FontColor, FontBackColor, HorizontalAlignment.Left);
        }
        static public bool Set_TextEx(UDP_Class uDP_Class, string IP, string Text, int x, int y,int width, int height, Font font, Color FontColor, Color FontBackColor , HorizontalAlignment horizontalAlignment)
        {
            using (Bitmap bitmap = TextToBitmap(Text, font, 1, width, height, Color.White, Color.Black, 0, Color.Black, horizontalAlignment))
            {
                if (bitmap == null) return true;
                int len = 0;
                int last_len = 0;
                List<byte[]> list_bmp_byte = SplitImage(BitmapToByteEx(bitmap, ref len), Image_Buffer_SIZE);

                //if (!Command_Set_Cusor_position(uDP_Class, IP, x, y, bitmap.Width, bitmap.Height))
                //{
                //    return false;
                //}

                if (!Command_FontColor(uDP_Class, IP, FontColor, FontBackColor))
                {
                    return false;
                }
                last_len = len % (Image_Buffer_SIZE * 8);
                for (int i = 0; i < list_bmp_byte.Count; i++)
                {
                    long StatrPtr = i * Image_Buffer_SIZE * 8;
                    int lenth = Image_Buffer_SIZE * 8;
                    if (i == list_bmp_byte.Count - 1) lenth = last_len;
                    if (!Command_FramebufferEx(uDP_Class, IP, StatrPtr, list_bmp_byte[i], lenth))
                    {
                        return false;
                    }
                }
                if (!Command_DrawCanvasEx(uDP_Class, IP, x, y, bitmap.Width, bitmap.Height))
                {
                    return false;
                }

            }

            return true;

        }
        static public bool Set_SetBitmap(UDP_Class uDP_Class, string IP, Bitmap bitmap, int x, int y)
        {
            List<byte[]> list_bmp_byte = SplitImage(BitmapToByte(bitmap), Image_Buffer_SIZE);

            if (!Command_Set_Cusor_position(uDP_Class, IP, x, y, bitmap.Width, bitmap.Height))
            {
                return false;
            }
            for (int i = 0; i < list_bmp_byte.Count; i++)
            {
                long StatrPtr = i * Image_Buffer_SIZE / 2;
                if (!Command_Framebuffer(uDP_Class, IP, StatrPtr, list_bmp_byte[i]))
                {
                    return false;
                }
            }
            if (!Command_DrawCanvas(uDP_Class, IP))
            {
                return false;
            }
            return true;
        }
        static public bool Set_SetBitmapEx(UDP_Class uDP_Class, string IP, Bitmap bitmap, int x, int y)
        {
            int len = 0;
            int last_len = 0;
            List<byte[]> list_bmp_byte = SplitImage(BitmapToByteEx(bitmap, ref len), Image_Buffer_SIZE);

            if (!Command_Set_Cusor_position(uDP_Class, IP, x, y, bitmap.Width, bitmap.Height))
            {
                return false;
            }
            if (!Command_FontColor(uDP_Class, IP, Color.White, Color.Black))
            {
                return false;
            }
            last_len = len % (Image_Buffer_SIZE * 8);
            for (int i = 0; i < list_bmp_byte.Count; i++)
            {
                long StatrPtr = i * Image_Buffer_SIZE * 8;
                int lenth = Image_Buffer_SIZE * 8;
                if (i == list_bmp_byte.Count - 1) lenth = last_len;
                if (!Command_FramebufferEx(uDP_Class, IP, StatrPtr, list_bmp_byte[i], lenth))
                {
                    return false;
                }
            }
            if (!Command_DrawCanvas(uDP_Class, IP))
            {
                return false;
            }
            return true;
        }
        static public bool Set_DrawLine(UDP_Class uDP_Class, string IP, Point p0, Point p1, Color color)
        {
            return Set_DrawLine(uDP_Class, IP, p0.X, p0.Y, p1.X, p1.Y, color);
        }
        static public bool Set_DrawLine(UDP_Class uDP_Class, string IP, int x0 , int y0 ,int x1 , int y1 ,Color color)
        {
            return Command_DrawLine(uDP_Class, IP, x0, y0, x1, y1, color);
        }
        static public bool Set_DrawRect(UDP_Class uDP_Class, string IP, int x, int y, int width, int height, int pen_width, Color color)
        {
            return Command_DrawRect(uDP_Class, IP, x, y, width, height, pen_width, color);
        }
        static public bool Set_WS2812_Blink(UDP_Class uDP_Class, string IP, int blinkTime, Color color)
        {
            return Command_WS2812_Blink(uDP_Class, IP, blinkTime, color);
        }
        static public bool Set_Code_FrameWrite(UDP_Class uDP_Class, string IP, string Text, int x, int y, Font font, Color FontColor, Color FontBackColor)
        {
            using (Bitmap bitmap = TextToBitmap(Text, font, Color.White, Color.Black))
            {
                int len = 0;
                int last_len = 0;
                List<byte[]> list_bmp_byte = SplitImage(BitmapToByteEx(bitmap, ref len), Image_Buffer_SIZE);

                if (!Command_Set_Cusor_position(uDP_Class, IP, x, y, bitmap.Width, bitmap.Height))
                {
                    return false;
                }

                if (!Command_FontColor(uDP_Class, IP, FontColor, FontBackColor))
                {
                    return false;
                }
                last_len = len % (Image_Buffer_SIZE * 8);
                for (int i = 0; i < list_bmp_byte.Count; i++)
                {
                    long StatrPtr = i * Image_Buffer_SIZE * 8;
                    int lenth = Image_Buffer_SIZE * 8;
                    if (i == list_bmp_byte.Count - 1) lenth = last_len;
                    if (!Command_FramebufferEx(uDP_Class, IP, StatrPtr, list_bmp_byte[i], lenth))
                    {
                        return false;
                    }
                }
                if (!Command_Code_FrameWrite(uDP_Class, IP, x,y, bitmap.Width, bitmap.Height, FontColor, FontBackColor))
                {
                    return false;
                }

            }

            return true;
        }
        static public bool Set_ESP32_Restart(UDP_Class uDP_Class, string IP)
        {
            return Command_ESP_Restart(uDP_Class, IP);
        }
        static public bool Set_OTAUpdate(UDP_Class uDP_Class, string IP)
        {
            return Command_OTAUpdate(uDP_Class, IP);
        }
        static public bool Set_Setting_Page(UDP_Class uDP_Class, string IP)
        {
            return Command_SetToPage(uDP_Class, IP, 3);
        }
        static public bool Set_ToPage(UDP_Class uDP_Class, string IP, int num)
        {
            return Command_SetToPage(uDP_Class, IP, num);
        }
        static public bool Set_Main_Page(UDP_Class uDP_Class, string IP)
        {
            return Command_SetToPage(uDP_Class, IP, 10);
        }
        static public bool Set_UDP_SendTime(UDP_Class uDP_Class, string IP, int ms)
        {
            return Command_Set_UDP_SendTime(uDP_Class, IP, ms);
        }
        static public bool Set_PIN(UDP_Class uDP_Class, string IP, int PIN, bool Statu)
        {
            return Command_Set_PIN(uDP_Class, IP, PIN, Statu);
        }
        static public bool Set_ScreenPageInit(UDP_Class uDP_Class, string IP, bool enable)
        {
            return Command_Set_ScreenPageInit(uDP_Class, IP, enable);
        }
        static public bool Set_ServerConfig(UDP_Class uDP_Class, string IP, string ServerIP, int ServerPort)
        {
            return Command_Set_ServerConfig(uDP_Class, IP, ServerIP, ServerPort);
        }
        static public bool Set_JsonStringSend(UDP_Class uDP_Class, string IP)
        {
            return Command_Set_JsonStringSend(uDP_Class, IP);
        }
        static public bool Set_SendTestData(UDP_Class uDP_Class, string IP, int port)
        {
            return Command_SendTestData(uDP_Class, IP, port);
        }
        static public bool Set_WS2812_Buffer(UDP_Class uDP_Class, string IP, int start_ptr, byte[] bytes_RGB)
        {
            if (!Basic.Net.Ping(IP, 2, 150))
            {
                Console.WriteLine($"Set_WS2812_Buffer {DateTime.Now.ToDateTimeString()} : Ping Failed {IP} ");
                return false;
            }
            return Command_Set_WS2812_Buffer(uDP_Class, IP, start_ptr, bytes_RGB);
        }
        static public byte[] Get_WS2812_Buffer(UDP_Class uDP_Class, string IP , int lenth)
        {
            if (!Basic.Net.Ping(IP, 2, 150))
            {
                Console.WriteLine($"Get_WS2812_Buffer {DateTime.Now.ToDateTimeString()} : Ping Failed {IP} ");
                return new byte[lenth];
            }
            return Command_Get_WS2812_Buffer(uDP_Class, IP, lenth);
        }
        static public bool Set_GatewayConfig(UDP_Class uDP_Class, string IP, string Geteway)
        {
            return Command_Set_GetewayConfig(uDP_Class, IP, Geteway);
        }
        static public bool Set_LocalPort(UDP_Class uDP_Class, string IP, int LocalPort)
        {
            return Command_Set_LocalPort(uDP_Class, IP, LocalPort);
        }
        static public bool Set_Output(UDP_Class uDP_Class, string IP, int value)
        {
            if (!Basic.Net.Ping(IP, 2, 150))
            {
                Console.WriteLine($"Set_Output {DateTime.Now.ToDateTimeString()} : Ping Failed {IP} ");
                return false;
            }
            return Command_Set_Output(uDP_Class, IP, value);
        }
        static public bool Set_OutputPIN(UDP_Class uDP_Class, string IP, int PIN_Num, bool value)
        {
            if (!Basic.Net.Ping(IP, 2, 150))
            {
                Console.WriteLine($"Set_OutputPIN {DateTime.Now.ToDateTimeString()} : Ping Failed {IP} ");
                return false;
            }
            return Command_Set_OutputPIN(uDP_Class, IP, PIN_Num, value);
        }
        static public bool Set_OutputTrigger(UDP_Class uDP_Class, string IP, int value)
        {
            if (!Basic.Net.Ping(IP, 2, 150))
            {
                Console.WriteLine($"Set_OutputTrigger {DateTime.Now.ToDateTimeString()} : Ping Failed {IP} ");
                return false;
            }
            return Command_Set_OutputTrigger(uDP_Class, IP, value);
        }
        static public bool Set_OutputPINTrigger(UDP_Class uDP_Class, string IP, int PIN_Num, bool value)
        {
            if (!Basic.Net.Ping(IP, 2, 150))
            {
                Console.WriteLine($"Set_OutputPINTrigger {DateTime.Now.ToDateTimeString()} : Ping Failed {IP} ");
                return false;
            }
            return Command_Set_OutputPINTrigger(uDP_Class, IP, PIN_Num, value);
        }
        static public bool Set_ADCMotorTrigger(UDP_Class uDP_Class, string IP, int PIN_Num, int time_ms)
        {
            if (!Basic.Net.Ping(IP, 2, 150))
            {
                Console.WriteLine($"Set_ADCMotorTrigger {DateTime.Now.ToDateTimeString()} : Ping Failed {IP} {DateTime.Now.ToDateTimeString()}");
                return false;
            }
            return Command_Set_ADCMotorTrigger(uDP_Class, IP, PIN_Num, time_ms);
        }
        static public bool Set_Input_dir(UDP_Class uDP_Class, string IP, int PIN_Num, bool value)
        {
            return Command_Set_Input_dir(uDP_Class, IP, PIN_Num, value);
        }
        static public bool Set_Output_dir(UDP_Class uDP_Class, string IP, int PIN_Num, bool value)
        {
            return Command_Set_Output_dir(uDP_Class, IP, PIN_Num, value);
        }
        static public bool Set_RFID_Enable(UDP_Class uDP_Class, string IP, int index, bool value)
        {
            return Command_Set_RFID_Enable(uDP_Class, IP, index, value);
        }
        static public bool Set_RFID_Beep(UDP_Class uDP_Class, string IP, int station)
        {
            return Command_Set_RFID_Beep(uDP_Class, IP, station);
        }
        static public bool Get_LEDSetting(UDP_Class uDP_Class, string IP, ref int[] InputPIN, ref int[] OutputPIN)
        {
            return Command_Get_LEDSetting(uDP_Class, IP, ref InputPIN, ref OutputPIN);
        }
        static public bool Set_LEDSetting(UDP_Class uDP_Class, string IP, int[] InputPIN, int[] OutputPIN)
        {
            return Command_Set_LEDSetting(uDP_Class, IP, InputPIN, OutputPIN);
        }
        static public bool GetKeyBoardValue(UDP_Class uDP_Class, string IP, ref int value)
        {
            return Command_GetKeyBoardValue(uDP_Class, IP, ref value);
        }
        static public bool Set_Framebuffer(UDP_Class uDP_Class, string IP, long start_ptr, byte[] value)
        {
            return Command_Framebuffer(uDP_Class, IP, start_ptr, value);
        }
        static public bool Set_Jpegbuffer(UDP_Class uDP_Class, string IP, byte[] value)
        {
            return Command_Set_Jpegbuffer(uDP_Class, IP, value);
        }
        static public bool Get_IO(UDP_Class uDP_Class, string IP, out int input, out int output)
        {
            input = 0;
            output = 0;
            if (!Basic.Net.Ping(IP, 2, 150))
            {
                Console.WriteLine($"Get_IO {DateTime.Now.ToDateTimeString()} : Ping Failed {IP} ");
                return false;
            }
            return Command_Get_IO(uDP_Class, IP, out input, out output);
        }
        static public bool Set_BlinkEnable(UDP_Class uDP_Class, string IP, int PIN_Num, bool value, int blinklTime)
        {
            return Command_Set_BlinkEnable(uDP_Class, IP, PIN_Num, value, blinklTime);
        }
        static public bool Set_WS2812B_breathing(UDP_Class uDP_Class, string IP, byte WS2812B_breathing_onAddVal, byte WS2812B_breathing_offSubVal, Color color)
        {
            return Command_Set_WS2812B_breathing(uDP_Class, IP, WS2812B_breathing_onAddVal, WS2812B_breathing_offSubVal, color);
        }

        static public int EPD_Get_LaserDistance(UDP_Class uDP_Class, string IP)
        {
            return Command_EPD_Get_LaserDistance(uDP_Class, IP);
        }
        static public bool EPD_Set_Sleep(UDP_Class uDP_Class, string IP)
        {
            return Command_EPD_Set_Sleep(uDP_Class, IP);
        }
        static public bool EPD_Set_WakeUp(UDP_Class uDP_Class, string IP)
        {
            return Command_EPD_Set_WakeUp(uDP_Class, IP);
        }
        static public bool EPD_DrawFrame_RW(UDP_Class uDP_Class, string IP)
        {
            return Command_EPD_DrawFrame_RW(uDP_Class, IP);
        }
        static public bool EPD_DrawFrame_BW(UDP_Class uDP_Class, string IP)
        {
            return Command_EPD_DrawFrame_BW(uDP_Class, IP);
        }
        static public bool EPD_RefreshCanvas(UDP_Class uDP_Class, string IP)
        {
            return Command_EPD_RefreshCanvas(uDP_Class, IP);
        }
        static public bool EPD_Send_Framebuffer(UDP_Class uDP_Class, string IP, long start_ptr, byte[] value)
        {
            return Command_EPD_Send_Framebuffer(uDP_Class, IP, start_ptr, value);
        }
        static public bool EPD_BW_Command(UDP_Class uDP_Class, string IP)
        {
            return Command_EPD_BW_Command(uDP_Class, IP);
        }
        static public bool EPD_RW_Command(UDP_Class uDP_Class, string IP)
        {
            return Command_EPD_RW_Command(uDP_Class, IP);
        }
        static public bool EPD_SendSPI(UDP_Class uDP_Class, string IP, long start_ptr, byte[] value)
        {
            return Command_EPD_SendSPI(uDP_Class, IP, start_ptr, value);
        }

        static public bool EPD_730_DrawImage(UDP_Class uDP_Class, string IP, Bitmap bmp)
        {
            if (!Basic.Net.Ping(IP, 2, 150))
            {
                Console.WriteLine($"EPD730 DrawImage start {DateTime.Now.ToDateTimeString()} : Ping Failed {IP} ");
                return false;
            }
            int EPD583_frameDIV = 0;
            if (Chip_Type == ChipType.ESP32)
            {
                EPD583_frameDIV = 20;
            }
            if (Chip_Type == ChipType.BW16)
            {
                EPD583_frameDIV = 40;
            }
            bool flag_OK;
            int width = bmp.Width;
            int height = bmp.Height;
            byte[] bytes = new byte[400 * height];
            BitmapToByte(bmp, ref bytes, EPD_Type.EPD730F);
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);
            flag_OK = EPD_DrawImageSPI(uDP_Class, IP, bytes, (400 * height) / EPD583_frameDIV);
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : EPD 730 DrawImage {string.Format(flag_OK ? "sucess" : "failed")}!   Time : {myTimer.GetTickTime().ToString("0.000")} ms");
            return flag_OK;
        }
        static public bool EPD_583_DrawImage(UDP_Class uDP_Class, string IP, Bitmap bmp)
        {
            if (!Basic.Net.Ping(IP, 2, 150))
            {
                Console.WriteLine($"EPD583 DrawImage start {DateTime.Now.ToDateTimeString()} : Ping Failed {IP} ");
                return false;
            }
            int EPD583_frameDIV = 0;
            if (Chip_Type == ChipType.ESP32)
            {
                EPD583_frameDIV = 1;
            }
            if (Chip_Type == ChipType.BW16)
            {
                EPD583_frameDIV = 4;
            }
            bool flag_OK;
            int width = bmp.Width;
            int height = bmp.Height;
            byte[] bytes_BW = new byte[81 * height];
            byte[] bytes_RW = new byte[81 * height];
            BitmapToByte(bmp, ref bytes_BW, ref bytes_RW , EPD_Type.EPD583);
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);
            flag_OK = EPD_DrawImage(uDP_Class, IP, bytes_BW, bytes_RW, 81 * height / EPD583_frameDIV);
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : EPD 583 DrawImage {string.Format(flag_OK ? "sucess" : "failed")}!   Time : {myTimer.GetTickTime().ToString("0.000")} ms");
            return flag_OK;
        }
        static public bool EPD_213_DrawImage(UDP_Class uDP_Class, string IP, Bitmap bmp)
        {
            if (!Basic.Net.Ping(IP, 2, 150))
            {
                Console.WriteLine($"EPD213 DrawImage start {DateTime.Now.ToDateTimeString()} : Ping Failed {IP}");
                return false;
            }
            Console.WriteLine($"EPD213 DrawImage start {DateTime.Now.ToDateTimeString()} ");
            using (Bitmap _bmp = bmp.DeepClone())
            {
                int EPD213_frameDIV = 0;
                if (Chip_Type == ChipType.ESP32)
                {
                    EPD213_frameDIV = 1;
                }
                if (Chip_Type == ChipType.BW16)
                {
                    EPD213_frameDIV = 1;
                }

                bool flag_OK;
                int width = bmp.Width;
                int height = bmp.Height;
                height = 128;
                _bmp.RotateFlip(RotateFlipType.Rotate90FlipXY);
                byte[] bytes_BW = new byte[(height / 8) * width];
                byte[] bytes_RW = new byte[(height / 8) * width];
                BitmapToByteEPD213(_bmp, ref bytes_BW, ref bytes_RW);
                MyTimer myTimer = new MyTimer();
                myTimer.StartTickTime(50000);
                flag_OK = EPD_DrawImage(uDP_Class, IP, bytes_BW, bytes_RW, (height / 8) * width / EPD213_frameDIV);
                if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : EPD 213 DrawImage {string.Format(flag_OK ? "sucess" : "failed")}!   Time : {myTimer.GetTickTime().ToString("0.000")} ms");
                return flag_OK;
            }


        }

        static public bool EPD_266_DrawImage(UDP_Class uDP_Class, string IP, Bitmap bmp)
        {
            if (!Basic.Net.Ping(IP, 2, 150))
            {
                Console.WriteLine($"EPD266 DrawImage start {DateTime.Now.ToDateTimeString()} : Ping Failed {IP}");
                return false;
            }
            Console.WriteLine($"EPD266 DrawImage start {DateTime.Now.ToDateTimeString()} ");
            using (Bitmap _bmp = bmp.DeepClone())
            {
                int EPD266_frameDIV = 0;
                if (Chip_Type == ChipType.ESP32)
                {
                    EPD266_frameDIV = 4;
                }
                if (Chip_Type == ChipType.BW16)
                {
                    EPD266_frameDIV = 4;
                }

                bool flag_OK;
                int width = bmp.Width;
                int height = bmp.Height;
                _bmp.RotateFlip(RotateFlipType.Rotate90FlipXY);
                byte[] bytes_BW = new byte[(height / 8) * width];
                byte[] bytes_RW = new byte[(height / 8) * width];
                BitmapToByte(_bmp, ref bytes_BW, ref bytes_RW, EPD_Type.EPD266_B);
                MyTimer myTimer = new MyTimer();
                myTimer.StartTickTime(50000);
                flag_OK =  EPD_DrawImage(uDP_Class, IP, bytes_BW, bytes_RW, (height / 8) * width / EPD266_frameDIV);
                if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : EPD 266 DrawImage {string.Format(flag_OK ? "sucess" : "failed")}!   Time : {myTimer.GetTickTime().ToString("0.000")} ms");
                return flag_OK;
            }
    

        }
        static public bool EPD_290_DrawImage(UDP_Class uDP_Class, string IP, Bitmap bmp)
        {
            if (!Basic.Net.Ping(IP, 2, 150))
            {
                Console.WriteLine($"EPD290 DrawImage start {DateTime.Now.ToDateTimeString()} : Ping Failed {IP}");
                return false;
            }
            Console.WriteLine($"EPD290 DrawImage start {DateTime.Now.ToDateTimeString()} ");
            using (Bitmap _bmp = bmp.DeepClone())
            {
                int EPD290_frameDIV = 0;
                if (Chip_Type == ChipType.ESP32)
                {
                    EPD290_frameDIV = 1;
                }
                if (Chip_Type == ChipType.BW16)
                {
                    EPD290_frameDIV = 4;
                }

                bool flag_OK;
                int width = bmp.Width;
                int height = bmp.Height;
                _bmp.RotateFlip(RotateFlipType.Rotate90FlipXY);
                byte[] bytes_BW = new byte[(height / 8) * width];
                byte[] bytes_RW = new byte[(height / 8) * width];
                BitmapToByte(_bmp, ref bytes_BW, ref bytes_RW, EPD_Type.EPD290_V2);
                MyTimer myTimer = new MyTimer();
                myTimer.StartTickTime(50000);
                flag_OK = EPD_DrawImage(uDP_Class, IP, bytes_BW, bytes_RW, (height / 8) * width / EPD290_frameDIV);
                if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : EPD 290 DrawImage {string.Format(flag_OK ? "sucess" : "failed")}!   Time : {myTimer.GetTickTime().ToString("0.000")} ms");
                return flag_OK;
            }
        }

        static public bool EPD_1020_DrawImage(UDP_Class uDP_Class, string IP, Bitmap bmp)
        {
            if (!Basic.Net.Ping(IP, 2, 150))
            {
                Console.WriteLine($"EPD1020 DrawImage start {DateTime.Now.ToDateTimeString()} : Ping Failed {IP}");
                return false;
            }
            using (Bitmap _bmp = bmp.DeepClone())
            {
                int frameDIV = 10;

                bool flag_OK;
                int width = bmp.Width;
                int height = bmp.Height;
                byte[] bytes_BW = new byte[(width / 8) * height];
                byte[] bytes_RW = new byte[(width / 8) * height];
                if (width == 960 && height == 640) _bmp.RotateFlip(RotateFlipType.Rotate180FlipX);
                if (width == 640 && height == 960) _bmp.RotateFlip(RotateFlipType.Rotate270FlipNone);
                if (width == 640 && height == 960) _bmp.RotateFlip(RotateFlipType.Rotate180FlipX);
                BitmapToByte(_bmp, ref bytes_BW, ref bytes_RW, EPD_Type.EPD1020);
                MyTimer myTimer = new MyTimer();
                myTimer.StartTickTime(50000);
                flag_OK = EPD_DrawImageEx0(uDP_Class, IP, bytes_BW, bytes_RW, (width / 8) * height / frameDIV);
                if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : EPD 1020 DrawImage {string.Format(flag_OK ? "sucess" : "failed")}!   Time : {myTimer.GetTickTime().ToString("0.000")} ms");
                return flag_OK;
            }


        }
        static public bool EPD_420_DrawImage(UDP_Class uDP_Class, string IP, Bitmap bmp)
        {
            using (Bitmap _bmp = bmp.DeepClone())
            {
                MyTimer myTimer = new MyTimer();
                myTimer.StartTickTime(50000);
                int frameDIV = 12;
                bool flag_OK;
                int width = bmp.Width;
                int height = bmp.Height;
                byte[] bytes_BW = new byte[(width / 8) * height];
                byte[] bytes_RW = new byte[(width / 8) * height];
                //_bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
                BitmapToByte(_bmp, ref bytes_BW, ref bytes_RW, EPD_Type.EPD420);
  
                flag_OK = EPD_DrawImage(uDP_Class, IP, bytes_BW, bytes_RW, (width / 8) * height / frameDIV);
                if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : EPD 420 DrawImage {string.Format(flag_OK ? "sucess" : "failed")}!   Time : {myTimer.GetTickTime().ToString("0.000")} ms");
                return flag_OK;
            }


        }

        static public bool LCD_144_DrawImage(UDP_Class uDP_Class, string IP, Bitmap bmp)
        {
            using (Bitmap _bmp = bmp.DeepClone())
            {
                MyTimer myTimer = new MyTimer();
                myTimer.StartTickTime(50000);
                int frameDIV = 50;
                bool flag_OK = false;
                List<byte> bytes = LCD144_BitmapToByte(bmp);
                flag_OK = LCD_DrawImage(uDP_Class, IP, bytes.ToArray(), bytes.Count / frameDIV);
                if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : LCD_144_DrawImage {string.Format(flag_OK ? "sucess" : "failed")}!   Time : {myTimer.GetTickTime().ToString("0.000")} ms");

                return flag_OK;
            }
        }
        static public bool LCD_144_DrawImageEx(UDP_Class uDP_Class, string IP, Bitmap bmp, Color ForeColoe, Color BackColor)
        {
            using (Bitmap _bmp = bmp.DeepClone())
            {
                MyTimer myTimer = new MyTimer();
                myTimer.StartTickTime(50000);
                int frameDIV = 4;
                bool flag_OK = false;
                int len = 0;
                List<byte> bytes = BitmapToByteEx(bmp, ref len);
                flag_OK = LCD_DrawImageEx(uDP_Class, IP, bytes.ToArray(), bytes.Count / frameDIV, ForeColoe, BackColor);
                if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : LCD_144_DrawImage {string.Format(flag_OK ? "sucess" : "failed")}!   Time : {myTimer.GetTickTime().ToString("0.000")} ms");

                return flag_OK;
            }
        }
        static public bool LCD_144_DrawImageEx(UDP_Class uDP_Class, string IP, Color BackColor)
        {
            using (Bitmap _bmp = Get_LCD_144_bmp("", new Font("標楷體", 14), Color.Black, Color.Black))
            {
                MyTimer myTimer = new MyTimer();
                myTimer.StartTickTime(50000);
                int frameDIV = 4;
                bool flag_OK = false;
                int len = 0;
                List<byte> bytes = BitmapToByteEx(_bmp, ref len);
                flag_OK = LCD_DrawImageEx(uDP_Class, IP, bytes.ToArray(), bytes.Count / frameDIV, BackColor, BackColor);
                if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : LCD_144_DrawImage {string.Format(flag_OK ? "sucess" : "failed")}!   Time : {myTimer.GetTickTime().ToString("0.000")} ms");

                return flag_OK;
            }
        }

        static public Bitmap Get_LCD_144_bmp(string text, Font font, Color ForeColor, Color BackColor)
        {
            try
            {
                Bitmap bitmap_Canvas = new Bitmap((int)(240), (int)(135));
                using (Graphics g = Graphics.FromImage(bitmap_Canvas))
                {
                    Storage.VlaueClass vlaueClass;
                    g.SmoothingMode = SmoothingMode.HighQuality; //使繪圖質量最高，即消除鋸齒
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
                    g.FillRectangle(new SolidBrush(BackColor), 0, 0, bitmap_Canvas.Width, bitmap_Canvas.Height);
                    DrawingClass.Draw.文字中心繪製(text, new Rectangle(0, 0, (int)(bitmap_Canvas.Width), (int)(bitmap_Canvas.Height)), font, ForeColor, g);
                }
                return bitmap_Canvas;
            }
            catch
            {
                return null;
            }
        }
        static public Bitmap Get_Storage_bmp(Storage storage, string[] valuenames ,double canvasScale)
        {
            int panelWidth = storage.PanelSize.Width;
            int panelHeight = storage.PanelSize.Height;
            storage.IP_Visable = false;
            storage.MinPackage_Visable = false;
            storage.Min_Package_Num_Visable = false;
            storage.Max_Inventory_Visable = false;
            storage.Port_Visable = false;
            storage.Label_Visable = false;
            Storage.VlaueClass vlaueClass;
            Bitmap bitmap = new Bitmap((int)(panelWidth * canvasScale), (int)(panelHeight * canvasScale));
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.HighQuality; //使繪圖質量最高，即消除鋸齒
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
                g.FillRectangle(new SolidBrush(storage.BackColor), 0, 0, (int)(panelWidth * canvasScale), (int)(panelHeight * canvasScale));

                for (int i = 0; i < valuenames.Length; i++)
                {
                    vlaueClass = storage.GetValue(Storage.GetValueName(valuenames[i]));
                    vlaueClass.Position.X = (int)(vlaueClass.Position.X * canvasScale);
                    vlaueClass.Position.Y = (int)(vlaueClass.Position.Y * canvasScale);
                    if (vlaueClass.Visable)
                    {
                        using (Bitmap bitmap_temp = storage.Get_EPD_Bitmap(Storage.GetValueName(valuenames[i]), canvasScale))
                        {
                            if (bitmap_temp == null) continue;
                            g.DrawImage(bitmap_temp, vlaueClass.Position);
                        }
                    }
                }
                string[] ip_array = storage.IP.Split('.');
                SizeF size_IP = new SizeF();
                if (ip_array.Length == 4)
                {
                    string ip = ip_array[2] + "." + ip_array[3];
                    size_IP = TextRenderer.MeasureText(ip, new Font("微軟正黑體", 8, FontStyle.Bold));
                    g.DrawString(ip, new Font("微軟正黑體", 8, FontStyle.Bold), new SolidBrush((Color)storage.GetValue(Storage.ValueName.IP, Storage.ValueType.ForeColor)), (float)((panelWidth - size_IP.Width) * canvasScale), (float)((panelHeight - size_IP.Height) * canvasScale));
                }

            }
            return bitmap;

        }


        static public bool LCD_DrawImage(UDP_Class uDP_Class, string IP, byte[] datas, int Split_DataSize)
        {
            int Width_Size = Split_DataSize;
            int NumOfArray = datas.Length / (Split_DataSize);
            List<byte[]> list_data = new List<byte[]>();
            for (int i = 0; i < NumOfArray; i++)
            {
                byte[] Array_temp = new byte[Width_Size];
                for (int k = 0; k < Width_Size; k++)
                {
                    Array_temp[k] = datas[i * Width_Size + k];
                }
                list_data.Add(Array_temp);
            }
            for (int i = 0; i < NumOfArray; i++)
            {
                if (!Set_Framebuffer(uDP_Class, IP, (i * Width_Size) / 2, list_data[i]))
                {
                    return false;
                }
            }
            if (!Command_DrawCanvas(uDP_Class, IP))
            {
                return false;
            }
            return true;
        }
        static public bool LCD_DrawImageEx(UDP_Class uDP_Class, string IP, byte[] datas, int Split_DataSize, Color ForeColoe, Color BackColor)
        {
            MyTimerBasic myTimerBasic = new MyTimerBasic(40000);
            int Width_Size = Split_DataSize;
            int NumOfArray = datas.Length / (Split_DataSize);
            List<byte[]> list_data = new List<byte[]>();
            for (int i = 0; i < NumOfArray; i++)
            {
                byte[] Array_temp = new byte[Width_Size];
                for (int k = 0; k < Width_Size; k++)
                {
                    Array_temp[k] = datas[i * Width_Size + k];
                }
                list_data.Add(Array_temp);
            }
            if (!Command_LCD114_FontColor(uDP_Class, IP, ForeColoe, BackColor))
            {
                return false;
            }
            //Console.WriteLine($"[LCD_DrawImageEx][LCD114_FontColor]{myTimerBasic}");
            for (int i = 0; i < NumOfArray; i++)
            {
                if (!Command_FramebufferEx(uDP_Class, IP, (i * Width_Size * 8) , list_data[i] , list_data[i].Length * 8))
                {
                    return false;
                }
                //Console.WriteLine($"[LCD_DrawImageEx][FramebufferEx-{i} ,length:{list_data[i].Length}]{myTimerBasic}");
            }
            if (!Command_DrawCanvas(uDP_Class, IP))
            {
                return false;
            }
            //Console.WriteLine($"[LCD_DrawImageEx][Command_DrawCanvas]{myTimerBasic}");
            return true;
        }



        static public bool EPD_DrawImage(UDP_Class uDP_Class, string IP, byte[] BW_data, byte[] RW_data, int Split_DataSize)
        {
            int Width_Size = Split_DataSize;
            int NumOfArray = BW_data.Length / (Split_DataSize);
            List<byte[]> list_BW_data = new List<byte[]>();
            for (int i = 0; i < NumOfArray; i++)
            {
                byte[] Array_temp = new byte[Width_Size];
                for (int k = 0; k < Width_Size; k++)
                {
                    Array_temp[k] = BW_data[i * Width_Size + k];
                }
                list_BW_data.Add(Array_temp);
            }
            List<byte[]> list_RW_data = new List<byte[]>();
            for (int i = 0; i < NumOfArray; i++)
            {
                byte[] Array_temp = new byte[Width_Size];
                for (int k = 0; k < Width_Size; k++)
                {
                    Array_temp[k] = RW_data[i * Width_Size + k];
                }
                list_RW_data.Add(Array_temp);
            }

            if (!EPD_Set_WakeUp(uDP_Class, IP))
            {
                return false;
            }

            for (int i = 0; i < NumOfArray; i++)
            {
                if (!EPD_Send_Framebuffer(uDP_Class, IP, i * Width_Size, list_BW_data[i]))
                {
                    return false;
                }
            }
            if (!EPD_DrawFrame_BW(uDP_Class, IP))
            {
                return false;
            }
            for (int i = 0; i < NumOfArray; i++)
            {
                if (!EPD_Send_Framebuffer(uDP_Class, IP, i * Width_Size, list_RW_data[i]))
                {
                    return false;
                }
            }
            if (!EPD_DrawFrame_RW(uDP_Class, IP))
            {
                return false;
            }

            if (!EPD_RefreshCanvas(uDP_Class, IP))
            {
                return false;
            }

            return true;

        }
        static public bool EPD_DrawImageSPI(UDP_Class uDP_Class, string IP, byte[] datas, int Split_DataSize)
        {
            int Width_Size = Split_DataSize;
            int NumOfArray = datas.Length / (Split_DataSize);
            List<byte[]> list_datas = new List<byte[]>();
            for (int i = 0; i < NumOfArray; i++)
            {
                byte[] Array_temp = new byte[Width_Size];
                for (int k = 0; k < Width_Size; k++)
                {
                    Array_temp[k] = datas[i * Width_Size + k];
                }
                list_datas.Add(Array_temp);
            }


            if (!EPD_Set_WakeUp(uDP_Class, IP))
            {
                return false;
            }
            for (int i = 0; i < NumOfArray / 2; i++)
            {
                if (!EPD_Send_Framebuffer(uDP_Class, IP, i * Width_Size, list_datas[i]))
                {
                    return false;
                }
            }
            if (!EPD_DrawFrame_RW(uDP_Class, IP))
            {
                return false;
            }
            for (int i = NumOfArray / 2; i < NumOfArray; i++)
            {
                if (!EPD_Send_Framebuffer(uDP_Class, IP, (i - NumOfArray / 2) * Width_Size, list_datas[i]))
                {
                    return false;
                }
            }
            if (!EPD_DrawFrame_BW(uDP_Class, IP))
            {
                return false;
            }
            //for (int i = 0; i < NumOfArray; i++)
            //{
            //    if (!EPD_SendSPI(uDP_Class, IP, i * Width_Size, list_datas[i]))
            //    {
            //        return false;
            //    }
            //}

            if (!EPD_RefreshCanvas(uDP_Class, IP))
            {
                return false;
            }

            return true;

        }
        static public bool EPD_DrawImageEx0(UDP_Class uDP_Class, string IP, byte[] BW_data, byte[] RW_data, int Split_DataSize)
        {
            int Width_Size = Split_DataSize;
            int NumOfArray = BW_data.Length / (Split_DataSize);
            List<byte[]> list_BW_data = new List<byte[]>();
            for (int i = 0; i < NumOfArray; i++)
            {
                byte[] Array_temp = new byte[Width_Size];
                for (int k = 0; k < Width_Size; k++)
                {
                    Array_temp[k] = BW_data[i * Width_Size + k];
                }
                list_BW_data.Add(Array_temp);
            }
            List<byte[]> list_RW_data = new List<byte[]>();
            for (int i = 0; i < NumOfArray; i++)
            {
                byte[] Array_temp = new byte[Width_Size];
                for (int k = 0; k < Width_Size; k++)
                {
                    Array_temp[k] = RW_data[i * Width_Size + k];
                }
                list_RW_data.Add(Array_temp);
            }

            if (!EPD_Set_WakeUp(uDP_Class, IP))
            {
                return false;
            }
            if (!EPD_BW_Command(uDP_Class, IP))
            {
                return false;
            }
            for (int i = 0; i < NumOfArray; i++)
            {
                if (!EPD_SendSPI(uDP_Class, IP, i * Width_Size, list_BW_data[i]))
                {
                    return false;
                }
            }
            if (!EPD_RW_Command(uDP_Class, IP))
            {
                return false;
            }
            for (int i = 0; i < NumOfArray; i++)
            {
                if (!EPD_SendSPI(uDP_Class, IP, i * Width_Size, list_RW_data[i]))
                {
                    return false;
                }
            }
   

            if (!EPD_RefreshCanvas(uDP_Class, IP))
            {
                return false;
            }

            return true;

        }
        static private bool Command_Set_Cusor_position(UDP_Class uDP_Class, string IP, int x, int y, int width, int height)
        {
            bool flag_OK = true;
            byte checksum = 0;
            List<byte> list_byte = new List<byte>();
            list_byte.Add(2);
            list_byte.Add((byte)(UDP_Command.Cusor_posituin));
            list_byte.Add((byte)(x));
            list_byte.Add((byte)((x >> 8)));
            list_byte.Add((byte)(y));
            list_byte.Add((byte)(y >> 8));
            list_byte.Add((byte)(width));
            list_byte.Add((byte)(width >> 8));
            list_byte.Add((byte)(height));
            list_byte.Add((byte)(height >> 8));
            list_byte.Add(3);
            for (int i = 0; i < list_byte.Count; i++)
            {
                checksum += list_byte[i];
            }
            MyTimer MyTimer_UART_TimeOut = new MyTimer();
            int retry = 0;
            int cnt = 0;
            while (true)
            {
                if (cnt == 0)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    uDP_Class.Set_ReadLineClearByIP(IP);
                    uDP_Class.WriteByte(list_byte.ToArray(), IP);
                    MyTimer_UART_TimeOut.TickStop();
                    MyTimer_UART_TimeOut.StartTickTime(UDP_TimeOut);
                    cnt++;
                }
                else if (cnt == 1)
                {
                    if (retry >= UDP_RetryNum)
                    {
                     
                        flag_OK = false;
                        break;
                    }
                    if (MyTimer_UART_TimeOut.IsTimeOut())
                    {
                        retry++;
                        cnt = 0;
                    }
                    string UDP_RX = uDP_Class.Get_ReadLineByIP(IP);
                    if (UDP_RX != "")
                    {
                        if (UDP_RX == checksum.ToString("000"))
                        {
                            flag_OK = true;
                            break;
                        }
                        else
                        {
                            cnt = 0;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : Set Cusor position {string.Format(flag_OK ? "sucess" : "failed")}!");
            return flag_OK;
        }
        static private bool Command_ClearCanvas(UDP_Class uDP_Class, string IP, Color BackColor)
        {
            bool flag_OK = true;
            byte checksum = 0;
            int temp;
            int temp_L;
            int temp_H;
            int R = BackColor.R;
            int G = BackColor.G;
            int B = BackColor.B;
            R = R >> 3;
            G = G >> 2;
            B = B >> 3;
            temp = (R << 11) | (G << 5) | B;
            temp_L = temp & 0x000000FF;
            temp_H = temp >> 8;

            List<byte> list_byte = new List<byte>();
            list_byte.Add(2);
            list_byte.Add((byte)(UDP_Command.ClearCanvas));
            list_byte.Add((byte)(temp_L));
            list_byte.Add((byte)(temp_H));
            list_byte.Add(3);
            for (int i = 0; i < list_byte.Count; i++)
            {
                checksum += list_byte[i];
            }
            MyTimer MyTimer_UART_TimeOut = new MyTimer();
            int retry = 0;
            int cnt = 0;
            while (true)
            {
                if (cnt == 0)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    uDP_Class.Set_ReadLineClearByIP(IP);
                    uDP_Class.WriteByte(list_byte.ToArray(),IP);
                    MyTimer_UART_TimeOut.TickStop();
                    MyTimer_UART_TimeOut.StartTickTime(UDP_TimeOut);
                    cnt++;
                }
                else if (cnt == 1)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    if (MyTimer_UART_TimeOut.IsTimeOut())
                    {
                        retry++;
                        cnt = 0;
                    }
                    string UDP_RX = uDP_Class.Get_ReadLineByIP(IP);
                    if (UDP_RX != "")
                    {
                        if (UDP_RX == checksum.ToString("000"))
                        {
                            flag_OK = true;
                            break;
                        }
                        else
                        {
                            cnt = 0;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : ClearCanvas {string.Format(flag_OK ? "sucess" : "failed")}!");
            return flag_OK;
        }
        static private bool Command_Framebuffer(UDP_Class uDP_Class, string IP, long start_ptr, byte[] value)
        {
            bool flag_OK = true;
            byte checksum = 0;
            List<byte> list_byte = new List<byte>();
            list_byte.Add(2);
            list_byte.Add((byte)UDP_Command.Framebuffer);
            list_byte.Add((byte)start_ptr);
            list_byte.Add((byte)(start_ptr >> 8));
            list_byte.Add((byte)(start_ptr >> 16));
            list_byte.Add((byte)(start_ptr >> 24));

            for (int i = 0; i < value.Length; i++)
            {
                list_byte.Add(value[i]);
            }
            list_byte.Add(3);
            for (int i = 0; i < list_byte.Count; i++)
            {
                checksum += list_byte[i];
            }
            MyTimer MyTimer_UART_TimeOut = new MyTimer();
            int retry = 0;
            int cnt = 0;
            while (true)
            {
                if (cnt == 0)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    uDP_Class.Set_ReadLineClearByIP(IP);
                    uDP_Class.WriteByte(list_byte.ToArray(),IP);
  
                    MyTimer_UART_TimeOut.TickStop();
                    MyTimer_UART_TimeOut.StartTickTime(UDP_TimeOut);
                    cnt++;
                }
                else if (cnt == 1)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    if (MyTimer_UART_TimeOut.IsTimeOut())
                    {
                        retry++;
                        cnt = 0;
                    }
                    string UDP_RX = uDP_Class.Get_ReadLineByIP(IP);
                   
                    if (UDP_RX != "")
                    {
                        if (UDP_RX == "RETRY")
                        {
                            retry++;
                            cnt = 0;
                            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : Framebuffer recieve 'RETRY'!");
                        }
                        else if (UDP_RX == checksum.ToString("000"))
                        {
                            flag_OK = true;
                            break;
                        }
                        else
                        {
                            cnt = 0;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : Framebuffer {string.Format(flag_OK ? "sucess" : "failed")}!");
            return flag_OK;
        }
        static private bool Command_Set_Jpegbuffer(UDP_Class uDP_Class, string IP, byte[] value)
        {
            bool flag_OK = true;
            byte checksum = 0;
            List<byte> list_byte = new List<byte>();
            list_byte.Add(2);
            list_byte.Add((byte)UDP_Command.Set_Jpegbuffer);


            for (int i = 0; i < value.Length; i++)
            {
                list_byte.Add(value[i]);
            }
            list_byte.Add(3);
            for (int i = 0; i < list_byte.Count; i++)
            {
                checksum += list_byte[i];
            }
            MyTimer MyTimer_UART_TimeOut = new MyTimer();
            int retry = 0;
            int cnt = 0;
            while (true)
            {
                if (cnt == 0)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    uDP_Class.Set_ReadLineClearByIP(IP);
                    uDP_Class.WriteByte(list_byte.ToArray(), IP);

                    MyTimer_UART_TimeOut.TickStop();
                    MyTimer_UART_TimeOut.StartTickTime(3000);
                    cnt++;
                }
                else if (cnt == 1)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    if (MyTimer_UART_TimeOut.IsTimeOut())
                    {
                        retry++;
                        cnt = 0;
                    }
                    string UDP_RX = uDP_Class.Get_ReadLineByIP(IP);

                    if (UDP_RX != "")
                    {
                        if (UDP_RX == "RETRY")
                        {
                            retry++;
                            cnt = 0;
                            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : Jpegbuffer recieve 'RETRY'!");
                        }
                        else if (UDP_RX == checksum.ToString("000"))
                        {
                            flag_OK = true;
                            break;
                        }
                        else
                        {
                            cnt = 0;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : Jpegbuffer {string.Format(flag_OK ? "sucess" : "failed")}!");
            return flag_OK;
        }
        
        static private bool Command_DrawCanvas(UDP_Class uDP_Class, string IP)
        {
            bool flag_OK = true;
            byte checksum = 0;
            List<byte> list_byte = new List<byte>();
            list_byte.Add(2);
            list_byte.Add((byte)UDP_Command.DrawCanvas);
            list_byte.Add(3);
            for (int i = 0; i < list_byte.Count; i++)
            {
                checksum += list_byte[i];
            }
            MyTimer MyTimer_UART_TimeOut = new MyTimer();
            int retry = 0;
            int cnt = 0;
            while (true)
            {
                if (cnt == 0)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    uDP_Class.Set_ReadLineClearByIP(IP);
                    uDP_Class.WriteByte(list_byte.ToArray(), IP);
                    MyTimer_UART_TimeOut.TickStop();
                    MyTimer_UART_TimeOut.StartTickTime(UDP_TimeOut);
                    cnt++;
                }
                else if (cnt == 1)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    if (MyTimer_UART_TimeOut.IsTimeOut())
                    {
                        retry++;
                        cnt = 0;
                    }
                    string UDP_RX = uDP_Class.Get_ReadLineByIP(IP);

                    if (UDP_RX != "")
                    {
                        if (UDP_RX == checksum.ToString("000"))
                        {
                            flag_OK = true;
                            break;
                        }
                        else
                        {
                            cnt = 0;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : DrawCanvas {string.Format(flag_OK ? "sucess" : "failed")}!");
            return flag_OK;
        }
        static private bool Command_DrawCanvasEx(UDP_Class uDP_Class, string IP, int x, int y, int width, int height)
        {
            bool flag_OK = true;
            byte checksum = 0;
            List<byte> list_byte = new List<byte>();
            list_byte.Add(2);
            list_byte.Add((byte)UDP_Command.DrawCanvasEx);
            list_byte.Add((byte)(x));
            list_byte.Add((byte)((x >> 8)));
            list_byte.Add((byte)(y));
            list_byte.Add((byte)(y >> 8));
            list_byte.Add((byte)(width));
            list_byte.Add((byte)(width >> 8));
            list_byte.Add((byte)(height));
            list_byte.Add((byte)(height >> 8));
            list_byte.Add(3);
            for (int i = 0; i < list_byte.Count; i++)
            {
                checksum += list_byte[i];
            }
            MyTimer MyTimer_UART_TimeOut = new MyTimer();
            int retry = 0;
            int cnt = 0;
            while (true)
            {
                if (cnt == 0)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    uDP_Class.Set_ReadLineClearByIP(IP);
                    uDP_Class.WriteByte(list_byte.ToArray(), IP);
                    MyTimer_UART_TimeOut.TickStop();
                    MyTimer_UART_TimeOut.StartTickTime(UDP_TimeOut);
                    cnt++;
                }
                else if (cnt == 1)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    if (MyTimer_UART_TimeOut.IsTimeOut())
                    {
                        retry++;
                        cnt = 0;
                    }
                    string UDP_RX = uDP_Class.Get_ReadLineByIP(IP);

                    if (UDP_RX != "")
                    {
                        if (UDP_RX == checksum.ToString("000"))
                        {
                            flag_OK = true;
                            break;
                        }
                        else
                        {
                            cnt = 0;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : DrawCanvas {string.Format(flag_OK ? "sucess" : "failed")}!");
            return flag_OK;
        }

        static private bool Command_DrawLine(UDP_Class uDP_Class, string IP, int x0, int y0, int x1, int y1, Color color)
        {
            bool flag_OK = true;
            byte checksum = 0;

            int temp;
            int temp_L;
            int temp_H;
            int R = color.R;
            int G = color.G;
            int B = color.B;
            R = R >> 3;
            G = G >> 2;
            B = B >> 3;
            temp = (R << 11) | (G << 5) | B;
            temp_L = temp & 0x000000FF;
            temp_H = temp >> 8;

            List<byte> list_byte = new List<byte>();
            list_byte.Add(2);
            list_byte.Add((byte)UDP_Command.DrawLine);
            list_byte.Add((byte)(x0));
            list_byte.Add((byte)((x0 >> 8)));
            list_byte.Add((byte)(y0));
            list_byte.Add((byte)((y0 >> 8)));
            list_byte.Add((byte)(x1));
            list_byte.Add((byte)((x1 >> 8)));
            list_byte.Add((byte)(y1));
            list_byte.Add((byte)((y1 >> 8)));
            list_byte.Add((byte)(temp_L));
            list_byte.Add((byte)(temp_H));
            list_byte.Add(3);
            for (int i = 0; i < list_byte.Count; i++)
            {
                checksum += list_byte[i];
            }
            MyTimer MyTimer_UART_TimeOut = new MyTimer();
            int retry = 0;
            int cnt = 0;
            while (true)
            {
                if (cnt == 0)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    uDP_Class.Set_ReadLineClearByIP(IP);
                    uDP_Class.WriteByte(list_byte.ToArray(),IP);
                    MyTimer_UART_TimeOut.TickStop();
                    MyTimer_UART_TimeOut.StartTickTime(UDP_TimeOut);
                    cnt++;
                }
                else if (cnt == 1)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    if (MyTimer_UART_TimeOut.IsTimeOut())
                    {
                        retry++;
                        cnt = 0;
                    }
                    string UDP_RX = uDP_Class.Get_ReadLineByIP(IP);
                    uDP_Class.readline = "";
                    if (UDP_RX != "")
                    {
                        if (UDP_RX == checksum.ToString("000"))
                        {
                            flag_OK = true;
                            break;
                        }
                        else
                        {
                            cnt = 0;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : DrawLine {string.Format(flag_OK ? "sucess" : "failed")}!");
            return flag_OK;
        }
        static private bool Command_WS2812_Blink(UDP_Class uDP_Class, string IP, int blinkTime, Color color)
        {
            bool flag_OK = true;
            byte checksum = 0;

            int R = color.R;
            int G = color.G;
            int B = color.B;
            List<byte> list_byte = new List<byte>();
            list_byte.Add(2);
            list_byte.Add((byte)UDP_Command.WS2812_Blink);
            list_byte.Add((byte)(blinkTime));
            list_byte.Add((byte)((blinkTime >> 8)));
            list_byte.Add((byte)(R));
            list_byte.Add((byte)(G));
            list_byte.Add((byte)(B));
            list_byte.Add(3);
            for (int i = 0; i < list_byte.Count; i++)
            {
                checksum += list_byte[i];
            }
            MyTimer MyTimer_UART_TimeOut = new MyTimer();
            int retry = 0;
            int cnt = 0;
            while (true)
            {
                if (cnt == 0)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    uDP_Class.Set_ReadLineClearByIP(IP);
                    uDP_Class.WriteByte(list_byte.ToArray(),IP);
                    MyTimer_UART_TimeOut.TickStop();
                    MyTimer_UART_TimeOut.StartTickTime(UDP_TimeOut);
                    cnt++;
                }
                else if (cnt == 1)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    if (MyTimer_UART_TimeOut.IsTimeOut())
                    {
                        retry++;
                        cnt = 0;
                    }
                    string UDP_RX = uDP_Class.Get_ReadLineByIP(IP);
        
                    if (UDP_RX != "")
                    {
                        if (UDP_RX == checksum.ToString("000"))
                        {
                            flag_OK = true;
                            break;
                        }
                        else
                        {
                            cnt = 0;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : Set_WS2812_Buffer {string.Format(flag_OK ? "sucess" : "failed")}!");
            return flag_OK;
        }
        static private bool Command_FontColor(UDP_Class uDP_Class, string IP, Color FontColor, Color ForeColor)
        {
            bool flag_OK = true;
            byte checksum = 0;

            int R = 0;
            int G = 0;
            int B = 0;
            int temp = 0;
            int FontColor_temp_H = 0;
            int FontColor_temp_L = 0;
            int ForeColor_temp_H = 0;
            int ForeColor_temp_L = 0;

            R = FontColor.R;
            G = FontColor.G;
            B = FontColor.B;
            R = R >> 3;
            G = G >> 2;
            B = B >> 3;
            temp = (R << 11) | (G << 5) | B;
            FontColor_temp_H = temp & 0x000000FF;
            FontColor_temp_L = temp >> 8;

            R = ForeColor.R;
            G = ForeColor.G;
            B = ForeColor.B;
            R = R >> 3;
            G = G >> 2;
            B = B >> 3;
            temp = (R << 11) | (G << 5) | B;
            ForeColor_temp_H = temp & 0x000000FF;
            ForeColor_temp_L = temp >> 8;

            List<byte> list_byte = new List<byte>();
            list_byte.Add(2);
            list_byte.Add((byte)UDP_Command.FontColor);
            list_byte.Add((byte)FontColor_temp_L);
            list_byte.Add((byte)FontColor_temp_H);
            list_byte.Add((byte)ForeColor_temp_L);
            list_byte.Add((byte)ForeColor_temp_H);
            list_byte.Add(3);
            for (int i = 0; i < list_byte.Count; i++)
            {
                checksum += list_byte[i];
            }
            MyTimer MyTimer_UART_TimeOut = new MyTimer();
            int retry = 0;
            int cnt = 0;
            while (true)
            {
                if (cnt == 0)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    uDP_Class.Set_ReadLineClearByIP(IP);
                    uDP_Class.WriteByte(list_byte.ToArray(),IP);
                    MyTimer_UART_TimeOut.TickStop();
                    MyTimer_UART_TimeOut.StartTickTime(UDP_TimeOut);
                    cnt++;
                }
                else if (cnt == 1)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    if (MyTimer_UART_TimeOut.IsTimeOut())
                    {
                        retry++;
                        cnt = 0;
                    }
                    string UDP_RX = uDP_Class.Get_ReadLineByIP(IP);
               
                    if (UDP_RX != "")
                    {
                        if (UDP_RX == checksum.ToString("000"))
                        {
                            flag_OK = true;
                            break;
                        }
                        else
                        {
                            cnt = 0;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : FontColor {string.Format(flag_OK ? "sucess" : "failed")}!");
            return flag_OK;
        }
        static private bool Command_LCD114_FontColor(UDP_Class uDP_Class, string IP, Color FontColor, Color ForeColor)
        {
            bool flag_OK = true;
            byte checksum = 0;

            int R = 0;
            int G = 0;
            int B = 0;
            int temp = 0;
            int FontColor_temp_H = 0;
            int FontColor_temp_L = 0;
            int ForeColor_temp_H = 0;
            int ForeColor_temp_L = 0;

            R = FontColor.R;
            G = FontColor.G;
            B = FontColor.B;

            // 565 格式的顏色運算
            B = (B * 31) / 255;   // 紅色轉換到5位 (0-31)
            R = (R * 63) / 255;   // 綠色轉換到6位 (0-63)
            G = (G * 31) / 255;   // 藍色轉換到5位 (0-31)
            temp = (B << 11) | (R << 5) | G;

            FontColor_temp_H = temp & 0x000000FF;
            FontColor_temp_L = temp >> 8;

            R = ForeColor.R;
            G = ForeColor.G;
            B = ForeColor.B;

            // 565 格式的顏色運算
            B = (B * 31) / 255;   // 紅色轉換到5位 (0-31)
            R = (R * 63) / 255;   // 綠色轉換到6位 (0-63)
            G = (G * 31) / 255;   // 藍色轉換到5位 (0-31)
            temp = (B << 11) | (R << 5) | G;

            ForeColor_temp_H = temp & 0x000000FF;
            ForeColor_temp_L = temp >> 8;

            List<byte> list_byte = new List<byte>();
            list_byte.Add(2);
            list_byte.Add((byte)UDP_Command.FontColor);
            list_byte.Add((byte)FontColor_temp_L);
            list_byte.Add((byte)FontColor_temp_H);
            list_byte.Add((byte)ForeColor_temp_L);
            list_byte.Add((byte)ForeColor_temp_H);
            list_byte.Add(3);
            for (int i = 0; i < list_byte.Count; i++)
            {
                checksum += list_byte[i];
            }
            MyTimer MyTimer_UART_TimeOut = new MyTimer();
            int retry = 0;
            int cnt = 0;
            while (true)
            {
                if (cnt == 0)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    uDP_Class.Set_ReadLineClearByIP(IP);
                    uDP_Class.WriteByte(list_byte.ToArray(), IP);
                    MyTimer_UART_TimeOut.TickStop();
                    MyTimer_UART_TimeOut.StartTickTime(UDP_TimeOut);
                    cnt++;
                }
                else if (cnt == 1)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    if (MyTimer_UART_TimeOut.IsTimeOut())
                    {
                        retry++;
                        cnt = 0;
                    }
                    string UDP_RX = uDP_Class.Get_ReadLineByIP(IP);

                    if (UDP_RX != "")
                    {
                        if (UDP_RX == checksum.ToString("000"))
                        {
                            flag_OK = true;
                            break;
                        }
                        else
                        {
                            cnt = 0;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : FontColor {string.Format(flag_OK ? "sucess" : "failed")}!");
            return flag_OK;
        }
        static private bool Command_FramebufferEx(UDP_Class uDP_Class, string IP, long start_ptr, byte[] value , int len)
        {
            bool flag_OK = true;
            byte checksum = 0;
            List<byte> list_byte = new List<byte>();
            list_byte.Add(2);
            list_byte.Add((byte)UDP_Command.FramebufferEx);
            list_byte.Add((byte)(len));
            list_byte.Add((byte)((len >> 8)));
            list_byte.Add((byte)start_ptr);
            list_byte.Add((byte)(start_ptr >> 8));
            list_byte.Add((byte)(start_ptr >> 16));
            list_byte.Add((byte)(start_ptr >> 24));

            for (int i = 0; i < value.Length; i++)
            {
                list_byte.Add(value[i]);
            }
            list_byte.Add(3);
            for (int i = 0; i < list_byte.Count; i++)
            {
                checksum += list_byte[i];
            }
            MyTimer MyTimer_UART_TimeOut = new MyTimer();
            int retry = 0;
            int cnt = 0;
            while (true)
            {
                if (cnt == 0)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    uDP_Class.Set_ReadLineClearByIP(IP);
                    uDP_Class.WriteByte(list_byte.ToArray() , IP);

                    MyTimer_UART_TimeOut.TickStop();
                    MyTimer_UART_TimeOut.StartTickTime(UDP_TimeOut);
                    cnt++;
                }
                else if (cnt == 1)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    if (MyTimer_UART_TimeOut.IsTimeOut())
                    {
                        retry++;
                        cnt = 0;
                    }
                    string UDP_RX = uDP_Class.Get_ReadLineByIP(IP);
                    if (UDP_RX != "")
                    {
                        if (UDP_RX == checksum.ToString("000"))
                        {
                            flag_OK = true;
                            break;
                        }
                        else
                        {
                            cnt = 0;
                        }
                    }
                }
             
                System.Threading.Thread.Sleep(1);
            }
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : FramebufferEx {string.Format(flag_OK ? "sucess" : "failed")}!");
            return flag_OK;
        }
        static private bool Command_Code_FrameWrite(UDP_Class uDP_Class, string IP, int x, int y, int width, int height, Color FontColor, Color ForeColor)
        {
            bool flag_OK = true;
            byte checksum = 0;
            int R = 0;
            int G = 0;
            int B = 0;
            int temp = 0;
            int FontColor_temp_H = 0;
            int FontColor_temp_L = 0;
            int ForeColor_temp_H = 0;
            int ForeColor_temp_L = 0;

            R = FontColor.R;
            G = FontColor.G;
            B = FontColor.B;
            R = R >> 3;
            G = G >> 2;
            B = B >> 3;
            temp = (R << 11) | (G << 5) | B;
            FontColor_temp_H = temp & 0x000000FF;
            FontColor_temp_L = temp >> 8;

            R = ForeColor.R;
            G = ForeColor.G;
            B = ForeColor.B;
            R = R >> 3;
            G = G >> 2;
            B = B >> 3;
            temp = (R << 11) | (G << 5) | B;
            ForeColor_temp_H = temp & 0x000000FF;
            ForeColor_temp_L = temp >> 8;

            List<byte> list_byte = new List<byte>();
            list_byte.Add(2);
            list_byte.Add((byte)UDP_Command.Code_FrameWrite);
            list_byte.Add((byte)(x));
            list_byte.Add((byte)((x >> 8)));
            list_byte.Add((byte)(y));
            list_byte.Add((byte)((y >> 8)));
            list_byte.Add((byte)(width));
            list_byte.Add((byte)((width >> 8)));
            list_byte.Add((byte)(height));
            list_byte.Add((byte)((height >> 8)));
            list_byte.Add((byte)FontColor_temp_L);
            list_byte.Add((byte)FontColor_temp_H);
            list_byte.Add((byte)ForeColor_temp_L);
            list_byte.Add((byte)ForeColor_temp_H);

            list_byte.Add(3);
            for (int i = 0; i < list_byte.Count; i++)
            {
                checksum += list_byte[i];
            }
            MyTimer MyTimer_UART_TimeOut = new MyTimer();
            int retry = 0;
            int cnt = 0;
            while (true)
            {
                if (cnt == 0)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    uDP_Class.Set_ReadLineClearByIP(IP);
                    uDP_Class.WriteByte(list_byte.ToArray(),IP);

                    MyTimer_UART_TimeOut.TickStop();
                    MyTimer_UART_TimeOut.StartTickTime(UDP_TimeOut);
                    cnt++;
                }
                else if (cnt == 1)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    if (MyTimer_UART_TimeOut.IsTimeOut())
                    {
                        retry++;
                        cnt = 0;
                    }
                    string UDP_RX = uDP_Class.Get_ReadLineByIP(IP);
                    if (UDP_RX != "")
                    {
                        if (UDP_RX == checksum.ToString("000"))
                        {
                            flag_OK = true;
                            break;
                        }
                        else
                        {
                            cnt = 0;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : Code_FrameWrite {string.Format(flag_OK ? "sucess" : "failed")}!");
            return flag_OK;
        }
        static private bool Command_ESP_Restart(UDP_Class uDP_Class, string IP)
        {
            bool flag_OK = true;
            byte checksum = 0;
            List<byte> list_byte = new List<byte>();
            list_byte.Add(2);
            list_byte.Add((byte)UDP_Command.ESP_Restart);
            list_byte.Add(3);
            for (int i = 0; i < list_byte.Count; i++)
            {
                checksum += list_byte[i];
            }
            MyTimer MyTimer_UART_TimeOut = new MyTimer();
            int retry = 0;
            int cnt = 0;
            while (true)
            {
                if (cnt == 0)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    uDP_Class.Set_ReadLineClearByIP(IP);
                    uDP_Class.WriteByte(list_byte.ToArray(),IP);
                    MyTimer_UART_TimeOut.TickStop();
                    MyTimer_UART_TimeOut.StartTickTime(UDP_TimeOut);
                    cnt++;
                }
                else if (cnt == 1)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    if (MyTimer_UART_TimeOut.IsTimeOut())
                    {
                        retry++;
                        cnt = 0;
                    }

                    string UDP_RX = uDP_Class.Get_ReadLineByIP(IP);
                    if (UDP_RX != "")
                    {
                        if (UDP_RX == checksum.ToString("000"))
                        {
                            flag_OK = true;
                            break;
                        }
                        else
                        {
                            cnt = 0;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : ESP_Restart {string.Format(flag_OK ? "sucess" : "failed")}!");
            return flag_OK;
        }
        static private bool Command_OTAUpdate(UDP_Class uDP_Class, string IP)
        {
            bool flag_OK = true;
            byte checksum = 0;
            List<byte> list_byte = new List<byte>();
            list_byte.Add(2);
            list_byte.Add((byte)UDP_Command.OTAUpdate);
            list_byte.Add(3);
            for (int i = 0; i < list_byte.Count; i++)
            {
                checksum += list_byte[i];
            }
            MyTimer MyTimer_UART_TimeOut = new MyTimer();
            int retry = 0;
            int cnt = 0;
            while (true)
            {
                if (cnt == 0)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    uDP_Class.Set_ReadLineClearByIP(IP);
                    uDP_Class.WriteByte(list_byte.ToArray() , IP);
                    MyTimer_UART_TimeOut.TickStop();
                    MyTimer_UART_TimeOut.StartTickTime(UDP_TimeOut);
                    cnt++;
                }
                else if (cnt == 1)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    if (MyTimer_UART_TimeOut.IsTimeOut())
                    {
                        retry++;
                        cnt = 0;
                    }
                    string UDP_RX = uDP_Class.Get_ReadLineByIP(IP);
                    if (UDP_RX != "")
                    {
                        if (UDP_RX == checksum.ToString("000"))
                        {
                            flag_OK = true;
                            break;
                        }
                        else
                        {
                            cnt = 0;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : OTAUpdate {string.Format(flag_OK ? "sucess" : "failed")}!");
            return flag_OK;
        }
        static private bool Command_SetToPage(UDP_Class uDP_Class, string IP , int pagenum)
        {
            bool flag_OK = true;
            byte checksum = 0;
            List<byte> list_byte = new List<byte>();
            list_byte.Add(2);
            list_byte.Add((byte)UDP_Command.SetToPage);
            list_byte.Add((byte)pagenum);
            list_byte.Add(3);
            for (int i = 0; i < list_byte.Count; i++)
            {
                checksum += list_byte[i];
            }
            MyTimer MyTimer_UART_TimeOut = new MyTimer();
            int retry = 0;
            int cnt = 0;
            while (true)
            {
                if (cnt == 0)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    uDP_Class.Set_ReadLineClearByIP(IP);
                    uDP_Class.WriteByte(list_byte.ToArray(), IP);
                    MyTimer_UART_TimeOut.TickStop();
                    MyTimer_UART_TimeOut.StartTickTime(UDP_TimeOut);
                    cnt++;
                }
                else if (cnt == 1)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    if (MyTimer_UART_TimeOut.IsTimeOut())
                    {
                        retry++;
                        cnt = 0;
                    }
                    string UDP_RX = uDP_Class.Get_ReadLineByIP(IP);
                    if (UDP_RX != "")
                    {
                        if (UDP_RX == checksum.ToString("000"))
                        {
                            flag_OK = true;
                            break;
                        }
                        else
                        {
                            cnt = 0;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : SetToPage {string.Format(flag_OK ? "sucess" : "failed")}!");
            return flag_OK;
        }
        static private bool Command_Set_PIN(UDP_Class uDP_Class, string IP, int PIN, bool Statu)
        {
            bool flag_OK = true;
            byte checksum = 0;
            List<byte> list_byte = new List<byte>();
            list_byte.Add(2);
            list_byte.Add((byte)UDP_Command.Set_PIN);
            list_byte.Add((byte)PIN);
            list_byte.Add(Statu ? (byte)1 : (byte)0);
            list_byte.Add(3);
            for (int i = 0; i < list_byte.Count; i++)
            {
                checksum += list_byte[i];
            }
            MyTimer MyTimer_UART_TimeOut = new MyTimer();
            int retry = 0;
            int cnt = 0;
            while (true)
            {
                if (cnt == 0)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    uDP_Class.Set_ReadLineClearByIP(IP);
                    uDP_Class.WriteByte(list_byte.ToArray(), IP);
                    MyTimer_UART_TimeOut.TickStop();
                    MyTimer_UART_TimeOut.StartTickTime(UDP_TimeOut);
                    cnt++;
                }
                else if (cnt == 1)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    if (MyTimer_UART_TimeOut.IsTimeOut())
                    {
                        retry++;
                        cnt = 0;
                    }
                    string UDP_RX = uDP_Class.Get_ReadLineByIP(IP);

                    if (UDP_RX != "")
                    {
                        if (UDP_RX == checksum.ToString("000"))
                        {
                            flag_OK = true;
                            break;
                        }
                        else
                        {
                            cnt = 0;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : Set_PIN {string.Format(flag_OK ? "sucess" : "failed")}!");
            return flag_OK;
        }
        static private bool Command_Set_TOF(UDP_Class uDP_Class, string IP, bool Statu)
        {
            bool flag_OK = true;
            byte checksum = 0;
            List<byte> list_byte = new List<byte>();
            list_byte.Add(2);
            list_byte.Add((byte)UDP_Command.Set_TOF);
            list_byte.Add(Statu ? (byte)0 : (byte)1);
            list_byte.Add(3);
            for (int i = 0; i < list_byte.Count; i++)
            {
                checksum += list_byte[i];
            }
            MyTimer MyTimer_UART_TimeOut = new MyTimer();
            int retry = 0;
            int cnt = 0;
            while (true)
            {
                if (cnt == 0)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    uDP_Class.Set_ReadLineClearByIP(IP);
                    uDP_Class.WriteByte(list_byte.ToArray(), IP);
                    MyTimer_UART_TimeOut.TickStop();
                    MyTimer_UART_TimeOut.StartTickTime(UDP_TimeOut);
                    cnt++;
                }
                else if (cnt == 1)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    if (MyTimer_UART_TimeOut.IsTimeOut())
                    {
                        retry++;
                        cnt = 0;
                    }
                    string UDP_RX = uDP_Class.Get_ReadLineByIP(IP);

                    if (UDP_RX != "")
                    {
                        if (UDP_RX == checksum.ToString("000"))
                        {
                            flag_OK = true;
                            break;
                        }
                        else
                        {
                            cnt = 0;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : 設定雷射狀態 {Statu} {string.Format(flag_OK ? "sucess" : "failed")}!");
            return flag_OK;
        }
        static private bool Command_DrawRect(UDP_Class uDP_Class, string IP, int x, int y, int width, int height, int pen_width, Color color)
        {
            bool flag_OK = true;
            byte checksum = 0;

            int temp;
            int temp_L;
            int temp_H;
            int R = color.R;
            int G = color.G;
            int B = color.B;
            R = R >> 3;
            G = G >> 2;
            B = B >> 3;
            temp = (R << 11) | (G << 5) | B;
            temp_L = temp & 0x000000FF;
            temp_H = temp >> 8;

            List<byte> list_byte = new List<byte>();
            list_byte.Add(2);
            list_byte.Add((byte)UDP_Command.DrawRect);
            list_byte.Add((byte)(x));
            list_byte.Add((byte)((x >> 8)));
            list_byte.Add((byte)(y));
            list_byte.Add((byte)((y >> 8)));
            list_byte.Add((byte)(width));
            list_byte.Add((byte)((width >> 8)));
            list_byte.Add((byte)(height));
            list_byte.Add((byte)((height >> 8)));
            list_byte.Add((byte)(pen_width));
            list_byte.Add((byte)((pen_width >> 8)));
            list_byte.Add((byte)(temp_L));
            list_byte.Add((byte)(temp_H));
            list_byte.Add(3);
            for (int i = 0; i < list_byte.Count; i++)
            {
                checksum += list_byte[i];
            }
            MyTimer MyTimer_UART_TimeOut = new MyTimer();
            int retry = 0;
            int cnt = 0;
            while (true)
            {
                if (cnt == 0)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    uDP_Class.Set_ReadLineClearByIP(IP);
                    uDP_Class.WriteByte(list_byte.ToArray(), IP);
                    MyTimer_UART_TimeOut.TickStop();
                    MyTimer_UART_TimeOut.StartTickTime(UDP_TimeOut);
                    cnt++;
                }
                else if (cnt == 1)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    if (MyTimer_UART_TimeOut.IsTimeOut())
                    {
                        retry++;
                        cnt = 0;
                    }
                    string UDP_RX = uDP_Class.Get_ReadLineByIP(IP);
                    uDP_Class.Set_ReadLineClearByIP(IP);
                    if (UDP_RX != "")
                    {
                        if (UDP_RX == checksum.ToString("000"))
                        {
                            flag_OK = true;
                            break;
                        }
                        else
                        {
                            cnt = 0;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : DrawRect {string.Format(flag_OK ? "sucess" : "failed")}!");
            return flag_OK;
        }
        static private bool Command_Set_UDP_SendTime(UDP_Class uDP_Class, string IP, int ms)
        {
            bool flag_OK = true;
            byte checksum = 0;
            List<byte> list_byte = new List<byte>();
            list_byte.Add(2);
            list_byte.Add((byte)(UDP_Command.Set_UDP_SendTime));
            list_byte.Add((byte)(ms));
            list_byte.Add((byte)((ms >> 8)));
            list_byte.Add(3);
            for (int i = 0; i < list_byte.Count; i++)
            {
                checksum += list_byte[i];
            }
            MyTimer MyTimer_UART_TimeOut = new MyTimer();
            int retry = 0;
            int cnt = 0;
            while (true)
            {
                if (cnt == 0)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    uDP_Class.Set_ReadLineClearByIP(IP);
                    uDP_Class.WriteByte(list_byte.ToArray(), IP);
                    MyTimer_UART_TimeOut.TickStop();
                    MyTimer_UART_TimeOut.StartTickTime(UDP_TimeOut);
                    cnt++;
                }
                else if (cnt == 1)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    if (MyTimer_UART_TimeOut.IsTimeOut())
                    {
                        retry++;
                        cnt = 0;
                    }
                    string UDP_RX = uDP_Class.Get_ReadLineByIP(IP);
                    if (UDP_RX != "")
                    {
                        if (UDP_RX == checksum.ToString("000"))
                        {
                            flag_OK = true;
                            break;
                        }
                        else
                        {
                            cnt = 0;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : Set UDP SendTime {string.Format(flag_OK ? "sucess" : "failed")}!");
            return flag_OK;
        }
        static private bool Command_Set_ScreenPageInit(UDP_Class uDP_Class, string IP, bool enable)
        {
            bool flag_OK = true;
            byte checksum = 0;
            List<byte> list_byte = new List<byte>();
            list_byte.Add(2);
            list_byte.Add((byte)(UDP_Command.Set_ScreenPageInit));
            list_byte.Add((byte)(enable ? (byte)1 : (byte)0));
            list_byte.Add(3);
            for (int i = 0; i < list_byte.Count; i++)
            {
                checksum += list_byte[i];
            }
            MyTimer MyTimer_UART_TimeOut = new MyTimer();
            int retry = 0;
            int cnt = 0;
            while (true)
            {
                if (cnt == 0)
                {
                    if (retry >= UDP_RetryNum)
                    {

                        flag_OK = false;
                        break;
                    }
                    uDP_Class.Set_ReadLineClearByIP(IP);
                    uDP_Class.WriteByte(list_byte.ToArray(), IP);
                    MyTimer_UART_TimeOut.TickStop();
                    MyTimer_UART_TimeOut.StartTickTime(UDP_TimeOut);
                    cnt++;
                }
                else if (cnt == 1)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    if (MyTimer_UART_TimeOut.IsTimeOut())
                    {
                        retry++;
                        cnt = 0;
                    }
                    string UDP_RX = uDP_Class.Get_ReadLineByIP(IP);
                    if (UDP_RX != "")
                    {
                        if (UDP_RX == checksum.ToString("000"))
                        {
                            flag_OK = true;
                            break;
                        }
                        else
                        {
                            cnt = 0;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : Set ScreenPageInit {string.Format(flag_OK ? "sucess" : "failed")}!");
            return flag_OK;
        }
  
        static private bool Command_Set_ServerConfig(UDP_Class uDP_Class, string IP, string ServerIP ,int ServerPort)
        {
            bool flag_OK = true;
            byte checksum = 0;

            string[] IP_Array = ServerIP.Split('.');
            if (IP_Array.Length != 4) return false;
            int IPA = IP_Array[0].StringToInt32();
            int IPB = IP_Array[1].StringToInt32();
            int IPC = IP_Array[2].StringToInt32();
            int IPD = IP_Array[3].StringToInt32();
            if (IPA < 0 || IPA > 255) return false;
            if (IPB < 0 || IPB > 255) return false;
            if (IPC < 0 || IPC > 255) return false;
            if (IPD < 0 || IPD > 255) return false;

            List<byte> list_byte = new List<byte>();
            list_byte.Add(2);
            list_byte.Add((byte)(UDP_Command.Set_ServerConfig));
            list_byte.Add((byte)(IPA));
            list_byte.Add((byte)(IPB));
            list_byte.Add((byte)(IPC));
            list_byte.Add((byte)(IPD));
            list_byte.Add((byte)(ServerPort));
            list_byte.Add((byte)((ServerPort >> 8)));
            list_byte.Add(3);
            for (int i = 0; i < list_byte.Count; i++)
            {
                checksum += list_byte[i];
            }
            MyTimer MyTimer_UART_TimeOut = new MyTimer();
            int retry = 0;
            int cnt = 0;
            while (true)
            {
                if (cnt == 0)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    uDP_Class.Set_ReadLineClearByIP(IP);
                    uDP_Class.WriteByte(list_byte.ToArray(), IP);
                    MyTimer_UART_TimeOut.TickStop();
                    MyTimer_UART_TimeOut.StartTickTime(UDP_TimeOut);
                    cnt++;
                }
                else if (cnt == 1)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    if (MyTimer_UART_TimeOut.IsTimeOut())
                    {
                        retry++;
                        cnt = 0;
                    }
                    string UDP_RX = uDP_Class.Get_ReadLineByIP(IP);
                    if (UDP_RX != "")
                    {
                        if (UDP_RX == checksum.ToString("000"))
                        {
                            flag_OK = true;
                            break;
                        }
                        else
                        {
                            cnt = 0;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : Set ServerConfig {string.Format(flag_OK ? "sucess" : "failed")}!");
            return flag_OK;
        }
        static private bool Command_Set_JsonStringSend(UDP_Class uDP_Class, string IP)
        {
            bool flag_OK = true;
            byte checksum = 0;
            List<byte> list_byte = new List<byte>();
            list_byte.Add(2);
            list_byte.Add((byte)(UDP_Command.Set_JsonStringSend));
            list_byte.Add(3);
            for (int i = 0; i < list_byte.Count; i++)
            {
                checksum += list_byte[i];
            }
            MyTimer MyTimer_UART_TimeOut = new MyTimer();
            int retry = 0;
            int cnt = 0;
            while (true)
            {
                if (cnt == 0)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    uDP_Class.Set_ReadLineClearByIP(IP);
                    uDP_Class.WriteByte(list_byte.ToArray(), IP);
                    MyTimer_UART_TimeOut.TickStop();
                    MyTimer_UART_TimeOut.StartTickTime(UDP_TimeOut);
                    cnt++;
                }
                else if (cnt == 1)
                {
                    if (retry >= UDP_RetryNum)
                    {                        flag_OK = false;
                        break;
                    }
                    if (MyTimer_UART_TimeOut.IsTimeOut())
                    {
                        retry++;
                        cnt = 0;
                    }
                    string UDP_RX = uDP_Class.Get_ReadLineByIP(IP);
                    if (UDP_RX != "")
                    {
                        if (UDP_RX == checksum.ToString("000"))
                        {
                            flag_OK = true;
                            break;
                        }
                        else
                        {
                            cnt = 0;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : Set JsonStringSend {string.Format(flag_OK ? "sucess" : "failed")}!");
            return flag_OK;
        }
        static private bool Command_Set_Output(UDP_Class uDP_Class, string IP, int value)
        {
            bool flag_OK = true;
            byte checksum = 0;
            List<byte> list_byte = new List<byte>();
            list_byte.Add(2);
            list_byte.Add((byte)(UDP_Command.Set_Output));
            list_byte.Add((byte)(value));
            list_byte.Add((byte)((value >> 8)));
            list_byte.Add(3);
            for (int i = 0; i < list_byte.Count; i++)
            {
                checksum += list_byte[i];
            }
            MyTimer MyTimer_UART_TimeOut = new MyTimer();
            int retry = 0;
            int cnt = 0;
            while (true)
            {
                if (cnt == 0)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    uDP_Class.Set_ReadLineClearByIP(IP);
                    uDP_Class.WriteByte(list_byte.ToArray(), IP);
                    MyTimer_UART_TimeOut.TickStop();
                    MyTimer_UART_TimeOut.StartTickTime(UDP_TimeOut);
                    cnt++;
                }
                else if (cnt == 1)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    if (MyTimer_UART_TimeOut.IsTimeOut())
                    {
                        retry++;
                        cnt = 0;
                    }
                    string UDP_RX = uDP_Class.Get_ReadLineByIP(IP);
                    if (UDP_RX != "")
                    {
                        if (UDP_RX == checksum.ToString("000"))
                        {
                            flag_OK = true;
                            break;
                        }
                        else
                        {
                            cnt = 0;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : Set Output {string.Format(flag_OK ? "sucess" : "failed")}!");
            return flag_OK;
        }
        static private bool Command_Set_OutputPIN(UDP_Class uDP_Class, string IP, int PIN_Num, bool value)
        {
            bool flag_OK = true;
            byte checksum = 0;
            List<byte> list_byte = new List<byte>();
            list_byte.Add(2);
            list_byte.Add((byte)(UDP_Command.Set_OutputPIN));
            list_byte.Add((byte)(PIN_Num));
            list_byte.Add((byte)(value ? 1 : 0));
            list_byte.Add(3);
            for (int i = 0; i < list_byte.Count; i++)
            {
                checksum += list_byte[i];
            }
            MyTimer MyTimer_UART_TimeOut = new MyTimer();
            int retry = 0;
            int cnt = 0;
            while (true)
            {
                if (cnt == 0)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    uDP_Class.Set_ReadLineClearByIP(IP);
                    uDP_Class.WriteByte(list_byte.ToArray(), IP);
                    MyTimer_UART_TimeOut.TickStop();
                    MyTimer_UART_TimeOut.StartTickTime(UDP_TimeOut);
                    cnt++;
                }
                else if (cnt == 1)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    if (MyTimer_UART_TimeOut.IsTimeOut())
                    {
                        retry++;
                        cnt = 0;
                    }
                    string UDP_RX = uDP_Class.Get_ReadLineByIP(IP);
                    if (UDP_RX != "")
                    {
                        if (UDP_RX == checksum.ToString("000"))
                        {
                            flag_OK = true;
                            break;
                        }
                        else
                        {
                            cnt = 0;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : Set OutputPIN {string.Format(flag_OK ? "sucess" : "failed")}!");
            return flag_OK;
        }
        static private bool Command_Set_OutputTrigger(UDP_Class uDP_Class, string IP, int value)
        {
            bool flag_OK = true;
            byte checksum = 0;
            List<byte> list_byte = new List<byte>();
            list_byte.Add(2);
            list_byte.Add((byte)(UDP_Command.Set_OutputTrigger));
            list_byte.Add((byte)(value));
            list_byte.Add((byte)((value >> 8)));
            list_byte.Add(3);
            for (int i = 0; i < list_byte.Count; i++)
            {
                checksum += list_byte[i];
            }
            MyTimer MyTimer_UART_TimeOut = new MyTimer();
            int retry = 0;
            int cnt = 0;
            while (true)
            {
                if (cnt == 0)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    uDP_Class.Set_ReadLineClearByIP(IP);
                    uDP_Class.WriteByte(list_byte.ToArray(), IP);
                    MyTimer_UART_TimeOut.TickStop();
                    MyTimer_UART_TimeOut.StartTickTime(UDP_TimeOut);
                    cnt++;
                }
                else if (cnt == 1)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    if (MyTimer_UART_TimeOut.IsTimeOut())
                    {
                        retry++;
                        cnt = 0;
                    }
                    string UDP_RX = uDP_Class.Get_ReadLineByIP(IP);
                    if (UDP_RX != "")
                    {
                        if (UDP_RX == checksum.ToString("000"))
                        {
                            flag_OK = true;
                            break;
                        }
                        else
                        {
                            cnt = 0;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : Set OutputTrigger {string.Format(flag_OK ? "sucess" : "failed")}!");
            return flag_OK;
        }
        static private bool Command_Set_OutputPINTrigger(UDP_Class uDP_Class, string IP, int PIN_Num, bool value)
        {
            bool flag_OK = true;
            byte checksum = 0;
            List<byte> list_byte = new List<byte>();
            list_byte.Add(2);
            list_byte.Add((byte)(UDP_Command.Set_OutputPINTrigger));
            list_byte.Add((byte)(PIN_Num));
            list_byte.Add((byte)(value ? 1 : 0));
            list_byte.Add(3);
            for (int i = 0; i < list_byte.Count; i++)
            {
                checksum += list_byte[i];
            }
            MyTimer MyTimer_UART_TimeOut = new MyTimer();
            int retry = 0;
            int cnt = 0;
            while (true)
            {
                if (cnt == 0)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    uDP_Class.Set_ReadLineClearByIP(IP);
                    uDP_Class.WriteByte(list_byte.ToArray(), IP);
                    MyTimer_UART_TimeOut.TickStop();
                    MyTimer_UART_TimeOut.StartTickTime(UDP_TimeOut);
                    cnt++;
                }
                else if (cnt == 1)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    if (MyTimer_UART_TimeOut.IsTimeOut())
                    {
                        retry++;
                        cnt = 0;
                    }
                    string UDP_RX = uDP_Class.Get_ReadLineByIP(IP);
                    if (UDP_RX != "")
                    {
                        if (UDP_RX == checksum.ToString("000"))
                        {
                            flag_OK = true;
                            break;
                        }
                        else
                        {
                            cnt = 0;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : Set OutputPINTrigger {string.Format(flag_OK ? "sucess" : "failed")}!");
            return flag_OK;
        }
        static public bool Command_Set_ADCMotorTrigger(UDP_Class uDP_Class, string IP, int PIN_Num, int time_ms)
        {
            bool flag_OK = true;
            byte checksum = 0;
            List<byte> list_byte = new List<byte>();
            list_byte.Add(2);
            list_byte.Add((byte)(UDP_Command.Set_ADCMotorTrigger));
            list_byte.Add((byte)(PIN_Num));
            list_byte.Add((byte)(time_ms));
            list_byte.Add((byte)((time_ms >> 8))); list_byte.Add(3);
            for (int i = 0; i < list_byte.Count; i++)
            {
                checksum += list_byte[i];
            }
            MyTimer MyTimer_UART_TimeOut = new MyTimer();
            int retry = 0;
            int cnt = 0;
            while (true)
            {
                if (cnt == 0)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    uDP_Class.Set_ReadLineClearByIP(IP);
                    uDP_Class.WriteByte(list_byte.ToArray(), IP);
                    MyTimer_UART_TimeOut.TickStop();
                    MyTimer_UART_TimeOut.StartTickTime(UDP_TimeOut);
                    cnt++;
                }
                else if (cnt == 1)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    if (MyTimer_UART_TimeOut.IsTimeOut())
                    {
                        retry++;
                        cnt = 0;
                    }
                    string UDP_RX = uDP_Class.Get_ReadLineByIP(IP);
                    if (UDP_RX != "")
                    {
                        if (UDP_RX == checksum.ToString("000"))
                        {
                            flag_OK = true;
                            break;
                        }
                        else
                        {
                            cnt = 0;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : Set ADCMotorTrigger {string.Format(flag_OK ? "sucess" : "failed")}! 作動延遲時間:{time_ms}ms {DateTime.Now.ToDateTimeString()}");
            return flag_OK;
        }
        static private bool Command_SendTestData(UDP_Class uDP_Class, string IP ,int port)
        {
            bool flag_OK = true;
            byte checksum = 0;
            List<byte> list_byte = new List<byte>();
            list_byte.Add(2);
            list_byte.Add((byte)UDP_Command.SendTestData);
            list_byte.Add(3);
            for (int i = 0; i < list_byte.Count; i++)
            {
                checksum += list_byte[i];
            }
            uDP_Class.WriteByte(list_byte.ToArray(), IP , port);
            return flag_OK;
        }
        static private bool Command_Set_WS2812_Buffer(UDP_Class uDP_Class, string IP, int start_ptr, byte[] bytes_RGB)
        {
            bool flag_OK = true;
            byte checksum = 0;
            List<byte> list_byte = new List<byte>();
            list_byte.Add(2);
            list_byte.Add((byte)(UDP_Command.Set_WS2812_Buffer));
            list_byte.Add((byte)start_ptr);
            list_byte.Add((byte)(start_ptr >> 8));
            for (int i = 0; i < bytes_RGB.Length; i++)
            {
                list_byte.Add(bytes_RGB[i]);
            }
            list_byte.Add(3);
            for (int i = 0; i < list_byte.Count; i++)
            {
                checksum += list_byte[i];
            }
            MyTimer MyTimer_UART_TimeOut = new MyTimer();
            int retry = 0;
            int cnt = 0;
            while (true)
            {
                if (cnt == 0)
                {
                    if (retry >= 3)
                    {
                        flag_OK = false;
                        break;
                    }
                    uDP_Class.Set_ReadLineClearByIP(IP);
                    uDP_Class.WriteByte(list_byte.ToArray(), IP);
                    MyTimer_UART_TimeOut.TickStop();
                    MyTimer_UART_TimeOut.StartTickTime(250);
                    cnt++;
                }
                else if (cnt == 1)
                {
                    if (retry >= 3)
                    {
                        flag_OK = false;
                        break;
                    }
                    if (MyTimer_UART_TimeOut.IsTimeOut())
                    {
                        retry++;
                        cnt = 0;
                    }
                    string UDP_RX = uDP_Class.Get_ReadLineByIP(IP);
                    if (UDP_RX != "")
                    {
                        if (UDP_RX == checksum.ToString("000"))
                        {
                            flag_OK = true;
                            break;
                        }
                        else
                        {
                            cnt = 0;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : Set_WS2812_Buffer {string.Format(flag_OK ? "sucess" : "failed")} {DateTime.Now.ToDateTimeString()}!");
            return flag_OK;
        }
        static private byte[] Command_Get_WS2812_Buffer(UDP_Class uDP_Class, string IP , int lenth)
        {
            bool flag_OK = true;
            byte checksum = 0;
            byte[] Dbyte = new byte[lenth];
            List<byte> list_byte = new List<byte>();
            list_byte.Add(2);
            list_byte.Add((byte)(UDP_Command.Get_WS2812_Buffer));
            list_byte.Add((byte)lenth);
            list_byte.Add((byte)(lenth >> 8));
            list_byte.Add(3);
            for (int i = 0; i < list_byte.Count; i++)
            {
                checksum += list_byte[i];
            }
            MyTimer MyTimer_UART_TimeOut = new MyTimer();
            int retry = 0;
            int cnt = 0;
            while (true)
            {
                if (cnt == 0)
                {
                    if (retry >= 3)
                    {
                        flag_OK = false;
                        break;
                    }
                    uDP_Class.Set_ReadLineClearByIP(IP);
                    uDP_Class.WriteByte(list_byte.ToArray(), IP);
                    MyTimer_UART_TimeOut.TickStop();
                    MyTimer_UART_TimeOut.StartTickTime(250);
                    cnt++;
                }
                else if (cnt == 1)
                {
                    if (retry >= 3)
                    {
                        flag_OK = false;
                        break;
                    }
                    if (MyTimer_UART_TimeOut.IsTimeOut())
                    {
                        retry++;
                        cnt = 0;
                    }
                    string UDP_RX = uDP_Class.Get_ReadLineByIP(IP);
                    if (UDP_RX != "")
                    {
                       
                        byte[] bytes_buf = Encoding.GetEncoding("BIG5").GetBytes(UDP_RX);
                        for (int i = 0; i < bytes_buf.Length; i++)
                        {
                            if (i > Dbyte.Length) break;
                            Dbyte[i] = bytes_buf[i];
                        }
                        break;
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : Get WS2812_Buffer {string.Format(flag_OK ? "sucess" : "failed")} {DateTime.Now.ToDateTimeString()}!");
            return Dbyte;
        }
        static private bool Command_Set_GetewayConfig(UDP_Class uDP_Class, string IP, string Geteway)
        {
            bool flag_OK = true;
            byte checksum = 0;

            string[] IP_Array = Geteway.Split('.');
            if (IP_Array.Length != 4) return false;
            int IPA = IP_Array[0].StringToInt32();
            int IPB = IP_Array[1].StringToInt32();
            int IPC = IP_Array[2].StringToInt32();
            int IPD = IP_Array[3].StringToInt32();
            if (IPA < 0 || IPA > 255) return false;
            if (IPB < 0 || IPB > 255) return false;
            if (IPC < 0 || IPC > 255) return false;
            if (IPD < 0 || IPD > 255) return false;

            List<byte> list_byte = new List<byte>();
            list_byte.Add(2);
            list_byte.Add((byte)(UDP_Command.Set_GetewayConfig));
            list_byte.Add((byte)(IPA));
            list_byte.Add((byte)(IPB));
            list_byte.Add((byte)(IPC));
            list_byte.Add((byte)(IPD));
            list_byte.Add(3);
            for (int i = 0; i < list_byte.Count; i++)
            {
                checksum += list_byte[i];
            }
            MyTimer MyTimer_UART_TimeOut = new MyTimer();
            int retry = 0;
            int cnt = 0;
            while (true)
            {
                if (cnt == 0)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    uDP_Class.Set_ReadLineClearByIP(IP);
                    uDP_Class.WriteByte(list_byte.ToArray(), IP);
                    MyTimer_UART_TimeOut.TickStop();
                    MyTimer_UART_TimeOut.StartTickTime(UDP_TimeOut);
                    cnt++;
                }
                else if (cnt == 1)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    if (MyTimer_UART_TimeOut.IsTimeOut())
                    {
                        retry++;
                        cnt = 0;
                    }
                    string UDP_RX = uDP_Class.Get_ReadLineByIP(IP);
                    if (UDP_RX != "")
                    {
                        if (UDP_RX == checksum.ToString("000"))
                        {
                            flag_OK = true;
                            break;
                        }
                        else
                        {
                            cnt = 0;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : Set GetewayConfig {string.Format(flag_OK ? "sucess" : "failed")}!");
            return flag_OK;
        }
        static private bool Command_Set_LocalPort(UDP_Class uDP_Class, string IP, int LocalPort)
        {
            bool flag_OK = true;
            byte checksum = 0;


            List<byte> list_byte = new List<byte>();
            list_byte.Add(2);
            list_byte.Add((byte)(UDP_Command.Set_LocalPort));
            list_byte.Add((byte)(LocalPort));
            list_byte.Add((byte)((LocalPort >> 8)));
            list_byte.Add(3);
            for (int i = 0; i < list_byte.Count; i++)
            {
                checksum += list_byte[i];
            }
            MyTimer MyTimer_UART_TimeOut = new MyTimer();
            int retry = 0;
            int cnt = 0;
            while (true)
            {
                if (cnt == 0)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    uDP_Class.Set_ReadLineClearByIP(IP);
                    uDP_Class.WriteByte(list_byte.ToArray(), IP);
                    MyTimer_UART_TimeOut.TickStop();
                    MyTimer_UART_TimeOut.StartTickTime(UDP_TimeOut);
                    cnt++;
                }
                else if (cnt == 1)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    if (MyTimer_UART_TimeOut.IsTimeOut())
                    {
                        retry++;
                        cnt = 0;
                    }
                    string UDP_RX = uDP_Class.Get_ReadLineByIP(IP);
                    if (UDP_RX != "")
                    {
                        if (UDP_RX == checksum.ToString("000"))
                        {
                            flag_OK = true;
                            break;
                        }
                        else
                        {
                            cnt = 0;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : Set LocalPort {string.Format(flag_OK ? "sucess" : "failed")}! Port : {LocalPort}");
            return flag_OK;
        }
        static private bool Command_Set_Input_dir(UDP_Class uDP_Class, string IP, int PIN_Num, bool value)
        {
            bool flag_OK = true;
            byte checksum = 0;
            List<byte> list_byte = new List<byte>();
            list_byte.Add(2);
            list_byte.Add((byte)(UDP_Command.Set_Input_dir));
            list_byte.Add((byte)(PIN_Num));
            list_byte.Add((byte)(value ? 1 : 0));
            list_byte.Add(3);
            for (int i = 0; i < list_byte.Count; i++)
            {
                checksum += list_byte[i];
            }
            MyTimer MyTimer_UART_TimeOut = new MyTimer();
            int retry = 0;
            int cnt = 0;
            while (true)
            {
                if (cnt == 0)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    uDP_Class.Set_ReadLineClearByIP(IP);
                    uDP_Class.WriteByte(list_byte.ToArray(), IP);
                    MyTimer_UART_TimeOut.TickStop();
                    MyTimer_UART_TimeOut.StartTickTime(UDP_TimeOut);
                    cnt++;
                }
                else if (cnt == 1)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    if (MyTimer_UART_TimeOut.IsTimeOut())
                    {
                        retry++;
                        cnt = 0;
                    }
                    string UDP_RX = uDP_Class.Get_ReadLineByIP(IP);
                    if (UDP_RX != "")
                    {
                        if (UDP_RX == checksum.ToString("000"))
                        {
                            flag_OK = true;
                            break;
                        }
                        else
                        {
                            cnt = 0;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : Set OutputPIN {string.Format(flag_OK ? "sucess" : "failed")}!");
            return flag_OK;
        }
        static private bool Command_Set_Output_dir(UDP_Class uDP_Class, string IP, int PIN_Num, bool value)
        {
            bool flag_OK = true;
            byte checksum = 0;
            List<byte> list_byte = new List<byte>();
            list_byte.Add(2);
            list_byte.Add((byte)(UDP_Command.Set_Output_dir));
            list_byte.Add((byte)(PIN_Num));
            list_byte.Add((byte)(value ? 1 : 0));
            list_byte.Add(3);
            for (int i = 0; i < list_byte.Count; i++)
            {
                checksum += list_byte[i];
            }
            MyTimer MyTimer_UART_TimeOut = new MyTimer();
            int retry = 0;
            int cnt = 0;
            while (true)
            {
                if (cnt == 0)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    uDP_Class.Set_ReadLineClearByIP(IP);
                    uDP_Class.WriteByte(list_byte.ToArray(), IP);
                    MyTimer_UART_TimeOut.TickStop();
                    MyTimer_UART_TimeOut.StartTickTime(UDP_TimeOut);
                    cnt++;
                }
                else if (cnt == 1)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    if (MyTimer_UART_TimeOut.IsTimeOut())
                    {
                        retry++;
                        cnt = 0;
                    }
                    string UDP_RX = uDP_Class.Get_ReadLineByIP(IP);
                    if (UDP_RX != "")
                    {
                        if (UDP_RX == checksum.ToString("000"))
                        {
                            flag_OK = true;
                            break;
                        }
                        else
                        {
                            cnt = 0;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : Set OutputPIN {string.Format(flag_OK ? "sucess" : "failed")}!");
            return flag_OK;
        }
        static private bool Command_Set_RFID_Enable(UDP_Class uDP_Class, string IP, int index, bool value)
        {
            bool flag_OK = true;
            byte checksum = 0;
            List<byte> list_byte = new List<byte>();
            list_byte.Add(2);
            list_byte.Add((byte)(UDP_Command.Set_RFID_Enable));
            list_byte.Add((byte)(index));
            list_byte.Add((byte)(value ? 1 : 0));
            list_byte.Add(3);
            for (int i = 0; i < list_byte.Count; i++)
            {
                checksum += list_byte[i];
            }
            MyTimer MyTimer_UART_TimeOut = new MyTimer();
            int retry = 0;
            int cnt = 0;
            while (true)
            {
                if (cnt == 0)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    uDP_Class.Set_ReadLineClearByIP(IP);
                    uDP_Class.WriteByte(list_byte.ToArray(), IP);
                    MyTimer_UART_TimeOut.TickStop();
                    MyTimer_UART_TimeOut.StartTickTime(UDP_TimeOut);
                    cnt++;
                }
                else if (cnt == 1)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    if (MyTimer_UART_TimeOut.IsTimeOut())
                    {
                        retry++;
                        cnt = 0;
                    }
                    string UDP_RX = uDP_Class.Get_ReadLineByIP(IP);
                    if (UDP_RX != "")
                    {
                        if (UDP_RX == checksum.ToString("000"))
                        {
                            flag_OK = true;
                            break;
                        }
                        else
                        {
                            cnt = 0;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : Set OutputPIN {string.Format(flag_OK ? "sucess" : "failed")}!");
            return flag_OK;
        }
        static private bool Command_Set_RFID_Beep(UDP_Class uDP_Class, string IP, int station)
        {
            bool flag_OK = true;
            byte checksum = 0;
            List<byte> list_byte = new List<byte>();
            list_byte.Add(2);
            list_byte.Add((byte)(UDP_Command.Set_RFID_Beep));
            list_byte.Add((byte)(station));
            list_byte.Add(3);
            for (int i = 0; i < list_byte.Count; i++)
            {
                checksum += list_byte[i];
            }
            MyTimer MyTimer_UART_TimeOut = new MyTimer();
            int retry = 0;
            int cnt = 0;
            while (true)
            {
                if (cnt == 0)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    uDP_Class.Set_ReadLineClearByIP(IP);
                    uDP_Class.WriteByte(list_byte.ToArray(), IP);
                    MyTimer_UART_TimeOut.TickStop();
                    MyTimer_UART_TimeOut.StartTickTime(UDP_TimeOut);
                    cnt++;
                }
                else if (cnt == 1)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    if (MyTimer_UART_TimeOut.IsTimeOut())
                    {
                        retry++;
                        cnt = 0;
                    }
                    string UDP_RX = uDP_Class.Get_ReadLineByIP(IP);
                    if (UDP_RX != "")
                    {
                        if (UDP_RX == checksum.ToString("000"))
                        {
                            flag_OK = true;
                            break;
                        }
                        else
                        {
                            cnt = 0;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : Set OutputPIN {string.Format(flag_OK ? "sucess" : "failed")}!");
            return flag_OK;
        }
        static private bool Command_Get_LEDSetting(UDP_Class uDP_Class, string IP ,ref int[] InputPIN, ref int[] OutputPIN)
        {
            bool flag_OK = true;
            byte checksum = 0;
            if (InputPIN.Length < 5) return false;
            if (OutputPIN.Length < 5) return false;
            List<byte> list_byte = new List<byte>();
            list_byte.Add(2);
            list_byte.Add((byte)(UDP_Command.Get_LEDSetting));
            list_byte.Add(3);
            for (int i = 0; i < list_byte.Count; i++)
            {
                checksum += list_byte[i];
            }
            MyTimer MyTimer_UART_TimeOut = new MyTimer();
            int retry = 0;
            int cnt = 0;
            while (true)
            {
                if (cnt == 0)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    uDP_Class.Set_ReadLineClearByIP(IP);
                    uDP_Class.WriteByte(list_byte.ToArray(), IP);
                    MyTimer_UART_TimeOut.TickStop();
                    MyTimer_UART_TimeOut.StartTickTime(UDP_TimeOut);
                    cnt++;
                }
                else if (cnt == 1)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    if (MyTimer_UART_TimeOut.IsTimeOut())
                    {
                        retry++;
                        cnt = 0;
                    }
                    string UDP_RX = uDP_Class.Get_ReadLineByIP(IP);
                    if (UDP_RX != "")
                    {
                        byte[] bytes_buf = Encoding.GetEncoding("BIG5").GetBytes(UDP_RX);
                        if (bytes_buf.Length == 10)
                        {
                            for (int i = 0; i < 5; i++)
                            {                               
                                InputPIN[i] = bytes_buf[i];
                            }
                            for (int i = 0; i < 5; i++)
                            {
                                OutputPIN[i] = bytes_buf[i + 5];
                            }
                            flag_OK = true;
                            break;
                        }
                        else
                        {
                            cnt = 0;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : Get LEDSetting {string.Format(flag_OK ? "sucess" : "failed")}!");
            return flag_OK;
        }
        static private bool Command_Set_LEDSetting(UDP_Class uDP_Class, string IP, int[] InputPIN, int[] OutputPIN)
        {
            bool flag_OK = true;
            byte checksum = 0;
            List<byte> list_byte = new List<byte>();
            list_byte.Add(2);
            list_byte.Add((byte)(UDP_Command.Set_LEDSetting));
            for (int i = 0; i < InputPIN.Length; i++)
            {
                list_byte.Add((byte)InputPIN[i]);
            }
            for (int i = 0; i < OutputPIN.Length; i++)
            {
                list_byte.Add((byte)OutputPIN[i]);
            }
            list_byte.Add(3);
            for (int i = 0; i < list_byte.Count; i++)
            {
                checksum += list_byte[i];
            }
            MyTimer MyTimer_UART_TimeOut = new MyTimer();
            int retry = 0;
            int cnt = 0;
            while (true)
            {
                if (cnt == 0)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    uDP_Class.Set_ReadLineClearByIP(IP);
                    uDP_Class.WriteByte(list_byte.ToArray(), IP);
                    MyTimer_UART_TimeOut.TickStop();
                    MyTimer_UART_TimeOut.StartTickTime(UDP_TimeOut);
                    cnt++;
                }
                else if (cnt == 1)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    if (MyTimer_UART_TimeOut.IsTimeOut())
                    {
                        retry++;
                        cnt = 0;
                    }
                    string UDP_RX = uDP_Class.Get_ReadLineByIP(IP);
                    if (UDP_RX != "")
                    {
                        if (UDP_RX == checksum.ToString("000"))
                        {
                            flag_OK = true;
                            break;
                        }
                        else
                        {
                            cnt = 0;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} :Set LEDSetting {string.Format(flag_OK ? "sucess" : "failed")}!");
            return flag_OK;
        }
        static private bool Command_GetKeyBoardValue(UDP_Class uDP_Class, string IP, ref int value)
        {
            bool flag_OK = true;
            byte checksum = 0;
            List<byte> list_byte = new List<byte>();
            list_byte.Add(2);
            list_byte.Add((byte)(UDP_Command.GetKeyBoardValue));
            list_byte.Add(3);
            for (int i = 0; i < list_byte.Count; i++)
            {
                checksum += list_byte[i];
            }
            MyTimer MyTimer_UART_TimeOut = new MyTimer();
            int retry = 0;
            int cnt = 0;
            while (true)
            {
                if (cnt == 0)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    uDP_Class.Set_ReadLineClearByIP(IP);
                    uDP_Class.WriteByte(list_byte.ToArray(), IP);
                    MyTimer_UART_TimeOut.TickStop();
                    MyTimer_UART_TimeOut.StartTickTime(UDP_TimeOut);
                    cnt++;
                }
                else if (cnt == 1)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    if (MyTimer_UART_TimeOut.IsTimeOut())
                    {
                        retry++;
                        cnt = 0;
                    }
                    string UDP_RX = uDP_Class.Get_ReadLineByIP(IP);
                    if (UDP_RX != "")
                    {
                        if (UDP_RX.Length == 5)
                        {
                            value = UDP_RX.ToString().StringToInt32();
                            flag_OK = true;
                            break;
                        }
                        else
                        {
                            cnt = 0;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : GetKeyBoardValue {string.Format(flag_OK ? "sucess" : "failed")}! value : {value.ToString()}");
            return flag_OK;
        }
        static private bool Command_Get_IO(UDP_Class uDP_Class, string IP , out int input , out int output)
        {
            bool flag_OK = true;
            byte checksum = 0;
            input = 0;
            output = 0;
            List<byte> list_byte = new List<byte>();
            list_byte.Add(2);
            list_byte.Add((byte)(UDP_Command.Get_IO));
            list_byte.Add(3);
            for (int i = 0; i < list_byte.Count; i++)
            {
                checksum += list_byte[i];
            }
            MyTimer MyTimer_UART_TimeOut = new MyTimer();
            int retry = 0;
            int cnt = 0;
            while (true)
            {
                if (cnt == 0)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    uDP_Class.Set_ReadLineClearByIP(IP);
                    uDP_Class.WriteByte(list_byte.ToArray(), IP);
                    MyTimer_UART_TimeOut.TickStop();
                    MyTimer_UART_TimeOut.StartTickTime(UDP_TimeOut);
                    cnt++;
                }
                else if (cnt == 1)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    if (MyTimer_UART_TimeOut.IsTimeOut())
                    {
                        retry++;
                        cnt = 0;
                    }
                    string UDP_RX = uDP_Class.Get_ReadLineByIP(IP);
                    if (UDP_RX != "")
                    {

                        byte[] bytes_buf = Encoding.GetEncoding("BIG5").GetBytes(UDP_RX);
                        if(bytes_buf.Length != 4)
                        {
                            retry++;
                            cnt = 0;
                        }
                        else
                        {
                            input = bytes_buf[0] | bytes_buf[1] << 8;
                            output = bytes_buf[2] | bytes_buf[3] << 8;
                            break;
                        }
                     
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
           // if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : Get IO {string.Format(flag_OK ? "sucess" : "failed")}!");
            return true;
        }
        static private bool Command_Set_BlinkEnable(UDP_Class uDP_Class, string IP, int PIN_Num, bool value, int blinkTime)
        {
            bool flag_OK = true;
            byte checksum = 0;
            List<byte> list_byte = new List<byte>();
            list_byte.Add(2);
            list_byte.Add((byte)(UDP_Command.Set_BlinkEnable));
            list_byte.Add((byte)(PIN_Num));
            list_byte.Add((byte)(value ? 1 : 0));
            list_byte.Add((byte)(blinkTime));
            list_byte.Add((byte)((blinkTime >> 8)));
            list_byte.Add(3);
            for (int i = 0; i < list_byte.Count; i++)
            {
                checksum += list_byte[i];
            }
            MyTimer MyTimer_UART_TimeOut = new MyTimer();
            int retry = 0;
            int cnt = 0;
            while (true)
            {
                if (cnt == 0)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    uDP_Class.Set_ReadLineClearByIP(IP);
                    uDP_Class.WriteByte(list_byte.ToArray(), IP);
                    MyTimer_UART_TimeOut.TickStop();
                    MyTimer_UART_TimeOut.StartTickTime(UDP_TimeOut);
                    cnt++;
                }
                else if (cnt == 1)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    if (MyTimer_UART_TimeOut.IsTimeOut())
                    {
                        retry++;
                        cnt = 0;
                    }
                    string UDP_RX = uDP_Class.Get_ReadLineByIP(IP);
                    if (UDP_RX != "")
                    {
                        if (UDP_RX == checksum.ToString("000"))
                        {
                            flag_OK = true;
                            break;
                        }
                        else
                        {
                            cnt = 0;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : Set Set_BlinkEnable {string.Format(flag_OK ? "sucess" : "failed")}!");
            return flag_OK;
        }
        static private bool Command_Set_WS2812B_breathing(UDP_Class uDP_Class, string IP, byte WS2812B_breathing_onAddVal, byte WS2812B_breathing_offSubVal, Color color)
        {
            bool flag_OK = true;
            byte checksum = 0;
            List<byte> list_byte = new List<byte>();
            list_byte.Add(2);
            list_byte.Add((byte)(UDP_Command.Set_WS2812B_breathing));
            list_byte.Add((byte)(WS2812B_breathing_onAddVal));
            list_byte.Add((byte)(WS2812B_breathing_offSubVal));
            list_byte.Add((byte)(color.R));
            list_byte.Add((byte)(color.G));
            list_byte.Add((byte)(color.B));

            list_byte.Add(3);
            for (int i = 0; i < list_byte.Count; i++)
            {
                checksum += list_byte[i];
            }
            MyTimer MyTimer_UART_TimeOut = new MyTimer();
            int retry = 0;
            int cnt = 0;
            while (true)
            {
                if (cnt == 0)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    uDP_Class.Set_ReadLineClearByIP(IP);
                    uDP_Class.WriteByte(list_byte.ToArray(), IP);
                    MyTimer_UART_TimeOut.TickStop();
                    MyTimer_UART_TimeOut.StartTickTime(UDP_TimeOut);
                    cnt++;
                }
                else if (cnt == 1)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    if (MyTimer_UART_TimeOut.IsTimeOut())
                    {
                        retry++;
                        cnt = 0;
                    }
                    string UDP_RX = uDP_Class.Get_ReadLineByIP(IP);
                    if (UDP_RX != "")
                    {
                        if (UDP_RX == checksum.ToString("000"))
                        {
                            flag_OK = true;
                            break;
                        }
                        else
                        {
                            cnt = 0;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : Set Set_WS2812B_breathing {string.Format(flag_OK ? "sucess" : "failed")}!");
            return flag_OK;
        }

        static private int Command_EPD_Get_LaserDistance(UDP_Class uDP_Class, string IP)
        {
            int distance = -1;
            bool flag_OK = true;
            byte checksum = 0;
            List<byte> list_byte = new List<byte>();
            list_byte.Add(2);
            list_byte.Add((byte)UDP_Command.EPD_Get_LaserDistance);
            list_byte.Add(3);
            for (int i = 0; i < list_byte.Count; i++)
            {
                checksum += list_byte[i];
            }
            MyTimer MyTimer_UART_TimeOut = new MyTimer();
            int retry = 0;
            int cnt = 0;
            while (true)
            {
                if (cnt == 0)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    uDP_Class.Set_ReadLineClearByIP(IP);
                    uDP_Class.WriteByte(list_byte.ToArray(), IP);
                    MyTimer_UART_TimeOut.TickStop();
                    MyTimer_UART_TimeOut.StartTickTime(UDP_TimeOut);
                    cnt++;
                }
                else if (cnt == 1)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    if (MyTimer_UART_TimeOut.IsTimeOut())
                    {
                        retry++;
                        cnt = 0;
                    }
                    string UDP_RX = uDP_Class.Get_ReadLineByIP(IP);
                    if (UDP_RX != "")
                    {
                        if (UDP_RX.StringToInt32() != -1)
                        {
                            distance = UDP_RX.StringToInt32();
                            break;
                        }
                        else
                        {
                            cnt = 0;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : EPD Get LaserDistance {string.Format(flag_OK ? "sucess" : "failed")}!");
            return distance;
        }
        static private bool Command_EPD_Set_Sleep(UDP_Class uDP_Class, string IP)
        {
            bool flag_OK = true;
            byte checksum = 0;
            List<byte> list_byte = new List<byte>();
            list_byte.Add(2);
            list_byte.Add((byte)UDP_Command.EPD_Set_Sleep);
            list_byte.Add(3);
            for (int i = 0; i < list_byte.Count; i++)
            {
                checksum += list_byte[i];
            }
            MyTimer MyTimer_UART_TimeOut = new MyTimer();
            int retry = 0;
            int cnt = 0;
            while (true)
            {
                if (cnt == 0)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    uDP_Class.Set_ReadLineClearByIP(IP);
                    uDP_Class.WriteByte(list_byte.ToArray(), IP);
                    MyTimer_UART_TimeOut.TickStop();
                    MyTimer_UART_TimeOut.StartTickTime(UDP_TimeOut);
                    cnt++;
                }
                else if (cnt == 1)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    if (MyTimer_UART_TimeOut.IsTimeOut())
                    {
                        retry++;
                        cnt = 0;
                    }
                    string UDP_RX = uDP_Class.Get_ReadLineByIP(IP);
                    if (UDP_RX != "")
                    {
                        if (UDP_RX == checksum.ToString("000"))
                        {
                            flag_OK = true;
                            break;
                        }
                        else
                        {
                            cnt = 0;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : EPD Set Sleep {string.Format(flag_OK ? "sucess" : "failed")}!");
            return flag_OK;
        }
        static private bool Command_EPD_Set_WakeUp(UDP_Class uDP_Class, string IP)
        {
            bool flag_OK = true;
            byte checksum = 0;
            List<byte> list_byte = new List<byte>();
            list_byte.Add(2);
            list_byte.Add((byte)UDP_Command.EPD_Set_WakeUp);
            list_byte.Add(3);
            for (int i = 0; i < list_byte.Count; i++)
            {
                checksum += list_byte[i];
            }
            MyTimer MyTimer_UART_TimeOut = new MyTimer();
            int retry = 0;
            int cnt = 0;
            while (true)
            {
                if (cnt == 0)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    uDP_Class.Set_ReadLineClearByIP(IP);
                    uDP_Class.WriteByte(list_byte.ToArray(), IP);
                    MyTimer_UART_TimeOut.TickStop();
                    MyTimer_UART_TimeOut.StartTickTime(UDP_TimeOut);
                    cnt++;
                }
                else if (cnt == 1)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    if (MyTimer_UART_TimeOut.IsTimeOut())
                    {
                        retry++;
                        cnt = 0;
                    }
                    string UDP_RX = uDP_Class.Get_ReadLineByIP(IP);
                    if (UDP_RX != "")
                    {
                        if (UDP_RX == checksum.ToString("000"))
                        {
                            flag_OK = true;
                            break;
                        }
                        else
                        {
                            cnt = 0;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : EPD Set WakeUp {string.Format(flag_OK ? "sucess" : "failed")}!");
            return flag_OK;
        }
        static private bool Command_EPD_DrawFrame_RW(UDP_Class uDP_Class, string IP)
        {
            bool flag_OK = true;
            byte checksum = 0;
            List<byte> list_byte = new List<byte>();
            list_byte.Add(2);
            list_byte.Add((byte)UDP_Command.EPD_DrawFrame_RW);
            list_byte.Add(3);
            for (int i = 0; i < list_byte.Count; i++)
            {
                checksum += list_byte[i];
            }
            MyTimer MyTimer_UART_TimeOut = new MyTimer();
            int retry = 0;
            int cnt = 0;
            while (true)
            {
                if (cnt == 0)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    uDP_Class.Set_ReadLineClearByIP(IP);
                    uDP_Class.WriteByte(list_byte.ToArray(), IP);
                    MyTimer_UART_TimeOut.TickStop();
                    MyTimer_UART_TimeOut.StartTickTime(UDP_TimeOut);
                    cnt++;
                }
                else if (cnt == 1)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    if (MyTimer_UART_TimeOut.IsTimeOut())
                    {
                        retry++;
                        cnt = 0;
                    }
                    string UDP_RX = uDP_Class.Get_ReadLineByIP(IP);
                    if (UDP_RX != "")
                    {
                        if (UDP_RX == checksum.ToString("000"))
                        {
                            flag_OK = true;
                            break;
                        }
                        else
                        {
                            cnt = 0;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : EPD DrawFrame RW {string.Format(flag_OK ? "sucess" : "failed")}!");
            return flag_OK;
        }
        static private bool Command_EPD_DrawFrame_BW(UDP_Class uDP_Class, string IP)
        {
            bool flag_OK = true;
            byte checksum = 0;
            List<byte> list_byte = new List<byte>();
            list_byte.Add(2);
            list_byte.Add((byte)UDP_Command.EPD_DrawFrame_BW);
            list_byte.Add(3);
            for (int i = 0; i < list_byte.Count; i++)
            {
                checksum += list_byte[i];
            }
            MyTimer MyTimer_UART_TimeOut = new MyTimer();
            int retry = 0;
            int cnt = 0;
            while (true)
            {
                if (cnt == 0)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    uDP_Class.Set_ReadLineClearByIP(IP);
                    uDP_Class.WriteByte(list_byte.ToArray(), IP);
                    MyTimer_UART_TimeOut.TickStop();
                    MyTimer_UART_TimeOut.StartTickTime(UDP_TimeOut);
                    cnt++;
                }
                else if (cnt == 1)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    if (MyTimer_UART_TimeOut.IsTimeOut())
                    {
                        retry++;
                        cnt = 0;
                    }
                    string UDP_RX = uDP_Class.Get_ReadLineByIP(IP);
                    if (UDP_RX != "")
                    {
                        if (UDP_RX == checksum.ToString("000"))
                        {
                            flag_OK = true;
                            break;
                        }
                        else
                        {
                            cnt = 0;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : EPD DrawFrame BW {string.Format(flag_OK ? "sucess" : "failed")}!");
            return flag_OK;
        }
        static private bool Command_EPD_RefreshCanvas(UDP_Class uDP_Class, string IP)
        {
            bool flag_OK = true;
            byte checksum = 0;
            List<byte> list_byte = new List<byte>();
            list_byte.Add(2);
            list_byte.Add((byte)UDP_Command.EPD_RefreshCanvas);
            list_byte.Add(3);
            for (int i = 0; i < list_byte.Count; i++)
            {
                checksum += list_byte[i];
            }
            MyTimer MyTimer_UART_TimeOut = new MyTimer();
            int retry = 0;
            int cnt = 0;
            while (true)
            {
                if (cnt == 0)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    uDP_Class.Set_ReadLineClearByIP(IP);
                    uDP_Class.WriteByte(list_byte.ToArray(), IP);
                    MyTimer_UART_TimeOut.TickStop();
                    MyTimer_UART_TimeOut.StartTickTime(UDP_TimeOut);
                    cnt++;
                }
                else if (cnt == 1)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    if (MyTimer_UART_TimeOut.IsTimeOut())
                    {
                        retry++;
                        cnt = 0;
                    }
                    string UDP_RX = uDP_Class.Get_ReadLineByIP(IP);
                    if (UDP_RX != "")
                    {
                        if (UDP_RX == checksum.ToString("000"))
                        {
                            flag_OK = true;
                            break;
                        }
                        else
                        {
                            cnt = 0;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : EPD RefreshCanvas {string.Format(flag_OK ? "sucess" : "failed")}!");
            return flag_OK;
        }
        static private bool Command_EPD_BW_Command(UDP_Class uDP_Class, string IP)
        {
            bool flag_OK = true;
            byte checksum = 0;
            List<byte> list_byte = new List<byte>();
            list_byte.Add(2);
            list_byte.Add((byte)UDP_Command.EPD_BW_Command);
            list_byte.Add(3);
            for (int i = 0; i < list_byte.Count; i++)
            {
                checksum += list_byte[i];
            }
            MyTimer MyTimer_UART_TimeOut = new MyTimer();
            int retry = 0;
            int cnt = 0;
            while (true)
            {
                if (cnt == 0)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    uDP_Class.Set_ReadLineClearByIP(IP);
                    uDP_Class.WriteByte(list_byte.ToArray(), IP);
                    MyTimer_UART_TimeOut.TickStop();
                    MyTimer_UART_TimeOut.StartTickTime(UDP_TimeOut);
                    cnt++;
                }
                else if (cnt == 1)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    if (MyTimer_UART_TimeOut.IsTimeOut())
                    {
                        retry++;
                        cnt = 0;
                    }
                    string UDP_RX = uDP_Class.Get_ReadLineByIP(IP);
                    if (UDP_RX != "")
                    {
                        if (UDP_RX == checksum.ToString("000"))
                        {
                            flag_OK = true;
                            break;
                        }
                        else
                        {
                            cnt = 0;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : EPD BW_Command {string.Format(flag_OK ? "sucess" : "failed")}!");
            return flag_OK;
        }
        static private bool Command_EPD_RW_Command(UDP_Class uDP_Class, string IP)
        {
            bool flag_OK = true;
            byte checksum = 0;
            List<byte> list_byte = new List<byte>();
            list_byte.Add(2);
            list_byte.Add((byte)UDP_Command.EPD_RW_Command);
            list_byte.Add(3);
            for (int i = 0; i < list_byte.Count; i++)
            {
                checksum += list_byte[i];
            }
            MyTimer MyTimer_UART_TimeOut = new MyTimer();
            int retry = 0;
            int cnt = 0;
            while (true)
            {
                if (cnt == 0)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    uDP_Class.Set_ReadLineClearByIP(IP);
                    uDP_Class.WriteByte(list_byte.ToArray(), IP);
                    MyTimer_UART_TimeOut.TickStop();
                    MyTimer_UART_TimeOut.StartTickTime(UDP_TimeOut);
                    cnt++;
                }
                else if (cnt == 1)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    if (MyTimer_UART_TimeOut.IsTimeOut())
                    {
                        retry++;
                        cnt = 0;
                    }
                    string UDP_RX = uDP_Class.Get_ReadLineByIP(IP);
                    if (UDP_RX != "")
                    {
                        if (UDP_RX == checksum.ToString("000"))
                        {
                            flag_OK = true;
                            break;
                        }
                        else
                        {
                            cnt = 0;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : EPD RW_Command {string.Format(flag_OK ? "sucess" : "failed")}!");
            return flag_OK;
        }
        static private bool Command_EPD_Send_Framebuffer(UDP_Class uDP_Class, string IP, long start_ptr, byte[] value)
        {
            bool flag_OK = true;
            byte checksum = 0;
            List<byte> list_byte = new List<byte>();
            list_byte.Add(2);
            list_byte.Add((byte)UDP_Command.EPD_Send_Framebuffer);
            list_byte.Add((byte)start_ptr);
            list_byte.Add((byte)(start_ptr >> 8));
            list_byte.Add((byte)(start_ptr >> 16));
            list_byte.Add((byte)(start_ptr >> 24));
            for (int i = 0; i < value.Length; i++)
            {
                list_byte.Add(value[i]);
            }
            list_byte.Add(3);
            for (int i = 0; i < list_byte.Count; i++)
            {
                checksum += list_byte[i];
            }
            MyTimer MyTimer_UART_TimeOut = new MyTimer();
            int retry = 0;
            int cnt = 0;
            while (true)
            {
                if (cnt == 0)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    uDP_Class.Set_ReadLineClearByIP(IP);
                    uDP_Class.WriteByte(list_byte.ToArray(), IP);
                    MyTimer_UART_TimeOut.TickStop();
                    MyTimer_UART_TimeOut.StartTickTime(UDP_TimeOut);
                    cnt++;
                }
                else if (cnt == 1)
                {
                    if (retry >= UDP_RetryNum)
                    {
                        flag_OK = false;
                        break;
                    }
                    if (MyTimer_UART_TimeOut.IsTimeOut())
                    {
                        retry++;
                        cnt = 0;
                        if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : EPD Send Framebuffer timeout!");
                    }
                    string UDP_RX = uDP_Class.Get_ReadLineByIP(IP);
                    if (UDP_RX != "")
                    {
                        if (UDP_RX == "RETRY")
                        {
                            retry++;
                            cnt = 0;
                            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : EPD Send Framebuffer recieve 'RETRY'!");
                        }
                        else if (UDP_RX == checksum.ToString("000"))
                        {
                            flag_OK = true;
                            break;
                        }
                        else
                        {
                            cnt = 0;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : EPD Send Framebuffer {string.Format(flag_OK ? "sucess" : "failed")}!");
            return flag_OK;
        }
        static private bool Command_EPD_SendSPI(UDP_Class uDP_Class, string IP, long start_ptr,  byte[] value)
        {
            bool flag_OK = true;
            byte checksum = 0;
            List<byte> list_byte = new List<byte>();
            list_byte.Add(2);
            list_byte.Add((byte)UDP_Command.EPD_SendSPI);
            list_byte.Add((byte)start_ptr);
            list_byte.Add((byte)(start_ptr >> 8));
            list_byte.Add((byte)(start_ptr >> 16));
            list_byte.Add((byte)(start_ptr >> 24));
            for (int i = 0; i < value.Length; i++)
            {
                list_byte.Add(value[i]);
            }
            list_byte.Add(3);
            for (int i = 0; i < list_byte.Count; i++)
            {
                checksum += list_byte[i];
            }
            MyTimer MyTimer_UART_TimeOut = new MyTimer();
            int retry = 0;
            int cnt = 0;
            while (true)
            {
                if (cnt == 0)
                {
                    if (retry >= 10)
                    {
                        flag_OK = false;
                        break;
                    }
                    uDP_Class.Set_ReadLineClearByIP(IP);
                    uDP_Class.WriteByte(list_byte.ToArray(), IP);
                    MyTimer_UART_TimeOut.TickStop();
                    MyTimer_UART_TimeOut.StartTickTime(2000);
                    cnt++;
                }
                else if (cnt == 1)
                {
                    if (retry >= 10)
                    {
                        flag_OK = false;
                        break;
                    }
                    if (MyTimer_UART_TimeOut.IsTimeOut())
                    {
                        retry++;
                        cnt = 0;
                        if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : EPD Send EPD_SendSPI timeout!");
                    }
                    string UDP_RX = uDP_Class.Get_ReadLineByIP(IP);
                    if (UDP_RX != "")
                    {
                        if (UDP_RX == "RETRY")
                        {
                            retry++;
                            cnt = 0;
                            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : EPD Send EPD_SendSPI recieve 'RETRY'!");
                        }
                        else if (UDP_RX == checksum.ToString("000"))
                        {
                            flag_OK = true;
                            break;
                        }
                        else
                        {
                            cnt = 0;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : EPD Send EPD_SendSPI {string.Format(flag_OK ? "sucess" : "failed")}!");
            return flag_OK;
        }

        #region UART
        static public int UART_TimeOut = 500;
        static public int UART_Delay = 10;
        static public int UART_RetryNum = 3;
        static public bool UART_ConsoletWrite = false;
        private enum UART_Command
        {
            Get_Setting = (byte)'0',
            Set_Station = (byte)'1',
            Set_IP_Adress = (byte)'2',
            Set_Subnet = (byte)'3',
            Set_Gateway = (byte)'4',
            Set_DNS = (byte)'5',
            Set_Server_IP_Adress = (byte)'6',
            Set_Local_Port = (byte)'7',
            Set_Server_Port = (byte)'8',
            Set_SSID = (byte)'9',
            Set_Password = (byte)'A',
            Set_UDP_SendTime = (byte)'B',
            Set_WS2812_Buffer = (byte)'C',
            ClearCanvas = (byte)'D',

            Sset_RFID_Enable = (byte)'Z',
            Get_Version = (byte)'v',
            Set_Locker = (byte)'L',

            RS485_GetIO = (byte)'E',
            RS485_SetOutput = (byte)'F',
            RS485_SetOutputPIN = (byte)'G',

            RS485_SetInputDir = (byte)'M',
            RS485_SetOutputDir = (byte)'H',
            RS485_GetInputDir = (byte)'I',
            RS485_GetOutputDir = (byte)'J',

            EPD_Set_WakeUp = (byte)'b',
            EPD_DrawFrame_RW = (byte)'c',
            EPD_DrawFrame_BW = (byte)'d',
            EPD_Send_Framebuffer = (byte)'e',
            EPD_RefreshCanvas = (byte)'f',


        }
        static public bool UART_Command_RS485_GetIO(MySerialPort MySerialPort, int station, ref int input, ref int output)
        {
            try
            {
                bool flag_OK = false;
                MyTimerBasic myTimerBasic = new MyTimerBasic();
                if (MySerialPort.IsConnected == false) MySerialPort.SerialPortOpen();
                MyTimer MyTimer_UART_TimeOut = new MyTimer();

                int retry = 0;
                int cnt = 0;
                MySerialPort.ClearReadByte();
                List<byte> list_byte = new List<byte>();
                list_byte.Add(2);
                list_byte.Add((byte)(station));
                list_byte.Add((byte)(UART_Command.RS485_GetIO));
                list_byte.Add(3);
                ushort CRC = Basic.MyConvert.Get_CRC16(list_byte.ToArray());
                list_byte.Add((byte)(CRC >> 0));
                list_byte.Add((byte)(CRC >> 8));
                while (true)
                {
                    if (cnt == 0)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            Console.Write($"[{MethodBase.GetCurrentMethod().Name}] Set data  error! station: {station}  TX_HEX : {list_byte.ToArray().ByteToStringHex()}\n");
                            flag_OK = false;
                            break;
                        }
                        MySerialPort.ClearReadByte();
                        MySerialPort.WriteByte(list_byte.ToArray());
                        MyTimer_UART_TimeOut.TickStop();
                        MyTimer_UART_TimeOut.StartTickTime(UART_TimeOut);
                        cnt++;
                    }
                    if (cnt == 1)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            flag_OK = false;
                            break;
                        }
                        if (MyTimer_UART_TimeOut.IsTimeOut())
                        {
                            //Console.Write($"station: {station} [{MethodBase.GetCurrentMethod().Name}] 等待逾時! \n");

                            retry++;
                            cnt = 0;
                        }
                        byte[] UART_RX = MySerialPort.ReadByteEx();
                        if (UART_RX != null && UART_RX.Length >= 10)
                        {
                            List<byte> temp = new List<byte>();
                            for (int i = 0; i < 10; i++)
                            {
                                temp.Add(UART_RX[i]);
                            }
                            CRC = Basic.MyConvert.Get_CRC16(UART_RX);
                            if (CRC == 0)
                            {
                                if (UART_RX[1] == (byte)(station) && UART_RX[2] == (byte)(UART_Command.RS485_GetIO))
                                {
                                    byte input_L = UART_RX[3];
                                    byte input_H = UART_RX[4];
                                    byte output_L = UART_RX[5];
                                    byte output_H = UART_RX[6];
                                    input = input_L | (input_H << 8);
                                    output = output_L | (output_H << 8);
                                    if (UART_ConsoletWrite) Console.Write($"{DateTime.Now.ToDateString()} : [{MethodBase.GetCurrentMethod().Name}] Set data  sucessed! station : {station} ,input : {input} ,output : {output}  {myTimerBasic.ToString()}\n {UART_RX.ByteToStringHex()} \n");
                                    flag_OK = true;
                                    break;
                                }
                            }
                            else
                            {
                                Console.Write($"station: {station} [{MethodBase.GetCurrentMethod().Name}] CRC 錯誤, HEX:{UART_RX.ByteToStringHex()} \n");

                                retry++;
                                cnt = 0;
                            }

                        }

                    }

                    System.Threading.Thread.Sleep(0);
                }
                //MySerialPort.SerialPortClose();
                return flag_OK;
            }
            catch
            {
                return false;
            }
            finally
            {
                System.Threading.Thread.Sleep(UART_Delay);
            }
            
        }
        static public bool UART_Command_RS485_SetOutput(MySerialPort MySerialPort, int station, int output)
        {
            try
            {
                bool flag_OK = false;
                MyTimerBasic myTimerBasic = new MyTimerBasic();
                if (MySerialPort.IsConnected == false) MySerialPort.SerialPortOpen();
                MyTimer MyTimer_UART_TimeOut = new MyTimer();

                int retry = 0;
                int cnt = 0;
                MySerialPort.ClearReadByte();
                List<byte> list_byte = new List<byte>();
                list_byte.Add(2);
                list_byte.Add((byte)(station));
                list_byte.Add((byte)(UART_Command.RS485_SetOutput));
                list_byte.Add((byte)(output >> 0));
                list_byte.Add((byte)(output >> 8));
                list_byte.Add(3);
                ushort CRC = Basic.MyConvert.Get_CRC16(list_byte.ToArray());
                list_byte.Add((byte)(CRC >> 0));
                list_byte.Add((byte)(CRC >> 8));
                while (true)
                {
                    if (cnt == 0)
                    {
                        if (retry >= UART_RetryNum)
                        {
                             Console.WriteLine($"[{MethodBase.GetCurrentMethod().Name}] Set data  error! station: {station}  HEX : {list_byte.ToArray().ByteToStringHex()}");
                            flag_OK = false;
                            break;
                        }
                        MySerialPort.ClearReadByte();
                        MySerialPort.WriteByte(list_byte.ToArray());
                        MyTimer_UART_TimeOut.TickStop();
                        MyTimer_UART_TimeOut.StartTickTime(UART_TimeOut);
                        cnt++;
                    }
                    if (cnt == 1)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            flag_OK = false;
                            break;
                        }
                        if (MyTimer_UART_TimeOut.IsTimeOut())
                        {
                            retry++;
                            cnt = 0;
                        }
                        byte[] UART_RX = MySerialPort.ReadByteEx();

                        if (UART_RX != null && UART_RX.Length == 8)
                        {
                            CRC = Basic.MyConvert.Get_CRC16(UART_RX);
                            if (CRC == 0)
                            {

                                if (UART_RX[1] == (byte)(station) && UART_RX[2] == (byte)(UART_Command.RS485_SetOutput))
                                {
                                    if (UART_ConsoletWrite) Console.Write($"{DateTime.Now.ToDateString()} : [{MethodBase.GetCurrentMethod().Name}] Set data  sucessed! station : {station} , {myTimerBasic.ToString()}\n {UART_RX.ByteToStringHex()} \n");
                                    flag_OK = true;
                                    break;
                                }
                            }
                            else
                            {
                                Console.WriteLine($"[{MethodBase.GetCurrentMethod().Name}] station: {station} ,CRC Error");

                                retry++;
                                cnt = 0;
                            }

                        }

                    }

                    System.Threading.Thread.Sleep(0);
                }
                //MySerialPort.SerialPortClose();
                return flag_OK;
            }
            catch
            {
                return false;
            }
            finally
            {
                System.Threading.Thread.Sleep(UART_Delay);
            }
           
        }
        static public bool UART_Command_RS485_SetOutputPIN(MySerialPort MySerialPort, int station, int PIN , bool state)
        {
            try
            {
                bool flag_OK = false;
                MyTimerBasic myTimerBasic = new MyTimerBasic();
                if (MySerialPort.IsConnected == false) MySerialPort.SerialPortOpen();
                MyTimer MyTimer_UART_TimeOut = new MyTimer();

                int retry = 0;
                int cnt = 0;
                MySerialPort.ClearReadByte();
                List<byte> list_byte = new List<byte>();
                list_byte.Add(2);
                list_byte.Add((byte)(station));
                list_byte.Add((byte)(UART_Command.RS485_SetOutputPIN));
                list_byte.Add((byte)(PIN >> 0));
                list_byte.Add((byte)(state ? 1 : 0));
                list_byte.Add(3);
                ushort CRC = Basic.MyConvert.Get_CRC16(list_byte.ToArray());
                list_byte.Add((byte)(CRC >> 0));
                list_byte.Add((byte)(CRC >> 8));
                while (true)
                {
                    if (cnt == 0)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            Console.WriteLine($"[{MethodBase.GetCurrentMethod().Name}] Set data  error! station: {station}  HEX : {list_byte.ToArray().ByteToStringHex()}");
                            flag_OK = false;
                            break;
                        }
                        MySerialPort.ClearReadByte();
                        MySerialPort.WriteByte(list_byte.ToArray());
                        MyTimer_UART_TimeOut.TickStop();
                        MyTimer_UART_TimeOut.StartTickTime(UART_TimeOut);
                        cnt++;
                    }
                    if (cnt == 1)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            flag_OK = false;
                            break;
                        }
                        if (MyTimer_UART_TimeOut.IsTimeOut())
                        {
                            retry++;
                            cnt = 0;
                        }
                        byte[] UART_RX = MySerialPort.ReadByteEx();
                        if (UART_RX != null && UART_RX.Length == 8)
                        {
                            CRC = Basic.MyConvert.Get_CRC16(UART_RX);
                            if (CRC == 0)
                            {
                                if (UART_RX[1] == (byte)(station) && UART_RX[2] == (byte)(UART_Command.RS485_SetOutputPIN))
                                {
                                    if (UART_ConsoletWrite) Console.Write($"{DateTime.Now.ToDateString()} : [{MethodBase.GetCurrentMethod().Name}] Set data  sucessed! station : {station} , {myTimerBasic.ToString()}\n {UART_RX.ByteToStringHex()} \n");
                                    flag_OK = true;
                                    break;
                                }
                            }
                            else
                            {
                                retry++;
                                cnt = 0;
                            }

                        }

                    }

                    System.Threading.Thread.Sleep(0);
                }
                // MySerialPort.SerialPortClose();
                return flag_OK;
            }
            catch
            {

                return false;
            }
            finally
            {
                System.Threading.Thread.Sleep(UART_Delay);
            }
          
        }

        static public bool UART_Command_RS485_GetOutputDir(MySerialPort MySerialPort, int station, ref int output_dir)
        {
            bool flag_OK = false;
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            if (MySerialPort.SerialPortOpen())
            {
                MyTimer MyTimer_UART_TimeOut = new MyTimer();

                int retry = 0;
                int cnt = 0;
                MySerialPort.ClearReadByte();
                List<byte> list_byte = new List<byte>();
                list_byte.Add(2);
                list_byte.Add((byte)(station));
                list_byte.Add((byte)(UART_Command.RS485_GetOutputDir));
                list_byte.Add(3);
                ushort CRC = Basic.MyConvert.Get_CRC16(list_byte.ToArray());
                list_byte.Add((byte)(CRC >> 0));
                list_byte.Add((byte)(CRC >> 8));
                while (true)
                {
                    if (cnt == 0)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            if (UART_ConsoletWrite) Console.Write($"[{MethodBase.GetCurrentMethod().Name}] Set data  error! station: {station}  HEX:{list_byte.ToArray().ByteToStringHex()}\n");
                            flag_OK = false;
                            break;
                        }
                        MySerialPort.ClearReadByte();
                        MySerialPort.WriteByte(list_byte.ToArray());
                        MyTimer_UART_TimeOut.TickStop();
                        MyTimer_UART_TimeOut.StartTickTime(UART_TimeOut);
                        cnt++;
                    }
                    if (cnt == 1)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            flag_OK = false;
                            break;
                        }
                        if (MyTimer_UART_TimeOut.IsTimeOut())
                        {
                            retry++;
                            cnt = 0;
                        }
                        byte[] UART_RX = MySerialPort.ReadByteEx();
                        if (UART_RX != null)
                        {
                            CRC = Basic.MyConvert.Get_CRC16(UART_RX);
                            if (CRC == 0)
                            {
                                if (UART_RX[1] == (byte)(station) && UART_RX[2] == (byte)(UART_Command.RS485_GetOutputDir))
                                {
                                    byte output_dir_L = UART_RX[3];
                                    byte output_dir_H = UART_RX[4];
                                    output_dir = output_dir_L | (output_dir_H << 8);
                                    if (UART_ConsoletWrite) Console.Write($"{DateTime.Now.ToDateString()} : [{MethodBase.GetCurrentMethod().Name}] Set data  sucessed! station : {station} , {myTimerBasic.ToString()}\n {UART_RX.ByteToStringHex()} \n");
                                    flag_OK = true;
                                    break;
                                }
                            }
                            else
                            {
                                retry++;
                                cnt = 0;
                            }

                        }

                    }

                    System.Threading.Thread.Sleep(1);
                }
            }
            MySerialPort.SerialPortClose();
            return flag_OK;
        }
        static public bool UART_Command_RS485_SetOutputDir(MySerialPort MySerialPort, int station, int output_dir)
        {
            bool flag_OK = false;
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            if (MySerialPort.SerialPortOpen())
            {
                MyTimer MyTimer_UART_TimeOut = new MyTimer();

                int retry = 0;
                int cnt = 0;
                MySerialPort.ClearReadByte();
                List<byte> list_byte = new List<byte>();
                list_byte.Add(2);
                list_byte.Add((byte)(station));
                list_byte.Add((byte)(UART_Command.RS485_SetOutputDir));
                list_byte.Add((byte)(output_dir >> 0));
                list_byte.Add((byte)(output_dir >> 8));
                list_byte.Add(3);
                ushort CRC = Basic.MyConvert.Get_CRC16(list_byte.ToArray());
                list_byte.Add((byte)(CRC >> 0));
                list_byte.Add((byte)(CRC >> 8));
                while (true)
                {
                    if (cnt == 0)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            if (UART_ConsoletWrite) Console.Write($"[{MethodBase.GetCurrentMethod().Name}] Set data  error! station: {station}  HEX:{list_byte.ToArray().ByteToStringHex()}\n");
                            flag_OK = false;
                            break;
                        }
                        MySerialPort.ClearReadByte();
                        MySerialPort.WriteByte(list_byte.ToArray());
                        MyTimer_UART_TimeOut.TickStop();
                        MyTimer_UART_TimeOut.StartTickTime(UART_TimeOut);
                        cnt++;
                    }
                    if (cnt == 1)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            flag_OK = false;
                            break;
                        }
                        if (MyTimer_UART_TimeOut.IsTimeOut())
                        {
                            retry++;
                            cnt = 0;
                        }
                        byte[] UART_RX = MySerialPort.ReadByteEx();
                        if (UART_RX != null)
                        {
                            CRC = Basic.MyConvert.Get_CRC16(UART_RX);
                            if (CRC == 0)
                            {
                                if (UART_RX[1] == (byte)(station) && UART_RX[2] == (byte)(UART_Command.RS485_SetOutputDir))
                                {
                
                                    if (UART_ConsoletWrite) Console.Write($"{DateTime.Now.ToDateString()} : [{MethodBase.GetCurrentMethod().Name}] Set data  sucessed! station : {station} , {myTimerBasic.ToString()}\n {UART_RX.ByteToStringHex()} \n");
                                    flag_OK = true;
                                    break;
                                }
                            }
                            else
                            {
                                retry++;
                                cnt = 0;
                            }

                        }

                    }

                    System.Threading.Thread.Sleep(1);
                }
            }
            MySerialPort.SerialPortClose();
            return flag_OK;
        }
        static public bool UART_Command_RS485_GetInputDir(MySerialPort MySerialPort, int station, ref int Input_dir)
        {
            bool flag_OK = false;
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            if (MySerialPort.SerialPortOpen())
            {
                MyTimer MyTimer_UART_TimeOut = new MyTimer();

                int retry = 0;
                int cnt = 0;
                MySerialPort.ClearReadByte();
                List<byte> list_byte = new List<byte>();
                list_byte.Add(2);
                list_byte.Add((byte)(station));
                list_byte.Add((byte)(UART_Command.RS485_GetInputDir));
                list_byte.Add(3);
                ushort CRC = Basic.MyConvert.Get_CRC16(list_byte.ToArray());
                list_byte.Add((byte)(CRC >> 0));
                list_byte.Add((byte)(CRC >> 8));
                while (true)
                {
                    if (cnt == 0)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            if (UART_ConsoletWrite) Console.Write($"[{MethodBase.GetCurrentMethod().Name}] Set data  error! station: {station}  HEX:{list_byte.ToArray().ByteToStringHex()}\n");
                            flag_OK = false;
                            break;
                        }
                        MySerialPort.ClearReadByte();
                        MySerialPort.WriteByte(list_byte.ToArray());
                        MyTimer_UART_TimeOut.TickStop();
                        MyTimer_UART_TimeOut.StartTickTime(UART_TimeOut);
                        cnt++;
                    }
                    if (cnt == 1)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            flag_OK = false;
                            break;
                        }
                        if (MyTimer_UART_TimeOut.IsTimeOut())
                        {
                            retry++;
                            cnt = 0;
                        }
                        byte[] UART_RX = MySerialPort.ReadByteEx();
                        if (UART_RX != null)
                        {
                            CRC = Basic.MyConvert.Get_CRC16(UART_RX);
                            if (CRC == 0)
                            {
                                if (UART_RX[1] == (byte)(station) && UART_RX[2] == (byte)(UART_Command.RS485_GetInputDir))
                                {
                                    byte Input_dir_L = UART_RX[3];
                                    byte Input_dir_H = UART_RX[4];
                                    Input_dir = Input_dir_L | (Input_dir_H << 8);
                                    if (UART_ConsoletWrite) Console.Write($"{DateTime.Now.ToDateString()} : [{MethodBase.GetCurrentMethod().Name}] Set data  sucessed! station : {station} , {myTimerBasic.ToString()}\n {UART_RX.ByteToStringHex()} \n");
                                    flag_OK = true;
                                    break;
                                }
                            }
                            else
                            {
                                retry++;
                                cnt = 0;
                            }

                        }

                    }

                    System.Threading.Thread.Sleep(1);
                }
            }
            MySerialPort.SerialPortClose();
            return flag_OK;
        }
        static public bool UART_Command_RS485_SetInputDir(MySerialPort MySerialPort, int station, int Input_dir)
        {
            bool flag_OK = false;
            MyTimerBasic myTimerBasic = new MyTimerBasic();
            if (MySerialPort.SerialPortOpen())
            {
                MyTimer MyTimer_UART_TimeOut = new MyTimer();

                int retry = 0;
                int cnt = 0;
                MySerialPort.ClearReadByte();
                List<byte> list_byte = new List<byte>();
                list_byte.Add(2);
                list_byte.Add((byte)(station));
                list_byte.Add((byte)(UART_Command.RS485_SetInputDir));
                list_byte.Add((byte)(Input_dir >> 0));
                list_byte.Add((byte)(Input_dir >> 8));
                list_byte.Add(3);
                ushort CRC = Basic.MyConvert.Get_CRC16(list_byte.ToArray());
                list_byte.Add((byte)(CRC >> 0));
                list_byte.Add((byte)(CRC >> 8));
                while (true)
                {
                    if (cnt == 0)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            if (UART_ConsoletWrite) Console.Write($"[{MethodBase.GetCurrentMethod().Name}] Set data  error! station: {station}  HEX:{list_byte.ToArray().ByteToStringHex()}\n");
                            flag_OK = false;
                            break;
                        }
                        MySerialPort.ClearReadByte();
                        MySerialPort.WriteByte(list_byte.ToArray());
                        MyTimer_UART_TimeOut.TickStop();
                        MyTimer_UART_TimeOut.StartTickTime(UART_TimeOut);
                        cnt++;
                    }
                    if (cnt == 1)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            flag_OK = false;
                            break;
                        }
                        if (MyTimer_UART_TimeOut.IsTimeOut())
                        {
                            retry++;
                            cnt = 0;
                        }
                        byte[] UART_RX = MySerialPort.ReadByteEx();
                        if (UART_RX != null && UART_RX.Length >=8)
                        {
                            List<byte> temp = new List<byte>();
                            for (int i = 0; i < 8; i++)
                            {
                                temp.Add(UART_RX[i]);
                            }
                            CRC = Basic.MyConvert.Get_CRC16(temp.ToArray());
                            if (CRC == 0)
                            {
                                if (UART_RX[1] == (byte)(station) && UART_RX[2] == (byte)(UART_Command.RS485_SetInputDir))
                                {

                                    if (UART_ConsoletWrite) Console.Write($"{DateTime.Now.ToDateString()} : [{MethodBase.GetCurrentMethod().Name}] Set data  sucessed! station : {station} , {myTimerBasic.ToString()}\n {UART_RX.ByteToStringHex()} \n");
                                    flag_OK = true;
                                    break;
                                }
                            }
                            else
                            {
                                retry++;
                                cnt = 0;
                            }

                        }

                    }

                    System.Threading.Thread.Sleep(1);
                }
            }
            MySerialPort.SerialPortClose();
            return flag_OK;
        }

        static public bool UART_Command_Get_Setting(MySerialPort MySerialPort, out string IP_Adress, out string Subnet, out string Gateway, out string DNS, out string Server_IP_Adress, out string Local_Port, out string Server_Port, out string SSID, out string Password, out string Station, out string UDP_SendTime ,out string RFID_Enable)
        {
            bool flag_OK = false;
            IP_Adress = "";
            Subnet = "";
            Gateway = "";
            DNS = "";
            Server_IP_Adress = "";
            Local_Port = "";
            Server_Port = "";
            SSID = "";
            Password = "";
            Station = "";
            UDP_SendTime = "";
            RFID_Enable = "";
            if (MySerialPort.SerialPortOpen())
            {
                MyTimer myTimer_Timeout = new MyTimer();
                MySerialPort.ClearReadByte();
                List<byte> list_byte = new List<byte>();
                list_byte.Add(2);
                list_byte.Add((byte)(UART_Command.Get_Setting));
                list_byte.Add(3);
                MyTimer MyTimer_UART_TimeOut = new MyTimer();
                int retry = 0;
                int cnt = 0;
                while (true)
                {
                    if (cnt == 0)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            if (ConsoleWrite) Console.Write("Receive data lenth error!\n");
                            flag_OK = false;
                            break;
                        }
                        MySerialPort.ClearReadByte();
                        MySerialPort.WriteByte(list_byte.ToArray());
                        MyTimer_UART_TimeOut.TickStop();
                        MyTimer_UART_TimeOut.StartTickTime(UART_TimeOut);
                        cnt++;
                    }
                    if (cnt == 1)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            flag_OK = false;
                            break;
                        }
                        if (MyTimer_UART_TimeOut.IsTimeOut())
                        {
                            retry++;
                            cnt = 0;
                        }
                        byte[] UART_RX = MySerialPort.ReadByteEx();
                        if (UART_RX != null)
                        {
                            string str = UART_GetString(UART_RX);
                            if(!str.StringIsEmpty())
                            {
                                string[] str_array = str.Split(',');
                                if (str_array.Length == 12)
                                {
                                    IP_Adress = str_array[0];
                                    Subnet = str_array[1];
                                    Gateway = str_array[2];
                                    DNS = str_array[3];
                                    Server_IP_Adress = str_array[4];
                                    Local_Port = str_array[5];
                                    Server_Port = str_array[6];
                                    SSID = str_array[7];
                                    Password = str_array[8];
                                    Station = str_array[9];
                                    UDP_SendTime = str_array[10];
                                    RFID_Enable = str_array[11];
                                    if (ConsoleWrite) Console.Write("Receive data sucessed!\n");
                                    flag_OK = true;
                                    break;
                                }
                                else
                                {
                                    retry++;
                                    cnt = 0;
                                }
                            }
                     
                        }
                    }
                    System.Threading.Thread.Sleep(1);
                }
            }
            MySerialPort.SerialPortClose();
            return flag_OK;
        }
        static public bool UART_Command_Get_Setting(MySerialPort MySerialPort, out string IP_Adress, out string Subnet, out string Gateway, out string DNS, out string Server_IP_Adress, out string Local_Port, out string Server_Port, out string SSID, out string Password, out string Station, out string UDP_SendTime)
        {
            bool flag_OK = false;
            IP_Adress = "";
            Subnet = "";
            Gateway = "";
            DNS = "";
            Server_IP_Adress = "";
            Local_Port = "";
            Server_Port = "";
            SSID = "";
            Password = "";
            Station = "";
            UDP_SendTime = "";
            if (MySerialPort.SerialPortOpen())
            {
                MyTimer myTimer_Timeout = new MyTimer();
                MySerialPort.ClearReadByte();
                List<byte> list_byte = new List<byte>();
                list_byte.Add(2);
                list_byte.Add((byte)(UART_Command.Get_Setting));
                list_byte.Add(3);
                MyTimer MyTimer_UART_TimeOut = new MyTimer();
                int retry = 0;
                int cnt = 0;
                while (true)
                {
                    if (cnt == 0)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            if (ConsoleWrite) Console.Write("Receive data lenth error!\n");
                            flag_OK = false;
                            break;
                        }
                        MySerialPort.ClearReadByte();
                        MySerialPort.WriteByte(list_byte.ToArray());
                        MyTimer_UART_TimeOut.TickStop();
                        MyTimer_UART_TimeOut.StartTickTime(UART_TimeOut);
                        cnt++;
                    }
                    if (cnt == 1)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            flag_OK = false;
                            break;
                        }
                        if (MyTimer_UART_TimeOut.IsTimeOut())
                        {
                            retry++;
                            cnt = 0;
                        }
                        byte[] UART_RX = MySerialPort.ReadByte();
                        if (UART_RX != null)
                        {
                            string str = UART_GetString(UART_RX);
                            if (!str.StringIsEmpty())
                            {
                                string[] str_array = str.Split(',');
                                if (str_array.Length == 11)
                                {
                                    IP_Adress = str_array[0];
                                    Subnet = str_array[1];
                                    Gateway = str_array[2];
                                    DNS = str_array[3];
                                    Server_IP_Adress = str_array[4];
                                    Local_Port = str_array[5];
                                    Server_Port = str_array[6];
                                    SSID = str_array[7];
                                    Password = str_array[8];
                                    Station = str_array[9];
                                    UDP_SendTime = str_array[10];
                                    if (ConsoleWrite) Console.Write("Receive data sucessed!\n");
                                    flag_OK = true;
                                    break;
                                }
                                else
                                {
                                    retry++;
                                    cnt = 0;
                                }
                            }

                            //if (UART_RX[0] == 2 && UART_RX[UART_RX.Length - 1] == 3)
                            //{
                            //    string str = "";
                            //    for (int i = 1; i < (UART_RX.Length - 1); i++)
                            //    {
                            //        str += (char)UART_RX[i];
                            //    }
                            //    string[] str_array = str.Split(',');
                            //    if (str_array.Length == 11)
                            //    {
                            //        IP_Adress = str_array[0];
                            //        Subnet = str_array[1];
                            //        Gateway = str_array[2];
                            //        DNS = str_array[3];
                            //        Server_IP_Adress = str_array[4];
                            //        Local_Port = str_array[5];
                            //        Server_Port = str_array[6];
                            //        SSID = str_array[7];
                            //        Password = str_array[8];
                            //        Station = str_array[9];
                            //        UDP_SendTime = str_array[10];
                            //        if (ConsoleWrite) Console.Write("Receive data sucessed!\n");
                            //        flag_OK = true;
                            //        break;
                            //    }
                            //    else
                            //    {

                            //        retry++;
                            //        cnt = 0;
                            //    }

                            //}
                        }

                    }

                    System.Threading.Thread.Sleep(1);
                }
            }
            MySerialPort.SerialPortClose();
            return flag_OK;
        }
        static public bool UART_Command_Set_Station(MySerialPort MySerialPort, int station)
        {
            bool flag_OK = false;

            if (MySerialPort.SerialPortOpen())
            {
                MyTimer MyTimer_UART_TimeOut = new MyTimer();
                int retry = 0;
                int cnt = 0;
                byte checksum = 0;
                MySerialPort.ClearReadByte();
                List<byte> list_byte = new List<byte>();
                list_byte.Add(2);
                list_byte.Add((byte)(UART_Command.Set_Station));
                list_byte.Add((byte)(station >> 0));
                list_byte.Add((byte)(station >> 8));
                list_byte.Add(3);

                for (int i = 0; i < list_byte.Count; i++)
                {
                    checksum += list_byte[i];
                }


                while (true)
                {
                    if (cnt == 0)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            if (ConsoleWrite) Console.Write($"Set data error! station: {station}\n");
                            flag_OK = false;
                            break;
                        }
                        MySerialPort.ClearReadByte();
                        MySerialPort.WriteByte(list_byte.ToArray());
                        MyTimer_UART_TimeOut.TickStop();
                        MyTimer_UART_TimeOut.StartTickTime(UART_TimeOut);
                        cnt++;
                    }
                    if (cnt == 1)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            flag_OK = false;
                            break;
                        }
                        if (MyTimer_UART_TimeOut.IsTimeOut())
                        {
                            retry++;
                            cnt = 0;
                        }
                        byte[] UART_RX = MySerialPort.ReadByteEx();
                        if (UART_RX != null)
                        {
                            if (UART_CheckSum(UART_RX, checksum))
                            {
                                if (ConsoleWrite) Console.Write($"Set Data sucessed! station : {station}\n");
                                flag_OK = true;
                                break;

                            }
                        }

                    }

                    System.Threading.Thread.Sleep(1);
                }
            }
            MySerialPort.SerialPortClose();
            return flag_OK;
        }
        static public bool UART_Command_Set_IP_Adress(MySerialPort MySerialPort, string IP)
        {
            bool flag_OK = false;
            if (!IP.Check_IP_Adress())
            {
                return false;
            }
            if (MySerialPort.SerialPortOpen())
            {
                MyTimer MyTimer_UART_TimeOut = new MyTimer();
                int retry = 0;
                int cnt = 0;
                byte checksum = 0;
                byte[] IP_bytes = IP2Bytes(IP);

                MySerialPort.ClearReadByte();
                List<byte> list_byte = new List<byte>();
                list_byte.Add(2);
                list_byte.Add((byte)(UART_Command.Set_IP_Adress));
                list_byte.Add(IP_bytes[0]);
                list_byte.Add(IP_bytes[1]);
                list_byte.Add(IP_bytes[2]);
                list_byte.Add(IP_bytes[3]);
                list_byte.Add(3);

                for (int i = 0; i < list_byte.Count; i++)
                {
                    checksum += list_byte[i];
                }


                while (true)
                {
                    if (cnt == 0)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            if (ConsoleWrite) Console.Write($"Set data error! IP: {IP}\n");
                            flag_OK = false;
                            break;
                        }
                        MySerialPort.ClearReadByte();
                        MySerialPort.WriteByte(list_byte.ToArray());
                        MyTimer_UART_TimeOut.TickStop();
                        MyTimer_UART_TimeOut.StartTickTime(UART_TimeOut);
                        cnt++;
                    }
                    if (cnt == 1)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            flag_OK = false;
                            break;
                        }
                        if (MyTimer_UART_TimeOut.IsTimeOut())
                        {
                            retry++;
                            cnt = 0;
                        }
                        byte[] UART_RX = MySerialPort.ReadByteEx();
                        if (UART_RX != null)
                        {
                            if (UART_CheckSum(UART_RX, checksum))
                            {
                                if (ConsoleWrite) Console.Write($"Set Data sucessed! IP : {IP}\n");
                                flag_OK = true;
                                break;

                            }
                        }

                    }

                    System.Threading.Thread.Sleep(1);
                }
            }
            MySerialPort.SerialPortClose();
            return flag_OK;
        }
        static public bool UART_Command_Set_Subnet(MySerialPort MySerialPort, string IP)
        {
            bool flag_OK = false;
            if (!IP.Check_IP_Adress())
            {
                return false;
            }
            if (MySerialPort.SerialPortOpen())
            {
                MyTimer MyTimer_UART_TimeOut = new MyTimer();
                int retry = 0;
                int cnt = 0;
                byte checksum = 0;
                byte[] IP_bytes = IP2Bytes(IP);

                MySerialPort.ClearReadByte();
                List<byte> list_byte = new List<byte>();
                list_byte.Add(2);
                list_byte.Add((byte)(UART_Command.Set_Subnet));
                list_byte.Add(IP_bytes[0]);
                list_byte.Add(IP_bytes[1]);
                list_byte.Add(IP_bytes[2]);
                list_byte.Add(IP_bytes[3]);
                list_byte.Add(3);

                for (int i = 0; i < list_byte.Count; i++)
                {
                    checksum += list_byte[i];
                }


                while (true)
                {
                    if (cnt == 0)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            if (ConsoleWrite) Console.Write($"Set data error! Subnet: {IP}\n");
                            flag_OK = false;
                            break;
                        }
                        MySerialPort.ClearReadByte();
                        MySerialPort.WriteByte(list_byte.ToArray());
                        MyTimer_UART_TimeOut.TickStop();
                        MyTimer_UART_TimeOut.StartTickTime(UART_TimeOut);
                        cnt++;
                    }
                    if (cnt == 1)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            flag_OK = false;
                            break;
                        }
                        if (MyTimer_UART_TimeOut.IsTimeOut())
                        {
                            retry++;
                            cnt = 0;
                        }
                        byte[] UART_RX = MySerialPort.ReadByteEx();
                        if (UART_RX != null)
                        {
                            if (UART_CheckSum(UART_RX, checksum))
                            {
                                if (ConsoleWrite) Console.Write($"Set Data sucessed! Subnet : {IP}\n");
                                flag_OK = true;
                                break;

                            }
                        }

                    }

                    System.Threading.Thread.Sleep(1);
                }
            }
            MySerialPort.SerialPortClose();
            return flag_OK;
        }
        static public bool UART_Command_Set_Gateway(MySerialPort MySerialPort, string IP)
        {
            bool flag_OK = false;
            if (!IP.Check_IP_Adress())
            {
                return false;
            }
            if (MySerialPort.SerialPortOpen())
            {
                MyTimer MyTimer_UART_TimeOut = new MyTimer();
                int retry = 0;
                int cnt = 0;
                byte checksum = 0;
                byte[] IP_bytes = IP2Bytes(IP);

                MySerialPort.ClearReadByte();
                List<byte> list_byte = new List<byte>();
                list_byte.Add(2);
                list_byte.Add((byte)(UART_Command.Set_Gateway));
                list_byte.Add(IP_bytes[0]);
                list_byte.Add(IP_bytes[1]);
                list_byte.Add(IP_bytes[2]);
                list_byte.Add(IP_bytes[3]);
                list_byte.Add(3);

                for (int i = 0; i < list_byte.Count; i++)
                {
                    checksum += list_byte[i];
                }


                while (true)
                {
                    if (cnt == 0)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            if (ConsoleWrite) Console.Write($"Set data error! Gateway: {IP}\n");
                            flag_OK = false;
                            break;
                        }
                        MySerialPort.ClearReadByte();
                        MySerialPort.WriteByte(list_byte.ToArray());
                        MyTimer_UART_TimeOut.TickStop();
                        MyTimer_UART_TimeOut.StartTickTime(UART_TimeOut);
                        cnt++;
                    }
                    if (cnt == 1)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            flag_OK = false;
                            break;
                        }
                        if (MyTimer_UART_TimeOut.IsTimeOut())
                        {
                            retry++;
                            cnt = 0;
                        }
                        byte[] UART_RX = MySerialPort.ReadByteEx();
                        if (UART_RX != null)
                        {
                            if (UART_CheckSum(UART_RX, checksum))
                            {
                                if (ConsoleWrite) Console.Write($"Set Data sucessed! Gateway : {IP}\n");
                                flag_OK = true;
                                break;

                            }
                        }

                    }

                    System.Threading.Thread.Sleep(1);
                }
            }
            MySerialPort.SerialPortClose();
            return flag_OK;
        }
        static public bool UART_Command_Set_DNS(MySerialPort MySerialPort, string IP)
        {
            bool flag_OK = false;
            if (!IP.Check_IP_Adress())
            {
                return false;
            }
            if (MySerialPort.SerialPortOpen())
            {
                MyTimer MyTimer_UART_TimeOut = new MyTimer();
                int retry = 0;
                int cnt = 0;
                byte checksum = 0;
                byte[] IP_bytes = IP2Bytes(IP);

                MySerialPort.ClearReadByte();
                List<byte> list_byte = new List<byte>();
                list_byte.Add(2);
                list_byte.Add((byte)(UART_Command.Set_DNS));
                list_byte.Add(IP_bytes[0]);
                list_byte.Add(IP_bytes[1]);
                list_byte.Add(IP_bytes[2]);
                list_byte.Add(IP_bytes[3]);
                list_byte.Add(3);

                for (int i = 0; i < list_byte.Count; i++)
                {
                    checksum += list_byte[i];
                }


                while (true)
                {
                    if (cnt == 0)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            if (ConsoleWrite) Console.Write($"Set data error! DNS: {IP}\n");
                            flag_OK = false;
                            break;
                        }
                        MySerialPort.ClearReadByte();
                        MySerialPort.WriteByte(list_byte.ToArray());
                        MyTimer_UART_TimeOut.TickStop();
                        MyTimer_UART_TimeOut.StartTickTime(UART_TimeOut);
                        cnt++;
                    }
                    if (cnt == 1)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            flag_OK = false;
                            break;
                        }
                        if (MyTimer_UART_TimeOut.IsTimeOut())
                        {
                            retry++;
                            cnt = 0;
                        }
                        byte[] UART_RX = MySerialPort.ReadByteEx();
                        if (UART_RX != null)
                        {
                            if (UART_CheckSum(UART_RX, checksum))
                            {
                                if (ConsoleWrite) Console.Write($"Set Data sucessed! DNS : {IP}\n");
                                flag_OK = true;
                                break;

                            }
                        }

                    }

                    System.Threading.Thread.Sleep(1);
                }
            }
            MySerialPort.SerialPortClose();
            return flag_OK;
        }
        static public bool UART_Command_Set_Server_IP_Adress(MySerialPort MySerialPort, string IP)
        {
            bool flag_OK = false;
            if (!IP.Check_IP_Adress())
            {
                return false;
            }
            if (MySerialPort.SerialPortOpen())
            {
                MyTimer MyTimer_UART_TimeOut = new MyTimer();
                int retry = 0;
                int cnt = 0;
                byte checksum = 0;
                byte[] IP_bytes = IP2Bytes(IP);

                MySerialPort.ClearReadByte();
                List<byte> list_byte = new List<byte>();
                list_byte.Add(2);
                list_byte.Add((byte)(UART_Command.Set_Server_IP_Adress));
                list_byte.Add(IP_bytes[0]);
                list_byte.Add(IP_bytes[1]);
                list_byte.Add(IP_bytes[2]);
                list_byte.Add(IP_bytes[3]);
                list_byte.Add(3);

                for (int i = 0; i < list_byte.Count; i++)
                {
                    checksum += list_byte[i];
                }


                while (true)
                {
                    if (cnt == 0)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            if (ConsoleWrite) Console.Write($"Set data error! Server IP: {IP}\n");
                            flag_OK = false;
                            break;
                        }
                        MySerialPort.ClearReadByte();
                        MySerialPort.WriteByte(list_byte.ToArray());
                        MyTimer_UART_TimeOut.TickStop();
                        MyTimer_UART_TimeOut.StartTickTime(UART_TimeOut);
                        cnt++;
                    }
                    if (cnt == 1)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            flag_OK = false;
                            break;
                        }
                        if (MyTimer_UART_TimeOut.IsTimeOut())
                        {
                            retry++;
                            cnt = 0;
                        }
                        byte[] UART_RX = MySerialPort.ReadByteEx();
                        if (UART_RX != null)
                        {
                            if (UART_CheckSum(UART_RX, checksum))
                            {
                                if (ConsoleWrite) Console.Write($"Set Data sucessed! Server IP : {IP}\n");
                                flag_OK = true;
                                break;

                            }
                        }

                    }

                    System.Threading.Thread.Sleep(1);
                }
            }
            MySerialPort.SerialPortClose();
            return flag_OK;
        }
        static public bool UART_Command_Set_Local_Port(MySerialPort MySerialPort, int port)
        {
            bool flag_OK = false;

            if (MySerialPort.SerialPortOpen())
            {
                MyTimer MyTimer_UART_TimeOut = new MyTimer();
                int retry = 0;
                int cnt = 0;
                byte checksum = 0;

                MySerialPort.ClearReadByte();
                List<byte> list_byte = new List<byte>();
                list_byte.Add(2);
                list_byte.Add((byte)(UART_Command.Set_Local_Port));
                list_byte.Add((byte)(port >> 0));
                list_byte.Add((byte)(port >> 8));
                list_byte.Add(3);

                for (int i = 0; i < list_byte.Count; i++)
                {
                    checksum += list_byte[i];
                }


                while (true)
                {
                    if (cnt == 0)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            if (ConsoleWrite) Console.Write($"Set data error! LocalPort : {port}\n");
                            flag_OK = false;
                            break;
                        }
                        MySerialPort.ClearReadByte();
                        MySerialPort.WriteByte(list_byte.ToArray());
                        MyTimer_UART_TimeOut.TickStop();
                        MyTimer_UART_TimeOut.StartTickTime(UART_TimeOut);
                        cnt++;
                    }
                    if (cnt == 1)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            flag_OK = false;
                            break;
                        }
                        if (MyTimer_UART_TimeOut.IsTimeOut())
                        {
                            retry++;
                            cnt = 0;
                        }
                        byte[] UART_RX = MySerialPort.ReadByteEx();
                        if (UART_RX != null)
                        {
                            if (UART_CheckSum(UART_RX, checksum))
                            {
                                if (ConsoleWrite) Console.Write($"Set Data sucessed! LocalPort : {port}\n");
                                flag_OK = true;
                                break;

                            }
                        }

                    }

                    System.Threading.Thread.Sleep(1);
                }
            }
            MySerialPort.SerialPortClose();
            return flag_OK;
        }
        static public bool UART_Command_Set_Server_Port(MySerialPort MySerialPort, int port)
        {
            bool flag_OK = false;

            if (MySerialPort.SerialPortOpen())
            {
                MyTimer MyTimer_UART_TimeOut = new MyTimer();
                int retry = 0;
                int cnt = 0;
                byte checksum = 0;

                MySerialPort.ClearReadByte();
                List<byte> list_byte = new List<byte>();
                list_byte.Add(2);
                list_byte.Add((byte)(UART_Command.Set_Server_Port));
                list_byte.Add((byte)(port >> 0));
                list_byte.Add((byte)(port >> 8));
                list_byte.Add(3);

                for (int i = 0; i < list_byte.Count; i++)
                {
                    checksum += list_byte[i];
                }


                while (true)
                {
                    if (cnt == 0)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            if (ConsoleWrite) Console.Write($"Set data error! ServerPort: {port}\n");
                            flag_OK = false;
                            break;
                        }
                        MySerialPort.ClearReadByte();
                        MySerialPort.WriteByte(list_byte.ToArray());
                        MyTimer_UART_TimeOut.TickStop();
                        MyTimer_UART_TimeOut.StartTickTime(UART_TimeOut);
                        cnt++;
                    }
                    if (cnt == 1)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            flag_OK = false;
                            break;
                        }
                        if (MyTimer_UART_TimeOut.IsTimeOut())
                        {
                            retry++;
                            cnt = 0;
                        }
                        byte[] UART_RX = MySerialPort.ReadByteEx();
                        if (UART_RX != null)
                        {
                            if (UART_CheckSum(UART_RX, checksum))
                            {
                                if (ConsoleWrite) Console.Write($"Set Data sucessed! ServerPort : {port}\n");
                                flag_OK = true;
                                break;

                            }
                        }

                    }

                    System.Threading.Thread.Sleep(1);
                }
            }
            MySerialPort.SerialPortClose();
            return flag_OK;
        }
        static public bool UART_Command_Set_SSID(MySerialPort MySerialPort, string ssid)
        {
            bool flag_OK = false;

            if (MySerialPort.SerialPortOpen())
            {
                MyTimer MyTimer_UART_TimeOut = new MyTimer();
                int retry = 0;
                int cnt = 0;
                byte checksum = 0;
                char[] chars = ssid.ToCharArray();
                MySerialPort.ClearReadByte();
                List<byte> list_byte = new List<byte>();
                list_byte.Add(2);
                list_byte.Add((byte)(UART_Command.Set_SSID));
                for (int i = 0; i < chars.Length; i++)
                {
                    list_byte.Add((byte)chars[i]);
                }
                list_byte.Add(3);

                for (int i = 0; i < list_byte.Count; i++)
                {
                    checksum += list_byte[i];
                }


                while (true)
                {
                    if (cnt == 0)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            if (ConsoleWrite) Console.Write($"Set data error! SSID: {ssid}\n");
                            flag_OK = false;
                            break;
                        }
                        MySerialPort.ClearReadByte();
                        MySerialPort.WriteByte(list_byte.ToArray());
                        MyTimer_UART_TimeOut.TickStop();
                        MyTimer_UART_TimeOut.StartTickTime(UART_TimeOut);
                        cnt++;
                    }
                    if (cnt == 1)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            flag_OK = false;
                            break;
                        }
                        if (MyTimer_UART_TimeOut.IsTimeOut())
                        {
                            retry++;
                            cnt = 0;
                        }
                        byte[] UART_RX = MySerialPort.ReadByteEx();
                        if (UART_RX != null)
                        {
                            if (UART_CheckSum(UART_RX, checksum))
                            {
                                if (ConsoleWrite) Console.Write($"Set Data sucessed! SSID : {ssid}\n");
                                flag_OK = true;
                                break;

                            }
                        }

                    }

                    System.Threading.Thread.Sleep(1);
                }
            }
            MySerialPort.SerialPortClose();
            return flag_OK;
        }
        static public bool UART_Command_Set_Password(MySerialPort MySerialPort, string password)
        {
            bool flag_OK = false;

            if (MySerialPort.SerialPortOpen())
            {
                MyTimer MyTimer_UART_TimeOut = new MyTimer();
                int retry = 0;
                int cnt = 0;
                byte checksum = 0;
                char[] chars = password.ToCharArray();
                MySerialPort.ClearReadByte();
                List<byte> list_byte = new List<byte>();
                list_byte.Add(2);
                list_byte.Add((byte)(UART_Command.Set_Password));
                for (int i = 0; i < chars.Length; i++)
                {
                    list_byte.Add((byte)chars[i]);
                }
                list_byte.Add(3);

                for (int i = 0; i < list_byte.Count; i++)
                {
                    checksum += list_byte[i];
                }


                while (true)
                {
                    if (cnt == 0)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            if (ConsoleWrite) Console.Write($"Set data error! Password: {password}\n");
                            flag_OK = false;
                            break;
                        }
                        MySerialPort.ClearReadByte();
                        MySerialPort.WriteByte(list_byte.ToArray());
                        MyTimer_UART_TimeOut.TickStop();
                        MyTimer_UART_TimeOut.StartTickTime(UART_TimeOut);
                        cnt++;
                    }
                    if (cnt == 1)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            flag_OK = false;
                            break;
                        }
                        if (MyTimer_UART_TimeOut.IsTimeOut())
                        {
                            retry++;
                            cnt = 0;
                        }
                        byte[] UART_RX = MySerialPort.ReadByteEx();
                        if (UART_RX != null)
                        {
                            if (UART_CheckSum(UART_RX, checksum))
                            {
                                if (ConsoleWrite) Console.Write($"Set Data sucessed! Password : {password}\n");
                                flag_OK = true;
                                break;

                            }
                        }

                    }

                    System.Threading.Thread.Sleep(1);
                }
            }
            MySerialPort.SerialPortClose();
            return flag_OK;
        }
        static public bool UART_Command_Set_UDP_SendTime(MySerialPort MySerialPort, int ms)
        {
            bool flag_OK = false;

            if (MySerialPort.SerialPortOpen())
            {
                MyTimer MyTimer_UART_TimeOut = new MyTimer();
                int retry = 0;
                int cnt = 0;
                byte checksum = 0;

                MySerialPort.ClearReadByte();
                List<byte> list_byte = new List<byte>();
                list_byte.Add(2);
                list_byte.Add((byte)(UART_Command.Set_UDP_SendTime));
                list_byte.Add((byte)(ms >> 0));
                list_byte.Add((byte)(ms >> 8));
                list_byte.Add(3);

                for (int i = 0; i < list_byte.Count; i++)
                {
                    checksum += list_byte[i];
                }


                while (true)
                {
                    if (cnt == 0)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            if (ConsoleWrite) Console.Write($"Set data error! UDP SendTime: {ms}\n");
                            flag_OK = false;
                            break;
                        }
                        MySerialPort.ClearReadByte();
                        MySerialPort.WriteByte(list_byte.ToArray());
                        MyTimer_UART_TimeOut.TickStop();
                        MyTimer_UART_TimeOut.StartTickTime(UART_TimeOut);
                        cnt++;
                    }
                    if (cnt == 1)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            flag_OK = false;
                            break;
                        }
                        if (MyTimer_UART_TimeOut.IsTimeOut())
                        {
                            retry++;
                            cnt = 0;
                        }
                        byte[] UART_RX = MySerialPort.ReadByteEx();
                        if (UART_RX != null)
                        {
                            if (UART_CheckSum(UART_RX, checksum))
                            {
                                if (ConsoleWrite) Console.Write($"Set Data sucessed! UDP SendTime : {ms}\n");
                                flag_OK = true;
                                break;

                            }
                        }

                    }

                    System.Threading.Thread.Sleep(1);
                }
            }
            MySerialPort.SerialPortClose();
            return flag_OK;
        }
        static public bool UART_Command_Set_RFID_Enable(MySerialPort MySerialPort, int Disenable)
        {
            bool flag_OK = false;

            if (MySerialPort.SerialPortOpen())
            {
                MyTimer MyTimer_UART_TimeOut = new MyTimer();
                int retry = 0;
                int cnt = 0;
                byte checksum = 0;

                MySerialPort.ClearReadByte();
                List<byte> list_byte = new List<byte>();
                list_byte.Add(2);
                list_byte.Add((byte)(UART_Command.Sset_RFID_Enable));
                list_byte.Add((byte)(Disenable >> 0));
                list_byte.Add(3);

                for (int i = 0; i < list_byte.Count; i++)
                {
                    checksum += list_byte[i];
                }


                while (true)
                {
                    if (cnt == 0)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            if (ConsoleWrite) Console.Write($"Set data error! RFID Disenable: {Disenable}\n");
                            flag_OK = false;
                            break;
                        }
                        MySerialPort.ClearReadByte();
                        MySerialPort.WriteByte(list_byte.ToArray());
                        MyTimer_UART_TimeOut.TickStop();
                        MyTimer_UART_TimeOut.StartTickTime(UART_TimeOut);
                        cnt++;
                    }
                    if (cnt == 1)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            flag_OK = false;
                            break;
                        }
                        if (MyTimer_UART_TimeOut.IsTimeOut())
                        {
                            retry++;
                            cnt = 0;
                        }
                        byte[] UART_RX = MySerialPort.ReadByteEx();
                        if (UART_RX != null)
                        {
                            if (UART_CheckSum(UART_RX, checksum))
                            {
                                if (ConsoleWrite) Console.Write($"Set Data sucessed!  RFID Disenable : {Disenable}\n");
                                flag_OK = true;
                                break;

                            }
                        }

                    }

                    System.Threading.Thread.Sleep(1);
                }
            }
            MySerialPort.SerialPortClose();
            return flag_OK;
        }
        static public bool UART_Command_Get_Version(MySerialPort MySerialPort, out string Version)
        {
            bool flag_OK = false;
            Version = "";
            if (MySerialPort.SerialPortOpen())
            {
                MyTimer myTimer_Timeout = new MyTimer();
                MySerialPort.ClearReadByte();
                List<byte> list_byte = new List<byte>();
                list_byte.Add(2);
                list_byte.Add((byte)(UART_Command.Get_Version));
                list_byte.Add(3);
                MyTimer MyTimer_UART_TimeOut = new MyTimer();
                int retry = 0;
                int cnt = 0;
                while (true)
                {
                    if (cnt == 0)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            if (ConsoleWrite) Console.Write("Receive data lenth error!\n");
                            flag_OK = false;
                            break;
                        }
                        MySerialPort.ClearReadByte();
                        MySerialPort.WriteByte(list_byte.ToArray());
                        MyTimer_UART_TimeOut.TickStop();
                        MyTimer_UART_TimeOut.StartTickTime(UART_TimeOut);
                        cnt++;
                    }
                    if (cnt == 1)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            flag_OK = false;
                            break;
                        }
                        if (MyTimer_UART_TimeOut.IsTimeOut())
                        {
                            retry++;
                            cnt = 0;
                        }
                        byte[] UART_RX = MySerialPort.ReadByteEx();
                        if (UART_RX != null)
                        {
                            string str = "";
                            for (int i = 0; i < (UART_RX.Length); i++)
                            {
                                str += (char)UART_RX[i];
                            }
                            Version = str;
                            if (ConsoleWrite) Console.Write($"Receive data sucessed! {Version}\n");
                            flag_OK = true;
                            break;
                        }

                    }

                    System.Threading.Thread.Sleep(1);
                }
            }
            MySerialPort.SerialPortClose();
            return flag_OK;
        }
        static public bool UART_Command_Set_Locker(MySerialPort MySerialPort, bool value, out string result)
        {
            bool flag_OK = false;
            result = "";
            if (MySerialPort.SerialPortOpen())
            {
                MyTimer myTimer_Timeout = new MyTimer();
                MySerialPort.ClearReadByte();
                List<byte> list_byte = new List<byte>();
                list_byte.Add(2);
                list_byte.Add((byte)(UART_Command.Set_Locker));
                list_byte.Add((byte)(value ? 1 : 0 ));
                list_byte.Add(3);
                MyTimer MyTimer_UART_TimeOut = new MyTimer();
                int retry = 0;
                int cnt = 0;
                while (true)
                {
                    if (cnt == 0)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            if (ConsoleWrite) Console.Write("Receive data lenth error!\n");
                            flag_OK = false;
                            break;
                        }
                        MySerialPort.ClearReadByte();
                        MySerialPort.WriteByte(list_byte.ToArray());
                        MyTimer_UART_TimeOut.TickStop();
                        MyTimer_UART_TimeOut.StartTickTime(UART_TimeOut);
                        cnt++;
                    }
                    if (cnt == 1)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            flag_OK = false;
                            break;
                        }
                        if (MyTimer_UART_TimeOut.IsTimeOut())
                        {
                            retry++;
                            cnt = 0;
                        }
                        byte[] UART_RX = MySerialPort.ReadByteEx();
                        if (UART_RX != null)
                        {
                            string str = "";
                            for (int i = 0; i < (UART_RX.Length); i++)
                            {
                                str += (char)UART_RX[i];
                            }
                            result = str;
                            if (ConsoleWrite) Console.Write($"Receive data sucessed! {result}\n");
                            flag_OK = true;
                            break;
                        }

                    }

                    System.Threading.Thread.Sleep(1);
                }
            }
            MySerialPort.SerialPortClose();
            return flag_OK;
        }
        static public bool UART_Command_Set_WS2812_Buffer(MySerialPort MySerialPort, int start_ptr, byte[] bytes_RGB)
        {
            bool flag_OK = false;
            if (MySerialPort.SerialPortOpen())
            {
                MyTimer myTimer_Timeout = new MyTimer();
                MySerialPort.ClearReadByte();
                List<byte> list_byte = new List<byte>();
                list_byte.Add(2);
                list_byte.Add((byte)(UART_Command.Set_WS2812_Buffer));
                list_byte.Add((byte)start_ptr);
                list_byte.Add((byte)(start_ptr >> 8));
                for (int i = 0; i < bytes_RGB.Length; i++)
                {
                    list_byte.Add(bytes_RGB[i]);
                }
                list_byte.Add(3);
                MyTimer MyTimer_UART_TimeOut = new MyTimer();
                int retry = 0;
                int cnt = 0;
                byte checksum = 0;
                for (int i = 0; i < list_byte.Count; i++)
                {
                    checksum += list_byte[i];
                }
                while (true)
                {
                    if (cnt == 0)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            if (ConsoleWrite) Console.Write("Receive data lenth error!\n");
                            flag_OK = false;
                            break;
                        }
                        MySerialPort.ClearReadByte();
                        MySerialPort.WriteByte(list_byte.ToArray());
                        MyTimer_UART_TimeOut.TickStop();
                        MyTimer_UART_TimeOut.StartTickTime(UART_TimeOut);
                        cnt++;
                    }
                    if (cnt == 1)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            flag_OK = false;
                            break;
                        }
                        if (MyTimer_UART_TimeOut.IsTimeOut())
                        {
                            retry++;
                            cnt = 0;
                        }
                        byte[] UART_RX = MySerialPort.ReadByteEx();
                        if (UART_RX != null)
                        {
                            if (UART_CheckSum(UART_RX, checksum))
                            {
                                if (ConsoleWrite) Console.Write($"Set WS2812_Buffer sucessed!  {DateTime.Now.ToDateTimeString()}\n");
                                flag_OK = true;
                                break;
                            }
                        }

                    }

                    System.Threading.Thread.Sleep(1);
                }
            }
            MySerialPort.SerialPortClose();
            return flag_OK;
        }
        static public bool UART_Command_Set_ClearCanvas(MySerialPort MySerialPort)
        {
            bool flag_OK = false;

            if (MySerialPort.SerialPortOpen())
            {
                MyTimer MyTimer_UART_TimeOut = new MyTimer();
                int retry = 0;
                int cnt = 0;
                byte checksum = 0;
                MySerialPort.ClearReadByte();
                List<byte> list_byte = new List<byte>();
                list_byte.Add(2);
                list_byte.Add((byte)(UART_Command.ClearCanvas));            
                list_byte.Add(3);

                for (int i = 0; i < list_byte.Count; i++)
                {
                    checksum += list_byte[i];
                }


                while (true)
                {
                    if (cnt == 0)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            if (ConsoleWrite) Console.Write($"Set ClearCanvas error! {DateTime.Now.ToDateTimeString()}\n");
                            flag_OK = false;
                            break;
                        }
                        MySerialPort.ClearReadByte();
                        MySerialPort.WriteByte(list_byte.ToArray());
                        MyTimer_UART_TimeOut.TickStop();
                        MyTimer_UART_TimeOut.StartTickTime(UART_TimeOut);
                        cnt++;
                    }
                    if (cnt == 1)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            flag_OK = false;
                            break;
                        }
                        if (MyTimer_UART_TimeOut.IsTimeOut())
                        {
                            retry++;
                            cnt = 0;
                        }
                        byte[] UART_RX = MySerialPort.ReadByteEx();
                        if (UART_RX != null)
                        {
                            if (UART_CheckSum(UART_RX, checksum))
                            {
                                if (ConsoleWrite) Console.Write($"Set ClearCanvas sucessed! {DateTime.Now.ToDateTimeString()}\n");
                                flag_OK = true;
                                break;

                            }
                        }

                    }

                    System.Threading.Thread.Sleep(1);
                }
            }
            MySerialPort.SerialPortClose();
            return flag_OK;
        }
        static public bool UART_Command_EPD_Set_WakeUp(MySerialPort MySerialPort)
        {
            bool flag_OK = false;

            if (MySerialPort.SerialPortOpen())
            {
                MyTimer MyTimer_UART_TimeOut = new MyTimer();
                int retry = 0;
                int cnt = 0;
                byte checksum = 0;
                MySerialPort.ClearReadByte();
                List<byte> list_byte = new List<byte>();
                list_byte.Add(2);
                list_byte.Add((byte)(UART_Command.EPD_Set_WakeUp));
                list_byte.Add(3);

                for (int i = 0; i < list_byte.Count; i++)
                {
                    checksum += list_byte[i];
                }


                while (true)
                {
                    if (cnt == 0)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            if (ConsoleWrite) Console.Write($"UART Set WakeUp error! {DateTime.Now.ToDateTimeString()}\n");
                            flag_OK = false;
                            break;
                        }
                        MySerialPort.ClearReadByte();
                        MySerialPort.WriteByte(list_byte.ToArray());
                        MyTimer_UART_TimeOut.TickStop();
                        MyTimer_UART_TimeOut.StartTickTime(UART_TimeOut);
                        cnt++;
                    }
                    if (cnt == 1)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            flag_OK = false;
                            break;
                        }
                        if (MyTimer_UART_TimeOut.IsTimeOut())
                        {
                            retry++;
                            cnt = 0;
                        }
                        byte[] UART_RX = MySerialPort.ReadByteEx();
                        if (UART_RX != null)
                        {
                            if (UART_CheckSum(UART_RX, checksum))
                            {
                                if (ConsoleWrite) Console.Write($"UART Set WakeUp sucessed! {DateTime.Now.ToDateTimeString()}\n");
                                flag_OK = true;
                                break;

                            }
                        }

                    }

                    System.Threading.Thread.Sleep(1);
                }
            }
            MySerialPort.SerialPortClose();
            return flag_OK;
        }
        static public bool UART_Command_EPD_Send_Framebuffer(MySerialPort MySerialPort, long start_ptr, byte[] value)
        {
            bool flag_OK = false;
            if (MySerialPort.SerialPortOpen())
            {
                MyTimer myTimer_Timeout = new MyTimer();
                MySerialPort.ClearReadByte();
                List<byte> list_byte = new List<byte>();
                list_byte.Add(2);
                list_byte.Add((byte)UART_Command.EPD_Send_Framebuffer);
                list_byte.Add((byte)start_ptr);
                list_byte.Add((byte)(start_ptr >> 8));
                list_byte.Add((byte)(start_ptr >> 16));
                list_byte.Add((byte)(start_ptr >> 24));
                for (int i = 0; i < value.Length; i++)
                {
                    list_byte.Add(value[i]);
                }
                list_byte.Add(3);
           
                MyTimer MyTimer_UART_TimeOut = new MyTimer();
                int retry = 0;
                int cnt = 0;
                byte checksum = 0;
                for (int i = 0; i < list_byte.Count; i++)
                {
                    checksum += list_byte[i];
                }
                while (true)
                {
                    if (cnt == 0)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            if (ConsoleWrite) Console.Write($"Send famebuffer receive data error!\n");
                            flag_OK = false;
                            break;
                        }
                        MySerialPort.ClearReadByte();
                        MySerialPort.WriteByte(list_byte.ToArray());
                        MyTimer_UART_TimeOut.TickStop();
                        MyTimer_UART_TimeOut.StartTickTime(UART_TimeOut);
                        cnt++;
                    }
                    if (cnt == 1)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            flag_OK = false;
                            break;
                        }
                        if (MyTimer_UART_TimeOut.IsTimeOut())
                        {
                            retry++;
                            cnt = 0;
                        }
                        byte[] UART_RX = MySerialPort.ReadByteEx();
                        if (UART_RX != null)
                        {
                            if (UART_CheckSum(UART_RX, checksum))
                            {
                                //if (ConsoleWrite) Console.Write($"Send famebuffer sucessed!  {DateTime.Now.ToDateTimeString()}\n");
                                flag_OK = true;
                                break;
                            }
                        }

                    }

                    System.Threading.Thread.Sleep(1);
                }
            }
            return flag_OK;
        }
        static public bool UART_Command_EPD_DrawFrame_BW(MySerialPort MySerialPort)
        {
            bool flag_OK = false;

            if (MySerialPort.SerialPortOpen())
            {
                MyTimer MyTimer_UART_TimeOut = new MyTimer();
                int retry = 0;
                int cnt = 0;
                byte checksum = 0;
                MySerialPort.ClearReadByte();
                List<byte> list_byte = new List<byte>();
                list_byte.Add(2);
                list_byte.Add((byte)(UART_Command.EPD_DrawFrame_BW));
                list_byte.Add(3);

                for (int i = 0; i < list_byte.Count; i++)
                {
                    checksum += list_byte[i];
                }


                while (true)
                {
                    if (cnt == 0)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            if (ConsoleWrite) Console.Write($"UART drafram BW error! {DateTime.Now.ToDateTimeString()}\n");
                            flag_OK = false;
                            break;
                        }
                        MySerialPort.ClearReadByte();
                        MySerialPort.WriteByte(list_byte.ToArray());
                        MyTimer_UART_TimeOut.TickStop();
                        MyTimer_UART_TimeOut.StartTickTime(UART_TimeOut);
                        cnt++;
                    }
                    if (cnt == 1)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            flag_OK = false;
                            break;
                        }
                        if (MyTimer_UART_TimeOut.IsTimeOut())
                        {
                            retry++;
                            cnt = 0;
                        }
                        byte[] UART_RX = MySerialPort.ReadByteEx();
                        if (UART_RX != null)
                        {
                            if (UART_CheckSum(UART_RX, checksum))
                            {
                                if (ConsoleWrite) Console.Write($"UART drafram BW sucessed! {DateTime.Now.ToDateTimeString()}\n");
                                flag_OK = true;
                                break;

                            }
                        }

                    }

                    System.Threading.Thread.Sleep(1);
                }
            }
            //MySerialPort.SerialPortClose();
            return flag_OK;
        }
        static public bool UART_Command_EPD_DrawFrame_RW(MySerialPort MySerialPort)
        {
            bool flag_OK = false;

            if (MySerialPort.SerialPortOpen())
            {
                MyTimer MyTimer_UART_TimeOut = new MyTimer();
                int retry = 0;
                int cnt = 0;
                byte checksum = 0;
                MySerialPort.ClearReadByte();
                List<byte> list_byte = new List<byte>();
                list_byte.Add(2);
                list_byte.Add((byte)(UART_Command.EPD_DrawFrame_RW));
                list_byte.Add(3);

                for (int i = 0; i < list_byte.Count; i++)
                {
                    checksum += list_byte[i];
                }


                while (true)
                {
                    if (cnt == 0)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            if (ConsoleWrite) Console.Write($"UART drafram RW error! {DateTime.Now.ToDateTimeString()}\n");
                            flag_OK = false;
                            break;
                        }
                        MySerialPort.ClearReadByte();
                        MySerialPort.WriteByte(list_byte.ToArray());
                        MyTimer_UART_TimeOut.TickStop();
                        MyTimer_UART_TimeOut.StartTickTime(UART_TimeOut);
                        cnt++;
                    }
                    if (cnt == 1)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            flag_OK = false;
                            break;
                        }
                        if (MyTimer_UART_TimeOut.IsTimeOut())
                        {
                            retry++;
                            cnt = 0;
                        }
                        byte[] UART_RX = MySerialPort.ReadByteEx();
                        if (UART_RX != null)
                        {
                            if (UART_CheckSum(UART_RX, checksum))
                            {
                                if (ConsoleWrite) Console.Write($"UART drafram RW sucessed! {DateTime.Now.ToDateTimeString()}\n");
                                flag_OK = true;
                                break;

                            }
                        }

                    }

                    System.Threading.Thread.Sleep(1);
                }
            }
            //MySerialPort.SerialPortClose();
            return flag_OK;
        }
        static public bool UART_Command_EPD_RefreshCanvas(MySerialPort MySerialPort)
        {
            bool flag_OK = false;

            if (MySerialPort.SerialPortOpen())
            {
                MyTimer MyTimer_UART_TimeOut = new MyTimer();
                int retry = 0;
                int cnt = 0;
                byte checksum = 0;
                MySerialPort.ClearReadByte();
                List<byte> list_byte = new List<byte>();
                list_byte.Add(2);
                list_byte.Add((byte)(UART_Command.EPD_RefreshCanvas));
                list_byte.Add(3);

                for (int i = 0; i < list_byte.Count; i++)
                {
                    checksum += list_byte[i];
                }


                while (true)
                {
                    if (cnt == 0)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            if (ConsoleWrite) Console.Write($"UART RefreshCanvas error! {DateTime.Now.ToDateTimeString()}\n");
                            flag_OK = false;
                            break;
                        }
                        MySerialPort.ClearReadByte();
                        MySerialPort.WriteByte(list_byte.ToArray());
                        MyTimer_UART_TimeOut.TickStop();
                        MyTimer_UART_TimeOut.StartTickTime(UART_TimeOut);
                        cnt++;
                    }
                    if (cnt == 1)
                    {
                        if (retry >= UART_RetryNum)
                        {
                            flag_OK = false;
                            break;
                        }
                        if (MyTimer_UART_TimeOut.IsTimeOut())
                        {
                            retry++;
                            cnt = 0;
                        }
                        byte[] UART_RX = MySerialPort.ReadByteEx();
                        if (UART_RX != null)
                        {
                            if (UART_CheckSum(UART_RX, checksum))
                            {
                                if (ConsoleWrite) Console.Write($"UART RefreshCanvas sucessed! {DateTime.Now.ToDateTimeString()}\n");
                                flag_OK = true;
                                break;

                            }
                        }

                    }

                    System.Threading.Thread.Sleep(1);
                }
            }
            MySerialPort.SerialPortClose();
            return flag_OK;
        }


        static private bool UART_CheckSum(byte[] UART_RX, byte checksum)
        {
            if (UART_RX.Length == 3)
            {
                string str = "";
                str += (char)UART_RX[0];
                str += (char)UART_RX[1];
                str += (char)UART_RX[2];
                byte temp = 0;

                byte.TryParse(str, out temp);
                if (temp == checksum) return true;

            }
            else
            {
                for (int i = 0; i < UART_RX.Length; i++)
                {
                    if (i + 4 >= UART_RX.Length) continue;
                    if (UART_RX[i] == 2 && UART_RX[i + 4] == 3)
                    {
                        string str = "";
                        str += (char)UART_RX[i + 1];
                        str += (char)UART_RX[i + 2];
                        str += (char)UART_RX[i + 3];
                        byte temp = 0;

                        byte.TryParse(str, out temp);
                        if (temp == checksum) return true;
                    }
                }

            }
            return false;
        }
        static private string UART_GetString(byte[] UART_RX)
        {
            string result = "";
            bool flag_start = false;
            for (int i = 0; i < UART_RX.Length; i++)
            {
                if (flag_start)
                {
                    for (int k = i; k < UART_RX.Length; k++)
                    {
                        if (UART_RX[k] == 3) return result;
                        result += (char)UART_RX[k];
                    }
                    return "";
                }
                if (UART_RX[i] == 2)
                {
                    flag_start = true;
                   
                }
              
            }
            return "";
        }

 
        static public bool UART_EPD_DrawImage(MySerialPort MySerialPort, byte[] BW_data, byte[] RW_data, int Split_DataSize)
        {
            try
            {
                int Width_Size = Split_DataSize;
                int NumOfArray = BW_data.Length / (Split_DataSize);
                List<byte[]> list_BW_data = new List<byte[]>();
                for (int i = 0; i < NumOfArray; i++)
                {
                    byte[] Array_temp = new byte[Width_Size];
                    for (int k = 0; k < Width_Size; k++)
                    {
                        Array_temp[k] = BW_data[i * Width_Size + k];
                    }
                    list_BW_data.Add(Array_temp);
                }
                List<byte[]> list_RW_data = new List<byte[]>();
                for (int i = 0; i < NumOfArray; i++)
                {
                    byte[] Array_temp = new byte[Width_Size];
                    for (int k = 0; k < Width_Size; k++)
                    {
                        Array_temp[k] = RW_data[i * Width_Size + k];
                    }
                    list_RW_data.Add(Array_temp);
                }
                if (ProcessBarEvent != null)
                {
                    ProcessBarEvent(0, 0, "建立通訊中...");
                }
                if (!UART_Command_EPD_Set_WakeUp(MySerialPort))
                {
                    return false;
                }
                if (ProcessBarEvent != null)
                {
                    ProcessBarEvent(0, 0, "建立通訊完成...");
                }
                for (int i = 0; i < NumOfArray; i++)
                {
                    Console.WriteLine($"Write BW frame {i}/{NumOfArray}");
                    if (!UART_Command_EPD_Send_Framebuffer(MySerialPort, i * Width_Size, list_BW_data[i]))
                    {
                        return false;
                    }
                    if (ProcessBarEvent != null)
                    {
                        ProcessBarEvent(i, NumOfArray * 2, "上傳資料...");
                    }
                }
                if (!UART_Command_EPD_DrawFrame_BW(MySerialPort))
                {
                    return false;
                }
                for (int i = 0; i < NumOfArray; i++)
                {
                    Console.WriteLine($"Write RW frame {i}/{NumOfArray}");
                    if (!UART_Command_EPD_Send_Framebuffer(MySerialPort, i * Width_Size, list_RW_data[i]))
                    {
                        return false;
                    }
                    if (ProcessBarEvent != null)
                    {
                        ProcessBarEvent(i + NumOfArray, NumOfArray * 2, "上傳資料...");
                    }
                }
                if (!UART_Command_EPD_DrawFrame_RW(MySerialPort))
                {
                    return false;
                }

                if (!UART_Command_EPD_RefreshCanvas(MySerialPort))
                {
                    return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                MySerialPort.SerialPortClose();

            }
           

        }
        static public bool UART_EPD_290_DrawImage(MySerialPort MySerialPort, Bitmap bmp)
        {
      
            using (Bitmap _bmp = bmp.DeepClone())
            {
                int EPD290_frameDIV = 80;


                bool flag_OK;
                int width = bmp.Width;
                int height = bmp.Height;
                _bmp.RotateFlip(RotateFlipType.Rotate90FlipXY);
                byte[] bytes_BW = new byte[(height / 8) * width];
                byte[] bytes_RW = new byte[(height / 8) * width];
                BitmapToByte(_bmp, ref bytes_BW, ref bytes_RW, EPD_Type.EPD290_V2);
                MyTimer myTimer = new MyTimer();
                myTimer.StartTickTime(50000);
                flag_OK = UART_EPD_DrawImage(MySerialPort, bytes_BW, bytes_RW, (height / 8) * width / EPD290_frameDIV);
                if (ConsoleWrite) Console.WriteLine($"EPD 290 DrawImage {string.Format(flag_OK ? "sucess" : "failed")}!   Time : {myTimer.GetTickTime().ToString("0.000")} ms");
                return flag_OK;
            }
        }
        #endregion
        public enum EPDColors
        {
            EPD_7IN3F_BLACK = 0x0,   // 000
            EPD_7IN3F_WHITE = 0x1,   // 001
            EPD_7IN3F_GREEN = 0x2,   // 010
            EPD_7IN3F_BLUE = 0x3,    // 011
            EPD_7IN3F_RED = 0x4,     // 100
            EPD_7IN3F_YELLOW = 0x5,  // 101
            EPD_7IN3F_ORANGE = 0x6,  // 110
            EPD_7IN3F_CLEAN = 0x7,   // 111   unavailable Afterimage
        }
        static public Bitmap Storage_GetBitmap(Storage storage)
        {
            return Storage_GetBitmap(storage, 1);
        }
        static public Bitmap Storage_GetBitmap(Storage storage , double scale)
        {
            if(storage.Enum_drawType == Storage.enum_DrawType.type1)
            {
                Bitmap bitmap = new Bitmap(storage.PanelSize.Width, storage.PanelSize.Height);
                int Pannel_Width = bitmap.Width;
                int Pannel_Height = bitmap.Height;
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.SmoothingMode = SmoothingMode.HighQuality; //使繪圖質量最高，即消除鋸齒
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
                    storage.BackColor = storage.IsWarning ? Color.Red : Color.White;

                    Rectangle rect = new Rectangle(0, 0, Pannel_Width, Pannel_Height);
                    int Line_Height = (Pannel_Height / 3) * 2;
                    g.FillRectangle(new SolidBrush(storage.BackColor), rect);

                    //this.Graphics_Draw_Bitmap.DrawLine(new Pen(storage.ForeColor, 2), new Point(0, Line_Height), new Point(Pannel_Width, Line_Height));

                    if (storage.BarCode_Height > 40) storage.BarCode_Height = 40;
                    if (storage.BarCode_Width > 120) storage.BarCode_Width = 120;

                    storage.SetValue(Device.ValueName.藥品碼, Device.ValueType.ForeColor, storage.IsWarning ? Color.White : Color.Black);
                    storage.SetValue(Device.ValueName.藥品碼, Device.ValueType.BackColor, storage.BackColor);

                    storage.SetValue(Device.ValueName.藥品名稱, Device.ValueType.ForeColor, storage.IsWarning ? Color.White : Color.Black);
                    storage.SetValue(Device.ValueName.藥品名稱, Device.ValueType.BackColor, storage.BackColor);

                    storage.SetValue(Device.ValueName.藥品學名, Device.ValueType.ForeColor, storage.IsWarning ? Color.White : Color.Black);
                    storage.SetValue(Device.ValueName.藥品學名, Device.ValueType.BackColor, storage.BackColor);

                    storage.SetValue(Device.ValueName.藥品中文名稱, Device.ValueType.ForeColor, storage.IsWarning ? Color.White : Color.Black);
                    storage.SetValue(Device.ValueName.藥品中文名稱, Device.ValueType.BackColor, storage.BackColor);

                    storage.SetValue(Device.ValueName.包裝單位, Device.ValueType.ForeColor, storage.IsWarning ? Color.White : Color.Black);
                    storage.SetValue(Device.ValueName.包裝單位, Device.ValueType.BackColor, storage.BackColor);

                    storage.SetValue(Device.ValueName.效期, Device.ValueType.ForeColor, storage.IsWarning ? Color.White : Color.Black);
                    storage.SetValue(Device.ValueName.效期, Device.ValueType.BackColor, storage.BackColor);

                    storage.SetValue(Device.ValueName.庫存, Device.ValueType.ForeColor, storage.IsWarning ? Color.White : Color.Black);
                    storage.SetValue(Device.ValueName.庫存, Device.ValueType.BackColor, storage.BackColor);

                    float posy = 0;

                    if ((storage.DRUGKIND.StringIsEmpty() == false && storage.DRUGKIND != "N") || storage.IsAnesthetic || storage.IsShapeSimilar || storage.IsSoundSimilar)
                    {
                        int temp_x = 0;
                        g.FillRectangle(new SolidBrush(Color.White), new Rectangle(0, 0, Pannel_Width, 30));
                        if ((storage.DRUGKIND.StringIsEmpty() == false && storage.DRUGKIND != "N"))
                        {
                            DrawHexagonText(g, new Point(temp_x, 0), 30, storage.DRUGKIND, new Font("Arial", 14), Color.White, Color.Black, Color.Red);
                            temp_x += 40;
                        }
                        if (storage.IsAnesthetic)
                        {
                            DrawCircleText(g, new Point(temp_x, 0), 30, "麻", new Font("Arial", 14), Color.White, Color.Black, Color.Red);
                            temp_x += 40;
                        }
                        if (storage.IsShapeSimilar)
                        {
                            DrawSquareText(g, new Point(temp_x, 0), 30, "形", new Font("Arial", 14), Color.Black, Color.Black, Color.White);
                            temp_x += 40;
                        }
                        if (storage.IsSoundSimilar)
                        {
                            DrawSquareText(g, new Point(temp_x, 0), 30, "音", new Font("Arial", 14), Color.Black, Color.Black, Color.White);
                            temp_x += 40;
                        }
                        posy += 30;
                    }



                    if (storage.Name_Visable)
                    {
                        SizeF size_name = g.MeasureString(storage.Name, storage.Name_font, new Size(rect.Width, rect.Height), StringFormat.GenericDefault);
                        size_name = new SizeF((int)size_name.Width, (int)size_name.Height);
                        //SizeF size_name = TextRenderer.MeasureText(g, storage.Name, storage.Name_font, new Size(rect.Width, rect.Height), TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl);
                        g.DrawString(storage.Name, storage.Name_font, new SolidBrush((Color)storage.GetValue(Storage.ValueName.藥品名稱, Storage.ValueType.ForeColor)), new RectangleF(0, posy, Pannel_Width, Pannel_Height), StringFormat.GenericDefault);
                        posy += size_name.Height;
                        DrawingClass.Draw.線段繪製(new PointF(0, posy), new PointF(rect.Width, posy), Color.Black, 1.5F, g, 1, 1);
                    }

                    if (storage.Scientific_Name_Visable)
                    {
                        SizeF size_Scientific_Name = g.MeasureString(storage.Scientific_Name, storage.Scientific_Name_font, new Size(rect.Width, rect.Height), StringFormat.GenericDefault);
                        size_Scientific_Name = new SizeF((int)size_Scientific_Name.Width, (int)size_Scientific_Name.Height);
                        // SizeF size_Scientific_Name_font = TextRenderer.MeasureText(storage.Scientific_Name, storage.Scientific_Name_font, new Size(rect.Width, rect.Height), TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl);
                        g.DrawString(storage.Scientific_Name, storage.Scientific_Name_font, new SolidBrush((Color)storage.GetValue(Storage.ValueName.藥品學名, Storage.ValueType.ForeColor)), new RectangleF(0, posy, Pannel_Width, Pannel_Height), StringFormat.GenericDefault);
                        posy += size_Scientific_Name.Height;
                        DrawingClass.Draw.線段繪製(new PointF(0, posy), new PointF(rect.Width, posy), Color.Black, 1.5F, g, 1, 1);
                    }
                    if (storage.ChineseName_Visable)
                    {
                        SizeF size_ChineseName = g.MeasureString(storage.ChineseName, storage.ChineseName_font, new Size(rect.Width, rect.Height), StringFormat.GenericDefault);
                        size_ChineseName = new SizeF((int)size_ChineseName.Width, (int)size_ChineseName.Height);
                        // SizeF size_ChineseName = TextRenderer.MeasureText(storage.ChineseName, storage.ChineseName_font, new Size(rect.Width, rect.Height), TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl);
                        g.DrawString(storage.ChineseName, storage.ChineseName_font, new SolidBrush((Color)storage.GetValue(Storage.ValueName.藥品中文名稱, Storage.ValueType.ForeColor)), new RectangleF(0, posy, Pannel_Width, Pannel_Height), StringFormat.GenericDefault);
                        posy += size_ChineseName.Height;
                        // DrawingClass.Draw.線段繪製(new PointF(0, posy), new PointF(rect.Width, posy), Color.Black, 1.5F, g, 1, 1);
                    }
                    posy += 3;
                    if (storage.Validity_period_Visable)
                    {
                        for (int i = 0; i < storage.List_Validity_period.Count; i++)
                        {
                            if (storage.List_Inventory[i] == "00") continue;
                            string str = $"{i + 1}.效期 : {storage.List_Validity_period[i]}   庫存 : {storage.List_Inventory[i]}";
                            storage.Validity_period_font = new Font(storage.Validity_period_font, FontStyle.Bold);
                            SizeF size_Validity_period = TextRenderer.MeasureText(str, storage.Validity_period_font);
                            g.DrawString(str, storage.Validity_period_font, new SolidBrush((Color)storage.GetValue(Storage.ValueName.效期, Storage.ValueType.ForeColor)), 5, 0 + posy);
                            Color color_pen = storage.IsWarning ? Color.Black : Color.Red;
                            g.DrawRectangle(new Pen(new SolidBrush(color_pen), 1), 5, 0 + posy, size_Validity_period.Width, size_Validity_period.Height);
                            posy += size_Validity_period.Height;
                        }
                    }

                    SizeF size_Code_font = TextRenderer.MeasureText(storage.Code, storage.Code_font);
                    if (storage.Code_Visable)
                    {
                        g.DrawString(storage.Code, storage.Code_font, new SolidBrush((Color)storage.GetValue(Storage.ValueName.藥品碼, Storage.ValueType.ForeColor)), 0, Pannel_Height - size_Code_font.Height);
                    }


                    SizeF size_Package_font = TextRenderer.MeasureText(storage.Package, storage.Package_font);
                    if (storage.Package_Visable)
                    {
                        DrawStorageString(g, storage, Device.ValueName.包裝單位, 0, Pannel_Height - size_Code_font.Height - size_Package_font.Height);
                    }


                    //g.DrawString(storage.Package, storage.Package_font, new SolidBrush((Color)storage.GetValue(Storage.ValueName.包裝單位, Storage.ValueType.ForeColor)), 0, Pannel_Height - size_Code_font.Height - size_Package_font.Height);
                    //g.DrawRectangle(new Pen(new SolidBrush((Color)storage.GetValue(Storage.ValueName.包裝單位, Storage.ValueType.ForeColor)), 1), 0, Pannel_Height - size_Code_font.Height - size_Package_font.Height, size_Package_font.Width, size_Package_font.Height);




                    string[] ip_array = storage.IP.Split('.');
                    SizeF size_IP = new SizeF();
                    if (ip_array.Length == 4)
                    {
                        string ip = ip_array[2] + "." + ip_array[3];
                        size_IP = TextRenderer.MeasureText(ip, new Font("微軟正黑體", 8, FontStyle.Bold));
                        g.DrawString(ip, new Font("微軟正黑體", 8, FontStyle.Bold), new SolidBrush((Color)storage.GetValue(Storage.ValueName.IP, Storage.ValueType.ForeColor)), (Pannel_Width - size_IP.Width), (Pannel_Height - size_IP.Height));
                    }
                    if (storage.Inventory_Visable)
                    {

                        SizeF size_Inventory = TextRenderer.MeasureText($"[{storage.Inventory}]", storage.Inventory_font);
                        PointF pointF = new PointF((Pannel_Width - size_Inventory.Width - 10), (Pannel_Height - size_IP.Height - size_Inventory.Height));
                        DrawingClass.Draw.方框繪製(pointF, new Size((int)size_Inventory.Width, (int)size_Inventory.Height), Color.Black, 1, false, g, 1, 1);
                        g.DrawString($"[{storage.Inventory}]", storage.Inventory_font, new SolidBrush((Color)storage.GetValue(Storage.ValueName.庫存, Storage.ValueType.ForeColor)), pointF.X, pointF.Y);

                    }

                }
                Bitmap bitmap_buf = null;
                if (storage.DeviceType == DeviceType.EPD266 || storage.DeviceType == DeviceType.EPD266_lock
                    || storage.DeviceType == DeviceType.EPD290 || storage.DeviceType == DeviceType.EPD290_lock
                    || storage.DeviceType == DeviceType.EPD420 || storage.DeviceType == DeviceType.EPD420_lock
                    || storage.DeviceType == DeviceType.EPD213 || storage.DeviceType == DeviceType.EPD213_lock)
                {
                    using (Graphics g_buf = Graphics.FromImage(bitmap))
                    {
                        if (storage.BarCode_Visable)
                        {
                            Bitmap bitmap_barcode = Communication.CreateBarCode($"{storage.Code}", storage.BarCode_Width, storage.BarCode_Height);

                            g_buf.DrawImage(bitmap_barcode, (Pannel_Width - storage.BarCode_Width) / 2, storage.PanelSize.Height - storage.BarCode_Height);
                            bitmap_barcode.Dispose();
                        }
                    }
                    bitmap_buf = Communication.ScaleImage(bitmap, (int)(storage.PanelSize.Width * scale), (int)(storage.PanelSize.Height * scale));
                

                }
            
                bitmap.Dispose();
                bitmap = null;
                return bitmap_buf;
            }
            else
            {
                return Get_Storage_bmp(storage, new StoragePanel.enum_ValueName().GetEnumNames(), scale);
            }
          
        }
        static public Bitmap EPD583_GetBitmap(Drawer drawer)
        {
            Bitmap bitmap = new Bitmap(DrawerUI_EPD_583.Pannel_Width, DrawerUI_EPD_583.Pannel_Height);
            Graphics g = Graphics.FromImage(bitmap);
            List<Box[]> Boxes = drawer.Boxes;
            for (int i = 0; i < Boxes.Count; i++)
            {
                for (int k = 0; k < Boxes[i].Length; k++)
                {
                    Rectangle rect = DrawerUI_EPD_583.Get_Box_Combine(drawer, Boxes[i][k]);
                    Box _box = Boxes[i][k];
                    if (Boxes[i][k].Slave == false)
                    {
                        float posy = 0;
                        Color backgroundColor = (_box.IsWarning ? Color.Red : Color.White);
                        Color foreColor = (_box.IsWarning ? Color.White : Color.Black);
                        g.FillRectangle(new SolidBrush(backgroundColor), rect);
                        g.DrawRectangle(new Pen(Color.Black, _box.Pen_Width), rect);

                        g.SmoothingMode = SmoothingMode.HighQuality; //使繪圖質量最高，即消除鋸齒
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.CompositingQuality = CompositingQuality.HighQuality;
                        g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

                        _box.SetValue(Device.ValueName.藥品碼, Device.ValueType.ForeColor, foreColor);
                        _box.SetValue(Device.ValueName.藥品碼, Device.ValueType.BackColor, backgroundColor);

                        _box.SetValue(Device.ValueName.藥品名稱, Device.ValueType.ForeColor, foreColor);
                        _box.SetValue(Device.ValueName.藥品名稱, Device.ValueType.BackColor, backgroundColor);

                        _box.SetValue(Device.ValueName.藥品學名, Device.ValueType.ForeColor, foreColor);
                        _box.SetValue(Device.ValueName.藥品學名, Device.ValueType.BackColor, backgroundColor);

                        _box.SetValue(Device.ValueName.藥品中文名稱, Device.ValueType.ForeColor, foreColor);
                        _box.SetValue(Device.ValueName.藥品中文名稱, Device.ValueType.BackColor, backgroundColor);

                        _box.SetValue(Device.ValueName.包裝單位, Device.ValueType.ForeColor, foreColor);
                        _box.SetValue(Device.ValueName.包裝單位, Device.ValueType.BackColor, backgroundColor);

                        _box.SetValue(Device.ValueName.效期, Device.ValueType.ForeColor, foreColor);
                        _box.SetValue(Device.ValueName.效期, Device.ValueType.BackColor, backgroundColor);

                        _box.SetValue(Device.ValueName.庫存, Device.ValueType.ForeColor, foreColor);
                        _box.SetValue(Device.ValueName.庫存, Device.ValueType.BackColor, backgroundColor);

                     
                        if ((_box.DRUGKIND.StringIsEmpty() == false && _box.DRUGKIND != "N") || _box.IsAnesthetic || _box.IsShapeSimilar || _box.IsSoundSimilar)
                        {
                            int temp_height = (int)g.MeasureString(_box.Name, _box.Name_font, new Size(10000, 10000), StringFormat.GenericDefault).Height;
                            int temp_x = 2;
                            posy += 2;
                            g.FillRectangle(new SolidBrush(Color.White), new Rectangle(rect.X + temp_x, rect.Y + (int)posy, rect.Width, (int)temp_height));
                            if ((_box.DRUGKIND.StringIsEmpty() == false && _box.DRUGKIND != "N"))
                            {
                                DrawHexagonText(g, new Point(rect.X + temp_x, rect.Y + (int)posy), (int)temp_height, _box.DRUGKIND, new Font("Arial", _box.Name_font.Size), Color.White, Color.Black, Color.Red);
                                temp_x += ((int)temp_height + 5);
                            }
                            if (_box.IsAnesthetic)
                            {
                                DrawCircleText(g, new Point(rect.X + temp_x, rect.Y + (int)posy), (int)temp_height, "麻", new Font("Arial", _box.Name_font.Size), Color.White, Color.Black, Color.Red);
                                temp_x += ((int)temp_height + 5);
                            }
                            if (_box.IsShapeSimilar)
                            {
                                DrawSquareText(g, new Point(rect.X + temp_x, rect.Y + (int)posy), (int)temp_height, "形", new Font("Arial", _box.Name_font.Size), Color.Black, Color.Black, Color.White);
                                temp_x += ((int)temp_height + 5);
                            }
                            if (_box.IsSoundSimilar)
                            {
                                DrawSquareText(g, new Point(rect.X + temp_x, rect.Y + (int)posy), (int)temp_height, "音", new Font("Arial", _box.Name_font.Size), Color.Black, Color.Black, Color.White);
                                temp_x += ((int)temp_height + 5);
                            }
                            posy += (int)temp_height;
                        }

                        SizeF size_Name = g.MeasureString(_box.Name, _box.Name_font, new Size(rect.Width, rect.Height), StringFormat.GenericDefault);
                        size_Name = new SizeF((int)size_Name.Width, (int)size_Name.Height);
                        g.DrawString(_box.Name, _box.Name_font, new SolidBrush(foreColor), new RectangleF(rect.X, rect.Y + posy, rect.Width, rect.Height), StringFormat.GenericDefault);
                        posy += size_Name.Height;
                        posy += 3;

                        if (_box.Validity_period_Visable)
                        {
                            for (int m = 0; m < _box.List_Validity_period.Count; m++)
                            {
                                if (_box.List_Inventory[m] == "00") continue;
                                string str = $"{_box.List_Validity_period[m]} [{_box.List_Inventory[m]}]";
                                _box.Validity_period_font = new Font(_box.Validity_period_font, FontStyle.Bold);
                                SizeF size_Validity_period = TextRenderer.MeasureText(str, _box.Validity_period_font);
                                Color Validity_foreColor = (_box.IsWarning ? Color.White : Color.Black);

                                g.DrawString(str, _box.Validity_period_font, new SolidBrush(Validity_foreColor), rect.X + 5, rect.Y + posy);
                                Color color_pen = _box.IsWarning ? Color.White : Color.Red;
                                g.DrawRectangle(new Pen(new SolidBrush(color_pen), 1), rect.X + 5, rect.Y + posy, size_Validity_period.Width, size_Validity_period.Height);
                                posy += size_Validity_period.Height;
                            }
                        }

                        SizeF size_Code = TextRenderer.MeasureText($"{_box.Code}[{_box.Inventory}]", _box.Code_font);
                        string Code_Inventory = "";
                        if (_box.Code_Visable) Code_Inventory += $"{_box.Code}";
                        if (_box.Inventory_Visable) Code_Inventory += $"[{_box.Inventory}]";
                        if (_box.Code.StringIsEmpty() == false)
                        {
                            if (_box.Code_Visable || _box.Inventory_Visable)
                            {
                                g.DrawString($"{Code_Inventory}", _box.Code_font, new SolidBrush(foreColor), rect.X, ((rect.Y + rect.Height) - size_Code.Height));
                            }

                        }
                    }


                }
            }
            string[] ip_array = drawer.IP.Split('.');
            if (ip_array.Length == 4)
            {
                string ip = ip_array[2] + "." + ip_array[3];
                SizeF size_IP = TextRenderer.MeasureText(ip, new Font("微軟正黑體", 10, FontStyle.Bold));
                g.DrawString(ip, new Font("微軟正黑體", 8, FontStyle.Bold), new SolidBrush(Color.Black), (DrawerUI_EPD_583.Pannel_Width - size_IP.Width), (DrawerUI_EPD_583.Pannel_Height - size_IP.Height));
            }
            g.Dispose();
            return bitmap;
        }
        static public Bitmap EPD730_GetBitmap(Drawer drawer)
        {
            Bitmap bitmap = new Bitmap(800,480);
            Graphics g = Graphics.FromImage(bitmap);
            List<Box[]> Boxes = drawer.Boxes;
            for (int i = 0; i < Boxes.Count; i++)
            {
                for (int k = 0; k < Boxes[i].Length; k++)
                {
                    Rectangle rect = DrawerUI_EPD_583.Get_Box_Combine(drawer, Boxes[i][k]);
                    Box _box = Boxes[i][k];
                    if (Boxes[i][k].Slave == false)
                    {
                        float posy = 0;
     
                        g.FillRectangle(new SolidBrush(_box.BackColor), rect);
                        g.DrawRectangle(new Pen(Color.Black, _box.Pen_Width), rect);

                        g.SmoothingMode = SmoothingMode.HighQuality; //使繪圖質量最高，即消除鋸齒
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.CompositingQuality = CompositingQuality.HighQuality;
                        g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;



                        if ((_box.DRUGKIND.StringIsEmpty() == false && _box.DRUGKIND != "N") || _box.IsAnesthetic || _box.IsShapeSimilar || _box.IsSoundSimilar)
                        {
                            int temp_height = (int)g.MeasureString(_box.Name, _box.Name_font, new Size(10000, 10000), StringFormat.GenericDefault).Height;
                            int temp_x = 2;
                            posy += 2;
                            g.FillRectangle(new SolidBrush(Color.White), new Rectangle(rect.X + temp_x, rect.Y + (int)posy, rect.Width, (int)temp_height));
                            if ((_box.DRUGKIND.StringIsEmpty() == false && _box.DRUGKIND != "N"))
                            {
                                DrawHexagonText(g, new Point(rect.X + temp_x, rect.Y + (int)posy), (int)temp_height, _box.DRUGKIND, new Font("Arial", _box.Name_font.Size), Color.White, Color.Black, Color.Red);
                                temp_x += ((int)temp_height + 5);
                            }
                            if (_box.IsAnesthetic)
                            {
                                DrawCircleText(g, new Point(rect.X + temp_x, rect.Y + (int)posy), (int)temp_height, "麻", new Font("Arial", _box.Name_font.Size), Color.White, Color.Black, Color.Red);
                                temp_x += ((int)temp_height + 5);
                            }
                            if (_box.IsShapeSimilar)
                            {
                                DrawSquareText(g, new Point(rect.X + temp_x, rect.Y + (int)posy), (int)temp_height, "形", new Font("Arial", _box.Name_font.Size), Color.Black, Color.Black, Color.White);
                                temp_x += ((int)temp_height + 5);
                            }
                            if (_box.IsSoundSimilar)
                            {
                                DrawSquareText(g, new Point(rect.X + temp_x, rect.Y + (int)posy), (int)temp_height, "音", new Font("Arial", _box.Name_font.Size), Color.Black, Color.Black, Color.White);
                                temp_x += ((int)temp_height + 5);
                            }
                            posy += (int)temp_height;
                        }

                        SizeF size_Name = g.MeasureString(_box.Name, _box.Name_font, new Size(rect.Width, rect.Height), StringFormat.GenericDefault);
                        size_Name = new SizeF((int)size_Name.Width, (int)size_Name.Height);
                        g.DrawString(_box.Name, _box.Name_font, new SolidBrush(_box.Name_ForeColor), new RectangleF(rect.X, rect.Y + posy, rect.Width, rect.Height), StringFormat.GenericDefault);
                        posy += size_Name.Height;
                        posy += 3;

                        if (_box.Validity_period_Visable)
                        {
                            for (int m = 0; m < _box.List_Validity_period.Count; m++)
                            {
                                if (_box.List_Inventory[m] == "00") continue;
                                string str = $"{_box.List_Validity_period[m]} [{_box.List_Inventory[m]}]";
                                _box.Validity_period_font = new Font(_box.Validity_period_font, FontStyle.Bold);
                                SizeF size_Validity_period = TextRenderer.MeasureText(str, _box.Validity_period_font);
                                Color Validity_foreColor = (_box.IsWarning ? Color.White : Color.Black);

                                g.DrawString(str, _box.Validity_period_font, new SolidBrush(Validity_foreColor), rect.X + 5, rect.Y + posy);
                                Color color_pen = _box.IsWarning ? Color.White : Color.Red;
                                g.DrawRectangle(new Pen(new SolidBrush(color_pen), 1), rect.X + 5, rect.Y + posy, size_Validity_period.Width, size_Validity_period.Height);
                                posy += size_Validity_period.Height;
                            }
                        }

                        SizeF size_Code = TextRenderer.MeasureText($"{_box.Code}[{_box.Inventory}]", _box.Code_font);
                        string Code_Inventory = "";
                        if (_box.Code_Visable) Code_Inventory += $"{_box.Code}";
                        if (_box.Inventory_Visable) Code_Inventory += $"[{_box.Inventory}]";
                        if (_box.Code.StringIsEmpty() == false)
                        {
                            if (_box.Code_Visable || _box.Inventory_Visable)
                            {
                                g.DrawString($"{Code_Inventory}", _box.Code_font, new SolidBrush(_box.Code_ForeColor), rect.X, ((rect.Y + rect.Height) - size_Code.Height));
                            }

                        }
                    }


                }
            }
            string[] ip_array = drawer.IP.Split('.');
            if (ip_array.Length == 4)
            {
                string ip = ip_array[2] + "." + ip_array[3];
                SizeF size_IP = TextRenderer.MeasureText(ip, new Font("微軟正黑體", 8, FontStyle.Bold));
                g.DrawString(ip, new Font("微軟正黑體", 8, FontStyle.Bold), new SolidBrush(Color.Black), (bitmap.Width - size_IP.Width), (bitmap.Height - size_IP.Height));
            }
            g.Dispose();
            return bitmap;
        }

        static public List<byte[]> SplitImage(List<byte> image, int Size)
        {
            List<byte[]> list_byte_image_array = new List<byte[]>();
            List<byte> list_buf = new List<byte>();
            int NumOfArray = image.Count / Size;
            int LastLine = image.Count % Size;
            int index = 0;
            for (int i = 0; i < NumOfArray; i++)
            {
                index = Size * i;
                list_buf.Clear();
                for (int k = 0; k < Size; k++)
                {
                    list_buf.Add(image[index + k]);
                }
                list_byte_image_array.Add(list_buf.ToArray());
            }
            index = Size * NumOfArray;
            list_buf.Clear();
            for (int i = 0; i < LastLine; i++)
            {
                list_buf.Add(image[index + i]);
            }
            list_byte_image_array.Add(list_buf.ToArray());
            return list_byte_image_array;
        }
        static unsafe public List<byte> BitmapToByte(Bitmap bimage)
        {
            List<byte> list_byte_image = new List<byte>();
            //pictureBox1.Image = bimage;
            BitmapData bmData = bimage.LockBits(new Rectangle(0, 0, bimage.Width, bimage.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int width = bmData.Width;
            int height = bmData.Height;
            int ByteOfSkip = GetBitmapSkip(width, 3);
            IntPtr SurfacePtr = bmData.Scan0;
            int ByteOfWidth = width * 3 + ByteOfSkip;
            int SrcWidthxY;
            int SrcIndex;
            int R;
            int G;
            int B;
            int temp;
            int temp_L;
            int temp_H;
            list_byte_image.Clear();
            unsafe
            {
                byte* SrcPtr = (byte*)SurfacePtr;
                for (int y = 0; y < height; y++)
                {
                    SrcWidthxY = ByteOfWidth * y;
                    for (int x = 0; x < width; x++)
                    {
                        SrcIndex = SrcWidthxY + x * 3;
                        B = SrcPtr[SrcIndex];
                        G = SrcPtr[SrcIndex + 1];
                        R = SrcPtr[SrcIndex + 2];

                        R = R >> 3;
                        G = G >> 2;                
                        B = B >> 3;
                        temp = (R << 11) | (G << 5) | B;
                        temp_H = temp & 0x000000FF;
                        temp_L = temp >> 8;
                        list_byte_image.Add((byte)(temp_L));
                        list_byte_image.Add((byte)(temp_H));
                    }
                }
            }
            bimage.UnlockBits(bmData);
            //bimage.Dispose();
            return list_byte_image;
        }
        static unsafe public List<byte> LCD144_BitmapToByte(Bitmap bimage)
        {
            List<byte> list_byte_image = new List<byte>();
            //pictureBox1.Image = bimage;
            BitmapData bmData = bimage.LockBits(new Rectangle(0, 0, bimage.Width, bimage.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int width = bmData.Width;
            int height = bmData.Height;
            int ByteOfSkip = GetBitmapSkip(width, 3);
            IntPtr SurfacePtr = bmData.Scan0;
            int ByteOfWidth = width * 3 + ByteOfSkip;
            int SrcWidthxY;
            int SrcIndex;
            int R;
            int G;
            int B;
            int temp;
            int temp_L;
            int temp_H;
            list_byte_image.Clear();
            unsafe
            {
                byte* SrcPtr = (byte*)SurfacePtr;
                for (int y = 0; y < height; y++)
                {
                    SrcWidthxY = ByteOfWidth * y;
                    for (int x = 0; x < width; x++)
                    {
                        SrcIndex = SrcWidthxY + x * 3;
                        B = SrcPtr[SrcIndex];      // 藍色分量
                        G = SrcPtr[SrcIndex + 1];  // 綠色分量
                        R = SrcPtr[SrcIndex + 2];  // 紅色分量

                        // 根據新的顏色定義進行位移處理
                        B = (B >> 3) & 0x1F;   // 紅色5位
                        R = (R >> 2) & 0x3F;   // 綠色6位
                        G = (G >> 3) & 0x1F;   // 藍色5位

                        // 組合為16位顏色值
                        temp = (B << 11) | (R << 5) | G;

                        // 拆分為兩個byte，低位和高位
                        temp_H = temp & 0x00FF;
                        temp_L = (temp >> 8) & 0x00FF;

                        // 添加到結果列表中
                        list_byte_image.Add((byte)(temp_L));
                        list_byte_image.Add((byte)(temp_H));
                    }
                }
            }
            bimage.UnlockBits(bmData);
            //bimage.Dispose();
            return list_byte_image;
        }

        static unsafe public List<byte> BitmapToByteEx(Bitmap bimage ,ref int len)
        {
            List<byte> list_byte_image = new List<byte>();
            //pictureBox1.Image = bimage;
            BitmapData bmData = bimage.LockBits(new Rectangle(0, 0, bimage.Width, bimage.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int width = bmData.Width;
            int height = bmData.Height;
            int ByteOfSkip = GetBitmapSkip(width, 3);
            IntPtr SurfacePtr = bmData.Scan0;
            int ByteOfWidth = width * 3 + ByteOfSkip;
            int SrcWidthxY;
            int SrcIndex;
            int R;
            int G;
            int B;
            int temp;
            int temp_L;
            int temp_H;
            list_byte_image.Clear();
            unsafe
            {
                byte* SrcPtr = (byte*)SurfacePtr;
                for (int y = 0; y < height; y++)
                {
                    SrcWidthxY = ByteOfWidth * y;
                    for (int x = 0; x < width; x++)
                    {
                        SrcIndex = SrcWidthxY + x * 3;
                        B = SrcPtr[SrcIndex];
                        G = SrcPtr[SrcIndex + 1];
                        R = SrcPtr[SrcIndex + 2];

                        if (R != 0 || G != 0 || B != 0)
                        {
                            list_byte_image.Add(255);
                        }
                        else
                        {
                            list_byte_image.Add(0);
                        }
                    }
                }
            }
            bimage.UnlockBits(bmData);
            //bimage.Dispose();
            list_byte_image = BmpByteToGray(list_byte_image, ref len);
            return list_byte_image;
        }
        static unsafe public string BitmapToHexString(string filename, Hex_Type hex_Type)
        {
            Bitmap bitmap = (Bitmap)Bitmap.FromFile(filename);
            string str = BitmapToHexString(bitmap , hex_Type);
            bitmap.Dispose();
            return str;
        }
        static unsafe public string BitmapToHexString(Bitmap bitmap , Hex_Type hex_Type)
        {
            List<byte> list_bytes = BitmapToByte(bitmap);
            System.Text.StringBuilder sb_uint16s = new StringBuilder();
            int index = 0;
            string temp = "";
            for (int i = 0; i < list_bytes.Count / 2; i++)
            {
                if (hex_Type == Hex_Type.H_L) temp = $"0x{list_bytes[(i * 2) + 1].ToString("X2")}{list_bytes[(i * 2) + 0].ToString("X2")},";
                if (hex_Type == Hex_Type.L_H) temp = $"0x{list_bytes[(i * 2) + 0].ToString("X2")}{list_bytes[(i * 2) + 1].ToString("X2")},";
                sb_uint16s.Append(temp);
                index++;
                if (index >= 20)
                {
                    sb_uint16s.Append($"\n");
                    index = 0;
                }
            }
            return sb_uint16s.ToString().ToUpper();
        }
        static unsafe public void BitmapToHexString(Bitmap bimage, ref string BW, ref string RW, EPD_Type ePD_Type)
        {
            byte[] BW_bytes = new byte[0];
            byte[] RW_bytes = new byte[0];
            BitmapToByte(bimage, ref BW_bytes, ref RW_bytes, ePD_Type);
            BW = "";
            RW = "";
            System.Text.StringBuilder sb_BW = new StringBuilder();
            System.Text.StringBuilder sb_RW = new StringBuilder();
            int index = 0;
            for (int i = 0; i < BW_bytes.Length; i++)
            {
                sb_BW.Append($"0x{BW_bytes[i].ToString("X2")},");
                index++;
                if (index >= 20)
                {
                    sb_BW.Append($"\n");
                    index = 0;
                }         
            }
            BW = sb_BW.ToString().ToUpper();
            for (int i = 0; i < RW_bytes.Length; i++)
            {
                sb_RW.Append($"0x{RW_bytes[i].ToString("X2")},");
                index++;
                if (index >= 20)
                {
                    sb_RW.Append($"\n");
                    index = 0;
                }

            }
            RW = sb_RW.ToString().ToUpper();
        }
        static unsafe public void BitmapToByte(Bitmap bimage, ref byte[] BW_bytes, ref byte[] RW_bytes, EPD_Type ePD_Type)
        {
            List<byte> list_byte_image_BW = new List<byte>();
            List<byte> list_byte_image_RW = new List<byte>();
            //pictureBox1.Image = bimage;
            BitmapData bmData = bimage.LockBits(new Rectangle(0, 0, bimage.Width, bimage.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int width = bmData.Width;
            int height = bmData.Height;
            int ByteOfSkip = GetBitmapSkip(width, 3);
            IntPtr SurfacePtr = bmData.Scan0;
            int ByteOfWidth = width * 3 + ByteOfSkip;
            int SrcWidthxY;
            int SrcIndex;
            int[] R = new int[8];
            int[] G = new int[8];
            int[] B = new int[8];
            byte temp_BW = 0;
            byte temp_RW = 0;
            int flag_index = 0;
            unsafe
            {
                byte* SrcPtr = (byte*)SurfacePtr;
                for (int y = 0; y < height; y++)
                {
                    SrcWidthxY = ByteOfWidth * y;

                    for (int x = 0; x < (width / 8); x++)
                    {
                        SrcIndex = SrcWidthxY + (x * 24);

                        B[0] = SrcPtr[SrcIndex + 00];
                        G[0] = SrcPtr[SrcIndex + 01];
                        R[0] = SrcPtr[SrcIndex + 02];

                        B[1] = SrcPtr[SrcIndex + 03];
                        G[1] = SrcPtr[SrcIndex + 04];
                        R[1] = SrcPtr[SrcIndex + 05];

                        B[2] = SrcPtr[SrcIndex + 06];
                        G[2] = SrcPtr[SrcIndex + 07];
                        R[2] = SrcPtr[SrcIndex + 08];

                        B[3] = SrcPtr[SrcIndex + 09];
                        G[3] = SrcPtr[SrcIndex + 10];
                        R[3] = SrcPtr[SrcIndex + 11];

                        B[4] = SrcPtr[SrcIndex + 12];
                        G[4] = SrcPtr[SrcIndex + 13];
                        R[4] = SrcPtr[SrcIndex + 14];

                        B[5] = SrcPtr[SrcIndex + 15];
                        G[5] = SrcPtr[SrcIndex + 16];
                        R[5] = SrcPtr[SrcIndex + 17];

                        B[6] = SrcPtr[SrcIndex + 18];
                        G[6] = SrcPtr[SrcIndex + 19];
                        R[6] = SrcPtr[SrcIndex + 20];

                        B[7] = SrcPtr[SrcIndex + 21];
                        G[7] = SrcPtr[SrcIndex + 22];
                        R[7] = SrcPtr[SrcIndex + 23];
                        temp_BW = 0;
                        temp_RW = 0;
                        for (int i = 0; i < 8; i++)
                        {
                            temp_BW <<= 1;
                            temp_RW <<= 1;
                            if (ePD_Type == EPD_Type.EPD583 || ePD_Type == EPD_Type.EPD213_B || ePD_Type == EPD_Type.EPD266_B || ePD_Type == EPD_Type.EPD1020)
                            {
                                if (R[i] > 0 && G[i] <= 128 && B[i] <= 128)
                                {
                                    temp_BW |= (byte)(0x00);
                                    temp_RW |= (byte)(0x01);
                                }
                                else if (R[i] > 0 && G[i] > 0 && B[i] >= 0)
                                {
                                    temp_BW |= (byte)(0x01);
                                    temp_RW |= (byte)(0x00);
                                }
                                else if (R[i] == 0 && G[i] == 0 && B[i] == 0)
                                {
                                    temp_BW |= (byte)(0x00);
                                    temp_RW |= (byte)(0x00);
                                }

                            }
                            else if (ePD_Type == EPD_Type.EPD420)
                            {
                                //if (y < 100)
                                //{
                                //    temp_BW |= (byte)(0x00);
                                //    temp_RW |= (byte)(0x00);
                                //}
                                //else if (y > 100 && y < 200)
                                //{
                                //    temp_BW |= (byte)(0x00);
                                //    temp_RW |= (byte)(0x01);
                                //}
                                //else
                                //{
                                //    temp_BW |= (byte)(0x01);
                                //    temp_RW |= (byte)(0x00);
                                //}

                                if (R[i] > 0 && G[i] <= 128 && B[i] <= 128)
                                {
                                    temp_BW |= (byte)(0x00);
                                    temp_RW |= (byte)(0x01);
                                }
                                else if (R[i] > 0 && G[i] > 0 && B[i] >= 0)
                                {
                                    temp_BW |= (byte)(0x01);
                                    temp_RW |= (byte)(0x00);
                                }
                                else if (R[i] == 0 && G[i] == 0 && B[i] == 0)
                                {
                                    temp_BW |= (byte)(0x00);
                                    temp_RW |= (byte)(0x00);
                                }

                           

                            }
                            else if (ePD_Type == EPD_Type.EPD290_V2)
                            {
                                if (R[i] <= 128 && G[i] <= 128 && B[i] <= 128)
                                {
                                    temp_BW |= (byte)(0x00);
                                    temp_RW |= (byte)(0x00);
                                }
                                else if (R[i] >= 128 && G[i] <= 128 && B[i] <= 128)
                                {
                                    temp_BW |= (byte)(0x00);
                                    temp_RW |= (byte)(0x01);
                                }
                                else if (R[i] > 0 && G[i] > 0 && B[i] >= 0)
                                {
                                    temp_BW |= (byte)(0x01);
                                    temp_RW |= (byte)(0x00);
                                }


                            }
                            else if (ePD_Type == EPD_Type.EPD290_V3)
                            {
                                if (R[i] > 0 && G[i] > 0 && B[i] >= 0)
                                {
                                    temp_RW |= (byte)(0x01);
                                    temp_BW |= (byte)(0x01);
                                }
                                else if (R[i] == 0 && G[i] == 0 && B[i] == 0)
                                {
                                    temp_BW |= (byte)(0x01);
                                    temp_RW |= (byte)(0x00);
                                }
                                else if (R[i] > 0 && G[i] <= 128 && B[i] <= 128)
                                {
                                    temp_BW |= (byte)(0x00);
                                    temp_RW |= (byte)(0x00);
                                }
                            }


                        }


                        list_byte_image_BW.Add(temp_BW);
                        list_byte_image_RW.Add(temp_RW);
                        flag_index++;
                    }
                }
            }

            bimage.UnlockBits(bmData);
            BW_bytes = list_byte_image_BW.ToArray();
            RW_bytes = list_byte_image_RW.ToArray();
            //bimage.Dispose();
        }
        static unsafe public void BitmapToByteEPD213(Bitmap bimage, ref byte[] BW_bytes, ref byte[] RW_bytes)
        {
            List<byte> list_byte_image_BW = new List<byte>();
            List<byte> list_byte_image_RW = new List<byte>();
            //pictureBox1.Image = bimage;
            BitmapData bmData = bimage.LockBits(new Rectangle(0, 0, bimage.Width, bimage.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int width = bmData.Width;
            int height = bmData.Height;
            int ByteOfSkip = GetBitmapSkip(width, 3);
            IntPtr SurfacePtr = bmData.Scan0;
            int ByteOfWidth = width * 3 + ByteOfSkip;
            int SrcWidthxY;
            int SrcIndex;
            int[] R = new int[8];
            int[] G = new int[8];
            int[] B = new int[8];
            byte temp_BW = 0;
            byte temp_RW = 0;
            int flag_index = 0;
            unsafe
            {
                byte* SrcPtr = (byte*)SurfacePtr;
                for (int y = 0; y < height; y++)
                {
                    SrcWidthxY = ByteOfWidth * y;

                    for (int x = 0; x < (16); x++)
                    {
                        SrcIndex = SrcWidthxY + (x * 24);

                        B[0] = SrcPtr[SrcIndex + 00];
                        G[0] = SrcPtr[SrcIndex + 01];
                        R[0] = SrcPtr[SrcIndex + 02];

                        B[1] = SrcPtr[SrcIndex + 03];
                        G[1] = SrcPtr[SrcIndex + 04];
                        R[1] = SrcPtr[SrcIndex + 05];

                        B[2] = SrcPtr[SrcIndex + 06];
                        G[2] = SrcPtr[SrcIndex + 07];
                        R[2] = SrcPtr[SrcIndex + 08];

                        B[3] = SrcPtr[SrcIndex + 09];
                        G[3] = SrcPtr[SrcIndex + 10];
                        R[3] = SrcPtr[SrcIndex + 11];

                        B[4] = SrcPtr[SrcIndex + 12];
                        G[4] = SrcPtr[SrcIndex + 13];
                        R[4] = SrcPtr[SrcIndex + 14];

                        B[5] = SrcPtr[SrcIndex + 15];
                        G[5] = SrcPtr[SrcIndex + 16];
                        R[5] = SrcPtr[SrcIndex + 17];

                        B[6] = SrcPtr[SrcIndex + 18];
                        G[6] = SrcPtr[SrcIndex + 19];
                        R[6] = SrcPtr[SrcIndex + 20];

                        B[7] = SrcPtr[SrcIndex + 21];
                        G[7] = SrcPtr[SrcIndex + 22];
                        R[7] = SrcPtr[SrcIndex + 23];
                        temp_BW = 0;
                        temp_RW = 0;
                        for (int i = 0; i < 8; i++)
                        {
                            temp_BW <<= 1;
                            temp_RW <<= 1;
                            if (R[i] > 0 && G[i] <= 128 && B[i] <= 128)
                            {
                                temp_BW |= (byte)(0x00);
                                temp_RW |= (byte)(0x01);
                            }
                            else if (R[i] > 0 && G[i] > 0 && B[i] >= 0)
                            {
                                temp_BW |= (byte)(0x01);
                                temp_RW |= (byte)(0x00);
                            }
                            else if (R[i] == 0 && G[i] == 0 && B[i] == 0)
                            {
                                temp_BW |= (byte)(0x00);
                                temp_RW |= (byte)(0x00);
                            }


                        }


                        list_byte_image_BW.Add(temp_BW);
                        list_byte_image_RW.Add(temp_RW);
                        flag_index++;
                    }
                }
            }

            bimage.UnlockBits(bmData);
            BW_bytes = list_byte_image_BW.ToArray();
            RW_bytes = list_byte_image_RW.ToArray();
            //bimage.Dispose();
        }

        static bool IsEqualColor(Color color, int R, int G, int B)
        {
            int offset = 5;
            int colorR_L = color.R - offset;
            int colorG_L = color.G - offset;
            int colorB_L = color.B - offset;

            int colorR_H = color.R + offset;
            int colorG_H = color.G + offset;
            int colorB_H = color.B + offset;

            if (colorR_L < 0) colorR_L = 0;
            if (colorG_L < 0) colorG_L = 0;
            if (colorB_L < 0) colorB_L = 0;

            if (colorR_H > 255) colorR_H = 255;
            if (colorG_H > 255) colorG_H = 255;
            if (colorB_H > 255) colorB_H = 255;
            //if (color.R == R && color.G == G && color.B == B) return true;
            //return false;
            if (R < colorR_L || R > colorR_H) return false;
            if (G < colorG_L || G > colorG_H) return false;
            if (B < colorB_L || B > colorB_H) return false;

            return true;
        }
        static unsafe public void BitmapToByte(Bitmap bimage, ref byte[] _7colors_bytes, EPD_Type ePD_Type)
        {
            List<byte> list_bytes = new List<byte>();
            //pictureBox1.Image = bimage;
            BitmapData bmData = bimage.LockBits(new Rectangle(0, 0, bimage.Width, bimage.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int width = bmData.Width;
            int height = bmData.Height;
            int ByteOfSkip = GetBitmapSkip(width, 3);
            IntPtr SurfacePtr = bmData.Scan0;
            int ByteOfWidth = width * 3 + ByteOfSkip;
            int SrcWidthxY;
            int SrcIndex;
            int[] R = new int[8];
            int[] G = new int[8];
            int[] B = new int[8];
            byte temp = 0;
            byte temp_L = 0;
            byte temp_H = 0;
            int flag_index = 0;
            unsafe
            {
                byte* SrcPtr = (byte*)SurfacePtr;
                for (int y = 0; y < height; y++)
                {
                    SrcWidthxY = ByteOfWidth * y;
                    for (int x = 0; x < (width / 2); x++)
                    {
                        if (ePD_Type == EPD_Type.EPD730F)
                        {
                            SrcIndex = SrcWidthxY + (x * 6);

                            B[0] = SrcPtr[SrcIndex + 00];
                            G[0] = SrcPtr[SrcIndex + 01];
                            R[0] = SrcPtr[SrcIndex + 02];

                            B[1] = SrcPtr[SrcIndex + 03];
                            G[1] = SrcPtr[SrcIndex + 04];
                            R[1] = SrcPtr[SrcIndex + 05];
                            temp_L = 0;
                            temp_H = 0;
                            if (IsEqualColor(Color.White, R[0], G[0], B[0]) == true) temp_L = (int)EPDColors.EPD_7IN3F_WHITE;
                            else if (IsEqualColor(Color.Yellow, R[0], G[0], B[0]) == true) temp_L = (int)EPDColors.EPD_7IN3F_YELLOW;
                            else if (IsEqualColor(Color.Red, R[0], G[0], B[0]) == true) temp_L = (int)EPDColors.EPD_7IN3F_RED;
                            else if (IsEqualColor(Color.Green, R[0], G[0], B[0]) == true) temp_L = (int)EPDColors.EPD_7IN3F_GREEN;
                            else if (IsEqualColor(Color.Blue, R[0], G[0], B[0]) == true) temp_L = (int)EPDColors.EPD_7IN3F_BLUE;
                            else if (IsEqualColor(Color.Orange, R[0], G[0], B[0]) == true) temp_L = (int)EPDColors.EPD_7IN3F_ORANGE;
                            else if (IsEqualColor(Color.Black, R[0], G[0], B[0]) == true) temp_L = (int)EPDColors.EPD_7IN3F_BLACK;
                            else
                            {

                            }

                            if (IsEqualColor(Color.White, R[1], G[1], B[1]) == true) temp_H = (int)EPDColors.EPD_7IN3F_WHITE;
                            else if (IsEqualColor(Color.Yellow, R[1], G[1], B[1]) == true) temp_H = (int)EPDColors.EPD_7IN3F_YELLOW;
                            else if (IsEqualColor(Color.Red, R[1], G[1], B[1]) == true) temp_H = (int)EPDColors.EPD_7IN3F_RED;
                            else if (IsEqualColor(Color.Green, R[1], G[1], B[1]) == true) temp_H = (int)EPDColors.EPD_7IN3F_GREEN;
                            else if (IsEqualColor(Color.Blue, R[1], G[1], B[1]) == true) temp_H = (int)EPDColors.EPD_7IN3F_BLUE;
                            else if (IsEqualColor(Color.Orange, R[1], G[1], B[1]) == true) temp_H = (int)EPDColors.EPD_7IN3F_ORANGE;
                            else if (IsEqualColor(Color.Black, R[1], G[1], B[1]) == true) temp_H = (int)EPDColors.EPD_7IN3F_BLACK;
                            else
                            {

                            }
               
                            temp = (byte)(temp_H | (temp_L << 4));
                            list_bytes.Add(temp);
                        }
                       
                        
                    }
                }
            }

            bimage.UnlockBits(bmData);
            _7colors_bytes = list_bytes.ToArray();
            //bimage.Dispose();
        }
        static private List<byte> BmpByteToGray(List<byte> list_bmp_Byte, ref int len)
        {
            List<byte> list_bmp_Byte_buf = new List<byte>();
            int indexOfbit = 0;
            len = 0;
            byte temp = 0;
            for (int i = 0; i < list_bmp_Byte.Count; i++)
            {
                if (indexOfbit == 8)
                {
                    list_bmp_Byte_buf.Add(temp);
                    indexOfbit = 0;
                    temp = 0;
                }
                if (list_bmp_Byte[i] > 0)
                {
                    temp |= (byte)(1 << indexOfbit);
                }
               
                indexOfbit++;
                len++;
            }
            
            if (indexOfbit > 0)
            {
                list_bmp_Byte_buf.Add(temp);
            }
            
            return list_bmp_Byte_buf;
        }
        static private int GetBitmapSkip(int ByteOfNumWidth, int ColorDepth)
        {
            // 計算每行的總字節數
            int totalBytesPerLine = ByteOfNumWidth * ColorDepth;

            // 計算不滿4字節的餘數
            int remainder = totalBytesPerLine % 4;

            // 計算需要補齊的字節數
            int byteOfSkip = (remainder > 0) ? (4 - remainder) : 0;

            return byteOfSkip;
        }
        static public Bitmap TextToBitmap(string text, Font font)
        {
            Size string_size = TextRenderer.MeasureText(text, font);
            return TextToBitmap(text, font, 1, string_size.Width, string_size.Height, Color.Black, Color.White, 0, Color.Black, HorizontalAlignment.Left);
        }
        static public Bitmap TextToBitmap(string text, Font font, Color foreColor, Color backColor)
        {
            Size string_size = TextRenderer.MeasureText(text, font);
            return TextToBitmap(text, font, 1, string_size.Width, string_size.Height, foreColor, backColor, 0, Color.Black, HorizontalAlignment.Left);
        }
        static public Bitmap TextToBitmap(string text, Font font,double scale ,int width,int height, Color foreColor, Color backColor, int borderSize , Color borderColor ,HorizontalAlignment horizontalAlignment)
        {
            if (text.StringIsEmpty())
            {
                text = "";
            }

            //Size size = DrawingClass.Draw.MeasureText(text, font);
    
            //if (width < size.Width) width = size.Width;
            //if (height < size.Height) height = size.Height;

            int _Width = (int)(width * scale);
            int _Height = (int)(height * scale);
            if (_Width == 0 || _Height == 0) return null;
       
            Bitmap bitmap = new Bitmap(_Width, _Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {

                g.SmoothingMode = SmoothingMode.HighQuality; //使繪圖質量最高，即消除鋸齒
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

               if(borderSize > 0) DrawPath(g, _Width, _Height, 1, borderSize, borderColor, backColor);
                Font _font = new Font(font.Name, (int)(font.Size * scale), font.Style, font.Unit);


                //g.FillRectangle(new SolidBrush(backColor), 0, 0, _Width, _Height);
               
                Size Rect_Size = new Size(_Width, _Height);
                Size string_size = MeasureMultilineText(text, _font, Rect_Size );
                Point string_point = new Point(0, (Rect_Size.Height - string_size.Height) / 2);
                Rectangle rectangle = new Rectangle(string_point, new Size(_Width, _Height));
                if (horizontalAlignment == HorizontalAlignment.Left)
                {
                    string_point.X = 0;
                }
                else if (horizontalAlignment == HorizontalAlignment.Center)
                {
                    string_point.X = (Rect_Size.Width - string_size.Width) / 2;
                }
                else if (horizontalAlignment == HorizontalAlignment.Right)
                {
                    string_point.X = (Rect_Size.Width - string_size.Width);
                }
                g.DrawString(text, _font, new SolidBrush(foreColor), rectangle, StringFormat.GenericDefault);


                //if (borderSize > 0)
                //{                
                //    g.DrawRectangle(new Pen(borderColor, borderSize), borderSize / 2, borderSize / 2, (int)(Rect_Size.Width - borderSize), (int)(Rect_Size.Height - borderSize));
                //}
              

            }
            return bitmap;
        }
        static public Size MeasureMultilineText(string text, Font font, Size maxSize)
        {
            SizeF stringSize;
            using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
            {
                // 定義格式，讓文字能夠換行
                StringFormat format = new StringFormat(StringFormat.GenericTypographic)
                {
                    Trimming = StringTrimming.Word,
                    FormatFlags = StringFormatFlags.LineLimit
                };

                // 設定最大尺寸，這樣文字會在達到這個尺寸時換行
                RectangleF layoutRectangle = new RectangleF(0, 0, maxSize.Width, maxSize.Height);

                // 使用Graphics.MeasureString來測量文字尺寸
                stringSize = g.MeasureString(text, font, layoutRectangle.Size, format);
            }

            return Size.Ceiling(stringSize);
        }
        static public Bitmap CreateBarCode(string content, int Width, int Height)
        {
            if (content.StringIsEmpty()) content = "None";
            ZXing.BarcodeWriter barcodeWriter = new ZXing.BarcodeWriter
            {
                Format = ZXing.BarcodeFormat.CODE_128,
                Options = new ZXing.Common.EncodingOptions
                {
                    Height = Height,
                    Width = Width,
                    PureBarcode = true,
                    Margin = 0
                }
            };
            var barCode = barcodeWriter.Write(content);
            return barCode;
        }
        static public Bitmap CreateQRCode(string content, int Width, int Height)
        {
            if (content.StringIsEmpty()) content = "None";
            ZXing.BarcodeWriter barcodeWriter = new ZXing.BarcodeWriter
            {
                Format = ZXing.BarcodeFormat.QR_CODE,
                Options = new ZXing.Common.EncodingOptions
                {
                    Height = Height,
                    Width = Width,
                    PureBarcode = true,
                    Margin = 0
                }
            };
            var barCode = barcodeWriter.Write(content);
            return barCode;
        }
        static public void DrawPath(Graphics g ,int width , int height ,int borderRadius , int borderSize, Color borderColor ,Color backcolor)
        {
            RectangleF rectSurface = new RectangleF(0, 0, width, height);
            RectangleF rectBorder = new RectangleF(1, 1, width - 0.8F, height - 1);

            if (borderRadius > 2)
            {
                using (GraphicsPath pathSurface = GetFigurePath(rectSurface, borderRadius))
                using (GraphicsPath pathBorder = GetFigurePath(rectBorder, borderRadius - 1F))
                using (Pen penSurface = new Pen(backcolor, 2))
                using (Pen penBorder = new Pen(borderColor, borderSize))
                {
                    penBorder.Alignment = PenAlignment.Inset;
                    g.DrawPath(penSurface, pathSurface);
                    g.FillPath(new SolidBrush(backcolor), pathBorder);
                    if (borderSize >= 1)
                    {
                        g.DrawPath(penBorder, pathBorder);
                      
                    }
                }
            }
            else
            {
                if (borderRadius >= 1)
                {
                    using (Pen penBorder = new Pen(borderColor, borderSize))
                    {
                        penBorder.Alignment = PenAlignment.Inset;
                        g.FillRectangle(new SolidBrush(backcolor), 0, 0, width - 1, height - 1);
                        g.DrawRectangle(penBorder, 0, 0, width - 1, height - 1);
                    }
                }
            }
        }
        static private GraphicsPath GetFigurePath(RectangleF rect, float radius)
        {
            GraphicsPath path = new GraphicsPath();
            if (radius < 0) radius = 0;
            path.StartFigure();
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Width - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Width - radius, rect.Height - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Height - radius, radius, radius, 90, 90);
            path.CloseFigure();

            return path;
        }
        static private byte[] IP2Bytes(string IP)
        {
            byte[] bytes = new byte[4];
            string[] str = IP.Split('.');
            if (str.Length == 4)
            {
                byte.TryParse(str[0], out bytes[0]);
                byte.TryParse(str[1], out bytes[1]);
                byte.TryParse(str[2], out bytes[2]);
                byte.TryParse(str[3], out bytes[3]);
            }
            return bytes;
        }
        public static Bitmap ScaleImage(Bitmap SrcBitmap, int dstWidth, int dstHeight)
        {
            Graphics g = null;
            try
            {
                Bitmap DstBitmap = new Bitmap(dstWidth, dstHeight);
                //按比例缩放           
                int width = (int)(SrcBitmap.Width * ((float)dstWidth / (float)SrcBitmap.Width));
                int height = (int)(SrcBitmap.Height * ((float)dstHeight / (float)SrcBitmap.Height));


                g = Graphics.FromImage(DstBitmap);
                g.Clear(Color.Transparent);

                //设置画布的描绘质量         
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(SrcBitmap, new Rectangle((width - width) / 2, (height - height) / 2, width, height), 0, 0, SrcBitmap.Width, SrcBitmap.Height, GraphicsUnit.Pixel);


                return DstBitmap;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (g != null)
                {
                    g.Dispose();
                }

            }
            return null;
        }
        public static void DrawStorageString(Graphics g, Device storage, Device.ValueName valueName, float x, float y)
        {
            string str = (string)storage.GetValue(valueName, Device.ValueType.Value);
            Font font = (Font)storage.GetValue(valueName, Device.ValueType.Font);
            Color foreColor = (Color)storage.GetValue(valueName, Device.ValueType.ForeColor);
            int borderSize = (int)storage.GetValue(valueName, Device.ValueType.BorderSize);
            Color borderColor = (Color)storage.GetValue(valueName, Device.ValueType.BorderColor);

            DrawStorageString(g, str, font, foreColor, borderSize, borderColor, x, y);
        }
        public static void DrawStorageString(Graphics g, string str, Font font, Color ForeColor, int BorderSize, Color BorderColor, float x, float y)
        {
            SizeF size_font = TextRenderer.MeasureText(str, font);

            g.SmoothingMode = SmoothingMode.HighQuality; //使繪圖質量最高，即消除鋸齒
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

            g.DrawString(str, font, new SolidBrush(ForeColor), x, y);
            if (BorderSize > 0)
            {
                Pen pen = new Pen(new SolidBrush(BorderColor), 1);
                g.DrawRectangle(pen, x, y, size_font.Width, size_font.Height);
            }
        }

        public static void DrawCircleText(Graphics g, Point topLeft ,int size, string text, Font font, Color textCodor, Color borderCodor, Color backCodor)
        {
            // 設定圓形外框顏色
            Pen whitePen = new Pen(borderCodor, 2);

            // 設定圓形填充顏色
            Brush redBrush = new SolidBrush(backCodor);


            // 設定文字顏色
            Brush textBrush = new SolidBrush(textCodor);

            // 計算文字的寬度和高度
            SizeF textSize = g.MeasureString(text, font);

            // 計算圓形的位置
            int centerX = topLeft.X + size / 2;
            int centerY = topLeft.Y + size / 2;

            // 繪製圓形外框
            g.DrawEllipse(whitePen, topLeft.X, topLeft.Y, size, size);

            // 繪製填充紅色的圓形
            g.FillEllipse(redBrush, topLeft.X, topLeft.Y, size, size);

            // 計算文字的位置使其置中於圓形內部
            float textX = centerX - (textSize.Width / 2);
            float textY = centerY - (textSize.Height / 2);

            // 繪製文字
            g.DrawString(text, font, textBrush, textX, textY);
        }
        public static void DrawHexagonText(Graphics g, Point topLeft, int size, string text, Font font, Color textCodor, Color borderCodor, Color backCodor)
        {
            // 設定六邊形外框顏色
            Pen whitePen = new Pen(borderCodor, 2);

            // 設定六邊形填充顏色
            Brush redBrush = new SolidBrush(backCodor);


            // 設定文字顏色
            Brush textBrush = new SolidBrush(textCodor);

            // 計算文字的寬度和高度
            SizeF textSize = g.MeasureString(text, font);

            // 計算六邊形的六個頂點
            PointF[] hexagonVertices = new PointF[6];
            float angle = 360f / 6;

            for (int i = 0; i < 6; i++)
            {
                float x = topLeft.X + size / 2 + (size / 2) * (float)Math.Cos(Math.PI / 180 * (angle * i));
                float y = topLeft.Y + size / 2 + (size / 2) * (float)Math.Sin(Math.PI / 180 * (angle * i));
                hexagonVertices[i] = new PointF(x, y);
            }

            // 繪製六邊形外框
            g.DrawPolygon(whitePen, hexagonVertices);

            // 繪製填充紅色的六邊形
            g.FillPolygon(redBrush, hexagonVertices);

            // 計算文字的位置使其置中於六邊形內部
            float textX = topLeft.X + size / 2 - (textSize.Width / 2);
            float textY = topLeft.Y + size / 2 - (textSize.Height / 2);

            // 繪製文字
            g.DrawString(text, font, textBrush, textX, textY);
        }
        public static void DrawSquareText(Graphics g, Point topLeft, int size, string text, Font font, Color textCodor, Color borderCodor, Color backCodor)
        {
            // 設定正方形外框顏色
            Pen whitePen = new Pen(borderCodor, 2);

            // 設定正方形填充顏色
            Brush redBrush = new SolidBrush(backCodor);


            // 設定文字顏色
            Brush textBrush = new SolidBrush(textCodor);

            // 計算文字的寬度和高度
            SizeF textSize = g.MeasureString(text, font);

            // 繪製正方形外框
            g.DrawRectangle(whitePen, topLeft.X, topLeft.Y, size, size);

            // 繪製填充紅色的正方形
            g.FillRectangle(redBrush, topLeft.X, topLeft.Y, size, size);

            // 計算文字的位置使其置中於正方形內部
            float textX = topLeft.X + (size - textSize.Width) / 2;
            float textY = topLeft.Y + (size - textSize.Height) / 2;

            // 繪製文字
            g.DrawString(text, font, textBrush, textX, textY);
        }
    }
}
