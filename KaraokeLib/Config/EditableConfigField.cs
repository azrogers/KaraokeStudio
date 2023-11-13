using KaraokeLib.Config.Attributes;
using KaraokeLib.Util;
using System;
using System.Reflection;

namespace KaraokeLib.Config
{
	public interface IConfigField
	{
		/// <summary>
		/// The name of the config field.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// The type of control to use for this field.
		/// </summary>
		ConfigControlType ControlType { get; }

		/// <summary>
		/// The ConfigRangeAttribute specified on this field, if present.
		/// </summary>
		ConfigRangeAttribute? ConfigRange { get; }

		/// <summary>
		/// Is this field a decimal type?
		/// </summary>
		bool IsDecimal { get; }

		/// <summary>
		/// The underlying type of this field.
		/// </summary>
		Type FieldType { get; }

		/// <summary>
		/// Gets the current value of this field on the given instance.
		/// </summary>
		T? GetValue<T>(object instance);

		/// <summary>
		/// Gets the current value of this field on the given instance and casts it to the given type.
		/// Non-generic version of <see cref="GetValue{T}(object)"/>.
		/// </summary>
		object? GetValue(Type t, object instance);

		/// <summary>
		/// Sets the value of this field on the given instance to the given value.
		/// </summary>
		void SetValue<T>(object instance, T val);

	}

	public class EditableConfigField : IConfigField
	{
		/// <summary>
		/// The name of the field on the config object.
		/// </summary>
		public string Name { get; private set; }

		/// <inheritdoc />
		public ConfigControlType ControlType { get; private set; }

		/// <summary>
		/// The ConfigRangeAttribute specified on this field, if present.
		/// </summary>
		public ConfigRangeAttribute? ConfigRange => _range;

		/// <inheritdoc />
		public bool IsDecimal
		{
			get
			{
				if (_range != null)
				{
					return _range.IsDecimal;
				}

				var c = Type.GetTypeCode(_fieldType);
				return c == TypeCode.Decimal || c == TypeCode.Single || c == TypeCode.Double;
			}
		}

		/// <inheritdoc />
		public Type FieldType => _fieldType;

		private FieldInfo _field;
		private ConfigRangeAttribute? _range;
		private Type _fieldType;

		/// <summary>
		/// Makes a new EditableConfigField from the given FieldInfo on an EditableConfig object.
		/// </summary>
		public EditableConfigField(FieldInfo field)
		{
			_field = field;
			_fieldType = field.FieldType;
			_range = field.GetCustomAttribute<ConfigRangeAttribute>();

			Name = field.Name;
			ControlType = GetControlType(_fieldType);
		}

		/// <inheritdoc/>
		public T? GetValue<T>(object instance)
		{
			var fieldValue = _field.GetValue(instance);
			return (T?)Convert.ChangeType(fieldValue, typeof(T));
		}

		/// <inheritdoc/>
		public object? GetValue(Type t, object instance)
		{
			return Convert.ChangeType(_field.GetValue(instance), t);
		}

		/// <inheritdoc/>
		public void SetValue<T>(object instance, T val)
		{
			_field.SetValue(instance, Convert.ChangeType(val, _field.FieldType));
		}

		private ConfigControlType GetControlType(Type fieldType)
		{
			if (fieldType.IsEnum)
			{
				return ConfigControlType.Enum;
			}

			if (IsNumericType(fieldType))
			{
				return _range?.HasMax ?? false ? ConfigControlType.Range : ConfigControlType.Numeric;
			}

			if (fieldType == typeof(KSize))
			{
				return ConfigControlType.Size;
			}

			if (fieldType == typeof(KColor))
			{
				return ConfigControlType.Color;
			}

			if (fieldType == typeof(KFont))
			{
				return ConfigControlType.Font;
			}

			if (fieldType == typeof(bool))
			{
				return ConfigControlType.Bool;
			}

			if (fieldType == typeof(KPadding))
			{
				return ConfigControlType.Padding;
			}

			if (fieldType == typeof(string))
			{
				return ConfigControlType.String;
			}

			throw new NotImplementedException($"No config control for {fieldType}");
		}

		private static bool IsNumericType(Type t)
		{
			switch (Type.GetTypeCode(t))
			{
				case TypeCode.Byte:
				case TypeCode.SByte:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.Decimal:
				case TypeCode.Double:
				case TypeCode.Single:
					return true;
				default:
					return false;
			}
		}
	}

	public enum ConfigControlType
	{
		Numeric,
		Range,
		Size,
		Color,
		Font,
		Enum,
		Bool,
		Padding,
		String
	}
}
