using System;
using System.Collections.Generic;
using System.Linq;
using SotnApi.Constants.Addresses;
using SotnApi.Interfaces;
using SotnRandoTools.Khaos.Interfaces;

namespace SotnRandoTools.Khaos
{
	public class EnemyRenamer : IEnemyRenamer
	{
		private readonly Random rng = new();
		private readonly ISotnApi sotnApi;

		public EnemyRenamer(ISotnApi sotnApi)
		{
			if (sotnApi is null) throw new ArgumentNullException(nameof(sotnApi));
			this.sotnApi = sotnApi;
		}

		public void OverwriteNames(string[] subscribers)
		{
			subscribers = subscribers.OrderBy(x => rng.Next()).ToArray();
			IOrderedEnumerable<KeyValuePair<string, long>>? randomizedBosses = Strings.BossNameAddresses.OrderBy(x => rng.Next());
			IOrderedEnumerable<KeyValuePair<string, long>>? randomizedEnemies = Strings.EnemyNameAddresses.OrderBy(x => rng.Next());
			int i = 0;
			foreach (KeyValuePair<string, long> boss in randomizedBosses)
			{
				if (i == subscribers.Length)
				{
					return;
				}
				sotnApi.GameApi.OverwriteString(boss.Value, subscribers[i], true);
				Console.WriteLine($"{boss.Key} renamed to {subscribers[i]}");
				i++;
			}
			foreach (KeyValuePair<string, long> enemy in randomizedEnemies)
			{
				if (i == subscribers.Length)
				{
					return;
				}
				sotnApi.GameApi.OverwriteString(enemy.Value, subscribers[i], true);
				Console.WriteLine($"{enemy.Key} renamed to {subscribers[i]}");
				i++;
			}
		}
	}
}
