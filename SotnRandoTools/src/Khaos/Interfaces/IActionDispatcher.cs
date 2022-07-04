using SotnRandoTools.Khaos.Models;

namespace SotnRandoTools.Khaos.Interfaces
{
	public interface IActionDispatcher
	{
		bool AutoKhaosOn { get; set; }

		void EnqueueAction(EventAddAction eventData);
		void StartActions();
		void StopActions();
	}
}