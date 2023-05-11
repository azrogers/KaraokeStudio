using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KaraokeStudio.Config
{
	public partial class EnumConfigControl : BaseConfigControl
	{
		private string[] _translatedEnumNames;
		private string[] _originalEnumNames;

		public EnumConfigControl()
		{
			InitializeComponent();

			_originalEnumNames = new string[0];
			_translatedEnumNames = new string[0];
		}

		internal override void UpdateValue(object config) 
		{
			SetOptions();

			if(Field == null)
			{
				return;
			}

			var val = Field.GetValue(Field.FieldType, config);
			if (val == null)
			{
				return;
			}

			var name = Enum.GetName(Field.FieldType, val);
			if(name == null)
			{
				return;
			}

			comboBox.SelectedIndex = Array.IndexOf(_originalEnumNames, name);
		}

		internal override void SetValue(object config) 
		{
			if(Field != null && _originalEnumNames.Any())
			{
				var realName = _originalEnumNames[comboBox.SelectedIndex];
				Field.SetValue(config, Enum.Parse(Field.FieldType, realName));
			}
		}

		private void SetOptions()
		{
			var item = comboBox.SelectedText;
			comboBox.Items.Clear();
			if(Field == null)
			{
				return;
			}

			var names = Enum.GetNames(Field.FieldType);
			_translatedEnumNames = new string[names.Length];
			_originalEnumNames = new string[names.Length];

			for(var i = 0; i < names.Length; i++)
			{
				_originalEnumNames[i] = names[i];
				_translatedEnumNames[i] = Util.HumanizeCamelCase(names[i]);
			}

			comboBox.Items.AddRange(_translatedEnumNames);
			if(item != null)
			{
				comboBox.SelectedText = item;
			}
			else
			{
				comboBox.SelectedIndex = 0;
			}
		}

		private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			SendValueChanged();
		}
	}
}
