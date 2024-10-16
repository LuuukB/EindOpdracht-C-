using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Win32;

namespace Client;

public class Connection
{
    private Socket socket;
    private StreamWriter writer;
    private StreamReader reader;
    private bool Downlaoding = false;
    private StringBuilder StringBuilder;
    
    public Connection()
    {
        
        try
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(IPAddress.Parse("127.0.0.1"), 8888);
        }
        catch (Exception e)
        {
            Console.WriteLine("can't connect to server");
            throw;
        }
        writer = new StreamWriter(new NetworkStream(socket));
        reader = new StreamReader(new BufferedStream( new NetworkStream(socket)));
        
        new Thread(HandleMessage).Start();
    }

    private void HandleMessage()
    {
        Console.WriteLine("verbonden!");
        try
        {
            while (true)
            {
                string message = reader.ReadLine();
                Console.WriteLine(reader.ReadLine());
                if (Downlaoding)
                {
                    switch (message)
                    {
                        case "{Download Stop}":
                            Downlaoding = false;
                            string downlaodstring = StringBuilder.ToString();
                            SaveFileDialog saveFileDialog = new SaveFileDialog();
                            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                            saveFileDialog.Title = "were do u want to save the file";
                            if (saveFileDialog.ShowDialog() == true)
                            {
                                string selectedFilePath = saveFileDialog.FileName;
                                
                                File.WriteAllText(selectedFilePath, downlaodstring);
                            }
                            break;
                        default:
                            StringBuilder.AppendLine(message);
                            break;
                    }
                } else if (message == "{Upload Start}")
                {
                    Downlaoding = true;
                } else if (message.StartsWith("{FILES}"))
                {
                    
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("No message wil be received anymore");
        }

    }

    public void Send(string message)
    {
        writer.WriteLine(message);
        writer.Flush();
    }

    public void SendFile(string fileRoute, string fileName)
    {
        Send("{UPLOAD START}");
        Send(fileName.Length + fileName);
        socket.SendFile(fileRoute);
        Send("{UPLOAD DONE}");
    }

    public void Disconnect()
    {
        socket.Close();
    }
}