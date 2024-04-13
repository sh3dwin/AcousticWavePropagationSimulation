using System;

namespace AcousticWavePropagationSimulation.DataStructures
{
    public class MediumParticle
    {
        private DelayLine _delayLine;

        private double _distance;
        private double _loss;

        public MediumParticle(double xDistance, double yDistance, double propagationSpeed)
        {

            _distance = Math.Sqrt(xDistance * xDistance + yDistance * yDistance);
            var samplePeriod = 1.0 / Globals.SampleRate;
            _delayLine = new DelayLine(_distance, propagationSpeed, samplePeriod);
            _loss = 1.0 / (_delayLine.Delay * 0.001 + 1);
        }

        public void RecalculateDelay(double propagationSpeed)
        {
            var samplePeriod = 1.0 / Globals.SampleRate;
            _delayLine = new DelayLine(_distance, propagationSpeed, samplePeriod);
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
