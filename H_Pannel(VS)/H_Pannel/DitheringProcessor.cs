using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using ZXing.QrCode.Internal;

namespace H_Pannel_lib
{
    static public class DitheringProcessor
    {
        public enum DitheringMode
        {
            ThreeColor,
            FourColor,
            SixColor
        }
        public unsafe static Bitmap ApplyFloydSteinbergDithering(this Bitmap inputBmp, DitheringMode mode)
        {
            int width = inputBmp.Width;
            int height = inputBmp.Height;
            Bitmap outputBmp = new Bitmap(width, height, inputBmp.PixelFormat);

            Color[] palette;
            if (mode == DitheringMode.SixColor)
            {
                palette = new Color[] { Color.Red, Color.Green, Color.Blue, Color.Yellow, Color.White, Color.Black };
            }
            else if(mode == DitheringMode.FourColor)
            {
                palette = new Color[] { Color.Red, Color.White, Color.Black, Color.Yellow };
            }
            else
            {
                palette = new Color[] { Color.Red, Color.White, Color.Black };
            }


            try
            { 
                if(mode == DitheringMode.SixColor)
                {
                    return ApplyFloydSteinbergDitheringSixColor(inputBmp);
                }
                else
                {
                    BitmapData inputData = inputBmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                    BitmapData outputData = outputBmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
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
                                DistributeError(inputPtr, stride, x + 1, y, errorR, errorG, errorB, 7 / 16.0);
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

                            // Reduce red noise in yellow regions (specific to 4-color mode)
                            if (mode == DitheringMode.FourColor && nearestColor == Color.Red)
                            {
                                ReduceRedNoise(outputPtr, stride, x, y, width, height, Color.Yellow);
                            }
                        }
                    }
                    inputBmp.UnlockBits(inputData);
                    outputBmp.UnlockBits(outputData);
                }
             
            }
            finally
            {

            }

