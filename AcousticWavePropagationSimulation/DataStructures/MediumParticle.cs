namespace AcousticWavePropagationSimulation.DataStructures
{
    public class MediumParticle
    {
        private DelayLine _delayLine;

        public double Amplitude;

        public MediumParticle(double distance, double speed, double samplePeriod)
        {
            _delayLine = new DelayLine(distance, speed, samplePeriod);
            Amplitude = 0;
        }

        public int GetDelay()
        {
            return _delayLine.Delay;
        }
    }
}
