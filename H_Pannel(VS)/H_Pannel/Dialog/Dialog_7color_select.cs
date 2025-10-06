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
namespace H_Pannel_lib
{
    public partial class Dialog_7color_select : MyDialog
    {
        public Color Value = Color.White;
        private Communication.EPDColors _ePDColors = Communication.EPDColors.EPD_7IN3F_WHITE;
        public Dialog_7color_select(Color color)
        {
            InitializeComponent();

            this.Load += Dialog_EPD730_顏色選擇_Load;

            this.rJ_Button_確認.MouseDownEvent += RJ_Button_確認_MouseDownEvent;
            rJ_Lable_Black.Click += RJ_Lable_Click;
            rJ_Lable_Red.Click += RJ_Lable_Click;
            rJ_Lable_Yellow.Click += RJ_Lable_Click;
            rJ_Lable_Lime.Click += RJ_Lable_Click;
            rJ_Lable_Blue.Click += RJ_Lable_Click;
            rJ_Lable_Orange.Click += RJ_Lable_Click;
            rJ_Lable_White.Click += RJ_Lable_Click;

            if (color == Color.White) this._ePDColors = Communication.EPDColors.EPD_7IN3F_WHITE;
            if (color == Color.Red) this._ePDColors = Communication.EPDColors.EPD_7IN3F_RED;
            if (color == Color.Yellow) this._ePDColors = Communication.EPDColors.EPD_7IN3F_YELLOW;
            if (color == Color.Green) this._ePDColors = Communication.EPDColors.EPD_7IN3F_GREEN;
            if (color == Color.Blue) this._ePDColors = Communication.EPDColors.EPD_7IN3F_BLUE;
            if (color == Color.Orange) this._ePDColors = Communication.EPDColors.EPD_7IN3F_ORANGE;
            if (color == Color.Black) this._ePDColors = Communication.EPDColors.EPD_7IN3F_BLACK;
        }

        public Dialog_7color_select(Communication.EPDColors ePDColors)
        {
            InitializeComponent();

            this.Load += Dialog_EPD730_顏色選擇_Load;

            this.rJ_Button_確認.MouseDownEvent += RJ_Button_確認_MouseDownEvent;
            rJ_Lable_Black.Click += RJ_Lable_Click;
            rJ_Lable_Red.Click += RJ_Lable_Click;
            rJ_Lable_Yellow.Click += RJ_Lable_Click;
            rJ_Lable_Lime.Click += RJ_Lable_Click;
            rJ_Lable_Blue.Click += RJ_Lable_Click;
            rJ_Lable_Orange.Click += RJ_Lable_Click;
            rJ_Lable_White.Click += RJ_Lable_Click;
            this._ePDColors = ePDColors;
        }

        private void Dialog_EPD730_顏色選擇_Load(object sender, EventArgs e)
        {
   
            if (_ePDColors == Communication.EPDColors.EPD_7IN3F_BLACK) RJ_Lable_Click(rJ_Lable_Black, null);
            if (_ePDColors == Communication.EPDColors.EPD_7IN3F_RED) RJ_Lable_Click(rJ_Lable_Red, null);
            if (_ePDColors == Communication.EPDColors.EPD_7IN3F_YELLOW) RJ_Lable_Click(rJ_Lable_Yellow, null);
            if (_ePDColors == Communication.EPDColors.EPD_7IN3F_GREEN) RJ_Lable_Click(rJ_Lable_Lime, null);
            if (_ePDColors == Communication.EPDColors.EPD_7IN3F_BLUE) RJ_Lable_Click(rJ_Lable_Blue, null);
            if (_ePDColors == Communication.EPDColors.EPD_7IN3F_ORANGE) RJ_Lable_Click(rJ_Lable_Orange, null);
            if (_ePDColors == Communication.EPDColors.EPD_7IN3F_WHITE) RJ_Lable_Click(rJ_Lable_White, null);

        }
        private void RJ_Lable_Click(object sender, EventArgs e)
        {
            RJ_Lable rJ_Lable = (RJ_Lable)sender;
            rJ_Lable_Black.BackgroundColor = (rJ_Lable == rJ_Lable_Black) ? Color.DimGray : rJ_Lable_Black.TextColor;
            rJ_Lable_Red.BackgroundColor = (rJ_Lable == rJ_Lable_Red) ? Color.White : rJ_Lable_Red.TextColor;
            rJ_Lable_Yellow.BackgroundColor = (rJ_Lable == rJ_Lable_Yellow) ? Color.White : rJ_Lable_Yellow.TextColor;
            rJ_Lable_Lime.BackgroundColor = (rJ_Lable == rJ_Lable_Lime) ? Color.White : rJ_Lable_Lime.TextColor;
            rJ_Lable_Blue.BackgroundColor = (rJ_Lable == rJ_Lable_Blue) ? Color.White : rJ_Lable_Blue.TextColor;
            rJ_Lable_Orange.BackgroundColor = (rJ_Lable == rJ_Lable_Orange) ? Color.White : rJ_Lable_Orange.TextColor;
            rJ_Lable_White.BackgroundColor = (rJ_Lable == rJ_Lable_White) ? Color.DimGray : rJ_Lable_White.TextColor;

            if (rJ_Lable == rJ_Lable_Black) Value = Color.Black;
            if (rJ_Lable == rJ_Lable_Red) Value = Color.Red;
            if (rJ_Lable == rJ_Lable_Yellow) Value = Color.Yellow;
            if (rJ_Lable == rJ_Lable_Lime) Value = Color.Green;
            if (rJ_Lable == rJ_Lable_Blue) Value = Color.Blue;
            if (rJ_Lable == rJ_Lable_Orange) Value = Color.Orange;
            if (rJ_Lable == rJ_Lable_White) Value = Color.White;

        }

        private void RJ_Button_確認_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate
            {
                DialogResult = DialogResult.Yes;
                this.Close();
            }));
        }
    }
}
