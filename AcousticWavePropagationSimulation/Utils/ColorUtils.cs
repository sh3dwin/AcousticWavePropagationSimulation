using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcousticWavePropagationSimulation.Utils
{
    public static class ColorUtils
    {
        public static void HSVToRGB(int h, int s, int v, out int r, out int g, out int b)
        {
            var rgb = new int[3];

            var baseColor = (h + 60) % 360 / 120;
            var shift = (h + 60) % 360 - (120 * baseColor + 60);
            var secondaryColor = (baseColor + (shift >= 0 ? 1 : -1) + 3) % 3;

            //Setting Hue
            rgb[baseColor] = 255;
            rgb[secondaryColor] = (int)((Math.Abs(shift) / 60.0f) * 255.0f);

            //Setting Saturation
            for (var i = 0; i < 3; i++)
                rgb[i] += (int)((255 - rgb[i]) * ((100 - s) / 100.0f));

            //Setting Value
            for (var i = 0; i < 3; i++)
                rgb[i] -= (int)(rgb[i] * (100 - v) / 100.0f);

            r = rgb[0];
            g = rgb[1];
            b = rgb[2];
        }
    }
}
