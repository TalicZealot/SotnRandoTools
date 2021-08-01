using System;
using SotnRandoTools.Khaos.Enums;

namespace SotnRandoTools.Services.Models
{
	public class ActionTimer
	{
		public string Name { get; set; }
		public TimeSpan Duration { get; set; }
		public int TotalDuration { get; set; }
		public ActionType Type { get; set; }
	}
}
