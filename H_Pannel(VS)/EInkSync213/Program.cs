using System;
using System.Drawing.Imaging;
using System.Drawing;
using H_Pannel_lib;
using MyOffice;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Basic;

namespace EInkSync213
{
    class Program
    {
        static string ServerIP = "192.168.5.250";
        static string path = @"C:\image\Light_bmp";
        static void Main(string[] args)
        {
            UDP_Class uDP_Class = new UDP_Class(ServerIP, 29000);
            List<DataTable> dataTables = MyOffice.ExcelClass.NPOI_LoadFile2DataTables(@"C:\Users\Administrator\Desktop\Light.xlsx");

            for (int i = 0; i < dataTables.Count; i++)
            {
                if (dataTables[i].TableName == "1")
                {
                    List<object[]> list_value = dataTables[i].DataTableToRowList();
                    List<Task> tasks = new List<Task>();
                    foreach (object[] value in list_value)
                    {
                        string filename = $@"{path}\{value[0].ObjectToString()}.bmp";
                        string ip_temp = $"192.168.{value[1].ObjectToString()}";
                        tasks.Add(Task.Run(new Action(delegate
                        {
                            Bitmap inputBmp = new Bitmap(filename);
                            H_Pannel_lib.Communication.EPD_213_BRW_V0_DrawFramebuffer(uDP_Class, ip_temp, inputBmp);
                            inputBmp.Dispose();
                        })));

                    }
                    Task.WhenAll(tasks).Wait();
                    tasks.Clear();
                    foreach (object[] value in list_value)
                    {
                        string ip_temp = $"192.168.{value[1].ObjectToString()}";
                        tasks.Add(Task.Run(new Action(delegate
                        {
                            H_Pannel_lib.Communication.EPD_RefreshCanvas(uDP_Class, ip_temp);
                        })));
                    }
                    Task.WhenAll(tasks).Wait();
                }
            }

            Console.ReadLine();
        }
    }
}
