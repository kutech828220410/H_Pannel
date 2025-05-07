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
using System.Threading;
using NPOI.SS.Formula.Functions;

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
                catch { }
            }
        }
    }

    class Program
    {
        static string ServerIP = "192.168.5.250";
        private static System.Threading.Mutex mutex;
        static string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        static bool flag_wall_refresh = true;
        static bool flag_table_refresh = false;

        static void Main(string[] args)
        {
            mutex = new System.Threading.Mutex(true, "EinkSync579_desk");
            if (mutex.WaitOne(0, false))
            {

            }
            else
            {

                return;
            }
            Communication.ConsoleWrite = false;
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            string folderPath = @"C:\Users\miniPC\Desktop\電子紙輪播\電子紙_桌面輪播_bmp";
            string[] sheetNames = { "48.10", "48.91" };
            int intervalSeconds = 60;
            int loopTimes = 0;
            int maxRetry = 3; // ✅ 重試次數設定

            UDP_Class uDP_Class = new UDP_Class(ServerIP, 29000, false);
            var allFiles = Directory.GetFiles(folderPath, "*.xlsx");

            try
            {
                int currentLoop = 0;

                while (loopTimes == 0 || currentLoop < loopTimes)
                {
                    currentLoop++;
                    Console.WriteLine($"\n🔁 第 {currentLoop} 輪播放開始");

                    foreach (string filePath in allFiles)
                    {
                        Console.WriteLine($"\n📄 開始處理檔案：{Path.GetFileName(filePath)}");

                        Dictionary<string, List<object[]>> tableDeviceMap = new Dictionary<string, List<object[]>>();
                        List<DataTable> dataTables = MyOffice.ExcelClass.NPOI_LoadFile2DataTables(filePath);

                        int totalDrawCount = 0;
                        foreach (var table in dataTables)
                        {
                            if (!sheetNames.Contains(table.TableName)) continue;

                            var list = table.DataTableToRowList();
                            tableDeviceMap[table.TableName] = list;
                            totalDrawCount += list.Count;
                        }

                        var successUploadIPs = new System.Collections.Concurrent.ConcurrentBag<string>();
                        int globalIndex = 0;
                        List<Task> drawTasks = new List<Task>();

                        foreach (var kvp in tableDeviceMap)
                        {
                            foreach (var value in kvp.Value)
                            {
                                string filename = value[0].ToString();
                                string ip = $"192.168.{value[1]}";
                                int current = System.Threading.Interlocked.Increment(ref globalIndex);

                                drawTasks.Add(Task.Run(() =>
                                {
                                    try
                                    {
                                        if (!Ping(ip, 3, 500))
                                        {
                                            Console.WriteLine($"{ip} ❌ Ping 失敗，略過上傳 ({current}/{totalDrawCount})");
                                            Logger.Log(ip, "Ping 失敗，略過上傳", current, totalDrawCount);
                                            return;
                                        }

                                        bool result = RetryDraw(uDP_Class, ip, filename, maxRetry);
                                        Console.WriteLine(result ? $"{ip} ✅ Draw OK ({current}/{totalDrawCount})" : $"{ip} ❌ Draw FAIL ({current}/{totalDrawCount})");
                                        if (result) successUploadIPs.Add(ip);
                                        else Logger.Log(ip, $"Draw FAIL (retry exhausted)", current, totalDrawCount);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"{ip} Exception during Draw: {ex.Message} ({current}/{totalDrawCount})");
                                        Logger.Log(ip, $"Draw Exception: {ex.Message}", current, totalDrawCount);
                                    }
                                }));
                            }
                        }

                        Task.WhenAll(drawTasks).Wait();
                        Console.WriteLine("✅ 所有圖片上傳完成");

                        int refreshIndex = 0;
                        List<Task> refreshTasks = new List<Task>();
                        foreach (var ip in successUploadIPs.Distinct())
                        {
                            int current = System.Threading.Interlocked.Increment(ref refreshIndex);
                            refreshTasks.Add(Task.Run(() =>
                            {
                                try
                                {
                                    bool result = RetryRefresh(uDP_Class, ip, maxRetry);
                                    Console.WriteLine(result ? $"{ip} ✅ Refresh OK ({current}/{successUploadIPs.Count})" : $"{ip} ❌ Refresh FAIL ({current}/{successUploadIPs.Count})");
                                    if (!result) Logger.Log(ip, $"Refresh FAIL (retry exhausted)", current, successUploadIPs.Count);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"{ip} Exception during Refresh: {ex.Message} ({current}/{successUploadIPs.Count})");
                                    Logger.Log(ip, $"Refresh Exception: {ex.Message}", current, successUploadIPs.Count);
                                }
                            }));
                        }

                        Task.WhenAll(refreshTasks).Wait();
                        Console.WriteLine("✅ 所有裝置刷新完成");

                        Console.WriteLine($"⏳ 等待 {intervalSeconds} 秒...");
                        Thread.Sleep(intervalSeconds * 1000);
                    }

                    Console.WriteLine($"✅ 第 {currentLoop} 輪播放完成");
                }

                Console.WriteLine("🎉 所有播放輪數完成，程式結束");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception : {ex.Message}");
                Logger.Log("SYSTEM", $"Critical Error: {ex.Message}", 0, 0);
                Console.ReadLine();
            }
        }

        public static bool Ping(string IP, int retrynum, int timeout)
        {
            Ping ping = new Ping();
            for (int i = 0; i < retrynum; i++)
            {
                try
                {
                    PingReply reply = ping.Send(IP, timeout);
                    if (reply.Status == IPStatus.Success) return true;
                }
                catch { }
            }
            return false;
        }

        public static bool RetryDraw(UDP_Class udp, string ip, string filename, int maxRetry)
        {
            for (int attempt = 1; attempt <= maxRetry; attempt++)
            {
                try
                {
                    using (Bitmap bmp = new Bitmap(filename))
                    {
                        if (Communication.EPD_579G_DrawFramebuffer(udp, ip, bmp)) return true;
                    }
                }
                catch { }
                Thread.Sleep(300);
            }
            return false;
        }

        public static bool RetryRefresh(UDP_Class udp, string ip, int maxRetry)
        {
            for (int attempt = 1; attempt <= maxRetry; attempt++)
            {
                try
                {
                    if (Communication.EPD_SPIdata_and_RefreshCanvas(udp, ip)) return true;
                }
                catch { }
                Thread.Sleep(300);
            }
            return false;
        }
    }
}
