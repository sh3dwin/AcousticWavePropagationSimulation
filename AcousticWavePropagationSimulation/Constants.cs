namespace AcousticWavePropagationSimulation
{
    public static class Constants
    {
        /// <summary>
        /// What number of the latest samples are buffered
        /// </summary>
        public const int BufferSize = 16777216;
        /// <summary>
        /// Propagation speed of an acoustic wave in water, in m/s
        /// </summary>
        public const int WaterPropagationSpeed = 1500;
        /// <summary>
        /// Propagation speed of an acoustic wave in air, in m/s
        /// </summary>
        public const int AirPropagationSpeed = 340;
    }
}
