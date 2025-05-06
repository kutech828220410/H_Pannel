using System;
using System.Drawing;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Net.NetworkInformation;
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
                List<DataTable> dataTables = MyOffice.ExcelClass.NPOI_LoadFile2DataTables($"{desktopPath}\\desk48.xlsx");
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
                        string filename = value[0].ToString();
                        string ip = $"192.168.{value[1]}";
                        int current = System.Threading.Interlocked.Increment(ref globalIndex);
                        tasks.Add(Task.Run(() =>
                        {
                            try
                            {
                                if (!Ping(ip, 2, 500))
                                {
                                    Console.WriteLine($"{ip} ❌ Ping 失敗，略過刷新 ({current}/{totalDrawCount})");
                                    Logger.Log(ip, "Ping 失敗，略過刷新", current, totalDrawCount);
                                    return;
                                }

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
                                return;
                            }

                            try
                            {
                                bool result = Communication.EPD_SPIdata_and_RefreshCanvas(uDP_Class, ip);
                                Console.WriteLine(result ? $"{ip} ✅ Refresh OK ({current}/{totalDrawCount})" : $"{ip} ❌ Refresh FAIL ({current}/{totalDrawCount})");
                                if (!result) Logger.Log(ip, "Refresh FAIL", current, totalDrawCount);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"{ip} Exception: {ex.Message} ({current}/{totalDrawCount})");
                                Logger.Log(ip, $"Exception: {ex.Message}", current, totalDrawCount);
                            }
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
            string[] wallTables = { "花蓮", "台東", "高雄", "台南", "台北左", "台北右", "台中", "嘉義", "馬祖", "街廓學" };
            string[] tableTables = { };

            if (wallTables.Contains(tableName)) return flag_wall_refresh;
            if (tableTables.Contains(tableName)) return flag_table_refresh;
            return false;
        }

        public static bool Ping(string IP, int retrynum, int timeout)
        {
            Ping ping = new Ping();
            for (int i = 0; i < retrynum; i++)
            {
                try
                {
                    PingReply reply = ping.Send(IP, timeout);
                    if (reply.Status == IPStatus.Success)
                    {
                        return true;
                    }
                }
                catch
                {
                    // Ignore and retry
                }
            }
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
