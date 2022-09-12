using System;
using BizHawk.Client.Common;
using BizHawk.Emulation.Common;
using SotnRandoTools.Constants;

namespace SotnRandoTools.Services
{
	internal sealed class WatchlistService : IWatchlistService
	{
		private readonly string systemId;
		private readonly IMemoryDomains memoryDomains;
		private readonly Config GlobalConfig;

		private WatchList relicWatches;
		private WatchList coopRelicWatches;
		private WatchList safeLocationWatches;
		private WatchList equipmentLocationWatches;
		private WatchList coopLocationWatches;
		private WatchList progressionItemWatches;
		private WatchList thrustSwordWatches;
		private WatchList warpsAndShortcutsWatches;
		private int[] coopLocationValues;

		public WatchlistService(IMemoryDomains? memoryDomains, string? systemId, Config GlobalConfig)
		{
			if (memoryDomains is null) throw new ArgumentNullException(nameof(memoryDomains));
			if (systemId is null) throw new ArgumentNullException("SystemID");
			if (systemId == "") throw new ArgumentException($"Parameter {nameof(systemId)} is empty!");
			this.systemId = systemId;
			this.memoryDomains = memoryDomains;
			this.GlobalConfig = GlobalConfig;

			relicWatches = new WatchList(this.memoryDomains, this.systemId);
			relicWatches.Load(Paths.RelicWatchesPath, false);
			RelicWatches = relicWatches;

			coopRelicWatches = new WatchList(this.memoryDomains, this.systemId);
			coopRelicWatches.Load(Paths.RelicWatchesPath, false);
			CoopRelicWatches = coopRelicWatches;

			safeLocationWatches = new WatchList(this.memoryDomains, this.systemId);
			safeLocationWatches.Load(Paths.SafeLocationWatchesPath, false);
			SafeLocationWatches = safeLocationWatches;

			equipmentLocationWatches = new WatchList(this.memoryDomains, this.systemId);
			equipmentLocationWatches.Load(Paths.EquipmentLocationWatchesPath, false);
			EquipmentLocationWatches = equipmentLocationWatches;

			coopLocationWatches = new WatchList(this.memoryDomains, this.systemId);
			coopLocationWatches.Load(Paths.SafeLocationWatchesPath, false);
			coopLocationWatches.Load(Paths.EquipmentLocationWatchesPath, true);
			CoopLocationWatches = coopLocationWatches;

			coopLocationValues = new int[coopLocationWatches.Count];
			for (int i = 0; i < coopLocationValues.Length; i++)
			{
				coopLocationValues[i] = 0;
			}
			CoopLocationValues = coopLocationValues;

			progressionItemWatches = new WatchList(this.memoryDomains, this.systemId);
			progressionItemWatches.Load(Paths.ProgressionItemWatchesPath, false);
			ProgressionItemWatches = progressionItemWatches;

			thrustSwordWatches = new WatchList(this.memoryDomains, this.systemId);
			thrustSwordWatches.Load(Paths.ThrustSwordWatchesPath, false);
			ThrustSwordWatches = thrustSwordWatches;

			warpsAndShortcutsWatches = new WatchList(this.memoryDomains, this.systemId);
			warpsAndShortcutsWatches.Load(Paths.WarpsAndShortcutsWatchPath, false);
			WarpsAndShortcutsWatches = warpsAndShortcutsWatches;
		}

		public WatchList RelicWatches { get; }
		public WatchList CoopRelicWatches { get; }
		public WatchList SafeLocationWatches { get; }
		public WatchList EquipmentLocationWatches { get; }
		public WatchList CoopLocationWatches { get; }
		public int[] CoopLocationValues { get; }
		public WatchList ProgressionItemWatches { get; }
		public WatchList ThrustSwordWatches { get; }
		public WatchList WarpsAndShortcutsWatches { get; }

		public void UpdateWatchlist(WatchList watches)
		{
			watches.UpdateValues(GlobalConfig.RamWatchDefinePrevious);
		}

		public void ClearAll()
		{
			relicWatches.ClearChangeCounts();
			safeLocationWatches.ClearChangeCounts();
			equipmentLocationWatches.ClearChangeCounts();
			progressionItemWatches.ClearChangeCounts();
			thrustSwordWatches.ClearChangeCounts();
		}
	}
}
