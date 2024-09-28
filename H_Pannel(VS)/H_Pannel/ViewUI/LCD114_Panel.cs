using System;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using Basic;

namespace H_Pannel_lib
{
    public partial class LCD114_Panel : UserControl
    {
        [ReadOnly(false), Browsable(false), Category(""), Description(""), DefaultValue("")]
        public int Pannel_Width
        {
            get
            {
                return 240;
            }
        }
        [ReadOnly(false), Browsable(false), Category(""), Description(""), DefaultValue("")]
        public int Pannel_Height
        {
            get
            {
                return 135;
            }
        }
        private float CanvasScale = 1.0F;
        private Bitmap bitmap_Canvas;
        private List<UDP_Class> List_UDP_Local;
        private Storage currentStorage;
        [ReadOnly(false), Browsable(false), Category(""), Description(""), DefaultValue("")]
        public Storage CurrentStorage { get => currentStorage; private set => currentStorage = value; }

        public LCD114_Panel()
        {
            InitializeComponent();
        }

        public void Init(List<UDP_Class> List_UDP_Local)
        {
            this.List_UDP_Local = List_UDP_Local;
            bitmap_Canvas = new Bitmap((int)(Pannel_Width * CanvasScale), (int)(Pannel_Height * CanvasScale));
        }
        public void DrawToPictureBox(string text, Font font, Color ForeColoe, Color BackColor)
        {

            using (Graphics g = pictureBox.CreateGraphics())
            {
                bitmap_Canvas = Communication.Get_LCD_144_bmp(text, font, ForeColoe, BackColor);
                g.DrawImage(bitmap_Canvas, new PointF());
            }
        }
        public bool DrawImage(string IP ,int port ,string text, Font font,Color ForeColoe, Color BackColor)
        {
            DrawToPictureBox(text, font, ForeColoe, BackColor);
            UDP_Class uDP_Class = List_UDP_Local.SortByPort(port);
            if (uDP_Class == null) return false;
            bitmap_Canvas = Communication.Get_LCD_144_bmp(text, font, Color.White, Color.Black);
            return Communication.LCD_144_DrawImageEx(uDP_Class, IP, bitmap_Canvas, ForeColoe, BackColor);
        }
    }
}
