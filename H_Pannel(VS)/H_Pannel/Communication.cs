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
    public enum ChipType
    {
        ESP32,
        BW16,
    }
    public class Communication
    {
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

            SendTestData = (byte)'Z',

            EPD_Set_Sleep = (byte)'a',
            EPD_Set_WakeUp = (byte)'b',
            EPD_DrawFrame_RW = (byte)'c',
            EPD_DrawFrame_BW = (byte)'d',
            EPD_Send_Framebuffer = (byte)'e',
            EPD_RefreshCanvas = (byte)'f',
            EPD_Get_LaserDistance = (byte)'g',
           
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
            return Command_Set_WS2812_Buffer(uDP_Class, IP, start_ptr, bytes_RGB);
        }
        static public byte[] Get_WS2812_Buffer(UDP_Class uDP_Class, string IP , int lenth)
        {
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
            return Command_Set_Output(uDP_Class, IP, value);
        }
        static public bool Set_OutputPIN(UDP_Class uDP_Class, string IP, int PIN_Num, bool value)
        {
            return Command_Set_OutputPIN(uDP_Class, IP, PIN_Num, value);
        }
        static public bool Set_OutputTrigger(UDP_Class uDP_Class, string IP, int value)
        {
            return Command_Set_OutputTrigger(uDP_Class, IP, value);
        }
        static public bool Set_OutputPINTrigger(UDP_Class uDP_Class, string IP, int PIN_Num, bool value)
        {
            return Command_Set_OutputPINTrigger(uDP_Class, IP, PIN_Num, value);
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
            return Command_Get_IO(uDP_Class, IP, out input, out output);
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
        static public bool EPD_583_DrawImage(UDP_Class uDP_Class, string IP, Bitmap bmp)
        {
            int EPD583_frameDIV = 0;
            if (Chip_Type == ChipType.ESP32)
            {
                EPD583_frameDIV = 1;
            }
            if (Chip_Type == ChipType.BW16)
            {
                EPD583_frameDIV = 1;
            }
            bool flag_OK;
            int width = bmp.Width;
            int height = bmp.Height;
            byte[] bytes_BW = new byte[81 * height];
            byte[] bytes_RW = new byte[81 * height];
            BitmapToByte(bmp, ref bytes_BW, ref bytes_RW);
            MyTimer myTimer = new MyTimer();
            myTimer.StartTickTime(50000);
            flag_OK = EPD_DrawImage(uDP_Class, IP, bytes_BW, bytes_RW, 81 * height / EPD583_frameDIV);
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : EPD 583 DrawImage {string.Format(flag_OK ? "sucess" : "failed")}!   Time : {myTimer.GetTickTime().ToString("0.000")} ms");
            return flag_OK;
        }
        static public bool EPD_266_DrawImage(UDP_Class uDP_Class, string IP, Bitmap bmp)
        {
            using (Bitmap _bmp = bmp.DeepClone())
            {
                int EPD266_frameDIV = 0;
                if (Chip_Type == ChipType.ESP32)
                {
                    EPD266_frameDIV = 1;
                }
                if (Chip_Type == ChipType.BW16)
                {
                    EPD266_frameDIV = 1;
                }

                bool flag_OK;
                int width = bmp.Width;
                int height = bmp.Height;
                _bmp.RotateFlip(RotateFlipType.Rotate90FlipXY);
                byte[] bytes_BW = new byte[(height / 8) * width];
                byte[] bytes_RW = new byte[(height / 8) * width];
                BitmapToByte(_bmp, ref bytes_BW, ref bytes_RW);
                MyTimer myTimer = new MyTimer();
                myTimer.StartTickTime(50000);
                flag_OK =  EPD_DrawImage(uDP_Class, IP, bytes_BW, bytes_RW, (height / 8) * width / EPD266_frameDIV);
                if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : EPD 266 DrawImage {string.Format(flag_OK ? "sucess" : "failed")}!   Time : {myTimer.GetTickTime().ToString("0.000")} ms");
                return flag_OK;
            }
    

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
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : Set_WS2812_Buffer {string.Format(flag_OK ? "sucess" : "failed")}!");
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
            if (ConsoleWrite) Console.WriteLine($"{IP}:{uDP_Class.Port} : Get WS2812_Buffer {string.Format(flag_OK ? "sucess" : "failed")}!");
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

        #region UART
        static private int UART_TimeOut = 1000;
        static private int UART_RetryNum = 3;
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

            Sset_RFID_Enable = (byte)'Z',
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
                            if (UART_RX[0] == 2 && UART_RX[UART_RX.Length - 1] == 3)
                            {
                                string str = "";
                                for (int i = 1; i < (UART_RX.Length - 1); i++)
                                {
                                    str += (char)UART_RX[i];
                                }
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
                            if (UART_RX[0] == 2 && UART_RX[UART_RX.Length - 1] == 3)
                            {
                                string str = "";
                                for (int i = 1; i < (UART_RX.Length - 1); i++)
                                {
                                    str += (char)UART_RX[i];
                                }
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
            return false;
        }
        #endregion

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
        static unsafe public void BitmapToByte(Bitmap bimage, ref byte[] BW_bytes, ref byte[] RW_bytes)
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
                            if (R[i] == 255 && G[i] == 255 && B[i] == 255)
                            {

                                temp_BW |= (byte)(0x01);
                            }
                            else
                            {
                                temp_BW |= (byte)(0x00);
                            }
                            if (R[i] == 255 && G[i] == 0 && B[i] == 0)
                            {

                                temp_RW |= (byte)(0x01);
                            }
                            else
                            {
                                temp_RW |= (byte)(0x00);
                            }
                        }


                        list_byte_image_BW.Add(temp_BW);
                        list_byte_image_RW.Add(temp_RW);
                        flag_index++;
                        ;
                    }
                }
            }

            bimage.UnlockBits(bmData);
            BW_bytes = list_byte_image_BW.ToArray();
            RW_bytes = list_byte_image_RW.ToArray();
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
            int ByteOfSkip = ByteOfNumWidth * ColorDepth % 4;
            if (ByteOfSkip > 0) ByteOfSkip = 4 - ByteOfSkip;
            return ByteOfSkip;
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
            if (text.StringIsEmpty()) return null;

            Size size = DrawingClass.Draw.MeasureText(text, font);
    
            if (width < size.Width) width = size.Width;
            if (height < size.Height) height = size.Height;

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

                DrawPath(g, _Width, _Height, 1, borderSize, borderColor, backColor);
                Font _font = new Font(font.Name, (int)(font.Size * scale), font.Style, font.Unit);


                //g.FillRectangle(new SolidBrush(backColor), 0, 0, _Width, _Height);
                Size string_size = TextRenderer.MeasureText(text, _font);
                Size Rect_Size = new Size(_Width, _Height);

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
                g.DrawString(text, _font, new SolidBrush(foreColor), string_point, StringFormat.GenericDefault);


                //if (borderSize > 0)
                //{                
                //    g.DrawRectangle(new Pen(borderColor, borderSize), borderSize / 2, borderSize / 2, (int)(Rect_Size.Width - borderSize), (int)(Rect_Size.Height - borderSize));
                //}
              

            }
            return bitmap;
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
                    //CharacterCommand = "UTF-8",
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
    }
}
