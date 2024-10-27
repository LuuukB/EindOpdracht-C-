namespace Client;

public class File
{
	public string fileName { get; }
	public string fileSize { get;  }
	public File(string name, string size)
	{
		fileName = name;
		fileSize = size;
	}

}