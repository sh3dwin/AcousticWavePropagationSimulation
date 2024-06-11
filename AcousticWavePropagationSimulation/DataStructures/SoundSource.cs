using AcousticWavePropagationSimulation.Utils;
using System.Linq;
using System.Numerics;

namespace AcousticWavePropagationSimulation.DataStructures
{
    public class SoundSource
    {
        public Vector2 Position {  get; private set; }
        public double PressureLevel => _pressureLevel;

        private double _pressureLevel;

        private float[] _buffer;

        public int NewestDataIndex {  get; private set; }

        public SoundSource(Vector2 position, double pressureLevel = 0)
        {
            Position = position;
            _buffer = new float[Constants.BufferSize];

            for (int i = 0; i < Constants.BufferSize; i++)
                _buffer[i] = 0;

            _pressureLevel = pressureLevel;
        }

        public SoundSource(SoundSource source)
        {
            Position = source.Position;
            _buffer = new float[Constants.BufferSize];

            source._buffer.CopyTo(_buffer, 0);
        }

        public void UpdateBuffer(CircularBuffer<float> buffer)
        {
            NewestDataIndex = buffer.Count;
            for (var i = 0; i < _buffer.Length; i++)
            {
                _buffer[i] = buffer.Buffer.ElementAt(i);
            }
        }

        public double GetSample(int delay)
        {
            if (delay > _buffer.Length)
                return 0;

            if (_buffer.Length == 0)
                return 0;

            var index = (int)(NewestDataIndex - 1.0 - delay).Clamp(0, _buffer.Length - 1);
            return _buffer[index];
        }
    }
}
