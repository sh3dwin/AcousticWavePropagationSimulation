using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AcousticWavePropagationSimulation.DataStructures
{
    public class MediumParticleField
    {
        private float _dimY;
        private float _dimX;

        private float _centerX;
        private float _centerY;

        private double _width;
        private double _height;

        private Dictionary<(int, int), MediumParticle> mediumParticles;

        public MediumParticleField(double width, double height, float numParticlesAlongXAxis, float numParticlesAlongYAxis)
        {
            _width = width;
            _height = height;

            _dimX = numParticlesAlongXAxis;
            _dimY = numParticlesAlongYAxis;

            IsInitialized = false;
        }

        /// <summary>
        /// Initializes a particle field with an offset.
        /// </summary>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        public void CreateParticleField()
        {
            mediumParticles = new Dictionary<(int, int), MediumParticle>();

            var stepX = _width / _dimX;
            var stepY = _height / _dimY;

            for (var i = 0; i < _dimX; i++)
            {
                for (var j = 0; j < _dimY; j++)
                {

                    var particle = new MediumParticle(new Vector2((float)(i * stepX), (float)(j * stepY)));
                    mediumParticles.Add((i, j), particle);
                }
            }

            IsInitialized = true;
        }

        public bool IsInitialized { get; private set; }


        /// <summary>
        /// Returns width * height values each corresponding to the amplitude of the wave at that location.
        /// </summary>
        /// <param name="waveData"></param>
        /// <returns></returns>
        public IDictionary<Vector2, double> GetMediumState(IEnumerable<SoundSource> soundSources)
        {
            var particleAmplitudes = new Dictionary<Vector2, double>(mediumParticles.Values.Count);

            foreach(var particle in mediumParticles.Values)
            {
                var amplitude = particle.CalculateAmplitude(soundSources);

                if (amplitude > 1 || amplitude < -1)
                    throw new Exception("Incorrect amplitude");

                particleAmplitudes[particle.Position] = amplitude;
            }

            return particleAmplitudes;
        }

        /// <summary>
        /// Returns width * height values each corresponding to the amplitude of the wave at that location.
        /// </summary>
        /// <param name="waveData"></param>
        /// <returns></returns>
        public IDictionary<Vector2, double> GetMediumStateParallel(IEnumerable<SoundSource> soundSources, int parallelism)
        {
            var particleAmplitudes = new Dictionary<Vector2, double>(mediumParticles.Values.Count);

            var xStep = _width / parallelism;
            var yStep = _height / parallelism;

            var indexes = new List<(int, int)>();

            for (int i = 0; i < parallelism; i++)
            {
                for(int j = 0; j < parallelism; j++)
                {
                    indexes.Add((i, j));
                }
            }

            Parallel.ForEach(indexes, new ParallelOptions { MaxDegreeOfParallelism = parallelism * parallelism}, step =>
            {
                for( var column = step.Item1 * yStep; column < (step.Item1 + 1) * yStep;  column++)
                {
                    for (var row = step.Item1 * yStep; row < (step.Item2 + 1) * yStep; row++)
                    {
                        var particle = mediumParticles[((int)column, (int)row)];
                        var amplitude = particle.CalculateAmplitude(soundSources);

                        if (amplitude > 1 || amplitude < -1)
                            throw new Exception("Incorrect amplitude");

                        particleAmplitudes[particle.Position] = amplitude;
                    }
                }
            });

            return particleAmplitudes;
        }
    }
}
