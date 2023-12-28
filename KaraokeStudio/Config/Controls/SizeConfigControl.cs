using KaraokeLib.Util;
using Newtonsoft.Json.Linq;

namespace KaraokeStudio.Config.Controls
{
	public partial class SizeConfigControl : BaseConfigControl
	{
		public int WidthValue { get; private set; }
		public int HeightValue { get; private set; }

		public SizeConfigControl()
		{
			InitializeComponent();
		}

		private void widthControl_ValueChanged(object? sender, EventArgs e)
		{
			WidthValue = (int)widthControl.Value;
			SendValueChanged();
		}

		private void heightControl_ValueChanged(object? sender, EventArgs e)
		{
			HeightValue = (int)heightControl.Value;
			SendValueChanged();
		}

		internal override void UpdateValue(object config)
		{
			var val = Field?.GetValue<KSize>(config);
			if (val == null)
			{
				throw new NullReferenceException("Can't convert field to KSize");
			}

			widthControl.ValueChanged -= widthControl_ValueChanged;
			widthControl.Value = val.Value.Width;
			widthControl.ValueChanged += widthControl_ValueChanged;
			heightControl.ValueChanged -= heightControl_ValueChanged;
			heightControl.Value = val.Value.Height;
			heightControl.ValueChanged += heightControl_ValueChanged;
		}

		internal override void SetValue(object config)
		{
			Field?.SetValue(config, new KSize(WidthValue, HeightValue));
		}
	}
}
