using System;
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
using System.Windows.Input;
using AcousticWavePropagationSimulation.WPF.Commands;
using System.Numerics;
using System.Threading;
using System.Collections.Concurrent;
using System.Diagnostics;
using PoissonDiskSampling;
using NewtonRaphsonSolver;

namespace AcousticWavePropagationSimulation.ViewModels
{
    public class VisualizationViewModel : INotifyPropertyChanged, IDisposable
    {
        private int _width;
        private int _height;

        private double _widthMeters;
        private double _heightMeters;

        private BitmapImage _image = new BitmapImage();
        private LoopbackAudioRecorder _recorder = new LoopbackAudioRecorder();

        private CircularBuffer<float> _waveBuffer = new CircularBuffer<float>(Constants.BufferSize);

        DirectBitmap _waveImage;
        MediumParticleField _particleField;

        private List<SoundSource> _soundSources = new List<SoundSource>();

        private ConcurrentQueue<bool> _eventQueue = new ConcurrentQueue<bool>();

        private bool _visualizeAcousticWaves = true;

        public VisualizationViewModel(double widthMeters, double heightMeters, int width, int height)
        {
            _widthMeters = widthMeters;
            _heightMeters = heightMeters;

            _width = width;
            _height = height;
            _waveImage = new DirectBitmap(width, height);

            InstantiateParticleField();
            InitializeVisualizationLoop();
            InstantiateSoundsSources();
        }

        private void InstantiateParticleField()
        {
            var particleField = new MediumParticleField(_widthMeters, _heightMeters, _width, _height);
            particleField.CreateParticleField();

            if (particleField.IsInitialized)
                _particleField = particleField;

            if (!_particleField.IsInitialized)
                throw new Exception("Error initializing particle fields");
        }

        private void InstantiateSoundsSources()
        {
            if (false)
                InstantiateSoundSourcesRandom();
            else
                InstantiateSoundSourcesPoissonDisks();
        }

        private void InstantiateSoundSourcesRandom()
        {
            var rand = new Random();
            for (int i = 0; i < Constants.NumberOfSoundSources; i++)
            {
                var x = rand.Next(0, _width);
                var y = rand.Next(0, _height);

                var pressure = rand.Next(100000, 1000000);

                _soundSources.Add(new SoundSource(new Vector2(x, y), pressure));
            }
        }

        private void InstantiateSoundSourcesPoissonDisks()
        {
            var maximumPressure = Globals.DeciBellsToPressureLevel(Globals.MaximumDBs);
            var minimumPressure = Globals.DeciBellsToPressureLevel(Constants.MinimumDBs);

            var initial = maximumPressure / minimumPressure;
            var radius = NewtonRaphsonSolver.NewtonRaphsonSolver.NewtonRaphson(initial, maximumPressure, minimumPressure, Globals.CalculateAttenuationRateStokesLaw());
            var points = _particleField.ParticlePositions.ToList();
            points.Insert(0, points.ElementAt((_width * _height / 2) + (_width / 2)));

            var soundSourceLocations = PoissonDiskSampling.PoissonDiskSampling.Apply(points, radius);

            _soundSources.Clear();
            _soundSources.AddRange(soundSourceLocations.Select(loc => new SoundSource(new Vector2((float)loc.Position.X, (float)loc.Position.Y), maximumPressure)));
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

        public int Loudness
        {
            get => (int)Globals.MaximumDBs;
            set
            {
                Globals.MaximumDBs = value;
                this._updated = true;
                RaisePropertyChanged(nameof(Loudness));
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

            _visualizationTask = Task.Run(() => UpdateVisualization(cts.Token));

            //UpdateBuffer();

            await _visualizationTask;
        }

        private async void UpdateVisualization(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var startTime = DateTime.Now;

                if (_eventQueue.TryDequeue(out bool _) || _updated)
                {
                    if (_updated)
                    {
                        _updated = false;
                        InstantiateSoundSourcesPoissonDisks();
                    }
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
        private bool _updated;
        private Task _visualizationTask;

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
            _visualizeAcousticWaves = !_visualizeAcousticWaves;
        }

        public void Dispose()
        {
            _visualizationTask.Dispose();

            _recorder.Dispose();

            _waveImage.Dispose();
        }
    }
}
