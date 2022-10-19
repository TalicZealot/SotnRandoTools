using System;
using System.Collections.Generic;
using System.Linq;
using BizHawk.Client.Common;
using SotnApi.Interfaces;
using SotnRandoTools.Constants;
using SotnRandoTools.Services.Models;

namespace SotnRandoTools.Services
{
	internal sealed class InputService : IInputService
	{
		private readonly IJoypadApi joypadApi;
		private readonly ISotnApi sotnApi;
		private List<IReadOnlyDictionary<string, object>> inputHistory = new();
		private List<Dictionary<string, object>> diractionalHistory = new();
		private List<Dictionary<string, bool>> moveHistory = new();
		private Dictionary<string, Input> inputs = new()
		{
			{
				InputKeys.DragonPunch,
				new Input
				{
					Enabled = false,
					MotionSequence = new List<Dictionary<string, object>>
					{
						new Dictionary<string, object> {[InputKeys.Forward] = true, [PlaystationInputKeys.Down] = false},
						new Dictionary<string, object> {[InputKeys.Forward] = false, [PlaystationInputKeys.Down] = true},
						new Dictionary<string, object> {[InputKeys.Forward] = true, [PlaystationInputKeys.Down] = true}
					},
					Activator = new Dictionary<string, object> { [PlaystationInputKeys.L2] = true }
				}
			},
			{
				InputKeys.HalfCircleForward,
				new Input
				{
					Enabled = false,
					MotionSequence = new List<Dictionary<string, object>>
					{
						new Dictionary<string, object> {[InputKeys.Back] = true, [PlaystationInputKeys.Up] = false},
						new Dictionary<string, object> {[InputKeys.Forward] = true, [PlaystationInputKeys.Up] = true},
						new Dictionary<string, object> {[InputKeys.Forward] = false, [PlaystationInputKeys.Up] = true},
						new Dictionary<string, object> {[InputKeys.Back] = true, [PlaystationInputKeys.Up] = true},
						new Dictionary<string, object> {[InputKeys.Forward] = true, [PlaystationInputKeys.Up] = false}
					},
					Activator = new Dictionary<string, object> { [PlaystationInputKeys.L2] = true }
				}
			},
			{
				InputKeys.QuarterCircleForward,
				new Input
				{
					Enabled = false,
					MotionSequence = new List<Dictionary<string, object>>
					{
						new Dictionary<string, object> {[InputKeys.Forward] = false, [PlaystationInputKeys.Down] = true},
						new Dictionary<string, object> {[InputKeys.Forward] = true, [PlaystationInputKeys.Down] = true},
						new Dictionary<string, object> {[InputKeys.Forward] = true, [PlaystationInputKeys.Down] = false}
					},
					Activator = new Dictionary<string, object> { [PlaystationInputKeys.Square] = true }
				}
			},
			{
				InputKeys.QuarterCircleRight,
				new Input
				{
					Enabled = false,
					MotionSequence = new List<Dictionary<string, object>>
					{
						new Dictionary<string, object> {[PlaystationInputKeys.Right] = false, [PlaystationInputKeys.Down] = true},
						new Dictionary<string, object> {[PlaystationInputKeys.Right] = true, [PlaystationInputKeys.Down] = true},
						new Dictionary<string, object> {[PlaystationInputKeys.Right] = true, [PlaystationInputKeys.Down] = false}
					},
					Activator = new Dictionary<string, object> { [PlaystationInputKeys.Square] = true }
				}
			},
			{
				InputKeys.QuarterCircleLeft,
				new Input
				{
					Enabled = false,
					MotionSequence = new List<Dictionary<string, object>>
					{
						new Dictionary<string, object> {[PlaystationInputKeys.Left] = false, [PlaystationInputKeys.Down] = true},
						new Dictionary<string, object> {[PlaystationInputKeys.Left] = true, [PlaystationInputKeys.Down] = true},
						new Dictionary<string, object> {[PlaystationInputKeys.Left] = true, [PlaystationInputKeys.Down] = false}
					},
					Activator = new Dictionary<string, object> { [PlaystationInputKeys.Square] = true }
				}
			},
			{
				InputKeys.Dash,
				new Input
				{
					Enabled = false,
					MotionSequence = new List<Dictionary<string, object>>
					{
						new Dictionary<string, object> {[InputKeys.Forward] = false},
						new Dictionary<string, object> {[InputKeys.Forward] = true},
						new Dictionary<string, object> {[InputKeys.Forward] = false},
						new Dictionary<string, object> {[InputKeys.Forward] = true}
					},
					Activator = null
				}
			}
		};
		private bool numashockCore = true;
		private string leftKey = PlaystationInputKeys.Left;
		private string rightKey = PlaystationInputKeys.Right;
		private string upKey = PlaystationInputKeys.Up;
		private string downKey = PlaystationInputKeys.Down;

		private bool cleared = true;

