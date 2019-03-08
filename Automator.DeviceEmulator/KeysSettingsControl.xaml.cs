using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Automator.DeviceEmulator.Game;
using Automator.DeviceEmulator.Properties;

namespace Automator.DeviceEmulator
{
	public partial class KeysSettingsControl
	{
		private readonly ICollection<KeysBlock> _keysBlocks = new Collection<KeysBlock>();

		public KeysBlock SelectedKeysBlock
		{
			get
			{
				return listBox.SelectedItem as KeysBlock;
			}
		}

		public KeysSettingsControl()
		{
			InitializeComponent();

			foreach (var block in KeysBlock.ParseCollection(Settings.Default.KeysBlocks))
				_keysBlocks.Add(block);

			TuneControls();
		}

		private void TuneControls()
		{
			var old = SelectedKeysBlock;

			listBox.ItemsSource = _keysBlocks.OrderBy(kb => kb.Interval);
			if (old != null && _keysBlocks.Contains(old))
				listBox.SelectedItem = old;

			removeBtn.IsEnabled = SelectedKeysBlock != null;
		}

		private void OnAddClick(object sender, RoutedEventArgs e)
		{
			var block = new KeysBlock();
			if (new KeysBlockWindow(block) {Owner = GetWindow()}.ShowDialog() == true)
			{
				_keysBlocks.Add(block);
				TuneControls();
			}
		}

		private Window GetWindow()
		{
			var w = (FrameworkElement)this;
			while (!(w is Window))
				w = (FrameworkElement)w.Parent;
			return w as Window;
		}

		public void Save()
		{
			Settings.Default.KeysBlocks = string.Join(Environment.NewLine, _keysBlocks.Select(kb => kb.ToString()));
		}

		private void OnRemoveClick(object sender, RoutedEventArgs e)
		{
			_keysBlocks.Remove(SelectedKeysBlock);
			TuneControls();
		}

		private void OnBlockMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.Handled)
				return;

			var block = ((FrameworkElement) sender).DataContext as KeysBlock;
			if (block == null)
				return;

			if (e.ClickCount == 2)
				if (new KeysBlockWindow(block) { Owner = GetWindow() }.ShowDialog() == true)
					TuneControls();
		}

		private void ListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			TuneControls();
		}
	}
}
