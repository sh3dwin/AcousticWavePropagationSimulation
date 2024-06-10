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
using System.Threading;
using System.Security.Policy;
using System.Windows.Media.Media3D;
using System.Windows;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Collections;

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

        private DataStructures.CircularBuffer<float> _waveBuffer;

        DirectBitmap _waveImage;
        MediumParticleField _particleField;

        private byte[] waveData;

        private List<SoundSource> _soundSources;

        private ConcurrentQueue<bool> _eventQueue = new ConcurrentQueue<bool>();

        public VisualizationViewModel(double widthMeters, double heightMeters, int width, int height)
        {
            _widthMeters = widthMeters;
            _heightMeters = heightMeters;

            _width = width;
            _height = height;
            _recorder = new LoopbackAudioRecorder();

            _waveBuffer = new DataStructures.CircularBuffer<float>(Constants.BufferSize);

            _soundSources = new List<SoundSource>();

            var rand = new Random();
            for (int i = 0; i < Constants.NumberOfSoundSources; i++) {
                var x = rand.Next(0, _width);
                var y = rand.Next(0, _height);

                var pressure = rand.Next(100000, 1000000);

                _soundSources.Add(new SoundSource(new Vector2(x, y), pressure));
            }

            _waveImage = new DirectBitmap(width, height);

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

            InitializeVisualizationLoop();
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
            _particleField.GetMediumState(_soundSources, sampleSkipCount);
        }

        private async void InitializeVisualizationLoop()
        {
            var cts = new CancellationTokenSource();
            var eventTrigger = new TaskCompletionSource<bool>(false);

            _recorder.DataAvailableEvent += (sender, e) =>
            {
                _eventQueue.Enqueue(true);
            };

            var visualizationTask = Task.Run(() => UpdateVisualization(cts.Token));

            //UpdateBuffer();

            await visualizationTask;
        }

        private async void UpdateVisualization(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var startTime = DateTime.Now;

                if (_eventQueue.TryDequeue(out bool _))
                {
                    lock (_waveBuffer)
                    {
                        UpdateBuffer();
                        sampleSkipCount = 0;
                    }
                }

                UpdateMediumState();
                UpdateImage();

                //sampleSkipCount += 100;

                var endTime = DateTime.Now - startTime;
                if(1 / (endTime.Milliseconds / 1000.0) < 30)
                    Debug.WriteLine($"FPS: {1.0 / (endTime.Milliseconds / 1000.0)}");
            }
        }

        private int sampleSkipCount = 0;

        private void UpdateBuffer()
        {
            if (!_recorder.ISBufferInitialized)
                return;

            sampleSkipCount = 0;
            var newData = _recorder.GetAudioData().ByteBuffer;

            float[] floatData = new float[newData.Length / sizeof(float)];

            Buffer.BlockCopy(newData, 0, floatData, 0, newData.Length);

            floatData = floatData.TakeWhile(x => x != 0).ToArray();

            for (var i = 0; i < floatData.Length * 4; i += 4)
                floatData[i / 4] = BitConverter.ToSingle(newData, i);

            _waveBuffer.Insert(floatData);//.Where((x, i) => i % 2 == 0).ToArray());

            //_waveBuffer.Insert(floatData);

            foreach (var soundSource in _soundSources)
                soundSource.UpdateBuffer(_waveBuffer);
        }

        private void UpdateImage()
        {
            var bitmap = new Bitmap(_waveImage.Bitmap);

            Image = BitmapToImageSource(bitmap);
            Image.Freeze();
        }


        private void UpdateMediumState()
        {
            var hueShift = DateTime.Now.Millisecond / 1000.0 * 360;

            //var amplitudes = _particleField.GetMediumStateAsArrayParallel(new List<SoundSource>(_soundSources), Constants.ParallelizationParameter, sampleSkipCount);
            //var amplitudes = _particleField.GetMediumState(new List<SoundSource>(_soundSources), sampleSkipCount);

            //MonochromeVisualizer.Draw(amplitudes, ref _waveImage, (int)hueShift);
            DeciBellVisualizer.Draw(_soundSources, _waveImage);

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
            return true;
        }

        private void ToggleVisualization()
        {
        }

        public void Dispose()
        {

            _recorder.Dispose();

            _waveImage.Dispose();
        }
    }
}
