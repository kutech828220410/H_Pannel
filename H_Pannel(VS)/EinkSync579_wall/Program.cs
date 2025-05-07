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
        static void Main(string[] args)
        {
            mutex = new System.Threading.Mutex(true, "EinkSync579_wall");
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

            Console.WriteLine("🔁 電子紙牆面輪播開始，按下 Q 可隨時結束輪播...\n");

            string folderPath = @"C:\\Users\\miniPC\\Desktop\\電子紙輪播\\電子紙_牆面輪播_bmp";
            string[] sheetNames = { "台南", "高雄", "台東", "花蓮", "台北左", "台北右", "台中", "嘉義" };
            int groupSizePerSheet = 14;
            int totalGroupSize = 112;
            int intervalSeconds = 2;
            int loopTimes = 0;
            int maxRetry = 3;

            UDP_Class uDP_Class = new UDP_Class(ServerIP, 29000, false);
            string[] allFiles = Directory.GetFiles(folderPath, "*.xlsx");

            int currentLoop = 0;
            bool exitRequested = false;

            while (!exitRequested && (loopTimes == 0 || currentLoop < loopTimes))
            {
                currentLoop++;
                Console.WriteLine($"\n🔁 第 {currentLoop} 輪牆面輪播開始");

                foreach (string filePath in allFiles)
                {
                    if (exitRequested) break;

                    string fileName = Path.GetFileName(filePath);
                    Console.WriteLine($"\n📄 處理檔案：{fileName}");

                    if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Q)
                    {
                        Console.WriteLine("\n🛑 偵測到 Q 鍵，結束輪播");
                        exitRequested = true;
                        break;
                    }

                    List<DataTable> dataTables = MyOffice.ExcelClass.NPOI_LoadFile2DataTables(filePath);
                    Dictionary<string, int> sheetOffsets = sheetNames.ToDictionary(name => name, name => 0);
                    List<(string ip, string filename, string sourceFile)> deviceData = new List<(string, string, string)>();

                    while (true)
                    {
                        var roundData = new List<(string, string, string)>();
                        foreach (string sheet in sheetNames)
                        {
                            var table = dataTables.FirstOrDefault(t => t.TableName == sheet);
                            if (table == null) continue;
                            var offset = sheetOffsets[sheet];
                            var rows = table.DataTableToRowList().Skip(offset).Take(groupSizePerSheet).ToList();
                            if (rows.Count == 0) continue;

                            foreach (var row in rows)
                            {
                                string bmpFilename = row[0].ToString();
                                string ip = $"192.168.{row[1]}";
                                roundData.Add((ip, bmpFilename, fileName));
                            }
                            sheetOffsets[sheet] += groupSizePerSheet;
                        }

                        if (roundData.Count == 0) break;
                        deviceData.AddRange(roundData);
                    }

                    Console.WriteLine($"📤 圖片總上傳數：{deviceData.Count}");
                    List<(string ip, string filename, string sourceFile)> successUploadList = new List<(string, string, string)>();
                    int uploadIndex = 0;
                    Parallel.ForEach(deviceData, data =>
                    {
                        int current = Interlocked.Increment(ref uploadIndex);
                        Console.WriteLine($"[{current}/{deviceData.Count}] 上傳 {data.ip} - {Path.GetFileName(data.filename)}");


                        if (!Ping(data.ip, 3, 500))
                        {
                            Logger.Log(data.ip, "Ping 失敗", current, deviceData.Count);
                            return;
                        }
                        if (RetryDraw(uDP_Class, data.ip, data.filename, maxRetry, current, deviceData.Count))
                        {
                            lock (successUploadList) successUploadList.Add(data);
                        }
                        else
                        {
                            Logger.Log(data.ip, $"Draw FAIL (來源:{data.sourceFile})", current, deviceData.Count);
                        }
                    });
                    successUploadList = ReorderBySegment(successUploadList);
                    Console.WriteLine($"🔃 開始刷新，共 {Math.Ceiling(successUploadList.Count / (double)totalGroupSize)} 組");
                    for (int i = 0; i < successUploadList.Count; i += totalGroupSize)
                    {
                        var group = successUploadList.Skip(i).Take(totalGroupSize).ToList();
                        int refreshIndex = 0;

                        Parallel.ForEach(group, item =>
                        {
                            int current = Interlocked.Increment(ref refreshIndex);
                            Console.WriteLine($"[{current}/{group.Count}] 刷新 {item.ip}");
                            if (!RetryRefresh(uDP_Class, item.ip, maxRetry, current, group.Count))
                            {
                                Logger.Log(item.ip, $"Refresh FAIL (來源:{item.sourceFile})", current, group.Count);
                            }
                        });

                        Console.WriteLine($"✅ 一組裝置刷新完成，等待 {intervalSeconds} 秒...\n");
                        Thread.Sleep(intervalSeconds * 1000);
                    }

                    Console.WriteLine($"\n🆗 檔案 {fileName} 輪播完成\n");
                }

                if (!exitRequested)
                {
                    Console.WriteLine($"🏁 第 {currentLoop} 輪播放結束\n");
                }
            }

            Console.WriteLine("🎉 所有輪播完成！");
        }
        public static List<(string ip, string filename, string sourceFile)> ReorderBySegment(List<(string ip, string filename, string sourceFile)> inputList)
        {
            var grouped = inputList
                .GroupBy(x => x.ip.Split('.')[2])
                .Where(g => new[] { "40", "41", "42", "43", "44", "45", "46", "47" }.Contains(g.Key))
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderBy(x => int.Parse(x.ip.Split('.')[3])).ToList() // 用第四段轉成數字排序
                );

            var result = new List<(string ip, string filename, string sourceFile)>();
            bool hasMore = true;

            while (hasMore)
            {
                hasMore = false;
                foreach (string seg in new[] { "40", "41", "42", "43", "44", "45", "46", "47" })
                {
                    if (!grouped.ContainsKey(seg)) continue;
                    var list = grouped[seg];
                    var take = list.Take(12).ToList();
                    if (take.Count > 0)
                    {
                        result.AddRange(take);
                        grouped[seg] = list.Skip(12).ToList();
                        hasMore = true;
                    }
                }
            }

            return result;
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

        public static bool RetryDraw(UDP_Class udp, string ip, string filename, int maxRetry, int current = 0, int total = 0)
        {
            for (int attempt = 1; attempt <= maxRetry; attempt++)
            {
                try
                {
                    using (Bitmap bmp = new Bitmap(filename))
                    {
                        if (Communication.EPD_579G_DrawFramebuffer(udp, ip, bmp))
                        {
                            Console.WriteLine($"[{current}/{total}] ✅ Draw OK - {ip} (嘗試: {attempt})");
                            return true;
                        }
                    }
                }
                catch { }
                Thread.Sleep(300);
            }
            Console.WriteLine($"[{current}/{total}] ❌ Draw FAIL - {ip}");
            return false;
        }

        public static bool RetryRefresh(UDP_Class udp, string ip, int maxRetry, int current = 0, int total = 0)
        {
            for (int attempt = 1; attempt <= maxRetry; attempt++)
            {
                try
                {
                    if (Communication.EPD_SPIdata_and_RefreshCanvas(udp, ip))
                    {
                        Console.WriteLine($"[{current}/{total}] ✅ Refresh OK - {ip} (嘗試: {attempt})");
                        return true;
                    }
                }
                catch { }
                Thread.Sleep(300);
            }
            Console.WriteLine($"[{current}/{total}] ❌ Refresh FAIL - {ip}");
            return false;
        }
    }
}