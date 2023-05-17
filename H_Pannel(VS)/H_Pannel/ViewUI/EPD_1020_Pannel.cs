using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.ComponentModel;
using System.Net;
using Basic;
using MyUI;
namespace H_Pannel_lib
{
    public partial class EPD_1020_Pannel : UserControl
    {
        public delegate void MouseDownEventHandler(List<Box> Boxes);
        public event MouseDownEventHandler MouseDownEvent;
        public delegate void MouseDoubleClickEventHandler(List<Box> Boxes);
        public event MouseDoubleClickEventHandler MouseDoubleClickEvent;
        public delegate void DrawerChangeEventHandler(Drawer drawer);
        public event DrawerChangeEventHandler DrawerChangeEvent;


        private enum ContextMenuStrip_Main
        {
            SaveToBMP,
            SaveToJPG,
            合併儲位,
            分割儲位,
            面板亮燈,
            儲位亮燈,         
            全部亮燈測試,
            清除燈號,
            上傳資料,
        }
        [ReadOnly(false), Browsable(false)]
        public Drawer CurrentDrawer
        {
            get
            {
                if (this.DesignMode) return null;
                return currentDrawer;
            }
            set
            {
                if (this.DesignMode) return;
                currentDrawer = value;
            }
        }

        private const int Selected_Box_PenWidth = 5;
        private Drawer currentDrawer = new Drawer();
        private List<int> Select_Column = new List<int>();
        private List<int> Select_Row = new List<int>();
        private bool flag_pictureBox_MouseDown = false;
        private int pictureBox_MouseDown_X = 0;
        private int pictureBox_MouseDown_Y = 0;
        private List<UDP_Class> List_UDP_Local;

        public EPD_1020_Pannel()
        {
            InitializeComponent();
        }

        private void EPD_1020_Pannel_Load(object sender, EventArgs e)
        {
     

        }
        public void Init(List<UDP_Class> List_UDP_Local)
        {
            this.List_UDP_Local = List_UDP_Local;


            // Basic.Reflection.MakeDoubleBuffered(this.pictureBox, true);
            this.pictureBox.MouseDown += PictureBox_MouseDown;
            this.pictureBox.MouseMove += PictureBox_MouseMove;
            this.pictureBox.MouseUp += PictureBox_MouseUp;
            this.pictureBox.MouseDoubleClick += PictureBox_MouseDoubleClick;
            this.pictureBox.Paint += PictureBox_Paint;
        }