            return outputBmp;
        }

        public static Bitmap ApplyFloydSteinbergDitheringSixColor(this Bitmap inputBmp)
        {
            int width = inputBmp.Width;
            int height = inputBmp.Height;
            Bitmap outputBmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            Color[] palette = new Color[] { Color.Red, Color.Green, Color.Blue, Color.Yellow, Color.White, Color.Black };

            BitmapData inputData = inputBmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData outputData = outputBmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            unsafe
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

                        if (IsPureColor(originalColor, out Color mapped))
                        {
                            outputPtr[index] = mapped.B;
                            outputPtr[index + 1] = mapped.G;
                            outputPtr[index + 2] = mapped.R;
                            continue;
                        }


                        // 🎯 抖色
                        Color nearestColor = GetNearestColor(originalColor, palette);
                        outputPtr[index] = nearestColor.B;
                        outputPtr[index + 1] = nearestColor.G;
                        outputPtr[index + 2] = nearestColor.R;

                        int errorR = r - nearestColor.R;
                        int errorG = g - nearestColor.G;
                        int errorB = b - nearestColor.B;

                        // Diffuse error to neighbors
                        DiffuseError(inputPtr, stride, width, height, x + 1, y, errorR, errorG, errorB, 7.0 / 16);
                        DiffuseError(inputPtr, stride, width, height, x - 1, y + 1, errorR, errorG, errorB, 3.0 / 16);
                        DiffuseError(inputPtr, stride, width, height, x, y + 1, errorR, errorG, errorB, 5.0 / 16);
                        DiffuseError(inputPtr, stride, width, height, x + 1, y + 1, errorR, errorG, errorB, 1.0 / 16);
                    }
                }
            }

            inputBmp.UnlockBits(inputData);
            outputBmp.UnlockBits(outputData);
            return outputBmp;
        }

        public unsafe static void DistributeError(byte* ptr, int stride, int x, int y, int errorR, int errorG, int errorB, double factor)
        {
            int index = y * stride + x * 3;
            ptr[index] = (byte)Clamp(ptr[index] + (int)(errorB * factor), 0, 255);
            ptr[index + 1] = (byte)Clamp(ptr[index + 1] + (int)(errorG * factor), 0, 255);
            ptr[index + 2] = (byte)Clamp(ptr[index + 2] + (int)(errorR * factor), 0, 255);
        }
        private unsafe static void ReduceRedNoise(byte* ptr, int stride, int x, int y, int width, int height, Color targetColor)
        {
            int index = y * stride + x * 3;

            // Check neighbors
            int yellowCount = 0;
            if (x > 0 && IsColorMatch(ptr + index - 3, targetColor)) yellowCount++;
            if (x < width - 1 && IsColorMatch(ptr + index + 3, targetColor)) yellowCount++;
            if (y > 0 && IsColorMatch(ptr + index - stride, targetColor)) yellowCount++;
            if (y < height - 1 && IsColorMatch(ptr + index + stride, targetColor)) yellowCount++;

            // If surrounded by yellow, convert to yellow
            if (yellowCount >= 2)
            {
                ptr[index] = targetColor.B;
                ptr[index + 1] = targetColor.G;
                ptr[index + 2] = targetColor.R;
            }
        }
        private unsafe static bool IsColorMatch(byte* ptr, Color color)
        {
            return ptr[0] == color.B && ptr[1] == color.G && ptr[2] == color.R;
        }
        private static Color GetNearestColor(Color originalColor, Color[] palette)
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
        private static Color GetNearestColorRestrictRed(Color c, Color[] palette)
        {
            double brightness = 0.2126 * c.R + 0.7152 * c.G + 0.0722 * c.B;
            bool allowRed = brightness < 160 && c.R > 150 && (c.R - Math.Max(c.G, c.B)) > 60;

            Color nearest = palette[0];
            double minDist = double.MaxValue;

            foreach (var color in palette)
            {
                if (color == Color.Red && !allowRed) continue;

                double dist = 2 * Math.Pow(c.R - color.R, 2)
                            + 4 * Math.Pow(c.G - color.G, 2)
                            + 3 * Math.Pow(c.B - color.B, 2);

                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = color;
                }
            }

            return nearest;
        }
        private static bool IsPureColor(Color c, out Color mappedPure)
        {
            int threshold = 80; // 容許誤差範圍
            Color[] pureColors = new Color[]
            {
        Color.Red,     // #FF0000
        Color.Green,   // #00FF00
        Color.Blue,    // #0000FF
        Color.Yellow,  // #FFFF00
        Color.White,   // #FFFFFF
        Color.Black    // #000000
            };

            foreach (var pc in pureColors)
            {
                int dr = Math.Abs(c.R - pc.R);
                int dg = Math.Abs(c.G - pc.G);
                int db = Math.Abs(c.B - pc.B);
                if (dr <= threshold && dg <= threshold && db <= threshold)
                {
                    mappedPure = pc;
                    return true;
                }
            }

            mappedPure = Color.Empty;
            return false;
        }
        private unsafe static void DiffuseError(byte* ptr, int stride, int width, int height, int x, int y, int errR, int errG, int errB, double factor)
        {
            if (x < 0 || x >= width || y < 0 || y >= height) return;

            int idx = y * stride + x * 3;
            ptr[idx] = (byte)Clamp(ptr[idx] + (int)(errB * factor), 0, 255);
            ptr[idx + 1] = (byte)Clamp(ptr[idx + 1] + (int)(errG * factor), 0, 255);
            ptr[idx + 2] = (byte)Clamp(ptr[idx + 2] + (int)(errR * factor), 0, 255);
        }



        private static Color GetNearestColor(Color originalColor, Color[] palette, bool avoidRed)
        {
            Color nearestColor = palette[0];
            double minDistance = double.MaxValue;

            foreach (var color in palette)
            {
                if (color == Color.Red && avoidRed)
                {
                    // 進階判斷：只在滿足紅色特徵時允許
                    if (originalColor.R < 150) continue; // 紅色亮度不夠
                    if (originalColor.G > 100 || originalColor.B > 100) continue; // 飽和度不足
                    if (originalColor.R - Math.Max(originalColor.G, originalColor.B) < 60) continue; // 紅色差異不夠明顯
                }

                // 加權距離計算（符合人眼感知）
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

        private static void Distribute(double[,,] buffer, int width, int height, int x, int y, double errR, double errG, double errB, double factor)
        {
            if (x < 0 || x >= width || y < 0 || y >= height) return;

            buffer[x, y, 0] += errR * factor;
            buffer[x, y, 1] += errG * factor;
            buffer[x, y, 2] += errB * factor;
        }

        private static int Clamp(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

    }
}
