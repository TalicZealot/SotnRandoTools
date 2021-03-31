using BizHawk.Client.Common;
using BizHawk.Emulation.Common;

namespace SotnRandoTools.Services.Adapters
{
	public interface ICheatCollectionAdapter
	{
		Cheat this[int index] { get; }

		void DisableAll();
		Cheat GetCheatByName(string name);
		bool Load(IMemoryDomains domains, string path, bool append);
	}
}