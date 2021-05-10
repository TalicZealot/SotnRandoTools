namespace SotnRandoTools.Services
{
	public interface IInputService
	{
		bool RegisteredMove(string moveName, int frames);
		bool ButtonPressed(string button, int frames);
		void UpdateInputs();
	}
}