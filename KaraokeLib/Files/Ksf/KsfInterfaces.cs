﻿namespace KaraokeLib.Files.Ksf
{
	/// <summary>
	/// Represents an object that will be read and written to a KSF file in binary format.
	/// </summary>
	internal interface IKsfBinaryObject
	{
		void Write(BinaryWriter writer);
		object Read(BinaryReader reader, byte version);
	}

	/// <summary>
	/// Represents an object that should be serialized as a collection of type <typeparamref name="T"/>.
	/// </summary>
	internal interface IKsfIntoCollection<T>
	{
		void AddToCollection(T[] newValues);
		T[] GetFromCollection();
	}
}
