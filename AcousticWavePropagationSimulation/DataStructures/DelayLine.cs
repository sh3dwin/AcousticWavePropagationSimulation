namespace AcousticWavePropagationSimulation.DataStructures
{
    public class DelayLine
    {
        public int Delay {  get; private set; }
        /// <summary>
        /// Creates a delay line given by the distance, propagation speed in the medium, and the sample period
        /// </summary>
        /// <param name="distance"></param>
        public DelayLine(double distance, double speed, double samplePeriod) {
            Delay = (int)(distance / (speed * samplePeriod));
        }
    }
}
