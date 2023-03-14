using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using Basic;
using MyUI;

namespace H_Pannel_lib
{
    public class DrawerUI_EPD_583 : DrawerUI
    {
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

        }

        #region 靜態參數
        static public int NumOfLED = 450;
        static private int Drawer_NumOf_H_Line = 4;
        static private int Drawer_NumOf_V_Line = 8;
        static private int NumOfLED_Pannel = 42;
        static private int NumOfLED_Drawer = 450 - NumOfLED_Pannel;


        static public int Pannel_Width
        {
            get
            {
                return 648;
            }
        }
        static public int Pannel_Height
        {
            get
            {
                return 480;
            }
        }

        private List<Drawer> Drawers = new List<Drawer>();
        static public byte[] Get_Empty_LEDBytes()
        {
            return new byte[Drawer.NumOfLED * 3];
        }

        static public byte[] LEDBytes_Pannel_Clear(Drawer drawer)
        {
            return LEDBytes_Pannel_Clear(drawer.LED_Bytes);
        }
        static public byte[] LEDBytes_Pannel_Clear(byte[] LED_Bytes)
        {
            for (int i = NumOfLED_Drawer * 3; i < Drawer.NumOfLED * 3; i++)
            {
                if (i > LED_Bytes.Length) return LED_Bytes;
                LED_Bytes[i] = 0;
            }
            return LED_Bytes;
        }

        static public byte[] LEDBytes_Drawer_Clear(Drawer drawer)
        {
            drawer.LED_Bytes = LEDBytes_Drawer_Clear(ref drawer.LED_Bytes);
            return drawer.LED_Bytes;
        }
        static public byte[] LEDBytes_Drawer_Clear(ref byte[] LED_Bytes)
        {
            for (int i = 0; i < NumOfLED * 3; i++)
            {
                if (i > LED_Bytes.Length) return LED_Bytes;
                LED_Bytes[i] = 0;
            }
            return LED_Bytes;
        }
        static public byte[] Set_LEDBytes(Drawer drawer, List<Box> boxes, Color color)
        {
            byte[] led_bytes = drawer.LED_Bytes;
            for(int i = 0; i < boxes.Count; i++)
            {
                if(boxes[i].IP == drawer.IP)
                {
                    led_bytes = Set_LEDBytes(drawer, boxes[i].Column, boxes[i].Row, color);
                }
               
            }
            return led_bytes;
        }
        static public byte[] Set_LEDBytes(Drawer drawer, Box box, Color color)
        {
            return Set_LEDBytes(drawer, box.Column, box.Row, color);
        }
        static public byte[] Set_LEDBytes(Drawer drawer, int col, int row, Color color)
        {
            if (col >= drawer.Boxes.Count) return drawer.LED_Bytes;
            for (int i = 0; i < drawer.Boxes.Count; i++)
            {
                if (row >= drawer.Boxes[i].Length) return drawer.LED_Bytes;
            }
            Rectangle rect = Get_Box_Combine(drawer, col, row);
            int width = rect.Width / (Pannel_Width / Drawer_NumOf_H_Line);
            int height = rect.Height / (Pannel_Height / Drawer_NumOf_V_Line);
            return Set_LEDBytes(col, row, width, height, ref drawer.LED_Bytes, color);
        }
        static public byte[] Set_LEDBytes(int col_x, int row_y, int width, int height, ref byte[] LEDBytes, Color color)
        {
            for (int i = 0; i < width; i++)
            {
                Set_Drawer_H_Leds(col_x + i + row_y * Drawer_NumOf_H_Line, ref LEDBytes, color);
                Set_Drawer_H_Leds(col_x + i + (height + row_y) * Drawer_NumOf_H_Line, ref LEDBytes, color);

            }
            for (int k = 0; k < height; k++)
            {
                Set_Drawer_V_Leds(row_y + k + col_x * Drawer_NumOf_V_Line, ref LEDBytes, color);
                Set_Drawer_V_Leds(row_y + k + (width + col_x) * Drawer_NumOf_V_Line, ref LEDBytes, color);
            }
            return LEDBytes;
        }

        static public byte[] Set_Pannel_LEDBytes(Drawer drawer, Color color)
        {
            return Set_Pannel_LEDBytes(ref drawer.LED_Bytes, color);
        }
        static public byte[] Set_Pannel_LEDBytes(ref byte[] LED_Bytes, Color color)
        {
            for (int i = NumOfLED_Drawer ; i < NumOfLED ; i++)
            {
                if (i * 3> LED_Bytes.Length) return LED_Bytes;
                LED_Bytes[i * 3 + 0] = color.R;
                LED_Bytes[i * 3 + 1] = color.G;
                LED_Bytes[i * 3 + 2] = color.B;         
            }
            return LED_Bytes;
        }

        static public bool Set_Pannel_LED_UDP(UDP_Class uDP_Class, Drawer drawer, Color color)
        {
            return Set_Pannel_LED_UDP(uDP_Class, drawer.IP, drawer.LED_Bytes, color);
        }
        static public bool Set_Pannel_LED_UDP(UDP_Class uDP_Class, string IP, byte[] LED_Bytes, Color color)
        {
            LED_Bytes = Set_Pannel_LEDBytes(ref LED_Bytes, color);
            if (uDP_Class != null)
            {
                return Set_LED_UDP(uDP_Class, IP, LED_Bytes);
            }
            return false;
        }


        static public bool Set_Drawer_LED_UDP(UDP_Class uDP_Class, Drawer drawer, int col, int row, Color color)
        {
            return Set_Drawer_LED_UDP(uDP_Class, drawer, col, row, color, false);
        }

        static public bool Set_Drawer_LED_UDP(UDP_Class uDP_Class, Drawer drawer, int col, int row, Color color, bool ClearAll)
        {
            return Set_Drawer_LED_UDP(uDP_Class, drawer, new int[] { col }, new int[] { row }, color, ClearAll);
        }
        static public bool Set_Drawer_LED_UDP(UDP_Class uDP_Class, Drawer drawer, List<int> col, List<int> row, Color color)
        {
            return Set_Drawer_LED_UDP(uDP_Class, drawer, col.ToArray(), row.ToArray(), color);
        }
        static public bool Set_Drawer_LED_UDP(UDP_Class uDP_Class, Drawer drawer, List<int> col, List<int> row, Color color, bool ClearAll)
        {
            return Set_Drawer_LED_UDP(uDP_Class, drawer, col.ToArray(), row.ToArray(), color, ClearAll);
        }
        static public bool Set_Drawer_LED_UDP(UDP_Class uDP_Class, Drawer drawer, int[] col, int[] row, Color color)
        {
            return Set_Drawer_LED_UDP(uDP_Class, drawer, col, row, color, false);
        }
        static public bool Set_Drawer_LED_UDP(UDP_Class uDP_Class, Drawer drawer, int[] col, int[] row, Color color, bool ClearAll)
        {
            if (col.Length != row.Length) return false;
            if (ClearAll)
            {
                LEDBytes_Drawer_Clear(drawer);
            }
            for (int i = 0; i < col.Length; i++)
            {
                Set_LEDBytes(drawer, col[i], row[i], color);
            }
            return Set_LED_UDP(uDP_Class, drawer);
        }
  
