using System;
using System.Linq;
using BizHawk.Client.Common;
using BizHawk.Emulation.Common;

namespace SotnRandoTools.Services.Adapters
{
	public class CheatCollectionAdapter : ICheatCollectionAdapter
	{
		private readonly CheatCollection cheats;
		public CheatCollectionAdapter(CheatCollection cheats)
		{
			if (cheats is null) throw new ArgumentNullException(nameof(cheats));
			this.cheats = cheats;
		}

		public Cheat this[int index]
		{
			get
			{
				return cheats[index];
			}
		}

		public bool Load(IMemoryDomains domains, string path, bool append)
		{
			return cheats.Load(domains, path, append);
		}

		public void DisableAll()
		{
			cheats.DisableAll();
		}

		public Cheat GetCheatByName(string name)
		{
			return cheats.Where(x => x.Name == name).FirstOrDefault();
		}
	}
}
