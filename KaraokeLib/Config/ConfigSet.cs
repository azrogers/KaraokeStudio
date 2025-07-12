using KaraokeLib.Config;
using KaraokeLib.Config.Attributes;

namespace KaraokeLib.Config
{
	public class ConfigSet
	{
		private List<IConfigField> _fields = new List<IConfigField>();

		public void AddConfig(IEditableConfig config)
		{
			_fields.AddRange(config.Fields);
		}

		public void AddField(IConfigField field)
		{
			_fields.Add(field);
		}

		public void AddCallback<T>(
			string name,
			ConfigControlType controlType,
			Func<object, T> getValue,
			Action<object, T> setValue,
			bool isDecimal = false,
			ConfigRangeAttribute? configRange = null,
			ConfigDropdownAttribute? configDropdown = null)
		{
			_fields.Add(new CallbackConfigField<T>(name, controlType, getValue, setValue, isDecimal, configRange, configDropdown));
		}

		internal class CallbackConfigField<T> : IConfigField
		{
			public string Name { get; private set; }

			public string? FriendlyName { get; private set; }

			public ConfigControlType ControlType { get; private set; }

			public ConfigRangeAttribute? ConfigRange { get; private set; }
			public ConfigDropdownAttribute? ConfigDropdown { get; private set; }
			public ConfigFileAttribute? ConfigFile { get; private set; }

			public bool IsDecimal { get; private set; }

			public Type FieldType { get; private set; }

			private Func<object, T> _getValueCallback;
			private Action<object, T> _setValueCallback;

			public CallbackConfigField(
				string name,
				ConfigControlType controlType,
				Func<object, T> getValueCallback,
				Action<object, T> setValueCallback,
				bool isDecimal = false,
				ConfigRangeAttribute? configRange = null,
				ConfigDropdownAttribute? configDropdown = null)
			{
				Name = name;
				ControlType = controlType;
				FieldType = typeof(T);
				IsDecimal = isDecimal;
				ConfigRange = configRange;
				ConfigDropdown = ConfigDropdown;
				_getValueCallback = getValueCallback;
				_setValueCallback = setValueCallback;
			}

			public U? GetValue<U>(object instance)
			{
				return (U?)Convert.ChangeType(_getValueCallback(instance), typeof(U));
			}

			public object? GetValue(Type t, object instance)
			{
				return Convert.ChangeType(_getValueCallback(instance), t);
			}

			public void SetValue<U>(object instance, U val)
			{
				_setValueCallback(instance, (T)Convert.ChangeType(val, typeof(T)));
			}
		}
	}
}
