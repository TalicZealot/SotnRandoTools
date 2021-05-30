using System;
using System.Collections.Generic;
using System.Linq;
using BizHawk.Client.Common;
using SotnApi.Interfaces;
using SotnRandoTools.Constants;
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
				new Dictionary<string, object> {[InputKeys.Forward] = true, [PlaystationInputKeys.Down] = false},
				new Dictionary<string, object> {[InputKeys.Forward] = false, [PlaystationInputKeys.Down] = true},
				new Dictionary<string, object> {[InputKeys.Forward] = true, [PlaystationInputKeys.Down] = true}
			},
			Activator = new Dictionary<string, object> { [PlaystationInputKeys.L2] = true }
		};
		private Input halfCircle = new Input
		{
			MotionSequence = new List<Dictionary<string, object>>
			{
				new Dictionary<string, object> {[InputKeys.Back] = true, [PlaystationInputKeys.Up] = false},
				new Dictionary<string, object> {[InputKeys.Forward] = true, [PlaystationInputKeys.Up] = true},
				new Dictionary<string, object> {[InputKeys.Forward] = false, [PlaystationInputKeys.Up] = true},
				new Dictionary<string, object> {[InputKeys.Back] = true, [PlaystationInputKeys.Up] = true},
				new Dictionary<string, object> {[InputKeys.Forward] = true, [PlaystationInputKeys.Up] = false}
			},
			Activator = new Dictionary<string, object> { [PlaystationInputKeys.L2] = true }
		};
		private Input dash = new Input
		{
			MotionSequence = new List<Dictionary<string, object>>
			{
				new Dictionary<string, object> {[InputKeys.Forward] = true},
				new Dictionary<string, object> {[InputKeys.Forward] = false},
				new Dictionary<string, object> {[InputKeys.Forward] = true}
			},
			Activator = null
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
				inputHistory[inputHistory.Count - 1].Add(InputKeys.Forward, Convert.ToBoolean(inputHistory[inputHistory.Count - 1][PlaystationInputKeys.Left]));
				inputHistory[inputHistory.Count - 1].Add(InputKeys.Back, Convert.ToBoolean(inputHistory[inputHistory.Count - 1][PlaystationInputKeys.Right]));
			}
			else
			{
				inputHistory[inputHistory.Count - 1].Add(InputKeys.Forward, Convert.ToBoolean(inputHistory[inputHistory.Count - 1][PlaystationInputKeys.Right]));
				inputHistory[inputHistory.Count - 1].Add(InputKeys.Back, Convert.ToBoolean(inputHistory[inputHistory.Count - 1][PlaystationInputKeys.Left]));
			}

			moveHistory.Add(new Dictionary<string, bool>());
			if (ReadInput(dragonPunch, Globals.InputBufferSize))
			{
				moveHistory[moveHistory.Count - 1].Add(InputKeys.DragonPunch, true);
			}
			else
			{
				moveHistory[moveHistory.Count - 1].Add(InputKeys.DragonPunch, false);
			}

			if (ReadInput(halfCircle, Globals.InputBufferSize))
			{
				moveHistory[moveHistory.Count - 1].Add(InputKeys.HalfCircleForward, true);
			}
			else
			{
				moveHistory[moveHistory.Count - 1].Add(InputKeys.HalfCircleForward, false);
			}

			if (ReadInput(dash, Globals.InputBufferSize))
			{
				moveHistory[moveHistory.Count - 1].Add(InputKeys.Dash, true);
			}
			else
			{
				moveHistory[moveHistory.Count - 1].Add(InputKeys.Dash, false);
			}

			if (moveHistory.Count > 120)
			{
				moveHistory.RemoveAt(0);
			}
		}

		public bool RegisteredMove(string moveName, int frames)
		{
			for (int i = 0; i < frames; i++)
			{
				if (moveHistory.Count < 10)
				{
					return false;
				}

				if (moveHistory[moveHistory.Count - 1 - i][moveName] == true)
				{
					return true;
				}
			}
			return false;
		}

		public bool ButtonPressed(string button, int frames)
		{
			for (int i = 0; i < frames; i++)
			{
				if (inputHistory.Count < 10)
				{
					return false;
				}

				if (Convert.ToBoolean(inputHistory[inputHistory.Count - 1 - i][button]) == true)
				{
					return true;
				}
			}
			return false;
		}

		public bool ButtonHeld(string button)
		{
			if (inputHistory.Count < 1)
			{
				return false;
			}
			if (Convert.ToBoolean(inputHistory[inputHistory.Count - 1][button]) == true)
			{
				return true;
			}
			return false;
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
			KeyValuePair<string, Object>[]? requiredActivator = null;

			if (moveInput.Activator is not null)
			{
				requiredActivator = moveInput.Activator.Where(pair => Convert.ToBoolean(pair.Value)).ToArray();
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
			}

			for (int i = 0; i < inputHistory.Count; i++)
			{
				pressed = true;

				if (inputBuffer > bufferSize)
				{
					inputIndex = 0;
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

				if (directionalInput && i < inputHistory.Count - 1 && requiredActivator is not null)
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
