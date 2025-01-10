using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace H_Pannel_lib
{
    static public class DitheringProcessor
    {
        public enum DitheringMode
        {
            ThreeColor,
            FourColor
        }
        public unsafe static Bitmap ApplyFloydSteinbergDithering(Bitmap inputBmp, DitheringMode mode)
        {
            int width = inputBmp.Width;
            int height = inputBmp.Height;
            Bitmap outputBmp = new Bitmap(width, height, inputBmp.PixelFormat);

            Color[] palette;
            if (mode == DitheringMode.ThreeColor)
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
            }
            finally
            {
                inputBmp.UnlockBits(inputData);
                outputBmp.UnlockBits(outputData);
            }

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
        private static int Clamp(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
    }
}
