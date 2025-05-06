using System;
using System.Drawing;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using H_Pannel_lib;
using MyOffice;
using Basic;
using System.Linq;
using System.Text;

namespace EInkSync589
{
    public static class Logger
    {
        private static readonly string logFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
        private static readonly string logFilePath;
        private static readonly object _lock = new object();

        static Logger()
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            if (!Directory.Exists(logFolder)) Directory.CreateDirectory(logFolder);
            logFilePath = Path.Combine(logFolder, $"log_{timestamp}.txt");
        }

        public static void Log(string message)
        {
            WriteLine($"[INFO] {message}");
        }

        public static void Log(string ip, string message, int current, int total)
        {
            string line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ({current}/{total}) {ip} {message}";
            WriteLine(line);
        }

        private static void WriteLine(string line)
        {
            lock (_lock)
            {
                try
                {
                    File.AppendAllText(logFilePath, line + Environment.NewLine);
                }
                catch
                {
                    // 防止 log 寫入錯誤影響主程式
                }
            }
        }
    }

    class Program
    {
        static string ServerIP = "192.168.5.250";
        static string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        static bool flag_wall_refresh = true;
        static bool flag_table_refresh = false;

        static void Main(string[] args)
        {
            Communication.ConsoleWrite = false;
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            try
            {
                UDP_Class uDP_Class = new UDP_Class(ServerIP, 29000);
                List<DataTable> dataTables = MyOffice.ExcelClass.NPOI_LoadFile2DataTables($"{desktopPath}\\wall46-47.xlsx");
                Dictionary<string, List<object[]>> tableDeviceMap = new Dictionary<string, List<object[]>>();

                int totalDrawCount = 0;
                foreach (var table in dataTables)
                {
                    if (!ShouldProcessTable(table.TableName)) continue;
                    var list = table.DataTableToRowList();
                    tableDeviceMap[table.TableName] = list;
                    totalDrawCount += list.Count;
                }

                int globalIndex = 0;
                List<Task> tasks = new List<Task>();

                foreach (var kvp in tableDeviceMap)
                {
                    var list = kvp.Value;
                    foreach (var value in list)
                    {
                        tasks.Add(Task.Run(() =>
                        {
                            int current = System.Threading.Interlocked.Increment(ref globalIndex);
                            string filename = value[0].ToString();
                            string ip = $"192.168.{value[1]}";

                            try
                            {
                                using (Bitmap bmp = new Bitmap(filename))
                                {
                                    bool result = Communication.EPD_579G_DrawFramebuffer(uDP_Class, ip, bmp);
                                    Console.WriteLine(result ? $"{ip} ✅ Draw OK ({current}/{totalDrawCount})" : $"{ip} ❌ Draw FAIL ({current}/{totalDrawCount})");
                                    if (!result) Logger.Log(ip, "Draw FAIL", current, totalDrawCount);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"{ip} Exception: {ex.Message} ({current}/{totalDrawCount})");
                                Logger.Log(ip, $"Exception: {ex.Message}", current, totalDrawCount);
                            }

                            try
                            {
                                bool result = Communication.EPD_RefreshCanvas(uDP_Class, ip);
                                Console.WriteLine(result ? $"{ip} ✅ Refresh OK ({current}/{totalDrawCount})" : $"{ip} ❌ Refresh FAIL ({current}/{totalDrawCount})");
                                if (!result) Logger.Log(ip, "Refresh FAIL", current, totalDrawCount);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"{ip} Exception: {ex.Message} ({current}/{totalDrawCount})");
                                Logger.Log(ip, $"Exception: {ex.Message}", current, totalDrawCount);
                            }

                            System.Threading.Thread.Sleep(100); // Optional delay between each device
                        }));
                    }
                }

                Task.WhenAll(tasks).Wait();
                Console.WriteLine("✅ 所有圖片上傳與刷新完成");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception : {ex.Message}");
                Logger.Log("SYSTEM", $"Critical Error: {ex.Message}", 0, 0);
                Console.ReadLine();
            }
        }

        static bool ShouldProcessTable(string tableName)
        {
            string[] wallTables = { "花蓮", "台東", "高雄", "台南", "台北左", "台北右" , "台中", "嘉義" };
            string[] tableTables = {  };

            if (wallTables.Contains(tableName)) return flag_wall_refresh;
            if (tableTables.Contains(tableName)) return flag_table_refresh;
            return false;
        }

        static int GetMaxDeviceCount(Dictionary<string, List<object[]>> tableDeviceMap)
        {
            int max = 0;
            foreach (var list in tableDeviceMap.Values)
            {
                if (list.Count > max) max = list.Count;
            }
            return max;
        }
    }
}
