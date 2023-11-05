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
using H_Pannel_lib;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Text.Json.Serialization;
using System.IO;
using System.Reflection;
using DrawingClass;
namespace EPD_Upload
{
    public partial class Form1 : Form
    {
        public enum enum_面板列表
        {
            GUID,
            藥碼,
            藥名,
        }
        public static string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        #region MyConfigClass 
        private static string MyConfigFileName = $@"{currentDirectory}\MyConfig.txt";
        public MyConfigClass myConfigClass = new MyConfigClass();
        public class MyConfigClass
        {
            private List<Storage> devices = new List<Storage>();

            public List<Storage> Devices { get => devices; set => devices = value; }
        }
        private void SaveMyConfig()
        {
            if (myConfigClass == null) myConfigClass = new MyConfigClass();
            string jsonstr = Basic.Net.JsonSerializationt<MyConfigClass>(myConfigClass, true);
            List<string> list_jsonstring = new List<string>();
            list_jsonstring.Add(jsonstr);
            MyFileStream.SaveFile($"{MyConfigFileName}", list_jsonstring);
            LoadMyConfig();
        }
        private void LoadMyConfig()
        {
            string jsonstr = MyFileStream.LoadFileAllText($"{MyConfigFileName}");
            if (jsonstr.StringIsEmpty())
            {
                jsonstr = Basic.Net.JsonSerializationt<MyConfigClass>(new MyConfigClass(), true);
                List<string> list_jsonstring = new List<string>();
                list_jsonstring.Add(jsonstr);
                if (!MyFileStream.SaveFile($"{MyConfigFileName}", list_jsonstring))
                {
                    MyMessageBox.ShowDialog($"建立{MyConfigFileName}檔案失敗!");
                }
            }
            else
            {
                myConfigClass = Basic.Net.JsonDeserializet<MyConfigClass>(jsonstr);

                jsonstr = Basic.Net.JsonSerializationt<MyConfigClass>(myConfigClass, true);
                List<string> list_jsonstring = new List<string>();
                list_jsonstring.Add(jsonstr);
                if (!MyFileStream.SaveFile($"{MyConfigFileName}", list_jsonstring))
                {
                    MyMessageBox.ShowDialog($"建立{MyConfigFileName}檔案失敗!");
                }

            }
            List<object[]> list_value = new List<object[]>();
            for(int i = 0; i < myConfigClass.Devices.Count; i++)
            {
                object[] value = new object[new enum_面板列表().GetLength()];
                value[(int)enum_面板列表.GUID] = myConfigClass.Devices[i].IP;
                value[(int)enum_面板列表.藥碼] = myConfigClass.Devices[i].Code;
                value[(int)enum_面板列表.藥名] = myConfigClass.Devices[i].Name;
                list_value.Add(value);
            }
            this.sqL_DataGridView_面板列表.RefreshGrid(list_value);
            // this.ftp_DounloadUI1.FTP_Server = myConfigClass.FTP_Server;
        }
        #endregion


        private MyThread MyThread_program = new MyThread();
        private MySerialPort mySerialPort = new MySerialPort();
        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            H_Pannel_lib.Communication.ConsoleWrite = true;
            mySerialPort.ConsoleWrite = false;
          
            MyMessageBox.form = this.FindForm();
            MyMessageBox.音效 = false;

            Dialog_藥碼輸入.form = this.FindForm();

            this.comboBox_PortName.Click += ComboBox_PortName_Click;
            ComboBox_PortName_Click(null, null);
            this.rJ_Button_連接.MouseDownEvent += RJ_Button_連接_MouseDownEvent;
            this.rJ_Button_上傳.MouseDownEvent += RJ_Button_上傳_MouseDownEvent;
            this.rJ_Button_存檔.MouseDownEvent += RJ_Button_存檔_MouseDownEvent;
            this.rJ_Button_重繪.MouseDownEvent += RJ_Button_重繪_MouseDownEvent;
            this.rJ_Button_面板列表_新增.MouseDownEvent += RJ_Button_面板列表_新增_MouseDownEvent;
            this.rJ_Button_面板列表_刪除.MouseDownEvent += RJ_Button_面板列表_刪除_MouseDownEvent;

            this.button_面板內容_字體_藥碼.Click += Button_面板內容_字體_藥碼_Click;
            this.button_面板內容_字體_包裝單位.Click += Button_面板內容_字體_包裝單位_Click;
            this.button_面板內容_字體_藥名.Click += Button_面板內容_字體_藥名_Click;
            this.button_面板內容_字體_商品名.Click += Button_面板內容_字體_商品名_Click;
            this.button_面板內容_字體_中文名.Click += Button_面板內容_字體_中文名_Click;

