using System.Collections.Generic;

namespace SotnRandoTools.Services
{
	public interface IInputService
	{
		bool RegisteredMove(string moveName);
		bool ButtonPressed(string button);
		void UpdateInputs();
	}
}