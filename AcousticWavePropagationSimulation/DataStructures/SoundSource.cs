using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AcousticWavePropagationSimulation.DataStructures
{
    public class SoundSource
    {
        public Vector2 Position {  get; private set; }
        private CircularBuffer<double> _buffer;

        public SoundSource(Vector2 position) {
            Position = position;
        }

        public void UpdateBuffer(ref CircularBuffer<double> buffer)
        {
            _buffer = buffer;
        }

        public double GetSample(int delay)
        {
            var index = Math.Max(0, _buffer.Count - 1 - delay);
            return _buffer[index];
        }
    }
}
