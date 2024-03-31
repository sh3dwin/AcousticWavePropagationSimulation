namespace AcousticWavePropagationSimulation.PropagationMediums
{
    internal class WaterMedium : PropagationMedium
    {
        private double _propagationSpeed = 1500;
        public override double GetPropagationSpeed()
        {
            return _propagationSpeed;
        }
    }
}
