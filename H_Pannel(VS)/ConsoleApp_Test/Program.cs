using System;
using System.Drawing;
using System.Drawing.Imaging;

public class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("請輸入影像檔案路徑:");
        string inputPath = Console.ReadLine();
        inputPath = @"C:\Users\User\Downloads\01.bmp";

        try
        {
            Console.WriteLine("選擇模式 (1: 紅白黑, 2: 紅白黑黃):");
            int mode = int.Parse(Console.ReadLine());

            Bitmap inputBmp = new Bitmap(inputPath);
            inputBmp = H_Pannel_lib.Communication.ScaleImage(inputBmp, 792, 272);
            Bitmap outputBmp = ApplyFloydSteinbergDithering(inputBmp, mode);
            Console.WriteLine("影像處理完成，按任意鍵顯示結果。");
            Console.ReadKey();

            // 顯示影像
            string tempOutputPath = "dithered_image.bmp";
            outputBmp.Save(tempOutputPath, ImageFormat.Bmp);
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = tempOutputPath,
                UseShellExecute = true
            });
            byte[] bytes = new byte[0];
            H_Pannel_lib.Communication.BitmapToByte(outputBmp, ref bytes, H_Pannel_lib.EPD_Type.EPD579G);
            string str = H_Pannel_lib.Communication.BytesToHexString(bytes);

        }
        catch (Exception ex)
        {
            Console.WriteLine($"處理失敗: {ex.Message}");
        }
    }

    unsafe static Bitmap ApplyFloydSteinbergDithering(Bitmap inputBmp, int mode)
    {
        int width = inputBmp.Width;
        int height = inputBmp.Height;
        Bitmap outputBmp = new Bitmap(width, height, inputBmp.PixelFormat);

        Color[] palette;
        if (mode == 1)
        {
            palette = new Color[] { Color.Red, Color.White, Color.Black };
        }
        else
        {
            palette = new Color[] { Color.Red, Color.White, Color.Black, Color.Yellow };
        }

        BitmapData inputData = inputBmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
        BitmapData outputData = outputBmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

        try
        {
            byte* inputPtr = (byte*)inputData.Scan0;
            byte* outputPtr = (byte*)outputData.Scan0;
            int stride = inputData.Stride;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * stride + x * 3;
                    byte b = inputPtr[index];
                    byte g = inputPtr[index + 1];
                    byte r = inputPtr[index + 2];

                    Color originalColor = Color.FromArgb(r, g, b);
                    Color nearestColor = GetNearestColor(originalColor, palette);

                    outputPtr[index] = nearestColor.B;
                    outputPtr[index + 1] = nearestColor.G;
                    outputPtr[index + 2] = nearestColor.R;

                    int errorR = r - nearestColor.R;
                    int errorG = g - nearestColor.G;
                    int errorB = b - nearestColor.B;

                    if (x + 1 < width)
                    {
                        DistributeError(inputPtr, stride, x + 1, y, errorR, errorG, errorB, 5 / 16.0);
                    }
                    if (x - 1 >= 0 && y + 1 < height)
                    {
                        DistributeError(inputPtr, stride, x - 1, y + 1, errorR, errorG, errorB, 3 / 16.0);
                    }
                    if (y + 1 < height)
                    {
                        DistributeError(inputPtr, stride, x, y + 1, errorR, errorG, errorB, 5 / 16.0);
                    }
                    if (x + 1 < width && y + 1 < height)
                    {
                        DistributeError(inputPtr, stride, x + 1, y + 1, errorR, errorG, errorB, 1 / 16.0);
                    }
                }
            }
        }
        finally
        {
            inputBmp.UnlockBits(inputData);
            outputBmp.UnlockBits(outputData);
        }

        return outputBmp;
    }

    unsafe static void DistributeError(byte* ptr, int stride, int x, int y, int errorR, int errorG, int errorB, double factor)
    {
        int index = y * stride + x * 3;
        ptr[index] = (byte)Clamp(ptr[index] + (int)(errorB * factor), 0, 255);
        ptr[index + 1] = (byte)Clamp(ptr[index + 1] + (int)(errorG * factor), 0, 255);
        ptr[index + 2] = (byte)Clamp(ptr[index + 2] + (int)(errorR * factor), 0, 255);
    }

    static Color GetNearestColor(Color originalColor, Color[] palette)
    {
        Color nearestColor = palette[0];
        double minDistance = double.MaxValue;

        foreach (var color in palette)
        {
            // Adjust weight for human perception
            double distance = 2 * Math.Pow(originalColor.R - color.R, 2) +
                              4 * Math.Pow(originalColor.G - color.G, 2) +
                              3 * Math.Pow(originalColor.B - color.B, 2);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestColor = color;
            }
        }

        return nearestColor;
    }

    static int Clamp(int value, int min, int max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }
}