            SQLUI.Table table = new SQLUI.Table("");
            table.AddColumnList("GUID", SQLUI.Table.StringType.CHAR, SQLUI.Table.IndexType.None);
            table.AddColumnList("藥碼", SQLUI.Table.StringType.CHAR, SQLUI.Table.IndexType.None);
            table.AddColumnList("藥名", SQLUI.Table.StringType.CHAR, SQLUI.Table.IndexType.None);
            sqL_DataGridView_面板列表.Init(table);
            sqL_DataGridView_面板列表.Set_ColumnVisible(false, "GUID");
            sqL_DataGridView_面板列表.Set_ColumnWidth(80, DataGridViewContentAlignment.MiddleCenter, "藥碼");
            sqL_DataGridView_面板列表.Set_ColumnWidth(350, DataGridViewContentAlignment.MiddleLeft, "藥名");
            sqL_DataGridView_面板列表.RowDoubleClickEvent += SqL_DataGridView_面板列表_RowDoubleClickEvent;

            LoadMyConfig();
            MyThread_program.Add_Method(sub_program);
            MyThread_program.AutoRun(true);
            MyThread_program.SetSleepTime(100);
            MyThread_program.Trigger();
        }



        private void sub_program()
        {

        }
        private Bitmap Get_Bitmap()
        {
            int Pannel_Width = 296;
            int Pannel_Height = 128;
            Bitmap bitmap = new Bitmap(Pannel_Width, Pannel_Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.HighQuality; //使繪圖質量最高，即消除鋸齒
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

                Rectangle rect = new Rectangle(0, 0, Pannel_Width, Pannel_Height);
                g.FillRectangle(new SolidBrush(Color.Red), rect);
            }
            return bitmap;
        }
        private Bitmap Get_Bitmap(Storage storage)
        {
            Bitmap bitmap = new Bitmap(296, 128);
            int Pannel_Width = bitmap.Width;
            int Pannel_Height = bitmap.Height;
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.HighQuality; //使繪圖質量最高，即消除鋸齒
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

                Rectangle rect = new Rectangle(0, 0, Pannel_Width, Pannel_Height);
                int Line_Height = (Pannel_Height / 3) * 2;
                g.FillRectangle(new SolidBrush(storage.BackColor), rect);

                //this.Graphics_Draw_Bitmap.DrawLine(new Pen(storage.ForeColor, 2), new Point(0, Line_Height), new Point(Pannel_Width, Line_Height));

                if (storage.BarCode_Height > 40) storage.BarCode_Height = 40;
                if (storage.BarCode_Width > 120) storage.BarCode_Width = 120;

                //storage.SetValue(Storage.ValueName.藥品碼, Storage.ValueType.ForeColor, storage.IsWarning ? Color.White : Color.Black);
                storage.SetValue(Storage.ValueName.藥品碼, Storage.ValueType.BackColor, storage.BackColor);

                //storage.SetValue(Storage.ValueName.藥品名稱, Storage.ValueType.ForeColor, storage.IsWarning ? Color.White : Color.Black);
                storage.SetValue(Storage.ValueName.藥品名稱, Storage.ValueType.BackColor, storage.BackColor);

                //storage.SetValue(Storage.ValueName.藥品學名, Storage.ValueType.ForeColor, storage.IsWarning ? Color.White : Color.Black);
                storage.SetValue(Storage.ValueName.藥品學名, Storage.ValueType.BackColor, storage.BackColor);

                //storage.SetValue(Storage.ValueName.藥品中文名稱, Storage.ValueType.ForeColor, storage.IsWarning ? Color.White : Color.Black);
                storage.SetValue(Storage.ValueName.藥品中文名稱, Storage.ValueType.BackColor, storage.BackColor);

                //storage.SetValue(Storage.ValueName.包裝單位, Storage.ValueType.ForeColor, storage.IsWarning ? Color.White : Color.Black);
                storage.SetValue(Storage.ValueName.包裝單位, Storage.ValueType.BackColor, storage.BackColor);

                //storage.SetValue(Storage.ValueName.效期, Storage.ValueType.ForeColor, storage.IsWarning ? Color.White : Color.Black);
                storage.SetValue(Storage.ValueName.效期, Storage.ValueType.BackColor, storage.BackColor);

                //storage.SetValue(Storage.ValueName.庫存, Storage.ValueType.ForeColor, storage.IsWarning ? Color.White : Color.Black);
                storage.SetValue(Storage.ValueName.庫存, Storage.ValueType.BackColor, storage.BackColor);

                float posy = 0;

                if ((storage.DRUGKIND.StringIsEmpty() == false && storage.DRUGKIND != "N") || storage.IsAnesthetic || storage.IsShapeSimilar || storage.IsSoundSimilar)
                {
                    int temp_x = 0;
                    g.FillRectangle(new SolidBrush(Color.White), new Rectangle(0, 0, Pannel_Width, 30));
                    if ((storage.DRUGKIND.StringIsEmpty() == false && storage.DRUGKIND != "N"))
                    {
                        Communication.DrawHexagonText(g, new Point(temp_x, 0), 30, storage.DRUGKIND, new Font("Arial", 14), Color.White, Color.Black, Color.Red);
                        temp_x += 40;
                    }
                    if (storage.IsAnesthetic)
                    {
                        Communication.DrawCircleText(g, new Point(temp_x, 0), 30, "麻", new Font("Arial", 14), Color.White, Color.Black, Color.Red);
                        temp_x += 40;
                    }
                    if (storage.IsShapeSimilar)
                    {
                        Communication.DrawSquareText(g, new Point(temp_x, 0), 30, "形", new Font("Arial", 14), Color.Black, Color.Black, Color.White);
                        temp_x += 40;
                    }
                    if (storage.IsSoundSimilar)
                    {
                        Communication.DrawSquareText(g, new Point(temp_x, 0), 30, "音", new Font("Arial", 14), Color.Black, Color.Black, Color.White);
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
                    Communication.DrawStorageString(g, storage, Storage.ValueName.包裝單位, 0, Pannel_Height - size_Code_font.Height - size_Package_font.Height);
                }

            }
      
            return bitmap;
        }
        private void LoadDevice(Storage device)
        {
            textBox_面板內容_GUID.Text = device.IP;
            textBox_面板內容_藥碼.Text = device.Code;
            textBox_面板內容_藥名.Text = device.Name;
            textBox_面板內容_包裝單位.Text = device.Package;
            textBox_面板內容_商品名.Text = device.Scientific_Name;
            textBox_面板內容_中文名.Text = device.ChineseName;

            comboBox_面板內容_字體顏色_藥碼.Text = GetForeColorStr(device.Code_ForeColor);
            comboBox_面板內容_字體顏色_包裝單位.Text = GetForeColorStr(device.Package_ForeColor);
            comboBox_面板內容_字體顏色_藥名.Text = GetForeColorStr(device.Name_ForeColor);
            comboBox_面板內容_字體顏色_商品名.Text = GetForeColorStr(device.Scientific_Name_ForeColor);
            comboBox_面板內容_字體顏色_中文名.Text = GetForeColorStr(device.ChineseName_ForeColor);
            comboBox_面板內容_背景顏色.Text = GetForeColorStr(device.BackColor);

        }
        private string GetForeColorStr(Color color)
        {
            if(color.ToColorString() == Color.Red.ToColorString())
            {
                return "紅";
            }
            if (color.ToColorString() == Color.White.ToColorString())
            {
                return "白";
            }
            return "黑";
        }
        private Color GetForeColor(string str)
        {
            if (str == "紅")
            {
                return Color.Red;
            }
            if (str == "白")
            {
                return Color.White;
            }
            return Color.Black;
        }
        #region Event
        private void SqL_DataGridView_面板列表_RowDoubleClickEvent(object[] RowValue)
        {
            string GUID = RowValue[(int)enum_面板列表.GUID].ObjectToString();
            Storage device = myConfigClass.Devices.SortByIP(GUID);
            if(device != null)
            {
                LoadDevice(device);
                RJ_Button_重繪_MouseDownEvent(null);
                this.rJ_Button_存檔.Enabled = true;
                this.rJ_Button_重繪.Enabled = true;
                this.rJ_Button_上傳.Enabled = true;
            }
        }
        private void RJ_Button_面板列表_新增_MouseDownEvent(MouseEventArgs mevent)
        {
            Dialog_藥碼輸入 dialog_藥碼輸入 = new Dialog_藥碼輸入();
            if(dialog_藥碼輸入.ShowDialog() == DialogResult.Yes)
            {
                List<Storage> devices = myConfigClass.Devices.SortByCode(dialog_藥碼輸入.Value);
                if (devices.Count > 0)
                {
                    MyMessageBox.ShowDialog("藥碼已建立資料!");
                    return;
                }
                Storage device = new Storage();
                device.Code = dialog_藥碼輸入.Value;
                device.IP = Guid.NewGuid().ToString();
                myConfigClass.Devices.Add_NewStorage(device);
                SaveMyConfig();
            }
        }
        private void RJ_Button_面板列表_刪除_MouseDownEvent(MouseEventArgs mevent)
        {
            List<object[]> list_value = this.sqL_DataGridView_面板列表.Get_All_Select_RowsValues();
            if (list_value.Count == 0)
            {
                MyMessageBox.ShowDialog("未選取資料!");
                return;
            }
            for(int i = 0; i < list_value.Count; i++)
            {
                string GUID = list_value[i][(int)enum_面板列表.GUID].ObjectToString();
                myConfigClass.Devices = myConfigClass.Devices.RemoveByIP(GUID);
            }
            SaveMyConfig();
            MyMessageBox.ShowDialog("刪除成功!");
            return;
        }

