﻿using System.Security.Policy;

namespace AcousticWavePropagationSimulation
{
    public static class Constants
    {
        /// <summary>
        /// What number of the latest samples are buffered
        /// </summary>
        public const int BufferSize = 9600 * 10;


        public const double ReferenceSoundPressureLevel = 1;

        public const int NumberOfSoundSources = 10;

        /// <summary>
        /// The number of threads that will run in parallel is n^2.
        /// The image is divided into the beforementioned number of squares and each
        /// is calculated separately.
        /// </summary>
        public const int ParallelizationParameter = 3;
    }
}
