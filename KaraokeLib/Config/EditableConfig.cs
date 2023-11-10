using KaraokeLib.Config.Attributes;
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

		/// <summary>
		/// Returns a copy of this config.
		/// </summary>
		public IEditableConfig Copy();
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
				var configHide = field.GetCustomAttribute<ConfigHideAttribute>();
				if(configHide != null)
				{
					continue;
				}
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
		public T CopyTyped()
		{
			var result = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(this));
			if (result == null)
			{
				throw new JsonSerializationException("Can't copy config!");
			}

			return result;
		}

		/// <inheritdoc />
		public IEditableConfig Copy() => CopyTyped();
	}
}
