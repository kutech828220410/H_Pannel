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
        // 設定伺服器 IP
        static string ServerIP = "192.168.5.250";

        // 各類型圖片所在資料夾路徑
        static string wall_path = @"C:\image\01.Tech wall";
        static string wall2_path = @"C:\image\02.Landscape-1";
        static string table_path = @"C:\image\03.Landscape-2";
     
        static string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        // 控制刷新牆面和桌面裝置的布林旗標
        static bool flag_wall_refresh = true;
        static bool flag_table_refresh = false;

        static void Main(string[] args)
        {
            Communication.ConsoleWrite = true;

            try
            {
                int index = 0; // 用來控制圖片切換
                UDP_Class uDP_Class = new UDP_Class(ServerIP, 29000); // 建立 UDP 傳輸物件

                while (true)
                {
                    Communication.ConsoleWrite = false;

                    int index_DrawFramebuffer = 0;
                    int index_RefreshCanvas = 0;

                    // 載入 Excel 檔案（wall.xlsx）中的所有工作表為 DataTable
                    List<DataTable> dataTables = MyOffice.ExcelClass.NPOI_LoadFile2DataTables($@"{desktopPath}\wall2.xlsx");

                    // 遍歷每個 DataTable（代表不同地區）
                    for (int i = 0; i < dataTables.Count; i++)
                    {
                        // 過濾只處理指定地區的工作表
                        if (dataTables[i].TableName == "花蓮" || dataTables[i].TableName == "台東"
                            || dataTables[i].TableName == "高雄" || dataTables[i].TableName == "台南"
                            || dataTables[i].TableName == "台北左" || dataTables[i].TableName == "台北右"
                            || dataTables[i].TableName == "台中" || dataTables[i].TableName == "嘉義")
                        {
                            // 若牆面刷新關閉，跳過牆面地區
                            if (flag_wall_refresh == false &&
                                (dataTables[i].TableName != "台中" && dataTables[i].TableName != "嘉義"))
                                continue;

                            // 若桌面刷新關閉，跳過桌面地區
                            if (flag_table_refresh == false &&
                                (dataTables[i].TableName == "台中" || dataTables[i].TableName == "嘉義"))
                                continue;

                            // 初始化刷新及繪圖次數
                            index_RefreshCanvas = 0;
                            index_DrawFramebuffer = 0;

                            Console.WriteLine($"★★★★★★{dataTables[i].TableName} ★★★★★★");

                            List<object[]> list_value = dataTables[i].DataTableToRowList(); // 取得所有行資料
                            List<Task> tasks = new List<Task>();

                            // 每次處理 9 列 x 14 欄 = 最多 126 台裝置
                            for (int m = 0; m < 9; m++)
                            {
                                for (int n = 0; n < 14; n++)
                                {
                                    int temp = m * 14 + n;
                                    if (temp >= list_value.Count) continue;

                                    object[] value = list_value[temp];

                                    // 依照檔名與切換 index 判斷圖片來源路徑
                                    string filename = $@"{value[0].ObjectToString()}";


                                    string ip_temp = $"192.168.{value[1].ObjectToString()}";

                                    // 加入非同步繪圖任務
                                    tasks.Add(Task.Run(() =>
                                    {
                                        try
                                        {
                                            bool flag = false;
                                            Bitmap inputBmp = new Bitmap(filename);
                                            flag = H_Pannel_lib.Communication.EPD_579G_DrawFramebuffer(uDP_Class, ip_temp, inputBmp);
                                            inputBmp.Dispose();
                                            if (!flag)
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
                                    }));
                                }

                                // 等待所有繪圖任務完成
                                Task.WhenAll(tasks).Wait();

                                // 執行 RefreshCanvas 指令
                                List<Task> tasks_refresh = new List<Task>();
                                for (int n = 0; n < 14; n++)
                                {
                                    int temp = m * 14 + n;
                                    if (temp >= list_value.Count) continue;
                                    object[] value = list_value[temp];

                               

                                    string ip_temp = $"192.168.{value[1].ObjectToString()}";

                                    // 加入非同步刷新任務
                                    tasks_refresh.Add(Task.Run(() =>
                                    {
                                        try
                                        {
                                            bool flag = H_Pannel_lib.Communication.EPD_RefreshCanvas(uDP_Class, ip_temp);
                                            if (!flag)
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
                                    }));
                                }

                                // 等待所有刷新任務完成
                                Task.WhenAll(tasks_refresh).Wait();

                                // 每次分批繪圖與刷新後休息 3 秒
                                Console.WriteLine($"Sleep 1s....");
                                System.Threading.Thread.Sleep(1000);
                                Console.WriteLine($"Sleep 2s....");
                                System.Threading.Thread.Sleep(1000);
                                Console.WriteLine($"Sleep 3s....");
                                System.Threading.Thread.Sleep(1000);
                            }
                        }
                    }

                    // index 為圖片版本切換控制變數（兩種圖片輪替）
                    index++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception : {ex.Message}");
                Console.ReadLine(); // 保持主控台視窗開啟
            }
        }
    }
}
