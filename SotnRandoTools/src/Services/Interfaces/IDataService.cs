namespace SotnRandoTools.Services.Interfaces
{
	public interface IDataService
	{
		void Connect();
		int GetPoints(string username);
		void SetPoints(string username, int points);
	}
}
