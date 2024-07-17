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
    public partial class EPD266_Paint_Form : MyDialog
    {
        private enum enum_ValueName
        {
            藥碼,
            藥名,
            中文名,
            商品名,
            儲位名稱,
            效期,
            庫存,
            單位,
            條碼,
        }
        public delegate void PanelSelectEventHandler(Storage.ValueName valueName);
        public event PanelSelectEventHandler PanelSelectEvent;

        public delegate void SureClickHandler(Storage storage);
        public event SureClickHandler SureClick;

        static public int Pannel_Width
        {
            get
            {
                return 296;
            }
        }
        static public int Pannel_Height
        {
            get
            {
                return 152;
            }
        }

        private int CanvasScale = 2;

        private Storage currentStorage = null;
        public Storage CurrentStorage
        {
            get
            {
                return currentStorage;
            }
        }
        private Storage currentStorage_buf;
        private List<int> List_ValueSelected = new List<int>();

        public enum TxMouseDownType
        {
            NONE,
            TOP,
            BUTTOM,
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
                switch (value)
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
                    case TxMouseDownType.BUTTOM:
                        this.Cursor = Cursors.SizeNS;
                        break;
                }
            }
        }

        static private string _selectedText;
        static private string selectedText
        {
            get
            {
          
                return _selectedText;
            }
        }
        private int mainSelect = -1;
        private int MainSelect
        {
            get
            {
                return mainSelect;
            }
            set
            {
                if(mainSelect != value)
                {

                    if (PanelSelectEvent != null) PanelSelectEvent(Storage.GetValueName(_selectedText));
                }

                mainSelect = value;

                this.Invoke(new Action(delegate
                {
                    this.comboBox_選擇項目.SelectedIndex = mainSelect;
                    _selectedText = this.comboBox_選擇項目.Text;
                }));
            }
        }

        public EPD266_Paint_Form(Storage _storage)
        {
            InitializeComponent();
            currentStorage = _storage;

            this.pictureBox_paint.Paint += PictureBox_paint_Paint;
            this.pictureBox_paint.MouseDown += PictureBox_paint_MouseDown;
            this.pictureBox_paint.MouseMove += PictureBox_paint_MouseMove;
            this.pictureBox_paint.MouseUp += PictureBox_paint_MouseUp;


            this.ShowDialogEvent += EPD266_Paint_Form_ShowDialogEvent;
            this.LoadFinishedEvent += EPD266_Paint_Form_LoadFinishedEvent;

            this.rJ_Button_確定.MouseDownEvent += RJ_Button_確定_MouseDownEvent;
            this.rJ_Button_取消.MouseDownEvent += RJ_Button_取消_MouseDownEvent;
        }



        private void EPD266_Paint_Form_ShowDialogEvent()
        {
            
        }
        private void EPD266_Paint_Form_LoadFinishedEvent(EventArgs e)
        {
            DrawToPictureBox(CurrentStorage);

            this.PanelSelectEvent += EPD266_Paint_Form_PanelSelectEvent;
            this.comboBox_選擇項目.DataSource = new enum_ValueName().GetEnumNames();
            this.comboBox_選擇項目.SelectedIndexChanged += ComboBox_圖形編輯_編輯內容名稱_SelectedIndexChanged;
            this.comboBox_選擇項目.SelectedIndex = 0;
            ComboBox_圖形編輯_編輯內容名稱_SelectedIndexChanged(null, null);
            this.button_字體.Click += Button_字體_Click;
            this.textBox_邊框大小.Click += TextBox_邊框大小_Click;
            this.rJ_Pannel_背景顏色.Click += RJ_Pannel_背景顏色_Click;
            this.rJ_Pannel_邊框顏色.Click += RJ_Pannel_邊框顏色_Click;
            this.rJ_Pannel_字體顏色.Click += RJ_Pannel_字體顏色_Click;


            this.checkBox_藥碼.Checked = currentStorage.Code_Visable;
            this.checkBox_藥名.Checked = currentStorage.Name_Visable;
            this.checkBox_中文名.Checked = currentStorage.ChineseName_Visable;
            this.checkBox_商品名.Checked = currentStorage.Scientific_Name_Visable;
            this.checkBox_儲位名稱.Checked = currentStorage.StorageName_Visable;
            this.checkBox_效期.Checked = currentStorage.Validity_period_Visable;
            this.checkBox_庫存.Checked = currentStorage.Inventory_Visable;
            this.checkBox_單位.Checked = currentStorage.Package_Visable;
            this.checkBox_條碼.Checked = currentStorage.BarCode_Visable;

            this.checkBox_藥碼.CheckedChanged += CheckBox_CheckedChanged;
            this.checkBox_藥名.CheckedChanged += CheckBox_CheckedChanged;
            this.checkBox_中文名.CheckedChanged += CheckBox_CheckedChanged;
            this.checkBox_商品名.CheckedChanged += CheckBox_CheckedChanged;
            this.checkBox_儲位名稱.CheckedChanged += CheckBox_CheckedChanged;
            this.checkBox_效期.CheckedChanged += CheckBox_CheckedChanged;
            this.checkBox_庫存.CheckedChanged += CheckBox_CheckedChanged;
            this.checkBox_單位.CheckedChanged += CheckBox_CheckedChanged;
            this.checkBox_條碼.CheckedChanged += CheckBox_CheckedChanged;
        }

        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            string name = ((CheckBox)sender).Text;

            Storage.ValueName valueName = Storage.GetValueName(name);
            CurrentStorage.SetValue(valueName, Device.ValueType.Visable, ((CheckBox)sender).Checked);
            this.DrawToPictureBox();
        }

        private void PictureBox_paint_MouseUp(object sender, MouseEventArgs e)
        {
            this.MouseDownType = TxMouseDownType.NONE;
            this.flag_pictureBox_mouseDown = false;
        }
        private void PictureBox_paint_MouseMove(object sender, MouseEventArgs e)
        {
            if (CurrentStorage == null) return;
            int X = e.X / this.CanvasScale;
            int Y = e.Y / this.CanvasScale;
            Storage.VlaueClass vlaueClass;

            if (!this.flag_pictureBox_mouseDown)
            {
                vlaueClass = CurrentStorage.GetValue(GetSelectValueName());
                this.MouseDownType = GetMouseDownType(X, Y, vlaueClass.Position.X, vlaueClass.Position.Y, vlaueClass.Width, vlaueClass.Height);
                // Output debug information to the console
                //Console.WriteLine($"MouseDownType: {this.MouseDownType}");
                //Console.WriteLine($"X: {X}, Y: {Y}, Value Position: ({vlaueClass.Position.X}, {vlaueClass.Position.Y}), Width: {vlaueClass.Width}, Height: {vlaueClass.Height}");
            }

            if (this.flag_pictureBox_mouseDown)
            {
                if (this.MouseDownType == TxMouseDownType.INSIDE)
                {
                    int move_X = X - this.pictureBox_mouseDown_X;
                    int move_Y = Y - this.pictureBox_mouseDown_Y;
                    for (int i = 0; i < List_ValueSelected.Count; i++)
                    {
                        string valueName = ((enum_ValueName)List_ValueSelected[i]).GetEnumName();
                        vlaueClass = currentStorage_buf.GetValue(Storage.GetValueName(valueName));
                        int result_po_X = move_X + vlaueClass.Position.X;
                        int result_po_Y = move_Y + vlaueClass.Position.Y;
                        if (result_po_X < 0) result_po_X = 0;
                        if (result_po_Y < 0) result_po_Y = 0;
                        string _value_selected = ((enum_ValueName)this.List_ValueSelected[i]).GetEnumName();
                        CurrentStorage.SetValue(Storage.GetValueName(_value_selected), Storage.ValueType.Position, new Point(result_po_X, result_po_Y));
                    }
                }
                else if (this.MouseDownType == TxMouseDownType.LEFT)
                {
                    int move_X = X - this.pictureBox_mouseDown_X;
                    int move_Y = Y - this.pictureBox_mouseDown_Y;

                    for (int i = 0; i < List_ValueSelected.Count; i++)
                    {
                        string _value_selected = ((enum_ValueName)this.List_ValueSelected[i]).GetEnumName();
                        vlaueClass = currentStorage_buf.GetValue(Storage.GetValueName(_value_selected));

                        int result_po_X = vlaueClass.Position.X + move_X;
                        int result_po_Y = vlaueClass.Position.Y;
                        int result_Width = vlaueClass.Width - move_X;
                        int result_Height = vlaueClass.Height;
                        if (result_po_X < 0) result_po_X = 0;
                        if (result_po_Y < 0) result_po_Y = 0;
                        if (result_Width < 0) result_Width = 0;
                        if (result_Height < 0) result_Height = 0;
                        CurrentStorage.SetValue(Storage.GetValueName(_value_selected), Storage.ValueType.Position, new Point(result_po_X, result_po_Y));
                        CurrentStorage.SetValue(Storage.GetValueName(_value_selected), Storage.ValueType.Width, result_Width);
                        CurrentStorage.SetValue(Storage.GetValueName(_value_selected), Storage.ValueType.Height, result_Height);
                    }
                }
                else if (this.MouseDownType == TxMouseDownType.RIGHT)
                {
                    int move_X = X - this.pictureBox_mouseDown_X;
                    int move_Y = Y - this.pictureBox_mouseDown_Y;

                    for (int i = 0; i < List_ValueSelected.Count; i++)
                    {
                        string _value_selected = ((enum_ValueName)this.List_ValueSelected[i]).GetEnumName();

                        vlaueClass = currentStorage_buf.GetValue(Storage.GetValueName(_value_selected));

                        int result_po_X = vlaueClass.Position.X;
                        int result_po_Y = vlaueClass.Position.Y;
                        int result_Width = vlaueClass.Width + move_X;
                        int result_Height = vlaueClass.Height;
                        if (result_po_X < 0) result_po_X = 0;
                        if (result_po_Y < 0) result_po_Y = 0;
                        if (result_Width < 0) result_Width = 0;
                        if (result_Height < 0) result_Height = 0;

                        CurrentStorage.SetValue(Storage.GetValueName(_value_selected), Storage.ValueType.Position, new Point(result_po_X, result_po_Y));
                        CurrentStorage.SetValue(Storage.GetValueName(_value_selected), Storage.ValueType.Width, result_Width);
                        CurrentStorage.SetValue(Storage.GetValueName(_value_selected), Storage.ValueType.Height, result_Height);
                    }
                }
                else if (this.MouseDownType == TxMouseDownType.TOP)
                {
                    int move_X = X - this.pictureBox_mouseDown_X;
                    int move_Y = Y - this.pictureBox_mouseDown_Y;

                    for (int i = 0; i < List_ValueSelected.Count; i++)
                    {
                        string _value_selected = ((enum_ValueName)this.List_ValueSelected[i]).GetEnumName();

                        vlaueClass = currentStorage_buf.GetValue(Storage.GetValueName(_value_selected));

                        int result_po_X = vlaueClass.Position.X;
                        int result_po_Y = vlaueClass.Position.Y + move_Y;
                        int result_Width = vlaueClass.Width;
                        int result_Height = vlaueClass.Height - move_Y;
                        if (result_po_X < 0) result_po_X = 0;
                        if (result_po_Y < 0) result_po_Y = 0;
                        if (result_Width < 0) result_Width = 0;
                        if (result_Height < 0) result_Height = 0;

                        CurrentStorage.SetValue(Storage.GetValueName(_value_selected), Storage.ValueType.Position, new Point(result_po_X, result_po_Y));
                        CurrentStorage.SetValue(Storage.GetValueName(_value_selected), Storage.ValueType.Width, result_Width);
                        CurrentStorage.SetValue(Storage.GetValueName(_value_selected), Storage.ValueType.Height, result_Height);
                    }
                }
                else if (this.MouseDownType == TxMouseDownType.BUTTOM)
                {
                    int move_X = X - this.pictureBox_mouseDown_X;
                    int move_Y = Y - this.pictureBox_mouseDown_Y;

                    for (int i = 0; i < List_ValueSelected.Count; i++)
                    {
                        string _value_selected = ((enum_ValueName)this.List_ValueSelected[i]).GetEnumName();

                        vlaueClass = currentStorage_buf.GetValue(Storage.GetValueName(_value_selected));

                        int result_po_X = vlaueClass.Position.X;
                        int result_po_Y = vlaueClass.Position.Y;
                        int result_Width = vlaueClass.Width;
                        int result_Height = vlaueClass.Height + move_Y;
                        if (result_po_X < 0) result_po_X = 0;
                        if (result_po_Y < 0) result_po_Y = 0;
                        if (result_Width < 0) result_Width = 0;
                        if (result_Height < 0) result_Height = 0;

                        CurrentStorage.SetValue(Storage.GetValueName(_value_selected), Storage.ValueType.Position, new Point(result_po_X, result_po_Y));
                        CurrentStorage.SetValue(Storage.GetValueName(_value_selected), Storage.ValueType.Width, result_Width);
                        CurrentStorage.SetValue(Storage.GetValueName(_value_selected), Storage.ValueType.Height, result_Height);
                    }

                }
            }
            if (flag_pictureBox_mouseDown)
            {
                this.DrawToPictureBox();
            }

        }
        private void PictureBox_paint_MouseDown(object sender, MouseEventArgs e)
        {
            if (CurrentStorage == null) return;
            int X = e.X / this.CanvasScale;
            int Y = e.Y / this.CanvasScale;
            Storage.VlaueClass vlaueClass;
            string[] valueNames = new enum_ValueName().GetEnumNames();
            for (int i = 0; i < valueNames.Length; i++)
            {
                vlaueClass = CurrentStorage.GetValue(Storage.GetValueName(valueNames[i]));
                this.MouseDownType = GetMouseDownType(X, Y, vlaueClass.Position.X, vlaueClass.Position.Y, vlaueClass.Width, vlaueClass.Height);
                if (this.MouseDownType != TxMouseDownType.NONE && vlaueClass.Visable)
                {
                    bool isCtrlPressed = (Control.ModifierKeys & Keys.Control) == Keys.Control;
                    bool isShiftPressed = (Control.ModifierKeys & Keys.Shift) == Keys.Shift;

                    if (isCtrlPressed || isShiftPressed)
                    {
                        int indexOf = this.List_ValueSelected.IndexOf(i);
                        if (indexOf == -1)
                        {
                            if (this.List_ValueSelected.Count == 0) MainSelect = i;
                            this.List_ValueSelected.Add(i);
                        }
                        else
                        {
                            MainSelect = i;
                        }
                    }
                    else
                    {
                        this.List_ValueSelected.Clear();
                        this.List_ValueSelected.Add(i);
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
        async private void PictureBox_paint_Paint(object sender, PaintEventArgs e)
        {
            Task task = Task.Factory.StartNew(new Action(delegate
            {
                this.DrawToPictureBox();
            }));
            await task;
        }
        private void RJ_Pannel_背景顏色_Click(object sender, EventArgs e)
        {
            Dialog_EPD266_顏色選擇 dialog_EPD266_顏色選擇 = new Dialog_EPD266_顏色選擇(this.rJ_Pannel_背景顏色.BackgroundColor);
            if (dialog_EPD266_顏色選擇.ShowDialog() != DialogResult.Yes) return;
            this.rJ_Pannel_背景顏色.BackgroundColor = dialog_EPD266_顏色選擇.Value;
            
            Storage.ValueName valueName = (GetSelectValueName());
            CurrentStorage.SetValue(valueName, Device.ValueType.ForeColor, this.rJ_Pannel_背景顏色.BackgroundColor);
            this.DrawToPictureBox();
        }
        private void RJ_Pannel_字體顏色_Click(object sender, EventArgs e)
        {
            Dialog_EPD266_顏色選擇 dialog_EPD266_顏色選擇 = new Dialog_EPD266_顏色選擇(this.rJ_Pannel_字體顏色.BackgroundColor);
            if (dialog_EPD266_顏色選擇.ShowDialog() != DialogResult.Yes) return;
            this.rJ_Pannel_字體顏色.BackgroundColor = dialog_EPD266_顏色選擇.Value;

            Storage.ValueName valueName = (GetSelectValueName());
            CurrentStorage.SetValue(valueName, Device.ValueType.ForeColor, this.rJ_Pannel_字體顏色.BackgroundColor);
            this.DrawToPictureBox();

        }
        private void RJ_Pannel_邊框顏色_Click(object sender, EventArgs e)
        {
            Dialog_EPD266_顏色選擇 dialog_EPD266_顏色選擇 = new Dialog_EPD266_顏色選擇(this.rJ_Pannel_邊框顏色.BackgroundColor);
            if (dialog_EPD266_顏色選擇.ShowDialog() != DialogResult.Yes) return;
            this.rJ_Pannel_邊框顏色.BackgroundColor = dialog_EPD266_顏色選擇.Value;

            Storage.ValueName valueName = (GetSelectValueName());
            CurrentStorage.SetValue(valueName, Device.ValueType.ForeColor, this.rJ_Pannel_邊框顏色.BackgroundColor);
            this.DrawToPictureBox();
        }
        private void EPD266_Paint_Form_PanelSelectEvent(Device.ValueName valueName)
        {
            this.textBox_邊框大小.Text = CurrentStorage.GetValue(valueName).BorderSize.ToString();
            textBox_字體.Text = CurrentStorage.GetValue(valueName).Font.ToFontString();
            rJ_Pannel_邊框顏色.BackgroundColor = CurrentStorage.GetValue(valueName).BorderColor;
            rJ_Pannel_字體顏色.BackgroundColor = CurrentStorage.GetValue(valueName).ForeColor;
            rJ_Pannel_背景顏色.BackgroundColor = CurrentStorage.BackColor;

        }
        private void ComboBox_圖形編輯_編輯內容名稱_SelectedIndexChanged(object sender, EventArgs e)
        {
            MainSelect = this.comboBox_選擇項目.SelectedIndex;
            this.List_ValueSelected.Clear();
            this.List_ValueSelected.Add(MainSelect);

            this.DrawToPictureBox();
        }
        private void Button_字體_Click(object sender, EventArgs e)
        {
            if (this.fontDialog.ShowDialog() != DialogResult.OK) return;
            Storage.ValueName valueName = (GetSelectValueName());
            CurrentStorage.SetValue(valueName, Device.ValueType.Font, this.fontDialog.Font);
            this.DrawToPictureBox();
        }
        private void TextBox_邊框大小_Click(object sender, EventArgs e)
        {
            Dialog_NumPannel dialog_NumPannel = new Dialog_NumPannel();
            dialog_NumPannel.ShowDialog();
            this.textBox_邊框大小.Text = dialog_NumPannel.Value.ToString();
            Storage.ValueName valueName = (GetSelectValueName());
            CurrentStorage.SetValue(valueName, Device.ValueType.BorderSize, dialog_NumPannel.Value);
            this.DrawToPictureBox();
        }
        private void RJ_Button_確定_MouseDownEvent(MouseEventArgs mevent)
        {
            this.DialogResult = DialogResult.Yes;
            if (SureClick != null) SureClick(currentStorage);
        }
        private void RJ_Button_取消_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Close();
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
                Storage.VlaueClass vlaueClass = storage.GetValue(Storage.GetValueName(selectedText));
                Size size = TextRenderer.MeasureText(vlaueClass.Value, vlaueClass.Font);
                if (vlaueClass.Width < size.Width) vlaueClass.Width = size.Width;
                if (vlaueClass.Height < size.Height) vlaueClass.Height = size.Height;
                this.CurrentStorage.SetValue(vlaueClass);
            }

            using (Bitmap bitmap = this.Get_Storage_bmp(storage))
            {
                using (Graphics g = pictureBox_paint.CreateGraphics())
                {
                    g.SmoothingMode = SmoothingMode.HighQuality; //使繪圖質量最高，即消除鋸齒
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
                    g.DrawImage(bitmap, new PointF());
                }
            }

        }
        private Storage.ValueName GetSelectValueName()
        {
            return Storage.GetValueName(_selectedText);
        }
        public Bitmap Get_Storage_bmp(Storage storage)
        {
            storage.IP_Visable = false;
            storage.MinPackage_Visable = false;
            storage.Min_Package_Num_Visable = false;
            storage.Max_Inventory_Visable = false;
            storage.Port_Visable = false;
            storage.Label_Visable = false;

            Storage.VlaueClass vlaueClass;
            Bitmap bitmap = new Bitmap(Pannel_Width * CanvasScale, Pannel_Height * CanvasScale);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.HighQuality; //使繪圖質量最高，即消除鋸齒
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
                g.FillRectangle(new SolidBrush(storage.BackColor), 0, 0, Pannel_Width * CanvasScale, Pannel_Height * CanvasScale);
                string[] valuenames = new enum_ValueName().GetEnumNames();

                for (int i = 0; i < valuenames.Length; i++)
                {
                    vlaueClass = storage.GetValue(Storage.GetValueName(valuenames[i]));
                    vlaueClass.Position.X = vlaueClass.Position.X * CanvasScale;
                    vlaueClass.Position.Y = vlaueClass.Position.Y * CanvasScale;
                    if (vlaueClass.Visable)
                    {
                        using (Bitmap bitmap_temp = storage.Get_EPD_Bitmap(Storage.GetValueName(valuenames[i]), CanvasScale))
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
                    size_IP = TextRenderer.MeasureText(ip, new Font("微軟正黑體", 8 * CanvasScale, FontStyle.Bold));
                    g.DrawString(ip, new Font("微軟正黑體", 8 * CanvasScale, FontStyle.Bold), new SolidBrush((Color)storage.GetValue(Storage.ValueName.IP, Storage.ValueType.ForeColor)), (Pannel_Width * CanvasScale - size_IP.Width) , (Pannel_Height * CanvasScale - size_IP.Height));
                }
                for (int i = 0; i < List_ValueSelected.Count; i++)
                {
                    string _value_selected = ((enum_ValueName)this.List_ValueSelected[i]).GetEnumName();

                    vlaueClass = storage.GetValue(Storage.GetValueName(_value_selected));
                    vlaueClass.Position.X = vlaueClass.Position.X * CanvasScale;
                    vlaueClass.Position.Y = vlaueClass.Position.Y * CanvasScale;
                    if (vlaueClass.Visable)
                    {
                        using (Bitmap bitmap_temp = storage.Get_EPD_Bitmap(Storage.GetValueName(_value_selected), CanvasScale, Color.Blue, 4, (List_ValueSelected[i] != MainSelect)))
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
            storage.IP_Visable = false;
            storage.MinPackage_Visable = false;
            storage.Min_Package_Num_Visable = false;
            storage.Max_Inventory_Visable = false;
            storage.Port_Visable = false;
            storage.Label_Visable = false;
            Storage.VlaueClass vlaueClass;
            Bitmap bitmap = new Bitmap(Pannel_Width * canvasScale, Pannel_Height * canvasScale);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.HighQuality; //使繪圖質量最高，即消除鋸齒
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
                g.FillRectangle(new SolidBrush(storage.BackColor), 0, 0, Pannel_Width * canvasScale, Pannel_Height * canvasScale);

                string[] valuenames = new enum_ValueName().GetEnumNames();

                for (int i = 0; i < valuenames.Length; i++)
                {
                    vlaueClass = storage.GetValue(Storage.GetValueName(valuenames[i]));
                    vlaueClass.Position.X = vlaueClass.Position.X * canvasScale;
                    vlaueClass.Position.Y = vlaueClass.Position.Y * canvasScale;
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
                    g.DrawString(ip, new Font("微軟正黑體", 8, FontStyle.Bold), new SolidBrush((Color)storage.GetValue(Storage.ValueName.IP, Storage.ValueType.ForeColor)), (Pannel_Width - size_IP.Width) * canvasScale, (Pannel_Height - size_IP.Height) * canvasScale);
                }

            }
            return bitmap;

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
                return TxMouseDownType.BUTTOM;
            }
            else
            {
                return TxMouseDownType.NONE;
            }
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
    }
}
