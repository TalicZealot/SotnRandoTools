using System.Collections.Generic;

namespace SotnRandoTools.Constants
{
	public static class PlaystationInputKeys
	{
		public const string Up = "P1 D-Pad Up";
		public const string Down = "P1 D-Pad Down";
		public const string Left = "P1 D-Pad Left";
		public const string Right = "P1 D-Pad Right";
		public const string Circle = "P1 ○";
		public const string Square = "P1 □";
		public const string Triangle = "P1 △";
		public const string Cross = "P1 X";
		public const string L1 = "P1 L1";
		public const string L2 = "P1 L2";
		public const string L3 = "P1 Left Stick, Button";
		public const string R1 = "P1 R1";
		public const string R2 = "P1 R2";
		public const string R3 = "P1 Right Stick, Button";
		public const string Select = "P1 Select";
		public const string Start = "P1 Start";

		public static Dictionary<string, string> OctoshockKeys = new Dictionary<string, string>
		{
			{ "P1 D-Pad Up", "P1 Up" },
			{ "P1 D-Pad Down", "P1 Down" },
			{ "P1 D-Pad Left", "P1 Left" },
			{ "P1 D-Pad Right", "P1 Right" },
			{ "P1 ○", "P1 Circle" },
			{ "P1 □", "P1 Square" },
			{ "P1 △", "P1 Triangle" },
			{ "P1 X", "P1 Cross" },
			{ "P1 L1", "P1 L1" },
			{ "P1 L2", "P1 L2" },
			{ "P1 Left Stick, Button", "P1 L3" },
			{ "P1 R1", "P1 R1" },
			{ "P1 R2", "P1 R2" },
			{ "P1 Right Stick, Button", "P1 R3" },
			{ "P1 Select", "P1 Select" },
			{ "P1 Start", "P1 Start" }
		};
	}
}
