using System;
using System.Windows.Controls;
using Microsoft.VisualStudio.Utilities;
using AcousticWavePropagationSimulation.Audio;

namespace AcousticWavePropagationSimulation.ViewModels
{
    public class VisualizationViewModel
    {


        private Canvas _canvas;
        private LoopbackAudioRecorder _recorder;
        private CircularBuffer<byte> _waveBuffer;
        public VisualizationViewModel(Canvas canvas) {
            _canvas = canvas;
            
            _waveBuffer = new CircularBuffer<byte>(Constants.BufferSize);

            _recorder = new LoopbackAudioRecorder();
            _recorder.DataAvailableEvent += UpdateBuffer;
        }

        private void UpdateBuffer(object sender, EventArgs e)
        {
            var newData = _recorder.GetAudioData();
            
            for(var i = 0; i < newData.ByteBuffer.Length; i++)
            {
                _waveBuffer.Add(newData.ByteBuffer[i]);
            }
        }

        private void UpdateMediumState()
        {

        }
    }
}
