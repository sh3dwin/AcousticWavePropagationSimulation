﻿using AcousticWavePropagationSimulation.DataStructures;
using AcousticWavePropagationSimulation.Utils;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AcousticWavePropagationSimulation.Visualizer
{
    public static class MonochromeVisualizer
    {
        public static void Draw(List<List<double>> particleAmplitudes, ref WriteableBitmap renderBuffer)
        {
            if (particleAmplitudes.Count == 0)
                throw new Exception("No information provided!");
            if (renderBuffer.Width < particleAmplitudes.Count || renderBuffer.Height < particleAmplitudes[0].Count)
            {
                throw new Exception("The information provided is larger than the canvas size");
            }

            try
            {
                // Reserve the back buffer for updates.
                renderBuffer.Lock();

                var hueShift = Math.Abs((int)(Math.Sin(DateTime.Now.Millisecond / 1000.0) * 360)) * 0;

                for (int row = 0; row < renderBuffer.Width; row++)
                {
                    var particleRow = particleAmplitudes[row];
                    for (int column = 0; column < renderBuffer.Height; column++)
                    {
                        unsafe
                        {
                            // Get a pointer to the back buffer.
                            IntPtr pBackBuffer = renderBuffer.BackBuffer;

                            // Find the address of the pixel to draw.
                            pBackBuffer += row * renderBuffer.BackBufferStride;
                            pBackBuffer += column * 4;

                            var amplitude = particleAmplitudes[row][column];
                            var hue = (int)((amplitude / 2.0 + 0.5) * 360);

                            ColorUtils.HSVToRGB((int)((hue + hueShift) % 360), 100, 100, out var r, out var g, out var b);

                            var saturation = (int)(128 + 128 * amplitude);

                            // Compute the pixel's color.
                            int color_data = saturation << 16; // R
                            color_data |= saturation << 8;   // G
                            color_data |= saturation << 0;   // B

                            // Assign the color data to the pixel.
                            *((int*)pBackBuffer) = color_data;
                        }

                        // Specify the area of the bitmap that changed.
                        renderBuffer.AddDirtyRect(new Int32Rect(column, row, 1, 1));

                    }
                }
            }
            finally
            {
                // Release the back buffer and make it available for display.
                renderBuffer.Unlock();
            }
        }

        public static void Draw(List<List<double>> particleAmplitudes, ref DirectBitmap renderBuffer, int hueShift)
        {
            if (particleAmplitudes.Count == 0)
                throw new Exception("No information provided!");
            if (renderBuffer.Width < particleAmplitudes.Count || renderBuffer.Height < particleAmplitudes[0].Count)
            {
                throw new Exception("The information provided is larger than the canvas size");
            }

            for (int row = 0; row < renderBuffer.Width; row++)
            {
                for (int column = 0; column < renderBuffer.Height; column++)
                {
                    var amplitude = particleAmplitudes[row][column];

                    var saturation = (int)(128 + 128 * amplitude);

                    var color = System.Drawing.Color.FromArgb(saturation, saturation, saturation);

                    // Specify the area of the bitmap that changed.
                    renderBuffer.SetPixel(column, row, color);

                }
            }

        }
    }
}
