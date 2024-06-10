using System;

namespace AcousticWavePropagationSimulation
{
    public static class Globals
    {
        public static int SampleRate;
        public static float AmplitudeScaling = 1;

        /// <summary>
        /// Propagation speed of an acoustic wave in air, in m/s
        /// </summary>
        public static int PropagationSpeed = 3400;

        public static double PressureLevelToDBs(double powerLevel)
        {
            return 10 * Math.Log10(powerLevel / Constants.ReferenceSoundPressureLevel);
        }
    }
}
