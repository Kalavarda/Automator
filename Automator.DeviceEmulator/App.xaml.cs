using System;
using System.Windows;

namespace Automator.DeviceEmulator
{
	public partial class App
	{
		public static void ShowError(Exception error)
		{
			if (Current.Dispatcher.CheckAccess())
				MessageBox.Show(error.Message + Environment.NewLine + Environment.NewLine + error);
			else
				Current.Dispatcher.Invoke(() => { ShowError(error); });
		}
	}
}