        static public bool Set_LED_UDP(UDP_Class uDP_Class, Drawer drawer , Box box , Color color)
        {
            List<Box> boxes = new List<Box>();
            boxes.Add(box);
            return Set_LED_UDP(uDP_Class, drawer, boxes, color);
        }
        static public bool Set_LED_UDP(UDP_Class uDP_Class, Drawer drawer, List<Box> boxes, Color color)
        {
            for(int i = 0; i < boxes.Count; i++)
            {
                if (boxes[i].IP == drawer.IP)
                {
                    drawer.LED_Bytes = Set_LEDBytes(drawer, boxes[i], color);
                }
            }
            drawer.LED_Bytes = Set_Pannel_LEDBytes(drawer, color);
            return Set_LED_UDP(uDP_Class, drawer.IP, drawer.LED_Bytes);
        }
        static public bool Set_LED_UDP(UDP_Class uDP_Class, Drawer drawer)
        {
            return Set_LED_UDP(uDP_Class , drawer.IP, drawer.LED_Bytes) ;
        }
        static public bool Set_LED_UDP(UDP_Class uDP_Class, string IP ,byte[] LED_Bytes)
        {
            if (uDP_Class != null)
            {
                return Communication.Set_WS2812_Buffer(uDP_Class, IP, 0, LED_Bytes);
            }
            return false;
        }
        static public bool Set_LED_UDP(UDP_Class uDP_Class, Drawer drawer, Color color)
        {
            return Set_LED_UDP(uDP_Class, drawer.IP, color);
        }
        static public bool Set_LED_UDP(UDP_Class uDP_Class, string IP, Color color)
        {
            byte[] LED_Bytes = Get_Empty_LEDBytes();
            for (int i = 0; i < NumOfLED; i++)
            {
                LED_Bytes[i * 3 + 0] = color.R;
                LED_Bytes[i * 3 + 1] = color.G;
                LED_Bytes[i * 3 + 2] = color.B;
            }
            return Set_LED_UDP(uDP_Class, IP, LED_Bytes);
        }

        static public bool Set_LED_Clear_UDP(UDP_Class uDP_Class, Drawer drawer)
        {
            return Set_LED_Clear_UDP(uDP_Class, drawer.IP);
        }
        static public bool Set_LED_Clear_UDP(UDP_Class uDP_Class, string IP)
        {
            byte[] LED_Bytes = Get_Empty_LEDBytes();
            return Set_LED_UDP(uDP_Class, IP, LED_Bytes);
        }

        static public bool DrawToEpd_UDP(UDP_Class uDP_Class, Drawer drawer)
        {
            using (Bitmap bitmap = Get_Drawer_bmp(drawer))
            {
                return DrawToEpd_UDP(uDP_Class, drawer.IP, bitmap);
            }
        }
 
        static public bool DrawToEpd_UDP(UDP_Class uDP_Class, string IP, Bitmap bmp)
        {
            return Communication.EPD_583_DrawImage(uDP_Class, IP, bmp);
        }
        static public byte[] Get_Drawer_LEDBytes_UDP(UDP_Class uDP_Class, string IP)
        {
            byte[] LED_Bytes = new byte[NumOfLED * 3];
            if (uDP_Class != null)
            {
                return Communication.Get_WS2812_Buffer(uDP_Class, IP, NumOfLED * 3);
            }
            return LED_Bytes;
        }

        static public bool Set_LockOpen(UDP_Class uDP_Class, string IP)
        {
            return Communication.Set_OutputPINTrigger(uDP_Class, IP, 1, true);
        }
      

