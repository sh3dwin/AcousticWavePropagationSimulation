using AcousticWavePropagationSimulation.DataStructures;
using AcousticWavePropagationSimulation.Utils;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

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
                        var propagationLoss = 1.0 / distanceToSoundSource;
                        var attenuationLoss = Globals.CalculateAttenuationLoss(distanceToSoundSource);
                        var soundSourcePressureLevelAtPosition = soundSource.PressureLevel * propagationLoss * attenuationLoss;

                        pressure += soundSourcePressureLevelAtPosition;
                    }
                    var dBFromSoundSource = Globals.PressureLevelToDeciBells(pressure);

                    maxPressureField[col, row] = dBFromSoundSource;
                }
            }

            //maxPressureField[0, 0] = Constants.MaximumDBs;

            var normalizedValues = maxPressureField.NormalizeValuesMinMax();

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
