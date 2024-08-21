using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
namespace H_Pannel_lib
{
    public partial class WT32_GPADC : UserControl
    {
        static public int Pannel_Width
        {
            get
            {
                return 480;
            }
        }
        static public int Pannel_Height
        {
            get
            {
                return 320;
            }
        }

        private bool pannel_Green_Visible = true;
        [ReadOnly(false), Browsable(true), Category("自訂屬性"), Description(""), DefaultValue("")]
        public bool Pannel_Green_Visible
        {
            get
            {
                return pannel_Green_Visible;
            }
            set
            {
                pannel_Green_Visible = value;
                if(this.IsHandleCreated)
                {
                    this.Invoke(new Action(delegate
                    {
                        this.panel_Green.Visible = value;
                        this.Invalidate();
                    }));                   
                }         
            }
        }

        private bool pannel_Red_Visible = true;
        [ReadOnly(false), Browsable(true), Category("自訂屬性"), Description(""), DefaultValue("")]
        public bool Pannel_Red_Visible
        {
            get
            {
                return pannel_Red_Visible;
            }
            set
            {
                pannel_Red_Visible = value;
                if (this.IsHandleCreated)
                {
                    this.Invoke(new Action(delegate
                    {
                        this.panel_Red.Visible = value;
                        this.Invalidate();
                    }));
                }
            }
        }

        private bool pannel_Lock_Visible = true;
        [ReadOnly(false), Browsable(true), Category("自訂屬性"), Description(""), DefaultValue("")]
        public bool Pannel_Lock_Visible 
        {
            get
            {
                return pannel_Lock_Visible;
            }
            set
            {
                pannel_Lock_Visible = value;
                if (this.IsHandleCreated)
                {
                    this.Invoke(new Action(delegate
                    {
                        this.panel_LOCK.Visible = value;
                        this.Invalidate();
                    }));
                }
            }
        }


  
        
        private List<UDP_Class> List_UDP_Local;
        private int CanvasScale = 2;
        private StorageUI_WT32.UDP_READ uDP_READ;
        private Storage currentStorage;
        public Storage CurrentStorage
        {
            get => currentStorage;
            private set
            {
                if(value != null)
                {
                    this.Invoke(new Action(delegate
                    {
                        panel_圖形編輯_面板背景顏色.BackColor = value.BackColor;
                    }));
          
                    currentStorage = value;
                }
    
            }
        }
        private Storage currentStorage_buf;
        private int mainSelect = -1; 
        private int MainSelect
        {
            get
            {
                return mainSelect;
            }
            set
            {
                mainSelect = value;
                this.Invoke(new Action(delegate
                {
                    panel_圖形編輯_面板背景顏色.BackColor = CurrentStorage.BackColor;
                    this.comboBox_圖形編輯_編輯內容名稱.SelectedIndex = mainSelect;
                    Storage.VlaueClass vlaueClass = CurrentStorage.GetValue((Storage.ValueName)mainSelect);
                    if (vlaueClass.Value != "None")
                    {
                        this.rJ_TextBox_圖形編輯_內容.Text = vlaueClass.Value;
                        this.panel_圖形編輯_邊框顏色.BackColor = vlaueClass.BorderColor;
                        this.comboBox_圖形編輯_邊框大小.Text = vlaueClass.BorderSize.ToString();
                        this.panel_圖形編輯_字體顏色.BackColor = vlaueClass.ForeColor;
                        this.panel_圖形編輯_背景顏色.BackColor = vlaueClass.BackColor;
                        this.comboBox_圖形編輯_對齊方式.Text = vlaueClass.HorizontalAlignment.GetEnumName();
                        this.rJ_TextBox_圖形編輯_字體.Text = vlaueClass.Font.ToString();
                        this.rJ_TextBox_圖形編輯_X位置.Text = vlaueClass.Position.X.ToString();
                        this.rJ_TextBox_圖形編輯_Y位置.Text = vlaueClass.Position.Y.ToString();
                        this.fontDialog.Font = vlaueClass.Font;
                        this.rJ_TextBox_圖形編輯_顯示.Checked = vlaueClass.Visable;
                    }
                }));
            }
        }


        private List<int> List_ValueSelected = new List<int>();
        private Storage.VlaueClass vlaueClass_Copy = null;
        public enum TxMouseDownType
        {
            NONE,
            TOP,
            BOTTOM,
            LEFT,
            RIGHT,
            INSIDE,
        }
        private int pictureBox_mouseDown_X = 0;
        private int pictureBox_mouseDown_Y = 0;
        private int pictureBox_mouseDown_position_X = 0;
        private int pictureBox_mouseDown_position_Y = 0;
        private int pictureBox_mouseDown_Width = 0;
        private int pictureBox_mouseDown_Height = 0;
        private bool flag_pictureBox_mouseDown = false;
        private TxMouseDownType mouseDownType;
        public TxMouseDownType MouseDownType
        {
            get
            {
                return mouseDownType;
            }
            set
            {
                mouseDownType = value;
                switch(value)
                {
                    case TxMouseDownType.NONE:
                        this.Cursor = Cursors.Default;
                        break;
                    case TxMouseDownType.INSIDE:
                        this.Cursor = Cursors.NoMove2D;
                        break;
                    case TxMouseDownType.LEFT:
                        this.Cursor = Cursors.SizeWE;
                        break;
                    case TxMouseDownType.RIGHT:
                        this.Cursor = Cursors.SizeWE;
                        break;
                    case TxMouseDownType.TOP:
                        this.Cursor = Cursors.SizeNS;
                        break;
                    case TxMouseDownType.BOTTOM:
                        this.Cursor = Cursors.SizeNS;
                        break;
                }
                    
        

            }
        }

       

        public WT32_GPADC()
        {
            InitializeComponent();
        }
        public void Init(List<UDP_Class> List_UDP_Local)
        {
            this.List_UDP_Local = List_UDP_Local;
            this.panel_Green.Visible = Pannel_Green_Visible;
            this.panel_Red.Visible = Pannel_Red_Visible;
            this.panel_LOCK.Visible = Pannel_Lock_Visible;

            Communication.ConsoleWrite = true;
    
            uDP_READ = new StorageUI_WT32.UDP_READ();
            uDP_READ.LEDStateChangeEvent += UDP_READ_LEDStateChangeEvent;
            uDP_READ.LockStateChangeEvent += UDP_READ_LockStateChangeEvent;
            this.pictureBox.Paint += PictureBox_Paint;
         
            comboBox_圖形編輯_編輯內容名稱.DataSource = new Storage.ValueName().GetEnumNames();
            comboBox_圖形編輯_編輯內容名稱.SelectedIndex = 0;
            comboBox_圖形編輯_對齊方式.DataSource = new HorizontalAlignment().GetEnumNames();

            this.rJ_Button_複製格式_大小.MouseDownEvent += RJ_Button_複製格式_大小_MouseDownEvent;
            this.rJ_Button_複製格式_寬度.MouseDownEvent += RJ_Button_複製格式_寬度_MouseDownEvent;
            this.rJ_Button_複製格式_高度.MouseDownEvent += RJ_Button_複製格式_高度_MouseDownEvent;
            this.rJ_Button_複製格式_靠左對齊.MouseDownEvent += RJ_Button_複製格式_靠左對齊_MouseDownEvent;
            this.rJ_Button_複製格式_靠右對齊.MouseDownEvent += RJ_Button_複製格式_靠右對齊_MouseDownEvent;
            this.rJ_Button_複製格式_靠上對齊.MouseDownEvent += RJ_Button_複製格式_靠上對齊_MouseDownEvent;
            this.rJ_Button_複製格式_靠下對齊.MouseDownEvent += RJ_Button_複製格式_靠下對齊_MouseDownEvent;
            this.rJ_Button_複製格式_字體背景顏色.MouseDownEvent += RJ_Button_複製格式_字體背景顏色_MouseDownEvent;
            this.rJ_Button_複製格式_邊框顏色.MouseDownEvent += RJ_Button_複製格式_邊框顏色_MouseDownEvent;
            this.rJ_Button_複製格式_邊框大小.MouseDownEvent += RJ_Button_複製格式_邊框大小_MouseDownEvent;
            this.rJ_Button_複製格式_文字對齊方式.MouseDownEvent += RJ_Button_複製格式_文字對齊方式_MouseDownEvent;
            this.rJ_Button_複製格式_字體.MouseDownEvent += RJ_Button_複製格式_字體_MouseDownEvent;
            this.rJ_Button_複製格式_字體顏色.MouseDownEvent += RJ_Button_複製格式_字體顏色_MouseDownEvent;

            this.rJ_Button_複製格式_垂直間距平均分配.MouseDownEvent += RJ_Button_複製格式_垂直間距平均分配_MouseDownEvent;
            this.rJ_Button_複製格式_水平間距平均分配.MouseDownEvent += RJ_Button_複製格式_水平間距平均分配_MouseDownEvent;
            this.rJ_Button_複製格式_垂直間距等距分配.MouseDownEvent += RJ_Button_複製格式_垂直間距等距分配_MouseDownEvent;
            this.rJ_Button_複製格式_水平間距等距分配.MouseDownEvent += RJ_Button_複製格式_水平間距等距分配_MouseDownEvent;
        }

 

