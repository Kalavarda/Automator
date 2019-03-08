using System;

namespace Automator.DeviceEmulator
{
	public class EmulationStatistics
	{
		private DateTime? _emulationStart;

		public DateTime? EmulationStart
		{
			get { return _emulationStart; }
			set
			{
				_emulationStart = value;

				if (EmulationStartChanged != null)
					EmulationStartChanged(this);
			}
		}

		public event Action<EmulationStatistics> EmulationStartChanged;
	}
}
