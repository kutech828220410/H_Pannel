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
    public partial class Dialog_3color_select : MyDialog
    {
        public Color Value = Color.White;

        public Dialog_3color_select(Color color)
        {
            InitializeComponent();
            this.Load += Dialog_3color_select_Load; 
            this.rJ_Button_確認.MouseDownEvent += RJ_Button_確認_MouseDownEvent;
            rJ_Lable_Black.Click += RJ_Lable_Click;
            rJ_Lable_Red.Click += RJ_Lable_Click;
            rJ_Lable_White.Click += RJ_Lable_Click;

            if(color == Color.Red)
            {
                RJ_Lable_Click(rJ_Lable_Red, null);
            }
            if (color == Color.Black)
            {
                RJ_Lable_Click(rJ_Lable_Black, null);
            }
            if (color == Color.White)
            {
                RJ_Lable_Click(rJ_Lable_White, null);
            }

            Value = color;
        }

        private void Dialog_3color_select_Load(object sender, EventArgs e)
        {

        }

        private void RJ_Lable_Click(object sender, EventArgs e)
        {
            RJ_Lable rJ_Lable = (RJ_Lable)sender;
            rJ_Lable_Black.BackgroundColor = (rJ_Lable == rJ_Lable_Black) ? Color.DimGray : rJ_Lable_Black.TextColor;
            rJ_Lable_Red.BackgroundColor = (rJ_Lable == rJ_Lable_Red) ? Color.White : rJ_Lable_Red.TextColor;
            rJ_Lable_White.BackgroundColor = (rJ_Lable == rJ_Lable_White) ? Color.DimGray : rJ_Lable_White.TextColor;

            if (rJ_Lable == rJ_Lable_Black) Value = Color.Black;
            if (rJ_Lable == rJ_Lable_Red) Value = Color.Red;
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
