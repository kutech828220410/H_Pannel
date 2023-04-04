using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace H_Pannel_lib
{
    public partial class EPD_290_Pannel : UserControl
    {
        [ReadOnly(false), Browsable(false)]
        public Storage CurrentStorage
        {
            get
            {
                if (this.DesignMode) return null;
                return currentStorage;
            }
            set
            {
                if (this.DesignMode) return;
                currentStorage = value;
            }
        }

        private Storage currentStorage = new Storage();
        private List<UDP_Class> List_UDP_Local;
        private enum ContextMenuStrip_Main
        {

        }

        public EPD_290_Pannel()
        {
            InitializeComponent();
        }
        public void Init(List<UDP_Class> List_UDP_Local)
        {
            this.List_UDP_Local = List_UDP_Local;

            this.pictureBox.MouseDown += PictureBox_MouseDown;
            this.pictureBox.Paint += PictureBox_Paint;
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
            this.DrawToPictureBox(currentStorage);
        }
        public void DrawToPictureBox(Storage storage)
        {
            if (storage == null) return;
            this.currentStorage = storage;
            using (Bitmap bitmap = this.Get_Drawer_bmp(storage))
            {
                using (Graphics g = pictureBox.CreateGraphics())
                {
                    g.DrawImage(bitmap, new PointF());
                }
            }
        }
        virtual public Bitmap Get_Drawer_bmp(Storage storage)
        {
            return StorageUI_EPD_290.Get_Storage_bmp(storage);
        }
        private void PictureBox_MouseDown(object sender, MouseEventArgs e)
        {

        }

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