        new public void Dispose()
        {
            this.Dispose(true);
        }

        private void UDP_READ_LockStateChangeEvent(bool statu)
        {
            this.Invoke(new System.Action(delegate 
            {
                if (statu)
                {
                    panel_LOCK.BackColor = Color.Lime;
                    panel_LOCK.BackgroundImage = global::H_Pannel_lib.Properties.Resources.LOCK;
                }
                else
                {
                    panel_LOCK.BackColor = Color.Red;
                    panel_LOCK.BackgroundImage = global::H_Pannel_lib.Properties.Resources.UNLOCK;
                }
            }));
        }
        private void UDP_READ_LEDStateChangeEvent(StorageUI_WT32.UDP_READ.LED_Type lED_Type, bool statu)
        {
            this.Invoke(new System.Action(delegate 
            {
                if (lED_Type == StorageUI_WT32.UDP_READ.LED_Type.GREEN)
                {
                    if (statu)
                    {
                        panel_Green.BackgroundImage = global::H_Pannel_lib.Properties.Resources.green_lamp_on_;
                    }
                    else
                    {
                        panel_Green.BackgroundImage = global::H_Pannel_lib.Properties.Resources.green_lamp_off_;
                    }
                }
                else if (lED_Type == StorageUI_WT32.UDP_READ.LED_Type.RED)
                {
                    if (statu)
                    {
                        panel_Red.BackgroundImage = global::H_Pannel_lib.Properties.Resources.red_lamp_on_;
                    }
                    else
                    {
                        panel_Red.BackgroundImage = global::H_Pannel_lib.Properties.Resources.red_lamp_off_;
                    }
                }
            }));
        
        }
        public bool Set_LED(string IP, int Port ,StorageUI_WT32.UDP_READ.LED_Type lED_Type , bool statu)
        {
            bool flag = false;
            //UDP_Class uDP_Class = new UDP_Class(IP, Port);
            //H_Pannel_lib.Communication Communication = new H_Pannel_lib.Communication();
            //flag =  Communication.Set_PIN(uDP_Class, IP, (int)lED_Type, statu);
            //uDP_Class.Dispose();

            H_Pannel_lib.Communication Communication = new H_Pannel_lib.Communication();
            for (int i = 0; i < List_UDP_Local.Count; i++)
            {
                if(List_UDP_Local[i].Port == Port)
                {
                    flag = Communication.Set_PIN(List_UDP_Local[i], IP, (int)lED_Type, statu);
                }
           
            }
           
            return flag;
        }

        public bool Set_JsonStringSend(Storage storage)
        {
            return this.Set_JsonStringSend(storage.IP, storage.Port);
        }
        public bool Set_JsonStringSend(string IP, int Port)
        {
            bool flag = false;
            for (int i = 0; i < List_UDP_Local.Count; i++)
            {
                if (List_UDP_Local[i].Port == Port)
                {
                    flag = Communication.Set_JsonStringSend(List_UDP_Local[i], IP);
                }

            }
            return flag;

        }
        public bool Set_ScreenPageInit(Storage storage, bool enable)
        {
            return this.Set_ScreenPageInit(storage.IP, storage.Port, enable);
        }

