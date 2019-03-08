using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Input;
using System.Windows.Media;
using Automator.DeviceEmulator.Game;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;

namespace Automator.DeviceEmulator
{
	public partial class ColorPicker
	{
		public Process Process
		{
			get { return MainWindow.Instance.SelectedProcess; }
		}

		private bool _picking;

		public ColorPicker()
		{
			InitializeComponent();
		}

		void Picking_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (!_picking)
				return;

			var position = PointToScreen(e.GetPosition(this));
			var p = new Point((int)position.X, (int)position.Y);
			var color = OtherProcess.GetColor(Process, p.X, p.Y);

			MouseLeftButtonUp -= Picking_MouseLeftButtonUp;
			MouseMove -= MainWindow_MouseMove;
			ReleaseMouseCapture();

			if (Picked != null)
				Picked(new ColorPoint { Point = p, Color = Convert(color) });

			_textBlock.Text = "Pick";
			_textBlock.Foreground = Brushes.Black;
			_rect.Fill = Brushes.Transparent;

			_picking = false;
		}

		private void OnPickDown(object sender, MouseButtonEventArgs e)
		{
			if (CaptureMouse())
			{
				MouseLeftButtonUp += Picking_MouseLeftButtonUp;
				MouseMove += MainWindow_MouseMove;
				_picking = true;
			}
		}

		void MainWindow_MouseMove(object sender, MouseEventArgs e)
		{
			if (_picking)
			{
				var position = PointToScreen(e.GetPosition(this));
				var color = OtherProcess.GetColor(Process, (int)position.X, (int)position.Y);
				_textBlock.Text = (int)position.X + "; " + (int)position.Y;
				_rect.Fill = new SolidColorBrush(Color.FromArgb(255, color.R, color.G, color.B));
				_textBlock.Foreground = new SolidColorBrush(Color.FromArgb(255, (byte)(255 - color.R), (byte)(255 - color.G), (byte)(255 - color.B)));
			}
		}

		public event Action<ColorPoint> Picked;

		public static Color Convert(System.Drawing.Color value)
		{
			return Color.FromArgb(255, value.R, value.G, value.B);
		}
	}
}
