using System;

namespace SotnRandoTools.Khaos.Models
{
	public class Redemption
	{
		public string RewardId { get; set; }
		public string RedemptionId { get; set; }
		public string Username { get; set; }
		public string Title { get; set; }
		public DateTime RedeemedAt { get; set; }
	}
}
