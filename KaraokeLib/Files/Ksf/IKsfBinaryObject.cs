using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeLib.Files.Ksf
{
	/// <summary>
	/// Represents an object that will be read and written to a KSF file in binary format.
	/// Every object that implements IKsfBinaryObject should also provide a public static Read(BinaryReader) method that returns the object.
	/// </summary>
	internal interface IKsfBinaryObject
	{
		void Write(BinaryWriter writer);
		object Read(BinaryReader reader);
	}
}
