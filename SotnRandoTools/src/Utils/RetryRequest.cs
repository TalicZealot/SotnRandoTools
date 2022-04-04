using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SotnRandoTools.Utils
{
	public static class RetryRequest
	{
		public static async Task<T> Do<T>(Func<Task<T>> action, int baseInterval, int attempts)
		{
			List<Exception> exceptions = new();

			for (int i = 0; i <= attempts; i++)
			{
				try
				{
					return await action();
				}
				catch (Exception e)
				{
					if (i == attempts)
					{
						exceptions.Add(e);
					}
					else
					{
						await Task.Delay((int) Math.Pow(baseInterval, i));
					}
				}
			}

			throw new AggregateException(exceptions);
		}

		public static async Task Do(Func<Task> action, int baseInterval, int attempts)
		{
			for (int i = 0; i <= attempts; i++)
			{
				try
				{
					await action();
					break;
				}
				catch (Exception e)
				{
					if (i == attempts)
					{
						throw new Exception("Server error.", e);
					}
					else
					{
						await Task.Delay((int) Math.Pow(baseInterval, i));
					}
				}
			}

			return;
		}
	}
}
