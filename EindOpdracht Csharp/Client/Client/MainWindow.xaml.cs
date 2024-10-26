using System.ComponentModel;
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
using System.Windows.Shapes;
using Microsoft.Win32;

namespace Client;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private string fileRoute = "'";
    private string fileName = "";
    private List<string> fileList;
    public MainWindow()
    {
        InitializeComponent();
        Refresh();

    }

    private void RefreshButtonClicked(object sender, RoutedEventArgs e)
    {
        Refresh();
    }

    private void DownloadButtonClicked(object sender, RoutedEventArgs e)
    {
        if (FileList.SelectedItem is string selectedItem)
        {
            App.Download(selectedItem);
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
            Console.WriteLine(fileRoute);
        }
    }

    private void UploadButtonClicked(object sender, RoutedEventArgs e)
    {
        fileName = FileName.Text;
        if (fileRoute.Length > 1 && fileName.Length > 1)
        {
            Console.WriteLine("File with Route: " + fileRoute + " is send with name: " + fileName);
            App.SendFile(fileRoute, fileName);
        }
        else
        {
            Console.WriteLine("File was not send");
        }
        FileName.Clear();
       
    }

    private void Refresh()
    {
        FileList.Items.Clear();
        App.Refresh();
        fileList = App.GetAvailableFileNames();
        fileList.ForEach(var => FileList.Items.Add(var));
    }
}