using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeStudio.Config
{
	public class BaseConfigControl : UserControl
	{
		internal ConfigFields.Field? Field;
		public event Action<Control>? OnValueChanged;

		protected void SendValueChanged()
		{
			OnValueChanged?.Invoke(this);
		}

		/// <summary>
		/// Update the value of this control based on the current value in the given object.
		/// </summary>
		internal virtual void UpdateValue(object config) { }
		/// <summary>
		/// Set the value of the given object field to the value of this control.
		/// </summary>
		internal virtual void SetValue(object config) { }
	}
}
