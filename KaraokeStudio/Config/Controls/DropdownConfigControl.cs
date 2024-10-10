using KaraokeLib.Config;
using KaraokeStudio.Util;
using Microsoft.VisualBasic;
using System.Linq;

namespace KaraokeStudio.Config.Controls
{
	public partial class DropdownConfigControl : BaseConfigControl
	{
		public DropdownConfigControl()
		{
			InitializeComponent();
		}

		internal override void UpdateValue(object config)
		{
			SetOptions(config.GetType());

			if (Field == null || Field.ConfigDropdown == null || comboBox.Items.Count == 0)
			{
				return;
			}

			var val = Field.GetValue(Field.FieldType, config);
			if (val == null)
			{
				return;
			}

			var values = Field.ConfigDropdown.GetNameValuePairs(config.GetType()).Select(pair => pair.Item2).ToArray();

			comboBox.SelectedIndexChanged -= comboBox_SelectedIndexChanged;
			comboBox.SelectedIndex = Array.IndexOf(values, val);
			comboBox.SelectedIndexChanged += comboBox_SelectedIndexChanged;
		}

		internal override void SetValue(object config)
		{
			var item = comboBox.SelectedItem as DropdownItem;
			if (Field != null && item != null)
			{
				Field.SetValue(config, item.Value);
			}
		}

		private void SetOptions(Type configType)
		{
			var item = comboBox.SelectedItem as DropdownItem;
			comboBox.Items.Clear();
			if (Field == null || Field.ConfigDropdown == null)
			{
				return;
			}

			var selectedIndex = 0;

			foreach (var (name, value) in Field.ConfigDropdown.GetNameValuePairs(configType))
			{
				comboBox.Items.Add(new DropdownItem(name, value));
				if(item != null && value == item.Value)
				{
					selectedIndex = comboBox.Items.Count - 1;
				}
			}

			if(comboBox.Items.Count > 0)
			{
				comboBox.SelectedIndex = selectedIndex;
			}
		}

		private void comboBox_SelectedIndexChanged(object? sender, EventArgs e)
		{
			SendValueChanged();
		}

		private class DropdownItem
		{
			private string _name;
			private object _value;

			public object Value => _value;

			public DropdownItem(string name, object value)
			{
				_name = name;
				_value = value;
			}

			public override string ToString()
			{
				return _name;
			}
		}
	}
}
