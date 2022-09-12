using System;
using System.Collections.Generic;
using System.IO;
using BizHawk.Client.Common;
using SotnRandoTools.Constants;
using SotnRandoTools.Services.Interfaces;

namespace SotnRandoTools.Services
{
	internal sealed class DataService : IDataService
	{
		private readonly ISQLiteApi sQLiteApi;

		public DataService(ISQLiteApi sQLiteApi)
		{
			if (sQLiteApi is null) throw new ArgumentNullException(nameof(sQLiteApi));
			this.sQLiteApi = sQLiteApi;
		}

		public void Connect()
		{
			string result;
			if (!File.Exists(Paths.KhaosDatabase))
			{
				CreateDatabase();
				result = sQLiteApi.OpenDatabase(Paths.KhaosDatabase);
				Console.WriteLine("Opening damtabase... \r\n" + result);
				CreateTable();
			}
			else
			{
				result = sQLiteApi.OpenDatabase(Paths.KhaosDatabase);
				Console.WriteLine("Opening damtabase... \r\n" + result);
			}
		}

		public int GetPoints(string username)
		{
			var result = sQLiteApi.ReadCommand($"SELECT khaos_points FROM users WHERE username = '{username}'");
			Console.WriteLine("Selecting points... \r\n" + result);
			if (result is not null && result.ToString() == "No rows found")
			{
				return 0;
			}
			else if (result is not null)
			{
				Dictionary<string, object>? table = result as Dictionary<string, object>;
				int points = Convert.ToInt32(table["khaos_points 0"]);
				Console.WriteLine("Points: " + points);
				return points;
			}
			else
			{
				return 0;
			}
		}

		public void SetPoints(string username, int points)
		{
			if (!UserExists(username))
			{
				AddUser(username);
			}
			var result = sQLiteApi.WriteCommand($"UPDATE users SET khaos_points = {points} WHERE username = '{username}'");
			Console.WriteLine("Adding points... \r\n" + result);
		}

		private void CreateDatabase()
		{
			var result = sQLiteApi.CreateDatabase(Paths.KhaosDatabase);
			Console.WriteLine("Creating database... \r\n" + result);
		}

		private void CreateTable()
		{
			var result = sQLiteApi.WriteCommand(
				@"CREATE TABLE users (
				username TEXT NOT NULL UNIQUE PRIMARY KEY,
				khaos_points INTEGER NOT NULL
				);");
			Console.WriteLine("Creating table... \r\n" + result);
		}

		private bool UserExists(string username)
		{
			var result = sQLiteApi.ReadCommand($"SELECT 1 FROM users WHERE username = '{username}'");
			Console.WriteLine("Checking user... \r\n" + result);
			return result.ToString() != "No rows found";
		}

		private void AddUser(string username)
		{
			var result = sQLiteApi.WriteCommand(
				$@"INSERT INTO users (username, khaos_points)
					VALUES 
					('{username}', 0);
				");
			Console.WriteLine("Adding user... \r\n" + result);
		}
	}
}
