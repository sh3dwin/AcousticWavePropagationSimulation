using AcousticWavePropagationSimulation.ViewModels;
using System;
using System.Windows;

namespace AcousticWavePropagationSimulation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private VisualizationViewModel _viewModel;
        public MainWindow()
        {
            InitializeComponent();

            InitializeViewModel();
        }

        private void InitializeViewModel()
        {
            _viewModel = new VisualizationViewModel((int)Width, (int)Height);

            DataContext = _viewModel;
        }
    }
}
