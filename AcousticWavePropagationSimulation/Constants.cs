using System;
using System.Text.RegularExpressions;
using System.Windows;

namespace AcousticWavePropagationSimulation
{
    public static class Constants
    {
        /// <summary>
        /// What number of the latest samples are buffered
        /// </summary>
        public const int BufferSize = 9600;// * 10 * 0;

        /// <summary>
        /// Dynamic viscosity coefficient of air at 20 degree celsius.
        /// </summary>
        public const double AirViscosityCoefficient = 18.5e-6;

        /// <summary>
        /// Density of air at 20 degree celsius.
        /// </summary>
        public const double AirDensity = 1.2041;

        public const double ReferenceSoundPressureLevel = 1;

        public const int NumberOfSoundSources = 10;

        public const double MaximumDBs = 120;
        public const double MinimumDBs = 60; 

        /// <summary>
        /// The number of threads that will run in parallel is n^2.
        /// The image is divided into the beforementioned number of squares and each
        /// is calculated separately.
        /// </summary>
        public const int ParallelizationParameter = 3;
    }
}
