using Server.Enums;

namespace UtilityLibrary;

/**
 * Library class with methods we can use at multiple points in the code
 */
public class Utils
{
	#region FileIO
	/**
	 * Method that creates an empty file for when a file is uploaded
	 * <param name="path">The path for the file</param>
	 */
	public static void CreateNewEmptyFile(string path)
	{
		try
		{ 
			File.Create(path).Dispose();
		}
		catch (Exception e)
		{
			Console.WriteLine($"Could not make file with exception\n{e}");
		}
	}
	
	/**
	 * Method to write string to a file
	 * <param name="writer">writer to the file</param>
	 * <param name="content">The string to put in the file</param>
	 * <param name="line">Indicate if you are writing a line or not</param>
	 * Side note: The "bool = false" in the constructor means it's optional.
	 */
	public static async Task WriteToFile(StreamWriter writer, string content, bool line = false)
	{
		try
		{
			if (line)
			{
				await writer.WriteLineAsync(content);
				return;
			}
			await writer.WriteAsync(content);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Something went wrong writing to the file \n{ex}");
		}
	}
	#endregion

	//Socket method that both projects would use
	public static void SendData(StreamWriter writer, string data)
	{
		try
		{
			writer.WriteLine(data);
			writer.Flush();
		}
		catch (Exception e)
		{
			Console.WriteLine($"Sending failed with exception\n{e}");
		}
	}
	
	/**
 * makes the unit from bytes to KB or MB when needed;
 */
	public static string FormatDataSize(long amountOfBytes)
	{
		long kbUnit = (long)DataSizeEnums.KB; //1.000 bytes
		long mbUnit = (long)DataSizeEnums.MB; //1.000.000 bytes
		long gbUnit = (long)DataSizeEnums.GB; //1.000.000.000 bytes
		
		//check for negative amount, if so unable to parse.
		if (amountOfBytes < 0)
			return "Unable to parse data size";
		
		//check if it's smaller than 1KB
		if (amountOfBytes < kbUnit)
			return $"{amountOfBytes} B";
		
		//check if it's smaller than 1MB
		if (amountOfBytes < mbUnit)
			return $"{amountOfBytes / kbUnit} KB";
		
		//check if it's smaller than 1GB
		if (amountOfBytes < gbUnit)
			return $"{amountOfBytes / mbUnit} MB";
		
		//anything bigger than MB just put GB
		return $"{amountOfBytes / gbUnit} GB";
	}
}