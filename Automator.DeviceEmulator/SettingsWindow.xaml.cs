using System;
using System.Linq;
using System.Windows;
using Automator.DeviceEmulator.Game;
using Automator.DeviceEmulator.Properties;

namespace Automator.DeviceEmulator
{
	public partial class SettingsWindow
	{
		public SettingsWindow()
		{
			InitializeComponent();

			_textBox.Text = Settings.Default.ProcessName;
			_colorTolerBox.Text = Settings.Default.ColorTolerance.ToString();

			if (!string.IsNullOrWhiteSpace(Settings.Default.BattleConditions))
			{
				var parts = ColorPoint.ParseCollection(Settings.Default.BattleConditions).ToList();
				_tb1.Text = parts[0].ToString();
				_tb2.Text = parts[1].ToString();
			}
		}

		private void OnSaveClick(object sender, RoutedEventArgs e)
		{
			try
			{
				Settings.Default.ProcessName = _textBox.Text;
				Settings.Default.ColorTolerance = byte.Parse(_colorTolerBox.Text);

				Settings.Default.BattleConditions = string.Join(Environment.NewLine, _tb1.Text, _tb2.Text);

				keysSettingsControl.Save();

				Settings.Default.Save();

				DialogResult = true;
			}
			catch (Exception)
			{
				throw new NotImplementedException();
			}
		}

		private void ColorPicker1_OnPicked(ColorPoint colorPoint)
		{
			_tb1.Text = colorPoint.ToString();
		}

		private void ColorPicker2_OnPicked(ColorPoint colorPoint)
		{
			_tb2.Text = colorPoint.ToString();
		}
	}
}
