using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcousticWavePropagationSimulation.DataStructures
{
    public class MediumParticleField
    {
        private float _height;
        private float _width;
        private MediumParticle[,] _particleField;
        public MediumParticleField(float width, float height) {
            _width = width;
            _height = height;
        }

        public void UpdateSize(float width, float height) { 
            _width = width;
            _height = height;

        }

        public void SetParticleField()
        {
            _particleField = new MediumParticle[];
        }
    }
}
