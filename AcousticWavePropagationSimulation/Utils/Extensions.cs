using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace AcousticWavePropagationSimulation.Utils
{
    public static class Extensions
    {
        public static Dictionary<Vector2, double> NormalizeValues(this IDictionary<Vector2, double> dictionary)
        {
            var max = double.MinValue;

            foreach (var pair in dictionary)
            {
                if(Math.Abs(pair.Value) > max)
                    max = Math.Abs(pair.Value);
            }

            if (max == 0)
                max = 1;

            var tempDictionary = new Dictionary<Vector2, double>(dictionary.Count);

            foreach(var key in dictionary.Keys)
            {
                tempDictionary[key] = dictionary[key] / max;
            }

            return tempDictionary;
        }

        public static double[] NormalizeValues(this double[] array)
        {
            var max = double.MinValue;

            foreach (var value in array)
            {
                if (Math.Abs(value) > max)
                    max = Math.Abs(value);
            }

            if (max == 0)
                max = 1;

            var result = new double[array.Length];

            for (var i = 0; i < result.Length; i++)
            {
                result[i] = array[i] / max;
            }

            return result;
        }

        public static double[,] NormalizeValues(this double[,] array)
        {
            var max = double.MinValue;

            foreach (var value in array)
            {
                if (Math.Abs(value) > max)
                    max = Math.Abs(value);
            }

            if (max == 0.0)
                max = 1.0;

            var result = new double[array.GetLength(0), array.GetLength(1)];

            for (var i = 0; i < result.GetLength(0); i++)
            {
                for(var j = 0; j < result.GetLength(1); j++)
                    result[i, j] = array[i, j] / max;
            }

            return result;
        }

        public static double Clamp(this double value, double min = 0, double max = 1)
        {
            return Math.Min(max, Math.Max(value, min));
        }

        public static float Clamp(this float value, float min = 0, float max = 1)
        {
            return Math.Min(max, Math.Max(value, min));
        }

        public static int Clamp(this int value, int min = 0, int max = 1)
        {
            return Math.Min(max, Math.Max(value, min));
        }

    }
}
