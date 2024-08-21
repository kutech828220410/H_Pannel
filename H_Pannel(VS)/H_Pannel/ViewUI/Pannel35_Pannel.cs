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
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;

namespace H_Pannel_lib
{
    
    public partial class Pannel35_Pannel : UserControl
    {
        static PaintHandler paintHandler;

        public delegate void MouseDownEventHandler(MouseEventArgs mevent);
        public event MouseDownEventHandler MouseDownEvent;

        public delegate void EditFinishedEventHandler(Storage storage);
        public event EditFinishedEventHandler EditFinishedEvent;

        [ReadOnly(false), Browsable(false), Category(""), Description(""), DefaultValue("")]
        public int Pannel_Width
        {
            get
            {
                return 480;
            }
        }
        [ReadOnly(false), Browsable(false), Category(""), Description(""), DefaultValue("")]
        public int Pannel_Height
        {
            get
            {
                return 320;
            }
        }

        private float CanvasScale = 0.75F;
        private Bitmap bitmap_Canvas;
        private List<UDP_Class> List_UDP_Local;
        private Storage currentStorage;
        [ReadOnly(false), Browsable(false), Category(""), Description(""), DefaultValue("")]
        public Storage CurrentStorage { get => currentStorage; private set => currentStorage = value; }


        public Pannel35_Pannel()
        {
            InitializeComponent();
        }
        public void Init(List<UDP_Class> List_UDP_Local)
        {
            this.List_UDP_Local = List_UDP_Local;
            bitmap_Canvas = new Bitmap((int)(Pannel_Width * CanvasScale), (int)(Pannel_Height * CanvasScale));
            this.pictureBox.Click += PictureBox_Click;
            this.pictureBox.MouseDown += PictureBox_MouseDown;
            this.pictureBox.Paint += PictureBox_Paint;

            paintHandler = this.BasePaint;
        }

        private void PictureBox_Click(object sender, EventArgs e)
        {
            if (currentStorage == null) return;
            Dialog_Pannel35 dialog_Pannel35 = new Dialog_Pannel35(currentStorage, List_UDP_Local);
            if (dialog_Pannel35.ShowDialog() == DialogResult.Yes)
            {

            }
            EditFinishedEvent?.Invoke(this.currentStorage);
            this.DrawToPictureBox();
        }

        async private void PictureBox_Paint(object sender, PaintEventArgs e)
        {
            Task task = Task.Factory.StartNew(new Action(delegate
            {
                this.DrawToPictureBox();
            }));
            await task;

        }
        private void PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            this.MouseDownEvent?.Invoke(e);
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

        public void DrawToPictureBox(Storage storage)
        {
            this.Set_Stroage(storage);
        }

        public void DrawToPictureBox()
        {            
            if(paintHandler != null)
            {
                paintHandler();
            }
            using (Graphics g = pictureBox.CreateGraphics())
            {
                g.DrawImage(bitmap_Canvas, new PointF());
            }
        }
        public void BasePaint()
        {
            if (this.CurrentStorage == null) return;
            try
            {
                using (Graphics g = Graphics.FromImage(bitmap_Canvas))
                {
                    Storage.VlaueClass vlaueClass;
                    g.SmoothingMode = SmoothingMode.HighQuality; //使繪圖質量最高，即消除鋸齒
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
                    g.FillRectangle(new SolidBrush(CurrentStorage.BackColor), 0, 0, this.Pannel_Width * CanvasScale, this.Pannel_Height * CanvasScale);
                    for (int i = 0; i < new Storage.ValueName().GetLength(); i++)
                    {
                        vlaueClass = CurrentStorage.GetValue((Storage.ValueName)i);
                        vlaueClass.Position.X = (int)(vlaueClass.Position.X * this.CanvasScale);
                        vlaueClass.Position.Y = (int)(vlaueClass.Position.Y * this.CanvasScale);
                        if (vlaueClass.Visable)
                        {
                            using (Bitmap bitmap = CurrentStorage.GetBitmap((Storage.ValueName)i, this.CanvasScale))
                            {
                                if (bitmap == null) continue;
                                g.DrawImage(bitmap, vlaueClass.Position);
                            }
                        }
                    }
                }
            }
            catch
            {

            }
            
        }

    }
}
