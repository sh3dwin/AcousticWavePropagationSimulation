using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AcousticWavePropagationSimulation.DataStructures
{
    public class MediumParticleField
    {
        private float _height;
        private float _width;

        private float _centerX;
        private float _centerY;
        private Dictionary<(int, int), MediumParticle> mediumParticles;

        public MediumParticleField(float width, float height, float centerX, float centerY) {
            _width = width;
            _height = height;
            _centerX = centerX;
            _centerY = centerY;

            IsInitialized = false;
        }

        public void CalculateDelay(float propagationSpeed)
        {
            foreach (var particle in mediumParticles.Values)
                particle.RecalculateDelay(propagationSpeed);
        }

        /// <summary>
        /// Initializes a particle field with an offset.
        /// </summary>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        public void CreateParticleField(int offsetX, int offsetY, float propagationSpeed)
        {
            mediumParticles = new Dictionary<(int, int), MediumParticle>();
            IsInitialized = true;

            for (var i = 0; i < _width; i++)
            {
                for (var j = 0; j < _height; j++)
                {
                    mediumParticles.Add((i, j), new MediumParticle((i + offsetX) - _centerX, (j + offsetY) - _centerY, propagationSpeed));
                }
            }
        }

        public bool IsInitialized { get; private set; }


        /// <summary>
        /// Returns width * height values each corresponding to the amplitude of the wave at that location.
        /// </summary>
        /// <param name="waveData"></param>
        /// <returns></returns>
        public List<List<double>> GetMediumState(ref CircularBuffer<double> waveData)
        {
            var result = new List<List<double>>();

            var count = waveData.Count;

            for (var i = 0; i < _width; i++)
            {
                var column = new List<double>();
                for (var j = 0; j < _height; j++)
                {
                    var particle = mediumParticles[(i, j)];
                    var delay = particle.GetDelay();
                    var loss = particle.GetLoss();
                    if (delay >= count)
                    {
                        column.Add(0);
                        continue;
                    }
                    var index = Math.Max(0, count - 1 - delay);
                    var amplitude = waveData[index] * loss;

                    if (amplitude > 1 || amplitude < -1)
                        throw new Exception("Incorrect amplitude");

                    column.Add(amplitude);
                }
                result.Add(column);
            }

            return result;
        }
    }
}
