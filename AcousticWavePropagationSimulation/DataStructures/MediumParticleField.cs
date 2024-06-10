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

        private Dictionary<(float, float), MediumParticle> _mediumParticles;

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
            _mediumParticles = new Dictionary<(float, float), MediumParticle>();

            var stepX = _width / _dimX;
            var stepY = _height / _dimY;

            for (var i = 0; i < _dimX; i++)
            {
                for (var j = 0; j < _dimY; j++)
                {

                    var particle = new MediumParticle(new Vector2((float)(i * stepX), (float)(j * stepY)));
                    _mediumParticles.Add((i, j), particle);
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
        public IDictionary<Vector2, double> GetMediumState(IEnumerable<SoundSource> soundSources, int sampleSkipCount)
        {
            var particleAmplitudes = new Dictionary<Vector2, double>(_mediumParticles.Values.Count);

            foreach (var particle in _mediumParticles.Values)
            {
                var amplitude = particle.CalculateAmplitude(soundSources, sampleSkipCount);

                particleAmplitudes[particle.Position] = amplitude;
            }

            return particleAmplitudes;
        }

        /// <summary>
        /// Returns width * height values each corresponding to the amplitude of the wave at that location.
        /// </summary>
        /// <param name="waveData"></param>
        /// <returns></returns>
        public IDictionary<Vector2, double> GetMediumStateParallel(IEnumerable<SoundSource> soundSources, int parallelism, int sampleSkipCount)
        {
            var numThreads = parallelism * parallelism;

            var subSections = new Dictionary<Vector2, double>[numThreads];

            var inverseParallelCount = 1.0 / parallelism;

            for (var i = 0; i < numThreads; i++)
            {
                subSections[i] = new Dictionary<Vector2, double>((int)(_dimX * inverseParallelCount * _dimY * inverseParallelCount + 1));
            }


            var parallelResult = Parallel.For(0, numThreads, new ParallelOptions { MaxDegreeOfParallelism = numThreads }, index =>
            {
                var startX = (int)((index / parallelism) * _dimX * inverseParallelCount);
                var startY = (int)((index % parallelism) * _dimY * inverseParallelCount);

                var endX = ((index / parallelism) + 1) * _dimX * inverseParallelCount;
                var endY = ((index % parallelism) + 1) * _dimY * inverseParallelCount;

                for (var i = startX; i < endX; i++)
                {
                    for (var j = startY; j < endY; j++)
                    {
                        var particle = _mediumParticles[(i, j)];
                        var amplitude = particle.CalculateAmplitude(soundSources, sampleSkipCount);

                        if (amplitude > soundSources.Count() || amplitude < -soundSources.Count())
                            throw new Exception("Incorrect amplitude");

                        subSections[index][particle.Position] = amplitude;
                    }
                }
            });


            var result = new Dictionary<Vector2, double>(_mediumParticles.Count);
            foreach (var dictionary in subSections)
            {
                foreach (var pair in dictionary)
                    result[pair.Key] = pair.Value;
            }

            return result;
        }

        /// <summary>
        /// Returns width * height values each corresponding to the amplitude of the wave at that location.
        /// </summary>
        /// <param name="waveData"></param>
        /// <returns></returns>
        public double[] GetMediumStateAsArrayParallel(IEnumerable<SoundSource> soundSources, int parallelism, int sampleSkipCount)
        {
            var numThreads = parallelism * parallelism;
            var inverseParallelCount = 1.0 / parallelism;

            var result = new double[(int)(_width * _height)];

            var parallelResult = Parallel.For(0, numThreads, new ParallelOptions { MaxDegreeOfParallelism = numThreads }, index =>
            {
                var startX = (int)((index / parallelism) * _width * inverseParallelCount);
                var startY = (int)((index % parallelism) * _height * inverseParallelCount);

                var endX = ((index / parallelism) + 1) * _width * inverseParallelCount;
                var endY = ((index % parallelism) + 1) * _height * inverseParallelCount;

                for (var j = startY; j < endY; j++)
                {
                    for (var i = startX; i < endX; i++)
                    {
                        var particle = _mediumParticles[(i, j)];
                        var amplitude = particle.CalculateAmplitude(soundSources, sampleSkipCount);

                        var ind = (int)(particle.Position.Y * _width + particle.Position.X);
                        result[ind] = amplitude;
                    }
                }
            });

            return result;
        }
    }
}
