using AcousticWavePropagationSimulation.DataStructures;
using AcousticWavePropagationSimulation.Utils;
using NAudio.Utils;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace AcousticWavePropagationSimulation.Visualizer
{
    public static class DeciBellVisualizer
    {
        public static void Draw(IEnumerable<SoundSource> soundSources, DirectBitmap renderBuffer)
        {
            var maxPressureField = new double[(int)renderBuffer.Width, (int)renderBuffer.Height];

            for(var row = 0; row < renderBuffer.Height; row++)
            {
                for(var col = 0; col < renderBuffer.Width; col++)
                {
                    var currentPosition = new Vector2(col, row);

                    var pressure = 0.0;

                    foreach(var soundSource in soundSources)
                    {
                        var distanceToSoundSource = Vector2.Distance(soundSource.Position, currentPosition).Clamp(1, float.MaxValue);
                        var loss = 1.0 / distanceToSoundSource;
                        var soundSourcePressureLevelAtPosition = soundSource.PressureLevel * loss;

                        pressure += soundSourcePressureLevelAtPosition;
                    }
                    var dBFromSoundSource = Globals.PressureLevelToDBs(pressure);

                    maxPressureField[col, row] = dBFromSoundSource;
                }
            }

            var normalizedValues = maxPressureField.NormalizeValues();

            var asd = new double[10, 20];

            var len = asd.GetLength(0);



            for (var row = 0; row < renderBuffer.Height; row++)
            {
                for (var col = 0; col < renderBuffer.Width; col++)
                {
                    var value = normalizedValues[col, row] * 360;
                    var color = ColorUtils.HsvToRgb(value, 1, 1);
                    try
                    {
                        renderBuffer.SetPixel(col, row, Color.FromArgb(color.r, color.g, color.b));
                    }
                    catch { }
                }
            }

            

        } 
    }
}
