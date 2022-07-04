namespace SotnRandoTools.Services
{
	public interface IInputService
	{
		bool ReadDash { set; }
		bool ReadDragonPunch { set; }
		bool ReadQuarterCircle { set; }
		bool ReadHalfCircle { set; }
		int Polling { get; set; }
		bool RegisteredMove(string moveName, int frames);
		bool ButtonPressed(string button, int frames);
		bool ButtonHeld(string button);
		bool DirectionHeld(string button);
		void UpdateInputs();
	}
}