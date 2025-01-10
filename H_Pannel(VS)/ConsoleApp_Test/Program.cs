using System;
using System.Drawing;
using System.Drawing.Imaging;
using H_Pannel_lib;
public class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("請輸入影像檔案路徑:");
        string inputPath = Console.ReadLine();
        inputPath = @"C:\Users\Administrator\Downloads\01.bmp";

        try
        {
            Console.WriteLine("選擇模式 (1: 紅白黑, 2: 紅白黑黃):");
            int mode = int.Parse(Console.ReadLine());
            DitheringProcessor.DitheringMode ditheringMode = (mode == 1) ? DitheringProcessor.DitheringMode.ThreeColor : DitheringProcessor.DitheringMode.FourColor;
            Bitmap inputBmp = new Bitmap(inputPath);
            inputBmp = H_Pannel_lib.Communication.ScaleImage(inputBmp, 198 * 4, 272);
            //Bitmap outputBmp = DitheringProcessor.ApplyFloydSteinbergDithering(inputBmp, ditheringMode);


            //// 顯示影像
            //string tempOutputPath = "dithered_image.bmp";
            ////outputBmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
            ////outputBmp.RotateFlip(RotateFlipType.Rotate180FlipY);
            //outputBmp.Save(tempOutputPath, ImageFormat.Bmp);
            //System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            //{
            //    FileName = tempOutputPath,
            //    UseShellExecute = true
            //});
            //byte[] bytes = new byte[0];
            Communication.ConsoleWrite = true;

            UDP_Class uDP_Class = new UDP_Class("192.168.5.50", 29000);
            
            H_Pannel_lib.Communication.EPD_579G_DrawFramebuffer(uDP_Class, "192.168.43.251", inputBmp);
            //H_Pannel_lib.Communication.Bi
            //tmapToByte(outputBmp, ref bytes, H_Pannel_lib.EPD_Type.EPD213_BRW_V0);
            //string str = H_Pannel_lib.Communication.BytesToHexString(bytes);

            Console.WriteLine("影像處理完成");
            Console.ReadKey();
            H_Pannel_lib.Communication.EPD_RefreshCanvas(uDP_Class, "192.168.43.251");


        }
        catch (Exception ex)
        
        {
            Console.WriteLine($"處理失敗: {ex.Message}");
        }
    }




}
