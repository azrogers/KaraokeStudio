namespace KaraokeLib.Config.Attributes
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	internal class ConfigDisplayAttribute : Attribute
	{
		public string? FriendlyName;
	}
}
