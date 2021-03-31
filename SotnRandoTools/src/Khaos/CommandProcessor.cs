using System.Collections.Generic;
using SotnRandoTools.Khaos.Models;

namespace SotnRandoTools.Khaos
{
	public class CommandProcessor
	{
		private List<Command> commands = new List<Command>
		{
		};
		public CommandProcessor()
		{
			//inject khaosController
		}
		public void GetAction(string command)
		{
			var commandArguments = command.Split(' ');
			string action = commandArguments[0];
			string user = commandArguments[1];
		}
	}
}
