using SotnRandoTools.Khaos.Models;

namespace SotnRandoTools.Khaos.Interfaces
{
	public interface IKhaosController
	{
		void OverwriteNames(string[] subscribers);
		public void EnqueueAction(EventAddAction eventData);
	}
}
