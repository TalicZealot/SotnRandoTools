namespace SotnRandoTools.Services.Interfaces
{
	internal interface IDataService
	{
		void Connect();
		int GetPoints(string username);
		void SetPoints(string username, int points);
	}
}
