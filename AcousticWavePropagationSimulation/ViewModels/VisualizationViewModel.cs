using System;
using System.Windows.Controls;
using Microsoft.VisualStudio.Utilities;
using AcousticWavePropagationSimulation.Audio;
using AcousticWavePropagationSimulation.DataStructures;
using AcousticWavePropagationSimulation.Visualizer;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace AcousticWavePropagationSimulation.ViewModels
{
    public class VisualizationViewModel: INotifyPropertyChanged
    {
        private int _width;
        private int _height;

        private WriteableBitmap _waveImage;
        private Image _image;
        private LoopbackAudioRecorder _recorder;
        private CircularBuffer<double> _waveBuffer;
        private MediumParticleField _particleField;
        public VisualizationViewModel(int width, int height) {        
            
            _width = width;
            _height = height;


            _waveImage = new WriteableBitmap
            (
                (int)width,
                (int)height,
                96,
                96,
                PixelFormats.Bgr32,
                null
            );

            _image = new Image { Source = _waveImage };

            _recorder = new LoopbackAudioRecorder();
            

            _waveBuffer = new CircularBuffer<double>(Constants.BufferSize);
            _particleField = new MediumParticleField((float)width, (float)height);

            _recorder.DataAvailableEvent += UpdateBuffer;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public WriteableBitmap WaveImage
        {
            get => _waveImage;
            set {
                _waveImage = value;
                RaisePropertyChanged(nameof(WaveImage));
            }
        }

        public Image Image
        {
            get => _image;
            set
            {
                _image = value;
                RaisePropertyChanged(nameof(Image));
            }
        }

        private void UpdateBuffer(object sender, EventArgs e)
        {
            var newData = _recorder.GetAudioData().ByteBuffer;
            
            float[] floatData = new float[newData.Length / sizeof(float)];

            Buffer.BlockCopy(newData, 0, floatData, 0, newData.Length);

            foreach (float f in floatData)
            {
                Console.WriteLine(f);
            }

            for (var i = 0; i < floatData.Length; i++)
            {
                _waveBuffer.Add(floatData[i]);
            }

            UpdateMediumState(sender, e);
        }

        private void UpdateMediumState(object sender, EventArgs e)
        {
            var particleAmplitudes = _particleField.GetMediumState(ref _waveBuffer);

            WaveImage.Dispatcher.Invoke(() =>
            {

                var frame = MonochromeVisualizer.Draw(particleAmplitudes, WaveImage);

                WaveImage = frame;

                Image = new Image { Source = WaveImage };

            });

        }

        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
