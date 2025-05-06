using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms; // 需在專案加入 System.Windows.Forms 引用
using H_Pannel_lib;
using System.Text;

namespace EinkSyncConsole
{
    internal class Program
    {
        static string configPath = "last_ip.txt";

        [STAThread]
        static void Main(string[] args)
        {
            Communication.ConsoleWrite = true;
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            while (true)
            {
                string defaultIP = LoadLastIP();
                Console.Clear();
                Console.WriteLine($"上次使用的 IP：{defaultIP}");
                Console.Write("請輸入目標 IP（按 Enter 使用預設）：");
                string inputIP = Console.ReadLine().Trim();
                string ip = string.IsNullOrWhiteSpace(inputIP) ? defaultIP : inputIP;

                Console.WriteLine("請選擇圖片檔案...");
                string imagePath = OpenImageFileDialog();

                if (string.IsNullOrEmpty(imagePath) || !File.Exists(imagePath))
                {
                    Console.WriteLine("❌ 圖片檔案無效，請按任意鍵重新開始...");
                    Console.ReadKey();
                    continue;
                }

                try
                {
                    using (Bitmap bmp = (Bitmap)Bitmap.FromFile(imagePath))
                    {
                        UDP_Class udp = new UDP_Class("192.168.5.250", 29000 , false); // 固定主機端 Server IP
                        bool success = Communication.EPD_579G_DrawImage(udp, ip, bmp);
                        Console.WriteLine(success ? "✅ 傳送成功！" : "❌ 傳送失敗！");
                    }

                    SaveLastIP(ip);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ 發生錯誤：{ex.Message}");
                }

                Console.WriteLine();
                Console.Write("按 Enter 重新執行，輸入 q 離開：");
                string again = Console.ReadLine().Trim();
                if (again.Equals("q", StringComparison.OrdinalIgnoreCase))
                    break;
            }
        }

        static string OpenImageFileDialog()
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Bitmap 圖片 (*.bmp)|*.bmp";
                ofd.Title = "選擇電子紙圖片檔案";
                return ofd.ShowDialog() == DialogResult.OK ? ofd.FileName : "";
            }
        }

        static string LoadLastIP()
        {
            if (File.Exists(configPath))
                return File.ReadAllText(configPath).Trim();
            return "192.168.46.20"; // 預設 IP
        }

        static void SaveLastIP(string ip)
        {
            File.WriteAllText(configPath, ip);
        }
    }
}
