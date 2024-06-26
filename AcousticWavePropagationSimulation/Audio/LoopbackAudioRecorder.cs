﻿using NAudio.Wave;
using System;

namespace AcousticWavePropagationSimulation.Audio
{
    public class LoopbackAudioRecorder
    {
        private WaveBuffer _buffer;
        private WasapiLoopbackCapture _capture;

        public LoopbackAudioRecorder()
        {
            _capture = new WasapiLoopbackCapture();

            Globals.SampleRate = _capture.WaveFormat.SampleRate;

            _capture.DataAvailable += DataAvailable;

            _capture.StartRecording();
        }

        

        public void DataAvailable(object sender, WaveInEventArgs e)
        {
            if(_buffer == null)
                _buffer = new WaveBuffer(e.Buffer);

            OnDataAvailable(e);
        }

        public event EventHandler DataAvailableEvent;

        public virtual void OnDataAvailable(EventArgs e)
        {
            DataAvailableEvent?.Invoke(this, e);
        }

        public bool ISBufferInitialized => _buffer != null;

        public WaveBuffer GetAudioData()
        {
            if (_buffer == null)
            {
                throw new Exception("No buffer available");
            }

            return _buffer;
        }

        public void Dispose()
        {
            _capture.Dispose();
        }
    }
}
