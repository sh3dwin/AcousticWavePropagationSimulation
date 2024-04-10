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
        private Dictionary<(int, int), MediumParticle> mediumParticles;
        public MediumParticleField(float width, float height) {
            _width = width;
            _height = height;

            CreateParticleField();
        }

        public void UpdateSize(float width, float height) { 
            _width = width;
            _height = height;

            CreateParticleField();
        }

        public void CreateParticleField()
        {
            mediumParticles = new Dictionary<(int, int), MediumParticle>();

            for (var i = 0; i < _width; i++)
            {
                for (var j = 0; j < _height; j++)
                {
                    mediumParticles.Add((i, j), new MediumParticle(i - (_width / 2), j - (_height / 2), Constants.PropagationSpeed));
                }
            }
        }

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
                    var amplitude = waveData[count - 1 - delay];
                    column.Add(amplitude);
                }
                result.Add(column);
            }

            return result;
        }
    }
}
