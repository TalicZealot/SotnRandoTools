using SotnRandoTools.Khaos.Models;

namespace SotnRandoTools.Khaos.Interfaces
{
	public interface IKhaosController
	{
		void OverwriteBossNames(string[] subscribers);
		public void EnqueueAction(EventAddAction eventData);
	}
}
