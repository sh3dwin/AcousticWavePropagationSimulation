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

namespace AcousticWavePropagationSimulation.ViewModels
{
    public class VisualizationViewModel : INotifyPropertyChanged, IDisposable
    {
        private int _width;
        private int _height;


        private BitmapImage _image;
        private LoopbackAudioRecorder _recorder;
        private CircularBuffer<double> _waveBuffer;
        private Dictionary<int, DirectBitmap> _waveImages;
        private Dictionary<int, MediumParticleField> _particleFields;

        private int _numberOfPartitions;
        private float _partitionWidth;
        private float _partitionHeight;
        public VisualizationViewModel(int width, int height)
        {

            _width = width;
            _height = height;
            _recorder = new LoopbackAudioRecorder();


            _waveBuffer = new CircularBuffer<double>(Constants.BufferSize);

            _numberOfPartitions = (int)Math.Pow(Constants.ParallelizationParameter, 2);
            _partitionWidth = width / Constants.ParallelizationParameter;
            _partitionHeight = height / Constants.ParallelizationParameter;

            _waveImages = new Dictionary<int, DirectBitmap>(_numberOfPartitions);

            for (var i = 0; i < _numberOfPartitions; i++)
            {
                var waveImage = new DirectBitmap((int)_partitionWidth, (int)_partitionHeight);

                _waveImages.Add(i, waveImage);
            }

            _image = new BitmapImage();

            _particleFields = new Dictionary<int, MediumParticleField>(_numberOfPartitions);
            for (var i = 0; i < _numberOfPartitions; i++)
            {
                var startX = width / Constants.ParallelizationParameter * (i % Constants.ParallelizationParameter);
                var startY = height / Constants.ParallelizationParameter * (i / Constants.ParallelizationParameter);

                var particleField = new MediumParticleField(_partitionWidth, _partitionHeight, width / 2, height / 2);
                particleField.CreateParticleField(startX, startY, PropagationSpeed);

                if (particleField.IsInitialized)
                {
                    _particleFields.Add(i, particleField);
                }
            }

            if (_particleFields.Values.Any(x => !x.IsInitialized))
            {
                throw new Exception("Error initializing particle fields");
            }

            _recorder.DataAvailableEvent += Update;
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

        private void RecalculateDelay()
        {
            foreach(var particleField in _particleFields.Values) { 
                particleField.CalculateDelay(PropagationSpeed);
            }
        }

        private void Update(object sender, EventArgs e)
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

            foreach (float f in floatData)
            {
                Console.WriteLine(f);
            }

            for (var i = 0; i < floatData.Length; i++)
            {
                _waveBuffer.Add(floatData[i]);
            }
        }

        private void UpdateImage(object sender, EventArgs e)
        {
            var bitmap = AppendBitmapPartitions();
            
            Image = BitmapToImageSource(bitmap);
            Image.Freeze();
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

        private void UpdateMediumState(object sender, EventArgs e)
        {
            var hueShift = Math.Abs((int)(Math.Sin(DateTime.Now.Millisecond / 10000.0) * 360));

            Parallel.For(0, _numberOfPartitions, partitionIndex =>
            {
                var particleField = _particleFields[partitionIndex];
                var particleAmplitudes = particleField.GetMediumState(ref _waveBuffer);
                var imagePartition = _waveImages[partitionIndex];

                MonochromeVisualizer.Draw(particleAmplitudes, ref imagePartition, 0);
            });
        }

        private Bitmap AppendBitmapPartitions()
        {
            var result = new Bitmap(_width, _height);

            using (Graphics g = Graphics.FromImage(result))
            {
                for (var i = 0; i < _numberOfPartitions; i++)
                {
                    var offsetY = _width / Constants.ParallelizationParameter * (i % Constants.ParallelizationParameter);
                    var offsetX = _height / Constants.ParallelizationParameter * (i / Constants.ParallelizationParameter);
                    g.DrawImage(_waveImages[i].Bitmap, offsetX, offsetY);
                }
            }
            return result;
        }

        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            _recorder.Dispose();

            foreach(var waveImage in _waveImages.Values)
                waveImage.Dispose();

        }
    }
}
