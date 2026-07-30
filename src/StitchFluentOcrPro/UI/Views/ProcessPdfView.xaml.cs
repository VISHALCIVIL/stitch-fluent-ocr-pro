using System.Windows;
using System.Windows.Controls;
using StitchFluentOcrPro.ViewModels;

namespace StitchFluentOcrPro.UI.Views
{
    public partial class ProcessPdfView : UserControl
    {
        public ProcessPdfView()
        {
            InitializeComponent();
        }

        private void BrowseInputFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFolderDialog
            {
                Title = "Select Main Input Folder containing PDFs"
            };

            if (dialog.ShowDialog() == true)
            {
                if (DataContext is ProcessPdfViewModel vm)
                {
                    vm.InputFolder = dialog.FolderName;
                }
            }
        }

        private void BrowseOutputFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFolderDialog
            {
                Title = "Select Destination Output Folder"
            };

            if (dialog.ShowDialog() == true)
            {
                if (DataContext is ProcessPdfViewModel vm)
                {
                    vm.OutputFolder = dialog.FolderName;
                }
            }
        }
    }
}