		public InputService(IJoypadApi joypadApi, ISotnApi sotnApi)
		{
			if (joypadApi is null) throw new ArgumentNullException(nameof(joypadApi));
			if (sotnApi is null) throw new ArgumentNullException(nameof(sotnApi));
			this.joypadApi = joypadApi;
			this.sotnApi = sotnApi;

			if (!CheckNymashock())
			{
				numashockCore = false;
				leftKey = OctoshockPlaystationInputKeys.Left;
				rightKey = OctoshockPlaystationInputKeys.Right;

				inputs[InputKeys.DragonPunch] = new Input
				{
					Enabled = false,
					MotionSequence = new List<Dictionary<string, object>>
					{
						new Dictionary<string, object> {[InputKeys.Forward] = true, [OctoshockPlaystationInputKeys.Down] = false},
						new Dictionary<string, object> {[InputKeys.Forward] = false, [OctoshockPlaystationInputKeys.Down] = true},
						new Dictionary<string, object> {[InputKeys.Forward] = true, [OctoshockPlaystationInputKeys.Down] = true}
				},
					Activator = new Dictionary<string, object> { [OctoshockPlaystationInputKeys.L2] = true }
				};
				inputs[InputKeys.HalfCircleForward] = new Input
				{
					Enabled = false,
					MotionSequence = new List<Dictionary<string, object>>
					{
						new Dictionary<string, object> {[InputKeys.Back] = true, [OctoshockPlaystationInputKeys.Up] = false},
						new Dictionary<string, object> {[InputKeys.Forward] = true, [OctoshockPlaystationInputKeys.Up] = true},
						new Dictionary<string, object> {[InputKeys.Forward] = false, [OctoshockPlaystationInputKeys.Up] = true},
						new Dictionary<string, object> {[InputKeys.Back] = true, [OctoshockPlaystationInputKeys.Up] = true},
						new Dictionary<string, object> {[InputKeys.Forward] = true, [OctoshockPlaystationInputKeys.Up] = false}
					},
					Activator = new Dictionary<string, object> { [OctoshockPlaystationInputKeys.L2] = true }
				};
			}
		}

		public bool ReadDash
		{
			set
			{
				inputs[InputKeys.Dash].Enabled = value;
			}
		}
		public bool ReadDragonPunch
		{
			set
			{
				inputs[InputKeys.DragonPunch].Enabled = value;
			}
		}
		public bool ReadQuarterCircle
		{
			set
			{
				inputs[InputKeys.QuarterCircleForward].Enabled = value;
			}
		}
		public bool ReadHalfCircle
		{
			set
			{
				inputs[InputKeys.HalfCircleForward].Enabled = value;
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
					diractionalHistory.Clear();
					moveHistory.Clear();
				}
				return;
			}
			else
			{
				cleared = false;
			}

			inputHistory.Add(joypadApi.Get());
			diractionalHistory.Add(new Dictionary<string, object>());
			moveHistory.Add(new Dictionary<string, bool>());

			if (inputHistory.Count > 120)
			{
				inputHistory.RemoveAt(0);
				diractionalHistory.RemoveAt(0);
				moveHistory.RemoveAt(0);
			}

			bool leftPressed = Convert.ToBoolean(inputHistory[inputHistory.Count - 1][leftKey]);
			bool rightPressed = Convert.ToBoolean(inputHistory[inputHistory.Count - 1][rightKey]);


			if (sotnApi.AlucardApi.FacingLeft)
			{
				diractionalHistory[diractionalHistory.Count - 1].Add(InputKeys.Forward, leftPressed);
				diractionalHistory[diractionalHistory.Count - 1].Add(InputKeys.Back, rightPressed);
			}
			else
			{
				diractionalHistory[diractionalHistory.Count - 1].Add(InputKeys.Forward, rightPressed);
				diractionalHistory[diractionalHistory.Count - 1].Add(InputKeys.Back, leftPressed);
			}

			diractionalHistory[diractionalHistory.Count - 1].Add(upKey, Convert.ToBoolean(inputHistory[inputHistory.Count - 1][upKey]));
			diractionalHistory[diractionalHistory.Count - 1].Add(downKey, Convert.ToBoolean(inputHistory[inputHistory.Count - 1][downKey]));
			diractionalHistory[diractionalHistory.Count - 1].Add(leftKey, leftPressed);
			diractionalHistory[diractionalHistory.Count - 1].Add(rightKey, rightPressed);

			foreach (var input in inputs)
			{
				if (input.Value.Enabled && ReadInput(input.Value, Globals.InputBufferSize))
				{
					moveHistory[moveHistory.Count - 1].Add(input.Key, true);
				}
				else
				{
					moveHistory[moveHistory.Count - 1].Add(input.Key, false);
				}
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

		public bool DirectionHeld(string button)
		{
			if (inputHistory.Count < 1)
			{
				return false;
			}
			if (Convert.ToBoolean(diractionalHistory[diractionalHistory.Count - 1][button]) == true)
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
						if (Convert.ToBoolean(diractionalHistory[i][pair.Key]) != Convert.ToBoolean(pair.Value))
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

		private bool CheckNymashock()
		{
			return joypadApi.Get().ContainsKey(PlaystationInputKeys.Circle);
		}
	}
}
