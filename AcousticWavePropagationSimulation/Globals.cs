using System;
using System.Numerics;

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


        public static double MaximumDBs = 90;

        public static double CalculateAttenuationRateStokesLaw()
        {
            var frequencyInHz = 1000.0;
            var angularFrequency = 2 * Math.PI * frequencyInHz;
            var dynamicViscosityCoefficient = Constants.AirViscosityCoefficient;
            var density = Constants.AirDensity;
            var speed = PropagationSpeed;

            return
                (2 * dynamicViscosityCoefficient * Math.Pow(angularFrequency, 2))
                /
                (3 * density * Math.Pow(speed, 3));
        }

        public static double CalculateAttenuationLoss(double distanceTraveled)
        {
            var attenuationRate = Globals.CalculateAttenuationRateStokesLaw();

            return Math.Exp(-attenuationRate * distanceTraveled);
        }

        public static double PressureLevelToDeciBells(double powerLevel)
        {
            return 10 * Math.Log10(powerLevel / Constants.ReferenceSoundPressureLevel);
        }

        public static double DeciBellsToPressureLevel(double dBs)
        {
            var exponent = dBs / 10;
            return Constants.ReferenceSoundPressureLevel * Math.Pow(10, exponent);
        }
    }
}
