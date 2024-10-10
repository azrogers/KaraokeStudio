using Newtonsoft.Json;
using System.Collections;

namespace KaraokeLib.Files.Ksf
{
	/// <summary>
	/// Handles writing a Karaoke Studio Format file to a stream.
	/// </summary>
	internal class KsfSerializer
	{
		// = "KSPF"
		private const uint MAGIC_NUMBER = 0x4650534B;
		private const byte VERSION = 0;

		private Stream _stream;
		private KsfSerializationInfo _serializationInfo;

		public KsfSerializer(Stream stream)
		{
			_stream = stream;
			_serializationInfo = new KsfSerializationInfo(typeof(KsfFileObject));
		}

		~KsfSerializer()
		{
			_stream.Dispose();
		}

		public void Write(KsfFileObject file)
		{
			using (var writer = new BinaryWriter(_stream))
			{
				writer.Write(MAGIC_NUMBER);
				writer.Write(VERSION);
				var info = _serializationInfo.GetTypeInfo(nameof(KsfFileObject));
				if (info == null)
				{
					throw new Exception($"Missing type info for KsfFileObject?");
				}

				Write(writer, file, info);
			}
		}

		private void Write(BinaryWriter writer, object obj, KsfSerializationInfo.KsfType type)
		{
			byte infoByte = 0;
			infoByte |= (byte)(type.HasBinary ? 1 : 0);
			infoByte |= (byte)((type.Values.Any() ? 1 : 0) << 1);

			writer.Write(infoByte);

			if (type.HasBinary)
			{
				((IKsfBinaryObject)obj).Write(writer);
			}

			if (type.Values.Any())
			{
				writer.Write(type.Values.Count);

				foreach (var v in type.Values)
				{
					writer.WriteNullTerminatedString(v.Key);

					var val = v.GetValue(obj) ?? new object();

					var valueType = v.ValueType;
					if (IsCollection(valueType))
					{
						// write a dummy count and come back later for the real thing
						var countOffset = writer.BaseStream.Position;
						writer.Write((uint)0);

						var count = 0u;
						valueType = valueType.GetGenericArguments()[0];
						if (val != null && (v.ValueType.IsArray || (val as IList) != null))
						{
							var iter = ((IEnumerable)val).GetEnumerator();
							using (iter as IDisposable)
							{
								while (iter.MoveNext())
								{
									count++;
									WriteValue(writer, valueType, iter.Current);
								}
							}
						}

						var oldOffset = writer.BaseStream.Position;
						writer.BaseStream.Position = countOffset;
						writer.Write(count);
						writer.BaseStream.Position = oldOffset;
					}
					else
					{
						WriteValue(writer, valueType, val);
					}
				}
			}
		}

		private void WriteValue(BinaryWriter writer, Type type, object value)
		{
			var typeInfo = _serializationInfo.GetTypeInfo(type.Name);
			if (typeInfo != null)
			{
				Write(writer, value, typeInfo);
			}
			else
			{
				var str = JsonConvert.SerializeObject(value);
				writer.WriteNullTerminatedString(str);
			}
		}

		public KsfFileObject Read()
		{
			using (var reader = new BinaryReader(_stream))
			{
				var magic = reader.ReadUInt32();
				if (magic != MAGIC_NUMBER)
				{
					throw new InvalidDataException($"Unknown magic number {magic:X}");
				}

				var version = reader.ReadByte();
				if (version > VERSION)
				{
					throw new InvalidDataException($"Can't read format version {version}");
				}

				var info = _serializationInfo.GetTypeInfo(nameof(KsfFileObject));
				if (info == null)
				{
					throw new Exception($"Missing type info for KsfFileObject?");
				}

				return (KsfFileObject)Read(reader, info);
			}
		}

		private object Read(BinaryReader reader, KsfSerializationInfo.KsfType type)
		{
			var infoByte = reader.ReadByte();
			var hasBinary = ((infoByte >> 0) & 1) == 1;
			var hasValues = ((infoByte >> 1) & 1) == 1;
			var newObjType = KsfTypeMapping.GetObjectClass(type.ObjectType);
			if (newObjType == null)
			{
				throw new ArgumentException($"Unknown type for KSF object: {type}");
			}

			var newObj = Activator.CreateInstance(newObjType, true);
			if (newObj == null)
			{
				throw new Exception($"Can't create new {newObjType.Name} - missing default constructor?");
			}

			if (hasBinary)
			{
				newObj = ((IKsfBinaryObject)newObj).Read(reader);
			}

			if (hasValues)
			{
				var count = reader.ReadInt32();
				for (var i = 0; i < count; i++)
				{
					var key = reader.ReadNullTerminatedString();
					var valueInfo = type.Values.Where(v => v.Key == key).FirstOrDefault();
					if (valueInfo == null)
					{
						throw new InvalidDataException($"Unknown key on {type.Name}: {key}");
					}

					var valueType = valueInfo.ValueType;
					if (IsCollection(valueType))
					{
						var arrayCount = reader.ReadUInt32();
						var arrayType = valueType.GetGenericArguments()[0];
						var values = new List<object>();
						for (var j = 0; j < arrayCount; j++)
						{
							values.Add(ReadValue(reader, arrayType));
						}

						// if it's an array, we can set directly more or less
						if (valueType.IsArray)
						{
							valueInfo.SetValue(newObj, values.ToArray());
						}
						else
						{
							// it's some sort of collection - let's see what we can do
							var newVal = Activator.CreateInstance(valueType);
							var listVal = newVal as IList;
							if (listVal != null)
							{
								foreach (var val in values)
								{
									listVal.Add(val);
								}
							}
							else
							{
								throw new NotImplementedException($"Don't know how to assign {arrayType.Name} to collection {valueType.Name}");
							}

							valueInfo.SetValue(newObj, newVal);
						}
					}
					else
					{
						valueInfo.SetValue(newObj, ReadValue(reader, valueType));
					}
				}
			}

			return newObj;
		}

		private object ReadValue(BinaryReader reader, Type type)
		{
			var typeInfo = _serializationInfo.GetTypeInfo(type.Name);
			if (typeInfo != null)
			{
				return Read(reader, typeInfo);
			}

			var str = reader.ReadNullTerminatedString();
			var obj = JsonConvert.DeserializeObject(str, type);
			if (obj == null)
			{
				throw new InvalidDataException($"Can't deserialize value of type {type.Name}");
			}

			return obj;
		}

		private static bool IsCollection(Type t) => t.IsArray || t.GetInterface(nameof(IList)) != null;
	}
}
