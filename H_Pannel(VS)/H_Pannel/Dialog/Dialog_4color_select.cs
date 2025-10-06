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
using SkiaSharp;
namespace H_Pannel_lib
{
    public partial class Dialog_4color_select : MyDialog
    {
        public Color Value = Color.White;

        public Dialog_4color_select(Color color)
        {
            InitializeComponent();
            this.rJ_Button_確認.MouseDownEvent += RJ_Button_確認_MouseDownEvent;
            rJ_Lable_Black.Click += RJ_Lable_Click;
            rJ_Lable_White.Click += RJ_Lable_Click;
            rJ_Lable_Red.Click += RJ_Lable_Click;
            rJ_Lable_Yellow.Click += RJ_Lable_Click;
            rJ_Lable_Transparent.Click += RJ_Lable_Click;


            if (color == Color.Black)
            {
                RJ_Lable_Click(rJ_Lable_Black, null);
            }
            if (color == Color.White)
            {
                RJ_Lable_Click(rJ_Lable_White, null);
            }
            if (color == Color.Red)
            {
                RJ_Lable_Click(rJ_Lable_Red, null);
            }       
            if (color == Color.Yellow)
            {
                RJ_Lable_Click(rJ_Lable_Yellow, null);
            }
            if (color == Color.Transparent)
            {
                RJ_Lable_Click(rJ_Lable_Transparent, null);
            }

            Value = color;
        }
        private void RJ_Lable_Click(object sender, EventArgs e)
        {
            RJ_Lable rJ_Lable = (RJ_Lable)sender;
            rJ_Lable_Black.BackgroundColor = (rJ_Lable == rJ_Lable_Black) ? Color.DimGray : rJ_Lable_Black.TextColor;
            rJ_Lable_White.BackgroundColor = (rJ_Lable == rJ_Lable_White) ? Color.DimGray : rJ_Lable_White.TextColor;
            rJ_Lable_Red.BackgroundColor = (rJ_Lable == rJ_Lable_Red) ? Color.White : rJ_Lable_Red.TextColor;
            rJ_Lable_Yellow.BackgroundColor = (rJ_Lable == rJ_Lable_Yellow) ? Color.White : rJ_Lable_Yellow.TextColor;
            rJ_Lable_Transparent.BackgroundColor = (rJ_Lable == rJ_Lable_Transparent) ? Color.Black : rJ_Lable_Transparent.TextColor;


            if (rJ_Lable == rJ_Lable_Black) Value = Color.Black;
            if (rJ_Lable == rJ_Lable_White) Value = Color.White;
            if (rJ_Lable == rJ_Lable_Red) Value = Color.Red;
            if (rJ_Lable == rJ_Lable_Yellow) Value = Color.Yellow;
            if (rJ_Lable == rJ_Lable_Transparent) Value = Color.Transparent;

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
