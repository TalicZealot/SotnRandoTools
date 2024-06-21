using SotnRandoTools.Coop.Enums;

namespace SotnRandoTools.Coop.Models
{
	internal struct Message
	{
		public MessageType Type { get; set; }
		public byte[] Data { get; set; }
	}
}
