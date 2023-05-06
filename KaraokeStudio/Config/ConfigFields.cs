using KaraokeLib;
using KaraokeLib.Util;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeStudio.Config
{
	internal class ConfigFields
	{
		private Dictionary<string, Field> _fields;

		public IEnumerable<Field> Fields => _fields.Values;

		public ConfigFields(Type configType)
		{
			_fields = new Dictionary<string, Field>();
			foreach(var field in configType.GetFields(BindingFlags.Public | BindingFlags.Instance))
			{
				_fields.Add(field.Name, new Field(field));
			}
		}

		internal class Field
		{
			public string Name { get; private set; }

			public ControlType ControlType { get; private set; }

			public ConfigRangeAttribute? ConfigRange => _range;

			public bool IsDecimal
			{
				get
				{
					if(_range != null)
					{
						return _range.IsDecimal;
					}

					var c = Type.GetTypeCode(_fieldType);
					return c == TypeCode.Decimal || c == TypeCode.Single || c == TypeCode.Double;
				}
			}

			private FieldInfo _field;
			private ConfigRangeAttribute? _range;
			private Type _fieldType;

			public Field(FieldInfo field)
			{
				_field = field;
				_fieldType = field.FieldType;
				_range = field.GetCustomAttribute<ConfigRangeAttribute>();

				Name = field.Name;
				ControlType = GetControlType(_fieldType);
			}

			public T? GetValue<T>(object o)
			{
				var fieldValue = _field.GetValue(o);
				return (T?)Convert.ChangeType(fieldValue, typeof(T));
			}

			public void SetValue<T>(object o, T val)
			{
				_field.SetValue(o, Convert.ChangeType(val, _field.FieldType));
			}

			private ControlType GetControlType(Type fieldType)
			{
				if(Util.IsNumericType(fieldType))
				{
					return (_range?.HasMax ?? false) ? ControlType.Range : ControlType.Numeric;
				}

				if(fieldType == typeof(KSize))
				{
					return ControlType.Size;
				}

				if(fieldType == typeof(KColor))
				{
					return ControlType.Color;
				}

				if(fieldType == typeof(KFont))
				{
					return ControlType.Font;
				}

				throw new NotImplementedException($"No config control for {fieldType}");
			}
		}

		internal enum ControlType
		{
			Numeric,
			Range,
			Size,
			Color,
			Font
		}
	}
}
