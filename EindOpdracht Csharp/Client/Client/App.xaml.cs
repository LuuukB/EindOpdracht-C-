using System.Windows;

namespace Client;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private static Connection connection;
    private static List<string> availablefileNames = new List<string>();
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Voer initiële logica uit bij het starten van de applicatie
        connection = new Connection();
        MainWindow mainWindow = new MainWindow(connection);
        mainWindow.Show();
    }
    
    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);
        
        if (connection != null)
        {
            connection.Disconnect();
        }
    }

    public static async Task SendFile(string fileRoute, string fileName)
    {
        if (connection != null)
        {
            await connection.SendFile(fileRoute, fileName);
        }
    }

    public static void Download(string fileName)
    {
        connection.Send("{DOWNLOAD} " + fileName);
    }

    public static void Refresh()
    {
        availablefileNames.Clear();
        connection.Send("{REFRESH}");
    }

    public static void AddAvailableFileNames(string[] fileName)
    {
        availablefileNames.AddRange(fileName);
    }
}