        public bool Set_ScreenPageInit(string IP, int Port, bool enable)
        {

            bool flag = false;
            for (int i = 0; i < List_UDP_Local.Count; i++)
            {
                if (List_UDP_Local[i].Port == Port)
                {
                    flag = Communication.Set_ScreenPageInit(List_UDP_Local[i], IP, enable);
                }

            }
            return flag;
        }
        public bool Set_SendTestData(string jsonString)
        {
            StorageUI_WT32.UDP_READ _uDP_READ = this.Get_JSON_String_Class(jsonString);
            if (_uDP_READ != null)
            {
                return this.Set_SendTestData(_uDP_READ.IP, _uDP_READ.Port);
            }
            return false;
        }
        public bool Set_SendTestData(string IP, int Port)
        {

            bool flag = false;
            for (int i = 0; i < List_UDP_Local.Count; i++)
            {
                if (List_UDP_Local[i].Port == Port)
                {
                    flag = Communication.Set_SendTestData(List_UDP_Local[i], IP , Port);
                }

            }
            return flag;
        }
        public bool Set_UDP_SendTime(string IP, int Port ,int ms)
        {

            bool flag = false;
            for (int i = 0; i < List_UDP_Local.Count; i++)
            {
                if (List_UDP_Local[i].Port == Port)
                {
                    flag = Communication.Set_UDP_SendTime(List_UDP_Local[i], IP ,ms);
                }

            }
            return flag;
        }
        public bool Set_ESP32_Restart(string IP , int Port)
        {
            bool flag = false;
            for (int i = 0; i < List_UDP_Local.Count; i++)
            {
                if (List_UDP_Local[i].Port == Port)
                {
                    flag = Communication.Set_ESP32_Restart(List_UDP_Local[i], IP);
                }

            }
            return flag;
     
        }
        public bool Set_OTAUpdate(string IP, int Port)
        {
            bool flag = false;
            for (int i = 0; i < List_UDP_Local.Count; i++)
            {
                if (List_UDP_Local[i].Port == Port)
                {
                    flag = Communication.Set_OTAUpdate(List_UDP_Local[i], IP);
                }

            }
            return flag;


        }
        public bool Set_Setting_Page(string IP, int Port)
        {
            bool flag = false;
            for (int i = 0; i < List_UDP_Local.Count; i++)
            {
                if (List_UDP_Local[i].Port == Port)
                {
                    flag = Communication.Set_Setting_Page(List_UDP_Local[i], IP);
                }

            }
            return flag;
        } 
        public bool Set_ToPage(string IP, int Port, int num)
        {
            bool flag = false;
            for (int i = 0; i < List_UDP_Local.Count; i++)
            {
                if (List_UDP_Local[i].Port == Port)
                {
                    flag = Communication.Set_ToPage(List_UDP_Local[i], IP, num);
                }

            }
            return flag;
        }
        public bool Set_ServerConfig(string IP, int Port ,string ServerIP,int ServerPort)
        {
            bool flag = false;
            for (int i = 0; i < List_UDP_Local.Count; i++)
            {
                if (List_UDP_Local[i].Port == Port)
                {
                    flag = Communication.Set_ServerConfig(List_UDP_Local[i], IP, ServerIP, ServerPort);
                }

            }
            return flag;
        }
        public bool Set_Main_Page(string jsonString)
        {
            StorageUI_WT32.UDP_READ _uDP_READ = this.Get_JSON_String_Class(jsonString);
            if (_uDP_READ != null)
            {
                return this.Set_Main_Page(_uDP_READ.IP, _uDP_READ.Port);
            }
            return false;
        }
        public bool Set_Main_Page(Storage storage)
        {
            return this.Set_Main_Page(storage.IP, storage.Port);
        }
        public bool Set_Main_Page(string IP, int Port)
        {

            bool flag = false;
            for (int i = 0; i < List_UDP_Local.Count; i++)
            {
                if (List_UDP_Local[i].Port == Port)
                {
                    flag = Communication.Set_Main_Page(List_UDP_Local[i], IP);
                }

            }
            return flag;
        }
        public bool Set_ClearCanvas(string jsonString, Color color)
        {
            StorageUI_WT32.UDP_READ _uDP_READ = this.Get_JSON_String_Class(jsonString);
            if (_uDP_READ != null)
            {
                return this.Set_ClearCanvas(_uDP_READ.IP, _uDP_READ.Port, color);
            }
            return false;
        }
        public bool Set_ClearCanvas(string IP, int Port ,Color color)
        {

            bool flag = false;
            for (int i = 0; i < List_UDP_Local.Count; i++)
            {
                if (List_UDP_Local[i].Port == Port)
                {
                    flag = Communication.Set_ClearCanvas(List_UDP_Local[i], IP, 0,0, Pannel_Width, Pannel_Height, color);
                }

            }     
            return flag;
        }   
        public bool Set_TextEx(string IP, int Port, string Text, int x, int y, Font font, Color FontColor, Color FontBackColor)
        {
            Size string_size = TextRenderer.MeasureText(Text, font);
            return this.Set_TextEx(IP, Port, Text, x, y, string_size.Width, string_size.Height, font, FontColor, FontBackColor, 0, Color.Black, HorizontalAlignment.Left);
        }
        public bool Set_TextEx(string IP, int Port, string Text, int x, int y, Font font, Color FontColor, Color FontBackColor, int BorderSize, Color BorderColor)
        {
            Size string_size = TextRenderer.MeasureText(Text, font);
            return this.Set_TextEx(IP, Port, Text, x, y, string_size.Width, string_size.Height, font, FontColor, FontBackColor, BorderSize, BorderColor, HorizontalAlignment.Left);
        }
        public bool Set_TextEx(string IP, int Port, string Text, int x, int y, Font font, Color FontColor, Color FontBackColor, int BorderSize, Color BorderColor, HorizontalAlignment horizontalAlignment)
        {
            Size string_size = TextRenderer.MeasureText(Text, font);
            return this.Set_TextEx(IP, Port, Text, x, y, string_size.Width, string_size.Height ,font, FontColor, FontBackColor, BorderSize, BorderColor, horizontalAlignment);
        }
        public bool Set_TextEx(string IP, int Port, string Text, int x, int y, int width, int height ,Font font, Color FontColor, Color FontBackColor ,int BorderSize , Color BorderColor, HorizontalAlignment horizontalAlignment)
        {
            bool flag = false;
            for (int i = 0; i < List_UDP_Local.Count; i++)
            {
                if (List_UDP_Local[i].Port == Port)
                {
                    flag = Communication.Set_TextEx(List_UDP_Local[i], IP, Text, x, y, width, height, font, FontColor, FontBackColor, horizontalAlignment);
                    if (!flag) return false;
                    if(BorderSize > 0)
                    {
                        flag = this.Set_DrawRect(IP, Port, x, y, width, height, BorderSize, BorderColor);
                    }
                }

            }
            return flag;
        }
        public bool Set_BarCodeEx(string jsonString, string content, int x, int y, int width, int height)
        {
            StorageUI_WT32.UDP_READ _uDP_READ = this.Get_JSON_String_Class(jsonString);
            if (_uDP_READ != null)
            {
                return this.Set_BarCodeEx(_uDP_READ.IP, _uDP_READ.Port, content, x, y, width, height);
            }
            return false;
        }
        public bool Set_BarCodeEx(string IP, int Port, string content, int x, int y, int width, int height)
        {

            bool flag = false;
            for (int i = 0; i < List_UDP_Local.Count; i++)
            {
                if (List_UDP_Local[i].Port == Port)
                {
                    flag = Communication.Set_BarCodeEx(List_UDP_Local[i], IP, content, x, y, width, height);
                }

            }
            return flag;
        }
        public bool Set_DrawRect(string jsonString, int x, int y, int width, int height, int pen_width, Color color)
        {
            StorageUI_WT32.UDP_READ _uDP_READ = this.Get_JSON_String_Class(jsonString);
            if (_uDP_READ != null)
            {
                return this.Set_DrawRect(_uDP_READ.IP, _uDP_READ.Port, x, y, width, height, pen_width, color);
            }
            return false;
        }
        public bool Set_DrawRect(string IP, int Port, int x, int y, int width, int height, int pen_width, Color color)
        {
            bool flag = false;
            for (int i = 0; i < List_UDP_Local.Count; i++)
            {
                if (List_UDP_Local[i].Port == Port)
                {
                    flag = Communication.Set_DrawRect(List_UDP_Local[i], IP, x, y, width, height, pen_width, color);
                }

            }
            return flag;
        }

        public List<StorageUI_WT32.UDP_READ> Get_JSON_String_Class(List<string> jsonString)
        {
            List<StorageUI_WT32.UDP_READ> list_UDP_READ = new List<StorageUI_WT32.UDP_READ>();
            for (int i = 0; i < jsonString.Count; i++)
            {
                list_UDP_READ.Add(this.Get_JSON_String_Class(jsonString[i]));
            }           
            return list_UDP_READ;
        }
        public StorageUI_WT32.UDP_READ Get_JSON_String_Class(string jsonString)
        {
            StorageUI_WT32.UDP_READ _uDP_READ = null;
            try
            {
                _uDP_READ = JsonSerializer.Deserialize<StorageUI_WT32.UDP_READ>(jsonString);//反序列化jsonString

            }
            catch
            {
                return null;
            }
            return _uDP_READ;
        }
        public void Set_JSON_String(string jsonString)
        {
            StorageUI_WT32.UDP_READ _uDP_READ = this.Get_JSON_String_Class(jsonString);
            if (_uDP_READ != null)
            {
              uDP_READ.IP = _uDP_READ.IP;
              uDP_READ.Port = _uDP_READ.Port;
              uDP_READ.Command = _uDP_READ.Command;
              uDP_READ.Touch_xPos = _uDP_READ.Touch_xPos;
              uDP_READ.Touch_yPos = _uDP_READ.Touch_yPos;
              uDP_READ.Touch_touched = _uDP_READ.Touch_touched;
              uDP_READ.INPUT_LOCK = _uDP_READ.INPUT_LOCK;
              uDP_READ.OUTPUT_LED_RED = _uDP_READ.OUTPUT_LED_RED;
              uDP_READ.OUTPUT_LED_GREEN = _uDP_READ.OUTPUT_LED_GREEN;
            }
           
        }
        public bool Set_DrawPannel()
        {
            return this.Set_DrawPannel(this.CurrentStorage);
        }
        public bool Set_DrawPannel(Storage storage, Storage.ValueName valueName)
        {
            Storage.VlaueClass vlaueClass = storage.GetValue(valueName);
            return this.Set_DrawPannel(storage.IP, storage.Port, vlaueClass);
        }
        public bool Set_DrawPannel(string IP, int Port, Storage.VlaueClass vlaueClass)
        {
            if (vlaueClass.Visable)
            {
                if (vlaueClass.valueName != Storage.ValueName.BarCode)
                {
                    this.Set_TextEx(IP, Port, vlaueClass.StringValue, vlaueClass.Position.X, vlaueClass.Position.Y, vlaueClass.Width, vlaueClass.Height, vlaueClass.Font, vlaueClass.ForeColor, vlaueClass.BackColor, vlaueClass.BorderSize, vlaueClass.BorderColor, vlaueClass.HorizontalAlignment);
                }
                else
                {
                    this.Set_BarCodeEx(IP, Port, vlaueClass.Value, vlaueClass.Position.X, vlaueClass.Position.Y, vlaueClass.Width, vlaueClass.Height);
                }
            }
            return true;
        }
        public bool Set_DrawPannel(Storage storage)
        {
            Storage.VlaueClass vlaueClass;
            this.Set_ClearCanvas(storage.IP, storage.Port, storage.BackColor);
            for (int i = 0; i < new Storage.ValueName().GetEnumNames().Length; i++)
            {
                vlaueClass = storage.GetValue((Storage.ValueName)i);
                if(vlaueClass.Visable)
                {
                    if (vlaueClass.valueName != Storage.ValueName.BarCode)
                    {
                        this.Set_TextEx(storage.IP, storage.Port, vlaueClass.StringValue, vlaueClass.Position.X, vlaueClass.Position.Y, vlaueClass.Width, vlaueClass.Height, vlaueClass.Font, vlaueClass.ForeColor, vlaueClass.BackColor, vlaueClass.BorderSize, vlaueClass.BorderColor, vlaueClass.HorizontalAlignment);
                    }
                    else
                    {
                        this.Set_BarCodeEx(storage.IP, storage.Port, vlaueClass.Value, vlaueClass.Position.X, vlaueClass.Position.Y, vlaueClass.Width, vlaueClass.Height);
                    }
                }
            
            }
            this.Set_ScreenPageInit(storage.IP, storage.Port, false);
            return true;
        }

