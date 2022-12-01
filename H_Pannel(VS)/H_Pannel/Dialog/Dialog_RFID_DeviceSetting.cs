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
    public partial class Dialog_RFID_DeviceSetting : Form
    {
        public RFIDClass rFIDClass;
        private List<CheckBox> checkBoxes_儲位致能 = new List<CheckBox>();
        public Dialog_RFID_DeviceSetting(RFIDClass rFIDClass)
        {
            InitializeComponent();
            this.rFIDClass = rFIDClass;
            this.checkBoxes_儲位致能.Add(this.checkBox_儲位致能_01);
            this.checkBoxes_儲位致能.Add(this.checkBox_儲位致能_02);
            this.checkBoxes_儲位致能.Add(this.checkBox_儲位致能_03);
            this.checkBoxes_儲位致能.Add(this.checkBox_儲位致能_04);
            this.checkBoxes_儲位致能.Add(this.checkBox_儲位致能_05);

            for(int i = 0; i < this.rFIDClass.DeviceClasses.Length; i++)
            {
                this.checkBoxes_儲位致能[i].Checked = this.rFIDClass.DeviceClasses[i].Enable;
            }
        }

        private void Dialog_RFID_DeviceSetting_Load(object sender, EventArgs e)
        {
            this.rJ_Button_退出.MouseDownEvent += RJ_Button_退出_MouseDownEvent;

            this.checkBox_儲位致能_01.CheckStateChanged += CheckBox_儲位致能_01_CheckStateChanged;
            this.checkBox_儲位致能_02.CheckStateChanged += CheckBox_儲位致能_02_CheckStateChanged;
            this.checkBox_儲位致能_03.CheckStateChanged += CheckBox_儲位致能_03_CheckStateChanged;
            this.checkBox_儲位致能_04.CheckStateChanged += CheckBox_儲位致能_04_CheckStateChanged;
            this.checkBox_儲位致能_05.CheckStateChanged += CheckBox_儲位致能_05_CheckStateChanged;
        }


        private void CheckBox_儲位致能_01_CheckStateChanged(object sender, EventArgs e)
        {
            this.rFIDClass.DeviceClasses[0].Enable = this.checkBox_儲位致能_01.Checked;
        }
        private void CheckBox_儲位致能_02_CheckStateChanged(object sender, EventArgs e)
        {
            this.rFIDClass.DeviceClasses[1].Enable = this.checkBox_儲位致能_02.Checked;
        }
        private void CheckBox_儲位致能_03_CheckStateChanged(object sender, EventArgs e)
        {
            this.rFIDClass.DeviceClasses[2].Enable = this.checkBox_儲位致能_03.Checked;
        }
        private void CheckBox_儲位致能_04_CheckStateChanged(object sender, EventArgs e)
        {
            this.rFIDClass.DeviceClasses[3].Enable = this.checkBox_儲位致能_04.Checked;
        }
        private void CheckBox_儲位致能_05_CheckStateChanged(object sender, EventArgs e)
        {
            this.rFIDClass.DeviceClasses[4].Enable = this.checkBox_儲位致能_05.Checked;
        }
 
        private void RJ_Button_退出_MouseDownEvent(MouseEventArgs mevent)
        {
            this.Invoke(new Action(delegate 
            {
                this.Close();
            }));
        }
    }
}
