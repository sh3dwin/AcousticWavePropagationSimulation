namespace AcousticWavePropagationSimulation.PropagationMediums
{
    public class AirMedium : PropagationMedium
    {
        private double _propagationSpeed = 340;
        public override double GetPropagationSpeed()
        {
            return _propagationSpeed;
        }
    }
}
