using System;
using System.Drawing.Imaging;
using System.Drawing;
using H_Pannel_lib;
using MyOffice;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Basic;
namespace EInkSync589
{
    class Program
    {
        static string ServerIP = "192.168.5.250";
        static string ClientIP = "192.168.40.10";
        static string wall_path = @"C:\image\Wall_bmp";
        static void Main(string[] args)
        {
            Communication.ConsoleWrite = true;
            try
            {
                string inputPath = @"C:\image\Wall_bmp\Wall (26).bmp";
                UDP_Class uDP_Class = new UDP_Class(ServerIP, 29000);


                List<DataTable> dataTables = MyOffice.ExcelClass.NPOI_LoadFile2DataTables(@"C:\Users\Administrator\Desktop\wall.xlsx");
                for (int i = 0; i < dataTables.Count; i++)
                {
                    if (dataTables[i].TableName == "台南")
                    {
                        List<object[]> list_value = dataTables[i].DataTableToRowList();
                        List<Task> tasks = new List<Task>();
                        foreach(object[] value in list_value)
                        {
                            string filename = $@"{wall_path}\{value[0].ObjectToString()}.bmp";
                            string ip_temp = $"192.168.{value[1].ObjectToString()}";
                            DitheringProcessor.DitheringMode ditheringMode = DitheringProcessor.DitheringMode.FourColor;
                            tasks.Add(Task.Run(new Action(delegate
                            {
                                Bitmap inputBmp = new Bitmap(filename);
                                H_Pannel_lib.Communication.EPD_579G_DrawFramebuffer(uDP_Class, ip_temp, inputBmp);
                                inputBmp.Dispose();
                            })));
                                                    
                        }
                        Task.WhenAll(tasks).Wait();
                        List<Task> tasks_refresh = new List<Task>();
                        foreach (object[] value in list_value)
                        {
                            string ip_temp = $"192.168.{value[1].ObjectToString()}";
                            tasks_refresh.Add(Task.Run(new Action(delegate
                            {
                                H_Pannel_lib.Communication.EPD_RefreshCanvas(uDP_Class, ip_temp);
                            })));                                        
                        }
                        Task.WhenAll(tasks_refresh).Wait();
                    }
                }

                Console.ReadLine();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Exception : {ex.Message}");
                Console.ReadLine();
            }

        
        }
    }
}
