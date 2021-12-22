using System.Windows.Forms;
using SotnRandoTools.Khaos.Enums;

namespace SotnRandoTools.Khaos.Models
{
	public class QueuedAction
	{
		public QueuedAction()
		{
			LocksSpeed = false;
			LocksMana = false;
			LocksInvincibility = false;
			LocksSpawning = false;
		}
		public string Name { get; set; }
		public MethodInvoker Invoker { get; set; }
		public bool LocksSpeed { get; set; }
		public bool LocksMana { get; set; }
		public bool LocksInvincibility { get; set; }
		public bool LocksSpawning { get; set; }
	}
}
