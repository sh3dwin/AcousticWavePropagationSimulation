﻿using System;
using System.Collections.Generic;
using System.Numerics;

namespace AcousticWavePropagationSimulation.DataStructures
{
    public class MediumParticle
    {
        public Vector2 Position { get; private set; }

        public MediumParticle(Vector2 position)
        {
            Position = position;
        }

        public double CalculateAmplitude(IEnumerable<SoundSource> soundSources, int sampleSkipCount)
        {
            var amplitude = 0.0;

            foreach(var soundSource in soundSources)
            {
                var sourcePosition = soundSource.Position;
                
                var sampleDelay = CalculateSampleDelay(sourcePosition) - sampleSkipCount;
                var propagationLoss = CalculatePropagationLoss(sourcePosition);
                var attenuationLoss = CalculateAttenuation(sourcePosition);

                var sourceAmplitude = soundSource.GetSample(sampleDelay);

                var pressureLevel = Globals.PressureLevelToDeciBells(soundSource.PressureLevel);

                amplitude += sourceAmplitude * propagationLoss * attenuationLoss * pressureLevel;
            }

            return amplitude;
        }

        private double CalculateAttenuation(Vector2 sourcePosition)
        {
            var attenuationRate = Globals.CalculateAttenuationRateStokesLaw();
            var distanceTraveled = Vector2.Distance(sourcePosition, Position);

            return Math.Exp(-attenuationRate * distanceTraveled);
        }

        private int CalculateSampleDelay(Vector2 sourcePosition)
        {
            var samplePeriod = 1.0 / Globals.SampleRate;
            var propagationSpeed = Globals.PropagationSpeed;
            var distanceFromSource = Vector2.Distance(Position, sourcePosition);

            var delayInSamples = distanceFromSource / (propagationSpeed * samplePeriod);

            return (int)delayInSamples;
        }
        private double CalculatePropagationLoss(Vector2 sourcePosition)
        {
            var distanceFromSource = Math.Max(Vector2.Distance(Position, sourcePosition), 1);
            var propagationLoss = 1.0 / distanceFromSource;

            var mediumPropagationLoss = 1.0;

            return propagationLoss * mediumPropagationLoss;
        }
    }
}
