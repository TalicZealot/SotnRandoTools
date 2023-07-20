using System;
using System.Collections.Generic;
using BizHawk.Client.Common;
using SotnApi.Interfaces;
using SotnRandoTools.Constants;

namespace SotnRandoTools.Services
{
	internal sealed class InputService : IInputService
	{
		private readonly IJoypadApi joypadApi;
		private readonly ISotnApi sotnApi;
		private List<IReadOnlyDictionary<string, object>> inputHistory = new();
		private bool numashockCore = true;

		private bool cleared = true;

		public InputService(IJoypadApi joypadApi, ISotnApi sotnApi)
		{
			this.joypadApi = joypadApi ?? throw new ArgumentNullException(nameof(joypadApi));
			this.sotnApi = sotnApi ?? throw new ArgumentNullException(nameof(sotnApi));

			if (!CheckNymashock())
			{
				numashockCore = false;
			}
		}

		public int Polling { get; set; }

		public void UpdateInputs()
		{
			if (Polling < 1)
			{
				if (!cleared)
				{
					cleared = true;
					inputHistory.Clear();
				}
				return;
			}
			else
			{
				cleared = false;
			}

			inputHistory.Add(joypadApi.Get());

			if (inputHistory.Count > 10)
			{
				inputHistory.RemoveAt(0);
			}
		}

		public bool ButtonPressed(string button, int frames)
		{
			if (frames < 1 || frames > Constants.Globals.MaximumCooldownFrames) throw new ArgumentOutOfRangeException(nameof(frames), $"Frames must be between 1 and {Constants.Globals.MaximumCooldownFrames}");
			if (string.IsNullOrEmpty(button)) throw new ArgumentNullException(nameof(button));

			if (inputHistory.Count < 10)
			{
				return false;
			}

			string buttonKey = button;

			if (!numashockCore)
			{
				buttonKey = PlaystationInputKeys.OctoshockKeys[button];
			}

			for (int i = 0; i < frames; i++)
			{
				if (Convert.ToBoolean(inputHistory[inputHistory.Count - 1 - i][buttonKey]) == true)
				{
					if (Convert.ToBoolean(inputHistory[inputHistory.Count - 2 - i][buttonKey]) == false)
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool SupportsR3()
		{
			IReadOnlyDictionary<string, object> inputs = joypadApi.Get();
			return inputs.ContainsKey(PlaystationInputKeys.R3);
		}

		private bool CheckNymashock()
		{
			return joypadApi.Get().ContainsKey(PlaystationInputKeys.Circle);
		}
	}
}
