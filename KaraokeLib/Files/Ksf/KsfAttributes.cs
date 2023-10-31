namespace KaraokeLib.Files.Ksf
{
	[AttributeUsage(AttributeTargets.Class)]
	internal class KsfSerializableAttribute : Attribute
	{
		public KsfObjectType ObjectType { get; private set; }

		public KsfSerializableAttribute(KsfObjectType type) { ObjectType = type; }
	}

	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	internal class KsfSerializeAttribute : Attribute
	{
	}
}
