using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeLib.Config.Attributes
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	internal class ConfigDisplayAttribute : Attribute
	{
		public string? FriendlyName;
	}
}
