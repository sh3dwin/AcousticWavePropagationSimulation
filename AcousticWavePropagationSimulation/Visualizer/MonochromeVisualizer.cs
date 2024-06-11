using AcousticWavePropagationSimulation.DataStructures;
using AcousticWavePropagationSimulation.Utils;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Windows;
using System.Windows.Media.Imaging;

namespace AcousticWavePropagationSimulation.Visualizer
{
    public static class MonochromeVisualizer
    {
        public static void Draw(IDictionary<Vector2, double> particleAmplitudes, ref WriteableBitmap renderBuffer)
        {
            if (particleAmplitudes.Count == 0)
                throw new Exception("No information provided!");
            if (renderBuffer.Width * renderBuffer.Height < particleAmplitudes.Count)
            {
                throw new Exception("The information provided is larger than the canvas size");
            }

            try
            {
                // Reserve the back buffer for updates.
                renderBuffer.Lock();

                var hueShift = Math.Abs((int)(Math.Sin(DateTime.Now.Millisecond / 1000.0) * 360)) * 0;

                for (int row = 0; row < renderBuffer.Height; row++)
                {
                    for (int column = 0; column < renderBuffer.Width; column++)
                    {
                        unsafe
                        {
                            // Get a pointer to the back buffer.
                            IntPtr pBackBuffer = renderBuffer.BackBuffer;

                            // Find the address of the pixel to draw.
                            pBackBuffer += row * renderBuffer.BackBufferStride;
                            pBackBuffer += column * 4;

                            var amplitude = particleAmplitudes[new Vector2(column, row)];
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

        public static void Draw(IDictionary<Vector2, double> particleAmplitudes, ref DirectBitmap renderBuffer, int hueShift)
        {
            particleAmplitudes = particleAmplitudes.NormalizeValues();

            if (particleAmplitudes.Count == 0)
                throw new Exception("No information provided!");
            if (renderBuffer.Width * renderBuffer.Height < particleAmplitudes.Count)
            {
                throw new Exception("The information provided is larger than the canvas size");
            }

            var result = new Int32[renderBuffer.Width * renderBuffer.Height];

            for (int row = 0; row < renderBuffer.Height; row++)
            {
                for (int column = 0; column < renderBuffer.Width; column++)
                {
                    var index = row * renderBuffer.Width + column;

                    var color = System.Drawing.Color.Red.ToArgb();

                    renderBuffer.SetPixel(column, row, System.Drawing.Color.FromArgb((int)(255 * (hueShift / 360.0)), 0, 0));
                }
            }

            renderBuffer.Bits = result;
        }

        public static void Draw(double[] particleAmplitudes, ref DirectBitmap renderBuffer, int hueShift)
        {
            if (particleAmplitudes.Length < 1)
                throw new Exception("No information provided!");
            if (renderBuffer.Width * renderBuffer.Height < particleAmplitudes.Length)
            {
                throw new Exception("The information provided is larger than the canvas size");
            }

            particleAmplitudes = particleAmplitudes.NormalizeValues();

            for (var i = 0; i < particleAmplitudes.Length; i++)
            {
                var x = i % renderBuffer.Width;
                var y = i / renderBuffer.Width;

                var color = System.Drawing.Color.FromArgb((int)(Math.Abs(particleAmplitudes[i] * 255)), 0, 0);

                renderBuffer.SetPixel(x, y, color);
            }
        }
    }
}
