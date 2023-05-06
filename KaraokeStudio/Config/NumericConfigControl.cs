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
	public partial class NumericConfigControl : BaseConfigControl
	{
		public NumericConfigControl()
		{
			InitializeComponent();
		}

		internal override void UpdateValue(object config)
		{
			ConfigureRange();

			var val = Field?.GetValue<decimal>(config);
			if(val != null)
			{
				numericUpDown.Value = (decimal)val;
			}
		}

		internal override void SetValue(object config)
		{
			Field?.SetValue(config, numericUpDown.Value);
		}

		private void ConfigureRange()
		{
			var configRange = Field?.ConfigRange;
			if(configRange == null)
			{
				numericUpDown.Minimum = decimal.MinValue;
				numericUpDown.Maximum = decimal.MaxValue;
			}
			else
			{
				numericUpDown.Minimum = (decimal)configRange.Minimum;
				numericUpDown.Maximum = configRange.HasMax ? (decimal)configRange.Maximum : decimal.MaxValue;
			}

			numericUpDown.DecimalPlaces = (Field?.IsDecimal ?? true) ? 3 : 0;
			numericUpDown.Increment = (Field?.IsDecimal ?? true) ? 0.25M : 1M;
		}

		private void numericUpDown_ValueChanged(object sender, EventArgs e)
		{
			SendValueChanged();
		}
	}
}