        public void Set_Stroage(Storage storage)
        {
            string Code = storage.Code;
            CurrentStorage = storage;

            Storage.VlaueClass vlaueClass;
            for (int i = 0; i < new Storage.ValueName().GetLength(); i++)
            {
                vlaueClass = CurrentStorage.GetValue((Storage.ValueName)i);
                Size size = TextRenderer.MeasureText(vlaueClass.StringValue, vlaueClass.Font);
                if (vlaueClass.Width < size.Width) vlaueClass.Width = size.Width;
                if (vlaueClass.Height < size.Height) vlaueClass.Height = size.Height;
                CurrentStorage.SetValue(vlaueClass);
                CurrentStorage.Code = Code;
            }
            this.DrawToPictureBox();
        }
        public Bitmap Get_Storage_bmp(Storage storage)
        {
            Storage.VlaueClass vlaueClass;
            Bitmap bitmap = new Bitmap(Pannel_Width * CanvasScale, Pannel_Height * CanvasScale);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.HighQuality; //使繪圖質量最高，即消除鋸齒
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
                g.FillRectangle(new SolidBrush(storage.BackColor), 0, 0, Pannel_Width * CanvasScale, Pannel_Height * CanvasScale);
                for (int i = 0; i < new Storage.ValueName().GetLength(); i++)
                {
                    vlaueClass = storage.GetValue((Storage.ValueName)i);
                    vlaueClass.Position.X = vlaueClass.Position.X * CanvasScale;
                    vlaueClass.Position.Y = vlaueClass.Position.Y * CanvasScale;
                    if (vlaueClass.Visable)
                    {
                        using (Bitmap bitmap_temp = storage.GetBitmap((Storage.ValueName)i, CanvasScale))
                        {
                            if (bitmap_temp == null) continue;
                            g.DrawImage(bitmap_temp, vlaueClass.Position);
                        }
                    }
                }

                for (int i = 0; i < List_ValueSelected.Count; i++)
                {
                    vlaueClass = storage.GetValue((Storage.ValueName)List_ValueSelected[i]);
                    vlaueClass.Position.X = vlaueClass.Position.X * CanvasScale;
                    vlaueClass.Position.Y = vlaueClass.Position.Y * CanvasScale;
                    if (vlaueClass.Visable)
                    {
                        using (Bitmap bitmap_temp = storage.GetBitmap((Storage.ValueName)List_ValueSelected[i], CanvasScale, Color.Blue, 4, (List_ValueSelected[i] != MainSelect)))
                        {
                            if (bitmap_temp == null) continue;
                            g.DrawImage(bitmap_temp, vlaueClass.Position);
                        }
                    }
                }


            }
            return bitmap;
        }
        static public Bitmap Get_Storage_bmp(Storage storage, int canvasScale)
        {
            Storage.VlaueClass vlaueClass;
            Bitmap bitmap = new Bitmap(Pannel_Width * canvasScale, Pannel_Height * canvasScale);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.HighQuality; //使繪圖質量最高，即消除鋸齒
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
                g.FillRectangle(new SolidBrush(storage.BackColor), 0, 0, Pannel_Width * canvasScale, Pannel_Height * canvasScale);
                for (int i = 0; i < new Storage.ValueName().GetLength(); i++)
                {
                    vlaueClass = storage.GetValue((Storage.ValueName)i);
                    vlaueClass.Position.X = vlaueClass.Position.X * canvasScale;
                    vlaueClass.Position.Y = vlaueClass.Position.Y * canvasScale;
                    if (vlaueClass.Visable)
                    {
                        using (Bitmap bitmap_temp = storage.GetBitmap((Storage.ValueName)i, canvasScale))
                        {
                            if (bitmap_temp == null) continue;
                            g.DrawImage(bitmap_temp, vlaueClass.Position);
                        }
                    }
                }

            }
            return bitmap;

        }
        static public byte[] Get_Storage_JepgBytes(Storage storage)
        {
            using (Bitmap bitmap = Get_Storage_bmp(storage, 1))
            {
                return ((Image)bitmap).ImageToJpegBytes();
            }
        }
        public void DrawToPictureBox()
        {
            this.DrawToPictureBox(this.currentStorage);
        }
        public void DrawToPictureBox(Storage storage)
        {
            if (storage == null) return;
            this.currentStorage = storage;

            for (int i = 0; i < new Storage.ValueName().GetLength(); i++)
            {
                Storage.VlaueClass vlaueClass = storage.GetValue((Storage.ValueName)i);
                Size size = TextRenderer.MeasureText(vlaueClass.StringValue, vlaueClass.Font);
                if (vlaueClass.Width < size.Width) vlaueClass.Width = size.Width;
                if (vlaueClass.Height < size.Height) vlaueClass.Height = size.Height;
                this.CurrentStorage.SetValue(vlaueClass);
            }

            using (Bitmap bitmap = this.Get_Storage_bmp(storage))
            {
                using (Graphics g = pictureBox.CreateGraphics())
                {
                    g.SmoothingMode = SmoothingMode.HighQuality; //使繪圖質量最高，即消除鋸齒
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
                    g.DrawImage(bitmap, new PointF());
                }
            }
         
        }


        #region Event
        #region 控制功能
        private void panel_Green_Click(object sender, System.EventArgs e)
        {
            if (uDP_READ.OUTPUT_LED_GREEN == 1)
            {
                this.Set_LED(uDP_READ.IP, uDP_READ.Port ,StorageUI_WT32.UDP_READ.LED_Type.GREEN, false);
            }
            else
            {
                this.Set_LED(uDP_READ.IP, uDP_READ.Port, StorageUI_WT32.UDP_READ.LED_Type.GREEN, true);
            }
        }
        private void panel_Red_Click(object sender, System.EventArgs e)
        {
            if (uDP_READ.OUTPUT_LED_RED == 1)
            {
                this.Set_LED(uDP_READ.IP, uDP_READ.Port, StorageUI_WT32.UDP_READ.LED_Type.RED, false);
            }
            else
            {
                this.Set_LED(uDP_READ.IP, uDP_READ.Port, StorageUI_WT32.UDP_READ.LED_Type.RED, true);
            }
        }

        #endregion
        #region 圖形編輯
        private void panel_圖形編輯_字體顏色_Click(object sender, EventArgs e)
        {        
            this.colorDialog.Color = panel_圖形編輯_字體顏色.BackColor;
            if (this.colorDialog.ShowDialog() == DialogResult.OK)
            {
                CurrentStorage.SetValue((Storage.ValueName)comboBox_圖形編輯_編輯內容名稱.SelectedIndex, Storage.ValueType.ForeColor, this.colorDialog.Color);
                panel_圖形編輯_字體顏色.BackColor = this.colorDialog.Color;
                this.DrawToPictureBox();
            }
        }
        private void panel_圖形編輯_背景顏色_Click(object sender, EventArgs e)
        {
            this.colorDialog.Color = panel_圖形編輯_背景顏色.BackColor;
            if (this.colorDialog.ShowDialog() == DialogResult.OK)
            {
                CurrentStorage.SetValue((Storage.ValueName)comboBox_圖形編輯_編輯內容名稱.SelectedIndex, Storage.ValueType.BackColor, this.colorDialog.Color);
                panel_圖形編輯_背景顏色.BackColor = this.colorDialog.Color;
                this.DrawToPictureBox();
            }
        }
        private void panel_圖形編輯_面板背景顏色_Click(object sender, EventArgs e)
        {
            this.colorDialog.Color = panel_圖形編輯_面板背景顏色.BackColor;
            if (this.colorDialog.ShowDialog() == DialogResult.OK)
            {
                CurrentStorage.BackColor = this.colorDialog.Color; 
                panel_圖形編輯_面板背景顏色.BackColor = this.colorDialog.Color;
                this.DrawToPictureBox();
            }
        }
        private void button_圖形編輯_字體_Click(object sender, EventArgs e)
        {
            this.fontDialog.Font = (Font)CurrentStorage.GetValue((Storage.ValueName)comboBox_圖形編輯_編輯內容名稱.SelectedIndex, Storage.ValueType.Font);
            if (this.fontDialog.ShowDialog() == DialogResult.OK)
            {
                CurrentStorage.SetValue((Storage.ValueName)comboBox_圖形編輯_編輯內容名稱.SelectedIndex, Storage.ValueType.Font, this.fontDialog.Font);
                rJ_TextBox_圖形編輯_字體.Text = this.fontDialog.Font.ToString();
                this.DrawToPictureBox();
            }
        }
        private void button_背景顏色_設為透明_Click(object sender, EventArgs e)
        {
            CurrentStorage.SetValue((Storage.ValueName)comboBox_圖形編輯_編輯內容名稱.SelectedIndex, Storage.ValueType.BackColor, this.panel_圖形編輯_面板背景顏色.BackColor);
            this.DrawToPictureBox();
        }
        private void rJ_TextBox_圖形編輯_顯示_CheckedChanged(object sender, EventArgs e)
        {
            CurrentStorage.SetValue((Storage.ValueName)comboBox_圖形編輯_編輯內容名稱.SelectedIndex, Storage.ValueType.Visable, this.rJ_TextBox_圖形編輯_顯示.Checked);
            this.DrawToPictureBox();
        }
        private void comboBox_圖形編輯_對齊方式_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(CurrentStorage != null)
            CurrentStorage.SetValue((Storage.ValueName)comboBox_圖形編輯_編輯內容名稱.SelectedIndex, Storage.ValueType.HorizontalAlignment, (HorizontalAlignment)this.comboBox_圖形編輯_對齊方式.SelectedIndex);
            this.DrawToPictureBox();
        }
        private void comboBox_圖形編輯_邊框大小_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentStorage.SetValue((Storage.ValueName)comboBox_圖形編輯_編輯內容名稱.SelectedIndex, Storage.ValueType.BorderSize, this.comboBox_圖形編輯_邊框大小.Text.StringToInt32());
            this.DrawToPictureBox();

        }
        private void panel_圖形編輯_邊框顏色_Click(object sender, EventArgs e)
        {
            this.colorDialog.Color = panel_圖形編輯_邊框顏色.BackColor;
            if (this.colorDialog.ShowDialog() == DialogResult.OK)
            {
                CurrentStorage.SetValue((Storage.ValueName)comboBox_圖形編輯_編輯內容名稱.SelectedIndex, Storage.ValueType.BorderColor, this.colorDialog.Color);
                panel_圖形編輯_邊框顏色.BackColor = this.colorDialog.Color;
                this.DrawToPictureBox();
            }
        }

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (CurrentStorage == null) return;
            int X = e.X / this.CanvasScale;
            int Y = e.Y / this.CanvasScale;
            Storage.VlaueClass vlaueClass;

            for (int i = 0; i < new Storage.ValueName().GetLength(); i++)
            {
                vlaueClass = CurrentStorage.GetValue((Storage.ValueName)i);
                this.MouseDownType = GetMouseDownType(X, Y, vlaueClass.Position.X, vlaueClass.Position.Y, vlaueClass.Width, vlaueClass.Height);
                if (this.MouseDownType != TxMouseDownType.NONE && vlaueClass.Visable)
                {
                    int indexOf = this.List_ValueSelected.IndexOf(i);
                    if (this.List_ValueSelected.IndexOf(i) == -1)
                    {
                        if (this.List_ValueSelected.Count == 0) MainSelect = i;
                        this.List_ValueSelected.Add(i);                    
                    }
                    else
                    {
                        MainSelect = i;
                    }
                    this.pictureBox_mouseDown_position_X = vlaueClass.Position.X;
                    this.pictureBox_mouseDown_position_Y = vlaueClass.Position.Y;
                    this.pictureBox_mouseDown_X = X;
                    this.pictureBox_mouseDown_Y = Y;
                    this.pictureBox_mouseDown_Width = vlaueClass.Width;
                    this.pictureBox_mouseDown_Height = vlaueClass.Height;
                    currentStorage_buf = currentStorage.DeepClone();
                    this.flag_pictureBox_mouseDown = true;
                    this.DrawToPictureBox();
                    return;

                }

            }

            this.List_ValueSelected.Clear();
            this.DrawToPictureBox();

        }
        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (CurrentStorage == null) return;
            int X = e.X / this.CanvasScale;
            int Y = e.Y / this.CanvasScale;
            Storage.VlaueClass vlaueClass;

            if (!this.flag_pictureBox_mouseDown)
            {
                vlaueClass = CurrentStorage.GetValue((Storage.ValueName)MainSelect);
                this.MouseDownType = GetMouseDownType(X, Y, vlaueClass.Position.X, vlaueClass.Position.Y, vlaueClass.Width, vlaueClass.Height);
     
            }

            if (this.flag_pictureBox_mouseDown)
            {
                if (this.MouseDownType == TxMouseDownType.INSIDE)
                {           
                    int move_X = X - this.pictureBox_mouseDown_X;
                    int move_Y = Y - this.pictureBox_mouseDown_Y;
                    for (int i = 0; i < List_ValueSelected.Count; i++)
                    {
                        vlaueClass = currentStorage_buf.GetValue((Storage.ValueName)List_ValueSelected[i]);
                        int result_po_X = move_X + vlaueClass.Position.X;
                        int result_po_Y = move_Y + vlaueClass.Position.Y;
                        if (result_po_X < 0) result_po_X = 0;
                        if (result_po_Y < 0) result_po_Y = 0;

                        CurrentStorage.SetValue((Storage.ValueName)this.List_ValueSelected[i], Storage.ValueType.Position, new Point(result_po_X, result_po_Y));
                    }
                }
                else if (this.MouseDownType == TxMouseDownType.LEFT)
                {
                    int move_X = X - this.pictureBox_mouseDown_X;
                    int move_Y = Y - this.pictureBox_mouseDown_Y;

                    for (int i = 0; i < List_ValueSelected.Count; i++)
                    {
                        vlaueClass = currentStorage_buf.GetValue((Storage.ValueName)List_ValueSelected[i]);

                        int result_po_X = vlaueClass.Position.X + move_X;
                        int result_po_Y = vlaueClass.Position.Y;
                        int result_Width = vlaueClass.Width - move_X;
                        int result_Height = vlaueClass.Height;
                        if (result_po_X < 0) result_po_X = 0;
                        if (result_po_Y < 0) result_po_Y = 0;
                        if (result_Width < 0) result_Width = 0;
                        if (result_Height < 0) result_Height = 0;
                        CurrentStorage.SetValue((Storage.ValueName)this.List_ValueSelected[i], Storage.ValueType.Position, new Point(result_po_X, result_po_Y));
                        CurrentStorage.SetValue((Storage.ValueName)this.List_ValueSelected[i], Storage.ValueType.Width, result_Width);
                        CurrentStorage.SetValue((Storage.ValueName)this.List_ValueSelected[i], Storage.ValueType.Height, result_Height);
                    }           
                }
                else if (this.MouseDownType == TxMouseDownType.RIGHT)
                {
                    int move_X = X - this.pictureBox_mouseDown_X;
                    int move_Y = Y - this.pictureBox_mouseDown_Y;

                    for (int i = 0; i < List_ValueSelected.Count; i++)
                    {
                        vlaueClass = currentStorage_buf.GetValue((Storage.ValueName)List_ValueSelected[i]);

                        int result_po_X = vlaueClass.Position.X;
                        int result_po_Y = vlaueClass.Position.Y;
                        int result_Width = vlaueClass.Width + move_X;
                        int result_Height = vlaueClass.Height;
                        if (result_po_X < 0) result_po_X = 0;
                        if (result_po_Y < 0) result_po_Y = 0;
                        if (result_Width < 0) result_Width = 0;
                        if (result_Height < 0) result_Height = 0;

                        CurrentStorage.SetValue((Storage.ValueName)this.List_ValueSelected[i], Storage.ValueType.Position, new Point(result_po_X, result_po_Y));
                        CurrentStorage.SetValue((Storage.ValueName)this.List_ValueSelected[i], Storage.ValueType.Width, result_Width);
                        CurrentStorage.SetValue((Storage.ValueName)this.List_ValueSelected[i], Storage.ValueType.Height, result_Height);
                    }
                }
                else if (this.MouseDownType == TxMouseDownType.TOP)
                {
                    int move_X = X - this.pictureBox_mouseDown_X;
                    int move_Y = Y - this.pictureBox_mouseDown_Y;

                    for (int i = 0; i < List_ValueSelected.Count; i++)
                    {
                        vlaueClass = currentStorage_buf.GetValue((Storage.ValueName)List_ValueSelected[i]);

                        int result_po_X = vlaueClass.Position.X;
                        int result_po_Y = vlaueClass.Position.Y+ move_Y;
                        int result_Width = vlaueClass.Width;
                        int result_Height = vlaueClass.Height - move_Y;
                        if (result_po_X < 0) result_po_X = 0;
                        if (result_po_Y < 0) result_po_Y = 0;
                        if (result_Width < 0) result_Width = 0;
                        if (result_Height < 0) result_Height = 0;

                        CurrentStorage.SetValue((Storage.ValueName)this.List_ValueSelected[i], Storage.ValueType.Position, new Point(result_po_X, result_po_Y));
                        CurrentStorage.SetValue((Storage.ValueName)this.List_ValueSelected[i], Storage.ValueType.Width, result_Width);
                        CurrentStorage.SetValue((Storage.ValueName)this.List_ValueSelected[i], Storage.ValueType.Height, result_Height);
                    }      
                }
                else if (this.MouseDownType == TxMouseDownType.BOTTOM)
                {
                    int move_X = X - this.pictureBox_mouseDown_X;
                    int move_Y = Y - this.pictureBox_mouseDown_Y;

                    for (int i = 0; i < List_ValueSelected.Count; i++)
                    {
                        vlaueClass = currentStorage_buf.GetValue((Storage.ValueName)List_ValueSelected[i]);

                        int result_po_X = vlaueClass.Position.X;
                        int result_po_Y = vlaueClass.Position.Y;
                        int result_Width = vlaueClass.Width;
                        int result_Height = vlaueClass.Height + move_Y;
                        if (result_po_X < 0) result_po_X = 0;
                        if (result_po_Y < 0) result_po_Y = 0;
                        if (result_Width < 0) result_Width = 0;
                        if (result_Height < 0) result_Height = 0;

                        CurrentStorage.SetValue((Storage.ValueName)this.List_ValueSelected[i], Storage.ValueType.Position, new Point(result_po_X, result_po_Y));
                        CurrentStorage.SetValue((Storage.ValueName)this.List_ValueSelected[i], Storage.ValueType.Width, result_Width);
                        CurrentStorage.SetValue((Storage.ValueName)this.List_ValueSelected[i], Storage.ValueType.Height, result_Height);
                    }
          
                }
            }
            if (flag_pictureBox_mouseDown)
            {        
                this.DrawToPictureBox();
            }
      
          
        }
        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            this.MouseDownType = TxMouseDownType.NONE;
            this.flag_pictureBox_mouseDown = false;
        }
        async private void PictureBox_Paint(object sender, PaintEventArgs e)
        {
            Task task = Task.Factory.StartNew(new Action(delegate
            {
                this.DrawToPictureBox();
            }));
            await task;
        }
        static public TxMouseDownType GetMouseDownType(int mouse_X, int mouse_Y, int X, int Y, int Width, int Height)
        {
            int start_X = X;
            int end_X = X + Width;
            int start_Y = Y;
            int end_Y = Y + Height;

            bool flag_inside_X = (mouse_X >= start_X) && (mouse_X <= end_X);
            bool flag_inside_Y = (mouse_Y >= start_Y) && (mouse_Y <= end_Y);
            bool flag_in_left_line = (mouse_X >= start_X - 5) && (mouse_X <= start_X + 5);
            bool flag_in_right_line = (mouse_X >= end_X - 5) && (mouse_X <= end_X + 5);
            bool flag_in_top_line = (mouse_Y >= start_Y - 5) && (mouse_Y <= start_Y + 5);
            bool flag_in_button_line = (mouse_Y >= end_Y - 5) && (mouse_Y <= end_Y + 5);

            if (flag_inside_X && flag_inside_Y)
            {
                return TxMouseDownType.INSIDE;
            }
            else if (flag_in_left_line && flag_inside_Y)
            {
                return TxMouseDownType.LEFT;
            }
            else if (flag_in_right_line && flag_inside_Y)
            {
                return TxMouseDownType.RIGHT;
            }
            else if (flag_in_top_line && flag_inside_X)
            {
                return TxMouseDownType.TOP;
            }
            else if (flag_in_button_line && flag_inside_X)
            {
                return TxMouseDownType.BOTTOM;
            }
            else
            {
                return TxMouseDownType.NONE;
            }
        }
        #endregion
        #region 格式

        private void RJ_Button_複製格式_高度_MouseDownEvent(MouseEventArgs mevent)
        {
            if (mainSelect == -1)
            {
                this.Invoke(new Action(delegate
                {
                    if (this.vlaueClass_Copy == null) MyMessageBox.ShowDialog("選取面板資料!");
                }));
                return;
            }
            this.vlaueClass_Copy = this.CurrentStorage.GetValue((Storage.ValueName)mainSelect);
            for (int i = 0; i < List_ValueSelected.Count; i++)
            {
                this.CurrentStorage.SetValue((Storage.ValueName)List_ValueSelected[i], Storage.ValueType.Height, vlaueClass_Copy.Height);
            }
            this.DrawToPictureBox();
        }
        private void RJ_Button_複製格式_寬度_MouseDownEvent(MouseEventArgs mevent)
        {
            if (mainSelect == -1)
            {
                this.Invoke(new Action(delegate
                {
                    if (this.vlaueClass_Copy == null) MyMessageBox.ShowDialog("選取面板資料!");
                }));
                return;
            }
            this.vlaueClass_Copy = this.CurrentStorage.GetValue((Storage.ValueName)mainSelect);
            for (int i = 0; i < List_ValueSelected.Count; i++)
            {
                this.CurrentStorage.SetValue((Storage.ValueName)List_ValueSelected[i], Storage.ValueType.Width, vlaueClass_Copy.Width);
            }
            this.DrawToPictureBox();
        }
        private void RJ_Button_複製格式_大小_MouseDownEvent(MouseEventArgs mevent)
        {
            if (mainSelect == -1)
            {
                this.Invoke(new Action(delegate
                {
                    if (this.vlaueClass_Copy == null) MyMessageBox.ShowDialog("選取面板資料!");
                }));
                return;
            }
            this.vlaueClass_Copy = this.CurrentStorage.GetValue((Storage.ValueName)mainSelect);
            for (int i = 0; i < List_ValueSelected.Count; i++)
            {
                this.CurrentStorage.SetValue((Storage.ValueName)List_ValueSelected[i], Storage.ValueType.Width, vlaueClass_Copy.Width);
                this.CurrentStorage.SetValue((Storage.ValueName)List_ValueSelected[i], Storage.ValueType.Height, vlaueClass_Copy.Height);
            }
            this.DrawToPictureBox();
        }

        private void RJ_Button_複製格式_靠下對齊_MouseDownEvent(MouseEventArgs mevent)
        {
            if (mainSelect == -1)
            {
                this.Invoke(new Action(delegate
                {
                    if (this.vlaueClass_Copy == null) MyMessageBox.ShowDialog("選取面板資料!");
                }));
                return;
            }
            this.vlaueClass_Copy = this.CurrentStorage.GetValue((Storage.ValueName)mainSelect);
            for (int i = 0; i < List_ValueSelected.Count; i++)
            {
                Point p0 = (Point)this.CurrentStorage.GetValue((Storage.ValueName)List_ValueSelected[i], Storage.ValueType.Position);
                int height = (int)this.CurrentStorage.GetValue((Storage.ValueName)List_ValueSelected[i], Storage.ValueType.Height);
                p0.Y = (vlaueClass_Copy.Position.Y + vlaueClass_Copy.Height) - height;
                this.CurrentStorage.SetValue((Storage.ValueName)List_ValueSelected[i], Storage.ValueType.Position, p0);
            }
            this.DrawToPictureBox();
        }
        private void RJ_Button_複製格式_靠上對齊_MouseDownEvent(MouseEventArgs mevent)
        {
            if (mainSelect == -1)
            {
                this.Invoke(new Action(delegate
                {
                    if (this.vlaueClass_Copy == null) MyMessageBox.ShowDialog("選取面板資料!");
                }));
                return;
            }
            this.vlaueClass_Copy = this.CurrentStorage.GetValue((Storage.ValueName)mainSelect);
            for (int i = 0; i < List_ValueSelected.Count; i++)
            {
                Point p0 = (Point)this.CurrentStorage.GetValue((Storage.ValueName)List_ValueSelected[i], Storage.ValueType.Position);
                p0.Y = vlaueClass_Copy.Position.Y;
                this.CurrentStorage.SetValue((Storage.ValueName)List_ValueSelected[i], Storage.ValueType.Position, p0);
            }
            this.DrawToPictureBox();
        }
        private void RJ_Button_複製格式_靠左對齊_MouseDownEvent(MouseEventArgs mevent)
        {
            if (mainSelect == -1)
            {
                this.Invoke(new Action(delegate
                {
                    if (this.vlaueClass_Copy == null) MyMessageBox.ShowDialog("選取面板資料!");
                }));
                return;
            }
            this.vlaueClass_Copy = this.CurrentStorage.GetValue((Storage.ValueName)mainSelect);
            for (int i = 0; i < List_ValueSelected.Count; i++)
            {
                Point p0 = (Point)this.CurrentStorage.GetValue((Storage.ValueName)List_ValueSelected[i], Storage.ValueType.Position);
                p0.X = vlaueClass_Copy.Position.X;
                this.CurrentStorage.SetValue((Storage.ValueName)List_ValueSelected[i], Storage.ValueType.Position, p0);
            }
            this.DrawToPictureBox();
        }
        private void RJ_Button_複製格式_靠右對齊_MouseDownEvent(MouseEventArgs mevent)
        {
            if (mainSelect == -1)
            {
                this.Invoke(new Action(delegate
                {
                    if (this.vlaueClass_Copy == null) MyMessageBox.ShowDialog("選取面板資料!");
                }));
                return;
            }
            this.vlaueClass_Copy = this.CurrentStorage.GetValue((Storage.ValueName)mainSelect);
            for (int i = 0; i < List_ValueSelected.Count; i++)
            {
                Point p0 = (Point)this.CurrentStorage.GetValue((Storage.ValueName)List_ValueSelected[i], Storage.ValueType.Position);
                int width = (int)this.CurrentStorage.GetValue((Storage.ValueName)List_ValueSelected[i], Storage.ValueType.Width);
                p0.X = (vlaueClass_Copy.Position.X + vlaueClass_Copy.Width) - width;
                this.CurrentStorage.SetValue((Storage.ValueName)List_ValueSelected[i], Storage.ValueType.Position, p0);
            }
            this.DrawToPictureBox();
        }
        private void RJ_Button_複製格式_字體顏色_MouseDownEvent(MouseEventArgs mevent)
        {
            if (mainSelect == -1)
            {
                this.Invoke(new Action(delegate
                {
                    if (this.vlaueClass_Copy == null) MyMessageBox.ShowDialog("選取面板資料!");
                }));
                return;
            }
            this.vlaueClass_Copy = this.CurrentStorage.GetValue((Storage.ValueName)mainSelect);
            for (int i = 0; i < List_ValueSelected.Count; i++)
            {
                this.CurrentStorage.SetValue((Storage.ValueName)List_ValueSelected[i], Storage.ValueType.ForeColor, vlaueClass_Copy.ForeColor);
            }
            this.DrawToPictureBox();
        }
        private void RJ_Button_複製格式_字體_MouseDownEvent(MouseEventArgs mevent)
        {
            if (mainSelect == -1)
            {
                this.Invoke(new Action(delegate
                {
                    if (this.vlaueClass_Copy == null) MyMessageBox.ShowDialog("選取面板資料!");
                }));
                return;
            }
            this.vlaueClass_Copy = this.CurrentStorage.GetValue((Storage.ValueName)mainSelect);
            for (int i = 0; i < List_ValueSelected.Count; i++)
            {
                this.CurrentStorage.SetValue((Storage.ValueName)List_ValueSelected[i], Storage.ValueType.Font, vlaueClass_Copy.Font);
            }
            this.DrawToPictureBox();
        }
        private void RJ_Button_複製格式_文字對齊方式_MouseDownEvent(MouseEventArgs mevent)
        {
            if (mainSelect == -1)
            {
                this.Invoke(new Action(delegate
                {
                    if (this.vlaueClass_Copy == null) MyMessageBox.ShowDialog("選取面板資料!");
                }));
                return;
            }
            this.vlaueClass_Copy = this.CurrentStorage.GetValue((Storage.ValueName)mainSelect);
            for (int i = 0; i < List_ValueSelected.Count; i++)
            {
                this.CurrentStorage.SetValue((Storage.ValueName)List_ValueSelected[i], Storage.ValueType.HorizontalAlignment, vlaueClass_Copy.HorizontalAlignment);
            }
            this.DrawToPictureBox();
        }
        private void RJ_Button_複製格式_邊框大小_MouseDownEvent(MouseEventArgs mevent)
        {
            if (mainSelect == -1)
            {
                this.Invoke(new Action(delegate
                {
                    if (this.vlaueClass_Copy == null) MyMessageBox.ShowDialog("選取面板資料!");
                }));
                return;
            }
            this.vlaueClass_Copy = this.CurrentStorage.GetValue((Storage.ValueName)mainSelect);
            for (int i = 0; i < List_ValueSelected.Count; i++)
            {
                this.CurrentStorage.SetValue((Storage.ValueName)List_ValueSelected[i], Storage.ValueType.BorderSize, vlaueClass_Copy.BorderSize);
            }
            this.DrawToPictureBox();
        }
        private void RJ_Button_複製格式_邊框顏色_MouseDownEvent(MouseEventArgs mevent)
        {
            if (mainSelect == -1)
            {
                this.Invoke(new Action(delegate
                {
                    if (this.vlaueClass_Copy == null) MyMessageBox.ShowDialog("選取面板資料!");
                }));
                return;
            }
            this.vlaueClass_Copy = this.CurrentStorage.GetValue((Storage.ValueName)mainSelect);
            for (int i = 0; i < List_ValueSelected.Count; i++)
            {
                this.CurrentStorage.SetValue((Storage.ValueName)List_ValueSelected[i], Storage.ValueType.BorderColor, vlaueClass_Copy.BorderColor);
            }
            this.DrawToPictureBox();

        }
        private void RJ_Button_複製格式_字體背景顏色_MouseDownEvent(MouseEventArgs mevent)
        {
            if (mainSelect == -1)
            {
                this.Invoke(new Action(delegate
                {
                    if (this.vlaueClass_Copy == null) MyMessageBox.ShowDialog("選取面板資料!");
                }));
                return;
            }
            this.vlaueClass_Copy = this.CurrentStorage.GetValue((Storage.ValueName)mainSelect);
            for (int i = 0; i < List_ValueSelected.Count; i++)
            {
                this.CurrentStorage.SetValue((Storage.ValueName)List_ValueSelected[i], Storage.ValueType.BackColor, vlaueClass_Copy.BackColor);
            }
            this.DrawToPictureBox();
        }
        private void RJ_Button_複製格式_水平間距平均分配_MouseDownEvent(MouseEventArgs mevent)
        {
            if (List_ValueSelected.Count <= 1)
            {
                this.Invoke(new Action(delegate
                {
                    if (this.vlaueClass_Copy == null) MyMessageBox.ShowDialog("請選取兩個以上面板資料!");
                }));
                return;
            }
            int maxValue = 0;
            int minValue = 99999;
            float average = 0;

            for (int i = 0; i < List_ValueSelected.Count; i++)
            {
                Point p0 = (Point)this.CurrentStorage.GetValue((Storage.ValueName)List_ValueSelected[i], Storage.ValueType.Position);
                if (maxValue < p0.X) maxValue = p0.X;
                if (minValue > p0.X) minValue = p0.X;
            }
            List_ValueSelected.Sort(new ICP_PostrionX(CurrentStorage));
            average = ((float)maxValue - (float)minValue) / (float) ( List_ValueSelected.Count - 1 );
            for (int i = 0; i < List_ValueSelected.Count; i++)
            {
                Point p0 = (Point)this.CurrentStorage.GetValue((Storage.ValueName)List_ValueSelected[i], Storage.ValueType.Position);
                p0.X = (int)(minValue + i * average);
                this.CurrentStorage.SetValue((Storage.ValueName)List_ValueSelected[i], Storage.ValueType.Position, p0);
            }
            this.DrawToPictureBox();
        }
        private void RJ_Button_複製格式_垂直間距平均分配_MouseDownEvent(MouseEventArgs mevent)
        {
            if (List_ValueSelected.Count <= 1)
            {
                this.Invoke(new Action(delegate
                {
                    if (this.vlaueClass_Copy == null) MyMessageBox.ShowDialog("請選取兩個以上面板資料!");
                }));
                return;
            }
            int maxValue = 0;
            int minValue = 99999;
            float average = 0;

            for (int i = 0; i < List_ValueSelected.Count; i++)
            {
                Point p0 = (Point)this.CurrentStorage.GetValue((Storage.ValueName)List_ValueSelected[i], Storage.ValueType.Position);
                if (maxValue < p0.Y) maxValue = p0.Y;
                if (minValue > p0.Y) minValue = p0.Y;
            }
            List_ValueSelected.Sort(new ICP_PostrionY(CurrentStorage));
            average = ((float)maxValue - (float)minValue) / (float)(List_ValueSelected.Count - 1);
            for (int i = 0; i < List_ValueSelected.Count; i++)
            {
                Point p0 = (Point)this.CurrentStorage.GetValue((Storage.ValueName)List_ValueSelected[i], Storage.ValueType.Position);
                p0.Y = (int)(minValue + i * average);
                this.CurrentStorage.SetValue((Storage.ValueName)List_ValueSelected[i], Storage.ValueType.Position, p0);
            }
            this.DrawToPictureBox();
        }
        private void RJ_Button_複製格式_水平間距等距分配_MouseDownEvent(MouseEventArgs mevent)
        {
            if (List_ValueSelected.Count <= 1)
            {
                this.Invoke(new Action(delegate
                {
                    if (this.vlaueClass_Copy == null) MyMessageBox.ShowDialog("請選取兩個以上面板資料!");
                }));
                return;
            }
            DialogResult dialogResult = DialogResult.None;
            int value = -1;
            this.Invoke(new Action(delegate
            {
                Dialog_NumPannel dialog_NumPannel = new Dialog_NumPannel();
                dialogResult = dialog_NumPannel.ShowDialog();
                value = dialog_NumPannel.Value;
            }));

            if (dialogResult != DialogResult.Yes) return;

            List_ValueSelected.Sort(new ICP_PostrionX(CurrentStorage));
            Point p0 = (Point)this.CurrentStorage.GetValue((Storage.ValueName)List_ValueSelected[0], Storage.ValueType.Position);
            for (int i = 0; i < List_ValueSelected.Count; i++)
            {
                this.CurrentStorage.SetValue((Storage.ValueName)List_ValueSelected[i], Storage.ValueType.Position, p0);
                int temp = (int)this.CurrentStorage.GetValue((Storage.ValueName)List_ValueSelected[i], Storage.ValueType.Width);
                p0.X = p0.X + value + temp;
            }
            this.DrawToPictureBox();
        }
        private void RJ_Button_複製格式_垂直間距等距分配_MouseDownEvent(MouseEventArgs mevent)
        {
            if (List_ValueSelected.Count <= 1)
            {
                this.Invoke(new Action(delegate
                {
                    if (this.vlaueClass_Copy == null) MyMessageBox.ShowDialog("請選取兩個以上面板資料!");
                }));
                return;
            }
            DialogResult dialogResult = DialogResult.None;
            int value = -1;
            this.Invoke(new Action(delegate
            {
                Dialog_NumPannel dialog_NumPannel = new Dialog_NumPannel();
                dialogResult = dialog_NumPannel.ShowDialog();
                value = dialog_NumPannel.Value;
            }));
                    
            if (dialogResult != DialogResult.Yes) return;
          
            List_ValueSelected.Sort(new ICP_PostrionY(CurrentStorage));
            Point p0 = (Point)this.CurrentStorage.GetValue((Storage.ValueName)List_ValueSelected[0], Storage.ValueType.Position);
            for (int i = 0; i < List_ValueSelected.Count; i++)
            {
                this.CurrentStorage.SetValue((Storage.ValueName)List_ValueSelected[i], Storage.ValueType.Position, p0);
                int temp = (int)this.CurrentStorage.GetValue((Storage.ValueName)List_ValueSelected[i], Storage.ValueType.Height);
                p0.Y = p0.Y + value + temp;
            }
            this.DrawToPictureBox();
        }

        private class ICP_PostrionX : IComparer<int>
        {
            Storage CurrentStorage;
            public ICP_PostrionX(Storage storage)
            {
                this.CurrentStorage = storage;
            }
            public int Compare(int temp0, int temp1)
            {
                Point p0 = (Point)this.CurrentStorage.GetValue((Storage.ValueName)temp0, Storage.ValueType.Position);
                Point p1 = (Point)this.CurrentStorage.GetValue((Storage.ValueName)temp1, Storage.ValueType.Position);
                return p0.X.CompareTo(p1.X);
            }
        }
        private class ICP_PostrionY : IComparer<int>
        {
            Storage CurrentStorage;
            public ICP_PostrionY(Storage storage)
            {
                this.CurrentStorage = storage;
            }
            public int Compare(int temp0, int temp1)
            {
                Point p0 = (Point)this.CurrentStorage.GetValue((Storage.ValueName)temp0, Storage.ValueType.Position);
                Point p1 = (Point)this.CurrentStorage.GetValue((Storage.ValueName)temp1, Storage.ValueType.Position);
                return p0.Y.CompareTo(p1.Y);
            }
        }

        #endregion
        private void rJ_Button_SAVE_Click(object sender, EventArgs e)
        {
            if (this.saveFileDialog_jpg.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            using (Bitmap bitmap = Get_Storage_bmp(currentStorage , 1))
            {
                ((Image)bitmap).SaveJpeg(this.saveFileDialog_jpg.FileName, 90);
            }

        }
        #endregion


    }
}
