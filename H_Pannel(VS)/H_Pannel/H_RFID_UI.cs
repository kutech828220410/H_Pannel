using System;
using System;
using System.Collections.Generic;
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
using MyUI;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Net;
using H_Pannel_lib;


namespace H_Pannel_lib
{
    public partial class H_RFID_UI : DeviceBasicUI
    {
        private void InitializeComponent()
        {
            this.SuspendLayout();
 
            this.sqL_DataGridView_DeviceTable.columnHeadersHeight = 19;

            this.sqL_DataGridView_UDP_DataReceive.columnHeadersHeight = 18;

            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.Name = "H_RFID_UI";
            this.ResumeLayout(false);

        }
    }
}
