using System;
using Microsoft.VisualStudio.Utilities;
using AcousticWavePropagationSimulation.Audio;
using AcousticWavePropagationSimulation.DataStructures;
using AcousticWavePropagationSimulation.Visualizer;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows.Input;
using AcousticWavePropagationSimulation.WPF.Commands;
using System.Numerics;

namespace AcousticWavePropagationSimulation.ViewModels
{
    public class VisualizationViewModel : INotifyPropertyChanged, IDisposable
    {
        private int _width;
        private int _height;

        private double _widthMeters;
        private double _heightMeters;

        private BitmapImage _image;
        private LoopbackAudioRecorder _recorder;
        private CircularBuffer<double> _waveBuffer;
        DirectBitmap _waveImage;
        MediumParticleField _particleField;

        private IEnumerable<SoundSource> _soundSources;

        private DispatcherTimer _dispatcherTimer;
        public VisualizationViewModel(double widthMeters, double heightMeters, int width, int height)
        {
            _widthMeters = widthMeters;
            _heightMeters = heightMeters;

            _width = width;
            _height = height;
            _recorder = new LoopbackAudioRecorder();

            _waveBuffer = new CircularBuffer<double>(Constants.BufferSize);

            _soundSources = new List<SoundSource>()
            {
                new SoundSource(new Vector2(width / 2, height / 2)),
                new SoundSource(new Vector2(width / 4, height / 4))
            };

            _waveImage = new DirectBitmap(width, height);

            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Interval = TimeSpan.FromMilliseconds(100);

            _image = new BitmapImage();

            var particleField = new MediumParticleField(_widthMeters, _heightMeters, width, height);
            particleField.CreateParticleField();

            if (particleField.IsInitialized)
            {
                _particleField = particleField;
            }

            if (!_particleField.IsInitialized)
            {
                throw new Exception("Error initializing particle fields");
            }

            //_dispatcherTimer.Tick += UpdateVisualization;
            _recorder.DataAvailableEvent += UpdateVisualization;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public BitmapImage Image
        {
            get => _image;
            set
            {
                _image = value;
                RaisePropertyChanged(nameof(Image));
            }
        }

        public int PropagationSpeed
        {
            get => Globals.PropagationSpeed;
            set
            {
                Globals.PropagationSpeed = value;
                RecalculateDelay();
                RaisePropertyChanged(nameof(PropagationSpeed));
            }
        }

        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



        private void RecalculateDelay()
        {
            _particleField.GetMediumState(_soundSources);
        }

        private void UpdateVisualization(object sender, EventArgs e)
        {
            UpdateBuffer(sender, e);
            UpdateMediumState(sender, e);
            UpdateImage(sender, e);
        }

        private void UpdateBuffer(object sender, EventArgs e)
        {
            var newData = _recorder.GetAudioData().ByteBuffer;

            float[] floatData = new float[newData.Length / sizeof(float)];

            Buffer.BlockCopy(newData, 0, floatData, 0, newData.Length);

            for (var i = 0; i < floatData.Length; i++)
            {
                _waveBuffer.Add(floatData[i]);
            }

            foreach (var soundSource in _soundSources)
                soundSource.UpdateBuffer(ref _waveBuffer);
        }

        private void UpdateImage(object sender, EventArgs e)
        {
            var bitmap = AppendBitmapPartitions();

            Image = BitmapToImageSource(bitmap);
            Image.Freeze();
        }

        private void UpdateMediumState(object sender, EventArgs e)
        {
            var hueShift = Math.Abs((int)(Math.Sin(DateTime.Now.Millisecond / 10000.0) * 360));

            var particleAmplitudes = _particleField.GetMediumState(_soundSources);

            MonochromeVisualizer.Draw(particleAmplitudes, ref _waveImage, 0);

        }

        private Bitmap AppendBitmapPartitions()
        {
            var result = new Bitmap(_width, _height);

            using (Graphics g = Graphics.FromImage(result))
            {
                    g.DrawImage(_waveImage.Bitmap, 0, 0);
            }
            return result;
        }

        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }

        private ICommand _toggleVisualization;

        public ICommand ToggleVisualizationCommand
        {
            get
            {
                if (_toggleVisualization == null)
                {
                    _toggleVisualization = new RelayCommand(
                        _ => ToggleVisualization(),
                        _ => CanToggle()
                    );
                }
                return _toggleVisualization;
            }
        }

        private bool CanToggle()
        {
            return _dispatcherTimer != null;
        }

        private void ToggleVisualization()
        {
            if (_dispatcherTimer.IsEnabled)
                _dispatcherTimer.Stop();
            else
                _dispatcherTimer.Start();
        }

        public void Dispose()
        {
            _recorder.Dispose();

            _waveImage.Dispose();

        }
    }
}
