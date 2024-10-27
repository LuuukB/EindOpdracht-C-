using System.Collections.ObjectModel;
using System.Windows;
using Microsoft.Win32;

namespace Client;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window, IUpdateFileList
{
    private string fileRoute = "'";
    private string fileName = "";
    private ObservableCollection<File> fileList;
    private Connection connection;
    public MainWindow(Connection connection)
    {
        InitializeComponent();
        connection.UpdateFileList = this;
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
}