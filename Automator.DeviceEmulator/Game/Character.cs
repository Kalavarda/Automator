using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Automator.DeviceEmulator.Properties;
using Color = System.Windows.Media.Color;

namespace Automator.DeviceEmulator.Game
{
	public interface ICharacter
	{
		/// <summary>
		/// Персонаж в бою
		/// </summary>
		bool? InBattle { get; }
	}

	public class Character : ICharacter
	{
		private readonly Process _process;

		public ICollection<ColorPoint> InBattleConditions { get; private set; }

		public bool? InBattle
		{
			get
			{
				if (!InBattleConditions.Any())
					return null;

				return InBattleConditions.All(cond =>
				{
					var color = OtherProcess.GetColor(_process, cond.Point.X, cond.Point.Y);
					return AreEquals(cond.Color, ColorPicker.Convert(color));
				});
			}
		}

		public Character(Process process)
		{
			_process = process;
			InBattleConditions = new List<ColorPoint>();
		}

		private static bool AreEquals(Color c1, Color c2)
		{
			if (Math.Abs(c2.R - c1.R) > Settings.Default.ColorTolerance)
				return false;

			if (Math.Abs(c2.G - c1.G) > Settings.Default.ColorTolerance)
				return false;

			if (Math.Abs(c2.B - c1.B) > Settings.Default.ColorTolerance)
				return false;

			return true;
		}
	}

	public class ColorPoint
	{
		public Point Point { get; set; }

		public Color Color { get; set; }

		public override string ToString()
		{
			return string.Join(";",
				"X=" + Point.X,
				"Y=" + Point.Y,
				"R=" + Color.R,
				"G=" + Color.G,
				"B=" + Color.B);
		}

		public static ColorPoint Parse(string value)
		{
			var parts = value.Split(';');

			var x = int.Parse(parts[0].Split('=')[1]);
			var y = int.Parse(parts[1].Split('=')[1]);
			var r = byte.Parse(parts[2].Split('=')[1]);
			var g = byte.Parse(parts[3].Split('=')[1]);
			var b = byte.Parse(parts[4].Split('=')[1]);

			return new ColorPoint
			{
				Point = new Point(x, y),
				Color = Color.FromRgb(r, g, b)
			};
		}

		public static IEnumerable<ColorPoint> ParseCollection(string value)
		{
			var parts = value.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			return parts.Select(Parse);
		}
	}

	public class KeysBlock
	{
		/// <summary>
		/// Персонаж в бою
		/// </summary>
		public const string Battle = "B";

		/// <summary>
		/// Персонаж не в бою
		/// </summary>
		public const string NoBattle = "!B";

		public const char Separator = ';';
		public const char CondSeparator = ' ';

		public TimeSpan Interval { get; set; }

		public IList<string> Keys { get; private set; }

		public KeysBlock()
		{
			Interval = TimeSpan.FromSeconds(0.5);
			Keys = new List<string>();
		}

		public override string ToString()
		{
			return string.Join(Separator.ToString(), new[] { Interval.ToString() }.Union(Keys));
		}

		public static KeysBlock Parse(string value)
		{
			var parts = value.Split(Separator);
			return new KeysBlock
			{
				Interval = TimeSpan.Parse(parts[0]),
				Keys = new List<string>(parts.Skip(1))
			};
		}

		public static IEnumerable<KeysBlock> ParseCollection(string value)
		{
			return !string.IsNullOrWhiteSpace(value)
				? value.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(Parse).ToList()
				: Enumerable.Empty<KeysBlock>();
		}

		public static char GetKeyChar(string value)
		{
			return value.Split(CondSeparator)[0][0];
		}

		public static IEnumerable<string> GetKeyConditions(string value)
		{
			return value.Split(CondSeparator).Skip(1);
		}
	}
}
