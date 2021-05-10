using BizHawk.Client.Common;

namespace SotnRandoTools.Services
{
	public interface IWatchlistService
	{
		WatchList EquipmentLocationWatches { get; }
		WatchList ProgressionItemWatches { get; }
		WatchList RelicWatches { get; }
		WatchList CoopRelicWatches { get; }
		WatchList SafeLocationWatches { get; }
		WatchList CoopLocationWatches { get; }
		int[] CoopLocationValues { get; }
		WatchList ThrustSwordWatches { get; }
		WatchList WarpsAndShortcutsWatches { get; }

		void UpdateWatchlist(WatchList watches);
		public void ClearAll();
	}
}