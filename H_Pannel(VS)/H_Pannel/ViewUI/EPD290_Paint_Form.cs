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
    public partial class EPD290_Paint_Form : MyDialog
    {
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
                return 128;
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
            BOTTOM,
            LEFT,
            RIGHT,
            INSIDE,
            TOP_LEFT,
            TOP_RIGHT,
            BOTTOM_LEFT,
            BOTTOM_RIGHT
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
                    case TxMouseDownType.BOTTOM:
                        this.Cursor = Cursors.SizeNS;
                        break;
                    case TxMouseDownType.TOP_LEFT:
                        this.Cursor = Cursors.SizeNWSE;
                        break;
                    case TxMouseDownType.TOP_RIGHT:
                        this.Cursor = Cursors.SizeNESW;
                        break;
                    case TxMouseDownType.BOTTOM_LEFT:
                        this.Cursor = Cursors.SizeNESW;
                        break;
                    case TxMouseDownType.BOTTOM_RIGHT:
                        this.Cursor = Cursors.SizeNWSE;
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
                this.Invoke(new Action(delegate
                {
                    this.comboBox_選擇項目.SelectedIndex = value;
                    _selectedText = this.comboBox_選擇項目.Text;
                }));

                if (mainSelect != value)
                {

                    if (PanelSelectEvent != null) PanelSelectEvent(Storage.GetValueName(_selectedText));
                }

                mainSelect = value;
            }
        }

        public EPD290_Paint_Form(Storage _storage)
        {
            InitializeComponent();
            currentStorage = _storage;

            this.pictureBox_paint.Paint += PictureBox_paint_Paint;
            this.pictureBox_paint.MouseDown += PictureBox_paint_MouseDown;
            this.pictureBox_paint.MouseMove += PictureBox_paint_MouseMove;
            this.pictureBox_paint.MouseUp += PictureBox_paint_MouseUp;


            this.ShowDialogEvent += EPD290_Paint_Form_ShowDialogEvent;
            this.LoadFinishedEvent += EPD290_Paint_Form_LoadFinishedEvent;

            this.rJ_Button_確定.MouseDownEvent += RJ_Button_確定_MouseDownEvent;
            this.rJ_Button_取消.MouseDownEvent += RJ_Button_取消_MouseDownEvent;
        }

        private void EPD290_Paint_Form_ShowDialogEvent()
        {

        }
        private void EPD290_Paint_Form_LoadFinishedEvent(EventArgs e)
        {
            DrawToPictureBox(CurrentStorage);

            this.PanelSelectEvent += EPD290_Paint_Form_PanelSelectEvent;
            this.comboBox_選擇項目.DataSource = new StoragePanel.enum_ValueName().GetEnumNames();
            this.comboBox_選擇項目.SelectedIndexChanged += ComboBox_圖形編輯_編輯內容名稱_SelectedIndexChanged;
            this.comboBox_選擇項目.SelectedIndex = 0;
            ComboBox_圖形編輯_編輯內容名稱_SelectedIndexChanged(null, null);
            this.button_字體.Click += Button_字體_Click;
            this.textBox_邊框大小.Click += TextBox_邊框大小_Click;
            this.rJ_Pannel_文字背景顏色.Click += RJ_Pannel_文字背景顏色_Click;
            this.rJ_Pannel_邊框顏色.Click += RJ_Pannel_邊框顏色_Click;
            this.rJ_Pannel_字體顏色.Click += RJ_Pannel_字體顏色_Click;
            this.rJ_Pannel_背景顏色.Click += RJ_Pannel_背景顏色_Click;


            this.button_對齊靠上.Click += Button_對齊靠上_Click;
            this.button_對齊靠下.Click += Button_對齊靠下_Click;
            this.button_對齊置中.Click += Button_對齊置中_Click;
            this.button_對齊靠左.Click += Button_對齊靠左_Click;
            this.button_對齊靠右.Click += Button_對齊靠右_Click;
            this.button_文本1_儲存.Click += Button_文本1_儲存_Click;
            this.button_文本2_儲存.Click += Button_文本2_儲存_Click;
            this.button_文本3_儲存.Click += Button_文本3_儲存_Click;

            rJ_TextBox_文本1.Texts = currentStorage.CustomText1;
            rJ_TextBox_文本2.Texts = currentStorage.CustomText2;
            rJ_TextBox_文本3.Texts = currentStorage.CustomText3;

            this.checkBox_藥碼.Checked = currentStorage.Code_Visable;
            this.checkBox_藥名.Checked = currentStorage.Name_Visable;
            this.checkBox_中文名.Checked = currentStorage.ChineseName_Visable;
            this.checkBox_商品名.Checked = currentStorage.Scientific_Name_Visable;
            this.checkBox_儲位名稱.Checked = currentStorage.StorageName_Visable;
            this.checkBox_效期.Checked = currentStorage.Validity_period_Visable;
            this.checkBox_庫存.Checked = currentStorage.Inventory_Visable;
            this.checkBox_單位.Checked = currentStorage.Package_Visable;
            this.checkBox_條碼.Checked = currentStorage.BarCode_Visable;
            this.checkBox_文本1.Checked = currentStorage.CustomText1_Visable;
            this.checkBox_文本2.Checked = currentStorage.CustomText2_Visable;
            this.checkBox_文本3.Checked = currentStorage.CustomText3_Visable;

            this.checkBox_藥碼.CheckedChanged += CheckBox_CheckedChanged;
            this.checkBox_藥名.CheckedChanged += CheckBox_CheckedChanged;
            this.checkBox_中文名.CheckedChanged += CheckBox_CheckedChanged;
            this.checkBox_商品名.CheckedChanged += CheckBox_CheckedChanged;
            this.checkBox_儲位名稱.CheckedChanged += CheckBox_CheckedChanged;
            this.checkBox_效期.CheckedChanged += CheckBox_CheckedChanged;
            this.checkBox_庫存.CheckedChanged += CheckBox_CheckedChanged;
            this.checkBox_單位.CheckedChanged += CheckBox_CheckedChanged;
            this.checkBox_條碼.CheckedChanged += CheckBox_CheckedChanged;
            this.checkBox_文本1.CheckedChanged += CheckBox_CheckedChanged;
            this.checkBox_文本2.CheckedChanged += CheckBox_CheckedChanged;
            this.checkBox_文本3.CheckedChanged += CheckBox_CheckedChanged;
            rJ_Pannel_背景顏色.BackgroundColor = CurrentStorage.BackColor;

            this.comboBox_圖片1.DataSource = new enum_PictureType().GetEnumNames();
            this.comboBox_圖片1.Text = currentStorage.Picture1_Name;
            if (this.comboBox_圖片1.Text.StringIsEmpty())
            {
                this.comboBox_圖片1.Text = "無";
            }
            this.comboBox_圖片1.SelectedIndexChanged += ComboBox_圖片1_SelectedIndexChanged;

            this.comboBox_圖片2.DataSource = new enum_PictureType().GetEnumNames();
            this.comboBox_圖片2.Text = currentStorage.Picture2_Name;
            if (this.comboBox_圖片2.Text.StringIsEmpty())
            {
                this.comboBox_圖片2.Text = "無";
            }
            this.comboBox_圖片2.SelectedIndexChanged += ComboBox_圖片2_SelectedIndexChanged;

            this.rJ_RatioButton_預設樣式1.CheckedChanged += RJ_RatioButton_預設樣式1_CheckedChanged;
            this.rJ_RatioButton_自定義.CheckedChanged += RJ_RatioButton_自定義_CheckedChanged;
            if (currentStorage.Enum_drawType == Storage.enum_DrawType.type1)
            {
                rJ_RatioButton_預設樣式1.Checked = true;
            }
            if (currentStorage.Enum_drawType == Storage.enum_DrawType.constom)
            {
                rJ_RatioButton_自定義.Checked = true;
            }
            this.Refresh();
        }

        private void ComboBox_圖片1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string name = "圖片1";

            Storage.ValueName valueName = Storage.GetValueName(name);
            CurrentStorage.SetValue(valueName, Device.ValueType.Value, this.comboBox_圖片1.Text);
            this.DrawToPictureBox();
        }
        private void ComboBox_圖片2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string name = "圖片2";

            Storage.ValueName valueName = Storage.GetValueName(name);
            CurrentStorage.SetValue(valueName, Device.ValueType.Value, this.comboBox_圖片2.Text);
            this.DrawToPictureBox();
        }

        private void ComboBox_圖形編輯_編輯內容名稱_SelectedIndexChanged(object sender, EventArgs e)
        {
            MainSelect = this.comboBox_選擇項目.SelectedIndex;
            this.List_ValueSelected.Clear();
            this.List_ValueSelected.Add(MainSelect);

            this.DrawToPictureBox();
        }
        private void EPD290_Paint_Form_PanelSelectEvent(Device.ValueName valueName)
        {
            this.textBox_邊框大小.Text = CurrentStorage.GetValue(valueName).BorderSize.ToString();
            textBox_字體.Text = CurrentStorage.GetValue(valueName).Font.ToFontString();
            rJ_Pannel_邊框顏色.BackgroundColor = CurrentStorage.GetValue(valueName).BorderColor;
            rJ_Pannel_字體顏色.BackgroundColor = CurrentStorage.GetValue(valueName).ForeColor;
            rJ_Pannel_文字背景顏色.BackgroundColor = CurrentStorage.GetValue(valueName).BackColor;

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
            if (rJ_RatioButton_自定義.Checked == false) return;
            this.MouseDownType = TxMouseDownType.NONE;
            this.flag_pictureBox_mouseDown = false;
        }
        private void PictureBox_paint_MouseMove(object sender, MouseEventArgs e)
        {
            if (rJ_RatioButton_自定義.Checked == false) return;
            if (CurrentStorage == null) return;
            int X = e.X / this.CanvasScale;
            int Y = e.Y / this.CanvasScale;
            Storage.VlaueClass vlaueClass;

            if (!this.flag_pictureBox_mouseDown)
            {
                vlaueClass = CurrentStorage.GetValue(GetSelectValueName());
                if (vlaueClass.Visable == false) return;
                this.MouseDownType = GetMouseDownType(X, Y, vlaueClass.Position.X, vlaueClass.Position.Y, vlaueClass.Width, vlaueClass.Height);
                // Output debug information to the console
                // Console.WriteLine($"MouseDownType: {this.MouseDownType}");
                // Console.WriteLine($"X: {X}, Y: {Y}, Value Position: ({vlaueClass.Position.X}, {vlaueClass.Position.Y}), Width: {vlaueClass.Width}, Height: {vlaueClass.Height}");
            }

            if (this.flag_pictureBox_mouseDown)
            {
                int move_X = X - this.pictureBox_mouseDown_X;
                int move_Y = Y - this.pictureBox_mouseDown_Y;

                for (int i = 0; i < List_ValueSelected.Count; i++)
                {
                    string valueName = ((StoragePanel.enum_ValueName)List_ValueSelected[i]).GetEnumName();
                    vlaueClass = currentStorage_buf.GetValue(Storage.GetValueName(valueName));
                    if (vlaueClass.Visable == false) return;

                    int result_po_X = vlaueClass.Position.X;
                    int result_po_Y = vlaueClass.Position.Y;
                    int result_Width = vlaueClass.Width;
                    int result_Height = vlaueClass.Height;

                    switch (this.MouseDownType)
                    {
                        case TxMouseDownType.INSIDE:
                            result_po_X += move_X;
                            result_po_Y += move_Y;
                            break;
                        case TxMouseDownType.LEFT:
                            result_po_X += move_X;
                            result_Width -= move_X;
                            break;
                        case TxMouseDownType.RIGHT:
                            result_Width += move_X;
                            break;
                        case TxMouseDownType.TOP:
                            result_po_Y += move_Y;
                            result_Height -= move_Y;
                            break;
                        case TxMouseDownType.BOTTOM:
                            result_Height += move_Y;
                            break;
                        case TxMouseDownType.TOP_LEFT:
                            result_po_X += move_X;
                            result_Width -= move_X;
                            result_po_Y += move_Y;
                            result_Height -= move_Y;
                            break;
                        case TxMouseDownType.TOP_RIGHT:
                            result_Width += move_X;
                            result_po_Y += move_Y;
                            result_Height -= move_Y;
                            break;
                        case TxMouseDownType.BOTTOM_LEFT:
                            result_po_X += move_X;
                            result_Width -= move_X;
                            result_Height += move_Y;
                            break;
                        case TxMouseDownType.BOTTOM_RIGHT:
                            result_Width += move_X;
                            result_Height += move_Y;
                            break;
                    }

                    // 防止位置和大小的负值
                    if (result_po_X < 0) result_po_X = 0;
                    if (result_po_Y < 0) result_po_Y = 0;
                    if (result_Width < 0) result_Width = 0;
                    if (result_Height < 0) result_Height = 0;

                    CurrentStorage.SetValue(Storage.GetValueName(valueName), Storage.ValueType.Position, new Point(result_po_X, result_po_Y));
                    CurrentStorage.SetValue(Storage.GetValueName(valueName), Storage.ValueType.Width, result_Width);
                    CurrentStorage.SetValue(Storage.GetValueName(valueName), Storage.ValueType.Height, result_Height);
                }
            }

            if (flag_pictureBox_mouseDown)
            {
                this.DrawToPictureBox();
            }
        }
        private void PictureBox_paint_MouseDown(object sender, MouseEventArgs e)
        {
            if (rJ_RatioButton_自定義.Checked == false) return;
            if (CurrentStorage == null) return;
            int X = e.X / this.CanvasScale;
            int Y = e.Y / this.CanvasScale;
            Storage.VlaueClass vlaueClass;
            string[] valueNames = new StoragePanel.enum_ValueName().GetEnumNames();
            for (int i = 0; i < valueNames.Length; i++)
            {
                vlaueClass = CurrentStorage.GetValue(Storage.GetValueName(valueNames[i]));
                Size size = DrawingClass.Draw.MeasureText(vlaueClass.Value, vlaueClass.Font);
                if (vlaueClass.Width == 0 || vlaueClass.Height == 0)
                {
                    vlaueClass.Width = Size.Width;
                    vlaueClass.Height = Size.Height;
                }
                this.MouseDownType = GetMouseDownType(X, Y, vlaueClass.Position.X, vlaueClass.Position.Y, vlaueClass.Width, vlaueClass.Height);
                if (this.MouseDownType != TxMouseDownType.NONE)
                {
                    if (vlaueClass.Visable == false) continue;
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
        private void RJ_Pannel_文字背景顏色_Click(object sender, EventArgs e)
        {
            Dialog_3color_select Dialog_3color_select = new Dialog_3color_select(this.rJ_Pannel_文字背景顏色.BackgroundColor);
            if (Dialog_3color_select.ShowDialog() != DialogResult.Yes) return;
            this.rJ_Pannel_文字背景顏色.BackgroundColor = Dialog_3color_select.Value;

            Storage.ValueName valueName = (GetSelectValueName());
            CurrentStorage.SetValue(valueName, Device.ValueType.BackColor, this.rJ_Pannel_文字背景顏色.BackgroundColor);
            this.DrawToPictureBox();
        }
        private void RJ_Pannel_字體顏色_Click(object sender, EventArgs e)
        {
            Dialog_3color_select Dialog_3color_select = new Dialog_3color_select(this.rJ_Pannel_字體顏色.BackgroundColor);
            if (Dialog_3color_select.ShowDialog() != DialogResult.Yes) return;
            this.rJ_Pannel_字體顏色.BackgroundColor = Dialog_3color_select.Value;

            Storage.ValueName valueName = (GetSelectValueName());
            CurrentStorage.SetValue(valueName, Device.ValueType.ForeColor, this.rJ_Pannel_字體顏色.BackgroundColor);
            this.DrawToPictureBox();

        }
        private void RJ_Pannel_邊框顏色_Click(object sender, EventArgs e)
        {
            Dialog_3color_select Dialog_3color_select = new Dialog_3color_select(this.rJ_Pannel_邊框顏色.BackgroundColor);
            if (Dialog_3color_select.ShowDialog() != DialogResult.Yes) return;
            this.rJ_Pannel_邊框顏色.BackgroundColor = Dialog_3color_select.Value;

            Storage.ValueName valueName = (GetSelectValueName());
            CurrentStorage.SetValue(valueName, Device.ValueType.BorderColor, this.rJ_Pannel_邊框顏色.BackgroundColor);
            this.DrawToPictureBox();
        }
        private void RJ_Pannel_背景顏色_Click(object sender, EventArgs e)
        {
            Dialog_3color_select Dialog_3color_select = new Dialog_3color_select(this.rJ_Pannel_背景顏色.BackgroundColor);
            if (Dialog_3color_select.ShowDialog() != DialogResult.Yes) return;
            this.rJ_Pannel_背景顏色.BackgroundColor = Dialog_3color_select.Value;

            CurrentStorage.BackColor = rJ_Pannel_背景顏色.BackgroundColor;
            this.DrawToPictureBox();
        }
        private void Button_文本3_儲存_Click(object sender, EventArgs e)
        {
            CurrentStorage.CustomText3 = rJ_TextBox_文本3.Texts;
            this.DrawToPictureBox();
            MyMessageBox.ShowDialog("完成");
        }
        private void Button_文本2_儲存_Click(object sender, EventArgs e)
        {
            CurrentStorage.CustomText2 = rJ_TextBox_文本2.Texts;
            this.DrawToPictureBox();
            MyMessageBox.ShowDialog("完成");
        }
        private void Button_文本1_儲存_Click(object sender, EventArgs e)
        {
            CurrentStorage.CustomText1 = rJ_TextBox_文本1.Texts;
            this.DrawToPictureBox();
            MyMessageBox.ShowDialog("完成");
        }
        private void Button_字體_Click(object sender, EventArgs e)
        {
            Storage.ValueName valueName = (GetSelectValueName());
            this.fontDialog.Font = CurrentStorage.GetValue(valueName).Font;
            if (this.fontDialog.ShowDialog() != DialogResult.OK) return;
          
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

        private void Button_對齊靠右_Click(object sender, EventArgs e)
        {
            Storage.VlaueClass vlaueClass;
            string _value_selected = "";
            _value_selected = ((StoragePanel.enum_ValueName)MainSelect).GetEnumName();
            vlaueClass = currentStorage.GetValue(Storage.GetValueName(_value_selected));
            int X = vlaueClass.Position.X;
            int Width = vlaueClass.Width;

            X = X + Width;

            for (int i = 0; i < List_ValueSelected.Count; i++)
            {
                _value_selected = ((StoragePanel.enum_ValueName)List_ValueSelected[i]).GetEnumName();
                vlaueClass = currentStorage.GetValue(Storage.GetValueName(_value_selected));
                CurrentStorage.SetValue(Storage.GetValueName(_value_selected), Storage.ValueType.Position, new Point(X - vlaueClass.Width, vlaueClass.Position.Y));
            }

            this.DrawToPictureBox();
        }
        private void Button_對齊靠左_Click(object sender, EventArgs e)
        {
            Storage.VlaueClass vlaueClass;
            string _value_selected = "";
            _value_selected = ((StoragePanel.enum_ValueName)MainSelect).GetEnumName();
            vlaueClass = currentStorage.GetValue(Storage.GetValueName(_value_selected));
            int X = vlaueClass.Position.X;
            int Y = vlaueClass.Position.Y;


            for (int i = 0; i < List_ValueSelected.Count; i++)
            {
                _value_selected = ((StoragePanel.enum_ValueName)List_ValueSelected[i]).GetEnumName();
                vlaueClass = currentStorage.GetValue(Storage.GetValueName(_value_selected));
                CurrentStorage.SetValue(Storage.GetValueName(_value_selected), Storage.ValueType.Position, new Point(X, vlaueClass.Position.Y));
            }

            this.DrawToPictureBox();
        }
        private void Button_對齊置中_Click(object sender, EventArgs e)
        {
            Storage.VlaueClass vlaueClass;
            string _value_selected = "";
            _value_selected = ((StoragePanel.enum_ValueName)MainSelect).GetEnumName();
            vlaueClass = currentStorage.GetValue(Storage.GetValueName(_value_selected));
            int X = vlaueClass.Position.X;
            int Width = vlaueClass.Width;
            X = X + (Width / 2);

            for (int i = 0; i < List_ValueSelected.Count; i++)
            {
                _value_selected = ((StoragePanel.enum_ValueName)List_ValueSelected[i]).GetEnumName();
                vlaueClass = currentStorage.GetValue(Storage.GetValueName(_value_selected));
                CurrentStorage.SetValue(Storage.GetValueName(_value_selected), Storage.ValueType.Position, new Point(X - (vlaueClass.Width / 2), vlaueClass.Position.Y));
            }

            this.DrawToPictureBox();
        }
        private void Button_對齊靠下_Click(object sender, EventArgs e)
        {
            Storage.VlaueClass vlaueClass;
            string _value_selected = "";
            _value_selected = ((StoragePanel.enum_ValueName)MainSelect).GetEnumName();
            vlaueClass = currentStorage.GetValue(Storage.GetValueName(_value_selected));
            int Y = vlaueClass.Position.Y;
            int Height = vlaueClass.Height;
            Y = Y + Height;
            for (int i = 0; i < List_ValueSelected.Count; i++)
            {
                _value_selected = ((StoragePanel.enum_ValueName)List_ValueSelected[i]).GetEnumName();
                vlaueClass = currentStorage.GetValue(Storage.GetValueName(_value_selected));
                CurrentStorage.SetValue(Storage.GetValueName(_value_selected), Storage.ValueType.Position, new Point(vlaueClass.Position.X, Y - vlaueClass.Height));
            }

            this.DrawToPictureBox();
        }
        private void Button_對齊靠上_Click(object sender, EventArgs e)
        {
            Storage.VlaueClass vlaueClass;
            string _value_selected = "";
            _value_selected = ((StoragePanel.enum_ValueName)MainSelect).GetEnumName();
            vlaueClass = currentStorage.GetValue(Storage.GetValueName(_value_selected));
            int Y = vlaueClass.Position.Y;
            int Height = vlaueClass.Height;

            for (int i = 0; i < List_ValueSelected.Count; i++)
            {
                _value_selected = ((StoragePanel.enum_ValueName)List_ValueSelected[i]).GetEnumName();
                vlaueClass = currentStorage.GetValue(Storage.GetValueName(_value_selected));
                CurrentStorage.SetValue(Storage.GetValueName(_value_selected), Storage.ValueType.Position, new Point(vlaueClass.Position.X, Y));
            }

            this.DrawToPictureBox();
        }

        private void RJ_RatioButton_自定義_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rJ_RatioButton_自定義.Checked)
            {
                currentStorage.Enum_drawType = Storage.enum_DrawType.constom;
                panel_選擇項目.Enabled = true;
                panel_字體.Enabled = true;
                panel_字體型態.Enabled = true;
                panel_邊框大小.Enabled = true;
                groupBox_對齊方式.Enabled = true;
                DrawToPictureBox();
            }
        }
        private void RJ_RatioButton_預設樣式1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.rJ_RatioButton_預設樣式1.Checked)
            {
                currentStorage.Enum_drawType = Storage.enum_DrawType.type1;
                panel_選擇項目.Enabled = true;
                panel_字體.Enabled = true;
                panel_字體型態.Enabled = true;
                panel_邊框大小.Enabled = true;
                groupBox_對齊方式.Enabled = true;
                DrawToPictureBox();
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
                Storage.VlaueClass vlaueClass = storage.GetValue(Storage.GetValueName(selectedText));
                Size size = TextRenderer.MeasureText(vlaueClass.Value, vlaueClass.Font);
                if (vlaueClass.valueName == Device.ValueName.圖片1)
                {
                    if (vlaueClass.Width < 20) vlaueClass.Width = 20;
                    if (vlaueClass.Height < 20) vlaueClass.Height = 20;
                }
                if (vlaueClass.valueName == Device.ValueName.圖片2)
                {
                    if (vlaueClass.Width < 20) vlaueClass.Width = 20;
                    if (vlaueClass.Height < 20) vlaueClass.Height = 20;
                }
                else if (vlaueClass.valueName == Device.ValueName.BarCode)
                {
                    if (vlaueClass.Width < 30) vlaueClass.Width = 30;
                    if (vlaueClass.Height < 20) vlaueClass.Height = 20;
                }
                else
                {
                    if (vlaueClass.Width < 50) vlaueClass.Width = 50;
                    if (vlaueClass.Height < 20) vlaueClass.Height = 20;
                }
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
            Bitmap bitmap = Communication.Storage_GetBitmap(storage, CanvasScale);
            using (Graphics g = Graphics.FromImage(bitmap))
            {

                for (int i = 0; i < List_ValueSelected.Count; i++)
                {
                    string _value_selected = ((StoragePanel.enum_ValueName)this.List_ValueSelected[i]).GetEnumName();

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
            bool flag_in_bottom_line = (mouse_Y >= end_Y - 5) && (mouse_Y <= end_Y + 5);

            bool flag_in_top_left_corner = flag_in_left_line && flag_in_top_line;
            bool flag_in_top_right_corner = flag_in_right_line && flag_in_top_line;
            bool flag_in_bottom_left_corner = flag_in_left_line && flag_in_bottom_line;
            bool flag_in_bottom_right_corner = flag_in_right_line && flag_in_bottom_line;

            if (flag_inside_X && flag_inside_Y)
            {
                return TxMouseDownType.INSIDE;
            }
            else if (flag_in_top_left_corner)
            {
                return TxMouseDownType.TOP_LEFT;
            }
            else if (flag_in_top_right_corner)
            {
                return TxMouseDownType.TOP_RIGHT;
            }
            else if (flag_in_bottom_left_corner)
            {
                return TxMouseDownType.BOTTOM_LEFT;
            }
            else if (flag_in_bottom_right_corner)
            {
                return TxMouseDownType.BOTTOM_RIGHT;
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
            else if (flag_in_bottom_line && flag_inside_X)
            {
                return TxMouseDownType.BOTTOM;
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
