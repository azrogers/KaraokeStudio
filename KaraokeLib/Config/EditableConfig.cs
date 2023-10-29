using Newtonsoft.Json;
using System.Reflection;

namespace KaraokeLib.Config
{
	public interface IEditableConfig
	{
		/// <summary>
		/// Information about all of the editable fields on this config object.
		/// </summary>
		public IEnumerable<EditableConfigField> Fields { get; }
	}

	/// <summary>
	/// A configuration class that KaraokeStudio can display using the ConfigEditor control.
	/// </summary>
	public class EditableConfig<T> : IEditableConfig where T : EditableConfig<T>
	{
		private Dictionary<string, EditableConfigField> _fields;

		/// <inheritdoc />
		[JsonIgnore]
		public IEnumerable<EditableConfigField> Fields => _fields.Values;

		/// <summary>
		/// Creates a new default config.
		/// </summary>
		public EditableConfig()
		{
			_fields = new Dictionary<string, EditableConfigField>();
			foreach (var field in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance))
			{
				_fields.Add(field.Name, new EditableConfigField(field));
			}
		}

		/// <summary>
		/// Creates an EditableConfig from existing config JSON.
		/// </summary>
		public EditableConfig(string configString) : this()
		{
			JsonConvert.PopulateObject(configString, this);
		}

		/// <summary>
		/// Serializes this object to JSON and writes it to the given file.
		/// </summary>
		public void Save(string filename)
		{
			File.WriteAllText(filename, JsonConvert.SerializeObject(this));
		}

		/// <summary>
		/// Returns a deep copy of this config.
		/// </summary>
		public T Copy()
		{
			var result = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(this));
			if (result == null)
			{
				throw new JsonSerializationException("Can't copy config!");
			}

			return result;
		}
	}

	public class ConfigRangeAttribute : Attribute
	{
		private bool _isDecimal = false;
		private bool _hasMin = true;
		private bool _hasMax = true;
		private double _min;
		private double _max;

		public bool HasMax => _hasMax;

		public bool IsDecimal => _isDecimal;

		public double Minimum => _min;

		public double Maximum => _max;

		public ConfigRangeAttribute(double min, double max)
		{
			_min = min;
			_max = max;
			_isDecimal = true;
		}

		public ConfigRangeAttribute(int min, int max)
		{
			_isDecimal = false;
			_min = min;
			_max = max;
		}

		public ConfigRangeAttribute(double min)
		{
			_min = min;
			_max = -1;
			_hasMax = false;
			_isDecimal = true;
		}

		public ConfigRangeAttribute(int min)
		{
			_isDecimal = false;
			_min = min;
			_max = -1;
			_hasMax = false;
		}
	}
}
