using System.Collections.Generic;

namespace SotnRandoTools.Services.Models
{
	public class Input
	{
		public List<Dictionary<string, object>> MotionSequence { get; set; }
		public Dictionary<string, object>? Activator { get; set; }
	}
}
