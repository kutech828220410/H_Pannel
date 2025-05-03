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
        static string wall_path = @"C:\image\Wall_bmp";
        static string wall2_path = @"C:\image\Wall2_bmp";
        static string table_path = @"C:\image\Table_bmp";
        static string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        static bool flag_wall_refresh = true;
        static bool flag_table_refresh = false;
        static void Main(string[] args)
        {
            Communication.ConsoleWrite = true;
            try
            {
                int index = 0;
                UDP_Class uDP_Class = new UDP_Class(ServerIP, 29000);
                while(true)
                {
                    Communication.ConsoleWrite = false;
                    int index_DrawFramebuffer = 0;
                    int index_RefreshCanvas = 0;
                    List<DataTable> dataTables = MyOffice.ExcelClass.NPOI_LoadFile2DataTables($@"{desktopPath}\wall.xlsx");
                    for (int i = 0; i < dataTables.Count; i++)
                    {
                        if (dataTables[i].TableName == "花蓮" || dataTables[i].TableName == "台東"
                            || dataTables[i].TableName == "高雄" || dataTables[i].TableName == "台南"
                            || dataTables[i].TableName == "台北左" || dataTables[i].TableName == "台北右"
                            || dataTables[i].TableName == "街廓學" || dataTables[i].TableName == "馬祖")
                        {
                            if(flag_wall_refresh == false)
                            {
                                if (dataTables[i].TableName == "花蓮" || dataTables[i].TableName == "台東"
                                 || dataTables[i].TableName == "高雄" || dataTables[i].TableName == "台南"
                                 || dataTables[i].TableName == "台北左" || dataTables[i].TableName == "台北右") continue;
                            }
                            if(flag_table_refresh == false)
                            {
                                if (dataTables[i].TableName == "街廓學" || dataTables[i].TableName == "馬祖") continue;
                            }
                            index_RefreshCanvas = 0;
                            index_DrawFramebuffer = 0;
                            Console.WriteLine($"★★★★★★{dataTables[i].TableName} ★★★★★★");
                            List<object[]> list_value = dataTables[i].DataTableToRowList();
                            List<Task> tasks = new List<Task>();
                            for (int m = 0; m < 9; m++)
                            {
                                for (int n = 0; n < 14; n++)
                                {
                                    int temp = m * 14 + n;
                                    if (temp >= list_value.Count) continue;
                                    object[] value = list_value[temp];

                                    string filename = $@"{wall2_path}\{value[0].ObjectToString().Replace("Wall", "Wall2")}.bmp";
                                    if (index % 2 == 1)
                                    {
                                        filename = $@"{wall_path}\{value[0].ObjectToString()}.bmp";
                                    }
                                    if (dataTables[i].TableName == "街廓學" || dataTables[i].TableName == "馬祖") filename = $@"{table_path}\{value[0].ObjectToString()}.bmp";
                                    string ip_temp = $"192.168.{value[1].ObjectToString()}";
                                    DitheringProcessor.DitheringMode ditheringMode = DitheringProcessor.DitheringMode.FourColor;
                                    tasks.Add(Task.Run(new Action(delegate
                                    {

                                        try
                                        {
                                            bool flag = false;
                                            Bitmap inputBmp = new Bitmap(filename);
                                            flag = H_Pannel_lib.Communication.EPD_579G_DrawFramebuffer(uDP_Class, ip_temp, inputBmp);
                                            inputBmp.Dispose();
                                            if (flag == false)
                                            {
                                                Console.WriteLine($"{ip_temp} EPD_579G_DrawFramebuffer failed..");
                                                return;
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine($"{ip_temp} Exception : {ex.Message}");
                                        }
                                        finally
                                        {
                                            Console.WriteLine($"{ip_temp} EPD_579G_DrawFramebuffer {index_DrawFramebuffer}/{list_value.Count}");
                                            index_DrawFramebuffer++;
                                        }

                                    })));
                                
                                   
                                }
                                Task.WhenAll(tasks).Wait();
                                List<Task> tasks_refresh = new List<Task>();
                                for (int n = 0; n < 14; n++)
                                {
                                    int temp = m * 14 + n;
                                    if (temp >= list_value.Count) continue;
                                    object[] value = list_value[temp];

                                    string filename = $@"{wall2_path}\{value[0].ObjectToString().Replace("Wall", "Wall2")}.bmp";
                                    if (index % 2 == 1)
                                    {
                                        filename = $@"{wall_path}\{value[0].ObjectToString()}.bmp";
                                    }
                                    if (dataTables[i].TableName == "街廓學" || dataTables[i].TableName == "馬祖") filename = $@"{table_path}\{value[0].ObjectToString()}.bmp";
                                    string ip_temp = $"192.168.{value[1].ObjectToString()}";
                                    DitheringProcessor.DitheringMode ditheringMode = DitheringProcessor.DitheringMode.FourColor;
                                    
                                   
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
                                            Console.WriteLine($"{ip_temp} EPD_RefreshCanvas {index_RefreshCanvas}/{list_value.Count}");
                                            index_RefreshCanvas++;
                                        }
                                    })));
                                     
                                }
                                Task.WhenAll(tasks_refresh).Wait();
                                Console.WriteLine($"Sleep 1s....");
                                System.Threading.Thread.Sleep(1000);
                                Console.WriteLine($"Sleep 2s....");
                                System.Threading.Thread.Sleep(1000);
                                Console.WriteLine($"Sleep 3s....");
                                System.Threading.Thread.Sleep(1000);
                            }






                        }
                    }
                   
                    index++;

                }
              

            }
            catch(Exception ex)
            {
                Console.WriteLine($"Exception : {ex.Message}");
                Console.ReadLine();
            }

        
        }
    }
}
