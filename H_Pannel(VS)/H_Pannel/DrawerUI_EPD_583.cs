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
        public class LightSensorClass
        {
            public List<string> H_Sensor_Check = new List<string>();
            public List<string> V_Sensor_Check = new List<string>();

            public override string ToString()
            {
                string str_H = "";
                string str_V = "";
                for (int i = 0; i < H_Sensor_Check.Count; i++)
                {
                    str_H += H_Sensor_Check[i];
                    if (i != H_Sensor_Check.Count - 1) str_H += ",";
                    else str_H = $"「{str_H}」";
                }
                for (int i = 0; i < V_Sensor_Check.Count; i++)
                {
                    str_V += V_Sensor_Check[i];
                    if (i != V_Sensor_Check.Count - 1) str_V += ",";
                    else str_V = $"「{str_V}」";
                }
                return $"LightSensorClass , {str_H} {str_V} ";
            }

          
        }
        static int LocalPort = 0;
        static int ServerPort = 0;
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
        static public double Lightness = 1.0D;
        static public int NumOfLED = 450;
        //static private int drawer.Num_Of_Columns = 4;
        //static private int drawer.Num_Of_Rows = 8;



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
            return LEDBytes_Pannel_Clear(drawer.LED_Bytes, drawer.DrawerType);
        }
        static public byte[] LEDBytes_Pannel_Clear(byte[] LED_Bytes, Drawer.Enum_DrawerType enum_DrawerType)
        {
            int NumOfLED_Pannel = 42;
            int NumOfLED_Drawer = 450 - NumOfLED_Pannel;
            if (enum_DrawerType == Drawer.Enum_DrawerType._3X8 || enum_DrawerType == Drawer.Enum_DrawerType._4X8)
            {
                NumOfLED_Pannel = 42;
                NumOfLED_Drawer = 450 - NumOfLED_Pannel;
            }
            if (enum_DrawerType == Drawer.Enum_DrawerType._4X8_A || enum_DrawerType == Drawer.Enum_DrawerType._5X8_A)
            {
                NumOfLED_Pannel = 6;
                NumOfLED_Drawer = 372 - NumOfLED_Pannel;
            }

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

        static public byte[] Set_LEDBytes(Drawer drawer, Color color)
        {
            for (int i = 0; i < NumOfLED; i++)
            {
                drawer.LED_Bytes[i * 3 + 0] = (byte)(color.R );
                drawer.LED_Bytes[i * 3 + 1] = (byte)(color.G );
                drawer.LED_Bytes[i * 3 + 2] = (byte)(color.B );
            }
            return drawer.LED_Bytes;
        }
        static public byte[] Set_LEDBytes(Drawer drawer, List<Box> boxes, Color color)
        {
            return Set_LEDBytes(drawer, boxes, color, false);
        }
        static public byte[] Set_LEDBytes(Drawer drawer, List<Box> boxes, Color color , bool check_lighton)
        {
            byte[] led_bytes = drawer.LED_Bytes;
            for(int i = 0; i < boxes.Count; i++)
            {
                if(boxes[i].IP == drawer.IP)
                {
                    if (color == Color.Black || check_lighton == false)
                    {
                        led_bytes = Set_LEDBytes(drawer, boxes[i].Column, boxes[i].Row, color);
                        boxes[i].LightOn = false;
                        continue;
                    }
                    if (boxes[i].LightOn == false)
                    {
                        led_bytes = Set_LEDBytes(drawer, boxes[i].Column, boxes[i].Row, color);
                        boxes[i].LightOn = true;
                    }
                }             
            }
            return led_bytes;
        }
        static public byte[] Set_LEDBytes(Drawer drawer, List<Box> boxes, Color color, ref byte[] LEDBytes)
        {
            for (int i = 0; i < boxes.Count; i++)
            {
                if (boxes[i].IP == drawer.IP)
                {
                    LEDBytes = Set_LEDBytes(drawer, boxes[i].Column, boxes[i].Row, ref LEDBytes, color);
                }

            }
            return LEDBytes;
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
            int width = rect.Width / (Pannel_Width / drawer.Num_Of_Columns);
            int height = rect.Height / (Pannel_Height / drawer.Num_Of_Rows);
            return Set_LEDBytes(col, row, width, height, ref drawer.LED_Bytes, color, drawer.DrawerType);
        }
        static public byte[] Set_LEDBytes(Drawer drawer, int col_x, int row_y, ref byte[] LEDBytes, Color color)
        {
            if (col_x >= drawer.Boxes.Count) return LEDBytes;
            for (int i = 0; i < drawer.Boxes.Count; i++)
            {
                if (row_y >= drawer.Boxes[i].Length) return LEDBytes;
            }

            Rectangle rect = Get_Box_Combine(drawer, col_x, row_y);
            int width = rect.Width / (Pannel_Width / drawer.Num_Of_Columns);
            int height = rect.Height / (Pannel_Height / drawer.Num_Of_Rows);
            return Set_LEDBytes(col_x, row_y, width, height, ref LEDBytes, color, drawer.DrawerType);
        }
        static public byte[] Set_LEDBytes(int col_x, int row_y, int width, int height, ref byte[] LEDBytes, Color color, Drawer.Enum_DrawerType enum_DrawerType)
        {
            int Num_Of_Columns = 4;
            int Num_Of_Rows = 8;
            if (enum_DrawerType == Drawer.Enum_DrawerType._4X8)
            {
                Num_Of_Columns = 4;
                Num_Of_Rows = 8;
            }
            if (enum_DrawerType == Drawer.Enum_DrawerType._3X8)
            {
                Num_Of_Columns = 4;
                Num_Of_Rows = 8;
            }
            if (enum_DrawerType == Drawer.Enum_DrawerType._4X8_A)
            {
                Num_Of_Columns = 5;
                Num_Of_Rows = 8;
            }
            if (enum_DrawerType == Drawer.Enum_DrawerType._5X8_A)
            {
                Num_Of_Columns = 5;
                Num_Of_Rows = 8;
            }
            for (int i = 0; i < width; i++)
            {
                Set_Drawer_H_Leds(col_x + i + row_y * Num_Of_Columns, ref LEDBytes, color, enum_DrawerType);
                Set_Drawer_H_Leds(col_x + i + (height + row_y) * Num_Of_Columns, ref LEDBytes, color, enum_DrawerType);

            }
            for (int k = 0; k < height; k++)
            {
                Set_Drawer_V_Leds(row_y + k + col_x * Num_Of_Rows, ref LEDBytes, color, enum_DrawerType);
                Set_Drawer_V_Leds(row_y + k + (width + col_x) * Num_Of_Rows, ref LEDBytes, color, enum_DrawerType);
            }
            return LEDBytes;
        }

        static public byte[] Set_Pannel_LEDBytes(Drawer drawer, Color color)
        {
            if (drawer.IsAllLight == false)
            {
                return Set_LEDBytes(drawer, color);
            }
            return Set_Pannel_LEDBytes(ref drawer.LED_Bytes, color, drawer.DrawerType);
        }
        static public byte[] Set_Pannel_LEDBytes(ref byte[] LED_Bytes, Color color, Drawer.Enum_DrawerType enum_DrawerType)
        {
            int NumOfLED_Pannel = 42;
            int NumOfLED_Drawer = 450 - NumOfLED_Pannel;
            if (enum_DrawerType == Drawer.Enum_DrawerType._3X8 || enum_DrawerType == Drawer.Enum_DrawerType._4X8)
            {
                NumOfLED_Pannel = 42;
                NumOfLED_Drawer = 450 - NumOfLED_Pannel;
            }
            if (enum_DrawerType == Drawer.Enum_DrawerType._4X8_A || enum_DrawerType == Drawer.Enum_DrawerType._5X8_A)
            {
                NumOfLED_Pannel = 6;
                NumOfLED_Drawer = 372 - NumOfLED_Pannel;
            }
            for (int i = NumOfLED_Drawer ; i < NumOfLED ; i++)
            {
                if (i * 3 > LED_Bytes.Length) return LED_Bytes;
                LED_Bytes[i * 3 + 0] = (byte)(color.R );
                LED_Bytes[i * 3 + 1] = (byte)(color.G );
                LED_Bytes[i * 3 + 2] = (byte)(color.B );
            }
            return LED_Bytes;
        }
 
        static public bool Set_Pannel_LED_UDP(UDP_Class uDP_Class, Drawer drawer, Color color)
        {
            return Set_Pannel_LED_UDP(uDP_Class, drawer.IP, drawer.LED_Bytes, color,drawer.DrawerType);
        }
        static public bool Set_Pannel_LED_UDP(UDP_Class uDP_Class, string IP, byte[] LED_Bytes, Color color)
        {
            return Set_Pannel_LED_UDP(uDP_Class, IP, LED_Bytes, color, Drawer.Enum_DrawerType._4X8);
        }

        static public bool Set_Pannel_LED_UDP(UDP_Class uDP_Class, string IP, byte[] LED_Bytes, Color color, Drawer.Enum_DrawerType enum_DrawerType)
        {
            LED_Bytes = Set_Pannel_LEDBytes(ref LED_Bytes, color, enum_DrawerType);
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
                byte[] LED_Bytes_buf = new byte[LED_Bytes.Length];
                for (int i = 0; i < (LED_Bytes_buf.Length / 3); i++)
                {
                    LED_Bytes_buf[i * 3 + 0] = (byte)(LED_Bytes[i * 3 + 0] * Lightness);
                    LED_Bytes_buf[i * 3 + 1] = (byte)(LED_Bytes[i * 3 + 1] * Lightness);
                    LED_Bytes_buf[i * 3 + 2] = (byte)(LED_Bytes[i * 3 + 2] * Lightness);
                }

                return Communication.Set_WS2812_Buffer(uDP_Class, IP, 0, LED_Bytes_buf);
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
                LED_Bytes[i * 3 + 0] = (byte)(color.R );
                LED_Bytes[i * 3 + 1] = (byte)(color.G );
                LED_Bytes[i * 3 + 2] = (byte)(color.B );
            }
            return Set_LED_UDP(uDP_Class, IP, LED_Bytes);
        }

        static public bool Set_LED_Clear_UDP(UDP_Class uDP_Class, Drawer drawer)
        {
            drawer.SetAllBoxes_LightOff();
            return Set_LED_Clear_UDP(uDP_Class, drawer.IP);
        }
        static public bool Set_LED_Clear_UDP(UDP_Class uDP_Class, string IP)
        {
            byte[] LED_Bytes = Get_Empty_LEDBytes();
            int cnt = 0;
            while(true)
            {
                if (cnt >= 1) break;
                if (!Set_LED_UDP(uDP_Class, IP, LED_Bytes))
                {
                    return false;
                }
                cnt++;
            }
           
           
            return true;
        }

        static public bool DrawToEpd_UDP(UDP_Class uDP_Class, Drawer drawer)
        {
            using (Bitmap bitmap = Get_Drawer_bmp(drawer))
            {
                return DrawToEpd_UDP(uDP_Class, drawer.IP, bitmap , drawer.DeviceType);
            }
        }
        static public bool DrawToEpd_BarCode_UDP(UDP_Class uDP_Class, Drawer drawer)
        {
            using (Bitmap bitmap = Get_Drawer_Barcode_bmp(drawer))
            {
                return DrawToEpd_UDP(uDP_Class, drawer.IP, bitmap);
            }
        }
        static public bool DrawToEpd_UDP(UDP_Class uDP_Class, string IP, Bitmap bmp , DeviceType deviceType)
        {
            if (deviceType == DeviceType.EPD730 || deviceType == DeviceType.EPD730_lock)
            {
                return Communication.EPD_730_DrawImage(uDP_Class, IP, bmp);
            }
            return Communication.EPD_583_DrawImage(uDP_Class, IP, bmp);
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

        static public void Set_Drawer_H_Leds(int col, ref byte[] LEDBytes, Color color, Drawer.Enum_DrawerType enum_DrawerType)
        {
            List<int[]> List_Drawer_H_Line_Leds = Get_H_Line_Leds(enum_DrawerType);
            for (int i = 0; i < List_Drawer_H_Line_Leds[col].Length; i++)
            {
                LEDBytes[List_Drawer_H_Line_Leds[col][i] * 3 + 0] = (byte)(color.R );
                LEDBytes[List_Drawer_H_Line_Leds[col][i] * 3 + 1] = (byte)(color.G );
                LEDBytes[List_Drawer_H_Line_Leds[col][i] * 3 + 2] = (byte)(color.B );
            }
        }
        static public void Set_Drawer_V_Leds(int row, ref byte[] LEDBytes, Color color, Drawer.Enum_DrawerType enum_DrawerType)
        {
            List<int[]> List_Drawer_V_Line_Leds = Get_V_Line_Leds(enum_DrawerType);
            for (int i = 0; i < List_Drawer_V_Line_Leds[row].Length; i++)
            {

                LEDBytes[List_Drawer_V_Line_Leds[row][i] * 3 + 0] = (byte)(color.R );
                LEDBytes[List_Drawer_V_Line_Leds[row][i] * 3 + 1] = (byte)(color.G );
                LEDBytes[List_Drawer_V_Line_Leds[row][i] * 3 + 2] = (byte)(color.B );
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
        static public Rectangle Get_Box_rect(Drawer drawer, Box box)
        {
            Rectangle rectangle = Get_Box_Combine(drawer, box);
            int width = rectangle.Width / (Pannel_Width / drawer.Num_Of_Columns);
            int height = rectangle.Height / (Pannel_Height / drawer.Num_Of_Rows);
            return new Rectangle(box.Column, box.Row, width, height);
        }
        static public LightSensorClass Get_LightSensorClass(Rectangle rectangle)
        {
            LightSensorClass lightSensorClass = new LightSensorClass();
            int x = rectangle.X;
            int y = rectangle.Y;
            int width = rectangle.Width;
            int height = rectangle.Height;

            for (int i = x; i < (x + width); i++)
            {
                if (i == 0) lightSensorClass.V_Sensor_Check.Add("Y0");
                else if (i == 1) lightSensorClass.V_Sensor_Check.Add("Y1");
                else if (i == 2) lightSensorClass.V_Sensor_Check.Add("Y2");
                else if (i == 3) lightSensorClass.V_Sensor_Check.Add("Y3");
            }
            for (int i = y; i < (y + height); i++)
            {
                if (i == 0 || i == 1) lightSensorClass.H_Sensor_Check.Add("X0");
                else if (i == 2 || i == 3) lightSensorClass.H_Sensor_Check.Add("X1");
                else if (i == 4 || i == 5) lightSensorClass.H_Sensor_Check.Add("X2");
                else if (i == 6 || i == 7) lightSensorClass.H_Sensor_Check.Add("X3");
            }

            lightSensorClass.V_Sensor_Check = (from temp in lightSensorClass.V_Sensor_Check
                                               select temp).Distinct().ToList();

            lightSensorClass.H_Sensor_Check = (from temp in lightSensorClass.H_Sensor_Check
                                               select temp).Distinct().ToList();

            return lightSensorClass;

        }

        static public Bitmap Get_Drawer_Barcode_bmp(Drawer drawer)
        {
            Bitmap bitmap = new Bitmap(DrawerUI_EPD_583.Pannel_Width, DrawerUI_EPD_583.Pannel_Height);
            Graphics g = Graphics.FromImage(bitmap);
            List<Box[]> Boxes = drawer.Boxes;
            for (int i = 0; i < Boxes.Count; i++)
            {
                for (int k = 0; k < Boxes[i].Length; k++)
                {
                    if (Boxes[i][k].Slave == false)
                    {
                        Box _box = Boxes[i][k];
                        Rectangle rect = Get_Box_Combine(drawer, _box);

                        g.FillRectangle(new SolidBrush(_box.BackColor), rect);
                        g.DrawRectangle(new Pen(Color.Black, _box.Pen_Width), rect);

                        g.SmoothingMode = SmoothingMode.HighQuality; //使繪圖質量最高，即消除鋸齒
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.CompositingQuality = CompositingQuality.HighQuality;
                        g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

                        if (_box.Code.StringIsEmpty() == true) _box.Code = "None";
                        SizeF size_Code = TextRenderer.MeasureText($"{_box.Code}", new Font("微軟正黑體", 9));
                        g.DrawString($"{_box.Code}", new Font("微軟正黑體", 9), new SolidBrush(Color.Black), ((rect.Width - size_Code.Width) / 2) + rect.X, (rect.Y + 3));

                        Bitmap bitmap_barcode = Communication.CreateBarCode(_box.BarCode, 150, 40);
                        g.DrawImage(bitmap_barcode, ((rect.Width - 150) / 2) + rect.X, (rect.Height - 40) + rect.Y);
                        bitmap_barcode.Dispose();
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
        static public Bitmap Get_Drawer_bmp(Drawer drawer)
        {
            if (drawer.DeviceType == DeviceType.EPD730 || drawer.DeviceType == DeviceType.EPD730_lock)
            {
                return Communication.EPD730_GetBitmap(drawer);
            }
            return Communication.EPD583_GetBitmap(drawer);

        }

        #endregion
        #region LED_Line
        public static List<int[]> Get_H_Line_Leds(Drawer.Enum_DrawerType enum_DrawerType)
        {
            List<int[]> List_Drawer_H_Line_Leds = new List<int[]>();
            if (enum_DrawerType == Drawer.Enum_DrawerType._3X8 || enum_DrawerType == Drawer.Enum_DrawerType._4X8)
            {
                int[] Line_H_00 = new int[] { 376 + 0, 376 + 1, 376 + 2, 376 + 3, 376 + 4, 376 + 5, 376 + 6, 376 + 7 };
                int[] Line_H_01 = new int[] { 384 + 0, 384 + 1, 384 + 2, 384 + 3, 384 + 4, 384 + 5, 384 + 6, 384 + 7 };
                int[] Line_H_02 = new int[] { 392 + 0, 392 + 1, 392 + 2, 392 + 3, 392 + 4, 392 + 5, 392 + 6, 392 + 7 };
                int[] Line_H_03 = new int[] { 400 + 0, 400 + 1, 400 + 2, 400 + 3, 400 + 4, 400 + 5, 400 + 6, 400 + 7 };

                int[] Line_H_04 = new int[] { 368 + 0, 368 + 1, 368 + 2, 368 + 3, 368 + 4, 368 + 5, 368 + 6, 368 + 7 };
                int[] Line_H_05 = new int[] { 360 + 0, 360 + 1, 360 + 2, 360 + 3, 360 + 4, 360 + 5, 360 + 6, 360 + 7 };
                int[] Line_H_06 = new int[] { 352 + 0, 352 + 1, 352 + 2, 352 + 3, 352 + 4, 352 + 5, 352 + 6, 352 + 7 };
                int[] Line_H_07 = new int[] { 344 + 0, 344 + 1, 344 + 2, 344 + 3, 344 + 4, 344 + 5, 344 + 6, 344 + 7 };

                int[] Line_H_08 = new int[] { 312 + 0, 312 + 1, 312 + 2, 312 + 3, 312 + 4, 312 + 5, 312 + 6, 312 + 7 };
                int[] Line_H_09 = new int[] { 320 + 0, 320 + 1, 320 + 2, 320 + 3, 320 + 4, 320 + 5, 320 + 6, 320 + 7 };
                int[] Line_H_10 = new int[] { 328 + 0, 328 + 1, 328 + 2, 328 + 3, 328 + 4, 328 + 5, 328 + 6, 328 + 7 };
                int[] Line_H_11 = new int[] { 336 + 0, 336 + 1, 336 + 2, 336 + 3, 336 + 4, 336 + 5, 336 + 6, 336 + 7 };

                int[] Line_H_12 = new int[] { 304 + 0, 304 + 1, 304 + 2, 304 + 3, 304 + 4, 304 + 5, 304 + 6, 304 + 7 };
                int[] Line_H_13 = new int[] { 296 + 0, 296 + 1, 296 + 2, 296 + 3, 296 + 4, 296 + 5, 296 + 6, 296 + 7 };
                int[] Line_H_14 = new int[] { 288 + 0, 288 + 1, 288 + 2, 288 + 3, 288 + 4, 288 + 5, 288 + 6, 288 + 7 };
                int[] Line_H_15 = new int[] { 280 + 0, 280 + 1, 280 + 2, 280 + 3, 280 + 4, 280 + 5, 280 + 6, 280 + 7 };

                int[] Line_H_16 = new int[] { 248 + 0, 248 + 1, 248 + 2, 248 + 3, 248 + 4, 248 + 5, 248 + 6, 248 + 7 };
                int[] Line_H_17 = new int[] { 256 + 0, 256 + 1, 256 + 2, 256 + 3, 256 + 4, 256 + 5, 256 + 6, 256 + 7 };
                int[] Line_H_18 = new int[] { 264 + 0, 264 + 1, 264 + 2, 264 + 3, 264 + 4, 264 + 5, 264 + 6, 264 + 7 };
                int[] Line_H_19 = new int[] { 272 + 0, 272 + 1, 272 + 2, 272 + 3, 272 + 4, 272 + 5, 272 + 6, 272 + 7 };

                int[] Line_H_20 = new int[] { 240 + 0, 240 + 1, 240 + 2, 240 + 3, 240 + 4, 240 + 5, 240 + 6, 240 + 7 };
                int[] Line_H_21 = new int[] { 232 + 0, 232 + 1, 232 + 2, 232 + 3, 232 + 4, 232 + 5, 232 + 6, 232 + 7 };
                int[] Line_H_22 = new int[] { 224 + 0, 224 + 1, 224 + 2, 224 + 3, 224 + 4, 224 + 5, 224 + 6, 224 + 7 };
                int[] Line_H_23 = new int[] { 216 + 0, 216 + 1, 216 + 2, 216 + 3, 216 + 4, 216 + 5, 216 + 6, 216 + 7 };

                int[] Line_H_24 = new int[] { 184 + 0, 184 + 1, 184 + 2, 184 + 3, 184 + 4, 184 + 5, 184 + 6, 184 + 7 };
                int[] Line_H_25 = new int[] { 192 + 0, 192 + 1, 192 + 2, 192 + 3, 192 + 4, 192 + 5, 192 + 6, 192 + 7 };
                int[] Line_H_26 = new int[] { 200 + 0, 200 + 1, 200 + 2, 200 + 3, 200 + 4, 200 + 5, 200 + 6, 200 + 7 };
                int[] Line_H_27 = new int[] { 208 + 0, 208 + 1, 208 + 2, 208 + 3, 208 + 4, 208 + 5, 208 + 6, 208 + 7 };

                int[] Line_H_28 = new int[] { 176 + 0, 176 + 1, 176 + 2, 176 + 3, 176 + 4, 176 + 5, 176 + 6, 176 + 7 };
                int[] Line_H_29 = new int[] { 168 + 0, 168 + 1, 168 + 2, 168 + 3, 168 + 4, 168 + 5, 168 + 6, 168 + 7 };
                int[] Line_H_30 = new int[] { 160 + 0, 160 + 1, 160 + 2, 160 + 3, 160 + 4, 160 + 5, 160 + 6, 160 + 7 };
                int[] Line_H_31 = new int[] { 152 + 0, 152 + 1, 152 + 2, 152 + 3, 152 + 4, 152 + 5, 152 + 6, 152 + 7 };

                int[] Line_H_32 = new int[] { 120 + 0, 120 + 1, 120 + 2, 120 + 3, 120 + 4, 120 + 5, 120 + 6, 120 + 7 };
                int[] Line_H_33 = new int[] { 128 + 0, 128 + 1, 128 + 2, 128 + 3, 128 + 4, 128 + 5, 128 + 6, 128 + 7 };
                int[] Line_H_34 = new int[] { 136 + 0, 136 + 1, 136 + 2, 136 + 3, 136 + 4, 136 + 5, 136 + 6, 136 + 7 };
                int[] Line_H_35 = new int[] { 144 + 0, 144 + 1, 144 + 2, 144 + 3, 144 + 4, 144 + 5, 144 + 6, 144 + 7 };

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
            }
            if (enum_DrawerType == Drawer.Enum_DrawerType._4X8_A || enum_DrawerType == Drawer.Enum_DrawerType._5X8_A)
            {

                int index = 0;

                index = 336;
                for (int k = 0; k < 5; k++)
                {
                    List<int> values = new List<int>();
                    for (int i = 0; i < 6; i++)
                    {
                        values.Add(index + ((k * 6) + i));
                    }
                    List_Drawer_H_Line_Leds.Add(values.ToArray());
                }

                index = 335;
                for (int k = 0; k < 5; k++)
                {
                    List<int> values = new List<int>();
                    for (int i = 0; i < 6; i++)
                    {
                        values.Add(index - ((k * 6) + i));
                    }
                    List_Drawer_H_Line_Leds.Add(values.ToArray());
                }

                index = 276;
                for (int k = 0; k < 5; k++)
                {
                    List<int> values = new List<int>();
                    for (int i = 0; i < 6; i++)
                    {
                        values.Add(index + ((k * 6) + i));
                    }
                    List_Drawer_H_Line_Leds.Add(values.ToArray());
                }

                index = 275;
                for (int k = 0; k < 5; k++)
                {
                    List<int> values = new List<int>();
                    for (int i = 0; i < 6; i++)
                    {
                        values.Add(index - ((k * 6) + i));
                    }
                    List_Drawer_H_Line_Leds.Add(values.ToArray());
                }

                index = 216;
                for (int k = 0; k < 5; k++)
                {
                    List<int> values = new List<int>();
                    for (int i = 0; i < 6; i++)
                    {
                        values.Add(index + ((k * 6) + i));
                    }
                    List_Drawer_H_Line_Leds.Add(values.ToArray());
                }

                index = 215;
                for (int k = 0; k < 5; k++)
                {
                    List<int> values = new List<int>();
                    for (int i = 0; i < 6; i++)
                    {
                        values.Add(index - ((k * 6) + i));
                    }
                    List_Drawer_H_Line_Leds.Add(values.ToArray());
                }

                index = 156;
                for (int k = 0; k < 5; k++)
                {
                    List<int> values = new List<int>();
                    for (int i = 0; i < 6; i++)
                    {
                        values.Add(index + ((k * 6) + i));
                    }
                    List_Drawer_H_Line_Leds.Add(values.ToArray());
                }

                index = 155;
                for (int k = 0; k < 5; k++)
                {
                    List<int> values = new List<int>();
                    for (int i = 0; i < 6; i++)
                    {
                        values.Add(index - ((k * 6) + i));
                    }
                    List_Drawer_H_Line_Leds.Add(values.ToArray());
                }

                index = 96;
                for (int k = 0; k < 5; k++)
                {
                    List<int> values = new List<int>();
                    for (int i = 0; i < 6; i++)
                    {
                        values.Add(index + ((k * 6) + i));
                    }
                    List_Drawer_H_Line_Leds.Add(values.ToArray());
                }
            }
            return List_Drawer_H_Line_Leds;
        }
        public static List<int[]> Get_V_Line_Leds(Drawer.Enum_DrawerType enum_DrawerType)
        {
            List<int[]> List_Drawer_V_Line_Leds = new List<int[]>();
            if (enum_DrawerType == Drawer.Enum_DrawerType._3X8 || enum_DrawerType == Drawer.Enum_DrawerType._4X8)
            {
                int[] Line_V_00 = new int[] { 96 + (0 * 3) + 0, 96 + (0 * 3) + 1, 96 + (0 * 3) + 2 };
                int[] Line_V_01 = new int[] { 96 + (1 * 3) + 0, 96 + (1 * 3) + 1, 96 + (1 * 3) + 2 };
                int[] Line_V_02 = new int[] { 96 + (2 * 3) + 0, 96 + (2 * 3) + 1, 96 + (2 * 3) + 2 };
                int[] Line_V_03 = new int[] { 96 + (3 * 3) + 0, 96 + (3 * 3) + 1, 96 + (3 * 3) + 2 };
                int[] Line_V_04 = new int[] { 96 + (4 * 3) + 0, 96 + (4 * 3) + 1, 96 + (4 * 3) + 2 };
                int[] Line_V_05 = new int[] { 96 + (5 * 3) + 0, 96 + (5 * 3) + 1, 96 + (5 * 3) + 2 };
                int[] Line_V_06 = new int[] { 96 + (6 * 3) + 0, 96 + (6 * 3) + 1, 96 + (6 * 3) + 2 };
                int[] Line_V_07 = new int[] { 96 + (7 * 3) + 0, 96 + (7 * 3) + 1, 96 + (7 * 3) + 2 };

                int[] Line_V_08 = new int[] { 95 - (0 * 3) - 0, 95 - (0 * 3) - 1, 95 - (0 * 3) - 2 };
                int[] Line_V_09 = new int[] { 95 - (1 * 3) - 0, 95 - (1 * 3) - 1, 95 - (1 * 3) - 2 };
                int[] Line_V_10 = new int[] { 95 - (2 * 3) - 0, 95 - (2 * 3) - 1, 95 - (2 * 3) - 2 };
                int[] Line_V_11 = new int[] { 95 - (3 * 3) - 0, 95 - (3 * 3) - 1, 95 - (3 * 3) - 2 };
                int[] Line_V_12 = new int[] { 95 - (4 * 3) - 0, 95 - (4 * 3) - 1, 95 - (4 * 3) - 2 };
                int[] Line_V_13 = new int[] { 95 - (5 * 3) - 0, 95 - (5 * 3) - 1, 95 - (5 * 3) - 2 };
                int[] Line_V_14 = new int[] { 95 - (6 * 3) - 0, 95 - (6 * 3) - 1, 95 - (6 * 3) - 2 };
                int[] Line_V_15 = new int[] { 95 - (7 * 3) - 0, 95 - (7 * 3) - 1, 95 - (7 * 3) - 2 };

                int[] Line_V_16 = new int[] { 48 + (0 * 3) + 0, 48 + (0 * 3) + 1, 48 + (0 * 3) + 2 };
                int[] Line_V_17 = new int[] { 48 + (1 * 3) + 0, 48 + (1 * 3) + 1, 48 + (1 * 3) + 2 };
                int[] Line_V_18 = new int[] { 48 + (2 * 3) + 0, 48 + (2 * 3) + 1, 48 + (2 * 3) + 2 };
                int[] Line_V_19 = new int[] { 48 + (3 * 3) + 0, 48 + (3 * 3) + 1, 48 + (3 * 3) + 2 };
                int[] Line_V_20 = new int[] { 48 + (4 * 3) + 0, 48 + (4 * 3) + 1, 48 + (4 * 3) + 2 };
                int[] Line_V_21 = new int[] { 48 + (5 * 3) + 0, 48 + (5 * 3) + 1, 48 + (5 * 3) + 2 };
                int[] Line_V_22 = new int[] { 48 + (6 * 3) + 0, 48 + (6 * 3) + 1, 48 + (6 * 3) + 2 };
                int[] Line_V_23 = new int[] { 48 + (7 * 3) + 0, 48 + (7 * 3) + 1, 48 + (7 * 3) + 2 };

                int[] Line_V_24 = new int[] { 47 - (0 * 3) - 0, 47 - (0 * 3) - 1, 47 - (0 * 3) - 2 };
                int[] Line_V_25 = new int[] { 47 - (1 * 3) - 0, 47 - (1 * 3) - 1, 47 - (1 * 3) - 2 };
                int[] Line_V_26 = new int[] { 47 - (2 * 3) - 0, 47 - (2 * 3) - 1, 47 - (2 * 3) - 2 };
                int[] Line_V_27 = new int[] { 47 - (3 * 3) - 0, 47 - (3 * 3) - 1, 47 - (3 * 3) - 2 };
                int[] Line_V_28 = new int[] { 47 - (4 * 3) - 0, 47 - (4 * 3) - 1, 47 - (4 * 3) - 2 };
                int[] Line_V_29 = new int[] { 47 - (5 * 3) - 0, 47 - (5 * 3) - 1, 47 - (5 * 3) - 2 };
                int[] Line_V_30 = new int[] { 47 - (6 * 3) - 0, 47 - (6 * 3) - 1, 47 - (6 * 3) - 2 };
                int[] Line_V_31 = new int[] { 47 - (7 * 3) - 0, 47 - (7 * 3) - 1, 47 - (7 * 3) - 2 };

                int[] Line_V_32 = new int[] { 00 + (0 * 3) + 0, 00 + (0 * 3) + 1, 00 + (0 * 3) + 2 };
                int[] Line_V_33 = new int[] { 00 + (1 * 3) + 0, 00 + (1 * 3) + 1, 00 + (1 * 3) + 2 };
                int[] Line_V_34 = new int[] { 00 + (2 * 3) + 0, 00 + (2 * 3) + 1, 00 + (2 * 3) + 2 };
                int[] Line_V_35 = new int[] { 00 + (3 * 3) + 0, 00 + (3 * 3) + 1, 00 + (3 * 3) + 2 };
                int[] Line_V_36 = new int[] { 00 + (4 * 3) + 0, 00 + (4 * 3) + 1, 00 + (4 * 3) + 2 };
                int[] Line_V_37 = new int[] { 00 + (5 * 3) + 0, 00 + (5 * 3) + 1, 00 + (5 * 3) + 2 };
                int[] Line_V_38 = new int[] { 00 + (6 * 3) + 0, 00 + (6 * 3) + 1, 00 + (6 * 3) + 2 };
                int[] Line_V_39 = new int[] { 00 + (7 * 3) + 0, 00 + (7 * 3) + 1, 00 + (7 * 3) + 2 };

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
            if (enum_DrawerType == Drawer.Enum_DrawerType._4X8_A || enum_DrawerType == Drawer.Enum_DrawerType._5X8_A)
            {

                int index = 0;

                index = 95;
                for (int k = 0; k < 8; k++)
                {
                    List<int> values = new List<int>();
                    for (int i = 0; i < 2; i++)
                    {
                        values.Add(index - ((k * 2) + i));
                    }
                    List_Drawer_V_Line_Leds.Add(values.ToArray());
                }
                index = 64;
                for (int k = 0; k < 8; k++)
                {
                    List<int> values = new List<int>();
                    for (int i = 0; i < 2; i++)
                    {
                        values.Add(index + ((k * 2) + i));
                    }
                    List_Drawer_V_Line_Leds.Add(values.ToArray());
                }

                index = 63;
                for (int k = 0; k < 8; k++)
                {
                    List<int> values = new List<int>();
                    for (int i = 0; i < 2; i++)
                    {
                        values.Add(index - ((k * 2) + i));
                    }
                    List_Drawer_V_Line_Leds.Add(values.ToArray());
                }
                index = 32;
                for (int k = 0; k < 8; k++)
                {
                    List<int> values = new List<int>();
                    for (int i = 0; i < 2; i++)
                    {
                        values.Add(index + ((k * 2) + i));
                    }
                    List_Drawer_V_Line_Leds.Add(values.ToArray());
                }

                index = 31;
                for (int k = 0; k < 8; k++)
                {
                    List<int> values = new List<int>();
                    for (int i = 0; i < 2; i++)
                    {
                        values.Add(index - ((k * 2) + i));
                    }
                    List_Drawer_V_Line_Leds.Add(values.ToArray());
                }
                index = 0;
                for (int k = 0; k < 8; k++)
                {
                    List<int> values = new List<int>();
                    for (int i = 0; i < 2; i++)
                    {
                        values.Add(index + ((k * 2) + i));
                    }
                    List_Drawer_V_Line_Leds.Add(values.ToArray());
                }
            }
            return List_Drawer_V_Line_Leds;
        }

     
        #endregion

        private enum ContextMenuStrip_Main
        {
            畫面設置,
            IO設定,
            參數設定,       
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
        private enum ContextMenuStrip_DeviceTable_參數設定
        {
            設為大抽屜4X8,
            設為小抽屜3X8,
            設為大抽屜4X8_A,
            設為小抽屜5X8_A,
            設為EPD583有鎖控,
            設為EPD583無鎖控,
            設為EPD730有鎖控,
            設為EPD730無鎖控,
            設為隔板亮燈,
            設為把手亮燈,
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
        private enum ContextMenuStrip_UDP_DataReceive_參數設定
        {
            設為大抽屜4X8,
            設為小抽屜3X8,
            設為大抽屜4X8_A,
            設為小抽屜5X8_A,
            設為EPD583有鎖控,
            設為EPD583無鎖控,
            設為EPD730有鎖控,
            設為EPD730無鎖控,
            設為隔板亮燈,
            設為把手亮燈,
        }

        public DrawerUI_EPD_583()
        {
            this.TableName = "EPD583_Jsonstring";


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
            return Set_Pannel_LED_UDP(drawer.IP, drawer.Port, drawer.LED_Bytes, color,drawer.DrawerType);
        }
        public bool Set_Pannel_LED_UDP(string IP, int Port, byte[] LED_Bytes, Color color)
        {
            return Set_Pannel_LED_UDP(IP, Port, LED_Bytes, color, Drawer.Enum_DrawerType._4X8);
        }
        public bool Set_Pannel_LED_UDP(string IP, int Port, byte[] LED_Bytes, Color color, Drawer.Enum_DrawerType enum_DrawerType)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
            return Set_Pannel_LED_UDP(uDP_Class, IP, LED_Bytes, color, enum_DrawerType);
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
        static public bool Set_WS2812B_breathing(UDP_Class uDP_Class, string IP, byte WS2812B_breathing_onAddVal, byte WS2812B_breathing_offSubVal, Color color)
        {
            if (uDP_Class != null)
            {
                return Communication.Set_WS2812B_breathing(uDP_Class, IP, WS2812B_breathing_onAddVal, WS2812B_breathing_offSubVal, color);
            }
            return false;
        }
        public bool Set_WS2812B_breathing(Drawer drawer, byte WS2812B_breathing_onAddVal, byte WS2812B_breathing_offSubVal, Color color)
        {
            return Set_WS2812B_breathing(drawer.IP, drawer.Port, WS2812B_breathing_onAddVal, WS2812B_breathing_offSubVal, color);
        }
        public bool Set_WS2812B_breathing(string IP, int Port, byte WS2812B_breathing_onAddVal, byte WS2812B_breathing_offSubVal, Color color)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
            return Set_WS2812B_breathing(uDP_Class, IP, WS2812B_breathing_onAddVal, WS2812B_breathing_offSubVal, color);
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
        public bool DrawToEpd_BarCode_UDP(Drawer drawer)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(drawer.Port);
            return DrawToEpd_BarCode_UDP(uDP_Class, drawer);
        }
        public bool DrawToEpd_UDP(Drawer drawer)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(drawer.Port);
            return DrawToEpd_UDP(uDP_Class, drawer);
        }
        public bool DrawToEpd_UDP(string IP, int Port, Bitmap bitmap)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
            return DrawToEpd_UDP(uDP_Class, IP , bitmap , DeviceType.EPD583);
        }
        public bool DrawToEpd_UDP(string IP, int Port, Bitmap bitmap , DeviceType deviceType)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
            return DrawToEpd_UDP(uDP_Class, IP, bitmap , deviceType);
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
        public bool GetInput(Drawer drawer)
        {
            return GetInput(drawer.IP);
        }
        public bool GetInput(string IP)
        {
            string jsonString = GetUDPJsonString(IP);
            if (jsonString.StringIsEmpty()) return false;
            UDP_READ uDP_READ = jsonString.JsonDeserializet<UDP_READ>();
            if(uDP_READ == null) return false;
            return ((uDP_READ.Input % 2) == 1);
        }
        public bool GetOutput(Drawer drawer)
        {
            return this.GetOutput(drawer.IP);
        }
        public bool GetOutput(string IP)
        {
            string jsonString = GetUDPJsonString(IP);
            if (jsonString.StringIsEmpty()) return false;
            UDP_READ uDP_READ = jsonString.JsonDeserializet<UDP_READ>();
            if (uDP_READ == null) return false;
            return uDP_READ.Get_Output(0);
        }
        public void SetOutput(Drawer drawer, bool value)
        {
            SetOutput(drawer.IP, drawer.Port, value);
        }
        public void SetOutput(string IP, int Port, bool value)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
            if (uDP_Class == null) return;
            Communication.Set_OutputPIN(uDP_Class, IP, 1, value);
        }
        public void SetInput_dir(Drawer drawer, bool value)
        {
            SetInput_dir(drawer.IP, drawer.Port, value);
        }
        public void SetInput_dir(string IP, int Port, bool value)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
            if (uDP_Class == null) return;
            Communication.Set_Input_dir(uDP_Class, IP, 1, value);
        }
        public bool GetInput_dir(Drawer drawer)
        {
            return GetInput_dir(drawer.IP);
        }
        public bool GetInput_dir(string IP)
        {
            string jsonString = GetUDPJsonString(IP);
            if (jsonString.StringIsEmpty()) return false;
            UDP_READ uDP_READ = jsonString.JsonDeserializet<UDP_READ>();
            if (uDP_READ == null) return false;
            return uDP_READ.Get_Input_dir(0);
        }
        public void SetOutput_dir(Drawer drawer, bool value)
        {
            SetOutput_dir(drawer.IP, drawer.Port, value);
        }
        public void SetOutput_dir(string IP, int Port, bool value)
        {
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(Port);
            if (uDP_Class == null) return;
            Communication.Set_Output_dir(uDP_Class, IP, 1, value);
        }
        public bool GetOutput_dir(Drawer drawer)
        {
            return GetOutput_dir(drawer.IP);
        }
        public bool GetOutput_dir(string IP)
        {
            string jsonString = GetUDPJsonString(IP);
            if (jsonString.StringIsEmpty()) return false;
            UDP_READ uDP_READ = jsonString.JsonDeserializet<UDP_READ>();
            if (uDP_READ == null) return false;
            return uDP_READ.Get_Output_dir(0);
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
        private Bitmap DrawTestImage(string IP, int Port)
        {
            return DrawTestImage(IP, Port, DeviceType.EPD583);
        }
        private Bitmap DrawTestImage(string IP , int Port , DeviceType deviceType)
        {
            Bitmap bitmap = null;
            if (deviceType == DeviceType.EPD583 || deviceType == DeviceType.EPD583_lock)
            {
                int width = DrawerUI_EPD_583.Pannel_Width;
                int height = DrawerUI_EPD_583.Pannel_Height;
                bitmap = new Bitmap(DrawerUI_EPD_583.Pannel_Width, DrawerUI_EPD_583.Pannel_Height);
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
            }
            if (deviceType == DeviceType.EPD730 || deviceType == DeviceType.EPD730_lock)
            {
                int width = 800;
                int height = 480;
                bitmap = new Bitmap(width, height);
                Graphics g = Graphics.FromImage(bitmap);
                g.SmoothingMode = SmoothingMode.HighQuality; //使繪圖質量最高，即消除鋸齒
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
                g.FillRectangle(new SolidBrush(Color.Red), new RectangleF((width / 4) * 0, (height / 3) * 0, (width), height / 3));
                g.FillRectangle(new SolidBrush(Color.Black), new RectangleF((width / 4) * 0, (height / 3) * 1, (width), height / 3));
                g.FillRectangle(new SolidBrush(Color.White), new RectangleF((width / 4) * 0, (height / 3) * 2, (width), height / 3));

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
            }
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
            else if (selectedText == ContextMenuStrip_Main.參數設定.GetEnumName())
            {
                Dialog_ContextMenuStrip dialog_ContextMenuStrip = new Dialog_ContextMenuStrip(new ContextMenuStrip_UDP_DataReceive_參數設定().GetEnumNames());
                if (dialog_ContextMenuStrip.ShowDialog() == DialogResult.Yes)
                {
                    if (dialog_ContextMenuStrip.Value == ContextMenuStrip_UDP_DataReceive_參數設定.設為大抽屜4X8.GetEnumName())
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
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_UDP_DataReceive_參數設定.設為小抽屜3X8.GetEnumName())
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
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_UDP_DataReceive_參數設定.設為大抽屜4X8_A.GetEnumName())
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
                                drawer.SetDrawerType(Drawer.Enum_DrawerType._4X8_A);
                                taskList.Add(Task.Run(() =>
                                {
                                    SQL_ReplaceDrawer(drawer);
                                }));
                            }
                            Task allTask = Task.WhenAll(taskList);
                        }
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_UDP_DataReceive_參數設定.設為小抽屜5X8_A.GetEnumName())
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
                                drawer.SetDrawerType(Drawer.Enum_DrawerType._5X8_A);
                                taskList.Add(Task.Run(() =>
                                {
                                    SQL_ReplaceDrawer(drawer);
                                }));
                            }
                            Task allTask = Task.WhenAll(taskList);
                        }
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_UDP_DataReceive_參數設定.設為EPD583有鎖控.GetEnumName())
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
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_UDP_DataReceive_參數設定.設為EPD583無鎖控.GetEnumName())
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
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_UDP_DataReceive_參數設定.設為隔板亮燈.GetEnumName())
                    {
                        List<Task> taskList = new List<Task>();
                        for (int i = 0; i < iPEndPoints.Count; i++)
                        {
                            string IP = iPEndPoints[i].Address.ToString();
                            int Port = iPEndPoints[i].Port;
                            Drawer drawer = this.SQL_GetDrawer(IP);
                            if (drawer == null) continue;
                            drawer.IsAllLight = true;
                            taskList.Add(Task.Run(() =>
                            {
                                SQL_ReplaceDrawer(drawer);
                            }));
                        }
                        Task allTask = Task.WhenAll(taskList);
                        this.sqL_DataGridView_DeviceTable.SQL_GetAllRows(true);
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_UDP_DataReceive_參數設定.設為把手亮燈.GetEnumName())
                    {
                        List<Task> taskList = new List<Task>();
                        for (int i = 0; i < iPEndPoints.Count; i++)
                        {
                            string IP = iPEndPoints[i].Address.ToString();
                            int Port = iPEndPoints[i].Port;
                            Drawer drawer = this.SQL_GetDrawer(IP);
                            if (drawer == null) continue;
                            drawer.IsAllLight = false;
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
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable_畫面設置.測試資訊.GetEnumName())
                    {
                        List<Task> taskList = new List<Task>();
                        for (int i = 0; i < iPEndPoints.Count; i++)
                        {
                            string IP = iPEndPoints[i].Address.ToString();
                            int Port = iPEndPoints[i].Port;
                            taskList.Add(Task.Run(() =>
                            {
                                Drawer drawer = this.SQL_GetDrawer(IP);
                                if (drawer == null) return;
                                using (Bitmap bitmap = this.DrawTestImage(IP, Port, drawer.DeviceType))
                                {
                                    this.DrawToEpd_UDP(IP, Port, bitmap, drawer.DeviceType);
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
            else if (selectedText == ContextMenuStrip_Main.參數設定.GetEnumName())
            {
                Dialog_ContextMenuStrip dialog_ContextMenuStrip = new Dialog_ContextMenuStrip(new ContextMenuStrip_DeviceTable_參數設定().GetEnumNames());
                if (dialog_ContextMenuStrip.ShowDialog() == DialogResult.Yes)
                {
                    if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable_參數設定.設為大抽屜4X8.GetEnumName())
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
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable_參數設定.設為小抽屜3X8.GetEnumName())
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
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable_參數設定.設為大抽屜4X8_A.GetEnumName())
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
                                drawer.SetDrawerType(Drawer.Enum_DrawerType._4X8_A);
                                taskList.Add(Task.Run(() =>
                                {
                                    SQL_ReplaceDrawer(drawer);
                                }));
                            }
                            Task allTask = Task.WhenAll(taskList);
                        }
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable_參數設定.設為小抽屜5X8_A.GetEnumName())
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
                                drawer.SetDrawerType(Drawer.Enum_DrawerType._5X8_A);
                                taskList.Add(Task.Run(() =>
                                {
                                    SQL_ReplaceDrawer(drawer);
                                }));
                            }
                            Task allTask = Task.WhenAll(taskList);
                        }
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable_參數設定.設為EPD583有鎖控.GetEnumName())
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
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable_參數設定.設為EPD583無鎖控.GetEnumName())
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
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable_參數設定.設為EPD730有鎖控.GetEnumName())
                    {
                        List<Task> taskList = new List<Task>();
                        for (int i = 0; i < iPEndPoints.Count; i++)
                        {
                            string IP = iPEndPoints[i].Address.ToString();
                            int Port = iPEndPoints[i].Port;
                            Drawer drawer = this.SQL_GetDrawer(IP);
                            if (drawer == null) continue;
                            drawer.SetDeviceType(DeviceType.EPD730_lock);
                            taskList.Add(Task.Run(() =>
                            {
                                SQL_ReplaceDrawer(drawer);
                            }));
                        }
                        Task allTask = Task.WhenAll(taskList);
                        this.sqL_DataGridView_DeviceTable.SQL_GetAllRows(true);
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable_參數設定.設為EPD730無鎖控.GetEnumName())
                    {
                        List<Task> taskList = new List<Task>();
                        for (int i = 0; i < iPEndPoints.Count; i++)
                        {
                            string IP = iPEndPoints[i].Address.ToString();
                            int Port = iPEndPoints[i].Port;
                            Drawer drawer = this.SQL_GetDrawer(IP);
                            if (drawer == null) continue;
                            drawer.SetDeviceType(DeviceType.EPD730);
                            taskList.Add(Task.Run(() =>
                            {
                                SQL_ReplaceDrawer(drawer);
                            }));
                        }
                        Task allTask = Task.WhenAll(taskList);
                        this.sqL_DataGridView_DeviceTable.SQL_GetAllRows(true);
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable_參數設定.設為隔板亮燈.GetEnumName())
                    {
                        List<Task> taskList = new List<Task>();
                        for (int i = 0; i < iPEndPoints.Count; i++)
                        {
                            string IP = iPEndPoints[i].Address.ToString();
                            int Port = iPEndPoints[i].Port;
                            Drawer drawer = this.SQL_GetDrawer(IP);
                            if (drawer == null) continue;
                            drawer.IsAllLight = true;
                            taskList.Add(Task.Run(() =>
                            {
                                SQL_ReplaceDrawer(drawer);
                            }));
                        }
                        Task allTask = Task.WhenAll(taskList);
                        this.sqL_DataGridView_DeviceTable.SQL_GetAllRows(true);
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_DeviceTable_參數設定.設為把手亮燈.GetEnumName())
                    {
                        List<Task> taskList = new List<Task>();
                        for (int i = 0; i < iPEndPoints.Count; i++)
                        {
                            string IP = iPEndPoints[i].Address.ToString();
                            int Port = iPEndPoints[i].Port;
                            Drawer drawer = this.SQL_GetDrawer(IP);
                            if (drawer == null) continue;
                            drawer.IsAllLight = false;
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

    }
}
