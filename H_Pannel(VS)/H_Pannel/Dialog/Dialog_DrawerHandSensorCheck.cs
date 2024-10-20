using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MyUI;
using Basic;
namespace H_Pannel_lib
{
    public partial class Dialog_DrawerHandSensorCheck : MyDialog
    {
        private MyThread myThread;
        private UDP_Class udP_Class;
        private string IP = "";
        private List<object[]> list_UDP_Rx = new List<object[]>();

        public Dialog_DrawerHandSensorCheck( string IP, List<object[]> list_UDP_Rx)
        {
            InitializeComponent();
            this.IP = IP;
            this.list_UDP_Rx = list_UDP_Rx;
            this.Load += Dialog_DrawerHandSensorCheck_Load;
            this.panel_paint.Paint += Panel_paint_Paint;
            this.FormClosing += Dialog_DrawerHandSensorCheck_FormClosing;
        }

        private void Dialog_DrawerHandSensorCheck_FormClosing(object sender, FormClosingEventArgs e)
        {
            myThread.Abort();
            myThread = null;
        }

        private void Panel_paint_Paint(object sender, PaintEventArgs e)
        {
           

        }
        
        private void Dialog_DrawerHandSensorCheck_Load(object sender, EventArgs e)
        {
            myThread = new MyThread();
            myThread.SetSleepTime(100);
            myThread.AutoRun(true);
            myThread.Add_Method(sub_program);
            myThread.Trigger();
        }

        private void sub_program()
        {
            //Console.WriteLine($"DrawerHandSensorCheck Paint event start....{IP}");
            list_UDP_Rx = list_UDP_Rx.GetRows((int)enum_UDP_DataReceive.IP, IP);
            if (list_UDP_Rx.Count == 0) return;
            string jsonString = list_UDP_Rx[0][(int)enum_UDP_DataReceive.Readline].ObjectToString();
            StorageUI_LCD_114.UDP_READ uDP_READ = jsonString.JsonDeserializet<StorageUI_LCD_114.UDP_READ>();
            if (uDP_READ == null) return;

            //Console.WriteLine($"DrawerHandSensorCheck Paint event....{uDP_READ}");
            using(Graphics g = this.panel_paint.CreateGraphics())
            {

                g.DrawRectangle(new Pen(new SolidBrush(Color.Black), 2), new Rectangle(new Point(0, 0), this.panel_paint.Size));

                int width = this.panel_paint.Width;
                int height = this.panel_paint.Height;

                int H_line_pixel = width / 5;
                int V_line_pixel = height / 5;

                g.DrawLine(new Pen(new SolidBrush(uDP_READ.Get_Input(StorageUI_LCD_114.enum_InputName.X0) ? Color.Lime : Color.Gray), 6), new Point(0, V_line_pixel * 1), new Point(width, V_line_pixel * 1));
                g.DrawString("X0", new Font("微軟正黑體", 16, FontStyle.Bold), new SolidBrush(Color.Black), new Point(0, V_line_pixel * 1));
                g.DrawLine(new Pen(new SolidBrush(uDP_READ.Get_Input(StorageUI_LCD_114.enum_InputName.X1) ? Color.Lime : Color.Gray), 6), new Point(0, V_line_pixel * 2), new Point(width, V_line_pixel * 2));
                g.DrawString("X1", new Font("微軟正黑體", 16, FontStyle.Bold), new SolidBrush(Color.Black), new Point(0, V_line_pixel * 2));
                g.DrawLine(new Pen(new SolidBrush(uDP_READ.Get_Input(StorageUI_LCD_114.enum_InputName.X2) ? Color.Lime : Color.Gray), 6), new Point(0, V_line_pixel * 3), new Point(width, V_line_pixel * 3));
                g.DrawString("X2", new Font("微軟正黑體", 16, FontStyle.Bold), new SolidBrush(Color.Black), new Point(0, V_line_pixel * 3));
                g.DrawLine(new Pen(new SolidBrush(uDP_READ.Get_Input(StorageUI_LCD_114.enum_InputName.X3) ? Color.Lime : Color.Gray), 6), new Point(0, V_line_pixel * 4), new Point(width, V_line_pixel * 4));
                g.DrawString("X3", new Font("微軟正黑體", 16, FontStyle.Bold), new SolidBrush(Color.Black), new Point(0, V_line_pixel * 4));

                g.DrawLine(new Pen(new SolidBrush(uDP_READ.Get_Input(StorageUI_LCD_114.enum_InputName.Y0) ? Color.Lime : Color.Gray), 6), new Point(H_line_pixel * 1, 0), new Point(H_line_pixel * 1, height));
                g.DrawString("Y0", new Font("微軟正黑體", 16, FontStyle.Bold), new SolidBrush(Color.Black), new Point(H_line_pixel * 1, 0));
                g.DrawLine(new Pen(new SolidBrush(uDP_READ.Get_Input(StorageUI_LCD_114.enum_InputName.Y1) ? Color.Lime : Color.Gray), 6), new Point(H_line_pixel * 2, 0), new Point(H_line_pixel * 2, height));
                g.DrawString("Y1", new Font("微軟正黑體", 16, FontStyle.Bold), new SolidBrush(Color.Black), new Point(H_line_pixel * 2, 0));
                g.DrawLine(new Pen(new SolidBrush(uDP_READ.Get_Input(StorageUI_LCD_114.enum_InputName.Y2) ? Color.Lime : Color.Gray), 6), new Point(H_line_pixel * 3, 0), new Point(H_line_pixel * 3, height));
                g.DrawString("Y2", new Font("微軟正黑體", 16, FontStyle.Bold), new SolidBrush(Color.Black), new Point(H_line_pixel * 3, 0));
                g.DrawLine(new Pen(new SolidBrush(uDP_READ.Get_Input(StorageUI_LCD_114.enum_InputName.Y3) ? Color.Lime : Color.Gray), 6), new Point(H_line_pixel * 4, 0), new Point(H_line_pixel * 4, height));
                g.DrawString("Y3", new Font("微軟正黑體", 16, FontStyle.Bold), new SolidBrush(Color.Black), new Point(H_line_pixel * 4, 0));

            }

        }
    }
}
