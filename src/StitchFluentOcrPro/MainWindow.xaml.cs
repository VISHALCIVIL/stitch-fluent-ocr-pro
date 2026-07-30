using System.Windows;
using StitchFluentOcrPro.ViewModels;

namespace StitchFluentOcrPro
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
