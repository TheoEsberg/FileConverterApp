using FileConverterApp.Models;
using System.Collections;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Win32; // For OpenFileDialog
using WinForms = System.Windows.Forms; // For FolderBrowsingDialog

namespace FileConverterApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string SelectedOutputFolder { get; set; } = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void FileDropStackPanel_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                // Note that you can have more than one file
                string[] files = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
                FileNameLabel.Content = files[0];

                foreach (string file in files)
                {
                    this.UpploadedFilesList.Items.Add(new FileItem
                    {
                        FileName = Path.GetFileName(file),
                        ImagePreview = new BitmapImage(new Uri(file, UriKind.Absolute)),
                        FilePath = Path.GetFullPath(file)
                    });
                }

                if (UpploadedFilesList.Items.Count > 1)
                {
                    FileNameLabel.Content = $"{UpploadedFilesList.Items.Count} files uploaded.";
                }
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            var result = System.Windows.MessageBox.Show("Are you sure you want to clear all files?", "Confirm Clear", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                this.UpploadedFilesList.Items.Clear();
                FileNameLabel.Content = "Cleared all files";
            }
        }

        private async void ConvertButton_Click(object sender, RoutedEventArgs e)
        {
            if (UpploadedFilesList.Items.Count < 1)
            {
                System.Windows.MessageBox.Show("Please upload at least one file to convert.", "No Files Uploaded", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedFormat = (FormatComboBox.SelectedItem as ComboBoxItem)?.Content.ToString()?.ToLower();
            
            if (string.IsNullOrEmpty(selectedFormat))
            {
                System.Windows.MessageBox.Show("Please select a format to convert to.", "No Format Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(SelectedOutputFolder))
            {
                System.Windows.MessageBox.Show("Please select an output folder.", "No Output Folder Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            List<string> upploadedFilepaths = new List<string>();
            foreach (FileItem item in UpploadedFilesList.Items)
            {
                upploadedFilepaths.Add(item.FilePath.ToString());
            }

            try
            {
                await Converters.ImageConverter.ConvertImageAsync(upploadedFilepaths, 
                    SelectedOutputFolder, selectedFormat);
                System.Windows.MessageBox.Show($"Files converted successfully to {SelectedOutputFolder}", "Conversion Successful", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception)
            {
                throw;
            }


        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            var items = UpploadedFilesList.SelectedItems;

            var result = System.Windows.MessageBox.Show($"Are you sure you want to remove {items.Count} items?", "Confirm Remove", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                var itemsToRemove = new ArrayList(items);
                foreach (var item in itemsToRemove)
                {
                    UpploadedFilesList.Items.Remove(item);
                }
            }

            if (UpploadedFilesList.Items.Count > 1)
            {
                FileNameLabel.Content = $"{UpploadedFilesList.Items.Count} files uploaded.";
            }
        }

        private void ChooseFileButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog { 
                Multiselect = true,
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp;*.webp;*.heic;|Video Files|*.mp4;",
            };

            bool? success = fileDialog.ShowDialog();
            if (success == true)
            {
                foreach (string file in fileDialog.FileNames)
                {
                    this.UpploadedFilesList.Items.Add(new FileItem
                    {
                        FileName = Path.GetFileName(file),
                        ImagePreview = new BitmapImage(new Uri(file, UriKind.Absolute)),
                        FilePath = Path.GetFullPath(file)
                    });
                }
                if (UpploadedFilesList.Items.Count > 1)
                {
                    FileNameLabel.Content = $"{UpploadedFilesList.Items.Count} files uploaded.";
                }
            } else {
                // Didnt select any files, do nothing
            }
        }

        private void ChooseOutputFolder_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();

            if (result == WinForms.DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
            {
                SelectedOutputFolder = dialog.SelectedPath;
            }
            else
            {
                System.Windows.MessageBox.Show("No folder selected.", "No Folder Selected", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}