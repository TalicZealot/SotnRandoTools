namespace SotnRandoTools.Services
{
	public interface IInputService
	{
		bool RegisteredDp { get; }
		bool RegisteredHcf { get; }

		void UpdateInputs();
	}
}