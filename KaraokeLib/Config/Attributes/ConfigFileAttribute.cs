namespace KaraokeLib.Config.Attributes
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class ConfigFileAttribute : Attribute
	{
		public string[] AllowedExtensions = [];
	}
}
