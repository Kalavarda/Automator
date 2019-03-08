using System;
using System.Linq;
using System.Windows;
using Automator.DeviceEmulator.Game;

namespace Automator.DeviceEmulator
{
	public partial class KeysBlockWindow
	{
		public KeysBlock KeysBlock { get; private set; }

		public KeysBlockWindow()
		{
			InitializeComponent();
		}

		public KeysBlockWindow(KeysBlock keysBlock): this()
		{
			KeysBlock = keysBlock;

			intervalBox.Text = keysBlock.Interval.ToString();

			TuneControls();
		}

		private void OnOkClick(object sender, RoutedEventArgs e)
		{
			try
			{
				KeysBlock.Interval = TimeSpan.Parse(intervalBox.Text);
				DialogResult = true;
			}
			catch (Exception error)
			{
				App.ShowError(error);
			}
		}

		private void OnAddClick(object sender, RoutedEventArgs e)
		{
			var window = new KeyWindow(string.Empty) { Owner = this };
			if (window.ShowDialog() == true)
				KeysBlock.Keys.Add(window.Key);
			TuneControls();
		}

		private void TuneControls()
		{
			listBox.ItemsSource = KeysBlock.Keys.OrderBy(s => s);
		}

		private void OnRemoveClick(object sender, RoutedEventArgs e)
		{
			throw new NotImplementedException();
		}
	}
}
