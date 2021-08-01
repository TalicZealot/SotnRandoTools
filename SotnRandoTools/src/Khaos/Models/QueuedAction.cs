using System.Windows.Forms;
using SotnRandoTools.Khaos.Enums;

namespace SotnRandoTools.Khaos.Models
{
	public class QueuedAction
	{
		public QueuedAction()
		{
			Type = ActionType.Debuff;
			LocksSpeed = false;
			LocksMana = false;
			LocksInvincibility = false;
		}
		public string Name { get; set; }
		public ActionType Type { get; set; }
		public MethodInvoker Invoker { get; set; }
		public bool LocksSpeed { get; set; }
		public bool LocksMana { get; set; }
		public bool LocksInvincibility { get; set; }
	}
}