        public void CombineBoxes()
        {
            int col;
            int row;
            if (this.Select_Column.Count <= 1) return;
            List<int[]> list_Col_Row = new List<int[]>();

            for (int i = 0; i < this.Select_Column.Count; i++)
            {
                list_Col_Row.Add(new int[] { Select_Column[i], Select_Row[i] });
            }
            list_Col_Row.Sort(new IcpCol());


            for (int i = 0; i < list_Col_Row.Count; i++)
            {
                col = list_Col_Row[i][0];
                row = list_Col_Row[i][1];
                if (i == 0)
                {
                    this.CurrentDrawer.Boxes[col][row].Slave = false;
                    this.CurrentDrawer.Boxes[col][row].SlaveBox_Column = list_Col_Row[i + 1][0];
                    this.CurrentDrawer.Boxes[col][row].SlaveBox_Row = list_Col_Row[i + 1][1];
                    this.CurrentDrawer.Boxes[col][row].MasterBox_Column = -1;
                    this.CurrentDrawer.Boxes[col][row].MasterBox_Row = -1;
                }
                else if (i == this.Select_Column.Count - 1)
                {
                    this.CurrentDrawer.Boxes[col][row].Slave = true;
                    this.CurrentDrawer.Boxes[col][row].SlaveBox_Column = -1;
                    this.CurrentDrawer.Boxes[col][row].SlaveBox_Row = -1;
                    this.CurrentDrawer.Boxes[col][row].MasterBox_Column = list_Col_Row[0][0];
                    this.CurrentDrawer.Boxes[col][row].MasterBox_Row = list_Col_Row[0][1];
                }
                else
                {
                    this.CurrentDrawer.Boxes[col][row].Slave = true;
                    this.CurrentDrawer.Boxes[col][row].SlaveBox_Column = list_Col_Row[i + 1][0];
                    this.CurrentDrawer.Boxes[col][row].SlaveBox_Row = list_Col_Row[i + 1][1];
                    this.CurrentDrawer.Boxes[col][row].MasterBox_Column = list_Col_Row[0][0];
                    this.CurrentDrawer.Boxes[col][row].MasterBox_Row = list_Col_Row[0][1];
                }
                this.CurrentDrawer.Boxes[col][row].Name = "";
                this.CurrentDrawer.Boxes[col][row].Code = "";
                this.CurrentDrawer.Boxes[col][row].Clear();
            }
            this.DrawToPictureBox(this.CurrentDrawer);
            if (DrawerChangeEvent != null) DrawerChangeEvent(CurrentDrawer);
        }
        public void SeparateBoxes()
        {
            int col;
            int row;
            if (this.Select_Column.Count <= 1) return;

            List<int[]> list_Col_Row = new List<int[]>();

            for (int i = 0; i < this.Select_Column.Count; i++)
            {
                list_Col_Row.Add(new int[] { Select_Column[i], Select_Row[i] });
            }
            list_Col_Row.Sort(new IcpCol());


            for (int i = 0; i < list_Col_Row.Count; i++)
            {
                col = list_Col_Row[i][0];
                row = list_Col_Row[i][1];
                this.CurrentDrawer.Boxes[col][row].Slave = false;
                this.CurrentDrawer.Boxes[col][row].SlaveBox_Column = -1;
                this.CurrentDrawer.Boxes[col][row].SlaveBox_Row = -1;
                this.CurrentDrawer.Boxes[col][row].MasterBox_Column = -1;
                this.CurrentDrawer.Boxes[col][row].MasterBox_Row = -1;
                this.CurrentDrawer.Boxes[col][row].Name = "";
                this.CurrentDrawer.Boxes[col][row].Code = "";
                this.CurrentDrawer.Boxes[col][row].Clear();
            }
            this.DrawToPictureBox(this.CurrentDrawer);
            if (DrawerChangeEvent != null) DrawerChangeEvent(CurrentDrawer);
        }
        public void ClearBoxes()
        {
            int col;
            int row;
            if (this.Select_Column.Count < 1) return;

            List<int[]> list_Col_Row = new List<int[]>();

            for (int i = 0; i < this.Select_Column.Count; i++)
            {
                list_Col_Row.Add(new int[] { Select_Column[i], Select_Row[i] });
            }
            list_Col_Row.Sort(new IcpCol());


            for (int i = 0; i < list_Col_Row.Count; i++)
            {
                col = list_Col_Row[i][0];
                row = list_Col_Row[i][1];
                this.CurrentDrawer.Boxes[col][row].Clear();
            }
            this.DrawToPictureBox(this.CurrentDrawer);
            if (DrawerChangeEvent != null) DrawerChangeEvent(CurrentDrawer);
        }
        public void InitBoxes()
        {
            bool flag_雙數 = false;
            for (int c = 0; c < this.CurrentDrawer.Boxes.Count; c++)
            {
                for (int r = 0; r < this.CurrentDrawer.Boxes[c].Length; r++)
                {
                    this.CurrentDrawer.Boxes[c][r].Clear();
                }
            }
            for (int c = 0; c < this.CurrentDrawer.Boxes.Count; c++)
            {
                for (int r = 0; r < this.CurrentDrawer.Boxes[c].Length; r++)
                {
                    flag_雙數 = ((r % 2) == 0);
                    if (flag_雙數)
                    {
                        this.CurrentDrawer.Boxes[c][r].Slave = false;
                        this.CurrentDrawer.Boxes[c][r].MasterBox_Column = -1;
                        this.CurrentDrawer.Boxes[c][r].MasterBox_Row = -1;
                        this.CurrentDrawer.Boxes[c][r].SlaveBox_Column = c;
                        this.CurrentDrawer.Boxes[c][r].SlaveBox_Row = r + 1;
                    }
                    else
                    {
                        this.CurrentDrawer.Boxes[c][r].Slave = true;
                        this.CurrentDrawer.Boxes[c][r].MasterBox_Column = c;
                        this.CurrentDrawer.Boxes[c][r].MasterBox_Row = r - 1;
                        this.CurrentDrawer.Boxes[c][r].SlaveBox_Column = -1;
                        this.CurrentDrawer.Boxes[c][r].SlaveBox_Row = -1;

                    }
                }
            }
            this.DrawToPictureBox(this.CurrentDrawer);
            if (DrawerChangeEvent != null) DrawerChangeEvent(CurrentDrawer);
        }
        public void SetSelectBox(int col, int row)
        {
            this.SetSelectBox(this.currentDrawer, col, row);
        }
        public void SetSelectBox(Drawer drawer, int col, int row)
        {
            for (int i = 0; i < drawer.Boxes.Count; i++)
            {
                if (row >= drawer.Boxes[i].Length) return;
            }
            DrawToPictureBox(drawer);
            Rectangle rect = this.Get_Box_Combine(CurrentDrawer.Boxes[col][row]);
            using (Graphics g = pictureBox.CreateGraphics())
            {
                g.DrawRectangle(new Pen(Color.Blue, Selected_Box_PenWidth), rect);
            }
             
            this.Select_Column.Clear();
            this.Select_Row.Clear();
            this.Select_Column.Add(col);
            this.Select_Row.Add(row);
            if (this.MouseDownEvent != null) this.MouseDownEvent(this.GetSelectBoxes());
        }

