namespace Client;

public interface IUpdateProgressBar
{
	void UpdateProgress(int bytesDone);
	void ReportFileDone();
}