using System.Reflection;

namespace KaraokeLib.Config.Attributes
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class ConfigDropdownAttribute : Attribute
	{
		private string _methodName;

		/// <summary>
		/// Specifies that this configuration field should be a dropdown with values populated by the static method <paramref name="methodName"/>.
		/// </summary>
		/// <param name="methodName">The name of a method with the signature <c>static IEnumerable&lt;(string, object)&gt; DropdownValues();</c>.</param>
		public ConfigDropdownAttribute(string methodName)
		{
			_methodName = methodName;
		}

		public IEnumerable<(string, object)> GetNameValuePairs(Type configType)
		{
			var methodInfo = configType.GetMethod(_methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			return methodInfo?.Invoke(null, null) as IEnumerable<(string, object)> ?? Enumerable.Empty<(string, object)>();
		}
	}
}
