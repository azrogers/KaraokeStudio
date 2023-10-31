using KaraokeLib.Events;
using KaraokeLib.Tracks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeLib.Files.Ksf
{
    internal enum KsfObjectType : byte
	{
		None = 0,
		File = 1,
		Track = 2,
		Event = 3
	}

	internal static class KsfTypeMapping
	{
		private static Dictionary<KsfObjectType, Type> _mapping = new Dictionary<KsfObjectType, Type>()
		{
			{ KsfObjectType.File, typeof(KsfFileObject) },
			{ KsfObjectType.Track, typeof(KaraokeTrack) },
			{ KsfObjectType.Event, typeof(KaraokeEvent) }
		};

		public static Type? GetObjectClass(KsfObjectType type) => _mapping.ContainsKey(type) ? _mapping[type] : null;
	}
}
