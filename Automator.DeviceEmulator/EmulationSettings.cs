using System;
using System.Collections.Generic;
using System.Diagnostics;
using Automator.DeviceEmulator.Game;

namespace Automator.DeviceEmulator
{
	public class EmulationSettings
	{
		public IEnumerable<IEmulation> Emulations { get; set; }

		public Process Process { get; set; }

		public Character Character { get; set; }
	}

	public interface IEmulation
	{
		DateTime? LastEvent { get; }

		Range<TimeSpan> Interval { get; }

		bool IsWorking { get; }

		void Emulate(Process process, ICharacter character);
	}

	public abstract class Range<T>
	{
		protected static readonly Random Rand = new Random();

		public T Min { get; set; }

		public T Max { get; set; }

		public abstract T GetRandomValue();
	}

	public class TimeSpanRange : Range<TimeSpan>
	{
		public override TimeSpan GetRandomValue()
		{
			return TimeSpan.FromMilliseconds(Min.TotalMilliseconds + (Max.TotalMilliseconds - Min.TotalMilliseconds) * Rand.NextDouble());
		}
	}
}
