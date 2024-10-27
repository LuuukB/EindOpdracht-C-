using UtilityLibrary;

namespace UnitTest;

public class TestDataFormatMethod
{
	[SetUp]
	public void Setup()
	{
	}

	[Test]
	public void TestAWrongByteAmount()
	{
		long bytes = -1;
		string formattedData = Utils.FormatDataSize(bytes);
		Assert.That(formattedData, Is.EqualTo("Unable to parse data size"));
	}
	
	[Test]
	public void TestByteFormat()
	{
		long bytes = 500;
		string formattedData = Utils.FormatDataSize(bytes);
		Assert.That(formattedData, Is.EqualTo("500 B"));
	}

	[Test]
	public void TestKbFormat()
	{
		//30 * 1000 = 30 KB
		long bytes = 30 * 1000;
		string formattedData = Utils.FormatDataSize(bytes);
		Assert.That(formattedData, Is.EqualTo("30 KB"));
	}

	[Test]
	public void TestMbFormat()
	{
		//30 * 1000 * 1000 = 30 MB
		long bytes = 30 * 1000 * 1000;
		string formattedData = Utils.FormatDataSize(bytes);
		Assert.That(formattedData, Is.EqualTo("30 MB"));
	}

	[Test]
	public void TestGbFormat()
	{
		//30 * 10^9 = 30 Gb (30 billion bytes)
		long bytes = 30 * (long) Math.Pow(10, 9);
		string formattedData = Utils.FormatDataSize(bytes);
		Assert.That(formattedData, Is.EqualTo("30 GB"));
	}
	
}