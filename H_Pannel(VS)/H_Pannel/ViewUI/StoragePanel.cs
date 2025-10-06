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
    public partial class StoragePanel : UserControl
    {
        public enum enum_ValueName
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
            圖片1,
            圖片2,
            文本1,
            文本2,
            文本3,

        }

        public delegate void SureClickHandler(Storage storage);
        public event SureClickHandler SureClick;
        private double sclae = 1D;
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

        public StoragePanel()
        {
            InitializeComponent();

            this.Load += StoragePanel_Load;
        }

        #region Event
        private void StoragePanel_Load(object sender, EventArgs e)
        {
            if(this.DesignMode == false)
            {
                this.pictureBox.MouseDown += PictureBox_MouseDown;
                this.pictureBox.Paint += PictureBox_Paint;
            }

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
            if (CurrentStorage == null) return;
            if (CurrentStorage.DeviceType == DeviceType.EPD213 || CurrentStorage.DeviceType == DeviceType.EPD213_lock)
            {
                EPD213_Paint_Form ePD213_Paint_Form = new EPD213_Paint_Form(this.CurrentStorage);
                if (ePD213_Paint_Form.ShowDialog() != DialogResult.Yes) return;
                if (SureClick != null) SureClick(currentStorage);
            }
            if (CurrentStorage.DeviceType == DeviceType.EPD266 || CurrentStorage.DeviceType == DeviceType.EPD266_lock)
            {
                EPD266_Paint_Form ePD266_Paint_Form = new EPD266_Paint_Form(this.CurrentStorage);
                if (ePD266_Paint_Form.ShowDialog() != DialogResult.Yes) return;
                if (SureClick != null) SureClick(currentStorage);
            }
            if (CurrentStorage.DeviceType == DeviceType.EPD290 || CurrentStorage.DeviceType == DeviceType.EPD290_lock)
            {
                EPD290_Paint_Form ePD290_Paint_Form = new EPD290_Paint_Form(this.CurrentStorage);
                if (ePD290_Paint_Form.ShowDialog() != DialogResult.Yes) return;
                if (SureClick != null) SureClick(currentStorage);
            }
            if (CurrentStorage.DeviceType.GetEnumName().Contains("EPD420"))
            {
                EPD420_Paint_Form ePD420_Paint_Form = new EPD420_Paint_Form(this.CurrentStorage);
                if (ePD420_Paint_Form.ShowDialog() != DialogResult.Yes) return;
                if (SureClick != null) SureClick(currentStorage);
            }
            if (CurrentStorage.DeviceType == DeviceType.EPD360E || CurrentStorage.DeviceType == DeviceType.EPD360E_lock)
            {
                EPD360E_Paint_Form ePD360E_Paint_Form = new EPD360E_Paint_Form(this.CurrentStorage);
                if (ePD360E_Paint_Form.ShowDialog() != DialogResult.Yes) return;
                if (SureClick != null) SureClick(currentStorage);
            }
            base.OnMouseDown(e);
        }


        #endregion

        public void DrawToPictureBox()
        {
            this.DrawToPictureBox(currentStorage);
        }
        public void DrawToPictureBox(Storage storage)
        {
            if (storage == null) return;
            if (storage.DeviceType.GetEnumName().Contains("EPD360E")) sclae = 0.75D;
            else sclae = 1;
            this.Invoke(new Action(delegate 
            {
                this.Width = (int)(storage.PanelSize.Width * sclae);
                this.Height = (int)(storage.PanelSize.Height * sclae);
            }));
            this.currentStorage = storage;
            using (Bitmap bitmap = this.Get_Storage_bmp(storage , sclae))
            {
                if(sclae != 1)
                {
                    bitmap.ScaleImage(Width, Height);   
                }
                using (Graphics g = pictureBox.CreateGraphics())
                {
                    g.DrawImage(bitmap, new PointF());
                }
            }
        }
        virtual public Bitmap Get_Storage_bmp(Storage storage , double scale = 1)
        {
            if(storage.DeviceType == DeviceType.EPD266 || storage.DeviceType == DeviceType.EPD266_lock)
            {
                return Communication.Storage_GetBitmap(storage, scale);
            }
            if (storage.DeviceType == DeviceType.EPD290 || storage.DeviceType == DeviceType.EPD290_lock)
            {
                return Communication.Storage_GetBitmap(storage, scale);
            }
            if (storage.DeviceType == DeviceType.EPD213 || storage.DeviceType == DeviceType.EPD213_lock)
            {
                return Communication.Storage_GetBitmap(storage, scale);
            }
            if (storage.DeviceType == DeviceType.EPD420 || storage.DeviceType == DeviceType.EPD420_lock)
            {
                return Communication.Storage_GetBitmap(storage, scale);
            }
            if (storage.DeviceType == DeviceType.EPD360E || storage.DeviceType == DeviceType.EPD360E_lock)
            {
                return Communication.Storage_GetBitmap(storage, scale);
            }
            return Communication.Get_Storage_bmp(storage, new StoragePanel.enum_ValueName().GetEnumNames(), scale);
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
