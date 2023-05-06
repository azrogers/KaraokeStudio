using KaraokeLib.Util;
using Newtonsoft.Json.Linq;
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
	public partial class SizeConfigControl : BaseConfigControl
	{
		public int WidthValue { get; private set; }
		public int HeightValue { get; private set; }

		public SizeConfigControl()
		{
			InitializeComponent();
		}

		private void widthControl_ValueChanged(object sender, EventArgs e)
		{
			WidthValue = (int)widthControl.Value;
			SendValueChanged();
		}

		private void heightControl_ValueChanged(object sender, EventArgs e)
		{
			HeightValue = (int)heightControl.Value;
			SendValueChanged();
		}

		internal override void UpdateValue(object config)
		{
			var val = Field?.GetValue<KSize>(config);
			if(val == null)
			{
				throw new NullReferenceException("Can't convert field to KSize");
			}

			widthControl.Value = val.Value.Width;
			heightControl.Value = val.Value.Height;
		}

		internal override void SetValue(object config)
		{
			Field?.SetValue(config, new KSize(WidthValue, HeightValue));
		}
	}
}