        public void BoxLightOn(Color color)
        {
            List<Task> taskList = new List<Task>();
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(CurrentDrawer.Port);
            List<int> cols = new List<int>();
            List<int> rows = new List<int>();
            int index = this.GetSelectBoxes(ref cols, ref rows);
            if (index == 0) return;
            taskList.Add(Task.Run(() =>
            {
                CurrentDrawer.LED_Bytes = DrawerUI_EPD_1020.Get_Drawer_LEDBytes_UDP(uDP_Class, CurrentDrawer.IP);
                DrawerUI_EPD_1020.Set_Drawer_LED_UDP(uDP_Class, CurrentDrawer, cols, rows, color);
            }));
            Task allTask = Task.WhenAll(taskList);
        }
        public void BoxLightOff()
        {
            List<Task> taskList = new List<Task>();
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(CurrentDrawer.Port);
            List<int> cols = new List<int>();
            List<int> rows = new List<int>();
            int index = this.GetSelectBoxes(ref cols, ref rows);
            if (index == 0) return;
            taskList.Add(Task.Run(() =>
            {
                CurrentDrawer.LED_Bytes = DrawerUI_EPD_1020.Get_Drawer_LEDBytes_UDP(uDP_Class, CurrentDrawer.IP);
                DrawerUI_EPD_1020.Set_Drawer_LED_UDP(uDP_Class, CurrentDrawer, cols, rows, Color.Black);
            }));
            Task allTask = Task.WhenAll(taskList);
        }
        public void AllLightOff()
        {
            List<Task> taskList = new List<Task>();
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(CurrentDrawer.Port);
            taskList.Add(Task.Run(() =>
            {
                DrawerUI_EPD_1020.Set_LED_Clear_UDP(uDP_Class, CurrentDrawer);
            }));
            Task allTask = Task.WhenAll(taskList);
        }
        public void AllLightOn(Color color)
        {
            List<Task> taskList = new List<Task>();

            UDP_Class uDP_Class = List_UDP_Local.SortByPort(CurrentDrawer.Port);
            taskList.Add(Task.Run(() =>
            {
                DrawerUI_EPD_1020.Set_LED_UDP(uDP_Class, CurrentDrawer, color);
            }));
            Task allTask = Task.WhenAll(taskList);
        }
        public void PannelLightOn(Color color)
        {
            List<Task> taskList = new List<Task>();
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(CurrentDrawer.Port);
            taskList.Add(Task.Run(() =>
            {
                CurrentDrawer.LED_Bytes = DrawerUI_EPD_1020.Get_Drawer_LEDBytes_UDP(uDP_Class, CurrentDrawer.IP);
                DrawerUI_EPD_1020.Set_Pannel_LED_UDP(uDP_Class, CurrentDrawer, color);
            }));
            Task allTask = Task.WhenAll(taskList);
        }
        public void PannelLightOff()
        {
            List<Task> taskList = new List<Task>();

            UDP_Class uDP_Class = List_UDP_Local.SortByPort(CurrentDrawer.Port);
            taskList.Add(Task.Run(() =>
            {
                CurrentDrawer.LED_Bytes = DrawerUI_EPD_1020.Get_Drawer_LEDBytes_UDP(uDP_Class, CurrentDrawer.IP);
                DrawerUI_EPD_1020.Set_Pannel_LED_UDP(uDP_Class, CurrentDrawer, Color.Black);
            }));
            Task allTask = Task.WhenAll(taskList);
        }
        virtual public Bitmap Get_Drawer_bmp(Drawer drawer)
        {
            return DrawerUI_EPD_1020.Get_Drawer_bmp(drawer);
        }
        virtual public Bitmap Get_Drawer_Barcode_bmp(Drawer drawer)
        {
            return DrawerUI_EPD_1020.Get_Drawer_Barcode_bmp(drawer);
        }
        async private void PictureBox_Paint(object sender, PaintEventArgs e)
        {
            Task task = Task.Factory.StartNew(new Action(delegate
            {
                this.DrawToPictureBox();
            }));
            await task;
        }

