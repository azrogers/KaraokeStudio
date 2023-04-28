using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeLib
{
	public static class Extensions
	{
		/// <summary>
		/// Reads a null-terminated string.
		/// </summary>
		public static string ReadNullTerminatedString(this BinaryReader reader)
		{
			var bytes = new List<byte>();
			byte b;
			while((b = reader.ReadByte()) != 0)
			{
				bytes.Add(b);
			}

			return Encoding.UTF8.GetString(bytes.ToArray());
		}

		/// <summary>
		/// Writes a null-terminated string.
		/// </summary>
		public static void WriteNullTerminatedString(this BinaryWriter writer, string str)
		{
			var bytes = Encoding.UTF8.GetBytes(str);
			writer.Write(bytes);
			writer.Write((byte)'\0');
		}
	}
}
