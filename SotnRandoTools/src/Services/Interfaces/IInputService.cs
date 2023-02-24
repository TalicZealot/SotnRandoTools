namespace SotnRandoTools.Services
{
	internal interface IInputService
	{
		int Polling { get; set; }
		bool ButtonPressed(string button, int frames);
		void UpdateInputs();
	}
}