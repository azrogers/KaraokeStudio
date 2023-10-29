namespace KaraokeLib.Audio
{
	internal static class FileTypeDetector
	{
		private static readonly byte[] ID3_HEADER = new byte[] { 0x49, 0x44, 0x33 };
		private static readonly byte[] FLAC_HEADER = new byte[] { 0x66, 0x4c, 0x61, 0x43 };
		private static readonly byte[] OGG_HEADER = new byte[] { 0x4f, 0x67, 0x67, 0x53 };
		private static readonly byte[] RIFF_HEADER = new byte[] { 0x52, 0x49, 0x46, 0x46 };
		private static readonly byte[] AIFF_HEADER = new byte[] { 0x46, 0x4f, 0x52, 0x4d };
		private static readonly byte[] WMA_HEADER = new byte[] {
		0x30, 0x26, 0xb2, 0x75, 0x8e, 0x66, 0xcf, 0x11,
		0xa6, 0xd9, 0x00, 0xaa, 0x00, 0x62, 0xce, 0x6c
	};

		private static readonly byte[] FTYP_BLOCK = new byte[] { 0x66, 0x74, 0x79, 0x70 };
		private static readonly byte[] M4A_HEADER = new byte[] { 0x4D, 0x34, 0x41, 0x20 };

		public static FileType Detect(string path)
		{
			using (var stream = File.OpenRead(path))
			using (var reader = new BinaryReader(stream))
			{
				var id3Bytes = reader.ReadBytes(3);
				var hasId3Header = CompareBytes(id3Bytes, ID3_HEADER);

				if (hasId3Header)
				{
					// skip id3 header
					reader.BaseStream.Position = 6;
					var sizeData = reader.ReadBytes(4);

					var size = (sizeData[0] << (7 * 3)) | (sizeData[1] << (7 * 2)) | (sizeData[2] << 7) | sizeData[3];
					reader.BaseStream.Position += size;
				}
				else
				{
					reader.BaseStream.Position = 0;
				}

				var startPos = reader.BaseStream.Position;

				if ((reader.BaseStream.Length - reader.BaseStream.Position) < 4)
				{
					return FileType.Unknown;
				}

				var firstFourBytes = reader.ReadBytes(4);
				if (CompareBytes(firstFourBytes, FLAC_HEADER))
				{
					return FileType.Flac;
				}

				if (CompareBytes(firstFourBytes, OGG_HEADER))
				{
					return FileType.Ogg;
				}

				if (CompareBytes(firstFourBytes, RIFF_HEADER))
				{
					return FileType.Wav;
				}

				if (CompareBytes(firstFourBytes, AIFF_HEADER))
				{
					return FileType.Aiff;
				}

				if (
					// first 11 bits = frame sync bits
					firstFourBytes[0] == 0xFF && (firstFourBytes[1] & 0xE0) == 0xE0 &&
					// 11 = mpeg version 1
					(firstFourBytes[1] & 0x18) == 0x18 &&
					// 01 == layer 3 (mp3)
					(firstFourBytes[1] & 0x06) == 0x02)
				{
					return FileType.Mp3;
				}

				if (
					// first 12 bits = frame sync bits
					firstFourBytes[0] == 0xFF && (firstFourBytes[1] & 0xF0) == 0xF0 &&
					// version, 0 for mpeg4
					(firstFourBytes[1] & 0x8) == 0x0 &&
					// layer, always set to 0
					(firstFourBytes[1] & 0x6) == 0x0)
				{
					return FileType.Aac;
				}

				if ((reader.BaseStream.Length - reader.BaseStream.Position) < 8)
				{
					return FileType.Unknown;
				}

				if (CompareBytes(reader.ReadBytes(4), FTYP_BLOCK) &&
					CompareBytes(reader.ReadBytes(4), M4A_HEADER))
				{
					return FileType.M4a;
				}

				reader.BaseStream.Position = startPos;

				if ((reader.BaseStream.Length - reader.BaseStream.Position) < 16)
				{
					return FileType.Unknown;
				}

				if (CompareBytes(reader.ReadBytes(16), WMA_HEADER))
				{
					return FileType.Wma;
				}

				return FileType.Unknown;
			}
		}

		private static bool CompareBytes(byte[] arr1, byte[] arr2)
		{
			if (arr1.Length != arr2.Length)
			{
				return false;
			}

			for (var i = 0; i < arr1.Length; i++)
			{
				if (arr1[i] != arr2[i])
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// The file type detected by the FileTypeDetector.
		/// The lowercase name of each field in this enum will be used directly as the extension
		/// (with the exception of Unknown, of course)
		/// </summary>
		public enum FileType
		{
			Unknown,
			Mp3,
			Flac,
			Wav,
			Aiff,
			Aac,
			M4a,
			Ogg,
			Wma
		}
	}
}