        public void DrawToPictureBox()
        {
            this.DrawToPictureBox(this.CurrentDrawer);
        }

        public void DrawBarCodeToPictureBox(Drawer drawer)
        {
            if (drawer == null) return;
            this.CurrentDrawer = drawer;
            using (Bitmap bitmap = this.Get_Drawer_Barcode_bmp(drawer))
            {
                using (Graphics g = pictureBox.CreateGraphics())
                {
                    g.DrawImage(bitmap, new PointF());
                }
            }

        }
        public void DrawToPictureBox(Drawer drawer)
        {
            if (drawer == null) return;
            this.CurrentDrawer = drawer;
            using(Bitmap bitmap = this.Get_Drawer_bmp(drawer))
            {
                using (Graphics g = pictureBox.CreateGraphics())
                {
                    g.DrawImage(bitmap, new PointF());
                }
            }

        }
        public void DrawToPictureBox(Bitmap bitmap)
        {
            using (Graphics g = pictureBox.CreateGraphics())
            {
                g.DrawImage(bitmap, new PointF());
            }

        }
        public List<Box> GetSelectBoxes()
        {
            List<Box> list_boxes = new List<Box>();
            for (int i = 0; i < this.Select_Column.Count; i++)
            {
                if (!this.CurrentDrawer.Boxes[this.Select_Column[i]][this.Select_Row[i]].Slave)
                {
                    list_boxes.Add(this.CurrentDrawer.Boxes[this.Select_Column[i]][this.Select_Row[i]]);
                }
            }
            return list_boxes;
        }
        public int GetSelectBoxes(ref List<int> col, ref List<int> row)
        {
            int index = 0;
            for (int i = 0; i < this.Select_Column.Count; i++)
            {
                if (!this.CurrentDrawer.Boxes[this.Select_Column[i]][this.Select_Row[i]].Slave)
                {
                    col.Add(this.CurrentDrawer.Boxes[this.Select_Column[i]][this.Select_Row[i]].Column);
                    row.Add(this.CurrentDrawer.Boxes[this.Select_Column[i]][this.Select_Row[i]].Row);
                    index++;
                }

            }
            return index;
        }

        private Rectangle Get_Box_Combine(Box box)
        {
            return DrawerUI_EPD_1020.Get_Box_Combine(currentDrawer, box);
        }
        private bool Check_Mouse_In_Box(int x, int y, Box box)
        {
            if ((x > box.X) && (x < box.X + box.Width))
            {
                if ((y > box.Y) && (y < box.Y + box.Height))
                {
                    return true;
                }
            }
            return false;
        }
       
        private class IcpCol : IComparer<int[]>
        {
            public int Compare(int[] x, int[] y)
            {
                return (x[0] * 10 + x[1]).CompareTo((y[0] * 10 + y[1]));
            }
        }
        private class IcpRow : IComparer<int[]>
        {
            public int Compare(int[] x, int[] y)
            {
                return x[1].CompareTo(y[1]);
            }
        }
        #region Envet
  
        private void PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                this.flag_pictureBox_MouseDown = true;
                this.Select_Column.Clear();
                this.Select_Row.Clear();

                this.pictureBox_MouseDown_X = e.X;
                this.pictureBox_MouseDown_Y = e.Y;