        static public void Set_Drawer_H_Leds(int col, ref byte[] LEDBytes, Color color)
        {
            for (int i = 0; i < List_Drawer_H_Line_Leds[col].Length; i++)
            {
                LEDBytes[List_Drawer_H_Line_Leds[col][i] * 3 + 0] = color.R;
                LEDBytes[List_Drawer_H_Line_Leds[col][i] * 3 + 1] = color.G;
                LEDBytes[List_Drawer_H_Line_Leds[col][i] * 3 + 2] = color.B;
            }
        }
        static public void Set_Drawer_V_Leds(int row, ref byte[] LEDBytes, Color color)
        {
            for (int i = 0; i < List_Drawer_V_Line_Leds[row].Length; i++)
            {
                LEDBytes[List_Drawer_V_Line_Leds[row][i] * 3 + 0] = color.R;
                LEDBytes[List_Drawer_V_Line_Leds[row][i] * 3 + 1] = color.G;
                LEDBytes[List_Drawer_V_Line_Leds[row][i] * 3 + 2] = color.B;
            }
        }
        static public Rectangle Get_Box_Combine(Drawer drawer, int col , int row)
        {
            if (col > drawer.Boxes.Count) return new Rectangle();
            if (row > drawer.Boxes[col].Length) return new Rectangle();
            return Get_Box_Combine(drawer, drawer.Boxes[col][row]);
        }
        static public Rectangle Get_Box_Combine(Drawer drawer ,Box box)
        {
            Box _box;
            if (box.Slave)
            {
                _box = drawer.Boxes[box.MasterBox_Column][box.MasterBox_Row];
            }
            else
            {
                _box = box;
            }
            int X0 = _box.X;
            int Y0 = _box.Y;
            int X1 = X0 + _box.Width;
            int Y1 = Y0 + _box.Height;
            int X_temp = 0;
            int Y_temp = 0;
            int SlaveBox_Column = _box.SlaveBox_Column;
            int SlaveBox_Row = _box.SlaveBox_Row;
            int SlaveBox_Column_temp = SlaveBox_Column;
            int SlaveBox_Row_temp = SlaveBox_Row;
            while (true)
            {
                if (SlaveBox_Column_temp == -1 && SlaveBox_Row_temp == -1) break;
                SlaveBox_Column = SlaveBox_Column_temp;
                SlaveBox_Row = SlaveBox_Row_temp;
                X_temp = drawer.Boxes[SlaveBox_Column][SlaveBox_Row].X + drawer.Boxes[SlaveBox_Column][SlaveBox_Row].Width;
                Y_temp = drawer.Boxes[SlaveBox_Column][SlaveBox_Row].Y + drawer.Boxes[SlaveBox_Column][SlaveBox_Row].Height;
                if (X_temp > X1) X1 = X_temp;
                if (Y_temp > Y1) Y1 = Y_temp;
                SlaveBox_Column_temp = drawer.Boxes[SlaveBox_Column][SlaveBox_Row].SlaveBox_Column;
                SlaveBox_Row_temp = drawer.Boxes[SlaveBox_Column][SlaveBox_Row].SlaveBox_Row;
            }
            Rectangle rectangle = new Rectangle(X0, Y0, X1 - X0, Y1 - Y0);
            return rectangle;


        }
        static public Bitmap Get_Drawer_bmp(Drawer drawer)
        {
            Bitmap bitmap = new Bitmap(DrawerUI_EPD_583.Pannel_Width, DrawerUI_EPD_583.Pannel_Height);
            Graphics g = Graphics.FromImage(bitmap);
            List<Box[]> Boxes = drawer.Boxes;
            for (int i = 0; i < Boxes.Count; i++)
            {
                for (int k = 0; k < Boxes[i].Length; k++)
                {
                    Rectangle rect = Get_Box_Combine(drawer, Boxes[i][k]);
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

                        _box.SetValue(Device.ValueName.藥品碼, Device.ValueType.ForeColor, _box.IsWarning ? Color.White : Color.Black);
                        _box.SetValue(Device.ValueName.藥品碼, Device.ValueType.BackColor, _box.BackColor);

                        _box.SetValue(Device.ValueName.藥品名稱, Device.ValueType.ForeColor, _box.IsWarning ? Color.White : Color.Black);
                        _box.SetValue(Device.ValueName.藥品名稱, Device.ValueType.BackColor, _box.BackColor);

                        _box.SetValue(Device.ValueName.藥品學名, Device.ValueType.ForeColor, _box.IsWarning ? Color.White : Color.Black);
                        _box.SetValue(Device.ValueName.藥品學名, Device.ValueType.BackColor, _box.BackColor);

                        _box.SetValue(Device.ValueName.藥品中文名稱, Device.ValueType.ForeColor, _box.IsWarning ? Color.White : Color.Black);
                        _box.SetValue(Device.ValueName.藥品中文名稱, Device.ValueType.BackColor, _box.BackColor);

                        _box.SetValue(Device.ValueName.包裝單位, Device.ValueType.ForeColor, _box.IsWarning ? Color.White : Color.Black);
                        _box.SetValue(Device.ValueName.包裝單位, Device.ValueType.BackColor, _box.BackColor);

                        _box.SetValue(Device.ValueName.效期, Device.ValueType.ForeColor, _box.IsWarning ? Color.White : Color.Black);
                        _box.SetValue(Device.ValueName.效期, Device.ValueType.BackColor, _box.BackColor);

                        _box.SetValue(Device.ValueName.庫存, Device.ValueType.ForeColor, _box.IsWarning ? Color.White : Color.Black);
                        _box.SetValue(Device.ValueName.庫存, Device.ValueType.BackColor, _box.BackColor);

                        SizeF size_Name = g.MeasureString(_box.Name, _box.Name_font, new Size(rect.Width, rect.Height), StringFormat.GenericDefault);
                        size_Name = new SizeF((int)size_Name.Width, (int)size_Name.Height);
                        g.DrawString(_box.Name, _box.Name_font, new SolidBrush(_box.ForeColor), rect, StringFormat.GenericDefault);
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
                                g.DrawString(str, _box.Validity_period_font, new SolidBrush((Color)_box.GetValue(Box.ValueName.效期, Box.ValueType.ForeColor)), rect.X + 5, rect.Y + posy);
                                Color color_pen = _box.IsWarning ? Color.Black : Color.Red;
                                g.DrawRectangle(new Pen(new SolidBrush(color_pen), 1), rect.X + 5, rect.Y + posy, size_Validity_period.Width, size_Validity_period.Height);
                                posy += size_Validity_period.Height;
                            }
                        }

                        SizeF size_Code = TextRenderer.MeasureText($"{_box.Code}[{_box.Inventory}]", _box.Code_font);
                        if (_box.Code.StringIsEmpty() == false) g.DrawString($"{_box.Code}[{_box.Inventory}]", _box.Code_font, new SolidBrush(Color.Black), rect.X, ((rect.Y + rect.Height) - size_Code.Height));
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
        

        #endregion
        #region LED_Line

        private static  List<int[]> List_Drawer_H_Line_Leds = new List<int[]>();
        private static  List<int[]> List_Drawer_V_Line_Leds = new List<int[]>();
        private static int[] Line_H_00 = new int[] { 376 + 0, 376 + 1, 376 + 2, 376 + 3, 376 + 4, 376 + 5, 376 + 6, 376 + 7 };
        private static int[] Line_H_01 = new int[] { 384 + 0, 384 + 1, 384 + 2, 384 + 3, 384 + 4, 384 + 5, 384 + 6, 384 + 7 };
        private static int[] Line_H_02 = new int[] { 392 + 0, 392 + 1, 392 + 2, 392 + 3, 392 + 4, 392 + 5, 392 + 6, 392 + 7 };
        private static int[] Line_H_03 = new int[] { 400 + 0, 400 + 1, 400 + 2, 400 + 3, 400 + 4, 400 + 5, 400 + 6, 400 + 7 };

        private static int[] Line_H_04 = new int[] { 368 + 0, 368 + 1, 368 + 2, 368 + 3, 368 + 4, 368 + 5, 368 + 6, 368 + 7 };
        private static int[] Line_H_05 = new int[] { 360 + 0, 360 + 1, 360 + 2, 360 + 3, 360 + 4, 360 + 5, 360 + 6, 360 + 7 };
        private static int[] Line_H_06 = new int[] { 352 + 0, 352 + 1, 352 + 2, 352 + 3, 352 + 4, 352 + 5, 352 + 6, 352 + 7 };
        private static int[] Line_H_07 = new int[] { 344 + 0, 344 + 1, 344 + 2, 344 + 3, 344 + 4, 344 + 5, 344 + 6, 344 + 7 };

        private static int[] Line_H_08 = new int[] { 312 + 0, 312 + 1, 312 + 2, 312 + 3, 312 + 4, 312 + 5, 312 + 6, 312 + 7 };
        private static int[] Line_H_09 = new int[] { 320 + 0, 320 + 1, 320 + 2, 320 + 3, 320 + 4, 320 + 5, 320 + 6, 320 + 7 };
        private static int[] Line_H_10 = new int[] { 328 + 0, 328 + 1, 328 + 2, 328 + 3, 328 + 4, 328 + 5, 328 + 6, 328 + 7 };
        private static int[] Line_H_11 = new int[] { 336 + 0, 336 + 1, 336 + 2, 336 + 3, 336 + 4, 336 + 5, 336 + 6, 336 + 7 };

        private static int[] Line_H_12 = new int[] { 304 + 0, 304 + 1, 304 + 2, 304 + 3, 304 + 4, 304 + 5, 304 + 6, 304 + 7 };
        private static int[] Line_H_13 = new int[] { 296 + 0, 296 + 1, 296 + 2, 296 + 3, 296 + 4, 296 + 5, 296 + 6, 296 + 7 };
        private static int[] Line_H_14 = new int[] { 288 + 0, 288 + 1, 288 + 2, 288 + 3, 288 + 4, 288 + 5, 288 + 6, 288 + 7 };
        private static int[] Line_H_15 = new int[] { 280 + 0, 280 + 1, 280 + 2, 280 + 3, 280 + 4, 280 + 5, 280 + 6, 280 + 7 };

        private static int[] Line_H_16 = new int[] { 248 + 0, 248 + 1, 248 + 2, 248 + 3, 248 + 4, 248 + 5, 248 + 6, 248 + 7 };
        private static int[] Line_H_17 = new int[] { 256 + 0, 256 + 1, 256 + 2, 256 + 3, 256 + 4, 256 + 5, 256 + 6, 256 + 7 };
        private static int[] Line_H_18 = new int[] { 264 + 0, 264 + 1, 264 + 2, 264 + 3, 264 + 4, 264 + 5, 264 + 6, 264 + 7 };
        private static int[] Line_H_19 = new int[] { 272 + 0, 272 + 1, 272 + 2, 272 + 3, 272 + 4, 272 + 5, 272 + 6, 272 + 7 };

        private static int[] Line_H_20 = new int[] { 240 + 0, 240 + 1, 240 + 2, 240 + 3, 240 + 4, 240 + 5, 240 + 6, 240 + 7 };
        private static int[] Line_H_21 = new int[] { 232 + 0, 232 + 1, 232 + 2, 232 + 3, 232 + 4, 232 + 5, 232 + 6, 232 + 7 };
        private static int[] Line_H_22 = new int[] { 224 + 0, 224 + 1, 224 + 2, 224 + 3, 224 + 4, 224 + 5, 224 + 6, 224 + 7 };
        private static int[] Line_H_23 = new int[] { 216 + 0, 216 + 1, 216 + 2, 216 + 3, 216 + 4, 216 + 5, 216 + 6, 216 + 7 };

        private static int[] Line_H_24 = new int[] { 184 + 0, 184 + 1, 184 + 2, 184 + 3, 184 + 4, 184 + 5, 184 + 6, 184 + 7 };
        private static int[] Line_H_25 = new int[] { 192 + 0, 192 + 1, 192 + 2, 192 + 3, 192 + 4, 192 + 5, 192 + 6, 192 + 7 };
        private static int[] Line_H_26 = new int[] { 200 + 0, 200 + 1, 200 + 2, 200 + 3, 200 + 4, 200 + 5, 200 + 6, 200 + 7 };
        private static int[] Line_H_27 = new int[] { 208 + 0, 208 + 1, 208 + 2, 208 + 3, 208 + 4, 208 + 5, 208 + 6, 208 + 7 };

        private static int[] Line_H_28 = new int[] { 176 + 0, 176 + 1, 176 + 2, 176 + 3, 176 + 4, 176 + 5, 176 + 6, 176 + 7 };
        private static int[] Line_H_29 = new int[] { 168 + 0, 168 + 1, 168 + 2, 168 + 3, 168 + 4, 168 + 5, 168 + 6, 168 + 7 };
        private static int[] Line_H_30 = new int[] { 160 + 0, 160 + 1, 160 + 2, 160 + 3, 160 + 4, 160 + 5, 160 + 6, 160 + 7 };
        private static int[] Line_H_31 = new int[] { 152 + 0, 152 + 1, 152 + 2, 152 + 3, 152 + 4, 152 + 5, 152 + 6, 152 + 7 };

        private static int[] Line_H_32 = new int[] { 120 + 0, 120 + 1, 120 + 2, 120 + 3, 120 + 4, 120 + 5, 120 + 6, 120 + 7 };
        private static int[] Line_H_33 = new int[] { 128 + 0, 128 + 1, 128 + 2, 128 + 3, 128 + 4, 128 + 5, 128 + 6, 128 + 7 };
        private static int[] Line_H_34 = new int[] { 136 + 0, 136 + 1, 136 + 2, 136 + 3, 136 + 4, 136 + 5, 136 + 6, 136 + 7 };
        private static int[] Line_H_35 = new int[] { 144 + 0, 144 + 1, 144 + 2, 144 + 3, 144 + 4, 144 + 5, 144 + 6, 144 + 7 };


        private static int[] Line_V_00 = new int[] { 96 + (0 * 3) + 0, 96 + (0 * 3) + 1, 96 + (0 * 3) + 2 };
        private static int[] Line_V_01 = new int[] { 96 + (1 * 3) + 0, 96 + (1 * 3) + 1, 96 + (1 * 3) + 2 };
        private static int[] Line_V_02 = new int[] { 96 + (2 * 3) + 0, 96 + (2 * 3) + 1, 96 + (2 * 3) + 2 };
        private static int[] Line_V_03 = new int[] { 96 + (3 * 3) + 0, 96 + (3 * 3) + 1, 96 + (3 * 3) + 2 };
        private static int[] Line_V_04 = new int[] { 96 + (4 * 3) + 0, 96 + (4 * 3) + 1, 96 + (4 * 3) + 2 };
        private static int[] Line_V_05 = new int[] { 96 + (5 * 3) + 0, 96 + (5 * 3) + 1, 96 + (5 * 3) + 2 };
        private static int[] Line_V_06 = new int[] { 96 + (6 * 3) + 0, 96 + (6 * 3) + 1, 96 + (6 * 3) + 2 };
        private static int[] Line_V_07 = new int[] { 96 + (7 * 3) + 0, 96 + (7 * 3) + 1, 96 + (7 * 3) + 2 };

        private static int[] Line_V_08 = new int[] { 95 - (0 * 3) - 0, 95 - (0 * 3) - 1, 95 - (0 * 3) - 2 };
        private static int[] Line_V_09 = new int[] { 95 - (1 * 3) - 0, 95 - (1 * 3) - 1, 95 - (1 * 3) - 2 };
        private static int[] Line_V_10 = new int[] { 95 - (2 * 3) - 0, 95 - (2 * 3) - 1, 95 - (2 * 3) - 2 };
        private static int[] Line_V_11 = new int[] { 95 - (3 * 3) - 0, 95 - (3 * 3) - 1, 95 - (3 * 3) - 2 };
        private static int[] Line_V_12 = new int[] { 95 - (4 * 3) - 0, 95 - (4 * 3) - 1, 95 - (4 * 3) - 2 };
        private static int[] Line_V_13 = new int[] { 95 - (5 * 3) - 0, 95 - (5 * 3) - 1, 95 - (5 * 3) - 2 };
        private static int[] Line_V_14 = new int[] { 95 - (6 * 3) - 0, 95 - (6 * 3) - 1, 95 - (6 * 3) - 2 };
        private static int[] Line_V_15 = new int[] { 95 - (7 * 3) - 0, 95 - (7 * 3) - 1, 95 - (7 * 3) - 2 };

        private static int[] Line_V_16 = new int[] { 48 + (0 * 3) + 0, 48 + (0 * 3) + 1, 48 + (0 * 3) + 2 };
        private static int[] Line_V_17 = new int[] { 48 + (1 * 3) + 0, 48 + (1 * 3) + 1, 48 + (1 * 3) + 2 };
        private static int[] Line_V_18 = new int[] { 48 + (2 * 3) + 0, 48 + (2 * 3) + 1, 48 + (2 * 3) + 2 };
        private static int[] Line_V_19 = new int[] { 48 + (3 * 3) + 0, 48 + (3 * 3) + 1, 48 + (3 * 3) + 2 };
        private static int[] Line_V_20 = new int[] { 48 + (4 * 3) + 0, 48 + (4 * 3) + 1, 48 + (4 * 3) + 2 };
        private static int[] Line_V_21 = new int[] { 48 + (5 * 3) + 0, 48 + (5 * 3) + 1, 48 + (5 * 3) + 2 };
        private static int[] Line_V_22 = new int[] { 48 + (6 * 3) + 0, 48 + (6 * 3) + 1, 48 + (6 * 3) + 2 };
        private static int[] Line_V_23 = new int[] { 48 + (7 * 3) + 0, 48 + (7 * 3) + 1, 48 + (7 * 3) + 2 };

        private static int[] Line_V_24 = new int[] { 47 - (0 * 3) - 0, 47 - (0 * 3) - 1, 47 - (0 * 3) - 2 };
        private static int[] Line_V_25 = new int[] { 47 - (1 * 3) - 0, 47 - (1 * 3) - 1, 47 - (1 * 3) - 2 };
        private static int[] Line_V_26 = new int[] { 47 - (2 * 3) - 0, 47 - (2 * 3) - 1, 47 - (2 * 3) - 2 };
        private static int[] Line_V_27 = new int[] { 47 - (3 * 3) - 0, 47 - (3 * 3) - 1, 47 - (3 * 3) - 2 };
        private static int[] Line_V_28 = new int[] { 47 - (4 * 3) - 0, 47 - (4 * 3) - 1, 47 - (4 * 3) - 2 };
        private static int[] Line_V_29 = new int[] { 47 - (5 * 3) - 0, 47 - (5 * 3) - 1, 47 - (5 * 3) - 2 };
        private static int[] Line_V_30 = new int[] { 47 - (6 * 3) - 0, 47 - (6 * 3) - 1, 47 - (6 * 3) - 2 };
        private static int[] Line_V_31 = new int[] { 47 - (7 * 3) - 0, 47 - (7 * 3) - 1, 47 - (7 * 3) - 2 };

        private static int[] Line_V_32 = new int[] { 00 + (0 * 3) + 0, 00 + (0 * 3) + 1, 00 + (0 * 3) + 2 };
        private static int[] Line_V_33 = new int[] { 00 + (1 * 3) + 0, 00 + (1 * 3) + 1, 00 + (1 * 3) + 2 };
        private static int[] Line_V_34 = new int[] { 00 + (2 * 3) + 0, 00 + (2 * 3) + 1, 00 + (2 * 3) + 2 };
        private static int[] Line_V_35 = new int[] { 00 + (3 * 3) + 0, 00 + (3 * 3) + 1, 00 + (3 * 3) + 2 };
        private static int[] Line_V_36 = new int[] { 00 + (4 * 3) + 0, 00 + (4 * 3) + 1, 00 + (4 * 3) + 2 };
        private static int[] Line_V_37 = new int[] { 00 + (5 * 3) + 0, 00 + (5 * 3) + 1, 00 + (5 * 3) + 2 };
        private static int[] Line_V_38 = new int[] { 00 + (6 * 3) + 0, 00 + (6 * 3) + 1, 00 + (6 * 3) + 2 };
        private static int[] Line_V_39 = new int[] { 00 + (7 * 3) + 0, 00 + (7 * 3) + 1, 00 + (7 * 3) + 2 };
        private void WS2812_Init()
        {
            List_Drawer_H_Line_Leds.Add(Line_H_00);
            List_Drawer_H_Line_Leds.Add(Line_H_01);
            List_Drawer_H_Line_Leds.Add(Line_H_02);
            List_Drawer_H_Line_Leds.Add(Line_H_03);

            List_Drawer_H_Line_Leds.Add(Line_H_04);
            List_Drawer_H_Line_Leds.Add(Line_H_05);
            List_Drawer_H_Line_Leds.Add(Line_H_06);
            List_Drawer_H_Line_Leds.Add(Line_H_07);

            List_Drawer_H_Line_Leds.Add(Line_H_08);
            List_Drawer_H_Line_Leds.Add(Line_H_09);
            List_Drawer_H_Line_Leds.Add(Line_H_10);
            List_Drawer_H_Line_Leds.Add(Line_H_11);

            List_Drawer_H_Line_Leds.Add(Line_H_12);
            List_Drawer_H_Line_Leds.Add(Line_H_13);
            List_Drawer_H_Line_Leds.Add(Line_H_14);
            List_Drawer_H_Line_Leds.Add(Line_H_15);

            List_Drawer_H_Line_Leds.Add(Line_H_16);
            List_Drawer_H_Line_Leds.Add(Line_H_17);
            List_Drawer_H_Line_Leds.Add(Line_H_18);
            List_Drawer_H_Line_Leds.Add(Line_H_19);

            List_Drawer_H_Line_Leds.Add(Line_H_20);
            List_Drawer_H_Line_Leds.Add(Line_H_21);
            List_Drawer_H_Line_Leds.Add(Line_H_22);
            List_Drawer_H_Line_Leds.Add(Line_H_23);

            List_Drawer_H_Line_Leds.Add(Line_H_24);
            List_Drawer_H_Line_Leds.Add(Line_H_25);
            List_Drawer_H_Line_Leds.Add(Line_H_26);
            List_Drawer_H_Line_Leds.Add(Line_H_27);

            List_Drawer_H_Line_Leds.Add(Line_H_28);
            List_Drawer_H_Line_Leds.Add(Line_H_29);
            List_Drawer_H_Line_Leds.Add(Line_H_30);
            List_Drawer_H_Line_Leds.Add(Line_H_31);

            List_Drawer_H_Line_Leds.Add(Line_H_32);
            List_Drawer_H_Line_Leds.Add(Line_H_33);
            List_Drawer_H_Line_Leds.Add(Line_H_34);
            List_Drawer_H_Line_Leds.Add(Line_H_35);

            List_Drawer_V_Line_Leds.Add(Line_V_00);
            List_Drawer_V_Line_Leds.Add(Line_V_01);
            List_Drawer_V_Line_Leds.Add(Line_V_02);
            List_Drawer_V_Line_Leds.Add(Line_V_03);
            List_Drawer_V_Line_Leds.Add(Line_V_04);

            List_Drawer_V_Line_Leds.Add(Line_V_05);
            List_Drawer_V_Line_Leds.Add(Line_V_06);
            List_Drawer_V_Line_Leds.Add(Line_V_07);
            List_Drawer_V_Line_Leds.Add(Line_V_08);
            List_Drawer_V_Line_Leds.Add(Line_V_09);

            List_Drawer_V_Line_Leds.Add(Line_V_10);
            List_Drawer_V_Line_Leds.Add(Line_V_11);
            List_Drawer_V_Line_Leds.Add(Line_V_12);
            List_Drawer_V_Line_Leds.Add(Line_V_13);
            List_Drawer_V_Line_Leds.Add(Line_V_14);

            List_Drawer_V_Line_Leds.Add(Line_V_15);
            List_Drawer_V_Line_Leds.Add(Line_V_16);
            List_Drawer_V_Line_Leds.Add(Line_V_17);
            List_Drawer_V_Line_Leds.Add(Line_V_18);
            List_Drawer_V_Line_Leds.Add(Line_V_19);

            List_Drawer_V_Line_Leds.Add(Line_V_20);
            List_Drawer_V_Line_Leds.Add(Line_V_21);
            List_Drawer_V_Line_Leds.Add(Line_V_22);
            List_Drawer_V_Line_Leds.Add(Line_V_23);
            List_Drawer_V_Line_Leds.Add(Line_V_24);

            List_Drawer_V_Line_Leds.Add(Line_V_25);
            List_Drawer_V_Line_Leds.Add(Line_V_26);
            List_Drawer_V_Line_Leds.Add(Line_V_27);
            List_Drawer_V_Line_Leds.Add(Line_V_28);
            List_Drawer_V_Line_Leds.Add(Line_V_29);

            List_Drawer_V_Line_Leds.Add(Line_V_30);
            List_Drawer_V_Line_Leds.Add(Line_V_31);
            List_Drawer_V_Line_Leds.Add(Line_V_32);
            List_Drawer_V_Line_Leds.Add(Line_V_33);
            List_Drawer_V_Line_Leds.Add(Line_V_34);

            List_Drawer_V_Line_Leds.Add(Line_V_35);
            List_Drawer_V_Line_Leds.Add(Line_V_36);
            List_Drawer_V_Line_Leds.Add(Line_V_37);
            List_Drawer_V_Line_Leds.Add(Line_V_38);
            List_Drawer_V_Line_Leds.Add(Line_V_39);
        }
        #endregion

        private enum ContextMenuStrip_Main
        {
            畫面設置,
            IO設定,
            設為大抽屜4X8,
            設為小抽屜3X8,
            設為有鎖控,
            設為無鎖控,
        }
        private enum ContextMenuStrip_DeviceTable_畫面設置
        {
            測試資訊,
            繪製儲位,
        }
        private enum ContextMenuStrip_DeviceTable_IO設定
        {
            面板亮燈,
            全部亮燈測試,
            清除燈號,
            IO測試,
        }
        private enum ContextMenuStrip_UDP_DataReceive_畫面設置
        {
            測試資訊,
            繪製儲位,
        }
        private enum ContextMenuStrip_UDP_DataReceive_IO設定
        {
            面板亮燈,
            全部亮燈測試,
            清除燈號,
            IO測試,
        }

        public DrawerUI_EPD_583()
        {
            this.TableName = "EPD583_Jsonstring";
            WS2812_Init();

            this.DeviceTableMouseDownRightEvent += DrawerUI_EPD_583_DeviceTableMouseDownRightEvent;
            this.UDP_DataReceiveMouseDownRightEvent += DrawerUI_EPD_583_UDP_DataReceiveMouseDownRightEvent;

            this.sqL_DataGridView_DeviceTable.DataGridRefreshEvent += SqL_DataGridView_DeviceTable_DataGridRefreshEvent;

            Enum_ContextMenuStrip_DeviceTable = new ContextMenuStrip_Main();
            Enum_ContextMenuStrip_UDP_DataReceive = new ContextMenuStrip_Main();
        }

        private void SqL_DataGridView_DeviceTable_DataGridRefreshEvent()
        {
            for (int i = 0; i < this.sqL_DataGridView_DeviceTable.dataGridView.Rows.Count; i++)
            {
                string jsonString = this.sqL_DataGridView_DeviceTable.dataGridView.Rows[i].Cells[(int)enum_DeviceTable.Value].Value.ObjectToString();
                Drawer drawer = jsonString.JsonDeserializet<Drawer>();
                if(drawer != null)
                {
                    Color color = Color.White;
                    if(drawer.Boxes[0][0].DeviceType == DeviceType.EPD583_lock)
                    {
                        this.sqL_DataGridView_DeviceTable.dataGridView.Rows[i].DefaultCellStyle.BackColor = Color.Yellow;
                        this.sqL_DataGridView_DeviceTable.dataGridView.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
                    }          
                }
            }
        }

        public byte[] Get_Drawer_LED_UDP(Drawer drawer)
        {
            drawer.LED_Bytes = Get_Drawer_LED_UDP(drawer.IP, drawer.Port);
            return drawer.LED_Bytes;
        }
        public byte[] Get_Drawer_LED_UDP(string IP, int Port)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
            return Get_Drawer_LEDBytes_UDP(uDP_Class, IP);
        }
        public bool Set_Pannel_LED_UDP(Drawer drawer, Color color)
        {
            return Set_Pannel_LED_UDP(drawer.IP, drawer.Port, drawer.LED_Bytes, color);
        }
        public bool Set_Pannel_LED_UDP(string IP, int Port, byte[] LED_Bytes, Color color)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
            return Set_Pannel_LED_UDP(uDP_Class, IP, LED_Bytes, color);
        }


        public bool Set_Drawer_LED_UDP(Drawer drawer, Box box, Color color)
        {
            List<Box> boxes = new List<Box>();
            boxes.Add(box);
            return this.Set_Drawer_LED_UDP(drawer, boxes, color);
        }
        public bool Set_Drawer_LED_UDP(Drawer drawer, List<Box> boxes, Color color)
        {
            List<int> cols = new List<int>();
            List<int> rows = new List<int>();

            for (int i = 0; i < boxes.Count; i++)
            {
                if(boxes[i].IP == drawer.IP)
                {
                    cols.Add(boxes[i].Column);
                    rows.Add(boxes[i].Row);
                }
               
            }
            return this.Set_Drawer_LED_UDP(drawer, cols.ToArray(), rows.ToArray(), color, false);
        }
        public bool Set_Drawer_LED_UDP(Drawer drawer, int[] col, int[] row, Color color)
        {
            return this.Set_Drawer_LED_UDP(drawer, col, row, color, false);
        }
        public bool Set_Drawer_LED_UDP(Drawer drawer, int[] col, int[] row, Color color, bool ClearAll)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(drawer.Port);
            return Set_Drawer_LED_UDP(uDP_Class , drawer, col, row, color, ClearAll);
        }


        public bool Set_LED_UDP(Drawer drawer)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(drawer.Port);
            return Set_LED_UDP(uDP_Class, drawer);
        }
        public bool Set_LED_UDP( Drawer drawer, List<Box> boxes, Color color)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(drawer.Port);
            return Set_LED_UDP(uDP_Class, drawer, boxes, color);
        }
        public bool Set_LED_UDP(Drawer drawer, Box box ,Color color)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(drawer.Port);
            return Set_LED_UDP(uDP_Class ,drawer, box, color);
        }
        public bool Set_LED_UDP(Drawer drawer, Color color)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(drawer.Port);
            return Set_LED_UDP(uDP_Class, drawer.IP, color);
        }
        public bool Set_LED_UDP(string IP, int Port, Color color)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
            return Set_LED_UDP(uDP_Class, IP, color);
        }
        public bool Set_LED_Clear_UDP(Drawer drawer)
        {
            drawer.LED_Bytes = Get_Empty_LEDBytes();
            return Set_LED_Clear_UDP(drawer.IP, drawer.Port);
        }
        public bool Set_LED_Clear_UDP(string IP , int Port)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
            return Set_LED_Clear_UDP(uDP_Class, IP);
        }
        public bool DrawToEpd_UDP(Drawer drawer)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(drawer.Port);
            return DrawToEpd_UDP(uDP_Class, drawer);
        }
        public bool DrawToEpd_UDP(string IP, int Port, Bitmap bitmap)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
            return DrawToEpd_UDP(uDP_Class, IP , bitmap);
        }
        public bool Set_LockOpen(Drawer drawer)
        {
            return Set_LockOpen(drawer.IP, drawer.Port);
        }
        public bool Set_LockOpen(string IP, int Port)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
            return Set_LockOpen(uDP_Class, IP);
        }
        public bool GetInput(string IP)
        {
            string jsonString = GetUDPJsonString(IP);
            if (jsonString.StringIsEmpty()) return false;
            UDP_READ uDP_READ = jsonString.JsonDeserializet<UDP_READ>();
            if(uDP_READ == null) return false;
            return ((uDP_READ.Input % 2) == 1);
        }
        public bool Set_OutputPINTrigger(string IP, int Port, int PIN_Num, bool value)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
            if (uDP_Class != null)
            {
                return Communication.Set_OutputPINTrigger(uDP_Class, IP, PIN_Num, value);
            }
            return false;
        }
        public bool Set_OutputTrigger(string IP, int Port, int value)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
            if (uDP_Class != null)
            {
                return Communication.Set_OutputTrigger(uDP_Class, IP, value);
            }
            return false;
        }
        private Bitmap DrawTestImage(string IP , int Port)
        {
            int width = DrawerUI_EPD_583.Pannel_Width;
            int height = DrawerUI_EPD_583.Pannel_Height;
            Bitmap bitmap = new Bitmap(DrawerUI_EPD_583.Pannel_Width, DrawerUI_EPD_583.Pannel_Height);
            Graphics g = Graphics.FromImage(bitmap);
            g.FillRectangle(new SolidBrush(Color.Red), new RectangleF((width / 3) * 0, (height / 3) * 0, (width), height / 3));
            g.FillRectangle(new SolidBrush(Color.Black), new RectangleF((width / 3) * 0, (height / 3) * 1, (width), height / 3));
            g.FillRectangle(new SolidBrush(Color.White), new RectangleF((width / 3) * 0, (height / 3) * 2, (width), height / 3));

            string text = $"{IP} : {Port}";
            Font font = new Font("微軟正黑體", 20);
            Size text_size = TextRenderer.MeasureText(text, font);
            Point text_point = new Point();

            text_point.X = ((width / 2) - text_size.Width) / 2;
            text_point.Y = ((height / 3) - text_size.Height) / 2;
            g.DrawString(text, font, new SolidBrush(Color.Black), text_point);
            text_point.X = ((width / 2) - text_size.Width) / 2;
            text_point.X += (width / 2);
            text_point.Y = ((height / 3) - text_size.Height) / 2;
            g.DrawString(text, font, new SolidBrush(Color.White), text_point);

            text_point.X = ((width / 2) - text_size.Width) / 2;
            text_point.Y = ((height / 3) - text_size.Height) / 2;
            text_point.Y += (height / 3) * 1;
            g.DrawString(text, font, new SolidBrush(Color.Red), text_point);
            text_point.X = ((width / 2) - text_size.Width) / 2;
            text_point.X += (width / 2);
            text_point.Y = ((height / 3) - text_size.Height) / 2;
            text_point.Y += (height / 3) * 1;
            g.DrawString(text, font, new SolidBrush(Color.White), text_point);

            text_point.X = ((width / 2) - text_size.Width) / 2;
            text_point.Y = ((height / 3) - text_size.Height) / 2;
            text_point.Y += (height / 3) * 2;
            g.DrawString(text, font, new SolidBrush(Color.Red), text_point);
            text_point.X = ((width / 2) - text_size.Width) / 2;
            text_point.X += (width / 2);
            text_point.Y = ((height / 3) - text_size.Height) / 2;
            text_point.Y += (height / 3) * 2;
            g.DrawString(text, font, new SolidBrush(Color.Black), text_point);

            g.Dispose();
            return bitmap;
        }

        private void DrawerUI_EPD_583_UDP_DataReceiveMouseDownRightEvent(string selectedText, List<System.Net.IPEndPoint> iPEndPoints)
        {
            if (selectedText == ContextMenuStrip_Main.畫面設置.GetEnumName())
            {
                Dialog_ContextMenuStrip dialog_ContextMenuStrip = new Dialog_ContextMenuStrip(new ContextMenuStrip_UDP_DataReceive_畫面設置().GetEnumNames());
                if (dialog_ContextMenuStrip.ShowDialog() == System.Windows.Forms.DialogResult.Yes)
                {
                    if (dialog_ContextMenuStrip.Value == ContextMenuStrip_UDP_DataReceive_畫面設置.繪製儲位.GetEnumName())
                    {
                        List<Task> taskList = new List<Task>();
                        for (int i = 0; i < iPEndPoints.Count; i++)
                        {
                            string IP = iPEndPoints[i].Address.ToString();
                            int Port = iPEndPoints[i].Port;
                            Drawer drawer = this.SQL_GetDrawer(IP);
                            if (drawer == null) continue;
                            taskList.Add(Task.Run(() =>
                            {
                                this.DrawToEpd_UDP(drawer);
                            }));
                        }
                        Task allTask = Task.WhenAll(taskList);

                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_UDP_DataReceive_畫面設置.測試資訊.GetEnumName())
                    {
                        List<Task> taskList = new List<Task>();
                        for (int i = 0; i < iPEndPoints.Count; i++)
                        {
                            string IP = iPEndPoints[i].Address.ToString();
                            int Port = iPEndPoints[i].Port;
                            taskList.Add(Task.Run(() =>
                            {
                                using (Bitmap bitmap = this.DrawTestImage(IP, Port))
                                {
                                    this.DrawToEpd_UDP(IP, Port, bitmap);
                                }
                            }));
                        }
                        Task allTask = Task.WhenAll(taskList);

                    }

                }
            }
            else if (selectedText == ContextMenuStrip_Main.IO設定.GetEnumName())
            {
                Dialog_ContextMenuStrip dialog_ContextMenuStrip = new Dialog_ContextMenuStrip(new ContextMenuStrip_UDP_DataReceive_IO設定().GetEnumNames());
                if (dialog_ContextMenuStrip.ShowDialog() == DialogResult.Yes)
                {
                    if (dialog_ContextMenuStrip.Value == ContextMenuStrip_UDP_DataReceive_IO設定.面板亮燈.GetEnumName())
                    {
                        List<Task> taskList = new List<Task>();
                        ColorDialog colorDialog = new ColorDialog();
                        if (colorDialog.ShowDialog() != DialogResult.OK) return;
                        for (int i = 0; i < iPEndPoints.Count; i++)
                        {
                            string IP = iPEndPoints[i].Address.ToString();
                            int Port = iPEndPoints[i].Port;
                            Color color = colorDialog.Color;
                            taskList.Add(Task.Run(() =>
                            {
                                byte[] led_bytes = Get_Drawer_LED_UDP(IP, Port);
                                this.Set_Pannel_LED_UDP(IP, Port, led_bytes, color);
                            }));
                        }
                        Task allTask = Task.WhenAll(taskList);
                    }
                    if (dialog_ContextMenuStrip.Value == ContextMenuStrip_UDP_DataReceive_IO設定.IO測試.GetEnumName())
                    {
                        List<object[]> list_value = this.sqL_DataGridView_UDP_DataReceive.Get_All_Select_RowsValues();
                        if (list_value.Count == 0) return;
                        string IP = list_value[0][(int)enum_UDP_DataReceive.IP].ObjectToString();
                        int Port = list_value[0][(int)enum_UDP_DataReceive.Port].ObjectToString().StringToInt32();
                        UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
                        if (uDP_Class == null) return;
                        Dialog_IO測試 Dialog_IO測試 = new Dialog_IO測試(uDP_Class, IP, list_UDP_Rx);
                        Dialog_IO測試.ShowDialog();
                    }
                    if (dialog_ContextMenuStrip.Value == ContextMenuStrip_UDP_DataReceive_IO設定.全部亮燈測試.GetEnumName())
                    {
                        List<Task> taskList = new List<Task>();
                        ColorDialog colorDialog = new ColorDialog();
                        if (colorDialog.ShowDialog() != DialogResult.OK) return;
                        for (int i = 0; i < iPEndPoints.Count; i++)
                        {
                            string IP = iPEndPoints[i].Address.ToString();
                            int Port = iPEndPoints[i].Port;
                            Color color = colorDialog.Color;
                            taskList.Add(Task.Run(() =>
                            {
                                byte[] led_bytes = Get_Drawer_LED_UDP(IP, Port);
                                this.Set_LED_UDP(IP, Port, color);
                            }));
                        }
                        Task allTask = Task.WhenAll(taskList);
                    }
                    if (dialog_ContextMenuStrip.Value == ContextMenuStrip_UDP_DataReceive_IO設定.清除燈號.GetEnumName())
                    {
                        List<Task> taskList = new List<Task>();
                        for (int i = 0; i < iPEndPoints.Count; i++)
                        {
                            string IP = iPEndPoints[i].Address.ToString();
                            int Port = iPEndPoints[i].Port;
                            taskList.Add(Task.Run(() =>
                            {
                                this.Set_LED_Clear_UDP(IP, Port);
                            }));
                        }
                        Task allTask = Task.WhenAll(taskList);
                    }
                }

            }
            else if (selectedText == ContextMenuStrip_Main.設為大抽屜4X8.GetEnumName())
            {
                if (MyMessageBox.ShowDialog("確認將設為大抽屜,且清除該抽屜所有儲位?", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) == DialogResult.Yes)
                {
                    List<Task> taskList = new List<Task>();
                    for (int i = 0; i < iPEndPoints.Count; i++)
                    {
                        string IP = iPEndPoints[i].Address.ToString();
                        int Port = iPEndPoints[i].Port;
                        Drawer drawer = this.SQL_GetDrawer(IP);
                        if (drawer == null) continue;
                        drawer.SetDrawerType(Drawer.Enum_DrawerType._4X8);
                        taskList.Add(Task.Run(() =>
                        {
                            SQL_ReplaceDrawer(drawer);
                        }));
                    }
                    Task allTask = Task.WhenAll(taskList);
                }
            }
            else if (selectedText == ContextMenuStrip_Main.設為小抽屜3X8.GetEnumName())
            {
                if (MyMessageBox.ShowDialog("確認將設為小抽屜,且清除該抽屜所有儲位?", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) == DialogResult.Yes)
                {
                    List<Task> taskList = new List<Task>();
                    for (int i = 0; i < iPEndPoints.Count; i++)
                    {
                        string IP = iPEndPoints[i].Address.ToString();
                        int Port = iPEndPoints[i].Port;
                        Drawer drawer = this.SQL_GetDrawer(IP);
                        if (drawer == null) continue;
                        drawer.SetDrawerType(Drawer.Enum_DrawerType._3X8);
                        taskList.Add(Task.Run(() =>
                        {
                            SQL_ReplaceDrawer(drawer);
                        }));
                    }
                    Task allTask = Task.WhenAll(taskList);
                }
            }
            else if (selectedText == ContextMenuStrip_Main.設為有鎖控.GetEnumName())
            {
                List<Task> taskList = new List<Task>();
                for (int i = 0; i < iPEndPoints.Count; i++)
                {
                    string IP = iPEndPoints[i].Address.ToString();
                    int Port = iPEndPoints[i].Port;
                    Drawer drawer = this.SQL_GetDrawer(IP);
                    if (drawer == null) continue;
                    drawer.SetDeviceType(DeviceType.EPD583_lock);
                    taskList.Add(Task.Run(() =>
                    {
                        SQL_ReplaceDrawer(drawer);
                    }));
                }
                Task allTask = Task.WhenAll(taskList);
                this.sqL_DataGridView_DeviceTable.SQL_GetAllRows(true);
            }
            else if (selectedText == ContextMenuStrip_Main.設為無鎖控.GetEnumName())
            {
                List<Task> taskList = new List<Task>();
                for (int i = 0; i < iPEndPoints.Count; i++)
                {
                    string IP = iPEndPoints[i].Address.ToString();
                    int Port = iPEndPoints[i].Port;
                    Drawer drawer = this.SQL_GetDrawer(IP);
                    if (drawer == null) continue;
                    drawer.SetDeviceType(DeviceType.EPD583);
                    taskList.Add(Task.Run(() =>
                    {
                        SQL_ReplaceDrawer(drawer);
                    }));
                }
                Task allTask = Task.WhenAll(taskList);
                this.sqL_DataGridView_DeviceTable.SQL_GetAllRows(true);
            }
        }
        private void DrawerUI_EPD_583_DeviceTableMouseDownRightEvent(string selectedText, List<System.Net.IPEndPoint> iPEndPoints)
        {
            if (selectedText == ContextMenuStrip_Main.畫面設置.GetEnumName())
            {
                Dialog_ContextMenuStrip dialog_ContextMenuStrip = new Dialog_ContextMenuStrip(new ContextMenuStrip_DeviceTable_畫面設置().GetEnumNames());
                if (dialog_ContextMenuStrip.ShowDialog() == System.Windows.Forms.DialogResult.Yes)
                {
                    if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable_畫面設置.繪製儲位.GetEnumName())
                    {
                        List<Task> taskList = new List<Task>();
                        for (int i = 0; i < iPEndPoints.Count; i++)
                        {
                            string IP = iPEndPoints[i].Address.ToString();
                            int Port = iPEndPoints[i].Port;
                            Drawer drawer = this.SQL_GetDrawer(IP);
                            if (drawer == null) continue;
                            taskList.Add(Task.Run(() =>
                            {
                                this.DrawToEpd_UDP(drawer);
                            }));
                        }
                        Task allTask = Task.WhenAll(taskList);
               
                    }
                    else if(dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable_畫面設置.測試資訊.GetEnumName())
                    {
                        List<Task> taskList = new List<Task>();
                        for (int i = 0; i < iPEndPoints.Count; i++)
                        {
                            string IP = iPEndPoints[i].Address.ToString();
                            int Port = iPEndPoints[i].Port;
                            taskList.Add(Task.Run(() =>
                            {
                                using (Bitmap bitmap = this.DrawTestImage(IP, Port))
                                {
                                    this.DrawToEpd_UDP(IP, Port, bitmap);
                                }
                            }));
                        }
                        Task allTask = Task.WhenAll(taskList);
                     
                    }

                }
            }
            else if (selectedText == ContextMenuStrip_Main.IO設定.GetEnumName())
            {
                Dialog_ContextMenuStrip dialog_ContextMenuStrip = new Dialog_ContextMenuStrip(new ContextMenuStrip_DeviceTable_IO設定().GetEnumNames());
                if (dialog_ContextMenuStrip.ShowDialog() == DialogResult.Yes)
                {
                    if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable_IO設定.面板亮燈.GetEnumName())
                    {
                        List<Task> taskList = new List<Task>();
                        ColorDialog colorDialog = new ColorDialog();
                        if (colorDialog.ShowDialog() != DialogResult.OK) return;
                        for (int i = 0; i < iPEndPoints.Count; i++)
                        {
                            string IP = iPEndPoints[i].Address.ToString();
                            int Port = iPEndPoints[i].Port;
                            Color color = colorDialog.Color;
                            taskList.Add(Task.Run(() =>
                            {
                                byte[] led_bytes = Get_Drawer_LED_UDP(IP, Port);
                                this.Set_Pannel_LED_UDP(IP, Port, led_bytes, color);
                            }));
                        }
                        Task allTask = Task.WhenAll(taskList);
                    }
                    if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable_IO設定.IO測試.GetEnumName())
                    {
                        List<object[]> list_value = this.sqL_DataGridView_DeviceTable.Get_All_Select_RowsValues();
                        if (list_value.Count == 0) return;
                        string IP = list_value[0][(int)enum_DeviceTable.IP].ObjectToString();
                        int Port = list_value[0][(int)enum_DeviceTable.Port].ObjectToString().StringToInt32();
                        UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
                        if (uDP_Class == null) return;                       
                        Dialog_IO測試 Dialog_IO測試 = new Dialog_IO測試(uDP_Class, IP, list_UDP_Rx);
                        Dialog_IO測試.ShowDialog();
                    }
                    if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable_IO設定.全部亮燈測試.GetEnumName())
                    {
                        List<Task> taskList = new List<Task>();
                        ColorDialog colorDialog = new ColorDialog();
                        if (colorDialog.ShowDialog() != DialogResult.OK) return;
                        for (int i = 0; i < iPEndPoints.Count; i++)
                        {
                            string IP = iPEndPoints[i].Address.ToString();
                            int Port = iPEndPoints[i].Port;
                            Color color = colorDialog.Color;
                            taskList.Add(Task.Run(() =>
                            {
                                byte[] led_bytes = Get_Drawer_LED_UDP(IP, Port);
                                this.Set_LED_UDP(IP, Port, color);
                            }));
                        }
                        Task allTask = Task.WhenAll(taskList);
                    }
                    if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable_IO設定.清除燈號.GetEnumName())
                    {
                        List<Task> taskList = new List<Task>();
                        for (int i = 0; i < iPEndPoints.Count; i++)
                        {
                            string IP = iPEndPoints[i].Address.ToString();
                            int Port = iPEndPoints[i].Port;
                            taskList.Add(Task.Run(() =>
                            {
                                this.Set_LED_Clear_UDP(IP, Port);
                            }));
                        }
                        Task allTask = Task.WhenAll(taskList);
                    }
                   
                }

            }
            else if (selectedText == ContextMenuStrip_Main.設為大抽屜4X8.GetEnumName())
            {
                if (MyMessageBox.ShowDialog("確認將設為大抽屜,且清除該抽屜所有儲位?", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) == DialogResult.Yes)
                {
                    List<Task> taskList = new List<Task>();
                    for (int i = 0; i < iPEndPoints.Count; i++)
                    {
                        string IP = iPEndPoints[i].Address.ToString();
                        int Port = iPEndPoints[i].Port;
                        Drawer drawer = this.SQL_GetDrawer(IP);
                        if (drawer == null) continue;
                        drawer.SetDrawerType(Drawer.Enum_DrawerType._4X8);
                        taskList.Add(Task.Run(() =>
                        {
                            SQL_ReplaceDrawer(drawer);
                        }));
                    }
                    Task allTask = Task.WhenAll(taskList);
                }
            }
            else if (selectedText == ContextMenuStrip_Main.設為小抽屜3X8.GetEnumName())
            {
                if (MyMessageBox.ShowDialog("確認將設為小抽屜,且清除該抽屜所有儲位?", MyMessageBox.enum_BoxType.Warning, MyMessageBox.enum_Button.Confirm_Cancel) == DialogResult.Yes)
                {
                    List<Task> taskList = new List<Task>();
                    for (int i = 0; i < iPEndPoints.Count; i++)
                    {
                        string IP = iPEndPoints[i].Address.ToString();
                        int Port = iPEndPoints[i].Port;
                        Drawer drawer = this.SQL_GetDrawer(IP);
                        if (drawer == null) continue;
                        drawer.SetDrawerType(Drawer.Enum_DrawerType._3X8);
                        taskList.Add(Task.Run(() =>
                        {
                            SQL_ReplaceDrawer(drawer);
                        }));
                    }
                    Task allTask = Task.WhenAll(taskList);
                }
            }
            else if (selectedText == ContextMenuStrip_Main.設為有鎖控.GetEnumName())
            {
                List<Task> taskList = new List<Task>();
                for (int i = 0; i < iPEndPoints.Count; i++)
                {
                    string IP = iPEndPoints[i].Address.ToString();
                    int Port = iPEndPoints[i].Port;
                    Drawer drawer = this.SQL_GetDrawer(IP);
                    if (drawer == null) continue;
                    drawer.SetDeviceType(DeviceType.EPD583_lock);
                    taskList.Add(Task.Run(() =>
                    {
                        SQL_ReplaceDrawer(drawer);
                    }));
                }
                Task allTask = Task.WhenAll(taskList);
                this.sqL_DataGridView_DeviceTable.SQL_GetAllRows(true);
            }
            else if (selectedText == ContextMenuStrip_Main.設為無鎖控.GetEnumName())
            {
                List<Task> taskList = new List<Task>();
                for (int i = 0; i < iPEndPoints.Count; i++)
                {
                    string IP = iPEndPoints[i].Address.ToString();
                    int Port = iPEndPoints[i].Port;
                    Drawer drawer = this.SQL_GetDrawer(IP);
                    if (drawer == null) continue;
                    drawer.SetDeviceType(DeviceType.EPD583);
                    taskList.Add(Task.Run(() =>
                    {
                        SQL_ReplaceDrawer(drawer);
                    }));
                }
                Task allTask = Task.WhenAll(taskList);
                this.sqL_DataGridView_DeviceTable.SQL_GetAllRows(true);
            }
        }

    }
}
