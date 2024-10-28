using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Win32;
using UtilityLibrary;

namespace Client;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window, IUpdateFileList, IUpdateProgressBar
{
    private string fileRoute = "'";
    private string fileName = "";
    private ObservableCollection<File> fileList;
    private Connection connection;
    private int selectedFileSizeInBytes;
    
    public MainWindow(Connection connection)
    {
        InitializeComponent();
        connection.UpdateFileList = this;
        connection.UpdateProgressBar = this;
        this.connection = connection;
        fileList = new ObservableCollection<File>();
        FileList.ItemsSource = fileList;
        Refresh();
    }

    #region ButtonEvends
    private void RefreshButtonClicked(object sender, RoutedEventArgs e)
    {
        Refresh();
    }

    private void DownloadButtonClicked(object sender, RoutedEventArgs e)
    {
        if (FileList.SelectedItem is File selectedItem)
        {
            ResetProgressBar();
            selectedFileSizeInBytes = Utils.FormatDataSizeToBytes(selectedItem.fileSize);
            App.Download(selectedItem.fileName);
        }
        else
        {
            MessageBox.Show("selected file dit not go trough");
        }
    }

    private void ChosenButtonClicked(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        openFileDialog.Filter = "Text files (*.txt)|*.txt";
        openFileDialog.Title = "Select a file to upload";
        if (openFileDialog.ShowDialog() == true)
        {
            fileRoute = openFileDialog.FileName;
            ChosenFileName.Content = System.IO.Path.GetFileName(fileRoute);
            Console.WriteLine(fileRoute);
        }
    }

    private void UploadButtonClicked(object sender, RoutedEventArgs e)
    {
        fileName = FileName.Text;
        if (fileRoute.Length > 1 && fileName.Length > 1)
        {
            Console.WriteLine("File with Route: " + fileRoute + " is send with name: " + fileName);
            App.SendFile(fileRoute, fileName + ".txt");
        }
        else
        {
            Console.WriteLine("File was not send");
        }
        FileName.Clear();
       
    }
    #endregion

    private void Refresh()
    {
        fileList.Clear();
        App.Refresh();
    }
    
    public void AddFile(File file)
    {
        Dispatcher.Invoke(() =>
        {
            if (fileList.Contains(file))
            {
                return;
            }
            fileList.Add(file);
        });
    }
    
    public void UpdateProgress(int bytesDone)
    {
        // (Part / Whole) * 100 = a percentage
        int percentage = (int) (bytesDone / (double) selectedFileSizeInBytes * 100);
        Dispatcher.Invoke(() =>
        {
            DownloadProgressBar.Value = percentage;
        });
    }

    public void ReportFileDone()
    {
        Dispatcher.Invoke(() =>
        {
            DownloadProgressBar.Value = 100;
        });
    }

    private void ResetProgressBar()
    {
        DownloadProgressBar.Value = 0;
    }
}