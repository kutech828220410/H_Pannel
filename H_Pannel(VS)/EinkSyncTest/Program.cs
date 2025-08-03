using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using H_Pannel_lib; // 假設 ApplyFloydSteinbergDithering 在此命名空間

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
                defaultIP = "192.168.0.20";
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
                        using (Bitmap bmp_buf_ = Communication.ScaleImage(bmp, 800, 480))
                        using (Bitmap bmp_buf = bmp_buf_.ApplyFloydSteinbergDithering(DitheringProcessor.DitheringMode.SixColor))
                        {

                            {
                                string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                                string fileName = $"dithered_6color_{DateTime.Now:yyyyMMdd_HHmmss}.bmp";
                                string savePath = Path.Combine(desktop, fileName);

                                //bmp_buf_.Save(savePath, System.Drawing.Imaging.ImageFormat.Bmp);
                                Console.WriteLine($"✅ 已儲存處理圖片到桌面：{savePath}");
                                UDP_Class uDP_Class = new UDP_Class("192.168.0.20", 29005, false);

                                Communication.EPD_730E_DrawImage(uDP_Class, ip, bmp);

                                ShowImage(new Bitmap(bmp_buf)); // 顯示處理後圖片
                                                                 // 儲存處理後圖片到桌面
                            }


                        }

                        // 範例傳送，可自行開啟
                        //UDP_Class udp = new UDP_Class("192.168.5.250", 29000 , false);
                        //bool success = Communication.EPD_579G_DrawImage(udp, ip, bmp_buf);
                        //Console.WriteLine(success ? "✅ 傳送成功！" : "❌ 傳送失敗！");
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
                ofd.Filter = "圖片檔案 (*.bmp;*.jpg;*.jpeg;*.png)|*.bmp;*.jpg;*.jpeg;*.png";
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

        static void ShowImage(Bitmap bmp)
        {
            Form form = new Form
            {
                Text = "圖片預覽",
                Width = bmp.Width + 40,
                Height = bmp.Height + 60,
                StartPosition = FormStartPosition.CenterScreen
            };

            PictureBox pictureBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                Image = bmp,
                SizeMode = PictureBoxSizeMode.Zoom
            };

            form.Controls.Add(pictureBox);
            Application.Run(form); // 阻塞直到關閉視窗
        }
    }
}
