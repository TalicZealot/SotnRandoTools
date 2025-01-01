using System;

namespace SotnRandoTools.RandoTracker.Models
{
	internal unsafe struct LocationRoom
	{
		public ushort roomIndex;
		public ushort stateIndex;
		public byte values;

		public bool Checked(byte value)
		{
			return (value & values) > 0;
		}
	}

	internal struct LocationLock
	{
		public ulong flags;
		public ushort stateIndex;
	}

	internal struct LocationState
	{
		private static byte VisitedFlag = 0b00000001;
		private static byte SecondCastleFlag = 0b00000010;
		private static byte SmallIndicatorFlag = 0b00000100;
		private static byte ReachabilityChangedFlag = 0b_00001000;

		public LocationState()
		{
			availabilityColor = MapColor.Unavailable;
			flags = 0;
		}

		public bool Visited
		{
			get
			{
				return (flags & VisitedFlag) != 0;
			}
			set
			{
				if (value)
				{
					flags |= VisitedFlag;
				}
				else
				{
					flags &= (byte) ~(VisitedFlag);
				}
			}
		}

		public bool SecondCastle
		{
			get
			{
				return (flags & SecondCastleFlag) != 0;
			}
			set
			{
				if (value)
				{
					flags |= SecondCastleFlag;
				}
				else
				{
					flags &= (byte) ~(SecondCastleFlag);
				}
			}
		}

		public bool SmallIndicator
		{
			get
			{
				return (flags & SmallIndicatorFlag) != 0;
			}
			set
			{
				if (value)
				{
					flags |= SmallIndicatorFlag;
				}
				else
				{
					flags &= (byte) ~(SmallIndicatorFlag);
				}
			}
		}

		public bool ReachabilityChanged
		{
			get
			{
				return (flags & ReachabilityChangedFlag) != 0;
			}
			set
			{
				if (value)
				{
					flags |= ReachabilityChangedFlag;
				}
				else
				{
					flags &= (byte) ~(ReachabilityChangedFlag);
				}
			}
		}

		public byte flags = 0;
		public MapColor availabilityColor;
		public byte x;
		public byte y;
	}

	internal sealed class Locations
	{
		private int stateCapacity = 200;
		public int stateCount = 0;
		private int roomCapacity = 300;
		public int roomCount = 0;
		private int lockCapacity = 1000;
		public int lockCount = 0;
		private int allowedLockCapacity = 1000;
		public int allowedLockCount = 0;

		public LocationState[] states;
		public LocationRoom[] rooms;
		public LocationLock[] locks;
		public LocationLock[] allowedLocks;
		public bool[] coopStates;
		public bool initialized = false;
		public Locations()
		{
			states = new LocationState[stateCapacity];
			rooms = new LocationRoom[roomCapacity];
			locks = new LocationLock[lockCapacity];
			allowedLocks = new LocationLock[allowedLockCapacity];
			coopStates = new bool[stateCapacity];
		}
		public int AddState(LocationState state)
		{
			if (!initialized)
			{
				initialized = true;
			}
			stateCount++;
			EnsureStateCapacity();
			states[stateCount - 1] = state;

			return stateCount - 1;
		}

		public int AddRoom(LocationRoom room)
		{
			roomCount++;
			EnsureRoomCapacity();
			rooms[roomCount - 1] = room;

			return roomCount - 1;
		}

		public int AddLock(LocationLock llock)
		{
			lockCount++;
			EnsureLockCapacity();
			locks[lockCount - 1] = llock;

			return lockCount - 1;
		}

		public int AddAllowedLock(LocationLock llock)
		{
			allowedLockCount++;
			EnsureAllowedLockCapacity();
			allowedLocks[allowedLockCount - 1] = llock;

			return allowedLockCount - 1;
		}

		public void Reset()
		{
			for (int i = 0; i < stateCount; i++)
			{
				states[i].Visited = false;
			}
		}

		private void EnsureStateCapacity()
		{
			if (stateCount == stateCapacity)
			{
				stateCapacity *= 2;
				LocationState[] newStates = new LocationState[stateCapacity];
				Array.Copy(states, newStates, states.Length);
				states = newStates;

				bool[] newCoopStates = new bool[stateCapacity];
				Array.Copy(coopStates, newCoopStates, coopStates.Length);
				coopStates = newCoopStates;
			}
		}

		private void EnsureRoomCapacity()
		{
			if (roomCount == roomCapacity)
			{
				roomCapacity *= 2;
				LocationRoom[] newRooms = new LocationRoom[roomCapacity];
				Array.Copy(rooms, newRooms, rooms.Length);
				rooms = newRooms;
			}
		}

		private void EnsureLockCapacity()
		{
			if (lockCount == lockCapacity)
			{
				lockCapacity *= 2;
				LocationLock[] newLocks = new LocationLock[lockCapacity];
				Array.Copy(locks, newLocks, locks.Length);
				locks = newLocks;
			}
		}

		private void EnsureAllowedLockCapacity()
		{
			if (allowedLockCount == allowedLockCapacity)
			{
				allowedLockCapacity *= 2;
				LocationLock[] newLocks = new LocationLock[allowedLockCapacity];
				Array.Copy(allowedLocks, newLocks, allowedLocks.Length);
				allowedLocks = newLocks;
			}
		}
	}
}
