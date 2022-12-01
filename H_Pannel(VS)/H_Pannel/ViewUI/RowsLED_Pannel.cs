using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Basic;
namespace H_Pannel_lib
{
    [DefaultEvent("ValueChanged")]
    public partial class RowsLED_Pannel : UserControl
    {
        private bool autoWrite = true;
        [Category("自訂屬性"), Description("自動寫入")]
        public bool AutoWrite
        {
            get
            {
                return this.autoWrite;
            }
            set
            {
                this.autoWrite = value;
            }
        }
        [Category("自訂屬性"), Description("背景条颜色")]
        public int SliderSize
        {
            get
            {
                return this.rJ_TrackBar.SliderSize;
            }
            set
            {
                this.rJ_TrackBar.SliderSize = value;
            }
        }
        [Category("自訂屬性"), Description("背景条颜色")]
        public Color BarColor
        {
            get { return this.rJ_TrackBar.BarColor; }
            set
            {
                this.rJ_TrackBar.BarColor = value;
            }
        }
        [Category("自訂屬性"), Description("滑块颜色")]
        public Color SliderColor
        {
            get { return this.rJ_TrackBar.SliderColor; }
            set
            {
                this.rJ_TrackBar.SliderColor = value;
                this.rJ_TrackBar_ValueChanged(this.MinValue, this.MaxValue);
            }
        }
        [Category("自訂屬性"), Description("滑块颜色")]
        public Color TopSliderColor
        {
            get { return this.rJ_TrackBar.TopSliderColor; }
            set
            {
                this.rJ_TrackBar.TopSliderColor = value;
            }
        }
        [Category("自訂屬性"), Description("滑块颜色")]
        public Color BottomSliderColor
        {
            get { return this.rJ_TrackBar.BottomSliderColor; }
            set
            {
                this.rJ_TrackBar.BottomSliderColor = value;
            }
        }
        [Category("自訂屬性"), Description("最小值<para>范围：大于等于0</para>")]
        public int Minimum
        {
            get { return this.rJ_TrackBar.Minimum; }
            set
            {
                this.rJ_TrackBar.Minimum = value;
            }
        }
        [Category("自訂屬性"), Description("最大值")]
        public int Maximum
        {
            get { return this.rJ_TrackBar.Maximum; }
            set
            {
                this.rJ_TrackBar.Maximum = value;
            }
        }
        [Category("自訂屬性"), Description("当前值")]
        public int MinValue
        {
            get { return this.rJ_TrackBar.MinValue; }
            set
            {
                this.rJ_TrackBar.MinValue = value;
            }
        }
        [Category("自訂屬性"), Description("当前值")]
        public int MaxValue
        {
            get { return this.rJ_TrackBar.MaxValue; }
            set
            {
                this.rJ_TrackBar.MaxValue = value;
            }
        }
        [Category("自訂屬性"), Description("滑条高度（水平）/宽度（垂直)")]
        public int BarSize
        {
            get { return this.rJ_TrackBar.BarSize; }
            set
            {
                this.rJ_TrackBar.BarSize = value;
         
            }
        }
        [Category("自訂屬性"), Description("滑條字體大小")]
        public Font BarFont
        {
            get { return this.rJ_TrackBar.BarFont; }
            set
            {
                this.rJ_TrackBar.BarFont = value;
            }
        }
        [Category("自訂屬性"), Description("滑條字體顏色")]
        public Color BarForeColor
        {
            get { return this.rJ_TrackBar.BarForeColor; }
            set
            {
                this.rJ_TrackBar.BarForeColor = value;
            }
        }


        public delegate void ValueChangedEventHandler(int MinValue, int MaxValue);
        public event ValueChangedEventHandler ValueChanged;

        private List<UDP_Class> List_UDP_Local;
        private bool flag_ValueChangedEvent_Enable = true;
        private MyThread MyThread_Program;
        private int minValue_buf = -1;
        private int maxValue_buf = -1;

        private RowsLED currentRowsLED;
        [Browsable(false)]
        public RowsLED CurrentRowsLED
        {
            set
            {
                currentRowsLED = value;
                minValue_buf = -1;
                maxValue_buf = -1;
            }
            get
            {
                return currentRowsLED;
            }
        }
        private RowsDevice currentRowsDevice;
        [Browsable(false)]
        public RowsDevice CurrentRowsDevice
        {
            set
            {
                currentRowsDevice = value;
                minValue_buf = -1;
                maxValue_buf = -1;
            }
            get
            {
                return currentRowsDevice;
            }
        }

