using AcousticWavePropagationSimulation.ViewModels;
using System.Windows;
using System.Windows.Controls;

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

            _viewModel = new VisualizationViewModel(new Canvas());
        }
    }
}
