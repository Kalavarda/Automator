using System.Windows;

namespace Automator.DeviceEmulator
{
	public partial class KeyWindow
	{
		public string Key { get; set; }

		public KeyWindow()
		{
			InitializeComponent();
		}

		public KeyWindow(string key): this()
		{
			Key = key;
			textBox.Text = key;
		}

		private void OnOkClick(object sender, RoutedEventArgs e)
		{
			Key = textBox.Text;
			DialogResult = true;
		}
	}
}