                this.pictureBox_Select_Check(e.X, e.Y);
                if (this.MouseDownEvent != null) this.MouseDownEvent(this.GetSelectBoxes());
            }
            else if(e.Button == MouseButtons.Right)
            {
                Dialog_ContextMenuStrip dialog_ContextMenuStrip = new Dialog_ContextMenuStrip(new ContextMenuStrip_Main().GetEnumNames());
                if (dialog_ContextMenuStrip.ShowDialog() == DialogResult.Yes)
                {
                    if (dialog_ContextMenuStrip.Value == ContextMenuStrip_Main.合併儲位.GetEnumName())
                    {
                        this.CombineBoxes();
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_Main.SaveToBMP.GetEnumName())
                    {
                        if (this.saveFileDialog_bmp.ShowDialog() != DialogResult.OK)
                        {
                            PictureBox_MouseDown(sender, e);
                            return;
                        }
                        using (Bitmap bitmap = this.Get_Drawer_bmp(CurrentDrawer))
                        {
                            bitmap.Save(this.saveFileDialog_bmp.FileName);
                        }

                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_Main.SaveToJPG.GetEnumName())
                    {
                        if (this.saveFileDialog_jpg.ShowDialog() != DialogResult.OK)
                        {
                            PictureBox_MouseDown(sender, e);
                            return;
                        }
                        using (Bitmap bitmap = this.Get_Drawer_bmp(CurrentDrawer))
                        {
                            ((Image)bitmap).SaveJpeg(this.saveFileDialog_jpg.FileName, 90);
                        }

                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_Main.分割儲位.GetEnumName())
                    {
                        this.SeparateBoxes();
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_Main.面板亮燈.GetEnumName())
                    {
                        List<Task> taskList = new List<Task>();
                        ColorDialog colorDialog = new ColorDialog();
                        if (colorDialog.ShowDialog() != DialogResult.OK) return;
                        Color color = colorDialog.Color;
                        UDP_Class uDP_Class = List_UDP_Local.SortByPort(CurrentDrawer.Port);
                        taskList.Add(Task.Run(() =>
                        {
                            CurrentDrawer.LED_Bytes = DrawerUI_EPD_1020.Get_Drawer_LEDBytes_UDP(uDP_Class, CurrentDrawer.IP);
                            DrawerUI_EPD_1020.Set_Pannel_LED_UDP(uDP_Class, CurrentDrawer, color);
                        }));
                        Task allTask = Task.WhenAll(taskList);
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_Main.儲位亮燈.GetEnumName())
                    {
                        List<Task> taskList = new List<Task>();
                        ColorDialog colorDialog = new ColorDialog();
                        if (colorDialog.ShowDialog() != DialogResult.OK) return;
                        Color color = colorDialog.Color;
                        UDP_Class uDP_Class = List_UDP_Local.SortByPort(CurrentDrawer.Port);
                        List<int> cols = new List<int>();
                        List<int> rows = new List<int>();
                        int index = this.GetSelectBoxes(ref cols, ref rows);
                        if (index == 0) return;
                        taskList.Add(Task.Run(() =>
                        {
                            CurrentDrawer.LED_Bytes = DrawerUI_EPD_1020.Get_Drawer_LEDBytes_UDP(uDP_Class, CurrentDrawer.IP);
                            DrawerUI_EPD_1020.Set_Drawer_LED_UDP(uDP_Class, CurrentDrawer, cols, rows, color);
                        }));
                        Task allTask = Task.WhenAll(taskList);
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_Main.清除燈號.GetEnumName())
                    {
                        List<Task> taskList = new List<Task>();
                        UDP_Class uDP_Class = List_UDP_Local.SortByPort(CurrentDrawer.Port);
                        taskList.Add(Task.Run(() =>
                        {
                            DrawerUI_EPD_1020.Set_LED_Clear_UDP(uDP_Class, CurrentDrawer);
                        }));
                        Task allTask = Task.WhenAll(taskList);
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_Main.全部亮燈測試.GetEnumName())
                    {
                        List<Task> taskList = new List<Task>();
                        ColorDialog colorDialog = new ColorDialog();
                        if (colorDialog.ShowDialog() != DialogResult.OK) return;
                        Color color = colorDialog.Color;
                        UDP_Class uDP_Class = List_UDP_Local.SortByPort(CurrentDrawer.Port);
                        taskList.Add(Task.Run(() =>
                        {
                            DrawerUI_EPD_1020.Set_LED_UDP(uDP_Class, CurrentDrawer, color);
                        }));
                        Task allTask = Task.WhenAll(taskList);
                    }
                    else if (dialog_ContextMenuStrip.Value == ContextMenuStrip_Main.上傳資料.GetEnumName())
                    {
                        List<Task> taskList = new List<Task>();
                        UDP_Class uDP_Class = List_UDP_Local.SortByPort(CurrentDrawer.Port);
                        taskList.Add(Task.Run(() =>
                        {
                            DrawerUI_EPD_1020.DrawToEpd_UDP(uDP_Class, CurrentDrawer);

                        }));
                        Task allTask = Task.WhenAll(taskList);
                    }
                }
            }
        
        }
        private void PictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.flag_pictureBox_MouseDown)
            {
                this.pictureBox_Select_Check(e.X, e.Y);
            }
        }
        private void PictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            this.flag_pictureBox_MouseDown = false;
        }
        private void PictureBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.MouseDoubleClickEvent != null) this.MouseDoubleClickEvent(this.GetSelectBoxes());
        }
        private void pictureBox_Select_Check(int pox, int poy)
        {
            if (CurrentDrawer == null) return;
            int Max_PoX = 0;
            int Max_PoY = 0;
            int Min_PoX = 999999;
            int Min_PoY = 999999;

            bool flag_check_OK = false;
            for (int y = 0; y < this.CurrentDrawer.Boxes.Count; y++)
            {
                for (int x = 0; x < CurrentDrawer.Boxes[y].Length; x++)
                {
                    if (this.Check_Mouse_In_Box(this.pictureBox_MouseDown_X, this.pictureBox_MouseDown_Y, CurrentDrawer.Boxes[y][x]))
                    {
                        Rectangle rect = this.Get_Box_Combine(CurrentDrawer.Boxes[y][x]);
                        if (Min_PoX > rect.X) Min_PoX = rect.X;
                        if (Min_PoY > rect.Y) Min_PoY = rect.Y;

                        if (Max_PoX < rect.X + rect.Width) Max_PoX = rect.X + rect.Width;
                        if (Max_PoY < rect.Y + rect.Height) Max_PoY = rect.Y + rect.Height;
                    }
                }
            }
            for (int y = 0; y < this.CurrentDrawer.Boxes.Count; y++)
            {
                for (int x = 0; x < CurrentDrawer.Boxes[y].Length; x++)
                {
                    if (this.Check_Mouse_In_Box(pox, poy, CurrentDrawer.Boxes[y][x]))
                    {
                        Rectangle rect = this.Get_Box_Combine(CurrentDrawer.Boxes[y][x]);
                        if (Min_PoX > rect.X) Min_PoX = rect.X;
                        if (Min_PoY > rect.Y) Min_PoY = rect.Y;

                        if (Max_PoX < rect.X + rect.Width) Max_PoX = rect.X + rect.Width;
                        if (Max_PoY < rect.Y + rect.Height) Max_PoY = rect.Y + rect.Height;
                    }
                }
            }
            for (int k = 0; k < this.Select_Column.Count; k++)
            {
                Rectangle rect = this.Get_Box_Combine(CurrentDrawer.Boxes[this.Select_Column[k]][this.Select_Row[k]]);
                if (Min_PoX > rect.X) Min_PoX = rect.X;
                if (Min_PoY > rect.Y) Min_PoY = rect.Y;

                if (Max_PoX < rect.X + rect.Width) Max_PoX = rect.X + rect.Width;
                if (Max_PoY < rect.Y + rect.Height) Max_PoY = rect.Y + rect.Height;

            }

            for (int y = 0; y < this.CurrentDrawer.Boxes.Count; y++)
            {
                for (int x = 0; x < CurrentDrawer.Boxes[y].Length; x++)
                {
                    if (CurrentDrawer.Boxes[y][x].X + CurrentDrawer.Boxes[y][x].Width > Min_PoX && CurrentDrawer.Boxes[y][x].Y + CurrentDrawer.Boxes[y][x].Height > Min_PoY)
                    {
                        if (CurrentDrawer.Boxes[y][x].X + CurrentDrawer.Boxes[y][x].Width <= Max_PoX && CurrentDrawer.Boxes[y][x].Y + CurrentDrawer.Boxes[y][x].Height <= Max_PoY)
                        {
                            bool flag_OK = true;
                            for (int k = 0; k < this.Select_Column.Count; k++)
                            {
                                if (this.Select_Column[k] == y && this.Select_Row[k] == x)
                                {
                                    flag_OK = false;
                                }
                            }
                            if (flag_OK)
                            {
                                this.Select_Column.Add(y);
                                this.Select_Row.Add(x);
                                flag_check_OK = true;
                            }

                        }
                    }
                }
            }

            if (flag_check_OK)
            {
                DrawToPictureBox(CurrentDrawer);
                for (int k = 0; k < this.Select_Column.Count; k++)
                {
                    int col = this.Select_Column[k];
                    int row = this.Select_Row[k];
                    Rectangle rect = this.Get_Box_Combine(CurrentDrawer.Boxes[col][row]);
                    using (Graphics g = pictureBox.CreateGraphics())
                    {
                        g.DrawRectangle(new Pen(Color.Blue, Selected_Box_PenWidth), rect);
                    }
                       
                }

            }

        }
        #endregion
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
       

    }
}
