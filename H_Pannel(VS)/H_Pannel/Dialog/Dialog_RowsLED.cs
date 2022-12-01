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
    public partial class Dialog_RowsLED : Form
    {
        private List<string> list_IP;
        private List<UDP_Class> list_UDP_Class;
        private Color color = Color.Black;
        public Dialog_RowsLED(List<UDP_Class> list_UDP_Class, List<string> list_IP, Color color)
        {
            InitializeComponent();
            this.list_IP = list_IP;
            this.list_UDP_Class = list_UDP_Class;
            this.color = color;
            this.rJ_TrackBar.BarColor = Color.LightGray;
            this.rJ_TrackBar.SliderColor = color;
            this.rJ_TrackBar.MinValue = 0;
            this.rJ_TrackBar.MaxValue = 10;
        }
        private void rJ_Button_OK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
            this.Close();
        }
        private void rJ_Button_Cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
            this.Close();
        }
        private void rJ_TrackBar_ValueChanged(int MinValue, int MaxValue)
        {
            List<Task> taskList = new List<Task>();
            for (int i = 0; i < list_IP.Count; i++)
            {
                string IP = list_IP[i];
                UDP_Class uDP_Class = list_UDP_Class[i];
                taskList.Add(Task.Run(() =>
                {
                    RowsLEDUI.Set_Rows_LED_UDP(uDP_Class, IP, MinValue, MaxValue, color);
                }));
            }
            Task allTask = Task.WhenAll(taskList);
            allTask.Wait();
        }
    }
}
