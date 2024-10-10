using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections;
using Newtonsoft.Json.Linq;

namespace KaraokeLib.Files.Ksf
{
	internal class KsfSerializationInfo
	{
		private Dictionary<string, KsfType> _types = new Dictionary<string, KsfType>();

		internal KsfType? GetTypeInfo(string key) => _types.ContainsKey(key) ? _types[key] : null;

		internal KsfSerializationInfo(Type serializationType)
		{
			var attr = serializationType.GetCustomAttribute<KsfSerializableAttribute>();
			if(attr == null)
			{
				throw new ArgumentException("Type passed to KsfSerializationInfo must have KsfSerializable attribute");
			}

			BuildTypeInformation(serializationType, attr);
		}

		private void BuildTypeInformation(Type type, KsfSerializableAttribute serializableAttribute)
		{
			var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
			var fields = type.GetFields(bindingFlags);
			var properties = type.GetProperties(bindingFlags);
			var hasBinary = typeof(IKsfBinaryObject).IsAssignableFrom(type);
			var ksfType = new KsfType(type.Name, serializableAttribute.ObjectType, hasBinary);

			foreach(var f in fields)
			{
				var serializeAttribute = f.GetCustomAttribute<KsfSerializeAttribute>();
				if(serializeAttribute == null)
				{
					// ignore non-serialized field
					continue;
				}

				MaybeBuildTypeInformation(f.FieldType);
				ksfType.Values.Add(new KsfFieldInfo(f.Name, f));
			}

			foreach(var p in properties)
			{
				var serializeAttribute = p.GetCustomAttribute<KsfSerializeAttribute>();
				if (serializeAttribute == null)
				{
					// ignore non-serialized field
					continue;
				}

				MaybeBuildTypeInformation(p.PropertyType);
				ksfType.Values.Add(new KsfPropertyInfo(p.Name, p));
			}

			_types[type.Name] = ksfType;
		}

		private void MaybeBuildTypeInformation(Type t)
		{
			var enumerable = t.GetInterface("IEnumerable`1");
			if (enumerable != null)
			{
				t = enumerable.GetGenericArguments()[0];
			}

			// if this contains a serializable type, get its type info too
			var serializableAttr = t.GetCustomAttribute<KsfSerializableAttribute>();
			if (serializableAttr != null)
			{
				BuildTypeInformation(t, serializableAttr);
			}
		}

		internal class KsfType
		{
			public string Name { get; }
			public bool HasBinary { get; }
			public KsfObjectType ObjectType { get; }

			public List<IKsfValue> Values = new List<IKsfValue>();

			public KsfType(string name, KsfObjectType type, bool hasBinary)
			{
				Name = name;
				ObjectType = type;
				HasBinary = hasBinary;
			}
		}

		internal interface IKsfValue
		{
			public string Key { get; }

			public Type ValueType { get; }

			public object? GetValue(object obj);

			public void SetValue(object obj, object? value);
		}

		internal class KsfPropertyInfo : IKsfValue
		{
			public string Key { get; }
			public Type ValueType => _property.PropertyType;

			private PropertyInfo _property;

			public KsfPropertyInfo(string key, PropertyInfo property)
			{
				Key = key;
				_property = property;
			}

			public object? GetValue(object obj) => _property.GetValue(obj);

			public void SetValue(object obj, object? value) => _property.SetValue(obj, value);
		}

		internal class KsfFieldInfo : IKsfValue
		{
			public string Key { get; }
			public Type ValueType => _field.FieldType;

			private FieldInfo _field;

			public KsfFieldInfo(string key, FieldInfo field)
			{
				Key = key;
				_field = field;
			}

			public object? GetValue(object obj) => _field.GetValue(obj);

			public void SetValue(object obj, object? value) => _field.SetValue(obj, value);
		}
	}
}
