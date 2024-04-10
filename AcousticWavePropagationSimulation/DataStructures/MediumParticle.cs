using System;

namespace AcousticWavePropagationSimulation.DataStructures
{
    public class MediumParticle
    {
        private DelayLine _delayLine;
        private double _loss;

        public MediumParticle(double xDistance, double yDistance, double speed)
        {
            var distance = Math.Sqrt(xDistance * xDistance + yDistance * yDistance);
            var samplePeriod = 1.0 / Globals.SampleRate;
            _delayLine = new DelayLine(distance, speed, samplePeriod);
            _loss = 1.0 / (_delayLine.Delay * 0.001 + 1);
        }

        public int GetDelay()
        {
            return _delayLine.Delay;
        }

        public double GetLoss()
        {
            if (_loss < 0 || _loss > 1)
            {
                throw new Exception("Incorrect loss");
            }
            return _loss;
        }
    }
}
