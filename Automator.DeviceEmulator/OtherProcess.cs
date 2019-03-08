using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Automator.DeviceEmulator.Properties;

namespace Automator.DeviceEmulator
{
	public static class OtherProcess
	{
		private const uint WM_CHAR = 0x0102;
		private const uint WM_KEYDOWN = 0x0100;
		private const uint WM_KEYUP = 0x0101;

		public static EmulationStatistics Statistics
		{
			private get; set;
		}

		public static void SendKey(Process process, Keys key)
		{
			if (Settings.Default.SendKey_KeyDownUp)
			{
				NativeMethods.PostMessage(process.MainWindowHandle, WM_KEYDOWN, (int)key, 0);
				Thread.Sleep(Settings.Default.SendKey_Delay);
			}

			if (Settings.Default.SendKey_Char)
			{
				NativeMethods.PostMessage(process.MainWindowHandle, WM_CHAR, (int)key, 0);
				Thread.Sleep(Settings.Default.SendKey_Delay);
			}

			if (Settings.Default.SendKey_KeyDownUp)
			{
				NativeMethods.PostMessage(process.MainWindowHandle, WM_KEYUP, (int)key, 0);
				Thread.Sleep(Settings.Default.SendKey_Delay);
			}
		}

		public static Color GetColor(Process process, int x, int y)
		{
			var dc = NativeMethods.GetDC(process.MainWindowHandle);
			var pixel = NativeMethods.GetPixel(dc, x, y);
			NativeMethods.ReleaseDC(process.MainWindowHandle, dc);

			return Color.FromArgb(
				(int)(pixel & 0x000000FF),
				(int)(pixel & 0x0000FF00) >> 8,
				(int)(pixel & 0x00FF0000) >> 16);
		}
	}

	internal static class NativeMethods
	{
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr PostMessage(IntPtr hwnd, UInt32 msg, Int32 wparam, Int32 lparam);
	
		[DllImport("user32.dll")]
		public static extern IntPtr GetDC(IntPtr hwnd);

		[DllImport("user32.dll")]
		public static extern Int32 ReleaseDC(IntPtr hwnd, IntPtr hdc);

		[DllImport("gdi32.dll")]
		public static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);
	}
}
