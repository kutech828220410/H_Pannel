using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Basic;
using H_Pannel_lib;
using System.Drawing;
namespace EinkSyncTest
{
    class Program
    {
        static string filePath = @"C:\Users\Andy\Desktop\電子紙牆0503\E-Paper-02.bmp";
        static string ServerIP = "192.168.5.250";
        static string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        static void Main(string[] args)
        {
            string ip_temp = "192.168.46.72";           
            Communication.ConsoleWrite = true;

            using (Bitmap inputBmp = (Bitmap)Bitmap.FromFile(filePath))
            {
                UDP_Class uDP_Class = new UDP_Class(ServerIP, 29000);
                bool flag_EPD_579B_DrawFramebuffer = H_Pannel_lib.Communication.EPD_579B_DrawImage(uDP_Class, ip_temp, inputBmp);
            }

            Console.ReadKey();

        }
    }
}
