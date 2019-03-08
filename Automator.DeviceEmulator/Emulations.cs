using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Automator.DeviceEmulator.Game;

namespace Automator.DeviceEmulator
{
	public abstract class EmulationBase : IEmulation
	{
		public DateTime? LastEvent { get; private set; }

		public abstract Range<TimeSpan> Interval { get; }

		public bool IsWorking { get; private set; }

		protected abstract void Work(Process process, ICharacter character);

		public void Emulate(Process process, ICharacter character)
		{
			try
			{
				IsWorking = true;
				Work(process, character);
			}
			finally
			{
				IsWorking = false;
				LastEvent = DateTime.Now;
			}
		}
	}

	public class OneKeyEmulation : EmulationBase
	{
		private readonly KeysBlock _keysBlock;
		private readonly TimeSpanRange _timeSpanRange;
		private int _keyIndex;

		public OneKeyEmulation(KeysBlock keysBlock)
		{
			_keysBlock = keysBlock;
			_timeSpanRange = new TimeSpanRange
			{
				Min = _keysBlock.Interval,
				Max = _keysBlock.Interval
			};
		}

		public override Range<TimeSpan> Interval
		{
			get
			{
				return _timeSpanRange;
			}
		}

		protected override void Work(Process process, ICharacter character)
		{
			var key = _keysBlock.Keys[_keyIndex];

			var conditions = KeysBlock.GetKeyConditions(key);
			if (conditions.Any(condition => !CheckConditions(condition, character)))
			{
				_keyIndex++;

				if (_keyIndex == _keysBlock.Keys.Count)
					_keyIndex = 0;

				return;
			}

			Press(process, KeysBlock.GetKeyChar(key));
			_keyIndex++;

			if (_keyIndex == _keysBlock.Keys.Count)
				_keyIndex = 0;
		}

		private static void Press(Process process, char ch)
		{
			Keys key;
			switch (ch)
			{
				case '1':
					key = Keys.D1;
					break;
				case '2':
					key = Keys.D2;
					break;
				case '3':
					key = Keys.D3;
					break;
				case '4':
					key = Keys.D4;
					break;
				case '5':
					key = Keys.D5;
					break;
				case '6':
					key = Keys.D6;
					break;
				case '7':
					key = Keys.D7;
					break;
				case '8':
					key = Keys.D8;
					break;
				case '9':
					key = Keys.D9;
					break;
				case '0':
					key = Keys.D0;
					break;
				case 'F':
					key = Keys.F;
					break;
				case ' ':
					key = Keys.Space;
					break;
				default:
					throw new NotImplementedException();
			}

			OtherProcess.SendKey(process, key);
		}

		private static bool CheckConditions(string condition, ICharacter character)
		{
			if (condition == KeysBlock.Battle)
				if (character.InBattle != true)
					return false;

			if (condition == KeysBlock.NoBattle)
				if (character.InBattle != false)
					return false;

			return true;
		}
	}
}
