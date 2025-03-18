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
        static string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        static void Main(string[] args)
        {
            UDP_Class uDP_Class = new UDP_Class(ServerIP, 29000);
            while(true)
            {
                List<DataTable> dataTables = MyOffice.ExcelClass.NPOI_LoadFile2DataTables($@"{ desktopPath}\Light.xlsx");
                Communication.ConsoleWrite = false;

                int index_DrawFramebuffer = 0;
                int index_RefreshCanvas = 0;
                for (int i = 0; i < dataTables.Count; i++)
                {
                    if (dataTables[i].TableName == "3")
                    {
                        List<object[]> list_value = dataTables[i].DataTableToRowList();
                        List<Task> tasks = new List<Task>();

                        foreach (object[] value in list_value)
                        {

                            string filename = $@"{path}\{value[0].ObjectToString()}.bmp";
                            string ip_temp = $"192.168.{value[1].ObjectToString()}";
                            tasks.Add(Task.Run(new Action(delegate
                            {

                                try
                                {
                                    bool flag = false;
                                    Bitmap inputBmp = new Bitmap(filename);
                                    flag = H_Pannel_lib.Communication.EPD_213_BRW_V0_DrawFramebuffer(uDP_Class, ip_temp, inputBmp);
                                    inputBmp.Dispose();
                                    if (flag == false)
                                    {
                                        Console.WriteLine($"{ip_temp} EPD_213_BRW_V0_DrawFramebuffer failed..");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"{ip_temp} Exception : {ex.Message}");
                                }
                                finally
                                {
                                    Console.WriteLine($"{index_DrawFramebuffer}/{list_value.Count}");
                                    index_DrawFramebuffer++;
                                }
                            })));

                        }
                        Task.WhenAll(tasks).Wait();
                        List<Task> tasks_refresh = new List<Task>();
                        foreach (object[] value in list_value)
                        {
                            string ip_temp = $"192.168.{value[1].ObjectToString()}";
                            tasks_refresh.Add(Task.Run(new Action(delegate
                            {
                                try
                                {
                                    bool flag = false;
                                    flag = H_Pannel_lib.Communication.EPD_RefreshCanvas(uDP_Class, ip_temp);
                                    if (flag == false)
                                    {
                                        Console.WriteLine($"{ip_temp} EPD_RefreshCanvas failed..");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"{ip_temp} Exception : {ex.Message}");
                                }
                                finally
                                {
                                    //Console.WriteLine($"{index_RefreshCanvas}/{list_value.Count}");
                                    index_RefreshCanvas++;
                                }

                            })));
                        }
                        Task.WhenAll(tasks_refresh).Wait();
                        System.Threading.Thread.Sleep(60000);
                    }
                    
                }
            
            }

            
        }
    }
}
