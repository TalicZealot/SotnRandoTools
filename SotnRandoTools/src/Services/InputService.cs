using System;
using System.Collections.Generic;
using System.Linq;
using BizHawk.Client.Common;
using SotnApi.Interfaces;
using SotnRandoTools.Services.Models;

namespace SotnRandoTools.Services
{
	public class InputService : IInputService
	{
		private readonly IJoypadApi joypadApi;
		private readonly IAlucardApi alucardApi;
		private List<Dictionary<string, object>> inputHistory = new();
		private List<Dictionary<string, bool>> moveHistory = new();
		private Input dragonPunch = new Input
		{
			MotionSequence = new List<Dictionary<string, object>>
			{
				new Dictionary<string, object> {["P1 Forward"] = true, ["P1 Down"] = false},
				new Dictionary<string, object> {["P1 Forward"] = false, ["P1 Down"] = true},
				new Dictionary<string, object> {["P1 Forward"] = true, ["P1 Down"] = true}
			},
			Activator = new Dictionary<string, object> { ["P1 L2"] = true }
		};
		private Input halfCircle = new Input
		{
			MotionSequence = new List<Dictionary<string, object>>
			{
				new Dictionary<string, object> {["P1 Back"] = true, ["P1 Down"] = false},
				new Dictionary<string, object> {["P1 Forward"] = true, ["P1 Down"] = true},
				new Dictionary<string, object> {["P1 Forward"] = false, ["P1 Down"] = true},
				new Dictionary<string, object> {["P1 Back"] = true, ["P1 Down"] = true},
				new Dictionary<string, object> {["P1 Forward"] = true, ["P1 Down"] = false}
			},
			Activator = new Dictionary<string, object> { ["P1 L2"] = true }
		};

		public InputService(IJoypadApi joypadApi, IAlucardApi alucardApi)
		{
			if (joypadApi is null) throw new ArgumentNullException(nameof(joypadApi));
			if (alucardApi is null) throw new ArgumentNullException(nameof(alucardApi));
			this.joypadApi = joypadApi;
			this.alucardApi = alucardApi;
		}

		public void UpdateInputs()
		{
			inputHistory.Add(joypadApi.Get().ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
			if (inputHistory.Count > 120)
			{
				inputHistory.RemoveAt(0);
			}

			if (alucardApi.FacingLeft)
			{
				inputHistory[inputHistory.Count - 1].Add("P1 Forward", Convert.ToBoolean(inputHistory[inputHistory.Count - 1]["P1 Left"]));
				inputHistory[inputHistory.Count - 1].Add("P1 Back", Convert.ToBoolean(inputHistory[inputHistory.Count - 1]["P1 Right"]));
			}
			else
			{
				inputHistory[inputHistory.Count - 1].Add("P1 Forward", Convert.ToBoolean(inputHistory[inputHistory.Count - 1]["P1 Right"]));
				inputHistory[inputHistory.Count - 1].Add("P1 Back", Convert.ToBoolean(inputHistory[inputHistory.Count - 1]["P1 Left"]));
			}

			moveHistory.Add(new Dictionary<string, bool>());
			if (ReadInput(dragonPunch, 30))
			{
				moveHistory[moveHistory.Count - 1].Add("DP", true);
			}
			else
			{
				moveHistory[moveHistory.Count - 1].Add("DP", false);
			}

			if (ReadInput(halfCircle, 30))
			{
				moveHistory[moveHistory.Count - 1].Add("HCF", true);
			}
			else
			{
				moveHistory[moveHistory.Count - 1].Add("HCF", false);
			}

			if (moveHistory.Count > 120)
			{
				moveHistory.RemoveAt(0);
			}
		}

		public bool RegisteredDp
		{
			get
			{
				for (int i = 0; i < 11; i++)
				{
					if (i >= moveHistory.Count)
					{
						return false;
					}

					if (moveHistory[i]["DP"] == true)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool RegisteredHcf
		{
			get
			{
				for (int i = 0; i < 11; i++)
				{
					if (i >= moveHistory.Count)
					{
						return false;
					}

					if (moveHistory[i]["HCF"] == true)
					{
						return true;
					}
				}
				return false;
			}
		}

		private bool ReadInput(Input moveInput, uint bufferSize)
		{
			if (moveInput is null) throw new ArgumentNullException(nameof(moveInput));
			if (bufferSize < 1) throw new ArgumentOutOfRangeException(nameof(bufferSize));

			int inputIndex = 0;
			int inputBuffer = 0;
			bool keyUp = true;
			bool directionalInput = false;
			bool pressed = true;

			var requiredActivator = moveInput.Activator.Where(pair => Convert.ToBoolean(pair.Value)).ToArray();
			foreach (var pair in requiredActivator)
			{
				if (inputHistory.Count > 0 && !Convert.ToBoolean(inputHistory[inputHistory.Count - 1][pair.Key]))
				{
					return false;
				}
				else if (inputHistory.Count == 0)
				{
					return false;
				}
			}

			for (int i = 0; i < inputHistory.Count; i++)
			{
				pressed = true;

				if (inputBuffer > bufferSize)
				{
					return false;
				}

				if (!keyUp && inputIndex > 0)
				{
					bool buttonHeld = true;
					var previousInputRequiredDirections = moveInput.MotionSequence[inputIndex - 1].Where(pair => Convert.ToBoolean(pair.Value)).ToArray();
					foreach (var pair in previousInputRequiredDirections)
					{
						if (!Convert.ToBoolean(inputHistory[i][pair.Key]))
						{
							keyUp = true;
							buttonHeld = false;
							break;
						}
					}
					if (buttonHeld)
					{
						inputBuffer++;
					}
				}
				else if (keyUp && inputIndex > 0)
				{
					inputBuffer++;
				}

				if (!directionalInput)
				{
					foreach (var pair in moveInput.MotionSequence[inputIndex])
					{
						if (Convert.ToBoolean(inputHistory[i][pair.Key]) != Convert.ToBoolean(pair.Value))
						{
							pressed = false;
							keyUp = true;
							break;
						}
					}
				}

				if (pressed && !directionalInput)
				{
					if (inputIndex == moveInput.MotionSequence.Count - 1)
					{
						directionalInput = true;
						inputBuffer = 0;
					}
					inputIndex++;
					inputBuffer = 0;
				}

				pressed = true;

				if (directionalInput && i < inputHistory.Count - 1)
				{
					foreach (var pair in requiredActivator)
					{
						if (!Convert.ToBoolean(inputHistory[i][pair.Key]))
						{
							pressed = false;
							break;
						}
					}
					if (pressed)
					{
						return false;
					}
				}
			}

			return directionalInput;
		}
	}
}