        [Browsable(false)]
        public string RowsDeviceGUID
        {
            set
            {
                if (CurrentRowsLED == null) return;
                flag_ValueChangedEvent_Enable = false;
                CurrentRowsDevice = CurrentRowsLED.SortByGUID(value);
                if (CurrentRowsDevice != null)
                {
                    this.rJ_TrackBar.SetValue(CurrentRowsDevice.StartLED, CurrentRowsDevice.EndLED);
                }
                else
                {
                    this.rJ_TrackBar.SetValue(0,0);

                }
                flag_ValueChangedEvent_Enable = true;
            }
            get
            {
                if (CurrentRowsDevice == null) return "";
                return CurrentRowsDevice.GUID;
            }
        }
        public RowsLED_Pannel()
        {
            InitializeComponent();
            this.rJ_TrackBar.ValueChanged += rJ_TrackBar_ValueChanged;
        }
        public void Init(List<UDP_Class> List_UDP_Local)
        {
            this.List_UDP_Local = List_UDP_Local;
            this.MyThread_Program = new MyThread();
            this.MyThread_Program.AutoRun(true);
            this.MyThread_Program.SetSleepTime(1);
            this.MyThread_Program.Add_Method(sub_Porgram);
            this.MyThread_Program.Trigger();
        }
        public void SetValue(int minValue, int maxValue)
        {
            this.rJ_TrackBar.SetValue(minValue, maxValue);
        }
        public void SetMaxMinmun(int minimum, int maximum)
        {
            this.rJ_TrackBar.SetMaxMinmun(minimum, maximum);
        }
        private List<UDP_Class> list_UDP_Class = new List<UDP_Class>();
        private List<string> list_UDP_IP = new List<string>();
        private List<byte[]> list_UPD_LEDByte = new List<byte[]>();
        private List<int> list_UPD_MinValue = new List<int>();
        private List<int> list_UPD_MaxValue = new List<int>();
        private List<Color> list_UPD_Color = new List<Color>();
        
        private void sub_Porgram()
        {
            if (list_UDP_Class.Count == 0) return;
            string IP = list_UDP_IP[list_UDP_IP.Count - 1];
            UDP_Class uDP_Class = list_UDP_Class[list_UDP_Class.Count - 1];
            byte[] UPD_LEDByte = list_UPD_LEDByte[list_UPD_LEDByte.Count - 1].ToArray();
            int UPD_MinValue = list_UPD_MinValue[list_UPD_MinValue.Count - 1];
            int UPD_MaxValue = list_UPD_MaxValue[list_UPD_MaxValue.Count - 1];
            Color UPD_Color = list_UPD_Color[list_UPD_Color.Count - 1];
            list_UDP_IP.RemoveAt(list_UDP_IP.Count - 1);
            list_UDP_Class.RemoveAt(list_UDP_Class.Count - 1);
            list_UPD_LEDByte.RemoveAt(list_UPD_LEDByte.Count - 1);
            list_UPD_MinValue.RemoveAt(list_UPD_MinValue.Count - 1);
            list_UPD_MaxValue.RemoveAt(list_UPD_MaxValue.Count - 1);
            list_UPD_Color.RemoveAt(list_UPD_Color.Count - 1);
          
            if (uDP_Class != null)
            {
                RowsLEDUI.Set_Rows_LED_UDP(uDP_Class, IP, UPD_LEDByte);
            }
            Console.WriteLine($"MinValue : {UPD_MinValue} ,MaxValue : {UPD_MaxValue} ");
        }
        private void rJ_TrackBar_ValueChanged(int MinValue, int MaxValue)
        {
            if (!flag_ValueChangedEvent_Enable) return;
            if (CurrentRowsLED != null)
            {
                if (AutoWrite)
                {
                    if (minValue_buf == MinValue && maxValue_buf == MaxValue) return;
                    minValue_buf = MinValue;
                    maxValue_buf = MaxValue;
                    CurrentRowsLED.LED_Bytes = RowsLEDUI.Get_Empty_LEDBytes();            
                    RowsLEDUI.Get_Rows_LEDBytes(ref CurrentRowsLED.LED_Bytes, MinValue, MaxValue, SliderColor);
                    UDP_Class uDP_Class = List_UDP_Local.SortByPort(CurrentRowsLED.Port);
                    if(uDP_Class != null)
                    {
                        list_UDP_Class.Insert(0, uDP_Class);
                        list_UDP_IP.Insert(0, CurrentRowsLED.IP);
                        list_UPD_LEDByte.Insert(0, CurrentRowsLED.LED_Bytes);
                        list_UPD_MinValue.Insert(0, MinValue);
                        list_UPD_MaxValue.Insert(0, MaxValue);
                        list_UPD_Color.Insert(0, SliderColor);
                    }
                     
                    
                }
                if (CurrentRowsDevice != null)
                {
                    CurrentRowsDevice.StartLED = MinValue;
                    CurrentRowsDevice.EndLED = MaxValue;
                }
            }

            ValueChanged?.Invoke(MinValue, MaxValue);
        }

    }
}
