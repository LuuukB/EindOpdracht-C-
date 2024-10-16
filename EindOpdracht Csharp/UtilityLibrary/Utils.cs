namespace UtilityLibrary;

/**
 * Library class with methods we can use at multiple points in the code
 */
public class Utils
{
	#region FileIO
	/**
	 * Method that creates a new file if needed, otherwise it will overwrite the contents (for now)
	 * <param name="path">Path to the file</param>
	 * <param name="content">The string to put in the file</param>
	 */
	public static void CreateNewFile(string path, string content)
	{
		using FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
		fs.Seek(0, SeekOrigin.End);
		WriteToFile(fs, content);
	}
	
	/**
	 * Method to write string to a file
	 * <param name="fileStream">Stream of the file</param>
	 * <param name="content">The string to put in the file</param>
	 */
	private static void WriteToFile(FileStream fileStream, string content)
	{
		try
		{
			using StreamWriter streamWriter = new StreamWriter(fileStream);
			streamWriter.Write(content);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Something went wrong writing to the file \n{ex}");
		}
	}
	#endregion
}