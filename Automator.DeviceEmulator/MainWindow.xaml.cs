using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using Automator.DeviceEmulator.Game;
using Automator.DeviceEmulator.Properties;

namespace Automator.DeviceEmulator
{
	public partial class MainWindow: IDisposable
	{
		internal static MainWindow Instance { get; private set; }

		private Timer _timer;
		private readonly EmulationStatistics _statistics = new EmulationStatistics();
		private readonly EmulationSettings _emulationSettings = new EmulationSettings();

		public MainWindow()
		{
			InitializeComponent();

			OtherProcess.Statistics = _statistics;

			RefreshEmulations();

			Activated += MainWindow_Activated;
			RefreshProcesses();

			Instance = this;
		}

		private void RefreshEmulations()
		{
			var es = new Collection<EmulationBase>();
			foreach (var keysBlock in KeysBlock.ParseCollection(Settings.Default.KeysBlocks))
				es.Add(new OneKeyEmulation(keysBlock));
			_emulationSettings.Emulations = es.ToArray();
		}

		void MainWindow_Activated(object sender, EventArgs e)
		{
			RefreshProcesses();
		}

		private IEnumerable<Process> GetCurrentProcesses()
		{
			if (Dispatcher.CheckAccess())
			{
				var processes = Process.GetProcesses()
					.Where(p => p.ProcessName.Contains(Settings.Default.ProcessName));
				if (!Settings.Default.FilterBySessionId)
					return processes;

				var sessionId = Process.GetCurrentProcess().SessionId;
				return processes.Where(p => p.SessionId == sessionId);
			}

			return Dispatcher.Invoke(() => GetCurrentProcesses());
		}

		private void RefreshProcesses()
		{
			if (Dispatcher.CheckAccess())
			{
				var selectedItem = processSelector.SelectedItem as Process;
				var processes = GetCurrentProcesses().OrderBy(p => p.StartTime).ToList();
				processSelector.ItemsSource = processes;

				if (selectedItem != null)
					processSelector.SelectedItem = processes.FirstOrDefault(p => p.Id == selectedItem.Id);

				if (processSelector.SelectedItem == null && processes.Count == 1)
					processSelector.SelectedItem = processes.First();
			}
			else
				Dispatcher.Invoke(RefreshProcesses);
		}

		private void OnStartClick(object sender, RoutedEventArgs e)
		{
			if (_timer == null)
				Start();
			else
				Stop();
		}

		private void Start()
		{
			if (SelectedProcess == null)
			{
				MessageBox.Show("Необходимо выбрать процесс.");
				return;
			}

			_emulationSettings.Process = SelectedProcess;

			var character = new Character(SelectedProcess);
			foreach (var colorPointCondition in ColorPoint.ParseCollection(Settings.Default.BattleConditions))
				character.InBattleConditions.Add(colorPointCondition);
			_emulationSettings.Character = character;

			_statistics.EmulationStart = DateTime.Now;

			_timer = new Timer(Work, _emulationSettings, TimeSpan.FromSeconds(1), Settings.Default.TimerInterval);

			TuneControls();
		}

		private void Stop()
		{
			if (_timer != null)
			{
				_timer.Dispose();
				_timer = null;
			}

			RefreshProcesses();

			TuneControls();
		}

		private void Work(object state)
		{
			try
			{
				var settings = (EmulationSettings) state;

				if (GetCurrentProcesses().All(p => p.Id != settings.Process.Id))
				{
					Stop();
					return;
				}

				foreach (var emulation in settings.Emulations.Where(e => !e.IsWorking))
				{
					if (emulation.LastEvent == null)
					{
						emulation.Emulate(settings.Process, settings.Character);
						continue;
					}

					if (DateTime.Now - emulation.LastEvent.Value > emulation.Interval.GetRandomValue())
						emulation.Emulate(settings.Process, settings.Character);
				}
			}
			catch (Exception error)
			{
				Stop();
				App.ShowError(error);
			}
		}

		internal Process SelectedProcess
		{
			get
			{
				return processSelector.SelectedItem as Process;
			}
		}

		private void TuneControls()
		{
			if (Dispatcher.CheckAccess())
			{
				startStopButton.Content = _timer != null ? "Стоп" : "Старт";
				_settingsBtn.IsEnabled = _timer == null;
			}
			else
				Dispatcher.Invoke(TuneControls);
		}

		public void Dispose()
		{
			if (_timer != null)
			{
				_timer.Dispose();
				_timer = null;
			}
		}

		private void OnSettingsClick(object sender, RoutedEventArgs e)
		{
			if (new SettingsWindow {Owner = this}.ShowDialog() == true)
			{
				RefreshProcesses();
				RefreshEmulations();
			}
		}
	}

	public class ProcessConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var process = value as Process;
			return process == null
				? null
				: string.Format("StartTime: {0}   (Id: {1})", process.StartTime.ToShortTimeString(), process.Id);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
