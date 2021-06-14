using System.Collections.Generic;
using SotnRandoTools.Khaos.Enums;
using SotnRandoTools.Khaos.Models;
using SotnRandoTools.Services.Models;

namespace SotnRandoTools.Services
{
	public interface INotificationService
	{
		List<QueuedAction> ActionQueue { get; set; }
		double Volume { set; }
		void AddMessage(string message);
		void AddTimer(ActionTimer timer);
		void PlayAlert(string uri);
	}
}