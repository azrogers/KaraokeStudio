namespace KaraokeStudio.Config.Controls
{
	public partial class RangeConfigControl : BaseConfigControl
	{
		public RangeConfigControl()
		{
			InitializeComponent();
			trackBar.TickFrequency = 10;
			trackBar.Maximum = 100;
			trackBar.Minimum = 0;
		}

		internal override void UpdateValue(object config)
		{
			var val = Field?.GetValue<double>(config);
			if (val != null)
			{
				var valNotNull = val ?? 0;
				var min = Field?.ConfigRange?.Minimum ?? 0.0;
				var max = Field?.ConfigRange?.Maximum ?? 1.0;
				var normalizedValue = Math.Clamp((valNotNull - min) / (max - min), 0.0, 1.0);

				// ignore events when setting value
				trackBar.ValueChanged -= trackBar_ValueChanged;
				trackBar.Value = (int)(normalizedValue * trackBar.Maximum);
				trackBar.ValueChanged += trackBar_ValueChanged;
				valueLabel.Text = CalculateValue().ToString();
			}
		}

		internal override void SetValue(object config)
		{
			Field?.SetValue(config, CalculateValue());
		}

		private double CalculateValue()
		{
			var min = Field?.ConfigRange?.Minimum ?? 0.0;
			var max = Field?.ConfigRange?.Maximum ?? 1.0;
			return min + (max - min) * ((double)trackBar.Value / trackBar.Maximum);
		}

		private void trackBar_ValueChanged(object? sender, EventArgs e)
		{
			valueLabel.Text = CalculateValue().ToString();
			SendValueChanged();
		}
	}
}
