namespace AcousticWavePropagationSimulation
{
    public static class Constants
    {
        /// <summary>
        /// What number of the latest samples are buffered
        /// </summary>
        public const int BufferSize = 16777216 / 16;
        /// <summary>
        /// Propagation speed of an acoustic wave in air, in m/s
        /// </summary>
        public const int PropagationSpeed = 3400;
    }
}
