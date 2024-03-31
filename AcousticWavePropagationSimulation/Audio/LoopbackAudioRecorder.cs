using NAudio.Wave;
using System;

namespace AcousticWavePropagationSimulation.Audio
{
    public class LoopbackAudioRecorder
    {
        private WaveBuffer _buffer;

        public LoopbackAudioRecorder()
        {
            Capture = new WasapiLoopbackCapture();

            Globals.SampleRate = Capture.WaveFormat.SampleRate;

            Capture.DataAvailable += DataAvailable;

            Capture.StartRecording();
        }

        public WasapiLoopbackCapture Capture { get; }

        public void DataAvailable(object sender, WaveInEventArgs e)
        {
            _buffer = new WaveBuffer(e.Buffer);
            OnDataAvailable(e);
        }

        public event EventHandler DataAvailableEvent;

        public virtual void OnDataAvailable(EventArgs e)
        {
            DataAvailableEvent?.Invoke(this, e);
        }

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
            Capture.Dispose();
        }
    }
}
