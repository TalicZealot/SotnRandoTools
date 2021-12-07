using System.Collections.Generic;
using SotnRandoTools.Khaos.Models;
using SotnRandoTools.Services.Models;

namespace SotnRandoTools.Khaos.Interfaces
{
	public interface IKhaosActionsInfoDisplay
	{
		List<QueuedAction> ActionQueue { get; set; }
		void AddTimer(ActionTimer timer);
		bool ContainsTimer(string name);
	}
}
