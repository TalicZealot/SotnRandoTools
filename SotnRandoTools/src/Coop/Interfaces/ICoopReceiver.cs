using System.Collections.Concurrent;

namespace SotnRandoTools.Coop.Interfaces
{
	internal interface ICoopReceiver
	{
		ConcurrentQueue<byte[]> MessageQueue { get; set; }
	}
}