        private void ComboBox_PortName_Click(object sender, EventArgs e)
        {
            string[] portNames = MySerialPort.GetPortNames();
            this.comboBox_PortName.DataSource = portNames;
        }
        private void RJ_Button_連接_MouseDownEvent(MouseEventArgs mevent)
        {
            try
            {
                bool flag_return = false;
                this.Invoke(new Action(delegate
                {
                    if (this.comboBox_PortName.SelectedItem == null)
                    {
                        MyMessageBox.ShowDialog("未選擇通訊Port!");
                        flag_return = true;
                    }
                }));
                if (flag_return)
                {
                    return;
                }
                this.Invoke(new Action(delegate
                {
                    mySerialPort.PortName = this.comboBox_PortName.SelectedItem.ToString();
                }));
                mySerialPort.Init(mySerialPort.PortName, 115200, 8, System.IO.Ports.Parity.None, System.IO.Ports.StopBits.One, false);
                if (mySerialPort.SerialPortOpen() == true)
                {
            
                    this.Invoke(new Action(delegate
                    {
                        this.label_狀態.Text = "已連接";
                        this.rJ_Button_連接.Enabled = false;
                        this.comboBox_PortName.Enabled = false;
                    

                    }));

                }
                else
                {
                    MyMessageBox.ShowDialog("連接失敗!");
                }
            }
            catch
            {

            }
            finally
            {
           
            }
        }
        private void RJ_Button_上傳_MouseDownEvent(MouseEventArgs mevent)
        {
            try
            {
                if (this.label_狀態.Text != "已連接")
                {
                    MyMessageBox.ShowDialog("請先連結通訊!");
                    return;
                }
                string GUID = textBox_面板內容_GUID.Text;
                if (GUID.StringIsEmpty())
                {
                    MyMessageBox.ShowDialog("未選取資料!");
                    return;
                }
                Storage device = myConfigClass.Devices.SortByIP(GUID);

                H_Pannel_lib.Communication.ProcessBarEvent += Communication_ProcessBarEvent;
                bool flag_OK = H_Pannel_lib.Communication.UART_EPD_290_DrawImage(mySerialPort, Get_Bitmap(device));
                H_Pannel_lib.Communication.ProcessBarEvent -= Communication_ProcessBarEvent;
                if (!flag_OK) MyMessageBox.ShowDialog("上傳失敗!");
            }
            catch
            {

            }
            finally
            {
                this.Invoke(new Action(delegate
                {
                    this.rJ_ProgressBar_上傳狀態.Maximum = 1;
                    this.rJ_ProgressBar_上傳狀態.Value = 1;
                    this.label_上傳狀態.Text = "上傳完成";
                    Application.DoEvents();
                }));
            }
  
        }
        private void RJ_Button_存檔_MouseDownEvent(MouseEventArgs mevent)
        {
            string GUID = textBox_面板內容_GUID.Text;
            Storage device = myConfigClass.Devices.SortByIP(GUID);
            if (device != null)
            {
                device.Name = textBox_面板內容_藥名.Text;
                device.Package = textBox_面板內容_包裝單位.Text;
                device.Scientific_Name = textBox_面板內容_商品名.Text;
                device.ChineseName = textBox_面板內容_中文名.Text;
                this.Invoke(new Action(delegate 
                {
                    device.Code_ForeColor = GetForeColor(comboBox_面板內容_字體顏色_藥碼.Text);
                    device.Package_ForeColor = GetForeColor(comboBox_面板內容_字體顏色_包裝單位.Text);
                    device.Name_ForeColor = GetForeColor(comboBox_面板內容_字體顏色_藥名.Text);
                    device.Scientific_Name_ForeColor = GetForeColor(comboBox_面板內容_字體顏色_商品名.Text);
                    device.ChineseName_ForeColor = GetForeColor(comboBox_面板內容_字體顏色_中文名.Text);

                    device.BackColor = GetForeColor(comboBox_面板內容_背景顏色.Text);
                }));
       
                SaveMyConfig();
            }
            this.SaveMyConfig();
            //MyMessageBox.ShowDialog($"存檔完成!");
        }
        private void RJ_Button_重繪_MouseDownEvent(MouseEventArgs mevent)
        {
            string GUID = textBox_面板內容_GUID.Text;
            if (GUID.StringIsEmpty())
            {
                MyMessageBox.ShowDialog("未選取資料!");
                return;
            }
            RJ_Button_存檔_MouseDownEvent(null);
            Storage device = myConfigClass.Devices.SortByIP(GUID);
            if (device != null)
            {
                using (Bitmap bitmap = this.Get_Bitmap(device))
                {
                    using (Bitmap bitmap_scale = Communication.ScaleImage(bitmap, panel_EPD290.Width, panel_EPD290.Height))
                    {
                        using (Graphics g = panel_EPD290.CreateGraphics())
                        {
                            g.DrawImage(bitmap_scale, new PointF());
                        }
                    }
                }
            }
 
        }
        private void Communication_ProcessBarEvent(int value, int max, string info)
        {
            this.Invoke(new Action(delegate
            {
                this.rJ_ProgressBar_上傳狀態.Maximum = max;
                this.rJ_ProgressBar_上傳狀態.Value = value;
                this.label_上傳狀態.Text = info;
                Application.DoEvents();
            }));

        }
        private void Button_面板內容_字體_中文名_Click(object sender, EventArgs e)
        {
            string GUID = textBox_面板內容_GUID.Text;
            if (GUID.StringIsEmpty())
            {
                MyMessageBox.ShowDialog("未選取資料!");
                return;
            }
            Storage device = myConfigClass.Devices.SortByIP(GUID);
            if (device == null) return;
            fontDialog.Font = device.Code_font;
            if(fontDialog.ShowDialog() == DialogResult.OK)
            {
                device.Code_font = fontDialog.Font;
            }
        }
        private void Button_面板內容_字體_商品名_Click(object sender, EventArgs e)
        {
            string GUID = textBox_面板內容_GUID.Text;
            if (GUID.StringIsEmpty())
            {
                MyMessageBox.ShowDialog("未選取資料!");
                return;
            }
            Storage device = myConfigClass.Devices.SortByIP(GUID);
            if (device == null) return;
            fontDialog.Font = device.Code_font;
            if (fontDialog.ShowDialog() == DialogResult.OK)
            {
                device.Scientific_Name_font = fontDialog.Font;
            }

        }
        private void Button_面板內容_字體_藥名_Click(object sender, EventArgs e)
        {
            string GUID = textBox_面板內容_GUID.Text;
            if (GUID.StringIsEmpty())
            {
                MyMessageBox.ShowDialog("未選取資料!");
                return;
            }
            Storage device = myConfigClass.Devices.SortByIP(GUID);
            if (device == null) return;
            fontDialog.Font = device.Code_font;
            if (fontDialog.ShowDialog() == DialogResult.OK)
            {
                device.Name_font = fontDialog.Font;
            }
        }
        private void Button_面板內容_字體_包裝單位_Click(object sender, EventArgs e)
        {
            string GUID = textBox_面板內容_GUID.Text;
            if (GUID.StringIsEmpty())
            {
                MyMessageBox.ShowDialog("未選取資料!");
                return;
            }
            Storage device = myConfigClass.Devices.SortByIP(GUID);
            if (device == null) return;
            fontDialog.Font = device.Code_font;
            if (fontDialog.ShowDialog() == DialogResult.OK)
            {
                device.Package_font = fontDialog.Font;
            }
        }
        private void Button_面板內容_字體_藥碼_Click(object sender, EventArgs e)
        {
            string GUID = textBox_面板內容_GUID.Text;
            if (GUID.StringIsEmpty())
            {
                MyMessageBox.ShowDialog("未選取資料!");
                return;
            }
            Storage device = myConfigClass.Devices.SortByIP(GUID);
            if (device == null) return;
            fontDialog.Font = device.Code_font;
            if (fontDialog.ShowDialog() == DialogResult.OK)
            {
                device.Code_font = fontDialog.Font;
            }
        }
        #endregion
    }
}
